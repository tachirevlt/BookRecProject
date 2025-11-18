// Đã sửa: Application/Commands/AddUserCommand.cs
using MediatR;
using Application.Events;
using Core.Entities;
using Core.Interfaces;
using Core.Models; 
using BCrypt.Net;

namespace Application.Commands
{
    public record AddUserCommand(UserRegistrationDto UserDto) : IRequest<UserEntity>;

    public class AddUserCommandHandler(IUserRepository userRepository, IPublisher mediator)
        : IRequestHandler<AddUserCommand, UserEntity>
    {
        public async Task<UserEntity> Handle(AddUserCommand request, CancellationToken cancellationToken)
        {
            if (await userRepository.GetUserByUsernameAsync(request.UserDto.Username, cancellationToken) != null)
            {
                throw new InvalidOperationException("Tên đăng nhập đã tồn tại.");
            }
            
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.UserDto.Password);

            var newUser = new UserEntity
            {
                UserId = Guid.NewGuid(),
                Username = request.UserDto.Username,
                Email = request.UserDto.Email,
                HashedPassword = hashedPassword, 
                Role = "User" // <-- GÁN VAI TRÒ MẶC ĐỊNH (Không để người dùng tự chọn)
            };

            var createdUser = await userRepository.AddUserAsync(newUser, cancellationToken);

            await mediator.Publish(new UserCreatedEvent(createdUser.UserId), cancellationToken); 

            return createdUser;
        }
    }
}