# Design - [Feature Name]

> Copy this template to `Documentation/SDD/<feature-slug>/design.md`.
> Input: `specify.md` in the same feature folder.
> Generated SDD artifacts should be written in English.

## Design Summary

Describe the proposed approach in one or two paragraphs.

**Context budget for Design is Medium.** Load architecture, ADRs, and integration points — not full codebase. See `sdd-operational.md` § Context Acquisition Governance.

**Outcome-oriented design:** State what must be true after implementation — behaviors, contracts, persistence outcomes, and constraints. Avoid prescribing specific classes, methods, factories, or mapping strategies unless an accepted ADR requires them. Execute owns implementation choices that satisfy these outcomes.

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
- **DTO / entity namespace check:** When a DTO class name matches or shadows an entity name (e.g. `Atendimento` in `DocAPI.Application.Data.Dtos.Atendimento`), confirm AutoMapper and read DTOs use explicit namespaces or `global::` qualifiers before Execute.
- EF mappings:
- Migration impact:
- SQL/runtime checks:

## API Contract

Use when controllers, routes, DTOs, IDs, request shape, response shape, or front compatibility are touched.

**Contract resolution:** Ambiguous routes, semantic contradictions, or unresolved naming decisions must be resolved during SDD Pre-Execution Review before Execute. Do not defer contract decisions to implementation.

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

## Runtime Validation Environment

Document the environment and manual checks needed to produce auditable runtime evidence during Execute and Verify.

| Check | Environment | Expected result | Evidence location |
|-------|-------------|-----------------|-------------------|
| Swagger smoke | DocAPI running locally | ... | **Verify owns durable evidence** in `verification.md` § Runtime Validation |
| SQL integration | Docker + credentials | ... | test output |

**Runtime validation ownership:**

- **Execute:** Confirm environment ready; run manual or scripted checks if time permits; record intent in session notes (not required to duplicate full evidence tables).
- **Verify:** Owns durable HTTP/runtime evidence in `verification.md` when API or manual smoke gates apply. If Execute did not record smoke notes, Verify runs equivalent checks and records the table.

## Optional Test Debt (Forward Migration Verticals)

Document acceptable gaps explicitly when residual risk is accepted during SDD Pre-Execution Review.

| Optional test | When to include | Default for backend stabilization |
|---------------|-----------------|-------------------------------------|
| AutoMapper mapping tests (`*MappingTests`) | DTO ↔ entity mapping is non-trivial | Recommended when multiple DTOs or owned types |
| Soft-deleted parent FK negative test | Create depends on parent FK | Recommended when parent uses global soft-delete filter |
| Controller HTTP integration tests | Critical status codes beyond unit mocks | Optional when Verify HTTP smoke covers routes |

## Fixture Chain (optional — for composite aggregates)

When the aggregate depends on prerequisite entities for integration tests, document the ordered creation chain. Prevents ad hoc fixture design during Execute. Mark N/A for simple aggregates without nested prerequisites.

| Step | Entity | Prerequisite | Notes |
|------|--------|-------------|-------|
| 1 | `Paciente` | None | Required for FK |
| 2 | `Atendimento` | `Paciente` | Validates `PacienteId` |
| 3 | `<Aggregate>` v1 | `Paciente`, `Atendimento` | Initial create |
| 4 | Correction | v1 | PUT in-place |
| 5 | Evolution | v1 | POST `/versoes` v2 |
| ... | ... | ... | ... |

Prontuario SQL Stabilization is the first exemplar: full chain Paciente → Atendimento → Prontuario v1 → correction → evolution → v2 covered in a single integration test.

## Testing Approach

Describe testability and expected implementation-time tests.

| Requirement | Test or check approach | Notes |
|-------------|------------------------|-------|
| `REQ-001` | ... | ... |

## Architectural Capabilities And Expected Test Suites

Identify which Architectural Capabilities are affected by this feature and which test suites are expected during Execute. See `test-governance.md` for capability definitions and test suite ownership.

| Architectural Capability | Expected test suite | Rationale |
|--------------------------|--------------------|-----------|
| Persistence Intent | Repository Tests | [e.g., New aggregate with versioning semantics] |
| Physical Persistence | SQL Integration Tests | [e.g., Schema changes, migration] |
| API Contract | Controller Tests | [e.g., New endpoints, DTO changes] |

For each Architectural Capability that is NOT expected, state why:

| Architectural Capability | Not expected because |
|--------------------------|---------------------|
| [e.g., Business Invariants] | [e.g., No new domain invariants — existing aggregate invariants already covered] |

### Architectural Decision To Test Scenario Mapping

Map architectural decisions from this design to expected repository test scenarios. This satisfies AD-TG-011: repository test scenarios shall be derived from SDD architectural decisions.

| Architectural decision | Expected repository test scenario(s) |
|------------------------|--------------------------------------|
| [e.g., Version immutability — created versions never mutate] | [e.g., Update after version creation preserves all fields] |
| [e.g., Soft delete excludes from reads but preserves for version queries] | [e.g., SoftDelete excludes from GetById; SoftDelete preserves version for MaxVersao] |

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
