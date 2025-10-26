using System.ComponentModel.DataAnnotations;

namespace LibHub.CatalogService.Application.DTOs;

public class UpdateBookDto
{
    [Required]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    public string Author { get; set; } = string.Empty;
    
    public string? Genre { get; set; }
    
    public string? Description { get; set; }
}
