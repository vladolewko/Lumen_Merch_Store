using System.ComponentModel.DataAnnotations;

namespace Lumen_Merch_Store.ViewModels;

public class ProductsCatalogViewModel
{
    public List<ProductCardViewModel> Products { get; set; } = new List<ProductCardViewModel>();
    public List<FilterOption> Categories { get; set; } = new List<FilterOption>();
    public List<FilterOption> Universes { get; set; } = new List<FilterOption>();
    
    public int TotalProductsCount { get; set; }
    
    public string CurrentSortOrder { get; set; } = string.Empty;
    public string CurrentSearchTerm { get; set; } = string.Empty;
    
    public decimal MinPrice { get; set; }
    public decimal MaxPrice { get; set; }
    
    public decimal? SelectedMinPrice { get; set; }
    public decimal? SelectedMaxPrice { get; set; }

    public int? SelectedCategoryId { get; set; }
    public int? SelectedUniverseId { get; set; }
}