using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mutraw_marketplace_api.Models;

public class Product
{
    public long Id { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal SellPrice { get; set; }
    [Required]
    public Uom DefaultUom { get; set; }
    [Required]
    public long ProductSubCategoryId { get; set; }
}