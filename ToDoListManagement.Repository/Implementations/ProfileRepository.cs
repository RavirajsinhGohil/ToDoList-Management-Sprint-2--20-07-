using Microsoft.EntityFrameworkCore;
using ToDoListManagement.Entity.Data;
using ToDoListManagement.Entity.Models;
using ToDoListManagement.Repository.Interfaces;

namespace ToDoListManagement.Repository.Implementations;

public class ProfileRepository : IProfileRepository
{
    private readonly ToDoListDbContext _context;

    public ProfileRepository(ToDoListDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        User? user = await _context.Users.Include(r => r.Role).FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);
        return user ?? null;
    }

    public async Task<bool> UpdateUserAsync(User user)
    {

        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        return true;
    }
}
