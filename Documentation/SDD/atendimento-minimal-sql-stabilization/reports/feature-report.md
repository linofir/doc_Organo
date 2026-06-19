# Feature Report - Atendimento Minimal SQL Stabilization

> Active SDD: `Documentation/SDD/atendimento-minimal-sql-stabilization/`  
> Verification: [verification.md](../verification.md)  
> Operational truth: [Documentation/State.md](../../../State.md)

## Summary

The Atendimento Minimal SQL backend vertical was stabilized as the second forward SDD in the approved migration sequence. The slice delivers persistence-only CRUD for the `Atendimento` aggregate root — Guid API contracts, slim DTOs, controller-level Paciente FK validation, soft delete (ADR-001), PHI-safe controller, repository unit tests, controller tests, SQL integration tests, and Verify-session HTTP smoke (TASK-008). All REQ-001 through REQ-010 were verified with accepted residual risk.

This unblocks Prontuario and Agendamento forward SDDs, which require a valid `AtendimentoId` FK. Workflow orchestration, frontend integration, and report implementation remain deferred to separate SDDs.

## Scope

- **Feature:** Atendimento Minimal SQL Stabilization
- **Active SDD:** `Documentation/SDD/atendimento-minimal-sql-stabilization/`
- **PM item:** WS01 — Persistência SQL e Infraestrutura (Atendimento Minimal slice)
- **SDD sizing:** Large
- **Included:** Backend domain factory/update/soft-delete methods; persistence-only `AtendimentoRepository`; Guid-aligned DTOs and mapping; `AtendimentoController` with Paciente FK validation, list-by-paciente route, 501 report routes preserved; unit and SQL integration tests; legacy characterization (REQ-010); mid-Execute harness credential alignment (`SqlConnectionResolver`, `.env.example`, scripts)
- **Excluded:** Workflow orchestration (`ValidacaoEtapa*`, pendências, clinical events); child entity persistence; Prontuario/Agendamento repositories; WS07 frontend; auth/`AtualizadoPor`; new EF migrations; report PDF implementation beyond 501 deferral; Google Sheets revival

## Requirements Delivered

| Requirement | Outcome | Evidence |
|-------------|---------|----------|
| `REQ-001` | Delivered | Factory sets `EtapaAtual = Consulta`; unit, SQL integration, HTTP POST 201 — [verification.md](../verification.md) § Requirement Evidence |
| `REQ-002` | Delivered | GetById with soft-delete exclusion; unit + HTTP GET 404 after delete |
| `REQ-003` | Delivered | `GetByPacienteIdAsync`; two-journey unit test; HTTP list count=2 |
| `REQ-004` | Delivered | `AtualizarMensagemParaMedico` only; EtapaAtual unchanged in tests; HTTP PUT 204 |
| `REQ-005` | Delivered | `MarcarComoExcluido()` per ADR-001; unit + SQL + HTTP exclusion |
| `REQ-006` | Partial — accepted | Invalid PacienteId → 404 tested; soft-deleted PacienteId not explicitly automated — [verification.md](../verification.md) § Findings |
| `REQ-007` | Delivered | Guid routes, DTOs, repository interface; code review + HTTP smoke |
| `REQ-008` | Delivered | `AtendimentoSqlIntegrationTests` passed on Verify re-run (Docker + credentials) |
| `REQ-009` | Delivered | No `Console.WriteLine` in controller; security-phi sensor OK |
| `REQ-010` | Delivered | Legacy preserve/adapt/abandon table in [specify.md](../specify.md) |

## Implementation Areas

| Area | Summary |
|------|---------|
| Domain | `Atendimento` factory (`PacienteId` required); `AtualizarMensagemParaMedico`; `MarcarComoExcluido`; initial `EtapaAtual = Consulta` |
| Persistence | `AtendimentoRepository` — CRUD, soft delete, `GetByPacienteIdAsync`; no FK or workflow logic in repository |
| API | `AtendimentoController` — Guid routes; aggregate factory on create; Paciente FK via `IPacienteRepository`; `GET /Atendimento/paciente/{pacienteId}`; report/followUp routes return 501 |
| Frontend | None — WS07 deferred |
| Documentation / Harness | SDD package (specify/design/tasks); [verification.md](../verification.md); [sdd-pilot-report-v0.6.md](../sdd-pilot-report-v0.6.md); Documentation Follow-Up synced State, PM, migration-sql, tasks; credential scripts (`SqlConnectionResolver`, `scripts/load-env.ps1`, `scripts/sql-integration-test.ps1`) |

**Primary code paths:** `DocAPI/Core/Entities/Atendimento.cs`, `DocAPI/Infrastructure/Repositories/AtendimentoRepository.cs`, `DocAPI/API/Controllers/AtendimentoController.cs`, `DocAPI.Tests/Infrastructure/AtendimentoRepositoryTests.cs`, `DocAPI.Tests/Controllers/AtendimentoControllerTests.cs`, `DocAPI.Tests/Integration/AtendimentoSqlIntegrationTests.cs`

## Verification Summary

Authoritative source: [verification.md](../verification.md)

- **Completion decision:** Complete with accepted residual risk
- **Gates passed:** Build; automated tests (27/27); SQL/persistence; API (HTTP smoke 17/17 + controller unit tests); security/PHI; domain review; test strategy (with suggestions); legacy characterization
- **Gates skipped or blocked:** UI (expected — WS07 deferred); documentation review partial at Verify time (closed in Documentation Follow-Up)
- **Review sensors applied:** domain-review, security-phi-review, check-docs, test-strategy
- **Residual risk:** Low — see verification.md; not re-classified here

## Documentation Follow-Up Summary

Documentation Follow-Up executed after Verify (2026-06-18). Operational truth synchronized:

- **State.md:** Atendimento Minimal backend verified; blockers updated (Prontuario/Agendamento prerequisite satisfied); verification status table; next step Prontuario SDD — [Documentation/State.md](../../../State.md)
- **PM:** WS01 Atendimento Minimal marked Verificado; Prontuario marked Próximo with prerequisite note — [Documentation/Product/PM_DocOrgano.md](../../../Product/PM_DocOrgano.md)
- **migration-sql.md:** Order #2 status Backend verified; Atendimento Minimal checklist complete; API contract section added — [Documentation/Technical/migration-sql.md](../../../Technical/migration-sql.md)
- **runbook.md:** Credential workflow via `.env` + `scripts/load-env.ps1`; `scripts/sql-integration-test.ps1` with Atendimento filter example — [Documentation/Technical/runbook.md](../../../Technical/runbook.md)
- **ADR:** None — ADR-001 referenced only
- **Architecture docs:** No changes required
- **SDD sync:** [tasks.md](../tasks.md) TASK-001 through TASK-009, VP-001, DF-001 marked complete
- **Rules / Skills / Review prompts / Templates:** No durable changes applied; calibration candidates captured in Lessons Learned below

## Lessons Learned

- **TASK-008 handoff gap is recurring (GOV-21)** — Execute completed implementation and automated tests but did not record structured Swagger session notes; Verify closed the gap with durable HTTP evidence in `verification.md`. Future `tasks.md` template should clarify Execute records intent while Verify owns durable runtime evidence.
- **Credential misalignment masked as environment failure (GOV-13–15)** — Integration tests skipped with misleading "Docker unreachable" when root cause was password mismatch across user-secrets, `.env`, and runbook placeholder. Mid-Execute harness fix (`SqlConnectionResolver`, canonical `.env` workflow) proved effective; Execution Prerequisites should include a credential probe.
- **Documentation drift lags verified implementation** — check-docs sensor flagged stale State and migration-sql during Verify; Documentation Follow-Up closed the gap. Prompt Follow-Up immediately after Verify completion decision.
- **Second aggregate pilot validates sequencing pattern** — Paciente patterns (persistence-only repository, soft delete, SQL integration class) reused with low friction; new patterns emerged: controller FK validation, list-by-paciente, multiple concurrent journeys, 501 route preservation.
- **PowerShell JSON BOM breaks API POST (GOV-22)** — curl smoke with `Set-Content -Encoding UTF8` produced invalid JSON; document UTF-8 no-BOM or add dedicated smoke script for future pilots.
- **Optional test debt is predictable** — no `AtendimentoMappingTests` (Paciente pilot has them); soft-deleted PacienteId → 404 not explicitly tested; acceptable residual risk but worth standardizing in forward SDDs.
- **Pilot report supersession works (GOV-23)** — v0.6 explicitly superseded Execute-only v0.5 when Verify completed; maintain this cadence for Prontuario pilot.
- **Reporting stays lightweight** — link to verification.md and State.md; do not duplicate gate tables or operational next steps.

Route durable harness changes through Documentation Update / Harness Calibration — not applied in this Reporting phase.

## Teacher Guide

- **Generated:** No — decision recorded for next phase
- **Decision:** **Generate**
- **Rationale:** Second SQL migration vertical (WS01 forward SDD); crosses domain, persistence, API, and test layers; introduces distinct patterns beyond Paciente (controller FK validation, list-by-paciente, multiple active journeys, 501 report deferral, credential resolver). Paciente [teacher-guide.md](../../paciente-sql-stabilization/teacher-guide.md) exists as baseline; a comparative or shortened Atendimento guide supports onboarding and third-aggregate (Prontuario) preparation.
- **Path (next phase):** `Documentation/SDD/atendimento-minimal-sql-stabilization/teacher-guide.md`
- **Eligibility hints:** Aggregate #2 in approved sequence; FK prerequisite pattern; harness credential alignment worth teaching
- **Code areas for extraction:** `Atendimento` entity factory; `AtendimentoRepository`; `AtendimentoController` FK validation; slim DTOs/`AtendimentoProfile`; SQL integration test round-trip; `SqlConnectionResolver`
- **Pitfall candidates:** FK validation belongs in controller not repository; do not expose stage advancement; report routes must remain (501); DTO namespace collision (`global::` workaround); credential source must be single canonical `.env`

## Remaining Work

- **Next feature:** Prontuario SQL Stabilization forward SDD (Specify → Design → Tasks → SDD Pre-Execution Review → Execute → Verify)
- **Optional test debt:** soft-deleted PacienteId → 404 on POST; `AtendimentoMappingTests`
- **Deferred scope:** Atendimento Workflow SDD (after Prontuario + Agendamento verified); WS07 frontend/API alignment; `AtualizadoPor` population (auth ADR); report PDF implementation
- **Harness calibration candidates:** Execution Prerequisites credential probe; TASK-008 handoff clarification; `sql-migration-workflow` skill update for resolver + scripts; integration test skip message improvement (auth vs network)
- **Implementation commit:** Code changes may remain uncommitted — check git status before merge
