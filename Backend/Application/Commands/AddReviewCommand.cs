using MediatR;
using System;

namespace Application.Commands
{
    // SỬA: UserId và BookId chuyển từ int sang Guid
    public record AddReviewCommand(Guid UserId, Guid BookId, int Rating) : IRequest<bool>;

    // Handler
    public class AddReviewCommandHandler : IRequestHandler<AddReviewCommand, bool>
    {
        private readonly Core.Interfaces.IReviewRepository _reviewRepository;

        public AddReviewCommandHandler(Core.Interfaces.IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public async Task<bool> Handle(AddReviewCommand request, CancellationToken cancellationToken)
        {
            var review = new Core.Entities.ReviewEntity
            {
                // Bây giờ request.UserId là Guid, gán vào ReviewEntity.UserId (cũng là Guid) sẽ OK
                UserId = request.UserId, 
                BookId = request.BookId,
                Rating = request.Rating
            };

            await _reviewRepository.AddReviewAsync(review);
            return true;
        }
    }
}