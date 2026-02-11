using BuildCore.HumanResources.Application.DTOs;
using BuildCore.HumanResources.Application.Interfaces;
using BuildCore.WebApp.Filters;
using Microsoft.AspNetCore.Mvc;

namespace BuildCore.WebApp.Controllers;

/// <summary>
/// Kullanıcı CRUD işlemleri için MVC Controller
/// </summary>
[RequireLogin]
public class UsersController : Controller
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Tüm kullanıcıları listeler
    /// </summary>
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var users = await _userService.GetAllUsersAsync(cancellationToken);
        return View(users);
    }

    /// <summary>
    /// Kullanıcı detay sayfası
    /// </summary>
    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        var user = await _userService.GetUserByIdAsync(id, cancellationToken);
        if (user == null)
        {
            return NotFound();
        }
        return View(user);
    }

    /// <summary>
    /// Yeni kullanıcı oluşturma formu
    /// </summary>
    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreateUserDto());
    }

    /// <summary>
    /// Yeni kullanıcı oluşturma işlemi
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateUserDto createUserDto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(createUserDto);
        }

        try
        {
            await _userService.CreateUserAsync(createUserDto, cancellationToken);
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(createUserDto);
        }
    }

    /// <summary>
    /// Kullanıcı güncelleme formu
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
    {
        var user = await _userService.GetUserByIdAsync(id, cancellationToken);
        if (user == null)
        {
            return NotFound();
        }

        var updateDto = new UpdateUserDto
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Department = user.Department,
            Position = user.Position,
            HireDate = user.HireDate
        };

        return View(updateDto);
    }

    /// <summary>
    /// Kullanıcı güncelleme işlemi
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, UpdateUserDto updateUserDto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(updateUserDto);
        }

        try
        {
            await _userService.UpdateUserAsync(id, updateUserDto, cancellationToken);
            return RedirectToAction(nameof(Index));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(updateUserDto);
        }
    }

    /// <summary>
    /// Kullanıcı silme onay sayfası
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var user = await _userService.GetUserByIdAsync(id, cancellationToken);
        if (user == null)
        {
            return NotFound();
        }
        return View(user);
    }

    /// <summary>
    /// Kullanıcı silme işlemi
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _userService.DeleteUserAsync(id, cancellationToken);
            return RedirectToAction(nameof(Index));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
