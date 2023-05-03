using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using Utils.Models;

namespace StaffAPI.Models.Tasks.DTO
{
    public class TaskProcessDTO
    {
        public string Id { get; set; }
        public StaffInfo Staff { get; set; }
        public string Content { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
