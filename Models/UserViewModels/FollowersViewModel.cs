using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Barker.Models.PostViewModels;

namespace Barker.Models.UserViewModels
{
    public class FollowersViewModel : BaseProfileViewModel
    {
        public ICollection<FollowerUser> FollowerUsers { get; set; }
    }

    public class FollowerUser 
    {
        public string UserName { get; set; }
        public int BarksCount { get; set; }
        public int FollowersCount { get; set; }
        public bool isFollowing { get; set; }
    }
}
