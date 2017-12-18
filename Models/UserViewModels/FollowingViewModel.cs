using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Barker.Models.PostViewModels;

namespace Barker.Models.UserViewModels
{
    public class FollowingViewModel
    {
        public string UserName { get; set; }
        public int BarksCount { get; set; }
        public int FollowingCount { get; set; }
        public int FollowersCount { get; set; }
        public int LikesCount { get; set; }
        public DateTime JoinDate { get; set; }
        public ICollection<string> Following { get; set; }
        public ICollection<string> OtherUsers { get; set; }
        public ICollection<FollowingUser> FollowingUsers { get; set; }

        public PostViewModel PostVm { get; set; }
    }

        public class FollowingUser 
    {
        public string UserName { get; set; }
        public int BarksCount { get; set; }
        public int FollowersCount { get; set; }
        public bool isFollowing { get; set; }
    }
}
