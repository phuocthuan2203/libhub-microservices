using LibHub.LoanService.Application.DTOs;
using LibHub.LoanService.Application.Interfaces;
using LibHub.LoanService.Domain.Entities;
using LibHub.LoanService.Domain.Interfaces;

namespace LibHub.LoanService.Application.Services;

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
        var availability = await _catalogService.GetBookAvailabilityAsync(createLoanDto.BookId);
        if (!availability.IsAvailable) 
            throw new Exception("Book is not available.");

        var loan = new Loan(userId, createLoanDto.BookId);
        await _loanRepository.AddAsync(loan);

        try
        {
            await _catalogService.UpdateBookStockAsync(createLoanDto.BookId, -1);
        }
        catch (Exception ex)
        {
            loan.Fail();
            await _loanRepository.UpdateAsync(loan);
            throw new Exception("Failed to update book stock. Borrowing process failed.", ex);
        }

        loan.ConfirmCheckout();
        await _loanRepository.UpdateAsync(loan);

        return new LoanDto
        {
            LoanId = loan.LoanId,
            UserId = loan.UserId,
            BookId = loan.BookId,
            Status = loan.Status,
            CheckoutDate = loan.CheckoutDate,
            DueDate = loan.DueDate,
            ReturnDate = loan.ReturnDate
        };
    }

    public async Task ReturnBookAsync(int loanId)
    {
        var loan = await _loanRepository.GetByIdAsync(loanId);
        if (loan == null)
            throw new Exception("Loan not found.");

        loan.MarkAsReturned();
        await _loanRepository.UpdateAsync(loan);

        await _catalogService.UpdateBookStockAsync(loan.BookId, 1);
    }

    public async Task<IEnumerable<LoanDto>> GetUserLoansAsync(int userId)
    {
        var loans = await _loanRepository.GetByUserIdAsync(userId);
        return loans.Select(loan => new LoanDto
        {
            LoanId = loan.LoanId,
            UserId = loan.UserId,
            BookId = loan.BookId,
            Status = loan.Status,
            CheckoutDate = loan.CheckoutDate,
            DueDate = loan.DueDate,
            ReturnDate = loan.ReturnDate
        });
    }
}
