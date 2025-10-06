using System.ComponentModel.DataAnnotations;

namespace Lumen_Merch_Store.Models;

public class ProductTranslation
{
    public int Id { get; set; }

    [Required] public int ProductId { get; set; }

    [Required] [MaxLength(2)] public string LanguageCode { get; set; } = null!;

    [Required] [MaxLength(150)] public string Name { get; set; } = string.Empty;

    [MaxLength(255)] public string? ShortDescription { get; set; }

    [MaxLength(1024)] public string? FullDescription { get; set; }

    /* Relations */

    public virtual Product Product { get; set; } = null!;

    public virtual Language Language { get; set; } = null!;
}