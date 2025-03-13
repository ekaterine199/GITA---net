using Microsoft.EntityFrameworkCore;
using RegisterLoginJWTMTO20.Models.Entities;

namespace RegisterLoginJWTMTO20.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
    }
}
