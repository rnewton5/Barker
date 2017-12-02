using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Barker.Models
{
    public class Post
    {
        [Key]
        public int Id { get; set; }
        
        // data
        [Required]
        [StringLength(150, MinimumLength = 1)]
        public string Message { get; set; }

        [Required]
        public string Author { get; set; }

        [Required]
        public DateTime PostDate { get; set; }

        // Foreign keys and navigation properties
        [ForeignKey("User")]
        public string UserId { get; set; }
        public User User { get; set; }

        public ICollection<Like> likes { get; set; }
    }
}