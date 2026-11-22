using System.Net;
using System.Net.Mail;
using EQIMTANVIZITI.Settings;
using Microsoft.Extensions.Options;

namespace EQIMTANVIZITI.Services;

public class EmailService : IEmailService
{
    private readonly SmtpSettings _s;
    public EmailService(IOptions<SmtpSettings> options) => _s = options.Value;

    public async Task SendAsync(string to, string subject, string htmlBody)
    {
        using var client = new SmtpClient(_s.Host, _s.Port)
        {
            EnableSsl = _s.EnableSsl,
            Credentials = new NetworkCredential(_s.User, _s.Password)
        };
        using var msg = new MailMessage(new MailAddress(_s.FromEmail, _s.FromName), new MailAddress(to))
        {
            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true
        };
        await client.SendMailAsync(msg);
    }
}
