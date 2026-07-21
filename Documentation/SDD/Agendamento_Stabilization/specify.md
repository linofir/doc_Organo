# Specify — Agendamento SQL Migration & Stabilization

> Feature SDD: `Documentation/SDD/Agendamento_Stabilization/`
> Generated SDD artifacts are written in English.
> **Template source:** `Documentation/AI-Harness/template/sdd/specify.md`

## Context

Agendamento is the fourth aggregate in the approved SQL migration sequence. Paciente, Atendimento Minimal, and Prontuario backends are verified. Agendamento controls administrative planning of medical procedures: scheduling, location, authorization codes (senhas), and procedural status tracking. It is the final blocking prerequisite before Atendimento Workflow can port ~700 LOC of Legacy journey orchestration.

- PM item: **WS01** — Agendamento SQL Migration & Stabilization (status: Planejado)
- Product or domain source: `Documentation/Product/PRD.md`, `Documentation/Product/PM_DocOrgano.md`, `Documentation/Architecture/Domain_Overview_Business_Rules.md`
- Current operational source: `Documentation/State.md`
- Related SDD, ADR, technical, or architecture docs:
  - `Documentation/Technical/migration-sql.md`
  - `Documentation/Architecture/ADR/ADR-001-soft-delete.md`
  - `Documentation/SDD/paciente-sql-stabilization/` (verified — owned-type pattern reference)
  - `Documentation/SDD/atendimento-minimal-sql-stabilization/` (verified — pattern reference for stub→SQL, Guid, FK validation)
  - `Documentation/SDD/prontuario-sql-stabilization/` (verified — Internacao FK validation pattern, dual-FK precedent)
  - `Documentation/SDD/Agendamento_Stabilization/research.md` (Parts 1 + 2 — 6 decision candidates resolved)
  - `Documentation/AI-Harness/Harness-Design/sdd-operational.md`

**Context budget for Specify is Medium.** Load product intent, domain rules, and scope boundaries — not implementation details. See `sdd-operational.md` § Context Acquisition Governance.

## Problem

`AgendamentoRepository` is fully stubbed — every method throws `NotImplementedException`. `AgendamentoController` is wired but non-functional: routes use `string` IDs, `IAgendamentoRepository` contract uses `string` IDs (not aligned with established `Guid` pattern), `Console.WriteLine` patterns risk PHI exposure, and there is no FK validation on create.

The entity model (`Agendamento.cs`) is misaligned with the `InitialCreate` migration schema:
- **Missing:** `PacienteId` (Guid?) — column exists in schema with FK constraint; entity lacks the property
- **Absent from schema:** `Procedimento` string — DTOs carry it but no column exists; behavior should derive from `Internacao → ProcedimentoInternacao`
- DTOs use `string` types for `StatusInstrucoes`/`StatusAtestado` while the entity uses enums with string conversion

Paciente, Atendimento Minimal, and Prontuario are verified upstream. Without Agendamento, the Atendimento Workflow SDD remains blocked — its Legacy `ValidacaoPreProcedimento` and `ValidacaoEtapaProcedimento` methods depend on SQL-persisted Agendamento data.

## Goals

- [ ] `REQ-001` — Entity model aligned with migration schema: add `PacienteId` (Guid?, nullable), add `Paciente` navigation property, remove any `Procedimento`-related entity state (column does not exist); preserve enums for `Status`, `InstrucaoStatus`, `AtestadoStatus` with `HasConversion<string>()`
- [ ] `REQ-002` — `IAgendamentoRepository` contract uses `Guid` IDs exclusively; `string` ID signatures replaced; alignment with Paciente/Atendimento/Prontuario pattern
- [ ] `REQ-003` — `AgendamentoRepository` SQL implementation: full CRUD (`CreateAsync`, `GetByIdAsync`, `UpdateAsync`, `DeleteAsync`) plus `GetAllAsync(skip, take)`, `GetByNameAsync`, `GetByPacienteIdAsync(Guid)`
- [ ] `REQ-004` — EF configuration (`AgendamentoConfiguration`) updated: add `PacienteId` FK + `Paciente` navigation mapping; confirm owned-type `SenhaAgendamento`, enum conversions, and existing indexes
- [ ] `REQ-005` — `AgendamentoController` hardened: all route/id parameters use `Guid`; `Console.WriteLine` PHI-bearing calls removed; proper HTTP status codes (200, 201, 204, 400, 404)
- [ ] `REQ-006` — Create with FK validation: `AtendimentoId` (required) validates against `IAtendimentoRepository`; `InternacaoId` (required) validates via `_context.Set<Internacao>()`; `PacienteId` (optional) validates against `IPacienteRepository` when provided; invalid or soft-deleted FK targets return 404
- [ ] `REQ-007` — Read endpoints: `GET /Agendamento?skip=&take=` (200 + paginated), `GET /Agendamento/{id:guid}` (200/404), `GET /Agendamento/by-name?nome=` (200/404, partial match), `GET /Agendamento/by-pacientId?pacienteId={guid}` (200/404)
- [ ] `REQ-008` — Update limited to mutable fields via dedicated update method on entity; FK targets (`InternacaoId`, `AtendimentoId`, `PacienteId`) and `ID` are immutable after create
- [ ] `REQ-009` — Soft delete per ADR-001 via entity method; deleted rows excluded from all default queries (global query filter); `DeletadoEm` set
- [ ] `REQ-010` — Dedicated API DTOs aligned with entity: `CreateAgendamentoDto` (no `Procedimento`; no `Status` — factory sets `SemSenha`; `PacienteId` Guid?; `StatusInstrucoes`/`StatusAtestado` as enums), `ReadAgendamentoDto` (Guid ID; Guid? PacienteId; enums; no `Procedimento`), `UpdateAgendamentoDto` (mutable scalar fields only; no FK or ID fields)
- [ ] `REQ-011` — Domain methods on `Agendamento` entity: aggregate factory (sets `Status = SemSenha`, `CriadoEm`), `SoftDelete()`, update helper for mutable fields including owned-type `SenhaAgendamento` mutation via a dedicated domain method
- [ ] `REQ-012` — Unit tests covering entity invariants, repository CRUD, and controller behavior; SQL integration tests exercising full CRUD round-trip against Docker SQL Server with FK validation; passing run is success criterion
- [ ] `REQ-013` — Legacy CRUD behavior characterized with explicit preserve/adapt/abandon decisions documented in this specification

## Out Of Scope

- `ValidacaoPreProcedimento`, `ValidacaoEtapaProcedimento` — owned by `atendimento-workflow-stabilization`
- Integration with `CollectSenhasAutorizadasDataService` (WS06)
- PDF report generation with Agendamento data
- WS07 Blazor frontend alignment (enum drift, Guid contract migration, `Procedimento` field removal, UI smoke)
- New EF migrations (default: none — `InitialCreate` is source of truth)
- Google Sheets import or re-enablement
- Auth/RBAC implementation (`AtualizadoPor` may remain unset; accepted residual risk)
- Atendimento Workflow stage advancement via Agendamento status (owned by workflow SDD)
- `InternacaoRepository` creation — Internacao is accessed via `_context.Set<Internacao>()` for FK validation only
- Internacao aggregate management — Internacao is a child of Prontuario; Agendamento only validates FK existence
- Financial features
- `Procedimento` field restoration on entity or DTOs (decision: abandon per Research DC-2)
- `GET /Agendamento/by-filter` generic filter endpoint (commented in controller; not activated)
- Verification artifact creation (`verification.md`) — owned by Verify phase
- Feature report or session-handoff creation — owned by reporting workflow after Verify

## Users And Scenarios

| Actor | Scenario | Outcome |
|-------|----------|---------|
| Developer / agent | Create Agendamento for verified Atendimento + Internacao via API | Valid `AgendamentoId` available; `Status = SemSenha` |
| Developer / agent | Create Agendamento with optional PacienteId | Row persisted; nullable FK validated when provided |
| Developer / agent | List agendamentos paginated or by PacienteId | Collection returned for scheduling views |
| Developer / agent | Search agendamentos by patient name (partial match) | Matching rows returned; empty array when none found |
| Developer / agent | Update mutable fields (Nome, Aviso, Data, Horario, Local, Sala, Status, InstrucaoStatus, AtestadoStatus, DataConsulta, SenhaAgendamento) | Updated row persisted; FK targets unchanged |
| Developer / agent | Soft delete an Agendamento | Row excluded from all default queries |
| Developer / agent | Run `dotnet test` with unit and SQL integration tests | All Agendamento backend tests pass |
| Developer / agent | Exercise Agendamento endpoints via Swagger against Docker SQL | CRUD, search, and list endpoints return documented status codes |
| Verifier | Review test output, Swagger checklist, legacy table, SDD traceability | Backend slice meets acceptance criteria with recorded evidence |
| Atendimento Workflow SDD implementer | Consume stabilized Agendamento SQL data | Can proceed Execute without stub dependency |

## Acceptance Criteria

- [ ] `REQ-001` — Entity has `PacienteId` (Guid?) and `Paciente` navigation; `SenhaAgendamento` remains owned type; `Status`, `InstrucaoStatus`, `AtestadoStatus` remain enums with string conversion; no `Procedimento` string property; verified by code review and compilation
- [ ] `REQ-002` — `IAgendamentoRepository` all method signatures use `Guid` for IDs; `GetByIdAsync(Guid)`, `GetByPacienteIdAsync(Guid)`, `UpdateAsync(Agendamento, Guid)`, `DeleteAsync(Guid)`; interface compiles; verified by code review
- [ ] `REQ-003` — Repository SQL implementation passes all unit and integration tests; `CreateAsync` persists the new row; `GetByIdAsync` returns a single row with optional inclusion of soft-deleted rows for audit scenarios; `GetByNameAsync` performs substring search per DQ-001; verified by integration tests
- [ ] `REQ-004` — EF configuration updated: `PacienteId` FK configured referencing the `Paciente` table via the existing `PacienteID` column; `Paciente` navigation property mapped; owned-type `SenhaAgendamento`, enum-to-string conversions, and all existing indexes preserved; verified by code review
- [ ] `REQ-005` — Controller has zero `Console.WriteLine` calls; route templates use `{id:guid}`; `string id` parameters replaced with `Guid id`; `CreatedAtAction` uses `Guid`; verified by code review and security-PHI sensor
- [ ] `REQ-006` — POST with invalid `AtendimentoId` returns 404; POST with invalid `InternacaoId` returns 404; POST with non-existent `PacienteId` (when provided) returns 404; POST with soft-deleted FK target returns 404; verified by negative unit tests and integration tests
- [ ] `REQ-007` — GET by id returns 200 + `ReadAgendamentoDto` for existing non-deleted row; 404 for missing or soft-deleted; GET by name returns 200 + array (empty when no matches); GET by PacienteId returns 200 + array; paginated GET returns correct skip/take window; verified by tests and Swagger
- [ ] `REQ-008` — PUT updates mutable fields; `InternacaoId`, `AtendimentoId`, `PacienteId`, and `ID` are not changed by update; returns 204 on success, 404 when not found; verified by tests
- [ ] `REQ-009` — DELETE sets `Deletado = true`, `DeletadoEm`; subsequent GET by id returns 404; deleted row excluded from GetAll, GetByName, GetByPacienteId; verified by tests
- [ ] `REQ-010` — `CreateAgendamentoDto` has no `Procedimento` field, no `Status` field, `PacienteId` as `Guid?`, `StatusInstrucoes`/`StatusAtestado` as enum types; `ReadAgendamentoDto` has `ID` as `Guid`, `PacienteId` as `Guid?`, no `Procedimento`, enums not strings; `UpdateAgendamentoDto` has no FK or ID fields; DTOs compile; verified by code review
- [ ] `REQ-011` — Entity constructor/factory accepts required FKs and scalar fields; factory sets `Status = SemSenha` and `CriadoEm = DateTime.UtcNow`; `SoftDelete()` method sets `Deletado = true`, `DeletadoEm = DateTime.UtcNow`; update method sets `AtualizadoEm` and exposes owned-type `SenhaAgendamento` mutation through a domain method; verified by unit tests
- [ ] `REQ-012` — Unit tests cover: entity construction invariants, factory defaults, soft delete, FK validation logic in controller/application; SQL integration test class exercises full CRUD round-trip (create → read → update → soft delete → read excluded) against Docker SQL; passing `dotnet test` run is success criterion
- [ ] `REQ-013` — Legacy behavior table in this document is complete with preserve/adapt/abandon decisions; all 17 Legacy sheet columns accounted for

## Prerequisite SDDs

| Prerequisite SDD | Status | Contract |
|------------------|--------|----------|
| Paciente SQL Stabilization | **Verified** | Consume `IPacienteRepository` for `PacienteId` FK validation; consume owned-type pattern reference (Endereco → SenhaAgendamento) |
| Atendimento Minimal SQL Stabilization | **Verified** | Consume `IAtendimentoRepository` for `AtendimentoId` FK validation; consume pattern: stub→SQL, Guid migration, controller hardening |
| Prontuario SQL Stabilization | **Verified** | Consume Internacao FK validation pattern; Internacao accessible via `_context.Set<Internacao>()`; dual-FK precedent (PacienteId + AtendimentoId) confirmed |

## Execution Prerequisites

Document infrastructure, credentials, and baseline evidence before Execute. Required for Large work.

**Inherit from prior verified SDDs:** Paciente, Atendimento Minimal, and Prontuario Execution Prerequisites. Reference `Documentation/Technical/runbook.md` for shared infrastructure. Credential Probe must still be executed for this feature — inheritance avoids documentation duplication, not verification.

| Prerequisite | Status | Notes |
|--------------|--------|-------|
| Docker SQL Server (`docorgano-sql`) | Required | `docker compose up -d`; container healthy on port 1433 |
| `SA_PASSWORD` available | Required | `.env` file at repo root; same value Docker uses |
| `DOCORGANO_TEST_CONNECTION` when SQL integration tests apply | Required | Connection string for integration test context |
| Database migrated | Required | `dotnet ef database update --project DocAPI/DocAPI.csproj` succeeds |
| API starts successfully | Required | `dotnet run --project DocAPI/DocAPI.csproj` starts without startup errors |
| Swagger available | Required | Swagger UI at `https://localhost:7004/swagger` |
| Baseline `dotnet build` | Pass | Capture before Execute for measurable progress |
| Baseline `dotnet test` count | ~79 tests | Current test count per State.md; capture before Execute |

Cross-reference `Documentation/Technical/runbook.md` for local setup steps.

### Credential Probe

Run before Execute when SQL integration tests or local SQL gates apply. Record results in Execution Prerequisites.

| Probe step | Pass criteria | Failure action |
|------------|---------------|----------------|
| Repo-root `.env` exists with `SA_PASSWORD` | File present and non-empty | Copy from `.env.example`; align password with Docker volume |
| `scripts/load-env.ps1` loads variables | `$env:SA_PASSWORD` set in shell | Run probe from repo root |
| Docker container `docorgano-sql` running | `docker compose ps` shows healthy | `docker compose up -d` |
| SQL connect with resolved credentials | `SqlConnectionResolver` or `dotnet test` filter `Sql` passes | If login fails, password mismatch — see runbook volume note; do not assume "Docker unreachable" |
| Database migrated | `dotnet ef database update --project DocAPI/DocAPI.csproj` succeeds | Apply migrations before Execute |
| Optional one-command check | `.\scripts\sql-integration-test.ps1` passes or skips with credential hint only when `.env` missing | Fix credentials before treating integration as blocked |

If the probe fails on **login** but Docker is running, treat as **credential misalignment**, not infrastructure unavailability.

## Backend Stabilization

- Frontend validation in scope: **No**
- Default for backend stabilization: exclude Blazor smoke and UI contract work
- Frontend/API drift follow-up: WS07 after all four aggregate backends are SQL-stable. Known drifts:
  - Frontend `StatusAgendamento` enum values 3–7 must align to backend 3–6
  - Frontend `AgendamentoService` / `AgendamentoState` expect `string` IDs — must migrate to `Guid`
  - Frontend `AgendamentoViewModel.Procedimento` must be removed; display derived from Internacao.Procedimentos
  - Route `GET /Agendamento/by-pacientId` param changes from `string` to `Guid`

## Sizing

| Field | Decision |
|-------|----------|
| Size | **Large** |
| Rationale | Fourth SQL migration vertical; entity-schema mismatch requires entity remodeling (add PacienteId, remove Procedimento); owned value object (SenhaAgendamento); 3 FK targets with validation; DTO redesign across all three DTO types; interface Guid migration; full stub→SQL repository; controller hardening with PHI remediation; enum/string alignment; Legacy characterization (17 sheet columns, 14 behaviors); unit + SQL integration tests; SDD Pre-Execution Review required. More complex than Atendimento Minimal (which was Large): entity remodeling, owned type, 3 FK targets, and broader update surface. |
| Required phases | Specify / Design / Tasks / SDD Pre-Execution Review / Execute / Verify / Documentation Follow-Up / Reporting |
| Escalation triggers | Schema migration delta required; InternacaoId nullability change; Procedimento column added back; InternacaoRepository created; new EF migration generated; Atendimento Workflow validations pulled in; WS07 frontend work pulled in; financial features expanded |

## Layer Ownership

### Repository (persistence-only)

- `AgendamentoRepository` performs SQL CRUD against `DocDbContext` only
- Repository does **not** own FK validation (AtendimentoId, InternacaoId, PacienteId)
- Repository does **not** own business rules or workflow logic
- Repository does **not** read or modify Internacao, Prontuario, or other aggregate state beyond FK existence checks

### Controller / Application

- FK validation on create is owned by Controller or Application layer:
  - `AtendimentoId`: lookup via `IAtendimentoRepository`
  - `InternacaoId`: lookup via `_context.Set<Internacao>()`
  - `PacienteId` (when provided): lookup via `IPacienteRepository`
- HTTP status mapping, PHI-safe handling, and Guid route enforcement are Controller responsibilities
- FK validation error responses return HTTP 404 with a body identifying which FK target failed, without exposing internal identifiers (exact format per DQ-006)
- Aggregate construction uses entity constructor/factory — not blind AutoMapper from DTO
- `CreatedAtAction` uses Guid id; `GetByID` route receives Guid

### Aggregate (domain state)

- `Agendamento` constructor/factory owns initial aggregate state (`Status = SemSenha`, `CriadoEm`)
- Create contract: `InternacaoId` (required), `AtendimentoId` (required), `Data` (required), `Horario` (required); `PacienteId` (optional, nullable); scalar fields optional with defaults
- `Status` starts at `SemSenha` — not client-writable on create
- Update method controls which fields are mutable; FK targets and ID are immutable after create
- Update method exposes owned-type `SenhaAgendamento` mutation through a dedicated domain method (e.g., `SetSenha(SenhaAgendamento?)`); Design defines the exact pattern
- `SoftDelete()` method sets `Deletado` and `DeletadoEm`

## Assumptions And Constraints

- ADR-001 soft delete is authoritative; global query filter on `Agendamento` already configured in `DocDbContext.HasQueryFilter(a => !a.Deletado)`
- Paciente, Atendimento Minimal, and Prontuario backends are verified upstream — valid FK sources exist
- Legacy Google Sheets code (`AgendamentoSheetsRepository.cs`) is behavioral reference only for CRUD surface
- No Google Sheets re-enablement on the SQL branch
- PHI must not appear in logs, tests, commits, or documentation examples
- SQL runtime uses Docker `docorgano-sql` with `SA_PASSWORD` environment variable
- Approved migration sequence: Paciente → Atendimento Minimal → Prontuario → **Agendamento** → Atendimento Workflow → WS07
- Test fixtures use synthetic data only (no real patient names or clinical data)
- Schema from `InitialCreate` migration is stable; no new migrations expected
- Migration column `PacienteID` is `nullable: true` — Agendamento can exist without direct Paciente link
- `InternacaoId` is NOT NULL in schema — Agendamento always requires procedure context. **Residual risk (Research DC-3, Medium confidence):** if outpatient scheduling without Internacao becomes a business requirement, this constraint becomes an ADR candidate before Execute
- `Nome` field is a user-provided value (not derived from Paciente FK) — matches Legacy behavior where Nome was independently stored
- Backend `StatusAgendamento` enum values (0–6) are authoritative; frontend drift (values 3–7) is WS07 debt
- `Procedimento` field is abandoned — procedure information derives from `Internacao → ProcedimentoInternacao` navigation
- `InstrucaoStatus` and `AtestadoStatus` remain enums in entity layer; EF `HasConversion<string>()` persists as varchar(30); DTOs use enum types (not strings)
- No `InternacaoRepository` is created; `_context.Set<Internacao>()` is used for FK validation
- Multiple agendamentos per patient, per atendimento, and per internacao are allowed (no uniqueness constraints)
- Pagination: default `skip=0, take=10`; maximum `take=100`; results ordered by `Data DESC, Horario ASC` (Design may adjust ordering)

## Open Questions

Record unresolved decisions here. Resolve during Design or SDD Pre-Execution Review.

| ID | Question | Options | Decision | Date | Owner |
|----|----------|---------|----------|------|-------|
| DQ-001 | `GetByNameAsync` — partial match or exact match? | Partial / Exact | **Resolved: Partial match (substring search)** — matches Legacy behavior (full name not guaranteed) and current controller cleaning logic; exact match would break existing frontend search patterns | 2026-07-17 | Specify |
| DQ-002 | `Nome` field — user-provided or derived from Paciente FK? | User-provided / Derived | **Resolved: User-provided** — Nome is independently stored per Legacy Sheets; deriving from Paciente FK adds read-time complexity and breaks the case where Nome differs from Paciente legal name; nullable PacienteId means Paciente may not be linked | 2026-07-17 | Specify |
| DQ-003 | Update scope — which fields are mutable? | Conservative (few fields) / Broad (all scalars) | **Defaulted: Broad scalar update** — Agendamento is an administrative aggregate (not a clinical journey like Atendimento); all scalar fields except FK targets and ID are mutable; status workflow changes are allowed via API. Rationale: Agendamento is scheduling/administrative data; clinical invariants are owned by Atendimento Workflow. Design may further restrict if domain analysis reveals immutability requirements. | 2026-07-17 | Specify (default to Design) |
| DQ-004 | `CreateAgendamentoDto` — should `SenhaAgendamento` be accepted on create? | Yes (full create) / No (separate endpoint) | **Defaulted: Accepted on create** — Legacy supports full-field create; owned type is simple value object; no separate senha-authorization workflow required for MVP. Design may split if authorization flow needs differ. | 2026-07-17 | Specify (default to Design) |
| DQ-005 | `Status` enum — should PUT allow client to set `Status` directly, or should status transitions be controlled by domain methods? | Client-settable / Domain-method transitions | **Defaulted: Client-settable via PUT** — Agendamento is administrative; Legacy allowed direct status changes; Atendimento Workflow will own clinical status transitions. This avoids over-engineering the aggregate for MVP. If status transition rules are needed, they belong in Atendimento Workflow SDD. | 2026-07-17 | Specify (default to Design) |
| DQ-006 | FK validation error response — what format should 404 error bodies use? | Simple string / Structured JSON / Design decides | **Defaulted: Design decides** — error responses must be HTTP 404 with a body that identifies which FK target failed (AtendimentoId, InternacaoId, or PacienteId) without exposing internal identifiers. Design defines the exact format. This avoids specification over-constraint while preventing internal-ID leakage in error paths. | 2026-07-17 | Specify (default to Design) |

## Domain Language

Terms match `Documentation/Architecture/Domain_Overview_Business_Rules.md`:

- **Agendamento** — Administrative scheduling aggregate for medical procedures; controls dates, location, authorization codes (senhas), and procedural status tracking
- **SenhaAgendamento** — Owned value object (Codigo, DataPedido, DataLiberacao, Validade); insurance/authorization code for a scheduled procedure
- **StatusAgendamento** — Procedural status enum (0–6): SemSenha, SenhaPendente, SenhaAprovada, AgendamentoEfetuado, AgendamentoRemarcado, ProcedimentoConcluido, Cancelada
- **StatusInstrucoes** — Pre-procedure instruction status enum: SemSolicitação, EmAnalise, Negado, Concluido
- **StatusAtestado** — Medical certificate status enum: NaoRealizado, Realizado, Pendente
- **Atendimento** — Care journey container; FK required on Agendamento create
- **Internacao** — Hospitalization record (child of Prontuario aggregate); FK required on Agendamento create; provides procedure information via `ProcedimentoInternacao` collection
- **Paciente** — Upstream aggregate; optional FK on Agendamento (nullable)
- **ProcedimentoInternacao** — Procedure codes and descriptions linked to Internacao; source of procedure information (Legacy `Procedimento` field abandoned)
- **Soft delete** — `Deletado` / `DeletadoEm` with global query filter per ADR-001

## Legacy Behavior

| Behavior | Legacy source | Decision | Notes |
|----------|--------------|----------|-------|
| `GetAllAsync(skip, take)` | `AgendamentoSheetsRepository` | **Preserve** | Paginated list |
| `GetByIdAsync(string id)` | Same | **Adapt** → `Guid` | String comparison → Guid PK |
| `GetByNameAsync(string name)` | Same | **Preserve** | Substring search; filter by Nome field |
| `GetByPacienteIdAsync(string pacienteId)` | Same | **Adapt** → `Guid` | Filter by PacienteId (Guid) |
| `CreateAsync` | Same | **Preserve** | Auto-generate Guid ID; factory sets `Status = SemSenha` |
| `UpdateAsync` | Same | **Adapt** → `Guid` | Row identified by Guid; broad scalar update |
| `DeleteAsync` | Same | **Adapt** → soft delete | Physical delete → ADR-001 soft delete |
| `InstantiateAgendamento(row)` — 17-column Sheets mapping | Same | **Preserve** (mapping logic only) | Column mapping informs entity field design; Sheets serialization abandoned |
| `CreateAgendamentoToSheets` | Same | **Abandon** | Sheets serialization |
| `ParseStatusAgendamento` | Same | **Preserve** (mapping logic) | String → enum mapping; EF `HasConversion<string>()` handles this |
| `CheckSenhaAgendmaneto` | Same | **Abandon** | Stubbed; returns empty Senha |
| `AgendamentosFilter` enum | Same | **Abandon** | Sheets infrastructure |
| Legacy sheet col 0: Nome | `InstantiateAgendamento` row[0] | **Preserve** | User-provided string |
| Legacy sheet col 4: Procedimento | `InstantiateAgendamento` row[4] | **Abandon** | Use `Internacao → ProcedimentoInternacao` |
| Legacy sheet col 7: Status | `InstantiateAgendamento` row[7] | **Preserve** | Enum with string conversion |
| Legacy sheet col 12: PacienteID | `InstantiateAgendamento` row[12] | **Preserve** | Guid?, nullable FK |
| Legacy sheet col 13: ID | `InstantiateAgendamento` row[13] | **Adapt** → `Guid` | String → Guid |
| Legacy sheet col 14: StatusAtestado | `InstantiateAgendamento` row[14] | **Preserve** | Enum with string conversion |
| Legacy sheet col 15: StatusInstrucoes | `InstantiateAgendamento` row[15] | **Preserve** | Enum with string conversion |

## Security And PHI

- Security/PHI review needed: **Yes**
- Rationale: Feature touches patient linkage (`PacienteId`, `Nome`), API exposure, controller logging remediation (current `Console.WriteLine` with `agendamento.Nome` and `agendamento.ID`), and test fixtures. Controller currently logs patient-identifying data on create (line 70: `O agendamento d@ {agendamento.Nome} foi efetuado`) and ID on create/get (lines 41, 71). All `Console.WriteLine` calls in `AgendamentoController` require removal. Test fixtures must use synthetic data only. `Nome` field — while not PHI when copied from Paciente name, it is stored independently and must be treated as potentially identifying.

## Initial Verification Expectations

| Requirement | Expected evidence |
|-------------|-------------------|
| `REQ-001` | Code review: entity has `PacienteId` (Guid?), `Paciente` nav, no `Procedimento` string; enums preserved with `HasConversion<string>()` |
| `REQ-002` | Code review: `IAgendamentoRepository` all ID parameters are `Guid`; interface compiles |
| `REQ-003` | Unit tests: repository methods mock DbContext; SQL integration tests: CRUD round-trip against Docker SQL |
| `REQ-004` | Code review: `AgendamentoConfiguration` includes `Paciente` relationship; existing config preserved |
| `REQ-005` | Code review + security-PHI sensor: zero `Console.WriteLine`; all routes use `{id:guid}` constraints |
| `REQ-006` | Unit tests: negative FK validation (invalid AtendimentoId, InternacaoId, PacienteId, soft-deleted targets); integration tests: FK validation against real SQL |
| `REQ-007` | Unit tests: query methods; Swagger smoke: GET endpoints return documented status codes |
| `REQ-008` | Unit tests: update confirms FK targets unchanged; integration tests: round-trip update |
| `REQ-009` | Unit tests: soft delete exclusion from all query methods; integration tests: deleted row not returned |
| `REQ-010` | Code review: DTOs have correct types, no `Procedimento`, no `Status` on create, enums not strings |
| `REQ-011` | Unit tests: factory sets `Status = SemSenha`, `CriadoEm`; `SoftDelete()` sets flags; `AtualizadoEm` on update; `SenhaAgendamento` mutation via domain method |
| `REQ-012` | **Passing** `dotnet test` output for Agendamento test classes; SQL integration tests pass when Docker available |
| `REQ-013` | Legacy behavior table in this document reviewed and accepted |

## References

- `Documentation/State.md`
- `AGENTS.md`
- `Documentation/Product/PM_DocOrgano.md`
- `Documentation/Product/PRD.md`
- `Documentation/Technical/migration-sql.md`
- `Documentation/Architecture/ADR/ADR-001-soft-delete.md`
- `Documentation/Architecture/Domain_Overview_Business_Rules.md`
- `Documentation/Architecture/erd.dbml`
- `Documentation/SDD/Agendamento_Stabilization/research.md`
- `Documentation/SDD/paciente-sql-stabilization/`
- `Documentation/SDD/atendimento-minimal-sql-stabilization/specify.md`
- `Documentation/SDD/prontuario-sql-stabilization/verification.md`
- `Documentation/AI-Harness/Harness-Design/sdd-operational.md`
- `Documentation/AI-Harness/Harness-Design/verification-governance.md`
- `DocAPI/Core/Entities/Agendamento.cs`
- `DocAPI/Core/Entities/Internacao.cs`
- `DocAPI/Core/Entities/ProcedimentoInternacao.cs`
- `DocAPI/Core/Entities/Prontuario.cs`
- `DocAPI/Core/Interfaces/Repositories/IAgendamentoRepository.cs`
- `DocAPI/Infrastructure/Repositories/AgendamentoRepository.cs`
- `DocAPI/API/Controllers/AgendamentoController.cs`
- `DocAPI/Infrastructure/SqlDb/Configurations/AgendamentoConfig.cs`
- `DocAPI/Infrastructure/SqlDb/DbContext/DbContext.cs`
- `DocAPI/Migrations/20260416212120_InitialCreate.cs`
- `DocAPI/Application/Data/Dtos/Agendamento/CreateAgendamentoDto.cs`
- `DocAPI/Application/Data/Dtos/Agendamento/ReadAgendamentoDtos.cs`
- `DocAPI/Application/Data/Dtos/Agendamento/UpdateAgendamento.cs`
- `DocAPI/Application/Mappings/Profiles/AgendamentoProfile.cs`
- `DocAPI/Legacy/_LegacySheetsDb/AgendamentoSheetsRepository.cs`