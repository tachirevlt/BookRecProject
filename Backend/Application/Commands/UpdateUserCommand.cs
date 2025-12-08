using MediatR;
using Core.Entities;
using Core.Interfaces;
using Core.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Application.Commands
{
    public record UpdateUserCommand(Guid UserId, UserUpdateDto UpdateData)
        : IRequest<UserEntity>;

    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserEntity>
    {
        private readonly IUserRepository _userRepository;

        // Regex kiểm tra email (có thể tái sử dụng hoặc để ở đây)
        private static readonly Regex EmailRegex = new Regex(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public UpdateUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserEntity> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            // 1. Validate Email Format
            if (!EmailRegex.IsMatch(request.UpdateData.Email))
            {
                throw new ArgumentException("Định dạng email không hợp lệ.");
            }

            // 2. Validate trùng lặp Username/Email (loại trừ chính user đang update)
            if (await _userRepository.IsUsernameExistsAsync(request.UpdateData.Username, request.UserId, cancellationToken))
            {
                throw new ArgumentException("Tên đăng nhập này đã được sử dụng bởi người khác.");
            }

            if (await _userRepository.IsEmailExistsAsync(request.UpdateData.Email, request.UserId, cancellationToken))
            {
                throw new ArgumentException("Email này đã được sử dụng bởi tài khoản khác.");
            }

            // 3. Lấy User hiện tại từ DB lên để cập nhật
            // QUAN TRỌNG: Phải lấy user cũ lên rồi mới gán giá trị mới, 
            // nếu new UserEntity() như cũ sẽ làm mất PasswordHash và các thông tin khác.
            var existingUser = await _userRepository.GetUserByIdAsync(request.UserId, cancellationToken);
            
            if (existingUser == null)
            {
                throw new KeyNotFoundException("Không tìm thấy người dùng.");
            }

            // 4. Cập nhật các thông tin cho phép (Chỉ Username và Email)
            existingUser.Username = request.UpdateData.Username;
            existingUser.Email = request.UpdateData.Email;

            // 5. Lưu xuống DB
            // (Lưu ý: Không đụng đến HashedPassword ở đây nữa)
            return await _userRepository.UpdateUserAsync(request.UserId, existingUser, cancellationToken);
        }
    }
}