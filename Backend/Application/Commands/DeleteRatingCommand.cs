using MediatR;
using Core.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Commands
{
    public record DeleteRatingCommand(Guid BookId, Guid UserId) : IRequest<bool>;

    // Handler
    public class DeleteRatingCommandHandler : IRequestHandler<DeleteRatingCommand, bool>
    {
        private readonly IReviewRepository _reviewRepository;

        public DeleteRatingCommandHandler(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public async Task<bool> Handle(DeleteRatingCommand request, CancellationToken cancellationToken)
        {
            // Gọi Repository để xóa
            return await _reviewRepository.DeleteReviewByBookIdAsync(request.BookId, request.UserId);
        }
    }
}