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
            if (!EmailRegex.IsMatch(request.UserDto.email))
            {
                throw new ArgumentException("Định dạng email không hợp lệ.");
            }

            if (await userRepository.IsUsernameExistsAsync(request.UserDto.user_name, null, cancellationToken))
            {
                throw new ArgumentException("Tên đăng nhập này đã được sử dụng.");
            }

            if (await userRepository.IsEmailExistsAsync(request.UserDto.email, null, cancellationToken))
            {
                throw new ArgumentException("email này đã được đăng ký bởi tài khoản khác.");
            }
            
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.UserDto.Password);

            var newUser = new UserEntity
            {
                user_id = Guid.NewGuid(),
                user_name = request.UserDto.user_name,
                email = request.UserDto.email,                
                sex = request.UserDto.sex,
                hashed_password = hashedPassword, 
                role = "User",                
                current_balance = 0,
                FavoriteBooks = new List<BookEntity>(),
                PurchasedBooks = new List<BookEntity>()
            };

            var createdUser = await userRepository.AddUserAsync(newUser, cancellationToken);

            await mediator.Publish(new UserCreatedEvent(createdUser.user_id), cancellationToken); 

            return createdUser;
        }
    }
}