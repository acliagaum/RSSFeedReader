# Quickstart: Validate Feed Subscriptions

This guide validates the API and the browser flow after the projects described in [plan.md](./plan.md) are implemented.

## Prerequisites

- .NET 10 SDK
- A browser
- The API and Blazor UI projects created under `backend\` and `frontend\`

## Run automated API tests

From the repository root, run:

```powershell
dotnet test backend\RSSFeedReader.Api.Tests\RSSFeedReader.Api.Tests.csproj
```

Expected results:

- An initially empty test host returns an empty subscription array.
- Posting a URL returns the appended subscription.
- A subsequent GET returns every posted URL in insertion order.
- Posting the same URL more than once produces separate array entries.
- A fresh API host starts empty, demonstrating the process-lifetime reset.

## Run the local application

1. Confirm the API profile listens at `http://localhost:5151` and the UI profile at `http://localhost:5213`, or update the values consistently.
2. Confirm `frontend\RSSFeedReader.UI\wwwroot\appsettings.json` has an `ApiBaseUrl` matching the API address and API path.
3. Confirm the API CORS policy allows the UI's actual origin from its launch profile.
4. In separate PowerShell windows, start the API and UI:

   ```powershell
   dotnet run --project backend\RSSFeedReader.Api\RSSFeedReader.Api.csproj
   ```

   ```powershell
   dotnet run --project frontend\RSSFeedReader.UI\RSSFeedReader.UI.csproj
   ```

5. Open `http://localhost:5213` and verify the subscription view loads with an empty list.
6. Submit `https://example.com/feed.xml`, then submit it again and submit a different URL.

Expected results:

- Every submitted value appears immediately without navigating or manually refreshing.
- The two identical values appear as two separate entries, in submission order.
- The browser console and API output show no connection or CORS errors.
- No feed content is fetched or displayed.
- Restarting the API clears the list; reloading the UI shows an empty list after the API restart.

Run equivalent commands on macOS/Linux using the platform's path separator for project paths.
