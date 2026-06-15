# Specify - Paciente SQL Stabilization

> Feature SDD: `Documentation/SDD/paciente-sql-stabilization/`
> Generated SDD artifacts are written in English.

## Context

Paciente is the first SQL migration vertical and the only aggregate with a fully implemented EF repository on the SQL branch. This feature exists to formally stabilize and harden that backend slice before expanding migration to Prontuario, Agendamento, and Atendimento.

- PM item: P0 — Validar vertical Paciente em SQL (WS01); P0 — Testes SQL de integração para Paciente (WS03); P0 — Remover logs com dados sensíveis (WS04)
- Product or domain source: `Documentation/Product/PRD.md`, `Documentation/Product/PM_DocOrgano.md`, `Documentation/Architecture/Domain_Overview_Business_Rules.md`
- Current operational source: `Documentation/State.md`
- Related SDD, ADR, technical, or architecture docs: `Documentation/Technical/migration-sql.md`, `Documentation/Architecture/ADR/ADR-001-soft-delete.md`, `Documentation/AI-Harness/Harness-Design/sdd-operational.md`

## Problem

Paciente SQL CRUD was implemented ahead of formal SDD and verification. The repository, controller, and five InMemory unit tests exist, but known gaps block declaring the first migration slice complete:

- Create/update may not persist all DTO fields (Plano, Carteira, RG, Endereco)
- `Console.WriteLine` logs patient-identifying data in the controller
- No SQL integration tests against Docker SQL Server
- API error handling gaps (duplicate CPF may return 500; update/delete may not map 404 correctly)
- Patient search API contract is ambiguous (single-result vs collection semantics on nome route)
- Legacy behavior not formally characterized with preserve/adapt/abandon decisions

PM P0 (WS01/WS03) requires backend production-ready stabilization: Docker SQL, Swagger, tests, and domain alignment per `Documentation/Technical/migration-sql.md` aggregate #1 done criteria (CRUD + SQL test + Swagger). Frontend alignment is deferred to WS07.

## Goals

- [ ] `REQ-001` — Paciente CRUD persists all DTO fields (Nome, Nascimento, CPF, RG, Email, Telefone, Plano, Carteira, Endereco) to SQL
- [ ] `REQ-002` — Soft delete hides deleted patients from default queries (ADR-001)
- [ ] `REQ-003` — Single-resource retrieval uses exact match (Guid PK, CPF after trim); collection search supports partial nome match (`Contains`) with paginated results
- [ ] `REQ-004` — API returns correct status codes: 201 create, 200 read/search, 204 update/delete, 400 validation, 404 not found for single-resource miss, 409 duplicate CPF or CPF data-integrity conflict
- [ ] `REQ-005` — No PHI in logs (`Console.WriteLine` removed or replaced with safe logging)
- [ ] `REQ-006` — SQL integration tests validate repository CRUD against real SQL Server (Docker)
- [ ] `REQ-007` — Swagger manual smoke confirms all Paciente API endpoints and documented status codes
- [ ] `REQ-008` — Legacy behavior characterized with explicit preserve/adapt/abandon decisions documented

## Out Of Scope

- Frontend validation, smoke tests, runtime validation, compatibility validation
- Blazor validation, UI testing, UI adaptation, component/page/navigation changes
- Design system work and WS07 activities (frontend alignment after backend stabilization)
- Prontuario, Agendamento, Atendimento repository implementation
- PDF report endpoints (`CreateReportByIdAsync`, `CreateReportByCpfAsync`)
- Data migration or import from Google Sheets
- New EF migrations (schema is stable unless stabilization reveals a mapping-only fix requiring none)
- Auth/RBAC implementation (`AtualizadoPor` may remain unset; accepted residual risk)
- Feature flag `Persistence:Provider`
- Financial features
- Any changes under `DocFront.Web/`
- Verification artifact creation (`verification.md`) — owned by Verify phase, not Execute
- Feature report or session-handoff creation — owned by reporting workflow after Verify

## Users And Scenarios

| Actor | Scenario | Outcome |
|-------|----------|---------|
| Developer / agent | Run `dotnet test` with unit and SQL integration tests | All Paciente backend tests pass or skipped with documented reason |
| Developer / agent | Exercise Paciente endpoints via Swagger against Docker SQL | CRUD, single-resource retrieval, and collection search return documented status codes |
| Verifier | Review test output, Swagger checklist, legacy table, SDD traceability | Backend slice meets acceptance criteria with recorded evidence |
| Future WS07 implementer | Consume stabilized API contracts | Can align Blazor without re-litigating backend behavior |

## Acceptance Criteria

- [ ] `REQ-001` — Create and update via API/repository persist Nome, Nascimento, CPF, RG, Email, Telefone, Plano, Carteira, and owned Endereco columns; verified by unit and integration tests
- [ ] `REQ-002` — Deleted patients are excluded from `GetAllAsync`, `GetByIdAsync`, and search methods; verified by repository tests
- [ ] `REQ-003` — `GET /Paciente/{id}` and `GET /Paciente/cpf/{cpf}` return at most one patient; CPF is exact after trim; nome collection search uses partial `Contains` and returns zero or more results; verified by repository and API tests
- [ ] `REQ-004` — POST returns 201 with `ReadPacienteDto`; GET single-resource returns 200 or 404; GET collection search returns 200 (including empty list); PUT/DELETE return 204 or 404; invalid input returns 400; duplicate CPF returns 409; verified by tests and Swagger smoke
- [ ] `REQ-005` — No `Console.WriteLine` or equivalent logging of patient names, CPF, or other PHI in touched Paciente controller code
- [ ] `REQ-006` — At least one SQL integration test class exercises Paciente CRUD against Docker SQL Server; skip reason documented if Docker unavailable (see tasks.md SQL integration criteria)
- [ ] `REQ-007` — Swagger smoke checklist completed for all active Paciente endpoints with recorded status codes
- [ ] `REQ-008` — Legacy behavior table in this document is complete with preserve/adapt/abandon decisions

Frontend compatibility is not a success criterion for this feature.

## Execution Prerequisites

Execute must not begin until all of the following are confirmed:

| Prerequisite | How to confirm |
|--------------|----------------|
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

### Manual validation checklist

Execute performs these checks via Swagger (or equivalent API client) and records results in session notes. Formal verification evidence is produced in the Verify phase.

| Scenario | Endpoint pattern | Expected outcome |
|----------|------------------|------------------|
| Create | POST `/Paciente` | 201 with `ReadPacienteDto`; all fields persisted |
| Read by ID | GET `/Paciente/{id}` | 200 for existing; 404 for missing or soft-deleted |
| Update | PUT `/Paciente/{id}` | 204 for existing; 404 for missing |
| Delete | DELETE `/Paciente/{id}` | 204 soft delete; 404 for missing |
| Duplicate CPF | POST `/Paciente` with existing CPF | 409 |
| Search by nome | GET `/Paciente/search?nome=...` (or equivalent collection search contract) | 200 with array; partial match; empty array when no matches |
| Read by CPF | GET `/Paciente/cpf/{cpf}` | 200 for unique match; 404 when not found |
| Soft delete exclusion | GET after delete | Deleted patient not returned by list, ID, CPF, or nome search |

## Sizing

| Field | Decision |
|-------|----------|
| Size | Large |
| Rationale | CRUD scaffolding exists, but completion requires backend stabilization across domain, SQL persistence, repository, API contracts, and DTO mapping; new SQL integration test infrastructure; PHI remediation; legacy behavior characterization; and harness pilot verification evidence. Brownfield clinical work with security sensitivity escalates beyond Medium. Frontend is out of scope (WS07 deferred). |
| Required phases | Specify / Design / Tasks / Execute / Verify |
| Escalation triggers | CPF uniqueness policy conflicts with import needs; full create payload cannot persist without schema change; integration test infrastructure blocked without Docker SQL |

## Assumptions And Constraints

- ADR-001 soft delete is authoritative; no hard delete for Paciente
- Legacy Google Sheets code is behavioral reference only; nome search is intentionally adapted to partial match
- No Google Sheets re-enablement on the SQL branch
- PHI must not appear in logs, tests, commits, or documentation examples
- SQL runtime uses Docker `docorgano-sql` with `SA_PASSWORD` environment variable
- Approved migration sequence: (1) stabilize domain → (2) stabilize SQL persistence → (3) stabilize API contracts → (4) validate via tests + Swagger → (5) align frontend later (WS07)
- Test fixtures use synthetic data only (no real patient names or CPF)
- Schema from `InitialCreate` migration is stable; no new migrations expected

## Open Questions

1. **CPF format validation** — Is 11-digit length sufficient for MVP2, or is checksum validation required? (Default for Execute: length/format only; checksum deferred.)
2. **Plano/Carteira required** — DTOs mark both as `[Required]`; EF allows null — should validation layer align with DTO or EF? (Default for Execute: align API validation with DTO annotations.)

Resolved during refinement:

- **Patient search API contract** — See `design.md` § API Contract Decision. Single-resource retrieval separated from collection search; nome route no longer uses ambiguous single-result semantics.
- **Integration test connection** — Use `DOCORGANO_TEST_CONNECTION` when set; otherwise fall back to Docker default connection string documented in test project. Skip only when neither is reachable.

## Domain Language

Terms match `Documentation/Architecture/Domain_Overview_Business_Rules.md`:

- **Paciente** — Person served by the clinic; aggregate root for cadastral identity, contact, and insurance (Plano/Carteira)
- **Endereco** — Value object owned by Paciente (Logradouro, Numero, Bairro, Cidade, UF, CEP)
- **Soft delete** — `Deletado` / `DeletadoEm` with global query filter per ADR-001
- **Single-resource retrieval** — Lookup expected to return zero or one patient (by Guid or unique CPF)
- **Collection search** — Lookup expected to return zero or more patients (paginated list, nome partial match)
- **Prontuario / Agendamento / Atendimento** — Related aggregates; out of scope for this feature (repositories stubbed)

## Legacy Behavior

| Behavior | Source | Decision | Notes |
|----------|--------|----------|-------|
| CRUD surface (`GetAll`, `GetById`, CPF/nome search, create, update, delete) | `DocAPI/Legacy/_LegacySheetsDb/PacienteSheetsRepository.cs` | Preserve | Same `IPacienteRepository` contract at repository layer |
| Soft delete | SQL + ADR-001 | Preserve | Domain upgrade over physical row deletion |
| CPF uniqueness | SQL unique index | Preserve | Stronger than legacy (duplicates tolerated in Sheets) |
| Nome search exact match | Legacy `GetAgendamentoByFilterAsync` | Adapt | SQL uses `Contains` partial match; API exposes collection search, not single-result nome route |
| List ordering (sheet row order) | Legacy `GetAllAsync` | Adapt | SQL orders by `Nome` |
| String ID in sheet column E | Legacy mapping | Abandon | SQL uses `Guid` PK |
| Physical row delete | Legacy `DeleteLineAsync` | Abandon | ADR-001 soft delete |
| `"0"` null sentinels in sheet writes | Legacy `CreatePacienteToSheet` | Abandon | SQL uses nullable columns |
| PDF reports by ID/CPF | Legacy (commented) | Abandon | Out of scope |
| No repository-layer validation | Legacy | Abandon | DTO DataAnnotations + EF constraints replace; domain validation layer optional for MVP2 |

## Security And PHI

- Security/PHI review needed: **Yes**
- Rationale: Feature touches patient identity (Nome, CPF, RG, Email, Telefone, Endereco), API exposure, logs, and test fixtures. Controller currently logs PHI via `Console.WriteLine`. Remediation is in scope.

## Initial Verification Expectations

| Requirement | Expected evidence |
|-------------|-------------------|
| `REQ-001` | Unit tests for full field round-trip; SQL integration test create/read/update |
| `REQ-002` | Repository test confirming soft-deleted patient excluded from queries |
| `REQ-003` | Repository tests for CPF exact single-resource; nome partial collection search |
| `REQ-004` | Swagger smoke checklist; automated tests for status codes where practical |
| `REQ-005` | Code review / security-phi-review sensor; no PHI in controller logs |
| `REQ-006` | `dotnet test` output for SQL integration test class or documented skip |
| `REQ-007` | Completed Swagger smoke checklist (endpoint + status code per route) |
| `REQ-008` | Legacy behavior table in this document reviewed and accepted |

## References

- `Documentation/State.md`
- `AGENTS.md`
- `Documentation/Product/PM_DocOrgano.md`
- `Documentation/Product/PRD.md`
- `Documentation/Technical/migration-sql.md`
- `Documentation/Architecture/ADR/ADR-001-soft-delete.md`
- `Documentation/Architecture/Domain_Overview_Business_Rules.md`
- `Documentation/AI-Harness/Harness-Design/sdd-operational.md`
- `Documentation/AI-Harness/Harness-Design/verification-governance.md`
