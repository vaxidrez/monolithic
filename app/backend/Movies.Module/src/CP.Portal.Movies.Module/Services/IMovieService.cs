using CP.Portal.Movies.Module.Data;
using CP.Portal.Movies.Module.Endpoints;

namespace CP.Portal.Movies.Module.Services;

public interface IMovieService
{
    public Task<List<Movie>> ListMoviesAsync();
    public Task<Movie?> GetMovieByIdAsync(Guid id);
    public Task CreateMovieAsync(Movie newMovie);
    public Task DeleteMovieAsync(Guid id);
    public Task UpdateMoviePriceAsync(Guid id, decimal newPrice);

}
