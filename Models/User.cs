﻿using System;
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
        // data
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; }

        // Navigation properties
        public List<Post> Posts { get; set; }
        public ICollection<Following> Followings { get; set; }
    }
}
