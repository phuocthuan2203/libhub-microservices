using LibHub.CatalogService.Domain.Entities;
using LibHub.CatalogService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibHub.CatalogService.Infrastructure.Persistence;

public class EfBookRepository : IBookRepository
{
    private readonly CatalogDbContext _context;

    public EfBookRepository(CatalogDbContext context)
    {
        _context = context;
    }

    public async Task<Book?> GetByIdAsync(int bookId)
    {
        return await _context.Books.FindAsync(bookId);
    }

    public async Task<IEnumerable<Book>> GetAllAsync()
    {
        return await _context.Books.ToListAsync();
    }

    public async Task AddAsync(Book book)
    {
        await _context.Books.AddAsync(book);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Book book)
    {
        _context.Books.Update(book);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Book book)
    {
        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
    }
}
