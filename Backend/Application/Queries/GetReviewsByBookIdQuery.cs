using Core.Entities;
using Core.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Models;

namespace Application.Queries
{
    public record GetReviewsByBookIdQuery(Guid BookId, int PageNumber = 1, int PageSize = 10) : IRequest<IEnumerable<ReviewDto>>;

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
            // 1. Lấy ra danh sách review (đã được phân trang, ví dụ chỉ 10 dòng)
            var reviews = await _reviewRepository.GetReviewsByBookIdAsync(request.BookId, request.PageNumber, request.PageSize);
            
            if (!reviews.Any())
            {
                return Enumerable.Empty<ReviewDto>();
            }

            // 2. Trích xuất danh sách user_id duy nhất của những bình luận trong TRANG HIỆN TẠI (tối đa 10 user)
            var userIdsInCurrentPage = reviews.Select(r => r.user_id).Distinct().ToList();

            // 3. Tối ưu tuyệt đối: Truyền list 10 user này xuống DB để kiểm tra xem ai đã mua sách
            var purchasedUserIds = await _userBookRepository.CheckUsersWhoBoughtBookAsync(
                request.BookId, 
                userIdsInCurrentPage, 
                cancellationToken);
            
            // Đưa vào HashSet để check O(1) ở vòng lặp map phía dưới
            var purchasedSet = new HashSet<Guid>(purchasedUserIds);

            // 4. Map dữ liệu
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