# Implementation Plan: LoanService

## Task 1: Implement the LoanService based on the Master Context

**Objective:** Scaffold and implement the full functionality of the `LoanService` according to the provided architecture, API, database, and internal design specifications.

**Preamble:** You have been provided with the master context documents. You are expected to have read and understood:
- `00_architecture-diagram.md`
- `02_database-schemas.md`
- `01_loan-service-api.md`
- `03_loanservice-internal-architecture.md`
- `Saga Sequence Diagram` (from the A&D phase)

Your tasks are to first scaffold the required project structure, then generate the C# code, adhering strictly to the constraints below.

---
### **Critical Environment & Configuration Constraints**

**You MUST adhere to the following rules:**

1.  **Database Provider:** All data access must be implemented using the **`Pomelo.EntityFrameworkCore.MySql`** NuGet package. The `DbContext` must be configured with `UseMySql`. No other database providers are permitted.

2.  **Configuration Source:** All configuration values, especially the `ConnectionStrings:DefaultConnection`, **must be read from `IConfiguration`**. The `appsettings.json` file should only contain the placeholder text: `"DefaultConnection": "THIS_WILL_BE_OVERRIDDEN_BY_ENV"`.

3.  **No Hardcoded Secrets or Placeholders:** There must be **no hardcoded secrets** or placeholder strings (e.g., passwords, keys) in the C# code. All such values must be sourced from `IConfiguration`.

---
### **Step 1: Project Scaffolding**
l
**Instructions:** The solution and API project for `LoanService` already exist. Execute the following `dotnet` CLI commands from the `src/LibHub.LoanService/` directory to create the Clean Architecture layers and link them correctly.

```bash
# From within src/LibHub.LoanService/

# 1. Create the empty class library projects for each layer
dotnet new classlib -n LibHub.LoanService.Domain
dotnet new classlib -n LibHub.LoanService.Application
dotnet new classlib -n LibHub.LoanService.Infrastructure

# 2. Add these new projects to the main solution file
dotnet sln add LibHub.LoanService.Domain/LibHub.LoanService.Domain.csproj
dotnet sln add LibHub.LoanService.Application/LibHub.LoanService.Application.csproj
dotnet sln add LibHub.LoanService.Infrastructure/LibHub.LoanService.Infrastructure.csproj

# 3. Add the project references to enforce the Dependency Rule
dotnet add LibHub.LoanService.Api/LibHub.LoanService.Api.csproj reference LibHub.LoanService.Application/LibHub.LoanService.Application.csproj
dotnet add LibHub.LoanService.Application/LibHub.LoanService.Application.csproj reference LibHub.LoanService.Domain/LibHub.LoanService.Domain.csproj
dotnet add LibHub.LoanService.Infrastructure/LibHub.LoanService.Infrastructure.csproj reference LibHub.LoanService.Application/LibHub.LoanService.Application.csproj
Step 2: Target File Structure

Instructions: After scaffolding, create the C# files within this structure.

code
Code
download
content_copy
expand_less
/src/LibHub.LoanService/
|-- LibHub.LoanService.sln
|-- LibHub.LoanService.Api/
|   |-- Controllers/
|   |   `-- LoansController.cs
|   |-- LibHub.LoanService.Api.csproj
|   `-- Program.cs
|-- LibHub.LoanService.Application/
|   |-- DTOs/
|   |   |-- BookAvailabilityDto.cs
|   |   |-- CreateLoanDto.cs
|   |   `-- LoanDto.cs
|   |-- Interfaces/
|   |   `-- ICatalogService.cs
|   |-- Services/
|   |   `-- LoanApplicationService.cs
|   `-- LibHub.LoanService.Application.csproj
|-- LibHub.LoanService.Domain/
|   |-- Entities/
|   |   `-- Loan.cs
|   |-- Interfaces/
|   |   `-- ILoanRepository.cs
|   `-- LibHub.LoanService.Domain.csproj
`-- LibHub.LoanService.Infrastructure/
    |-- HttpClients/
    |   `-- CatalogServiceHttpClient.cs
    |-- Persistence/
    |   |-- LoanDbContext.cs
    |   `-- EfLoanRepository.cs
    `-- LibHub.LoanService.Infrastructure.csproj
Step 3: Implement the Domain Layer
3.1. Create the Loan Entity

File to create: src/LibHub.LoanService/LibHub.LoanService.Domain/Entities/Loan.cs

Instructions: Implement the Loan class as defined in the internal architecture document, including its state machine logic (ConfirmCheckout, MarkAsReturned, Fail).

3.2. Create the ILoanRepository Interface

File to create: src/LibHub.LoanService/LibHub.LoanService.Domain/Interfaces/ILoanRepository.cs

Instructions: Define the repository interface with all required methods (GetByIdAsync, GetByUserIdAsync, AddAsync, UpdateAsync).

Step 4: Implement the Application Layer
4.1. Create DTOs and Infrastructure Interfaces

Instructions: In the LibHub.LoanService.Application project, create all DTOs and the crucial ICatalogService.cs interface, which defines the contract for communicating with the CatalogService.

4.2. Implement the LoanApplicationService

File to create: src/LibHub.LoanService/LibHub.LoanService.Application/Services/LoanApplicationService.cs

Instructions: Implement this application service. It is the Saga Orchestrator. Inject both ILoanRepository and ICatalogService. The BorrowBookAsync method must follow the distributed transaction logic outlined in the Saga Sequence Diagram and the internal architecture document.

Step 5: Implement the Infrastructure Layer
5.1. Implement Persistence with EF Core for MySQL

Files to create: src/LibHub.LoanService/LibHub.LoanService.Infrastructure/Persistence/LoanDbContext.cs and EfLoanRepository.cs.

Instructions: Create the LoanDbContext and the concrete EfLoanRepository implementation. Ensure the Pomelo.EntityFrameworkCore.MySql NuGet package is added to this project.

5.2. Implement the External Service Client

File to create: src/LibHub.LoanService/LibHub.LoanService.Infrastructure/HttpClients/CatalogServiceHttpClient.cs

Instructions: Implement the ICatalogService interface. Use HttpClient (injected via IHttpClientFactory) to make the actual REST calls to the CatalogService. This class will be responsible for service discovery in a real-world scenario.

Step 6: Implement the Presentation Layer
6.1. Implement the LoansController

File to create: src/LibHub.LoanService/LibHub.LoanService.Api/Controllers/LoansController.cs

Instructions: Implement the API controller. It must be "thin" and match the OpenAPI specification. The BorrowBook endpoint must extract the userId from the JWT claims.

Step 7: Configure Dependency Injection and appsettings.json

File to modify: src/LibHub.LoanService/LibHub.LoanService.Api/Program.cs

Instructions: Wire up all dependencies. This is a critical step for this service.

File to modify: src/LibHub.LoanService/LibHub.LoanService.Api/appsettings.json

Instructions: Add the placeholder ConnectionStrings section.

code
C#
download
content_copy
expand_less
// In Program.cs:

// 1. Configure EF Core for MySQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<LoanDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// 2. Register application services and repositories
builder.Services.AddScoped<ILoanRepository, EfLoanRepository>();
builder.Services.AddScoped<LoanApplicationService>();

// 3. Register and configure the typed HttpClient for the CatalogService
builder.Services.AddHttpClient<ICatalogService, CatalogServiceHttpClient>(client =>
{
    // In a real-world scenario with dynamic service discovery,
    // the base address would be resolved here by querying Consul.
    // For this project, we can configure it to point to the CatalogService's container name.
    client.BaseAddress = new Uri("http://catalogservice"); 
});
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

File to create: ai-docs/implementation/loan-service/implementation-report.md

Instructions: Use the provided template to create your report, filling in the checkboxes for completed tasks.

code
Code
download
content_copy
expand_less