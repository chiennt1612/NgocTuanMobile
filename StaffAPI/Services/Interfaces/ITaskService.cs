using StaffAPI.Models.Tasks;
using StaffAPI.Models.Tasks.DTO;
using System.Threading.Tasks;
using Utils.Models;

namespace StaffAPI.Services.Interfaces
{
    public interface ITaskService
    {
        // Finding customer
        Task<ContractResult> GetCustomer(ContractInput inv);
        // Finding the task by ID
        Task<TaskResultDTO> GetTaskById(string Id);
        // Finding the task list
        Task<TaskListResultDTO> GetTaskList(TaskFilterDTO _filter);
        // Create a work.
        Task<TaskResultDTO> CreateTask(TaskDTO task);
        // Update a work.
        Task<TaskResultDTO> UpdateTask(TaskDTO task);

        //Task<TaskListResultDTO> GetAsync(TaskFilterDTO _filter);
        //Task<TaskResultDTO> GetAsync(string id);
        //Task<TaskResultDTO> CreateAsync(TaskDTO task);
        //Task<TaskResultDTO> UpdateAsync(string id, TaskDTO task);
        //Task<TaskResultDTO> RemoveAsync(TaskDTO task);
        //Task<TaskResultDTO> RemoveAsync(string id);
        //Task<TaskResultDTO> UpdateAsync(string id, TaskProcessDTO taskProcess);
        //Task<TaskResultDTO> UpdateAsync(string id, DepartmentDTO department);
        //Task<TaskResultDTO> UpdateAsync(string id, StaffDTO staff);
        //Task<TaskResultDTO> UpdateAsync(string id, CasherDTO staff);
    }
}
