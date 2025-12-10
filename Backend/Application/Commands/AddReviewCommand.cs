using MediatR;
using System;
using System.Threading;             // Thêm namespace này cho CancellationToken
using System.Threading.Tasks;       // Thêm namespace này cho Task

namespace Application.Commands
{
    // 1. Cập nhật Record: Nhận 5 điểm đánh giá và Comment
    public record AddReviewCommand(
        Guid UserId, 
        Guid BookId, 
        int ratings_1, 
        int ratings_2, 
        int ratings_3, 
        int ratings_4, 
        int ratings_5, 
        string? Comment
    ) : IRequest<bool>;

    // 2. Cập nhật Handler
    public class AddReviewCommandHandler : IRequestHandler<AddReviewCommand, bool>
    {
        private readonly Core.Interfaces.IReviewRepository _reviewRepository;

        public AddReviewCommandHandler(Core.Interfaces.IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public async Task<bool> Handle(AddReviewCommand request, CancellationToken cancellationToken)
        {
            // Map dữ liệu từ Command sang Entity
            var review = new Core.Entities.ReviewEntity
            {
                UserId = request.UserId, 
                BookId = request.BookId,
                // Gán 5 cột điểm
                ratings_1 = request.ratings_1,
                ratings_2 = request.ratings_2,
                ratings_3 = request.ratings_3,
                ratings_4 = request.ratings_4,
                ratings_5 = request.ratings_5,
                // Gán bình luận
                Comment = request.Comment,
                // Thời gian tạo (thường Entity đã có default, nhưng gán lại cho chắc chắn)
                CreatedAt = DateTime.UtcNow
            };

            await _reviewRepository.AddReviewAsync(review);
            return true;
        }
    }
}