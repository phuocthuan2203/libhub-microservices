using System.ComponentModel.DataAnnotations;

namespace LibHub.CatalogService.Application.DTOs;

public class UpdateStockDto
{
    [Required]
    public int ChangeAmount { get; set; }
}
