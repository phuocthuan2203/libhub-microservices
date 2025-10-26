using LibHub.CatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibHub.CatalogService.Infrastructure.Persistence;

public class CatalogDbContext : DbContext
{
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options)
    {
    }

    public DbSet<Book> Books { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.BookId);
            
            entity.Property(e => e.BookId)
                .HasColumnName("book_id")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.Isbn)
                .HasColumnName("isbn")
                .HasMaxLength(13)
                .IsRequired();

            entity.HasIndex(e => e.Isbn)
                .IsUnique();

            entity.Property(e => e.Title)
                .HasColumnName("title")
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(e => e.Author)
                .HasColumnName("author")
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(e => e.Genre)
                .HasColumnName("genre")
                .HasMaxLength(100);

            entity.Property(e => e.Description)
                .HasColumnName("description")
                .HasColumnType("text");

            entity.Property(e => e.TotalCopies)
                .HasColumnName("total_copies")
                .HasDefaultValue(1)
                .IsRequired();

            entity.Property(e => e.AvailableCopies)
                .HasColumnName("available_copies")
                .HasDefaultValue(1)
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .IsRequired();

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at");
        });
    }
}
