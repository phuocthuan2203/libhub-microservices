using LibHub.CatalogService.Domain.Entities;

namespace LibHub.CatalogService.Domain.Interfaces;

public interface IBookRepository
{
    Task<Book?> GetByIdAsync(int bookId);
    Task<IEnumerable<Book>> GetAllAsync();
    Task AddAsync(Book book);
    Task UpdateAsync(Book book);
    Task DeleteAsync(Book book);
}
