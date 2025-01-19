FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY ./src ./src
COPY ./test ./test

COPY ./wait-for-it.sh /app/wait-for-it.sh

WORKDIR /app/src

RUN dotnet restore UserService.sln

WORKDIR /app/src/UserService.Api
RUN dotnet build "UserService.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UserService.Api.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=publish /app/publish .
COPY ./wait-for-it.sh /app/wait-for-it.sh

RUN chmod +x /app/wait-for-it.sh

EXPOSE 80

ENTRYPOINT ["/app/wait-for-it.sh", "sqlserver:1433", "--", "dotnet", "UserService.Api.dll"]
