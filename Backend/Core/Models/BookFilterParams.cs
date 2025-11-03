namespace Core.Models;

public record BookFilterParams(
    string? Title,
    string? Author,
    string? Genre
);