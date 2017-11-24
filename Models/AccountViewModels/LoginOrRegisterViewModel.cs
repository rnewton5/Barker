using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Barker.Models.AccountViewModels
{
    public class LoginOrRegisterViewModel
    {
        /* Login */
        [EmailAddress]
        public string LoginEmail { get; set; }

        [DataType(DataType.Password)]
        public string LoginPassword { get; set; }

        [Display(Name = "Remember me?")]
        public bool LoginRememberMe { get; set; }


        /* Register */
        [Display(Name = "Full Name")]
        [StringLength(50, MinimumLength = 2)]
        public string RegisterName { get; set; }

        [Display(Name = "User Name")]
        [StringLength(25, MinimumLength = 3)]
        public string RegisterUserName { get; set; }

        [EmailAddress]
        [Display(Name = "Email")]
        public string RegisterEmail { get; set; }

        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string RegisterPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("RegisterPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string RegisterConfirmPassword { get; set; }
    }
}
