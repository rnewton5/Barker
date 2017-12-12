using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Barker.Models;
using Barker.Data;
using System.Net;
using Barker.Models.PostViewModels;
using Microsoft.EntityFrameworkCore;

namespace Barker.Controllers.Api
{
    public class PostController : Controller
    {
        private readonly BarkerDbContext _context;
        private readonly UserManager<User> _userManager;

        public PostController(BarkerDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        [HttpPost]
        public async Task<JsonResult> SubmitPost(PostViewModel model)
        {
            if (!User.Identity.IsAuthenticated)
            {
                Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return Json(new { Message = "You must be signed in to do that" });
            }
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _userManager.GetUserAsync(User);
                    Post post = new Post()
                    {
                        Message = model.Message,
                        User = user,
                        Author = user.UserName,
                        PostDate = DateTime.Now
                    };
                    _context.Posts.Add(post);
                    await _context.SaveChangesAsync();
                    Response.StatusCode = (int)HttpStatusCode.Created;
                    return Json(new { Message = "Success!" });
                }
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { Message = "An error occurred when submitting your bark." });
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { Message = e.Message });
            }
        }

        public async Task<JsonResult> GetPost(int? postId)
        {
            if (postId == null)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return Json(new { Message = "Unable to find post" });
            }

            try
            {
                var post = await _context.Posts.AsNoTracking().SingleOrDefaultAsync(x => x.Id == postId);
                if (post == null)
                {
                    Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return Json(new { Message = "Unable to find post" });
                }
                Response.StatusCode = (int)HttpStatusCode.OK;
                return Json(new { Bark = post });
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { Message = e.Message });
            }
        }

        //TODO: update this method to work more dynamically
        public async Task<JsonResult> GetPosts(string userName, string lastId)
        {
            try
            {
                int latestId = int.Parse(lastId);
                latestId = latestId < 0 ? int.MaxValue : latestId;
                // if userName is not null, get the posts exclusively from that user.
                // if it is null, get posts for the feed of the currently logged in user
                if (userName != null)
                {
                    var user = await _context.Users.AsNoTracking().SingleOrDefaultAsync(x => x.UserName.ToLower() == userName.ToLower());
                    if (user == null)
                    {
                        Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        return Json(new { Message = "Unable to find user " + userName });
                    }
                    Response.StatusCode = (int)HttpStatusCode.OK;
                    return Json(new
                    {
                        Barks = _context.Posts
                                    .AsNoTracking()
                                    .Where(x => x.Author == userName && x.Id < latestId)
                                    .OrderByDescending(x => x.PostDate)
                                    .Take(10)
                                    .ToArray()
                    });
                }
                else
                {
                    // THIS IS A TEMPORARY SOLUTION
                    // In the final product it will only get the barks by people you follow
                    Response.StatusCode = (int)HttpStatusCode.OK;
                    return Json(new { Barks = _context.Posts
                                    .AsNoTracking()
                                    .Where(x => x.Id < latestId)
                                    .OrderByDescending(x => x.PostDate)
                                    .Take(10)
                                    .ToArray() });
                }
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { Message = e.Message });
            }
        }

        [HttpPost]
        public async Task<JsonResult> EditPost(PostViewModel model, int? postId)
        {
            // if postId is null or if the post does not exist return error message
            if (postId == null || !postExists(postId))
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return Json(new { Message = "Unable to find post" });
            }

            // if the currently logged in user is not the owner of the post, return an error message
            if (!UserOwnsPost(postId))
            {
                Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return Json(new { Message = "You cannot alter a post that is not yours" });
            }

            // if the user owns the post, attempt to alter the body of it
            try
            {
                if (ModelState.IsValid)
                {
                    var postToEdit = await _context.Posts.SingleAsync(x => x.Id == postId);
                    postToEdit.Message = model.Message;
                    _context.Posts.Update(postToEdit);
                    await _context.SaveChangesAsync();
                    Response.StatusCode = (int)HttpStatusCode.OK;
                    return Json(new { Message = "Success!" });
                }
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { Message = "Unable to update post" });
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { Message = e.Message });
            }
        }

        public async Task<JsonResult> DeletePost(int? postId)
        {
            // if postId is null or if the post does not exist return error message
            if (postId == null || !postExists(postId))
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return Json(new { Message = "Unable to find post" });
            }

            // if the currently logged in user is not the owner of the post, return an error message
            if (!UserOwnsPost(postId))
            {
                Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return Json(new { Message = "You cannot alter a post that is not yours" });
            }

            // if the user owns the post, attempt to delete it from the database
            try
            {
                var postToRemove = await _context.Posts.SingleAsync(x => x.Id == postId);
                _context.Posts.Remove(postToRemove);
                await _context.SaveChangesAsync();
                Response.StatusCode = (int)HttpStatusCode.OK;
                return Json(new { Message = "Success!" });
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { Message = e.Message });
            }
        }

        // Returns true if the currently logged in user is the author of the post. false if they do not
        private bool UserOwnsPost(int? postId)
        {
            if (postId == null)
            {
                try
                {
                    var userId = _userManager.GetUserId(User);
                    var postOwnerId = _context.Posts.Single(x => x.Id == postId).UserId;
                    return (userId == postOwnerId);
                }
                catch (Exception) { /* do nothing */ }
            }
            return false;
        }

        // Returns true if a post with the corresponding Id exists in the database
        private bool postExists(int? postId)
        {
            return _context.Posts.Any(x => x.Id == postId);
        }
    }
}
