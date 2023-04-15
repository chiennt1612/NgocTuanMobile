using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using StaffAPI.Helper;
using StaffAPI.Models.Tasks;
using StaffAPI.Services.Interfaces;
using System.Threading.Tasks;

namespace StaffAPI.Services
{
    public class TaskService : ITaskService
    {
        private readonly IMongoCollection<TaskDTO> _Task;
        private readonly ILogger<TaskService> _Log;
        public TaskService(IDBSetting settings, ILogger<TaskService> Log)
        {
            _Log = Log;
            _Log.LogInformation("Start object Task!");
            var client = new MongoClient(settings.CHAT_MONGODB_URL);
            var database = client.GetDatabase(settings.DatabaseName);

            _Task = database.GetCollection<TaskDTO>("Tasks");
        }

        public async Task<TaskResultDTO> CreateAsync(TaskDTO task)
        {
            throw new System.NotImplementedException();
        }

        public async Task<TaskListResultDTO> GetAsync(TaskFilterDTO _filter)
        {
            throw new System.NotImplementedException();
        }

        public async Task<TaskResultDTO> GetAsync(string id)
        {
            var a = (await _Task.FindAsync<TaskDTO>(task => task.Id == id)).FirstOrDefault();
            if (a != null)
                return new TaskResultDTO()
                {
                    data = a,
                    Code = 200,
                    InternalMessage = LanguageAll.Language.Success,
                    MoreInfo = LanguageAll.Language.Success,
                    Status = 200,
                    UserMessage = LanguageAll.Language.Success
                };
            else
                return new TaskResultDTO()
                {
                    data = null,
                    Code = 200,
                    InternalMessage = LanguageAll.Language.NotFound,
                    MoreInfo = LanguageAll.Language.NotFound,
                    Status = 500,
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
                    data = new TaskDTO()
                    {
                        Id = id,
                        Owner = null,
                        Name = ""
                    },
                    Code = 200,
                    InternalMessage =LanguageAll.Language.NotFound,
                    MoreInfo = LanguageAll.Language.NotFound,
                    Status = 500,
                    UserMessage = LanguageAll.Language.NotFound
                };
            }

            await _Task.DeleteOneAsync(task => task.Id == id);
            return new TaskResultDTO()
            {
                data = a[0],
                Code = 200,
                InternalMessage = LanguageAll.Language.DeleteSuccess,
                MoreInfo = LanguageAll.Language.DeleteSuccess,
                Status = 200,
                UserMessage = LanguageAll.Language.DeleteSuccess
            };
        }

        public async Task<TaskResultDTO> UpdateAsync(string id, TaskDTO task)
        {
            throw new System.NotImplementedException();
        }

        public async Task<TaskResultDTO> UpdateAsync(string id, TaskProcessDTO taskProcess)
        {
            throw new System.NotImplementedException();
        }

        public async Task<TaskResultDTO> UpdateAsync(string id, DepartmentDTO department)
        {
            throw new System.NotImplementedException();
        }

        public async Task<TaskResultDTO> UpdateAsync(string id, StaffDTO staff)
        {
            throw new System.NotImplementedException();
        }

        public async Task<TaskResultDTO> UpdateAsync(string id, CasherDTO staff)
        {
            throw new System.NotImplementedException();
        }
    }
}
