using MediatR;
using Core.Entities;
using Core.Interfaces;
using Core.Models; 
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Commands
{
    public record UpdateUserCommand(Guid UserId, UserUpdateDto UpdateData)
        : IRequest<UserEntity>;

    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserEntity>
    {
        private readonly IUserRepository _userRepository;

        public UpdateUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserEntity> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var userEntityUpdates = new UserEntity
            {
                Username = request.UpdateData.Username,
                Email = request.UpdateData.Email,
            };

            if (!string.IsNullOrEmpty(request.UpdateData.Password))
            {
                userEntityUpdates.HashedPassword = BCrypt.Net.BCrypt.HashPassword(request.UpdateData.Password);
            }
            else
            {
                userEntityUpdates.HashedPassword = string.Empty;
            }

            return await _userRepository.UpdateUserAsync(request.UserId, userEntityUpdates, cancellationToken);
        }
    }
}