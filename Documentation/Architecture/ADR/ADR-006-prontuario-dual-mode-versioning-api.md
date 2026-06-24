# ADR-006: Prontuario Dual-Mode Versioning API

## Status

Accepted (2026-06-19)

## Context

Prontuario is a versioned clinical snapshot aggregate. Legacy Google Sheets behavior used **in-place PUT** with no versioning. Domain documentation states that relevant clinical changes should create a new prontuario version, while operational reality also requires **corrections** to the current version (typos, administrative fixes) without advancing clinical history.

Prior Research explored a single-mode API (PUT always creates a successor row). The Prontuario SQL Stabilization SDD resolved **DQ-001** to explicit dual-mode operations:

- **Correction** — in-place update of the current version
- **Clinical evolution** — immutable successor snapshot

This is a **public API contract** change versus Legacy Sheets and the current stub SQL controller (string IDs, blind AutoMapper PUT). WS07 and future API consumers must align with this policy.

Related decisions already captured in feature SDD (not duplicated here as new architecture):

- Version numbering scoped to **`PacienteId`** (patient-wide lineage)
- Soft delete per version ([ADR-001](ADR-001-soft-delete.md))
- Application obtains `max(Versao)` from persistence; aggregate validates assigned `nextVersao` (Design **D-02**)

## Decision

Adopt a **dual-mode Prontuario versioning HTTP API** on the SQL branch:

| Operation | Route | Behavior | Success response |
|-----------|-------|----------|------------------|
| Create v1 | `POST /Prontuario` | First version for patient/atendimento context | **201** + read DTO |
| **Correction** | `PUT /Prontuario/{id}` | Update **correction-safe fields only** on the **same** row (`Id` and `Versao` unchanged) | **204** |
| **Clinical evolution** | `POST /Prontuario/{id}/versoes` | Insert **new** row: new `Id`, incremented patient-wide `Versao`, `ProntuarioAnteriorId` set; prior row unchanged | **201** + read DTO |
| Latest reads | `GET .../paciente/{id}/atual`, `GET .../atendimento/{id}/atual` | Max `Versao` among non-deleted for scope | **200** / **404** |
| Concurrent evolution | Same patient lineage | Unique index `(PacienteId, Versao)` collision | **409 Conflict** |

### Mandatory rules

1. The **client explicitly chooses** correction vs evolution via HTTP verb and route. The backend **must not infer** evolution from payload diffs alone.
2. **`UpdateProntuarioDto` exposes correction-safe fields only** (structural contract — Design **D-01**). Evolution-only fields appear only on create and `CreateVersaoProntuarioDto`.
3. Each evolved version is a **complete clinical snapshot** for supplied collections — **replace**, not merge from predecessor (Design **D-05**).
4. **PUT correction does not mutate child collections** (Exames, AcoesCD, Internacao) in MVP2; clinical or collection changes require POST `/versoes`.
5. **Version assignment:** Application layer queries persistence for `max(Versao)` per `PacienteId`; aggregate validates and assigns `nextVersao`; aggregate **does not** query the database (Design **D-02**).

### Breaking changes vs Legacy

| Legacy (Sheets) | ADR-006 SQL API |
|-----------------|-----------------|
| PUT always in-place | PUT = correction only (in-place) |
| No version increment | Evolution only via POST `/versoes` |
| No `/versoes` or `/atual` routes | Required routes for evolution and latest reads |

## Consequences

**Positive**

- Clear separation between administrative correction and clinical progression.
- Aligns with immutable snapshot domain intent for evolution paths.
- Durable policy for WS07, workflow SDDs, and agents — ADR overrides active SDD if conflict arises after acceptance.

**Negative**

- WS07 and any client assuming Legacy PUT semantics must be updated (documented follow-up, not part of backend stabilization Execute).
- Dual-mode API increases controller and verification surface versus single-verb CRUD.
- Patient-wide `Versao` may confuse atendimento-scoped UX (documented as **DQ-010** future consideration; out of scope for this ADR).

## Actions

| Horizon | Action |
|---------|--------|
| Execute | Implement per `Documentation/SDD/prontuario-sql-stabilization/` specify, design, tasks |
| WS07 | Align Blazor services with Guid, dual verbs, `/atual`, `Tipo` as int |
| Documentation Follow-Up | Optional qualification note in Domain Overview linking evolution vs correction |
| Verify | domain-review sensor; confirm D-01–D-06 in implementation |

## References

- [ADR-001: Soft Delete](ADR-001-soft-delete.md)
- [ADR-005: Legacy Sheets as Behavioral Reference](ADR-005-legacy-sheets-behavior-reference.md)
- `Documentation/SDD/prontuario-sql-stabilization/specify.md` — DQ-001, REQ-006, REQ-007
- `Documentation/SDD/prontuario-sql-stabilization/design.md` — D-01 through D-06, API Contract
- `DocAPI/Legacy/_LegacySheetsDb/ProntuarioSheetsRepository.cs` (behavioral reference only)
