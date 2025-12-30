using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using smart_receipt_web.Models;
using smart_receipt_web.Services;
using System.Security.Claims;

namespace smart_receipt_web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (_authService.IsAuthenticated())
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var result = await _authService.LoginAsync(model.Username, model.Password);

                if (result?.Success == true && result.Data != null)
                {
                    // Create claims for cookie authentication
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, result.Data.UserId.ToString()),
                        new Claim(ClaimTypes.Name, result.Data.Username)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    _logger.LogInformation($"User {model.Username} logged in successfully");

                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }

                    return RedirectToAction("Index", "Home");
                }

                model.ErrorMessage = result?.Message ?? "Giriş başarısız. Lütfen tekrar deneyin.";
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Login error: {ex.Message}");
                model.ErrorMessage = "Bağlantı hatası. Lütfen tekrar deneyin.";
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (_authService.IsAuthenticated())
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.Password != model.ConfirmPassword)
            {
                model.ErrorMessage = "Şifreler eşleşmiyor.";
                return View(model);
            }

            try
            {
                var result = await _authService.RegisterAsync(model.Username, model.Password);

                if (result?.Success == true)
                {
                    TempData["Success"] = "Kayıt başarılı! Şimdi giriş yapabilirsiniz.";
                    return RedirectToAction("Login");
                }

                model.ErrorMessage = result?.Message ?? "Kayıt başarısız. Lütfen tekrar deneyin.";
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Register error: {ex.Message}");
                model.ErrorMessage = "Bağlantı hatası. Lütfen tekrar deneyin.";
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [HttpGet]
        public async Task<IActionResult> LogoutGet()
        {
            await _authService.LogoutAsync();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}

