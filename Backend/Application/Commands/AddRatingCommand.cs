using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;

namespace Application.Commands
{
    public class AddRatingCommand : IRequest<bool>
    {
        public Guid user_id { get; set; }
        public Guid book_id { get; set; }
        public int rating { get; set; }
    }

    public class AddRatingCommandHandler : IRequestHandler<AddRatingCommand, bool>
    {
        private readonly IRatingRepository _ratingRepository;
        private readonly IBookRepository _bookRepository;

        public AddRatingCommandHandler(IRatingRepository ratingRepository, IBookRepository bookRepository)
        {
            _ratingRepository = ratingRepository;
            _bookRepository = bookRepository;
        }

        public async Task<bool> Handle(AddRatingCommand request, CancellationToken cancellationToken)
        {
            var book = await _bookRepository.GetBookByIdAsync(request.book_id, cancellationToken);
            if (book == null) return false;

            var existingRating = await _ratingRepository.GetRatingAsync(request.user_id, request.book_id, cancellationToken);

            if (existingRating != null)
            {
                // Trừ đi đánh giá cũ của người dùng
                UpdateBookRatingCount(book, existingRating.rating, -1);
                
                // Cập nhật rating mới
                existingRating.rating = request.rating;
                existingRating.time = DateTime.UtcNow;
                await _ratingRepository.UpdateRatingAsync(existingRating, cancellationToken);
            }
            else
            {
                // Tạo mới
                var newRating = new RatingEntity
                {
                    user_id = request.user_id,
                    book_id = request.book_id,
                    rating = request.rating,
                    time = DateTime.UtcNow
                };
                await _ratingRepository.AddRatingAsync(newRating, cancellationToken);
            }

            // Cộng thêm đánh giá mới vào sách
            UpdateBookRatingCount(book, request.rating, 1);
            await _bookRepository.UpdateBookAsync(book.book_id, book, cancellationToken);

            return true;
        }

        private void UpdateBookRatingCount(BookEntity book, int rating, int modifier)
        {
            switch (rating)
            {
                case 1: book.ratings_1 = Math.Max(0, book.ratings_1 + modifier); break;
                case 2: book.ratings_2 = Math.Max(0, book.ratings_2 + modifier); break;
                case 3: book.ratings_3 = Math.Max(0, book.ratings_3 + modifier); break;
                case 4: book.ratings_4 = Math.Max(0, book.ratings_4 + modifier); break;
                case 5: book.ratings_5 = Math.Max(0, book.ratings_5 + modifier); break;
            }
        }
    }
}