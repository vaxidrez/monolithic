using FastEndpoints.Testing;

namespace CP.Portal.Movies.Module.Tests.Endpoints;

public class Fixture : AppFixture<Program>
{
    public HttpClient Client { get; private set; } = default!;

    protected override ValueTask SetupAsync()
    {
        Client = CreateClient();
        // return Task.CompletedTask;
        return ValueTask.CompletedTask;
    }


    protected override ValueTask TearDownAsync()
    {
        Client.Dispose();
        return base.TearDownAsync();
    }
}
