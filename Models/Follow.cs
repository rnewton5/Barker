using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Barker.Models
{
    public class Follow
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string FolloweeId { get; set; }

        [ForeignKey("Follower")]
        public string FollowerId { get; set; }
        public User Follower { get; set; }
    }
}