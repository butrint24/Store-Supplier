using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StoreSupplier.Models;

namespace StoreSupplier.Infrastructure
{
    public class StoreSupplierContext : IdentityDbContext<AppUser>
    {
        public StoreSupplierContext(DbContextOptions<StoreSupplierContext> options)
            : base(options)
        {
        }
        public DbSet<Page> Pages { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }

    }
}
