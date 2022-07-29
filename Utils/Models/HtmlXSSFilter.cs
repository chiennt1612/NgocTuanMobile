using System.Collections.Generic;

namespace Utils.Models
{
    public class HtmlXSSFilter
    {
        public List<string> TagNotAllow { get; set; }
        public List<string> AttributeNotAllow { get; set; }
    }
}
