using Microsoft.EntityFrameworkCore;

namespace CP.Portal.Movies.Module.Data.Seedings;

internal static class MovieCastsAsyncSeeder
{
    public static async Task SeedAsync(MovieDbContext db, IReadOnlyDictionary<string, Guid> movies, CancellationToken ct)
    {
        if (await db.MovieCasts.AnyAsync(ct))
        {
            return;
        }

        var matrixId = movies["The Matrix"];
        var inceptionId = movies["Inception"];

        await db.MovieCasts.AddRangeAsync(
        [
            new MovieCast { MovieId = matrixId,    PersonId = SeedConstants.PersonKeanu, CharacterName = "Neo",     CastOrder = 1 },
            new MovieCast { MovieId = matrixId,    PersonId = SeedConstants.PersonMoss,  CharacterName = "Trinity", CastOrder = 2 },
            new MovieCast { MovieId = inceptionId, PersonId = SeedConstants.PersonLeo,   CharacterName = "Cobb",    CastOrder = 1 },
        ], ct);

        await db.SaveChangesAsync(ct);
    }
}
