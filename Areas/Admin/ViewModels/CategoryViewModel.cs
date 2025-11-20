using System.ComponentModel.DataAnnotations;

namespace Lumen_Merch_Store.Areas.Admin.ViewModels;

// ViewModel для відображення, створення та редагування Категорій
public class CategoryViewModel
{
    public int Id { get; set; }
    public List<TranslationViewModel> Translations { get; set; } = new();
        
    // Для таблиці
    public string? NameForGrid { get; set; }
}