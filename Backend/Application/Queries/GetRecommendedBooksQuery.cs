using MediatR;
using Core.Entities;
using Core.Interfaces;
using Core.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Queries
{
    // Đổi thành BookDTO
    public record GetRecommendedBooksQuery(int Count = 8) : IRequest<IReadOnlyList<BookDTO>>;

    public class GetRecommendedBooksQueryHandler : IRequestHandler<GetRecommendedBooksQuery, IReadOnlyList<BookDTO>>
    {
        private readonly IBookRepository _bookRepository;

        public GetRecommendedBooksQueryHandler(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<IReadOnlyList<BookDTO>> Handle(GetRecommendedBooksQuery request, CancellationToken cancellationToken)
        {
            var books = await _bookRepository.GetRecommendedBooksAsync(request.Count, cancellationToken);
            return books.Select(b => BookDTO.FromEntity(b))
                            .Where(dto => dto != null)
                            .ToList()!;
        }
    }
}