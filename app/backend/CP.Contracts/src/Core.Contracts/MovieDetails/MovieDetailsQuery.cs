using Core.MediatOR.Contracts;

namespace CP.Core.Contracts.MovieDetails;

public record MovieDetailsQuery(Guid MovieId) : IRequest<MovieDetailsResponse?>;

public record MovieDetailsResponse(Guid MovieId, string Title, string Description, decimal Price);
