using System.ComponentModel.DataAnnotations.Schema;

namespace CP.Portal.Movies.Module.Data;

public interface IReadOnlyMovieRepository
{
    public Task<Movie?> GetByIdAsync(Guid id);
    public Task<List<Movie>> ListAsync();
}

public class Movie
{
    public Guid MovieId { get; private set; } = Guid.CreateVersion7();
    public string Title { get; private set; } = string.Empty;
    public string? OriginalTitle { get; private set; }
    public string? Synopsis { get; private set; }
    public DateOnly ReleaseYear { get; private set; }
    public int DurationMinutes { get; private set; }
    public string Language { get; private set; } = null!;
    public decimal RentalPrice { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public ICollection<MovieGenre> MovieGenres { get; } = new List<MovieGenre>();
    public ICollection<MovieCast> Cast { get; } = new List<MovieCast>();
    public ICollection<MovieCrew> Crew { get; } = new List<MovieCrew>();

    // Mark derived convenience projections as not mapped to prevent EF from inferring relationships
    [NotMapped]
    public IEnumerable<Genre> Genres => MovieGenres.Select(mg => mg.Genre);

    [NotMapped]
    public IEnumerable<Person> CastPeople => Cast.Select(c => c.Person);

    [NotMapped]
    public IEnumerable<Person> CrewPeople => Crew.Select(c => c.Person);

    public Movie(
        string title,
        DateOnly releaseYear,
        int durationMinutes,
        string language,
        decimal rentalPrice,
        string? originalTitle = null,
        string? synopsis = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be null, empty or whitespace.", nameof(title));
        if (originalTitle is not null && string.IsNullOrWhiteSpace(originalTitle))
            throw new ArgumentException("OriginalTitle, when provided, cannot be empty or whitespace.", nameof(originalTitle));
        if (synopsis is not null && synopsis.Length > 4000)
            throw new ArgumentException("Synopsis exceeds maximum length (4000).", nameof(synopsis));

        var year = releaseYear.Year;
        var currentYear = DateTime.UtcNow.Year;
        if (year < 1888 || year > currentYear + 2)
            throw new ArgumentOutOfRangeException(nameof(releaseYear), $"ReleaseYear must be between 1888 and {currentYear + 2}.");
        if (durationMinutes <= 0)
            throw new ArgumentOutOfRangeException(nameof(durationMinutes), "DurationMinutes must be greater than 0.");
        if (string.IsNullOrWhiteSpace(language))
            throw new ArgumentException("Language cannot be null, empty or whitespace.", nameof(language));
        if (rentalPrice < 0m)
            throw new ArgumentOutOfRangeException(nameof(rentalPrice), "RentalPrice cannot be negative.");

        Title = title.Trim();
        OriginalTitle = originalTitle?.Trim();
        Synopsis = synopsis?.Trim();
        ReleaseYear = releaseYear;
        DurationMinutes = durationMinutes;
        Language = language.Trim();
        RentalPrice = rentalPrice;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdatePrice(decimal newPrice)
    {
        if (newPrice < 0m)
            throw new ArgumentOutOfRangeException(nameof(newPrice), "New rental price cannot be negative.");
        if (newPrice == RentalPrice)
            return;
        RentalPrice = newPrice;
    }

    public void AddGenre(Genre genre)
    {
        if (genre is null) throw new ArgumentNullException(nameof(genre));
        if (MovieGenres.Any(mg => mg.GenreId == genre.GenreId))
            return;

        MovieGenres.Add(new MovieGenre
        {
            Movie = this,
            MovieId = MovieId,
            Genre = genre,
            GenreId = genre.GenreId
        });
        genre.MovieGenres.Add(MovieGenres.Last());
    }

    public void RemoveGenre(Guid genreId)
    {
        var link = MovieGenres.FirstOrDefault(mg => mg.GenreId == genreId);
        if (link is null) return;
        MovieGenres.Remove(link);
        link.Genre.MovieGenres.Remove(link);
    }

    public void AddCast(Person person, string characterName, int castOrder)
    {
        if (person is null) throw new ArgumentNullException(nameof(person));
        if (string.IsNullOrWhiteSpace(characterName))
            throw new ArgumentException("Character name cannot be empty.", nameof(characterName));
        if (castOrder < 0)
            throw new ArgumentOutOfRangeException(nameof(castOrder), "Cast order must be >= 0.");

        if (Cast.Any(c => c.PersonId == person.PersonId && c.CharacterName.Equals(characterName, StringComparison.OrdinalIgnoreCase)))
            return;

        var entry = new MovieCast
        {
            Movie = this,
            MovieId = MovieId,
            Person = person,
            PersonId = person.PersonId,
            CharacterName = characterName.Trim(),
            CastOrder = castOrder
        };
        Cast.Add(entry);
        person.CastCredits.Add(entry);
    }

    public void RemoveCast(Guid personId, string? characterName = null)
    {
        var matches = Cast.Where(c => c.PersonId == personId &&
            (characterName == null || c.CharacterName.Equals(characterName, StringComparison.OrdinalIgnoreCase)))
            .ToList();
        if (matches.Count == 0) return;
        foreach (var m in matches)
        {
            Cast.Remove(m);
            m.Person.CastCredits.Remove(m);
        }
    }

    public void AddCrew(Person person, string role)
    {
        if (person is null) throw new ArgumentNullException(nameof(person));
        if (string.IsNullOrWhiteSpace(role))
            throw new ArgumentException("Role cannot be empty.", nameof(role));

        if (Crew.Any(c => c.PersonId == person.PersonId && c.Role.Equals(role, StringComparison.OrdinalIgnoreCase)))
            return;

        var entry = new MovieCrew
        {
            Movie = this,
            MovieId = MovieId,
            Person = person,
            PersonId = person.PersonId,
            Role = role.Trim()
        };
        Crew.Add(entry);
        person.CrewCredits.Add(entry);
    }

    public void RemoveCrew(Guid personId, string? role = null)
    {
        var matches = Crew.Where(c => c.PersonId == personId &&
            (role == null || c.Role.Equals(role, StringComparison.OrdinalIgnoreCase)))
            .ToList();
        if (matches.Count == 0) return;
        foreach (var m in matches)
        {
            Crew.Remove(m);
            m.Person.CrewCredits.Remove(m);
        }
    }
}
public class Genre
{
    public Guid GenreId { get; set; } = Guid.CreateVersion7();
    public string Name { get; set; } = null!;
    public ICollection<MovieGenre> MovieGenres { get; } = new List<MovieGenre>();
}

public class Person
{
    public Guid PersonId { get; set; } = Guid.CreateVersion7();
    public string FullName { get; set; } = null!;
    public DateTime BirthDate { get; set; }
    public string? Bio { get; set; }

    public ICollection<MovieCast> CastCredits { get; } = new List<MovieCast>();
    public ICollection<MovieCrew> CrewCredits { get; } = new List<MovieCrew>();
}

public class MovieGenre
{
    public Guid MovieId { get; set; }
    public Guid GenreId { get; set; }

    public Movie Movie { get; set; } = null!;
    public Genre Genre { get; set; } = null!;
}

public class MovieCast
{
    public Guid MovieId { get; set; }
    public Guid PersonId { get; set; }
    public string CharacterName { get; set; } = null!;
    public int CastOrder { get; set; }

    public Movie Movie { get; set; } = null!;
    public Person Person { get; set; } = null!;
}

public class MovieCrew
{
    public Guid MovieId { get; set; }
    public Guid PersonId { get; set; }
    public string Role { get; set; } = null!; // Director, Writer, Producer...

    public Movie Movie { get; set; } = null!;
    public Person Person { get; set; } = null!;
}