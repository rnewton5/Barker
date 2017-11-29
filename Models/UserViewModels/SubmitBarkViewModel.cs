using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Barker.Models.ProfileViewModels
{
    public class SubmitBarkViewModel
    {
        [Required]
        [StringLength(150, MinimumLength = 1, ErrorMessage = "Message must be between 1 and 150 characters.")]
        public string Message { get; set; }
    }
}
