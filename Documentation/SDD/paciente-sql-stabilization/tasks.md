# Tasks - Paciente SQL Stabilization

> Inputs: `specify.md` and `design.md` in `Documentation/SDD/paciente-sql-stabilization/`
> Generated SDD artifacts are written in English.

## Execution Boundaries

### Included

- Backend stabilization of Paciente domain persistence, repository behavior, API contracts, unit tests, SQL integration tests, PHI remediation, and Swagger runtime validation per `specify.md` Runtime Validation Environment
- Legacy characterization sign-off (documentation review only)
- Session notes capturing Swagger smoke results and test output for Verify handoff

### Excluded — Execute must NOT

| Category | Excluded work |
|----------|---------------|
| Frontend | Any changes under `DocFront.Web/`, Blazor pages, UI smoke, frontend compatibility fixes |
| Schema | New EF migrations, schema changes, financial tables |
| Other aggregates | Prontuario, Agendamento, Atendimento repository implementation |
| Legacy revival | Re-enabling Google Sheets, modifying `Legacy/_LegacySheetsDb/` |
| Reports | PDF report endpoints |
| Data import | Sheets or production data migration |
| Auth | RBAC, `AtualizadoPor` population, auth ADR work |
| Infrastructure flags | `Persistence:Provider` feature flag |
| Verify artifacts | Creating or finalizing `verification.md` (Verify phase) |
| Reporting artifacts | `reports/feature-report.md`, `reports/session-handoff.md` (reporting workflow) |
| State / PM updates | Direct edits to `Documentation/State.md`, PM, or RoadMap (Documentation Follow-Up after Verify) |
| Scope expansion | Financial features, new API endpoints beyond Paciente stabilization contract |

Current execution does not begin until this task plan is accepted and Execution Prerequisites in `specify.md` are satisfied.

## Execution Prerequisites

Confirm before TASK-002 (first implementation task):

- [ ] Docker SQL container `docorgano-sql` running
- [ ] `SA_PASSWORD` set
- [ ] `dotnet ef database update --project DocAPI/DocAPI.csproj` succeeded
- [ ] `dotnet run --project DocAPI/DocAPI.csproj` starts successfully
- [ ] Baseline `dotnet test` executed; result recorded in session notes
- [ ] Swagger UI reachable

## Task List

### Pre-implementation

- [ ] `TASK-001` - Legacy characterization sign-off
  - **Requirements:** `REQ-008`
  - **Files:** `Documentation/SDD/paciente-sql-stabilization/specify.md` (Legacy Behavior section)
  - **Depends on:** None
  - **Tests or checks:** Review legacy table completeness; confirm preserve/adapt/abandon decisions
  - **Done when:** Legacy behavior table is complete and accepted; no code changes required

### Implementation

- [ ] `TASK-002` - Ensure full field persistence on create/update
  - **Requirements:** `REQ-001`
  - **Files:** Paciente domain, mapping, and related application layer as needed
  - **Depends on:** `TASK-001`, Execution Prerequisites
  - **Tests or checks:** Manual or automated check that all DTO fields persist before TASK-003
  - **Done when:** Create and update persist Nome, Nascimento, CPF, RG, Email, Telefone, Plano, Carteira, and Endereco

- [ ] `TASK-003` - Add mapping/persistence unit tests for full field round-trip
  - **Requirements:** `REQ-001`
  - **Files:** `DocAPI.Tests/` (repository or mapping tests)
  - **Depends on:** `TASK-002`
  - **Tests or checks:** `dotnet test --filter Paciente`
  - **Done when:** Tests assert Plano, Carteira, RG, and all Endereco fields persist on create and update

- [ ] `TASK-004` - Expand repository unit tests
  - **Requirements:** `REQ-002`, `REQ-003`
  - **Files:** `DocAPI.Tests/Infrastructure/PacienteRepositoryTests.cs`
  - **Depends on:** None (parallel with TASK-002 after prerequisites)
  - **Tests or checks:** `dotnet test --filter PacienteRepository`
  - **Done when:** Tests cover pagination/ordering, nome partial collection search, CPF exact lookup, and soft-delete query filter exclusion

- [ ] `TASK-005` - API stabilization (status codes, PHI, create response)
  - **Requirements:** `REQ-004`, `REQ-005`
  - **Files:** `DocAPI/API/Controllers/PacienteController.cs`
  - **Depends on:** `TASK-002`
  - **Tests or checks:** Code review; automated tests where practical
  - **Done when:** PHI logging removed; duplicate CPF returns 409; missing resource returns 404; POST returns `ReadPacienteDto` with 201; PUT/DELETE return 204 or 404

- [ ] `TASK-006` - Implement patient search API contract
  - **Requirements:** `REQ-003`, `REQ-004`
  - **Files:** `DocAPI/API/Controllers/PacienteController.cs`, repository if query exposure needed
  - **Depends on:** `TASK-004`, `TASK-005`
  - **Tests or checks:** Repository and API tests; Swagger collection search scenarios
  - **Done when:** Single-resource routes (`/{id}`, `/cpf/{cpf}`) and collection search route per `design.md` API Contract Decision are implemented; retired nome single-result semantics removed; 200 + empty array for zero nome matches

- [ ] `TASK-007` - SQL integration tests
  - **Requirements:** `REQ-006`
  - **Files:** `DocAPI.Tests/Integration/` (new or updated test class)
  - **Depends on:** `TASK-003`, `TASK-004`
  - **Tests or checks:** `dotnet test --filter PacienteSql` (or project filter)
  - **Done when:** Integration tests meet SQL Integration Testing criteria below

- [ ] `TASK-008` - Swagger runtime validation
  - **Requirements:** `REQ-007`
  - **Files:** Session notes only (not `verification.md`)
  - **Depends on:** `TASK-006`, `TASK-007`
  - **Tests or checks:** Manual checklist per `specify.md` Runtime Validation Environment
  - **Done when:** All scenarios recorded: Create, Read, Update, Delete, Duplicate CPF, Search, Soft delete

- [ ] `TASK-009` - Build and automated test gate
  - **Requirements:** All (`REQ-001` through `REQ-008`)
  - **Files:** Solution-wide
  - **Depends on:** `TASK-003` through `TASK-008`
  - **Tests or checks:** `dotnet build`, `dotnet test`
  - **Done when:** Build succeeds; all non-skipped tests pass

## SQL Integration Testing

### Mandatory verification expectations

When Docker SQL is reachable (Execution Prerequisites met):

| Expectation | Detail |
|-------------|--------|
| Test class exists | At least one integration test class for Paciente repository CRUD |
| Operations covered | Create, read by ID, update, soft delete, and post-delete read exclusion |
| Connection source | `DOCORGANO_TEST_CONNECTION` if set; otherwise documented Docker default |
| Synthetic data only | No real patient names, CPF, or clinical data |
| Evidence | Passing `dotnet test` output recorded in session notes |

### Acceptable skip conditions

Integration tests may skip (not fail) only when:

| Condition | Required action |
|-----------|-------------------|
| Docker container not running | Skip with reason; document in session notes |
| `DOCORGANO_TEST_CONNECTION` unreachable and Docker default unreachable | Skip with reason |
| `SA_PASSWORD` not set | Skip with reason |
| Database not migrated | Skip with reason — do not skip if prerequisites were confirmed at start |

Skip must use explicit test framework skip (e.g., `Skip.If`, conditional `[Fact(Skip = "...")]`), not silent pass or empty test body.

### Completion criteria for REQ-006

| Outcome | REQ-006 status |
|---------|----------------|
| Integration tests pass against Docker SQL | **Met** |
| Integration tests skipped with documented reason and prerequisites were attempted | **Met with residual risk** — Verify must accept skip and record follow-up for local Docker run |
| No integration test class added | **Not met** |
| Tests fail due to code defects | **Not met** — fix before Execute complete |

Full verification without skip requires a passing SQL integration run on a machine with Docker SQL available.

## Verification Preparation

Post-implementation work prepared for the Verify phase. **Not part of Execute implementation.**

- [ ] `VP-001` - Verification handoff preparation
  - **Requirements:** All
  - **Owner:** Verify phase (may be drafted during Execute session notes)
  - **Depends on:** `TASK-009`, `TASK-008`
  - **Artifacts:** `Documentation/SDD/paciente-sql-stabilization/verification.md` (created in Verify, not Execute)
  - **Done when:** `verification.md` lists gates run, evidence paths, skipped gates with reasons, residual risks, and requirement traceability status

## Documentation Follow-Up Preparation

Post-verify routing candidates. **Not part of Execute implementation.**

- [ ] `DF-001` - Documentation follow-up routing
  - **Requirements:** Governance
  - **Owner:** Documentation Update after Verify
  - **Depends on:** `VP-001`
  - **Artifacts:** Handoff notes; Documentation Update skill routing
  - **Done when:** Follow-up candidates evaluated for State, `migration-sql.md`, PM, optional API contract doc, skills

## Dependency Map

| Task | Depends on | Can run in parallel with | Notes |
|------|------------|--------------------------|-------|
| `TASK-001` | None | Prerequisites confirmation | Documentation only |
| `TASK-002` | `TASK-001`, Prerequisites | — | Critical path |
| `TASK-003` | `TASK-002` | — | Requires persistence fix |
| `TASK-004` | Prerequisites | `TASK-002` | Independent unit tests |
| `TASK-005` | `TASK-002` | — | API depends on persistence |
| `TASK-006` | `TASK-004`, `TASK-005` | — | Search contract after repo + API base |
| `TASK-007` | `TASK-003`, `TASK-004` | — | Integration after unit coverage |
| `TASK-008` | `TASK-006`, `TASK-007` | — | Swagger after API + SQL ready |
| `TASK-009` | `TASK-003`–`TASK-008` | — | Final automated gate |
| `VP-001` | `TASK-009`, `TASK-008` | — | Verify phase |
| `DF-001` | `VP-001` | — | Post-verify |

## Requirement Traceability

| Requirement | Tasks | Tests or checks | Verification evidence |
|-------------|-------|-----------------|-----------------------|
| `REQ-001` | `TASK-002`, `TASK-003` | Unit + integration field round-trip | Test output |
| `REQ-002` | `TASK-004` | Soft-delete exclusion test | Test output |
| `REQ-003` | `TASK-004`, `TASK-006` | CPF single-resource; nome collection search | Test output + Swagger search |
| `REQ-004` | `TASK-005`, `TASK-006` | Swagger smoke; status code tests | Session notes + test output |
| `REQ-005` | `TASK-005` | security-phi-review sensor | Code review |
| `REQ-006` | `TASK-007` | SQL integration tests | Test output or skip reason |
| `REQ-007` | `TASK-008` | Swagger manual smoke | Session notes |
| `REQ-008` | `TASK-001` | Legacy table review | specify.md table |

## Verification Expectations

| Gate category | Expected / Not expected | Evidence or rationale |
|---------------|-------------------------|-----------------------|
| Build | Expected | `dotnet build` |
| Automated tests | Expected | `dotnet test` unit + integration |
| SQL / Persistence | Expected | Docker SQL integration test results or documented skip |
| API | Expected | Swagger smoke checklist in session notes |
| UI | Not expected | Frontend deferred to WS07 |
| Security / PHI | Expected | security-phi-review sensor; no PHI logs |
| Domain review | Expected | domain-review sensor |
| Documentation review | Expected | check-docs on SDD and touched docs |
| Test strategy review | Expected | test-strategy sensor |
| ADR evaluation | Not expected | No new ADR; ADR-001 referenced only |
| Legacy characterization | Expected | Legacy table in specify.md |

## Review Sensors

- [ ] `Documentation/AI-Harness/review-prompts/domain-review.md`
- [ ] `Documentation/AI-Harness/review-prompts/security-phi-review.md`
- [ ] `Documentation/AI-Harness/review-prompts/check-docs.md`
- [ ] `Documentation/AI-Harness/review-prompts/test-strategy.md`

## Known Risks And Skipped Checks

| Risk or skipped check | Reason | Owner / follow-up |
|-----------------------|--------|-------------------|
| SQL integration tests skipped | Docker SQL unavailable in environment | Document skip reason; Verify accepts residual risk or requires local Docker rerun |
| UI smoke | Out of scope — WS07 | PM WS07 future feature |
| CPF checksum validation | Open question; not required for MVP2 | Future domain enhancement |
| `AtualizadoPor` unset | Auth ADR not in scope | Accepted residual risk |
| Frontend API consumption | Backend-only stabilization | WS07 alignment feature |
| Nome route contract change | Single-result route retired | WS07 adopts collection search |

## Documentation Follow-up Candidates

- State: Update Paciente backend stabilization status and next steps
- ADR: None expected
- Architecture docs: None unless domain validation added
- Technical docs: `migration-sql.md` Paciente checklist; optional API contract doc for Paciente endpoints
- Rules: security-phi if new patterns emerge
- Skills: sql-migration-workflow if integration test pattern standardizes
- Review prompts: None unless findings require updates
- Templates: SDD pilot calibration notes
- Active SDD: Mark complete after Verify phase

## Commit Guidance

- Keep commits atomic and focused per task slice
- Do not commit secrets, `.env` files, credentials, real patient data, or clinical data
- Commit message should reflect change type and scope (e.g., `fix(api): stabilize Paciente search contract and remove PHI logs`)

## Completion Handoff

Before marking Execute complete, provide material for Verify (via session notes; formal artifact is VP-001):

- Implemented task list with status per TASK-001 through TASK-009
- Requirement traceability status for REQ-001 through REQ-008
- Tests/checks run (`dotnet build`, `dotnet test`, Swagger checklist)
- Tests/checks skipped with reasons per SQL Integration Testing policy
- Review sensors applied or deferred to Verify
- Residual risks (`AtualizadoPor`, CPF checksum, frontend deferred to WS07, SQL skip if applicable)
- Documentation follow-up candidates for DF-001 routing
