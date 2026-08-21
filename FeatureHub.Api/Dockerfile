FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

COPY FeatureHub.Api/FeatureHub.Api.csproj FeatureHub.Api/
COPY FeatureHub.Application/FeatureHub.Application.csproj FeatureHub.Application/
COPY FeatureHub.Domain/FeatureHub.Domain.csproj FeatureHub.Domain/
COPY FeatureHub.Infrastructure/FeatureHub.Infrastructure.csproj FeatureHub.Infrastructure/

RUN dotnet restore FeatureHub.Api/FeatureHub.Api.csproj

COPY FeatureHub.Api/. FeatureHub.Api/
COPY FeatureHub.Application/. FeatureHub.Application/
COPY FeatureHub.Domain/. FeatureHub.Domain/
COPY FeatureHub.Infrastructure/. FeatureHub.Infrastructure/

RUN dotnet publish -c Release -o out --no-restore FeatureHub.Api/FeatureHub.Api.csproj

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

COPY --from=build /app/out .

ENTRYPOINT ["dotnet", "FeatureHub.Api.dll"]