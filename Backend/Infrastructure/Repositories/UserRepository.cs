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

        public async Task<UserEntity?> GetUserByIdAsync(Guid userId, CancellationToken ct = default)
        {

            return await _db.Users
                .Include(u => u.FavoriteBooks)
                .FirstOrDefaultAsync(u => u.UserId == userId, ct);
        }


        public async Task<UserEntity> AddUserAsync(UserEntity user, CancellationToken ct = default)
        {
            await _db.Users.AddAsync(user, ct);
            await _db.SaveChangesAsync(ct);
            return user; // Trả về entity đã được thêm (EF Core sẽ cập nhật ID)
        }
        public async Task<UserEntity?> GetUserByUsernameAsync(string username, CancellationToken ct = default)
        {
            // Dùng FirstOrDefaultAsync để tìm user theo tên
            return await _db.Users
                .FirstOrDefaultAsync(u => u.Username == username, ct);
        }

        public async Task<UserEntity> UpdateUserAsync(Guid userId, UserEntity updatedUserData, CancellationToken ct = default)
        {
            var existingUser = await _db.Users.FindAsync(new object?[] { userId }, ct);
            
            if (existingUser is null)
            {
                throw new KeyNotFoundException($"Không tìm thấy user với ID: {userId}");
            }
            existingUser.Username = updatedUserData.Username;
            existingUser.Email = updatedUserData.Email;

            if (!string.IsNullOrEmpty(updatedUserData.HashedPassword))
            {
                existingUser.HashedPassword = updatedUserData.HashedPassword;
            }
            await _db.SaveChangesAsync(ct);
            
            return existingUser;
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
        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _db.SaveChangesAsync(ct);
        }
        public async Task<bool> IsEmailExistsAsync(string email, Guid? excludeUserId = null, CancellationToken ct = default)
        {
            // Kiểm tra xem có user nào KHÁC (người có ID != excludeUserId) đang dùng email này không
            return await _db.Users
                .AnyAsync(u => u.Email == email && (!excludeUserId.HasValue || u.UserId != excludeUserId), ct);
        }
        public async Task<bool> IsUsernameExistsAsync(string username, Guid? excludeUserId = null, CancellationToken ct = default)
        {
            return await _db.Users
                .AnyAsync(u => u.Username == username && (!excludeUserId.HasValue || u.UserId != excludeUserId), ct);
        }
    }
}
