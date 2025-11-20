using Lumen_Merch_Store.Areas.Admin.ViewModels;
using Lumen_Merch_Store.Data;
using Lumen_Merch_Store.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Lumen_Merch_Store.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly string[] _supportedCultures = new[] { "uk", "en" }; // Мови сайту

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Admin/Products
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                .Include(p => p.Translations.Where(t => t.LanguageCode == "uk")) // Беремо укр для адмінки
                .ToListAsync();

            var model = products.Select(p => new ProductViewModel
            {
                Id = p.Id,
                Price = p.Price,
                Stock = p.Stock,
                ImageUrl = p.ImageUrl,
                NameForGrid = p.Translations.FirstOrDefault()?.Name ?? "---"
            }).ToList();

            return View(model);
        }

        // GET: Create
        public async Task<IActionResult> Create()
        {
            var viewModel = new ProductViewModel();
            // Ініціалізуємо пусті переклади для всіх мов
            PrepareTranslations(viewModel);
            await PopulateDropdowns(viewModel);
            return View("Edit", viewModel); // Використовуємо спільну View "Edit"
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var product = new Product
                {
                    Price = viewModel.Price,
                    Stock = viewModel.Stock,
                    CategoryId = viewModel.CategoryId,
                    UniverseId = viewModel.UniverseId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    ImageUrl = await SaveImage(viewModel.ImageFile)
                };

                // Зберігаємо переклади
                foreach (var t in viewModel.Translations)
                {
                    if (!string.IsNullOrWhiteSpace(t.Name))
                    {
                        product.Translations.Add(new ProductTranslation
                        {
                            LanguageCode = t.LanguageCode,
                            Name = t.Name,
                            ShortDescription = t.ShortDescription,
                            FullDescription = t.FullDescription
                        });
                    }
                }

                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            await PopulateDropdowns(viewModel);
            return View("Edit", viewModel);
        }

        // GET: Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                .Include(p => p.Translations)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            var viewModel = new ProductViewModel
            {
                Id = product.Id,
                Price = product.Price,
                Stock = product.Stock,
                CategoryId = product.CategoryId,
                UniverseId = product.UniverseId,
                ImageUrl = product.ImageUrl
            };

            // Заповнюємо переклади з БД або створюємо пусті
            PrepareTranslations(viewModel, product.Translations.ToList());
            await PopulateDropdowns(viewModel);

            return View(viewModel);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductViewModel viewModel)
        {
            if (id != viewModel.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var product = await _context.Products
                    .Include(p => p.Translations)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (product == null) return NotFound();

                product.Price = viewModel.Price;
                product.Stock = viewModel.Stock;
                product.CategoryId = viewModel.CategoryId;
                product.UniverseId = viewModel.UniverseId;
                product.UpdatedAt = DateTime.Now;

                // Оновлюємо фото, якщо завантажено нове
                if (viewModel.ImageFile != null)
                {
                    product.ImageUrl = await SaveImage(viewModel.ImageFile);
                }

                // Оновлюємо переклади
                UpdateTranslations(product, viewModel.Translations);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            await PopulateDropdowns(viewModel);
            return View(viewModel);
        }
        
        // POST: Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // === HELPERS ===

        private void PrepareTranslations(ProductViewModel model, List<ProductTranslation>? dbTranslations = null)
        {
            model.Translations = new List<TranslationViewModel>();
            foreach (var lang in _supportedCultures)
            {
                var existing = dbTranslations?.FirstOrDefault(t => t.LanguageCode == lang);
                model.Translations.Add(new TranslationViewModel
                {
                    LanguageCode = lang,
                    Name = existing?.Name ?? "",
                    ShortDescription = existing?.ShortDescription,
                    FullDescription = existing?.FullDescription
                });
            }
        }

        private void UpdateTranslations(Product product, List<TranslationViewModel> viewTranslations)
        {
            foreach (var tView in viewTranslations)
            {
                var tDb = product.Translations.FirstOrDefault(t => t.LanguageCode == tView.LanguageCode);
                if (tDb != null)
                {
                    tDb.Name = tView.Name;
                    tDb.ShortDescription = tView.ShortDescription;
                    tDb.FullDescription = tView.FullDescription;
                }
                else if (!string.IsNullOrWhiteSpace(tView.Name))
                {
                    product.Translations.Add(new ProductTranslation
                    {
                        LanguageCode = tView.LanguageCode,
                        Name = tView.Name,
                        ShortDescription = tView.ShortDescription,
                        FullDescription = tView.FullDescription,
                        ProductId = product.Id
                    });
                }
            }
        }

        private async Task<string> SaveImage(IFormFile? file)
        {
            if (file == null) return null;
            
            var uploadsFolder = Path.Combine("wwwroot", "images", "products");
            Directory.CreateDirectory(uploadsFolder);
            
            var uniqueFileName = Guid.NewGuid() + "_" + file.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
            
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            
            return "/images/products/" + uniqueFileName;
        }

        private async Task PopulateDropdowns(ProductViewModel viewModel)
        {
            viewModel.Categories = await _context.Categories
                .Include(c => c.Translations.Where(t => t.LanguageCode == "uk"))
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Translations.FirstOrDefault().Name })
                .ToListAsync();

            viewModel.Universes = await _context.Universes
                .Include(u => u.Translations.Where(t => t.LanguageCode == "uk"))
                .Select(u => new SelectListItem { Value = u.Id.ToString(), Text = u.Translations.FirstOrDefault().Name })
                .ToListAsync();
        }
    }
}