using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFramework.API.Entities
{
    public class InvoiceSave
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
        public int InvAmount { get; set; }
        public int PaymentStatus { get; set; }
    }
}
