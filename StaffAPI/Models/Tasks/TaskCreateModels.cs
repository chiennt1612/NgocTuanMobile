using StaffAPI.Models.Tasks.DTO;
using System;
using System.Collections.Generic;

namespace StaffAPI.Models.Tasks
{
    public class TaskCreateModels
    {
        public string Name { get; set; }
        public long ServiceId { get; set; }
        public CustomerDTO Customer { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public List<string> Attachment { get; set; }
        public string Content { get; set; }
    }
}
