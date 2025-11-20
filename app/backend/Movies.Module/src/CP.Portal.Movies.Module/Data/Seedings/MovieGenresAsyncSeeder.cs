using Microsoft.EntityFrameworkCore;

namespace CP.Portal.Movies.Module.Data.Seedings;

internal static class MovieGenresAsyncSeeder
{
    public static async Task SeedAsync(MovieDbContext db, IReadOnlyDictionary<string, Guid> movies, CancellationToken ct)
    {
        // Idempotente: si ya hay vínculos para Matrix, asumimos sembrado
        if (await db.MovieGenres.AnyAsync(ct))
            return;

        var matrixId = movies["The Matrix"];
        var inceptionId = movies["Inception"];
        var avatarId = movies["Avatar"];

        await db.MovieGenres.AddRangeAsync(
        [
            // Matrix -> Sci-Fi, Action
            new MovieGenre { MovieId = matrixId,   GenreId = SeedConstants.GenreSciFi },
            new MovieGenre { MovieId = matrixId,   GenreId = SeedConstants.GenreAction },

            // Inception -> Sci-Fi, Thriller
            new MovieGenre { MovieId = inceptionId, GenreId = SeedConstants.GenreSciFi },
            new MovieGenre { MovieId = inceptionId, GenreId = SeedConstants.GenreThriller },

            // Avatar -> Sci-Fi, Fantasy
            new MovieGenre { MovieId = avatarId,   GenreId = SeedConstants.GenreSciFi },
            new MovieGenre { MovieId = avatarId,   GenreId = SeedConstants.GenreFantasy },
        ], ct);

        await db.SaveChangesAsync(ct);
    }
}
