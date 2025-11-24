using System.Reflection;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CP.Portal.Movies.Module.Data;

public class MovieDbContext : DbContext
{
    public MovieDbContext(DbContextOptions<MovieDbContext> options) : base(options) { }

    internal DbSet<Movie> Movies { get; set; } = null!;
    internal DbSet<Genre> Genres { get; set; } = null!;
    internal DbSet<Person> People { get; set; } = null!;
    internal DbSet<MovieGenre> MovieGenres { get; set; } = null!;
    internal DbSet<MovieCast> MovieCasts { get; set; } = null!;
    internal DbSet<MovieCrew> MovieCrews { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("movies");
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        SeedGenres(modelBuilder);
        SeedPeople(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<decimal>().HavePrecision(18, 6);
    }

    private static void SeedGenres(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Genre>().HasData(
            new Genre { GenreId = Guid.Parse("00000000-0000-0000-0000-000000000001"), Name = "Action" },
            new Genre { GenreId = Guid.Parse("00000000-0000-0000-0000-000000000002"), Name = "Drama" },
            new Genre { GenreId = Guid.Parse("00000000-0000-0000-0000-000000000003"), Name = "Comedy" },
            new Genre { GenreId = Guid.Parse("00000000-0000-0000-0000-000000000004"), Name = "Sci-Fi" },
            new Genre { GenreId = Guid.Parse("00000000-0000-0000-0000-000000000005"), Name = "Thriller" },
            new Genre { GenreId = Guid.Parse("00000000-0000-0000-0000-000000000006"), Name = "Fantasy" },
            new Genre { GenreId = Guid.Parse("00000000-0000-0000-0000-000000000007"), Name = "Horror" }
        );
    }

    private static void SeedPeople(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Person>().HasData(
            new Person { PersonId = Guid.Parse("00000000-0000-0000-0000-000000000001"), FullName = "Christopher Nolan", BirthDate = new DateTime(1970, 7, 30, 0, 0, 0, DateTimeKind.Utc), Bio = "Director." },
            new Person { PersonId = Guid.Parse("00000000-0000-0000-0000-000000000002"), FullName = "Keanu Reeves", BirthDate = new DateTime(1964, 9, 2, 0, 0, 0, DateTimeKind.Utc), Bio = "Actor." },
            new Person { PersonId = Guid.Parse("00000000-0000-0000-0000-000000000003"), FullName = "Carrie-Anne Moss", BirthDate = new DateTime(1967, 8, 21, 0, 0, 0, DateTimeKind.Utc), Bio = "Actress." },
            new Person { PersonId = Guid.Parse("00000000-0000-0000-0000-000000000004"), FullName = "Leonardo DiCaprio", BirthDate = new DateTime(1974, 11, 11, 0, 0, 0, DateTimeKind.Utc), Bio = "Actor." },
            new Person { PersonId = Guid.Parse("00000000-0000-0000-0000-000000000005"), FullName = "Hans Zimmer", BirthDate = new DateTime(1957, 9, 12, 0, 0, 0, DateTimeKind.Utc), Bio = "Composer." },
            new Person { PersonId = Guid.Parse("00000000-0000-0000-0000-000000000006"), FullName = "Quentin Tarantino", BirthDate = new DateTime(1963, 3, 27, 0, 0, 0, DateTimeKind.Utc), Bio = "Director." },
            new Person { PersonId = Guid.Parse("00000000-0000-0000-0000-000000000007"), FullName = "James Cameron", BirthDate = new DateTime(1954, 8, 16, 0, 0, 0, DateTimeKind.Utc), Bio = "Director." }
        );
    }
}

internal class MovieConfiguration : IEntityTypeConfiguration<Movie>
{
    public void Configure(EntityTypeBuilder<Movie> builder)
    {
        builder.ToTable("movies", "movies");
        builder.HasKey(p => p.MovieId);
        builder.Property(p => p.MovieId).ValueGeneratedNever();

        builder.Property(p => p.Title)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH)
            .IsRequired();

        builder.Property(p => p.OriginalTitle)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH);

        builder.Property(p => p.Synopsis)
            .HasMaxLength(4000);

        builder.Property(p => p.Language)
            .IsRequired();

        builder.HasMany(m => m.MovieGenres)
            .WithOne(mg => mg.Movie)
            .HasForeignKey(mg => mg.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(m => m.Cast)
            .WithOne(mc => mc.Movie)
            .HasForeignKey(mc => mc.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(m => m.Crew)
            .WithOne(mc => mc.Movie)
            .HasForeignKey(mc => mc.MovieId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

internal class GenreConfiguration : IEntityTypeConfiguration<Genre>
{
    public void Configure(EntityTypeBuilder<Genre> builder)
    {
        builder.ToTable("genre", "movies");
        builder.HasKey(g => g.GenreId);
        builder.Property(g => g.GenreId).ValueGeneratedNever();

        builder.Property(g => g.Name)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH)
            .IsRequired();

        builder.HasMany(g => g.MovieGenres)
            .WithOne(mg => mg.Genre)
            .HasForeignKey(mg => mg.GenreId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

internal class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.ToTable("person", "movies");
        builder.HasKey(p => p.PersonId);
        builder.Property(p => p.PersonId).ValueGeneratedNever();

        builder.Property(p => p.FullName)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH)
            .IsRequired();

        builder.Property(p => p.Bio)
            .HasMaxLength(4000);

        builder.HasMany(p => p.CastCredits)
            .WithOne(mc => mc.Person)
            .HasForeignKey(mc => mc.PersonId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.CrewCredits)
            .WithOne(mc => mc.Person)
            .HasForeignKey(mc => mc.PersonId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

internal class MovieGenreConfiguration : IEntityTypeConfiguration<MovieGenre>
{
    public void Configure(EntityTypeBuilder<MovieGenre> builder)
    {
        builder.ToTable("movies_genres", "movies");
        builder.HasKey(mg => new { mg.MovieId, mg.GenreId });

        builder.HasOne(mg => mg.Movie)
            .WithMany(m => m.MovieGenres)
            .HasForeignKey(mg => mg.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(mg => mg.Genre)
            .WithMany(g => g.MovieGenres)
            .HasForeignKey(mg => mg.GenreId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(mg => mg.GenreId);
        builder.HasIndex(mg => mg.MovieId);
    }
}

internal class MovieCastConfiguration : IEntityTypeConfiguration<MovieCast>
{
    public void Configure(EntityTypeBuilder<MovieCast> builder)
    {
        builder.ToTable("movies_cast", "movies");
        builder.Property(mc => mc.CharacterName)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH)
            .IsRequired();

        builder.HasKey(mc => new { mc.MovieId, mc.PersonId, mc.CharacterName });

        builder.HasOne(mc => mc.Movie)
            .WithMany(m => m.Cast)
            .HasForeignKey(mc => mc.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(mc => mc.Person)
            .WithMany(p => p.CastCredits)
            .HasForeignKey(mc => mc.PersonId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(mc => new { mc.MovieId, mc.CastOrder });
        builder.Property(mc => mc.CastOrder).IsRequired();
    }
}

internal class MovieCrewConfiguration : IEntityTypeConfiguration<MovieCrew>
{
    public void Configure(EntityTypeBuilder<MovieCrew> builder)
    {
        builder.ToTable("movies_crew", "movies");
        builder.Property(mc => mc.Role)
            .HasMaxLength(DataSchemaConstants.DEFAULT_NAME_LENGTH)
            .IsRequired();

        builder.HasKey(mc => new { mc.MovieId, mc.PersonId, mc.Role });

        builder.HasOne(mc => mc.Movie)
            .WithMany(m => m.Crew)
            .HasForeignKey(mc => mc.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(mc => mc.Person)
            .WithMany(p => p.CrewCredits)
            .HasForeignKey(mc => mc.PersonId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(mc => mc.Role);
    }
}

public static class DataSchemaConstants
{
    public const int DEFAULT_NAME_LENGTH = 100;
}