using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Interfaces;

namespace Application.Commands
{
    public class RemoveFromWishlistCommand : IRequest<bool>
    {
        public Guid user_id { get; set; }
        public Guid book_id { get; set; }
    }

    public class RemoveFromWishlistCommandHandler : IRequestHandler<RemoveFromWishlistCommand, bool>
    {
        private readonly IWishlistRepository _wishlistRepository;

        public RemoveFromWishlistCommandHandler(IWishlistRepository wishlistRepository)
        {
            _wishlistRepository = wishlistRepository;
        }

        public async Task<bool> Handle(RemoveFromWishlistCommand request, CancellationToken cancellationToken)
        {
            return await _wishlistRepository.RemoveFromWishlistAsync(request.user_id, request.book_id, cancellationToken);
        }
    }
}
