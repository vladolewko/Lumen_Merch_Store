using System.ComponentModel.DataAnnotations;

namespace Lumen_Merch_Store.ViewModels;

public class ProductSizeViewModel
{
    public int Id { get; set; }
    
    // Ініціалізовано, щоб уникнути null
    public string Size { get; set; } = string.Empty;
    
    public int Stock { get; set; }
}