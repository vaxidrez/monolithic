using CP.Portal.Users.Module.Data;

using FastEndpoints;

using Microsoft.AspNetCore.Identity;

namespace CP.Portal.Users.Module.Endpoints;

internal class CreateUserEndpoint : Endpoint<CreateUserRequest>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public CreateUserEndpoint(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public override void Configure()
    {
        Post("/api/users");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CreateUserRequest request,
      CancellationToken cancellationToken)
    {
        var newUser = new ApplicationUser
        {
            Email = request.Email,
            UserName = request.Email
        };

        await _userManager.CreateAsync(newUser, request.Password);

        await Send.OkAsync();
    }
}
