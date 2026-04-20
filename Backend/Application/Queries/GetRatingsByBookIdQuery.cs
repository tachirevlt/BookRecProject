using MediatR;
using Application.Models;
using Core.Interfaces;
using Core.Models;
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
        private readonly IBookRepository _bookRepository;

        // Tiêm cả IBookRepository vào để lấy dữ liệu rating từ BookEntity
        public GetRatingsByBookIdQueryHandler(IReviewRepository reviewRepository, IBookRepository bookRepository)
        {
            _reviewRepository = reviewRepository;
            _bookRepository = bookRepository;
        }

        public async Task<BookRatingResponse> Handle(GetRatingsByBookIdQuery request, CancellationToken cancellationToken)
        {
            // 1. Tính toán điểm đánh giá trung bình từ BookEntity
            var book = await _bookRepository.GetBookByIdAsync(request.BookId, cancellationToken);
            
            double averageRating = 0;
            int totalRatings = 0;

            if (book != null)
            {
                totalRatings = book.ratings_1 + book.ratings_2 + book.ratings_3 + book.ratings_4 + book.ratings_5;
                if (totalRatings > 0)
                {
                    averageRating = (double)(book.ratings_1 * 1 + book.ratings_2 * 2 + book.ratings_3 * 3 + book.ratings_4 * 4 + book.ratings_5 * 5) / totalRatings;
                }
            }

            // 2. Lấy danh sách bình luận (chữ) từ ReviewEntity
            var reviewEntities = await _reviewRepository.GetReviewsByBookIdAsync(request.BookId);
            
            // 3. Map sang ReviewDto
            var reviewDtos = reviewEntities.Select(r => new ReviewDto
            {
                Id = r.Id,
                user_id = r.user_id,
                review = r.review, // Dùng trường nội dung bình luận thay cho Rating cũ
                time = r.time      // Dùng time thay cho CreatedAt
            }).ToList();

            // 4. Trả về kết quả
            return new BookRatingResponse
            {
                AverageRating = Math.Round(averageRating, 1),
                TotalReviews = reviewDtos.Count, // Dùng thuộc tính Count của List
                Reviews = reviewDtos
            };
        }
    }
}