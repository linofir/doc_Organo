# Specify — Prontuario SQL Stabilization

> Feature SDD: `Documentation/SDD/prontuario-sql-stabilization/`  
> Generated SDD artifacts are written in English.

## Context

Prontuario is the third aggregate in the approved SQL migration sequence. Paciente and Atendimento Minimal are verified upstream. `ProntuarioRepository` is fully stubbed; the controller and DTO layer carry Legacy contract drift (string IDs, missing root FKs, unsafe AutoMapper, PHI logging). This feature stabilizes SQL-backed clinical snapshot persistence — composite owned value objects plus nested Exames, Internacao, and AcoesCD — with explicit versioning semantics and Atendimento FK consumption from the verified prerequisite SDD.

**Specify decision delta (supersedes Research Part 3 defaults):**

- Versioning API: **PUT = correction** (in-place, same version); **POST `/Prontuario/{id}/versoes` = clinical evolution** (successor row) — not PUT-always-successor.
- List by paciente: all versions ordered **`Versao` descending**; latest via dedicated **`/atual`** endpoints.
- Version numbering: **per `PacienteId`** (schema-native; no migration in default scope).

- PM item: WS01 — Prontuario SQL Migration & Stabilization
- Product or domain source: `Documentation/Product/PRD.md`, `Documentation/Product/PM_DocOrgano.md`, `Documentation/Architecture/Domain_Overview_Business_Rules.md`
- Current operational source: `Documentation/State.md`
- Related SDD, ADR, technical, or architecture docs: `Documentation/Technical/migration-sql.md`, `Documentation/Architecture/ADR/ADR-001-soft-delete.md`, `Documentation/Architecture/ADR/ADR-006-prontuario-dual-mode-versioning-api.md`, `Documentation/SDD/paciente-sql-stabilization/` (pilot reference), `Documentation/SDD/atendimento-minimal-sql-stabilization/` (verified prerequisite), `Documentation/SDD/atendimento-workflow-stabilization/research.md` (downstream consumer), `Documentation/SDD/prontuario-sql-stabilization/research.md`, `Documentation/AI-Harness/Harness-Design/sdd-operational.md`

## Problem

`ProntuarioRepository` throws on all operations. `ProntuarioController` is active but non-functional against SQL persistence: string IDs vs Guid entities, no root `PacienteId` / `AtendimentoId` on create DTOs, `Console.WriteLine` with patient-identifying data, and inconsistent HTTP status mapping. `ProntuarioProfile` AutoMapper mappings are unsafe (including known cross-mapping of Internacao into `InformacoesExtras` on update).

The aggregate is a **composite clinical snapshot** with versioning fields (`Versao`, `ProntuarioAnteriorId`), mandatory `AtendimentoId` FK, and up to five persistence surfaces per write. Legacy Google Sheets behavior used in-place updates with no versioning; domain authority requires immutable snapshots for clinical **evolution**, while **corrections** to the current version are a distinct business operation exposed explicitly in the API.

Without this slice, Prontuario tabs remain blocked on stub API, Agendamento and Atendimento Workflow forward SDDs remain blocked on Prontuario verification, and merge criterion #2 in `migration-sql.md` cannot advance.

## Goals

- [ ] `REQ-001` — Create Prontuario **version 1** via domain factory with root `PacienteId`, `AtendimentoId`, and full clinical payload (owned VOs plus child collections as supplied)
- [ ] `REQ-002` — Validate `AtendimentoId` on create: exists, not soft-deleted, and `Atendimento.PacienteId` matches `PacienteId`; return **404** on failure (Paciente/Atendimento precedent)
- [ ] `REQ-003` — Retrieve a specific Prontuario version by Guid id; soft-deleted rows excluded from default queries
- [ ] `REQ-004` — List all non-deleted Prontuario versions for a patient via `GET /Prontuario/paciente/{pacienteId}`; results ordered by **`Versao` descending**, tie-breaker **`CriadoEm` descending**
- [ ] `REQ-005` — Expose latest-version reads: `GET /Prontuario/paciente/{pacienteId}/atual` (highest `Versao` among non-deleted for patient) and `GET /Prontuario/atendimento/{atendimentoId}/atual` (highest `Versao` among non-deleted for that atendimento)
- [ ] `REQ-006` — **Correction:** `PUT /Prontuario/{id}` updates the **current version in place** (same `Id`, same `Versao`); returns **204** or **404**; does not create a successor row
- [ ] `REQ-007` — **Clinical evolution:** `POST /Prontuario/{id}/versoes` creates a **successor version** (new `Id`, incremented `Versao` per `PacienteId`, `ProntuarioAnteriorId` set, prior row unchanged); returns **201** + `ReadProntuarioDto`
- [ ] `REQ-008` — Soft delete **single version** per ADR-001; other versions remain visible unless separately deleted
- [ ] `REQ-009` — Persist nested graph: Exames, ProntuarioAcaoCD, optional Internacao with ProcedimentoInternacao; replace collection semantics on new version creation documented in acceptance criteria
- [ ] `REQ-010` — When Internacao payload is present, validate required fields and existing `CIDCodigo`; unknown CID returns **400** or **404** (not unhandled FK **500**)
- [ ] `REQ-011` — Guid end-to-end on repository interface and API; **dedicated API DTOs** — no domain entity types in public request/response DTOs
- [ ] `REQ-012` — PHI-safe controller: no `Console.WriteLine` or equivalent logging of patient-identifying or clinical payload data
- [ ] `REQ-013` — SQL integration tests **required**; fixture chain Paciente → Atendimento → Prontuario v1 → correction → evolution → v2 with nested children (**repository-direct** round-trip per Atendimento pilot; HTTP smoke in Runtime Validation); synthetic CID rows in test setup when Internacao scenarios apply
- [ ] `REQ-014` — Legacy behavior characterized with explicit preserve/adapt/abandon decisions in this document
- [ ] `REQ-015` — HTTP status codes Paciente-aligned: **201** create/evolution, **204** correction/delete, **404** not found / FK miss, **400** validation; no conflation of missing resource with **500**
- [ ] `REQ-016` — **Workflow-readable persistence readiness (non-functional):** persist the full clinical graph on existing schema fields with stable types; workflow-relevant fields must remain queryable without requiring aggregate reconstruction; persistence must allow future workflow consumers efficient reads without domain reconstruction logic
- [ ] `REQ-017` — **Workflow logic explicitly out of scope:** must not implement workflow logic, stage evaluators, stage progression, pendências, `ClinicalEvent` writes, or workflow-specific contracts
- [ ] `REQ-018` — **`Tipo`** exposed as **int** in API DTOs matching entity and database (canonical type)

## Out Of Scope

- **Atendimento** repository/controller implementation — **verified prerequisite**; consume `Documentation/SDD/atendimento-minimal-sql-stabilization/` only
- Auto-create Atendimento inside Prontuario create
- Atendimento Workflow (`ValidacaoEtapa*`, pendências, journey projection, stage advancement) — `atendimento-workflow-stabilization` after Prontuario + Agendamento verified
- Defining workflow-specific fields or evaluator contracts — Workflow SDD owns those decisions
- WS07 Blazor integration, frontend validation, UI smoke, onboarding UX (guide user to create Atendimento after Paciente)
- `POST /Prontuario/from-pdf` and PDF patient report endpoints — future **Prontuario PDF ingestion** when extraction model exists
- CID catalog management in production — future feature; synthetic CID in integration tests only
- Agendamento repository implementation
- Google Sheets import or re-enablement
- Auth/RBAC (`AtualizadoPor` may remain unset; accepted residual risk)
- New EF migrations (**default: none**) — version numbering uses existing unique index `(PacienteId, Versao)` per **DQ-007 Option A**
- PATCH endpoints; partial update without explicit correction vs evolution choice
- Financial features
- Verification artifact creation (`verification.md`) — Verify phase
- Feature report, session-handoff, Teacher Guide — post-Verify reporting chain
- Any changes under `DocFront.Web/`

## Users And Scenarios

| Actor | Scenario | Outcome |
|-------|----------|---------|
| Developer / agent | Create Atendimento via verified API, then POST Prontuario with explicit FKs | Version 1 persisted with nested clinical sections |
| Developer / agent | Correct documentation error on current version | PUT returns 204; same `Id` and `Versao`; row updated in place |
| Developer / agent | Record clinical evolution after initial consult | POST `/versoes` returns 201; new `Id`; prior version immutable |
| Developer / agent | List patient prontuario history | All non-deleted versions, newest `Versao` first |
| Developer / agent | Fetch latest for patient or atendimento | `/atual` endpoints return single latest version |
| Developer / agent | Run `dotnet test` with unit and SQL integration tests | Prontuario tests pass; SQL integration passing run expected when Docker available |
| Developer / agent | Exercise Prontuario endpoints via Swagger | Documented status codes for create, read, list, correction, evolution, delete |
| Verifier | Review tests, legacy table, versioning semantics, PHI remediation | Backend slice meets acceptance criteria with recorded evidence |
| Workflow SDD implementer (future) | Query persisted Prontuario by `AtendimentoId` with Tipo, DataConsulta, CD, Internacao | Data available without workflow logic in this slice |

## Acceptance Criteria

- [ ] `REQ-001` — POST creates row with `Versao = 1`, valid FKs, owned VOs and supplied children persisted; `DescricaoBasica` identity snapshot populated from `Paciente` where applicable; verified by unit and integration tests
- [ ] `REQ-002` — POST with missing, soft-deleted, or Paciente-mismatched `AtendimentoId` returns **404**; verified by negative tests
- [ ] `REQ-003` — GET by id returns **200** + `ReadProntuarioDto` including `Versao`, `ProntuarioAnteriorId`, `PacienteId`, `AtendimentoId`; **404** for missing or soft-deleted; verified by tests and Swagger
- [ ] `REQ-004` — GET list-by-paciente returns **200** + array of all non-deleted versions; ordered **`Versao` DESC**; empty array when none; verified by tests
- [ ] `REQ-005` — GET `/paciente/{id}/atual` returns **200** + latest patient-wide version or **404** when none; GET `/atendimento/{id}/atual` returns **200** + latest for that atendimento or **404**; verified by tests including multi-version scenarios
- [ ] `REQ-006` — PUT on existing non-deleted version updates row in place; **`Id` and `Versao` unchanged**; returns **204** or **404**; prior-version rows unaffected; verified by unit and integration tests
- [ ] `REQ-007` — POST `/versoes` creates new row with `Versao = max(existing for PacienteId) + 1`, `ProntuarioAnteriorId` pointing to source id, same `AtendimentoId` unless payload explicitly changes it (Design may constrain); source row **byte-stable** for clinical owned columns; returns **201** + new DTO; verified by integration round-trip
- [ ] `REQ-008` — DELETE soft-deletes only targeted version; other versions still returned by list and get-by-id; verified by tests
- [ ] `REQ-009` — Create and evolution persist Exames, AcoesCD, and optional Internacao graph; evolution uses **replace** semantics for child collections on the new version row; verified by integration test with nested payload
- [ ] `REQ-010` — Create/evolution with Internacao and unknown `CIDCodigo` returns **400** or **404**; with valid synthetic CID in test DB succeeds; verified by negative and positive integration tests
- [ ] `REQ-011` — Repository interface and controller routes use `Guid`; public DTOs do not expose domain entity types; verified by code review and Swagger
- [ ] `REQ-012` — No PHI in touched `ProntuarioController` code paths; security-phi-review sensor applied at Verify
- [ ] `REQ-013` — SQL integration test class **required**; exercises Paciente → Atendimento → Prontuario v1 → PUT correction → POST versoes → v2; passing run is success criterion — skip only for documented operational/environment blockers (not expected success)
- [ ] `REQ-014` — Legacy behavior table in this document complete and accepted
- [ ] `REQ-015` — Status codes match Paciente patterns for equivalent operations; verified by tests and Swagger smoke
- [ ] `REQ-016` — Integration test loads Prontuario by `AtendimentoId` with nested children intact via repository or API read paths without aggregate reconstruction helpers; workflow-relevant fields readable from persisted shape; documented in test naming or notes for downstream SDD consumers
- [ ] `REQ-017` — Implementation contains no workflow evaluators, stage progression, pendência writes, or workflow contract types; verified by code review and scope boundary check at SDD Pre-Execution Review
- [ ] `REQ-018` — **`Tipo`** stored and exposed as **int** matching entity/DB (canonical type); verified by contract tests

Frontend compatibility is not a success criterion for this feature.

## Execution Prerequisites

Execute must not begin until all of the following are confirmed. Inherit patterns from verified pilots — see `Documentation/SDD/atendimento-minimal-sql-stabilization/specify.md` Execution Prerequisites and Credential Probe.

| Prerequisite | How to confirm |
|--------------|----------------|
| Paciente SQL slice verified | `Documentation/SDD/paciente-sql-stabilization/verification.md` or State confirms verified |
| Atendimento Minimal SQL slice verified | `Documentation/SDD/atendimento-minimal-sql-stabilization/verification.md` — **27 tests** baseline |
| Docker SQL container running | `docker compose up -d`; container `docorgano-sql` healthy on port 1433 |
| `SA_PASSWORD` environment variable set | Required for SQL Server container and local connection |
| Database migrated | `dotnet ef database update --project DocAPI/DocAPI.csproj` succeeds |
| API starts successfully | `dotnet run --project DocAPI/DocAPI.csproj` starts without startup errors |
| Baseline tests executed | `dotnet test` once before changes; record **27 tests** baseline pass/fail/skip |
| Swagger available | Swagger UI reachable at DocAPI dev URL (default `https://localhost:7004/swagger`) |

If any prerequisite fails, stop Execute and resolve infrastructure before implementation tasks.

### Credential Probe

Run before Execute when SQL integration tests apply. Record results in session notes and SDD Pre-Execution Review.

| Probe step | Pass criteria | Failure action |
|------------|---------------|----------------|
| Repo-root `.env` exists with `SA_PASSWORD` | File present and non-empty | Copy from `.env.example`; align password with Docker volume |
| `scripts/load-env.ps1` loads variables | `$env:SA_PASSWORD` set in shell | Run probe from repo root |
| Docker container `docorgano-sql` running | `docker compose ps` shows healthy | `docker compose up -d` |
| SQL connect with resolved credentials | `dotnet test` filter `Sql` passes or `SqlConnectionResolver` succeeds | If login fails, password mismatch — see runbook; not "Docker unreachable" |
| Database migrated | `dotnet ef database update --project DocAPI/DocAPI.csproj` succeeds | Apply migrations before Execute |
| Optional one-command check | `.\scripts\sql-integration-test.ps1` passes or skips with credential hint only when `.env` missing | Fix credentials before treating integration as blocked |

If the probe fails on **login** but Docker is running, treat as **credential misalignment**, not infrastructure unavailability.

## Runtime Validation Environment

### Environment

| Component | Requirement |
|-----------|-------------|
| Docker SQL | `docorgano-sql` with migrated schema from `InitialCreate` |
| Local API | DocAPI running against `DefaultConnection` |
| Swagger | DocAPI Swagger UI — DocFront.Web not required |
| Upstream fixtures | Synthetic Paciente + Atendimento rows via verified API or test helpers |
| Synthetic CID | Test-only CID row(s) when Internacao scenarios run (e.g. `Z99.9`) |

### Manual validation checklist

Execute performs these via Swagger and records intent in session notes. Durable evidence is Verify-owned (`verification.md` § Runtime Validation when TASK-008 applies).

| Scenario | Endpoint pattern | Expected outcome |
|----------|------------------|------------------|
| Create v1 | POST `/Prontuario` | **201** + `ReadProntuarioDto`; `versao = 1` |
| Read by ID | GET `/Prontuario/{id}` | **200** / **404** |
| List by paciente | GET `/Prontuario/paciente/{pacienteId}` | **200**; all versions; **Versao DESC** |
| Latest by paciente | GET `/Prontuario/paciente/{pacienteId}/atual` | **200** / **404** |
| Latest by atendimento | GET `/Prontuario/atendimento/{atendimentoId}/atual` | **200** / **404** |
| Correction | PUT `/Prontuario/{id}` | **204**; same id returned on subsequent GET |
| Evolution | POST `/Prontuario/{id}/versoes` | **201**; new id; incremented versao |
| Delete version | DELETE `/Prontuario/{id}` | **204** soft delete |
| Invalid AtendimentoId | POST with bad FK | **404** |
| Paginated list | GET `/Prontuario?skip=&take=` | **200** + array |

## Backend Stabilization

- Frontend validation in scope: **No**
- Default for backend stabilization: exclude Blazor smoke and UI contract work
- Frontend/API drift follow-up: WS07 — Guid IDs, root `PacienteId`/`AtendimentoId`, correction vs evolution verbs, `/atual` routes, `Tipo` as int

## Sizing

| Field | Decision |
|-------|----------|
| Size | **Large** |
| Rationale | Composite aggregate with nested persistence, dual versioning API (correction vs evolution), Atendimento FK consumption, Guid/DTO contract migration, AutoMapper replacement, PHI remediation, required SQL integration fixture chain, and third forward post-calibration SDD pilot. Greater complexity than Paciente/Atendimento Minimal. |
| Migration vertical note | Third SQL vertical pilot; Large reflects harness and verification burden, not file count alone |
| Required phases | Specify / Design / Tasks / SDD Pre-Execution Review / Execute / Verify / Documentation Follow-Up / Reporting / Teacher Guide when warranted |
| Design follow-up | Field classification (immutable / correction-safe / evolution-only) — see Design Follow-Up section |
| Escalation triggers | Schema migration required (e.g. DQ-007 Option B lineage versioning); PDF import pulled in scope; workflow evaluators pulled in scope; correction/evolution API rejected by product |

## Layer Ownership

### Repository (persistence-only)

- SQL CRUD, includes, transactions, and FK existence checks at persistence boundary
- Persists aggregate-assigned `Versao` and version chain fields — **does not** calculate next business version numbers
- Does **not** own HTTP status mapping or DTO validation orchestration
- Does **not** implement workflow logic or read Atendimento beyond FK validation needs

### Controller / Application

- HTTP semantics, PHI-safe handling, DTO validation when Internacao section present
- Atendimento FK validation before create (existence, Paciente match, not soft-deleted)
- Maps correction (PUT) vs evolution (POST versoes) to domain/repository operations

### Aggregate (domain state)

- Factories for v1 create; `AplicarCorrecao` (or equivalent) for in-place correction; `CriarNovaVersao(source, nextVersao, payload)` for evolution
- **Owns versioning rule** — validates assigned `nextVersao` and assigns `Versao` / `ProntuarioAnteriorId` on evolution (see ADR-006, Design D-02); does **not** query persistence for max version
- Application layer obtains `nextVersao` from repository lookup; repository persists aggregate-assigned values only
- Version assignment per **`PacienteId`** scope (`Versao = 1` on first patient prontuario; increment on evolution only)
- Child collection ownership on new version; soft delete per version
- Does **not** own persistence or HTTP concerns

## Assumptions And Constraints

- ADR-001 soft delete is authoritative
- Paciente and Atendimento Minimal SQL are verified — Prontuario **consumes** Atendimento API/contracts; does not re-implement
- Approved migration sequence: Paciente → Atendimento Minimal → **Prontuario** → Agendamento → Atendimento Workflow → WS07
- Legacy Google Sheets code is behavioral reference only (`ProntuarioSheetsRepository.cs` targeted reads)
- No Google Sheets re-enablement on the SQL branch
- PHI must not appear in logs, tests, commits, or documentation examples
- Version lineage is **patient-wide** (domain decision — see Versioning § Version Lineage); unique index **`(PacienteId, Versao)`** aligns with that decision (**DQ-007 Option A**)
- **`Tipo`** canonical type is **int** matching entity and database
- No new EF migrations unless Design proves unavoidable and escalation accepts migration
- Test fixtures use synthetic data only
- Correction (PUT) and evolution (POST versoes) are **explicit client operations** — backend does not infer intent from payload shape alone

## Open Questions

| ID | Question | Options | Decision | Date | Owner |
|----|----------|---------|----------|------|-------|
| DQ-001 | Versioning API strategy | A) PUT always successor B) PUT correction + POST `/versoes` evolution | **Resolved: B** | 2026-06-19 | Specify |
| DQ-006 | ListByPaciente scope | A) All versions ordered B) Latest only on list route | **Resolved: A** — latest via `/atual` | 2026-06-19 | Specify |
| DQ-007 | Version numbering scope | A) Per PacienteId B) Per lineage (`ProntuarioOriginalId`) | **Resolved: A** — no migration | 2026-06-19 | Specify |
| DQ-008 | Stable error code when `AtendimentoId` missing/invalid for UX | A) **404** only B) **400/404** + machine-readable code (e.g. `atendimento_required`) | **Resolved: A — 404 only** (Paciente/Atendimento precedent; WS07 out of scope; machine-readable frontend contracts deferred) | 2026-06-19 | Specify |
| DQ-009 | ADR for dual-mode versioning API | A) Formal ADR B) Defer branch-local authority | **Resolved: A — ADR-006 accepted** | 2026-06-19 | Pre-Execution Review |

## Domain Language

Terms align with `Documentation/Architecture/Domain_Overview_Business_Rules.md` with Specify qualifications:

- **Prontuário** — Clinical snapshot aggregate; versioned history
- **Correction** — In-place update of the **current version** (`PUT`); same `Id` and `Versao`; intended for fixing errors on the active record
- **Clinical evolution** — New immutable snapshot (`POST /versoes`); new row, incremented `Versao`, `ProntuarioAnteriorId` set; prior row unchanged
- **Versao** — Integer sequence scoped to **patient-wide lineage** (`PacienteId`); domain decision — see Versioning § Version Lineage; incremented only on evolution, not on correction
- **Atendimento** — Verified upstream journey container; `AtendimentoId` required on every Prontuario version row
- **Soft delete** — Per ADR-001; deletes one version, not entire patient history
- **Internacao** — Optional 1:1 child; when present, CID FK required

Domain doc states *"Cada alteração clínica gera um novo prontuário"* — qualified here: **evolution** uses POST `/versoes`; **correction** is an explicit exception via PUT when the client chooses correction over evolution.

## Versioning

### Business Rules — Correction vs Evolution

- The client **explicitly chooses** between:
  - **Correction** — `PUT /Prontuario/{id}`
  - **Clinical evolution** — `POST /Prontuario/{id}/versoes`
- The backend **must never** infer clinical evolution from payload changes.
- Payload content alone **must not** determine whether a new version is created.
- Version creation is **always** an explicit client decision.

### Version Lineage — Decision Record

**Decision:** Version lineage is **patient-wide**.

**Rationale:**

- Medical history belongs to the **patient**, not to a single Atendimento.
- Multiple Atendimentos may contribute to the patient's clinical evolution.
- Successive Prontuario versions represent the patient's evolving medical history.
- Version numbering therefore remains scoped to **`PacienteId`**.

This is a **domain decision**, not merely a database-index convenience. The existing unique index `(PacienteId, Versao)` implements the decision; reversing lineage scope would require domain re-evaluation and likely schema change.

### Version Generation — Domain Ownership

Authoritative policy: [ADR-006](../../Architecture/ADR/ADR-006-prontuario-dual-mode-versioning-api.md); implementation split per Design **D-02**:

- The **aggregate** owns the versioning **rule** — validates assigned `nextVersao`, assigns identity and chain fields on evolution; **must not** query the database.
- The **Application layer** (controller orchestration) obtains `max(Versao)` for `PacienteId` via repository lookup, then computes `nextVersao`.
- The **repository** returns persistence facts only (e.g. `GetMaxVersaoForPacienteAsync`) and persists aggregate-assigned values — **must never** decide business version policy.

## Design Follow-Up

The **Design** phase (`design.md`) must explicitly classify:

- **Immutable fields**
- **Correction-safe fields** (mutable via `PUT`)
- **Evolution-only fields** (changed only via `POST /versoes`)

This Specify artifact **does not** define those field-level classifications. Design owns the final classification to prevent ambiguity between correction and clinical evolution operations.

## Legacy Behavior

| Behavior | Source | Decision | Notes |
|----------|--------|----------|-------|
| CRUD surface (list, by id, by paciente, create, update, delete) | `ProntuarioSheetsRepository` | Preserve | Repository contract; Guid adapt |
| Multi-section clinical payload | Same | Preserve | AGO, AP, AF, PosOp, CD, Exames, Internacao |
| Patient linkage | Same | Adapt | Root `PacienteId` + snapshot in `DescricaoBasica` |
| In-place update (legacy PUT) | Same | **Adapt** | **Correction path only** (`PUT`); not default for clinical evolution |
| Version increment on save | Legacy absent | **Adapt** | **Evolution** via POST `/versoes` only |
| 3-sheet write pattern | Same | Adapt | Normalized SQL tables |
| String IDs | Legacy/API today | Abandon | Guid end-to-end |
| Hard delete across sheets | Same | Adapt | ADR-001 soft delete per version |
| `CreateFromPdfAsync` / `/from-pdf` | Controller + commented service | Abandon | Out of scope — future PDF ingestion |
| PDF patient reports | Commented routes | Abandon | Out of scope |
| Legacy PHI logging | `ProntuarioController` | Abandon | REQ-012 remediation |
| AutoMapper create/update | `ProntuarioProfile` | Abandon | Replace with factories + explicit projection |
| Domain entity types in DTOs | Current DTOs | Abandon | Dedicated API DTOs |

## Security And PHI

- Security/PHI review needed: **Yes**
- Rationale: Rich clinical PHI (CPF, history, gynecologic data, hospitalization details) in payloads; controller logging remediation required; test fixtures must remain synthetic.

## Initial Verification Expectations

| Requirement | Expected evidence |
|-------------|-------------------|
| `REQ-001` | Unit factory test; integration create v1; Swagger POST 201 |
| `REQ-002` | Negative tests invalid AtendimentoId → 404 |
| `REQ-003` | Unit/integration GET; version metadata on DTO |
| `REQ-004` | List ordering test Versao DESC |
| `REQ-005` | `/atual` tests patient-wide and atendimento-scoped |
| `REQ-006` | PUT correction: same Id/Versao after update |
| `REQ-007` | POST versoes: new Id, chain integrity, prior row unchanged |
| `REQ-008` | Soft delete single version exclusion |
| `REQ-009` | Integration nested graph Exames + Internacao + CD |
| `REQ-010` | CID negative + synthetic CID positive |
| `REQ-011` | Code review; Swagger Guid contracts |
| `REQ-012` | security-phi-review sensor |
| `REQ-013` | **Passing** `ProntuarioSqlIntegrationTests` (or equivalent) full fixture chain |
| `REQ-014` | Legacy table reviewed |
| `REQ-015` | Swagger smoke status codes |
| `REQ-016` | Integration read by AtendimentoId; persisted fields queryable without aggregate reconstruction |
| `REQ-017` | Code review; no workflow evaluators or contracts in scope |
| `REQ-018` | Contract test `Tipo` as int |

Verifier selects final gates. domain-review sensor recommended for versioning vs legacy table.

## ADR Evaluation

| Candidate | Trigger | Status |
|-----------|---------|--------|
| Dual-mode versioning API (PUT correction vs POST evolution) | Breaking semantic change vs legacy and prior Research PUT-default | **Accepted — [ADR-006](../../Architecture/ADR/ADR-006-prontuario-dual-mode-versioning-api.md)** (2026-06-19, Pre-Execution Review) |
| Version numbering per PacienteId | Accepted via DQ-007 A without schema change | **No ADR** unless product reverses to lineage scope |

## References

- `Documentation/State.md`
- `AGENTS.md`
- `Documentation/Product/PM_DocOrgano.md`
- `Documentation/Product/PRD.md`
- `Documentation/Technical/migration-sql.md`
- `Documentation/Technical/runbook.md`
- `Documentation/Architecture/ADR/ADR-001-soft-delete.md`
- `Documentation/Architecture/ADR/ADR-006-prontuario-dual-mode-versioning-api.md`
- `Documentation/SDD/prontuario-sql-stabilization/reports/pre-execution-review-2026-06-19.md`
- `Documentation/Architecture/Domain_Overview_Business_Rules.md`
- `Documentation/SDD/prontuario-sql-stabilization/research.md`
- `Documentation/SDD/paciente-sql-stabilization/`
- `Documentation/SDD/atendimento-minimal-sql-stabilization/`
- `Documentation/SDD/atendimento-workflow-stabilization/research.md`
- `Documentation/AI-Harness/Harness-Design/sdd-operational.md`
- `Documentation/AI-Harness/Harness-Design/verification-governance.md`
- `DocAPI/Legacy/_LegacySheetsDb/ProntuarioSheetsRepository.cs` (behavioral reference only)
