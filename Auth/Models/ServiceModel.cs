using EntityFramework.API.Entities;
using System.Collections.Generic;

namespace Auth.Models
{
    public class ServiceDetailModel
    {
        public Service _Detail { get; set; }
        public IEnumerable<Service> _Related { get; set; }
    }
    public class ServiceModel
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Img { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public double PricePerson { get; set; }
        public double PriceCompany { get; set; }
        public string PriceText { get; set; }
    }

    public class ServiceInputModel
    {
        public long ServiceId { get; set; }
        public bool IsCompany { get; set; }
        public string CompanyName { get; set; } // CMND
        public double Price { get; set; }
        public string Fullname { get; set; }
        public string Address { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public bool IsAgree { get; set; } = true;
    }

    public class ChangeContractInputModel
    {
        public long ServiceId { get; set; }
        public bool IsCompany { get; set; }
        public string CompanyName { get; set; } // CMND
        public double Price { get; set; }
        public string Fullname { get; set; }
        public string Address { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public bool IsAgree { get; set; } = true;

        public string Noted { get; set; }
    }

    public class ChangePositionInputModel
    {
        public long ServiceId { get; set; }
        public bool IsCompany { get; set; }
        public string CompanyName { get; set; } // CMND
        public double Price { get; set; }
        public string Fullname { get; set; }
        public string Address { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public bool IsAgree { get; set; } = true;

        public string Noted { get; set; }
        public string Img { get; set; }
        public string Noted2 { get; set; }
        public string Img2 { get; set; }
    }

    public class ChangePriceInputModel
    {
        public long ServiceId { get; set; }
        public bool IsCompany { get; set; }
        public string CompanyName { get; set; } // CMND
        public double Price { get; set; }
        public string Fullname { get; set; }
        public string Address { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public bool IsAgree { get; set; } = true;
        public string Img { get; set; }
    }

    public enum PaymentMethod
    {
        Cash = 1,
        Tranfer = 2,
        PayOnline = 3
    }
}
