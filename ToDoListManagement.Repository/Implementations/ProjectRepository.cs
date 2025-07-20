using Microsoft.EntityFrameworkCore;
using ToDoListManagement.Entity.Data;
using ToDoListManagement.Entity.Helper;
using ToDoListManagement.Entity.Models;
using ToDoListManagement.Repository.Interfaces;

namespace ToDoListManagement.Repository.Implementations;

public class ProjectRepository : IProjectRepository
{
    private readonly ToDoListDbContext _context;
    public ProjectRepository(ToDoListDbContext context)
    {
        _context = context;
    }

    public async Task<Pagination<Project>> GetPaginatedProjectsAsync(Pagination<Project> pagination, int userId, bool isAdmin)
    {
        IQueryable<Project> query = _context.Projects
            .Where(p => !p.IsDeleted)
            .Include(pu => pu.ProjectUsers)
            .ThenInclude(u => u.User)
            .Include(p => p.AssignedPM);

        if (!isAdmin)
        {
            query = query.Where(pu => pu.ProjectUsers != null && pu.ProjectUsers.Any(u => u.UserId == userId && u.User != null && !u.User.IsDeleted));
        }

        if (!string.IsNullOrEmpty(pagination.SearchKeyword))
        {
            query = query.Where(p => p.ProjectName != null && p.ProjectName.ToLower().Contains(pagination.SearchKeyword.Trim().ToLower()));
        }

        query = pagination.SortColumn switch
        {
            "projectName" => pagination.SortDirection?.ToLower() == "asc"
                                ? query.OrderBy(p => p.ProjectName)
                                : query.OrderByDescending(p => p.ProjectName),
            _ => pagination.SortDirection?.ToLower() == "asc"
                                ? query.OrderBy(p => p.ProjectId)
                                : query.OrderByDescending(p => p.ProjectId),
        };

        List<Project> pagedData = await query
            .Skip((pagination.CurrentPage - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync();
        
        int totalRecords = await query.CountAsync();
        int totalPages = (int)Math.Ceiling((double)totalRecords / pagination.PageSize);

        return new Pagination<Project>
        {
            Items = pagedData,
            CurrentPage = pagination.CurrentPage,
            TotalPages = totalPages,
            TotalRecords = totalRecords
        };
    }

    public async Task<bool> AddProject(Project project)
    {
        await _context.Projects.AddAsync(project);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AddProjectUserAsync(ProjectUser projectUser)
    {
        await _context.ProjectUsers.AddAsync(projectUser);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Project?> GetProjectByIdAsync(int projectId)
    {
        return await _context.Projects
            .Include(p => p.ProjectUsers)
            .Include(p => p.AssignedPM)
            .FirstOrDefaultAsync(p => p.ProjectId == projectId && !p.IsDeleted);
    }

    public async Task<List<User>> GetProjectManagersAsync()
    {
        return await _context.Users
            .Where(u => u.RoleId == 2 && !u.IsDeleted)
            .ToListAsync();
    }

    public async Task<List<User>> GetScrumMastersAsync()
    {
        return await _context.Users
            .Where(u => u.RoleId == 4 && !u.IsDeleted)
            .ToListAsync();
    }

    public async Task<Project?> GetProjectByNameAsync(string projectName)
    {
        return await _context.Projects.FirstOrDefaultAsync(p => p.ProjectName != null && p.ProjectName.Trim().ToLower() == projectName);
    }

    public async Task<bool> UpdateProjectAsync(Project project)
    {
        _context.Projects.Update(project);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Pagination<User>> GetPaginatedMembersAsync(Pagination<User> pagination)
    {
        IQueryable<User>? query = _context.Users.Where(u => u.RoleId != 1 && u.RoleId != 2 && u.RoleId != 4 && !u.IsDeleted);

        if (!string.IsNullOrEmpty(pagination.SearchKeyword))
        {
            query = query.Where(u => u.Name != null && u.Name.ToLower().Contains(pagination.SearchKeyword.Trim().ToLower()) || u.Email != null && u.Email.Contains(pagination.SearchKeyword.Trim().ToLower()) || u.PhoneNumber != null && u.PhoneNumber.Contains(pagination.SearchKeyword.Trim().ToLower()));
        }

        query = pagination.SortColumn switch
        {
            "name" => pagination.SortDirection?.ToLower() == "asc"
                                ? query.OrderBy(p => p.Name)
                                : query.OrderByDescending(p => p.Name),
            "email" => pagination.SortDirection?.ToLower() == "asc"
                                ? query.OrderBy(p => p.Email)
                                : query.OrderByDescending(p => p.Email),
            "phoneNumber" => pagination.SortDirection?.ToLower() == "asc"
                                ? query.OrderBy(p => p.PhoneNumber)
                                : query.OrderByDescending(p => p.PhoneNumber),
            _ => pagination.SortDirection?.ToLower() == "asc"
                                ? query.OrderBy(p => p.UserId)
                                : query.OrderByDescending(p => p.UserId),
        };

        int totalRecords = await query.CountAsync();
        int totalPages = (int)Math.Ceiling((double)totalRecords / pagination.PageSize);

        List<User> pagedData = await query
            .Skip((pagination.CurrentPage - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync();

        return new Pagination<User>
        {
            Items = pagedData,
            CurrentPage = pagination.CurrentPage,
            TotalPages = totalPages,
            TotalRecords = totalRecords
        };
    }

    public async Task<List<ProjectUser>> GetAssignedMembers(int projectId)
    {
        return await _context.ProjectUsers.Include(pu => pu.User).Where(pu => pu.ProjectId == projectId && pu.User != null && pu.User.RoleId != 1 && pu.User.RoleId != 2).ToListAsync();
    }

    public async Task<bool> DeleteProjectAsync(int projectId, int userId)
    {
        Project? project = await _context.Projects.FirstOrDefaultAsync(p => p.ProjectId == projectId && !p.IsDeleted);

        if (project == null)
        {
            return false;
        }

        project.IsDeleted = true;
        _context.Projects.Update(project);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> AssignMembersAsync(int projectId, List<int>? userIds)
    {
        IQueryable<ProjectUser>? existingUsers = _context.ProjectUsers.Where(pu => pu.ProjectId == projectId && pu.User != null && pu.User.RoleId != 2);
        _context.ProjectUsers.RemoveRange(existingUsers);

        IEnumerable<ProjectUser>? newProjectUsers = (userIds ?? new List<int>()).Select(id => new ProjectUser
        {
            ProjectId = projectId,
            UserId = id
        });

        await _context.ProjectUsers.AddRangeAsync(newProjectUsers);

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<Project>> GetProjectNamesAsync(int userId, bool isAdmin)
    {
        IQueryable<Project> query = _context.Projects
            .Where(p => !p.IsDeleted)
            .Include(pu => pu.ProjectUsers)
            .ThenInclude(u => u.User)
            .Include(p => p.AssignedPM);

        if (!isAdmin)
        {
            query = query.Where(pu => pu.ProjectUsers != null && pu.ProjectUsers.Any(u => u.UserId == userId && u.User != null && !u.User.IsDeleted));
        }

        return await query.OrderBy(p => p.ProjectName).ToListAsync();
    }

    public async Task<ProjectUser?> GetProjectByUserIdAsnc(int userId)
    {
        ProjectUser? projectUser =  _context.ProjectUsers.Where(pu => pu.UserId == userId).Include(pu => pu.Project).FirstOrDefault();
        return projectUser != null ? projectUser : null;
    }
}