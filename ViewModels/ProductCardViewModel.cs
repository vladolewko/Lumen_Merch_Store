using System.ComponentModel.DataAnnotations;

namespace Lumen_Merch_Store.ViewModels;

public class ProductCardViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }  = String.Empty;
    public string ShortDescription { get; set; }  = String.Empty;
    public decimal Price { get; set; }
    public string ImageUrl { get; set; }  = String.Empty;
    public string UniverseName { get; set; }  = String.Empty;
    public int UniverseId { get; set; }
    public int CategoryId { get; set; }
}
