# Tasks — Atendimento Minimal SQL Stabilization

> Inputs: `specify.md` and `design.md` in `Documentation/SDD/atendimento-minimal-sql-stabilization/`  
> Generated SDD artifacts are written in English.

## Execution Boundaries

### Included

- Backend stabilization of Atendimento aggregate factory, domain methods, EF config review, **persistence-only** repository SQL implementation, Controller/Application Paciente FK validation, Guid API alignment, slim DTOs (no workflow DTOs), unit tests, **required** SQL integration tests, PHI remediation, and Swagger runtime validation per `specify.md` Runtime Validation Environment
- Legacy characterization sign-off (documentation review only)
- Session notes capturing Swagger smoke results and test output for Verify handoff

### Excluded — Execute must NOT

| Category | Excluded work |
|----------|---------------|
| Frontend | Any changes under `DocFront.Web/`, Blazor pages, UI smoke, frontend compatibility fixes |
| Workflow | `ValidacaoEtapa*`, stage evaluators, pendência engine, journey projections, workflow DTOs, `AvancarEtapa` API exposure |
| Child persistence | Writes to `ClinicalEvent`, `AtendimentoPendencia`, `ChecklistExecution` |
| Other aggregates | Prontuario, Agendamento repository implementation |
| Reports | PDF report **implementation** beyond 501 deferral — routes must **remain present** returning 501 |
| Repository validation | Paciente FK checks, business rules, or cross-aggregate reads inside `AtendimentoRepository` |
| Legacy revival | Re-enabling Google Sheets, modifying `Legacy/_LegacySheetsDb/` |
| Data import | Sheets or production data migration |
| Auth | RBAC, `AtualizadoPor` population, auth ADR work |
| Schema | New EF migrations unless stabilization reveals unavoidable mapping fix (escalate first) |
| Verify artifacts | Creating or finalizing `verification.md` (Verify phase) |
| Reporting artifacts | `reports/feature-report.md`, `reports/session-handoff.md` (reporting workflow) |
| State / PM updates | Direct edits to `Documentation/State.md`, PM, or RoadMap (Documentation Follow-Up after Verify) |
| Scope expansion | Financial features, auto-create Atendimento in Prontuario/Agendamento |

Current execution does not begin until SDD Pre-Execution Review exit criteria are satisfied and Execution Prerequisites in `specify.md` are confirmed.

## Execution Prerequisites

Confirm before `TASK-003` (first implementation task):

- [ ] Paciente SQL slice verified (upstream FK source)
- [ ] Docker SQL container `docorgano-sql` running
- [ ] `SA_PASSWORD` set
- [ ] `dotnet ef database update --project DocAPI/DocAPI.csproj` succeeded
- [ ] `dotnet run --project DocAPI/DocAPI.csproj` starts successfully
- [ ] Baseline `dotnet test` executed; result recorded in session notes
- [ ] Swagger UI reachable

## Task List

### Pre-implementation

- [ ] `TASK-001` — Legacy characterization sign-off
  - **Requirements:** `REQ-010`
  - **Files:** `Documentation/SDD/atendimento-minimal-sql-stabilization/specify.md` (Legacy Behavior section)
  - **Depends on:** None
  - **Tests or checks:** Review legacy table completeness; confirm preserve/adapt/abandon decisions
  - **Done when:** Legacy behavior table is complete and accepted; no code changes required

- [ ] `TASK-002` — Review Atendimento aggregate and EF configuration
  - **Requirements:** `REQ-001`, `REQ-004`, `REQ-005`
  - **Files:** `DocAPI/Core/Entities/Atendimento.cs`, `DocAPI/Infrastructure/SqlDb/Configurations/AtendimentoConfig.cs`, `DocAPI/Infrastructure/SqlDb/DbContext/DbContext.cs`
  - **Depends on:** `TASK-001`
  - **Tests or checks:** Entity consistency review; configuration audit (FK, soft-delete filter, `EtapaAtual` conversion)
  - **Done when:** Entity has factory/create path (`PacienteId` required; optional `MensagemParaMedico`); limited update method (`MensagemParaMedico` only); soft-delete method; `EtapaAtual` not client-writable; EF config confirmed or gap documented

### Implementation

- [ ] `TASK-003` — Align repository interface and slim DTOs to Guid contract
  - **Requirements:** `REQ-007`
  - **Files:** `DocAPI/Core/Interfaces/Repositories/IAtendimentoRepository.cs`, `DocAPI/Application/Data/Dtos/Atendimento/CreateAtendimentoDto.cs`, `UpdateAtendimentoDto.cs`, `ReadAtendimentoDto.cs`, `DocAPI/Application/Mappings/Profiles/AtendimentoProfile.cs`
  - **Depends on:** `TASK-002`, Execution Prerequisites
  - **Tests or checks:** Solution compiles; abandoned Legacy DTO fields removed
  - **Done when:** Interface uses `Guid`; report methods removed from interface; slim DTOs match `design.md` API Contract — create has `PacienteId` + optional `MensagemParaMedico` only; update has `MensagemParaMedico` only; no workflow DTOs or `EtapaAtual` on write DTOs; read mapping explicit

- [ ] `TASK-004` — Implement `AtendimentoRepository` SQL CRUD (persistence-only)
  - **Requirements:** `REQ-001`, `REQ-002`, `REQ-003`, `REQ-004`, `REQ-005`
  - **Files:** `DocAPI/Infrastructure/Repositories/AtendimentoRepository.cs`
  - **Depends on:** `TASK-003`
  - **Tests or checks:** Manual or unit verification per operation before `TASK-005`
  - **Done when:** `GetAllAsync`, `GetByIdAsync`, `GetByPacienteIdAsync`, `CreateAsync`, `UpdateAsync`, `DeleteAsync` implemented against `DocDbContext`; soft delete via entity method; **no** Paciente FK validation, **no** workflow logic, **no** cross-aggregate reads

- [ ] `TASK-005` — Repository unit tests
  - **Requirements:** `REQ-001` through `REQ-005`
  - **Files:** `DocAPI.Tests/Infrastructure/AtendimentoRepositoryTests.cs` (new)
  - **Depends on:** `TASK-004`
  - **Tests or checks:** `dotnet test --filter AtendimentoRepository`
  - **Done when:** Tests cover create (Consulta initial stage via factory), read, list-by-paciente (including two active per patient), update message only (EtapaAtual unchanged), soft-delete exclusion; repository tests do **not** cover Paciente FK validation (owned by Controller/Application)

- [ ] `TASK-006` — Controller hardening (Guid, Paciente FK validation, status codes, PHI, 501 reports)
  - **Requirements:** `REQ-006`, `REQ-007`, `REQ-009`
  - **Files:** `DocAPI/API/Controllers/AtendimentoController.cs`
  - **Depends on:** `TASK-004`
  - **Tests or checks:** Code review; automated tests where practical; negative test invalid PacienteId → 404
  - **Done when:** Guid route params; aggregate factory used on create (not blind AutoMapper); Paciente FK validated via `IPacienteRepository` before repository create; POST returns 201 + `ReadAtendimentoDto`; PUT/DELETE return 204 or 404; PHI logging removed; report/followUp routes **remain present** and return 501 without repository call; `GET /Atendimento/paciente/{pacienteId}` added; no stage-advance endpoints

- [ ] `TASK-007` — SQL integration tests
  - **Requirements:** `REQ-008`
  - **Files:** `DocAPI.Tests/Integration/AtendimentoSqlIntegrationTests.cs` (new)
  - **Depends on:** `TASK-005`
  - **Tests or checks:** `dotnet test --filter AtendimentoSql`
  - **Done when:** Integration tests meet SQL Integration Testing criteria below

- [ ] `TASK-008` — Swagger runtime validation
  - **Requirements:** All API-facing requirements
  - **Files:** Session notes only (not `verification.md`)
  - **Depends on:** `TASK-006`, `TASK-007`
  - **Tests or checks:** Manual checklist per `specify.md` Runtime Validation Environment
  - **Done when:** All scenarios recorded: Create, Read, List, ListByPaciente, Update, Delete, Invalid PacienteId, Soft delete exclusion, Report routes 501 (routes present)

- [ ] `TASK-009` — Build and automated test gate
  - **Requirements:** All (`REQ-001` through `REQ-010`)
  - **Files:** Solution-wide
  - **Depends on:** `TASK-005` through `TASK-008`
  - **Tests or checks:** `dotnet build`, `dotnet test`
  - **Done when:** Build succeeds; all non-skipped tests pass

## SQL Integration Testing

SQL integration tests are **required** for this feature. A passing run against Docker SQL is the expected success outcome. Skip is permitted only for documented operational or environment blockers and is **not** expected success behavior.

### Mandatory verification expectations

When Docker SQL is reachable (Execution Prerequisites met):

| Expectation | Detail |
|-------------|--------|
| Test class exists | `AtendimentoSqlIntegrationTests` for repository CRUD — **required** |
| Operations covered | Create Paciente → create Atendimento → read → update → soft delete → post-delete exclusion |
| Connection source | `DOCORGANO_TEST_CONNECTION` if set; otherwise documented Docker default |
| Synthetic data only | No real patient names, CPF, or clinical data |
| Success evidence | **Passing** `dotnet test` output recorded in session notes |
| Passing run | Expected when Execution Prerequisites are met at start of Execute |

### Acceptable skip conditions

Skip is **not** expected success behavior. Integration tests may skip (not fail) only when operational/environment blockers prevent Docker SQL access:

| Condition | Required action |
|-----------|-------------------|
| Docker container not running | Skip with reason; document in session notes; Verify records residual risk |
| `DOCORGANO_TEST_CONNECTION` unreachable and Docker default unreachable | Skip with reason |
| `SA_PASSWORD` not set | Skip with reason |
| Database not migrated | Skip with reason — do not skip if prerequisites were confirmed at start |

Skip must use explicit test framework skip (e.g., `SkippableFact`, conditional `[Fact(Skip = "...")]`), not silent pass or empty test body.

### Completion criteria for REQ-008

| Outcome | REQ-008 status |
|---------|----------------|
| Integration tests pass against Docker SQL | **Met** — expected success |
| Integration tests skipped with documented operational/environment reason | **Met with residual risk** — Verify records follow-up; not treated as full success |
| No integration test class added | **Not met** |
| Tests fail due to code defects | **Not met** — fix before Execute complete |

## Verification Preparation

Post-implementation work prepared for the Verify phase. **Not part of Execute implementation.**

- [ ] `VP-001` — Verification handoff preparation
  - **Requirements:** All
  - **Owner:** Verify phase (may be drafted during Execute session notes)
  - **Depends on:** `TASK-009`, `TASK-008`
  - **Artifacts:** `Documentation/SDD/atendimento-minimal-sql-stabilization/verification.md` (created in Verify, not Execute)
  - **Done when:** `verification.md` lists gates run, evidence paths, skipped gates with reasons, residual risks, and requirement traceability status

## Documentation Follow-Up Preparation

Post-verify routing candidates. **Not part of Execute implementation.**

- [ ] `DF-001` — Documentation follow-up routing
  - **Requirements:** Governance
  - **Owner:** Documentation Update after Verify
  - **Depends on:** `VP-001`
  - **Artifacts:** Handoff notes; Documentation Update skill routing
  - **Done when:** Follow-up candidates evaluated for State, `migration-sql.md`, PM, optional API contract doc, skills

## Dependency Map

| Task | Depends on | Can run in parallel with | Notes |
|------|------------|--------------------------|-------|
| `TASK-001` | None | Prerequisites confirmation | Documentation only |
| `TASK-002` | `TASK-001` | — | Entity + EF review |
| `TASK-003` | `TASK-002`, Prerequisites | — | Interface + DTO alignment |
| `TASK-004` | `TASK-003` | — | Critical path — repository |
| `TASK-005` | `TASK-004` | — | Unit tests after repo |
| `TASK-006` | `TASK-004` | `TASK-005` (after repo stable) | Controller after repo |
| `TASK-007` | `TASK-005` | — | Integration after unit coverage |
| `TASK-008` | `TASK-006`, `TASK-007` | — | Swagger after API + SQL ready |
| `TASK-009` | `TASK-005`–`TASK-008` | — | Final automated gate |
| `VP-001` | `TASK-009`, `TASK-008` | — | Verify phase |
| `DF-001` | `VP-001` | — | Post-verify |

## Requirement Traceability

| Requirement | Tasks | Tests or checks | Verification evidence |
|-------------|-------|-----------------|-----------------------|
| `REQ-001` | `TASK-002`, `TASK-004`, `TASK-005` | Unit + integration create | Test output |
| `REQ-002` | `TASK-004`, `TASK-005` | GetById; missing id | Test output |
| `REQ-003` | `TASK-004`, `TASK-005`, `TASK-006` | ListByPaciente; two per patient | Test output + Swagger |
| `REQ-004` | `TASK-002`, `TASK-004`, `TASK-005` | Update message; stage unchanged | Test output |
| `REQ-005` | `TASK-002`, `TASK-004`, `TASK-005` | Soft-delete exclusion | Test output |
| `REQ-006` | `TASK-006` | Controller negative test invalid PacienteId | Test output + Swagger |
| `REQ-007` | `TASK-003`, `TASK-006` | Guid contracts; Swagger | Code review + session notes |
| `REQ-008` | `TASK-007` | SQL integration tests — passing run required | Test output; skip = residual risk only |
| `REQ-009` | `TASK-006` | security-phi-review sensor | Code review |
| `REQ-010` | `TASK-001` | Legacy table review | specify.md table |

## Verification Expectations

| Gate category | Expected / Not expected | Evidence or rationale |
|---------------|-------------------------|-----------------------|
| Build | Expected | `dotnet build` |
| Automated tests | Expected | `dotnet test` unit + integration |
| SQL / Persistence | Expected | **Passing** Docker SQL integration test run required; skip only for operational/environment blockers with residual risk |
| API | Expected | Swagger smoke checklist in session notes (including report routes 501, routes present) |
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
| SQL integration tests skipped | Operational/environment blocker — **not** expected success | Document skip reason; Verify records residual risk; local Docker rerun required for full verify |
| Workflow validations deferred | Separate SDD | `atendimento-workflow-stabilization` |
| Report endpoints | Implementation deferred | Routes preserved; 501 at controller; future workflow/report feature |
| UI smoke | Out of scope — WS07 | PM WS07 future feature |
| Repository business validation | Architectural constraint | Paciente FK in Controller/Application only |
| `AtualizadoPor` unset | Auth ADR not in scope | Accepted residual risk |
| Prontuario blocked until Verify | Downstream dependency | Execute Prontuario only after this slice verified |

## Documentation Follow-up Candidates

- State: Update Atendimento Minimal status; next step Prontuario SQL Stabilization
- ADR: None expected
- Architecture docs: None unless domain patterns formalized
- Technical docs: `migration-sql.md` Atendimento Minimal checklist; optional `api-contract.md` Atendimento section
- Rules: security-phi if new patterns emerge
- Skills: sql-migration-workflow — second aggregate pattern
- Review prompts: None unless findings require updates
- Templates: SDD sequencing calibration for aggregate #2
- Active SDD: Mark complete after Verify phase

## Commit Guidance

- Keep commits atomic and focused per task slice
- Do not commit secrets, `.env` files, credentials, real patient data, or clinical data
- Commit message should reflect change type and scope (e.g., `feat(api): implement Atendimento SQL repository with Guid contract`)

## Completion Handoff

Before marking Execute complete, provide material for Verify (via session notes; formal artifact is VP-001):

- Implemented task list with status per TASK-001 through TASK-009
- Requirement traceability status for REQ-001 through REQ-010
- Tests/checks run (`dotnet build`, `dotnet test`, Swagger checklist)
- Tests/checks skipped with reasons per SQL Integration Testing policy
- Review sensors applied or deferred to Verify
- Residual risks (workflow deferred, reports 501-only with routes preserved, `AtualizadoPor`, SQL skip if applicable — skip is not success)
- Documentation follow-up candidates for DF-001 routing
