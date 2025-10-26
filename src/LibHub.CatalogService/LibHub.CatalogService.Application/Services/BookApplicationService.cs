using LibHub.CatalogService.Application.DTOs;
using LibHub.CatalogService.Domain.Entities;
using LibHub.CatalogService.Domain.Interfaces;

namespace LibHub.CatalogService.Application.Services;

public class BookApplicationService
{
    private readonly IBookRepository _bookRepository;

    public BookApplicationService(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<BookDto> CreateBookAsync(CreateBookDto createDto)
    {
        var book = new Book(createDto.Isbn, createDto.Title, createDto.Author, createDto.TotalCopies);
        
        if (!string.IsNullOrWhiteSpace(createDto.Genre) || !string.IsNullOrWhiteSpace(createDto.Description))
        {
            book.UpdateDetails(createDto.Title, createDto.Author, createDto.Genre, createDto.Description);
        }

        await _bookRepository.AddAsync(book);

        return MapToDto(book);
    }

    public async Task<BookDto?> GetBookByIdAsync(int bookId)
    {
        var book = await _bookRepository.GetByIdAsync(bookId);
        return book != null ? MapToDto(book) : null;
    }

    public async Task<IEnumerable<BookDto>> GetAllBooksAsync()
    {
        var books = await _bookRepository.GetAllAsync();
        return books.Select(MapToDto);
    }

    public async Task<bool> UpdateBookAsync(int bookId, UpdateBookDto updateDto)
    {
        var book = await _bookRepository.GetByIdAsync(bookId);
        if (book == null)
            return false;

        book.UpdateDetails(updateDto.Title, updateDto.Author, updateDto.Genre, updateDto.Description);
        await _bookRepository.UpdateAsync(book);
        
        return true;
    }

    public async Task<bool> DeleteBookAsync(int bookId)
    {
        var book = await _bookRepository.GetByIdAsync(bookId);
        if (book == null)
            return false;

        await _bookRepository.DeleteAsync(book);
        return true;
    }

    public async Task<bool> UpdateStockAsync(int bookId, UpdateStockDto stockDto)
    {
        var book = await _bookRepository.GetByIdAsync(bookId);
        if (book == null)
            return false;

        if (stockDto.ChangeAmount < 0)
        {
            for (int i = 0; i < Math.Abs(stockDto.ChangeAmount); i++)
            {
                book.DecrementStock();
            }
        }
        else if (stockDto.ChangeAmount > 0)
        {
            for (int i = 0; i < stockDto.ChangeAmount; i++)
            {
                book.IncrementStock();
            }
        }

        await _bookRepository.UpdateAsync(book);
        return true;
    }

    private static BookDto MapToDto(Book book)
    {
        return new BookDto
        {
            BookId = book.BookId,
            Isbn = book.Isbn,
            Title = book.Title,
            Author = book.Author,
            Genre = book.Genre,
            Description = book.Description,
            TotalCopies = book.TotalCopies,
            AvailableCopies = book.AvailableCopies
        };
    }
}
