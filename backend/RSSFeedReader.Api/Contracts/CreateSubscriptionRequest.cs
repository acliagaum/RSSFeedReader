namespace RSSFeedReader.Api.Contracts;

public sealed class CreateSubscriptionRequest
{
    public required string Url { get; init; }
}
