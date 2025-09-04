using Microsoft.EntityFrameworkCore;
using HelpdeskSystem.Models;

namespace HelpdeskSystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Ticket> Tickets { get; set; } // Represents Tickets table
        public DbSet<Comment> Comments { get; set; }
    }
}
