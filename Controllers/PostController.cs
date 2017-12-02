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

namespace Barker.Controllers
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

        //[Route("{userName?}")]
        public JsonResult GetBarks(string userName)
        {
            // if userName is not null, get the posts exclusively from that user.
            // if it is null, get posts for the feed of the currently logged in user
            if(userName != null)
            {
                var user = _context.Users.Where(x => x.UserName.ToLower() == userName.ToLower()).SingleOrDefault();
                if (user == null)
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return Json(new { Message = "Unable to find user " + userName});
                }
                return Json(new { 
                    Message = "Success!", 
                    Barks = _context.Posts.Where(x => x.User.UserName == userName).OrderByDescending(x => x.PostDate).ToList()
                });
            }
            else
            {
                // THIS IS A TEMPORARY SOLUTION
                return Json(new { Barks = _context.Posts.OrderByDescending(x => x.PostDate).Take(10).ToList()});
            }
        }

        [HttpPost]
        public async Task<JsonResult> SubmitPost(SubmitPostViewModel model, string returnUrl)
        {
            if(!User.Identity.IsAuthenticated) 
            {
                Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return Json(new { Message = "You must be signed in to do that"});
            }
            try
            {
                if(ModelState.IsValid)
                {
                    var author = _userManager.GetUserAsync(User);
                    Post post = new Post(){
                        Message = model.Message,
                        User = author.Result,
                        Author = author.Result.UserName,
                        PostDate = DateTime.Now
                    };
                    _context.Posts.Add(post);
                    await _context.SaveChangesAsync();
                    Response.StatusCode = (int)HttpStatusCode.Created;
                    return Json(new { Message = "Success!"});
                }
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { Message = "An error occurred when submitting your bark."});     
            }
            catch(Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { Message = e.Message});
            }      
        }

        //TODO: add edit and delete methods
    }
}
