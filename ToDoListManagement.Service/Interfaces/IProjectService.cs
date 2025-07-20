using ToDoListManagement.Entity.Helper;
using ToDoListManagement.Entity.ViewModel;

namespace ToDoListManagement.Service.Interfaces;

public interface IProjectService
{
    Task<List<UserViewModel>> GetProjectManagersAsync();
    Task<Pagination<ProjectViewModel>> GetPaginatedProjectsAsync(Pagination<ProjectViewModel> pagination, UserViewModel user);
    Task<List<UserViewModel>> GetScrumMastersAsync();
    Task<bool> CheckProjectNameExistsAsync(string projectName, int projectId = 0);
    Task<bool> AddProject(ProjectViewModel model, int userId);
    Task<ProjectViewModel?> GetProjectByIdAsync(int projectId   );
    Task<bool> UpdateProjectAsync(ProjectViewModel model);
    Task<Pagination<MemberViewModel>> GetAssignedMembersAsync(Pagination<MemberViewModel> pagination, int projectId);
    Task<bool> AssignMembersAsync(int projectId, List<int>? userIds);
    Task<bool> DeleteProjectAsync(int projectId, int userId);
    Task<List<ProjectDropDown>> GetProjectNamesAsync(int userId, bool isAdmin);
}
