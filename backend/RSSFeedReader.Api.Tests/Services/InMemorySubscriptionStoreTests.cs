using RSSFeedReader.Api.Services;

namespace RSSFeedReader.Api.Tests.Services;

public sealed class InMemorySubscriptionStoreTests
{
    [Fact]
    public void New_store_has_no_subscriptions()
    {
        var store = new InMemorySubscriptionStore();

        Assert.Empty(store.GetAll());
    }

    [Fact]
    public void Add_retains_insertion_order_and_duplicates()
    {
        var store = new InMemorySubscriptionStore();

        store.Add("https://example.com/first.xml");
        store.Add("https://example.com/second.xml");
        store.Add("https://example.com/first.xml");

        Assert.Equal(
            [
                "https://example.com/first.xml",
                "https://example.com/second.xml",
                "https://example.com/first.xml"
            ],
            store.GetAll().Select(subscription => subscription.Url));
    }

    [Fact]
    public void Store_instances_are_independent()
    {
        var firstStore = new InMemorySubscriptionStore();
        var secondStore = new InMemorySubscriptionStore();

        firstStore.Add("https://example.com/feed.xml");

        Assert.Single(firstStore.GetAll());
        Assert.Empty(secondStore.GetAll());
    }

    [Fact]
    public void Get_all_returns_a_snapshot()
    {
        var store = new InMemorySubscriptionStore();
        store.Add("https://example.com/first.xml");

        var snapshot = store.GetAll();
        store.Add("https://example.com/second.xml");

        Assert.Single(snapshot);
        Assert.Equal("https://example.com/first.xml", snapshot[0].Url);
    }
}
