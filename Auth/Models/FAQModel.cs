using EntityFramework.API.Entities;
using System.Collections.Generic;

namespace Auth.Models
{
    public class FAQModel
    {
        public FAQ fAQ { get; set; }
        public IEnumerable<FAQ> fAQs { get; set; }
    }
}
