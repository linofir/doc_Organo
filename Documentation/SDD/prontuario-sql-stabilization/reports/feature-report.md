# Feature Report — Prontuario SQL Stabilization

> Active SDD: `Documentation/SDD/prontuario-sql-stabilization/`  
> Verification: [verification.md](../verification.md)  
> PM reference: WS01  
> Sizing: Large  
> Date: 2026-07-09

## Reporting Timing

Written after Documentation Follow-Up synchronized operational truth. State.md reflects final verified status as of 2026-07-09.

## Summary

Prontuario aggregate stabilized on SQL with a dual-mode versioning API (ADR-006). The slice delivers full nested-graph CRUD, correction-only PUT, clinical evolution via POST `/versoes`, soft delete per version, patient-wide version numbering, and PHI-safe controller logging — all backed by 79 passing tests (11 unit + 27 controller + 1 SQL integration). This is the third clinical SQL vertical confirmed, enabling Agendamento migration to proceed without versioning-related design debt.

## Scope

- **Feature:** Prontuario SQL Stabilization
- **Active SDD:** `Documentation/SDD/prontuario-sql-stabilization/`
- **PM item:** WS01
- **Included:** Backend stabilization of Prontuario aggregate domain methods, SQL repository with version lookup, Controller orchestration (FK/CID validation, correction vs evolution routing, 409 concurrency), dedicated DTOs with structural correction contract (D-01), read projection, unit tests, SQL integration tests, PHI remediation, Legacy characterization sign-off
- **Excluded:** Frontend (WS07 deferred), Atendimento re-implementation, Workflow evaluators (REQ-017), Agendamento repository, PDF import (501 only), CID catalog in production, Auth/RBAC (`AtualizadoPor`), new EF migrations, Financial features

## Requirements Delivered

| Requirement | Outcome | Evidence |
|-------------|---------|----------|
| REQ-001 | Delivered | Unit + integration tests: create v1 with Paciente identity snapshot, nested graph persistence |
| REQ-002 | Delivered | Controller tests: invalid/soft-deleted AtendimentoId → 404; PacienteId mismatch → 404 |
| REQ-003 | Delivered | Repository + controller tests: GetById with full nested graph, 404 for not-found |
| REQ-004 | Delivered | Repository tests: list ordered by Versao DESC; integration: 2 versions listed correctly |
| REQ-005 | Delivered | Repository + integration tests: `/atual` by PacienteId and AtendimentoId, both scopes |
| REQ-006 | Delivered | Repository tests: PUT correction preserves Id/Versao, updates correction-safe fields only (D-01) |
| REQ-007 | Delivered | Repository + integration tests: evolution increments Versao per PacienteId, ProntuarioAnteriorId set, V1 unchanged |
| REQ-008 | Delivered | Repository tests: soft delete excludes from reads, preserves occupied Versao slot in max lookup |
| REQ-009 | Delivered | Repository + integration tests: D-05 no-merge — V2 contains only evolution-supplied Exames, V1 unchanged |
| REQ-010 | Delivered | Controller tests: unknown CID → BadRequest on create and evolution; integration: synthetic CID positive path |
| REQ-011 | Delivered | All repository methods use Guid; dedicated DTOs; no domain entity types in public contracts; read-only AutoMapper |
| REQ-012 | Delivered | Code review: no Console.WriteLine in ProntuarioController; generic 404 messages; synthetic test data only |
| REQ-013 | Delivered | ProntuarioSqlIntegrationTests class exists; full fixture chain passes against Docker SQL |
| REQ-014 | Delivered | Legacy behavior table in specify.md reviewed against implementation; all decisions honored |
| REQ-015 | Delivered | Controller tests cover all status codes: 201, 204, 200, 404, 400, 409, 501 |
| REQ-016 | Delivered | Integration test: load by AtendimentoId with nested graph intact; EF includes verified |
| REQ-017 | Delivered | Scope boundary confirmed: no workflow evaluators, stage progression, or pendência writes |
| REQ-018 | Delivered | Tipo as int on all DTOs; controller asserts Tipo=0 on create, Tipo=1 on evolution |

## Implementation Areas

| Area | Summary |
|------|---------|
| Domain | `Prontuario` aggregate: `AplicarCriacao`, `AplicarCorrecao`, `CriarNovaVersao`, `MarcarComoExcluido`; `DescricaoBasica.AplicarCorrecao`; owned VOs (Exame, AcaoCD, Internacao, Procedimento). Aggregate validates `nextVersao` but does not query DB (D-02). |
| Persistence | `ProntuarioRepository`: full SQL CRUD, version lookup (`GetMaxVersaoForPacienteAsync` with `.IgnoreQueryFilters()` per ADR-001), EF includes for nested graph reads, soft delete per version, unique constraint `(PacienteId, Versao)` for 409 on concurrent evolution. `DocDbContext` entity configs unchanged from InitialCreate. |
| API | `ProntuarioController`: 10 endpoints, Guid routes, correction vs evolution routing per ADR-006, FK/CID validation, 409 concurrency, PHI-safe error messages. `POST /from-pdf` returns 501 immediately. |
| Frontend | No changes — WS07 deferred per Backend Stabilization Rule. |
| Documentation / Harness | `specify.md` Legacy Behavior table signed off; `design.md` Field Classification D-01–D-06 validated; ADR-006 accepted and honored; `migration-sql.md` Prontuario checklist complete; `State.md` updated with verification results. |

## Verification Summary

Per [verification.md](../verification.md):

- **Completion decision:** Verified with Minor Findings (2026-07-09)
- **Gates passed:** Build (0 errors), Automated Tests (79/0/0), SQL/Persistence (1 integration test), Security/PHI, Domain review, Documentation review, Test strategy review, ADR evaluation, Legacy characterization
- **Gates partially verified:** API (TASK-009 Swagger smoke pending — F-01, low severity)
- **Gates skipped:** UI (WS07 deferred), New EF migration (none required)
- **Review sensors applied:** security-phi-review, domain-review, test-strategy, check-docs
- **Residual risk:** Low — F-01 Swagger smoke not executed (27 controller tests + 1 SQL integration test provide strong mitigation); CID catalog absent in production (accepted per SDD scope); WS07 Guid + dual verb drift (Backend Stabilization Rule); `AtualizadoPor` unset (Auth out of scope); Workflow evaluators deferred (REQ-017)

## Documentation Follow-Up Summary

Updates applied during Documentation Follow-Up:

- **State.md:** Prontuario verified status, 79 tests (+52 from 27 baseline), all 18 REQs satisfied, ADR-006 honored, Agendamento next; next steps updated with TASK-009 Swagger smoke, Reporting, and Teacher Guide evaluation
- **migration-sql.md:** Prontuario (Aggregate #3) checklist marked complete with ADR-006, D-01, D-05 entries; migration sequence table updated with verification link
- **ADR:** ADR-006 accepted at Pre-Execution Review (2026-06-19) — no new ADR required; Domain Overview note on correction vs evolution deferred as optional
- **SDD sync:** No implementation divergence from specify.md or design.md — no sync needed

## Lessons Learned

- **SDD Pre-Execution Review durability:** The 19-day gap between Pre-Execution Review (2026-06-19) and Execute resumption (2026-07-08, after AI Harness Multi-Tool Evaluation) revealed no drift. The review checklist and Credential Probe captured sufficient context for a clean resume. This validates the Pre-Execution Review as an effective phase boundary for multi-session Large features.
- **D-02 reconciliation overhead:** The version generation authority conflict between `specify.md` ("aggregate owns version generation") and `design.md` D-02 ("Application layer obtains max, aggregate validates") required explicit reconciliation at Pre-Execution Review. Templates could be improved by flagging authority conflicts discovered during Design-to-Tasks transition earlier.
- **TASK-009 handoff gap:** Swagger smoke session notes were not recorded during Execute, leaving Verification without runtime HTTP evidence. Controller unit tests (27) provided strong mitigation, but the gap would have been more significant for a feature with fewer controller tests. The TASK-008/TASK-009 split (repository-direct integration vs HTTP smoke) works well but requires clearer Execute→Verify handoff expectations for the runtime validation task.
- **Third-vertical pattern confirmation:** The repository-direct integration test pattern (Paciente → Atendimento → Prontuario fixture chain) scaled cleanly to a third aggregate with nested children and versioning. The `SqlIntegrationTestGate` / `SqlConnectionResolver` infrastructure proved sufficient — no new abstractions needed. This confirms the pattern for Agendamento and future aggregates.
- **Dual-mode API governed by ADR:** ADR-006's correction vs evolution split prevented scope creep during implementation. The PUT/POST `/versoes` distinction was clear enough that no implementation divergence occurred. This reinforces ADR evaluation at Pre-Execution Review for non-trivial API design decisions.

## Teacher Guide

- **Decision:** Generate
- **Rationale:** Prontuario is the third clinical SQL vertical and the first with dual-mode versioning API (ADR-006), patient-wide version numbering, correction-vs-evolution semantics, nested child collection snapshot semantics (D-05), and concurrent evolution 409 handling. It crosses 3+ layers (domain, persistence, API, tests). The implementation patterns (aggregate factories, D-02 version generation split, D-01 structural correction contract, repository-direct integration tests with skip policy) are novel and worth capturing for Agendamento and future aggregates that may need versioning.

## Remaining Work

1. **TASK-009 Swagger smoke** — execute manual checklist per `specify.md` Runtime Validation Environment; record results (F-01 follow-up)
2. **Agendamento SQL Migration & Stabilization** — next vertical slice; Atendimento Minimal prerequisite satisfied
3. **Optional test debt:** Two-atendimento patient-wide Versao scenario (D-03); soft-deleted AtendimentoId → 404 on POST; `ProntuarioMappingTests` for read projection
4. **WS07 frontend alignment** — after Atendimento Workflow SDD
5. **CID Catalog Management** — future PM item