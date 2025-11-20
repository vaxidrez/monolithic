namespace CP.Portal.Movies.Module.Data.Repositories;

public interface IMovieRepository : IReadOnlyMovieRepository
{ 
    public Task AddAsync(Movie movie);
    public Task DeleteAsync(Movie movie);
    public Task SaveChangesAsync();
}
