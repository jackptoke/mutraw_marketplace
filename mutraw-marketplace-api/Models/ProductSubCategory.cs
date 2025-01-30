using System.ComponentModel.DataAnnotations;

namespace mutraw_marketplace_api.Models;

public class ProductSubCategory
{
    public long Id { get; set; }
    [Required]
    [MinLength(3)]
    public string Name { get; set; } = string.Empty;
    [Required]
    public long ProductCategoryId { get; set; }
}