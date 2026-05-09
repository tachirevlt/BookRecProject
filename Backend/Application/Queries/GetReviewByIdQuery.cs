using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Interfaces;
using Core.Models;
namespace Application.Queries
{
    public record GetReviewByIdQuery(Guid ReviewId) : IRequest<ReviewDto>;

    public class GetReviewByIdQueryHandler : IRequestHandler<GetReviewByIdQuery, ReviewDto>
    {
        private readonly IReviewRepository _reviewRepository;

        public GetReviewByIdQueryHandler(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public async Task<ReviewDto> Handle(GetReviewByIdQuery request, CancellationToken cancellationToken)
        {
            var review = await _reviewRepository.GetReviewByIdAsync(request.ReviewId);
            if (review == null) return null!;

            return new ReviewDto
            {
                id = review.id,
                user_id = review.user_id,
                book_id = review.book_id,
                review = review.review,
                time = review.time
            };
        }
    }
}