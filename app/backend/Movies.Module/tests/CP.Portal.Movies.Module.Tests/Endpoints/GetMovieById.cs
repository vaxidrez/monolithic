using System.Net;

using CP.Portal.Movies.Module.Endpoints;

using FastEndpoints;
using FastEndpoints.Testing;

using Shouldly;

namespace CP.Portal.Movies.Module.Tests.Endpoints;

public class GetMovieById(Fixture fixture) : TestBase<Fixture>
{

    [Theory]
    [InlineData("11111111-1111-1111-1111-111111111111", "Title A")]
    [InlineData("22222222-2222-2222-2222-222222222222", "Title B")]
    [InlineData("33333333-3333-3333-3333-333333333333", "Title C")]
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