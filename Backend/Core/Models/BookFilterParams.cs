namespace Core.Models;

public record BookFilterParams(
    string? SearchTerm,
    string? Title,
    string? Author,
    string? Genre, 
    string? SortBy,
    string? SortOrder,
    int? MinYear,
    int? MaxYear
);