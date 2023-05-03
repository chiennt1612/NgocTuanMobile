using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using Utils.Models;

namespace StaffAPI.Models.Tasks.DTO
{
    // StaffDTO = StaffInfo
    // ServiceDTO = ServiceModel
    // CustomerDTO = CustomerInfo
    public class TaskDTO
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        // 0. Create new
        // 1. In-Progress
        // 2. Done
        // 3. Re-open
        // -9. Cancle
        public int? Status { get; set; }
        // 0. Un-payment
        // 1. Payment
        public int? PaymentStatus { get; set; }
        public CasherDTO? Casher { get; set; }
        public List<string>? Attachment { get; set; }
        public long? WorkFlowId { get; set; }
        public ServiceDTO? Service { get; set; }
        public List<StaffDTO>? Staff { get; set; }
        public CustomerDTO? Customer { get; set; }
        public List<DepartmentDTO>? Department { get; set; }
        public List<TaskProcessDTO>? TaskProcess { get; set; }
        public string Content { get; set; }
        public StaffDTO? Owner { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? ExpiredDate { get; set; }
        public StaffDTO? ChangedUser { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}
