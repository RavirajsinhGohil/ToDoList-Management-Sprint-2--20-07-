using ToDoListManagement.Entity.ViewModel;

namespace ToDoListManagement.Service.Interfaces;

public interface IProfileService
{
    Task<ProfileViewModel?> GetUserProfileAsync(string? email);
    Task<bool> UpdateUserProfileAsync(ProfileViewModel model);
}
