namespace Lumen_Merch_Store.ViewModels;

public class CartViewModel
{
    public List<CartItemViewModel> Items { get; set; } = new();
    public decimal Subtotal => Items.Sum(i => i.Total);
    public int TotalItems => Items.Sum(i => i.Quantity);
}

