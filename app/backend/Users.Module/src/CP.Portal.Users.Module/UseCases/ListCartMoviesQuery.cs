using Core.MediatOR.Contracts;

using CP.Core.Contracts.Core;
using CP.Portal.Users.Module.Endpoints;

namespace CP.Portal.Users.Module.UseCases;


public record ListCartMoviesQuery(string EmailAddress) : IRequest<List<CartMovieResponse>>;
