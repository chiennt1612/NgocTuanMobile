using Utils.Models;

namespace Auth.Models
{
    public class InvoiceFindModel : SearchDateModel
    {
        //public string CustomerCode { get; set; }
        public int? PaymentStatus { get; set; }
    }



    public class InvoiceModel : InvoiceFindModel
    {
        public int CompanyID { get; set; } = 0;
        public string CustomerCode { get; set; }
    }
}
