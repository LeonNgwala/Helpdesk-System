using Helpdesk.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Helpdesk.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options) {}

        public DbSet<Ticket> Tickets => Set<Ticket>();
        public DbSet<Comment> Comments => Set<Comment>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Relationship: Ticket 1..* Comments
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Ticket)
                .WithMany(t => t.Comments)
                .HasForeignKey(c => c.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes that help querying in real-world scenarios
            modelBuilder.Entity<Ticket>()
                .HasIndex(t => new { t.Status, t.Priority });

            base.OnModelCreating(modelBuilder);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // auto-update timestamps
            var entries = ChangeTracker.Entries<Ticket>();
            var now = DateTime.UtcNow;
            foreach (var e in entries)
            {
                if (e.State == EntityState.Added) e.Entity.CreatedAtUtc = now;
                if (e.State == EntityState.Added || e.State == EntityState.Modified) e.Entity.UpdatedAtUtc = now;
                if (e.State == EntityState.Modified && e.Entity.Status == TicketStatus.Closed)
                    e.Entity.ClosedAtUtc ??= now;
            }
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
