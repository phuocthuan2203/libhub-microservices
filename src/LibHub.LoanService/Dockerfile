# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-stage
WORKDIR /src

# Copy the solution file first
COPY ["LibHub.LoanService.sln", "."]

# Copy all the .csproj files for all layers of this service
COPY ["LibHub.LoanService.Api/LibHub.LoanService.Api.csproj", "LibHub.LoanService.Api/"]
COPY ["LibHub.LoanService.Application/LibHub.LoanService.Application.csproj", "LibHub.LoanService.Application/"]
COPY ["LibHub.LoanService.Domain/LibHub.LoanService.Domain.csproj", "LibHub.LoanService.Domain/"]
COPY ["LibHub.LoanService.Infrastructure/LibHub.LoanService.Infrastructure.csproj", "LibHub.LoanService.Infrastructure/"]

# Restore dependencies for the entire solution
RUN dotnet restore "LibHub.LoanService.sln"

# Copy the rest of the source code
COPY . .

# Build and publish the API project
WORKDIR "/src/LibHub.LoanService.Api"
RUN dotnet publish "LibHub.LoanService.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Create the final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final-stage
WORKDIR /app
COPY --from=build-stage /app/publish .
ENTRYPOINT ["dotnet", "LibHub.LoanService.Api.dll"]
