using Core.MediatOR.Contracts;

using CP.Portal.Users.Module.Data.Repositories;
using CP.Portal.Users.Module.Endpoints.CartEndpoints;

namespace CP.Portal.Users.Module.UseCases;

internal class ListCartItemsQueryHandler : IRequestHandler<ListCartMoviesQuery,
  List<CartMovieResponse>>
{
    private readonly IApplicationUserRepository _userRepository;

    public ListCartItemsQueryHandler(IApplicationUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<List<CartMovieResponse>> Handle(ListCartMoviesQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserWithCartByEmailAsync(request.EmailAddress);

        if (user is null)
        {
            return [];
        }

        return user.CartItems
          .Select(item => new CartMovieResponse(item.Id, item.MovieId,
          item.Description, item.Quantity, item.UnitPrice))
          .ToList();
    }
}

