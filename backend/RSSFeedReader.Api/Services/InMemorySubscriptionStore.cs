using RSSFeedReader.Api.Models;

namespace RSSFeedReader.Api.Services;

public sealed class InMemorySubscriptionStore
{
    private readonly object _sync = new();
    private readonly List<Subscription> _subscriptions = [];

    public Subscription Add(string url)
    {
        var subscription = new Subscription(url);

        lock (_sync)
        {
            _subscriptions.Add(subscription);
        }

        return subscription;
    }

    public IReadOnlyList<Subscription> GetAll()
    {
        lock (_sync)
        {
            return _subscriptions.ToArray();
        }
    }
}
