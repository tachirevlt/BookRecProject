using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;

namespace Application.Commands
{
    public class DeleteRatingCommand : IRequest<bool>
    {
        public Guid user_id { get; set; }
        public Guid book_id { get; set; }
    }

    public class DeleteRatingCommandHandler : IRequestHandler<DeleteRatingCommand, bool>
    {
        private readonly IRatingRepository _ratingRepository;
        private readonly IBookRepository _bookRepository;

        public DeleteRatingCommandHandler(IRatingRepository ratingRepository, IBookRepository bookRepository)
        {
            _ratingRepository = ratingRepository;
            _bookRepository = bookRepository;
        }

        public async Task<bool> Handle(DeleteRatingCommand request, CancellationToken cancellationToken)
        {
            var existingRating = await _ratingRepository.GetRatingAsync(request.user_id, request.book_id, cancellationToken);
            if (existingRating == null) return false;

            var book = await _bookRepository.GetBookByIdAsync(request.book_id, cancellationToken);
            if (book != null)
            {
                switch (existingRating.rating)
                {
                    case 1: book.ratings_1 = Math.Max(0, book.ratings_1 - 1); break;
                    case 2: book.ratings_2 = Math.Max(0, book.ratings_2 - 1); break;
                    case 3: book.ratings_3 = Math.Max(0, book.ratings_3 - 1); break;
                    case 4: book.ratings_4 = Math.Max(0, book.ratings_4 - 1); break;
                    case 5: book.ratings_5 = Math.Max(0, book.ratings_5 - 1); break;
                }
                await _bookRepository.UpdateBookAsync(book.book_id, book, cancellationToken);
            }

            await _ratingRepository.DeleteRatingAsync(request.user_id, request.book_id, cancellationToken);
            return true;
        }
    }
}