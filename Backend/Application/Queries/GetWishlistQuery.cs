using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;

namespace Application.Queries
{
    public record GetWishlistQuery(Guid UserId) : IRequest<IReadOnlyList<UserWishlistEntity>>;

    public class GetWishlistQueryHandler : IRequestHandler<GetWishlistQuery, IReadOnlyList<UserWishlistEntity>>
    {
        private readonly IWishlistRepository _wishlistRepository;

        public GetWishlistQueryHandler(IWishlistRepository wishlistRepository)
        {
            _wishlistRepository = wishlistRepository;
        }

        public async Task<IReadOnlyList<UserWishlistEntity>> Handle(GetWishlistQuery request, CancellationToken cancellationToken)
        {
            return await _wishlistRepository.GetUserWishlistAsync(request.UserId, cancellationToken);
        }
    }
}
