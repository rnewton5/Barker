using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Barker.Models.PostViewModels;

namespace Barker.Models.UserViewModels
{
    public class BaseProfileViewModel
    {
        public string UserName { get; set; }
        public int BarksCount { get; set; }
        public int FollowingCount { get; set; }
        public int FollowersCount { get; set; }
        public int LikesCount { get; set; }
        
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd - H:mm}")]
        public DateTime JoinDate { get; set; }
        public ICollection<string> Following { get; set; }
        public ICollection<string> OtherUsers { get; set; }

        public PostViewModel PostVm { get; set; }
    }
}
