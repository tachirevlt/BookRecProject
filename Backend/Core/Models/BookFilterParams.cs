namespace Core.Models;

public record BookFilterParams(
    string? SearchTerm,
    string? Title,
    string? Author,
    List<string>? Genre, 
    string? SortBy,
    string? SortOrder,
    decimal? MinRating,
    decimal? MaxRating,
    int? MinYear,
    int? MaxYear
);