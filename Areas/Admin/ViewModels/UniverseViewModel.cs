// Lumen_Merch_Store.Areas.Admin.ViewModels/UniverseViewModel.cs

using System.ComponentModel.DataAnnotations;

namespace Lumen_Merch_Store.Areas.Admin.ViewModels;

public class UniverseViewModel
{
    public int Id { get; set; }

    // Поля перекладу (Українська)
    public List<TranslationViewModel> Translations { get; set; } = new();
    public string? NameForGrid { get; set; }
}