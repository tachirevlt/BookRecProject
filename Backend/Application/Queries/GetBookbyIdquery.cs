using MediatR;
using Core.Entities;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Models;
namespace Application.Queries
{
    public record GetBookByIdQuery(Guid BookId) : IRequest<BookDTO?>;

    public class GetBookByIdQueryHandler(IBookRepository bookRepository)
        : IRequestHandler<GetBookByIdQuery, BookDTO?>
    {
        public async Task<BookDTO?> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
        {
            var book = await bookRepository.GetBookByIdAsync(request.BookId, cancellationToken);
            return BookDTO.FromEntity(book);
        }
    }
}
