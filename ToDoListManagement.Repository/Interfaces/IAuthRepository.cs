using ToDoListManagement.Entity.Models;

namespace ToDoListManagement.Repository.Interfaces;

public interface IAuthRepository
{
    Task<bool> ValidateUser(string email, string password);
    Task<User> GetUserByEmailAsync(string email);
    Task<User> GetUserByIdAsync(int userId);
    Task<bool> CheckEmailExistsAsync(string email);
    Task<bool> RegisterUserAsync(User user);
    Task LogError(Exception exception);
    Task<bool> UpdateUserPassword(User user);
}
