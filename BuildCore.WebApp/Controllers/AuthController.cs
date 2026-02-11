using BuildCore.HumanResources.Application.DTOs;
using BuildCore.HumanResources.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BuildCore.WebApp.Controllers;

/// <summary>
/// Authentication işlemleri için MVC Controller
/// </summary>
public class AuthController : Controller
{
    private readonly IAuthenticationService _authenticationService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IAuthenticationService authenticationService,
        ILogger<AuthController> logger)
    {
        _authenticationService = authenticationService;
        _logger = logger;
    }

    /// <summary>
    /// Login sayfası
    /// </summary>
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginDto());
    }

    /// <summary>
    /// Login işlemi
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginDto loginDto, string? returnUrl = null, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return View(loginDto);
        }

        try
        {
            var tokenDto = await _authenticationService.LoginAsync(loginDto, cancellationToken);

            // Token'ı cookie'ye kaydet
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = Request.IsHttps, // HTTPS ise Secure, değilse false
                SameSite = SameSiteMode.Lax,
                Expires = tokenDto.ExpiresAt // DateTime direkt kullanılabilir
            };

            Response.Cookies.Append("AuthToken", tokenDto.AccessToken, cookieOptions);

            // Kullanıcı bilgilerini session'a kaydet
            HttpContext.Session.SetString("UserId", tokenDto.User.Id.ToString());
            HttpContext.Session.SetString("UserEmail", tokenDto.User.Email);
            HttpContext.Session.SetString("UserName", tokenDto.User.FullName);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }
        catch (UnauthorizedAccessException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(loginDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login işlemi sırasında bir hata oluştu.");
            ModelState.AddModelError("", "Giriş işlemi başarısız oldu. Lütfen tekrar deneyin.");
            return View(loginDto);
        }
    }

    /// <summary>
    /// Register sayfası
    /// </summary>
    [HttpGet]
    public IActionResult Register()
    {
        return View(new RegisterDto());
    }

    /// <summary>
    /// Register işlemi
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterDto registerDto, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return View(registerDto);
        }

        try
        {
            var user = await _authenticationService.RegisterAsync(registerDto, cancellationToken);
            TempData["SuccessMessage"] = "Kayıt işlemi başarılı! Giriş yapabilirsiniz.";
            return RedirectToAction(nameof(Login));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(registerDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Kayıt işlemi sırasında bir hata oluştu.");
            ModelState.AddModelError("", "Kayıt işlemi başarısız oldu. Lütfen tekrar deneyin.");
            return View(registerDto);
        }
    }

    /// <summary>
    /// Logout işlemi
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        // Cookie'yi sil
        Response.Cookies.Delete("AuthToken");

        // Session'ı temizle
        HttpContext.Session.Clear();

        return RedirectToAction("Index", "Home");
    }
}
