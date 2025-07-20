using ToDoListManagement.Entity.Data;
using ToDoListManagement.Repository.Interfaces;
using ToDoListManagement.Entity.Models;
using Microsoft.EntityFrameworkCore;
using ToDoListManagement.Entity.Helper;

namespace ToDoListManagement.Repository.Implementations;

public class LeaveRepository : ILeaveRepository
{
    private readonly ToDoListDbContext _context;
    public LeaveRepository(ToDoListDbContext context)
    {
        _context = context;
    }

    public async Task<List<Leave>> GetAllAsync()
    {
        return await _context.Leaves.ToListAsync();
    }
    
    public async Task<Leave?> GetByIdAsync(int id)
    {
        return await _context.Leaves
            .Include(l => l.RequestedUser)
            .Include(l => l.ApprovalUser)
            .FirstOrDefaultAsync(l => l.LeaveId == id && !l.IsDeleted);
    }
    
    public async Task<bool> AddAsync(Leave entity)
    {
        await _context.Leaves.AddAsync(entity);
        await _context.SaveChangesAsync();
        return true;
    }
    
    public async Task<bool> UpdateAsync(Leave entity)
    {
        _context.Leaves.Update(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Pagination<Leave>> GetPaginatedLeavesAsync(Pagination<Leave> pagination, int? userId)
    {
        IQueryable<Leave> query = _context.Leaves.Include(l => l.RequestedUser).Include(l => l.ApprovalUser).Where(l => !l.IsDeleted ).AsQueryable();

        if(userId != null)
        {
            query = query.Where(l => l.RequestedUserId == userId);
        }
        
        if (!string.IsNullOrEmpty(pagination.SearchKeyword))
        {
            query = query.Where(l => (l.RequestedUser != null && l.RequestedUser.Name != null && l.RequestedUser.Name.ToLower().Contains(pagination.SearchKeyword.Trim().ToLower())) || (l.Status != null && l.Status.ToLower().Contains(pagination.SearchKeyword.Trim().ToLower()) ));
        }

        query = pagination.SortColumn switch
        {
            "requestedName" => (pagination.SortDirection?.ToLower() ?? "asc") == "asc"
                                ? query.OrderBy(p => p.RequestedUser.Name)
                                : query.OrderByDescending(p => p.RequestedUser.Name),

            "startDate" => (pagination.SortDirection?.ToLower() ?? "asc") == "asc"
                                ? query.OrderBy(p => p.StartDate)
                                : query.OrderByDescending(p => p.StartDate),

            "endDate" => (pagination.SortDirection?.ToLower() ?? "asc") == "asc"
                                ? query.OrderBy(p => p.EndDate)
                                : query.OrderByDescending(p => p.EndDate),

            _ => pagination.SortDirection?.ToLower() == "asc"
                                ? query.OrderBy(p => p.StartDate)
                                : query.OrderByDescending(p => p.StartDate),
        };

        if (!string.IsNullOrEmpty(pagination.StatusFilter) && pagination.StatusFilter != "All")
        {
            query = query.Where(l => l.Status != null && l.Status.ToLower() == pagination.StatusFilter.ToLower());
        }

        if(pagination.StartDate != null)
        {
            query = query.Where(l => l.StartDate >= pagination.StartDate);
        }

        if(pagination.EndDate != null)
        {
            query = query.Where(l => l.EndDate <= pagination.EndDate);
        }

        List<Leave> pagedData = await query
            .Skip((pagination.CurrentPage - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync();

        int totalRecords = await query.CountAsync();
        int totalPages = (int)Math.Ceiling((double)totalRecords / pagination.PageSize);

        return new Pagination<Leave>
        {
            Items = pagedData,
            CurrentPage = pagination.CurrentPage,
            TotalPages = totalPages,
            TotalRecords = totalRecords
        };
    }

    public async Task<int> GetCountOfLeavesAsync(int userId)
    {
        return await _context.Leaves.CountAsync(l => l.RequestedUserId == userId && !l.IsDeleted);
    }
}