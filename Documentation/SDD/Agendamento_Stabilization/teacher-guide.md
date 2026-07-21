# Teacher Guide — Agendamento SQL Migration & Stabilization

> Feature SDD: `Documentation/SDD/Agendamento_Stabilization/`
> **Guide Date:** 2026-07-20
> **Lifecycle position:** Research → Specify → Design → Tasks → SDD Pre-Execution Review → Execute → Verify → Documentation Follow-Up → Reporting → **Teacher Guide**
> **Guide type:** New

## Eligibility Decision

| Item | Value |
|------|-------|
| Decision | **Generate** |
| Reason | The Feature Report recommended Skip on the basis that Agendamento follows patterns already documented in prior Teacher Guides. However, Agendamento is the first vertical that combines **entity-schema alignment without migration delta**, **three-FK validation on create with structured error bodies**, and **owned value object mutation through a dedicated domain method** — a combination not present in any prior vertical. The explicit user request for generation confirms this decision. |

---

## Knowledge Inventory

The following concepts were identified from the verified implementation. This inventory was extracted from production code (primary source), tests, the active SDD, and verification evidence.

### Architectural Concepts

| Concept | Category | Description |
|---------|----------|-------------|
| Single-aggregate SQL vertical migration | Architectural | The pattern of migrating one aggregate at a time from stub → SQL persistence, following a sequenced dependency order. Agendamento is the fourth vertical (Paciente → Atendimento Minimal → Prontuario → Agendamento). |
| Entity-schema alignment without migration delta | Architectural | Surfacing an existing database column (`PacienteID`) on the C# entity model without generating a new EF migration. Requires precise property naming (matching the column name via convention) and FK relationship mapping that references the existing constraint. |
| Three-FK validation with structured error body | Architectural | Validating three foreign key targets from separate sources (repository, DbContext.Set, repository) using fail-fast semantics. Each failure returns HTTP 404 with a structured JSON body identifying the failing field by name only — no internal identifiers leaked. |
| Repository as persistence-only boundary | Architectural | The repository performs SQL CRUD only. FK validation, business rules, PHI handling, and HTTP concerns belong to the application layer (controller) and aggregate. This separation is consistent across all four SQL verticals. |
| Domain operations as sole mutation paths | Architectural | The aggregate controls all state changes through dedicated methods (constructor/factory, `Update`, `SoftDelete`, `SetSenha`). Automated object mapping (AutoMapper) is disabled for write paths — DTO → entity mapping is manual, routed through domain methods. |
| Write/read AutoMapper split | Architectural | Write-direction mappings (DTO → aggregate) use manual construction through domain operations. Read-direction mappings (aggregate → DTO) may use automated mapping. This pattern prevents mapping tools from bypassing aggregate invariants. |
| Soft delete per ADR-001 | Architectural | Domain method sets `Deletado = true` and `DeletadoEm`. Global EF query filter excludes deleted rows from all standard reads. Repository DeleteAsync invokes the domain method rather than physical deletion. |

### Domain Concepts

| Concept | Category | Description |
|---------|----------|-------------|
| Agendamento as administrative aggregate | Domain | Agendamento controls scheduling, location, authorization codes (senhas), and procedural status tracking. It is administrative — clinical journey validation is owned by Atendimento Workflow. |
| Owned value object — `SenhaAgendamento` | Domain | `SenhaAgendamento` is an owned type with four properties (`Codigo`, `DataPedido`, `DataLiberacao`, `Validade`) mapped inline on the parent table as `SenhaAgendamento_*` columns. Mutation is controlled by `SetSenha()` — no direct property access. |
| Status lifecycle via domain enums | Domain | Three enums (`StatusAgendamento`, `StatusInstrucoes`, `StatusAtestado`) represent procedural state. Stored as strings in the database via `HasConversion<string>()` for readability and resilience against ordinal reordering. |
| Procedimento derived from Prontuario aggregate | Domain | The `Procedimento` string field (present in Legacy Sheets) is abandoned. Procedure information is derived from `Internacao → ProcedimentoInternacao` — the Prontuario aggregate owns this data. Agendamento reads it through navigation, not duplication. |
| FK dependency chain | Domain | Agendamento requires valid `InternacaoId` (child of Prontuario) and `AtendimentoId`. `PacienteId` is optional and nullable. This chain enforces that scheduling can only occur within a procedure context (Internacao) and a care journey (Atendimento). |

### Persistence Concepts

| Concept | Category | Description |
|---------|----------|-------------|
| EF Fluent configuration with owned types | Persistence | `AgendamentoConfiguration` maps the entity to the `Agendamento` table with explicit column types, enum-to-string conversions, owned-type properties via `OwnsOne`, FK relationships via `HasOne().WithMany().HasForeignKey`, and composite indexes. |
| Zero migration delta constraint | Persistence | The `InitialCreate` schema is treated as source of truth. No new migration is generated. Every entity property maps to an existing column. Entity changes (adding `PacienteId`) expose schema columns that already exist. |
| Global query filter for soft delete | Persistence | `DocDbContext` applies `HasQueryFilter(a => !a.Deletado)` globally. Soft-deleted rows are transparently excluded from all LINQ queries. Audit/report scenarios requiring deleted rows must use `.IgnoreQueryFilters()`. |
| Pagination with clamping | Persistence | `GetAllAsync` respects `skip`/`take` parameters with a hard ceiling of 100 results (via `Math.Clamp`). Default page size is 10. Ordering is `Data DESC, Horario ASC`. |

### API Concepts

| Concept | Category | Description |
|---------|----------|-------------|
| Guid route constraints | API | All route templates use `{id:guid}` — the routing layer rejects non-Guid values before the controller action executes. This eliminates string-to-Guid parsing errors in controller code. |
| PHI remediation through removal | API | All `Console.WriteLine` calls logging patient-identifying data (nome, ID) are removed. No structured logging replaces them. The controller produces zero diagnostic output — no risk of PHI in logs. |
| Structured FK error responses | API | FK validation failures return HTTP 404 with a JSON body containing `title`, `status`, `field` (the FK name only), and `detail`. The failing value is never echoed. This pattern is composable across endpoints. |
| CreatedAtAction with Guid identity | API | `POST` uses `CreatedAtAction(nameof(GetByID), new { id = agendamento.ID }, dto)` — the `Location` header carries a Guid route. This matches the pattern established by prior verticals. |
| Empty array for no-match queries | API | `GET /Agendamento?skip=&take=`, `GET /Agendamento/by-name`, and `GET /Agendamento/by-pacientId` return `200 OK` with an empty array when no results match — never 404. |

### Testing Concepts

| Concept | Category | Description |
|---------|----------|-------------|
| Three-suite test structure | Testing | Entity tests (Business Invariants) + Repository tests with InMemoryDatabase (Persistence Intent) + Controller tests with mocks (API Contract). Provides strong coverage without requiring a running SQL instance for the build/test gate. |
| SQL integration tests as physical persistence gate | Testing | CRT round-trip against Docker SQL Server using `SkippableFact` — skips gracefully when the environment is unavailable. Tests create a full FK prerequisite chain (Paciente → Atendimento → Prontuario → Internacao → Agendamento). |
| Synthetic fixture data | Testing | All test data is synthetic — no real patient names or clinical data. This satisfies the PHI security requirement across all test suites. |
| Environment-Dependent Evidence policy | Testing | SQL integration and Swagger smoke gates are skippable when Docker is unavailable. The Verifier accepts residual risk based on: integration test source exists, controller unit tests cover status code paths, and entity properties map to existing schema columns. |

### Reusable Patterns

| Pattern | Prior vertical precedent? | What's new in Agendamento |
|---------|--------------------------|---------------------------|
| Stub → SQL repository | Yes (Paciente, Atendimento, Prontuario) | Same pattern — full CRUD against `DocDbContext` |
| Guid migration | Yes (all three priors) | Same pattern — all ID parameters use `Guid` |
| FK validation on create | Yes (Atendimento, Prontuario) | **New:** Three simultaneous FK validations from three different source types (repository, DbContext.Set, repository) with fail-fast structured 404 body |
| Soft delete | Yes (all three priors) | Same pattern — domain method + global query filter |
| PHI remediation | Yes (all three priors) | Same pattern — remove diagnostic logging, no structured replacement |
| Domain methods for mutation | Yes (all three priors) | **New:** `SetSenha()` — owned value object mutation through a dedicated domain method with null-to-clear semantics |
| Entity-schema alignment without migration | No | **New to this vertical** — `PacienteID` column existed in schema but not on entity |
| DTO enum type safety | Partial (Prontuario) | **New:** All three DTO types use domain enums (not strings) for status fields |

### Common Pitfalls

1. **Adding `PacienteId` to the entity and generating a migration.** If the property name doesn't match the existing column name via convention, EF will try to add a new column. The fix: name the property `PacienteID` (matching the schema column) and configure the FK relationship to reference the existing constraint.
2. **Allowing AutoMapper to write directly to aggregate properties.** This bypasses the factory (`Status = SemSenha`, `CriadoEm`) and domain methods (`SetSenha`, `Update`). The fix: remove write-direction AutoMapper mappings entirely and construct aggregates manually in the controller.
3. **Putting FK validation in the repository.** This violates the "repository is persistence-only" boundary. FK validation is an application concern — it uses multiple dependency sources and produces HTTP-specific error responses. The fix: perform FK checks in the controller before aggregate construction.
4. **Returning 400 for FK validation failures.** "Referenced resource not found" is semantically a 404 — the client is requesting creation with a reference to something that doesn't exist. The fix: use 404 with a structured body identifying the failing field.
5. **Echoing the submitted FK value in error responses.** This can leak internal identifiers. The fix: include only the field name in the error body (`"field": "atendimentoId"`), never the value.

### Recommended Follow-Up Study

| Topic | Why | Where to learn |
|-------|-----|----------------|
| Atendimento Workflow Stabilization | Next SDD — porting Legacy journey orchestration (~700 LOC). Agendamento is the last blocking prerequisite. | `Documentation/SDD/atendimento-workflow-stabilization/` (pending) |
| WS07 Blazor frontend alignment | Guid, enum drift, Procedimento removal, and UI smoke for all four verticals | `Documentation/Technical/front-architecture.md` |
| ADR-001 soft delete | Understand the full lifecycle: domain method, global query filter, `.IgnoreQueryFilters()` for audit | `Documentation/Architecture/ADR/ADR-001-soft-delete.md` |
| EF Core owned types | Understand how `OwnsOne` maps value objects to parent table columns | `DocAPI/Core/Entities/Paciente.cs` (Endereco owned type pattern) |
| AutoMapper profile patterns | Understand the write/read mapping split used across all verticals | `DocAPI/Application/Mappings/Profiles/AgendamentoProfile.cs` |

---

## Concepts

### 1. Single-Aggregate SQL Vertical Migration

Doc Organo's SQL migration strategy moves one aggregate at a time from stub (in-memory placeholder) to full SQL Server persistence. Each vertical is a self-contained feature with its own SDD, tests, and verification.

**Why this approach exists:** Migrating all aggregates simultaneously is high-risk. A vertical-by-vertical approach isolates risk, allows each aggregate's contract to stabilize before downstream consumers depend on it, and produces independently verifiable increments. The sequence is dependency-ordered: each vertical can only begin after its FK dependencies are SQL-stable.

**Agendamento's position:** Fourth in the approved sequence. Paciente, Atendimento Minimal, and Prontuario are verified upstream. Agendamento was the last blocking prerequisite before Atendimento Workflow (the clinical journey orchestration) can proceed.

**How it's implemented:** The repository stub (`AgendamentoRepository` throwing `NotImplementedException`) is replaced with a `DocDbContext`-backed implementation. The interface contract is updated to use `Guid` identity (matching the established pattern). The controller is hardened — routes, PHI safety, status codes, and FK validation. No new migrations are generated.

**Study recommendation:** Read `Documentation/Technical/migration-sql.md` for the full migration strategy and sequenced dependency order.

---

### 2. Entity-Schema Alignment Without Migration Delta

The `InitialCreate` migration defines the database schema as source of truth. When the schema contains columns that the entity model doesn't surface (like `PacienteID` on the `Agendamento` table), the entity model is aligned to the schema — not the other way around.

**The problem this solves:** The `Agendamento` table has a `PacienteID` column (nullable, with an FK constraint to the `Paciente` table). The entity model had no corresponding property, so the FK value existed in the database but was invisible to the application. The entity model also had a `Procedimento` string concept with no corresponding database column.

**The solution:** Add `PacienteID` (Guid?, nullable) and `Paciente` navigation to the entity. Remove any `Procedimento`-related entity state. Procedure information is derived from `Internacao → ProcedimentoInternacao` — owned by the Prontuario aggregate.

**The critical implementation detail:** The property is named `PacienteID` (matching the database column `PacienteID` exactly — PascalCase C# convention maps to PascalCase column name by EF convention). The FK relationship in `AgendamentoConfiguration` references this column:

```csharp
// The relationship maps to the EXISTING FK constraint — no new migration
builder.HasOne(x => x.Paciente)
    .WithMany()
    .HasForeignKey(x => x.PacienteID);
```

If the property name didn't match the column name, EF would generate a migration to add a new column. The naming convention is the mechanism that prevents migration delta.

**Why not generate a migration?** `InitialCreate` is the authoritative schema. Adding migrations for entity changes that simply expose existing columns would fragment the migration history and create unnecessary deployment risk. The Agendamento vertical proves that entity-schema alignment can be achieved entirely through naming convention and FK mapping configuration.

**When would a migration be needed?** If a new column, constraint, or index must be added that doesn't already exist in the schema. That requires ADR evaluation — schema changes are durable, irreversible decisions.

---

### 3. Three-FK Validation on Create

Creating an Agendamento requires three foreign keys to be valid: `AtendimentoId` (required), `InternacaoId` (required), and `PacienteId` (optional). Each FK is validated before aggregate construction, using the appropriate data source for each.

**Why FK validation belongs in the application layer, not the repository:** The repository is a persistence-only boundary — it stores and retrieves aggregate state. FK validation is a cross-aggregate concern: it queries three different sources (`IAtendimentoRepository`, `_context.Set<Internacao>()`, `IPacienteRepository`) and produces HTTP-specific error responses. This composition of dependencies and HTTP awareness is an application-layer responsibility.

**The three-source pattern:**

| FK Target | Source | Why this source |
|-----------|--------|-----------------|
| `AtendimentoId` | `IAtendimentoRepository` | Atendimento is a standalone aggregate with its own repository |
| `InternacaoId` | `_context.Set<Internacao>()` | Internacao is a child entity of Prontuario — no standalone repository exists |
| `PacienteId` (optional) | `IPacienteRepository` | Paciente is a standalone aggregate with its own repository |

The Internacao case is important: accessing it through `_context.Set<Internacao>()` (the `DbSet` for the entity type) avoids creating an `InternacaoRepository` that would only exist for FK validation. This pattern — direct `DbSet` access for child entities without repositories — is established by Atendimento Minimal and reused here.

**Fail-fast semantics:** Validation checks each FK sequentially and returns on the first failure. The error body uses a structured JSON format:

```json
{
  "title": "Foreign key reference not found",
  "status": 404,
  "field": "atendimentoId",
  "detail": "The referenced resource was not found or has been removed."
}
```

The `field` identifies which FK failed by name only. The submitted value is never echoed — this prevents internal identifier leakage in error responses. The 404 status code is correct because "the referenced resource was not found" is the semantic root cause, not "the client sent bad data" (which would be 400).

**Why this pattern is composable:** When a new aggregate requires FK validation across multiple sources, the same layered approach applies: one validation per FK source, ordered by dependency, with a structured error body identifying the failing field. Future verticals (like Internacao if it gains its own repository) can add validation blocks without restructuring existing code.

---

### 4. Domain Operations as Sole Mutation Paths

The `Agendamento` aggregate controls all state changes through four dedicated operations. No external code — not the controller, not AutoMapper, not the repository — can directly set properties that affect aggregate invariants.

**The four operations:**

| Operation | What it does | What it prevents |
|-----------|-------------|------------------|
| Constructor (factory) | Generates `Guid` ID, sets `Status = SemSenha`, records `CriadoEm` | Client setting initial status arbitrarily; bypassing identity generation |
| `Update(...)` | Mutates scalar fields; sets `AtualizadoEm` | Changing FK targets or ID after creation; silent updates without timestamp |
| `SoftDelete()` | Sets `Deletado = true`, `DeletadoEm` | Physical deletion; bypassing audit trail |
| `SetSenha(senha?)` | Replaces or clears the owned `SenhaAgendamento`; sets `AtualizadoEm` | Direct property access to owned type; stale timestamps |

**Why this matters:** Aggregates are consistency boundaries. If external code can directly set properties, invariants become unenforceable — a client could create an Agendamento with `Status = AgendamentoEfetuado`, or change the `InternacaoId` after creation, or clear a senha without updating the timestamp. Domain methods are the gatekeepers.

**The AutoMapper split:** Write-direction mappings (`CreateAgendamentoDto` → `Agendamento`) are removed from the AutoMapper profile. The controller constructs the aggregate manually via the constructor. Read-direction mappings (`Agendamento` → `ReadAgendamentoDto`) may remain — reading doesn't mutate state, so automated mapping is safe.

This pattern is established across Paciente, Atendimento Minimal, and Prontuario. Agendamento extends it with the `SetSenha()` method — the first dedicated domain method for owned value object mutation in the codebase.

---

### 5. Owned Value Object — `SenhaAgendamento`

`SenhaAgendamento` is an owned value object — it has no independent identity and is mapped inline on the `Agendamento` table as a set of prefixed columns.

**The concept:** A value object is defined by its values, not by an identity. `SenhaAgendamento` has no `ID` — it is semantically part of an Agendamento, not a separate entity. EF Core's `OwnsOne` mapping stores its properties as columns on the parent table:

```
Agendamento table columns:
  ...
  SenhaAgendamento_Codigo (varchar(20))
  SenhaAgendamento_DataPedido (date)
  SenhaAgendamento_DataLiberacao (date)
  SenhaAgendamento_Validade (date)
  ...
```

**The mutation concern:** Before Agendamento Stabilization, `SenhaAgendamento` properties were publicly settable — external code could mutate individual properties without the aggregate's knowledge. The `SetSenha(SenhaAgendamento?)` method encapsulates mutation:
- A non-null argument replaces the entire owned value
- A null argument clears the authorization code
- `AtualizadoEm` is always set on mutation

This is the same pattern as `Endereco` on the `Paciente` aggregate — the first owned type to use a dedicated mutation method established the precedent.

**Why `SetSenha` is separate from `Update`:** The `Update` method handles scalar fields. `SenhaAgendamento` is a value object with its own internal structure. Separating the mutation paths keeps `Update` focused on simple fields and gives `SetSenha` the freedom to handle null-to-clear semantics and any future validation (e.g., date ordering checks).

---

### 6. Soft Delete per ADR-001

ADR-001 mandates soft delete for all aggregates: deleted rows are marked with `Deletado = true` and `DeletadoEm` timestamp, and excluded from all default queries.

**How it's implemented in Agendamento:**

1. **Domain method:** `SoftDelete()` sets `Deletado = true` and `DeletadoEm = DateTime.UtcNow`
2. **Repository:** `DeleteAsync` loads the aggregate, calls `SoftDelete()`, and saves — no physical row removal
3. **Global query filter:** `DocDbContext.HasQueryFilter(a => !a.Deletado)` — automatically applied to all LINQ queries
4. **Controller:** `DELETE /Agendamento/{id}` returns 204 on success (the row is still in the database but invisible to reads)

**Verification behavior:** A deleted Agendamento:
- Returns 404 from `GetByIdAsync` (global filter excludes it)
- Does not appear in `GetAllAsync`, `GetByNameAsync`, or `GetByPacienteIdAsync`
- Can be accessed for audit by explicitly calling `.IgnoreQueryFilters()` — but no controller endpoint currently exposes this

**Why soft delete matters in a clinical system:** Clinical data should never be physically destroyed — audit trails, medical history, and regulatory compliance require that records be retained. Soft delete makes records invisible to normal operations while preserving them for authorized audit access.

---

### 7. Repository Responsibility Boundary

The `AgendamentoRepository` is responsible for persistence only — storing and retrieving aggregate state. It does not:
- Validate foreign keys (application layer responsibility)
- Enforce business rules (aggregate responsibility)
- Handle PHI concerns (controller/architecture responsibility)
- Map DTOs (controller/AutoMapper responsibility)

**The seven methods:**

| Method | Type | Returns |
|--------|------|---------|
| `GetAllAsync(skip, take)` | Read | `IEnumerable<Agendamento>` — paginated, ordered by Data desc, Horario asc |
| `GetByIdAsync(Guid id)` | Read | `Agendamento?` — single aggregate or null |
| `GetByNameAsync(string name)` | Read | `List<Agendamento>` — substring match on Nome |
| `GetByPacienteIdAsync(Guid pacienteId)` | Read | `List<Agendamento>` — filtered by nullable FK |
| `CreateAsync(Agendamento)` | Write | void — persists and saves |
| `UpdateAsync(Agendamento, Guid id)` | Write | void — attaches and saves changed state |
| `DeleteAsync(Guid id)` | Write | void — loads, invokes SoftDelete, saves |

Clamping in `GetAllAsync` uses `Math.Clamp(take, 1, 100)` — this is a data safety guard, not a business rule. Without it, a client requesting `take=1000000` could cause performance degradation. The cap of 100 is a reasonable operational limit for a paginated list endpoint.

---

### 8. Test Suite Architecture

The 30-test suite is organized in three layers, each targeting a different architectural concern:

**Entity tests (Business Invariants):** Test the aggregate's domain operations directly — constructor sets `Status = SemSenha` and `CriadoEm`, `SoftDelete` sets flags, `Update` changes scalar fields and timestamps, `SetSenha` mutates the owned value. No database, no HTTP — pure domain logic.

**Repository tests (Persistence Intent):** Test repository behavior using EF Core's in-memory database. Each test creates a fresh `DocDbContext` with a unique database name (via `Guid.NewGuid().ToString()`), so tests are isolated. These tests verify that repository methods produce the correct persistence outcomes — create-then-read returns the entity, update persists changes, soft delete excludes from reads.

The in-memory database is not SQL Server — it doesn't validate FK constraints or run real SQL. That's intentional: these tests verify persistence intent (what should be persisted), not physical persistence (how it's executed against SQL Server).

**Controller tests (API Contract):** Test HTTP behavior with mocked dependencies. These verify route constraints, status codes, FK validation responses, and DTO mapping. No database, no real HTTP server — just contract verification.

**SQL integration tests (Physical Persistence):** Exercise full CRUD round-trip against Docker SQL Server. These are gated with `SkippableFact` — they skip gracefully when Docker is not available. They create the prerequisite chain (Paciente → Atendimento → Prontuario → Internacao → Agendamento) to validate FK constraints at the database level. The test source exists and has been reviewed, but execution was skipped during Verify due to environment unavailability.

This four-layer approach means that 30 tests pass on every `dotnet test` run without Docker, while the SQL integration tests stand ready to validate physical persistence when the environment is available.

---

## Architecture Summary

```
┌─────────────────────────────────────────────────┐
│                   Controller                     │
│  FK validation → DTO mapping → status codes     │
│  Three FK sources: repo, DbSet, repo            │
│  PHI-safe: zero diagnostic output               │
└────────────────────┬────────────────────────────┘
                     │ IAgendamentoRepository (Guid)
┌────────────────────▼────────────────────────────┐
│               Repository                         │
│  Persistence only — SQL CRUD against DocDbContext│
│  No FK validation, no business rules            │
│  Pagination clamp, ordering, soft delete invoke │
└────────────────────┬────────────────────────────┘
                     │ DocDbContext
┌────────────────────▼────────────────────────────┐
│              Aggregate                           │
│  Agendamento + SenhaAgendamento (owned)          │
│  Factory: Status=SemSenha, CriadoEm=UtcNow       │
│  Update(), SoftDelete(), SetSenha()              │
│  FK targets immutable after create              │
└────────────────────┬────────────────────────────┘
                     │ AgendamentoConfiguration
┌────────────────────▼────────────────────────────┐
│                SQL Schema                         │
│  InitialCreate — unchanged                       │
│  FK constraints, indexes, owned-type columns     │
│  Global query filter: !Deletado                 │
└─────────────────────────────────────────────────┘
```

**Layer boundaries:**
- **Controller** owns FK validation, DTO ↔ aggregate mapping, HTTP status codes, PHI safety
- **Repository** owns persistence mechanics only
- **Aggregate** owns domain state, construction invariants, and all mutation paths
- **EF Configuration** owns entity-to-schema mapping — zero migration delta

**What Agendamento consumes from upstream:**
- `IAtendimentoRepository` → FK validation
- `IPacienteRepository` → FK validation
- `_context.Set<Internacao>()` → FK validation (child of Prontuario aggregate)
- `DocDbContext` → persistence context (shared infrastructure)

**What consumes Agendamento:**
- Atendimento Workflow Stabilization (next SDD) — clinical journey orchestration that reads Agendamento data
- WS07 Blazor frontend alignment (deferred) — UI contract migration for Guid, enums, and Procedimento removal

---

## References

| Artifact | Role |
|----------|------|
| `DocAPI/Core/Entities/Agendamento.cs` | Aggregate root, owned value object, domain enums |
| `DocAPI/Core/Interfaces/Repositories/IAgendamentoRepository.cs` | Persistence contract (Guid identity) |
| `DocAPI/Infrastructure/Repositories/AgendamentoRepository.cs` | SQL persistence implementation |
| `DocAPI/API/Controllers/AgendamentoController.cs` | HTTP endpoints, FK validation, PHI safety |
| `DocAPI/Infrastructure/SqlDb/Configurations/AgendamentoConfig.cs` | Entity-to-schema mapping |
| `Documentation/Architecture/ADR/ADR-001-soft-delete.md` | Soft delete authority |
| `Documentation/Technical/migration-sql.md` | SQL migration strategy and sequence |
| `Documentation/Architecture/Domain_Overview_Business_Rules.md` | Domain language and business rules |
| `Documentation/SDD/Agendamento_Stabilization/specify.md` | Feature specification |
| `Documentation/SDD/Agendamento_Stabilization/design.md` | Design outcomes and contracts |
| `Documentation/SDD/Agendamento_Stabilization/verification.md` | Verification evidence and decisions |
| `Documentation/State.md` | Current operational truth |

---

## Completion Decision

| Item | Value |
|------|-------|
| Decision | **Complete** |
| Deferred topics | None |
| Notes | All reusable concepts extracted. Guide covers the architectural patterns, domain design, and implementation rationale without duplicating Verification, Reporting, or SDD content. Validation checks pass — no implementation chronology, no line-by-line code explanation, no duplicated project documentation. |

---

> **Teacher Guide:** Complete  
> **Authority:** Implementation truth. Production code is the final authority.  
> **Next artifact:** None — this is the final artifact of the Agendamento Stabilization feature lifecycle.