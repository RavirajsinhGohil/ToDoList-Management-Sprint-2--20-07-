using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ToDoListManagement.Entity.Constants;
using ToDoListManagement.Entity.Models;
using ToDoListManagement.Entity.ViewModel;
using ToDoListManagement.Repository.Interfaces;
using ToDoListManagement.Service.Interfaces;

namespace ToDoListManagement.Service.Implementations;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _authRepository;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IPermissionService _permissionService;
    private readonly IEmailService _emailService;
    public AuthService(IAuthRepository authRepository, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IPermissionService permissionService, IEmailService emailService)
    {
        _authRepository = authRepository;
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
        _permissionService = permissionService;
        _emailService = emailService;
    }

    public ClaimsPrincipal? ValidateJwtToken(string token)
    {
        try
        {
            JwtSecurityTokenHandler tokenHandler = new();
            string jwtKey = _configuration["JWT:Key"] ?? throw new ArgumentNullException("JWT:Key configuration is missing.");

            TokenValidationParameters validationParameters = new()
            {
                ValidateIssuer = true,
                ValidIssuer = _configuration["JWT:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["JWT:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
            };

            ClaimsPrincipal principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            return principal;
        }
        catch (SecurityTokenExpiredException)
        {
            return null;
        }
    }

    public async Task<bool> ValidateUserAsync(LoginViewModel model)
    {
        if (model == null || string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
        {
            return false;
        }

        bool isValid = await _authRepository.ValidateUser(model.Email, model.Password);

        if (isValid)
        {
            User user = await _authRepository.GetUserByEmailAsync(model.Email.Trim().ToLower());
            List<PermissionViewModel>? permissions = await _permissionService.GetPermissionsByRoleAsync(user.RoleId);

            string? permissionsJson = JsonSerializer.Serialize(permissions);

            List<Claim>? claims =
            [
                new Claim(ClaimTypes.Name, user.Name ?? string.Empty),
                new Claim("UserId", user.UserId.ToString()),
                new Claim("Permissions", permissionsJson)
            ];

            string? token = await GenerateJwtToken(model.Email, claims, TimeSpan.FromHours(1));
            string? refreshToken = await GenerateJwtToken(model.Email, claims, TimeSpan.FromDays(30));

            CookieOptions? cookie = new()
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            };

            if (model.RememberMe)
            {
                cookie.Expires = DateTime.Now.AddDays(30);
            }

            _httpContextAccessor.HttpContext?.Response.Cookies.Append("Token", token, cookie);
            _httpContextAccessor.HttpContext?.Response.Cookies.Append("RefreshToken", refreshToken, cookie);

            return true;
        }
        else
        {
            return false;
        }
    }

    public async Task<bool> TryRefreshAccessTokenAsync(string refreshToken)
    {
        ClaimsPrincipal? principal = ValidateJwtToken(refreshToken);
        if (principal == null) return false;

        string? email = principal.FindFirst(ClaimTypes.Email)?.Value;
        if (string.IsNullOrEmpty(email)) return false;

        User? user = await _authRepository.GetUserByEmailAsync(email);
        if (user == null) return false;

        List<PermissionViewModel> permissions = await _permissionService.GetPermissionsByRoleAsync(user.RoleId);
        string permissionsJson = JsonSerializer.Serialize(permissions);

        List<Claim>? claims =
        [
            new Claim(ClaimTypes.Name, user.Name ?? string.Empty),
            new Claim("UserId", user.UserId.ToString()),
            new Claim("Permissions", permissionsJson)
        ];

        string newAccessToken = await GenerateJwtToken(email, claims, TimeSpan.FromHours(1));

        CookieOptions? cookie = new()
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict
        };

        _httpContextAccessor.HttpContext?.Response.Cookies.Append("Token", newAccessToken, cookie);
        return true;
    }

    public async Task RefreshUserPermissionsTokenAsync(int userId)
    {
        User user = await _authRepository.GetUserByIdAsync(userId);
        if (user == null) return;

        List<PermissionViewModel> updatedPermissions = await _permissionService.GetPermissionsByRoleAsync(user.RoleId);

        string permissionsJson = JsonSerializer.Serialize(updatedPermissions);

        List<Claim>? claims =
        [
            new Claim(ClaimTypes.Name, user.Name ?? string.Empty),
            new Claim("UserId", user.UserId.ToString()),
            new Claim("Permissions", permissionsJson)
        ];

        string token = await GenerateJwtToken(user.Email ?? string.Empty, claims, TimeSpan.FromHours(1));

        CookieOptions? cookieOptions = new()
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.Now.AddHours(1)
        };

        _httpContextAccessor.HttpContext?.Response.Cookies.Append("Token", token, cookieOptions);
    }

    public async Task<bool> CheckEmailExistsAsync(string email)
    {
        return await _authRepository.CheckEmailExistsAsync(email.Trim().ToLower());
    }

    public async Task<string> GenerateJwtToken(string email, List<Claim> extraClaims, TimeSpan expiresIn)
    {
        User user = await _authRepository.GetUserByEmailAsync(email.ToLower());

        Claim[]? claims =
        [
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            new Claim(ClaimTypes.Role, user.Role?.RoleName?.ToString() ?? string.Empty)
        ];

        if (extraClaims != null && extraClaims.Count > 0)
        {
            claims = claims.Concat(extraClaims).ToArray();
        }

        string jwtKey = _configuration["JWT:Key"] ?? throw new ArgumentNullException("JWT:Key configuration is missing.");
        SymmetricSecurityKey? key = new(Encoding.UTF8.GetBytes(jwtKey));
        SigningCredentials? creds = new(key, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken? token = new(
            issuer: _configuration["JWT:Issuer"],
            audience: _configuration["JWT:Audience"],
            claims: claims,
            expires: DateTime.Now.Add(expiresIn),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<bool> RegisterUserAsync(RegisterViewModel model)
    {
        if (model == null || string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
        {
            return false;
        }
        User user = new()
        {
            Name = model.Name?.Trim(),
            Email = model.Email.Trim().ToLower(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password.Trim()),
            RoleId = 3,
            IsDeleted = false
        };

        bool isRegistered = await _authRepository.RegisterUserAsync(user);

        return isRegistered;
    }

    public async Task<UserViewModel?> GetUserFromToken(string token)
    {
        ClaimsPrincipal? principal = ValidateJwtToken(token);
        if (principal == null) return null;

        string? email = principal.FindFirst(ClaimTypes.Email)?.Value;
        if (string.IsNullOrEmpty(email)) return null;

        User? user = await _authRepository.GetUserByEmailAsync(email);
        if (user == null) return null;

        return new UserViewModel
        {
            Email = email,
            Name = user.Name,
            UserId = user.UserId,
            Role = user.Role?.RoleName
        };
    }

    public async Task<int?> SendResetPasswordEmailAsync(string email, string resetUrl)
    {
        if (string.IsNullOrEmpty(email) && string.IsNullOrEmpty(resetUrl))
        {
            return 0;
        }

        User user = await _authRepository.GetUserByEmailAsync(email.Trim().ToLower());
        if (user == null)
        {
            return -1;
        }

        string emailBody = Constants.ResetPasswordEmailBody;

        emailBody = emailBody.Replace("{ResetLink}", resetUrl);

        string subject = Constants.ResetPasswordEmailSubject;
        await _emailService.SendEmailAsync(email, subject, emailBody);

        return 1;
    }

    public void LogError(Exception? exception)
    {
        if (exception != null)
        {
            _authRepository.LogError(exception);
        }
    }

    public async Task<bool> ResetPasswordAsync(ResetPasswordViewModel model)
    {
        if (string.IsNullOrEmpty(model.Email))
        {
            return false;
        }

        User user = await _authRepository.GetUserByEmailAsync(model.Email.Trim().ToLower());
        if (user == null)
        {
            return false;
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword?.Trim());
        bool isUpdated = await _authRepository.UpdateUserPassword(user);

        return isUpdated;
    }
}