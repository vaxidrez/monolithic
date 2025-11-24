using Core.MediatOR.Contracts;
using Core.Results;

namespace CP.Core.Contracts.MovieDetails;

public record MovieDetailsQuery(Guid MovieId) : IRequest<Result<MovieDetailsResponse?>>;

public record MovieDetailsResponse(Guid MovieId, string Title, string Description, decimal Price);
