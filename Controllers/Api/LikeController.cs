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

namespace Barker.Api.Controllers
{
    public class LikeController : Controller
    {
        private readonly BarkerDbContext _context;
        private readonly UserManager<User> _userManager;

        public LikeController(BarkerDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<JsonResult> ToggleLike(int? postId)
        {
            // Check if the user is signed in
            if (!User.Identity.IsAuthenticated)
            {
                Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return Json(new { Message = "You must be signed in to do that" });
            }

            // if postId is null or if the post does not exist return error message
            if (postId == null || !postExists(postId))
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return Json(new { Message = "Unable to find post" });
            }

            // if the currently logged in user is not the owner of the post, return an error message
            if (UserOwnsPost(postId))
            {
                Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return Json(new { Message = "You cannot like your own post" });
            }

            // Getting all the likes that the current post has.
            var postLikes = _context.Likes.Where(l => l.PostId == postId);

            try
            {
                // If the user Has already liked the post, we unlike it. and vice versa
                if (postLikes.Any(l => l.UserId == _userManager.GetUserId(User)))
                {
                    var likeToRemove = await _context.Likes.SingleAsync(l => l.UserId == _userManager.GetUserId(User));
                    _context.Likes.Remove(likeToRemove);
                    await _context.SaveChangesAsync();
                    Response.StatusCode = (int)HttpStatusCode.OK;
                    return Json(new { Message = "Unliked!" });
                }
                else
                {
                    Like like = new Like()
                    {
                        UserId = _userManager.GetUserId(User),
                        Post = await _context.Posts.SingleAsync(p => p.Id == postId)
                    };
                    _context.Likes.Add(like);
                    await _context.SaveChangesAsync();
                    Response.StatusCode = (int)HttpStatusCode.Created;
                    return Json(new { Message = "Liked!" });
                }
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
