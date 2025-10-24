using System.ComponentModel.DataAnnotations;

namespace Lumen_Merch_Store.ViewModels;

public class FilterOption
{
    public int Id { get; set; }
    
    // Ініціалізовано, щоб уникнути null
    public string Name { get; set; } = string.Empty;
    
    public int? Count { get; set; } 
}