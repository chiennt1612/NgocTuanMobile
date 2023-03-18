using EntityFramework.API.Entities;

namespace StaffAPI.Models
{
    public class PayModel
    {
        public string Url { get; set; }
        public Contact order { get; set; }
    }
}
