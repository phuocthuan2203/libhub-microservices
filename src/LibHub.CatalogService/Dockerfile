# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-stage
WORKDIR /src

# Copy the solution file first
COPY ["LibHub.CatalogService.sln", "."]

# Copy all the .csproj files for all layers of this service
COPY ["LibHub.CatalogService.Api/LibHub.CatalogService.Api.csproj", "LibHub.CatalogService.Api/"]
COPY ["LibHub.CatalogService.Application/LibHub.CatalogService.Application.csproj", "LibHub.CatalogService.Application/"]
COPY ["LibHub.CatalogService.Domain/LibHub.CatalogService.Domain.csproj", "LibHub.CatalogService.Domain/"]
COPY ["LibHub.CatalogService.Infrastructure/LibHub.CatalogService.Infrastructure.csproj", "LibHub.CatalogService.Infrastructure/"]

# Restore dependencies for the entire solution
RUN dotnet restore "LibHub.CatalogService.sln"

# Copy the rest of the source code
COPY . .

# Build and publish the API project
WORKDIR "/src/LibHub.CatalogService.Api"
RUN dotnet publish "LibHub.CatalogService.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Create the final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final-stage
WORKDIR /app
COPY --from=build-stage /app/publish .
ENTRYPOINT ["dotnet", "LibHub.CatalogService.Api.dll"]
