namespace StaffAPI.Models.Tasks.DTO
{
    public class DepartmentDTO
    {
        public int DepartmentId { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string TaskName { get; set; }
        public int? StatusId { get; set; }
        //0. Un process
        //1. Processing
        //2. Finish
        //9. Create
    }
}
