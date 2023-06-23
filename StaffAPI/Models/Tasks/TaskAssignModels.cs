using StaffAPI.Models.Tasks.DTO;
using System.Collections.Generic;

namespace StaffAPI.Models.Tasks
{
    public class TaskAssignModels
    {
        public string TaskId { get; set; }
        public StaffDTO Staff { get; set; }
    }

    public class TaskUnAssignModels
    {
        public string TaskId { get; set; }
        public string StaffCode { get; set; }
    }

    public class TaskCasherModels
    {
        public string TaskId { get; set; }
        public CasherDTO Casher { get; set; }
    }

    public class TaskProcessModels
    {
        public string TaskId { set; get; }
        public string Content { set; get; }
        public int Status { get; set; }
        public List<Attachment>? Attachments { get; set; }
    }

    public class Attachment
    {
        public string FileName { get; set; }
        public string FileData { get; set; }
    }

    public class TaskProcessDepartmentModels
    {
        public string TaskId { set; get; }
        public string Content { set; get; }
        public int Status { get; set; }
        public List<Attachment>? Attachments { get; set; }
        // 1. Next department
        // -1. Prev department
        // 0. Current department
        public int? NextDepartment { get; set; }
    }
}
