using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using Utils.Models;
using System.Collections.Generic;

namespace StaffAPI.Models.Tasks.DTO
{
    public class TaskProcessDTO
    {
        public string Id { get; set; }
        public StaffInfo Staff { get; set; }
        public string TaskName { get; set; }
        public string Content { get; set; }
        public List<string>? Attachments { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
