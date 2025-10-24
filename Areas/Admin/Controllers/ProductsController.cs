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

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Admin/Products
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                .Include(p => p.Translations.Where(t => t.LanguageCode == "uk"))
                .ToListAsync();

            var productList = products.Select(p => new ProductViewModel
            {
                Id = p.Id,
                Price = p.Price,
                Stock = p.Stock,
                NameUk = p.Translations.FirstOrDefault()?.Name ?? "Назва відсутня" 
            }).ToList();

            return View(productList);
        }

        // GET: /Admin/Products/Create
        public async Task<IActionResult> Create()
        {
            var viewModel = new ProductViewModel();
            await PopulateDropdowns(viewModel);
            return View(viewModel);
        }

        // POST: /Admin/Products/Create
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
                    Translations = new List<ProductTranslation>
                    {
                        new ProductTranslation
                        {
                            Name = viewModel.NameUk,
                            ShortDescription = viewModel.ShortDescriptionUk,
                            FullDescription = viewModel.FullDescriptionUk,
                            LanguageCode = "uk"
                        }
                    }
                };
                
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            await PopulateDropdowns(viewModel);
            return View(viewModel);
        }

        // GET: /Admin/Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var product = await GetProductWithDetails(id.Value);

            if (product == null) return NotFound();

            var viewModel = MapToViewModel(product);
            
            await PopulateDropdowns(viewModel); 
            
            return View(viewModel);
        }

        // GET: /Admin/Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var product = await GetProductWithDetails(id.Value);
            
            if (product == null) return NotFound();

            var viewModel = MapToViewModel(product);
            await PopulateDropdowns(viewModel);
            
            return View(viewModel);
        }
        
        // POST: /Admin/Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductViewModel viewModel)
        {
            if (id != viewModel.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var product = await GetProductWithDetails(id);
                    if (product == null) return NotFound();

                    product.Price = viewModel.Price;
                    product.Stock = viewModel.Stock;
                    product.CategoryId = viewModel.CategoryId;
                    product.UniverseId = viewModel.UniverseId;
                    product.UpdatedAt = DateTime.Now;

                    // Оновлюємо переклад 
                    var translation = product.Translations.FirstOrDefault(t => t.LanguageCode == "uk");
                    if (translation != null)
                    {
                        translation.Name = viewModel.NameUk;
                        translation.ShortDescription = viewModel.ShortDescriptionUk;
                        translation.FullDescription = viewModel.FullDescriptionUk;
                    }
                    else
                    {
                        product.Translations.Add(new ProductTranslation
                        {
                            Name = viewModel.NameUk,
                            ShortDescription = viewModel.ShortDescriptionUk,
                            FullDescription = viewModel.FullDescriptionUk,
                            LanguageCode = "uk",
                            ProductId = product.Id 
                        });
                    }

                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await ProductExists(viewModel.Id))
                    {
                        return NotFound();
                    }
                    throw; 
                }
                return RedirectToAction(nameof(Index));
            }

            await PopulateDropdowns(viewModel);
            return View(viewModel);
        }

        // POST: /Admin/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products
                                        .Include(p => p.Translations)
                                        .FirstOrDefaultAsync(p => p.Id == id);
                                        
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private Task<Product?> GetProductWithDetails(int id)
        {
            // Отримати продукт, включаючи переклади, Категорію та Всесвіт
            return _context.Products
                .Include(p => p.Translations.Where(t => t.LanguageCode == "uk"))
                .Include(p => p.Category)
                .Include(p => p.Universe)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        private async Task PopulateDropdowns(ProductViewModel viewModel)
        {
            // Завантажуємо категорії та всесвіти з українським перекладом для SelectListItem
            var categories = await _context.Categories
                .Include(c => c.Translations.Where(t => t.LanguageCode == "uk"))
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Translations.FirstOrDefault()!.Name // Беремо українську назву
                }).ToListAsync();
            
            var universes = await _context.Universes
                .Include(u => u.Translations.Where(t => t.LanguageCode == "uk"))
                .Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = u.Translations.FirstOrDefault()!.Name // Беремо українську назву
                }).ToListAsync();

            viewModel.Categories = categories;
            viewModel.Universes = universes;
        }

        private ProductViewModel MapToViewModel(Product product)
        {
            var translationUk = product.Translations.FirstOrDefault();
            
            return new ProductViewModel
            {
                Id = product.Id,
                Price = product.Price,
                Stock = product.Stock,
                CategoryId = product.CategoryId,
                UniverseId = product.UniverseId,
                NameUk = translationUk?.Name ?? string.Empty,
                ShortDescriptionUk = translationUk?.ShortDescription ?? string.Empty,
                FullDescriptionUk = translationUk?.FullDescription ?? string.Empty
            };
        }

        private Task<bool> ProductExists(int id)
        {
            return _context.Products.AnyAsync(e => e.Id == id);
        }
    }
}