using Microsoft.EntityFrameworkCore;
using RegisterLoginJWT.Models.Entities;
using RegisterLoginJWT.Models.DTOs.Role;

namespace RegisterLoginJWT.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) 
        {
        } 
        
        public DbSet<User> Users {  get; set; }
        public DbSet<Role> Roles { get; set; }

    }
}
