using LibHub.LoanService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibHub.LoanService.Infrastructure.Persistence;

public class LoanDbContext : DbContext
{
    public LoanDbContext(DbContextOptions<LoanDbContext> options) : base(options)
    {
    }

    public DbSet<Loan> Loans { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Loan>(entity =>
        {
            entity.HasKey(e => e.LoanId);
            entity.Property(e => e.LoanId).ValueGeneratedOnAdd();
            entity.Property(e => e.UserId).IsRequired();
            entity.Property(e => e.BookId).IsRequired();
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.CheckoutDate).IsRequired();
            entity.Property(e => e.DueDate).IsRequired();
            entity.Property(e => e.ReturnDate);
        });

        base.OnModelCreating(modelBuilder);
    }
}
