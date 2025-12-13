using Lumen_Merch_Store.Areas.Admin.ViewModels;
using Lumen_Merch_Store.Data;
using Lumen_Merch_Store.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lumen_Merch_Store.Areas.Admin.Controllers;

[Area("Admin")]
public class OrdersController : Controller
{
    private readonly ApplicationDbContext _context;

    public OrdersController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: /Admin/Orders
    public async Task<IActionResult> Index(OrderStatus? status = null)
    {
        var query = _context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderItems)
            .AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(o => o.Status == status.Value);
        }

        var orders = await query
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new OrderViewModel
            {
                Id = o.Id,
                CustomerName = o.User.Name,
                CustomerEmail = o.User.Email ?? "",
                CustomerPhone = o.User.PhoneNumber,
                Status = o.Status,
                Total = o.Total,
                CreatedAt = o.CreatedAt,
                ItemsCount = o.OrderItems.Sum(oi => oi.Quantity)
            })
            .ToListAsync();

        ViewBag.CurrentStatus = status;
        ViewBag.Statuses = Enum.GetValues<OrderStatus>();
        
        return View(orders);
    }

    // GET: /Admin/Orders/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var order = await _context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                    .ThenInclude(p => p.Translations)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Size)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
        {
            return NotFound();
        } 

        var viewModel = new OrderDetailViewModel
        {
            Id = order.Id,
            CustomerName = order.User.Name,
            CustomerEmail = order.User.Email ?? "",
            CustomerPhone = order.User.PhoneNumber,
            Status = order.Status,
            Total = order.Total,
            CreatedAt = order.CreatedAt,
            Items = order.OrderItems.Select(oi => new OrderItemViewModel
            {
                Id = oi.Id,
                ProductId = oi.ProductId,
                ProductName = oi.Product.Translations.FirstOrDefault()?.Name ?? $"Product #{oi.ProductId}",
                ProductImage = oi.Product.ImageUrl,
                SizeName = oi.Size?.Size,
                Quantity = oi.Quantity,
                Price = oi.Price
            }).ToList()
        };

        ViewBag.Statuses = Enum.GetValues<OrderStatus>();
        
        return View(viewModel);
    }

    // POST: /Admin/Orders/UpdateStatus/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, OrderStatus status)
    {
        var order = await _context.Orders.FindAsync(id);
        
        if (order == null)
        {
            return NotFound();
        }

        order.Status = status;
        await _context.SaveChangesAsync();

        TempData["Success"] = $"Order #{id} status updated to {status}";
        
        return RedirectToAction(nameof(Details), new { id });
    }

    // POST: /Admin/Orders/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
        {
            return NotFound();
        }

        _context.OrderItems.RemoveRange(order.OrderItems);
        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();

        TempData["Success"] = $"Order #{id} has been deleted";
        
        return RedirectToAction(nameof(Index));
    }
}
