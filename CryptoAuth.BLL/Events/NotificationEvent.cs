using CryptoAuth.BLL.DTOs;

namespace CryptoAnalyzer.Core.Events;

public class NotificationEvent
{
    public string Email { get; set; }
    public string Value { get; set; }
    public NotificationType NotificationType { get; set; }
}