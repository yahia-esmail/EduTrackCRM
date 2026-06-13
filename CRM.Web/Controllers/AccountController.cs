using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CRM.Domain.Entities;
using CRM.Application.Interfaces;
using CRM.Web.Models;

namespace CRM.Web.Controllers;

public class AccountController : Controller
{
    private readonly SignInManager<AppUser> _signInManager;
    private readonly UserManager<AppUser> _userManager;
    private readonly ITokenService _tokenService;

    public AccountController(
        SignInManager<AppUser> signInManager,
        UserManager<AppUser> userManager,
        ITokenService tokenService)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _tokenService = tokenService;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null && !user.IsActive)
            {
                ModelState.AddModelError(string.Empty, "Your account has been deactivated.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                return RedirectToAction("Index", "Home");
            }
            
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login", "Account");
    }

    [HttpPost]
    [AllowAnonymous]
    [Route("api/v1/auth/token")]
    public async Task<IActionResult> GenerateToken([FromBody] LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null || !user.IsActive)
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        var checkPassword = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
        if (!checkPassword.Succeeded)
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        var token = _tokenService.GenerateToken(user);
        return Ok(new
        {
            token,
            expiration = DateTime.UtcNow.AddMinutes(120),
            user = new
            {
                user.Id,
                user.Email,
                user.FullName,
                Role = user.Role.ToString()
            }
        });
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
}
