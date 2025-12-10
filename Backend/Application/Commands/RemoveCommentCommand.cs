using MediatR;
using Core.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Commands
{
    public record RemoveCommentCommand(Guid BookId, Guid UserId) : IRequest<bool>;

    public class RemoveCommentCommandHandler : IRequestHandler<RemoveCommentCommand, bool>
    {
        private readonly IReviewRepository _reviewRepository;

        public RemoveCommentCommandHandler(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public async Task<bool> Handle(RemoveCommentCommand request, CancellationToken cancellationToken)
        {
            // 1. Tìm bản ghi review của user với sách này
            var review = await _reviewRepository.GetReviewByUserAndBookAsync(request.UserId, request.BookId);
            
            if (review == null) return false; // Không tìm thấy

            // 2. Xóa nội dung comment (set về null)
            review.Comment = null;

            // 3. Lưu thay đổi
            await _reviewRepository.UpdateReviewAsync(review);
            
            
            return true;
        }
    }
}