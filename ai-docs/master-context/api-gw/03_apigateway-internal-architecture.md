# API Gateway - Internal Architecture Design

## 1. Guiding Architectural Principles

This document outlines the internal architecture for the `ApiGateway`. Unlike the other services, the API Gateway **does not follow Clean Architecture**. Its primary role is to act as a reverse proxy and a centralized point for cross-cutting concerns.

Therefore, its design is **configuration-driven**, focused on routing rules and the middleware pipeline, rather than complex business logic layers. The goal is to create a robust, configurable, and secure entry point for the LibHub system.

## 2. Core Technology and Project Structure

*   **Project Type:** A standard, single-project ASP.NET Core Web API application.
*   **Key NuGet Packages:**
    *   `Ocelot`: The core library for the reverse proxy and routing functionality.
    *   `Ocelot.Provider.Consul`: The specific package to integrate Ocelot with our Consul Service Registry.
    *   `Microsoft.AspNetCore.Authentication.JwtBearer`: To enable JWT validation at the gateway level.

## 3. Configuration-driven Design (`ocelot.json`)

The heart of the gateway is its configuration file. This JSON file defines all routing rules, service discovery integration, and security policies for each route. It should be placed in the root of the API Gateway's project and configured to be loaded on startup.

```json
{
  "Routes": [
    // --- UserService Routes (Public) ---
    {
      "UpstreamPathTemplate": "/api/users/register",
      "UpstreamHttpMethod": [ "POST" ],
      "DownstreamScheme": "http",
      "ServiceName": "UserService",
      "DownstreamPathTemplate": "/api/users/register"
    },
    {
      "UpstreamPathTemplate": "/api/users/login",
      "UpstreamHttpMethod": [ "POST" ],
      "DownstreamScheme": "http",
      "ServiceName": "UserService",
      "DownstreamPathTemplate": "/api/users/login"
    },
    
    // --- CatalogService Routes (Protected) ---
    {
      "UpstreamPathTemplate": "/api/books",
      "UpstreamHttpMethod": [ "GET", "POST" ],
      "DownstreamScheme": "http",
      "ServiceName": "CatalogService",
      "DownstreamPathTemplate": "/api/books",
      "AuthenticationOptions": {
        "AuthenticationProviderKey": "Bearer",
        "AllowedScopes": []
      }
    },
    {
      "UpstreamPathTemplate": "/api/books/{id}",
      "UpstreamHttpMethod": [ "GET", "PUT", "DELETE" ],
      "DownstreamScheme": "http",
      "ServiceName": "CatalogService",
      "DownstreamPathTemplate": "/api/books/{id}",
      "AuthenticationOptions": {
        "AuthenticationProviderKey": "Bearer",
        "AllowedScopes": []
      }
    },

    // --- LoanService Routes (Protected) ---
    {
      "UpstreamPathTemplate": "/api/loans",
      "UpstreamHttpMethod": [ "POST" ],
      "DownstreamScheme": "http",
      "ServiceName": "LoanService",
      "DownstreamPathTemplate": "/api/loans",
      "AuthenticationOptions": {
        "AuthenticationProviderKey": "Bearer",
        "AllowedScopes": []
      }
    },
    {
      "UpstreamPathTemplate": "/api/users/{userId}/loans",
      "UpstreamHttpMethod": [ "GET" ],
      "DownstreamScheme": "http",
      "ServiceName": "LoanService",
      "DownstreamPathTemplate": "/api/users/{userId}/loans",
      "AuthenticationOptions": {
        "AuthenticationProviderKey": "Bearer",
        "AllowedScopes": []
      }
    }
  ],
  "GlobalConfiguration": {
    "BaseUrl": "http://localhost:5000",
    "ServiceDiscoveryProvider": {
      "Scheme": "http",
      "Host": "consul",
      "Port": 8500,
      "Type": "Consul"
    }
  }
}
```

4. Service and Middleware Configuration (Program.cs)

The Program.cs file is responsible for setting up dependency injection and configuring the request processing pipeline. The order of operations is critical.

```csharp
// --- File: Program.cs ---

var builder = WebApplication.CreateBuilder(args);

// 1. Load ocelot.json configuration
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// 2. Configure JWT Authentication
// In a real app, these values would come from a secure configuration source.
builder.Services.AddAuthentication()
    .AddJwtBearer("Bearer", options =>
    {
        // For this project, we assume the UserService is the authority.
        // Ocelot will forward token validation to the user service if needed,
        // or validate it here if a shared secret is used.
        // For simplicity, we define the validation parameters.
        options.Authority = "http://userservice"; // Address of the UserService for discovery
        options.Audience = "LibHubAudience";
        options.RequireHttpsMetadata = false; // For development environment
        // Additional validation parameters (TokenValidationParameters) would be set here.
    });

// 3. Configure Ocelot and its integration with Consul
builder.Services.AddOcelot()
        .AddConsul();

// --- Build the application ---
var app = builder.Build();

// --- Configure the Middleware Pipeline ---

// 4. (Optional but recommended) Add Correlation ID middleware first
// This custom middleware will generate/pass along the X-Correlation-ID header.
// app.UseCorrelationId(); 

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();

// 5. Add Authentication & Authorization middleware
// This must come before the Ocelot middleware.
app.UseAuthentication();
app.UseAuthorization();

// 6. Add the Ocelot middleware to the pipeline.
// This is the final step that executes the routing logic.
await app.UseOcelot();

app.Run();
