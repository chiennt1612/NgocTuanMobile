using StaffAPI.Helper;
using StaffAPI.Services.Interfaces;
using Decryptor;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Mail;
using System.Threading.Tasks;

namespace StaffAPI.Services
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly ILogger<SmtpEmailSender> _logger;
        private readonly SmtpConfiguration _configuration;
        private readonly SmtpClient _client;
        private readonly IDecryptorProvider _decryptor;
        public SmtpEmailSender(ILogger<SmtpEmailSender> logger, SmtpConfiguration configuration, IDecryptorProvider decryptor)
        {
            _logger = logger;
            _decryptor = decryptor;
            _configuration = configuration;
            _client = new SmtpClient
            {
                Host = _configuration.Host,
                Port = _configuration.Port,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                EnableSsl = _configuration.UseSSL
            };

            if (!string.IsNullOrEmpty(_configuration.Password))
                _client.Credentials = new System.Net.NetworkCredential(_decryptor.Decrypt(_configuration.Login), _decryptor.Decrypt(_configuration.Password));
            else
                _client.UseDefaultCredentials = true;
        }
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            _logger.LogInformation($"Sending email: {email}, subject: {subject}, message: {htmlMessage}");
            try
            {
                var from = String.IsNullOrEmpty(_configuration.From) ? _decryptor.Decrypt(_configuration.Login) : _decryptor.Decrypt(_configuration.From);
                var mail = new MailMessage(new MailAddress(from, "CLEAN WATER NGOC TUAN - NAGAOKA COMPANY LIMITED"), new MailAddress(email));
                mail.IsBodyHtml = true;
                mail.Subject = subject;
                mail.Body = htmlMessage;
                mail.Bcc.Add(from);

                _client.Send(mail);
                _logger.LogInformation($"Email: {email}, subject: {subject}, message: {htmlMessage} successfully sent");

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception {ex} during sending email: {email}, subject: {subject}");
                throw;
            }
        }
    }
}
