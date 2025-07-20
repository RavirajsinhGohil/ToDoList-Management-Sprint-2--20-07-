using Org.BouncyCastle.Crypto.Fpe;
using ToDoListManagement.Entity.Models;
using ToDoListManagement.Entity.ViewModel;
using ToDoListManagement.Repository.Interfaces;
using ToDoListManagement.Service.Interfaces;

namespace ToDoListManagement.Service.Implementations;

public class DashboardService : IDashboardService
{
    private readonly IProjectRepository _projectRepository;
    private readonly ILeaveRepository _leaveRepository;
    private readonly ITaskRepository _taskRepository;
    private readonly IEmployeeRepository _employeeRepository;
    public DashboardService(IProjectRepository projectRepository, ILeaveRepository leaveRepository, ITaskRepository taskRepository, IEmployeeRepository employeeRepository)
    {
        _projectRepository = projectRepository;
        _leaveRepository = leaveRepository;
        _taskRepository = taskRepository;
        _employeeRepository = employeeRepository;
    }

    public async Task<DashboardViewModel> GetDashboardData(UserViewModel user)
    {
        DashboardViewModel model = new();
        bool isAdmin = user.Role == "Admin";
        List<Project> projects = await _projectRepository.GetProjectNamesAsync(user.UserId, isAdmin);
        model.TotalNoOfProjects = projects.Count;
        model.TotalNoOfLeaves = await _leaveRepository.GetCountOfLeavesAsync(user.UserId);
        model.TotalNoOfTasks = await _taskRepository.GetCountOfTasksAsync(user.UserId);
        model.TotalNoOfUsers = await _employeeRepository.GetCountOfEmployeesAsync();
        foreach(Project project in projects)
        {
            model.Projects.Add(new ProjectViewModel() {
                ProjectId = project.ProjectId,
                ProjectName = project.ProjectName,
                Status = project.Status,
                PMName = project.AssignedPM?.Name
            });
        }
        return model;
    }
}