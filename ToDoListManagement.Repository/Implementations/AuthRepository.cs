using Microsoft.EntityFrameworkCore;
using ToDoListManagement.Entity.Data;
using ToDoListManagement.Entity.Models;
using ToDoListManagement.Repository.Interfaces;

namespace ToDoListManagement.Repository.Implementations;

public class AuthRepository : IAuthRepository
{
    private readonly ToDoListDbContext _context;
    public AuthRepository(ToDoListDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ValidateUser(string email, string password)
    {
        User? user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email.Trim().ToLower() && !u.IsDeleted);

        if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            return true;
        }
        return false;
    }

    public async Task<User> GetUserByEmailAsync(string email)
    {
        return await _context.Users.Include(u => u.Role).FirstAsync(u => u.Email == email && !u.IsDeleted);
    }

    public async Task<User> GetUserByIdAsync(int userId)
    {
        return await _context.Users.Include(u => u.Role).FirstAsync(u => u.UserId == userId && !u.IsDeleted);
    }

    public async Task<bool> CheckEmailExistsAsync(string email)
    {
        return await _context.Users.AnyAsync(u => u.Email == email.Trim().ToLower() && !u.IsDeleted);
    }

    public async Task<bool> RegisterUserAsync(User user)
    {
        if (user == null || string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.PasswordHash))
        {
            return false;
        }
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task LogError(Exception exception)
    {
        ErrorLog errorLog = new()
        {
            ErrorMessage = exception.Message,
            StackTrace = exception.StackTrace,
            CreatedOn = DateTime.UtcNow
        };

        await _context.ErrorLogs.AddAsync(errorLog);
        _context.SaveChanges();
    }

    public async Task<bool> UpdateUserPassword(User user)
    {
        User? existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email && !u.IsDeleted);
        if (existingUser == null)
        {
            return false;
        }

        existingUser.PasswordHash = user.PasswordHash;
        _context.Users.Update(existingUser);
        await _context.SaveChangesAsync();
        return true;
    }
}
