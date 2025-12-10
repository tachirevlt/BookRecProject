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

    public class GetRatingsByBookIdQueryHandler : IRequestHandler<GetRatingsByBookIdQuery, BookRatingResponse>
    {
        private readonly IReviewRepository _reviewRepository;

        public GetRatingsByBookIdQueryHandler(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public async Task<BookRatingResponse> Handle(GetRatingsByBookIdQuery request, CancellationToken cancellationToken)
        {
            // 1. Lấy danh sách review
            var reviewEntities = await _reviewRepository.GetReviewsByBookIdAsync(request.BookId);

            // 2. Map sang DTO
            var reviewDtos = reviewEntities.Select(r => new ReviewDto
            {
                Id = r.Id,
                UserId = r.UserId,
                ratings_1 = r.ratings_1,
                ratings_2 = r.ratings_2,
                ratings_3 = r.ratings_3,
                ratings_4 = r.ratings_4,
                ratings_5 = r.ratings_5,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt
            }).ToList();

            double averageRating = 0;
            
            var ratedReviews = reviewDtos.Where(r => 
                (r.ratings_1 + r.ratings_2 + r.ratings_3 + r.ratings_4 + r.ratings_5) > 0
            ).ToList();

            if (ratedReviews.Any())
            {
                averageRating = ratedReviews.Average(r => 
                    r.ratings_1 * 1 + 
                    r.ratings_2 * 2 + 
                    r.ratings_3 * 3 + 
                    r.ratings_4 * 4 + 
                    r.ratings_5 * 5
                );
            }

            return new BookRatingResponse
            {
                AverageRating = Math.Round(averageRating, 1),
                TotalReviews = reviewDtos.Count,
                Reviews = reviewDtos
            };
        }
    }
}