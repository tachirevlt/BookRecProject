using Core.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Commands
{
    public record DeleteReviewByIdCommand(Guid ReviewId) : IRequest<bool>;

    public class DeleteReviewByIdCommandHandler : IRequestHandler<DeleteReviewByIdCommand, bool>
    {
        private readonly IReviewRepository _reviewRepository;

        public DeleteReviewByIdCommandHandler(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public async Task<bool> Handle(DeleteReviewByIdCommand request, CancellationToken cancellationToken)
        {
            return await _reviewRepository.DeleteReviewByIdAsync(request.ReviewId);
        }
    }
}