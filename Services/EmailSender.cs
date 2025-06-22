using PhasePlayWeb.Models;

namespace PhasePlayWeb.Services
{
    public class EmailSender : IEmailSender
    {
        private IFunctional _functional { get; }
        private readonly IConfiguration _configuration;

        public EmailSender(IFunctional functional, IConfiguration configuration)
        {
            _functional = functional;
            _configuration = configuration;
        }

        public async Task<Task> SendEmailAsync(string email, string subject, string message)
        {
            try
            {
                EmailConfig _smtpOptions = new EmailConfig
                {
                    Email = _configuration["SmtpSettings:Username"],
                    Password = _configuration["SmtpSettings:Password"],
                    Hostname = _configuration["SmtpSettings:Host"],
                    Port = _configuration.GetValue<int>("SmtpSettings:Port"),
                    SSLEnabled = _configuration.GetValue<bool>("SmtpSettings:EnableSsl"),
                };
                _functional.SendEmailByGmailAsync(_smtpOptions.Email,
                                                _smtpOptions.SenderFullName,
                                                subject,
                                                message,
                                                email,
                                                email,
                                                _smtpOptions.Email,
                                                _smtpOptions.Password,
                                                _smtpOptions.Hostname,
                                                _smtpOptions.Port,
                                                _smtpOptions.SSLEnabled)
                                                .Wait();
                return Task.CompletedTask;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
