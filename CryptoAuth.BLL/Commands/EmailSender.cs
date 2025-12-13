using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace CryptoAuth.BLL.Commands;

public class EmailSender : IEmailSender
{
    private readonly ILogger<EmailSender> _logger;
    private readonly EmailSettings _emailOptions;
    private readonly ISendGridClient _client;

    public EmailSender(IOptions<EmailSettings> emailOptions, ILogger<EmailSender> logger, ISendGridClient client)
    {
        _logger = logger;
        _emailOptions = emailOptions.Value;
        _client = client;
    }
    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var sender = new EmailAddress(_emailOptions.SenderEmail, _emailOptions.SenderName);
        var receiver = new EmailAddress(email);

        var message = MailHelper.CreateSingleEmail(sender, receiver, subject, plainTextContent: null, htmlMessage);

        var response = await _client.SendEmailAsync(message);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Body.ReadAsStringAsync();
            throw new Exception($"SendGrid API failed with status {response.StatusCode}. Details: {errorBody}"); 
        }
    }
}

public class EmailSettings
{
    public string ApiKey { get; set; }
    public string SenderEmail { get; set; }
    public string SenderName { get; set; }
}