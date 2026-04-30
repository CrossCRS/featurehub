using Paramore.Darker;

namespace FeatureHub.Application.Features.Hello.Queries;

public sealed class GetHelloQuery : IQuery<string>
{
}

public sealed class GetHelloQueryHandler : QueryHandlerAsync<GetHelloQuery, string>
{
    public override async Task<string> ExecuteAsync(GetHelloQuery query, CancellationToken cancellationToken = default)
    {
        await Task.Yield();
        return "Hello from Darker!";
    }
}
