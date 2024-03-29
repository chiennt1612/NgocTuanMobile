﻿using System.Collections.Generic;

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
        public List<string> MobileTest { get; set; }
        public string OTPTest { get; set; }
    }

    public class AboutID
    {
        public long Vi { get; set; }
        public long En { get; set; }
    }

    public class AboutPage
    {
        public AboutID AboutID { get; set; }
        public GuideID GuideID { get; set; }
        public GuideID ExtendID { get; set; }
    }

    public class GuideID
    {
        public List<long> Vi { get; set; }
        public List<long> En { get; set; }
    }

    public class SmtpConfiguration
    {
        public string From { get; set; }
        public string Host { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public int Port { get; set; } = 587; // default smtp port
        public bool UseSSL { get; set; } = true;
    }

    public class LoggingFiles
    {
        public string FileName { get; set; }
        public long FileSizeLimitBytes { get; set; }
        public int RollingInterval { get; set; }
        public string OutputTemplate { get; set; }
    }

    public class LoggingMongoDB
    {
        public string URI { get; set; }
        public string Collection { get; set; }
    }

    public class LoggingProvider
    {
        public LoggingMongoDB LogMongoDB { get; set; }
        public LoggingFiles LogFiles { get; set; }
        public int LoggingType { get; set; }
        public LoggingProvider()
        {
            LogMongoDB = new LoggingMongoDB();
            LogFiles = new LoggingFiles();
        }
    }
}
