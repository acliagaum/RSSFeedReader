# Data Model: Add Feed Subscriptions

## Subscription

Represents one URL submission in the current API process.

| Field | Type | Required | Meaning |
|---|---|---:|---|
| `url` | string | Yes | The submitted feed URL, retained as entered and displayed as text |

### Collection behavior

- The API maintains an ordered list of `Subscription` entries in memory.
- A successful submission appends one entry. Duplicate URL strings remain separate entries and retain their submission order.
- The API process starts with an empty list. Restarting that process clears the list.
- There is no database, disk persistence, URL normalization, URL-format validation, feed retrieval, or feed parsing.
- The API requires a JSON request object with a string `url` property; this checks the request shape, not whether the value is a working RSS or Atom URL.

### Relationships and identity

There are no relationships or durable identifiers in this MVP. Each list position is a separate subscription entry, including when multiple entries contain the same URL.

## API mapping

- `GET /api/subscriptions` returns the ordered list as JSON.
- `POST /api/subscriptions` appends the request URL and returns the created entry as JSON.

See [subscriptions.openapi.yaml](./contracts/subscriptions.openapi.yaml) for request and response schemas.
