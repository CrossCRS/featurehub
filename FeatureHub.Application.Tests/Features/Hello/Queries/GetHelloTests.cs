using FeatureHub.Application.Features.Hello.Queries;

namespace FeatureHub.Application.Tests.Features.Hello.Queries;

public class GetHelloTests
{
    [Fact]
    public async Task GetHello_ReturnsHello()
    {
        var handler = new GetHelloQueryHandler();
        var query = new GetHelloQuery();

        var result = await handler.ExecuteAsync(query);

        Assert.NotNull(result);
        Assert.Equal("Hello from Darker!", result);
    }
}
