namespace Auth.Helper
{
    public class RegisterConfiguration
    {
        public bool Enabled { get; set; } = true;
    }

    public enum LoginResolutionPolicy
    {
        Username = 0,
        Email = 1
    }
    public class LoginConfiguration
    {
        public LoginResolutionPolicy ResolutionPolicy { get; set; } = LoginResolutionPolicy.Username;
        public int OTPTimeLife { get; set; } = 3;
        public int OTPLimitedOnDay { get; set; } = 5;
        public string OTPSMSContent { get; set; } = "Your security code is: {OTPCODE}";
        public string MobileTest { get; set; }
        public string OTPTest { get; set; }
    }
}
