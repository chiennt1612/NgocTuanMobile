using EntityFramework.API.Entities;
using System.Collections.Generic;

namespace StaffAPI.Models
{
    public class FAQModel
    {
        public FAQ fAQ { get; set; }
        public IEnumerable<FAQ> fAQs { get; set; }
    }
}
