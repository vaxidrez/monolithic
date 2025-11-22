using System.Security.Claims;

using Core.MediatOR.Contracts;

using CP.Portal.Users.Module.UseCases;

using FastEndpoints;

namespace CP.Portal.Users.Module.Endpoints;

internal class AddMovie : Endpoint<AddCartMovieRequest>
{
    private readonly IMediatOR _mediator;

    public AddMovie(IMediatOR mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/api/cart");
        AllowAnonymous();
        //Claims("EmailAddress");
    }

    public override async Task HandleAsync(AddCartMovieRequest request,
      CancellationToken cancellationToken)
    {
        var emailAddress = User.FindFirstValue("EmailAddress") ?? "vaxi.drez@gmail.com";
        
        var command = new AddMovieToCartCommand(request.MovieId, request.Quantity, emailAddress!);

        var result = await _mediator!.Send(command, cancellationToken);

        await Send.OkAsync();


        //if (result.Status == ResultStatus.Unauthorized)
        //{
        //    await SendUnauthorizedAsync();
        //}
        //else
        //{
        //    await SendOkAsync();
        //}
    }
}
