namespace Lumen_Merch_Store.ViewModels;

public class AddToCartViewModel
{
    public int ProductId { get; set; }
    public int Quantity { get; set; } = 1;
    public int? SizeId { get; set; }
}
