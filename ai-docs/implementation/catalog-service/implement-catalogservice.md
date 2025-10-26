# Implementation Plan: CatalogService

## Task 1: Implement the CatalogService based on the Master Context

**Objective:** Scaffold and implement the full functionality of the `CatalogService` according to the provided architecture, API, database, and internal design specifications.

**Preamble:** You have been provided with the master context documents. You are expected to have read and understood:
- `00_architecture-diagram.md`
- `02_database-schemas.md`
- `01_catalog-service-api.md`
- `03_catalogservice-internal-architecture.md`

Your tasks are to first scaffold the required project structure, then generate the C# code, adhering strictly to the constraints below.

---
### **Critical Environment & Configuration Constraints**

**You MUST adhere to the following rules:**

1.  **Database Provider:** All data access must be implemented using the **`Pomelo.EntityFrameworkCore.MySql`** NuGet package. The `DbContext` must be configured with `UseMySql`. No other database providers (e.g., SQL Server) are permitted.

2.  **Configuration Source:** All configuration values, especially the `ConnectionStrings:DefaultConnection`, **must be read from `IConfiguration`**. The `appsettings.json` file should only contain the placeholder text: `"DefaultConnection": "THIS_WILL_BE_OVERRIDDEN_BY_ENV"`.

3.  **No Hardcoded Secrets or Placeholders:** There must be **no hardcoded secrets** or placeholder strings (e.g., passwords, keys) in the C# code. All such values must be sourced from `IConfiguration`.

---
### **Step 1: Project Scaffolding**

**Instructions:** The solution and API project for `CatalogService` already exist. Execute the following `dotnet` CLI commands from the `src/LibHub.CatalogService/` directory to create the Clean Architecture layers and link them correctly.

```bash
# From within src/LibHub.CatalogService/

# 1. Create the empty class library projects for each layer
dotnet new classlib -n LibHub.CatalogService.Domain
dotnet new classlib -n LibHub.CatalogService.Application
dotnet new classlib -n LibHub.CatalogService.Infrastructure

# 2. Add these new projects to the main solution file
dotnet sln add LibHub.CatalogService.Domain/LibHub.CatalogService.Domain.csproj
dotnet sln add LibHub.CatalogService.Application/LibHub.CatalogService.Application.csproj
dotnet sln add LibHub.CatalogService.Infrastructure/LibHub.CatalogService.Infrastructure.csproj

# 3. Add the project references to enforce the Dependency Rule
dotnet add LibHub.CatalogService.Api/LibHub.CatalogService.Api.csproj reference LibHub.CatalogService.Application/LibHub.CatalogService.Application.csproj
dotnet add LibHub.CatalogService.Application/LibHub.CatalogService.Application.csproj reference LibHub.CatalogService.Domain/LibHub.CatalogService.Domain.csproj
dotnet add LibHub.CatalogService.Infrastructure/LibHub.CatalogService.Infrastructure.csproj reference LibHub.CatalogService.Application/LibHub.CatalogService.Application.csproj
Step 2: Target File Structure

Instructions: After scaffolding, create the C# files within this structure.

code
Code
download
content_copy
expand_less
/src/LibHub.CatalogService/
|-- LibHub.CatalogService.sln
|-- LibHub.CatalogService.Api/
|   |-- Controllers/
|   |   `-- BooksController.cs
|   |-- LibHub.CatalogService.Api.csproj
|   `-- Program.cs
|-- LibHub.CatalogService.Application/
|   |-- DTOs/
|   |   |-- BookDto.cs
|   |   |-- CreateBookDto.cs
|   |   |-- UpdateBookDto.cs
|   |   `-- UpdateStockDto.cs
|   |-- Services/
|   |   `-- BookApplicationService.cs
|   `-- LibHub.CatalogService.Application.csproj
|-- LibHub.CatalogService.Domain/
|   |-- Entities/
|   |   `-- Book.cs
|   |-- Interfaces/
|   |   `-- IBookRepository.cs
|   `-- LibHub.CatalogService.Domain.csproj
`-- LibHub.CatalogService.Infrastructure/
    |-- Persistence/
    |   |-- CatalogDbContext.cs
    |   `-- EfBookRepository.cs
    `-- LibHub.CatalogService.Infrastructure.csproj
Step 3: Implement the Domain Layer
3.1. Create the Book Entity

File to create: src/LibHub.CatalogService/LibHub.CatalogService.Domain/Entities/Book.cs

Instructions: Implement the Book class as defined in the internal architecture document, including its business logic methods (DecrementStock, IncrementStock).

3.2. Create the IBookRepository Interface

File to create: src/LibHub.CatalogService/LibHub.CatalogService.Domain/Interfaces/IBookRepository.cs

Instructions: Define the repository interface with all required methods (GetByIdAsync, GetAllAsync, AddAsync, UpdateAsync).

Step 4: Implement the Application Layer
4.1. Create DTOs

Instructions: In the LibHub.CatalogService.Application project, create all required DTO files.

4.2. Implement the BookApplicationService

File to create: src/LibHub.CatalogService/LibHub.CatalogService.Application/Services/BookApplicationService.cs

Instructions: Implement the application service logic as defined in the architecture, injecting IBookRepository and orchestrating the use cases.

Step 5: Implement the Infrastructure Layer
5.1. Implement Persistence with EF Core for MySQL

Files to create: src/LibHub.CatalogService/LibHub.CatalogService.Infrastructure/Persistence/CatalogDbContext.cs and EfBookRepository.cs.

Instructions:

Create the CatalogDbContext, ensuring it has a DbSet<Book>.

Implement the EfBookRepository class, inheriting from IBookRepository and using the CatalogDbContext.

Ensure the Pomelo.EntityFrameworkCore.MySql NuGet package is added to this project.

Step 6: Implement the Presentation Layer
6.1. Implement the BooksController

File to create: src/LibHub.CatalogService/LibHub.CatalogService.Api/Controllers/BooksController.cs

Instructions: Implement the API controller, ensuring it is "thin" and matches the OpenAPI specification exactly. Pay attention to the required [Authorize] attributes for admin and internal service calls.

Step 7: Configure Dependency Injection and appsettings.json

File to modify: src/LibHub.CatalogService/LibHub.CatalogService.Api/Program.cs

Instructions: Wire up all the interfaces and their concrete implementations for dependency injection.

File to modify: src/LibHub.CatalogService/LibHub.CatalogService.Api/appsettings.json

Instructions: Add the placeholder ConnectionStrings section.

code
C#
download
content_copy
expand_less
// In Program.cs:

// 1. Configure EF Core for MySQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<CatalogDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// 2. Register application services and repositories
builder.Services.AddScoped<IBookRepository, EfBookRepository>();
builder.Services.AddScoped<BookApplicationService>();
code
JSON
download
content_copy
expand_less
// In appsettings.json:
{
  // ... other settings
  "ConnectionStrings": {
    "DefaultConnection": "THIS_WILL_BE_OVERRIDDEN_BY_ENV"
  }
}
Task 2: Generate Implementation Report

Objective: After completing the implementation, generate a report detailing what was done and any pending items.

File to create: ai-docs/implementation/catalog-service/implementation-report.md

Instructions: Use the provided template to create your report, filling in the checkboxes for completed tasks.

