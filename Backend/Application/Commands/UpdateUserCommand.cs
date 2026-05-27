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
    public record UpdateUserCommand(Guid user_id, UserUpdateDto UpdateData)
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
            // 1. Validate email Format
            if (!EmailRegex.IsMatch(request.UpdateData.email))
            {
                throw new ArgumentException("Định dạng email không hợp lệ.");
            }

            // 2. Validate trùng lặp Username/email (loại trừ chính user đang update)
            if (await _userRepository.IsUsernameExistsAsync(request.UpdateData.user_name, request.user_id, cancellationToken))
            {
                throw new ArgumentException("Tên đăng nhập này đã được sử dụng bởi người khác.");
            }

            if (await _userRepository.IsEmailExistsAsync(request.UpdateData.email, request.user_id, cancellationToken))
            {
                throw new ArgumentException("email này đã được sử dụng bởi tài khoản khác.");
            }

            // 3. Lấy User hiện tại từ DB lên để cập nhật
            // QUAN TRỌNG: Phải lấy user cũ lên rồi mới gán giá trị mới, 
            // nếu new UserEntity() như cũ sẽ làm mất PasswordHash và các thông tin khác.
            var existingUser = await _userRepository.GetUserByIdAsync(request.user_id, cancellationToken);
            
            if (existingUser == null)
            {
                throw new KeyNotFoundException("Không tìm thấy người dùng.");
            }

            // 4. Cập nhật các thông tin cho phép (Chỉ user_name và email)
            existingUser.user_name = request.UpdateData.user_name;
            existingUser.full_name = request.UpdateData.full_name;
            existingUser.email = request.UpdateData.email;
            existingUser.sex = request.UpdateData.sex;

            // 5. Lưu xuống DB
            // (Lưu ý: Không đụng đến hashed_password ở đây nữa)
            return await _userRepository.UpdateUserAsync(request.user_id, existingUser, cancellationToken);
        }
    }
}