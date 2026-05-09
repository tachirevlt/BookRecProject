using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Interfaces; 
using System.Collections.Generic;
using Core.Entities;
using System.Linq; // Cần thêm dòng này để dùng được hàm Any()

namespace Application.Commands
{
    public record AddBookToFavoritesCommand(Guid user_id, Guid BookId) : IRequest<bool>;

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
            var user = await _userRepository.GetUserByIdAsync(request.user_id, cancellationToken);
            if (user == null) throw new KeyNotFoundException($"Không tìm thấy User: {request.user_id}");

            var book = await _bookRepository.GetBookByIdAsync(request.BookId, cancellationToken);
            if (book == null) throw new KeyNotFoundException($"Không tìm thấy Book: {request.BookId}");

            // Đã sửa b.BookId thành b.book_id
            if (user.FavoriteBooks.Any(b => b.book_id == request.BookId))
            {
                return false;
            }

            user.FavoriteBooks.Add(book);

            await _userRepository.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}