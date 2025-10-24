using Lumen_Merch_Store.Data;
using Lumen_Merch_Store.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic; // Додано для Dictionary та List
using System; // Додано для Math.Ceiling

namespace Lumen_Merch_Store.Controllers
{
    public class CatalogueController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const string DefaultLanguageCode = "uk"; // Код мови за замовчуванням
        private const int DefaultPageSize = 12; // Кількість товарів на сторінці

        public CatalogueController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Допоміжний метод для отримання поточної мови
        private string GetCurrentLanguageCode()
        {
            // У реальному проекті тут має бути логіка отримання поточної мови
            return DefaultLanguageCode;
        }

        // ------------------------------------------------------------------
        // ## 1. Товари (Каталог)
        // ------------------------------------------------------------------
        // ... (у вашому CatalogueController.cs) ...

        [HttpGet]
        public async Task<IActionResult> Products(
            string search,
            int? categoryId,
            int? universeId,
            string sortOrder,
            decimal? minPrice,
            decimal? maxPrice,
            int page = 1) // Додано параметр для пагінації
        {
            var langCode = GetCurrentLanguageCode();

            // 1. Отримання загального діапазону цін (для слайдера/плейсхолдерів)
            // ВИКОНУЄМО ПОСЛІДОВНО
            var dbMinPrice = await _context.Products.MinAsync(p => (decimal?)p.Price) ?? 0;
            var dbMaxPrice = await _context.Products.MaxAsync(p => (decimal?)p.Price) ?? 0;

            // 2. Базовий запит для товарів
            var productsQuery = _context.Products
                .Include(p => p.Translations.Where(t => t.LanguageCode == langCode))
                .Include(p => p.Universe).ThenInclude(u => u.Translations.Where(t => t.LanguageCode == langCode))
                .AsQueryable();

            // 3. Фільтрація
            if (!string.IsNullOrWhiteSpace(search))
            {
                string lowerSearch = search.ToLower();
                productsQuery =
                    productsQuery.Where(p => p.Translations.Any(t => t.Name.ToLower().Contains(lowerSearch)));
            }

            if (categoryId.HasValue && categoryId.Value != 0)
            {
                productsQuery = productsQuery.Where(p => p.CategoryId == categoryId.Value);
            }

            if (universeId.HasValue && universeId.Value != 0)
            {
                productsQuery = productsQuery.Where(p => p.UniverseId == universeId.Value);
            }

            if (minPrice.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.Price <= maxPrice.Value);
            }

            // 4. Сортування
            productsQuery = sortOrder switch
            {
                "price-asc" => productsQuery.OrderBy(p => p.Price),
                "price-desc" => productsQuery.OrderByDescending(p => p.Price),
                "newest" => productsQuery.OrderByDescending(p => p.Id),
                _ => productsQuery.OrderBy(p => p.Id), // За замовчуванням
            };

            // 5. Виконання запитів (ПОСЛІДОВНО)

            // Отримуємо загальну кількість
            var totalCount = await productsQuery.CountAsync();

            // Отримуємо товари для поточної сторінки
            var products = await productsQuery
                .Skip((page - 1) * DefaultPageSize)
                .Take(DefaultPageSize)
                .Select(p => new ProductCardViewModel
                {
                    Id = p.Id,
                    Name = p.Translations.FirstOrDefault().Name ?? "N/A",
                    Price = p.Price,
                    ImageUrl = p.ImageUrl ?? "/images/placeholder.jpg",
                    UniverseName = p.Universe.Translations.FirstOrDefault().Name ?? "N/A"
                })
                .ToListAsync();

            // 6. Завантаження даних для фільтрів (ПОСЛІДОВНО)
            var categories = await GetCategoryFilterOptionsAsync(langCode);
            var universes = await GetUniverseFilterOptionsAsync(langCode);

            // 7. Формування ViewModel
            var model = new ProductsCatalogViewModel
            {
                Products = products, // Використовуємо прямий результат
                TotalProductsCount = totalCount, // Використовуємо прямий результат

                // Дані для фільтрів
                Categories = categories, // Використовуємо прямий результат
                Universes = universes, // Використовуємо прямий результат

                // Діапазон цін для слайдера
                MinPrice = dbMinPrice, // Використовуємо прямий результат
                MaxPrice = dbMaxPrice, // Використовуємо прямий результат

                // Поточний стан фільтрів
                CurrentSortOrder = sortOrder,
                CurrentSearchTerm = search,
                SelectedMinPrice = minPrice,
                SelectedMaxPrice = maxPrice,
                SelectedCategoryId = categoryId,
                SelectedUniverseId = universeId,
            };

            return View(model);
        }

        // ------------------------------------------------------------------
        // ## 2. Сторінка детального опису товару
        // ------------------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> Product(int id)
        {
            var langCode = GetCurrentLanguageCode();

            var product = await _context.Products
                .Include(p => p.Translations.Where(t => t.LanguageCode == langCode))
                .Include(p => p.Universe).ThenInclude(u => u.Translations.Where(t => t.LanguageCode == langCode))
                .Include(p => p.ProductSizes)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            var translation = product.Translations.FirstOrDefault();

            var model = new ProductDetailViewModel
            {
                Id = product.Id,
                Name = translation?.Name ?? "N/A",
                FullDescription = translation?.FullDescription ?? "N/A",
                Price = product.Price,
                // Припускаємо, що у Product є властивість ImageUrl
                ImageUrl = product.ImageUrl ?? "/images/placeholder.jpg",
                UniverseName = product.Universe?.Translations.FirstOrDefault()?.Name ?? "N/A",
                Sizes = product.ProductSizes.Select(ps => new ProductSizeViewModel
                {
                    Id = ps.Id,
                    Size = ps.Size,
                    Stock = ps.Stock
                }).ToList()
            };

            return View(model);
        }

        // ------------------------------------------------------------------
        // ## 3. Категорії
        // ------------------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> Categories(string search)
        {
            var langCode = GetCurrentLanguageCode();
            var categories = await GetCategoryFilterOptionsAsync(langCode);

            if (!string.IsNullOrWhiteSpace(search))
            {
                string lowerSearch = search.ToLower();
                categories = categories.Where(c => c.Name.ToLower().Contains(lowerSearch)).ToList();
            }

            var model = new CategoryListViewModel
            {
                Categories = categories,
                CurrentSearchTerm = search
            };

            return View(model);
        }

        // ------------------------------------------------------------------
        // ## 4. Всесвіти
        // ------------------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> Universes(string search)
        {
            var langCode = GetCurrentLanguageCode();
            var universes = await GetUniverseFilterOptionsAsync(langCode);

            if (!string.IsNullOrWhiteSpace(search))
            {
                string lowerSearch = search.ToLower();
                universes = universes.Where(u => u.Name.ToLower().Contains(lowerSearch)).ToList();
            }

            var model = new UniverseListViewModel
            {
                Universes = universes,
                CurrentSearchTerm = search
            };

            return View(model);
        }

        // ------------------------------------------------------------------
        // ## ДОПОМІЖНІ МЕТОДИ (ОПТИМІЗАЦІЯ)
        // ------------------------------------------------------------------

        /// <summary>
        /// Ефективно завантажує всі категорії з перекладами та кількістю товарів.
        /// </summary>
        private async Task<List<FilterOption>> GetCategoryFilterOptionsAsync(string langCode)
        {
            // 1. Отримуємо кількість товарів для КОЖНОЇ категорії (1 запит до БД)
            var categoryCounts = await _context.Products
                .GroupBy(p => p.CategoryId)
                .Select(g => new { Id = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Id, x => x.Count);

            // 2. Отримуємо назви категорій (1 запит до БД)
            var categories = await _context.Categories
                .Include(c => c.Translations.Where(t => t.LanguageCode == langCode))
                .Select(c => new FilterOption
                {
                    Id = c.Id,
                    Name = c.Translations.FirstOrDefault().Name ?? "N/A"
                })
                .ToListAsync();

            // 3. Об'єднуємо дані в пам'яті (дуже швидко)
            foreach (var category in categories)
            {
                category.Count = categoryCounts.GetValueOrDefault(category.Id);
            }

            return categories;
        }

        /// <summary>
        /// Ефективно завантажує всі всесвіти з перекладами та кількістю товарів.
        /// </summary>
        private async Task<List<FilterOption>> GetUniverseFilterOptionsAsync(string langCode)
        {
            // 1. Отримуємо кількість товарів (1 запит до БД)
            var universeCounts = await _context.Products
                .GroupBy(p => p.UniverseId)
                // Фільтруємо продукти без всесвіту (припускаючи, що 0 - це не валідний ID)
                .Where(g => g.Key != 0) // <-- ВИПРАВЛЕНО
                .Select(g => new { Id = g.Key, Count = g.Count() }) // <-- ВИПРАВЛЕНО
                .ToDictionaryAsync(x => x.Id, x => x.Count);

            // 2. Отримуємо назви всесвітів (1 запит до БД)
            var universes = await _context.Universes
                .Include(u => u.Translations.Where(t => t.LanguageCode == langCode))
                .Select(u => new FilterOption
                {
                    Id = u.Id,
                    Name = u.Translations.FirstOrDefault().Name ?? "N/A"
                })
                .ToListAsync();

            // 3. Об'єднуємо дані в пам'яті
            foreach (var universe in universes)
            {
                universe.Count = universeCounts.GetValueOrDefault(universe.Id);
            }

            return universes;
        }
    }
}