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

        public UserController(BarkerDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Home()
        {
            if (!User.Identity.IsAuthenticated)
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
            var following = _context.Follows.Where(f => f.FollowerId == _userManager.GetUserId(User)).Select(f => f.FolloweeId).ToList();
            following = _context.Users.Where(u => following.Contains(u.Id)).Select(u => u.UserName).ToList();
            var userNames = _context.Users.Select(u => u.UserName).Where(x => x != _userManager.GetUserName(User)).OrderBy(x => random.Next()).Take(10).ToList();

            HomeViewModel model = new HomeViewModel()
            {
                UserName = userName,
                BarksCount = barksCount,
                FollowingCount = followingCount,
                FollowersCount = followersCount,
                PostVm = new PostViewModel(),
                Following = following,
                OtherUsers = userNames
            };

            return View(model);
        }

        //User/Profile/<userName>
        public IActionResult Profile(string userName)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("LoginOrRegister", "Account");
            }
            if (userName == null || userName == "")
            {
                userName = _userManager.GetUserName(User);
            }
            var user = _context.Users.Where(x => x.UserName.ToLower() == userName.ToLower()).SingleOrDefault();
            if (user == null)
            {
                TempData["error"] = "Unable to find user " + userName + ".";
                return RedirectToAction(nameof(Home));
            }
            Random random = new Random();


            string realUserName = user.UserName;
            int barksCount = _context.Posts.Where(p => p.Author == userName).Count();
            int followingCount = _context.Follows.Where(f => f.FollowerId == user.Id).Count();
            int followersCount = _context.Follows.Where(f => f.FolloweeId == user.Id).Count();
            int likesCount = _context.Likes.Where(l => l.UserId == user.Id).Count();
            var following = _context.Follows.Where(f => f.FollowerId == _userManager.GetUserId(User)).Select(f => f.FolloweeId).ToList();
            following = _context.Users.Where(u => following.Contains(u.Id)).Select(u => u.UserName).ToList();
            var userNames = _context.Users.Select(u => u.UserName).Where(x => x != _userManager.GetUserName(User)).OrderBy(x => random.Next()).Take(10).ToList();

            ProfileViewModel model = new ProfileViewModel()
            {
                UserName = realUserName,
                BarksCount = barksCount,
                FollowingCount = followingCount,
                FollowersCount = followersCount,
                LikesCount = likesCount,
                PostVm = new PostViewModel(),
                JoinDate = user.JoinDate,
                Following = following,
                OtherUsers = userNames
            };

            return View(model);
        }

        //User/Likes/<userName>
        public IActionResult Likes(string userName)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("LoginOrRegister", "Account");
            }
            if (userName == null || userName == "")
            {
                userName = _userManager.GetUserName(User);
            }
            var user = _context.Users.Where(x => x.UserName.ToLower() == userName.ToLower()).SingleOrDefault();
            if (user == null)
            {
                TempData["error"] = "Unable to find user " + userName + ".";
                return RedirectToAction(nameof(Home));
            }
            Random random = new Random();


            string realUserName = user.UserName;
            int barksCount = _context.Posts.Where(p => p.Author == userName).Count();
            int followingCount = _context.Follows.Where(f => f.FollowerId == user.Id).Count();
            int followersCount = _context.Follows.Where(f => f.FolloweeId == user.Id).Count();
            int likesCount = _context.Likes.Where(l => l.UserId == user.Id).Count();
            var following = _context.Follows.Where(f => f.FollowerId == _userManager.GetUserId(User)).Select(f => f.FolloweeId).ToList();
            following = _context.Users.Where(u => following.Contains(u.Id)).Select(u => u.UserName).ToList();
            var userNames = _context.Users.Select(u => u.UserName).Where(x => x != _userManager.GetUserName(User)).OrderBy(x => random.Next()).Take(10).ToList();

            ProfileViewModel model = new ProfileViewModel()
            {
                UserName = realUserName,
                BarksCount = barksCount,
                FollowingCount = followingCount,
                FollowersCount = followersCount,
                LikesCount = likesCount,
                PostVm = new PostViewModel(),
                JoinDate = user.JoinDate,
                Following = following,
                OtherUsers = userNames
            };

            return View(model);
        }

        public IActionResult Following(string userName)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("LoginOrRegister", "Account");
            }
            if (userName == null || userName == "")
            {
                userName = _userManager.GetUserName(User);
            }
            var user = _context.Users.Where(x => x.UserName.ToLower() == userName.ToLower()).SingleOrDefault();
            if (user == null)
            {
                TempData["error"] = "Unable to find user " + userName + ".";
                return RedirectToAction(nameof(Home));
            }
            Random random = new Random();

            string realUserName = user.UserName;
            string loggedInUserId = _userManager.GetUserId(User);

            int barksCount = _context.Posts.Where(p => p.Author == userName).Count();
            int followingCount = _context.Follows.Where(f => f.FollowerId == user.Id).Count();
            int followersCount = _context.Follows.Where(f => f.FolloweeId == user.Id).Count();
            int likesCount = _context.Likes.Where(l => l.UserId == user.Id).Count();
            var following = _context.Follows.Where(f => f.FollowerId == loggedInUserId).Select(f => f.FolloweeId).ToList();
            following = _context.Users.Where(u => following.Contains(u.Id)).Select(u => u.UserName).ToList();
            var userNames = _context.Users.Select(u => u.UserName).Where(x => x != _userManager.GetUserName(User)).OrderBy(x => random.Next()).Take(10).ToList();

            // Getting the list of users followed by the specified user
            string userId = _context.Users.Single(u => u.UserName == realUserName).Id;
            var followingUsersIds = _context.Follows.Where(f => f.FollowerId == userId).Select(f => f.FolloweeId).ToList();
            var followingUsers = _context.Users.Where(u => followingUsersIds.Contains(u.Id)).ToList();

            // Putting the users into the correct list format
            var formattedUserList = new List<FollowingUser>();
            foreach(var fUser in followingUsers) {
                formattedUserList.Add(new FollowingUser{
                    UserName = fUser.UserName,
                    BarksCount = _context.Posts.Count(p => p.User == fUser),
                    FollowersCount = _context.Follows.Count(f => f.FolloweeId == fUser.Id),
                    isFollowing = _context.Follows.Any(f => f.FolloweeId == fUser.Id && f.FollowerId == loggedInUserId)
                });
            }

            var model = new FollowingViewModel
            {
                UserName = realUserName,
                BarksCount = barksCount,
                FollowingCount = followingCount,
                FollowersCount = followersCount,
                LikesCount = likesCount,
                PostVm = new PostViewModel(),
                JoinDate = user.JoinDate,
                Following = following,
                OtherUsers = userNames,
                FollowingUsers = formattedUserList
            };

            return View(model);
        }

        public IActionResult Followers(string userName)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("LoginOrRegister", "Account");
            }
            if (userName == null || userName == "")
            {
                userName = _userManager.GetUserName(User);
            }
            var user = _context.Users.Where(x => x.UserName.ToLower() == userName.ToLower()).SingleOrDefault();
            if (user == null)
            {
                TempData["error"] = "Unable to find user " + userName + ".";
                return RedirectToAction(nameof(Home));
            }
            Random random = new Random();

            string realUserName = user.UserName;
            string loggedInUserId = _userManager.GetUserId(User);

            int barksCount = _context.Posts.Where(p => p.Author == userName).Count();
            int followingCount = _context.Follows.Where(f => f.FollowerId == user.Id).Count();
            int followersCount = _context.Follows.Where(f => f.FolloweeId == user.Id).Count();
            int likesCount = _context.Likes.Where(l => l.UserId == user.Id).Count();
            var following = _context.Follows.Where(f => f.FollowerId == loggedInUserId).Select(f => f.FolloweeId).ToList();
            following = _context.Users.Where(u => following.Contains(u.Id)).Select(u => u.UserName).ToList();
            var userNames = _context.Users.Select(u => u.UserName).Where(x => x != _userManager.GetUserName(User)).OrderBy(x => random.Next()).Take(10).ToList();

            // Getting the list of users who are following the specified user
            string userId = _context.Users.Single(u => u.UserName == realUserName).Id;
            var followerUsersIds = _context.Follows.Where(f => f.FolloweeId == userId).Select(f => f.FollowerId).ToList();
            var followerUsers = _context.Users.Where(u => followerUsersIds.Contains(u.Id)).ToList();

            // Putting the users into the correct list format
            var formattedUserList = new List<FollowerUser>();
            foreach(var fUser in followerUsers) {
                formattedUserList.Add(new FollowerUser{
                    UserName = fUser.UserName,
                    BarksCount = _context.Posts.Count(p => p.User == fUser),
                    FollowersCount = _context.Follows.Count(f => f.FolloweeId == fUser.Id),
                    isFollowing = _context.Follows.Any(f => f.FolloweeId == fUser.Id && f.FollowerId == loggedInUserId)
                });
            }

            var model = new FollowersViewModel
            {
                UserName = realUserName,
                BarksCount = barksCount,
                FollowingCount = followingCount,
                FollowersCount = followersCount,
                LikesCount = likesCount,
                PostVm = new PostViewModel(),
                JoinDate = user.JoinDate,
                Following = following,
                OtherUsers = userNames,
                FollowerUsers = formattedUserList
            };

            return View(model);
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

        public IActionResult Notifications()
        {
            return NoContent(); // NOT YET IMPLEMENTED
        }

        public IActionResult Messages()
        {
            return NoContent(); // NOT YET IMPLEMENTED
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
