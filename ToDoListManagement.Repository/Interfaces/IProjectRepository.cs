using ToDoListManagement.Entity.Helper;
using ToDoListManagement.Entity.Models;

namespace ToDoListManagement.Repository.Interfaces;

public interface IProjectRepository
{
    Task<Pagination<Project>> GetPaginatedProjectsAsync(Pagination<Project> pagination, int userId, bool isAdmin);
    Task<bool> AddProject(Project project);
    Task<bool> AddProjectUserAsync(ProjectUser projectUser);
    Task<Project?> GetProjectByIdAsync(int projectId);
    Task<List<User>> GetProjectManagersAsync();
    Task<List<User>> GetScrumMastersAsync();
    Task<Project?> GetProjectByNameAsync(string projectName);
    Task<bool> UpdateProjectAsync(Project project);
    Task<Pagination<User>> GetPaginatedMembersAsync(Pagination<User> pagination);
    Task<List<ProjectUser>> GetAssignedMembers(int projectId);
    Task<bool> AssignMembersAsync(int projectId, List<int>? userIds);
    Task<bool> DeleteProjectAsync(int projectId, int userId);
    Task<List<Project>> GetProjectNamesAsync(int userId, bool isAdmin);
    Task<ProjectUser> GetProjectByUserIdAsnc(int userId);
}