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
        private readonly string[] _supportedCultures = new[] { "uk", "en" };

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var items = await _context.Categories
                .Include(c => c.Translations.Where(t => t.LanguageCode == "uk"))
                .ToListAsync();

            return View(items.Select(c => new CategoryViewModel 
            { 
                Id = c.Id, 
                NameForGrid = c.Translations.FirstOrDefault()?.Name ?? "---" 
            }));
        }

        public IActionResult Create()
        {
            var model = new CategoryViewModel();
            PrepareTranslations(model);
            return View("Edit", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var category = new Category();
                UpdateTranslations(category, viewModel.Translations);
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View("Edit", viewModel);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var category = await _context.Categories.Include(c => c.Translations).FirstOrDefaultAsync(c => c.Id == id);
            if (category == null) return NotFound();

            var model = new CategoryViewModel { Id = category.Id };
            PrepareTranslations(model, category.Translations.ToList());
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryViewModel viewModel)
        {
            if (id != viewModel.Id) return NotFound();
            if (ModelState.IsValid)
            {
                var category = await _context.Categories.Include(c => c.Translations).FirstOrDefaultAsync(c => c.Id == id);
                if (category == null) return NotFound();

                UpdateTranslations(category, viewModel.Translations);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        // Допоміжні методи
        private void PrepareTranslations(CategoryViewModel model, List<CategoryTranslation>? dbTranslations = null)
        {
            model.Translations = new List<TranslationViewModel>();
            foreach (var lang in _supportedCultures)
            {
                var existing = dbTranslations?.FirstOrDefault(t => t.LanguageCode == lang);
                model.Translations.Add(new TranslationViewModel
                {
                    LanguageCode = lang,
                    Name = existing?.Name ?? "",
                    Description = existing?.Description
                });
            }
        }

        private void UpdateTranslations(Category category, List<TranslationViewModel> viewTranslations)
        {
            foreach (var tView in viewTranslations)
            {
                var tDb = category.Translations.FirstOrDefault(t => t.LanguageCode == tView.LanguageCode);
                if (tDb != null)
                {
                    tDb.Name = tView.Name;
                    tDb.Description = tView.Description;
                }
                else if (!string.IsNullOrWhiteSpace(tView.Name))
                {
                    category.Translations.Add(new CategoryTranslation
                    {
                        LanguageCode = tView.LanguageCode,
                        Name = tView.Name,
                        Description = tView.Description
                    });
                }
            }
        }
        
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null) { _context.Categories.Remove(category); await _context.SaveChangesAsync(); }
            return RedirectToAction(nameof(Index));
        }
    }
}