namespace StaffAPI.Models.Invoice
{
    public class PayInputModel
    {
        public int CompanyID { get; set; } = 0;
        public string CustomerCode { get; set; }
        public long InvCode { get; set; }
        public double InvAmount { get; set; }
        public bool IsSave { get; set; } = false;
        public bool IsAgree { get; set; } = true;
    }
}
