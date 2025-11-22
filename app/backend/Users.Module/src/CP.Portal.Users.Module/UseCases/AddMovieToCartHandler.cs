using Core.MediatOR.Contracts;

using CP.Core.Contracts.MovieDetails;
using CP.Portal.Users.Module.Data;
using CP.Portal.Users.Module.Data.Repositories;

namespace CP.Portal.Users.Module.UseCases;

public class AddMovieToCartHandler : IRequestHandler<AddMovieToCartCommand, Guid?>
{
    private readonly IApplicationUserRepository _userRepository;
    private readonly IMediatOR _mediator;

    public AddMovieToCartHandler(IApplicationUserRepository userRepository,
      IMediatOR mediator)
    {
        _userRepository = userRepository;
        _mediator = mediator;
    }

    public async Task<Guid?> Handle(AddMovieToCartCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserWithCartByEmailAsync(request.EmailAddress);

        if (user is null)
        {
            return null;
        }
        var query = new MovieDetailsQuery(request.MovieId);

        var movieDetails = await _mediator.Send(query, cancellationToken);

        if (movieDetails is null) return null;

        var newCartItem = new CartMovie(request.MovieId,
          movieDetails.Description,
          request.Quantity,
          movieDetails.Price);

        user.AddItemToCart(newCartItem);

        await _userRepository.SaveChangesAsync();
        return newCartItem.Id;
    }
}
