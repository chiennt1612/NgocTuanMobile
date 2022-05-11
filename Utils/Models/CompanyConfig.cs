using System.Collections.Generic;

namespace Utils.Models
{
    public class CompanyConfig
    {
        public List<Company> Companys { get; set; }

    }
    public class Company
    {
        public CompanyInfo Info { get; set; }
        public InvoiceConfig Config { get; set; }

    }

    public class CompanyInfo
    {
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string CompanyNameEn { get; set; }
        public string CompanyLogo { get; set; }
    }

    public class CompanyInfoInput
    {
        public string CompanyCode { get; set; }
    }

    public class InvoiceConfig
    {
        public string APIUrl { get; set; }
        public string APIToken { get; set; }
        public List<string> APIFunctions { get; set; }
    }
}
