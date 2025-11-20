using Lumen_Merch_Store.Data;
using Lumen_Merch_Store.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Lumen_Merch_Store.Controllers
{
    public class CatalogueController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int DefaultPageSize = 12;

        public CatalogueController(ApplicationDbContext context)
        {
            _context = context;
        }

        private string CurrentLang => CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

        [HttpGet]
        public async Task<IActionResult> Products(
            string search,
            int? categoryId,
            int? universeId,
            string sortOrder,
            decimal? minPrice,
            decimal? maxPrice,
            int page = 1) 
        {
            var langCode = CurrentLang;

            var dbMinPrice = await _context.Products.MinAsync(p => (decimal?)p.Price) ?? 0;
            var dbMaxPrice = await _context.Products.MaxAsync(p => (decimal?)p.Price) ?? 0;

            var productsQuery = _context.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                string lowerSearch = search.ToLower();
                productsQuery = productsQuery.Where(p => p.Translations.Any(t => 
                    t.LanguageCode == langCode && t.Name.ToLower().Contains(lowerSearch)));
            }

            if (categoryId.HasValue && categoryId.Value != 0)
                productsQuery = productsQuery.Where(p => p.CategoryId == categoryId.Value);

            if (universeId.HasValue && universeId.Value != 0)
                productsQuery = productsQuery.Where(p => p.UniverseId == universeId.Value);

            if (minPrice.HasValue)
                productsQuery = productsQuery.Where(p => p.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                productsQuery = productsQuery.Where(p => p.Price <= maxPrice.Value);

            productsQuery = sortOrder switch
            {
                "price-asc" => productsQuery.OrderBy(p => p.Price),
                "price-desc" => productsQuery.OrderByDescending(p => p.Price),
                "newest" => productsQuery.OrderByDescending(p => p.Id),
                _ => productsQuery.OrderBy(p => p.Id),
            };

            var totalCount = await productsQuery.CountAsync();
            var products = await productsQuery
                .Skip((page - 1) * DefaultPageSize)
                .Take(DefaultPageSize)
                .Select(p => new ProductCardViewModel
                {
                    Id = p.Id,
                    Name = p.Translations.FirstOrDefault(t => t.LanguageCode == langCode)!.Name,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl,
                    UniverseName = p.Universe.Translations.FirstOrDefault(t => t.LanguageCode == langCode)!.Name,
                    UniverseId = p.UniverseId,
                    CategoryId = p.CategoryId
                })
                .ToListAsync();

            var categories = await GetCategoryFilterOptionsAsync(langCode);
            var universes = await GetUniverseFilterOptionsAsync(langCode);

            var model = new ProductsCatalogViewModel
            {
                Products = products, 
                TotalProductsCount = totalCount,
                Categories = categories, 
                Universes = universes,
                MinPrice = dbMinPrice, 
                MaxPrice = dbMaxPrice,
                CurrentSortOrder = sortOrder,
                CurrentSearchTerm = search,
                SelectedMinPrice = minPrice,
                SelectedMaxPrice = maxPrice,
                SelectedCategoryId = categoryId,
                SelectedUniverseId = universeId,
            };

            return View(model);
        }
        
        [HttpGet]
        public async Task<IActionResult> Product(int id)
        {
            var langCode = CurrentLang;

            // Тут ми не використовуємо Select, а завантажуємо об'єкт, тому Include.Where працює краще
            var product = await _context.Products
                .Include(p => p.Translations.Where(t => t.LanguageCode == langCode)) // Спробує завантажити тільки потрібну мову
                .Include(p => p.Universe).ThenInclude(u => u.Translations.Where(t => t.LanguageCode == langCode))
                .Include(p => p.ProductSizes)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            // Якщо Include не знайшов переклад (наприклад, його немає), беремо дефолтний запитом
            // Але для простоти поки припускаємо, що він є, або виводимо заглушку.
            var translation = product.Translations.FirstOrDefault(); 

            var model = new ProductDetailViewModel
            {
                Id = product.Id,
                Name = translation?.Name ?? "No Translation",
                FullDescription = translation?.FullDescription ?? "",
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                UniverseName = product.Universe?.Translations.FirstOrDefault()?.Name ?? "Unknown",
                Sizes = product.ProductSizes.Select(ps => new ProductSizeViewModel
                {
                    Id = ps.Id,
                    Size = ps.Size,
                    Stock = ps.Stock
                }).ToList()
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Categories(string search)
        {
            var langCode = CurrentLang;
            var categories = await GetCategoryFilterOptionsAsync(langCode);

            if (!string.IsNullOrWhiteSpace(search))
            {
                string lowerSearch = search.ToLower();
                categories = categories.Where(c => c.Name.ToLower().Contains(lowerSearch)).ToList();
            }

            return View(new CategoryListViewModel { Categories = categories, CurrentSearchTerm = search });
        }

        [HttpGet]
        public async Task<IActionResult> Universes(string search)
        {
            var langCode = CurrentLang;
            var universes = await GetUniverseFilterOptionsAsync(langCode);

            if (!string.IsNullOrWhiteSpace(search))
            {
                string lowerSearch = search.ToLower();
                universes = universes.Where(u => u.Name.ToLower().Contains(lowerSearch)).ToList();
            }

            return View(new UniverseListViewModel { Universes = universes, CurrentSearchTerm = search });
        }

        // === ВИПРАВЛЕНІ ДОПОМІЖНІ МЕТОДИ ===
        
        private async Task<List<FilterOption>> GetCategoryFilterOptionsAsync(string langCode)
        {
            var categoryCounts = await _context.Products
                .GroupBy(p => p.CategoryId)
                .Select(g => new { Id = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Id, x => x.Count);

            // ВИПРАВЛЕНО: Фільтрація мови прямо в Select
            var categories = await _context.Categories
                .Select(c => new FilterOption
                {
                    Id = c.Id,
                    Name = c.Translations.FirstOrDefault(t => t.LanguageCode == langCode).Name 
                           ?? c.Translations.FirstOrDefault().Name // Фолбек
                })
                .ToListAsync();

            foreach (var category in categories)
            {
                category.Count = categoryCounts.GetValueOrDefault(category.Id);
            }

            return categories;
        }

        private async Task<List<FilterOption>> GetUniverseFilterOptionsAsync(string langCode)
        {
            var universeCounts = await _context.Products
                .GroupBy(p => p.UniverseId)
                .Where(g => g.Key != 0)
                .Select(g => new { Id = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Id, x => x.Count);

            // ВИПРАВЛЕНО: Фільтрація мови прямо в Select
            var universes = await _context.Universes
                .Select(u => new FilterOption
                {
                    Id = u.Id,
                    Name = u.Translations.FirstOrDefault(t => t.LanguageCode == langCode).Name
                })
                .ToListAsync();

            foreach (var universe in universes)
            {
                universe.Count = universeCounts.GetValueOrDefault(universe.Id);
            }

            return universes;
        }
    }
}