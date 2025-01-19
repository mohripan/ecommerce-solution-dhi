FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY ./src ./src
COPY ./test ./test

WORKDIR /app/src

RUN dotnet restore TransactionService.sln

WORKDIR /app/src/TransactionService.Api
RUN dotnet build "TransactionService.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TransactionService.Api.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=publish /app/publish .
COPY ./wait-for-it.sh /app/wait-for-it.sh

RUN chmod +x /app/wait-for-it.sh

EXPOSE 80

ENTRYPOINT ["/app/wait-for-it.sh", "postgres:5432", "--", "dotnet", "TransactionService.Api.dll"]