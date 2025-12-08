using MediatR;
using Microsoft.AspNetCore.Identity; // Giữ dòng này lại nếu UserManager báo đỏ
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

// 👇 QUAN TRỌNG: Thay dòng này bằng namespace chứa class AppUser thực tế trong dự án của bạn
// Ví dụ: BookRecProject.Domain.Entities hoặc BookRecProject.Core.Entities
using Core.Entities; 

namespace Application.commands // Hoặc namespace đúng của file này
{
    // Command
    public record ChangePasswordCommand(Guid UserId, string CurrentPassword, string NewPassword) : IRequest<Unit>;

    // Handler
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Unit>
    {
        private readonly UserManager<AppUser> _userManager;

        public ChangePasswordCommandHandler(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Unit> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            // 1. Tìm user
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            
            if (user == null)
            {
                throw new KeyNotFoundException("Không tìm thấy người dùng.");
            }

            // 2. Đổi mật khẩu
            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

            if (!result.Succeeded)
            {
                // Gom lỗi trả về
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new ArgumentException(errors);
            }

            return Unit.Value;
        }
    }
}