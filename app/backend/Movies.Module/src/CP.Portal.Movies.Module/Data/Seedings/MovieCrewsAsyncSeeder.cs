using Microsoft.EntityFrameworkCore;

namespace CP.Portal.Movies.Module.Data.Seedings;

internal static class MovieCrewsAsyncSeeder
{
    public static async Task SeedAsync(MovieDbContext db, IReadOnlyDictionary<string, Guid> movies, CancellationToken ct)
    {
        if (await db.MovieCrews.AnyAsync(ct))
            return;

        var matrixId = movies["The Matrix"];
        var inceptionId = movies["Inception"];
        var avatarId = movies["Avatar"];

        await db.MovieCrews.AddRangeAsync(
        [
            new MovieCrew { MovieId = matrixId,    PersonId = SeedConstants.PersonTarantino, Role = "Director" },
            new MovieCrew { MovieId = inceptionId, PersonId = SeedConstants.PersonNolan,     Role = "Director" },
            new MovieCrew { MovieId = avatarId,    PersonId = SeedConstants.PersonCameron,   Role = "Director" },
        ], ct);

        await db.SaveChangesAsync(ct);
    }
}
