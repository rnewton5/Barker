using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Barker.Models.PostViewModels;

namespace Barker.Models.SearchViewModels
{
    public class SearchViewModel
    {
        public ICollection<UserSearchItem> Users { get; set; }

        public PostViewModel PostVm { get; set; }
    }

    public class UserSearchItem 
    {
        public string UserName { get; set; }
        public int BarksCount { get; set; }
        public int FollowersCount { get; set; }
        public bool isFollowing { get; set; }
    }
}
