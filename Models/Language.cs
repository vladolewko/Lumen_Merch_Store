using System.ComponentModel.DataAnnotations;

namespace Lumen_Merch_Store.Models;

public class Language
{
    [Key] [MaxLength(2)] public string Code { get; set; } = string.Empty; // 'en', 'uk', etc.'

    [Required] [MaxLength(100)] public string Name { get; set; } = string.Empty;

    /* Relations */

    public ICollection<ProductTranslation> ProductTranslations { get; set; } = new List<ProductTranslation>();
}