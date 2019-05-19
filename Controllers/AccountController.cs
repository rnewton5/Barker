using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Barker.Models;
using Barker.Models.AccountViewModels;
using System.Web;
using Barker.Data;

namespace Barker.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger _logger;

        private readonly BarkerDbContext _context;

        public AccountController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger<AccountController> logger,
            BarkerDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _context = context;
        }

        [TempData]
        public string ErrorMessage { get; set; }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginOrRegister()
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {    
            if (ModelState.IsValid)
            {
                if(_context.Users.Any(x => x.Email == model.Email))
                {
                    string userName = _context.Users.Where(x => x.Email == model.Email).SingleOrDefault().UserName;
                    var result = await _signInManager.PasswordSignInAsync(userName, model.Password, model.RememberMe, lockoutOnFailure: false);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User logged in.");
                        return RedirectToAction(nameof(UserController.Home), "User");
                    }
                }
            }            
            // If we got this far, something failed, redisplay form
            TempData["info"] = "Invalid login attempt.";
            return RedirectToAction(nameof(LoginOrRegister));
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["info"] = "Invalid information provided";
                return RedirectToAction(nameof(LoginOrRegister));
            }
            if (_context.Users.Any(u => u.UserName == model.UserName))
            {
                TempData["info"] = "An account already exists with that user name";
                return RedirectToAction(nameof(LoginOrRegister));
            }
            if (_context.Users.Any(u => u.Email == model.Email))
            {
                TempData["info"] = "An account already exists with that email address";
                return RedirectToAction(nameof(LoginOrRegister));
            }
            
            var user = new User {  
                UserName = model.UserName, 
                Email = model.Email,
                JoinDate = DateTime.Now
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                _logger.LogInformation("User created a new account with password.");

                await _signInManager.SignInAsync(user, isPersistent: false);
                _logger.LogInformation("User created a new account with password.");
                return RedirectToAction(nameof(UserController.Home), "User");
            }

            AddErrors(result);
            TempData["info"] = "Invalid information provided";
            return RedirectToAction(nameof(LoginOrRegister));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction(nameof(LoginOrRegister));
        }


        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(UserController.Home), "Home");
            }
        }

        #endregion
    }
}
