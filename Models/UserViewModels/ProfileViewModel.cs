using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Barker.Models.PostViewModels;

namespace Barker.Models.UserViewModels
{
    public class ProfileViewModel
    {
        public string Name { get; set; }
        public string UserName { get; set; }

        public SubmitPostViewModel SubmitPostVm { get; set; }
        public List<BarkerPost> Barks { get; set; }
    }
}
