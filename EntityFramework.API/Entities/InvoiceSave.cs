using System;
using System.ComponentModel.DataAnnotations;

namespace EntityFramework.API.Entities
{
    public class InvoiceSave
    {
        public long Id { get; set; }
        [StringLength(30)]
        public string CustomerCode { get; set; }
        [StringLength(150)]
        public string CustomerName { get; set; }
        [StringLength(230)]
        public string Address { get; set; }
        [StringLength(50)]
        public string MaSoBiMat { get; set; }
        [StringLength(20)]
        public string InvSerial { get; set; }
        [StringLength(20)]
        public string InvNumber { get; set; }
        public DateTime InvDate { get; set; }
        public double TaxPer { get; set; }
        public double InvAmountWithoutTax { get; set; }
        [StringLength(30)]
        public string InvCode { get; set; }
        [StringLength(300)]
        public string InvRemarks { get; set; }
        public double InvAmount { get; set; }
        public int PaymentStatus { get; set; }
        [StringLength(30)]
        public int CompanyId { get; set; }
        [StringLength(30)]
        public string CompanyCode { get; set; }
        [StringLength(150)]
        public string CompanyName { get; set; }
        [StringLength(150)]
        public string CompanyNameEn { get; set; }
        [StringLength(200)]
        public string CompanyLogo { get; set; }
        [StringLength(30)]
        public string Taxcode { get; set; }
        public long UserId { get; set; }
        [StringLength(500)]
        public string Link { get; set; }
    }
}
