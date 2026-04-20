using Microsoft.EntityFrameworkCore;
using Core.Entities;

namespace Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BookEntity>(entity =>
            {
                entity.HasKey(b => b.book_id);
                entity.Property(b => b.book_id).ValueGeneratedNever();
                entity.Property(b => b.cost).HasPrecision(18, 2);
            });

            modelBuilder.Entity<UserEntity>(entity =>
            {
                entity.HasKey(u => u.UserId);
                entity.Property(u => u.CurrentBalance).HasPrecision(18, 2);

                entity.HasMany(u => u.FavoriteBooks)
                    .WithMany()
                    .UsingEntity("UserFavoriteBooks");

                entity.HasMany(u => u.PurchasedBooks)
                    .WithMany()
                    .UsingEntity("UserPurchasedBooks"); 
            });

            modelBuilder.Entity<RatingEntity>(entity =>
            {
                entity.HasKey(r => new { r.user_id, r.book_id });
            });
        }

        public DbSet<BookEntity> Books { get; set; } = null!;
        public DbSet<UserEntity> Users { get; set; } = null!;
        public DbSet<ReviewEntity> Reviews { get; set; } = null!;
        public DbSet<RatingEntity> Ratings { get; set; } = null!;
    }
}