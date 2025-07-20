using System.Security.Claims;
using ToDoListManagement.Service.Interfaces;

public class JwtRefreshMiddleware
{
    private readonly RequestDelegate _next;

    public JwtRefreshMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        IAuthService authService = context.RequestServices.GetRequiredService<IAuthService>();

        string? token = context.Request.Cookies["Token"];
        string? refreshToken = context.Request.Cookies["RefreshToken"];

        ClaimsPrincipal? principal = null;

        if (!string.IsNullOrEmpty(token))
        {
            principal = authService.ValidateJwtToken(token);
        }

        // If token is missing or invalid
        if (principal == null && !string.IsNullOrEmpty(refreshToken))
        {
            bool isRefreshed = await authService.TryRefreshAccessTokenAsync(refreshToken);
            if (isRefreshed)
            {
                context.Response.Redirect(context.Request.Path);
                return;
            }
        }
        await _next(context);
    }
}