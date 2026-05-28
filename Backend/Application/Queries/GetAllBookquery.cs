using MediatR;
using Core.Entities;
using Core.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Core.Models; 

namespace Application.Queries
{

    public record GetAllBooksQuery(
        PaginationParams Pagination,
        BookFilterParams Filters
    ) : IRequest<PagedList<BookDTO>>;
    
    public class GetAllBooksQueryHandler(IBookRepository bookRepository)
        : IRequestHandler<GetAllBooksQuery, PagedList<BookDTO>>
    {
        public async Task<PagedList<BookDTO>> Handle(GetAllBooksQuery request, CancellationToken cancellationToken)
        {
            var (books, totalCount) = await bookRepository.GetAllBooksWithPaginationAndFilteringAsync(
                request.Pagination,
                request.Filters,
                cancellationToken
            );

            var dtoList = books.Select(b => BookDTO.FromEntity(b)!).ToList();

            return new PagedList<BookDTO>(
                dtoList, 
                totalCount, 
                request.Pagination.PageNumber, 
                request.Pagination.PageSize
            );
        }
    }
}