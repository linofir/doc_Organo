# Teacher Guide — Paciente SQL Stabilization

> **Audience:** Junior developers, developers new to Doc Organo, future maintainers, and the project owner learning the architecture.
>
> **Prerequisite artifacts:** This guide teaches *how and why* the feature works. For scope, verification, and operational status, see the linked artifacts at the end — do not expect this document to repeat them.

---

## How This Guide Differs From Other Artifacts

| Artifact | What it answers | This guide does not repeat it |
|----------|-----------------|-------------------------------|
| `specify.md` / `design.md` | What to build and how to design it | Requirement tables, full API contract tables |
| `verification.md` | Whether it passed and what evidence exists | Gate results, completion decisions |
| `feature-report.md` | What shipped and workflow lessons | Delivery summary, PM references |
| `session-handoff.md` | What to do in the next session | Next steps, branch status |
| `State.md` / PM | Current operational truth | Runtime or backlog status |
| ADR-001 | Durable soft-delete decision | Full ADR text |
| `migration-sql.md` | Migration checklist and endpoint reference | Full route/status tables |

**This guide owns:** concepts, reasoning, architecture flow, code connections, and a study path so you can understand and safely extend the Paciente backend.

---

## 1. Feature Overview

### Business purpose

**Paciente** is the person served by the clinic — the root of cadastral identity (name, CPF, contact, insurance plan, address). Every clinical workflow (appointments, records, care episodes) eventually references a patient. Getting patient persistence right first reduces risk for the rest of the SQL migration.

### Technical objective

Paciente was the **first aggregate migrated from Google Sheets to SQL Server**. CRUD scaffolding already existed, but the backend was not production-ready: optional fields were not always persisted, the API search contract was ambiguous, PHI appeared in logs, and tests did not cover real SQL behavior.

This feature **stabilized** the existing vertical — it did not introduce a new aggregate or schema migration.

### Stabilization goals (what “done” meant for learning purposes)

1. **Full-field persistence** — All DTO fields (including Plano, Carteira, RG, and owned Endereco) round-trip to SQL.
2. **Correct delete semantics** — Soft delete per ADR-001; deleted patients disappear from normal queries.
3. **Clear API contract** — Single-resource lookups (by ID, by CPF) separated from collection search (paginated list, partial nome search).
4. **Safe handling of clinical identity data** — No patient-identifying data in controller logs or error messages.
5. **Test confidence** — Repository unit tests, mapping tests, and at least one SQL integration test against Docker SQL Server.

Frontend alignment was intentionally deferred (WS07). The backend is the learning baseline for the next aggregates.

---

## 2. What Changed

Think of the stabilization as four layers of fixes, not a greenfield rewrite.

### Domain layer

- **`ComplementarCadastro()`** — Central place to set optional cadastral fields (RG, Plano, Carteira, Endereco) after construction.
- **`MarcarComoExcluido()`** — Domain method that sets `Deletado` and `DeletadoEm`; delete is logical, not physical.
- **`AplicarAtualizacao()`** — Repository loads the tracked entity and applies changes through this method instead of replacing the row blindly.

### Application layer (mapping)

- **`PacienteProfile`** — Explicit AutoMapper maps for create and update using `ConstructUsing` + `AfterMap` so optional fields are not lost during DTO → entity conversion.
- **Guid → string** — `ReadPacienteDto.ID` is exposed as string for API consumers while the entity uses `Guid`.

### Infrastructure layer (repository + EF)

- **Update path** — Loads existing entity, calls `AplicarAtualizacao`, saves; throws `KeyNotFoundException` when missing.
- **Search semantics** — CPF exact match after trim; nome partial match via `Contains`; list ordered by `Nome` with pagination.
- **Soft delete** — `DeleteAsync` calls `MarcarComoExcluido()`; global query filter in `DocDbContext` hides deleted rows from default queries.

### API layer (controller)

- **PHI remediation** — All `Console.WriteLine` removed; errors are generic (“Paciente com CPF duplicado”, not patient names or CPF values).
- **HTTP semantics** — 201 + body on create; 204 on update/delete; 404 for missing single resources; 409 for duplicate CPF; 400 for invalid input.
- **Search contract** — `GET /Paciente/search?nome=` returns a collection (200 + array, possibly empty). The old single-result `GET /Paciente/nome/{nome}` route was retired.

### Tests

- Repository tests grew from basic CRUD to cover full fields, search semantics, pagination, and soft-delete exclusion.
- Mapping tests assert DTO ↔ entity round-trip for all fields.
- SQL integration test exercises create → read → update → soft delete against real SQL Server when Docker is available.

---

## 3. Architecture Walkthrough

Doc Organo uses a **light Clean Architecture** split. For Paciente, a typical **create** request flows like this:

```text
HTTP POST /Paciente (JSON body: CreatePacienteDto)
        │
        ▼
PacienteController.Post
        │  validates model (DataAnnotations on DTO)
        │  maps DTO → Paciente via IMapper / PacienteProfile
        ▼
Paciente (domain entity)
        │  constructed with required fields (Nome, Nascimento, CPF, Email, Telefone)
        │  ComplementarCadastro() applied in AfterMap for optional fields
        ▼
PacienteRepository.CreateAsync
        │  AplicarCriacao() sets CriadoEm
        │  _context.Pacientes.Add(paciente)
        │  SaveChangesAsync()
        ▼
EF Core + PacienteConfig
        │  maps entity to Paciente table + owned Endereco_* columns
        │  enforces unique index on CPF (IX_Paciente_CPF)
        ▼
SQL Server (Docker docorgano-sql)
        │
        ▼
Controller maps Paciente → ReadPacienteDto
        │
        ▼
HTTP 201 Created + ReadPacienteDto body
```

### Read by ID (simpler path)

```text
GET /Paciente/{id}
  → PacienteController.GetByID
  → PacienteRepository.GetByIdAsync
  → EF query on Pacientes (global filter excludes Deletado = true)
  → null → 404 | entity → map to ReadPacienteDto → 200
```

### Soft delete

```text
DELETE /Paciente/{id}
  → PacienteRepository.DeleteAsync
  → load entity (or KeyNotFoundException → 404)
  → MarcarComoExcluido()
  → SaveChangesAsync
  → subsequent GetByIdAsync returns null (query filter)
```

### Layer responsibilities (Paciente slice)

| Layer | Location | Responsibility |
|-------|----------|----------------|
| API | `DocAPI/API/Controllers/PacienteController.cs` | HTTP mapping, status codes, input checks, exception → status translation |
| Application | `DocAPI/Application/Mappings/Profiles/PacienteProfile.cs` | DTO ↔ entity mapping |
| Domain | `DocAPI/Core/Entities/Paciente.cs` | Entity structure and behavior methods |
| Contract | `DocAPI/Core/Interfaces/Repositories/IPacienteRepository.cs` | Persistence operations the API depends on |
| Infrastructure | `DocAPI/Infrastructure/Repositories/PacienteRepository.cs` | EF Core queries and persistence |
| EF config | `PacienteConfig.cs`, `DocDbContext.cs` | Table mapping, owned Endereco, indexes, soft-delete filter |

The controller **never** talks to `DocDbContext` directly. It always goes through `IPacienteRepository` — that boundary is the reference pattern for Prontuario, Agendamento, and Atendimento.

---

## 4. Concept Inventory

Each concept below was **demonstrably used** in this feature. Study priority: **High** = required before modifying code; **Medium** = deepens understanding; **Low** = useful for extension or hardening.

### Domain concepts

| Concept | Appears in | Why it exists | How it works | Related concepts | Study priority |
|---------|------------|---------------|--------------|------------------|----------------|
| Aggregate root (Paciente) | `Paciente.cs` | Paciente owns cadastral identity and links to clinical entities | Entity with private setters and behavior methods | Value object, repository | High |
| Value object (Endereco) | `Paciente.Endereco`, `PacienteConfig.OwnsOne` | Address is part of patient cadastral data, not a separate aggregate | Owned type embedded in Paciente row (`Endereco_*` columns) | EF owned entities | Medium |
| Rich domain methods | `ComplementarCadastro`, `AplicarAtualizacao`, `MarcarComoExcluido`, `AplicarCriacao` | Keep state changes explicit and testable | Methods mutate entity in controlled ways | Repository orchestration | High |
| Soft delete | `MarcarComoExcluido`, ADR-001 | Preserve clinical history; never physically remove patient rows | Sets `Deletado` / `DeletadoEm`; filter hides from queries | Global query filter | High |

### Application concepts

| Concept | Appears in | Why it exists | How it works | Related concepts | Study priority |
|---------|------------|---------------|--------------|------------------|----------------|
| DTO pattern | `CreatePacienteDto`, `UpdatePacienteDto`, `ReadPacienteDto` | API contract separate from persistence model | Controller exposes DTOs; repository works with entities | AutoMapper | High |
| AutoMapper profiles | `PacienteProfile.cs` | Avoid manual field copying in controllers | `CreateMap` with `ConstructUsing` + `AfterMap` | DI (`IMapper`) | High |
| Two-phase entity construction | `PacienteProfile` create/update maps | Required ctor args vs optional fields | `ConstructUsing` builds with required fields; `AfterMap` calls `ComplementarCadastro` | Domain methods | High |

### Infrastructure concepts

| Concept | Appears in | Why it exists | How it works | Related concepts | Study priority |
|---------|------------|---------------|--------------|------------------|----------------|
| Repository pattern | `IPacienteRepository`, `PacienteRepository` | Hide EF details from API layer | Interface in Core; implementation in Infrastructure | DI, DbContext | High |
| EF Core DbContext | `DocDbContext.cs` | Unit of work for SQL persistence | `DbSet<Paciente>`, `SaveChangesAsync` | LINQ, Fluent API | High |
| Fluent API configuration | `PacienteConfig.cs` | Map entity to table, columns, indexes | `IEntityTypeConfiguration<Paciente>` | Owned types, indexes | Medium |
| Global query filter | `DocDbContext.ApplySoftDeleteQueryFilter` | Default queries exclude soft-deleted rows | `HasQueryFilter(p => !p.Deletado)` | ADR-001, `IgnoreQueryFilters()` | High |
| Owned entity mapping | `PacienteConfig` — `OwnsOne(x => x.Endereco)` | Store address columns on Paciente table | Prefixed columns `Endereco_Logradouro`, etc. | Value object | Medium |
| Unique index (CPF) | `PacienteConfig.HasIndex(CPF).IsUnique()` | One patient per CPF in SQL | DB constraint; duplicate insert → `DbUpdateException` | Exception handling in controller | Medium |
| LINQ queries | `PacienteRepository` | Express search and pagination in database | `Contains`, `OrderBy`, `Skip`, `Take` | EF Core | High |

### API concepts

| Concept | Appears in | Why it exists | How it works | Related concepts | Study priority |
|---------|------------|---------------|--------------|------------------|----------------|
| REST single-resource vs collection | `PacienteController` routes | Different HTTP semantics for “one patient” vs “search results” | `/{id}`, `/cpf/{cpf}` vs `?skip/take` and `/search?nome=` | HTTP status codes | High |
| HTTP status mapping | All controller actions | Predictable API for frontend and Swagger | 201/200/204/404/409/400 | Exception filters | High |
| Exception → HTTP translation | `Post`, `UpdatePaciente`, `DeletePaciente` | Map persistence errors to client-facing codes | `KeyNotFoundException` → 404; duplicate CPF → 409 | EF exceptions | Medium |
| Pagination | `GetPacientes`, `SearchByNome` | Avoid loading entire patient table | Query params `skip`, `take` with validation | Repository | Medium |
| Dependency injection | Controller constructor | Testability and layer separation | Injects `IPacienteRepository`, `IMapper` | ASP.NET Core DI | High |

### Testing concepts

| Concept | Appears in | Why it exists | How it works | Related concepts | Study priority |
|---------|------------|---------------|--------------|------------------|----------------|
| InMemory EF tests | `PacienteRepositoryTests` | Fast feedback without Docker | `UseInMemoryDatabase` + repository directly | xUnit | High |
| Mapping unit tests | `PacienteMappingTests` | Catch DTO field loss independent of DB | `MapperConfiguration` + assert all fields | AutoMapper | Medium |
| SQL integration tests | `PacienteSqlIntegrationTests` | Prove behavior against real SQL provider | `UseSqlServer` + `[SkippableFact]` | Docker, env vars | Medium |
| Skip policy for integration | `GetConnectionString`, `Skip.If` | CI/local without SQL must not fail build | Skip when `SA_PASSWORD` / Docker unavailable | runbook.md | Medium |
| Synthetic test data | All test classes | PHI must not appear in tests or docs | Fake names, generated CPF patterns | security-phi rule | High |
| `IgnoreQueryFilters()` in tests | `DeleteAsync_SoftDeletesPaciente` | Prove soft delete sets flag while filter hides row | Direct context query bypasses filter | Query filters | Medium |

### Security concepts

| Concept | Appears in | Why it exists | How it works | Related concepts | Study priority |
|---------|------------|---------------|--------------|------------------|----------------|
| PHI (Protected Health Information) | Project rules, controller | Patient names, CPF, contact data are sensitive | Never log or echo in errors/commits | Safe logging | High |
| PHI-safe error messages | `PacienteController` | Clients get actionable but non-identifying errors | Generic Portuguese messages | No `Console.WriteLine` | High |

### Concepts not used in this feature (do not expect them here)

Use Cases layer, MediatR/CQRS, Domain Events, Unit of Work abstraction, auth/RBAC, CPF checksum validation, automated controller/API tests, new EF migrations, feature flags.

---

## 5. Deep Dive By Component

### Paciente (`DocAPI/Core/Entities/Paciente.cs`)

**Responsibility:** Represent the patient aggregate root in the domain layer.

**Construction:** Public constructor takes required fields (Nome, Nascimento, CPF, Email, Telefone) and assigns a new `Guid` ID. Protected parameterless constructor exists for EF materialization.

**Key methods you will call from mapping or repository:**

- `ComplementarCadastro(rg, plano, carteira, endereco)` — Sets optional cadastral fields after creation. This is how Plano, Carteira, RG, and Endereco reach the entity when mapping from DTOs.
- `AplicarCriacao()` — Sets `CriadoEm` on first persist (called from repository, not controller).
- `AplicarAtualizacao(...)` — Applies all updatable fields and sets `AtualizadoEm`. Repository loads tracked entity and calls this.
- `MarcarComoExcluido()` — Soft delete; sets `Deletado = true` and `DeletadoEm`.

**Computed property:** `Idade` is calculated from `Nascimento` — not stored in SQL.

**Navigation collections:** `Prontuarios`, `Agendamentos`, `Atendimentos` exist for EF relationships; this feature did not implement those repositories yet.

---

### IPacienteRepository / PacienteRepository

**Interface location:** `DocAPI/Core/Interfaces/Repositories/IPacienteRepository.cs` — defines the contract Legacy Sheets also implemented, so the API layer stays stable across persistence migrations.

**Implementation location:** `DocAPI/Infrastructure/Repositories/PacienteRepository.cs`

| Method | Behavior |
|--------|----------|
| `GetAllAsync(skip, take)` | Active patients ordered by Nome, paginated |
| `GetByIdAsync(id)` | Single patient or null (respects soft-delete filter) |
| `GetPacienteByCpfAsync(cpf)` | Exact match after trim; returns list (controller expects 0 or 1) |
| `GetPacienteByNomeAsync(nome, skip, take)` | Partial match `Contains`, ordered, paginated |
| `CreateAsync` | `AplicarCriacao()`, Add, SaveChanges |
| `UpdateAsync` | Load by id, `AplicarAtualizacao` from mapped entity, SaveChanges; throws if missing |
| `DeleteAsync` | Load by id, `MarcarComoExcluido`, SaveChanges; throws if missing |

**Design choice:** Update does not attach a new entity with `Update()`. It mutates the tracked row — safer for owned Endereco and audit fields.

---

### PacienteController (`DocAPI/API/Controllers/PacienteController.cs`)

**Responsibility:** Translate HTTP to repository calls and map entities back to DTOs. No business rules beyond input validation and HTTP semantics.

**Routes (as implemented):**

| HTTP | Route | Role |
|------|-------|------|
| POST | `/Paciente` | Create → 201 + `ReadPacienteDto` |
| GET | `/Paciente` | Paginated list |
| GET | `/Paciente/search` | Collection search by nome |
| GET | `/Paciente/{id}` | Single resource by Guid |
| GET | `/Paciente/cpf/{cpf}` | Single resource by CPF |
| PUT | `/Paciente/{id}` | Update → 204 |
| DELETE | `/Paciente/{id}` | Soft delete → 204 |

**Duplicate CPF handling:** `IsDuplicateCpfViolation` inspects `DbUpdateException` message for index name or SQL duplicate-key phrases. This is pragmatic for MVP2 but fragile if index names change — an advanced topic for later hardening.

**CPF by route:** Controller trims input; repository trims again. Controller returns 409 if repository returns more than one row (data integrity violation).

---

### PacienteProfile (`DocAPI/Application/Mappings/Profiles/PacienteProfile.cs`)

**Responsibility:** Configure AutoMapper maps between DTOs and `Paciente` / `Endereco`.

**Critical pattern — create and update maps:**

```csharp
CreateMap<CreatePacienteDto, Paciente>()
    .ConstructUsing(dto => new Paciente(dto.Nome, dto.Nascimento, dto.CPF, dto.Email, dto.Telefone))
    .AfterMap((dto, dest) => dest.ComplementarCadastro(...));
```

Why not a single flat map? The entity constructor only accepts required fields. Optional fields and owned Endereco are applied in `AfterMap` via `ComplementarCadastro`. **If you add a new DTO field, you must update both the domain method and this profile** — mapping tests will catch omissions.

**Read map:** `Paciente` → `ReadPacienteDto` converts `Guid ID` to string for JSON consumers.

---

### DocDbContext (`DocAPI/Infrastructure/SqlDb/DbContext/DbContext.cs`)

**Responsibility:** EF Core entry point — connection, `DbSet`s, conventions, and cross-cutting filters.

**Paciente-relevant parts:**

- `DbSet<Paciente> Pacientes`
- `ApplySoftDeleteQueryFilter` registers `HasQueryFilter(p => !p.Deletado)` for Paciente (and other clinical aggregates)
- `DateOnly` conversion for `Nascimento`

**Learning note:** The filter applies automatically to all normal queries. Audit or admin scenarios that need deleted rows must use `.IgnoreQueryFilters()` explicitly (see soft-delete test).

---

### PacienteConfig (`DocAPI/Infrastructure/SqlDb/Configurations/PacienteConfig.cs`)

**Responsibility:** Fluent API mapping for the Paciente table — column lengths, required fields, owned Endereco columns, indexes.

**Important mappings:**

- Table name: `Paciente`
- Primary key: `ID` (Guid)
- **Owned Endereco** → columns prefixed `Endereco_`
- **Unique index on CPF** → duplicate inserts fail at database level
- Index on Email (non-unique)

When debugging “field not persisted”, check three places: DTO → Profile → PacienteConfig (is the property mapped and allowed?).

---

### Integration tests (`DocAPI.Tests/Integration/PacienteSqlIntegrationTests.cs`)

**Responsibility:** Prove the repository works against **real SQL Server**, not InMemory provider.

**Connection resolution order:**

1. Environment variable `DOCORGANO_TEST_CONNECTION` if set
2. Otherwise build connection from `SA_PASSWORD` and Docker default (`127.0.0.1:1433`, database `DocDb`)

**Skip policy:** Test skips (does not fail) when password or Docker is unavailable. This keeps `dotnet test` green in environments without SQL while still allowing full validation locally.

**What the round-trip test proves:** Create with all fields → read → update → soft delete → patient absent from get-by-id, CPF, and nome search.

---

## 6. Testing Walkthrough

Tests serve different learning and confidence goals. Use the right type when extending Paciente or copying patterns to Prontuario.

### Repository unit tests (`PacienteRepositoryTests`)

**Provider:** EF Core InMemory  
**Speed:** Milliseconds  
**What they prove:** Repository query logic, soft-delete interaction with query filter, pagination, search semantics

**Representative tests to read in order:**

1. `CreateAsync_PersistsAllFields` / `UpdateAsync_PersistsAllFields` — full cadastral round-trip
2. `GetPacienteByCpfAsync_UsesExactMatch` — partial CPF must not match
3. `GetPacienteByNomeAsync_ReturnsPartialMatches` — `Contains` behavior
4. `DeleteAsync_SoftDeletesPaciente` — row still exists with `IgnoreQueryFilters()`
5. `DeleteAsync_ExcludesPatientFromDefaultQueries` — all read paths respect delete

**Why InMemory here:** Fast feedback loop while developing repository logic. InMemory is not SQL Server — that gap is why integration tests exist.

### Mapping tests (`PacienteMappingTests`)

**What they prove:** AutoMapper configuration maps every DTO field to the entity (and back for read DTO)

**Why separate from repository tests:** Mapping bugs fail before SQL is touched. When REQ-001 failed originally, the fix was in `PacienteProfile` — mapping tests prevent regression.

### SQL integration tests (`PacienteSqlIntegrationTests`)

**Provider:** SQL Server via Docker  
**What they prove:** Fluent mappings, owned types, unique constraints, and query filters behave correctly on real SQL

**Why SkippableFact:** SQL is an environment dependency documented in `runbook.md`. Skipping is acceptable; failing the whole suite because Docker is off is not.

### What is not automated (know the gap)

- Controller HTTP status codes (validated manually via Swagger during Execute)
- Empty nome search returning `200` + `[]` (behavior is implemented; no dedicated test)

When you change controller behavior, run Swagger smoke or add API tests — do not assume repository tests cover HTTP.

---

## 7. Security And PHI Concepts

### What PHI means in this project

**Protected Health Information** includes anything that identifies a patient or their care context: name, CPF, RG, email, phone, address, clinical notes. Doc Organo treats all patient data as sensitive even in a private repository.

### What was wrong before stabilization

The controller used `Console.WriteLine` with patient data during development. Console output can end up in logs, terminal history, CI output, or screenshots — all unacceptable for clinical identity data.

### What the implementation does now

- No `Console.WriteLine` in `PacienteController`
- Error responses are **generic** (“Paciente com CPF duplicado”) — they do not echo submitted PHI
- Tests use **synthetic** names, emails, and generated CPF patterns — never real patient data

### Safe logging principles (apply beyond Paciente)

1. Do not log request bodies for clinical endpoints.
2. Do not log identifiers (CPF, name) in catch blocks.
3. Use structured logging without PHI fields when logging is added later.
4. Keep synthetic data in tests, docs, prompts, and commits.

For review checklist details, see `Documentation/AI-Harness/review-prompts/security-phi-review.md` and the project `security-phi` rule.

---

## 8. Knowledge Map

Concept dependencies — read top-to-bottom within each chain when learning.

```text
HTTP / REST basics
    └── ASP.NET Core Controllers
            └── DTO pattern
                    └── AutoMapper (PacienteProfile)
                            └── Domain entity (Paciente)
                                    └── Repository pattern (PacienteRepository)
                                            └── EF Core DbContext
                                                    ├── Fluent API (PacienteConfig)
                                                    ├── LINQ
                                                    ├── Owned types (Endereco)
                                                    └── SQL Server

Soft delete (business rule)
    └── ADR-001
            └── MarcarComoExcluido() [domain]
                    └── Global query filter [DbContext]
                            └── IgnoreQueryFilters() [tests / future audit code]

Duplicate CPF (integrity)
    └── Unique index [PacienteConfig]
            └── DbUpdateException [EF]
                    └── IsDuplicateCpfViolation [controller]
                            └── HTTP 409

PHI safety
    └── security-phi rule
            └── Generic errors + no console logging [controller]
                    └── Synthetic test fixtures [tests]

Testing pyramid (Paciente slice)
    └── Mapping tests (fast, isolated)
            └── Repository InMemory tests (behavior)
                    └── SQL integration tests (environment-dependent)
                            └── Swagger manual smoke (HTTP layer, not automated)
```

---

## 9. Study Guide

### Beginner — read before modifying Paciente code

| Topic | Why it matters | Where in this feature | Suggested order |
|-------|----------------|----------------------|-----------------|
| C# classes and async/await | All API and repository methods are async | Entire vertical | 1 |
| ASP.NET Core Web API basics | Controllers, routes, status codes | `PacienteController` | 2 |
| JSON DTOs vs domain entities | API shape differs from persistence model | DTOs + `Paciente` | 3 |
| EF Core getting started | DbContext, SaveChanges, basic LINQ | `PacienteRepository`, `DocDbContext` | 4 |
| xUnit fundamentals | Project test runner | `DocAPI.Tests` | 5 |

**External:** [ASP.NET Core Web API](https://learn.microsoft.com/en-us/aspnet/core/web-api/), [EF Core overview](https://learn.microsoft.com/en-us/ef/core/)

### Intermediate — read to understand *why* the code is shaped this way

| Topic | Why it matters | Where in this feature | Suggested order |
|-------|----------------|----------------------|-----------------|
| Repository pattern | Consistent persistence boundary for all aggregates | `IPacienteRepository`, `PacienteRepository` | 1 |
| AutoMapper profiles | Prevents manual mapping bugs on create/update | `PacienteProfile`, `PacienteMappingTests` | 2 |
| EF owned entities | Endereco stored in Paciente table | `PacienteConfig.OwnsOne` | 3 |
| Global query filters | Soft delete without deleting rows | `DocDbContext`, delete tests | 4 |
| REST collection vs single resource | Explains search route redesign | Controller GET routes, SDD `design.md` | 5 |
| PHI-safe API design | Required for all clinical endpoints | Controller error handling | 6 |

**Internal:** [design.md](design.md) (API Contract Decision section), [ADR-001-soft-delete.md](../../Architecture/ADR/ADR-001-soft-delete.md)

### Advanced — read before extending or hardening

| Topic | Why it matters | Where in this feature | Suggested order |
|-------|----------------|----------------------|-----------------|
| Query filters + relationships | ADR-001 caveats on related entities | ADR-001, DbContext | 1 |
| Duplicate key detection via exceptions | Fragile string matching on SQL errors | `IsDuplicateCpfViolation` | 2 |
| InMemory vs SQL Server provider differences | InMemory tests do not catch all SQL issues | Repository vs integration tests | 3 |
| Legacy preserve/adapt/abandon | Methodology for next aggregates | `specify.md` Legacy Behavior table | 4 |
| Integration test skip policies | CI design for Docker-dependent tests | `PacienteSqlIntegrationTests`, `runbook.md` | 5 |
| Missing controller tests | Known gap; options for Prontuario pilot | verification findings | 6 |

---

## 10. Suggested Next Feature — Prontuario

The migration plan orders aggregates: **Paciente → Prontuario → Agendamento → Atendimento**. Prontuario is the logical next forward SDD pilot.

### Concepts you already learned from Paciente (reuse directly)

- Repository + interface in Core/Infrastructure split
- EF Fluent API config class per aggregate
- AutoMapper profile for DTO round-trip
- Controller → repository → DbContext flow
- InMemory repository tests + SQL integration test with skip policy
- Soft delete and global query filter (already on Prontuario in DbContext)
- PHI-safe controller patterns

### Concepts that will expand

| Area | Paciente baseline | Prontuario likely adds |
|------|-------------------|------------------------|
| Relationships | Navigation properties unused in API | Foreign key to Paciente; loading related data |
| Domain complexity | Cadastral CRUD | Clinical record semantics, tabs, possibly PDF |
| Frontend | Deferred (WS07) | Prontuario Blazor tabs per migration checklist |
| Legacy port | Simple Sheets CRUD reference | Richer Legacy behavior to characterize |
| Query filters | Single-entity filter | Relationship + filter interaction (ADR-001 short-term action) |

**Study approach for Prontuario:** Re-read this guide’s architecture walkthrough, then read `ProntuarioSheetsRepository` in Legacy as behavioral reference only — implement against SQL patterns established here, not by copying Sheets code.

---

## Quick Reference — Files To Open First

When onboarding to Paciente backend, read in this order:

1. `DocAPI/Core/Entities/Paciente.cs` — domain behavior
2. `DocAPI/Application/Mappings/Profiles/PacienteProfile.cs` — DTO mapping
3. `DocAPI/Infrastructure/Repositories/PacienteRepository.cs` — persistence
4. `DocAPI/API/Controllers/PacienteController.cs` — HTTP surface
5. `DocAPI/Infrastructure/SqlDb/Configurations/PacienteConfig.cs` — SQL mapping
6. `DocAPI.Tests/Infrastructure/PacienteRepositoryTests.cs` — expected behavior
7. `DocAPI.Tests/Integration/PacienteSqlIntegrationTests.cs` — SQL proof

---

## Related Artifacts (for context, not repetition)

| Artifact | Path |
|----------|------|
| SDD specify | [specify.md](specify.md) |
| SDD design (API contract rationale) | [design.md](design.md) |
| SDD tasks | [tasks.md](tasks.md) |
| Verification evidence | [verification.md](verification.md) |
| Feature report | [reports/feature-report.md](reports/feature-report.md) |
| Soft delete ADR | [ADR-001-soft-delete.md](../../Architecture/ADR/ADR-001-soft-delete.md) |
| Migration plan + API table | [migration-sql.md](../../Technical/migration-sql.md) |
| Local SQL + integration test setup | [runbook.md](../../Technical/runbook.md) |
| Domain language | [Domain_Overview_Business_Rules.md](../../Architecture/Domain_Overview_Business_Rules.md) |
