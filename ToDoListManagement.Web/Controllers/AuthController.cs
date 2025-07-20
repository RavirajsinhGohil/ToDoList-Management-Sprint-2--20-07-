using Microsoft.AspNetCore.Mvc;
using ToDoListManagement.Entity.Constants;
using ToDoListManagement.Entity.ViewModel;
using ToDoListManagement.Service.Interfaces;

namespace ToDoListManagement.Web.Controllers;

public class AuthController : BaseController
{
    private readonly IAuthService _authService;
    public AuthController(IAuthService authService) : base(authService)
    {
        _authService = authService;
    }

    #region Login

    [HttpGet]
    public IActionResult Login()
    {
        string? token = Request.Cookies["Token"];

        if (!string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Index", "Dashboard");
        }
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            bool isValid = await _authService.ValidateUserAsync(model);
            if (isValid)
            {
                TempData["SuccessMessage"] = Constants.LoginSuccessMessage;
                return RedirectToAction("Index", "Dashboard");
            }
            else
            {
                TempData["ErrorMessage"] = Constants.LoginErrorMessage;
                return View(model);
            }
        }
        return View(model);
    }

    public async Task<IActionResult> CheckEmailExists(string email)
    {
        await Task.Delay(1000);
        bool exists = await _authService.CheckEmailExistsAsync(email.Trim().ToLower());
        return Json(!exists);
    }

    #endregion

    #region Registration

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            bool isRegistered = await _authService.RegisterUserAsync(model);
            if (isRegistered)
            {
                TempData["SuccessMessage"] = Constants.RegistrationSuccessMessage;
                return RedirectToAction("Login");
            }
            else
            {
                TempData["ErrorMessage"] = Constants.RegistrationErrorMessage;
            }
        }
        return View(model);
    }

    #endregion

    #region Logout

    public IActionResult Logout()
    {
        foreach (string? cookie in Request.Cookies.Keys)
        {
            Response.Cookies.Delete(cookie);
        }
        return RedirectToAction("Login", "Auth");
    }

    #endregion

    #region Forgot Password

    [HttpGet]
    public async Task<IActionResult> ForgotPassword(string? email)
    {
        ForgotPasswordViewModel model = new()
        {
            Email = email
        };
        bool isRegistered = !string.IsNullOrEmpty(email) && await _authService.CheckEmailExistsAsync(email);
        if (!isRegistered && !string.IsNullOrEmpty(email))
        {
            TempData["ErrorMessage"] = Constants.EmailNotRegisteredMessage;
            return RedirectToAction("Login", "Auth");
        }
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> SendResetPasswordEmail(string? email)
    {
        if (!string.IsNullOrEmpty(email))
        {
            if (!await _authService.CheckEmailExistsAsync(email.Trim().ToLower()))
            {
                TempData["ErrorMessage"] = Constants.EmailNotRegisteredMessage;
                return RedirectToAction("ForgotPassword", "Auth");
            }

            string? token = await _authService.GenerateJwtToken(email, [], TimeSpan.FromHours(1));

            string? resetUrl = Url.Action("ResetPassword", "Auth", new { token = Uri.EscapeDataString(token) }, Request.Scheme);

            if (string.IsNullOrEmpty(resetUrl))
            {
                TempData["ErrorMessage"] = Constants.InvalidResetPasswordLinkMessage;
                return RedirectToAction("ForgotPassword", "Auth");
            }

            int? isEmailSent = await _authService.SendResetPasswordEmailAsync(email, resetUrl);

            if (isEmailSent > 0)
            {
                TempData["SuccessMessage"] = Constants.ResetPasswordSuccessMessage;
                return RedirectToAction("Login", "Auth");
            }
            else if (isEmailSent == 0)
            {
                TempData["ErrorMessage"] = Constants.InvalidEmailMessage;
            }
            else
            {
                TempData["ErrorMessage"] = Constants.EmailNotRegisteredMessage;
            }
        }
        return View(email);
    }

    #endregion

    #region Reset Password

    [HttpGet]
    public async Task<IActionResult> ResetPassword(string token)
    {
        if (!string.IsNullOrEmpty(token))
        {
            UserViewModel? user = await _authService.GetUserFromToken(token);
            if (user == null)
            {
                TempData["ErrorMessage"] = Constants.InvalidResetPasswordLinkMessage;
                return RedirectToAction("Login", "Auth");
            }

            return View(new ResetPasswordViewModel
            {
                Token = token,
                Email = user.Email
            });
        }

        TempData["ErrorMessage"] = Constants.InvalidResetPasswordLinkMessage;
        return RedirectToAction("Login", "Auth");
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (ModelState.IsValid)
        {
            bool isReset = await _authService.ResetPasswordAsync(model);
            if (isReset)
            {
                TempData["SuccessMessage"] = Constants.ResetPasswordSuccessMessage;
                return RedirectToAction("Login", "Auth");
            }
            else
            {
                TempData["ErrorMessage"] = Constants.ResetPasswordErrorMessage;
            }
        }
        return View(model);
    }

    #endregion
}