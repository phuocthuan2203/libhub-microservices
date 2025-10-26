# Implementation Plan: UserService

## Task 1: Implement the UserService based on the Master Context

**Objective:** Scaffold and implement the full functionality of the `UserService` according to the provided architecture, API, database, and internal design specifications.

**Preamble:** You have been provided with the master context documents. You are expected to have read and understood:
- `00_architecture-diagram.md`
- `02_database-schemas.md`
- `01_user-service-api.md`
- `03_userservice-internal-architecture.md`

Your tasks are to first scaffold the required project structure using `dotnet` CLI commands, then generate the C# code for the files listed below.

---
### **Step 1: Project Scaffolding**

**Instructions:** The solution and API project for `UserService` already exist. Execute the following `dotnet` CLI commands from the `src/LibHub.UserService/` directory to create the Clean Architecture layers and link them correctly.

```bash
# From within src/LibHub.UserService/

# 1. Create the empty class library projects for each layer
dotnet new classlib -n LibHub.UserService.Domain
dotnet new classlib -n LibHub.UserService.Application
dotnet new classlib -n LibHub.UserService.Infrastructure

# 2. Add these new projects to the main solution file
dotnet sln add LibHub.UserService.Domain/LibHub.UserService.Domain.csproj
dotnet sln add LibHub.UserService.Application/LibHub.UserService.Application.csproj
dotnet sln add LibHub.UserService.Infrastructure/LibHub.UserService.Infrastructure.csproj

# 3. Add the project references to enforce the Dependency Rule
dotnet add LibHub.UserService.Api/LibHub.UserService.Api.csproj reference LibHub.UserService.Application/LibHub.UserService.Application.csproj
dotnet add LibHub.UserService.Application/LibHub.UserService.Application.csproj reference LibHub.UserService.Domain/LibHub.UserService.Domain.csproj
dotnet add LibHub.UserService.Infrastructure/LibHub.UserService.Infrastructure.csproj reference LibHub.UserService.Application/LibHub.UserService.Application.csproj
Step 2: Target File Structure

Instructions: After scaffolding, the project structure for UserService should look like this. You will be creating the C# files within this structure.

code
Code
download
content_copy
expand_less
/src/LibHub.UserService/
|-- LibHub.UserService.sln
|-- LibHub.UserService.Api/
|   |-- Controllers/
|   |   `-- UsersController.cs
|   |-- LibHub.UserService.Api.csproj
|   `-- Program.cs
|-- LibHub.UserService.Application/
|   |-- DTOs/
|   |   |-- LoginDto.cs
|   |   |-- RegisterUserDto.cs
|   |   |-- TokenDto.cs
|   |   `-- UserDto.cs
|   |-- Interfaces/
|   |   |-- IJwtTokenGenerator.cs
|   |   `-- IPasswordHasher.cs
|   |-- Services/
|   |   `-- IdentityApplicationService.cs
|   `-- LibHub.UserService.Application.csproj
|-- LibHub.UserService.Domain/
|   |-- Entities/
|   |   `-- User.cs
|   |-- Interfaces/
|   |   `-- IUserRepository.cs
|   `-- LibHub.UserService.Domain.csproj
`-- LibHub.UserService.Infrastructure/
    |-- Persistence/
    |   |-- EfUserRepository.cs
    |   `-- UserDbContext.cs
    |-- Security/
    |   |-- JwtTokenGenerator.cs
    |   `-- PasswordHasher.cs
    `-- LibHub.UserService.Infrastructure.csproj
Step 3: Implement the Domain Layer
3.1. Create the User Entity

File to create: src/LibHub.UserService/LibHub.UserService.Domain/Entities/User.cs

Instructions: Implement the User class as defined in the internal architecture document.

3.2. Create the IUserRepository Interface

File to create: src/LibHub.UserService/LibHub.UserService.Domain/Interfaces/IUserRepository.cs

Instructions: Define the repository interface with its methods.

Step 4: Implement the Application Layer
4.1. Create DTOs and Infrastructure Interfaces

Instructions: In the LibHub.UserService.Application project, create the DTO and interface files in their respective folders.

4.2. Implement the IdentityApplicationService

File to create: src/LibHub.UserService/LibHub.UserService.Application/Services/IdentityApplicationService.cs

Instructions: Implement the application service logic as defined in the architecture, injecting dependencies and orchestrating the use cases.

Step 5: Implement the Infrastructure Layer
5.1. Implement Persistence with EF Core

Files to create: src/LibHub.UserService/LibHub.UserService.Infrastructure/Persistence/UserDbContext.cs and EfUserRepository.cs.

Instructions: Create the UserDbContext and the concrete EfUserRepository implementation.

5.2. Implement Security Services

Files to create: src/LibHub.UserService/LibHub.UserService.Infrastructure/Security/PasswordHasher.cs and JwtTokenGenerator.cs.

Instructions: Implement the IPasswordHasher and IJwtTokenGenerator interfaces.

Step 6: Implement the Presentation Layer
6.1. Implement the UsersController

File to create: src/LibHub.UserService/LibHub.UserService.Api/Controllers/UsersController.cs

Instructions: Implement the API controller, ensuring it is "thin" and matches the OpenAPI specification exactly.

Step 7: Configure Dependency Injection

File to modify: src/LibHub.UserService/LibHub.UserService.Api/Program.cs

Instructions: Wire up all the interfaces and their concrete implementations for dependency injection.

Task 2: Generate Implementation Report

Objective: After completing the implementation, generate a report detailing what was done and any pending items.

File to create: ai-docs/implementation/user-service/implementation-report.md

Instructions: Use the provided template to create your report, filling in the checkboxes for completed tasks.
