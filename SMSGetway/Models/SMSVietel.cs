namespace SMSGetway.Models
{
    public class SMSVietel
    {
        public string User { get; set; }
        public string Password { get; set; }
        public string CPCode { get; set; }
        public string urlAPI { get; set; }
        public string CommandCode { get; set; }

        public int ContentType { get; set; }
        public int RequestID { get; set; }
    }
}
