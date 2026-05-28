using MediatR;
using Core.Interfaces;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace Application.Queries
{
    // Yêu cầu truyền vào cả UserId và BookId, trả về trực tiếp int?
    public record GetRatingsByBookIdQuery(Guid UserId, Guid BookId) : IRequest<int?>;

    public class GetRatingsByBookIdQueryHandler : IRequestHandler<GetRatingsByBookIdQuery, int?>
    {
        private readonly IRatingRepository _ratingRepository;

        // Chỉ cần inject đúng IRatingRepository
        public GetRatingsByBookIdQueryHandler(IRatingRepository ratingRepository)
        {
            _ratingRepository = ratingRepository;
        }

        public async Task<int?> Handle(GetRatingsByBookIdQuery request, CancellationToken cancellationToken)
        {
            var ratingEntity = await _ratingRepository.GetRatingAsync(request.UserId, request.BookId, cancellationToken);
            
            // Nếu có entity thì trả về số sao (1-5), nếu không có thì trả về null
            return ratingEntity?.rating;
        }
    }
}