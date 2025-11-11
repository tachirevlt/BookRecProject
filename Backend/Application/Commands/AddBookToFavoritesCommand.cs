using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Interfaces; // Cần cho IUserRepository và IBookRepository
using System.Collections.Generic;
using Core.Entities; // Cần cho KeyNotFoundException

namespace Application.Commands
{
    /// <summary>
    /// Command để thêm một sách vào danh sách yêu thích của User.
    /// Tương ứng với: POST /api/v1/users/favorites/{book_id}
    /// (Giả định UserId sẽ được lấy từ token xác thực)
    /// </summary>
    public record AddBookToFavoritesCommand(Guid UserId, Guid BookId) : IRequest<bool>;

    /// <summary>
    /// Handler xử lý logic thêm sách vào yêu thích.
    /// </summary>
    public class AddBookToFavoritesCommandHandler : IRequestHandler<AddBookToFavoritesCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly IBookRepository _bookRepository;

        public AddBookToFavoritesCommandHandler(IUserRepository userRepository, IBookRepository bookRepository)
        {
            _userRepository = userRepository;
            _bookRepository = bookRepository;
        }

        public async Task<bool> Handle(AddBookToFavoritesCommand request, CancellationToken cancellationToken)
        {
            // 1. Lấy User (bao gồm cả danh sách favorites hiện tại)
            var user = await _userRepository.GetUserByIdAsync(request.UserId, cancellationToken);
            if (user == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy User với ID: {request.UserId}");
            }

            // 2. Lấy Book
            var book = await _bookRepository.GetBookByIdAsync(request.BookId, cancellationToken);
            if (book == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy Book với ID: {request.BookId}");
            }

            // 3. Kiểm tra xem sách đã có trong danh sách yêu thích chưa
            if (user.FavoriteBooks.Any(b => b.BookId == request.BookId))
            {
                // Sách đã có, không làm gì cả (hoặc có thể trả về false/throw lỗi)
                return false; 
            }

            // 4. Thêm sách vào danh sách favorites của User
            // Nhờ cấu hình OnModelCreating, EF Core sẽ tự động
            // cập nhật bảng liên kết "UserFavoriteBooks"
            user.FavoriteBooks.Add(book);

            // 5. Lưu thay đổi vào CSDL
            await _userRepository.UpdateUserAsync(request.UserId, user, cancellationToken);

            return true;
        }
    }
}