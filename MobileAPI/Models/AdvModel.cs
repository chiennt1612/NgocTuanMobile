using EntityFramework.API.Entities;
namespace MobileAPI.Models
{
    public class AdvModel
    {
        public long Id { get; set; }
        public string CustomerName { get; set; }
        public string Website { get; set; }
        public AdvPosition Position { get; set; }
        public string Img { get; set; }
        public string AdvScript { get; set; }
    }
}
