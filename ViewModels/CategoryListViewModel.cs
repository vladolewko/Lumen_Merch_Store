using System.ComponentModel.DataAnnotations;

namespace Lumen_Merch_Store.ViewModels;

public class CategoryListViewModel
{
    // Змінено на List<> та ініціалізовано, щоб уникнути NullReferenceException у View
    public List<FilterOption> Categories { get; set; } = new List<FilterOption>();
    
    // Ініціалізовано, щоб уникнути null
    public string CurrentSearchTerm { get; set; } = string.Empty;
}