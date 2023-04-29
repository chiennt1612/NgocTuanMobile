using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace StaffAPI.Models.Tasks.DTO
{
    public class TaskFilterModels
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Keyword { get; set; }        
        public string CustomerCode { get; set; }
        public int? Status { get; set; }
        public bool? IsExpired { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public int? Page { get; set; }
        public int? PageSize { get; set; }
    }
}
