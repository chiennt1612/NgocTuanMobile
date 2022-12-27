using System;
using Utils.Models;

namespace MobileAPI.Models
{
    public class OrderModel
    {
        public long Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string UserName { get; set; }
        public string Description { get; set; }
        public int StatusId { get; set; }
        public string Fullname { get; set; }
        public string Address { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public int? PaymentMethod { get; set; }
        public double? Total { get; set; }
        public double? FeeShip { get; set; }
        public long? ServiceId { get; set; }
    }

    public class OrderSearchModel : SearchDateModel
    {
        public int? StatusId { get; set; }
        public bool IsService { get; set; } = false;
    }
}
