using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CP.Portal.Movies.Module.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialMovies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "movies");

            migrationBuilder.CreateTable(
                name: "genre",
                schema: "movies",
                columns: table => new
                {
                    genre_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_genre", x => x.genre_id);
                });

            migrationBuilder.CreateTable(
                name: "movies",
                schema: "movies",
                columns: table => new
                {
                    movie_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    original_title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    synopsis = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    release_year = table.Column<DateOnly>(type: "date", nullable: false),
                    duration_minutes = table.Column<int>(type: "integer", nullable: false),
                    language = table.Column<string>(type: "text", nullable: false),
                    rental_price = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_movies", x => x.movie_id);
                });

            migrationBuilder.CreateTable(
                name: "person",
                schema: "movies",
                columns: table => new
                {
                    person_id = table.Column<Guid>(type: "uuid", nullable: false),
                    full_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    birth_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    bio = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_person", x => x.person_id);
                });

            migrationBuilder.CreateTable(
                name: "movies_genres",
                schema: "movies",
                columns: table => new
                {
                    movie_id = table.Column<Guid>(type: "uuid", nullable: false),
                    genre_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_movies_genres", x => new { x.movie_id, x.genre_id });
                    table.ForeignKey(
                        name: "fk_movies_genres_genre_genre_id",
                        column: x => x.genre_id,
                        principalSchema: "movies",
                        principalTable: "genre",
                        principalColumn: "genre_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_movies_genres_movies_movie_id",
                        column: x => x.movie_id,
                        principalSchema: "movies",
                        principalTable: "movies",
                        principalColumn: "movie_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "movies_cast",
                schema: "movies",
                columns: table => new
                {
                    movie_id = table.Column<Guid>(type: "uuid", nullable: false),
                    person_id = table.Column<Guid>(type: "uuid", nullable: false),
                    character_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    cast_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_movies_cast", x => new { x.movie_id, x.person_id, x.character_name });
                    table.ForeignKey(
                        name: "fk_movies_cast_movies_movie_id",
                        column: x => x.movie_id,
                        principalSchema: "movies",
                        principalTable: "movies",
                        principalColumn: "movie_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_movies_cast_people_person_id",
                        column: x => x.person_id,
                        principalSchema: "movies",
                        principalTable: "person",
                        principalColumn: "person_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "movies_crew",
                schema: "movies",
                columns: table => new
                {
                    movie_id = table.Column<Guid>(type: "uuid", nullable: false),
                    person_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_movies_crew", x => new { x.movie_id, x.person_id, x.role });
                    table.ForeignKey(
                        name: "fk_movies_crew_movies_movie_id",
                        column: x => x.movie_id,
                        principalSchema: "movies",
                        principalTable: "movies",
                        principalColumn: "movie_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_movies_crew_people_person_id",
                        column: x => x.person_id,
                        principalSchema: "movies",
                        principalTable: "person",
                        principalColumn: "person_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "movies",
                table: "genre",
                columns: new[] { "genre_id", "name" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-000000000001"), "Action" },
                    { new Guid("00000000-0000-0000-0000-000000000002"), "Drama" },
                    { new Guid("00000000-0000-0000-0000-000000000003"), "Comedy" },
                    { new Guid("00000000-0000-0000-0000-000000000004"), "Sci-Fi" },
                    { new Guid("00000000-0000-0000-0000-000000000005"), "Thriller" },
                    { new Guid("00000000-0000-0000-0000-000000000006"), "Fantasy" },
                    { new Guid("00000000-0000-0000-0000-000000000007"), "Horror" }
                });

            migrationBuilder.InsertData(
                schema: "movies",
                table: "person",
                columns: new[] { "person_id", "bio", "birth_date", "full_name" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-000000000001"), "Director.", new DateTime(1970, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Christopher Nolan" },
                    { new Guid("00000000-0000-0000-0000-000000000002"), "Actor.", new DateTime(1964, 9, 2, 0, 0, 0, 0, DateTimeKind.Utc), "Keanu Reeves" },
                    { new Guid("00000000-0000-0000-0000-000000000003"), "Actress.", new DateTime(1967, 8, 21, 0, 0, 0, 0, DateTimeKind.Utc), "Carrie-Anne Moss" },
                    { new Guid("00000000-0000-0000-0000-000000000004"), "Actor.", new DateTime(1974, 11, 11, 0, 0, 0, 0, DateTimeKind.Utc), "Leonardo DiCaprio" },
                    { new Guid("00000000-0000-0000-0000-000000000005"), "Composer.", new DateTime(1957, 9, 12, 0, 0, 0, 0, DateTimeKind.Utc), "Hans Zimmer" },
                    { new Guid("00000000-0000-0000-0000-000000000006"), "Director.", new DateTime(1963, 3, 27, 0, 0, 0, 0, DateTimeKind.Utc), "Quentin Tarantino" },
                    { new Guid("00000000-0000-0000-0000-000000000007"), "Director.", new DateTime(1954, 8, 16, 0, 0, 0, 0, DateTimeKind.Utc), "James Cameron" }
                });

            migrationBuilder.CreateIndex(
                name: "ix_movies_cast_movie_id_cast_order",
                schema: "movies",
                table: "movies_cast",
                columns: new[] { "movie_id", "cast_order" });

            migrationBuilder.CreateIndex(
                name: "ix_movies_cast_person_id",
                schema: "movies",
                table: "movies_cast",
                column: "person_id");

            migrationBuilder.CreateIndex(
                name: "ix_movies_crew_person_id",
                schema: "movies",
                table: "movies_crew",
                column: "person_id");

            migrationBuilder.CreateIndex(
                name: "ix_movies_crew_role",
                schema: "movies",
                table: "movies_crew",
                column: "role");

            migrationBuilder.CreateIndex(
                name: "ix_movies_genres_genre_id",
                schema: "movies",
                table: "movies_genres",
                column: "genre_id");

            migrationBuilder.CreateIndex(
                name: "ix_movies_genres_movie_id",
                schema: "movies",
                table: "movies_genres",
                column: "movie_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "movies_cast",
                schema: "movies");

            migrationBuilder.DropTable(
                name: "movies_crew",
                schema: "movies");

            migrationBuilder.DropTable(
                name: "movies_genres",
                schema: "movies");

            migrationBuilder.DropTable(
                name: "person",
                schema: "movies");

            migrationBuilder.DropTable(
                name: "genre",
                schema: "movies");

            migrationBuilder.DropTable(
                name: "movies",
                schema: "movies");
        }
    }
}
