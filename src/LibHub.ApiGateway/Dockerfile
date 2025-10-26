# File: src/LibHub.ApiGateway/Dockerfile
# This is a simpler Dockerfile for a single-project service.

# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy only the .csproj file and restore dependencies
COPY ["LibHub.ApiGateway.Api/LibHub.ApiGateway.Api.csproj", "LibHub.ApiGateway.Api/"]
RUN dotnet restore "LibHub.ApiGateway.Api/LibHub.ApiGateway.Api.csproj"

# Copy the rest of the source code
COPY . .
WORKDIR "/src/LibHub.ApiGateway.Api"
RUN dotnet build "LibHub.ApiGateway.Api.csproj" -c Release -o /app/build

# Stage 2: Publish the application
FROM build AS publish
RUN dotnet publish "LibHub.ApiGateway.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Create the final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LibHub.ApiGateway.Api.dll"]