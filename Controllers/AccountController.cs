using Lumen_Merch_Store.Models;
using Lumen_Merch_Store.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Lumen_Merch_Store.Controllers;

public class AccountController : Controller
{
    private readonly ILogger<AccountController> _logger;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ILogger<AccountController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (ModelState.IsValid)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, true);

            if (result.Succeeded)
            {
                _logger.LogInformation("User logged in.");
                return RedirectToLocal(returnUrl);
            }

            if (result.IsLockedOut)
            {
                _logger.LogWarning("User account locked out.");
                return RedirectToAction(nameof(Lockout));
            }

            ModelState.AddModelError(string.Empty, "Невірні дані для входу.");
            return View(model);
        }

        return View(model);
    }

    [HttpGet]
    public IActionResult Register(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (ModelState.IsValid)
        {
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                Name = model.Name,
                PhoneNumber = model.Phone
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("User created a new account with password.");

                // Adding a role of user for the newly created user.
                await _userManager.AddToRoleAsync(user, "User");

                await _signInManager.SignInAsync(user, false);
                _logger.LogInformation("User created a new account with password.");
                return RedirectToLocal(returnUrl);
            }

            AddErrors(result);
        }

        return View(model);
    }
    
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Profile()
    {
        var user = await _userManager.GetUserAsync(User);
    
        if (user == null)
        {
            return NotFound($"Неможливо завантажити користувача.");
        }

        var model = new ProfileViewModel
        {
            Email = user.Email ?? string.Empty,
            Name = user.Name,
            Phone = user.PhoneNumber,
            PhotoUrl = user.PhotoUrl,
            CreatedAt = user.CreatedAt
        };

        return View(model); 
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        _logger.LogInformation("User logged out.");
        return RedirectToAction(nameof(HomeController.Index), "Home");
    }

    [HttpGet]
    public IActionResult Lockout()
    {
        return View();
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Edit()
    {
        var user = await _userManager.GetUserAsync(User);
    
        if (user == null)
        {
            return NotFound($"Неможливо завантажити користувача.");
        }

        var model = new EditProfileViewModel
        {
            Email = user.Email ?? string.Empty,
            Name = user.Name,
            Phone = user.PhoneNumber,
            PhotoUrl = user.PhotoUrl
        };

        return View(model);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditProfileViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound($"Неможливо завантажити користувача.");
        }

        var existingUser = await _userManager.FindByEmailAsync(model.Email);
        if (existingUser != null && existingUser.Id != user.Id)
        {
            ModelState.AddModelError("Email", "Цей email вже використовується.");
            return View(model);
        }

        user.Name = model.Name;
        user.Email = model.Email;
        user.UserName = model.Email;
        user.PhoneNumber = model.Phone;

        if (model.PhotoFile != null)
        {
            var uploadsFolder = Path.Combine("wwwroot", "images", "profiles");
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid() + "_" + model.PhotoFile.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await model.PhotoFile.CopyToAsync(fileStream);
            }

            user.PhotoUrl = "/images/profiles/" + uniqueFileName;
        }

        var result = await _userManager.UpdateAsync(user);
        
        if (result.Succeeded)
        {
            _logger.LogInformation("User profile updated successfully.");
            return RedirectToAction(nameof(Profile));
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(model);
    }

    private IActionResult RedirectToLocal(string? returnUrl)
    {
        if (Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);

        return RedirectToAction(nameof(HomeController.Index), "Home");
    }

    private void AddErrors(IdentityResult result)
    {
        foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);
    }
}