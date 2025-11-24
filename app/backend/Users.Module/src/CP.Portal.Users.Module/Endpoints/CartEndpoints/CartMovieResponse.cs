namespace CP.Portal.Users.Module.Endpoints.CartEndpoints;

public record CartMovieResponse(Guid Id, Guid MovieId, string Description,
                        int Quantity, decimal UnitPrice);
