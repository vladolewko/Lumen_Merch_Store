using Lumen_Merch_Store.Data;
using Lumen_Merch_Store.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lumen_Merch_Store.Controllers;

public class ProductsController : Controller
{
    private readonly ApplicationDbContext _context;

    public ProductsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: /Products
    public async Task<IActionResult> Index()
    {
        var products = await _context.Products
            .Include(p => p.Translations.Where(t => t.LanguageCode == "uk"))
            .Include(p => p.Universe)
                .ThenInclude(u => u.Translations.Where(t => t.LanguageCode == "uk"))
            .Include(p => p.Category)
                .ThenInclude(c => c.Translations.Where(t => t.LanguageCode == "uk"))
            .ToListAsync();

        return View(products);
    }

    // GET: /Products/View/5
    public async Task<IActionResult> View(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var product = await _context.Products
            .Include(p => p.Translations.Where(t => t.LanguageCode == "uk"))
            .Include(p => p.Universe)
                .ThenInclude(u => u.Translations.Where(t => t.LanguageCode == "uk"))
            .Include(p => p.Category)
                .ThenInclude(c => c.Translations.Where(t => t.LanguageCode == "uk"))
            .Include(p => p.ProductSizes)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }
}