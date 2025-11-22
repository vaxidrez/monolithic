using CP.Portal.Users.Module.Data.Repositories;

using Microsoft.EntityFrameworkCore;

namespace CP.Portal.Users.Module.Data;

internal class EfApplicationUserRepository : IApplicationUserRepository
{
    private readonly UserDbContext _dbContext;

    public EfApplicationUserRepository(UserDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<ApplicationUser?> GetUserWithCartByEmailAsync(string email)
    {
        return _dbContext.ApplicationUsers
          .Include(user => user.CartItems)
          .SingleAsync(user => user.Email == email)!;
    }

    public Task SaveChangesAsync()
    {
        return _dbContext.SaveChangesAsync();
    }
}

