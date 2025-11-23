using Core.MediatOR;
using Core.MediatOR.Contracts;

using CP.Portal.Users.Module.Data;
using CP.Portal.Users.Module.Data.Repositories;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Serilog;

namespace CP.Portal.Users.Module;

public static class UsersModuleExtensions
{
    public static IServiceCollection AddUserModuleServices(
      this IServiceCollection services,
      ConfigurationManager config,
      ILogger logger,
      List<System.Reflection.Assembly> mediatRAssemblies)
    {

        string? connectionString = config.GetConnectionString("UsersConnectionString");


        services.AddDbContext<UserDbContext>(opt =>
        {
            opt.UseNpgsql(connectionString)
               .UseSnakeCaseNamingConvention()
               .UseAsyncSeeding(async (db, isFirstRun, ct) =>
               {
                   var ctx = (UserDbContext)db;

                   if (!await ctx.ApplicationUsers.AnyAsync(ct))
                   {
                       const string pwd = "NetUniversity@90";
                       var hasher = new PasswordHasher<ApplicationUser>();

                       var user1 = new ApplicationUser
                       {
                           UserName = "vaxi.drez",
                           Email = "vaxi.drez@gmail.com",
                           FullName = "Vaxi Drez",
                           NormalizedEmail = "VAXI.DREZ@GMAIL.COM",
                           NormalizedUserName = "VAXI.DREZ",
                           SecurityStamp = Guid.NewGuid().ToString(),
                           ConcurrencyStamp = Guid.NewGuid().ToString()
                       };
                       user1.PasswordHash = hasher.HashPassword(user1, pwd);

                       var user2 = new ApplicationUser
                       {
                           UserName = "juan.perez",
                           Email = "juan.perez@gmail.com",
                           FullName = "Juan Perez",
                           NormalizedEmail = "JUAN.PEREZ@GMAIL.COM",
                           NormalizedUserName = "JUAN.PEREZ",
                           SecurityStamp = Guid.NewGuid().ToString(),
                           ConcurrencyStamp = Guid.NewGuid().ToString()
                       };
                       user2.PasswordHash = hasher.HashPassword(user2, pwd);

                       await ctx.ApplicationUsers.AddRangeAsync([user1, user2], ct);
                       await ctx.SaveChangesAsync(ct);
                   }
               });
        });

        services.AddIdentityCore<ApplicationUser>()
            .AddEntityFrameworkStores<UserDbContext>();

        // Add User Services
        services.AddScoped<IApplicationUserRepository, EfApplicationUserRepository>();


        // if using MediatR in this module, add any assemblies that contain handlers to the list
        mediatRAssemblies.Add(typeof(UsersModuleExtensions).Assembly);
        services.AddMediatOR(typeof(UsersModuleExtensions).Assembly);

        logger.Information("{Module} module services registered", "Users");

        return services;
    }
}

