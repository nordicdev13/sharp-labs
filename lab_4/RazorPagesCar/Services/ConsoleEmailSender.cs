using Microsoft.Extensions.Logging;

namespace RazorPagesCar.Services;

public class ConsoleEmailSender : IEmailSender
{
    private readonly ILogger<ConsoleEmailSender> _logger;

    public ConsoleEmailSender(ILogger<ConsoleEmailSender> logger)
    {
        _logger = logger;
    }

    public Task SendAsync(string toEmail, string subject, string htmlBody, string? plainText = null)
    {
        _logger.LogInformation("\n================ [EMAIL SENDER (CONSOLE)] ================\nДо: {To}\nТема: {Subject}\nТіло:\n{Body}\n=========================================================\n", 
            toEmail, subject, plainText ?? htmlBody);
        return Task.CompletedTask;
    }
}
