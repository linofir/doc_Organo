---
paths:
  - "DocFront.Web/**/*"
---

# DocFront.Web — Blazor Server

## Stack

Blazor **Server** (`AddServerSideBlazor`) — not WebAssembly.

## Data flow

```
API → Service (HttpClient) → State (cache) → UI (Pages/Components)
```

- UI should not call `HttpClient` directly.
- Use Mappers for DTO ↔ ViewModel in State or Service consistently.

## API base URL

Configured in `appsettings.json` → `ApiSettings:BaseUrl` (default `https://localhost:7004`).

## Conventions

- Match API route names (`paciente`, `prontuario`, `agendamento`).
- Do not introduce new UI flows that depend on unstable API contracts.

## Additional Context

See:
`Documentation/Technical/front-architecture.md`