using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Barker.Models;

namespace Barker.Data
{
    public class BarkerDbContext : IdentityDbContext<BarkerUser>
    {
        public BarkerDbContext(DbContextOptions<BarkerDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
        public DbSet<BarkerPost> Barks { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<BarkerPost>().ToTable("BarkerPost");

            base.OnModelCreating(builder);
        }
    }
}
