// File: Backend/Application/Commands/AddUserCommand.cs
using MediatR;
using Application.Events;
using Core.Entities;
using Core.Interfaces;
using Core.Models; 
using BCrypt.Net;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions; // Thêm thư viện Regex

namespace Application.Commands
{
    public record AddUserCommand(UserRegistrationDto UserDto) : IRequest<UserEntity>;

    public class AddUserCommandHandler(IUserRepository userRepository, IPublisher mediator)
        : IRequestHandler<AddUserCommand, UserEntity>
    {
        private static readonly Regex EmailRegex = new Regex(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public async Task<UserEntity> Handle(AddUserCommand request, CancellationToken cancellationToken)
        {
            if (!EmailRegex.IsMatch(request.UserDto.Email))
            {
                throw new ArgumentException("Định dạng email không hợp lệ.");
            }

            if (await userRepository.IsUsernameExistsAsync(request.UserDto.Username, null, cancellationToken))
            {
                throw new ArgumentException("Tên đăng nhập này đã được sử dụng.");
            }

            if (await userRepository.IsEmailExistsAsync(request.UserDto.Email, null, cancellationToken))
            {
                throw new ArgumentException("Email này đã được đăng ký bởi tài khoản khác.");
            }
            
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.UserDto.Password);

            var newUser = new UserEntity
            {
                UserId = Guid.NewGuid(),
                Username = request.UserDto.Username,
                Email = request.UserDto.Email,
                HashedPassword = hashedPassword, 
                Role = "User"
            };

            // 6. Lưu vào DB
            var createdUser = await userRepository.AddUserAsync(newUser, cancellationToken);

            // 7. Bắn sự kiện
            await mediator.Publish(new UserCreatedEvent(createdUser.UserId), cancellationToken); 

            return createdUser;
        }
    }
}