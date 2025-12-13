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

        var htmlContent = GenerateHtmlTemplate(htmlMessage);

        var message = MailHelper.CreateSingleEmail(sender, receiver, subject, plainTextContent: null, htmlContent);

        var response = await _client.SendEmailAsync(message);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Body.ReadAsStringAsync();
            throw new Exception($"SendGrid API failed with status {response.StatusCode}. Details: {errorBody}"); 
        }
    }
    
    private static string GenerateHtmlTemplate(string otpCode)
    {
        return $@"
        <div style=""font-family: Arial, sans-serif; background-color: #F4F0FF; padding: 20px; border-radius: 8px; max-width: 600px; margin: 20px auto; border: 1px solid #d0c0ff;"">
            
            <h1 style=""color: #3C2E59; text-align: center; margin-bottom: 30px;"">Відновлення Паролю</h1>

            <p style=""color: #4A4A4A; font-size: 16px; line-height: 1.6;"">
                Шановний користувачу!
            </p>
            <p style=""color: #4A4A4A; font-size: 16px; line-height: 1.6;"">
                Ви отримали цей лист, оскільки було надіслано запит на відновлення паролю для вашого облікового запису.
            </p>

            <div style=""background-color: #ffffff; border: 1px dashed #d0c0ff; padding: 30px; text-align: center; margin: 30px 0; border-radius: 5px; box-shadow: 0 4px 8px rgba(0, 0, 0, 0.05);"">
                <p style=""color: #3C2E59; font-size: 18px; margin-bottom: 15px;"">Ваш одноразовий код:</p>
                <p style=""color: #6A0DAD; font-size: 52px; font-weight: 900; letter-spacing: 5px; margin: 0;"">{otpCode}</p>
            </div>

            <p style=""color: #4A4A4A; font-size: 16px; line-height: 1.6;"">
                Цей код дійсний протягом 5 хвилин. Будь ласка, введіть його на сторінці відновлення паролю для завершення процесу.
            </p>
            <p style=""color: #4A4A4A; font-size: 16px; line-height: 1.6;"">
                Якщо ви не запитували відновлення паролю, проігноруйте цей лист.
            </p>

            <hr style=""border: 0; border-top: 1px solid #d0c0ff; margin: 30px 0;"">
            <p style=""color: #888888; font-size: 12px; text-align: center;"">
                З повагою,<br>
                Команда CryptoAuth
            </p>
        </div>";
    }
}

public class EmailSettings
{
    public string ApiKey { get; set; }
    public string SenderEmail { get; set; }
    public string SenderName { get; set; }
}