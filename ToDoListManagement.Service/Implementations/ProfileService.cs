using ToDoListManagement.Entity.Models;
using ToDoListManagement.Entity.ViewModel;
using ToDoListManagement.Repository.Interfaces;
using ToDoListManagement.Service.Interfaces;

namespace ToDoListManagement.Service.Implementations;

public class ProfileService : IProfileService
{
    private readonly IProfileRepository _profileRepository;

    public ProfileService(IProfileRepository profileRepository)
    {
        _profileRepository = profileRepository;
    }

    public async Task<ProfileViewModel?> GetUserProfileAsync(string? email)
    {
        if (string.IsNullOrEmpty(email))
        {
            return new ProfileViewModel();
        }
        User? user = await _profileRepository.GetUserByEmailAsync(email);
        if (user == null)
        {
            return new ProfileViewModel();
        }
        return new ProfileViewModel
        {
            EmployeeId = user.UserId,
            Name = user.Name ?? string.Empty,
            Email = user.Email ?? string.Empty,
            PhoneNumber = user.PhoneNumber ?? string.Empty,
            Role = user.RoleId,
            RoleName = user.Role?.RoleName ?? string.Empty
        };
    }

    public async Task<bool> UpdateUserProfileAsync(ProfileViewModel model)
    {
        User? user = await _profileRepository.GetUserByEmailAsync(model.Email!.Trim().ToLower());
        if (user == null)
        {
            return false;
        }
        user.Name = model.Name?.Trim();
        user.PhoneNumber = model.PhoneNumber?.Trim();
        return await _profileRepository.UpdateUserAsync(user);
    }
}
