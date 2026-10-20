# Research: Add Feed Subscriptions

Research inputs: the feature specification, project constitution, [ProjectGoals.md](../../StakeholderDocuments/ProjectGoals.md), [TechStack.md](../../StakeholderDocuments/TechStack.md), and current Microsoft .NET 10 documentation.

## Runtime and API

**Decision**: Target .NET 10 and use an ASP.NET Core Minimal API for the add/list endpoints.

**Rationale**: The installed SDK is .NET 10.0.402; the stakeholder-selected stack is ASP.NET Core, and the feature only needs two small HTTP operations. Microsoft's .NET 10 Minimal API guidance covers route mapping and read/create endpoint patterns.

**Alternatives considered**: ASP.NET Core MVC controllers would work but add ceremony for this small API. Choosing an older target framework is not justified by the current repository or stakeholder requirements.

**Sources**:
- [Create a minimal API with ASP.NET Core (.NET 10)](https://learn.microsoft.com/en-us/aspnet/core/tutorials/min-web-api?view=aspnetcore-10.0)

## In-memory state lifetime

**Decision**: Keep an ordered subscription collection in a singleton store registered in the API process. Protect mutations and list snapshots against concurrent requests. Treat process restart as the reset boundary.

**Rationale**: The scope is one local user and subscriptions exist only while the application is running. A singleton store preserves the data across HTTP requests without persistence or session infrastructure. This design is process-wide, not isolated by browser session; that matches the single-user local assumption. Multiple API processes would have independent lists and are outside this MVP.

**Alternatives considered**: ASP.NET Core Session would add cookies, session middleware, and cache configuration without a need for per-browser isolation. A database or distributed cache would violate the in-memory-only scope. A scoped service would not preserve a shared collection across requests.

**Source**:
- [Session and app state in ASP.NET Core (.NET 10)](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/app-state?view=aspnetcore-10.0) (reviewed as an alternative; not selected)

## Blazor API communication

**Decision**: Register a scoped `HttpClient` using the API base URL from frontend configuration. Load the list through GET and use the successful POST response to append the new entry to the displayed list.

**Rationale**: The backend remains the source of truth, while the POST response permits immediate UI feedback without a second request. Since the API and frontend use separate local origins, configure CORS to match the frontend launch origin and keep the API URL in configuration.

**Alternatives considered**: Keeping the list only in component state would lose API ownership and not exercise the specified architecture. Fetching the list again after each POST is valid but adds an unnecessary request.

**Sources**:
- [Call a web API from an ASP.NET Core Blazor app (.NET 10)](https://learn.microsoft.com/en-us/aspnet/core/blazor/call-web-api?view=aspnetcore-10.0)
- [Dependency injection in ASP.NET Core Blazor (.NET 10)](https://learn.microsoft.com/en-us/aspnet/core/blazor/fundamentals/dependency-injection?view=aspnetcore-10.0)

## API testing

**Decision**: Use xUnit tests for the store and ASP.NET Core integration tests with `Microsoft.AspNetCore.Mvc.Testing` / `WebApplicationFactory<Program>`. Isolate tests that assert an empty store by using a fresh test host.

**Rationale**: This tests the real HTTP contract and confirms that additions are visible on later list requests, while test-host isolation prevents singleton state leaking between cases. The UI/API flow is also verified in a browser using the quickstart, avoiding a new browser automation dependency for this MVP.

**Alternatives considered**: Unit tests alone would not verify HTTP routing, serialization, or API registration. Automated browser testing would cover more browser behavior but adds tooling and setup beyond the chosen minimal stack.

**Source**:
- [Integration tests in ASP.NET Core (.NET 10)](https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-10.0)
