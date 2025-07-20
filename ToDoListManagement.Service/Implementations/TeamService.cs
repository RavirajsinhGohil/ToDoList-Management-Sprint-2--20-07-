using ToDoListManagement.Entity.Helper;
using ToDoListManagement.Entity.Models;
using ToDoListManagement.Entity.ViewModel;
using ToDoListManagement.Repository.Interfaces;
using ToDoListManagement.Service.Interfaces;

namespace ToDoListManagement.Service.Implementations;

public class TeamService : ITeamService
{
    private readonly ITeamUserRepository _teamUserRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IEmployeeRepository _employeeRepository;

    public TeamService(ITeamUserRepository teamUserRepository, IProjectRepository projectRepository, IEmployeeRepository employeeRepository)
    {
        _teamUserRepository = teamUserRepository;
        _projectRepository = projectRepository;
        _employeeRepository = employeeRepository;
    }

    public async Task<Pagination<EmployeeViewModel>> GetMembersAsync(Pagination<EmployeeViewModel> pagination, int teamManagerId)
    {
        Pagination<User> userPagination = new()
        {
            SearchKeyword = pagination.SearchKeyword,
            CurrentPage = pagination.CurrentPage,
            PageSize = pagination.PageSize,
            SortColumn = pagination.SortColumn,
            SortDirection = pagination.SortDirection
        };
        Pagination<TeamUserMapping> teamMembers = await _teamUserRepository.GetAllTeamMembersAsync(teamManagerId, userPagination);
        List<EmployeeViewModel> members = [];
        foreach (TeamUserMapping employee in teamMembers.Items)
        {
            if (employee.TeamMember != null)
            {
                ProjectUser? projectUser = await _projectRepository.GetProjectByUserIdAsnc(employee.TeamMember.UserId);
                ProjectViewModel projectViewModel = new();

                if (projectUser != null)
                {
                    Project? project = projectUser.Project;
                    if (project != null)
                    {
                        projectViewModel.ProjectId = project.ProjectId;
                        projectViewModel.ProjectName = project.ProjectName;
                    }
                }
                members.Add(new EmployeeViewModel
                {
                    EmployeeId = employee.UserId,
                    Name = employee.TeamMember.Name ?? string.Empty,
                    Email = employee.TeamMember.Email ?? string.Empty,
                    PhoneNumber = employee.TeamMember.PhoneNumber ?? string.Empty,
                    Role = employee.TeamMember.RoleId,
                    RoleName = employee.TeamMember.Role?.RoleName ?? string.Empty,
                    Status = employee.TeamMember.IsActive ? "Active" : "Inactive",
                    Project = projectViewModel,
                    ReportingPerson = employee.TeamManager != null ? new UserViewModel()
                    {
                        UserId = employee.TeamManager.UserId,
                        Name = employee.TeamManager.Name,
                        Email = employee.TeamManager.Email,
                        Role = employee.TeamManager.Role?.RoleName
                    } : null
                });
            }
        }

        return new Pagination<EmployeeViewModel>
        {
            Items = members,
            CurrentPage = teamMembers.CurrentPage,
            TotalPages = teamMembers.TotalPages,
            TotalRecords = teamMembers.TotalRecords,
            PageSize = teamMembers.PageSize
        };
    }

    public async Task<List<EmployeeViewModel>?> GetNotAssignedMembersAsync()
    {
        List<User> notAssignedMembers = await _employeeRepository.GetNotAssignedUsersAsync();
        List<EmployeeViewModel> members = [];
        foreach (User member in notAssignedMembers)
        {
            members.Add(new EmployeeViewModel()
            {
                EmployeeId = member.UserId,
                Name = member.Name,
                Email = member.Email,
                PhoneNumber = member.PhoneNumber
            });
        }
        return members;
    }

    public async Task<List<UserViewModel>> GetAllTeamMemberNamesAsync(int teamManagerid)
    {
        List<TeamUserMapping> teamMembers = await _teamUserRepository.GetAllTeamMemberNamesAsync(teamManagerid);
        List<UserViewModel> teamMembersViews = [];
        foreach (TeamUserMapping? teamMember in teamMembers)
        {
            teamMembersViews.Add(new UserViewModel()
            {
                UserId = teamMember.UserId ?? 0,
                Name = teamMember.TeamMember?.Name,
                Role = teamMember.TeamMember?.Role?.RoleName,
                Email = teamMember.TeamMember?.Email
            });
        }
        return teamMembersViews;
    }

    public async Task<bool> AddTeamMembersAsync(TeamMembersViewModel model, int teamManagerId)
    {
        List<TeamUserMapping> teamUsers = [];
        foreach (int teamUserId in model.TeamMemberIds)
        {
            teamUsers.Add(new TeamUserMapping()
            {
                TeamManagerId = model.TeamManagerId != null ? model.TeamManagerId : teamManagerId,
                UserId = teamUserId,
                CreatedAt = DateOnly.FromDateTime(DateTime.Now),
                IsDeleted = false
            });
        }
        return await _teamUserRepository.AddTeamMembersAsync(teamUsers);
    }

    public async Task<TeamViewModel?> GetTeamMemberByIdAsync(int teamMemberId, UserViewModel teamManager)
    {
        TeamUserMapping? teamMember = await _teamUserRepository.GetByIdAsync(teamMemberId);
        if (teamMember == null)
            return null;

        TeamViewModel? model = new TeamViewModel()
        {
            TeamMember = new UserViewModel()
            {
                UserId = teamMember.UserId ?? 0,
                Name = teamMember.TeamMember?.Name
            },
            SelectedTeamManagerId = teamMember.TeamManagerId,
            TeamManagers = await GetAllTeamMemberNamesAsync(teamManager.UserId)
        };
        model.TeamManagers.Add(new UserViewModel()
        {
            UserId = teamManager.UserId,
            Name = teamManager.Name
        });
        UserViewModel? itemToRemove = model.TeamManagers.SingleOrDefault(r => r.UserId == teamMemberId);
        if (itemToRemove != null)
        {
            model.TeamManagers.Remove(itemToRemove);
        }
        return model;
    }

    public async Task<bool> ChangeTeamMemberAsync(TeamViewModel model)
    {
        TeamUserMapping? teamUser = await _teamUserRepository.GetByIdAsync(model.TeamMember.UserId);
        if (teamUser != null)
        {
            teamUser.TeamManagerId = model.SelectedTeamManagerId;
            teamUser.UpdatedAt = DateOnly.FromDateTime(DateTime.Now);
            return await _teamUserRepository.UpdateAsync(teamUser);
        }
        else
        {
            return false;
        }
    }

    public async Task<bool> RemoveTeamMemberAsync(int teamMemberId)
    {
        TeamUserMapping? teamUser = await _teamUserRepository.GetByIdAsync(teamMemberId);
        if (teamUser == null)
        {
            return false;
        }

        teamUser.IsDeleted = true;
        teamUser.UpdatedAt = DateOnly.FromDateTime(DateTime.Now);
        return await _teamUserRepository.UpdateAsync(teamUser);
    }

    public async Task<List<TeamViewModel>> GetMembersByTeamManagerIdAsync(int teamManagerId)
    {
        List<TeamUserMapping> teamUserMappings = await _teamUserRepository.GetMembersByTeamManagerIdAsync(teamManagerId);

        List<TeamViewModel> teams = [];
        foreach (TeamUserMapping team in teamUserMappings)
        {
            TeamViewModel teamView = new();
            teamView.TeamManager = new()
            {
                UserId = team.TeamManager?.UserId ?? 0,
                Name = team.TeamManager?.Name,
                Email = team.TeamManager?.Email,
                Role = team.TeamManager?.Role?.RoleName
            };
            List<TeamUserMapping> teamMembers = await _teamUserRepository.GetMembersByTeamManagerIdAsync(team.TeamManagerId ?? 0);
            foreach (TeamUserMapping teamMember in teamMembers)
            {
                teamView.TeamMembers.Add(new UserViewModel()
                {
                    UserId = teamMember.UserId ?? 0,
                    Name = teamMember.TeamMember?.Name,
                    Email = teamMember.TeamMember?.Email,
                    Role = teamMember.TeamMember?.Role?.RoleName
                });
            }
            TeamViewModel? itemToAdd = teams.SingleOrDefault(r => r.TeamManager.UserId == teamView.TeamManager.UserId);
            if (itemToAdd == null)
            {
                teams.Add(teamView);
            }
        }
        return teams;
    }
}