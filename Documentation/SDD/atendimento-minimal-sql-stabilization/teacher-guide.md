# Teacher Guide — Atendimento Minimal SQL Stabilization

> **Audience:** Junior developers, developers new to Doc Organo, future maintainers, and the project owner learning the architecture.
>
> **Prerequisite artifacts:** This guide teaches *how and why* the feature works. For scope, verification, and operational status, see the linked artifacts at the end — do not expect this document to repeat them.

---

## How This Guide Differs From Other Artifacts

| Artifact | What it answers | This guide does not repeat it |
|----------|-----------------|-------------------------------|
| `specify.md` / `design.md` | What to build and how to design it | Requirement tables, full API contract tables, Legacy preserve/adapt/abandon matrix |
| `verification.md` | Whether it passed and what evidence exists | Gate results, completion decisions, HTTP smoke tables |
| `feature-report.md` | What shipped and workflow lessons | Delivery summary, PM references |
| `session-handoff.md` | What to do in the next session | Next steps, branch status |
| `State.md` / PM | Current operational truth | Runtime or backlog status |
| ADR-001 | Durable soft-delete decision | Full ADR text |
| `migration-sql.md` | Migration checklist and endpoint reference | Full route/status tables |
| [Paciente teacher guide](../paciente-sql-stabilization/teacher-guide.md) | First aggregate vertical patterns | Paciente-specific CRUD and search semantics |

**This guide owns:** concepts, reasoning, architecture flow, code connections, and a study path so you can understand and safely extend the Atendimento **Minimal** backend — the persistence prerequisite for Prontuario and Agendamento.

---

## 1. Feature Overview

### Business purpose

**Atendimento** is the clinical care **journey container** — the episode that links a patient to stages of care (consultation, pre-procedure, procedure, post-procedure, finalized), medical messages, pendências, and clinical events. Prontuario and Agendamento both require a valid `AtendimentoId` foreign key before they can persist in SQL.

This feature stabilizes only the **minimal** slice: a real `AtendimentoId` row exists in SQL so downstream aggregates can reference it. Journey orchestration (`ValidacaoEtapa*`, pendências, stage advancement) belongs to a separate SDD and is intentionally **not** implemented here.

### Technical objective

`AtendimentoRepository` was fully stubbed (`NotImplementedException` on every method). The controller used legacy string IDs, bloated DTOs, risky AutoMapper create paths, and `Console.WriteLine` patterns. The minimal stabilization replaces stubs with SQL CRUD while **preserving ownership boundaries** approved in the SDD: factory-owned create, workflow-owned stage, controller-owned Paciente FK validation, persistence-only repository.

### Stabilization goals (what “done” meant for learning purposes)

1. **Valid journey container** — Create via aggregate factory; initial `EtapaAtual` is always `Consulta`.
2. **FK prerequisite for downstream** — Prontuario/Agendamento can reference `AtendimentoId` after Verify.
3. **Multiple concurrent journeys** — Same patient may have more than one active Atendimento (list-by-paciente).
4. **Safe, limited mutation** — Update changes `MensagemParaMedico` only; no client-driven stage changes.
5. **Soft delete per ADR-001** — Deleted rows excluded from default queries.
6. **Cross-aggregate validation at API layer** — Paciente must exist (and be non-deleted via query filter) before create.
7. **Test confidence** — Repository unit tests, controller FK tests, SQL integration round-trip against Docker SQL Server.

Frontend (WS07), workflow rules, report PDF implementation, and auth/`AtualizadoPor` were intentionally deferred.

---

## 2. What Changed

Think of the stabilization as layer-specific responsibilities, not a greenfield aggregate.

### Domain layer

- **Public constructor factory** — `Atendimento(Guid pacienteId, string? mensagemParaMedico)` assigns `Id`, sets `EtapaAtual = Consulta`, `CriadoEm`, optional message.
- **`AtualizarMensagemParaMedico`** — Only safe root-field update in the minimal slice; sets `AtualizadoEm`.
- **`MarcarComoExcluido`** — Soft delete per ADR-001.
- **Existing workflow methods unchanged but not exposed** — `AvancarEtapa()`, `RegistrarEvento()`, `AdicionarPendencia()` remain on the entity for future workflow SDD; no API or repository calls them in this slice.

### Application layer (mapping)

- **Slim DTOs** — Create has `PacienteId` + optional `MensagemParaMedico`; update has message only; read exposes `EtapaAtual` read-only.
- **`AtendimentoProfile`** — **Read map only** (`Atendimento` → `ReadAtendimentoDto`). Create does **not** use AutoMapper — the controller calls the factory directly. This prevents PacienteId or stage loss on create (a known Legacy risk).
- **`global::` enum reference** — `ReadAtendimentoDto.EtapaAtual` uses `global::DocAPI.Core.Entities.Atendimento.EtapaAtendimento` to avoid namespace collision with the DTO namespace `DocAPI.Data.Dtos.Atendimento`.

### Infrastructure layer (repository + EF)

- **`AtendimentoRepository`** — Full SQL CRUD against `DocDbContext`; `GetByPacienteIdAsync` for multi-journey listing; **no** Paciente FK checks, **no** workflow logic.
- **Update path** — Loads tracked entity, calls `AtualizarMensagemParaMedico` from a carrier entity, saves.
- **Soft delete** — `DeleteAsync` calls `MarcarComoExcluido()`; global query filter hides deleted rows.
- **`AtendimentoConfiguration`** — FK to Paciente with `Restrict` delete; `EtapaAtual` stored as string; index on `PacienteId`; child relationships configured but not written in this slice.

### API layer (controller)

- **Guid contract** — All route parameters and DTO ids use `Guid`.
- **Paciente FK validation** — `IPacienteRepository.GetByIdAsync` before factory + create; missing Paciente → 404.
- **List-by-paciente route** — `GET /Atendimento/paciente/{pacienteId}` returns all non-deleted journeys for a patient.
- **501 report routes preserved** — `report-id` and `followUp-id` return 501 without touching repository or Legacy PDF paths.
- **PHI remediation** — No `Console.WriteLine`; generic error messages only.

### Harness (operational, not feature scope)

- **`SqlConnectionResolver`** — Single credential source for integration tests (`DOCORGANO_TEST_CONNECTION` → `SA_PASSWORD` from env or repo-root `.env`).
- **Scripts** — `scripts/load-env.ps1`, `scripts/sql-integration-test.ps1` align Docker, EF, API, and tests.

### Tests

- **8 repository unit tests** — InMemory EF: create with Consulta, read, list-by-paciente (including two per patient), update message only, soft-delete exclusion, pagination.
- **3 controller unit tests** — Moq: invalid PacienteId → 404 (create never called); valid create → 201 + Consulta; report routes → 501.
- **1 SQL integration test** — Paciente → Atendimento round-trip against Docker SQL when credentials available.

---

## 3. Architecture Walkthrough

Doc Organo uses a **light Clean Architecture** split. For Atendimento Minimal, a typical **create** request flows like this:

```text
HTTP POST /Atendimento (JSON: CreateAtendimentoDto)
        │
        ▼
AtendimentoController.Post
        │  null body → 400
        │  IPacienteRepository.GetByIdAsync(dto.PacienteId)
        │      └── null → 404 (Paciente missing or soft-deleted via query filter)
        ▼
new Atendimento(dto.PacienteId, dto.MensagemParaMedico)   ← factory, NOT AutoMapper
        │  Id = Guid.NewGuid()
        │  EtapaAtual = Consulta
        │  CriadoEm = UtcNow
        ▼
AtendimentoRepository.CreateAsync
        │  _context.Atendimentos.Add(atendimento)
        │  SaveChangesAsync()
        ▼
EF Core + AtendimentoConfiguration
        │  FK PacienteId → Paciente (Restrict)
        │  EtapaAtual stored as string
        │  global query filter excludes Deletado = true
        ▼
SQL Server (Docker docorgano-sql)
        │
        ▼
IMapper.Map<ReadAtendimentoDto>(atendimento)
        │
        ▼
HTTP 201 Created + ReadAtendimentoDto (etapaAtual = Consulta)
```

### List by Paciente (multi-journey)

```text
GET /Atendimento/paciente/{pacienteId}
  → AtendimentoRepository.GetByPacienteIdAsync
  → WHERE PacienteId = id ORDER BY CriadoEm DESC
  → global filter excludes soft-deleted rows
  → map collection to ReadAtendimentoDto[]
  → 200 (empty array if none)
```

A patient may have **two or more** active rows — Prontuario/Agendamento UIs will eventually need an explicit journey selector (WS07, after Workflow SDD).

### Update (message only)

```text
PUT /Atendimento/{id} (UpdateAtendimentoDto — message only)
  → controller builds carrier: new Atendimento(Guid.Empty, dto.MensagemParaMedico)
  → AtendimentoRepository.UpdateAsync(carrier, id)
  → load existing by id (or KeyNotFoundException → 404)
  → existing.AtualizarMensagemParaMedico(carrier.MensagemParaMedico)
  → EtapaAtual unchanged
  → 204 No Content
```

The carrier entity with `Guid.Empty` is a pragmatic pattern: the repository only reads `MensagemParaMedico` from the passed entity. Do not treat it as a second persisted aggregate.

### Soft delete

```text
DELETE /Atendimento/{id}
  → AtendimentoRepository.DeleteAsync
  → load entity (or KeyNotFoundException → 404)
  → MarcarComoExcluido()
  → SaveChangesAsync
  → subsequent GetByIdAsync / GetByPacienteIdAsync exclude row (query filter)
```

### Layer responsibilities (Atendimento Minimal slice)

| Layer | Location | Responsibility |
|-------|----------|----------------|
| API | `DocAPI/API/Controllers/AtendimentoController.cs` | HTTP mapping, Paciente FK validation, factory on create, 501 report stubs |
| Application | `DocAPI/Application/Mappings/Profiles/AtendimentoProfile.cs` | Entity → read DTO only |
| Domain | `DocAPI/Core/Entities/Atendimento.cs` | Factory, limited update, soft delete; workflow methods present but unused in API |
| Contract | `DocAPI/Core/Interfaces/Repositories/IAtendimentoRepository.cs` | Persistence operations |
| Infrastructure | `DocAPI/Infrastructure/Repositories/AtendimentoRepository.cs` | EF queries and persistence — **no cross-aggregate validation** |
| EF config | `AtendimentoConfiguration.cs`, `DocDbContext.cs` | Table mapping, FK, indexes, soft-delete filter |
| Cross-aggregate | `IPacienteRepository` (injected in controller) | Existence check before Atendimento create |

**Compared to Paciente:** Atendimento adds **controller-layer FK validation** and **factory-only create**. The repository never reads Paciente — that boundary prevents hidden coupling Prontuario must also respect when validating `AtendimentoId`.

---

## 4. Concept Inventory

Each concept below was **demonstrably used** in this feature. Study priority: **High** = required before modifying code; **Medium** = deepens understanding; **Low** = useful for extension or hardening.

### Domain concepts

| Concept | Appears in | Why it exists | How it works | Related concepts | Study priority |
|---------|------------|---------------|--------------|------------------|----------------|
| Aggregate factory (create) | `Atendimento` constructor | Client must not own Id, stage, or audit fields | Public ctor sets `Id`, `PacienteId`, `Consulta`, `CriadoEm` | Slim create DTO, controller Post | High |
| Workflow-owned `EtapaAtual` | `Atendimento` ctor; not in update DTO | Stage transitions belong to workflow SDD, not CRUD API | Factory sets `Consulta`; update does not change stage; `AvancarEtapa()` exists but is not exposed | Factory, workflow SDD | High |
| Limited domain update | `AtualizarMensagemParaMedico` | Minimal slice allows only safe root mutation | Sets message + `AtualizadoEm`; repository delegates to this method | Update DTO, repository UpdateAsync | High |
| Soft delete | `MarcarComoExcluido`, ADR-001 | Preserve clinical history | Sets `Deletado` / `DeletadoEm`; filter hides from queries | Global query filter | High |
| Multiple journeys per patient | Entity model + `GetByPacienteIdAsync` | Real clinic may run parallel care episodes | No uniqueness constraint on PacienteId; list returns all non-deleted | List-by-paciente route | High |

### Application concepts

| Concept | Appears in | Why it exists | How it works | Related concepts | Study priority |
|---------|------------|---------------|--------------|------------------|----------------|
| Slim DTO contract | `CreateAtendimentoDto`, `UpdateAtendimentoDto`, `ReadAtendimentoDto` | Separate create/update/read shapes prevent workflow field leakage | Create: PacienteId + message; Update: message only; Read: includes EtapaAtual | Factory, AutoMapper read map | High |
| Read-only AutoMapper | `AtendimentoProfile` | Avoid AutoMapper dropping PacienteId or setting wrong stage on create | Single map: entity → `ReadAtendimentoDto` | Factory on create | High |
| DTO namespace collision | `ReadAtendimentoDto.EtapaAtual` | DTO namespace `...Dtos.Atendimento` shadows entity type name | `global::DocAPI.Core.Entities.Atendimento.EtapaAtendimento` | Enum in entity | Low |

### Infrastructure concepts

| Concept | Appears in | Why it exists | How it works | Related concepts | Study priority |
|---------|------------|---------------|--------------|------------------|----------------|
| Persistence-only repository | `AtendimentoRepository` | Keep repositories free of business rules and cross-aggregate reads | CRUD + list-by-paciente only; no Paciente lookup | Controller FK validation | High |
| Global query filter | `DocDbContext` — `HasQueryFilter(a => !a.Deletado)` | Default queries exclude soft-deleted Atendimentos | Applies to GetById, GetAll, GetByPacienteId | ADR-001, `IgnoreQueryFilters()` in tests | High |
| FK with Restrict delete | `AtendimentoConfiguration` | Prevent orphan Atendimentos and accidental cascade deletes | `PacienteId` required; `OnDelete(DeleteBehavior.Restrict)` | Paciente aggregate | Medium |
| Enum string storage | `AtendimentoConfiguration` — `HasConversion<string>()` | Readable SQL values for stage column | `Consulta`, `PreProcedimento`, etc. stored as varchar | EtapaAtual | Low |
| Child navigation (schema only) | `Eventos`, `Pendencias`, `Checklists` on entity | EF models future workflow persistence | Configured in Fluent API; **no writes** in minimal slice | Workflow SDD | Medium |

### API concepts

| Concept | Appears in | Why it exists | How it works | Related concepts | Study priority |
|---------|------------|---------------|--------------|------------------|----------------|
| Controller-layer FK validation | `AtendimentoController.Post` | Repository must not read other aggregates | `IPacienteRepository.GetByIdAsync` before factory; 404 if null | Persistence-only repository | High |
| 501 route preservation | `GetPatientReportPdf`, `GetPatientFollowUp` | Keep Swagger contract for future report feature without partial implementation | Immediate `StatusCode(501)`; no repository call | Workflow/report SDD | Medium |
| Guid authoritative contract | Routes, DTOs, `IAtendimentoRepository` | Abandon Legacy string IDs | All ids are `Guid` end-to-end | REQ-007 | High |
| Exception → HTTP translation | Update/Delete actions | Map missing entity to 404 | `KeyNotFoundException` from repository → `NotFound()` | Repository throws | Medium |

### Testing concepts

| Concept | Appears in | Why it exists | How it works | Related concepts | Study priority |
|---------|------------|---------------|--------------|------------------|----------------|
| InMemory repository tests | `AtendimentoRepositoryTests` | Fast feedback without Docker | Seed Paciente in same InMemory context; test Atendimento repo | xUnit, EF InMemory | High |
| Moq controller tests | `AtendimentoControllerTests` | Prove FK validation without HTTP pipeline | Mock `IPacienteRepository` returning null → 404; verify CreateAsync never called | DI, Moq | Medium |
| SQL integration round-trip | `AtendimentoSqlIntegrationTests` | Prove SQL provider + FK + filters | Paciente create → Atendimento CRUD → soft delete exclusion | Docker, SqlConnectionResolver | High |
| SqlConnectionResolver | `SqlConnectionResolver.cs` | Single local credential source | `DOCORGANO_TEST_CONNECTION` or `SA_PASSWORD` from env/`.env` | runbook, scripts | Medium |
| SkippableFact skip policy | `[SkippableFact]` + `Skip.If` | Environments without Docker must not fail build | Skip with setup hint when connection unavailable | Paciente integration pattern | Medium |
| Synthetic test data | All test classes | PHI must not appear in tests | Fake names, generated CPF, example.com emails | security-phi rule | High |

### Security concepts

| Concept | Appears in | Why it exists | How it works | Related concepts | Study priority |
|---------|------------|---------------|--------------|------------------|----------------|
| PHI-safe controller | `AtendimentoController` | Journey data links to patients | No console logging; generic bad-request messages | Paciente pattern | High |

### Concepts not used in this feature (do not expect them here)

Application Use Cases / stage evaluators (`ValidacaoEtapa*`), AutoMapper create maps, repository Paciente FK validation, workflow DTOs, `AvancarEtapa` API, pendência/clinical event persistence, `AtendimentoMappingTests`, full controller HTTP integration tests, auth/`AtualizadoPor`, new EF migrations, MediatR/CQRS, report PDF implementation.

---

## 5. Deep Dive By Component

### Atendimento (`DocAPI/Core/Entities/Atendimento.cs`)

**Responsibility:** Represent the care journey aggregate root.

**Construction:** Public constructor `(Guid pacienteId, string? mensagemParaMedico = null)` is the **only** approved create path for the minimal API. It assigns a new `Guid` id, sets `EtapaAtual = Consulta`, and stamps `CriadoEm`. Protected parameterless constructor exists for EF.

**Methods used in minimal slice:**

- `AtualizarMensagemParaMedico(string?)` — Updates message and `AtualizadoEm`.
- `MarcarComoExcluido()` — Soft delete.

**Methods present but not called from minimal API/repository:**

- `AvancarEtapa()` — Increments stage enum; throws if already `Finalizado`.
- `RegistrarEvento(...)`, `AdicionarPendencia(...)` — Add child entities in memory; persistence deferred to workflow SDD.

**Navigation collections:** `Eventos`, `Pendencias`, `Checklists` — EF relationships exist; minimal repository never loads or writes them.

**Enum `EtapaAtendimento`:** `Consulta → PreProcedimento → Procedimento → PosProcedimento → Finalizado`. Only `Consulta` is set automatically in this slice.

---

### IAtendimentoRepository / AtendimentoRepository

**Interface:** `DocAPI/Core/Interfaces/Repositories/IAtendimentoRepository.cs` — Guid-based CRUD + `GetByPacienteIdAsync`. Report methods were removed from the interface (501 handled in controller only).

**Implementation:** `DocAPI/Infrastructure/Repositories/AtendimentoRepository.cs`

| Method | Behavior |
|--------|----------|
| `GetAllAsync(skip, take)` | Non-deleted rows, newest first, paginated |
| `GetByIdAsync(id)` | Single row or null (respects soft-delete filter) |
| `GetByPacienteIdAsync(pacienteId)` | All non-deleted journeys for patient, newest first |
| `CreateAsync` | Add + SaveChanges — assumes valid entity from factory |
| `UpdateAsync(carrier, id)` | Load by id, `AtualizarMensagemParaMedico` from carrier; throws if missing |
| `DeleteAsync` | Load by id, `MarcarComoExcluido`, SaveChanges; throws if missing |

**Design constraint:** The repository **never** calls `IPacienteRepository`. If you need to validate Paciente or Atendimento existence for another aggregate, do it in the controller or a future application use case — not here.

---

### AtendimentoController (`DocAPI/API/Controllers/AtendimentoController.cs`)

**Responsibility:** HTTP translation, Paciente FK gate on create, aggregate factory invocation, read mapping, 501 report stubs.

**Dependencies:** `IAtendimentoRepository`, `IPacienteRepository`, `IMapper`.

**Create path (critical):**

```csharp
var paciente = await _pacienteRepository.GetByIdAsync(dto.PacienteId);
if (paciente == null) return NotFound();
var atendimento = new Atendimento(dto.PacienteId, dto.MensagemParaMedico);
await _repository.CreateAsync(atendimento);
```

**Why Paciente lookup returns 404 for soft-deleted patients:** `PacienteRepository.GetByIdAsync` uses the filtered `DbSet` — soft-deleted Paciente rows appear as null. This behavior is **implicit** (not covered by a dedicated automated test).

**Report routes:** Return 501 immediately — do not add repository or Legacy PDF calls in the minimal slice.

---

### AtendimentoProfile (`DocAPI/Application/Mappings/Profiles/AtendimentoProfile.cs`)

**Responsibility:** Map persisted entity to API read model only.

```csharp
CreateMap<Atendimento, ReadAtendimentoDto>();
```

**Why no create map:** The SDD explicitly rejected blind AutoMapper create after Legacy showed PacienteId could be dropped. If you add fields to `ReadAtendimentoDto`, update the profile and add mapping tests (currently deferred — see Testing Walkthrough).

---

### AtendimentoConfiguration / DocDbContext

**AtendimentoConfiguration** (`DocAPI/Infrastructure/SqlDb/Configurations/AtendimentoConfig.cs`):

- Table `Atendimento`, key `Id`
- FK `PacienteId` → `Paciente` with `Restrict`
- `EtapaAtual` → string column (max 50)
- Index on `PacienteId` (list-by-paciente performance)
- Child relationships configured for future workflow

**DocDbContext** applies `HasQueryFilter(a => !a.Deletado)` on `Atendimento` alongside Paciente, Prontuario, Agendamento.

---

### SqlConnectionResolver (`DocAPI/Infrastructure/SqlDb/SqlConnectionResolver.cs`)

**Responsibility:** Resolve SQL connection for integration tests and tooling consistently.

**Priority:** `DOCORGANO_TEST_CONNECTION` → `SA_PASSWORD` (environment or repo-root `.env`) → null with setup hint.

**Learning note:** Misaligned credentials previously caused integration tests to skip with misleading “Docker unreachable” messages. Always use `.env` + `scripts/load-env.ps1` per runbook before blaming network issues.

---

### Integration tests (`DocAPI.Tests/Integration/AtendimentoSqlIntegrationTests.cs`)

**Flow:** Build synthetic Paciente → create via `PacienteRepository` → Atendimento CRUD via `AtendimentoRepository` → assert Consulta stage → update message → soft delete → assert exclusion from get and list-by-paciente.

**Dependency:** Requires upstream Paciente SQL slice — the test proves the **Paciente → Atendimento** FK chain on real SQL Server.

---

## 6. Testing Walkthrough

### Repository unit tests (`AtendimentoRepositoryTests`)

**Provider:** EF Core InMemory  
**Setup pattern:** Each test creates its own InMemory database; seeds a `Paciente` in the same context (Atendimento FK requires parent row even in InMemory).

**Representative tests to read in order:**

1. `CreateAsync_PersistsAtendimentoWithConsultaStage` — factory defaults
2. `GetByPacienteIdAsync_ReturnsAllForPatient` — two journeys same patient + isolation from other patient
3. `UpdateAsync_ChangesMessageOnly` — `EtapaAtual` stays `Consulta`
4. `DeleteAsync_SoftDeletesAtendimento` — `IgnoreQueryFilters()` proves flag set; default queries empty

**Why InMemory:** Same rationale as Paciente — fast repository feedback. SQL semantics (string enum conversion, Restrict FK) validated in integration test.

### Controller unit tests (`AtendimentoControllerTests`)

**Tooling:** Moq for repositories, real AutoMapper profile.

**What they prove:**

- Invalid PacienteId → 404 and **CreateAsync never invoked** (FK gate works)
- Valid create → 201 payload with `Consulta` and message
- Report routes → 501

**Gap:** GET/PUT/DELETE HTTP status codes were validated via Verify HTTP smoke, not fully automated in unit tests.

### SQL integration tests (`AtendimentoSqlIntegrationTests`)

**Provider:** SQL Server via Docker  
**Skip policy:** `[SkippableFact]` when `SqlConnectionResolver` returns null or connection fails.

**What the round-trip proves:** Cross-aggregate FK, enum persistence, soft-delete filter on real SQL.

### Known gaps (accepted residual risk)

| Gap | Impact | Mitigation today |
|-----|--------|------------------|
| No `AtendimentoMappingTests` | Read DTO field drift | Manual review; add tests when extending read model |
| Soft-deleted PacienteId → 404 not explicit test | Likely correct via query filter | Add negative test when touching FK validation |
| Limited controller HTTP coverage | Status code regressions | Verify smoke or future API tests |
| `AtualizadoPor` never set | Audit incomplete | Auth ADR deferred |

When changing controller behavior, run Swagger smoke or extend controller tests — repository tests do not cover HTTP or FK validation.

---

## 7. Security And PHI Concepts

### What PHI means for Atendimento

Atendimento rows link to **PacienteId** and may carry **MensagemParaMedico** (clinical communication). Even without patient name on the Atendimento DTO, the record is clinically sensitive in context.

### What the implementation does

- No `Console.WriteLine` in `AtendimentoController`
- Error messages are generic (“corpo da requisição inválido”) — they do not echo message content or patient identifiers
- Tests use synthetic Paciente fixtures (fake names, generated CPF, example.com emails)
- Report routes return 501 without invoking Legacy PDF paths that might log patient content

### Safe patterns when extending

1. Do not log `MensagemParaMedico` or PacienteId in catch blocks.
2. Keep synthetic data in tests and smoke scripts.
3. When Workflow SDD adds pendências/events, apply the same PHI rules to new controllers and use cases.

See `Documentation/AI-Harness/review-prompts/security-phi-review.md` and the project `security-phi` rule.

---

## 8. Knowledge Map

Concept dependencies — read top-to-bottom within each chain when learning.

```text
Paciente aggregate (upstream FK)
    └── IPacienteRepository.GetByIdAsync [controller]
            └── Global query filter on Paciente [DbContext]
                    └── 404 when missing or soft-deleted

Atendimento create ownership
    └── Aggregate factory constructor [domain]
            └── Slim CreateAtendimentoDto [application]
                    └── Controller Post (no AutoMapper create)
                            └── AtendimentoRepository.CreateAsync [infrastructure]

Stage ownership
    └── EtapaAtual enum [domain]
            └── Factory sets Consulta only [minimal slice]
                    └── ReadAtendimentoDto exposes read-only stage
                            └── AvancarEtapa / ValidacaoEtapa* [workflow SDD — not here]

Soft delete
    └── ADR-001
            └── MarcarComoExcluido() [domain]
                    └── Global query filter [DbContext]
                            └── IgnoreQueryFilters() [tests]

Multi-journey model
    └── No unique (PacienteId) constraint [schema]
            └── GetByPacienteIdAsync [repository]
                    └── GET paciente/{pacienteId} [controller]

Cross-aggregate validation boundary
    └── Controller validates Paciente [API]
            └── Repository persistence-only [infrastructure]
                    └── Prontuario will validate AtendimentoId similarly [future]

SQL integration credentials
    └── .env SA_PASSWORD [harness]
            └── SqlConnectionResolver
                    └── AtendimentoSqlIntegrationTests
                            └── scripts/load-env.ps1 / sql-integration-test.ps1
```

```text
Report contract deferral
    └── Legacy PDF routes existed [reference]
            └── 501 Not Implemented [controller]
                    └── No repository / Legacy call [minimal slice]
                            └── Future workflow/report SDD [planned]

Read mapping only
    └── AtendimentoProfile
            └── ReadAtendimentoDto
                    └── global:: enum disambiguation [DTO namespace collision]
```

---

## 9. Study Guide

### Beginner — required before safely modifying this feature

| Topic | Why it matters | Where in this feature | Suggested order |
|-------|----------------|----------------------|-----------------|
| Aggregate factory create | Wrong create path loses PacienteId or wrong stage | `Atendimento` constructor; `AtendimentoController.Post` | 1 |
| Persistence-only repository | FK/workflow logic in repo creates coupling | `AtendimentoRepository` — compare with controller Post | 2 |
| Controller Paciente FK gate | 404 semantics depend on this check | `AtendimentoController.Post` + `AtendimentoControllerTests.Post_InvalidPacienteId_ReturnsNotFound` | 3 |
| Soft delete + query filter | Deleted rows must disappear from API | `MarcarComoExcluido`, `DocDbContext` filter, `DeleteAsync_SoftDeletesAtendimento` | 4 |
| Slim DTO shapes | Adding fields to wrong DTO leaks workflow concerns | `CreateAtendimentoDto`, `UpdateAtendimentoDto`, `ReadAtendimentoDto` | 5 |
| List-by-paciente semantics | Downstream UIs assume multiple journeys possible | `GetByPacienteIdAsync`, `GetByPacienteIdAsync_ReturnsAllForPatient` | 6 |

### Intermediate — explains why the code is structured as it is

| Topic | Why it matters | Where in this feature | Suggested order |
|-------|----------------|----------------------|-----------------|
| Workflow-owned EtapaAtual | Explains why update DTO has no stage field | Entity ctor vs `AtualizarMensagemParaMedico`; `UpdateAsync_ChangesMessageOnly` | 1 |
| Read-only AutoMapper | Explains absence of create map | `AtendimentoProfile` | 2 |
| 501 report preservation | Explains stub routes in Swagger | `GetPatientReportPdf`, `ReportRoutes_Return501` | 3 |
| EF FK Restrict + PacienteId index | Explains delete behavior and list performance | `AtendimentoConfiguration` | 4 |
| SqlConnectionResolver | Explains integration test skip vs fail | `SqlConnectionResolver`, `AtendimentoSqlIntegrationTests` | 5 |
| Carrier entity update pattern | Explains `Guid.Empty` in Update action | `AtendimentoController.UpdateAtendimento`, `UpdateAsync` | 6 |

### Advanced — extension, hardening, cross-feature implications

| Topic | Why it matters | Where in this feature | Suggested order |
|-------|----------------|----------------------|-----------------|
| DTO namespace collision | New DTOs named like entities need disambiguation | `ReadAtendimentoDto.EtapaAtual` global:: prefix | 1 |
| Child entity navigations | Workflow SDD will write Eventos/Pendencias | Entity collections + Fluent API relationships | 2 |
| Prontuario FK consumption | Next aggregate validates AtendimentoId same way | Compare controller Paciente check with future Prontuario create | 3 |
| Adding mapping tests | Close gap vs Paciente pilot | Missing `AtendimentoMappingTests` — add when read DTO grows | 4 |
| HTTP smoke vs unit tests | Controller status coverage strategy | verification.md Runtime Validation; controller test gaps | 5 |

---

## 10. Common Pitfalls

### Pitfall 1 — Putting Paciente FK validation in the repository

**What commonly breaks:** Hidden coupling — repository tests need Paciente setup; Prontuario/Agendamento patterns become inconsistent; agents copy the anti-pattern to every repo.

**Why it breaks:** The SDD assigned cross-aggregate existence checks to Controller/Application layer. Repository is persistence-only by architecture decision.

**How to avoid it:** Keep `GetByIdAsync` on `IPacienteRepository` in `AtendimentoController.Post` only. If Prontuario needs `AtendimentoId` validation, inject `IAtendimentoRepository` in **ProntuarioController**, not inside `ProntuarioRepository`.

---

### Pitfall 2 — Using AutoMapper (or DTO binding) for Atendimento create

**What commonly breaks:** Silent data loss — missing `PacienteId`, wrong or client-supplied `EtapaAtual`, orphan rows or invalid journey state.

**Why it breaks:** Legacy used denormalized DTOs; AutoMapper create maps do not invoke the aggregate factory. The minimal slice explicitly uses `new Atendimento(...)`.

**How to avoid it:** On create, always call the factory in the controller. Restrict `AtendimentoProfile` to read maps. If you add create mapping, stop and re-read `design.md` Aggregate creation ownership.

---

### Pitfall 3 — Exposing stage advancement or workflow fields on the minimal API

**What commonly breaks:** Scope creep into workflow SDD — partial `ValidacaoEtapa*` in controller, client-driven stage jumps, pendência writes without characterization tests.

**Why it breaks:** `EtapaAtual` is workflow-owned after factory sets `Consulta`. Entity methods `AvancarEtapa`, `RegistrarEvento`, `AdicionarPendencia` exist for future work but must not be wired to HTTP in this slice.

**How to avoid it:** Do not add `EtapaAtual` to create/update DTOs. Do not call `AvancarEtapa()` from controller or repository. Route workflow changes to `atendimento-workflow-stabilization` SDD.

---

### Pitfall 4 — Removing or implementing report routes partially

**What commonly breaks:** Swagger contract drift — frontend or integrators assume 501 routes are gone; partial PDF implementation pulls in Legacy dependencies and PHI risk.

**Why it breaks:** SDD requires routes **present** with 501 until workflow/report feature ships.

**How to avoid it:** Keep `report-id` and `followUp-id` actions returning 501 without repository calls. Extend `AtendimentoControllerTests.ReportRoutes_Return501` if routes change.

---

### Pitfall 5 — Credential mismatch disguised as Docker failure

**What commonly breaks:** `AtendimentoSqlIntegrationTests` skips; developers assume SQL Server is down; Verify records false residual risk.

**Why it breaks:** Different tools read passwords from user-secrets, missing `.env`, or stale Docker volume passwords.

**How to avoid it:** Copy `.env.example` → `.env`, set `SA_PASSWORD`, run `.\scripts\load-env.ps1` or `.\scripts\sql-integration-test.ps1 -Filter "FullyQualifiedName~AtendimentoSql"`. Prefer `SqlConnectionResolver` over ad-hoc connection strings.

---

## 11. Feature Evolution History

### Stub repository → SQL CRUD

- **Before:** Every `AtendimentoRepository` method threw `NotImplementedException`.
- **After:** Full CRUD + list-by-paciente against `DocDbContext`.
- **Rationale:** Prontuario and Agendamento require real `AtendimentoId` FK targets.

### String IDs → Guid contract

- **Before:** Legacy controller routes used string identifiers.
- **After:** Routes, DTOs, and repository interface use `Guid`.
- **Rationale:** Align with Paciente SQL slice and EF schema; abandon Sheets string keys.

### Bloated DTOs → slim create/update/read

- **Before:** Legacy DTOs carried denormalized workflow fields.
- **After:** Create/update DTOs carry only minimal safe fields; read DTO exposes current stage read-only.
- **Rationale:** Prevent client-owned journey state and silent mapping loss.

### AutoMapper create → factory on create

- **Before:** Risk of AutoMapper dropping `PacienteId` on create (Legacy characterization).
- **After:** Controller calls `new Atendimento(...)`; profile maps read only.
- **Rationale:** Aggregate factory owns invariants (`Consulta`, `Id`, timestamps).

### Repository validation → controller FK validation

- **Before:** Unclear ownership; agents might add Paciente checks in repository.
- **After:** `IPacienteRepository` lookup in controller; repository never reads Paciente.
- **Rationale:** Persistence-only repository pattern for SQL migration verticals.

### Report routes → 501 preserved

- **Before:** Legacy implemented PDF/followUp behavior in Sheets repository.
- **After:** Routes remain; controller returns 501 without implementation.
- **Rationale:** Avoid partial report feature and PHI exposure in minimal slice.

### PHI logging remediation

- **Before:** Development `Console.WriteLine` patterns in controller area.
- **After:** No console logging; generic HTTP errors.
- **Rationale:** REQ-009 and security-phi governance.

### Credential harness alignment (mid-Execute)

- **Before:** Integration tests, EF tools, and API resolved SQL passwords from different sources.
- **After:** `SqlConnectionResolver`, `.env.example`, load/run scripts.
- **Rationale:** Operational friction blocked Verify; not feature scope but required for reliable SQL integration evidence.

### Accepted tradeoffs

- **`AtualizadoPor` unset** — Auth ADR deferred; audit field exists in schema but not populated.
- **No mapping tests** — Paciente pilot pattern not yet copied; acceptable residual risk.
- **Workflow methods on entity unused** — Keeps domain ready for workflow SDD without exposing incomplete API.

---

## 12. Suggested Next Feature

**Prontuario SQL Stabilization** is the logical next forward SDD ([research.md](../prontuario-sql-stabilization/research.md)).

| Concept from Atendimento Minimal | Reuse in Prontuario | Expand |
|----------------------------------|---------------------|--------|
| Persistence-only repository | ProntuarioRepository — CRUD only | Nested child entities (versioning) |
| Controller FK validation | Validate `AtendimentoId` via `IAtendimentoRepository.GetByIdAsync` before create | May also validate Paciente depending on design |
| Factory / domain methods on create | Prontuario aggregate factory pattern | Versioning rules |
| Soft delete + query filter | Same ADR-001 pattern | Prontuario-specific filters |
| SQL integration test class | `ProntuarioSqlIntegrationTests` round-trip | Paciente → Atendimento → Prontuario chain |
| Slim DTOs + read AutoMapper | Same split | More complex read model if nested DTOs |
| Guid contract | Continue | — |

**Later:** Agendamento (same FK prerequisite pattern), then **Atendimento Workflow** (stage evaluators via Application Use Cases — **not** repository methods), then WS07 frontend with journey selector UI.

**Compare with Paciente guide:** Paciente taught single-aggregate CRUD and search. Atendimento Minimal adds **upstream FK validation** and **downstream FK provision** — the mental model Prontuario must adopt.

---

## 13. Quick Reference — Code Reading Order

Read in this order:

1. `DocAPI/Core/Entities/Atendimento.cs` — factory, safe update, soft delete; note workflow methods not wired to API
2. `DocAPI/Application/Data/Dtos/Atendimento/*.cs` — slim contract; note `global::` on read DTO enum
3. `DocAPI/Application/Mappings/Profiles/AtendimentoProfile.cs` — read map only
4. `DocAPI/Core/Interfaces/Repositories/IAtendimentoRepository.cs` — persistence contract
5. `DocAPI/Infrastructure/Repositories/AtendimentoRepository.cs` — SQL operations; confirm no FK checks
6. `DocAPI/API/Controllers/AtendimentoController.cs` — FK gate, factory create, 501 routes
7. `DocAPI/Infrastructure/SqlDb/Configurations/AtendimentoConfig.cs` — FK, enum string conversion, indexes
8. `DocAPI/Infrastructure/SqlDb/DbContext/DbContext.cs` — `DbSet<Atendimento>` and soft-delete filter line
9. `DocAPI.Tests/Infrastructure/AtendimentoRepositoryTests.cs` — behavior proof (InMemory)
10. `DocAPI.Tests/Controllers/AtendimentoControllerTests.cs` — FK and 501 proof (Moq)
11. `DocAPI.Tests/Integration/AtendimentoSqlIntegrationTests.cs` — SQL proof + Paciente dependency
12. `DocAPI/Infrastructure/SqlDb/SqlConnectionResolver.cs` — optional but important when tests skip

### Rationale

Start at the **domain factory** because create invariants (`Consulta`, `Id`) are the most violated boundary in this slice. DTOs and read profile come next so you see what clients send versus what the factory owns. Repository and controller follow — the **split of FK validation (controller) vs persistence (repository)** is the architectural lesson of aggregate #2. EF config and DbContext explain SQL shape and soft delete. Tests validate each layer’s claim in ascending environment fidelity.

### Dependency progression

Entity factory defines valid state → DTOs express allowed client input → repository persists without re-validating business rules → controller orchestrates FK check + factory + repository → EF maps to SQL → unit tests prove repository semantics → controller tests prove FK gate → integration test proves Paciente→Atendimento on real SQL.

---

## 14. Related Artifacts

| Artifact | Path |
|----------|------|
| SDD — Specify | [specify.md](../specify.md) |
| SDD — Design | [design.md](../design.md) |
| SDD — Tasks | [tasks.md](../tasks.md) |
| Verification | [verification.md](../verification.md) |
| Feature report | [reports/feature-report.md](../reports/feature-report.md) |
| Session handoff | [reports/session-handoff.md](../reports/session-handoff.md) |
| Pilot report (governance) | [sdd-pilot-report-v0.6.md](../sdd-pilot-report-v0.6.md) |
| ADR-001 Soft delete | [Documentation/Architecture/ADR/ADR-001-soft-delete.md](../../../Architecture/ADR/ADR-001-soft-delete.md) |
| Migration plan + API reference | [Documentation/Technical/migration-sql.md](../../../Technical/migration-sql.md) |
| Local runbook | [Documentation/Technical/runbook.md](../../../Technical/runbook.md) |
| Paciente Teacher Guide (baseline) | [../paciente-sql-stabilization/teacher-guide.md](../paciente-sql-stabilization/teacher-guide.md) |
| Operational status | [Documentation/State.md](../../../State.md) |
| Security review prompt | [Documentation/AI-Harness/review-prompts/security-phi-review.md](../../../AI-Harness/review-prompts/security-phi-review.md) |
| Prontuario research (next) | [../prontuario-sql-stabilization/research.md](../prontuario-sql-stabilization/research.md) |
| Workflow research (deferred) | [../atendimento-workflow-stabilization/research.md](../atendimento-workflow-stabilization/research.md) |
