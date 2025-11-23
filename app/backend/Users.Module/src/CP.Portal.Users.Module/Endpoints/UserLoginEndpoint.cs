using System.IdentityModel.Tokens.Jwt;

using CP.Portal.Users.Module.Data;

using FastEndpoints;
using FastEndpoints.Security;

using Microsoft.AspNetCore.Identity;

namespace CP.Portal.Users.Module.Endpoints;

internal class UserLoginEndpoint : Endpoint<UserLoginRequest>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserLoginEndpoint(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }
    public override void Configure()
    {
        Post("/users/login");
        AllowAnonymous();
    }

    public override async Task HandleAsync(UserLoginRequest request,
      CancellationToken ct)
    {
        var user = await _userManager.FindByEmailAsync(request.Email!);
        if (user == null)
        {
            await Send.UnauthorizedAsync();
            return;
        }
        var loginSuccessful = await _userManager.CheckPasswordAsync(user,
                          request.Password);

        if (!loginSuccessful)
        {
            await Send.UnauthorizedAsync();
            return;
        }

        var jwtSecret = Config["Auth:JwtSecret"]!;

        var token = JwtBearer.CreateToken(options =>
        {
            options.SigningKey = jwtSecret;
            options.ExpireAt = DateTime.UtcNow.AddHours(500);
            options.User["sub"] = user.Id;
            options.User["email"] = user.Email!;
            options.User["name"] = user.FullName;
            options.User["EmailAddress"] = user.Email!;
        });

        await Send.OkAsync(token);
    }

}
