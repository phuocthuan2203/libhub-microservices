# Implementation Plan: ApiGateway

## Task 1: Implement the ApiGateway based on the Master Context

**Objective:** Implement the full functionality of the `ApiGateway` according to the provided architecture and internal design specifications.

**Preamble:** You have been provided with the master context documents. You are expected to have read and understood:
- `00_architecture-diagram.md`
- `03_apigateway-internal-architecture.md`

Your task is to add the required packages and generate the configuration and C# code for the files listed below, adhering strictly to the constraints.

---
### **Critical Environment & Configuration Constraints**

**You MUST adhere to the following rules:**

1.  **Architectural Pattern:** This service **does NOT use Clean Architecture**. It is a single-project, configuration-driven application. Do not create Domain, Application, or Infrastructure layers.

2.  **Configuration Source:** All sensitive or environment-specific values (like JWT settings) **must be read from `IConfiguration`**. The `appsettings.json` file should only contain non-sensitive default values or placeholder text.

3.  **No Hardcoded Secrets or Placeholders:** There must be **no hardcoded secrets** (e.g., JWT keys) in the C# code. All such values must be sourced from `IConfiguration`.

---
### **Step 1: Add Required NuGet Packages**

**Instructions:** The solution and project for `ApiGateway` already exist. Execute the following `dotnet` CLI commands from the `src/LibHub.ApiGateway/` directory to add the necessary packages.

```bash
# From within src/LibHub.ApiGateway/

# 1. Navigate into the API project directory
cd LibHub.ApiGateway.Api

# 2. Add the required packages
dotnet add package Ocelot
dotnet add package Ocelot.Provider.Consul
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer

# 3. Navigate back to the solution root
cd ..
Step 2: Target File Structure

Instructions: After adding the packages, the primary files you will be creating/modifying are:

code
Code
download
content_copy
expand_less
/src/LibHub.ApiGateway/
|-- LibHub.ApiGateway.sln
`-- LibHub.ApiGateway.Api/
    |-- LibHub.ApiGateway.Api.csproj
    |-- Program.cs
    |-- appsettings.json
    `-- ocelot.json  <-- This file needs to be created
Step 3: Implement Configuration (ocelot.json)

File to create: src/LibHub.ApiGateway/LibHub.ApiGateway.Api/ocelot.json

Instructions: Create this file and populate it with the full routing configuration from the internal architecture document. The ServiceName property is what Ocelot uses to query Consul for service discovery.

code
JSON
download
content_copy
expand_less
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
    
    // --- Protected Routes ---
    {
      "UpstreamPathTemplate": "/api/books",
      "UpstreamHttpMethod": [ "GET", "POST" ],
      "DownstreamScheme": "http",
      "ServiceName": "CatalogService",
      "DownstreamPathTemplate": "/api/books",
      "AuthenticationOptions": { "AuthenticationProviderKey": "Bearer", "AllowedScopes": [] }
    },
    {
      "UpstreamPathTemplate": "/api/books/{id}",
      "UpstreamHttpMethod": [ "GET", "PUT", "DELETE" ],
      "DownstreamScheme": "http",
      "ServiceName": "CatalogService",
      "DownstreamPathTemplate": "/api/books/{id}",
      "AuthenticationOptions": { "AuthenticationProviderKey": "Bearer", "AllowedScopes": [] }
    },
    {
      "UpstreamPathTemplate": "/api/loans",
      "UpstreamHttpMethod": [ "POST" ],
      "DownstreamScheme": "http",
      "ServiceName": "LoanService",
      "DownstreamPathTemplate": "/api/loans",
      "AuthenticationOptions": { "AuthenticationProviderKey": "Bearer", "AllowedScopes": [] }
    },
    {
      "UpstreamPathTemplate": "/api/users/{userId}/loans",
      "UpstreamHttpMethod": [ "GET" ],
      "DownstreamScheme": "http",
      "ServiceName": "LoanService",
      "DownstreamPathTemplate": "/api/users/{userId}/loans",
      "AuthenticationOptions": { "AuthenticationProviderKey": "Bearer", "AllowedScopes": [] }
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
Step 4: Implement Service and Middleware (Program.cs)

File to modify: src/LibHub.ApiGateway/LibHub.ApiGateway.Api/Program.cs

Instructions: Modify this file to configure Ocelot, Consul integration, JWT authentication, and the middleware pipeline. The order of middleware is critical.

code
C#
download
content_copy
expand_less
var builder = WebApplication.CreateBuilder(args);

// 1. Load ocelot.json configuration
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// 2. Configure JWT Bearer Authentication
// The Authority points to the user service for metadata, but validation happens at the gateway.
builder.Services.AddAuthentication()
    .AddJwtBearer("Bearer", options =>
    {
        // For development, we allow HTTP. In production, this must be HTTPS.
        options.RequireHttpsMetadata = false;
        options.Authority = "http://userservice"; // Ocelot will route this internally
        
        // These values should match what the UserService uses to generate the token
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateAudience = true,
            ValidAudience = "LibHubAudience",
            ValidateIssuer = true,
            ValidIssuer = "LibHub.UserService"
        };
    });

// 3. Configure Ocelot with Consul
builder.Services.AddOcelot()
        .AddConsul();


var app = builder.Build();

// 4. Configure middleware pipeline - ORDER IS CRITICAL
app.UseRouting();

// Authentication and Authorization must be configured before Ocelot
app.UseAuthentication();
app.UseAuthorization();

// This is the final middleware that executes the routing logic
await app.UseOcelot();

app.Run();
Step 5: Configure appsettings.json

File to modify: src/LibHub.ApiGateway/LibHub.ApiGateway.Api/appsettings.json

Instructions: For consistency and future use, add a JwtSettings section with placeholder values, even though the JWT settings are configured in Program.cs. This makes the configuration intent clear.

code
JSON
download
content_copy
expand_less
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "JwtSettings": {
    "SecretKey": "THIS_IS_MANAGED_BY_USER_SERVICE_AND_USED_FOR_VALIDATION_SETUP",
    "Issuer": "LibHub.UserService",
    "Audience": "LibHubAudience"
  }
}
Task 2: Generate Implementation Report

Objective: After completing the implementation, generate a report detailing what was done.

File to create: ai-docs/implementation/api-gw/implementation-report.md

Instructions: Use the provided template to create your report.
