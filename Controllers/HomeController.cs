using System.Diagnostics;
using System.Globalization;
using Lumen_Merch_Store.Data;
using Lumen_Merch_Store.Models;
using Lumen_Merch_Store.ViewModels;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lumen_Merch_Store.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    private string CurrentLang => CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

    [HttpPost]
    public IActionResult SetLanguage(string culture, string returnUrl)
    {
        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            new CookieOptions 
            { 
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                IsEssential = true,
                SameSite = SameSiteMode.Strict 
            }
        );

        return LocalRedirect(returnUrl);
    }

    public async Task<IActionResult> Index()
    {
        var langCode = CurrentLang;

        // Get bestseller products (random selection for now, could be based on order count later)
        var bestsellerProducts = await _context.Products
            .OrderByDescending(p => p.Id) // Can be changed to order by sales count
            .Take(8)
            .Select(p => new ProductCardViewModel
            {
                Id = p.Id,
                Name = p.Translations.FirstOrDefault(t => t.LanguageCode == langCode)!.Name ?? "Unknown",
                Price = p.Price,
                ImageUrl = p.ImageUrl ?? "",
                UniverseName = p.Universe.Translations.FirstOrDefault(t => t.LanguageCode == langCode)!.Name ?? "Unknown",
                UniverseId = p.UniverseId,
                CategoryId = p.CategoryId
            })
            .ToListAsync();

        // Get newest products
        var newProducts = await _context.Products
            .OrderByDescending(p => p.CreatedAt)
            .Take(4)
            .Select(p => new ProductCardViewModel
            {
                Id = p.Id,
                Name = p.Translations.FirstOrDefault(t => t.LanguageCode == langCode)!.Name ?? "Unknown",
                Price = p.Price,
                ImageUrl = p.ImageUrl ?? "",
                UniverseName = p.Universe.Translations.FirstOrDefault(t => t.LanguageCode == langCode)!.Name ?? "Unknown",
                UniverseId = p.UniverseId,
                CategoryId = p.CategoryId
            })
            .ToListAsync();

        // Get featured universes
        var featuredUniverses = await _context.Universes
            .Select(u => new FilterOption
            {
                Id = u.Id,
                Name = u.Translations.FirstOrDefault(t => t.LanguageCode == langCode)!.Name ?? "Unknown",
                Count = u.Products.Count
            })
            .Take(6)
            .ToListAsync();

        var model = new HomeViewModel
        {
            BestsellerProducts = bestsellerProducts,
            NewProducts = newProducts,
            FeaturedUniverses = featuredUniverses
        };

        return View(model);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}