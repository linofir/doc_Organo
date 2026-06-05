# ADR-004: Identity and Auth as a Supporting Capability for MVP2

## Status

Accepted for MVP2 planning

## Context

Doc Organo handles sensitive clinical data. Authentication and authorization are necessary for broader real-world use, but the MVP currently runs in a controlled environment.

The domain model is centered on Clinical, Support and future Financial contexts. Identity/Auth is not yet a product surface with its own domain language or lifecycle.

## Decision

Treat Identity/Auth as a supporting capability for MVP2, not as a separate bounded context.

Plan a minimal authentication and authorization posture that protects clinical data without introducing a large IAM design prematurely.

Revisit this decision if Doc Organo becomes multi-clinic, multi-tenant or requires user/permission management as a first-class product feature.

## Consequences

**Positive**

- Keeps MVP2 focused on clinical SQL migration and safety.
- Avoids overengineering Identity before requirements are clear.
- Allows authorization policies to be introduced around clinical use cases.

**Negative**

- Requires explicit documentation of controlled-environment assumptions.
- A future RBAC implementation may require refactoring policies and UI states.

## Actions

| Horizon | Action |
|---------|--------|
| Short | Remove PHI from logs and prompts |
| Short | Define minimum auth decision for MVP2 |
| Medium | Add auth-aware UI/API behavior where needed |
| Future | Implement advanced RBAC if user management becomes product scope |

## References

- `Documentation/Product/PM_DocOrgano.md`
- `Documentation/Product/RoadMap.md`
- `Documentation/Architecture/Architecture_Overview.md`
