// Đã sửa: Application/Commands/AddUserCommand.cs
using MediatR;
using Application.Events;
using Core.Entities;
using Core.Interfaces;
using Core.Models; // <-- Cần dùng DTO
using BCrypt.Net; // <-- Thư viện để hash mật khẩu

namespace Application.Commands
{
    // Sửa: Command nhận UserRegistrationDto
    public record AddUserCommand(UserRegistrationDto UserDto) : IRequest<UserEntity>;

    // Sửa: Thêm IUserRepository để kiểm tra tồn tại và hash mật khẩu
    public class AddUserCommandHandler(IUserRepository userRepository, IPublisher mediator)
        : IRequestHandler<AddUserCommand, UserEntity>
    {
        public async Task<UserEntity> Handle(AddUserCommand request, CancellationToken cancellationToken)
        {
            // BỔ SUNG: Kiểm tra xem username hoặc email đã tồn tại chưa
            if (await userRepository.GetUserByUsernameAsync(request.UserDto.Username, cancellationToken) != null)
            {
                throw new InvalidOperationException("Tên đăng nhập đã tồn tại.");
            }
            // (Bạn cũng nên thêm GetUserByEmailAsync để kiểm tra email)

            // LỖ HỔNG BẢO MẬT: Phải HASH mật khẩu!
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.UserDto.Password);

            var newUser = new UserEntity
            {
                UserId = Guid.NewGuid(),
                Username = request.UserDto.Username,
                Email = request.UserDto.Email,
                HashedPassword = hashedPassword, // <-- Lưu mật khẩu đã hash
                Role = "User" // <-- GÁN VAI TRÒ MẶC ĐỊNH (Không để người dùng tự chọn)
            };

            var createdUser = await userRepository.AddUserAsync(newUser, cancellationToken);

            // Sửa: Event này nên là UserCreatedEvent, không phải BookCreatedEvent
            // và truyền vào createdUser.Id
            await mediator.Publish(new UserCreatedEvent(createdUser.UserId), cancellationToken); 

            return createdUser;
        }
    }

    // Bổ sung (Bạn chưa có file này):
    // Application/Events/UserCreatedEvent.cs
    public record UserCreatedEvent(Guid UserId) : INotification;
}