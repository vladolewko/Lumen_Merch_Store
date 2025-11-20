using Lumen_Merch_Store.Areas.Admin.ViewModels;
using Lumen_Merch_Store.Data;
using Lumen_Merch_Store.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lumen_Merch_Store.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UniversesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly string[] _supportedCultures = new[] { "uk", "en" };

        public UniversesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Admin/Universes
        public async Task<IActionResult> Index()
        {
            var items = await _context.Universes
                .Include(c => c.Translations.Where(t => t.LanguageCode == "uk"))
                .ToListAsync();

            return View(items.Select(c => new UniverseViewModel 
            { 
                Id = c.Id, 
                NameForGrid = c.Translations.FirstOrDefault()?.Name ?? "---" 
            }));
        }

        // GET: Create
        public IActionResult Create()
        {
            var model = new UniverseViewModel();
            PrepareTranslations(model);
            return View("Edit", model);
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UniverseViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var universe = new Universe();
                UpdateTranslations(universe, viewModel.Translations);
                
                _context.Universes.Add(universe); // Виправлено на Universes
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View("Edit", viewModel);
        }

        // GET: Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            
            // Виправлено: шукаємо у Universes
            var universe = await _context.Universes
                .Include(c => c.Translations)
                .FirstOrDefaultAsync(c => c.Id == id);
            
            if (universe == null) return NotFound();

            // Виправлено: використовуємо UniverseViewModel
            var model = new UniverseViewModel { Id = universe.Id };
            PrepareTranslations(model, universe.Translations.ToList());
            return View(model);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UniverseViewModel viewModel) // Виправлено тип ViewModel
        {
            if (id != viewModel.Id) return NotFound();
            
            if (ModelState.IsValid)
            {
                // Виправлено: шукаємо у Universes
                var universe = await _context.Universes
                    .Include(c => c.Translations)
                    .FirstOrDefaultAsync(c => c.Id == id);
                
                if (universe == null) return NotFound();

                UpdateTranslations(universe, viewModel.Translations);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        // POST: Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var universe = await _context.Universes.FindAsync(id); // Виправлено на Universes
            if (universe != null) 
            { 
                _context.Universes.Remove(universe); 
                await _context.SaveChangesAsync(); 
            }
            return RedirectToAction(nameof(Index));
        }

        // === ДОПОМІЖНІ МЕТОДИ (Адаптовані під Universe) ===

        private void PrepareTranslations(UniverseViewModel model, List<UniverseTranslation>? dbTranslations = null)
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

        private void UpdateTranslations(Universe universe, List<TranslationViewModel> viewTranslations)
        {
            foreach (var tView in viewTranslations)
            {
                var tDb = universe.Translations.FirstOrDefault(t => t.LanguageCode == tView.LanguageCode);
                
                if (tDb != null)
                {
                    // Оновлюємо існуючий переклад
                    tDb.Name = tView.Name;
                    tDb.Description = tView.Description;
                }
                else if (!string.IsNullOrWhiteSpace(tView.Name))
                {
                    // Додаємо новий переклад
                    universe.Translations.Add(new UniverseTranslation
                    {
                        LanguageCode = tView.LanguageCode,
                        Name = tView.Name,
                        Description = tView.Description,
                        UniverseId = universe.Id // EF Core сам підтягне ID, але можна явно вказати
                    });
                }
            }
        }
    }
}