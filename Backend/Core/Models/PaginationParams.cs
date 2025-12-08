namespace Core.Models;
public record PaginationParams(
    int PageNumber = 1,
    int PageSize = 4
);