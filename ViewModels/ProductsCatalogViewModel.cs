using System.ComponentModel.DataAnnotations;

namespace Lumen_Merch_Store.ViewModels;

public class ProductsCatalogViewModel
{
    // Усі колекції ініціалізовано
    public List<ProductCardViewModel> Products { get; set; } = new List<ProductCardViewModel>();
    public List<FilterOption> Categories { get; set; } = new List<FilterOption>();
    public List<FilterOption> Universes { get; set; } = new List<FilterOption>();
    
    public int TotalProductsCount { get; set; }
    
    // Усі рядки ініціалізовано
    public string CurrentSortOrder { get; set; } = string.Empty;
    public string CurrentSearchTerm { get; set; } = string.Empty;
    
    public decimal MinPrice { get; set; }
    public decimal MaxPrice { get; set; }

    // Змінено на nullable (decimal?), щоб коректно представляти 
    // "ціну не обрано" (null), а не 0.
    // Це відповідає логіці з вашого .cshtml
    public decimal? SelectedMinPrice { get; set; }
    public decimal? SelectedMaxPrice { get; set; }

    public int? SelectedCategoryId { get; set; }
    public int? SelectedUniverseId { get; set; }
}