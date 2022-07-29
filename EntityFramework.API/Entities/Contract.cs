using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFramework.API.Entities
{
    public class Contract
    {
        public long Id { get; set; }
        public int CompanyId { get; set; }
        [StringLength(30)]
        public string CustomerCode { get; set; }
        [StringLength(150)]
        public string CustomerName { get; set; }
        [StringLength(150)]
        public string CustomerType { get; set; }
        [StringLength(30)]
        public string Mobile { get; set; }
        [StringLength(230)]
        public string Address { get; set; }
        [StringLength(150)]
        public string Email { get; set; }
        [StringLength(30)]
        public string WaterIndexCode { get; set; }
        [StringLength(30)]
        public string TaxCode { get; set; }
        public long UserId { get; set; }
    }
}
