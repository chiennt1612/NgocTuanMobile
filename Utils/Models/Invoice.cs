using System;
using System.Collections.Generic;

namespace Utils.Models
{
    #region input class
    public class ContractInput : ContractInputBase
    {
        public int CompanyID { get; set; } = 0;
    }
    public class ContractInputBase
    {
        public string Mobile { get; set; }
    }
    public class InvoiceInput
    {
        public int CompanyID { get; set; } = 0;
        public string CustomerCode { get; set; }
    }
    public class CheckPayInput
    {
        public int CompanyID { get; set; } = 0;
        public string OnePayID { get; set; }
    }
    public class PayInput : InvoiceInput
    {
        public string OnePayID { get; set; }
        public string InvoiceNo { get; set; }
        public int InvoiceAmount { get; set; }

        public bool IsAgree { get; set; } = true;
    }
    public class InvoiceAllInput : InvoiceInput
    {
        public int Page { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string PaymentStatus { get; set; }
    }
    public class InvoiceAllAInput
    {
        public int CompanyID { get; set; } = 0;
        public string CustomerCodeList { get; set; }
        public int Page { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string PaymentStatus { get; set; }
    }
    public class EVNCodeInput
    {
        public int CompanyID { get; set; } = 0;
        public string EVNCode { get; set; }
    }
    public class InvQrCodeInput
    {
        public int CompanyID { get; set; } = 0;
        public string InvoiceSerial { get; set; }
        public string InvoiceNumber { get; set; }
    }
    #endregion
    #region base class
    public class CustomerInfo : ContractInputBase
    {
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string CustomerType { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string WaterIndexCode { get; set; }
        public string TaxCode { get; set; }
    }
    public class ResultBase
    {
        public string Message { get; set; }
        public int ResponseStatus { get; set; }
    }
    public class DataResultBase : ResultBase
    {
        public string DataStatus { get; set; }
    }
    public class InvoiceBase
    {
        public long InvCode { get; set; }
        public string InvRemarks { get; set; }
        public string MaSoBiMat { get; set; }
        public string InvNumber { get; set; }
        public string InvSerial { get; set; }
        public DateTime InvDate { get; set; }
        public double TaxPer { get; set; }
        public double InvAmountWithoutTax { get; set; }
        public double InvAmount { get; set; }
    }
    public class InvoiceStatusBase : InvoiceBase
    {
        public int PaymentStatus { get; set; } // 1. Da thanh toan; 0. Chua thanh toan
    }
    #endregion
    #region result class
    public class InvoiceResult : DataResultBase
    {
        public ItemsData ItemsData { get; set; }
        public string Keyword { get; set; } = "";
        public bool IsAgree { get; set; } = true;
    }
    public class UndoPayResult : ResultBase
    {
        public string UndoPayStatus { get; set; }
    }
    public class PayResult : ResultBase
    {
        public string PayStatus { get; set; }
    }
    public class InvoiceAllResult : DataResultBase
    {
        public int Rowcount { get; set; }
        public ItemsDataAll ItemsData { get; set; }
        public string Keyword { get; set; } = "";
        public bool IsAgree { get; set; } = true;
    }
    public class InvoiceDataResult : DataResultBase
    {
        public List<ItemsDatum> ItemsData { get; set; }
    }
    public class InvoiceQRCode : DataResultBase
    {
        public InvQrCode ItemsData { get; set; }
        public CompanyInfo CompanyInfo { get; set; }
    }
    public class CustomerInfoResult : DataResultBase
    {
        public List<CustomerInfo> ItemsData { get; set; }
    }
    #endregion
    #region Other
    public class ItemsData
    {
        public int CompanyID { get; set; } = 0;
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string WaterIndexCode { get; set; }
        public List<InvList> InvList { get; set; }
    }
    public class InvList : InvoiceBase
    {
    }
    public class InvListAll : InvoiceStatusBase
    {
    }
    public class ItemsDataAll : CustomerInfo
    {
        public List<InvListAll> InvList { get; set; }
    }
    public class ContractList : CustomerInfo
    {
    }
    public class ItemsList : ContractInputBase
    {
        public List<ContractList> ContractList { get; set; }
    }
    public class ContractResult : DataResultBase
    {
        public ItemsList ItemsData { get; set; }
        public CompanyInfo CompanyInfo { get; set; }
    }
    public class ContractInfo
    {
        public List<ContractList> ContractList { get; set; }
        public CompanyInfo CompanyInfo { get; set; }
    }
    public class ContractOneInfo
    {
        public ContractList ContractInfo { get; set; }
        public CompanyInfo CompanyInfo { get; set; }
    }
    public class ItemsDatum : InvQrCode
    {
        public int CompanyId { get; set; } = 0;
    }
    public class InvQrCode : InvoiceStatusBase
    {
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string WaterIndexCode { get; set; }
    }
    #endregion
}
