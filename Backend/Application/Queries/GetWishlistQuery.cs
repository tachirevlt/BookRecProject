using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Core.Models;

namespace Application.Queries
{
    // Đổi thành UserWishlistDTO
    public record GetWishlistQuery(Guid UserId) : IRequest<IReadOnlyList<UserWishlistDTO>>;

    public class GetWishlistQueryHandler : IRequestHandler<GetWishlistQuery, IReadOnlyList<UserWishlistDTO>>
    {
        private readonly IWishlistRepository _wishlistRepository;

        public GetWishlistQueryHandler(IWishlistRepository wishlistRepository)
        {
            _wishlistRepository = wishlistRepository;
        }

        public async Task<IReadOnlyList<UserWishlistDTO>> Handle(GetWishlistQuery request, CancellationToken cancellationToken)
        {
            var wishlists = await _wishlistRepository.GetUserWishlistAsync(request.UserId, cancellationToken);
            return wishlists.Select(w => UserWishlistDTO.FromEntity(w))
                            .Where(dto => dto != null)
                            .ToList()!;
        }
    }
}