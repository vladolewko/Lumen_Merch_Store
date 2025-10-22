using Lumen_Merch_Store.Areas.Admin.ViewModels;
using Lumen_Merch_Store.Data;
using Lumen_Merch_Store.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lumen_Merch_Store.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Admin/Categories
        public async Task<IActionResult> Index()
        {
            var items = await _context.Categories
                .Include(c => c.Translations.Where(t => t.LanguageCode == "uk"))
                .ToListAsync();

            var viewModelList = items.Select(c => MapToViewModel(c)).ToList();

            return View(viewModelList);
        }

        // GET: /Admin/Categories/Create
        public IActionResult Create()
        {
            return View(new CategoryViewModel());
        }

        // POST: /Admin/Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var category = new Category
                {
                    Translations = new List<CategoryTranslation>
                    {
                        new CategoryTranslation
                        {
                            Name = viewModel.NameUk,
                            Description = viewModel.DescriptionUk,
                            LanguageCode = "uk"
                        }
                    }
                };

                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        // GET: /Admin/Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var category = await GetItemWithDetails(id.Value);

            if (category == null) return NotFound();

            return View(MapToViewModel(category));
        }

        // GET: /Admin/Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var category = await GetItemWithDetails(id.Value);

            if (category == null) return NotFound();

            return View(MapToViewModel(category));
        }
        
        // POST: /Admin/Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryViewModel viewModel)
        {
            if (id != viewModel.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var category = await GetItemWithDetails(id);
                if (category == null) return NotFound();

                var translation = category.Translations.FirstOrDefault(t => t.LanguageCode == "uk");
                
                if (translation != null)
                {
                    translation.Name = viewModel.NameUk;
                    translation.Description = viewModel.DescriptionUk;
                }
                else
                {
                    // Додаємо новий переклад, якщо він відсутній
                    category.Translations.Add(new CategoryTranslation
                    {
                        Name = viewModel.NameUk,
                        Description = viewModel.DescriptionUk,
                        LanguageCode = "uk",
                        CategoryId = category.Id
                    });
                }
                
                _context.Update(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }
        
        // POST: /Admin/Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await GetItemWithDetails(id);

            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // --- Приватні допоміжні методи ---
        private Task<Category?> GetItemWithDetails(int id)
        {
            return _context.Categories
                .Include(c => c.Translations.Where(t => t.LanguageCode == "uk"))
                .FirstOrDefaultAsync(m => m.Id == id);
        }
        
        private CategoryViewModel MapToViewModel(Category category)
        {
            var translationUk = category.Translations.FirstOrDefault(t => t.LanguageCode == "uk");

            return new CategoryViewModel
            {
                Id = category.Id,
                NameUk = translationUk?.Name ?? string.Empty,
                DescriptionUk = translationUk?.Description
            };
        }
    }
}
