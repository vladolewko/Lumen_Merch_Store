using System.ComponentModel.DataAnnotations;

namespace Lumen_Merch_Store.ViewModels;

public class ProductDetailViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty; 
    public string FullDescription { get; set; } = string.Empty; 
    public decimal Price { get; set; }
    public string ImageUrl { get; set; } = string.Empty; 
    public string UniverseName { get; set; } = string.Empty; 
    public List<ProductSizeViewModel> Sizes { get; set; } = new List<ProductSizeViewModel>();
}