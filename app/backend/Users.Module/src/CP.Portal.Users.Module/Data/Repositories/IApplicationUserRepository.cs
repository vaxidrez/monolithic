namespace CP.Portal.Users.Module.Data.Repositories;

public interface IApplicationUserRepository
{
    public Task<ApplicationUser?> GetUserWithCartByEmailAsync(string email);
    public Task SaveChangesAsync();
}
