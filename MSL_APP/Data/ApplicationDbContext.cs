using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MSL_APP.Models;

namespace MSL_APP.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<MSL_APP.Models.EligibleStudent> EligibleStudent { get; set; }
        public DbSet<MSL_APP.Models.Product> Product { get; set; }
        public DbSet<MSL_APP.Models.ProductKey> ProductKey { get; set; }
        public DbSet<MSL_APP.Models.Logs> Logs { get; set; }
    }
}
