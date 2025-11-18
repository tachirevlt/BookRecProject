using MediatR;
using Core.Entities;
using Core.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Commands
{
    public record UpdateUserCommand(Guid UserId, UserEntity User)
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
            if (!string.IsNullOrEmpty(request.User.HashedPassword))
            {
                request.User.HashedPassword = BCrypt.Net.BCrypt.HashPassword(request.User.HashedPassword);
            }

            return await _userRepository.UpdateUserAsync(request.UserId, request.User, cancellationToken);
        }
    }
}