using System;
using System.Text.Json.Serialization;
using Utils.Models;

namespace EntityFramework.API.Entities
{
    public class Notice
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string DeviceID { get; set; }
        public OSType OS { get; set; }
        public int NoticeTypeId { get; set; }
        public string NoticeTypeName { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public bool IsHTML { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadDate { get; set; }
        public string Author { get; set; }
        public DateTime CreateDate { get; set; }
        [JsonIgnore]
        public bool IsDelete { get; set; }
        [JsonIgnore]
        public DateTime? DeleteDate { get; set; }
    }
}
