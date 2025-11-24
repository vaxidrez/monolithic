using CP.Portal.Movies.Module.Data;

namespace CP.Portal.Movies.Module.Services;

internal interface IMovieService
{
    Task<List<Movie>> ListMoviesAsync();
    Task<Movie?> GetMovieByIdAsync(Guid id);
    Task CreateMovieAsync(Movie newMovie);
    Task DeleteMovieAsync(Guid id);
    Task UpdateMoviePriceAsync(Guid id, decimal newPrice);

}
