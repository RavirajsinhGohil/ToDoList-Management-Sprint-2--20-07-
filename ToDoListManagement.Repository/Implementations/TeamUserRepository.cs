using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Npgsql;
using ToDoListManagement.Entity.Data;
using ToDoListManagement.Entity.Helper;
using ToDoListManagement.Entity.Models;
using ToDoListManagement.Repository.Interfaces;

namespace ToDoListManagement.Repository.Implementations;

public class TeamUserRepository : ITeamUserRepository
{
    private readonly ToDoListDbContext _context;
    private readonly IConfiguration _configuration;
    public TeamUserRepository(ToDoListDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<List<TeamUserMapping>> GetAllAsync()
    {
        return await _context.Set<TeamUserMapping>().ToListAsync();
    }

    public async Task<TeamUserMapping?> GetByIdAsync(int id)
    {
        return await _context.Set<TeamUserMapping>().Where(tu => !tu.IsDeleted).Include(tu => tu.TeamMember).Include(tu => tu.TeamManager).FirstAsync(tu => tu.UserId == id);
    }

    public async Task<bool> AddAsync(TeamUserMapping entity)
    {
        await _context.Set<TeamUserMapping>().AddAsync(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateAsync(TeamUserMapping entity)
    {
        _context.Set<TeamUserMapping>().Update(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Pagination<TeamUserMapping>> GetAllTeamMembersAsync(int teamManagerId, Pagination<User> pagination)
    {
        string? connectionString = _configuration["ConnectionStrings:ToDoListDbConnection"];
        List<TeamUserMapping> teamMembers = [];

        using NpgsqlConnection conn = new(connectionString);
        await conn.OpenAsync();

        using NpgsqlCommand cmd = new("SELECT * FROM get_team_hierarchy(@manager_id, @search_keyword, @sort_column, @sort_direction, @page_size, @page_number)", conn);
        cmd.Parameters.AddWithValue("manager_id", teamManagerId);
        cmd.Parameters.AddWithValue("search_keyword", pagination.SearchKeyword ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("sort_column", pagination.SortColumn ?? "name");
        cmd.Parameters.AddWithValue("sort_direction", pagination.SortDirection ?? "asc");
        cmd.Parameters.AddWithValue("page_size", pagination.PageSize);
        cmd.Parameters.AddWithValue("page_number", pagination.CurrentPage);

        using NpgsqlDataReader? reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            teamMembers.Add(new TeamUserMapping
            {
                TeamUserMappingId = reader.GetInt32(0),
                TeamManagerId = reader.GetInt32(1),
                TeamManager = new User()
                {
                    UserId = reader.GetInt32(1),
                    Name = reader.IsDBNull(2) ? null : reader.GetString(2),
                    Role = new Role()
                    {
                        RoleName = reader.IsDBNull(3) ? null : reader.GetString(3),
                    }
                },
                UserId = reader.GetInt32(4),
                TeamMember = new User()
                {
                    UserId = reader.GetInt32(4),
                    Name = reader.IsDBNull(5) ? null : reader.GetString(5),
                    Email = reader.IsDBNull(6) ? null : reader.GetString(6),
                    Role = new Role()
                    {
                        RoleName = reader.IsDBNull(7) ? null : reader.GetString(7),
                    },
                    PhoneNumber = reader.IsDBNull(8) ? null : reader.GetString(8)
                }
            });
        }

        int totalRecords = teamMembers.Count;
        int totalPages = (int)Math.Ceiling((double)totalRecords / pagination.PageSize);

        return new Pagination<TeamUserMapping>
        {
            Items = teamMembers,
            CurrentPage = pagination.CurrentPage,
            PageSize = pagination.PageSize,
            TotalRecords = totalRecords,
            TotalPages = totalPages
        };
    }

    public async Task<List<TeamUserMapping>> GetAllTeamMemberNamesAsync(int teamManagerId)
    {
        List<TeamUserMapping>? allMappings = await _context.TeamUserMappings
            .Where(t => !t.IsDeleted)
            .Include(t => t.TeamManager)
            .ThenInclude(tm => tm.Role)
            .Include(t => t.TeamMember)
            .ThenInclude(tm => tm.Role)
            .ToListAsync();

        List<TeamUserMapping> result = [];
        GetSubTeamMembers(teamManagerId, allMappings, result);
        return result.OrderBy(t => t.TeamMember != null ? t.TeamMember.Name : string.Empty).ToList();
    }

    private static void GetSubTeamMembers(int managerId, List<TeamUserMapping> allMappings, List<TeamUserMapping> result)
    {
        List<TeamUserMapping>? filterdRecords = allMappings
            .Where(t => t.TeamManagerId == managerId)
            .ToList();

        foreach (TeamUserMapping? report in filterdRecords)
        {
            result.Add(report);
            if (report.UserId != null)
            {
                GetSubTeamMembers(report.UserId.Value, allMappings, result);
            }
        }
    }

    public async Task<bool> AddTeamMembersAsync(List<TeamUserMapping> teamUserMappings)
    {
        await _context.TeamUserMappings.AddRangeAsync(teamUserMappings);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<TeamUserMapping>> GetMembersByTeamManagerIdAsync(int teamManagerId)
    {
        return await _context.Set<TeamUserMapping>()
                .Where(tu => tu.TeamManagerId == teamManagerId).Include(t => t.TeamManager)
                .ThenInclude(tm => tm.Role)
                .Include(t => t.TeamMember)
                .ThenInclude(tm => tm.Role)
                .ToListAsync();
    }
}