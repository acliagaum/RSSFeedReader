---

description: "Implementation tasks for Add Feed Subscriptions"
---

# Tasks: Add Feed Subscriptions

**Input**: Design documents from `/specs/001-add-feed-subscriptions/`

**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/subscriptions.openapi.yaml

**Organization**: Tasks are grouped by user story so the feature can be implemented and verified as an independent increment.

## Phase 1: Setup

**Purpose**: Create the .NET API, Blazor WebAssembly UI, and API test projects described in the plan.

- [X] T001 [P] Create the .NET 10 Minimal API project, including `backend\RSSFeedReader.Api\RSSFeedReader.Api.csproj`, `backend\RSSFeedReader.Api\Program.cs`, and `backend\RSSFeedReader.Api\Properties\launchSettings.json`; configure its local launch URL as `http://localhost:5151`.
- [X] T002 [P] Create the .NET 10 Blazor WebAssembly project and shell under `frontend\RSSFeedReader.UI\`, including `RSSFeedReader.UI.csproj`, `Program.cs`, and `wwwroot\appsettings.json`.
- [X] T003 Create `backend\RSSFeedReader.Api.Tests\RSSFeedReader.Api.Tests.csproj` with xUnit, `Microsoft.AspNetCore.Mvc.Testing`, and a project reference to `backend\RSSFeedReader.Api\RSSFeedReader.Api.csproj`.

---

## Phase 2: Foundational

**Purpose**: Configure API/UI connectivity required before the subscription flow can be implemented.

- [X] T004 Configure CORS in `backend\RSSFeedReader.Api\Program.cs` to allow the UI origin `http://localhost:5213` and apply the policy to API requests.
- [X] T005 Configure `frontend\RSSFeedReader.UI\Program.cs` to register an `HttpClient` using `ApiBaseUrl` from `frontend\RSSFeedReader.UI\wwwroot\appsettings.json`, with the local API address `http://localhost:5151/`.

**Checkpoint**: The API, UI, and test projects exist, and the UI can make cross-origin requests to the local API.

---

## Phase 3: User Story 1 - Add and View Feed Subscriptions (Priority: P1)

**Goal**: Let a user submit feed URL text and immediately see each submission in insertion order, including duplicates, for the lifetime of the API process.

**Independent Test**: Start the API and UI with an empty in-memory store, submit five values including a duplicate, and verify that every value appears separately and in submission order without leaving the page. Restart the API and verify the list is empty.

### Tests for User Story 1

- [X] T006 [P] [US1] Add `backend\RSSFeedReader.Api.Tests\Services\InMemorySubscriptionStoreTests.cs` covering an empty initial list, append/insertion order, duplicate retention, and independent store instances.
- [X] T007 [P] [US1] Add `backend\RSSFeedReader.Api.Tests\SubscriptionsApiTests.cs` using `WebApplicationFactory<Program>` to cover empty GET, POST response, subsequent ordered GET, duplicate entries, malformed request shape returning 400, and a fresh host starting empty.

### Implementation for User Story 1

- [X] T008 [P] [US1] Define the `Subscription` model in `backend\RSSFeedReader.Api\Models\Subscription.cs` with the submitted URL text as its value.
- [X] T009 [P] [US1] Define request and response JSON contracts in `backend\RSSFeedReader.Api\Contracts\CreateSubscriptionRequest.cs` and `backend\RSSFeedReader.Api\Contracts\SubscriptionResponse.cs`, matching the `url` properties in `specs\001-add-feed-subscriptions\contracts\subscriptions.openapi.yaml`.
- [X] T010 [US1] Implement `backend\RSSFeedReader.Api\Services\InMemorySubscriptionStore.cs` as a process-wide, thread-safe ordered store that appends each submitted string unchanged, retains duplicates, and returns safe list snapshots.
- [X] T011 [US1] Register the store as a singleton and map `GET /api/subscriptions` and `POST /api/subscriptions` in `backend\RSSFeedReader.Api\Program.cs`; return the ordered JSON list and the newly appended entry with the contract’s `200` status, and return `400` for a missing or non-string `url` without URL-format validation.
- [X] T012 [US1] Implement the root subscription view in `frontend\RSSFeedReader.UI\Pages\Home.razor`: load existing entries with GET, submit entered text with POST, append the successful response immediately, show loading/API errors, and render URL values as ordinary encoded text.
- [X] T013 [US1] Make the subscription view the sole `/` route and remove unused template demo pages and navigation links in `frontend\RSSFeedReader.UI\Pages\` and `frontend\RSSFeedReader.UI\Layout\NavMenu.razor`.

**Checkpoint**: The complete subscription add/list flow works independently; repeated submissions remain distinct and the API process restart clears the list.

---

## Phase 4: Polish & Cross-Cutting Concerns

**Purpose**: Validate the complete implementation and the documented local workflow.

- [X] T014 Run `dotnet test backend\RSSFeedReader.Api.Tests\RSSFeedReader.Api.Tests.csproj`, build the API and UI projects, and follow `specs\001-add-feed-subscriptions\quickstart.md` to verify the browser flow, CORS configuration, duplicate display, and reset after API restart.

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies; T001 and T002 can start in parallel. T003 depends on the API project from T001.
- **Foundational (Phase 2)**: Depends on the API and UI projects from Phase 1; T004 and T005 can proceed in parallel.
- **User Story 1 (Phase 3)**: Depends on the project and connectivity setup. Test tasks T006 and T007 can be developed in parallel. T008 and T009 can proceed in parallel; T010 depends on T008; T011 depends on T009 and T010; T012 depends on T005 and the API contract/endpoints; T013 follows T012's route setup.
- **Polish (Phase 4)**: Depends on completion of User Story 1.

### User Story Dependencies

- **User Story 1 (P1)**: Can start after the foundational phase; it has no dependency on another user story.

### Parallel Opportunities

- T001 and T002 initialize independent project directories.
- T004 and T005 configure the API and UI sides independently.
- T006 and T007 cover separate test files; T008 and T009 define separate model/contract files.
- After the foundational phase, backend storage/API work and frontend view work can proceed independently once their shared contracts are agreed.

## Parallel Example: User Story 1

```text
Work in parallel:
  T006 - Add store unit tests in backend\RSSFeedReader.Api.Tests\Services\InMemorySubscriptionStoreTests.cs
  T007 - Add API integration tests in backend\RSSFeedReader.Api.Tests\SubscriptionsApiTests.cs

Then in parallel:
  T008 - Define the model in backend\RSSFeedReader.Api\Models\Subscription.cs
  T009 - Define the API DTOs in backend\RSSFeedReader.Api\Contracts\
```

## Implementation Strategy

### MVP First (User Story 1)

1. Complete Phase 1 project setup.
2. Complete Phase 2 connectivity configuration.
3. Implement and validate Phase 3, User Story 1.
4. Run Phase 4 tests/builds and the browser quickstart; stop with this story as the MVP.

### Incremental Delivery

The specification contains one user story. Deliver its API and UI flow as one independently testable increment; defer persistence, feed retrieval/parsing, URL validation, deletion, and polling.
