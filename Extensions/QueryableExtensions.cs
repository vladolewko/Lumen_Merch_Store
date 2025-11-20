using Lumen_Merch_Store.Models; // Перевір namespace моделі Product
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Lumen_Merch_Store.Extensions
{
    public static class QueryableExtensions
    {
        // Завантаження перекладів (Eager Loading)
        public static IQueryable<Product> IncludeTranslations(this IQueryable<Product> query, string langCode)
        {
            return query
                .Include(p => p.Translations.Where(t => t.LanguageCode == langCode))
                .Include(p => p.Universe).ThenInclude(u => u.Translations.Where(t => t.LanguageCode == langCode))
                .Include(p => p.Category).ThenInclude(c => c.Translations.Where(t => t.LanguageCode == langCode));
        }

        // Фільтрація
        public static IQueryable<Product> FilterByCriteria(
            this IQueryable<Product> query, 
            string search, 
            int? categoryId, 
            int? universeId, 
            decimal? minPrice, 
            decimal? maxPrice)
        {
            if (!string.IsNullOrWhiteSpace(search))
            {
                string lowerSearch = search.ToLower();
                // Шукаємо в перекладах назви
                query = query.Where(p => p.Translations.Any(t => t.Name.ToLower().Contains(lowerSearch)));
            }

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            if (universeId.HasValue && universeId.Value > 0)
            {
                query = query.Where(p => p.UniverseId == universeId.Value);
            }

            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            return query;
        }

        // Сортування
        public static IQueryable<Product> SortBy(this IQueryable<Product> query, string sortOrder)
        {
            return sortOrder switch
            {
                "price-asc" => query.OrderBy(p => p.Price),
                "price-desc" => query.OrderByDescending(p => p.Price),
                "newest" => query.OrderByDescending(p => p.Id),
                _ => query.OrderBy(p => p.Id) // Default
            };
        }
    }
}