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
    ) : IRequest<PagedList<BookEntity>>;
    
    public class GetAllBooksQueryHandler(IBookRepository bookRepository)
        : IRequestHandler<GetAllBooksQuery, PagedList<BookEntity>>
    {
        public async Task<PagedList<BookEntity>> Handle(GetAllBooksQuery request, CancellationToken cancellationToken)
        {
            // Gọi phương thức mới trong Repository để xử lý phân trang và lọc
            var (books, totalCount) = await bookRepository.GetAllBooksWithPaginationAndFilteringAsync(
                request.Pagination,
                request.Filters,
                cancellationToken
            );

            // Trả về danh sách đã được đóng gói trong mô hình PagedList
            return new PagedList<BookEntity>(
                books, 
                totalCount, 
                request.Pagination.PageNumber, 
                request.Pagination.PageSize
            );
        }
    }
}