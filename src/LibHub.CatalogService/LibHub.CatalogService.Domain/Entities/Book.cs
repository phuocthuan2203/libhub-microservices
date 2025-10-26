namespace LibHub.CatalogService.Domain.Entities;

public class Book
{
    public int BookId { get; private set; }
    public string Isbn { get; private set; }
    public string Title { get; private set; }
    public string Author { get; private set; }
    public string? Genre { get; private set; }
    public string? Description { get; private set; }
    public int TotalCopies { get; private set; }
    public int AvailableCopies { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private Book() { }

    public Book(string isbn, string title, string author, int totalCopies)
    {
        if (string.IsNullOrWhiteSpace(isbn))
            throw new ArgumentException("ISBN cannot be null or empty", nameof(isbn));
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be null or empty", nameof(title));
        if (string.IsNullOrWhiteSpace(author))
            throw new ArgumentException("Author cannot be null or empty", nameof(author));
        if (totalCopies <= 0)
            throw new ArgumentException("Total copies must be greater than zero", nameof(totalCopies));

        Isbn = isbn;
        Title = title;
        Author = author;
        TotalCopies = totalCopies;
        AvailableCopies = totalCopies;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateDetails(string title, string author, string? genre = null, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be null or empty", nameof(title));
        if (string.IsNullOrWhiteSpace(author))
            throw new ArgumentException("Author cannot be null or empty", nameof(author));

        Title = title;
        Author = author;
        Genre = genre;
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }

    public void DecrementStock()
    {
        if (AvailableCopies <= 0)
        {
            throw new InvalidOperationException("No copies of this book are available for loan.");
        }
        AvailableCopies--;
        UpdatedAt = DateTime.UtcNow;
    }

    public void IncrementStock()
    {
        if (AvailableCopies >= TotalCopies)
        {
            throw new InvalidOperationException("Cannot increment stock beyond the total number of copies.");
        }
        AvailableCopies++;
        UpdatedAt = DateTime.UtcNow;
    }
}
