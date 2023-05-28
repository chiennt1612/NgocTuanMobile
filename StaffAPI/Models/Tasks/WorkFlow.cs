using StaffAPI.Models.Tasks.DTO;
using System.Collections.Generic;

namespace StaffAPI.Models.Tasks
{
    public interface IWorkFlowConfig
    {
        public IList<Services>? WorkFlow { get; set; }
        public IList<string>? DepartmentAlow { get; set; }
    }

    public class WorkFlowConfig : IWorkFlowConfig
    {
        public IList<Services>? WorkFlow { get; set; }
        public IList<string>? DepartmentAlow { get; set; }
    }

    public class Services
    {
        public long? Id { get; set; }
        public int? CustomerType { get; set; }
        public long? ServiceId { get; set; }
        public string ServiceName { get; set; }
        public string Image { get; set; }
        public List<DepartmentDTO>? Flow { get; set; }
    }

    public class TaskType
    {
        public long? TaskTypeId { get; set; }
        public string TaskTypeName { get; set; }
    }
}
