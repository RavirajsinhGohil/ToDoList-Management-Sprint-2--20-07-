using ToDoListManagement.Entity.Helper;
using ToDoListManagement.Entity.ViewModel;

namespace ToDoListManagement.Service.Interfaces;

public interface ITeamService
{
    Task<Pagination<EmployeeViewModel>> GetMembersAsync(Pagination<EmployeeViewModel> pagination, int teamManagerId);
    Task<List<EmployeeViewModel>?> GetNotAssignedMembersAsync();
    Task<List<UserViewModel>> GetAllTeamMemberNamesAsync(int teamManagerid);
    Task<bool> AddTeamMembersAsync(TeamMembersViewModel model, int teamManagerId);
    Task<TeamViewModel?> GetTeamMemberByIdAsync(int teamMemberId, UserViewModel teamManager);
    Task<bool> ChangeTeamMemberAsync(TeamViewModel model);
    Task<bool> RemoveTeamMemberAsync(int teamMemberId);
    Task<List<TeamViewModel>> GetMembersByTeamManagerIdAsync(int teamManagerId);
}
