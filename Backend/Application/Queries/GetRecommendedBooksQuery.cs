using MediatR;
using Core.Entities;
using Core.Interfaces;

namespace Application.Queries
{
    // Chỉ nhận tham số Count
    public record GetRecommendedBooksQuery(int Count = 8) : IRequest<IReadOnlyList<BookEntity>>;

    public class GetRecommendedBooksQueryHandler : IRequestHandler<GetRecommendedBooksQuery, IReadOnlyList<BookEntity>>
    {
        private readonly IBookRepository _bookRepository;

        public GetRecommendedBooksQueryHandler(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<IReadOnlyList<BookEntity>> Handle(GetRecommendedBooksQuery request, CancellationToken cancellationToken)
        {
            // Truyền request.Count xuống Repo
            return await _bookRepository.GetRecommendedBooksAsync(request.Count, cancellationToken);
        }
    }
}