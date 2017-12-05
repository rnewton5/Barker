using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Barker.Models.PostViewModels;

namespace Barker.Models.UserViewModels
{
    public class HomeViewModel
    {
        public string UserName { get; set; }
        public int BarksCount { get; set; }
        public int FollowingCount { get; set; }
        public int FollowersCount { get; set; }

        public PostViewModel PostVm { get; set; }
        public List<Post> Barks { get; set; }
    }
}