namespace mutraw_marketplace_api.Models;

public class Product
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public decimal SellPrice { get; set; }
    public Uom DefaultUom { get; set; }
    public long ProductSubCategoryId { get; set; }
}