using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Barker.Models.PostViewModels;

namespace Barker.Models.ManageViewModels
{
    public class ChangeUserNameViewModel
    {
        [Required]
        [Display(Name = "Current UserName")]
        public string CurrentUserName { get; set; }

        [Required]
        [StringLength(25, MinimumLength = 3)]
        [RegularExpression(pattern: "[a-zA-Z0-9-_]+")]
        [Display(Name = "New UserName")]
        public string NewUserName { get; set; }
    }
}
