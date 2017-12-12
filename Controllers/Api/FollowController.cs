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
    public class FollowController : Controller
    {
        private readonly BarkerDbContext _context;
        private readonly UserManager<User> _userManager;

        public FollowController(BarkerDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        
        public JsonResult ToggleFollow(string userName)
        {
            // Check if the user is signed in
            if (!User.Identity.IsAuthenticated)
            {
                Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return Json(new { Message = "You must be signed in to do that" });
            }

            // if userName is null or if the user does not exist return error message
            if (userName == null || !_context.Users.Any(u => u.UserName == userName))
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return Json(new { Message = "Unable to find user" });
            }

            // Getting all the Follows that the logged in user has.
            var userFollows = _context.Follows.Where(f => f.FollowerId == _userManager.GetUserId(User));
            var followee = _userManager.FindByNameAsync(userName).Result;

            try
            {
                // If the logged in user is already following the followee, unfollow. and vice versa
                if (userFollows.Any(f => f.FolloweeId == followee.Id))
                {
                    var followToRemove = _context.Follows.Where(f => f.FolloweeId == followee.Id);
                    _context.Follows.RemoveRange(followToRemove);
                    _context.SaveChanges();
                    Response.StatusCode = (int)HttpStatusCode.OK;
                    return Json(new { Message = "Unfollowed!" });
                }
                else
                {
                    Follow follow = new Follow()
                    {
                        Follower = _userManager.GetUserAsync(User).Result,
                        FolloweeId = followee.Id
                    };
                    _context.Follows.Add(follow);
                    _context.SaveChanges();
                    Response.StatusCode = (int)HttpStatusCode.Created;
                    return Json(new { Message = "Followed!" });
                }
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { Message = e.Message });
            }
        }
    }
}
