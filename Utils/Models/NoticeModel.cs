using System;
using System.Collections.Generic;

namespace Utils.Models
{
    public class SearchDateModel
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }

    public class InvoiceSaveSearchModel : SearchDateModel
    {
        public string CustomerCode { get; set; }
    }

    public class InvoiceSaveSearchModelA : InvoiceSaveSearchModel
    {
        public long UserId { get; set; }
    }

    public class NoticeIdListModel
    {
        public IList<long> Ids { get; set; }
    }
    public class NoticeModel : SearchDateModel
    {
        //public string DeviceId { get; set; }
        public int? NoticeTypeId { get; set; }
        public bool? IsRead { get; set; }
        public string Author { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }

    public class NoticeInputModel
    {
        public int CompanyId { get; set; } = 0;
        public string CustomerCode { get; set; }
        public int NoticeTypeId { get; set; }
        public string NoticeTypeName { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public bool IsHTML { get; set; }
        public string Author { get; set; }
        public string Link { get; set; }
    }

    public class NoticePushFirebaseModel
    {
        public string Username { get; set; }
        public IList<string> DeviceID { get; set; }
        public IList<string> Token { get; set; }
        public int NoticeTypeId { get; set; }
        public string NoticeTypeName { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public bool IsHTML { get; set; }
        public string Author { get; set; }
        public string Link { get; set; }
        public bool IsRead { get; set; } = false;
    }

    public enum OSType
    {
        IOs = 2,
        Android = 1,
        PC = 0
    }
}
