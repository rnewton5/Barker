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
                Name = user.Result.Name,
                UserName = user.Result.UserName,
                SubmitPostVm = new SubmitPostViewModel(),
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
                Name = user.Name,
                UserName = user.UserName,
                SubmitPostVm = new SubmitPostViewModel(),
                Barks = _context.Posts.Where(x => x.User == user)
                            .OrderByDescending(x => x.PostDate).Take(10).ToList()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitBark(SubmitPostViewModel model){
            if(!User.Identity.IsAuthenticated) 
            {
                TempData["ErrorMessage"] = "You must be logged in to do that.";
                return RedirectToAction("LoginOrRegister", "Account");
            }
            if(ModelState.IsValid){
                var author = _userManager.GetUserAsync(HttpContext.User);
                Post post = new Post(){
                    Message = model.Message,
                    User = author.Result,
                    Author = author.Result.UserName,
                    PostDate = DateTime.Now
                };
                _context.Posts.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Home));
            }
            TempData["ErrorMessage"] = "An error occurred when submitting your Bark."
                + "Please try again later.";
            return RedirectToAction(nameof(Home));            
        }

        public IActionResult Notifications()
        {
            return ReturnViewIfLoggedIn(); // NOT YET IMPLEMENTED
        }

        public IActionResult Messages()
        {
            return ReturnViewIfLoggedIn(); // NOT YET IMPLEMENTED
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

        private IActionResult ReturnViewIfLoggedIn()
        {
            if(!User.Identity.IsAuthenticated) 
            {
                return RedirectToAction("LoginOrRegister", "Account");
            }
            return View();
        }
    }
}
