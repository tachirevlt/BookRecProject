using MediatR;
using Application.Models;
using Core.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace Application.Queries
{
    public record GetRatingsByBookIdQuery(Guid BookId) : IRequest<BookRatingResponse>;

    // Handler
    public class GetRatingsByBookIdQueryHandler : IRequestHandler<GetRatingsByBookIdQuery, BookRatingResponse>
    {
        private readonly IReviewRepository _reviewRepository;

        public GetRatingsByBookIdQueryHandler(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public async Task<BookRatingResponse> Handle(GetRatingsByBookIdQuery request, CancellationToken cancellationToken)
        {
            var reviewEntities = await _reviewRepository.GetReviewsByBookIdAsync(request.BookId);
            var averageRating = await _reviewRepository.GetAverageRatingAsync(request.BookId);
            var reviewDtos = reviewEntities.Select(r => new ReviewDto
            {
                Id = r.Id,
                UserId = r.UserId,
                Rating = r.Rating,
                CreatedAt = r.CreatedAt
            }).ToList();

            return new BookRatingResponse
            {
                AverageRating = Math.Round(averageRating, 1),
                TotalReviews = reviewDtos.Count,
                Reviews = reviewDtos
            };
        }
    }
}