using Core.MediatOR.Contracts;

namespace CP.Portal.Users.Module.UseCases;

public record AddMovieToCartCommand(Guid MovieId, int Quantity, string EmailAddress)
  : IRequest<Guid?>;