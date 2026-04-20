using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;

namespace Application.Commands
{
    public class AddReviewCommand : IRequest<bool>
    {
        public Guid user_id { get; set; }
        public Guid book_id { get; set; }
        public string review { get; set; } = null!;
    }

    public class AddReviewCommandHandler : IRequestHandler<AddReviewCommand, bool>
    {
        private readonly IReviewRepository _reviewRepository;

        public AddReviewCommandHandler(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public async Task<bool> Handle(AddReviewCommand request, CancellationToken cancellationToken)
        {
            var newReview = new ReviewEntity
            {
                Id = Guid.NewGuid(),
                user_id = request.user_id,
                book_id = request.book_id,
                review = request.review,
                time = DateTime.UtcNow
            };

            await _reviewRepository.AddReviewAsync(newReview);
            return true;
        }
    }
}