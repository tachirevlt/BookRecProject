using Microsoft.EntityFrameworkCore;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Persistence;
using Core.Models;

namespace Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _db;

        public UserRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<UserEntity?> GetUserByIdAsync(Guid id, CancellationToken ct = default)
        {
            var user = await _db.Users.FindAsync(new object?[] { id }, ct);
            // Xử lý trường hợp không tìm thấy nếu cần (ví dụ throw exception)
            if (user == null) throw new KeyNotFoundException($"Không tìm thấy sách với ID: {id}");
            return user;
        }


        public async Task<UserEntity> AddUserAsync(UserEntity user, CancellationToken ct = default)
        {
            await _db.Users.AddAsync(user, ct);
            await _db.SaveChangesAsync(ct);
            return user; // Trả về entity đã được thêm (EF Core sẽ cập nhật ID)
        }

        public async Task<UserEntity?> GetUserByIdWithFavoritesAsync(Guid id, CancellationToken ct = default)
                {
                    // Dùng Include để tải danh sách FavoriteBooks cùng lúc với User
                    return await _db.Users
                        .Include(u => u.FavoriteBooks)
                        .FirstOrDefaultAsync(u => u.UserId == id, ct);
                }
        public async Task<UserEntity> UpdateUserAsync(Guid userId, UserEntity updatedUserData, CancellationToken ct = default)
        {
            var existingUser = await _db.Users.FindAsync(new object?[] { userId }, ct);
            if (existingUser is null)
            {
                // Nên throw exception hoặc trả về null/Result pattern tùy thiết kế
                throw new KeyNotFoundException($"Không tìm thấy user với ID: {userId}");
            }

            existingUser.userName = updatedUserData.userName;
            existingUser.FavoriteBooks = updatedUserData.FavoriteBooks;


            _db.Users.Update(existingUser); // Update entity đã được track
            await _db.SaveChangesAsync(ct);
            return existingUser; // Trả về entity đã được cập nhật
        }

        public async Task<bool> DeleteUserAsync(Guid id, CancellationToken ct = default)
        {
            var entity = await _db.Users.FindAsync(new object?[] { id }, ct);
            if (entity is null)
            {
                return false; // Không tìm thấy để xóa
            }
            _db.Users.Remove(entity);
            await _db.SaveChangesAsync(ct);
            return true; // Xóa thành công
        }
    }
}
