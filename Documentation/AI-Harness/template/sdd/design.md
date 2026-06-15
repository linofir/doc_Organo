# Design - [Feature Name]

> Copy this template to `Documentation/SDD/<feature-slug>/design.md`.
> Input: `specify.md` in the same feature folder.
> Generated SDD artifacts should be written in English.

## Design Summary

Describe the proposed approach in one or two paragraphs.

## Requirement Mapping

| Requirement | Design response |
|-------------|-----------------|
| `REQ-001` | ... |

## Affected Components

| Component | Path | Responsibility | Change |
|-----------|------|----------------|--------|
| ... | `...` | ... | Add / Update / Remove / Verify |

## Reuse Analysis

Prefer existing patterns before introducing new abstractions.

| Existing code or pattern | Reuse decision | Notes |
|--------------------------|----------------|-------|
| ... | Reuse / Adapt / Do not reuse | ... |

## Architecture And Ownership

Describe bounded-context, aggregate, service, repository, controller, frontend, or documentation ownership implications.

- Architecture impact:
- Ownership boundaries:
- Conflicts or constraints:

## Data And Persistence

Use when entities, EF configuration, migrations, repositories, schema, or SQL behavior are touched.

- Entities:
- DTOs:
- EF mappings:
- Migration impact:
- SQL/runtime checks:

## API Contract

Use when controllers, routes, DTOs, IDs, request shape, response shape, or front compatibility are touched.

| Method | Route | Request | Response | Compatibility notes |
|--------|-------|---------|----------|---------------------|
| ... | ... | ... | ... | ... |

## Frontend Impact

Use when Blazor pages, services, state, mappers, UI flow, or smoke checks are touched.

- Pages/components:
- Services/state:
- UI behavior:
- Smoke checks:

## Security And PHI Design

Describe how the design avoids exposing patient identity, clinical data, files, logs, prompts, tests, auth assumptions, API exposure, or secrets.

- Security-sensitive: Yes / No
- Review prompt expected: Yes / No
- Mitigations:

## Legacy Behavior Decision

Use when behavior is compared with Legacy code.

| Behavior | Source | Preserve / Adapt / Abandon | Design rationale |
|----------|--------|----------------------------|------------------|
| ... | `DocAPI/Legacy/_LegacySheetsDb/...` | ... | ... |

## Testing Approach

Describe testability and expected implementation-time tests.

| Requirement | Test or check approach | Notes |
|-------------|------------------------|-------|
| `REQ-001` | ... | ... |

## Verification Handoff Notes

List expected verifier inputs. The Verifier will select final gates.

- Expected gates:
- Expected review sensors:
- Known skipped or manual checks:
- Evidence the implementation must provide:

## Risks

| Risk | Impact | Mitigation |
|------|--------|------------|
| ... | ... | ... |

## ADR Evaluation

| Question | Answer |
|----------|--------|
| Does this change affect durable architecture, persistence, schema lifecycle, security/auth, API contracts, ownership, runtime, or irreversible migration decisions? | Yes / No |
| Existing ADRs referenced | ... |
| New ADR candidate | None / ... |
| Decision needed before tasks or execution? | Yes / No |

## Documentation Follow-up Candidates

List possible follow-up. Documentation Update will route final ownership.

- State:
- ADR:
- Architecture docs:
- Technical docs:
- Rules:
- Skills:
- Review prompts:
- Templates:
