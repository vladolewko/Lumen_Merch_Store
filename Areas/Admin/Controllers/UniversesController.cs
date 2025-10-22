using Lumen_Merch_Store.Data;
using Lumen_Merch_Store.Areas.Admin.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Lumen_Merch_Store.Models;

namespace Lumen_Merch_Store.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UniversesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UniversesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Admin/Universes
        public async Task<IActionResult> Index()
        {
            var items = await _context.Universes
                .Include(u => u.Translations.Where(t => t.LanguageCode == "uk"))
                .ToListAsync();

            var viewModelList = items.Select(u => MapToViewModel(u)).ToList();

            return View(viewModelList);
        }

        // GET: /Admin/Universes/Create
        public IActionResult Create()
        {
            return View(new UniverseViewModel());
        }

        // POST: /Admin/Universes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UniverseViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var universe = new Universe
                {
                    Translations = new List<UniverseTranslation>
                    {
                        // 1. Український переклад
                        new UniverseTranslation
                        {
                            Name = viewModel.NameUk,
                            Description = viewModel.DescriptionUk,
                            LanguageCode = "uk"
                        },
                        // 2. Англійський переклад
                        new UniverseTranslation
                        {
                            Name = viewModel.NameEn,
                            Description = viewModel.DescriptionEn,
                            LanguageCode = "en"
                        }
                    }
                };

                _context.Add(universe);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        // GET: /Admin/Universes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var universe = await GetItemWithDetails(id.Value);

            if (universe == null) return NotFound();

            return View(MapToViewModel(universe));
        }

        // GET: /Admin/Universes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var universe = await GetItemWithDetails(id.Value);

            if (universe == null) return NotFound();

            return View(MapToViewModel(universe));
        }

        // POST: /Admin/Universes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UniverseViewModel viewModel)
        {
            if (id != viewModel.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var universe = await GetItemWithDetails(id);
                if (universe == null) return NotFound();

                UpsertTranslation(universe, "uk", viewModel.NameUk, viewModel.DescriptionUk);
                UpsertTranslation(universe, "en", viewModel.NameEn, viewModel.DescriptionEn);
                
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        private void UpsertTranslation(Universe universe, string languageCode, string name, string? description)
        {
            // Шукаємо існуючий переклад
            var translation = universe.Translations.FirstOrDefault(t => t.LanguageCode == languageCode);

            if (translation != null)
            {
                // Якщо переклад існує - оновлюємо
                translation.Name = name;
                translation.Description = description;
            }
            else
            {
                // Якщо переклад відсутній - додаємо новий
                universe.Translations.Add(new UniverseTranslation
                {
                    Name = name,
                    Description = description,
                    LanguageCode = languageCode,
                    // UniverseId буде встановлено автоматично через навігаційну властивість
                });
            }
        }

        // POST: /Admin/Universes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var universe = await GetItemWithDetails(id);

            if (universe != null)
            {
                _context.Universes.Remove(universe);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private Task<Universe?> GetItemWithDetails(int id)
        {
            return _context.Universes
                .Include(u => u.Translations) 
                .FirstOrDefaultAsync(m => m.Id == id);
        }
        
        private UniverseViewModel MapToViewModel(Universe universe)
        {
            var translationUk = universe.Translations.FirstOrDefault(t => t.LanguageCode == "uk");
            var translationEn = universe.Translations.FirstOrDefault(t => t.LanguageCode == "en");

            return new UniverseViewModel
            {
                Id = universe.Id,
                // Українська
                NameUk = translationUk?.Name ?? string.Empty,
                DescriptionUk = translationUk?.Description,
                // Англійська
                NameEn = translationEn?.Name ?? string.Empty,
                DescriptionEn = translationEn?.Description
            };
        }
    }
}
