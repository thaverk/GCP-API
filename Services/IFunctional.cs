namespace PhasePlayWeb.Services
{
    public interface IFunctional
    {
        Task SendEmailByGmailAsync(string fromEmail,
                string fromFullName,
                string subject,
                string messageBody,
                string toEmail,
                string toFullName,
                string smtpUser,
                string smtpPassword,
                string smtpHost,
                int smtpPort,
                bool smtpSSL);
    }
}
