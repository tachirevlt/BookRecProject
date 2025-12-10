using MediatR;
using Core.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Commands
{
    public record RemoveRatingCommand(Guid BookId, Guid UserId) : IRequest<bool>;

    public class RemoveRatingCommandHandler : IRequestHandler<RemoveRatingCommand, bool>
    {
        private readonly IReviewRepository _reviewRepository;

        public RemoveRatingCommandHandler(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public async Task<bool> Handle(RemoveRatingCommand request, CancellationToken cancellationToken)
        {
            // 1. Tìm bản ghi review
            var review = await _reviewRepository.GetReviewByUserAndBookAsync(request.UserId, request.BookId);
            
            if (review == null) return false;

            // 2. Reset tất cả điểm đánh giá về 0 (Xóa rating)
            review.ratings_1 = 0;
            review.ratings_2 = 0;
            review.ratings_3 = 0;
            review.ratings_4 = 0;
            review.ratings_5 = 0;

            // 3. Lưu thay đổi
            await _reviewRepository.UpdateReviewAsync(review);

            return true;
        }
    }
}