using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Barker.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class BarkerUser : IdentityUser
    {
        public string Name { get; set; }
        public List<BarkerPost> Posts { get; set; }
    }
}
