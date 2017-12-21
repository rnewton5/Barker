using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Barker.Models.PostViewModels;

namespace Barker.Models.ManageViewModels
{
    public class ChangeBioViewModel
    {
        [Required]
        [StringLength(200, MinimumLength = 3)]
        public string Bio { get; set; }
    }
}
