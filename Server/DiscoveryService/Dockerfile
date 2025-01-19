FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY ./src ./src

WORKDIR /app/src

RUN dotnet restore DiscoveryService.sln

WORKDIR /app/src/DiscoveryService.Api
RUN dotnet build "DiscoveryService.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DiscoveryService.Api.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "DiscoveryService.Api.dll"]