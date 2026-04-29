using FeatureHub.Application;
using FeatureHub.Infrastructure;
using FeatureHub.Infrastructure.Data;

namespace FeatureHub.Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.AddApiServices();
        builder.AddInfrastructureServices();
        builder.AddApplicationServices();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            await app.InitialiseDatabaseAsync();

            app.MapOpenApi();

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/openapi/v1.json", "v1");
            });
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
