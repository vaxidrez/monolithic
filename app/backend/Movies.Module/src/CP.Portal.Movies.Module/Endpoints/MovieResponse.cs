namespace CP.Portal.Movies.Module.Endpoints;

public record MovieResponse(
    Guid Id,
    string Title,
    string? OriginalTitle,
    string? Synopsis,
    DateOnly ReleaseYear,
    int DurationMinutes,
    string Language,
    decimal RentalPrice,
    DateTime CreatedAt
);
