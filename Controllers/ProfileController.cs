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
using Barker.Models.ProfileViewModels;
using Barker.Data;

namespace Barker.Controllers
{
    public class ProfileController : Controller
    {
        private readonly BarkerDbContext _context;
        private readonly UserManager<BarkerUser> _userManager;

        public ProfileController(BarkerDbContext context, UserManager<BarkerUser> userManager){
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
            HomeViewModel homeVm = new HomeViewModel() {
                Name = user.Result.Name,
                UserName = user.Result.UserName,
                SubmitBarkVm = new SubmitBarkViewModel(),
                Barks = _context.Barks.OrderByDescending(x => x.PostDate).Take(10).ToList()
            };

            return View(homeVm);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitBark(SubmitBarkViewModel model){
            if(ModelState.IsValid){
                var author = _userManager.GetUserAsync(HttpContext.User);
                BarkerPost post = new BarkerPost(){
                    Message = model.Message,
                    User = author.Result,
                    Author = author.Result.UserName,
                    PostDate = DateTime.Now
                };
                _context.Barks.Add(post);
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
