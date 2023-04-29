using Utils.Models;
using System.Collections.Generic;

namespace StaffAPI.Models.Tasks.DTO
{
    public class TaskListDTO
    {
        public List<TaskDTO> Data { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public int? Rowcount { get; set; }
    }

    public class TaskListResultDTO : ResponseBase
    {
        public TaskListDTO Data { get; set; }
    }
}
