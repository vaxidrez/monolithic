namespace CP.Portal.Users.Module.Endpoints;

public record AddCartMovieRequest(Guid MovieId, int Quantity);