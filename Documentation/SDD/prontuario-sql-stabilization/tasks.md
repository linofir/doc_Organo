# Tasks — Prontuario SQL Stabilization

> Inputs: `specify.md` and `design.md` in `Documentation/SDD/prontuario-sql-stabilization/`  
> Generated SDD artifacts are written in English.

## Execution Boundary

### Included

- Backend stabilization of Prontuario aggregate factories and domain methods (`AplicarCorrecao`, `CriarNovaVersao`, soft delete per version), **persistence-only** `ProntuarioRepository` SQL implementation (composite graph + versioning lookup), Controller/Application orchestration (Atendimento FK validation, correction vs evolution routing, **409** on concurrent evolution), Guid API alignment, dedicated DTOs (**D-01** structural correction contract), explicit read projection (no AutoMapper on write paths), unit tests, **required** SQL integration tests with full fixture chain, PHI remediation, and Swagger runtime validation intent per `specify.md` Runtime Validation Environment
- Legacy characterization and field classification sign-off (documentation review)
- Session notes capturing Swagger smoke results and test output for Verify handoff

### Excluded — Execute must NOT

| Category | Excluded work |
|----------|---------------|
| Frontend | Any changes under `DocFront.Web/`, Blazor, UI smoke, WS07 contract alignment |
| Atendimento | Re-implement `AtendimentoRepository` / controller — **verified prerequisite** only |
| Workflow | Stage evaluators, pendências, `ClinicalEvent` writes, journey projections, workflow DTOs/services (**REQ-017**) |
| Agendamento | Repository implementation |
| PDF | `POST /Prontuario/from-pdf` implementation — route may return **501** only |
| CID catalog | Production seed or admin UI — synthetic CID in test fixtures only |
| Legacy revival | Re-enabling Google Sheets, modifying `Legacy/_LegacySheetsDb/` for runtime |
| Auth | RBAC, `AtualizadoPor` population |
| Schema | New EF migrations unless stabilization reveals unavoidable fix (**escalate first**) |
| Verify artifacts | Creating or finalizing `verification.md` (**Verify** phase) |
| Reporting | `reports/feature-report.md`, `reports/session-handoff.md` |
| State / PM updates | Direct edits to `Documentation/State.md`, PM, `migration-sql.md`, `runbook.md` (**Documentation Follow-Up** after Verify) |
| Teacher Guide | `teacher-guide.md` generation |
| Scope expansion | Financial features, workflow logic, auto-create Atendimento inside Prontuario create |

**Explicitly excluded from Execute:** verification artifacts, reporting, project documentation updates, Teacher Guide generation.

Current execution does **not** begin until **SDD Pre-Execution Review** exit criteria are satisfied and **Execution Prerequisites** in `specify.md` are confirmed (Credential Probe, baseline **27 tests**).

**Pre-Execution Review:** Complete — see [reports/pre-execution-review-2026-06-19.md](reports/pre-execution-review-2026-06-19.md). **[ADR-006](../../Architecture/ADR/ADR-006-prontuario-dual-mode-versioning-api.md)** accepted. **Execute authorized** from TASK-001.

## Execution Prerequisites

Confirm before `TASK-004` (first repository implementation task).

**`TASK-001` through `TASK-003` may proceed without Docker or Credential Probe** (documentation sign-off, domain methods, DTO/interface alignment). **`TASK-004` and later require the full checklist below.**

Credential Probe executed **2026-06-19** — see [reports/pre-execution-review-2026-06-19.md](reports/pre-execution-review-2026-06-19.md).

- [x] Paciente SQL slice verified (`Documentation/SDD/paciente-sql-stabilization/verification.md`)
- [x] Atendimento Minimal SQL slice verified (`Documentation/SDD/atendimento-minimal-sql-stabilization/verification.md`)
- [x] Docker SQL container `docorgano-sql` running
- [x] `SA_PASSWORD` set (Credential Probe per `specify.md`) — inferred from passing SQL integration tests
- [x] `dotnet ef database update --project DocAPI/DocAPI.csproj` succeeded — schema reachable (SQL tests pass); full command not re-run while DocAPI process locked binaries
- [x] `dotnet run --project DocAPI/DocAPI.csproj` starts successfully — DocAPI process active locally
- [x] Baseline `dotnet test` executed — **27/27** recorded in [pre-execution-review-2026-06-19.md](reports/pre-execution-review-2026-06-19.md)
- [x] Swagger UI reachable — DocAPI running at dev URL (default `https://localhost:7004/swagger`)
- [x] **DQ-009 / ADR:** **[ADR-006](../../Architecture/ADR/ADR-006-prontuario-dual-mode-versioning-api.md)** accepted at Pre-Execution Review (2026-06-19)

### Version generation authority (D-02 vs Specify)

`specify.md` states the aggregate "owns version generation." **`design.md` D-02** and **[ADR-006](../../Architecture/ADR/ADR-006-prontuario-dual-mode-versioning-api.md)** are authoritative for Execute: the **Application layer** (controller orchestration) obtains `max(Versao)` via `GetMaxVersaoForPacienteAsync`; the **aggregate** receives `nextVersao`, validates the versioning rule, and assigns identity — it **must not** query the database. Reconciled at Pre-Execution Review 2026-06-19.

## Implementation Tasks (TASK)

### Pre-implementation

- [ ] `TASK-001` — Legacy characterization and field classification sign-off
  - **Requirements:** `REQ-014`
  - **Files:** `Documentation/SDD/prontuario-sql-stabilization/specify.md` (Legacy Behavior); `design.md` (Field Classification, D-01–D-06)
  - **Depends on:** None
  - **Tests or checks:** Review preserve/adapt/abandon table; confirm correction vs evolution API semantics; confirm `UpdateProntuarioDto` structural rule
  - **Done when:** Legacy table and design field classification accepted; no code changes required

- [ ] `TASK-002` — Implement Prontuario aggregate domain methods and review EF configuration
  - **Requirements:** `REQ-001`, `REQ-006`, `REQ-007`, `REQ-008`
  - **Files:** `DocAPI/Core/Entities/Prontuario.cs` (+ owned types as needed), `DocAPI/Infrastructure/SqlDb/Configurations/ProntuarioConfig.cs`, child configs, `DocDbContext`
  - **Depends on:** `TASK-001`
  - **Tests or checks:** Entity review; configuration audit (FKs, soft-delete filter, `(PacienteId, Versao)` unique index)
  - **Done when:** Factory for v1 create accepts full clinical payload and supports identity snapshot fields (`DescricaoBasica` Nome/CPF/Idade from caller — populated by controller from `Paciente` per **REQ-001**); `AplicarCorrecao` for correction-safe fields only; `CriarNovaVersao(source, nextVersao, payload)` validates versioning rule (**D-02** — aggregate validates assigned `nextVersao`, does **not** query DB or compute max from persistence); `MarcarComoExcluido` per version

### Contract and interface

- [ ] `TASK-003` — Align repository interface and dedicated DTOs to Guid; remove AutoMapper write paths
  - **Requirements:** `REQ-011`, `REQ-018`, `REQ-006` (D-01)
  - **Files:** `DocAPI/Core/Interfaces/Repositories/IProntuarioRepository.cs`, `DocAPI/Application/Data/Dtos/Prontuario/` (create/update/createVersao/read DTOs; retire legacy `DocAPI.Data.Dtos.ProntuarioDtos` namespace in touched code), `DocAPI/Application/Mappings/Profiles/ProntuarioProfile.cs`
  - **Depends on:** `TASK-002`
  - **Tests or checks:** Solution compiles; `UpdateProntuarioDto` has **no** evolution-only properties; `Tipo` is **int** on write/read DTOs
  - **Done when:** `IProntuarioRepository` uses `Guid` throughout; legacy string IDs, `UpdateAsync` generic update, and `CreateFromPdfAsync` **removed or replaced** with explicit operations; interface includes read/lookup methods per `design.md`: `GetByIdAsync`, `GetAllAsync`, `GetByPacienteIdAsync`, `GetLatestByPacienteIdAsync`, `GetLatestByAtendimentoIdAsync`, `GetMaxVersaoForPacienteAsync`; write surface supports create v1, correction, evolution insert, and soft delete (method names are Execute choice); DTOs match `design.md` API Contract (`CreateProntuarioDto`, `UpdateProntuarioDto` correction-safe only, `CreateVersaoProntuarioDto`, `ReadProntuarioDto`); domain entity types removed from public DTOs; create/update/evolution AutoMapper maps **removed**; read projection strategy chosen (explicit mapper/helper — not blind AutoMapper create/update)

### Repository

- [ ] `TASK-004` — Implement `ProntuarioRepository` reads and version lookup
  - **Requirements:** `REQ-003`, `REQ-004`, `REQ-005`, `REQ-016`, `REQ-007` (lookup half of D-02)
  - **Files:** `DocAPI/Infrastructure/Repositories/ProntuarioRepository.cs`
  - **Depends on:** `TASK-003`, Execution Prerequisites
  - **Tests or checks:** Manual or unit verification before write tasks
  - **Done when:** `GetByIdAsync`, `GetAllAsync`, `GetByPacienteIdAsync` (Versao DESC, CriadoEm DESC), `GetLatestByPacienteIdAsync`, `GetLatestByAtendimentoIdAsync`, `GetMaxVersaoForPacienteAsync` implemented; EF includes load nested graph for workflow-readable reads; soft-deleted rows excluded; **repository returns max Versao fact only — no business policy**

- [ ] `TASK-005` — Implement `ProntuarioRepository` writes (create, correction, evolution, soft delete)
  - **Requirements:** `REQ-001`, `REQ-006`, `REQ-007`, `REQ-008`, `REQ-009`
  - **Files:** `DocAPI/Infrastructure/Repositories/ProntuarioRepository.cs`
  - **Depends on:** `TASK-004`
  - **Tests or checks:** Unit tests in `TASK-007`; integration in `TASK-008`
  - **Done when:** **Checkpoints (complete in order):** (1) create v1 persists full graph in single transaction; (2) correction updates same row — **no child collection mutation** per design; (3) evolution inserts **complete snapshot** new row (**D-05** — no merge from predecessor); (4) soft delete single version only; (5) unique constraint violation on concurrent evolution surfaces as catchable conflict for **409** mapping (**D-06**); **no** Atendimento FK or CID validation inside repository

### Controller / Application

- [ ] `TASK-006` — `ProntuarioController` orchestration, FK/CID validation, routes, PHI, status codes
  - **Requirements:** `REQ-001`, `REQ-002`, `REQ-010`, `REQ-012`, `REQ-015`, `REQ-017`, `REQ-018`
  - **Files:** `DocAPI/API/Controllers/ProntuarioController.cs`
  - **Depends on:** `TASK-005`
  - **Tests or checks:** Code review; negative tests invalid or soft-deleted `AtendimentoId` → **404**; compile-time check `UpdateProntuarioDto` shape; apply `security-phi-review.md` sensor on touched controller paths when task completes
  - **Done when:** Guid routes; POST create **201** — loads `Paciente` via `IPacienteRepository` and passes identity snapshot into v1 factory (**REQ-001**); PUT correction **204**; POST `/versoes` **201**; DELETE **204**; `/atual` routes wired; evolution flow: `GetMaxVersaoForPacienteAsync` → `nextVersao` → `CriarNovaVersao` → persist (**D-02**); Atendimento FK validated via `IAtendimentoRepository` before create (exists, not soft-deleted, `PacienteId` match) → **404** on failure (**REQ-002**); when Internacao section present, CID existence validated before persist — unknown CID → **400** or **404**, not unhandled **500** (**REQ-010**); PHI logging removed; `POST /from-pdf` returns **501** immediately — **no** repository call, file processing, or PDF side effects (retire active stub path); **409** on version conflict; PUT with non-contract JSON members may return **400** if binding rejects unknown properties; **no** workflow services introduced

### Tests

- [ ] `TASK-007` — Repository and domain unit tests (InMemory)
  - **Requirements:** `REQ-001`, `REQ-003`, `REQ-004`, `REQ-005`, `REQ-006`, `REQ-007`, `REQ-008`, `REQ-009`, `REQ-018`
  - **Files:** `DocAPI.Tests/` (e.g. `ProntuarioRepositoryTests.cs`, domain tests as appropriate)
  - **Depends on:** `TASK-005`
  - **Tests or checks:** `dotnet test --filter Prontuario`
  - **Done when:** Tests cover v1 create, correction (same Id/Versao), evolution (new Id, prior unchanged, patient-wide Versao increment), list ordering, soft-delete exclusion, evolution snapshot without merge (**D-05** example scenario); recommended: `ProntuarioMappingTests` or equivalent for read projection

- [ ] `TASK-008` — SQL integration tests (**required**)
  - **Requirements:** `REQ-002`, `REQ-010`, `REQ-013`, `REQ-016`
  - **Files:** `DocAPI.Tests/Integration/ProntuarioSqlIntegrationTests.cs` (+ CID seed helper)
  - **Depends on:** `TASK-007`
  - **Approach:** **Repository-direct round-trip** against Docker SQL (same pattern as `AtendimentoSqlIntegrationTests`) — use verified `PacienteRepository` + `AtendimentoRepository` fixtures, then `ProntuarioRepository`; HTTP/Swagger E2E is **TASK-009**, not required here
  - **Tests or checks:** `dotnet test --filter ProntuarioSql`
  - **Done when:** Full fixture chain passes: Paciente → Atendimento → Prontuario v1 → PUT correction → POST `/versoes` v2 → read by `AtendimentoId` with nested graph (**REQ-016**); **mandatory D-05 scenario:** V1 has Exames `[E1, E2]`; evolution payload supplies `[E3]` only; V2 must **not** contain E1/E2; V1 unchanged; Internacao with synthetic CID (positive) and unknown CID (negative → **400/404**) covered (**REQ-010**); **recommended:** two-atendimento patient-wide Versao scenario (**D-03**); **optional:** concurrent evolution **409**

### Runtime validation and gate

- [ ] `TASK-009` — Runtime validation intent (Swagger smoke)
  - **Requirements:** `REQ-015` and API-facing REQs
  - **Files:** Session notes only (**not** `verification.md`)
  - **Depends on:** `TASK-006`, `TASK-008`
  - **Tests or checks:** Manual checklist per `specify.md` Runtime Validation Environment
  - **Done when:** Scenarios attempted or explicitly deferred with reason: create, read, list, `/atual` (patient + atendimento), PUT correction, POST `/versoes`, delete, invalid AtendimentoId **404**, paginated list, PUT with non-contract JSON members → **400** (if binding configured); **Verify** owns durable evidence table

- [ ] `TASK-010` — Build and automated test gate
  - **Requirements:** All (`REQ-001` through `REQ-018`)
  - **Files:** Solution-wide
  - **Depends on:** `TASK-007`, `TASK-008`, `TASK-009`
  - **Tests or checks:** `dotnet build`, `dotnet test`
  - **Done when:** Build succeeds; all non-skipped tests pass; test count increase from **27** baseline recorded (expect materially higher count — domain + repository unit tests + SQL integration; record exact before/after in session notes)

## Verify Preparation (VP)

Workflow preparation for Verify — **not** Execute implementation.

- [ ] `VP-001` — Verification handoff preparation
  - **Purpose:** Structure and inputs for `verification.md`
  - **Depends on:** `TASK-010`, `TASK-009`
  - **Done when:** Verify phase can populate `Documentation/SDD/prontuario-sql-stabilization/verification.md` with gates, evidence paths, REQ traceability, skipped gates, residual risks, Runtime Validation § (TASK-009 session notes as input)

## Runtime Validation (TASK-009 / Verify ownership)

| Role | Responsibility |
|------|----------------|
| **Execute (`TASK-009`)** | Confirm environment; run Swagger checklist; record intent in session notes |
| **Verify** | Owns durable HTTP/runtime evidence in `verification.md` § Runtime Validation |

## Documentation Follow-Up Preparation (DF)

Workflow preparation for Documentation Follow-Up — **not** Execute implementation.

- [ ] `DF-001` — Documentation follow-up routing
  - **Purpose:** Evaluate post-Verify updates per Documentation Integration Model
  - **Depends on:** `VP-001`
  - **Done when:** Candidates evaluated for State, PM, `migration-sql.md`, `runbook.md`, ADR (DQ-009), `erd.dbml`, WS07 drift note, pilot report v0.2; Documentation Update skill routes ownership

## Dependency Map

| Task | Depends on | Can run in parallel with | Notes |
|------|------------|--------------------------|-------|
| `TASK-001` | None | Prerequisites confirmation | Docs only |
| `TASK-002` | `TASK-001` | — | Domain implementation (not review-only) |
| `TASK-003` | `TASK-002` | — | Contract alignment |
| `TASK-004` | `TASK-003`, Prerequisites | — | Reads before writes |
| `TASK-005` | `TASK-004` | — | Critical path — writes |
| `TASK-006` | `TASK-005` | `TASK-007` (after TASK-005) | Controller after repo |
| `TASK-007` | `TASK-005` | `TASK-006` (partial overlap after repo stable) | Unit tests |
| `TASK-008` | `TASK-007` | — | SQL integration required; repository-direct (not HTTP) |
| `TASK-009` | `TASK-006`, `TASK-008` | — | Swagger intent |
| `TASK-010` | `TASK-007`–`TASK-009` | — | Final gate |
| `VP-001` | `TASK-010` | — | Verify phase |
| `DF-001` | `VP-001` | — | Post-Verify |

## Requirement Traceability

| Requirement | Tasks | Tests or checks | Verification evidence |
|-------------|-------|-----------------|-----------------------|
| `REQ-001` | `TASK-002`, `TASK-005`, `TASK-006`, `TASK-007`, `TASK-008` | Unit + integration create v1; Paciente identity snapshot | Test output |
| `REQ-002` | `TASK-006`, `TASK-008`, `TASK-009` | Negative / soft-deleted AtendimentoId → 404 | Test output + Swagger |
| `REQ-003` | `TASK-004`, `TASK-006`, `TASK-007` | GetById with metadata | Test output |
| `REQ-004` | `TASK-004`, `TASK-007` | List Versao DESC | Test output |
| `REQ-005` | `TASK-004`, `TASK-007`, `TASK-008` | `/atual` patient + atendimento | Test output |
| `REQ-006` | `TASK-002`, `TASK-003`, `TASK-005`, `TASK-006`, `TASK-007` | PUT same Id/Versao; structural DTO | Test output |
| `REQ-007` | `TASK-002`, `TASK-004`, `TASK-005`, `TASK-006`, `TASK-007`, `TASK-008` | Evolution chain; optional 409 | Test output |
| `REQ-008` | `TASK-002`, `TASK-005`, `TASK-007` | Soft delete single version | Test output |
| `REQ-009` | `TASK-005`, `TASK-007`, `TASK-008` | Nested graph; D-05 no-merge | Integration output |
| `REQ-010` | `TASK-006`, `TASK-008` | Controller CID validation; integration negative + synthetic positive | Test output + code review |
| `REQ-011` | `TASK-003`, `TASK-006` | Guid; no entity in public DTOs | Code review + Swagger |
| `REQ-012` | `TASK-006` | security-phi-review sensor at TASK-006 completion | Code review |
| `REQ-013` | `TASK-008` | **Passing** SQL integration required | Test output |
| `REQ-014` | `TASK-001` | Legacy + classification review | specify.md + design.md |
| `REQ-015` | `TASK-006`, `TASK-009` | Swagger status codes | Session notes |
| `REQ-016` | `TASK-004`, `TASK-008` | Read by AtendimentoId + includes | Integration test |
| `REQ-017` | `TASK-006` | Scope boundary review | Pre-Execution Review + code review |
| `REQ-018` | `TASK-003`, `TASK-007` | `Tipo` int contract | Unit/contract test |

## SQL Integration Testing

SQL integration tests are **required** for this feature (**REQ-013**). A passing run against Docker SQL is the expected success outcome. Skip is permitted only for documented operational or environment blockers and is **not** expected success behavior.

### Test approach

Follow **`AtendimentoSqlIntegrationTests`**: repository-direct round-trip against Docker SQL using `SqlIntegrationTestGate` / `SqlConnectionResolver`. Upstream fixtures use verified `PacienteRepository` and `AtendimentoRepository` contracts — not HTTP. Swagger/HTTP validation is **TASK-009** scope.

### Mandatory verification expectations

When Docker SQL is reachable (Execution Prerequisites met):

| Expectation | Detail |
|-------------|--------|
| Test class exists | `ProntuarioSqlIntegrationTests` — **required** |
| Fixture chain | Paciente → Atendimento → Prontuario v1 → correction → POST `/versoes` → v2 (repository operations) |
| D-05 no-merge | **Mandatory** — V2 collections must not inherit predecessor items omitted from evolution payload |
| Synthetic CID | Insert test CID row when Internacao scenarios run; unknown CID negative test |
| Connection source | `DOCORGANO_TEST_CONNECTION` if set; otherwise Docker default + Credential Probe |
| Synthetic data only | No real patient names, CPF, or clinical data |
| Success evidence | **Passing** `dotnet test` output recorded |
| Workflow read check | Load by `AtendimentoId` with nested shape (**REQ-016**) |

### Acceptable skip conditions

| Condition | Required action |
|-----------|-------------------|
| Docker not running | Skip with reason; Verify records residual risk |
| Credential probe failure | Skip with reason — distinguish login vs unreachable |
| Database not migrated | Skip with reason |
| Prerequisites were confirmed at Execute start but env failed mid-session | Document in session notes |

Skip must use explicit test framework skip — not silent pass.

### Completion criteria for REQ-013

| Outcome | REQ-013 status |
|---------|----------------|
| Integration tests pass against Docker SQL | **Met** |
| Skipped with documented operational reason | **Met with residual risk** |
| No integration test class | **Not met** |
| Tests fail due to code defects | **Not met** |

## Verification Expectations

The Verifier selects final gates. This section lists expected evidence from the implementation plan.

| Gate category | Expected / Not expected | Evidence or rationale |
|---------------|-------------------------|-----------------------|
| Build | Expected | `dotnet build` |
| Automated tests | Expected | `dotnet test` unit + integration |
| SQL / Persistence | Expected | **Passing** `ProntuarioSqlIntegrationTests`; skip = residual risk only |
| API | Expected | Swagger smoke checklist (TASK-009 session notes) |
| UI | **Not expected** | WS07 deferred |
| Security / PHI | Expected | security-phi-review sensor |
| Domain review | Expected | Versioning vs legacy; correction vs evolution; D-01–D-06 |
| Documentation review | Expected | check-docs on SDD sync if implementation diverges |
| Test strategy review | Expected | test-strategy sensor |
| ADR evaluation | **Expected** | ADR-006 accepted; Verify confirms implementation alignment |
| Legacy characterization | Expected | REQ-014 table + design legacy section |

## Review Sensors

| Sensor | When to apply | Owner if deferred |
|--------|---------------|-------------------|
| `security-phi-review.md` | **Execute** — when `TASK-006` completes (touched controller paths) | Verify |
| `domain-review.md` | **Verify** — versioning, D-01–D-06, legacy table | — |
| `test-strategy.md` | **Verify** — after `TASK-010` test output available | — |
| `check-docs.md` | **Verify** — if implementation diverges from specify/design | — |

Execute checklist (minimum):

- [ ] `security-phi-review.md` applied or explicitly deferred to Verify with reason

## Known Risks And Skipped Checks

| Risk or skipped check | Reason | Owner / follow-up |
|-----------------------|--------|-------------------|
| SQL integration skipped | Environment/credential blocker — not success | Verify records residual risk |
| DQ-009 / ADR deferred | — | **Closed** — ADR-006 accepted 2026-06-19 |
| Controller orchestration complexity | Evolution + CID + Paciente snapshot + 409 in one class | Follow Atendimento precedent; extract Application service only if controller exceeds maintainability |
| CID catalog absent in production | Accepted debt | Documentation Follow-Up / future feature |
| WS07 Guid + dual verb drift | Backend Stabilization | WS07 PM item |
| Workflow deferred | REQ-017 | `atendimento-workflow-stabilization` |
| Patient-wide Versao confusion | DQ-010 documented | **Recommended** integration two-atendimento test (TASK-008) |
| Concurrent evolution 409 | Expected (D-06) | Client retry guidance in API docs later |
| `AtualizadoPor` unset | Auth out of scope | Accepted residual risk |
| AutoMapper reintroduced on write | Critical data loss | Code review gate TASK-003 |
| Evolution merge-from-predecessor | Wrong clinical data | TASK-005/008 assert D-05 |

## Documentation Follow-up Candidates

- **State:** Prontuario verified status; test count; next Agendamento
- **ADR:** ADR-006 accepted — optional Domain Overview note in Documentation Follow-Up
- **Architecture:** Optional Domain Overview note on correction vs evolution
- **Technical:** `migration-sql.md` Prontuario checklist; synthetic CID test note
- **Rules:** security-phi if new patterns emerge
- **Skills:** `sql-migration-workflow` — third vertical confirmation
- **Review prompts:** None unless Verify finds gaps
- **Templates:** Pilot report **v0.2** after Pre-Execution Review
- **Active SDD:** Sync if implementation diverges from specify/design

## Commit Guidance

- Keep commits atomic and focused per task slice
- Do not commit secrets, `.env` files, credentials, real patient data, or clinical data
- Commit message should reflect change type and scope (e.g., `feat(api): implement Prontuario SQL repository with versioning`)

## Completion Handoff

Before marking Execute complete, provide material for Verify (session notes; formal artifact is `VP-001` → `verification.md`):

- Implemented task list (`TASK-001` through `TASK-010`)
- Requirement traceability for `REQ-001` through `REQ-018`
- Tests run: `dotnet build`, `dotnet test`, Swagger checklist (TASK-009)
- Tests skipped with reasons per SQL Integration Testing policy
- Review sensors: `security-phi-review` applied at TASK-006 or deferred to Verify with reason; other sensors per Review Sensors table
- Residual risks (SQL skip, CID catalog, WS07, `AtualizadoPor`, workflow out of scope)
- Documentation follow-up candidates for `DF-001`
- Scope boundary confirmation: no workflow evaluators, no Atendimento re-implementation, no frontend changes

### Task Checkbox Ownership

| Artifact | Execute | Verify | Documentation Follow-Up |
|----------|---------|--------|-------------------------|
| `TASK-*` | Marks complete when implementation + embedded tests done | Confirms REQ acceptance | Does **not** substitute for Verify |
| `VP-*` | Prepares inputs | Populates `verification.md` | — |
| `DF-*` | Identifies candidates | Identifies mandatory targets | Executes doc sync |
| `TASK-009` runtime intent | Execute records session notes | Verify owns durable Runtime Validation evidence | — |

Do not treat Documentation Follow-Up as owner for marking `TASK-*` complete unless Verify confirms acceptance.

## Pre-Execution Review Handoff (Next Phase)

**Status: Complete (2026-06-19)** — [reports/pre-execution-review-2026-06-19.md](reports/pre-execution-review-2026-06-19.md), [sdd-pilot-report-v0.2.md](reports/sdd-pilot-report-v0.2.md)

Before Execute, SDD Pre-Execution Review confirmed:

- [x] Execution Prerequisites + Credential Probe validated
- [x] Baseline **27/27 tests** captured
- [x] API contract table in `design.md` accepted (PUT correction DTO, POST `/versoes`, `/atual`, **409**)
- [x] Backend Stabilization scope — no WS07, no workflow, no Atendimento re-implementation
- [x] **DQ-009 / ADR** — **[ADR-006](../../Architecture/ADR/ADR-006-prontuario-dual-mode-versioning-api.md)** accepted
- [x] Field classification (D-01) reflected in planned DTO shapes
- [x] **D-02 reconciled** with Specify and ADR-006
- [x] SQL integration approach accepted: repository-direct (Atendimento pattern); HTTP smoke in TASK-009 only
- [x] Execute Boundary accepted — no hidden Verify/reporting/State work in TASK list

**Next:** Proceed to **Execute — TASK-001**.
