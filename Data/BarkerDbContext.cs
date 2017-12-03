using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Barker.Models;

namespace Barker.Data
{
    public class BarkerDbContext : IdentityDbContext<User>
    {
        public BarkerDbContext(DbContextOptions<BarkerDbContext> options)
            : base(options)
        { }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Follow> Follows { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Post>().ToTable("Posts");
            builder.Entity<Like>().ToTable("Likes");
            builder.Entity<Follow>().ToTable("Follows");

            base.OnModelCreating(builder);
        }
    }
}
