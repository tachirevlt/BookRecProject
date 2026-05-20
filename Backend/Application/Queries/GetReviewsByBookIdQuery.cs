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
        private readonly IUserBookRepository _userBookRepository;

        public GetReviewsByBookIdQueryHandler(IReviewRepository reviewRepository, IUserBookRepository userBookRepository)
        {
            _reviewRepository = reviewRepository;
            _userBookRepository = userBookRepository;
        }

        public async Task<IEnumerable<ReviewDto>> Handle(GetReviewsByBookIdQuery request, CancellationToken cancellationToken)
        {
            var reviews = await _reviewRepository.GetReviewsByBookIdAsync(request.BookId);
            var purchasedUserIds = await _userBookRepository.GetUsersWhoBoughtBookAsync(request.BookId, cancellationToken);
            
            // Chuyển sang HashSet để tra cứu siêu tốc (O(1)) khi số lượng mua lớn
            var purchasedSet = new HashSet<Guid>(purchasedUserIds);

            // Map từ Entity sang Dto để loại bỏ các trường thừa/nhạy cảm
            return reviews.Select(r => new ReviewDto
            {
                id = r.id,
                user_id = r.user_id,
                book_id = r.book_id,
                full_name = r.User?.full_name,
                is_purchased = purchasedSet.Contains(r.user_id),
                review = r.review,
                time = r.time
            });
        }
    }
}