FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY ./src ./src
COPY ./test ./test

WORKDIR /app/src

RUN dotnet restore DiscoveryService.sln --verbosity detailed

WORKDIR /app/src/DiscoveryService.Api

RUN dotnet build "DiscoveryService.Api.csproj" -c Release -o /app/build

FROM build AS publish
WORKDIR /app/src/DiscoveryService.Api

RUN dotnet publish "DiscoveryService.Api.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=publish /app/publish .

EXPOSE 80

ENTRYPOINT ["dotnet", "DiscoveryService.Api.dll"]