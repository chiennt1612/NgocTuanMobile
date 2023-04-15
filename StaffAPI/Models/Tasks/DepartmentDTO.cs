namespace StaffAPI.Models.Tasks
{
    public class DepartmentDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int StatusId { get; set; } 
        //0. Un process
        //1. Processing
        //2. Finish
    }
}
