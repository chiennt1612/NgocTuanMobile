using System;
using System.Collections.Generic;

namespace Utils.Models
{
    public class InvoiceResult
    {
        public ItemsData ItemsData { get; set; }
        public string DataStatus { get; set; }
        public string Message { get; set; }
        public int ResponseStatus { get; set; }

        public string Keyword { get; set; } = "";
        public bool IsAgree { get; set; } = true;
    }

    public class InvoiceInput
    {
        public string CustomerCode { get; set; }
    }

    public class ItemsData
    {
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public List<InvList> InvList { get; set; }
    }

    public class InvList
    {
        public string InvCode { get; set; }
        public string InvRemarks { get; set; }
        public int InvAmount { get; set; }
    }

    public class CheckPayInput
    {
        public string OnePayID { get; set; }
    }

    public class PayInput
    {
        public string OnePayID { get; set; }
        public string CustomerCode { get; set; }
        public string InvoiceNo { get; set; }
        public int InvoiceAmount { get; set; }

        public bool IsAgree { get; set; } = true;
    }

    public class InvoiceAllInput
    {
        public string CustomerCode { get; set; }
        public int Page { get; set; }
    }

    public class UndoPayResult
    {
        public string UndoPayStatus { get; set; }
        public string Message { get; set; }
        public string ResponseStatus { get; set; }
    }

    public class PayResult
    {
        public string PayStatus { get; set; }
        public string Message { get; set; }
        public string ResponseStatus { get; set; }
    }

    public class InvoiceAllResult
    {
        public int Rowcount { get; set; }
        public ItemsDataAll ItemsData { get; set; }
        public string DataStatus { get; set; }
        public string Message { get; set; }
        public int ResponseStatus { get; set; }

        public string Keyword { get; set; } = "";
        public bool IsAgree { get; set; } = true;
    }

    public class InvListAll
    {
        public int InvCode { get; set; }
        public string InvRemarks { get; set; }
        public string MaSoBiMat { get; set; }
        public string InvNumber { get; set; }
        public string InvSerial { get; set; }
        public DateTime InvDate { get; set; }
        public double TaxPer { get; set; }
        public double InvAmountWithoutTax { get; set; }
        public double InvAmount { get; set; }
        public int PaymentStatus { get; set; } // 1. Da thanh toan; 0. Chua thanh toan
    }

    public class ItemsDataAll
    {
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string WaterIndexCode { get; set; }
        public List<InvListAll> InvList { get; set; }
    }
}
