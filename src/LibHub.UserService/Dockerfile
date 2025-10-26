# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-stage
WORKDIR /src

# Copy the solution file first
COPY ["LibHub.UserService.sln", "."]

# Copy all the .csproj files for all layers of this service
COPY ["LibHub.UserService.Api/LibHub.UserService.Api.csproj", "LibHub.UserService.Api/"]
COPY ["LibHub.UserService.Application/LibHub.UserService.Application.csproj", "LibHub.UserService.Application/"]
COPY ["LibHub.UserService.Domain/LibHub.UserService.Domain.csproj", "LibHub.UserService.Domain/"]
COPY ["LibHub.UserService.Infrastructure/LibHub.UserService.Infrastructure.csproj", "LibHub.UserService.Infrastructure/"]

# Restore dependencies for the entire solution
RUN dotnet restore "LibHub.UserService.sln"

# Copy the rest of the source code
COPY . .

# Build and publish the API project
WORKDIR "/src/LibHub.UserService.Api"
RUN dotnet publish "LibHub.UserService.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Create the final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final-stage
WORKDIR /app
COPY --from=build-stage /app/publish .
ENTRYPOINT ["dotnet", "LibHub.UserService.Api.dll"]
