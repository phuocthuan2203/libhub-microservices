# LoanService - Internal Architecture Design

## 1. Guiding Architectural Pattern: Clean Architecture

This document outlines the internal architecture for the `LoanService`. The service is built following the principles of **Clean Architecture**.

The core principle is **The Dependency Rule**: all source code dependencies must point inwards, from outer layers to inner layers. The `Domain` layer at the core must have zero dependencies on any other layer. This ensures the business logic is independent of infrastructure concerns like databases or frameworks, making the application robust, testable, and maintainable.

The implementation will be structured into four separate C# projects, representing the four layers.

## 2. Architectural Diagram (Template)

The following diagram illustrates the standard four-layer structure and the direction of dependencies.

```plantuml
@startuml
!theme plain
skinparam componentStyle uml2

title Clean Architecture for a Single Microservice

package "Presentation Layer (Web API)" {
  [API Controllers]
  [Middleware]
  [Dependency Injection]
}

package "Application Layer (Use Cases)" {
  [Application Services]
  [DTOs]
  [Interfaces for Infrastructure]
  [Validation]
}

package "Domain Layer (Core Business Rules)" {
  [Entities (Aggregates)]
  [Repository Interfaces]
}

package "Infrastructure Layer (Implementations)" {
  [EF Core DbContext]
  [Repository Implementations]
  [External Service Clients]
}

' --- Dependency Arrows ---
Presentation_Layer --> Application_Layer
Application_Layer --> Domain_Layer
Infrastructure_Layer --> Application_Layer
Infrastructure_Layer --> Domain_Layer

note right of Infrastructure_Layer
  Implements interfaces
  defined in the inner layers.
end note

note bottom of Domain_Layer
  **The Dependency Rule:** All arrows point inwards.
  The core Domain Layer has zero external dependencies.
end note

@enduml
3. Layer-by-Layer Breakdown
3.1. Domain Layer (LibHub.LoanService.Domain)

This is the core of the service, containing the business logic and state for a loan.

Entity: Loan.cs

This is the Aggregate Root. It contains the business rules for managing the lifecycle of a single loan.

code
C#
download
content_copy
expand_less
public class Loan
{
    public int LoanId { get; private set; }
    public int UserId { get; private set; }
    public int BookId { get; private set; }
    public string Status { get; private set; } // e.g., "PENDING", "CheckedOut", "Returned", "FAILED"
    public DateTime CheckoutDate { get; private set; }
    public DateTime DueDate { get; private set; }
    public DateTime? ReturnDate { get; private set; }

    // Constructor for starting the borrow process
    public Loan(int userId, int bookId)
    {
        UserId = userId;
        BookId = bookId;
        Status = "PENDING"; // Saga starts in a pending state
        CheckoutDate = DateTime.UtcNow;
        DueDate = DateTime.UtcNow.AddDays(14); // Business rule: 14-day loan period
    }

    // Business logic methods for state transitions
    public void ConfirmCheckout()
    {
        if (Status != "PENDING") throw new InvalidOperationException("Loan is not in a pending state.");
        Status = "CheckedOut";
    }

    public void MarkAsReturned()
    {
        if (Status != "CheckedOut") throw new InvalidOperationException("Cannot return a loan that is not checked out.");
        Status = "Returned";
        ReturnDate = DateTime.UtcNow;
    }

    public void Fail()
    {
        Status = "FAILED";
    }
}
Repository Interface: ILoanRepository.cs

Defines the contract for loan data persistence.

code
C#
download
content_copy
expand_less
public interface ILoanRepository
{
    Task<Loan?> GetByIdAsync(int loanId);
    Task<IEnumerable<Loan>> GetByUserIdAsync(int userId);
    Task AddAsync(Loan loan);
    Task UpdateAsync(Loan loan);
}
3.2. Application Layer (LibHub.LoanService.Application)

This layer orchestrates the "Borrow Book" Saga and other use cases.

Data Transfer Objects (DTOs):

CreateLoanDto.cs: { int BookId }

LoanDto.cs: { int LoanId, int UserId, int BookId, string Status, DateTime CheckoutDate, ... }

BookAvailabilityDto.cs (from external service): { int BookId, bool IsAvailable }

Infrastructure Interfaces (Abstractions for External Services):

ICatalogService.cs: Defines the contract for what the LoanService needs from the CatalogService.

code
C#
download
content_copy
expand_less
public interface ICatalogService
{
    Task<BookAvailabilityDto> GetBookAvailabilityAsync(int bookId);
    Task UpdateBookStockAsync(int bookId, int changeAmount);
}

Application Service: LoanApplicationService.cs (The Saga Orchestrator)
This service contains the orchestration logic for the distributed transaction.

code
C#
download
content_copy
expand_less
public class LoanApplicationService
{
    private readonly ILoanRepository _loanRepository;
    private readonly ICatalogService _catalogService;

    public LoanApplicationService(ILoanRepository loanRepository, ICatalogService catalogService) 
    {
        _loanRepository = loanRepository;
        _catalogService = catalogService;
    }

    public async Task<LoanDto> BorrowBookAsync(int userId, CreateLoanDto createLoanDto)
    {
        // SAGA STEP 1: Verify pre-conditions with external service
        var availability = await _catalogService.GetBookAvailabilityAsync(createLoanDto.BookId);
        if (!availability.IsAvailable) throw new Exception("Book is not available.");

        // SAGA STEP 2: Create local transaction (pending state)
        var loan = new Loan(userId, createLoanDto.BookId);
        await _loanRepository.AddAsync(loan);

        try
        {
            // SAGA STEP 3: Execute downstream transaction
            await _catalogService.UpdateBookStockAsync(createLoanDto.BookId, -1);
        }
        catch (Exception ex)
        {
            // SAGA STEP 4 (Failure): Execute compensating transaction
            loan.Fail();
            await _loanRepository.UpdateAsync(loan);
            throw new Exception("Failed to update book stock. Borrowing process failed.", ex);
        }

        // SAGA STEP 5 (Success): Finalize local transaction
        loan.ConfirmCheckout();
        await _loanRepository.UpdateAsync(loan);

        // 6. Map to DTO and return
        return new LoanDto { /* mapping logic */ };
    }
}
3.3. Infrastructure Layer (LibHub.LoanService.Infrastructure)

Contains the concrete implementations for data access and external service communication.

Persistence: EfLoanRepository.cs

Implements ILoanRepository using Entity Framework Core and a LoanDbContext.

External Service Client: CatalogServiceHttpClient.cs

Implements the ICatalogService interface.

Uses IHttpClientFactory to create a pre-configured HttpClient.

This is where service discovery logic would be used to resolve the base URL of the CatalogService by querying Consul before making the call.

Handles serialization/deserialization of requests/responses.

3.4. Presentation Layer (LibHub.LoanService.Api)

The ASP.NET Core Web API project.

Controller: LoansController.cs
The controller extracts the user's identity and delegates to the application service.

code
C#
download
content_copy
expand_less
[ApiController]
[Route("api/loans")]
[Authorize]
public class LoansController : ControllerBase
{
    private readonly LoanApplicationService _loanService;

    public LoansController(LoanApplicationService loanService)
    {
        _loanService = loanService;
    }

    [HttpPost]
    public async Task<IActionResult> BorrowBook(CreateLoanDto createLoanDto)
    {
        // Extract userId from the JWT claims provided by the gateway
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        var loanDto = await _loanService.BorrowBookAsync(userId, createLoanDto);
        return CreatedAtAction(nameof(GetLoanById), new { id = loanDto.LoanId }, loanDto);
    }
}

Dependency Injection (Program.cs):

services.AddDbContext<LoanDbContext>(...)

services.AddScoped<ILoanRepository, EfLoanRepository>();

services.AddScoped<LoanApplicationService>();

services.AddHttpClient<ICatalogService, CatalogServiceHttpClient>(); // Wires up the typed HttpClient
