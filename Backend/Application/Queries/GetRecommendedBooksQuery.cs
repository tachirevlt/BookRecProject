using MediatR;
using Core.Entities;
using Core.Interfaces;

namespace Application.Queries
{
    // Đổi tên Query
    public record GetRecommendedBooksQuery(Guid BookId) : IRequest<IReadOnlyList<BookEntity>>;

    // Đổi tên Handler
    public class GetRecommendedBooksQueryHandler : IRequestHandler<GetRecommendedBooksQuery, IReadOnlyList<BookEntity>>
    {
        private readonly IBookRepository _bookRepository;

        public GetRecommendedBooksQueryHandler(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<IReadOnlyList<BookEntity>> Handle(GetRecommendedBooksQuery request, CancellationToken cancellationToken)
        {
            // Gọi phương thức Repository đã đổi tên
            return await _bookRepository.GetRecommendedBooksAsync(request.BookId, 10, cancellationToken);
        }
    }
}