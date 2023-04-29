namespace StaffAPI.Helper
{
    public class DBSetting : IDBSetting
    {
        public string MONGODB_URL { get; set; }
        public string DatabaseName { get; set; }
    }

    public interface IDBSetting
    {
        string MONGODB_URL { get; set; }
        string DatabaseName { get; set; }
    }
}
