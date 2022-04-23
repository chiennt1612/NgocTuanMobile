namespace Utils.Models
{
    public class ResponseBase
    {
        public int? Status { get; set; }
        public int? Code { get; set; }
        public string? UserMessage { get; set; }
        public string? InternalMessage { get; set; }
        public string? MoreInfo { get; set; }
    }
}
