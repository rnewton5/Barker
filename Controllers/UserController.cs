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
using Barker.Models.UserViewModels;
using Barker.Data;
using Barker.Models.PostViewModels;

namespace Barker.Controllers
{
    public class UserController : Controller
    {
        private readonly BarkerDbContext _context;
        private readonly UserManager<User> _userManager;

        public UserController(BarkerDbContext context, UserManager<User> userManager){
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Home()
        {
            if(!User.Identity.IsAuthenticated) 
            {
                return RedirectToAction("LoginOrRegister", "Account");
            }
            Random random = new Random();

            var user = _userManager.GetUserAsync(HttpContext.User);
            string userName = user.Result.UserName;
            int barksCount = _context.Posts.Where(p => p.Author == userName).Count();
            int followingCount = _context.Follows.Where(f => f.FollowerId == user.Result.Id).Count();
            int followersCount = _context.Follows.Where(f => f.FolloweeId == user.Result.Id).Count();
            int likesCount = _context.Likes.Where(l => l.UserId == user.Result.Id).Count();
            var userNames = _context.Users.Select(u => u.UserName).Where(x => x != _userManager.GetUserName(User)).OrderBy(x => random.Next()).Take(10).ToList();

            HomeViewModel model = new HomeViewModel() {
                UserName = userName,
                BarksCount = barksCount,
                FollowingCount = followingCount,
                FollowersCount = followersCount,
                PostVm = new PostViewModel(),
                OtherUsers = userNames
            };

            return View(model);
        }

        //User/Profile/<userName>
        public IActionResult Profile(string userName){
            if(!User.Identity.IsAuthenticated) 
            {
                return RedirectToAction("LoginOrRegister", "Account");
            }
            if(userName == null || userName == ""){
                userName = _userManager.GetUserName(User);
            }
            var user = _context.Users.Where(x => x.UserName.ToLower() == userName.ToLower()).SingleOrDefault();
            if (user == null) {
                TempData["error"] = "Unable to find user " + userName + ".";
                return RedirectToAction(nameof(Home));
            }
            Random random = new Random();

            string realUserName = user.UserName;
            int barksCount = _context.Posts.Where(p => p.Author == userName).Count();
            int followingCount = _context.Follows.Where(f => f.FollowerId == user.Id).Count();
            int followersCount = _context.Follows.Where(f => f.FolloweeId == user.Id).Count();
            int likesCount = _context.Likes.Where(l => l.UserId == user.Id).Count();
            bool isFollowing = _context.Follows.Any(f => f.FollowerId == _userManager.GetUserId(User) && f.FolloweeId == user.Id);
            var userNames = _context.Users.Select(u => u.UserName).Where(x => x != _userManager.GetUserName(User)).OrderBy(x => random.Next()).Take(10).ToList();

            ProfileViewModel model = new ProfileViewModel() {
                UserName = realUserName,
                BarksCount = barksCount,
                FollowingCount = followingCount,
                FollowersCount = followersCount,
                LikesCount = likesCount,
                PostVm = new PostViewModel(),
                JoinDate = user.JoinDate,
                IsFollowing = isFollowing,
                OtherUsers = userNames
            };

            return View(model);
        }

        public IActionResult Notifications()
        {
            return NoContent(); // NOT YET IMPLEMENTED
        }

        public IActionResult Messages()
        {
            return NoContent(); // NOT YET IMPLEMENTED
        }


        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
