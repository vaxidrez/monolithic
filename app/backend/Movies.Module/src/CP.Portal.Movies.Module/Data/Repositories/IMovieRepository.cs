namespace CP.Portal.Movies.Module.Data.Repositories;

internal interface IMovieRepository : IReadOnlyMovieRepository
{ 
    Task AddAsync(Movie movie);
    Task DeleteAsync(Movie movie);
    Task SaveChangesAsync();
}
