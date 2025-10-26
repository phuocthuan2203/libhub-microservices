using LibHub.CatalogService.Application.DTOs;
using LibHub.CatalogService.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibHub.CatalogService.Api.Controllers;

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
    public async Task<IActionResult> CreateBook([FromBody] CreateBookDto createDto)
    {
        try
        {
            var bookDto = await _bookService.CreateBookAsync(createDto);
            return CreatedAtAction(nameof(GetBookById), new { id = bookDto.BookId }, bookDto);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occurred while creating the book");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllBooks()
    {
        try
        {
            var books = await _bookService.GetAllBooksAsync();
            return Ok(books);
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occurred while retrieving books");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBookById(int id)
    {
        try
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null)
                return NotFound();

            return Ok(book);
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occurred while retrieving the book");
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateBook(int id, [FromBody] UpdateBookDto updateDto)
    {
        try
        {
            var success = await _bookService.UpdateBookAsync(id, updateDto);
            if (!success)
                return NotFound();

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occurred while updating the book");
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteBook(int id)
    {
        try
        {
            var success = await _bookService.DeleteBookAsync(id);
            if (!success)
                return NotFound();

            return NoContent();
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occurred while deleting the book");
        }
    }

    [HttpPut("{id}/stock")]
    [Authorize]
    public async Task<IActionResult> UpdateStock(int id, [FromBody] UpdateStockDto stockDto)
    {
        try
        {
            var success = await _bookService.UpdateStockAsync(id, stockDto);
            if (!success)
                return NotFound();

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occurred while updating stock");
        }
    }
}
