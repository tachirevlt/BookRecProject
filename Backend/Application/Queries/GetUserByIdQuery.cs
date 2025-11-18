using MediatR;
using Core.Entities;
using Core.Interfaces;
using Core.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Queries
{
    public record GetUserByIdQuery(Guid TargetUserId, Guid? RequestingUserId, bool IsAdmin) : IRequest<UserDto?>;

    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto?>
    {
        private readonly IUserRepository _userRepository;

        public GetUserByIdQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserDto?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            // Lấy User từ DB
            var user = await _userRepository.GetUserByIdAsync(request.TargetUserId, cancellationToken);
            
            if (user == null) return null;

            // Kiểm tra quyền xem thông tin nhạy cảm (Email)
            // Xem được nếu: Là Admin HOẶC Là chính chủ (ID người xem trùng ID user lấy ra)
            bool canViewPrivateInfo = request.IsAdmin || 
                                      (request.RequestingUserId.HasValue && request.RequestingUserId == user.UserId);

            // Map từ Entity sang DTO (Loại bỏ HashedPassword)
            return new UserDto
            {
                UserId = user.UserId,
                Username = user.Username,
                Role = user.Role,
                FavoriteBooks = user.FavoriteBooks,
                Email = canViewPrivateInfo ? user.Email : null 
            };
        }
    }
}