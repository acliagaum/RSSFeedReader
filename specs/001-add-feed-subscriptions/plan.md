# Implementation Plan: Add Feed Subscriptions

**Branch**: `001-add-feed-subscriptions` | **Date**: 2026-10-21 | **Spec**: [spec.md](./spec.md)

**Input**: Feature specification plus [ProjectGoals.md](../../StakeholderDocuments/ProjectGoals.md) and [TechStack.md](../../StakeholderDocuments/TechStack.md).

## Summary

Implement the MVP subscription list with an ASP.NET Core Web API and Blazor WebAssembly UI. The API owns an ordered, process-local in-memory collection and provides add/list operations; the UI submits the entered text and immediately displays the returned subscription. Do not fetch or parse feeds, validate URL syntax, persist data, or deduplicate entries.

## Technical Context

**Language/Version**: C# 14 / .NET 10 (the repository environment has .NET SDK 10.0.402 installed)

**Primary Dependencies**: ASP.NET Core Minimal API, Blazor WebAssembly, built-in `HttpClient` and `System.Text.Json`; xUnit and `Microsoft.AspNetCore.Mvc.Testing` for API tests

**Storage**: In-memory, process-wide subscription store; no database or disk persistence

**Testing**: xUnit unit/integration tests for the API and store; browser-based manual end-to-end validation using the quickstart

**Target Platform**: Cross-platform .NET 10 backend and browser-based WebAssembly frontend; local development on Windows, macOS, and Linux

**Project Type**: Web application (ASP.NET Core API + Blazor WebAssembly)

**Performance Goals**: Each submitted subscription appears in the UI within one second; five additions, including duplicates, remain visible in order

**Constraints**: One local user; API store resets when its process restarts; no URL-format validation, feed network requests, parsing, persistence, deletion, or polling; frontend/API origins and configured base URL must agree for local CORS

**Scale/Scope**: One subscription view, one add/list API resource, and a small in-memory list for a single local user

## Constitution Check

### Pre-design gate

- **I. Security and Safe Data Handling — PASS**: Treat submitted text as untrusted, return it as JSON, and render it through ordinary Blazor text interpolation (not raw markup). No feed destinations are contacted.
- **II. MVP Scope and Incremental Delivery — PASS**: Only in-memory add/list behavior is planned; persistence, removal, fetching, parsing, and polling remain out of scope.
- **III. Maintainable Separation of Concerns — PASS**: The API owns subscription state and behavior; Blazor owns input and presentation; the API base URL comes from frontend configuration.
- **IV. Verifiable Quality — PASS**: Plan API unit/integration tests and a runnable browser scenario for adding and displaying subscriptions.
- **V. Simplicity and Dependency Discipline — PASS**: Use platform-provided API, JSON, and HTTP capabilities; add only the API integration-test package required for host-based tests.

No pre-design gate violations.

## Project Structure

### Documentation (this feature)

```text
specs/001-add-feed-subscriptions/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── subscriptions.openapi.yaml
└── tasks.md                 # Created by /speckit.tasks, not this plan
```

### Source Code (repository root)

```text
backend/
├── RSSFeedReader.Api/
│   ├── Contracts/
│   ├── Models/
│   ├── Properties/
│   ├── Services/
│   └── Program.cs
└── RSSFeedReader.Api.Tests/
frontend/
└── RSSFeedReader.UI/
    ├── Pages/
    ├── Properties/
    ├── wwwroot/
    └── Program.cs
```

**Structure Decision**: Use the backend/frontend split and project names described in the stakeholder TechStack document. Keep the store and subscription model in the API, expose request/response DTOs as explicit contracts, and make the Blazor root page the subscription view. When scaffolding the Blazor app, remove unused template demo pages/navigation and ensure only the subscription view owns the `/` route.

## Design Decisions

- Register the in-memory store as a singleton within the API process so data survives requests and is shared by the single local user, but is cleared on process restart.
- Preserve insertion order and append every submitted string, including duplicates; do not normalize or de-duplicate URLs.
- Use `GET /api/subscriptions` and `POST /api/subscriptions`. A successful POST returns the appended subscription so the UI can update immediately without a second request; initial page load uses GET.
- Configure the frontend `HttpClient` from `wwwroot/appsettings.json` and configure API CORS for the actual frontend launch origin.
- Verify that posted values are represented as encoded text in the UI. Do not use `MarkupString` or introduce feed fetching.

## Constitution Check (post-design)

- **I. Security and Safe Data Handling — PASS**: The contract transports text as JSON; the UI renders it as encoded text, and the planned API makes no outbound feed requests.
- **II. MVP Scope and Incremental Delivery — PASS**: The data model and endpoints do not introduce persistence or feed-management features beyond add/list.
- **III. Maintainable Separation of Concerns — PASS**: Backend store/API and frontend view remain separate, with configured API addressing and explicit JSON DTOs.
- **IV. Verifiable Quality — PASS**: Unit/integration tests cover storage and API behavior; the quickstart validates the cross-origin, end-to-end user flow.
- **V. Simplicity and Dependency Discipline — PASS**: No database, feed parser, background service, or browser-test dependency is required for this scope.

No post-design gate violations; no complexity exceptions are required.
