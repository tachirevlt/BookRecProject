using Core.Entities;
using System;
using System.Threading; 
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IUserRepository
    {
        Task<UserEntity?> GetUserByIdAsync(Guid id, CancellationToken ct = default);
        Task<UserEntity> AddUserAsync(UserEntity entity, CancellationToken ct = default);
        Task<UserEntity> UpdateUserAsync(Guid userId, UserEntity entity, CancellationToken ct = default);
        Task<bool> DeleteUserAsync(Guid userId, CancellationToken ct = default);
        Task<UserEntity?> GetUserByUsernameAsync(string username, CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default);
        Task<bool> IsEmailExistsAsync(string email, Guid? excludeUserId = null, CancellationToken ct = default);
        Task<bool> IsUsernameExistsAsync(string username, Guid? excludeUserId = null, CancellationToken ct = default);
        Task<bool> PurchaseBookAsync(Guid userId, Guid bookId, CancellationToken ct = default);
    }
}