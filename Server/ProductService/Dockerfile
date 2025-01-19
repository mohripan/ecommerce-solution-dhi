FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY ./src ./src
COPY ./test ./test

COPY ./wait-for-it.sh /app/wait-for-it.sh

WORKDIR /app/src

RUN dotnet restore ProductService.sln

WORKDIR /app/src/ProductService.Api
RUN dotnet build "ProductService.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ProductService.Api.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=publish /app/publish .
COPY ./wait-for-it.sh /app/wait-for-it.sh

RUN chmod +x /app/wait-for-it.sh
EXPOSE 80
ENTRYPOINT ["/app/wait-for-it.sh", "mysql:3306", "--", "dotnet", "ProductService.Api.dll"]
