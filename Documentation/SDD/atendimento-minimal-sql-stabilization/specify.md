# Specify — Atendimento Minimal SQL Stabilization

> Feature SDD: `Documentation/SDD/atendimento-minimal-sql-stabilization/`  
> Generated SDD artifacts are written in English.

## Context

Atendimento is the second aggregate in the approved SQL migration sequence. Prontuario and Agendamento both require a valid `AtendimentoId` FK before their repositories can persist. The minimal slice isolates persistence-only work: SQL CRUD for the `Atendimento` aggregate root so downstream verticals receive a real journey container without implementing workflow orchestration.

- PM item: WS01 — Persistência SQL e Infraestrutura (Atendimento Minimal slice)
- Product or domain source: `Documentation/Product/PRD.md`, `Documentation/Product/PM_DocOrgano.md`, `Documentation/Architecture/Domain_Overview_Business_Rules.md`
- Current operational source: `Documentation/State.md`
- Related SDD, ADR, technical, or architecture docs: `Documentation/Technical/migration-sql.md`, `Documentation/Architecture/ADR/ADR-001-soft-delete.md`, `Documentation/SDD/paciente-sql-stabilization/` (pilot reference), `Documentation/SDD/prontuario-sql-stabilization/research.md`, `Documentation/SDD/atendimento-workflow-stabilization/research.md`, `Documentation/AI-Harness/Harness-Design/sdd-operational.md`

## Problem

`AtendimentoRepository` is fully stubbed — every method throws `NotImplementedException`. `AtendimentoController` is wired but non-functional: routes use `string` IDs, DTOs carry abandoned Legacy denormalized fields, AutoMapper may drop `PacienteId` on create, and `Console.WriteLine` patterns risk PHI exposure.

Paciente SQL is verified upstream. Without Atendimento Minimal, Prontuario and Agendamento Execute remain blocked on `AtendimentoId` FK constraints. Workflow validations (`ValidacaoEtapa*`), pendências, and clinical events belong to a separate SDD and must not delay this prerequisite slice.

## Goals

- [ ] `REQ-001` — Create Atendimento via aggregate constructor/factory (`PacienteId` required; `MensagemParaMedico` optional); initial `EtapaAtual` is Consulta — client does not own other aggregate state
- [ ] `REQ-002` — Retrieve Atendimento by Guid id; soft-deleted rows excluded from default queries
- [ ] `REQ-003` — List Atendimentos by PacienteId; returns all non-deleted atendimentos for the patient (supports multiple concurrent journeys)
- [ ] `REQ-004` — Update Atendimento limited to safe root fields (`MensagemParaMedico` and audit metadata); no workflow-driven stage changes via API
- [ ] `REQ-005` — Soft delete Atendimento per ADR-001; deleted rows excluded from GetById, GetAll, and ListByPaciente
- [ ] `REQ-006` — Controller/Application layer validates Paciente FK on create (via `IPacienteRepository`); invalid or soft-deleted PacienteId returns 404; repository does not perform this validation
- [ ] `REQ-007` — Guid is the authoritative API and repository contract; legacy string IDs abandoned
- [ ] `REQ-008` — SQL integration tests **required**; validate repository CRUD against Docker SQL Server; passing run is success criterion
- [ ] `REQ-009` — PHI-safe controller: no `Console.WriteLine` or equivalent logging of patient-identifying data
- [ ] `REQ-010` — Legacy CRUD behavior characterized with explicit preserve/adapt/abandon decisions documented

## Out Of Scope

- Workflow orchestration (`ValidacaoEtapa*`, stage evaluators, pendência engine, journey projections)
- Workflow DTOs (stage status objects, pendência payloads, journey state projections)
- Client-driven `EtapaAtual` selection on create or modification on update
- API endpoints that advance journey stages (`AvancarEtapa` or equivalent)
- Pendência generation, `ClinicalEvent` writes, checklist execution persistence
- Child entity persistence (`AtendimentoPendencia`, `ClinicalEvent`, `ChecklistExecution`)
- Clinical journey orchestration, senhas integration, PDF report **implementation** (`report-id`, `followUp-id` return 501 — routes preserved)
- Prontuario and Agendamento repository implementation
- Auto-create Atendimento inside Prontuario or Agendamento create flows
- Google Sheets import or re-enablement
- WS07 Blazor integration, frontend validation, UI smoke, or any changes under `DocFront.Web/`
- Auth/RBAC implementation (`AtualizadoPor` may remain unset; accepted residual risk)
- New EF migrations (default: none — `InitialCreate` remains source of truth)
- Financial features
- Exposing `AvancarEtapa()` or stage progression via API
- Verification artifact creation (`verification.md`) — owned by Verify phase
- Feature report or session-handoff creation — owned by reporting workflow after Verify

## Users And Scenarios

| Actor | Scenario | Outcome |
|-------|----------|---------|
| Developer / agent | Create Atendimento for verified Paciente via API or repository | Valid `AtendimentoId` available for downstream FK |
| Developer / agent | List atendimentos by PacienteId | Collection returned for explicit `AtendimentoId` selection in Prontuario/Agendamento |
| Developer / agent | Run `dotnet test` with unit and SQL integration tests | All Atendimento backend tests pass; SQL integration passing run expected when Docker available |
| Developer / agent | Exercise Atendimento endpoints via Swagger against Docker SQL | CRUD and list-by-paciente return documented status codes |
| Verifier | Review test output, Swagger checklist, legacy table, SDD traceability | Backend slice meets acceptance criteria with recorded evidence |
| Prontuario / Agendamento SDD implementer | Consume stabilized `AtendimentoId` FK | Can proceed Execute without stub dependency |

## Acceptance Criteria

- [ ] `REQ-001` — POST with valid `pacienteId` creates row via aggregate factory with `EtapaAtual = Consulta`, `CriadoEm` set; create DTO has no workflow or stage fields; verified by unit and integration tests
- [ ] `REQ-002` — GET by id returns 200 + `ReadAtendimentoDto` for existing non-deleted row; 404 for missing or soft-deleted; verified by tests and Swagger
- [ ] `REQ-003` — GET list-by-paciente returns 200 + array of non-deleted atendimentos; empty array when none exist; two active atendimentos for same patient supported; verified by tests
- [ ] `REQ-004` — PUT updates `MensagemParaMedico` only; does not accept client-driven `EtapaAtual` changes; returns 204 or 404; verified by tests
- [ ] `REQ-005` — DELETE soft-deletes row; subsequent GetById and ListByPaciente exclude it; verified by tests
- [ ] `REQ-006` — POST with non-existent or soft-deleted `pacienteId` returns 404; validation performed in Controller/Application layer before repository create; verified by negative tests
- [ ] `REQ-007` — All route parameters and DTO id fields use `Guid`; repository interface uses `Guid`; verified by Swagger and code review
- [ ] `REQ-008` — SQL integration test class **required**; exercises Atendimento CRUD round-trip (Paciente → Atendimento) against Docker SQL; passing run is the success criterion — skip permitted only for documented operational/environment blockers and is **not** expected success behavior
- [ ] `REQ-009` — No PHI in touched `AtendimentoController` code paths
- [ ] `REQ-010` — Legacy behavior table in this document is complete with preserve/adapt/abandon decisions

Frontend compatibility is not a success criterion for this feature.

## Execution Prerequisites

Execute must not begin until all of the following are confirmed:

| Prerequisite | How to confirm |
|--------------|----------------|
| Paciente SQL slice verified | Paciente repository functional; `Documentation/SDD/paciente-sql-stabilization/` Verify complete or State confirms backend verified |
| Docker SQL container running | `docker compose up -d`; container `docorgano-sql` healthy on port 1433 |
| `SA_PASSWORD` environment variable set | Required for SQL Server container and local connection |
| Database migrated | `dotnet ef database update --project DocAPI/DocAPI.csproj` succeeds |
| API starts successfully | `dotnet run --project DocAPI/DocAPI.csproj` starts without startup errors |
| Baseline tests executed | `dotnet test` run once before changes; baseline pass/fail/skip recorded in session notes |
| Swagger available | Swagger UI reachable at DocAPI dev URL (default `https://localhost:7004/swagger` or configured launch URL) |

If any prerequisite fails, stop Execute and resolve infrastructure before implementation tasks.

## Runtime Validation Environment

### Environment

| Component | Requirement |
|-----------|-------------|
| Docker SQL | `docorgano-sql` with migrated schema from `InitialCreate` |
| Local API | DocAPI running against `DefaultConnection` in `DocAPI/appsettings.json` |
| Swagger | DocAPI Swagger UI — DocFront.Web not required for this feature |
| Upstream Paciente | At least one synthetic Paciente row for FK validation scenarios |

### Manual validation checklist

Execute performs these checks via Swagger (or equivalent API client) and records results in session notes. Formal verification evidence is produced in the Verify phase.

| Scenario | Endpoint pattern | Expected outcome |
|----------|------------------|------------------|
| Create | POST `/Atendimento` | 201 + `ReadAtendimentoDto`; `etapaAtual` is Consulta |
| Read by ID | GET `/Atendimento/{id}` | 200 for existing; 404 for missing or soft-deleted |
| List by Paciente | GET `/Atendimento/paciente/{pacienteId}` | 200 + array; may include multiple active journeys |
| Paginated list | GET `/Atendimento?skip=&take=` | 200 + array |
| Update | PUT `/Atendimento/{id}` | 204 for existing; 404 for missing |
| Delete | DELETE `/Atendimento/{id}` | 204 soft delete; 404 for missing |
| Invalid PacienteId | POST with unknown `pacienteId` | 404 |
| Soft delete exclusion | GET after delete | Deleted atendimento not returned by id or list-by-paciente |
| Report routes | GET `/Atendimento/report-id/{id}`, GET `/Atendimento/followUp-id/{id}` | 501 Not Implemented — routes **remain present**; no partial PDF/followUp implementation |

## Backend Stabilization

- Frontend validation in scope: **No**
- Default for backend stabilization: exclude Blazor smoke and UI contract work
- Frontend/API drift follow-up: WS07 after all four aggregate backends are SQL-stable

## Sizing

| Field | Decision |
|-------|----------|
| Size | **Large** |
| Rationale | Re-evaluated from Medium: aggregate code surface is smaller than Paciente, but harness and verification burden matches a backend stabilization vertical — full stub-to-SQL repository implementation, Guid/PHI/DTO contract migration, Controller/Application FK validation, required SQL integration tests, security-PHI review, legacy characterization, and SDD Pre-Execution Review. Aligns with Paciente pilot sizing model (Large = backend vertical stabilization, not raw line count). |
| Required phases | Specify / Design / Tasks / SDD Pre-Execution Review / Execute / Verify |
| Escalation triggers | Schema migration required; child entity persistence pulled in; workflow validations pulled in; report endpoints implemented beyond 501 deferral |

## Layer Ownership

### Repository (persistence-only)

- `AtendimentoRepository` performs SQL CRUD against `DocDbContext` only
- Repository does **not** own business validation (including Paciente FK checks)
- Repository does **not** own workflow logic, stage evaluation, or cross-aggregate orchestration
- Repository does **not** read Prontuario, Agendamento, or other downstream aggregates

### Controller / Application

- Paciente FK validation on create is owned by Controller or Application layer (lookup via `IPacienteRepository`)
- HTTP status mapping, PHI-safe handling, and 501 report-route deferral are Controller responsibilities
- Aggregate construction uses entity constructor/factory — not blind AutoMapper from DTO

### Aggregate (domain state)

- `Atendimento` constructor/factory owns initial aggregate state (`EtapaAtual = Consulta`, `CriadoEm`)
- Create contract: `PacienteId` required; `MensagemParaMedico` optional — client does not own other aggregate state fields
- `EtapaAtual` is **workflow-owned** after create; minimal slice must not expose create-time stage selection, update-time stage modification, or stage-advance endpoints

## Assumptions And Constraints

- ADR-001 soft delete is authoritative; global query filter on `Atendimento` already configured in `DocDbContext`
- Paciente SQL is verified upstream — valid `PacienteId` FK source exists
- Legacy Google Sheets code is behavioral reference only for CRUD surface
- No Google Sheets re-enablement on the SQL branch
- PHI must not appear in logs, tests, commits, or documentation examples
- SQL runtime uses Docker `docorgano-sql` with `SA_PASSWORD` environment variable
- Approved migration sequence: Paciente (verified) → **Atendimento Minimal** → Prontuario → Agendamento → Atendimento Workflow → WS07
- Test fixtures use synthetic data only (no real patient names or clinical data)
- Schema from `InitialCreate` migration is stable; no new migrations expected
- Multiple concurrent active atendimentos per patient are allowed (no uniqueness constraint)
- Prontuario and Agendamento callers must supply explicit `AtendimentoId` — not inferred from patient alone

## Open Questions

1. **POST response shape** — Return `ReadAtendimentoDto` in body (Paciente pattern) or Location header only?  
   **Resolved for Execute:** Return 201 + `ReadAtendimentoDto` per Paciente pilot and Research API Discovery.

2. **ListByPaciente filter** — Return all atendimentos or only "active" journeys?  
   **Resolved for Execute:** Return all **non-deleted** atendimentos (global query filter applies). Multiple concurrent journeys per patient are supported; no "close previous journey" logic in minimal slice.

## Domain Language

Terms match `Documentation/Architecture/Domain_Overview_Business_Rules.md`:

- **Atendimento** — Care journey container linked to a Paciente; aggregate root for journey identity in this slice
- **EtapaAtual** — Current journey stage (`EtapaAtendimento` enum); set to Consulta by aggregate factory on create only; **workflow-owned** thereafter — not client-writable and not modifiable via minimal-slice API
- **Paciente** — Upstream aggregate; FK required on Atendimento create
- **Soft delete** — `Deletado` / `DeletadoEm` with global query filter per ADR-001
- **Care journey** — 1:N Paciente-to-Atendimento; multiple concurrent active journeys allowed
- **Prontuario / Agendamento** — Downstream aggregates requiring `AtendimentoId` FK; out of scope for implementation but acknowledged as consumers

## Legacy Behavior

| Behavior | Source | Decision | Notes |
|----------|--------|----------|-------|
| `GetAllAsync(skip, take)` | `AtendimentoSheetsRepository` | Preserve | Paginated list |
| `GetByIdAsync` | Same | Preserve | Guid PK (adapt from string) |
| `CreateAsync` | Same | Preserve | Requires valid Paciente |
| `UpdateAsync` | Same | Adapt | Slim field set; no stage orchestration |
| `DeleteAsync` | Same | Adapt | Soft delete per ADR-001 |
| List/filter by PacienteId | Implied by multi-journey + Prontuario UX | Preserve | New explicit API route |
| `CreateReportByIdAsync` | Same | Adapt | Route preserved; returns 501 until workflow/report feature |
| `CreateReportFollwUpByIdAsync` | Same | Adapt | Route preserved; returns 501 until workflow/report feature |
| `ValidacaoEtapa*` methods | Same | Separate SDD | `atendimento-workflow-stabilization` |
| Nested stage objects (`EtapaConsulta`, etc.) | Legacy DTO shape | Abandon | Not recreated |
| Denormalized `ProntuariosId` / `AgendamentosId` on DTO | Legacy DTO | Abandon | Not persisted |
| `NomePaciente` on create DTO | Legacy DTO | Abandon | Paciente is FK source of truth |
| Client-set `EtapaAtualAtendimento` on create | Legacy DTO | Abandon | Factory sets Consulta |
| String IDs | Legacy mapping | Abandon | SQL uses Guid |
| PHI `Console.WriteLine` in controller | `AtendimentoController` | Abandon | Remediate per Paciente pattern |

## Security And PHI

- Security/PHI review needed: **Yes**
- Rationale: Feature touches patient linkage (`PacienteId`), API exposure, controller logging remediation, and test fixtures. Controller currently uses `Console.WriteLine` and error paths may expose identifiers.

## Initial Verification Expectations

| Requirement | Expected evidence |
|-------------|-------------------|
| `REQ-001` | Unit test create; SQL integration create; Swagger POST 201 |
| `REQ-002` | Unit test GetById; negative test missing id |
| `REQ-003` | Unit test ListByPaciente; two-atendimentos-same-patient case |
| `REQ-004` | Unit test update message; assert EtapaAtual unchanged |
| `REQ-005` | Unit test soft delete exclusion |
| `REQ-006` | Negative test invalid PacienteId → 404 |
| `REQ-007` | Code review; Swagger Guid route validation |
| `REQ-008` | **Passing** `dotnet test` output for SQL integration class against Docker SQL; skip only with documented operational blocker — skip is residual risk, not success |
| `REQ-009` | security-phi-review sensor; no PHI in controller |
| `REQ-010` | Legacy behavior table in this document reviewed and accepted |

## References

- `Documentation/State.md`
- `AGENTS.md`
- `Documentation/Product/PM_DocOrgano.md`
- `Documentation/Product/PRD.md`
- `Documentation/Technical/migration-sql.md`
- `Documentation/Architecture/ADR/ADR-001-soft-delete.md`
- `Documentation/Architecture/Domain_Overview_Business_Rules.md`
- `Documentation/SDD/atendimento-minimal-sql-stabilization/research.md`
- `Documentation/SDD/paciente-sql-stabilization/`
- `Documentation/SDD/prontuario-sql-stabilization/research.md`
- `Documentation/SDD/atendimento-workflow-stabilization/research.md`
- `Documentation/AI-Harness/Harness-Design/sdd-operational.md`
- `Documentation/AI-Harness/Harness-Design/verification-governance.md`
