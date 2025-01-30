using System.ComponentModel.DataAnnotations;

namespace mutraw_marketplace_api.Models;

public class ProductCategory
{
    public long Id { get; set; }
    [Required]
    [MinLength(3)]
    public string Name { get; set; } = string.Empty;
}