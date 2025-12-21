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

        var htmlContent = htmlMessage.Length == 6 ? GenerateHtmlTemplate(htmlMessage) : GenerateEmailConfirmation(htmlMessage);

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

    private static string GenerateEmailConfirmation(string url)
    {
        return $@"
        <div style=""font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #F9F7FF; padding: 40px 20px; border-radius: 12px; max-width: 550px; margin: 20px auto; border: 1px solid #E6E0FF; box-shadow: 0 10px 25px rgba(106, 13, 173, 0.05);"">
    
    <div style=""text-align: center; margin-bottom: 30px;"">
        <div style=""display: inline-block; background-color: #6A0DAD; padding: 12px; border-radius: 12px; margin-bottom: 15px;"">
             <svg width=""32"" height=""32"" viewBox=""0 0 24 24"" fill=""none"" xmlns=""http://www.w3.org/2000/svg"">
                <path d=""M22 6C22 4.9 21.1 4 20 4H4C2.9 4 2 4.9 2 6M22 6V18C22 19.1 21.1 20 20 20H4C2.9 20 2 19.1 2 18V6M22 6L12 13L2 6"" stroke=""white"" stroke-width=""2"" stroke-linecap=""round"" stroke-linejoin=""round""/>
             </svg>
        </div>
        <h1 style=""color: #3C2E59; font-size: 24px; font-weight: 800; margin: 0; letter-spacing: -0.5px;"">Підтвердження пошти</h1>
    </div>

    <div style=""background-color: #ffffff; padding: 40px; border-radius: 16px; text-align: center; border: 1px solid #EFEDF5;"">
        <p style=""color: #4A4A4A; font-size: 16px; line-height: 1.6; margin-bottom: 10px;"">
            Вітаємо в <strong>CryptoAuth</strong>! 
        </p>
        <p style=""color: #716D85; font-size: 15px; line-height: 1.6; margin-bottom: 30px;"">
            Ми раді, що ви з нами. Щоб почати користуватися всіма можливостями сервісу, будь ласка, підтвердьте вашу адресу.
        </p>

        <a href=""{url}"" style=""display: inline-block; background-color: #6A0DAD; color: #ffffff; padding: 16px 36px; border-radius: 10px; text-decoration: none; font-weight: 700; font-size: 16px; transition: background-color 0.3s ease; box-shadow: 0 4px 12px rgba(106, 13, 173, 0.25);"">
            Підтвердити Email
        </a>

        <p style=""color: #9B98AC; font-size: 13px; margin-top: 30px; line-height: 1.5;"">
            Або скопіюйте це посилання в браузер:<br>
            <span style=""color: #6A0DAD; word-break: break-all;"">{url}</span>
        </p>
    </div>

    <div style=""margin-top: 30px; text-align: center;"">
        <p style=""color: #888888; font-size: 13px; line-height: 1.6;"">
            Якщо ви не реєструвалися на нашому сайті, просто проігноруйте цей лист.
        </p>
        <hr style=""border: 0; border-top: 1px solid #E6E0FF; margin: 20px 0;"">
        <p style=""color: #B2B0C2; font-size: 11px; text-transform: uppercase; letter-spacing: 1px;"">
            © 2025 CryptoAuth Inc. • Digital Assets Security
        </p>
    </div>
</div>";
    }
}

public class EmailSettings
{
    public string ApiKey { get; set; }
    public string SenderEmail { get; set; }
    public string SenderName { get; set; }
}