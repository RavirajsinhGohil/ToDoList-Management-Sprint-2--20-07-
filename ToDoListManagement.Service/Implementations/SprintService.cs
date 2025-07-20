using ToDoListManagement.Entity.Models;
using ToDoListManagement.Entity.ViewModel;
using ToDoListManagement.Repository.Interfaces;
using ToDoListManagement.Service.Interfaces;

namespace ToDoListManagement.Service.Implementations;

public class SprintService : ISprintService
{
    private readonly ISprintRepository _sprintRepository;
    private readonly ITaskRepository _taskRepository;

    public SprintService(ISprintRepository sprintRepository, ITaskRepository taskRepository)
    {
        _sprintRepository = sprintRepository;
        _taskRepository = taskRepository;
    }

    public async Task<List<SprintViewModel>> GetAllSprints(int projetId)
    {
        List<Sprint> sprints = await _sprintRepository.GetSprintsByProjectId(projetId);
        List<SprintViewModel> sprintViewModels = [];
        foreach(Sprint? sprint in sprints)
        {
            sprintViewModels.Add(new() {
                SprintId = sprint.SprintId,
                Name = sprint.SprintName,
                Status = sprint.Status,
                StartDate = sprint.StartDate,
                EndDate = sprint.EndDate,
                ScrumMasterId = sprint.ScrumMasterId,
                ProjectId = sprint.ProjectId
            });
        }
        return sprintViewModels;
    }

    public async Task<bool> AddSprintAsync(SprintViewModel model)
    {
        Sprint? sprint = new() {
            SprintName = model.Name,
            Status = "Not Started",
            Description = model.Description,
            ScrumMasterId = model.ScrumMasterId ?? 0,
            ProjectId = model.ProjectId
        };

        return await _sprintRepository.AddAsync(sprint);
    }

    public async Task<bool> CheckSprintNameExistsAsync(string sprintName, int projectId)
    {
        Sprint? sprint = await _sprintRepository.GetSprintByNameAsync(sprintName.Trim().ToLower(), projectId);
        return sprint != null;
    }

    public async Task<SprintViewModel?> GetSprintByIdAsync(int sprintId)
    {
        Sprint? sprint = await _sprintRepository.GetByIdAsync(sprintId);
        if (sprint == null)
        {
            return null;
        }
        List<ToDoList>? tasks = await _taskRepository.GetTasksBySprintIdAsync(sprintId);
        int progress = 0;
        if (tasks != null)
        {
            int totalTasks = tasks.Count;
            int completedTasks = tasks.Count(t => t.Status == "Done");
            progress = totalTasks > 0 ? (completedTasks * 100 / totalTasks) : 0;
        }
        else
        {
            progress = 0;
        }

        if (sprint != null)
        {
            return new SprintViewModel
            {
                SprintId = sprint.SprintId,
                Name = sprint.SprintName,
                Description = sprint.Description,
                Status = sprint.Status,
                StartDate = sprint.StartDate,
                EndDate = sprint.EndDate,
                ScrumMasterId = sprint.ScrumMasterId,
                ProjectId = sprint.ProjectId,
                Progress = progress
            };
        }
        return null;
    }

    public async Task<bool> StartSprintAsync(int sprintId)
    {
        List<SprintViewModel> sprints = await GetAllSprints(sprintId);
        if (sprints.Any(s => s.SprintId != sprintId && s.Status == "In Progress"))
        {
            return false;
        }
        Sprint? sprint = await _sprintRepository.GetByIdAsync(sprintId);
        if (sprint == null)
        {
            return false;
        }

        List<ToDoList> tasks = await _taskRepository.GetTasksBySprintIdAsync(sprintId);
        if (tasks.Count == 0)
        {
            return false;
        }
        if (tasks.Any(t => t.Status != "To Do"))
        {
            return false;
        }

        sprint.Status = "In Progress";
        sprint.StartDate = DateTime.UtcNow;
        sprint.EndDate = DateTime.UtcNow.AddDays(10);

        return await _sprintRepository.UpdateAsync(sprint);
    }

    public async Task<bool> CompleteSprintAsync(int sprintId)
    {
        Sprint? sprint = await _sprintRepository.GetByIdAsync(sprintId);
        if (sprint == null)
        {
            return false;
        }

        List<ToDoList> tasks = await _taskRepository.GetTasksBySprintIdAsync(sprintId);
        if (tasks.Any(t => t.Status != "Done"))
        {
            return false;
        }
        sprint.Status = "Completed";

        return await _sprintRepository.UpdateAsync(sprint);
    }
}