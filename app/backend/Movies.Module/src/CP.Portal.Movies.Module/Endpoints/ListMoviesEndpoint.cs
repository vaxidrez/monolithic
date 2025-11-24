using CP.Core.Contracts.Core;
using CP.Portal.Movies.Module.Data;
using CP.Portal.Movies.Module.Services;

using FastEndpoints;

namespace CP.Portal.Movies.Module.Endpoints;

// ========== CREATE MOVIE ==========
public class CreateMovieRequest
{
    // Required by domain model
    public string Title { get; set; } = string.Empty;
    public DateOnly ReleaseYear { get; set; }
    public int DurationMinutes { get; set; }
    public string Language { get; set; } = string.Empty;
    public decimal Price { get; set; } // maps to Movie.RentalPrice

    // Optional by domain model
    public string? OriginalTitle { get; set; }
    public string Description { get; set; } = string.Empty; // maps to Movie.Synopsis
}

// Validator se ejecutará automáticamente por ValidationEndpointFilter
internal sealed class CreateMovieRequestValidator : IValidator<CreateMovieRequest>
{
    public IEnumerable<ValidationError> Validate(CreateMovieRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Title))
            yield return new ValidationError(nameof(req.Title), "Title is required.");
        if (string.IsNullOrWhiteSpace(req.Description))
            yield return new ValidationError(nameof(req.Description), "Description is required.");
        if (req.DurationMinutes <= 0)
            yield return new ValidationError(nameof(req.DurationMinutes), "DurationMinutes must be > 0.");
        if (string.IsNullOrWhiteSpace(req.Language))
            yield return new ValidationError(nameof(req.Language), "Language is required.");
        if (req.Price < 0m)
            yield return new ValidationError(nameof(req.Price), "Price cannot be negative.");
    }
}

internal static class MovieExtensions
{
    extension(CreateMovieRequest req)
    {
        public Movie ToMovie()
        {
            return new Movie(
                req.Title,
                req.ReleaseYear,
                req.DurationMinutes,
                req.Language,
                req.Price,
                originalTitle: req.OriginalTitle,
                synopsis: req.Description
            );
        }
    }

    extension(Movie movie)
    {
        public MovieResponse ToMovieResponse()
        {
            return new MovieResponse(
                movie.MovieId,
                movie.Title,
                movie.OriginalTitle,
                movie.Synopsis,
                movie.ReleaseYear,
                movie.DurationMinutes,
                movie.Language,
                movie.RentalPrice,
                movie.CreatedAt
            );
        }
    }
}



internal class CreateMovieEndpoint(IMovieService movieService) : ValidatedEndpoint<CreateMovieRequest>
{
    private readonly IMovieService _movieService = movieService;

    public override void Configure()
    {
        Post("/api/movies");
        AllowAnonymous();
    }

    // Updated to match ValidatedEndpoint<TRequest> template method name
    protected override async Task OnValidatedAsync(CreateMovieRequest req, CancellationToken ct)
    {
        var movie = req.ToMovie();
        await _movieService.CreateMovieAsync(movie);

        await Send.CreatedAtAsync<GetMovieByIdEndpoint>(
            new { Id = movie.MovieId },
            movie.ToMovieResponse(),
            cancellation: ct
        );
    }
}

// ========== GET MOVIE BY ID ==========
public class GetMovieByIdRequest
{
    public Guid Id { get; set; }
}

internal class GetMovieByIdEndpoint(IMovieService movieService) : Endpoint<GetMovieByIdRequest, MovieResponse>
{
    private readonly IMovieService _movieService = movieService;

    public override void Configure()
    {
        Get("/api/movies/{Id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetMovieByIdRequest req, CancellationToken ct)
    {
        var movie = await _movieService.GetMovieByIdAsync(req.Id);
        if (movie is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await Send.OkAsync(movie.ToMovieResponse(), ct);
    }
}

// ========== LIST MOVIES ==========

internal class ListMoviesEndpoint(IMovieService movieService)
    : EndpointWithoutRequest<ListMoviesResponse>
{
    private readonly IMovieService _movieService = movieService;

    public override void Configure()
    {
        Get("/api/movies");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct = default)
    {
        var movies = await _movieService.ListMoviesAsync();

        var moviesResponse = movies.Select(movie => movie.ToMovieResponse()).ToList();

        var response = new ListMoviesResponse
        {
            Movies = moviesResponse
        };

        await Send.OkAsync(response, ct);
    }
}

// ========== UPDATE MOVIE PRICE ==========
public class UpdateMoviePriceRequest
{
    public Guid Id { get; set; }
    public decimal NewPrice { get; set; }
}

internal class UpdateMoviePriceEndpoint(IMovieService movieService) : Endpoint<UpdateMoviePriceRequest>
{
    private readonly IMovieService _movieService = movieService;

    public override void Configure()
    {
        Put("/api/movies/{Id}/price");
        AllowAnonymous();
    }

    public override async Task HandleAsync(UpdateMoviePriceRequest req, CancellationToken ct)
    {
        await _movieService.UpdateMoviePriceAsync(req.Id, req.NewPrice);
        await Send.NoContentAsync(ct);
    }
}

// ========== DELETE MOVIE ==========
public class DeleteMovieRequest
{
    public Guid Id { get; set; }
}

internal class DeleteMovieEndpoint(IMovieService movieService) : Endpoint<DeleteMovieRequest>
{
    private readonly IMovieService _movieService = movieService;

    public override void Configure()
    {
        Delete("/api/movies/{Id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(DeleteMovieRequest req, CancellationToken ct)
    {
        await _movieService.DeleteMovieAsync(req.Id);
        await Send.NoContentAsync(ct);
    }
}