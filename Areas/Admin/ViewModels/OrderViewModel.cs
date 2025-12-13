using Lumen_Merch_Store.Models.Enums;

namespace Lumen_Merch_Store.Areas.Admin.ViewModels;

public class OrderViewModel
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string? CustomerPhone { get; set; }
    public OrderStatus Status { get; set; }
    public decimal Total { get; set; }
    public DateTime CreatedAt { get; set; }
    public int ItemsCount { get; set; }
}

