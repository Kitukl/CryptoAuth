using CryptoAnalyzer.Core.Events;
using CryptoAuth.BLL.DTOs;
using CryptoAuth.DAL.Repositories;
using MassTransit;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace CryptoAuth.BLL.Consumers;

public class NewsEventConsumer : IConsumer<NewsEvent>
{
    private readonly TrigersRepository _repository;

    public NewsEventConsumer(TrigersRepository repository)
    {
        _repository = repository;
    }
    public async Task Consume(ConsumeContext<NewsEvent> context)
    {
        var message = context.Message;

        var users = await _repository.GetTrigerForNotification(message.NewsText, message.Sentiment);
        
        var publishTasks = users.Select(user => context.Publish(new NotificationEvent
        {
            Email = user.Email,
            NotificationType = NotificationType.TrigerNews,
            Value = JsonSerializer.Serialize(new NewsMessage
            {
                Text = message.NewsText,
                Sentiment = message.Sentiment
            })
        }));

        await Task.WhenAll(publishTasks);
    }
}