using MediatR;
using Core.Entities;
using Core.Interfaces;
using Core.Models; 
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Application.Commands
{
    public record UpdateUserCommand(Guid UserId, UserUpdateDto UpdateData)
        : IRequest<UserEntity>;

    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserEntity>
    {
        private readonly IUserRepository _userRepository;

        private static readonly Regex EmailRegex = new Regex(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public UpdateUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserEntity> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            if (!EmailRegex.IsMatch(request.UpdateData.Email))
            {
                throw new ArgumentException("Định dạng email không hợp lệ.");
            }

            if (await _userRepository.IsUsernameExistsAsync(request.UpdateData.Username, request.UserId, cancellationToken))
            {
                throw new ArgumentException("Tên đăng nhập này đã được sử dụng bởi người khác.");
            }

            if (await _userRepository.IsEmailExistsAsync(request.UpdateData.Email, request.UserId, cancellationToken))
            {
                throw new ArgumentException("Email này đã được sử dụng bởi tài khoản khác.");
            }
            
            var userEntityUpdates = new UserEntity
            {
                Username = request.UpdateData.Username,
                Email = request.UpdateData.Email
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