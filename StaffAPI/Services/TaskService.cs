using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using StaffAPI.Helper;
using StaffAPI.Models.Tasks;
using StaffAPI.Models.Tasks.DTO;
using StaffAPI.Repository.Interfaces;
using StaffAPI.Services.Interfaces;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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
        private readonly List<int> _status = new List<int>() { 1, 2, 3, -9 };
        public TaskService(ITaskRepository Task, ILogger<TaskService> Log, IInvoiceRepository _invoice)
        {
            _Log = Log;
            _Log.LogInformation("Start object Task!");
            _Task = Task;
            this._invoice = _invoice;
        }

        #region Private
        private TaskResultDTO ValidateProcess(TaskResultDTO a, string id, int Status, int NextDepartment, out int indexDepartment)
        {
            indexDepartment = 1;
            var task = a.Data;
            if (task.WorkFlowId == 99)
            {
                _Log.LogError($"TaskProcess taskid={id}: Tasks do not apply workflow {task.WorkFlowId}");
                a.Status = 0;
                a.Code = 500;
                a.InternalMessage = LanguageAll.Language.NoPermission;
                a.UserMessage = LanguageAll.Language.NoPermission;
                a.UserMessage = LanguageAll.Language.NoPermission;
                return a;
            }
            if (Status == 3 && NextDepartment != -1 || NextDepartment == -1 && Status != 3)
            {
                _Log.LogError($"TaskProcess taskid={id}: The current step of the task needs to be reopened if you want to go to the previous step {Status} || {NextDepartment}");
                return new TaskResultDTO()
                {
                    Data = task,
                    Code = 500,
                    InternalMessage = LanguageAll.Language.NoPermission,
                    MoreInfo = LanguageAll.Language.NoPermission,
                    UserMessage = LanguageAll.Language.NoPermission,
                    Status = 0
                };
            }
            
            indexDepartment = GetCurrentDepartment(task);
            if (Status == 2)
            {
                if (indexDepartment < task.Department.Count && NextDepartment != 1)
                {
                    _Log.LogError($"TaskProcess taskid={id}: The current step {indexDepartment} of the task is done, then you have to go to the next step {Status} || {NextDepartment}");
                    return new TaskResultDTO()
                    {
                        Data = task,
                        Code = 500,
                        InternalMessage = LanguageAll.Language.NoPermission,
                        MoreInfo = LanguageAll.Language.NoPermission,
                        UserMessage = LanguageAll.Language.NoPermission,
                        Status = 0
                    };
                }

                if (indexDepartment == task.Department.Count && NextDepartment != 0)
                {
                    _Log.LogError($"TaskProcess taskid={id}: The last step {indexDepartment} of the task is done, then you have to saved current step {Status} || {NextDepartment}");
                    return new TaskResultDTO()
                    {
                        Data = task,
                        Code = 500,
                        InternalMessage = LanguageAll.Language.NoPermission,
                        MoreInfo = LanguageAll.Language.NoPermission,
                        UserMessage = LanguageAll.Language.NoPermission,
                        Status = 0
                    };
                }
            }

            return a;
        }
        private async Task<TaskResultDTO> ValidateProcess(string id, int Status)
        {
            if (!_status.Contains(Status))
            {
                _Log.LogError($"TaskProcess taskid={id}: Status is wrong {Status}");
                return new TaskResultDTO()
                {
                    Data = null,
                    Code = 500,
                    InternalMessage = LanguageAll.Language.StatusIsWrong,
                    MoreInfo = LanguageAll.Language.StatusIsWrong,
                    UserMessage = LanguageAll.Language.StatusIsWrong,
                    Status = 0
                };
            }
            TaskResultDTO a = await GetTaskById(id);
            if (a.Code == 500)
            {
                _Log.LogError($"TaskProcess taskid={id}: Notfound");
                return a;
            }
            return a;
        }
        private int GetCurrentDepartment(TaskDTO task)
        {
            int cntStep = task.Department.Count;
            int i = 0;
            bool found = false;
            while ((i < cntStep) && !found)
            {
                if (task.Department[i].StatusId.Value != 1) found = true;
                i++;
            }
            return i;
        }
        #endregion

        #region Task
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

        // Assign staff
        public async Task<TaskResultDTO> AssignStaff(string id, StaffDTO staff, bool isAdd = true)
        {
            return await _Task.UpdateAsync(id, staff, isAdd);
        }
        // Assign casher
        public async Task<TaskResultDTO> AssignCasher(string id, CasherDTO staff)
        {
            return await _Task.UpdateAsync(id, staff);
        }

        // Task process
        public async Task<TaskResultDTO> TaskProcess(string id, TaskProcessDTO taskProcess, int Status)
        {
            return await _Task.UpdateAsync(id, taskProcess, Status);
        }
        // Task process
        public async Task<TaskResultDTO> TaskProcess(string id, TaskProcessDTO taskProcess, 
            int NextDepartment, IWorkFlowConfig workFlow, int Status)
        {
            TaskResultDTO a = await ValidateProcess(id, Status);
            if (a.Code == 500) return a;
            TaskDTO task = a.Data;

            int indexDepartment = 1;
            a = ValidateProcess(a, id, Status, NextDepartment, out indexDepartment);
            if (a.Code == 500) return a;

            switch (NextDepartment)
            {
                case 0:
                    task.Department[indexDepartment - 1].StatusId = Status;
                    if (Status == 2 && indexDepartment == task.Department.Count || Status == -9) task.Status = Status;
                    break;
                case 1:
                    task.Department[indexDepartment - 1].StatusId = Status;
                    task.Department[indexDepartment].StatusId = 1;
                    break;
                case -1:
                    task.Department[indexDepartment - 1].StatusId = Status;
                    task.Department[indexDepartment - 2].StatusId = 1;
                    break;
                default:
                    break;
            }
            return await _Task.UpdateAsync(task, taskProcess);
        }
        #endregion

        #region Other
        // Finding customer
        public async Task<ContractResult> GetCustomer(ContractInput inv)
        {
            return await _invoice.GetContract(inv);
        }

        // Finding staff
        public async Task<StaffInfoResult> GetStaff(StaffCodeInput inv)
        {
            return await _invoice.getStaffInfo(inv);
        }
        #endregion
    }
}
