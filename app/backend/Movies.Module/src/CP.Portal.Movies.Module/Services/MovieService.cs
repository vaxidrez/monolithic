using CP.Portal.Movies.Module.Data;
using CP.Portal.Movies.Module.Data.Repositories;


namespace CP.Portal.Movies.Module.Services;

internal class MovieService : IMovieService
{
    private readonly IMovieRepository _movieRepository;

    public MovieService(IMovieRepository movieRepository)
    {
        _movieRepository = movieRepository;
    }

    public async Task CreateMovieAsync(Movie newMovie)
    {
        await _movieRepository.AddAsync(newMovie);
        await _movieRepository.SaveChangesAsync();
    }

    public async Task DeleteMovieAsync(Guid id)
    {
        var movieToDelete = await _movieRepository.GetByIdAsync(id);
        if (movieToDelete is not null) { 
            await _movieRepository.DeleteAsync(movieToDelete);
            await _movieRepository.SaveChangesAsync();
            
        }

    }
    public async Task<Movie?> GetMovieByIdAsync(Guid id)
    {
        return await _movieRepository.GetByIdAsync(id);
        
    }

  

    public async Task<List<Movie>> ListMoviesAsync()
    {
        //var movies = (await _movieRepository.ListAsync())
        //               .Select(movie => new MovieResponse(movie.MovieId, movie.Title, movie.Synopsis ?? movie.Synopsis!)).ToList();


        return await _movieRepository.ListAsync();
    }

    public async Task UpdateMoviePriceAsync(Guid id, decimal newPrice)
    {
        var movie = await _movieRepository.GetByIdAsync(id);
        movie!.UpdatePrice(newPrice);
        await _movieRepository.SaveChangesAsync();
    }
}
