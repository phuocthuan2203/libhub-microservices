using LibHub.LoanService.Application.DTOs;

namespace LibHub.LoanService.Application.Interfaces;

public interface ICatalogService
{
    Task<BookAvailabilityDto> GetBookAvailabilityAsync(int bookId);
    Task UpdateBookStockAsync(int bookId, int changeAmount);
}
