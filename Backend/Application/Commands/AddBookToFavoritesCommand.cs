using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Interfaces; 
using System.Collections.Generic;
using Core.Entities;

namespace Application.Commands
{

    public record AddBookToFavoritesCommand(Guid UserId, Guid BookId) : IRequest<bool>;

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
            // 1. Lấy User (EF Core bắt đầu theo dõi User này + List Favorites)
            var user = await _userRepository.GetUserByIdAsync(request.UserId, cancellationToken);
            if (user == null) throw new KeyNotFoundException($"Không tìm thấy User: {request.UserId}");

            // 2. Lấy Book (EF Core bắt đầu theo dõi Book này)
            var book = await _bookRepository.GetBookByIdAsync(request.BookId, cancellationToken);
            if (book == null) throw new KeyNotFoundException($"Không tìm thấy Book: {request.BookId}");

            // 3. Kiểm tra trùng
            if (user.FavoriteBooks.Any(b => b.BookId == request.BookId))
            {
                return false;
            }

            user.FavoriteBooks.Add(book);

            await _userRepository.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}