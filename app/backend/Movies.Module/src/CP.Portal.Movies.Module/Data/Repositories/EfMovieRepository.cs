using Microsoft.EntityFrameworkCore;

namespace CP.Portal.Movies.Module.Data.Repositories;

public class EfMovieRepository : IMovieRepository
{
    private readonly MovieDbContext _dbContext;

    public EfMovieRepository(MovieDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task AddAsync(Movie movie)
    {
        _dbContext.Add(movie);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Movie movie)
    { 
        _dbContext.Remove(movie);
        return Task.CompletedTask;
    }

    public async Task<Movie?> GetByIdAsync(Guid id)
    { 
        return await _dbContext.Movies.FindAsync(id);
    }

    public async Task<List<Movie>> ListAsync()
    { 
        return await _dbContext.Movies.ToListAsync();
    }
    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}
