using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Interfaces; 
using System.Collections.Generic;
using System.Linq; // Bắt buộc phải có using này để dùng FirstOrDefault()

namespace Application.Commands
{
    public record RemoveBookFromFavoritesCommand(Guid user_id, Guid BookId) : IRequest<bool>;

    public class RemoveBookFromFavoritesCommandHandler : IRequestHandler<RemoveBookFromFavoritesCommand, bool>
    {
        private readonly IUserRepository _userRepository;

        public RemoveBookFromFavoritesCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> Handle(RemoveBookFromFavoritesCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByIdAsync(request.user_id, cancellationToken);
            if (user == null) throw new KeyNotFoundException($"Không tìm thấy User: {request.user_id}");

            // Đã sửa b.BookId thành b.book_id
            var bookToRemove = user.FavoriteBooks.FirstOrDefault(b => b.book_id == request.BookId);
            
            if (bookToRemove == null)
            {
                return false; // Sách không có trong danh sách yêu thích
            }

            user.FavoriteBooks.Remove(bookToRemove);

            await _userRepository.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}