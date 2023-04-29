using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using StaffAPI.Helper;
using StaffAPI.Models.Tasks;
using StaffAPI.Models.Tasks.DTO;
using StaffAPI.Repository.Interfaces;
using StaffAPI.Services.Interfaces;
using System.Threading.Tasks;
using Utils.Models;
using Utils.Repository.Interfaces;

namespace StaffAPI.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _Task;
        private readonly IInvoiceRepository _invoice;
        private readonly ILogger<TaskService> _Log;
        public TaskService(ITaskRepository Task, ILogger<TaskService> Log, IInvoiceRepository _invoice)
        {
            _Log = Log;
            _Log.LogInformation("Start object Task!");
            _Task = Task;
            this._invoice = _invoice;
        }

        // Finding the task by ID
        public async Task<TaskResultDTO> GetTaskById(string Id)
        {
            return await _Task.GetAsync(Id);
        }

        // Finding the task list
        public async Task<TaskListResultDTO> GetTaskList(TaskFilterDTO _filter)
        {
            return await _Task.GetAsync(_filter);
        }

        // Create a work.
        public async Task<TaskResultDTO> CreateTask(TaskDTO task)
        {
            return await _Task.CreateAsync(task);
        }

        // Update a work.
        public async Task<TaskResultDTO> UpdateTask(TaskDTO task)
        {
            return await _Task.UpdateAsync(task.Id, task);
        }

        // Finding customer
        public async Task<ContractResult> GetCustomer(ContractInput inv)
        {
            return await _invoice.GetContract(inv);
        }
    }
}
