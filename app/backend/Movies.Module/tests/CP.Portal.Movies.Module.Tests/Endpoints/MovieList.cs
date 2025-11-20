using System.Net;

using CP.Portal.Movies.Module.Endpoints;

using FastEndpoints;
using FastEndpoints.Testing;
using Shouldly;

namespace CP.Portal.Movies.Module.Tests.Endpoints;

public class MovieList(Fixture fixture) : TestBase<Fixture>
{
    [Fact]
    public async Task Get_Should_Return_Movies_Async()
    {
        var response = await fixture.Client.GETAsync<ListMoviesEndpoint, ListMoviesResponse>();
        response.Response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}
