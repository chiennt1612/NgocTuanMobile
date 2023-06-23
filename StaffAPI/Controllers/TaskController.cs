using EntityFramework.API.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StaffAPI.Models.Tasks;
using StaffAPI.Models.Tasks.DTO;
using StaffAPI.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using Utils;
using Utils.ExceptionHandling;
using Utils.Models;
using Utils.Repository.Interfaces;
using StaffAPI.Models;
using System.Collections;
using StaffAPI.Helper;
using EntityFramework.API.Entities.Ordering;
using System.Net.Mail;

namespace StaffAPI.Controllers
{
    [ApiVersion("1.0")]
    [Route("[controller]")]
    [ApiController]
    [TypeFilter(typeof(ControllerExceptionFilterAttribute))]
    [Produces("application/json", "application/problem+json")]
    [Authorize]
    public class TaskController : ControllerBase
    {
        #region Properties
        private readonly LoginConfiguration _loginConfiguration;
        private readonly UserManager<AppUser> _userManager;
        private readonly IDistributedCache _cache;
        private readonly ILogger<TaskController> _logger;
        private readonly IStringLocalizer<TaskController> _localizer;
        private readonly IAllService _Service;
        private readonly ITaskService _TaskService;
        private readonly IEmailSender _emailSender;
        private readonly IWorkFlowConfig _WorkFlow;
        private readonly IInvoiceRepository _invoice;
        private IConfiguration _configuration;
        public ICompanyConfig companyConfig;
        private List<string> invoiceCacheKeys;
        #endregion

        public TaskController(IConfiguration _configuration, IEmailSender _emailSender, IDistributedCache _cache,
            ILogger<TaskController> _logger, IStringLocalizer<TaskController> _localizer, IAllService _Service,
            UserManager<AppUser> userManager, ITaskService _TaskService, IWorkFlowConfig _WorkFlow, ICompanyConfig companyConfig,
            IInvoiceRepository _invoice, LoginConfiguration _loginConfiguration)
        {
            this._logger = _logger;
            this._Service = _Service;
            this._localizer = _localizer;
            this._cache = _cache;
            this._emailSender = _emailSender;
            this._configuration = _configuration;
            this._TaskService = _TaskService;
            this._WorkFlow = _WorkFlow;
            this._logger.WriteLog(_configuration, "Starting invoice page");
            this.companyConfig = companyConfig;
            this._invoice = _invoice;
            this._loginConfiguration = _loginConfiguration;
            _userManager = userManager;

            var b = _cache.GetAsync<List<string>>("InvoiceCacheKeys").GetAwaiter();
            invoiceCacheKeys = b.GetResult();
            if (invoiceCacheKeys == null)
            {
                invoiceCacheKeys = new List<string>();
            }
        }

        #region Private
        private async Task<StaffInfoResult> GetStaffInfo(ClaimsPrincipal _user)
        {
            var user = await GetCurrentUserAsync(_user);
            StaffInfoResult staff = null;
            for (var i = 0; i < companyConfig.Companys.Count; i++)
            {
                var inv = new StaffCodeInput()
                {
                    CompanyID = companyConfig.Companys[i].Info.CompanyId,
                    StaffCode = user.UserName
                };
                staff = await _Service.invoiceServices.getStaffInfo(inv);
                if (staff.DataStatus == "00") break;
            }
            return staff;
        }
        private async Task<StaffDTO> GetStaff(ClaimsPrincipal _user)
        {
            var staff = await GetStaffInfo(_user);
            if (staff == null) return null;
            if (staff.DataStatus != "00") return null;
            StaffDTO Owner = new StaffDTO()
            {
                Address = staff.ItemsData[0].Address,
                DepartmentCode = staff.ItemsData[0].DepartmentCode,
                DepartmentId = staff.ItemsData[0].DepartmentId,
                DepartmentName = staff.ItemsData[0].DepartmentName,
                Email = staff.ItemsData[0].Email,
                StaffCode = staff.ItemsData[0].StaffCode,
                StaffName = staff.ItemsData[0].StaffName,
                TypeName = staff.ItemsData[0].TypeName,
                TypeCode = staff.ItemsData[0].TypeCode,
                Mobile2 = staff.ItemsData[0].Mobile2,
                Mobile = staff.ItemsData[0].Mobile,
                TaxCode = staff.ItemsData[0].TaxCode,
                POSName = staff.ItemsData[0].POSName,
                POSCode = staff.ItemsData[0].POSCode,
                POSId = staff.ItemsData[0].POSId
            };
            return Owner;
        }
        private async Task<CasherDTO> GetCasher(ClaimsPrincipal _user)
        {
            var staff = await GetStaffInfo(_user);
            if (staff == null) return null;
            if (staff.DataStatus != "00") return null;
            CasherDTO Owner = new CasherDTO()
            {
                Address = staff.ItemsData[0].Address,
                DepartmentCode = staff.ItemsData[0].DepartmentCode,
                DepartmentId = staff.ItemsData[0].DepartmentId,
                DepartmentName = staff.ItemsData[0].DepartmentName,
                Email = staff.ItemsData[0].Email,
                StaffCode = staff.ItemsData[0].StaffCode,
                StaffName = staff.ItemsData[0].StaffName,
                TypeName = staff.ItemsData[0].TypeName,
                TypeCode = staff.ItemsData[0].TypeCode,
                Mobile2 = staff.ItemsData[0].Mobile2,
                Mobile = staff.ItemsData[0].Mobile,
                TaxCode = staff.ItemsData[0].TaxCode,
                POSName = staff.ItemsData[0].POSName,
                POSCode = staff.ItemsData[0].POSCode,
                POSId = staff.ItemsData[0].POSId
            };
            return Owner;
        }
        private Task<AppUser> GetCurrentUserAsync(ClaimsPrincipal user)
        {
            return _userManager.GetUserAsync(user);
        }
        private Models.Tasks.Services GetWorkFlow(long ServiceId)
        {
            return _WorkFlow.WorkFlow.Where(u => u.Id == ServiceId).FirstOrDefault();
        }
        private ResponseOK Validate(Models.Tasks.Services _workFlow)
        {
            if (_workFlow == null)
            {
                return new ResponseOK()
                {
                    Code = 400,
                    InternalMessage = LanguageAll.Language.ServiceNotFound,
                    MoreInfo = LanguageAll.Language.ServiceNotFound,
                    Status = 0,
                    UserMessage = LanguageAll.Language.ServiceNotFound,
                    data = null
                };
            }
            return null;
        }
        private async Task<ResponseOK> Validate(Models.Tasks.Services _workFlow, CustomerDTO Customer)
        {
            if (_workFlow.Id != 99)
            {
                if (Customer == null)
                {
                    return new ResponseOK()
                    {
                        Code = 400,
                        InternalMessage = LanguageAll.Language.CustomerIsNull,
                        MoreInfo = LanguageAll.Language.CustomerIsNull,
                        Status = 0,
                        UserMessage = LanguageAll.Language.CustomerIsNull,
                        data = null
                    };
                }

                long workFlowId = 0;
                if (_workFlow.Id.HasValue) workFlowId = _workFlow.Id.Value;
                //The register work have not the customer infomation. This is input by manual
                if (workFlowId == 1 || workFlowId == 2)
                {
                    Customer.CustomerType = _workFlow.CustomerType.HasValue ? _workFlow.CustomerType.Value.ToString() : "90";
                    Customer.CustomerCode = "[RegisterNew]";
                    Customer.WaterIndexCode = "[RegisterNew]";
                }

                if (_workFlow.CustomerType.HasValue)
                {
                    string CustomerType = _workFlow.CustomerType.Value.ToString();
                    if (CustomerType != Customer.CustomerType.Substring(0, CustomerType.Length))
                    {
                        return new ResponseOK()
                        {
                            Code = 400,
                            InternalMessage = LanguageAll.Language.CustomerIsWrong,
                            MoreInfo = LanguageAll.Language.CustomerIsWrong,
                            Status = 0,
                            UserMessage = LanguageAll.Language.CustomerIsWrong,
                            data = null
                        };
                    }                    
                }

                // Re-Validate customer information
                if (workFlowId > 2)
                {
                    List<ContractList> contractList = await GetCustomer(Customer.CustomerCode);
                    if (contractList.Count < 1)
                    {
                        return new ResponseOK()
                        {
                            Code = 400,
                            InternalMessage = LanguageAll.Language.CustomerIsWrong,
                            MoreInfo = LanguageAll.Language.CustomerIsWrong,
                            Status = 0,
                            UserMessage = LanguageAll.Language.CustomerIsWrong,
                            data = null
                        };
                    }
                }
            }
            return null;
        }
        private ResponseOK Validate(Models.Tasks.Services _workFlow, StaffDTO staff)
        {
            if (staff == null)
            {
                return new ResponseOK()
                {
                    Code = 400,
                    InternalMessage = LanguageAll.Language.NoPermission,
                    MoreInfo = LanguageAll.Language.NoPermission,
                    Status = 0,
                    UserMessage = LanguageAll.Language.NoPermission,
                    data = null
                };
            }
            if (_workFlow.Id.Value != 99)
            {
                if (!_WorkFlow.DepartmentAlow.Contains(staff.DepartmentCode) && 
                    !(_loginConfiguration.MobileTest.Contains(staff.Mobile) || _loginConfiguration.MobileTest.Contains(staff.Mobile2)))
                {
                    return new ResponseOK()
                    {
                        Code = 400,
                        InternalMessage = LanguageAll.Language.NoPermission,
                        MoreInfo = LanguageAll.Language.NoPermission,
                        Status = 0,
                        UserMessage = LanguageAll.Language.NoPermission,
                        data = null
                    };
                }
            }
            return null;
        }
        private ResponseOK Validate(TaskCreateModels taskInput)
        {
            if (string.IsNullOrEmpty(taskInput.Content))
            {
                return new ResponseOK()
                {
                    Code = 500,
                    InternalMessage = LanguageAll.Language.ContentIsNull,
                    MoreInfo = LanguageAll.Language.ContentIsNull,
                    Status = 0,
                    UserMessage = LanguageAll.Language.ContentIsNull,
                    data = null
                };
            }

            if (taskInput.FromDate == null)
            {
                return new ResponseOK()
                {
                    Code = 500,
                    InternalMessage = LanguageAll.Language.DateFromIsWrong,
                    MoreInfo = LanguageAll.Language.DateFromIsWrong,
                    Status = 0,
                    UserMessage = LanguageAll.Language.DateFromIsWrong,
                    data = null
                };
            }

            if (taskInput.FromDate.Value < DateTime.Today)
            {
                return new ResponseOK()
                {
                    Code = 500,
                    InternalMessage = LanguageAll.Language.DateFromIsWrong,
                    MoreInfo = LanguageAll.Language.DateFromIsWrong,
                    Status = 0,
                    UserMessage = LanguageAll.Language.DateFromIsWrong,
                    data = null
                };
            }

            if (taskInput.ToDate == null)
            {
                return new ResponseOK()
                {
                    Code = 500,
                    InternalMessage = LanguageAll.Language.DateToIsWrong,
                    MoreInfo = LanguageAll.Language.DateToIsWrong,
                    Status = 0,
                    UserMessage = LanguageAll.Language.DateToIsWrong,
                    data = null
                };
            }

            if (taskInput.ToDate.Value < taskInput.FromDate.Value)
            {
                return new ResponseOK()
                {
                    Code = 500,
                    InternalMessage = LanguageAll.Language.DateToIsWrong,
                    MoreInfo = LanguageAll.Language.DateToIsWrong,
                    Status = 0,
                    UserMessage = LanguageAll.Language.DateToIsWrong,
                    data = null
                };
            }
            return null;
        }
        private ResponseOK Validate(TaskUpdateModel taskInput)
        {
            if (string.IsNullOrEmpty(taskInput.Content))
            {
                return new ResponseOK()
                {
                    Code = 500,
                    InternalMessage = LanguageAll.Language.ContentIsNull,
                    MoreInfo = LanguageAll.Language.ContentIsNull,
                    Status = 0,
                    UserMessage = LanguageAll.Language.ContentIsNull,
                    data = null
                };
            }

            if (taskInput.FromDate == null)
            {
                return new ResponseOK()
                {
                    Code = 500,
                    InternalMessage = LanguageAll.Language.DateFromIsWrong,
                    MoreInfo = LanguageAll.Language.DateFromIsWrong,
                    Status = 0,
                    UserMessage = LanguageAll.Language.DateFromIsWrong,
                    data = null
                };
            }

            if (taskInput.FromDate.Value < DateTime.Today)
            {
                return new ResponseOK()
                {
                    Code = 500,
                    InternalMessage = LanguageAll.Language.DateFromIsWrong,
                    MoreInfo = LanguageAll.Language.DateFromIsWrong,
                    Status = 0,
                    UserMessage = LanguageAll.Language.DateFromIsWrong,
                    data = null
                };
            }

            if (taskInput.ToDate == null)
            {
                return new ResponseOK()
                {
                    Code = 500,
                    InternalMessage = LanguageAll.Language.DateToIsWrong,
                    MoreInfo = LanguageAll.Language.DateToIsWrong,
                    Status = 0,
                    UserMessage = LanguageAll.Language.DateToIsWrong,
                    data = null
                };
            }

            if (taskInput.ToDate.Value < taskInput.FromDate.Value)
            {
                return new ResponseOK()
                {
                    Code = 500,
                    InternalMessage = LanguageAll.Language.DateToIsWrong,
                    MoreInfo = LanguageAll.Language.DateToIsWrong,
                    Status = 0,
                    UserMessage = LanguageAll.Language.DateToIsWrong,
                    data = null
                };
            }
            return null;
        }
        private async Task<List<string>> Attachments(List<StaffAPI.Models.Tasks.Attachment>? attachments, string subFolder, string folder)
        {
            if (attachments == null) return default;
            if (attachments.Count() == 0) return default;
            List<string> a = new List<string>();
            foreach (var item in attachments)
            {
                int i = item.FileName.LastIndexOf(".");
                string fileName = item.FileName;
                string fileExtension = ".jpg";
                if (i > 1 && fileName.Length > 2)
                {
                    fileExtension = item.FileName.Substring(i);
                    fileName = item.FileName.Substring(0, i);
                }
                a.Add(await Tools.Upload(item.FileData, fileName, subFolder, folder, fileExtension));
            }
            return a;
        }
        #endregion

        #region Create/Update Task
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Create([FromBody] TaskCreateModels taskInput)
        {
            var _startTime = _logger.DebugStart(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}");

            #region Create new Task Object
            TaskDTO task = new TaskDTO()
            {
                WorkFlowId = taskInput.ServiceId,
                Name = taskInput.Name,
                // 0. Create new
                // 1. In-Progress
                // 2. Done
                // 3. Re-open
                // -9. Cancle
                Status = 0,
                // 0. Un-payment
                // 1. Payment
                CurrentDepartmentId = 0,
                PaymentStatus = 0,
                Casher = null,
                Attachment = null,
                Service = null,
                Staff = null,
                Customer = null,
                Department = null,
                TaskProcess = null,
                Content = taskInput.Content,
                Owner = null,
                FromDate = taskInput.FromDate,
                ToDate = taskInput.ToDate,
                CreateDate = DateTime.Now,
                ExpiredDate = null,
                ChangedUser = null,
                ModifyDate = null
            };
            #endregion

            #region Services
            var _workFlow = GetWorkFlow(taskInput.ServiceId);
            var r1 = Validate(_workFlow);
            if (r1 != null) return StatusCode(StatusCodes.Status200OK, r1);

            if (_workFlow.Flow != null)
            {
                task.Department = new List<DepartmentDTO>();
                for (var j = 0; j < _workFlow.Flow.Count(); j++)
                {
                    var _a = _workFlow.Flow[j];
                    if (j == 0) _a.StatusId = 2;
                    if (j == 1) {
                        _a.StatusId = 1;
                        task.CurrentDepartmentId = _workFlow.Flow[j].DepartmentId;
                    }                    
                    task.Department.Add(_a);
                }
            }

            var _service = await _Service.serviceServices.GetByIdAsync(_workFlow.ServiceId.Value);
            ServiceDTO Service = new ServiceDTO();
            if (_service != null)
            {
                Service.Id = _service.Id;
                Service.Description = _service.Description;
                Service.Img = _service.Img;
                Service.PriceCompany = _service.Price1;
                Service.PricePerson = _service.Price;
                Service.PriceText = _service.PriceText;
                Service.Summary = _service.Summary;
                Service.Title = _service.Title;
                Service.Url = $"https://nuocngoctuan.com/Service/Details/{_service.Id}";
            }
            else
            {
                Service.Id = _workFlow.ServiceId.Value;
                Service.Description = _workFlow.ServiceName;
                Service.Img = _workFlow.Image;
                Service.PriceCompany = 0;
                Service.PricePerson = 0;
                Service.PriceText = "";
                Service.Summary = _workFlow.ServiceName;
                Service.Title = _workFlow.ServiceName;
                Service.Url = $"";
            }

            task.Service = Service;
            #endregion

            #region Customer  
            r1 = await Validate(_workFlow, taskInput.Customer);
            if (r1 != null) return StatusCode(StatusCodes.Status200OK, r1);

            if (taskInput.Customer != null) task.Customer = taskInput.Customer;
            #endregion

            #region Owner
            var Owner = await GetStaff(HttpContext.User);

            r1 = Validate(_workFlow, Owner);
            if (r1 != null) return StatusCode(StatusCodes.Status200OK, r1);
            task.Owner = Owner;
            #endregion

            #region Other
            r1 = Validate(taskInput);
            if (r1 != null) return StatusCode(StatusCodes.Status200OK, r1);
            #endregion

            #region Attachment
            List<string> Attachment = null;
            if (taskInput.Attachment != null)
            {
                if (taskInput.Attachment.Count > 0)
                {
                    Attachment = new List<string>();
                    for (var i = 0; i < taskInput.Attachment.Count; i++)
                    {
                        Attachment.Add(await Tools.Upload(taskInput.Attachment[i], Guid.NewGuid().ToString().Replace("-", ""), "Task", companyConfig.AvatarFolder));
                    }
                }
            }

            if (Attachment != null) task.Attachment = Attachment;
            #endregion

            TaskResultDTO a = await _TaskService.CreateTask(task);
            _logger.WriteLog(_configuration, $"List {JsonConvert.SerializeObject(taskInput)}: {JsonConvert.SerializeObject(a)}", "List");
            _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
            return Ok(a);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Update([FromBody] TaskUpdateModel taskInput)
        {
            var _startTime = _logger.DebugStart(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}");

            var g = await _TaskService.GetTaskById(taskInput.Id);
            if (g.Code.Value != 200)
                return StatusCode(StatusCodes.Status200OK, new ResponseOK()
                {
                    Code = 400,
                    InternalMessage = LanguageAll.Language.TaskNotFound,
                    MoreInfo = LanguageAll.Language.TaskNotFound,
                    Status = 0,
                    UserMessage = LanguageAll.Language.TaskNotFound,
                    data = null
                });
            TaskDTO task = g.Data;

            #region Services
            var _workFlow = GetWorkFlow(taskInput.ServiceId);
            var r1 = Validate(_workFlow);
            if (r1 != null) return StatusCode(StatusCodes.Status200OK, r1);

            if (_workFlow.Flow != null) task.Department = _workFlow.Flow;

            var _service = await _Service.serviceServices.GetByIdAsync(_workFlow.ServiceId.Value);
            ServiceDTO Service = new ServiceDTO();
            if (_service != null)
            {
                Service.Id = _service.Id;
                Service.Description = _service.Description;
                Service.Img = _service.Img;
                Service.PriceCompany = _service.Price1;
                Service.PricePerson = _service.Price;
                Service.PriceText = _service.PriceText;
                Service.Summary = _service.Summary;
                Service.Title = _service.Title;
                Service.Url = $"https://nuocngoctuan.com/Service/Details/{_service.Id}";
            }
            else
            {
                Service.Id = _workFlow.ServiceId.Value;
                Service.Description = _workFlow.ServiceName;
                Service.Img = _workFlow.Image;
                Service.PriceCompany = 0;
                Service.PricePerson = 0;
                Service.PriceText = "";
                Service.Summary = _workFlow.ServiceName;
                Service.Title = _workFlow.ServiceName;
                Service.Url = $"";
            }

            task.Service = Service;
            #endregion

            #region Customer  
            r1 = await Validate(_workFlow, taskInput.Customer);
            if (r1 != null) return StatusCode(StatusCodes.Status200OK, r1);

            if (taskInput.Customer != null) task.Customer = taskInput.Customer;
            #endregion

            #region Owner
            var Owner = await GetStaff(HttpContext.User);

            r1 = Validate(_workFlow, Owner);
            if (r1 != null) return StatusCode(StatusCodes.Status200OK, r1);
            task.Owner = Owner;
            #endregion

            #region Other
            r1 = Validate(taskInput);
            if (r1 != null) return StatusCode(StatusCodes.Status200OK, r1);
            #endregion

            #region Attachment
            List<string> Attachment = null;
            if (taskInput.Attachment != null)
            {
                if (taskInput.Attachment.Count > 0)
                {
                    Attachment = new List<string>();
                    for (var i = 0; i < taskInput.Attachment.Count; i++)
                    {
                        Attachment.Add(await Tools.Upload(taskInput.Attachment[i], Guid.NewGuid().ToString().Replace("-", ""), "Task", companyConfig.AvatarFolder));
                    }
                }
            }

            if (Attachment != null) task.Attachment = Attachment;
            #endregion

            TaskResultDTO a = await _TaskService.UpdateTask(task);
            _logger.WriteLog(_configuration, $"List {JsonConvert.SerializeObject(taskInput)}: {JsonConvert.SerializeObject(a)}", "List");
            _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
            return Ok(a);
        }
        #endregion

        #region Finding the task
        // The list task of mine
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> TaskOfMine([FromBody] TaskFilterModels model)
        {
            var _startTime = _logger.DebugStart(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}");
            _logger.LogInformation($"Register. ModelState: {ModelState.IsValid}\nmodel: {JsonConvert.SerializeObject(model)}");
            if (ModelState.IsValid)
            {
                var user = await GetCurrentUserAsync(HttpContext.User);
                TaskFilterDTO taskFilter = new TaskFilterDTO()
                {
                    CustomerCode = model.CustomerCode,
                    FromDate = model.FromDate,
                    IsExpired = model.IsExpired,
                    IsOwner = user.UserName,
                    IsPIC = null,
                    Keyword = model.Keyword,
                    Page = model.Page,
                    PageSize = model.PageSize,
                    Status = model.Status,
                    ToDate = model.ToDate
                };
                var a = await _TaskService.GetTaskList(taskFilter);
                return Ok(a);
            }
            _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
            return StatusCode(StatusCodes.Status200OK,
                new ResponseBase(LanguageAll.Language.NotFound, LanguageAll.Language.NotFound, LanguageAll.Language.NotFound));
        }

        // The list task assign to me
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> TaskAssignedMe([FromBody] TaskFilterModels model)
        {
            var _startTime = _logger.DebugStart(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}");
            _logger.LogInformation($"Register. ModelState: {ModelState.IsValid}\nmodel: {JsonConvert.SerializeObject(model)}");
            if (ModelState.IsValid)
            {
                var user = await GetCurrentUserAsync(HttpContext.User);
                TaskFilterDTO taskFilter = new TaskFilterDTO()
                {
                    CustomerCode = model.CustomerCode,
                    FromDate = model.FromDate,
                    IsExpired = model.IsExpired,
                    IsOwner = null,
                    IsPIC = null,
                    IsAssigne = user.UserName,
                    Keyword = model.Keyword,
                    Page = model.Page,
                    PageSize = model.PageSize,
                    Status = model.Status,
                    ToDate = model.ToDate
                };
                var a = await _TaskService.GetTaskList(taskFilter);
                return Ok(a);
            }
            _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
            return StatusCode(StatusCodes.Status200OK,
                new ResponseBase(LanguageAll.Language.NotFound, LanguageAll.Language.NotFound, LanguageAll.Language.NotFound));
        }

        // The list task need to work
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> TaskNeedWork([FromBody] TaskFilterModels model)
        {
            var _startTime = _logger.DebugStart(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}");
            _logger.LogInformation($"Register. ModelState: {ModelState.IsValid}\nmodel: {JsonConvert.SerializeObject(model)}");
            if (ModelState.IsValid)
            {
                var user = await GetCurrentUserAsync(HttpContext.User);
                TaskFilterDTO taskFilter = new TaskFilterDTO()
                {
                    CustomerCode = model.CustomerCode,
                    FromDate = model.FromDate,
                    IsExpired = model.IsExpired,
                    IsOwner = null,
                    IsPIC = ":::" + User.Claims.GetClaimValue("DepartmentId") + ":::",
                    IsAssigne = null,
                    Keyword = model.Keyword,
                    Page = model.Page,
                    PageSize = model.PageSize,
                    Status = model.Status,
                    ToDate = model.ToDate
                };
                var a = await _TaskService.GetTaskList(taskFilter);
                return Ok(a);
            }
            _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
            return StatusCode(StatusCodes.Status200OK,
                new ResponseBase(LanguageAll.Language.NotFound, LanguageAll.Language.NotFound, LanguageAll.Language.NotFound));
        }
        // The list task processing
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> TaskProcessing([FromBody] TaskFilterModels model)
        {
            var _startTime = _logger.DebugStart(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}");
            _logger.LogInformation($"Register. ModelState: {ModelState.IsValid}\nmodel: {JsonConvert.SerializeObject(model)}");
            if (ModelState.IsValid)
            {
                var user = await GetCurrentUserAsync(HttpContext.User);
                TaskFilterDTO taskFilter = new TaskFilterDTO()
                {
                    CustomerCode = model.CustomerCode,
                    FromDate = model.FromDate,
                    IsExpired = model.IsExpired,
                    IsOwner = null,
                    IsPIC = user.UserName + ":::" + User.Claims.GetClaimValue("DepartmentId") + ":::",
                    IsAssigne = null,
                    Keyword = model.Keyword,
                    Page = model.Page,
                    PageSize = model.PageSize,
                    Status = model.Status,
                    ToDate = model.ToDate
                };
                var a = await _TaskService.GetTaskList(taskFilter);
                return Ok(a);
            }
            _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
            return StatusCode(StatusCodes.Status200OK,
                new ResponseBase(LanguageAll.Language.NotFound, LanguageAll.Language.NotFound, LanguageAll.Language.NotFound));
        }
        // The list task processed
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> TaskProcessed([FromBody] TaskFilterModels model)
        {
            var _startTime = _logger.DebugStart(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}");
            _logger.LogInformation($"Register. ModelState: {ModelState.IsValid}\nmodel: {JsonConvert.SerializeObject(model)}");
            if (ModelState.IsValid)
            {
                var user = await GetCurrentUserAsync(HttpContext.User);
                TaskFilterDTO taskFilter = new TaskFilterDTO()
                {
                    CustomerCode = model.CustomerCode,
                    FromDate = model.FromDate,
                    IsExpired = model.IsExpired,
                    IsOwner = null,
                    IsPIC = user.UserName + ":::" + User.Claims.GetClaimValue("DepartmentId") + ":::2",
                    IsAssigne = null,
                    Keyword = model.Keyword,
                    Page = model.Page,
                    PageSize = model.PageSize,
                    Status = model.Status,
                    ToDate = model.ToDate
                };
                var a = await _TaskService.GetTaskList(taskFilter);
                return Ok(a);
            }
            _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
            return StatusCode(StatusCodes.Status200OK,
                new ResponseBase(LanguageAll.Language.NotFound, LanguageAll.Language.NotFound, LanguageAll.Language.NotFound));
        }
        #endregion

        #region Processing task
        // Assign staff
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> TaskAssign([FromBody] TaskAssignModels model)
        {
            var _startTime = _logger.DebugStart(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}");
            _logger.LogInformation($"Register. ModelState: {ModelState.IsValid}\nmodel: {JsonConvert.SerializeObject(model)}");
            if (ModelState.IsValid)
            {
                var a = await _TaskService.AssignStaff(model.TaskId, model.Staff);
                return Ok(a);
            }
            _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
            return StatusCode(StatusCodes.Status200OK,
                new ResponseBase(LanguageAll.Language.NotFound, LanguageAll.Language.NotFound, LanguageAll.Language.NotFound));
        }

        // Unassign staff
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> TaskUnAssign([FromBody] TaskUnAssignModels model)
        {
            var _startTime = _logger.DebugStart(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}");
            _logger.LogInformation($"Register. ModelState: {ModelState.IsValid}\nmodel: {JsonConvert.SerializeObject(model)}");
            if (ModelState.IsValid)
            {
                var _Staff = new StaffDTO() { StaffCode = model.StaffCode };
                var a = await _TaskService.AssignStaff(model.TaskId, _Staff, false);
                return Ok(a);
            }
            _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
            return StatusCode(StatusCodes.Status200OK,
                new ResponseBase(LanguageAll.Language.NotFound, LanguageAll.Language.NotFound, LanguageAll.Language.NotFound));
        }

        // Assign Casher
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> TaskCasher([FromBody] TaskCasherModels model)
        {
            var _startTime = _logger.DebugStart(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}");
            _logger.LogInformation($"Register. ModelState: {ModelState.IsValid}\nmodel: {JsonConvert.SerializeObject(model)}");
            if (ModelState.IsValid)
            {
                var a = await _TaskService.AssignCasher(model.TaskId, model.Casher);
                return Ok(a);
            }
            _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
            return StatusCode(StatusCodes.Status200OK,
                new ResponseBase(LanguageAll.Language.NotFound, LanguageAll.Language.NotFound, LanguageAll.Language.NotFound));
        }

        // Task process
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> TaskProcess([FromBody] TaskProcessModels model)
        {
            var _startTime = _logger.DebugStart(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}");
            _logger.LogInformation($"Register. ModelState: {ModelState.IsValid}\nmodel: {JsonConvert.SerializeObject(model)}");
            if (ModelState.IsValid)
            {
                TaskProcessDTO taskProcess = new TaskProcessDTO()
                {
                    Id = Guid.NewGuid().ToString(),
                    CreateDate = DateTime.Now,
                    Content = model.Content,
                    Staff = await GetStaff(HttpContext.User),
                    Attachments = await Attachments(model.Attachments, "TaskProcess", companyConfig.AvatarFolder)
                };

                var a = await _TaskService.TaskProcess(model.TaskId, taskProcess, model.Status);
                return Ok(a);
            }
            _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
            return StatusCode(StatusCodes.Status200OK,
                new ResponseBase(LanguageAll.Language.Fail, LanguageAll.Language.Fail, LanguageAll.Language.Fail));
        }

        // Task process department
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> TaskProcessWorkflow([FromBody] TaskProcessDepartmentModels model)
        {
            var _startTime = _logger.DebugStart(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}");
            _logger.LogInformation($"Register. ModelState: {ModelState.IsValid}\nmodel: {JsonConvert.SerializeObject(model)}");
            if (ModelState.IsValid)
            {
                TaskProcessDTO taskProcess = new TaskProcessDTO()
                {
                    Id = Guid.NewGuid().ToString(),
                    CreateDate = DateTime.Now,
                    Content = model.Content,
                    Staff = await GetStaff(HttpContext.User),
                    Attachments = await Attachments(model.Attachments, "TaskProcess", companyConfig.AvatarFolder)
                };
                TaskResultDTO a = await _TaskService.TaskProcess(model.TaskId, taskProcess, model.NextDepartment.Value, _WorkFlow, model.Status);

                return Ok(a);
            }
            _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
            return StatusCode(StatusCodes.Status200OK,
                new ResponseBase(LanguageAll.Language.Fail, LanguageAll.Language.Fail, LanguageAll.Language.Fail));
        }
        #endregion

        #region Other
        // Finding the customer
        #region private
        private async Task<List<ContractList>> GetCustomer(string CustomerCode)
        {
            List<ContractList> ContractList = new List<ContractList>();
            for (var i = 0; i < companyConfig.Companys.Count; i++)
            {
                var inv = new ContractInput()
                {
                    CompanyID = companyConfig.Companys[i].Info.CompanyId,
                    Mobile = CustomerCode
                };
                var cust = await _TaskService.GetCustomer(inv);
                if (cust.DataStatus == "00")
                {
                    ContractList.AddRange(cust.ItemsData.ContractList);
                }
            }
            return ContractList;
        }
        #endregion

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> GetCustomer([FromBody] CustomerModel model)
        {
            var _startTime = _logger.DebugStart(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}");
            _logger.LogInformation($"Register. ModelState: {ModelState.IsValid}\nmodel: {JsonConvert.SerializeObject(model)}");
            if (ModelState.IsValid)
            {
                List<ContractList> ContractList = await GetCustomer(model.CustomerCode);
                if (ContractList.Count() > 0)
                    return Ok(new ResponseOK()
                    {
                        Code = 200,
                        InternalMessage = LanguageAll.Language.Success,
                        MoreInfo = LanguageAll.Language.Success,
                        Status = 1,
                        UserMessage = LanguageAll.Language.Success,
                        data = ContractList
                    });
                else
                    return Ok(new ResponseOK()
                    {
                        Code = 400,
                        InternalMessage = LanguageAll.Language.NotFound,
                        MoreInfo = LanguageAll.Language.NotFound,
                        Status = 0,
                        UserMessage = LanguageAll.Language.NotFound,
                        data = null
                    });
            }
            _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
            return StatusCode(StatusCodes.Status200OK,
                new ResponseBase(LanguageAll.Language.NotFound, LanguageAll.Language.NotFound, LanguageAll.Language.NotFound));
        }

        // Finding the staff
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> GetStaff([FromBody] StaffModels model)
        {
            var _startTime = _logger.DebugStart(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}");
            _logger.LogInformation($"Register. ModelState: {ModelState.IsValid}\nmodel: {JsonConvert.SerializeObject(model)}");
            if (ModelState.IsValid)
            {
                List<StaffInfo> ContractList = new List<StaffInfo>();
                for (var i = 0; i < companyConfig.Companys.Count; i++)
                {
                    var inv = new StaffCodeInput()
                    {
                        CompanyID = companyConfig.Companys[i].Info.CompanyId,
                        StaffCode = model.StaffCode
                    };
                    var cust = await _TaskService.GetStaff(inv);
                    if (cust.DataStatus == "00")
                    {
                        ContractList.AddRange(cust.ItemsData);
                    }
                }
                if (ContractList.Count() > 0)
                    return Ok(new ResponseOK()
                    {
                        Code = 200,
                        InternalMessage = LanguageAll.Language.Success,
                        MoreInfo = LanguageAll.Language.Success,
                        Status = 1,
                        UserMessage = LanguageAll.Language.Success,
                        data = ContractList
                    });
                else
                    return Ok(new ResponseOK()
                    {
                        Code = 400,
                        InternalMessage = LanguageAll.Language.NotFound,
                        MoreInfo = LanguageAll.Language.NotFound,
                        Status = 0,
                        UserMessage = LanguageAll.Language.NotFound,
                        data = null
                    });
            }
            _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
            return StatusCode(StatusCodes.Status200OK,
                new ResponseBase(LanguageAll.Language.NotFound, LanguageAll.Language.NotFound, LanguageAll.Language.NotFound));
        }

        // Finding the Task type
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetTaskType()
        {
            var _startTime = _logger.DebugStart(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}");
            List<TaskType> a = new List<TaskType>();
            for (int i = 0; i < _WorkFlow.WorkFlow.Count(); i++)
            {
                a.Add(new TaskType()
                {
                    TaskTypeId = _WorkFlow.WorkFlow[i].Id,
                    TaskTypeName = _WorkFlow.WorkFlow[i].ServiceName
                });
            }
            _logger.DebugEnd(_configuration, $"Class {this.GetType().Name}/ Function {MethodBase.GetCurrentMethod().ReflectedType.Name}", _startTime);
            return Ok(new ResponseOK()
            {
                Code = 200,
                InternalMessage = LanguageAll.Language.Success,
                MoreInfo = LanguageAll.Language.Success,
                Status = 1,
                UserMessage = LanguageAll.Language.Success,
                data = a
            });
        }
        #endregion

    }
}
