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
            // 1. (FIX LỖI) Chỉ định rõ ràng BookId là Khóa chính
            entity.HasKey(b => b.BookId);

            // 2. Cấu hình HasPrecision (code cũ của bạn)
            entity.Property(b => b.average_rating)
                .HasPrecision(3, 2);

            // 3. Cấu hình C# tự tạo Guid (đã dùng BookId - đúng)
            entity.Property(b => b.BookId)
                .ValueGeneratedNever();
        });

        modelBuilder.Entity<UserEntity>(entity =>
        {
            // 1. (FIX LỖI) Chỉ định rõ ràng Id là Khóa chính
            entity.HasKey(u => u.UserId);

            // 2. Cấu hình Favorites (code cũ của bạn)
            entity.HasMany(u => u.FavoriteBooks)
                .WithMany()
                .UsingEntity("UserFavoriteBooks");
        });
    }
        public DbSet<BookEntity> Books { get; set; } = null!;
        public DbSet<UserEntity> Users { get; set; } = null!;
        public DbSet<ReviewEntity> Reviews { get; set; }
    }
}

