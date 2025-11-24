using Core.MediatOR.Contracts;
using Core.Results;

using CP.Core.Contracts.MovieDetails;
using CP.Portal.Users.Module.Data;
using CP.Portal.Users.Module.Data.Repositories;

namespace CP.Portal.Users.Module.UseCases;

public class AddMovieToCartHandler : IRequestHandler<AddMovieToCartCommand, Result>
{
    private readonly IApplicationUserRepository _userRepository;
    private readonly IMediatOR _mediator;

    public AddMovieToCartHandler(IApplicationUserRepository userRepository,
      IMediatOR mediator)
    {
        _userRepository = userRepository;
        _mediator = mediator;
    }

    public async Task<Result> Handle(AddMovieToCartCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserWithCartByEmailAsync(request.EmailAddress);

        if (user is null)
        {
            return Result.Unauthorized();
        }
        var query = new MovieDetailsQuery(request.MovieId);

        var result = await _mediator.Send(query, cancellationToken);

        if (result.Status == ResultStatus.NotFound) return Result.NotFound();

        var movieDetails = result.Value;

        var newCartItem = new CartMovie(request.MovieId,
          movieDetails!.Description,
          request.Quantity,
          movieDetails.Price);

        user.AddItemToCart(newCartItem);

        await _userRepository.SaveChangesAsync();

        return Result.Success();
    }
}
