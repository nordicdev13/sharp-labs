using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace RazorPagesCar.Services;

public class SmtpEmailSender : IEmailSender
{
    private readonly EmailOptions _opt;

    public SmtpEmailSender(IOptions<EmailOptions> opt) => _opt = opt.Value;

    public async Task SendAsync(string toEmail, string subject, string htmlBody, string? plainText = null)
    {
        using var client = new SmtpClient(_opt.Host, _opt.Port)
        {
            EnableSsl = _opt.UseSsl,
            Credentials = new NetworkCredential(_opt.User, _opt.Password)
        };

        var msg = new MailMessage
        {
            From = new MailAddress(_opt.From, _opt.FromName),
            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true
        };
        msg.To.Add(new MailAddress(toEmail));

        if (!string.IsNullOrWhiteSpace(plainText))
        {
            msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(plainText, null, "text/plain"));
        }

        await client.SendMailAsync(msg);
    }
}
