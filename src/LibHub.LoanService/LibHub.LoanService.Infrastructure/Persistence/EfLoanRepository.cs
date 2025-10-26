using LibHub.LoanService.Domain.Entities;
using LibHub.LoanService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibHub.LoanService.Infrastructure.Persistence;

public class EfLoanRepository : ILoanRepository
{
    private readonly LoanDbContext _context;

    public EfLoanRepository(LoanDbContext context)
    {
        _context = context;
    }

    public async Task<Loan?> GetByIdAsync(int loanId)
    {
        return await _context.Loans.FindAsync(loanId);
    }

    public async Task<IEnumerable<Loan>> GetByUserIdAsync(int userId)
    {
        return await _context.Loans
            .Where(l => l.UserId == userId)
            .ToListAsync();
    }

    public async Task AddAsync(Loan loan)
    {
        await _context.Loans.AddAsync(loan);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Loan loan)
    {
        _context.Loans.Update(loan);
        await _context.SaveChangesAsync();
    }
}
