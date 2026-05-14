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
                entity.Property(b => b.price).HasPrecision(18, 2);
            });

            modelBuilder.Entity<UserEntity>(entity =>
            {
                entity.HasKey(u => u.user_id);
                entity.Property(u => u.current_balance).HasPrecision(18, 2);

                entity.HasMany(u => u.FavoriteBooks)
                    .WithMany()
                    .UsingEntity<Dictionary<string, object>>(
                        "UserFavoriteBooks",
                        j => j.HasOne<BookEntity>().WithMany().HasForeignKey("book_id"),
                        j => j.HasOne<UserEntity>().WithMany().HasForeignKey("user_id"));

                entity.HasMany(u => u.PurchasedBooks)
                    .WithMany()
                    .UsingEntity<Dictionary<string, object>>(
                        "UserPurchasedBooks",
                        j => j.HasOne<BookEntity>().WithMany().HasForeignKey("book_id"),
                        j => j.HasOne<UserEntity>().WithMany().HasForeignKey("user_id")); 
            });

            modelBuilder.Entity<RatingEntity>(entity =>
            {
                entity.HasKey(r => new { r.user_id, r.book_id });
            });
            modelBuilder.Entity<TrackingEventEntity>(entity =>
            {
                entity.HasKey(t => t.id);
            });
        }

        public DbSet<BookEntity> Books { get; set; } = null!;
        public DbSet<UserEntity> Users { get; set; } = null!;
        public DbSet<ReviewEntity> Reviews { get; set; } = null!;
        public DbSet<RatingEntity> Ratings { get; set; } = null!;
        public DbSet<TrackingEventEntity> TrackingEvents { get; set; } = null!;
    }
}