using System.ComponentModel.DataAnnotations;

namespace LibHub.CatalogService.Application.DTOs;

public class CreateBookDto
{
    [Required]
    public string Isbn { get; set; } = string.Empty;
    
    [Required]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    public string Author { get; set; } = string.Empty;
    
    public string? Genre { get; set; }
    
    public string? Description { get; set; }
    
    [Range(1, int.MaxValue, ErrorMessage = "Total copies must be greater than 0")]
    public int TotalCopies { get; set; }
}
