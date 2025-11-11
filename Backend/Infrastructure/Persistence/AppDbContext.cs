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

            modelBuilder.Entity<BookEntity>(entity =>
            {
                entity.Property(b => b.average_rating)
                    .HasPrecision(3, 2);

                entity.Property(b => b.BookId)
                    .ValueGeneratedNever();

            });

            modelBuilder.Entity<UserEntity>(entity =>
            {
                entity.HasMany(u => u.FavoriteBooks)
                    .WithMany()
                    .UsingEntity("UserFavoriteBooks");

            });
        }
        public DbSet<BookEntity> Books { get; set; } = null!;
        public DbSet<UserEntity> Users { get; set; } = null!;
    }
}

