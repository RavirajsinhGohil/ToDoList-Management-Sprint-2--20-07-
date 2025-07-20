using ToDoListManagement.Entity.ViewModel;

namespace ToDoListManagement.Service.Interfaces;

public interface ISprintService
{
    Task<List<SprintViewModel>> GetAllSprints(int projetId);
    Task<bool> AddSprintAsync(SprintViewModel model);
    Task<bool> CheckSprintNameExistsAsync(string sprintName, int sprintId = 0);
    Task<SprintViewModel?> GetSprintByIdAsync(int sprintId);
    Task<bool> StartSprintAsync(int sprintId);
    Task<bool> CompleteSprintAsync(int sprintId);
}