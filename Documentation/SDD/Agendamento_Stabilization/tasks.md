# Tasks — Agendamento SQL Migration & Stabilization

> Feature SDD: `Documentation/SDD/Agendamento_Stabilization/`
> Inputs: `specify.md` (2026-07-17), `design.md` (2026-07-17)
> Generated SDD artifacts are written in English.
> **Template source:** `Documentation/AI-Harness/template/sdd/tasks.md`

## Execution Strategy

**Implementation approach:** Capability-first vertical decomposition. Each execution wave delivers a complete architectural capability — domain invariants, persistence contract, API surface, and validation — before moving to the next. Within each wave, tasks are ordered by dependency (entity before interface, config before repository, DTOs before controller, unit tests before integration tests).

**Execution sequence:** Domain → Persistence → Application/API → Testing → Runtime Validation → Verification & Documentation Preparation.

**Critical path:** Entity (TASK-001) → Interface (TASK-002) → Repository (TASK-004) → Controller (TASK-007) → Controller unit tests (TASK-010) → SQL integration tests (TASK-011). Total critical path: 6 tasks (50% of TASKs).

**Parallel opportunities:** TASK-003 (EF config) and TASK-005 (DTOs) can execute in parallel with TASK-002 within Wave 1–2. TASK-006 (AutoMapper) can execute in parallel with TASK-007 (controller). TASK-008 and TASK-009 can execute in parallel once their dependencies are complete.

## Execution Boundary

- **Included:** Entity model alignment (PacienteId, domain methods), interface Guid migration, EF configuration update, repository SQL implementation, DTO redesign, AutoMapper profile update, controller hardening (Guid routes, PHI remediation, FK validation, status codes), unit tests (entity, repository, controller), SQL integration tests, runtime validation intent.
- **Excluded:** Verification artifacts (`verification.md`), reporting, project documentation updates (State, PM, migration-sql, runbook), Teacher Guide generation, frontend alignment (WS07), Atendimento Workflow validations, new EF migrations, Internacao repository creation, Procedimento field restoration, `GET /Agendamento/by-filter` activation, Google Sheets re-enablement, auth/RBAC, financial features.
- Current execution does not begin until SDD Pre-Execution Review exit criteria are satisfied.

## Execution Waves

### Wave 1 — Domain Foundation

Establish aggregate invariants, identity contract, and domain operations. No persistence or API layer changes.

**Architectural Capability delivered:** Business Invariants — aggregate construction, SoftDelete, Update immutability, owned-value mutation.

### Wave 2 — Persistence Layer

Wire entity-to-schema mapping and implement SQL CRUD against the existing `InitialCreate` schema. No API surface changes.

**Architectural Capability delivered:** Persistence Intent — repository behavior, query semantics, soft-delete exclusion; Physical Persistence — entity-to-schema mapping alignment, FK relationships, zero migration delta.

### Wave 3 — Application/API Layer

Redesign DTO contracts, update object mapping, and harden the controller. Persistence is already functional from Wave 2.

**Architectural Capability delivered:** API Contract — Guid routing, HTTP status codes, FK error response format, DTO contract alignment.

### Wave 4 — Testing

Verify entity invariants, repository behavior, controller logic, and full CRUD round-trip. All implementation from Waves 1–3 is complete.

**Architectural Capability exercised:** Business Invariants + Persistence Intent + Physical Persistence + API Contract — all four capabilities tested across unit and integration suites.

### Wave 5 — Runtime Validation & Verification Preparation

Confirm environment readiness and prepare verification evidence. No implementation changes.

**Architectural Capability delivered:** Runtime Validation — manual smoke observations recorded for Verify.

---

## Operational Execution Plan

This section is an operational guide for Execute. It complements — does not replace — the detailed TASK descriptions below.

### Recommended Execution Order

1. **Batch 1.1** — Entity + Interface (TASK-001, TASK-002)
2. **Batch 2.1** — EF Config + Repository (TASK-003, TASK-004)
3. **Batch 3.1** — DTOs + AutoMapper (TASK-005, TASK-006)
4. **Batch 3.2** — Controller (TASK-007)
5. **Batch 4.1** — Entity + Repository + Controller Unit Tests (TASK-008, TASK-009, TASK-010)
6. **Batch 4.2** — SQL Integration Tests (TASK-011)
7. **Batch 5.1** — Runtime Validation + Verification Prep + Doc Candidates (TASK-012, VP-001, DF-001)

### Context Loading Between Batches

Acquire context per-batch, not per-feature. See `sdd-operational.md` § Context Acquisition Governance.

| Starting batch | Load before executing |
|----------------|----------------------|
| Batch 1.1 | Entity (`Agendamento.cs`), ADR-001, `specify.md` § Domain Language, `design.md` § Requirement Mapping (REQ-001, REQ-011) |
| Batch 2.1 | EF config (`AgendamentoConfig.cs`), `DbContext.cs` (query filter), `InitialCreate` migration, interface file, `design.md` § Data And Persistence |
| Batch 3.1 | Current DTOs, `AgendamentoProfile.cs`, `design.md` § DTO contracts, entity from Wave 1 |
| Batch 3.2 | Controller (`AgendamentoController.cs`), repository from Wave 2, DTOs from Batch 3.1, `design.md` § API Contract + FK error format, `ProntuarioControllerTests.cs` (FK pattern reference) |
| Batch 4.1 | Entity from Wave 1, repository from Wave 2, controller from Wave 3, existing test files for pattern reference (`ProntuarioRepositoryTests.cs`, `AtendimentoControllerTests.cs`) |
| Batch 4.2 | `SqlIntegrationTestGate.cs`, existing SQL integration tests (`ProntuarioSqlIntegrationTests.cs`), `design.md` § Fixture Chain, `.env` / `load-env.ps1` |
| Batch 5.1 | `design.md` § Runtime Validation Environment + § Verification Handoff Notes + § Documentation Follow-up Candidates |

Do not preload context for future batches. After each batch completes, release loaded context and acquire only what the next batch requires.

### Validation Cadence

| After batch | Validate |
|-------------|----------|
| Batch 1.1 | `dotnet build DocAPI` |
| Batch 2.1 | `dotnet build DocAPI` + schema diff (zero delta) |
| Batch 3.1 | `dotnet build DocAPI` |
| Batch 3.2 | `dotnet build DocAPI` |
| Batch 4.1 | `dotnet test --filter "FullyQualifiedName~Agendamento"` (unit only — Entity + Repository + Controller test classes) |
| Batch 4.2 | `dotnet test --filter "FullyQualifiedName~Agendamento"` (all tests including SQL integration) |
| Batch 5.1 | Swagger smoke checklist (manual); session notes captured |

Each batch must pass its validation before the next batch begins. If a batch validation fails, stop and fix within the current batch — do not proceed with failing validation.

### Commit Cadence

| Commit | Batch | Contents |
|--------|-------|----------|
| 1 | 1.1 | Entity + Interface (`Agendamento.cs`, `IAgendamentoRepository.cs`) |
| 2 | 2.1 | EF Config + Repository (`AgendamentoConfig.cs`, `AgendamentoRepository.cs`) |
| 3 | 3.1 | DTOs + AutoMapper (`CreateAgendamentoDto.cs`, `ReadAgendamentoDtos.cs`, `UpdateAgendamento.cs`, `AgendamentoProfile.cs`) |
| 4 | 3.2 | Controller (`AgendamentoController.cs`) |
| 5 | 4.1 | Unit tests (`AgendamentoEntityTests.cs`, `AgendamentoRepositoryTests.cs`, `AgendamentoControllerTests.cs`) |
| 6 | 4.2 | SQL integration tests (`AgendamentoSqlIntegrationTests.cs`) |
| 7 | 5.1 | Runtime session notes |

Commit message format: `feat(agendamento): <description>` with task reference. Do not commit secrets, `.env` files, credentials, real patient data, or clinical data.

### Rollback Boundary

The rollback boundary is a single batch. If a batch cannot be completed or its validation fails irrecoverably:
- Revert the batch's commit.
- Stop and request a new planning cycle (return to Design or SDD Pre-Execution Review).
- Do not attempt to work around failures by expanding scope or modifying architecture.

### When to Stop and Request a New Planning Cycle

Stop Execute and escalate to Design or SDD Pre-Execution Review if:
- Schema diff produces unexpected migration delta in Batch 2.1 (TASK-003).
- `InternacaoId` NOT NULL constraint becomes a real blocking requirement (outpatient scheduling without Internacao).
- Upstream aggregate contracts (`IAtendimentoRepository`, `IPacienteRepository`) are incompatible with FK validation needs in TASK-007.
- Entity redesign in TASK-001 reveals hidden architectural decisions not covered by `design.md`.
- New requirements emerge during implementation that fall outside the Execution Boundary.
- A batch validation fails and the fix would cross batch boundaries.

---

## Implementation Tasks (TASK)

### Wave 1 — Domain Foundation

#### Batch 1.1: Entity + Interface

**Objective:** Establish the aggregate model with correct schema alignment, domain operations, and Guid-based persistence contract.

**Architectural Capability:** Business Invariants.

**Expected validation:** `dotnet build DocAPI` clean.

**Expected commit point:** After both TASK-001 and TASK-002 are complete and build passes.

**Context boundary for next batch:** Entity types, interface signatures, and enum definitions are stable — EF configuration and DTOs can consume them.

- [x] `TASK-001` — Entity model alignment: add PacienteId, Paciente navigation, and domain operations
  - **Requirements:** `REQ-001`, `REQ-011`
  - **Files:** `DocAPI/Core/Entities/Agendamento.cs`
  - **Depends on:** None
  - **Expected test suite(s):** Entity Tests
  - **Risk:** Medium | **Estimated Context:** Medium
  - **Tests or checks:**
    - Code review: entity has `PacienteId` (Guid?), `Paciente` navigation property, no `Procedimento` string property
    - Code review: `Status`, `InstrucaoStatus`, `AtestadoStatus` remain enums — no changes to enum definitions or `HasConversion<string>()` usage
    - Code review: `SenhaAgendamento` owned value object preserved; properties accessible for mutation via aggregate method
  - **Done when:**
    - Entity has `PacienteId` property (`Guid?`, nullable) and `Paciente` navigation property (`Paciente?`)
    - No `Procedimento` string property exists on the entity
    - Constructor/factory accepts `PacienteId` as optional parameter (`Guid?`, default null); accepts `Nome`, `Aviso`, `Local`, `Sala`, `DataConsulta`, `InstrucaoStatus`, `AtestadoStatus`, `SenhaAgendamento` as optional scalar parameters with reasonable defaults
    - Factory sets `Status = StatusAgendamento.SemSenha` and `CriadoEm = DateTime.UtcNow`
    - `SoftDelete()` method sets `Deletado = true`, `DeletadoEm = DateTime.UtcNow`
    - `Update(Nome, Aviso, Data, Horario, Local, Sala, Status, InstrucaoStatus, AtestadoStatus, DataConsulta)` method sets mutable scalar fields and `AtualizadoEm = DateTime.UtcNow`; FK targets and `ID` are not writable through this method
    - `SetSenha(SenhaAgendamento?)` method mutates the owned `SenhaAgendamento` value — null argument clears it, non-null replaces; sets `AtualizadoEm`
    - All new properties and methods have `private set` for properties, `public` for domain methods
    - Project compiles (`dotnet build DocAPI`)

- [x] `TASK-002` — Interface Guid migration: convert all ID signatures to Guid
  - **Requirements:** `REQ-002`
  - **Files:** `DocAPI/Core/Interfaces/Repositories/IAgendamentoRepository.cs`
  - **Depends on:** `TASK-001`
  - **Expected test suite(s):** None (signature change — validated by compilation and downstream tests)
  - **Risk:** Low | **Estimated Context:** Small
  - **Tests or checks:**
    - Code review: `GetByIdAsync(Guid id)`, `GetByPacienteIdAsync(Guid pacienteId)`, `UpdateAsync(Agendamento agendamento, Guid id)`, `DeleteAsync(Guid id)` — all ID parameters are `Guid`
    - Code review: `GetAllAsync`, `GetByNameAsync`, `CreateAsync` signatures unchanged (no ID parameters to migrate)
  - **Done when:**
    - All method signatures use `Guid` for ID parameters
    - Interface compiles with `TASK-001` entity changes
    - No `string` ID parameters remain on any method

### Wave 2 — Persistence Layer

#### Batch 2.1: Configuration + Repository

**Objective:** Wire entity-to-schema mapping and implement full SQL CRUD against the existing `InitialCreate` schema with zero migration delta.

**Architectural Capability:** Persistence Intent + Physical Persistence.

**Expected validation:** `dotnet build DocAPI` clean + schema diff confirms zero pending changes.

**Expected commit point:** After both TASK-003 and TASK-004 are complete, build passes, and schema diff is verified.

**Context boundary for next batch:** Repository is fully functional — controller and DTOs can consume it.

- [x] `TASK-003` — EF configuration update: add Paciente relationship mapping
  - **Requirements:** `REQ-004`
  - **Files:** `DocAPI/Infrastructure/SqlDb/Configurations/AgendamentoConfig.cs`
  - **Depends on:** `TASK-001`
  - **Expected test suite(s):** None (mapping-only — validated by SQL integration tests + schema diff)
  - **Risk:** Medium | **Estimated Context:** Small
  - **Tests or checks:**
    - Code review: `Paciente` relationship mapped with `HasOne(x => x.Paciente).WithMany().HasForeignKey(x => x.PacienteId)` referencing existing `PacienteID` column
    - Code review: existing mappings preserved — `Internacao` FK, `Atendimento` FK, owned-type `SenhaAgendamento` (4 columns), enum-to-string conversions, 3 indexes (composite `Data`+`Horario`, `Status`, `InternacaoId`)
    - Code review: no new indexes introduced
    - Schema diff verification: `dotnet ef migrations add` produces zero pending changes (empty `Up`/`Down`) — any delta indicates mapping mismatch requiring resolution, not a new migration
    - `DbSet<Agendamento>` already registered in `DocDbContext` — no change needed
    - Global query filter already active in `DocDbContext.ApplySoftDeleteQueryFilter` — no change needed
  - **Done when:**
    - `AgendamentoConfiguration` includes `Paciente` relationship referencing `PacienteID` column
    - All existing configuration is preserved without modification
    - Schema diff confirms zero migration delta

- [x] `TASK-004` — Repository SQL implementation: full CRUD against DocDbContext
  - **Requirements:** `REQ-003`
  - **Files:** `DocAPI/Infrastructure/Repositories/AgendamentoRepository.cs`
  - **Depends on:** `TASK-002`, `TASK-003`
  - **Expected test suite(s):** Repository Tests, SQL Integration Tests
  - **Risk:** Medium | **Estimated Context:** Medium
  - **Tests or checks:**
    - Code review: repository constructor accepts `DocDbContext` (injected via DI)
    - Code review: `CreateAsync` calls `_context.Agendamentos.AddAsync(agendamento)` then `_context.SaveChangesAsync()`
    - Code review: `GetByIdAsync` uses `_context.Agendamentos.FirstOrDefaultAsync(a => a.ID == id)` — soft-deleted rows excluded by global query filter; no `.IgnoreQueryFilters()` unless explicitly documented for audit
    - Code review: `GetAllAsync` uses `_context.Agendamentos.OrderByDescending(a => a.Data).ThenBy(a => a.Horario).Skip(skip).Take(take).ToListAsync()`; `take` capped at 100
    - Code review: `GetByNameAsync` uses `_context.Agendamentos.Where(a => a.Nome.Contains(name)).ToListAsync()` — case-insensitive substring search; EF `Contains` translates to SQL `LIKE`
    - Code review: `GetByPacienteIdAsync` uses `_context.Agendamentos.Where(a => a.PacienteId == pacienteId).ToListAsync()`
    - Code review: `UpdateAsync` calls `_context.Agendamentos.Update(agendamento)` then `_context.SaveChangesAsync()`
    - Code review: `DeleteAsync` loads entity, calls `agendamento.SoftDelete()`, then `_context.SaveChangesAsync()`
    - Repository performs **no** FK validation, business rule enforcement, or PHI handling
    - All `NotImplementedException` stubs replaced
  - **Done when:**
    - All 7 interface methods implemented against `DocDbContext`
    - Repository compiles against `TASK-002` interface
    - No `NotImplementedException` remains in the file
    - DI registration in `Program.cs` already maps `IAgendamentoRepository` → `AgendamentoRepository` — verify registration is still correct after constructor change

### Wave 3 — Application/API Layer

#### Batch 3.1: DTOs + Mapping

**Objective:** Redesign all three DTO contracts for Guid identity, enum type safety, and FK clarity. Remove write-direction AutoMapper mappings.

**Architectural Capability:** API Contract (DTO shapes, write-path enforcement).

**Expected validation:** `dotnet build DocAPI` clean.

**Expected commit point:** After both TASK-005 and TASK-006 are complete and build passes.

**Context boundary for next batch:** DTO contracts and mapping profile are stable — controller can consume them.

- [x] `TASK-005` — DTO redesign: align all three DTOs with entity contract
  - **Requirements:** `REQ-010`
  - **Files:** `DocAPI/Application/Data/Dtos/Agendamento/CreateAgendamentoDto.cs`, `DocAPI/Application/Data/Dtos/Agendamento/ReadAgendamentoDtos.cs`, `DocAPI/Application/Data/Dtos/Agendamento/UpdateAgendamento.cs`
  - **Depends on:** `TASK-001`, `TASK-002`
  - **Expected test suite(s):** None directly (data contracts — validated by compilation + controller tests)
  - **Risk:** Medium | **Estimated Context:** Medium
  - **Tests or checks:**
    - Code review: `CreateAgendamentoDto` — no `Procedimento` field, no `Status` field, `PacienteId` is `Guid?` (nullable, optional), `AtendimentoId` is `Guid` (required), `InternacaoId` is `Guid` (required), `StatusInstrucoes` and `StatusAtestado` are enum types, scalar fields for `Nome`/`Aviso`/`Data`/`Horario`/`Local`/`Sala`/`DataConsulta`/`SenhaAgendamento`
    - Code review: `ReadAgendamentoDto` — `ID` is `Guid`, `PacienteId` is `Guid?`, `AtendimentoId` is `Guid`, `InternacaoId` is `Guid`, no `Procedimento` field, all status fields use domain enum types, `CriadoEm` and `AtualizadoEm` are `DateTime`
    - Code review: `UpdateAgendamentoDto` — no `ID` field, no FK fields, no `Procedimento` field, mutable scalar fields only, enum-typed fields use domain enum types
    - Code review: all three DTOs reside in namespace `DocAPI.Data.Dtos.AgendamentoDtos`
  - **Done when:**
    - All three DTO files compile with correct types
    - `Procedimento` field absent from all three DTOs
    - `Status` field absent from `CreateAgendamentoDto`
    - `ID` and FK fields absent from `UpdateAgendamentoDto`
    - Enum-typed fields use domain enum types, not strings

- [x] `TASK-006` — AutoMapper profile update: read mappings only; write mappings removed
  - **Requirements:** `REQ-010` (write-path enforcement), `REQ-007` (read-path support)
  - **Files:** `DocAPI/Application/Mappings/Profiles/AgendamentoProfile.cs`
  - **Depends on:** `TASK-005`
  - **Expected test suite(s):** Object-mapping tests included in TASK-008 entity tests
  - **Risk:** Low | **Estimated Context:** Small
  - **Tests or checks:**
    - Code review: `CreateMap<CreateAgendamentoDto, Agendamento>()` write-direction mapping **removed** — controller must use manual construction through aggregate factory
    - Code review: `CreateMap<UpdateAgendamentoDto, Agendamento>()` write-direction mapping **removed** — controller must use manual construction through aggregate `Update()` method
    - Code review: `CreateMap<Agendamento, ReadAgendamentoDto>()` read-direction mapping **preserved and updated** for new entity fields
  - **Done when:**
    - Write-direction mappings (DTO → Agendamento) are removed from the profile
    - Read-direction mapping (Agendamento → ReadAgendamentoDto) is updated and compiles
    - Project compiles against `TASK-005` DTOs and `TASK-001` entity

#### Batch 3.2: Controller

**Objective:** Harden all endpoints with Guid routing, FK validation, PHI remediation, and correct HTTP semantics. Controller depends on fully functional repository (Wave 2) and stable DTO contracts (Batch 3.1).

**Architectural Capability:** API Contract (routing, status codes, FK error format).

**Expected validation:** `dotnet build DocAPI` clean.

**Expected commit point:** After TASK-007 is complete and build passes.

**Context boundary for next batch:** All implementation is complete — tests can consume the full stack.

- [x] `TASK-007` — Controller hardening: Guid routes, PHI remediation, FK validation, status codes
  - **Requirements:** `REQ-005`, `REQ-006`, `REQ-007`, `REQ-008`, `REQ-009`
  - **Files:** `DocAPI/API/Controllers/AgendamentoController.cs`
  - **Depends on:** `TASK-004`, `TASK-005`, `TASK-006`
  - **Expected test suite(s):** Controller Tests
  - **Risk:** High | **Estimated Context:** Large
  - **Tests or checks:**
    - **PHI remediation (REQ-005):** zero `Console.WriteLine` calls in entire file; no structured logging added
    - **Guid routes (REQ-005):** `[HttpGet("{id:guid}")]`, `[HttpPut("{id:guid}")]`, `[HttpDelete("{id:guid}")]` — all route parameters use `Guid id`; `CreatedAtAction` uses `Guid`
    - **FK validation on create (REQ-006):** before aggregate construction, validate `AtendimentoId` via `IAtendimentoRepository.GetByIdAsync()` (must exist and not be soft-deleted), `InternacaoId` via `_context.Set<Internacao>().FirstOrDefaultAsync()` (must exist), `PacienteId` when non-null via `IPacienteRepository.GetByIdAsync()` (must exist and not be soft-deleted); any FK failure returns 404 with structured body per `design.md` API Contract (`{"title":"Foreign key reference not found","status":404,"field":"...","detail":"The referenced resource was not found or has been removed."}`); submitted ID value is never echoed; fail-fast on first invalid FK
    - **Read endpoints (REQ-007):** `GET /Agendamento?skip=&take=` returns 200 + `ReadAgendamentoDto[]` (empty array when no rows); `GET /Agendamento/{id:guid}` returns 200 + single DTO or 404; `GET /Agendamento/by-name?nome=` returns 200 + array (empty when no matches — never 404); `GET /Agendamento/by-pacientId?pacienteId={guid}` returns 200 + array; pagination cap at 100
    - **Update (REQ-008):** `PUT /Agendamento/{id:guid}` — load entity, call `agendamento.Update(...)` with DTO scalar fields, save via repository, return 204; 404 if not found or soft-deleted; FK targets immutable — DTO excludes them at contract level
    - **Soft delete (REQ-009):** `DELETE /Agendamento/{id:guid}` — load entity, call `agendamento.SoftDelete()`, save via repository, return 204; 404 if not found or already deleted
    - Controller constructor injects `IAgendamentoRepository`, `IMapper`, `IAtendimentoRepository`, `IPacienteRepository`, `DocDbContext` (for Internacao set access)
  - **Done when:**
    - Zero diagnostic-logging calls in controller
    - All route templates use `Guid` constraints
    - All 6 endpoints return correct status codes per `design.md` API Contract table
    - POST validates all 3 FK targets before aggregate construction; returns 404 with structured body on any FK failure
    - PUT updates mutable fields only; FK targets unchanged
    - DELETE performs soft delete; row excluded from subsequent reads
    - Controller compiles with updated dependencies
    - Error responses never echo submitted ID values

### Wave 4 — Testing

#### Batch 4.1: Unit Tests

**Objective:** Verify aggregate invariants, repository CRUD behavior in isolation, and controller logic with mocked dependencies.

**Architectural Capability exercised:** Business Invariants (entity tests), Persistence Intent (repository tests), API Contract (controller tests).

**Expected validation:** `dotnet test --filter "FullyQualifiedName~Agendamento"` (unit tests only — entity, repository, controller).

**Expected commit point:** After all three test classes pass.

**Context boundary for next batch:** Unit-level verification complete — SQL integration can test full stack.

- [x] `TASK-008` — Entity unit tests: aggregate invariants and domain operations
  - **Requirements:** `REQ-001`, `REQ-011`
  - **Files:** `DocAPI.Tests/Infrastructure/AgendamentoEntityTests.cs` (new file)
  - **Depends on:** `TASK-001`
  - **Expected test suite(s):** Entity Tests
  - **Risk:** Low | **Estimated Context:** Medium
  - **Tests or checks:**
    - Factory sets `Status = StatusAgendamento.SemSenha` and `CriadoEm ≈ DateTime.UtcNow` with non-empty `ID`
    - Factory accepts required FKs (`InternacaoId`, `AtendimentoId`, `Data`, `Horario`) and optional scalar fields + `PacienteId`
    - `SoftDelete()` sets `Deletado = true`, `DeletadoEm ≈ DateTime.UtcNow`
    - `Update()` sets mutable scalar fields and `AtualizadoEm`; FK targets and `ID` are unchanged after update
    - `SetSenha()` with non-null value replaces `SenhaAgendamento` and sets `AtualizadoEm`
    - `SetSenha()` with null value clears `SenhaAgendamento` and sets `AtualizadoEm`
    - Object-mapping test: `AutoMapper` maps `Agendamento` → `ReadAgendamentoDto` correctly for all fields
  - **Done when:** All entity tests pass with `dotnet test --filter "FullyQualifiedName~AgendamentoEntityTests"`

- [x] `TASK-009` — Repository unit tests: CRUD behavior in isolation
  - **Requirements:** `REQ-003`
  - **Files:** `DocAPI.Tests/Infrastructure/AgendamentoRepositoryTests.cs` (new file)
  - **Depends on:** `TASK-004`
  - **Expected test suite(s):** Repository Tests
  - **Risk:** Low | **Estimated Context:** Medium
  - **Tests or checks:**
    - Use `InMemoryDatabase` (pattern from `ProntuarioRepositoryTests`)
    - `CreateAsync` persists entity and `GetByIdAsync` retrieves it
    - `GetAllAsync` returns paginated results; ordering by `Data` descending, `Horario` ascending; `take` capped at 100
    - `GetByNameAsync` returns entities whose `Nome` contains the search string
    - `GetByPacienteIdAsync` returns entities matching the `PacienteId`
    - `UpdateAsync` persists changed fields
    - `DeleteAsync` invokes `SoftDelete()` and saves — row excluded from subsequent reads
    - Repository performs no FK validation
  - **Done when:** All repository unit tests pass with `dotnet test --filter "FullyQualifiedName~AgendamentoRepositoryTests"`

- [x] `TASK-010` — Controller unit tests: routing, FK validation, status codes
  - **Requirements:** `REQ-005`, `REQ-006`, `REQ-007`, `REQ-008`, `REQ-009`
  - **Files:** `DocAPI.Tests/Controllers/AgendamentoControllerTests.cs` (new file)
  - **Depends on:** `TASK-007`
  - **Expected test suite(s):** Controller Tests
  - **Risk:** Medium | **Estimated Context:** Medium
  - **Tests or checks:**
    - Mock `IAgendamentoRepository`, `IAtendimentoRepository`, `IPacienteRepository`, `IMapper`, `DocDbContext`
    - `GET /Agendamento` returns 200 + mapped DTO array; empty array when repository returns empty
    - `GET /Agendamento/{id}` returns 200 + DTO when found; 404 when null
    - `GET /Agendamento/by-name` returns 200 + array; empty array when no matches (not 404)
    - `GET /Agendamento/by-pacientId` returns 200 + array; empty array when no matches
    - `POST` returns 201 with `Location` header and DTO body when all FKs valid
    - `POST` returns 404 with structured body when `AtendimentoId` not found
    - `POST` returns 404 with structured body when `InternacaoId` not found
    - `POST` returns 404 with structured body when `PacienteId` (provided) not found
    - `POST` returns 404 with structured body when FK target is soft-deleted
    - FK error body includes `field` identifying the failing FK, never echoes submitted ID value
    - `PUT` returns 204 on success; 404 when aggregate not found; FK targets unchanged
    - `DELETE` returns 204 on success; 404 when not found or already deleted
  - **Done when:** All controller tests pass with `dotnet test --filter "FullyQualifiedName~AgendamentoControllerTests"`

#### Batch 4.2: Integration Tests

**Objective:** Exercise full CRUD round-trip against Docker SQL Server with FK validation and soft-delete exclusion.

**Architectural Capability exercised:** Physical Persistence + API Contract.

**Expected validation:** `dotnet test --filter "FullyQualifiedName~AgendamentoSqlIntegrationTests"` passes when Docker SQL is available.

**Expected commit point:** After integration tests pass. This is the final implementation commit — all code changes are complete.

**Context boundary for next batch:** Full stack verified — runtime validation and handoff preparation can proceed.

- [x] `TASK-011` — SQL integration tests: full CRUD round-trip against Docker SQL **(test source exists; execution skipped during Verify — environment unavailable; accepted residual risk per Environment-Dependent Evidence Policy)**
  - **Requirements:** `REQ-003`, `REQ-006`, `REQ-007`, `REQ-008`, `REQ-009`, `REQ-012`
  - **Files:** `DocAPI.Tests/Integration/AgendamentoSqlIntegrationTests.cs` (new file)
  - **Depends on:** `TASK-010`
  - **Expected test suite(s):** SQL Integration Tests
  - **Risk:** Medium | **Estimated Context:** Large
  - **Tests or checks:**
    - Follow fixture chain per `design.md` § Fixture Chain: Paciente → Atendimento → Prontuario (with Internacao) → Agendamento
    - All fixture data is synthetic (no real patient names, CPF, or clinical data)
    - Test uses `SqlIntegrationTestGate` pattern (existing in `DocAPI.Tests/Infrastructure/SqlIntegrationTestGate.cs`)
    - CRUD round-trip: create → read by ID → read by name → read by PacienteId → update → read updated → soft delete → verify exclusion from all reads
    - Negative FK tests: POST with invalid `AtendimentoId` returns 404; POST with invalid `InternacaoId` returns 404; POST with non-existent `PacienteId` returns 404; POST with soft-deleted FK target returns 404
    - Pagination: verify default skip/take, page-size cap at 100
    - Test both with and without `PacienteId` provided on create
    - `Internacao` is created as child of `Prontuario` — no standalone Internacao repository
  - **Done when:** SQL integration tests pass with `dotnet test --filter "FullyQualifiedName~AgendamentoSqlIntegrationTests"` when Docker SQL is available
  - **Note:** Tests must skip gracefully (not fail) when Docker SQL connection is unavailable, using the established `SqlIntegrationTestGate` pattern

### Wave 5 — Runtime Validation & Verification Preparation

#### Batch 5.1: Runtime + Verify Prep + Doc Candidates

**Objective:** Record runtime smoke observations and prepare verification evidence for handoff. No implementation changes.

**Architectural Capability exercised:** Runtime Validation.

**Expected validation:** Session notes captured; verification evidence compiled.

**Expected commit point:** After session notes are recorded. This is a documentation-only commit.

**Context boundary:** Final batch — handoff to Verify.

- [x] `TASK-012` — Runtime validation intent **(Execute session notes not available; Swagger/Runtime smoke skipped during Verify — environment unavailable; accepted residual risk)**
  - **Requirements:** `design.md` § Runtime Validation Environment
  - **Files:** Session notes (handoff to Verify)
  - **Depends on:** `TASK-011`
  - **Expected test suite(s):** None (manual smoke)
  - **Risk:** Low | **Estimated Context:** Small
  - **Execute owns:** Environment confirmed; smoke checklist attempted or explicitly deferred with reason in handoff notes. Record session notes for Verify handoff — do not rely on chat history.
  - **Verify owns:** Durable evidence table in `verification.md` § Runtime Validation (HTTP/Swagger scenarios, pass/fail). Verify checks that TASK-012 session notes were provided before populating the Runtime Validation table.
  - **Smoke checklist (Execute attempts; Verify owns durable evidence):**
    - Swagger UI accessible at `https://localhost:7004/swagger`
    - All 6 endpoints visible with Guid parameters and correct DTO shapes
    - GET `/Agendamento?skip=0&take=10` returns 200
    - GET `/Agendamento/{id}` returns 200/404 as expected
    - GET `/Agendamento/by-name?nome=test` returns 200 (empty or populated)
    - GET `/Agendamento/by-pacientId?pacienteId={guid}` returns 200
    - POST with valid FKs returns 201 with Location header
    - POST with invalid FK returns 404 with structured error body
    - PUT returns 204 on success
    - DELETE returns 204 on success
    - GET by-name with no matches returns 200 with `[]` (not 404)
  - **Done when (Execute):** Implementation complete, runtime intent recorded, session notes captured.

## Verify Preparation (VP)

Workflow preparation for Verify — not Execute implementation.

- [x] `VP-001` — Verification evidence preparation **(completed by Verify — see verification.md § Evidence Inventory)**
  - **Purpose:** Compile expected evidence inputs for Verify per `design.md` § Verification Handoff Notes. Prepare but do not execute verification.
  - **Depends on:** `TASK-011`, `TASK-012`
  - **Done when:**
    - `dotnet test` output captured showing all Agendamento test suites pass (entity, repository, controller, SQL integration)
    - `dotnet build` confirmed clean for `DocAPI` and `DocAPI.Tests` projects
    - Session notes from TASK-012 runtime validation captured for Verify handoff
    - File list of all changed/added files compiled for review sensor targeting
    - Known skipped checks documented: `AtualizadoPor` remains unset (accepted residual risk — no auth), controller HTTP integration tests optional per `design.md` § Optional Test Debt, Internacao FK validation note (Internacao has no soft-delete flag — verify in schema during review)
    - Review sensors identified: `security-phi-review.md` (diagnostic-logging removal), API contract review (Guid contract, status codes, FK error format), EF migration review (zero delta)

## Documentation Follow-Up Preparation (DF)

Workflow preparation for Documentation Follow-Up — not Execute implementation.

- [x] `DF-001` — Documentation follow-up candidate identification **(completed by Verify — see verification.md § Documentation Follow-Up Candidates + current Documentation Follow-Up execution)**
  - **Purpose:** Identify documentation candidates per `design.md` § Documentation Follow-up Candidates. Do not modify documentation.
  - **Depends on:** `TASK-011`, `VP-001`
  - **Done when:**
    - State candidates identified: update branch/status when Execute begins, update test counts after Execute (79 + new Agendamento tests), update SDD research status table
    - Technical docs candidates: `Documentation/Technical/migration-sql.md` — note Agendamento as fourth verified SQL vertical
    - PM candidates: `Documentation/Product/PM_DocOrgano.md` — update WS01 status after Verify
    - No ADR, architecture, rules, skills, review prompts, or template changes identified
    - Active SDD: `tasks.md` completion status to be updated by Execute; `specify.md` § Open Questions all resolved (DQ-001 through DQ-006)

## Dependency Map

| Task | Depends on | Can run in parallel with | Notes |
|------|------------|--------------------------|-------|
| `TASK-001` | None | — | Entity changes — no upstream dependencies |
| `TASK-002` | `TASK-001` | `TASK-003`, `TASK-005` | Interface depends on entity types |
| `TASK-003` | `TASK-001` | `TASK-002`, `TASK-005` | EF config depends on entity properties |
| `TASK-004` | `TASK-002`, `TASK-003` | — | Repository depends on interface + mapping |
| `TASK-005` | `TASK-001`, `TASK-002` | `TASK-003` | DTOs depend on entity enums + interface Guids |
| `TASK-006` | `TASK-005` | `TASK-007` | AutoMapper depends on DTO shapes |
| `TASK-007` | `TASK-004`, `TASK-005`, `TASK-006` | — | Controller depends on repository + DTOs + mapping |
| `TASK-008` | `TASK-001` | `TASK-009` | Entity tests depend on entity only |
| `TASK-009` | `TASK-004` | `TASK-008` | Repository tests depend on repository |
| `TASK-010` | `TASK-007` | — | Controller tests depend on controller |
| `TASK-011` | `TASK-010` | — | Integration tests depend on all unit tests passing |
| `TASK-012` | `TASK-011` | `VP-001`, `DF-001` | Runtime validation after tests pass |
| `VP-001` | `TASK-011`, `TASK-012` | `DF-001` | Verification prep after implementation complete |
| `DF-001` | `TASK-011`, `VP-001` | — | Doc follow-up prep after verification prep |

**Critical Path:** `TASK-001` → `TASK-002` → `TASK-004` → `TASK-007` → `TASK-010` → `TASK-011` (6 tasks, 50% of TASKs). Graph is acyclic.

## Requirement Traceability

| Requirement | Tasks | Tests or checks | Verification evidence |
|-------------|-------|-----------------|-----------------------|
| `REQ-001` — Entity aligned with migration schema | `TASK-001`, `TASK-008` | Code review + entity unit tests passing | Code review + `dotnet test` (entity tests) |
| `REQ-002` — Interface uses Guid IDs | `TASK-002` | Code review: all ID parameters are `Guid`; interface compiles | Code review + compilation |
| `REQ-003` — Repository SQL implementation | `TASK-004`, `TASK-009`, `TASK-011` | Unit tests (isolation) + SQL integration tests (round-trip) | `dotnet test` (repository + integration) |
| `REQ-004` — EF configuration updated | `TASK-003` | Code review + schema diff: zero migration delta | Code review + schema diff |
| `REQ-005` — Controller PHI-safe, Guid routes | `TASK-007`, `TASK-010` | Code review + security-PHI sensor: zero `Console.WriteLine`, `{id:guid}` constraints | Code review + controller unit tests |
| `REQ-006` — Create with FK validation | `TASK-007`, `TASK-010`, `TASK-011` | Unit tests (negative FK) + SQL integration (FK constraint behavior) | `dotnet test` (controller + integration) |
| `REQ-007` — Read endpoints with correct status codes | `TASK-007`, `TASK-010`, `TASK-011` | Unit tests (status codes) + runtime smoke | `dotnet test` (controller + integration) + runtime notes |
| `REQ-008` — Update immutable FKs, mutable scalars | `TASK-007`, `TASK-010`, `TASK-011` | Unit tests (FK unchanged) + SQL integration (round-trip update) | `dotnet test` (controller + integration) |
| `REQ-009` — Soft delete per ADR-001 | `TASK-007`, `TASK-010`, `TASK-011` | Unit tests (exclusion from queries) + SQL integration (deleted row not returned) | `dotnet test` (controller + integration) |
| `REQ-010` — DTOs aligned | `TASK-005`, `TASK-006` | Code review: correct types, no `Procedimento`, no `Status` on Create, enums on Read, no FK on Update | Code review + compilation |
| `REQ-011` — Domain methods | `TASK-001`, `TASK-008` | Unit tests: factory, SoftDelete, Update, SetSenha | `dotnet test` (entity tests) |
| `REQ-012` — Tests passing | `TASK-008`, `TASK-009`, `TASK-010`, `TASK-011` | `dotnet test` run for all Agendamento test suites | Full `dotnet test` output |
| `REQ-013` — Legacy behavior characterized | None (specify acceptance) | Legacy behavior table in specify.md reviewed and accepted | specify.md § Legacy Behavior accepted in Design |

## Verification Expectations

The Verifier selects final gates. This section lists expected evidence from the implementation plan.

| Gate category | Expected / Not expected | Evidence or rationale |
|---------------|-------------------------|-----------------------|
| Build | Expected | `dotnet build` for `DocAPI` and `DocAPI.Tests` — clean |
| Automated tests | Expected | `dotnet test` for all Agendamento test suites — entity, repository, controller, SQL integration |
| SQL / Persistence | Expected | SQL integration tests pass against Docker SQL; schema diff confirms zero migration delta |
| API | Expected | Swagger smoke checklist (TASK-012 session notes) — all 6 endpoints with Guid params, correct DTO shapes, correct status codes |
| UI | Not expected | Backend stabilization — frontend excluded (WS07) |
| Security / PHI | Expected | `security-phi-review.md` review sensor — zero diagnostic-logging calls in controller; no PHI in test fixtures |
| Domain review | Expected | Code review — aggregate invariants, domain method behavior, FK immutability |
| Documentation review | Expected | `check-docs.md` review sensor — affected components documented, scope boundary respected |
| Test strategy review | Expected | `test-strategy.md` review sensor — test coverage across Architectural Capabilities; Optional Test Debt decisions documented |
| Architectural completeness | Expected | All four Architectural Capabilities (Business Invariants, Persistence Intent, Physical Persistence, API Contract) have test suites — no gaps |
| ADR evaluation | Expected | ADR-001 honored (soft delete); no new ADR candidates — documented in `design.md` § ADR Evaluation |
| Legacy characterization | Expected | specify.md § Legacy Behavior table accepted — 17 sheet columns accounted for |

## Review Sensors

Review sensors expected to apply during Verify. Execute does not apply review sensors — Verify owns their execution.

- [x] `Documentation/AI-Harness/review-prompts/security-phi-review.md` — PHI-bearing logging removal verification (**applied during Verify — OK, zero findings**)
- [x] `Documentation/AI-Harness/review-prompts/domain-review.md` — aggregate invariants, domain method behavior (**applied during Verify — 2 findings (F-01, F-02), 1 observation (O-001)**)
- [x] `Documentation/AI-Harness/review-prompts/test-strategy.md` — test coverage across Architectural Capabilities (**applied during Verify — OK, all four ACs covered**)
- [x] EF migration review (manual schema diff) — zero migration delta verification (**applied during Verify — unable to fully verify (SQL unavailable); low risk — all entity properties map to existing InitialCreate columns**)

## Known Risks And Skipped Checks

| Risk or skipped check | Reason | Owner / follow-up |
|-----------------------|--------|-------------------|
| `AtualizadoPor` remains unset on all writes | No auth/RBAC implemented — accepted residual risk across all SQL verticals | Accepted residual risk — re-evaluate when auth is implemented |
| Controller HTTP integration tests (through pipeline) | Optional per `design.md` § Optional Test Debt — covered by controller unit tests + SQL integration tests + runtime smoke | Accepted — Verify HTTP smoke covers routes |
| Object-mapping tests (DTO ↔ entity) | Recommended — included in TASK-008 entity tests | Mitigated — entity tests include AutoMapper read-direction mapping assertion |
| Soft-deleted parent FK negative test | Recommended — included in TASK-010 (mocked) and TASK-011 (integration) | Mitigated — both unit and integration coverage |
| Internacao has no soft-delete flag | FK validation must verify Internacao existence — if Internacao gains soft-delete in future, FK validation must exclude soft-deleted Internacao records | Documented risk — verify Internacao schema during Execute |
| `InternacaoId` NOT NULL blocks outpatient scheduling | Research DC-3 residual risk (Medium confidence). If required, evaluate ADR before relaxing constraint | Accepted residual risk — no current business requirement |
| Name search unindexed | Substring `LIKE` on `Nome` scans full table on large datasets | Accepted for MVP — `Nome` is varchar(20); future SDD may add index |
| Frontend breaks on Guid identity | WS07 deferred — known drift documented in `design.md` § Frontend Impact | WS07 follow-up — backend tested independently |
| Schema diff may reveal unexpected delta | Entity-to-schema mapping mismatch after Paciente relationship addition | Mitigated — TASK-003 requires explicit schema diff verification before completion |

## Documentation Follow-up Candidates

Documentation Update decides final routing. Execute identifies candidates only — does not modify documentation.

- **State:** Update branch/status when Execute begins; update test count after Execute (79 baseline + new Agendamento tests); update SDD research status table row for Agendamento_Stabilization.
- **ADR:** None.
- **Architecture docs:** No changes — aggregate boundaries, domain rules, and migration sequencing unchanged.
- **Technical docs:** `Documentation/Technical/migration-sql.md` — note Agendamento as fourth verified SQL vertical.
- **Rules:** No changes identified.
- **Skills:** No changes identified.
- **Review prompts:** No new prompts needed.
- **Templates:** No changes identified.
- **Active SDD:** `tasks.md` checkbox completion status; `specify.md` § Open Questions — all DQ-001 through DQ-006 resolved.
- **PM:** `Documentation/Product/PM_DocOrgano.md` — update WS01 status after Verify.

## Completion Handoff

Before marking tasks complete, provide Verify with:

- Implemented task list with checkbox status.
- Requirement traceability status — all 13 REQs mapped to completed TASKs.
- Tests/checks run — `dotnet test` output for all Agendamento test suites.
- Tests/checks skipped with reasons — `AtualizadoPor` unset, HTTP integration tests optional.
- Review sensors identified for Verify application — Execute does not apply review sensors.
- Residual risks — Internacao soft-delete ambiguity, outpatient scheduling constraint, name search index, frontend Guid drift.
- Documentation follow-up candidates — State, migration-sql.md, PM_DocOrgano.md.

### Task Checkbox Ownership

| Artifact | Execute | Verify | Documentation Follow-Up |
|----------|---------|--------|-------------------------|
| `TASK-*` implementation | Marks work done in handoff; may check boxes when implementation complete | Validates acceptance against requirements | Does **not** own implementation status |
| `VP-*` | Prepares evidence inputs | Uses for gate selection | — |
| `DF-*` | Identifies candidates only | Identifies mandatory targets | Executes doc sync; may mark `DF-*` complete |
| `tasks.md` checkboxes | Execute may mark `TASK-*` complete when done | Verify confirms acceptance | May mark `VP-*` / `DF-*` when workflow phases complete |