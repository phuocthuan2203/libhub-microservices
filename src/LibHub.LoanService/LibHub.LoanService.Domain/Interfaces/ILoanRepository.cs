using LibHub.LoanService.Domain.Entities;

namespace LibHub.LoanService.Domain.Interfaces;

public interface ILoanRepository
{
    Task<Loan?> GetByIdAsync(int loanId);
    Task<IEnumerable<Loan>> GetByUserIdAsync(int userId);
    Task AddAsync(Loan loan);
    Task UpdateAsync(Loan loan);
}
