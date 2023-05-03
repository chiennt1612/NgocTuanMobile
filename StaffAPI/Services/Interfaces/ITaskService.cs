using StaffAPI.Models.Tasks;
using StaffAPI.Models.Tasks.DTO;
using System.Threading.Tasks;
using Utils.Models;

namespace StaffAPI.Services.Interfaces
{
    public interface ITaskService
    {
        #region Other
        // Finding staff
        Task<StaffInfoResult> GetStaff(StaffCodeInput inv);
        // Finding customer
        Task<ContractResult> GetCustomer(ContractInput inv);
        #endregion
        #region Task
        // Finding the task by ID
        Task<TaskResultDTO> GetTaskById(string Id);
        // Finding the task list
        Task<TaskListResultDTO> GetTaskList(TaskFilterDTO _filter);
        // Create a work.
        Task<TaskResultDTO> CreateTask(TaskDTO task);
        // Update a work.
        Task<TaskResultDTO> UpdateTask(TaskDTO task);
        // Assign staff
        Task<TaskResultDTO> AssignStaff(string id, StaffDTO staff, bool isAdd = true);
        // Assign casher
        Task<TaskResultDTO> AssignCasher(string id, CasherDTO staff);
        // Task process
        Task<TaskResultDTO> TaskProcess(string id, TaskProcessDTO taskProcess, int Status);
        // Task process
        Task<TaskResultDTO> TaskProcess(string id, TaskProcessDTO taskProcess, int NextDepertment, IWorkFlowConfig workFlow, int Status);
        #endregion
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
