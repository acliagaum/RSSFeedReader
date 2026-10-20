using RSSFeedReader.Api.Contracts;
using RSSFeedReader.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("BlazorUi", policy =>
    {
        policy.WithOrigins("http://localhost:5213")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
builder.Services.AddSingleton<InMemorySubscriptionStore>();

var app = builder.Build();

app.UseCors("BlazorUi");

app.MapGet("/api/subscriptions", (InMemorySubscriptionStore store) =>
    Results.Ok(store.GetAll().Select(subscription => new SubscriptionResponse
    {
        Url = subscription.Url
    })));

app.MapPost("/api/subscriptions", (CreateSubscriptionRequest request, InMemorySubscriptionStore store) =>
{
    if (request.Url is null)
    {
        return Results.BadRequest();
    }

    var subscription = store.Add(request.Url);
    return Results.Ok(new SubscriptionResponse { Url = subscription.Url });
});

app.Run();

public partial class Program { }
