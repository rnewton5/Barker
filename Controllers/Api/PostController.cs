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
        private const int NUM_BARKS = 10;  // the number of posts to return at one time

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

        // Returns only posts that have been liked by the user specified by 'userName'
        public async Task<JsonResult> GetLikedPosts(string userName, string lastId)
        {
            try
            {
                int latestId = int.Parse(lastId);
                latestId = latestId < 0 ? int.MaxValue : latestId;
                // If the username provided is null, we get the username for thecurrently logged in user
                if(userName == null){
                    userName = _userManager.GetUserName(User);
                }
                // getting the user that owns the username and verifying that they exist
                var user = await _context.Users.SingleAsync(u => u.UserName.ToLower() == userName.ToLower());
                if(user == null){
                    throw new ApplicationException($"Unable to find user");
                }
                // getting a list of all the postsIds that the user as liked
                var likedPostIds = _context.Likes.Where(l => l.UserId == user.Id).Select(l => l.PostId).ToList();

                var barks = _context.Posts
                                .AsNoTracking()
                                .Where(x => likedPostIds.Contains(x.Id) && x.Id < latestId)
                                .OrderByDescending(x => x.PostDate)
                                .Take(NUM_BARKS).ToList();
                var loggedInUserId = _userManager.GetUserId(User);
                var likes = _context.Likes.Where(l => l.UserId == loggedInUserId);
                var follows = _context.Follows.Where(f => f.FollowerId == loggedInUserId);

                List<object> result = new List<object>();
                foreach(var bark in barks)
                {
                    result.Add(new 
                    {
                        Bark = bark,
                        LikesPost = likes.Any(l => l.PostId == bark.Id),
                        Following = follows.Any(f => f.FolloweeId == bark.UserId),
                        Owner = bark.UserId == loggedInUserId
                    });
                }

                Response.StatusCode = (int)HttpStatusCode.OK;
                return Json(new {result});
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { Message = e.Message });
            }
        }

        // Returns a list of posts authored by the currently logged in user, and everyone that they follow
        public JsonResult GetPostFeed(string lastId)
        {
            try
            {
                int latestId = int.Parse(lastId);
                latestId = latestId < 0 ? int.MaxValue : latestId;
                // Getting the name of the currently logged in user
                var userName = _userManager.GetUserName(User);

                // getting the list of ids for the users that that are being followed
                List<string> userFollowsIds = _context.Follows.Where(x => x.FollowerId == _userManager.GetUserId(User)).Select(x => x.FolloweeId).ToList();
                var barks = _context.Posts
                                .AsNoTracking()
                                .Where(x => x.Id < latestId)
                                .Where(x => x.Author == userName || userFollowsIds.Contains(x.UserId))
                                .OrderByDescending(x => x.PostDate)
                                .Take(NUM_BARKS).ToList();
                var loggedInUserId = _userManager.GetUserId(User);
                var likes = _context.Likes.Where(l => l.UserId == loggedInUserId);
                var follows = _context.Follows.Where(f => f.FollowerId == loggedInUserId);

                List<object> result = new List<object>();
                foreach(var bark in barks){
                    result.Add(new 
                    {
                        Bark = bark,
                        LikesPost = likes.Any(l => l.PostId == bark.Id),
                        Following = follows.Any(f => f.FolloweeId == bark.UserId),
                        Owner = bark.UserId == loggedInUserId
                    });
                }

                Response.StatusCode = (int)HttpStatusCode.OK;
                return Json(new {result});
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { Message = e.Message });
            }
        }

        // Returns a list of posts authored user specified by 'userName'
        public JsonResult GetPostsByUser(string userName, string lastId)
        {
            try
            {
                int latestId = int.Parse(lastId);
                latestId = latestId < 0 ? int.MaxValue : latestId;
                // If the username provided is null, we get the username for thecurrently logged in user
                if(userName == null){
                    userName = _userManager.GetUserName(User);
                }
                // checking if a user exists with the userName provided
                if (!_context.Users.Any(x => x.UserName.ToLower() == userName.ToLower())){
                    throw new ApplicationException($"Unable to find user");
                }
                // if the user does exist, we get some of their posts
                var barks = _context.Posts
                                .AsNoTracking()
                                .Where(x => x.Author == userName && x.Id < latestId)
                                .OrderByDescending(x => x.PostDate)
                                .Take(NUM_BARKS).ToList();
                var loggedInUserId = _userManager.GetUserId(User);
                var likes = _context.Likes.Where(l => l.UserId == loggedInUserId);
                var follows = _context.Follows.Where(f => f.FollowerId == loggedInUserId);

                List<object> result = new List<object>();
                foreach(var bark in barks)
                {
                    result.Add(new 
                    {
                        Bark = bark,
                        LikesPost = likes.Any(l => l.PostId == bark.Id),
                        Following = follows.Any(f => f.FolloweeId == bark.UserId),
                        Owner = bark.UserId == loggedInUserId
                    });
                }

                Response.StatusCode = (int)HttpStatusCode.OK;
                return Json(new {result});
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
                return Json(new { Message = "Deleted!" });
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
            if (postId != null)
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
