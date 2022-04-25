using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFramework.API.Entities.EntityBase
{
    public class BaseEntityList<T>
    {
        public IEnumerable<T> list { get; set; } = default;
        public int totalRecords { get; set; } = 0;
        public int page { get; set; } = 0;
        public int pageSize { get; set; } = 0;
    }
}
