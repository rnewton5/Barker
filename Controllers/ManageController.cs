using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Barker.Models;
using Barker.Models.ManageViewModels;
using Barker.Services;
using Microsoft.AspNetCore.Http;
using System.IO;
using Barker.Data;

namespace Barker.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class ManageController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;
        private readonly UrlEncoder _urlEncoder;
        private readonly BarkerDbContext _context;

        private const string AuthenicatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

        public ManageController(
          UserManager<User> userManager,
          SignInManager<User> signInManager,
          IEmailSender emailSender,
          ILogger<ManageController> logger,
          UrlEncoder urlEncoder,
          BarkerDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
            _urlEncoder = urlEncoder;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> ChangeUserName()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var model = new ChangeUserNameViewModel
            {
                CurrentUserName = user.UserName,
                NewUserName = ""
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeUserName(ChangeUserNameViewModel model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var result = await _userManager.SetUserNameAsync(user, model.NewUserName);
            if (!result.Succeeded)
            {
                AddErrors(result);
                return View(model);
            }
            
            await _signInManager.SignInAsync(user, isPersistent: false);
            TempData["Status"] = "Your Username has been changed.";
            return RedirectToAction(nameof(ChangeUserName));
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            return View(new ChangePasswordViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                AddErrors(changePasswordResult);
                return View(model);
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            _logger.LogInformation("User changed their password successfully.");
            TempData["Status"] = "Your password has been changed.";

            return RedirectToAction(nameof(ChangePassword));
        }

        [HttpGet]
        public IActionResult UploadProfileImage()
        {
            return View();
        }

        [HttpPost]
        public IActionResult UploadProfileImage(IList<IFormFile> files)
        {
            try
            {
                IFormFile uploadedImage = files.FirstOrDefault();
                if (uploadedImage == null || uploadedImage.ContentType.ToLower().StartsWith("image/"))
                {
                    MemoryStream ms = new MemoryStream();
                    uploadedImage.OpenReadStream().CopyTo(ms);

                    System.Drawing.Image image = System.Drawing.Image.FromStream(ms);
                    var imageId = Guid.NewGuid();
                    var user = _userManager.GetUserAsync(User).Result;
                    Models.Image imageEntity = new Models.Image()
                    {
                        Id = imageId,
                        Name = uploadedImage.Name,
                        Data = ms.ToArray(),
                        Width = image.Width,
                        Height = image.Height,
                        ContentType = uploadedImage.ContentType,
                        User = _userManager.GetUserAsync(User).Result
                    };

                    // TEMPORARY    
                    // for right now, the most recently uploaded image is the users profile image
                    user.ProfileImageId = imageId;

                    _context.Images.Add(imageEntity);
                    _context.Users.Update(user);
                    _context.SaveChanges();
                }

                TempData["Status"] = "Your profile picture has been changed.";
                return View();
            }
            catch (Exception e)
            {
                TempData["Status"] = e.Message;
                return View();
            }
        }

        
        [HttpGet]
        public IActionResult ChangeBio()
        {
            var model = new ChangeBioViewModel();
            model.Bio = _userManager.GetUserAsync(User).Result.Bio;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeBio(ChangeBioViewModel model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            
            try
            {
                user.Bio = model.Bio;
                _context.Users.Update(user);
                TempData["Status"] = "Your password has been changed.";
                return RedirectToAction(nameof(ChangePassword));
            }
            catch (Exception e) {
                TempData["Status"] = "An error occurred when updating your bio.";
                return View(model);
            }
        }

        /*
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendVerificationEmail(IndexViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
            var email = user.Email;
            await _emailSender.SendEmailConfirmationAsync(email, callbackUrl);

            StatusMessage = "Verification email sent. Please check your email.";
            return RedirectToAction(nameof(Index));
        }
        */

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        #endregion
    }
}
