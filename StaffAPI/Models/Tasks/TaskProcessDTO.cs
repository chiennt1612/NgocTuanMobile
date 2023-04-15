using System;
using Utils.Models;

namespace StaffAPI.Models.Tasks
{
    public class TaskProcessDTO
    {
        public string Id { get; set; }
        public StaffInfo Staff { get; set; }
        public string Content { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}
