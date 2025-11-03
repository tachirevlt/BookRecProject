namespace Core.Models;

public record BookFilterParams(
    string? Title,
    string? Author,
    string? Genre, // Sẽ được map tới tag_name
    string? SortBy,
    string? SortOrder,
    decimal? MinRating,
    decimal? MaxRating,
    int? MinYear,
    int? MaxYear
);