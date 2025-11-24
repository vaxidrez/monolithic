namespace CP.Portal.Users.Module.Endpoints.CartEndpoints;

public record AddCartMovieRequest(Guid MovieId, int Quantity);