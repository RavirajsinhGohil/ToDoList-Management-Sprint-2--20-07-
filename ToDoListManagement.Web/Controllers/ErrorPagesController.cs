using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ToDoListManagement.Entity.Constants;
using ToDoListManagement.Service.Interfaces;

namespace ToDoListManagement.Web.Controllers;

public class ErrorPagesController : Controller
{
    private readonly IAuthService _authService;
    public ErrorPagesController(IAuthService authService)
    {
        _authService = authService;
    }

    [Route("ErrorPages/ShowError/{statusCode}")]
    public IActionResult ShowError(int statusCode)
    {
        string statusHeader = statusCode switch
        {
            404 => Constants.ErrorCode404,
            500 => Constants.ErrorCode500,
            403 => Constants.ErrorCode403,
            401 => Constants.ErrorCode401,
            _ => Constants.ErrorCodeDefault,
        };

        string statusMessage = statusCode switch
        {
            404 => Constants.ErrorStatus404,
            500 => Constants.ErrorStatus500,
            403 => Constants.ErrorStatus403,
            401 => Constants.ErrorStatus401,
            _ => Constants.ErrorStatusDefault,
        };

        ViewData["StatusCode"] = statusCode;
        ViewData["StatusHeader"] = statusHeader;
        ViewData["StatusMessage"] = statusMessage;
        return View("ErrorPage");
    }

    public IActionResult Error()
    {
        IExceptionHandlerFeature? feature = HttpContext.Features.Get<IExceptionHandlerFeature>();
        Exception? exception = feature?.Error;

        if(exception != null)
        {
            _authService.LogError(exception);
        }

        if (HttpContext.Request.Headers.XRequestedWith == "XMLHttpRequest")
        {
            return Json(new
            {
                success = false,
                message = "An error occurred while processing your request.",
                detail = exception?.Message
            });
        }

        return View("Error", exception);
    }
}