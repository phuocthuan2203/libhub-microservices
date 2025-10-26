# CatalogService - Internal Architecture Design

## 1. Guiding Architectural Pattern: Clean Architecture

This document outlines the internal architecture for the `CatalogService`. The service is built following the principles of **Clean Architecture**.

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
3.1. Domain Layer (LibHub.CatalogService.Domain)

This is the core of the service, containing the business rules for books and inventory.

Entity: Book.cs

This is the Aggregate Root. It's not just a data container; it contains the business logic for managing its own state, particularly the inventory, to prevent inconsistencies.

code
C#
download
content_copy
expand_less
public class Book
{
    public int BookId { get; private set; }
    public string Isbn { get; private set; }
    public string Title { get; private set; }
    public string Author { get; private set; }
    public int TotalCopies { get; private set; }
    public int AvailableCopies { get; private set; }

    // Constructor for creating a new book
    public Book(string isbn, string title, string author, int totalCopies)
    {
        // Business rule: A new book starts with all copies available.
        Isbn = isbn;
        Title = title;
        Author = author;
        TotalCopies = totalCopies;
        AvailableCopies = totalCopies;
    }

    // Business logic method for borrowing
    public void DecrementStock()
    {
        // Business rule: Cannot borrow a book if none are available.
        if (AvailableCopies <= 0)
        {
            throw new Exception("No copies of this book are available for loan.");
        }
        AvailableCopies--;
    }

    // Business logic method for returning
    public void IncrementStock()
    {
        // Business rule: Cannot return a book if it would exceed the total copies.
        if (AvailableCopies >= TotalCopies)
        {
            // This indicates a data consistency issue and should be logged.
            throw new Exception("Cannot increment stock beyond the total number of copies.");
        }
        AvailableCopies++;
    }
}
Repository Interface: IBookRepository.cs

Defines the contract for book data persistence.

code
C#
download
content_copy
expand_less
public interface IBookRepository
{
    Task<Book?> GetByIdAsync(int bookId);
    Task<IEnumerable<Book>> GetAllAsync();
    Task AddAsync(Book book);
    Task UpdateAsync(Book book); // Used to save changes after stock updates
}
3.2. Application Layer (LibHub.CatalogService.Application)

This layer orchestrates the use cases for the CatalogService.

Data Transfer Objects (DTOs):

CreateBookDto.cs: { string Isbn, string Title, string Author, int TotalCopies }

BookDto.cs: { int BookId, string Isbn, string Title, string Author, int AvailableCopies, int TotalCopies }

UpdateStockDto.cs: { int ChangeAmount } (e.g., -1 for borrow, +1 for return)

Application Service: BookApplicationService.cs
Contains the logic to handle the book management and inventory use cases.

code
C#
download
content_copy
expand_less
public class BookApplicationService
{
    private readonly IBookRepository _bookRepository;

    public BookApplicationService(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<BookDto> CreateBookAsync(CreateBookDto createDto)
    {
        // 1. Create the domain entity
        var book = new Book(createDto.Isbn, createDto.Title, createDto.Author, createDto.TotalCopies);

        // 2. Persist using the repository
        await _bookRepository.AddAsync(book);

        // 3. Map to DTO and return
        return new BookDto { /* mapping logic */ };
    }

    public async Task UpdateStockAsync(int bookId, UpdateStockDto stockDto)
    {
        // 1. Get the book entity from persistence
        var book = await _bookRepository.GetByIdAsync(bookId);
        if (book == null) throw new Exception("Book not found.");

        // 2. Execute the domain logic on the entity
        if (stockDto.ChangeAmount < 0)
        {
            book.DecrementStock();
        }
        else
        {
            book.IncrementStock();
        }

        // 3. Save the updated entity back to the database
        await _bookRepository.UpdateAsync(book);
    }
}
3.3. Infrastructure Layer (LibHub.CatalogService.Infrastructure)

Contains the concrete implementations for data access.

Persistence: EfBookRepository.cs

Implements the IBookRepository interface using Entity Framework Core.

Will contain a CatalogDbContext that defines the DbSet<Book>.

The UpdateAsync method will simply call _context.SaveChangesAsync() to persist the changes made to the Book entity in the Application Layer.

3.4. Presentation Layer (LibHub.CatalogService.Api)

The ASP.NET Core Web API project.

Controller: BooksController.cs
The API endpoints are thin wrappers around the BookApplicationService.

code
C#
download
content_copy
expand_less
[ApiController]
[Route("api/books")]
public class BooksController : ControllerBase
{
    private readonly BookApplicationService _bookService;

    public BooksController(BookApplicationService bookService)
    {
        _bookService = bookService;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateBook(CreateBookDto createDto)
    {
        var bookDto = await _bookService.CreateBookAsync(createDto);
        return CreatedAtAction(nameof(GetBookById), new { id = bookDto.BookId }, bookDto);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBookById(int id) { /* Implementation using the service */ }

    [HttpPut("{id}/stock")]
    [Authorize] // This should be secured for internal service-to-service calls
    public async Task<IActionResult> UpdateStock(int id, UpdateStockDto stockDto)
    {
        await _bookService.UpdateStockAsync(id, stockDto);
        return NoContent();
    }
}

Dependency Injection (Program.cs):

services.AddDbContext<CatalogDbContext>(...)

services.AddScoped<IBookRepository, EfBookRepository>();

services.AddScoped<BookApplicationService>();
