using Core.MediatOR.Contracts;
using Core.Results;

namespace CP.Portal.Users.Module.UseCases;

public record AddMovieToCartCommand(Guid MovieId, int Quantity, string EmailAddress)
  : IRequest<Result>;