using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Barker.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class User : IdentityUser
    {   
        //data
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy/MM/dd - H:mm}")]
        public DateTime JoinDate { get; set; }
        public Guid ProfileImageId { get; set; }

        // Navigation properties
        public List<Post> Posts { get; set; }
        public ICollection<Follow> Followees { get; set; }
        public ICollection<Image> Images { get; set; }
    }
}
