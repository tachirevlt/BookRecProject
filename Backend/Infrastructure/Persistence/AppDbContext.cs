using Microsoft.EntityFrameworkCore;
using Core.Entities;

namespace Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình cụ thể cho BookEntity
            modelBuilder.Entity<BookEntity>(entity =>
            {

                entity.Property(b => b.average_rating)
                      .HasPrecision(3, 2);
            });
        }
        public DbSet<BookEntity> Books { get; set; } = null!;
    }
}

