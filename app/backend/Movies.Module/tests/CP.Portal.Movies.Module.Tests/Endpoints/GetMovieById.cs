using System.Net;

using CP.Portal.Movies.Module.Endpoints;

using FastEndpoints;
using FastEndpoints.Testing;

using Shouldly;

namespace CP.Portal.Movies.Module.Tests.Endpoints;

public class GetMovieById(Fixture fixture) : TestBase<Fixture>
{

    [Theory]
    [InlineData("019aa1cc-31bb-72af-9e2b-09af865e6c66", "Avatar")]
    [InlineData("019aa1cc-31bb-7f4c-9923-0baad54e87fb", "Inception")]
    [InlineData("019aa263-700f-75b9-bd6b-adbd939d08ca", "New Movie")]
    public async Task Get_With_Unknown_Id_Should_Return_404_Async(Guid id, string title)
    {
        var request = new GetMovieByIdRequest { Id = id };
        
        // Act
        var response = await fixture.Client.GETAsync<GetMovieByIdEndpoint, GetMovieByIdRequest, MovieResponse >(request);

        // Assert
        response.Response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Result.ShouldNotBeNull();
        response.Result.Title.ShouldBe(title);
    }
}