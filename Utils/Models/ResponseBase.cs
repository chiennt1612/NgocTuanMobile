using Microsoft.AspNetCore.Http;
namespace Utils.Models
{
    public class ResponseBase
    {
        public int? Status { get; set; }
        public int? Code { get; set; }
        public string? UserMessage { get; set; }
        public string? InternalMessage { get; set; }
        public string? MoreInfo { get; set; }

        public ResponseBase()
        {

        }
        public ResponseBase(string UserMessage, string InternalMessage, string MoreInfo, int Status = 0, int Code = StatusCodes.Status500InternalServerError)
        {
            this.Status = Status;
            this.Code = Code;
            this.UserMessage = UserMessage;
            this.InternalMessage = InternalMessage;
            this.MoreInfo = MoreInfo;
        }
    }
}
