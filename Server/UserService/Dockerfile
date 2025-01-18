FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the entire source and test directories to preserve relative paths
COPY ./src ./src
COPY ./test ./test

# Set the working directory to src for the solution file
WORKDIR /app/src

# Restore the solution
RUN dotnet restore UserService.sln

# Build the API project
WORKDIR /app/src/UserService.Api
RUN dotnet build "UserService.Api.csproj" -c Release -o /app/build

# Publish the API
FROM build AS publish
RUN dotnet publish "UserService.Api.csproj" -c Release -o /app/publish

# Use the runtime image to host the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "UserService.Api.dll"]