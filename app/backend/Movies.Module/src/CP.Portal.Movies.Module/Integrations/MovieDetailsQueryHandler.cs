using Core.MediatOR.Contracts;
using CP.Core.Contracts.MovieDetails;
using CP.Portal.Movies.Module.Services;

namespace CP.Portal.Movies.Module.Integrations;

internal class MovieDetailsQueryHandler : IRequestHandler<MovieDetailsQuery, MovieDetailsResponse?>
{
    private readonly IMovieService _movieService;

    public MovieDetailsQueryHandler(IMovieService movieService)
    {
        _movieService = movieService;
    }

    public async Task<MovieDetailsResponse?> Handle(MovieDetailsQuery request, CancellationToken cancellationToken)
    {
        var movie = await _movieService.GetMovieByIdAsync(request.MovieId);

        if (movie is null)
        {
            return null!;
        }

        var response = new MovieDetailsResponse(movie.MovieId, movie.Title, movie.Synopsis ?? string.Empty, movie.RentalPrice);
        return response;
    }
}
