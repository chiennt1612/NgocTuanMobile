﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using StaffAPI.Helper;
using StaffAPI.Models.Tasks;
using StaffAPI.Models.Tasks.DTO;
using StaffAPI.Repository.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils.Models;
using Utils.Repository.Interfaces;
using static MongoDB.Libmongocrypt.CryptContext;

namespace StaffAPI.Repository
{
    public class TaskRepository : ITaskRepository
    {
        private readonly IMongoCollection<TaskDTO> _Task;
        private readonly ILogger<TaskRepository> _Log;
        private readonly ICompanyConfig _companyConfig;
        private readonly IWorkFlowConfig _workFlow;
        private readonly IInvoiceRepository _iInvoiceServices;
        private readonly List<int> _status = new List<int>() { 0, 1, 2, 3, -9 };
        private readonly List<int> _paymentStatus = new List<int>() { 0, 1 };
        public TaskRepository(IDBSetting settings, ILogger<TaskRepository> Log, ICompanyConfig companyConfig, IInvoiceRepository iInvoiceServices, IWorkFlowConfig workFlow)
        {
            _Log = Log;
            _Log.LogInformation("Start object Task!");
            var client = new MongoClient(settings.MONGODB_URL);
            var database = client.GetDatabase(settings.DatabaseName);

            _Task = database.GetCollection<TaskDTO>("Tasks");
            _companyConfig = companyConfig;
            _iInvoiceServices = iInvoiceServices;
            _workFlow = workFlow;
        }

        #region validate
        private TaskResultDTO Validate(int status, bool isPayment = false)
        {
            if (!_status.Contains(status) && !isPayment)
            {
                return new TaskResultDTO()
                {
                    Data = null,
                    Status = 0,
                    Code = StatusCodes.Status500InternalServerError,
                    UserMessage = LanguageAll.Language.StatusIsWrong,
                    InternalMessage = LanguageAll.Language.StatusIsWrong,
                    MoreInfo = LanguageAll.Language.StatusIsWrong
                };
            }
            if (!_paymentStatus.Contains(status))
            {
                return new TaskResultDTO()
                {
                    Data = null,
                    Status = 0,
                    Code = StatusCodes.Status500InternalServerError,
                    UserMessage = LanguageAll.Language.PaymentStatusIsWrong,
                    InternalMessage = LanguageAll.Language.PaymentStatusIsWrong,
                    MoreInfo = LanguageAll.Language.PaymentStatusIsWrong
                };
            }
            return default;
        }
        private TaskResultDTO Validate(TaskDTO task, bool isNew = true)
        {
            #region Service
            if (task.Service == null)
            {
                return new TaskResultDTO()
                {
                    Data = task,
                    Status = 0,
                    Code = StatusCodes.Status500InternalServerError,
                    UserMessage = LanguageAll.Language.ServiceIsNull,
                    InternalMessage = LanguageAll.Language.ServiceIsNull,
                    MoreInfo = LanguageAll.Language.ServiceIsNull
                };
            }

            var w = _workFlow.WorkFlow.Where(u => u.Id == task.WorkFlowId).FirstOrDefault();

            if (w == null)
            {
                return new TaskResultDTO()
                {
                    Data = task,
                    Status = 0,
                    Code = StatusCodes.Status500InternalServerError,
                    UserMessage = LanguageAll.Language.ServiceIsNull,
                    InternalMessage = LanguageAll.Language.ServiceIsNull,
                    MoreInfo = LanguageAll.Language.ServiceIsNull
                };
            }

            if (String.IsNullOrEmpty(task.Name))
            {
                task.Name = task.Service.Title;
            }

            #region WorkFolow
            if (task.Service.Id != 99)
            {
                task.Department = w.Flow;
                if (task.Customer == null)
                {
                    return new TaskResultDTO()
                    {
                        Data = task,
                        Status = 0,
                        Code = StatusCodes.Status500InternalServerError,
                        UserMessage = LanguageAll.Language.CustomerIsNull,
                        InternalMessage = LanguageAll.Language.CustomerIsNull,
                        MoreInfo = LanguageAll.Language.CustomerIsNull
                    };
                }
            }
            #endregion
            #endregion
            #region Create
            if (task.Owner == null && isNew)
            {
                return new TaskResultDTO()
                {
                    Data = task,
                    Status = 0,
                    Code = StatusCodes.Status500InternalServerError,
                    UserMessage = LanguageAll.Language.OwnerIsNull,
                    InternalMessage = LanguageAll.Language.OwnerIsNull,
                    MoreInfo = LanguageAll.Language.OwnerIsNull
                };
            }

            if (task.CreateDate == null && isNew)
            {
                return new TaskResultDTO()
                {
                    Data = task,
                    Status = 0,
                    Code = StatusCodes.Status500InternalServerError,
                    UserMessage = LanguageAll.Language.CreateDateIsNull,
                    InternalMessage = LanguageAll.Language.CreateDateIsNull,
                    MoreInfo = LanguageAll.Language.CreateDateIsNull
                };
            }
            #endregion
            #region Change
            if (task.ChangedUser == null && !isNew)
            {
                return new TaskResultDTO()
                {
                    Data = task,
                    Status = 0,
                    Code = StatusCodes.Status500InternalServerError,
                    UserMessage = LanguageAll.Language.ChangedUserIsNull,
                    InternalMessage = LanguageAll.Language.ChangedUserIsNull,
                    MoreInfo = LanguageAll.Language.ChangedUserIsNull
                };
            }

            if (task.ModifyDate == null && !isNew)
            {
                return new TaskResultDTO()
                {
                    Data = task,
                    Status = 0,
                    Code = StatusCodes.Status500InternalServerError,
                    UserMessage = LanguageAll.Language.ModifyDateIsNull,
                    InternalMessage = LanguageAll.Language.ModifyDateIsNull,
                    MoreInfo = LanguageAll.Language.ModifyDateIsNull
                };
            }
            #endregion
            return default;
        }
        private TaskResultDTO Validate(TaskDTO task, TaskProcessDTO taskProcess)
        {
            if (taskProcess.Staff == null)
            {
                return new TaskResultDTO()
                {
                    Data = task,
                    Status = 0,
                    Code = StatusCodes.Status500InternalServerError,
                    UserMessage = LanguageAll.Language.StaffIsNull,
                    InternalMessage = LanguageAll.Language.StaffIsNull,
                    MoreInfo = LanguageAll.Language.StaffIsNull
                };
            }

            if (String.IsNullOrEmpty(taskProcess.Content))
            {
                return new TaskResultDTO()
                {
                    Data = task,
                    Status = 0,
                    Code = StatusCodes.Status500InternalServerError,
                    UserMessage = LanguageAll.Language.ContentOfTaskIsNull,
                    InternalMessage = LanguageAll.Language.ContentOfTaskIsNull,
                    MoreInfo = LanguageAll.Language.ContentOfTaskIsNull
                };
            }

            if (taskProcess.CreateDate == null)
            {
                return new TaskResultDTO()
                {
                    Data = task,
                    Status = 0,
                    Code = StatusCodes.Status500InternalServerError,
                    UserMessage = LanguageAll.Language.CreateDateIsNull,
                    InternalMessage = LanguageAll.Language.CreateDateIsNull,
                    MoreInfo = LanguageAll.Language.CreateDateIsNull
                };
            }

            return default;
        }
        private StaffDTO GetStaff(string StaffCode, List<StaffDTO> Staff)
        {
            StaffDTO a = null;
            if (Staff != null) a = Staff.Where(u => u.StaffCode == StaffCode || u.Mobile == StaffCode || u.Mobile2 == StaffCode).FirstOrDefault();
            return a;
        }
        private TaskResultDTO Validate(TaskDTO task, StaffDTO staff, bool isAdd = true)
        {
            StaffDTO a = GetStaff(staff.StaffCode, task.Staff);
            if (a != null && isAdd)
            {
                return new TaskResultDTO()
                {
                    Data = task,
                    Status = 0,
                    Code = StatusCodes.Status500InternalServerError,
                    UserMessage = LanguageAll.Language.StaffIsExists,
                    InternalMessage = LanguageAll.Language.StaffIsExists,
                    MoreInfo = LanguageAll.Language.StaffIsExists
                };
            }
            if (a == null && !isAdd)
            {
                return new TaskResultDTO()
                {
                    Data = task,
                    Status = 0,
                    Code = StatusCodes.Status500InternalServerError,
                    UserMessage = LanguageAll.Language.StaffNotExists,
                    InternalMessage = LanguageAll.Language.StaffNotExists,
                    MoreInfo = LanguageAll.Language.StaffNotExists
                };
            }
            return null;
        }
        private TaskResultDTO Validate(TaskDTO task, CasherDTO staff)
        {
            if (task.Casher != null)
            {
                return new TaskResultDTO()
                {
                    Data = task,
                    Status = 0,
                    Code = StatusCodes.Status500InternalServerError,
                    UserMessage = LanguageAll.Language.CasherIsExists,
                    InternalMessage = LanguageAll.Language.CasherIsExists,
                    MoreInfo = LanguageAll.Language.CasherIsExists
                };
            }
            return null;
        }
        private string GetStepName(TaskDTO task)
        {
            Models.Tasks.Services a = _workFlow.WorkFlow.Where(u => u.Id == task.WorkFlowId).FirstOrDefault();
            if (a == null) return "N/A";
            if (a.Flow == null) return "N/A";
            if (a.Flow.Count() < 1) return "N/A";
            DepartmentDTO b = a.Flow.Where(u => u.DepartmentId == task.CurrentDepartmentId).FirstOrDefault();
            if (b == null) return "N/A";
            return b.TaskName;
        }
        private void AddStaffIfNull(TaskDTO task, StaffInfo _staff)
        {
            StaffDTO staff = new StaffDTO()
            {
                Address = _staff.Address,
                DepartmentCode = _staff.DepartmentCode,
                DepartmentId = _staff.DepartmentId,
                DepartmentName = _staff.DepartmentName,
                Email = _staff.Email,
                Mobile = _staff.Mobile,
                Mobile2 = _staff.Mobile2,
                POSCode = _staff.POSCode,
                POSId = _staff.POSId,
                POSName = _staff.POSName,
                StaffCode = _staff.StaffCode,
                StaffName = _staff.StaffName,
                TaxCode = _staff.TaxCode,
                TypeCode = _staff.TypeCode,
                TypeName = _staff.TypeName
            };
            var a = Validate(task, staff);
            if (a != null) return;
            if (task.Staff == null) task.Staff = new List<StaffDTO>();
            task.Staff.Add(staff);
        }
        private List<FilterDefinition<TaskDTO>> CreateFilterFolow(TaskFilterDTO _filter)
        {
            var filters = new List<FilterDefinition<TaskDTO>>();
            if (!String.IsNullOrEmpty(_filter.Keyword))
            {
                var filter = Builders<TaskDTO>.Filter.Where(u => (u.Name.Contains(_filter.Keyword) || u.Content.Contains(_filter.Keyword)));
                filters.Add(filter);
            }

            if (!String.IsNullOrEmpty(_filter.CustomerCode))
            {
                var filter = Builders<TaskDTO>.Filter.Where(u => u.Customer.CustomerCode == _filter.CustomerCode);
                filters.Add(filter);
            }

            if (_filter.Status.HasValue)
            {
                var filter = Builders<TaskDTO>.Filter.Where(u => u.Status == _filter.Status.Value);
                filters.Add(filter);
            }

            if (_filter.IsExpired.HasValue)
            {
                if (_filter.IsExpired.Value)
                {
                    var filter = Builders<TaskDTO>.Filter.Where(u => u.ToDate <= DateTime.Now);
                    filters.Add(filter);
                }
                else
                {
                    var filter = Builders<TaskDTO>.Filter.Where(u => u.ToDate >= DateTime.Today);
                    filters.Add(filter);
                }
            }

            if (_filter.FromDate.HasValue)
            {
                var filter = Builders<TaskDTO>.Filter.Where(u => u.FromDate >= _filter.FromDate.Value);
                filters.Add(filter);
            }

            if (_filter.ToDate.HasValue)
            {
                var filter = Builders<TaskDTO>.Filter.Where(u => u.ToDate <= _filter.ToDate.Value);
                filters.Add(filter);
            }

            if (!String.IsNullOrEmpty(_filter.IsPIC))
            {
                var arr = _filter.IsPIC.Split(":::", StringSplitOptions.None);
                string PIC = arr[0];
                int DepartmentId = 0;
                int Status = 0;
                if (arr.Length > 1) int.TryParse(arr[1], out DepartmentId);
                if (arr.Length > 2) int.TryParse(arr[2], out Status);

                if (!String.IsNullOrEmpty(PIC))
                {
                    var filter = Builders<TaskDTO>.Filter.ElemMatch(u => u.Staff, u1 => u1.StaffCode == PIC);
                    filters.Add(filter);
                }

                if (DepartmentId > 0)
                {
                    var filter1 = Builders<TaskDTO>.Filter.Where(u => (
                    !u.CurrentDepartmentId.HasValue ||
                    u.CurrentDepartmentId.HasValue && (u.CurrentDepartmentId.Value == 0 || u.CurrentDepartmentId.Value == DepartmentId)
                    ));
                    filters.Add(filter1);
                }

                if (DepartmentId > 0 && (Status == -9 || Status == 2))
                {
                    var filter1 = Builders<TaskDTO>.Filter.ElemMatch(u => u.Department, u1 => (u1.DepartmentId == DepartmentId) && (u1.StatusId.Value == Status));
                    filters.Add(filter1);
                }
            }

            if (!String.IsNullOrEmpty(_filter.IsAssigne))
            {
                var filter = Builders<TaskDTO>.Filter.ElemMatch(u => u.Staff, u1 => u1.StaffCode == _filter.IsAssigne);
                filters.Add(filter);
            }

            if (!String.IsNullOrEmpty(_filter.IsOwner))
            {
                var filter = Builders<TaskDTO>.Filter.Where(u => (u.Owner.StaffCode == _filter.IsOwner));
                filters.Add(filter);
            }

            return filters;
        }

        private List<FilterDefinition<TaskDTO>> CreateFilterNotFolow(TaskFilterDTO _filter)
        {
            var filters = new List<FilterDefinition<TaskDTO>>();
            if (!String.IsNullOrEmpty(_filter.Keyword))
            {
                var filter = Builders<TaskDTO>.Filter.Where(u => (u.Name.Contains(_filter.Keyword) || u.Content.Contains(_filter.Keyword)));
                filters.Add(filter);
            }

            if (_filter.Status.HasValue)
            {
                var filter = Builders<TaskDTO>.Filter.Where(u => u.Status == _filter.Status.Value);
                filters.Add(filter);
            }

            if (_filter.IsExpired.HasValue)
            {
                if (_filter.IsExpired.Value)
                {
                    var filter = Builders<TaskDTO>.Filter.Where(u => u.ToDate <= DateTime.Now);
                    filters.Add(filter);
                }
                else
                {
                    var filter = Builders<TaskDTO>.Filter.Where(u => u.ToDate >= DateTime.Today);
                    filters.Add(filter);
                }
            }

            if (_filter.FromDate.HasValue)
            {
                var filter = Builders<TaskDTO>.Filter.Where(u => u.FromDate >= _filter.FromDate.Value);
                filters.Add(filter);
            }

            if (_filter.ToDate.HasValue)
            {
                var filter = Builders<TaskDTO>.Filter.Where(u => u.ToDate <= _filter.ToDate.Value);
                filters.Add(filter);
            }

            if (!String.IsNullOrEmpty(_filter.IsPIC))
            {
                var arr = _filter.IsPIC.Split(":::", StringSplitOptions.None);
                string PIC = arr[0];
                int DepartmentId = 0;
                int Status = 0;
                if (arr.Length > 1) int.TryParse(arr[1], out DepartmentId);
                if (arr.Length > 2) int.TryParse(arr[2], out Status);

                if (!String.IsNullOrEmpty(PIC))
                {
                    var filter = Builders<TaskDTO>.Filter.ElemMatch(u => u.Staff, u1 => u1.StaffCode == PIC);
                    filters.Add(filter);
                }

                if (Status == -9 || Status == 2)
                {
                    var filter = Builders<TaskDTO>.Filter.Where(u => u.Status == _filter.Status.Value);
                    filters.Add(filter);
                }
            }

            if (!String.IsNullOrEmpty(_filter.IsAssigne))
            {
                var filter = Builders<TaskDTO>.Filter.ElemMatch(u => u.Staff, u1 => u1.StaffCode == _filter.IsAssigne);
                filters.Add(filter);
            }

            if (!String.IsNullOrEmpty(_filter.IsOwner))
            {
                var filter = Builders<TaskDTO>.Filter.Where(u => (u.Owner.StaffCode == _filter.IsOwner));
                filters.Add(filter);
            }

            return filters;
        }
        #endregion

        public async Task<TaskResultDTO> CreateAsync(TaskDTO task)
        {
            TaskResultDTO a1 = Validate((task.PaymentStatus.HasValue ? task.PaymentStatus.Value : 0), true);
            if (a1 != null) return a1;

            TaskResultDTO a2 = Validate((task.Status.HasValue ? task.Status.Value : 0), false);
            if (a2 != null) return a2;

            TaskResultDTO a = Validate(task, true);
            if (a != null)
            {
                return a;
            }
            await _Task.InsertOneAsync(task);
            return new TaskResultDTO()
            {
                Data = task,
                Code = 200,
                InternalMessage = LanguageAll.Language.Success,
                MoreInfo = LanguageAll.Language.Success,
                UserMessage = LanguageAll.Language.Success,
                Status = 1
            };
        }

        public async Task<TaskListResultDTO> GetAsync(TaskFilterDTO _filter)
        {
            var a = new List<TaskDTO>();

            var filters1 = CreateFilterFolow(_filter);
            var complexFilter1 = Builders<TaskDTO>.Filter.And(filters1);
            var a1 = (await _Task.FindAsync(complexFilter1)).ToList();

            var filters2 = CreateFilterNotFolow(_filter);
            var complexFilter2 = Builders<TaskDTO>.Filter.And(filters2);
            var a2 = (await _Task.FindAsync(complexFilter2)).ToList();

            a.AddRange(a1);
            a.AddRange(a2);

            int PageSize = 10;
            if (_filter.PageSize.HasValue) PageSize = _filter.PageSize.Value;
            int Page = 1;
            if (_filter.Page.HasValue) Page = _filter.Page.Value;            

            if (a.Count() > 0)
            {
                var b = a.OrderByDescending(u => u.CreateDate).Skip(PageSize * (Page - 1)).Take(PageSize).ToList();
                return new TaskListResultDTO()
                {
                    Data = new TaskListDTO()
                    {
                        Data = b,
                        Page = Page,
                        PageSize = PageSize,
                        Rowcount = a.Count()
                    },
                    Code = 200,
                    InternalMessage = LanguageAll.Language.Success,
                    MoreInfo = LanguageAll.Language.Success,
                    Status = 1,
                    UserMessage = LanguageAll.Language.Success
                };
            }                
            else
                return new TaskListResultDTO()
                {
                    Data = new TaskListDTO()
                    {
                        Data = null,
                        Page = Page,
                        PageSize = PageSize,
                        Rowcount = 0
                    },
                    Code = 500,
                    InternalMessage = LanguageAll.Language.NotFound,
                    MoreInfo = LanguageAll.Language.NotFound,
                    Status = 0,
                    UserMessage = LanguageAll.Language.NotFound
                };
        }

        public async Task<TaskResultDTO> GetAsync(string id)
        {
            var a = (await _Task.FindAsync<TaskDTO>(task => task.Id == id)).FirstOrDefault();
            if (a != null)
                return new TaskResultDTO()
                {
                    Data = a,
                    Code = 200,
                    InternalMessage = LanguageAll.Language.Success,
                    MoreInfo = LanguageAll.Language.Success,
                    Status = 1,
                    UserMessage = LanguageAll.Language.Success
                };
            else
                return new TaskResultDTO()
                {
                    Data = null,
                    Code = 500,
                    InternalMessage = LanguageAll.Language.NotFound,
                    MoreInfo = LanguageAll.Language.NotFound,
                    Status = 0,
                    UserMessage = LanguageAll.Language.NotFound
                };
        }

        public async Task<TaskResultDTO> RemoveAsync(TaskDTO task)
        {
            return await RemoveAsync(task.Id);
        }

        public async Task<TaskResultDTO> RemoveAsync(string id)
        {
            var a = (await _Task.FindAsync(task => task.Id == id)).ToList();
            if (a.Count < 1)
            {
                return new TaskResultDTO()
                {
                    Data = new TaskDTO()
                    {
                        Id = id,
                        Owner = null,
                        Name = ""
                    },
                    Code = 200,
                    InternalMessage = LanguageAll.Language.NotFound,
                    MoreInfo = LanguageAll.Language.NotFound,
                    Status = 500,
                    UserMessage = LanguageAll.Language.NotFound
                };
            }

            await _Task.DeleteOneAsync(task => task.Id == id);
            return new TaskResultDTO()
            {
                Data = a[0],
                Code = 200,
                InternalMessage = LanguageAll.Language.DeleteSuccess,
                MoreInfo = LanguageAll.Language.DeleteSuccess,
                Status = 200,
                UserMessage = LanguageAll.Language.DeleteSuccess
            };
        }

        public async Task<TaskResultDTO> UpdateAsync(string id, TaskDTO task)
        {
            TaskResultDTO a = Validate(task, false);
            if (a != null) return a;

            TaskResultDTO a1 = Validate((task.PaymentStatus.HasValue ? task.PaymentStatus.Value : 0), true);
            if (a1 != null) return a1;

            await _Task.ReplaceOneAsync(u => u.Id == id, task);
            return new TaskResultDTO()
            {
                Data = task,
                Code = 200,
                InternalMessage = LanguageAll.Language.Success,
                MoreInfo = LanguageAll.Language.Success,
                UserMessage = LanguageAll.Language.Success,
                Status = 1
            };
        }

        public async Task<TaskResultDTO> UpdateAsync(string id, TaskProcessDTO taskProcess, int Status)
        {
            TaskResultDTO a1 = Validate(Status, false);
            if (a1 != null) return a1;

            TaskResultDTO a = await GetAsync(id);
            if (a.Code == 500) return a;
            TaskDTO task = a.Data;

            TaskResultDTO a2 = Validate(task, taskProcess);
            if (a2 != null) return a2;

            taskProcess.TaskName = GetStepName(task);

            AddStaffIfNull(task, taskProcess.Staff);

            if (task.TaskProcess == null) task.TaskProcess = new List<TaskProcessDTO>();
            task.TaskProcess.Add(taskProcess);
            task.Status = Status;
            await _Task.ReplaceOneAsync(u => u.Id == id, task);
            return new TaskResultDTO()
            {
                Data = task,
                Code = 200,
                InternalMessage = LanguageAll.Language.Success,
                MoreInfo = LanguageAll.Language.Success,
                UserMessage = LanguageAll.Language.Success,
                Status = 200
            };
        }

        public async Task<TaskResultDTO> UpdateAsync(TaskDTO task, TaskProcessDTO taskProcess)//, int Status)
        {
            //TaskResultDTO a1 = Validate(Status, false);
            //if (a1 != null) return a1;

            //TaskResultDTO a = await GetAsync(id);
            //if (a.Code == 500) return a;
            //TaskDTO task = a.Data;

            TaskResultDTO a2 = Validate(task, taskProcess);
            if (a2 != null) return a2;

            taskProcess.TaskName = GetStepName(task);

            AddStaffIfNull(task, taskProcess.Staff);

            if (task.TaskProcess == null) task.TaskProcess = new List<TaskProcessDTO>();
            task.TaskProcess.Add(taskProcess);
            //task.Status = Status;

            await _Task.ReplaceOneAsync(y => y.Id == task.Id, task);
            return new TaskResultDTO()
            {
                Data = task,
                Code = 200,
                InternalMessage = LanguageAll.Language.Success,
                MoreInfo = LanguageAll.Language.Success,
                UserMessage = LanguageAll.Language.Success,
                Status = 200
            };
        }

        public async Task<TaskResultDTO> UpdateAsync(string id, StaffDTO staff, bool isAdd = true)
        {
            TaskResultDTO a = await GetAsync(id);
            if (a.Code == 500) return a;
            TaskDTO task = a.Data;

            TaskResultDTO a1 = Validate(task, staff, isAdd);
            if (a1 != null) return a1;

            if (isAdd)
            {
                if (task.Staff == null) task.Staff = new List<StaffDTO>();
                task.Staff.Add(staff);
            }
            else
            {
                var _a = GetStaff(staff.StaffCode, task.Staff);
                task.Staff.Remove(_a);
            }
            await _Task.ReplaceOneAsync(u => u.Id == id, task);
            return new TaskResultDTO()
            {
                Data = task,
                Code = 200,
                InternalMessage = LanguageAll.Language.Success,
                MoreInfo = LanguageAll.Language.Success,
                UserMessage = LanguageAll.Language.Success,
                Status = 200
            };
        }

        public async Task<TaskResultDTO> UpdateAsync(string id, CasherDTO staff)
        {
            TaskResultDTO a = await GetAsync(id);
            if (a.Code == 500) return a;
            TaskDTO task = a.Data;

            TaskResultDTO a1 = Validate(task, staff);
            if (a1 != null) return a1;

            task.Casher = staff;

            await _Task.ReplaceOneAsync(u => u.Id == id, task);
            return new TaskResultDTO()
            {
                Data = task,
                Code = 200,
                InternalMessage = LanguageAll.Language.Success,
                MoreInfo = LanguageAll.Language.Success,
                UserMessage = LanguageAll.Language.Success,
                Status = 200
            };
        }
    }
}
