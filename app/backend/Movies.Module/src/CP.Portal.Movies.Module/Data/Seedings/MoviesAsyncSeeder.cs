using Microsoft.EntityFrameworkCore;

namespace CP.Portal.Movies.Module.Data.Seedings;

internal static class MoviesAsyncSeeder
{
    // Devuelve un mapa Title -> MovieId para que las demás semillas relacionen por título
    public static async Task<Dictionary<string, Guid>> SeedAsync(MovieDbContext db, CancellationToken ct)
    {
        var map = new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);

        if (await db.Movies.AnyAsync(ct))
        {
            var existing = await db.Movies
                .Select(m => new { m.Title, m.MovieId })
                .ToListAsync(ct);

            foreach (var e in existing)
                map[e.Title] = e.MovieId;

            return map;
        }

        var m1 = new Movie("The Matrix", new DateOnly(1999, 1, 1), 136, "en", 12.34m, synopsis: "A hacker discovers reality is a simulation.");
        var m2 = new Movie("Inception", new DateOnly(2010, 1, 1), 148, "en", 15.99m, synopsis: "A thief infiltrates dreams to plant ideas.");
        var m3 = new Movie("Avatar", new DateOnly(2009, 1, 1), 162, "en", 10.50m, synopsis: "A marine on an alien planet.");

        await db.Movies.AddRangeAsync([m1, m2, m3], ct);
        await db.SaveChangesAsync(ct);

        map["The Matrix"] = m1.MovieId;
        map["Inception"] = m2.MovieId;
        map["Avatar"] = m3.MovieId;

        return map;
    }
}
