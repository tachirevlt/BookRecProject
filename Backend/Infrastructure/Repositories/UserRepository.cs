using Microsoft.EntityFrameworkCore;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Persistence;

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
                .FirstOrDefaultAsync(u => u.user_id == userId, ct);
        }

        public async Task<UserEntity> AddUserAsync(UserEntity user, CancellationToken ct = default)
        {
            await _db.Users.AddAsync(user, ct);
            await _db.SaveChangesAsync(ct);
            return user;
        }

        public async Task<UserEntity?> GetUserByUsernameAsync(string username, CancellationToken ct = default)
        {
            return await _db.Users
                .FirstOrDefaultAsync(u => u.user_name == username, ct);
        }

        public async Task<UserEntity> UpdateUserAsync(Guid userId, UserEntity updatedUserData, CancellationToken ct = default)
        {
            var existingUser = await _db.Users.FindAsync(new object?[] { userId }, ct);
            if (existingUser is null) throw new KeyNotFoundException($"Không tìm thấy user với ID: {userId}");
            existingUser.user_name = updatedUserData.user_name;
            existingUser.email = updatedUserData.email;
            existingUser.sex = updatedUserData.sex;
            if (!string.IsNullOrEmpty(updatedUserData.hashed_password)) existingUser.hashed_password = updatedUserData.hashed_password;
            await _db.SaveChangesAsync(ct);
            return existingUser;
        }

        public async Task<bool> DeleteUserAsync(Guid id, CancellationToken ct = default)
        {
            var entity = await _db.Users.FindAsync(new object?[] { id }, ct);
            if (entity is null) return false;
            _db.Users.Remove(entity);
            await _db.SaveChangesAsync(ct);
            return true;
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _db.SaveChangesAsync(ct);
        }

        public async Task<bool> IsEmailExistsAsync(string email, Guid? excludeuser_id = null, CancellationToken ct = default)
        {
            return await _db.Users
                .AnyAsync(u => u.email == email && (!excludeuser_id.HasValue || u.user_id != excludeuser_id), ct);
        }

        public async Task<bool> IsUsernameExistsAsync(string username, Guid? excludeuser_id = null, CancellationToken ct = default)
        {
            return await _db.Users
                .AnyAsync(u => u.user_name == username && (!excludeuser_id.HasValue || u.user_id != excludeuser_id), ct);
        }
    }
}
