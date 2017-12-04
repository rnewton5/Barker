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

            var user = _userManager.GetUserAsync(HttpContext.User);
            ProfileViewModel model = new ProfileViewModel() {
                UserName = user.Result.UserName,
                PostVm = new PostViewModel(),
                Barks = _context.Posts.OrderByDescending(x => x.PostDate).Take(10).ToList()
            };

            return View(model);
        }

        //User/Profile/<userName>
        public IActionResult Profile(string userName){
            if(!User.Identity.IsAuthenticated) 
            {
                return RedirectToAction("LoginOrRegister", "Account");
            }
            if(userName == null){
                return RedirectToAction(nameof(Home));
            }
            var user = _context.Users.Where(x => x.UserName.ToLower() == userName.ToLower()).SingleOrDefault();
            if (user == null) {
                TempData["error"] = "Unable to find user " + userName + ".";
                return RedirectToAction(nameof(Home));
            }
            ProfileViewModel model = new ProfileViewModel() {
                UserName = user.UserName,
                PostVm = new PostViewModel(),
                Barks = _context.Posts.Where(x => x.User == user)
                            .OrderByDescending(x => x.PostDate).Take(10).ToList()
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
