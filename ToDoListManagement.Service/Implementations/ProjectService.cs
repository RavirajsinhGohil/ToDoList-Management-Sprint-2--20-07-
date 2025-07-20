using ToDoListManagement.Entity.Helper;
using ToDoListManagement.Entity.Models;
using ToDoListManagement.Entity.ViewModel;
using ToDoListManagement.Repository.Interfaces;
using ToDoListManagement.Service.Interfaces;

namespace ToDoListManagement.Service.Implementations;

public class ProjectService : IProjectService
{
    private readonly IBaseRepository<Project> _projectBaseRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IBaseRepository<ProjectUser> _projectUserBaseRepository;
    public ProjectService(IBaseRepository<Project> projectBaseRepository, IBaseRepository<ProjectUser> projectUserBaseRepository, IProjectRepository projectRepository)
    {
        _projectBaseRepository = projectBaseRepository;
        _projectUserBaseRepository = projectUserBaseRepository;
        _projectRepository = projectRepository;
    }

    public async Task<Pagination<ProjectViewModel>> GetPaginatedProjectsAsync(Pagination<ProjectViewModel> pagination, UserViewModel user)
    {
        Pagination<Project> projectPagination = new()
        {
            SearchKeyword = pagination.SearchKeyword,
            CurrentPage = pagination.CurrentPage,
            PageSize = pagination.PageSize,
            SortColumn = pagination.SortColumn,
            SortDirection = pagination.SortDirection
        };

        bool isAdmin = user.Role == "Admin";

        Pagination<Project> projects = await _projectRepository.GetPaginatedProjectsAsync(projectPagination, user.UserId, isAdmin);
        List<ProjectViewModel> projectViewModels = new();
        foreach (Project project in projects.Items)
        {
            List<UserViewModel> users = [];
            if (project.ProjectUsers != null)
            {
                foreach (ProjectUser? projectUser in project.ProjectUsers)
                {
                    if (projectUser.User?.RoleId != 1 && projectUser.User?.RoleId != 2 && projectUser.User?.RoleId != 4 && projectUser.User != null && !projectUser.User.IsDeleted)
                    {
                        users.Add(new UserViewModel
                        {
                            UserId = projectUser.User?.UserId ?? 0,
                            Name = projectUser.User?.Name ?? string.Empty,
                            Email = projectUser.User?.Email ?? string.Empty
                        });
                    }
                }
            }
            projectViewModels.Add(new ProjectViewModel
            {
                ProjectId = project.ProjectId,
                ProjectName = project.ProjectName ?? string.Empty,
                Description = project.Description ?? string.Empty,
                StartDate = project.CreatedOn.HasValue ? project.CreatedOn.Value.ToLocalTime() : null,
                PMName = (project.AssignedPM != null && !project.AssignedPM.IsDeleted) ? project.AssignedPM.Name : string.Empty,
                EndDate = project.EndDate,
                Status = project.Status ?? string.Empty,
                Users = users
            });
        }

        return new Pagination<ProjectViewModel>
        {
            Items = projectViewModels,
            TotalPages = projects.TotalPages,
            TotalRecords = projects.TotalRecords
        };
    }

    public async Task<List<UserViewModel>> GetProjectManagersAsync()
    {
        List<User> projectManagers = await _projectRepository.GetProjectManagersAsync();
        return projectManagers.Select(pm => new UserViewModel
        {
            UserId = pm.UserId,
            Name = pm.Name ?? string.Empty,
            Email = pm.Email ?? string.Empty
        }).ToList();
    }

    public async Task<List<UserViewModel>> GetScrumMastersAsync()
    {
        List<User> projectManagers = await _projectRepository.GetScrumMastersAsync();
        return projectManagers.Select(pm => new UserViewModel
        {
            UserId = pm.UserId,
            Name = pm.Name ?? string.Empty,
            Email = pm.Email ?? string.Empty
        }).ToList();
    }

    public async Task<bool> CheckProjectNameExistsAsync(string projectName, int projectId)
    {
        Project? project = await _projectRepository.GetProjectByNameAsync(projectName.Trim().ToLower());
        if (project != null)
        {
            if (project.ProjectId != projectId)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }

    public async Task<bool> AddProject(ProjectViewModel model, int userId)
    {
        TimeZoneInfo localZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        Project project = new()
        {
            ProjectName = model.ProjectName?.Trim(),
            CreatedBy = userId,
            Description = model.Description,
            Status = "Active",
            AssignedToProgramManager = model.AssignedToProgramManager,
            AssignedToScrumMaster = model.AssignedToScrumMaster,
            IsDeleted = false,
            CreatedOn = TimeZoneInfo.ConvertTimeToUtc(model.StartDate ?? DateTime.UtcNow, localZone),
            EndDate = TimeZoneInfo.ConvertTimeToUtc(model.EndDate ?? DateTime.UtcNow, localZone)
        };

        bool isAdded = await _projectBaseRepository.AddAsync(project);
        bool isProjectUserAdded = false;
        if (isAdded)
        {
            ProjectUser projectUser = new()
            {
                ProjectId = project.ProjectId,
                UserId = project.AssignedToProgramManager
            };
            isProjectUserAdded = await _projectUserBaseRepository.AddAsync(projectUser);
        }

        if (isAdded || isProjectUserAdded)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public async Task<ProjectViewModel?> GetProjectByIdAsync(int projectId)
    {
        if (projectId <= 0)
        {
            return null;
        }

        Project? project = await _projectRepository.GetProjectByIdAsync(projectId);
        if (project == null)
        {
            return null;
        }

        List<User> projectManagers = await _projectRepository.GetProjectManagersAsync();
        List<UserViewModel> managers = projectManagers.Select(pm => new UserViewModel
        {
            UserId = pm.UserId,
            Name = pm.Name ?? string.Empty,
            Email = pm.Email ?? string.Empty
        }).ToList();

        List<User> scrumMasters = await _projectRepository.GetScrumMastersAsync();
        List<UserViewModel> masters = scrumMasters.Select(sm => new UserViewModel
        {
            UserId = sm.UserId,
            Name = sm.Name ?? string.Empty,
            Email = sm.Email ?? string.Empty
        }).ToList();

        return new ProjectViewModel
        {
            ProjectId = project.ProjectId,
            ProjectName = project.ProjectName,
            Description = project.Description,
            StartDate = project.CreatedOn,
            EndDate = project.EndDate,
            AssignedToProgramManager = project.AssignedToProgramManager,
            AssignedToScrumMaster = project.AssignedToScrumMaster,
            PMName = project.AssignedPM?.Name ?? string.Empty,
            ProjectManagers = managers,
            Status = project.Status,
            ScrumMasters = masters
        };
    }

    public async Task<bool> UpdateProjectAsync(ProjectViewModel model)
    {
        TimeZoneInfo localZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        Project? project = await _projectRepository.GetProjectByIdAsync(model.ProjectId ?? 0);
        if (project != null)
        {
            project.ProjectName = model.ProjectName?.Trim();
            project.Description = model.Description;
            project.AssignedToProgramManager = model.AssignedToProgramManager;
            project.AssignedToScrumMaster = model.AssignedToScrumMaster;
            project.CreatedOn = TimeZoneInfo.ConvertTimeToUtc(model.StartDate ?? DateTime.UtcNow, localZone);
            project.EndDate = TimeZoneInfo.ConvertTimeToUtc(model.EndDate ?? DateTime.UtcNow, localZone);
            return await _projectBaseRepository.UpdateAsync(project);
        }
        return false;
    }

    public async Task<bool> DeleteProjectAsync(int projectId, int userId)
    {
        Project? project = await _projectRepository.GetProjectByIdAsync(projectId);
        if (project != null)
        {
            project.IsDeleted = true;
            return await _projectBaseRepository.UpdateAsync(project);
        }
        return false;
    }

    public async Task<Pagination<MemberViewModel>> GetAssignedMembersAsync(Pagination<MemberViewModel> pagination, int projectId)
    {
        Pagination<User> userPagination = new()
        {
            SearchKeyword = pagination.SearchKeyword,
            CurrentPage = pagination.CurrentPage,
            PageSize = pagination.PageSize,
            SortColumn = pagination.SortColumn,
            SortDirection = pagination.SortDirection
        };

        Pagination<User> members = await _projectRepository.GetPaginatedMembersAsync(userPagination);
        List<ProjectUser> assignedMembers = await _projectRepository.GetAssignedMembers(projectId);

        List<int?> assignedUserIds = assignedMembers.Select(am => am.UserId).ToList();

        List<MemberViewModel> memberViewModels = [];
        foreach (User member in members.Items)
        {
            memberViewModels.Add(new MemberViewModel()
            {
                UserId = member.UserId,
                Name = member.Name,
                Email = member.Email,
                PhoneNumber = member.PhoneNumber,
                IsAssigned = assignedUserIds.Contains(member.UserId)
            });
        }

        return new Pagination<MemberViewModel>
        {
            Items = memberViewModels,
            TotalPages = members.TotalPages,
            TotalRecords = members.TotalRecords
        };
    }

    public async Task<bool> AssignMembersAsync(int projectId, List<int>? userIds)
    {
        return await _projectRepository.AssignMembersAsync(projectId, userIds);
    }

    public async Task<List<ProjectDropDown>> GetProjectNamesAsync(int userId, bool isAdmin)
    {
        List<Project> projects = await _projectRepository.GetProjectNamesAsync(userId, isAdmin);

        List<ProjectDropDown> viewProjects = [];
        foreach (Project project in projects)
        {
            viewProjects.Add(new ProjectDropDown
            {
                ProjectId = project.ProjectId,
                ProjectName = project.ProjectName
            });
        }

        return viewProjects;
    }
}