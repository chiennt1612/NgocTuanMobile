namespace StaffAPI.Models.News
{
    public class CategoryNewsModel
    {
        public long? ParentId { get; set; }
        public long Id { get; set; }
        public string Name { get; set; }
        public string Img { get; set; }
    }
}
