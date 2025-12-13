using Lumen_Merch_Store.Data;
using Lumen_Merch_Store.Models;
using Lumen_Merch_Store.Models.Enums;
using Lumen_Merch_Store.Services;
using Lumen_Merch_Store.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lumen_Merch_Store.Controllers;

public class CartController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ICartService _cartService;
    private readonly ITelegramNotificationService _telegramService;
    private readonly UserManager<ApplicationUser> _userManager;

    public CartController(
        ApplicationDbContext context,
        ICartService cartService,
        ITelegramNotificationService telegramService,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _cartService = cartService;
        _telegramService = telegramService;
        _userManager = userManager;
    }

    private string GetCurrentLanguage()
    {
        var culture = HttpContext.Features.Get<Microsoft.AspNetCore.Localization.IRequestCultureFeature>();
        return culture?.RequestCulture.UICulture.TwoLetterISOLanguageName ?? "uk";
    }

    // GET: /Cart
    public IActionResult Index()
    {
        var cart = _cartService.GetCart();
        return View("Index", cart);
    }

    // POST: /Cart/Add
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add([FromBody] AddToCartViewModel model)
    {
        var lang = GetCurrentLanguage();
        
        var product = await _context.Products
            .Include(p => p.Translations.Where(t => t.LanguageCode == lang))
            .Include(p => p.ProductSizes)
            .FirstOrDefaultAsync(p => p.Id == model.ProductId);

        if (product == null)
        {
            return Json(new { success = false, message = "Product not found" });
        }

        var productName = product.Translations.FirstOrDefault()?.Name ?? "Product";
        var imageUrl = product.ImageUrl ?? "";
        string? sizeName = null;

        if (model.SizeId.HasValue)
        {
            var size = product.ProductSizes.FirstOrDefault(s => s.Id == model.SizeId);
            sizeName = size?.Size;
        }

        _cartService.AddToCart(
            product.Id,
            productName,
            imageUrl,
            product.Price,
            model.Quantity,
            model.SizeId,
            sizeName);

        return Json(new { 
            success = true, 
            cartCount = _cartService.GetCartItemCount(),
            message = "Product added to cart"
        });
    }

    // POST: /Cart/Update
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Update(int productId, int? sizeId, int quantity)
    {
        _cartService.UpdateQuantity(productId, sizeId, quantity);
        return RedirectToAction(nameof(Index));
    }

    // POST: /Cart/Remove
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Remove(int productId, int? sizeId)
    {
        _cartService.RemoveFromCart(productId, sizeId);
        return RedirectToAction(nameof(Index));
    }

    // POST: /Cart/RemoveAjax
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult RemoveAjax([FromBody] RemoveFromCartRequest request)
    {
        _cartService.RemoveFromCart(request.ProductId, request.SizeId);
        var cart = _cartService.GetCart();
        return Json(new { 
            success = true, 
            cartCount = cart.TotalItems,
            subtotal = cart.Subtotal
        });
    }

    // GET: /Cart/Checkout
    public async Task<IActionResult> Checkout()
    {
        var cart = _cartService.GetCart();
        
        if (!cart.Items.Any())
        {
            return RedirectToAction(nameof(Index));
        }

        var model = new CheckoutViewModel
        {
            Cart = cart
        };

        // Pre-fill user data if logged in
        if (User.Identity?.IsAuthenticated == true)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                model.Name = user.Name;
                model.Email = user.Email ?? "";
                model.Phone = user.PhoneNumber ?? "";
            }
        }

        return View(model);
    }

    // POST: /Cart/Checkout
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout(CheckoutViewModel model)
    {
        var cart = _cartService.GetCart();
        model.Cart = cart;

        if (!cart.Items.Any())
        {
            return RedirectToAction(nameof(Index));
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Create order
        var order = new Order
        {
            UserId = User.Identity?.IsAuthenticated == true 
                ? (await _userManager.GetUserAsync(User))?.Id ?? 0 
                : 0,
            Status = OrderStatus.Pending,
            Total = cart.Subtotal,
            CreatedAt = DateTime.UtcNow
        };

        // If user is not authenticated, we need to handle this differently
        // For now, we'll require authentication or handle guest orders
        if (order.UserId == 0 && User.Identity?.IsAuthenticated != true)
        {
            // For guest checkout, we can create a temporary user or handle differently
            // For simplicity, let's redirect to login
            TempData["ReturnUrl"] = Url.Action(nameof(Checkout));
            return RedirectToAction("Login", "Account");
        }

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        // Add order items
        foreach (var item in cart.Items)
        {
            var orderItem = new OrderItem
            {
                OrderId = order.Id,
                ProductId = item.ProductId,
                SizeId = item.SizeId,
                Quantity = item.Quantity,
                Price = item.Price
            };
            _context.OrderItems.Add(orderItem);
        }

        await _context.SaveChangesAsync();

        // Reload order with items for Telegram notification
        var orderWithItems = await _context.Orders
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                    .ThenInclude(p => p.Translations)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Size)
            .FirstOrDefaultAsync(o => o.Id == order.Id);

        // Send Telegram notification
        if (orderWithItems != null)
        {
            await _telegramService.SendOrderNotificationAsync(
                orderWithItems,
                model.Name,
                model.Email,
                model.Phone,
                model.DeliveryAddress,
                model.Notes);
        }

        // Clear cart
        _cartService.ClearCart();

        return RedirectToAction(nameof(OrderConfirmation), new { orderId = order.Id });
    }

    // GET: /Cart/OrderConfirmation
    public async Task<IActionResult> OrderConfirmation(int orderId)
    {
        var order = await _context.Orders
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                    .ThenInclude(p => p.Translations)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Size)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }

    // GET: /Cart/GetCartCount
    [HttpGet]
    public IActionResult GetCartCount()
    {
        return Json(new { count = _cartService.GetCartItemCount() });
    }
}

public class RemoveFromCartRequest
{
    public int ProductId { get; set; }
    public int? SizeId { get; set; }
}
