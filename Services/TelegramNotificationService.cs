using System.Text;
using System.Text.Json;
using Lumen_Merch_Store.Models;

namespace Lumen_Merch_Store.Services;

public interface ITelegramNotificationService
{
    Task SendOrderNotificationAsync(Order order, string customerName, string customerEmail, string customerPhone, string deliveryAddress, string? notes);
}

public class TelegramNotificationService : ITelegramNotificationService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<TelegramNotificationService> _logger;

    public TelegramNotificationService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<TelegramNotificationService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendOrderNotificationAsync(Order order, string customerName, string customerEmail, string customerPhone, string deliveryAddress, string? notes)
    {
        var botToken = _configuration["Telegram:BotToken"];
        var chatId = _configuration["Telegram:ChatId"];

        if (string.IsNullOrEmpty(botToken) || string.IsNullOrEmpty(chatId))
        {
            _logger.LogWarning("Telegram bot token or chat ID not configured. Skipping notification.");
            return;
        }

        try
        {
            var message = BuildOrderMessage(order, customerName, customerEmail, customerPhone, deliveryAddress, notes);
            
            var url = $"https://api.telegram.org/bot{botToken}/sendMessage";
            
            var payload = new
            {
                chat_id = chatId,
                text = message,
                parse_mode = "HTML"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync(url, content);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed to send Telegram notification. Status: {Status}, Response: {Response}", 
                    response.StatusCode, errorContent);
            }
            else
            {
                _logger.LogInformation("Order notification sent to Telegram for order #{OrderId}", order.Id);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending Telegram notification for order #{OrderId}", order.Id);
        }
    }

    private string BuildOrderMessage(Order order, string customerName, string customerEmail, string customerPhone, string deliveryAddress, string? notes)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine("🛒 <b>НОВЕ ЗАМОВЛЕННЯ!</b>");
        sb.AppendLine();
        sb.AppendLine($"📦 <b>Замовлення №{order.Id}</b>");
        sb.AppendLine($"📅 Дата: {order.CreatedAt:dd.MM.yyyy HH:mm}");
        sb.AppendLine();
        sb.AppendLine("<b>👤 Клієнт:</b>");
        sb.AppendLine($"   Ім'я: {EscapeHtml(customerName)}");
        sb.AppendLine($"   Email: {EscapeHtml(customerEmail)}");
        sb.AppendLine($"   Телефон: {EscapeHtml(customerPhone)}");
        sb.AppendLine();
        sb.AppendLine("<b>📍 Адреса доставки:</b>");
        sb.AppendLine($"   {EscapeHtml(deliveryAddress)}");
        
        if (!string.IsNullOrEmpty(notes))
        {
            sb.AppendLine();
            sb.AppendLine("<b>📝 Примітки:</b>");
            sb.AppendLine($"   {EscapeHtml(notes)}");
        }
        
        sb.AppendLine();
        sb.AppendLine("<b>🛍 Товари:</b>");
        
        foreach (var item in order.OrderItems)
        {
            var productName = item.Product?.Translations?.FirstOrDefault()?.Name ?? $"Product #{item.ProductId}";
            var sizePart = item.Size != null ? $" ({item.Size.Size})" : "";
            sb.AppendLine($"   • {EscapeHtml(productName)}{sizePart}");
            sb.AppendLine($"     {item.Quantity} x {item.Price:N2} ₴ = {item.Quantity * item.Price:N2} ₴");
        }
        
        sb.AppendLine();
        sb.AppendLine($"<b>💰 Загальна сума: {order.Total:N2} ₴</b>");
        
        return sb.ToString();
    }

    private static string EscapeHtml(string text)
    {
        return text
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;");
    }
}
