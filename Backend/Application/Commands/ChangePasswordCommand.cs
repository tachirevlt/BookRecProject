using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using BCrypt.Net;

namespace Application.Commands
{
    // Command
    public record ChangePasswordCommand(Guid UserId, string CurrentPassword, string NewPassword) : IRequest<Unit>;

    // Handler
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Unit>
    {
        private readonly IUserRepository _userRepository;

        public ChangePasswordCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Unit> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            // 1. SỬA LỖI: Dùng hàm GetUserByIdAsync
            var user = await _userRepository.GetUserByIdAsync(request.UserId, cancellationToken);
            
            if (user == null)
            {
                throw new KeyNotFoundException("Không tìm thấy người dùng.");
            }

            // 2. Kiểm tra mật khẩu cũ
            bool isPasswordCorrect = BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.HashedPassword);

            if (!isPasswordCorrect)
            {
                throw new ArgumentException("Mật khẩu hiện tại không chính xác.");
            }

            // 3. Mã hóa mật khẩu mới
            string newPasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

            // 4. Cập nhật thông tin
            user.HashedPassword = newPasswordHash;
            // user.SecurityStamp = Guid.NewGuid().ToString(); // Bỏ comment dòng này nếu bạn đã thêm trường SecurityStamp vào UserEntity

            // 5. SỬA LỖI: Dùng hàm UpdateUserAsync (truyền cả ID và User)
            await _userRepository.UpdateUserAsync(user.UserId, user, cancellationToken);

            return Unit.Value;
        }
    }
}