using System;

namespace Auth.Models
{
    public class InvoiceFindModel : SearchDateModel
    {
        public string CustomerCode { get; set; }
        public int? PaymentStatus { get; set; }
    }

    public class SearchDateModel
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }

    public class InvoiceModel : SearchDateModel
    {
        public int CompanyID { get; set; } = 0;
        public string CustomerCode { get; set; }
        public int? PaymentStatus { get; set; }
    }
}
