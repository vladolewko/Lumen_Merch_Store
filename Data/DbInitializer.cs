using Lumen_Merch_Store.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Lumen_Merch_Store.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(ApplicationDbContext context)
        {
            // 1. Перевірка, чи база даних вже заповнена
            if (context.Products.Any())
            {
                return; // База даних вже містить дані
            }

            // 2. Languages
            var languages = new Language[]
            {
                new Language { Code = "uk", Name = "Українська" },
                new Language { Code = "en", Name = "English" }
            };
            context.Languages.AddRange(languages);
            await context.SaveChangesAsync();

            // 3. Universes
            var universes = new Universe[]
            {
                new Universe { /* ID буде згенеровано */ }, // SW
                new Universe { /* ID буде згенеровано */ }, // CP
                new Universe { /* ID буде згенеровано */ }  // HP
            };
            context.Universes.AddRange(universes);
            await context.SaveChangesAsync();
            
            // Отримаємо згенеровані ID для подальшого використання
            var swId = universes[0].Id;
            var cpId = universes[1].Id;
            var hpId = universes[2].Id;

            // 4. Universe Translations
            context.UniverseTranslations.AddRange(
                new UniverseTranslation { UniverseId = swId, LanguageCode = "uk", Name = "Зоряні Війни", Description = "Далека-далека галактика." },
                new UniverseTranslation { UniverseId = swId, LanguageCode = "en", Name = "Star Wars", Description = "A galaxy far, far away." },
                new UniverseTranslation { UniverseId = cpId, LanguageCode = "uk", Name = "Кіберпанк 2077", Description = "Похмурий Найт-Сіті." },
                new UniverseTranslation { UniverseId = cpId, LanguageCode = "en", Name = "Cyberpunk 2077", Description = "The dark Night City." },
                new UniverseTranslation { UniverseId = hpId, LanguageCode = "uk", Name = "Гаррі Поттер", Description = "Чарівний світ Гоґвортсу." },
                new UniverseTranslation { UniverseId = hpId, LanguageCode = "en", Name = "Harry Potter", Description = "The magical world of Hogwarts." }
            );

            // 5. Categories
            var categories = new Category[]
            {
                new Category { /* ID буде згенеровано */ }, // Apparel
                new Category { /* ID буде згенеровано */ }, // Figures
                new Category { /* ID буде згенеровано */ }  // Accessories
            };
            context.Categories.AddRange(categories);
            await context.SaveChangesAsync();
            
            var apparelId = categories[0].Id;
            var figureId = categories[1].Id;
            var accessoriesId = categories[2].Id;
            
            context.CategoryTranslations.AddRange(
                new CategoryTranslation { CategoryId = apparelId, LanguageCode = "uk", Name = "Одяг", Description = "Одяг для фанатів." },
                new CategoryTranslation { CategoryId = figureId, LanguageCode = "uk", Name = "Фігурки", Description = "Колекційні фігурки." },
                new CategoryTranslation { CategoryId = accessoriesId, LanguageCode = "uk", Name = "Аксесуари", Description = "Дрібниці та сувеніри." }
            );

            // 6. Products
            var products = new Product[]
            {
                new Product { UniverseId = swId, CategoryId = apparelId, Price = 899.00M, Stock = 50 }, // SW Tee
                new Product { UniverseId = cpId, CategoryId = figureId, Price = 2599.50M, Stock = 15 }, // CP Figure
                new Product { UniverseId = hpId, CategoryId = accessoriesId, Price = 349.00M, Stock = 100 }, // HP Mug
                new Product { UniverseId = cpId, CategoryId = apparelId, Price = 1499.00M, Stock = 30 }  // CP Hoodie
            };
            context.Products.AddRange(products);
            await context.SaveChangesAsync();
            
            var product1Id = products[0].Id;
            var product4Id = products[3].Id;

            // 7. Product Translations
            context.ProductTranslations.AddRange(
                new ProductTranslation { ProductId = product1Id, LanguageCode = "uk", Name = "Футболка 'Батько'", ShortDescription = "Класична футболка SW.", FullDescription = "Високоякісна бавовняна футболка." },
                new ProductTranslation { ProductId = product1Id, LanguageCode = "en", Name = "T-Shirt 'Father'", ShortDescription = "Classic SW tee.", FullDescription = "High-quality cotton t-shirt." },
                new ProductTranslation { ProductId = product4Id, LanguageCode = "uk", Name = "Худі 'Самурай'", ShortDescription = "Тепле худі CP.", FullDescription = "Худі з логотипом 'Самурай'." }
                // ... (інші переклади)
            );

            // 8. Product Sizes
            context.ProductSizes.AddRange(
                new ProductSize { ProductId = product1Id, Size = "S", Stock = 15 },
                new ProductSize { ProductId = product4Id, Size = "L", Stock = 10 }
                // ... (інші розміри)
            );

            await context.SaveChangesAsync();
        }
    }
}