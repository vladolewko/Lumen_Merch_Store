namespace Lumen_Merch_Store.ViewModels;

public class HomeViewModel
{
    public List<ProductCardViewModel> BestsellerProducts { get; set; } = new();
    public List<ProductCardViewModel> NewProducts { get; set; } = new();
    public List<FilterOption> FeaturedUniverses { get; set; } = new();
}
