using StaffAPI.Models.Tasks.DTO;
using System.Threading.Tasks;
using Utils.Models;

namespace StaffAPI.Repository.Interfaces
{
    public interface ITaskRepository
    {
        Task<TaskListResultDTO> GetAsync(TaskFilterDTO _filter);
        Task<TaskResultDTO> GetAsync(string id);
        Task<TaskResultDTO> CreateAsync(TaskDTO task);
        Task<TaskResultDTO> UpdateAsync(string id, TaskDTO task);
        Task<TaskResultDTO> RemoveAsync(TaskDTO task);
        Task<TaskResultDTO> RemoveAsync(string id);
        Task<TaskResultDTO> UpdateAsync(string id, TaskProcessDTO taskProcess, int Status);
        Task<TaskResultDTO> UpdateAsync(string id, TaskProcessDTO taskProcess, DepartmentDTO department, int Status);
        Task<TaskResultDTO> UpdateAsync(string id, StaffDTO staff, bool isAdd = true);
        Task<TaskResultDTO> UpdateAsync(string id, CasherDTO staff);
    }
}
