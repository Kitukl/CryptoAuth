namespace CryptoAnalyzer.Core.Events;

public class NewsEvent
{
    public string NewsText { get; set; }
    public double? Sentiment { get; set; }
}