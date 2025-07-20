using ToDoListManagement.Entity.Helper;
using ToDoListManagement.Entity.ViewModel;

namespace ToDoListManagement.Service.Interfaces;

public interface ILeaveService
{
    Task<Pagination<LeaveViewModel>> GetPaginatedLeavesAsync(Pagination<LeaveViewModel> pagination, int? userId);
    Task<bool> AddLeaveAsync(LeaveViewModel leaveViewModel, UserViewModel user);
    Task<LeaveViewModel?> GetLeaveByIdAsync(int leaveId);
    Task<bool> UpdateLeaveAsync(LeaveViewModel model);
    Task<bool> DeleteLeaveAsync(int leaveId);
}
