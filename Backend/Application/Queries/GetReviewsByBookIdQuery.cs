using Core.Entities;
using Core.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Models;

namespace Application.Queries
{
    public record GetReviewsByBookIdQuery(Guid BookId) : IRequest<IEnumerable<ReviewDto>>;

    public class GetReviewsByBookIdQueryHandler : IRequestHandler<GetReviewsByBookIdQuery, IEnumerable<ReviewDto>>
    {
        private readonly IReviewRepository _reviewRepository;

        public GetReviewsByBookIdQueryHandler(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public async Task<IEnumerable<ReviewDto>> Handle(GetReviewsByBookIdQuery request, CancellationToken cancellationToken)
        {
            var reviews = await _reviewRepository.GetReviewsByBookIdAsync(request.BookId);

            // Map từ Entity sang Dto để loại bỏ các trường thừa/nhạy cảm
            return reviews.Select(r => new ReviewDto
            {
                id = r.id,
                user_id = r.user_id,
                book_id = r.book_id,
                review = r.review,
                time = r.time
            });
        }
    }
}