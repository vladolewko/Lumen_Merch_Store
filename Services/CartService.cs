using System.Text.Json;
using Lumen_Merch_Store.ViewModels;

namespace Lumen_Merch_Store.Services;

public interface ICartService
{
    CartViewModel GetCart();
    void AddToCart(int productId, string productName, string imageUrl, decimal price, int quantity, int? sizeId, string? sizeName);
    void UpdateQuantity(int productId, int? sizeId, int quantity);
    void RemoveFromCart(int productId, int? sizeId);
    void ClearCart();
    int GetCartItemCount();
}

public class CartService : ICartService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private const string CartSessionKey = "ShoppingCart";

    public CartService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ISession Session => _httpContextAccessor.HttpContext?.Session 
        ?? throw new InvalidOperationException("No HTTP context available");

    public CartViewModel GetCart()
    {
        var cartJson = Session.GetString(CartSessionKey);
        if (string.IsNullOrEmpty(cartJson))
        {
            return new CartViewModel();
        }

        return JsonSerializer.Deserialize<CartViewModel>(cartJson) ?? new CartViewModel();
    }

    private void SaveCart(CartViewModel cart)
    {
        var cartJson = JsonSerializer.Serialize(cart);
        Session.SetString(CartSessionKey, cartJson);
    }

    public void AddToCart(int productId, string productName, string imageUrl, decimal price, int quantity, int? sizeId, string? sizeName)
    {
        var cart = GetCart();
        
        var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId && i.SizeId == sizeId);
        
        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
        }
        else
        {
            cart.Items.Add(new CartItemViewModel
            {
                ProductId = productId,
                ProductName = productName,
                ImageUrl = imageUrl,
                Price = price,
                Quantity = quantity,
                SizeId = sizeId,
                SizeName = sizeName
            });
        }

        SaveCart(cart);
    }

    public void UpdateQuantity(int productId, int? sizeId, int quantity)
    {
        var cart = GetCart();
        var item = cart.Items.FirstOrDefault(i => i.ProductId == productId && i.SizeId == sizeId);
        
        if (item != null)
        {
            if (quantity <= 0)
            {
                cart.Items.Remove(item);
            }
            else
            {
                item.Quantity = quantity;
            }
            SaveCart(cart);
        }
    }

    public void RemoveFromCart(int productId, int? sizeId)
    {
        var cart = GetCart();
        var item = cart.Items.FirstOrDefault(i => i.ProductId == productId && i.SizeId == sizeId);
        
        if (item != null)
        {
            cart.Items.Remove(item);
            SaveCart(cart);
        }
    }

    public void ClearCart()
    {
        Session.Remove(CartSessionKey);
    }

    public int GetCartItemCount()
    {
        return GetCart().TotalItems;
    }
}
