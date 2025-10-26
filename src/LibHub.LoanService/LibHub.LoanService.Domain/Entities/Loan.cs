namespace LibHub.LoanService.Domain.Entities;

public class Loan
{
    public int LoanId { get; private set; }
    public int UserId { get; private set; }
    public int BookId { get; private set; }
    public string Status { get; private set; } = string.Empty;
    public DateTime CheckoutDate { get; private set; }
    public DateTime DueDate { get; private set; }
    public DateTime? ReturnDate { get; private set; }

    private Loan() { }

    public Loan(int userId, int bookId)
    {
        UserId = userId;
        BookId = bookId;
        Status = "PENDING";
        CheckoutDate = DateTime.UtcNow;
        DueDate = DateTime.UtcNow.AddDays(14);
    }

    public void ConfirmCheckout()
    {
        if (Status != "PENDING") 
            throw new InvalidOperationException("Loan is not in a pending state.");
        Status = "CheckedOut";
    }

    public void MarkAsReturned()
    {
        if (Status != "CheckedOut") 
            throw new InvalidOperationException("Cannot return a loan that is not checked out.");
        Status = "Returned";
        ReturnDate = DateTime.UtcNow;
    }

    public void Fail()
    {
        Status = "FAILED";
    }
}
