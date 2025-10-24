using System.ComponentModel.DataAnnotations;

namespace Lumen_Merch_Store.ViewModels;

public class UniverseListViewModel
{
    // Змінено на List<> та ініціалізовано
    public List<FilterOption> Universes { get; set; } = new List<FilterOption>();
    
    // Ініціалізовано, щоб уникнути null
    public string CurrentSearchTerm { get; set; } = string.Empty;
}