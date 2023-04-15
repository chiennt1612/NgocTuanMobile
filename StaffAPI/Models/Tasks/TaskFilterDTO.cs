using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace StaffAPI.Models.Tasks
{
    public class TaskFilterDTO
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }

        public string PIC { get; set; }
        public string Owner { get; set; }
        public string StaffCode { get; set; }
        public string DepartmentCode { get; set; }
        public string CustomerCode { get; set; }
        public int? Status { get; set; }
        public bool? IsExpired { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public int? Page { get; set; }
        public int? PageSize { get; set; }
    }
}
