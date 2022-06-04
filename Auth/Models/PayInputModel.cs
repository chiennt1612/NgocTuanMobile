using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Auth.Models
{
    public class PayInputModel
    {
        public int CompanyID { get; set; } = 0;
        public string CustomerCode { get; set; }
        public string InvoiceNo { get; set; }
        public int InvoiceAmount { get; set; }

        public bool IsAgree { get; set; } = true;
    }
}
