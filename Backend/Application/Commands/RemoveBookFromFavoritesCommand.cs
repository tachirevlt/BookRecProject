using MediatR;
using Core.Interfaces;
using Core.Entities;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Application.Commands
{
    public record RemoveBookFromFavoritesCommand(Guid UserId, Guid BookId) : IRequest<bool>;

    public class RemoveBookFromFavoritesCommandHandler : IRequestHandler<RemoveBookFromFavoritesCommand, bool>
    {
        private readonly IUserRepository _userRepository;

        public RemoveBookFromFavoritesCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> Handle(RemoveBookFromFavoritesCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByIdAsync(request.UserId, cancellationToken);

            if (user == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy User với ID: {request.UserId}");
            }

            var bookToRemove = user.FavoriteBooks.FirstOrDefault(b => b.BookId == request.BookId);

            if (bookToRemove == null)
            {
                return false;
            }

            user.FavoriteBooks.Remove(bookToRemove);

            await _userRepository.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}