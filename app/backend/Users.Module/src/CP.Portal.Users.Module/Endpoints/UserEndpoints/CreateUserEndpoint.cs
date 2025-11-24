using CP.Core.Contracts.Core;
using CP.Portal.Users.Module.Data;

using FastEndpoints;

using Microsoft.AspNetCore.Identity;

namespace CP.Portal.Users.Module.Endpoints.UserEndpoints;


internal class CreateUserEndpoint : ValidatedEndpoint<CreateUserRequest>
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

    protected override async Task OnValidatedAsync(CreateUserRequest request,
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

public sealed class CreateUserRequestValidator : IValidator<CreateUserRequest>
{
    public IEnumerable<ValidationError> Validate(CreateUserRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Email))
            yield return new ValidationError(nameof(req.Email), "Email is required.");
        if (string.IsNullOrWhiteSpace(req.Password))
            yield return new ValidationError(nameof(req.Password), "Password is required.");
    }
}


//internal class CreateUserEndpoint : Endpoint<CreateUserRequest>
//{
//    private readonly UserManager<ApplicationUser> _userManager;

//    public CreateUserEndpoint(UserManager<ApplicationUser> userManager)
//    {
//        _userManager = userManager;
//    }

//    public override void Configure()
//    {
//        Post("/api/users");
//        AllowAnonymous();
//    }

//    public override async Task HandleAsync(CreateUserRequest request,
//      CancellationToken cancellationToken)
//    {
//        var newUser = new ApplicationUser
//        {
//            Email = request.Email,
//            UserName = request.Email
//        };

//        await _userManager.CreateAsync(newUser, request.Password);

//        await Send.OkAsync();
//    }
//}
