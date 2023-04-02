using E_Project_.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace E_Project_.Data
{
    public class ApplicationDbContext :IdentityDbContext
    {
        public DbSet<Category> Category { set; get; }
        public DbSet<Product> Product { get; set; }
        public DbSet<ApplicationType> ApplicationType { get; set; }
        public DbSet<ApplicationUser> ApplicationUser { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options){}




    }
}
