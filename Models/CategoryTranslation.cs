using System.ComponentModel.DataAnnotations;

namespace Lumen_Merch_Store.Models;

public class CategoryTranslation
{
    public int Id { get; set; }

    [Required] public int CategoryId { get; set; }

    [Required] [MaxLength(2)] public string LanguageCode { get; set; } = null!;

    [Required] [MaxLength(100)] public string Name { get; set; } = string.Empty;

    [MaxLength(255)] public string? Description { get; set; }

    /* Relations */
    public virtual Category Category { get; set; } = null!;
    public virtual Language Language { get; set; } = null!;
}
