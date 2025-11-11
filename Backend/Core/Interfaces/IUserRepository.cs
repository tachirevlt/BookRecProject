using Core.Entities;
using System;
using System.Threading; 
using System.Threading.Tasks;
using Core.Models; 
namespace Core.Interfaces
{
    public interface IUserRepository
    {
        Task<UserEntity?> GetUserByIdAsync(Guid id, CancellationToken ct = default);
        Task<UserEntity> AddUserAsync(UserEntity entity, CancellationToken ct = default);
        Task<UserEntity> UpdateUserAsync(Guid userId, UserEntity entity, CancellationToken ct = default);
        Task<bool> DeleteUserAsync(Guid userId, CancellationToken ct = default); 
        Task<UserEntity?> GetUserByUsernameAsync(string username, CancellationToken ct = default);
    }
}