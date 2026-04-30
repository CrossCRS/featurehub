using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Paramore.Brighter.Extensions.DependencyInjection;
using Paramore.Darker.AspNetCore;
using Paramore.Darker.Policies;
using System.Reflection;

namespace FeatureHub.Application;

public static class DependencyInjection
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddBrighter()
            .AutoFromAssemblies([Assembly.GetExecutingAssembly()])
            .ValidatePipelines()
            .DescribePipelines();

        builder.Services.AddDarker(options =>
        {
            options.QueryProcessorLifetime = ServiceLifetime.Scoped;
        })
            .AddHandlersFromAssemblies([Assembly.GetExecutingAssembly()])
            .AddDefaultPolicies();
    }
}
