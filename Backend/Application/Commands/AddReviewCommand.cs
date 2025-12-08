using MediatR;
using System;

namespace Application.Commands
{
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
                UserId = request.UserId, 
                BookId = request.BookId,
                Rating = request.Rating
            };

            await _reviewRepository.AddReviewAsync(review);
            return true;
        }
    }
}