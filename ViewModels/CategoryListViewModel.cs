using System.ComponentModel.DataAnnotations;

namespace Lumen_Merch_Store.ViewModels;

public class CategoryListViewModel
{
    public List<FilterOption> Categories { get; set; } = new List<FilterOption>();
    
    public string CurrentSearchTerm { get; set; } = string.Empty;
}