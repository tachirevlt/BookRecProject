using MediatR;
using Core.Entities;
using Core.Interfaces;
using Core.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Queries
{
    public record GetUserByIdQuery(Guid Targetuser_id, Guid? Requestinguser_id, bool IsAdmin) : IRequest<UserDto?>;

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
            var user = await _userRepository.GetUserByIdAsync(request.Targetuser_id, cancellationToken);
            
            if (user == null) return null;

            // Kiểm tra quyền xem thông tin nhạy cảm (email)
            // Xem được nếu: Là Admin HOẶC Là chính chủ (ID người xem trùng ID user lấy ra)
            bool canViewPrivateInfo = request.IsAdmin || 
                                      (request.Requestinguser_id.HasValue && request.Requestinguser_id == user.user_id);

            // Map từ Entity sang DTO (Loại bỏ hashed_password)
            return new UserDto
            {
                user_id = user.user_id,
                user_name = user.user_name,
                full_name = user.full_name,
                role = user.role,
                sex = user.sex,
                FavoriteBooks = user.FavoriteBooks,
                email = canViewPrivateInfo ? user.email : null ,
                current_balance = canViewPrivateInfo ? user.current_balance : (decimal?)null,
                PurchasedBooks = canViewPrivateInfo ? user.PurchasedBooks : null
            };
        }
    }
}