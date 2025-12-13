namespace Lumen_Merch_Store.ViewModels;

public class CartItemViewModel
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public int? SizeId { get; set; }
    public string? SizeName { get; set; }
    public decimal Total => Price * Quantity;
}

