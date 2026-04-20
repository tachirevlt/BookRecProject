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
                entity.HasKey(b => b.book_id);
                entity.Property(b => b.book_id).ValueGeneratedNever();
            });

            modelBuilder.Entity<UserEntity>(entity =>
            {
                entity.HasKey(u => u.UserId);

                entity.HasMany(u => u.FavoriteBooks)
                    .WithMany()
                    .UsingEntity("UserFavoriteBooks");
            });

            // Ràng buộc cho RatingEntity: 1 user và 1 sách chỉ tạo 1 đánh giá duy nhất
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