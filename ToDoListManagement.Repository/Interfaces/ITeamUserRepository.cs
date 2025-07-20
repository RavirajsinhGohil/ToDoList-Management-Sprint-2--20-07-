using ToDoListManagement.Entity.Helper;
using ToDoListManagement.Entity.Models;

namespace ToDoListManagement.Repository.Interfaces;

public interface ITeamUserRepository : IBaseRepository<TeamUserMapping>
{
    new Task<List<TeamUserMapping>> GetAllAsync();
    new Task<TeamUserMapping?> GetByIdAsync(int id);
    new Task<bool> AddAsync(TeamUserMapping entity);
    new Task<bool> UpdateAsync(TeamUserMapping entity);
    Task<Pagination<TeamUserMapping>> GetAllTeamMembersAsync(int teamManagerId, Pagination<User> pagination);
    Task<List<TeamUserMapping>> GetAllTeamMemberNamesAsync(int teamManagerId);
    Task<bool> AddTeamMembersAsync(List<TeamUserMapping> teamUserMappings);
    Task<List<TeamUserMapping>> GetMembersByTeamManagerIdAsync(int teamManagerId);
}