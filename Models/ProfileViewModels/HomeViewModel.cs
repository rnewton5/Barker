using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Barker.Models.ProfileViewModels
{
    public class HomeViewModel
    {
        public SubmitBarkViewModel SubmitBarkVm { get; set; }

        public List<BarkerPost> Barks { get; set; }
    }
}
