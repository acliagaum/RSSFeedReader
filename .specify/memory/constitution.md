<!--
Sync Impact Report
Version change: unversioned scaffold -> 1.0.0
Modified principles:
- Principle 1 placeholder -> I. Security and Safe Data Handling
- Principle 2 placeholder -> II. MVP Scope and Incremental Delivery
- Principle 3 placeholder -> III. Maintainable Separation of Concerns
- Principle 4 placeholder -> IV. Verifiable Quality
- Principle 5 placeholder -> V. Simplicity and Dependency Discipline
Added sections:
- Architecture and Scope Constraints
- Quality Gates and Development Workflow
Removed sections: None
Follow-up TODO: Record the original ratification date when known.
-->

# RSS Feed Reader Constitution

## Core Principles

### I. Security and Safe Data Handling
All changes MUST keep secrets out of source control and logs, handle user-provided values
as untrusted input at relevant boundaries, and encode feed-derived text before rendering
it as HTML. The MVP does not fetch feeds; any feature that introduces network fetching
MUST define and test protections against unsafe destinations and redirects before release.
This protects users without adding network machinery to the current scope.

### II. MVP Scope and Incremental Delivery
The MVP MUST support adding a feed URL and displaying the resulting subscription list.
It MUST NOT introduce feed fetching, parsing, persistence, removal, or background polling
unless a feature specification explicitly brings that capability into scope. Keep the MVP
in-memory and do not require URL validation while no network operation uses the URL.
This keeps delivery focused on the approved proof-of-concept.

### III. Maintainable Separation of Concerns
The backend MUST own subscription data and API behavior; the Blazor WebAssembly frontend
MUST own user interaction and presentation. New behavior MUST be placed in the layer
responsible for it and communicated through explicit API contracts. Configuration such
as the API base URL MUST be read from configuration rather than duplicated as source
code constants. This preserves clear ownership and supports incremental changes.

### IV. Verifiable Quality
Each behavior change MUST include or update automated tests at the narrowest meaningful
level. Changes spanning the API and UI MUST verify the end-to-end subscription flow:
adding a URL and seeing it in the displayed list. Before a change is considered complete,
the affected projects MUST build and relevant tests MUST pass; any unrun validation or
known failure MUST be reported. This makes correctness observable rather than assumed.

### V. Simplicity and Dependency Discipline
Implement only the simplest design that meets the approved requirements. Add packages,
services, storage systems, or background processes only when an in-scope requirement
needs them, and explain their purpose in the change. Prefer existing platform capabilities
and established project patterns over duplicate abstractions. This supports rapid,
understandable development without blocking future, explicitly scoped growth.

## Architecture and Scope Constraints

The application uses an ASP.NET Core Web API backend and a Blazor WebAssembly frontend.
For the MVP, the backend stores subscriptions in memory and exposes the operations needed
to add and list them; the frontend provides the subscription input and list. The API URL,
frontend origin, and backend CORS configuration MUST remain consistent for local
development. Cross-platform development on Windows, macOS, and Linux is a project goal;
new development instructions and tooling MUST avoid unnecessary operating-system
dependencies.

Feed fetching and parsing, persistence, subscription removal, and automatic polling are
future capabilities, not implied MVP requirements. Their addition requires an explicit
scope update and the corresponding security, data-handling, and quality requirements.

## Quality Gates and Development Workflow

Before implementation, confirm the change is within the approved feature scope and
identify its affected frontend, backend, API contract, and tests. Keep changes focused
and update directly related documentation when behavior, setup, or contracts change.

For validation, build each affected project and run its relevant tests. For changes to
frontend/backend communication, verify the configured API base URL and CORS origin and
test the subscription add-and-display flow. Review changes for input handling, safe
rendering, configuration exposure, unnecessary dependencies, and consistency with these
principles. Report validation results and any remaining limitations.

## Governance

This constitution governs project work. Feature specifications, implementation plans,
and code changes MUST remain consistent with it; a conflict requires an explicit
constitution amendment or a scope adjustment before implementation proceeds.

Amendments MUST document their rationale and impact on principles, architecture,
development workflow, and existing or planned features. The amendment MUST be reviewed
for consistency with the project goals and stakeholder requirements before adoption.

Constitution versions use semantic versioning: MAJOR for incompatible governance changes
or removal/redefinition of a principle, MINOR for a new principle or materially expanded
governance, and PATCH for clarifications or non-semantic wording changes. Every feature
change and review MUST check compliance with the applicable principles and report any
intentional exception with its justification.

**Version**: 1.0.0 | **Ratified**: TODO(RATIFICATION_DATE): Original adoption date is not recorded. | **Last Amended**: 2026-10-21
