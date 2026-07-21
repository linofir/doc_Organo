# Design — Agendamento SQL Migration & Stabilization

> Feature SDD: `Documentation/SDD/Agendamento_Stabilization/`
> Input: `specify.md` (2026-07-17)
> Generated SDD artifacts are written in English.
> **Template source:** `Documentation/AI-Harness/template/sdd/design.md`

## Design Summary

Agendamento Stabilization is the fourth vertical migration in the approved clinical sequence (Paciente → Atendimento Minimal → Prontuario → Agendamento). The aggregate transitions from an in-memory stub to full SQL persistence under an unchanged `InitialCreate` schema — no migration delta is generated.

Three architectural outcomes define this feature:

1. **Entity-schema alignment.** The existing `PacienteID` column is surfaced on the entity model as a nullable FK with navigation. The `Procedimento` field is removed from all DTOs and not added to the entity — procedure data is derived from the Prontuario aggregate via its Internacao child. All enums remain domain-enforced with string storage in the database.

2. **Consistent aggregate contract.** The repository interface, controller routes, and DTOs adopt `Guid` identity — matching the Paciente, Atendimento, and Prontuario contracts. The repository is responsible for persistence only. FK validation on create belongs to the application layer. Soft delete follows ADR-001 via a domain method on the aggregate.

3. **PHI-safe API surface.** All diagnostic logging is removed from the controller. No structured logging replaces it. Error responses identify the failing FK field without echoing internal identifiers.

Frontend alignment (WS07) and Atendimento Workflow validations (separate SDD) are excluded. The aggregate is a backend stabilization vertical.

**Outcome-oriented design:** This document defines what must be true after implementation — behaviors, contracts, persistence outcomes, and constraints. It does not prescribe specific .NET types, framework APIs, or mapping strategies unless an accepted ADR requires them. Execute owns local implementation choices that satisfy these outcomes.

## Requirement Mapping

| Requirement | Design response |
|-------------|-----------------|
| `REQ-001` | The `Agendamento` entity gains a nullable `PacienteId` FK and `Paciente` navigation property. The `PacienteId` property maps to the existing migration column `PacienteID`. No `Procedimento` property exists on the entity. The `Status`, `InstrucaoStatus`, and `AtestadoStatus` enums remain unchanged — stored as strings in the database with domain-level type safety. The `SenhaAgendamento` owned value object is preserved without structural changes. |
| `REQ-002` | Every ID parameter on `IAgendamentoRepository` uses `Guid` — `GetByIdAsync`, `GetByPacienteIdAsync`, `UpdateAsync`, `DeleteAsync`. The interface compiles with the same namespace and assembly references as the current contract. |
| `REQ-003` | The repository implements all contract methods against a persistence context (the application's `DbContext`). Behavior: `CreateAsync` persists a new row and saves. `GetByIdAsync` returns a single aggregate by `Guid` ID — soft-deleted rows are excluded by default. `GetByNameAsync` supports case-insensitive substring search on the `Nome` field. `GetByPacienteIdAsync` filters where `PacienteId` matches the provided `Guid`. `GetAllAsync` returns paginated results ordered by `Data` descending, `Horario` ascending, with default page size 10 and a hard ceiling of 100 results per page. `UpdateAsync` persists changed aggregate state. `DeleteAsync` invokes the aggregate's soft-delete method before saving. The repository is responsible for persistence only — it performs no FK validation, no business rule enforcement, and no PHI handling. |
| `REQ-004` | The persistence configuration maps the `Paciente` relationship with a FK referencing the existing `PacienteID` database column. The owned-type `SenhaAgendamento`, enum-to-string conversions, existing `Internacao` and `Atendimento` FK relationships, and all existing indexes are preserved. No new indexes are introduced — FK constraints already provide implicit indexes for foreign keys. The entity-schema mapping must produce zero migration delta when verified against `InitialCreate`. |
| `REQ-005` | All controller route parameters accepting an ID use `Guid` with route constraints that reject non-Guid values at the routing layer. Every diagnostic-logging call is removed — the controller produces no console output or structured log entries in any request path. HTTP status codes: 200 for successful lists and queries, 201 for creation, 204 for successful mutations, 400 for invalid input, 404 for missing resources, and 500 only for unexpected failures in catch-all paths (not for validation logic). The `Location` header on creation uses the `Guid` ID. |
| `REQ-006` | FK validation for create requests belongs to the application layer — not the repository and not the aggregate. Three FK targets are validated before aggregate construction: `AtendimentoId` (required) must reference an existing, non-deleted Atendimento. `InternacaoId` (required) must reference an existing, non-deleted Internacao. `PacienteId` (optional, when non-null) must reference an existing, non-deleted Paciente. Validation uses each aggregate's own persistence contract — Atendimento via `IAtendimentoRepository`, Paciente via `IPacienteRepository`, and Internacao via direct access to the Internacao table (Internacao is a child entity without its own repository). A failed FK check produces HTTP 404 with a structured error body identifying which FK field failed, without echoing the submitted value (see API Contract section). Validation fails on the first invalid FK encountered. |
| `REQ-007` | `GET /Agendamento` with pagination query parameters returns 200 with a `ReadAgendamentoDto` array — empty when no rows match. `GET /Agendamento/{id}` returns 200 with a single DTO for an existing non-deleted aggregate, 404 otherwise. `GET /Agendamento/by-name` returns 200 with a DTO array — empty when no matches (never 404). `GET /Agendamento/by-pacientId` returns 200 with a DTO array — empty when no matches. Pagination defaults to skip 0, take 10, capped at 100 results per page. Results are ordered by `Data` descending then `Horario` ascending. |
| `REQ-008` | Update applies to mutable scalar fields only: `Nome`, `Aviso`, `Data`, `Horario`, `Local`, `Sala`, `Status`, `InstrucaoStatus`, `AtestadoStatus`, `DataConsulta`, `SenhaAgendamento`. FK targets and `ID` are immutable after creation — the update operation cannot alter them regardless of request content. The `UpdateAgendamentoDto` excludes FK and identity fields at the contract level. Update sets the aggregate's last-modified timestamp. The `SenhaAgendamento` owned value is mutated through a dedicated domain operation (see REQ-011). HTTP response: 204 on success, 404 when the aggregate is not found or is soft-deleted. |
| `REQ-009` | Soft delete follows ADR-001: a domain method on the aggregate sets the deletion flag and timestamp. The persistence layer's global query filter — already active for this aggregate — automatically excludes soft-deleted rows from all standard query paths (`GetByIdAsync`, `GetAllAsync`, `GetByNameAsync`, `GetByPacienteIdAsync`). HTTP response: 204 on success, 404 when the aggregate is not found or is already deleted. |
| `REQ-010` | Three API DTO contracts are redesigned for Guid identity, enum type safety, and FK clarity:<br>**CreateAgendamentoDto** — requires `AtendimentoId` (Guid) and `InternacaoId` (Guid). Accepts `PacienteId` (optional, Guid). Includes all scalar fields except `Procedure` and `Status` — both are excluded (procedure data lives in the Prontuario aggregate; status is set by the aggregate factory). `StatusInstrucoes` and `StatusAtestado` use domain enum types, not strings.<br>**ReadAgendamentoDto** — `ID`, `AtendimentoId`, and `InternacaoId` are `Guid`. `PacienteId` is nullable `Guid`. All status fields use domain enum types. Includes `CriadoEm` and `AtualizadoEm` timestamps. No `Procedimento` field.<br>**UpdateAgendamentoDto** — contains only mutable scalar fields listed in REQ-008. No FK fields, no `ID`, no `Procedimento`. Enum-typed fields use domain enum types.<br>DTO construction for writes uses manual mapping through aggregate methods — automated object mapping must not bypass domain behavior. Read mappings from entity to DTO may use automated mapping. |
| `REQ-011` | The aggregate exposes four domain operations:<br>**Factory (construction)** — accepts required FKs (`InternacaoId`, `AtendimentoId`, `Data`, `Horario`) plus optional scalar fields and optional `PacienteId`. Generates a new `Guid` identity. Initializes `Status` to `SemSenha`. Sets the creation timestamp. This is the only path to produce an Agendamento for persistence.<br>**SoftDelete** — transitions the aggregate to the deleted state with a deletion timestamp. No other state changes. No behavioral side effects.<br>**Update** — accepts mutable scalar fields (listed in REQ-008) and sets the last-modified timestamp. FK targets and identity are not writable through this operation.<br>**SetSenha** — mutates the owned `SenhaAgendamento` value. A null argument clears the authorization code; a non-null argument replaces it. Sets the last-modified timestamp. |
| `REQ-012` | Tests are organized in two layers:<br>**Unit tests** cover aggregate invariants (factory defaults, soft-delete state, update timestamp behavior, owned-value mutation), repository behavior in isolation, and application-layer FK validation logic with mocked dependencies.<br>**SQL integration tests** exercise a full CRUD round-trip against the Docker SQL Server: create → read by ID → read by name → read by PacienteId → update → read updated → soft delete → verify exclusion from reads. The integration fixture constructs a prerequisite chain: Paciente → Atendimento → Prontuario (with Internacao as a child) → Agendamento. All fixture data is synthetic. `dotnet test` must pass for all Agendamento test suites. |
| `REQ-013` | The legacy behavior table in `specify.md` § Legacy Behavior is binding — all 17 sheet columns are accounted for with preserve/adapt/abandon decisions. This design does not modify those decisions. Execute must follow them. |

## Affected Components

| Component | Path | Responsibility | Change |
|-----------|------|----------------|--------|
| `Agendamento` aggregate | `DocAPI/Core/Entities/Agendamento.cs` | Domain state, construction, SoftDelete, Update, SetSenha | Update — add `PacienteId`, `Paciente` nav, domain operations |
| `SenhaAgendamento` owned value | `DocAPI/Core/Entities/Agendamento.cs` | Authorization code value object | Verify — structural preservation; mutation path may need widening |
| `IAgendamentoRepository` | `DocAPI/Core/Interfaces/Repositories/IAgendamentoRepository.cs` | Persistence contract | Update — all ID parameters become `Guid` |
| `AgendamentoRepository` | `DocAPI/Infrastructure/Repositories/AgendamentoRepository.cs` | SQL persistence implementation | Update — implement full CRUD against persistence context |
| `AgendamentoConfiguration` | `DocAPI/Infrastructure/SqlDb/Configurations/AgendamentoConfig.cs` | Entity-to-schema mapping | Update — add Paciente relationship mapping referencing `PacienteID` column |
| Persistence context | `DocAPI/Infrastructure/SqlDb/DbContext/DbContext.cs` | Database session and global filters | Verify — query filter and configuration already registered |
| `AgendamentoController` | `DocAPI/API/Controllers/AgendamentoController.cs` | HTTP endpoints | Update — Guid routes, diagnostic-logging removal, FK validation, status codes |
| `CreateAgendamentoDto` | `DocAPI/Application/Data/Dtos/Agendamento/CreateAgendamentoDto.cs` | Create request contract | Update — remove `Procedimento`/`Status`, `PacienteId` becomes `Guid?`, enums replace strings, add FK fields |
| `ReadAgendamentoDto` | `DocAPI/Application/Data/Dtos/Agendamento/ReadAgendamentoDtos.cs` | Read response contract | Update — ID becomes `Guid`, `PacienteID` becomes `Guid?`, remove `Procedimento`, enums replace strings, add FK + timestamp fields |
| `UpdateAgendamentoDto` | `DocAPI/Application/Data/Dtos/Agendamento/UpdateAgendamento.cs` | Update request contract | Update — remove `ID`/FK fields/`Procedimento`, enums replace strings, `Status` becomes enum |
| AutoMapper profile | `DocAPI/Application/Mappings/Profiles/AgendamentoProfile.cs` | Read-direction mapping | Update — write paths replaced by manual mapping; read paths may remain |
| Unit tests | `DocAPI.Tests/` | Aggregate, controller, repository isolation | Add |
| SQL integration tests | `DocAPI.Tests/Integration/` | Full CRUD round-trip against Docker SQL | Add |

## Reuse Analysis

| Existing code or pattern | Reuse decision | Notes |
|--------------------------|----------------|-------|
| FK validation pattern (Atendimento) | **Reuse** | Application layer validates FK targets via their own persistence contracts; returns 404 on missing or soft-deleted parent. Established by Atendimento Minimal. |
| Soft-delete pattern (Prontuario) | **Reuse** | Aggregate method sets deletion markers; persistence layer saves; global query filter excludes deleted rows on reads. Identical across all three prior verticals. |
| Owned-value pattern (Paciente — Endereco) | **Reuse** | `SenhaAgendamento` is already mapped as an owned type. The Paciente owned-type precedent confirms this is the correct persistence pattern. |
| Guid routing and status codes (Atendimento) | **Reuse** | Route constraints, `CreatedAtAction`, `NoContent` semantics. Same pattern across all Guid-contract controllers. |
| FK validation error format (Prontuario) | **Adapt** | Prontuario uses 400 for FK failures. Agendamento uses 404 with a structured body identifying the failing FK field — more specific and correct for "referenced resource not found" semantics. |
| Aggregate query structure (Prontuario) | **Adapt** | Prontuario uses complex nested includes for versioning. Agendamento's query surface is simpler — no versioning, no deep navigation required for standard reads beyond the aggregate root. |
| FK validation for child entities (Atendimento) | **Reuse** | Direct table access for FK validation of an entity that has no standalone repository — Atendimento established this pattern. Agendamento applies it for Internacao validation. |
| Object mapping (all prior verticals) | **Adapt** | Write-direction mappings (DTO → aggregate) use manual construction through domain operations. Read-direction mappings (aggregate → DTO) may use automated mapping. This pattern is consistent across Prontuario and Atendimento. |
| Legacy Sheets mapping | **Abandon** | Behavioral reference only — the 17-column mapping informs field design but has no implementation role in the SQL vertical. |

## Architecture And Ownership

- **Architecture impact:** Low. The feature follows the established single-aggregate SQL vertical pattern. No new layers, no new abstractions, no migration delta, and no bounded-context changes. The repository stub is replaced with a persistence-context-backed implementation under the same DI registration and interface contract (with Guid identity alignment).

- **Ownership boundaries:**
  - **Aggregate:** Owns its internal state and all domain operations — construction (factory), mutation (Update, SetSenha), and lifecycle transitions (SoftDelete). The aggregate is the sole authority on which fields are mutable and what invariants hold after creation.
  - **Repository:** Owns persistence mechanics only — storing and retrieving aggregate state. Does not perform FK validation, enforce business rules, or handle PHI concerns.
  - **Application layer (controller):** Owns FK validation on create, DTO-to-aggregate mapping for writes, HTTP status code selection, and PHI-safe error responses. Reads and routes aggregates via the repository contract.
  - **EF configuration:** Owns the entity-to-schema mapping — column names, FK relationships, owned-type structure, enum storage format, and indexes. Responsible for zero migration delta against `InitialCreate`.
  - **Internacao:** Child entity of the Prontuario aggregate. The Agendamento feature reads Internacao for FK validation only — it does not create, modify, or delete Internacao instances.
  - **Paciente and Atendimento:** Independent aggregates. Read-only FK sources — not modified by Agendamento operations.

- **Constraints:**
  - `InternacaoId` is required (NOT NULL in schema). An Agendamento cannot be created without a valid Internacao. If outpatient scheduling without an Internacao becomes a business requirement, the constraint must be relaxed through an ADR before implementation (residual risk from Research DC-3).
  - `PacienteId` is nullable — an Agendamento may exist without a direct patient link. The patient can be inferred through the Atendimento chain when absent.
  - No uniqueness constraints — multiple agendamentos per patient, per atendimento, and per internacao are permitted.

## Data And Persistence

- **Aggregate state changes:**
  - `Agendamento`: gains `PacienteId` (nullable Guid, mapping to migration column `PacienteID`) and `Paciente` navigation property. No `Procedimento` property. The constructor accepts `PacienteId` as an optional parameter. All enum-typed properties (`Status`, `InstrucaoStatus`, `AtestadoStatus`) remain domain enums stored as strings.
  - `SenhaAgendamento`: unchanged owned value object. Its properties must support mutation through the aggregate's `SetSenha` operation — this may require widening internal accessibility without changing the public contract.
  - `Internacao`, `Paciente`, `Atendimento`: no changes. Read-only references for FK validation.

- **DTO contracts:** Three DTOs redesigned per REQ-010. Write mappings use manual construction through aggregate operations — automated object mapping must not bypass domain behavior. Read mappings may use automated mapping. All three DTOs should reside in a single, consistent namespace (current code has mixed namespaces).

- **Entity-to-schema mapping:**
  - The `Paciente` relationship is mapped to the existing `PacienteID` database column with the existing FK constraint `FK_Agendamento_Paciente_PacienteID`.
  - All existing mappings are preserved: `Internacao` FK, `Atendimento` FK, owned-type `SenhaAgendamento` with its four columns, enum-to-string storage for `Status`/`InstrucaoStatus`/`AtestadoStatus`, and three existing indexes (composite `Data`+`Horario`, `Status`, `InternacaoId`).
  - No new indexes are introduced. FK constraints already provide implicit indexes for `AtendimentoId` and `PacienteID`.
  - The global query filter excluding soft-deleted rows is already active for this aggregate — no change needed. Access to deleted rows for audit or reporting is reserved for explicit opt-out paths only, and only with documented rationale.

- **Migration impact:** NONE. The `InitialCreate` migration is authoritative. All required columns exist in the current schema. When the entity model is aligned, a schema diff must produce zero pending changes — any detected delta indicates a mapping mismatch that must be resolved without generating a new migration.

- **Schema alignment expectations:**
  - `PacienteID` column: nullable in schema → nullable Guid on entity.
  - `InternacaoId` column: NOT NULL in schema → non-nullable Guid on entity.
  - `AtendimentoId` column: NOT NULL in schema → non-nullable Guid on entity.
  - Soft-delete filter: active globally → repository reads exclude deleted rows by default.
  - Internacao table: no global soft-delete filter → FK validation must explicitly exclude soft-deleted Internacao records.

## API Contract

**Contract resolution:** DQ-006 FK validation error format is resolved below. DQ-001 through DQ-005 were resolved in `specify.md`. DQ-003 (broad scalar update), DQ-004 (SenhaAgendamento on create), and DQ-005 (client-settable Status) are accepted as specified.

| Method | Route | Request | Response | Compatibility notes |
|--------|-------|---------|----------|---------------------|
| GET | `/Agendamento?skip=0&take=10` | Query: `skip` (int, default 0), `take` (int, default 10, max 100) | `200 OK`: `ReadAgendamentoDto[]` — empty array when no rows. Ordered by `Data` descending, `Horario` ascending. | Pagination contract maintained. |
| GET | `/Agendamento/{id:guid}` | Route: `id` (Guid) | `200 OK`: `ReadAgendamentoDto`. `404 Not Found`: ID not found or soft-deleted. | `string` → `Guid` breaks frontend (WS07). |
| GET | `/Agendamento/by-name?nome={nome}` | Query: `nome` (string, required) | `200 OK`: `ReadAgendamentoDto[]` — empty array when no matches. Case-insensitive substring search. | Response changed: previously returned 404 for no matches, now returns 200 with empty array. |
| GET | `/Agendamento/by-pacientId?pacienteId={guid}` | Query: `pacienteId` (Guid, required) | `200 OK`: `ReadAgendamentoDto[]` — empty array when no matches. | `string` → `Guid` breaks frontend (WS07). |
| POST | `/Agendamento` | Body: `CreateAgendamentoDto`. Required: `AtendimentoId` (Guid), `InternacaoId` (Guid). Optional: `PacienteId` (Guid?). Scalar fields. No `Status` (set by aggregate). No `Procedimento`. | `201 Created`: `ReadAgendamentoDto` with `Location` header. `400 Bad Request`: invalid body. `404 Not Found`: FK target missing or soft-deleted — see FK error format below. | FK fields are new to the DTO. No `Procedimento` — frontend must adapt (WS07). |
| PUT | `/Agendamento/{id:guid}` | Route: `id` (Guid). Body: `UpdateAgendamentoDto` — mutable scalar fields only. No FK fields, no `ID`. | `204 No Content`: update applied. `404 Not Found`: ID not found or soft-deleted. `400 Bad Request`: invalid body. | FK immutability enforced at contract level — DTO excludes FK fields. |
| DELETE | `/Agendamento/{id:guid}` | Route: `id` (Guid). | `204 No Content`: soft delete applied. `404 Not Found`: ID not found or already deleted. | Physical delete replaced by soft delete (ADR-001). |

**FK validation error response format (DQ-006 resolution):**
The POST endpoint returns 404 with a structured body:
```json
{
  "title": "Foreign key reference not found",
  "status": 404,
  "field": "atendimentoId",
  "detail": "The referenced resource was not found or has been removed."
}
```
- `field` identifies the failing FK: `atendimentoId`, `internacaoId`, or `pacienteId`.
- The submitted ID value is **not** echoed in the error body — only the field name is exposed.
- If multiple FK targets are invalid, only the first detected failure is reported (fail-fast).

**Unresolved contract items for SDD Pre-Execution Review:** None.

## Frontend Impact

Backend Stabilization rule — excluded from Execute scope. Deferred to WS07.

**Known frontend drifts (WS07 follow-up):**

| Drift | Impact | WS07 action |
|-------|--------|-------------|
| `string` → `Guid` IDs | All `AgendamentoService` and `AgendamentoState` methods use `string` IDs — will break on Guid endpoints | Migrate all ID parameters and state to `Guid` |
| `Procedimento` field | `AgendamentoViewModel.Procedimento` displays a string no longer in API responses | Remove field; derive procedure display from Internacao navigation |
| `StatusAgendamento` enum values 3–7 | Backend authoritative values are 0–6; frontend enum is offset by +1 for values 3–7 | Align frontend enum to backend authoritative values |
| `GET /Agendamento/by-pacientId` param type | Query parameter changes from `string` to `Guid` | Update `AgendamentoService.GetByPacienteIdAsync` signature |
| `StatusInstrucoes`/`StatusAtestado` types | Frontend may expect strings; backend now uses domain enum types | Verify enum deserialization in Blazor |
| `InternacaoId`/`AtendimentoId` in Read DTO | Frontend may not have these fields in `AgendamentoViewModel` | Add fields if needed for FK context display |

## Security And PHI Design

- **Security-sensitive: Yes**
- **Review prompt expected: Yes** — `Documentation/AI-Harness/review-prompts/security-phi-review.md`
- **Mitigations:**
  - All diagnostic-logging calls (`Console.WriteLine`) are removed from the controller — the controller produces zero console output in any request path. No structured logging replaces the removed calls.
  - Test fixtures use synthetic data exclusively — no real patient names, CPF, or clinical data.
  - The `Nome` field is user-provided (not derived from Paciente) and is treated as potentially identifying. It is never written to logs.
  - `PacienteId` is a GUID — not PHI on its own but linked to patient data. It is never written to logs.
  - FK error responses identify the failing field by name only — the submitted ID value is not echoed.
  - No credentials, connection strings, or secrets appear in test code or documentation.

## Legacy Behavior Decision

All decisions are documented in `specify.md` § Legacy Behavior. The following table summarizes binding decisions for Execute.

| Behavior | Source | Disposition | Design rationale |
|----------|--------|-------------|------------------|
| `GetAllAsync(skip, take)` | Legacy Sheets | **Preserve** | Paginated list; ordering by `Data` descending, `Horario` ascending |
| `GetByIdAsync(string id)` | Legacy Sheets | **Adapt → Guid** | String row-key replaced by Guid PK across all four verified verticals |
| `GetByNameAsync(string name)` | Legacy Sheets | **Preserve** | Case-insensitive substring search (DQ-001); empty array when no matches |
| `GetByPacienteIdAsync(string pacienteId)` | Legacy Sheets | **Adapt → Guid** | String → Guid; filter by nullable FK; empty array when no matches |
| `CreateAsync` | Legacy Sheets | **Preserve** | Auto-generated Guid identity; aggregate factory sets `Status = SemSenha` |
| `UpdateAsync` | Legacy Sheets | **Adapt → Guid** | Row identified by Guid; broad scalar update with FK immutability |
| `DeleteAsync` | Legacy Sheets | **Adapt → soft delete** | Physical delete replaced by ADR-001 soft delete via aggregate method |
| 17-column Sheets mapping | Legacy Sheets | **Preserve (mapping logic)** | Column mapping informs entity field design; Sheets serialization abandoned |
| `CreateAgendamentoToSheets` | Legacy Sheets | **Abandon** | Sheets serialization infrastructure |
| `ParseStatusAgendamento` | Legacy Sheets | **Preserve (mapping logic)** | Enum storage format handled by persistence configuration |
| `CheckSenhaAgendmaneto` | Legacy Sheets | **Abandon** | Stub returning empty value |
| `AgendamentosFilter` enum | Legacy Sheets | **Abandon** | Sheets infrastructure |
| Legacy column (Procedimento) | Legacy Sheets | **Abandon** | No database column exists; procedure data sourced from Internacao |

## Runtime Validation Environment

| Check | Environment | Expected result | Evidence location |
|-------|-------------|-----------------|-------------------|
| Swagger smoke — all 7 endpoints | DocAPI running locally, Docker SQL | All endpoints return documented status codes; Swagger UI reflects Guid params and correct DTO shapes | **Verify owns durable evidence** in `verification.md` § Runtime Validation |
| SQL integration — CRUD round-trip | Docker `docorgano-sql` + credentials | Create → read → update → soft delete → read excluded passes | Test output |
| HTTP smoke — POST with invalid FK | DocAPI running + SQL | 404 with body matching the FK error format defined in API Contract | Swagger or manual check |
| HTTP smoke — GET by-name with no matches | DocAPI running + SQL | 200 with `[]` (not 404) | Swagger or manual check |

**Runtime validation ownership:**
- **Execute:** Confirm environment readiness; run manual or scripted checks as time permits; record intent in session notes.
- **Verify:** Owns durable HTTP/runtime evidence in `verification.md`. If Execute did not record smoke notes, Verify runs the checks and records the results.

## Optional Test Debt (Forward Migration Verticals)

| Optional test | When to include | Default for backend stabilization |
|---------------|-----------------|-------------------------------------|
| Object-mapping tests | DTO ↔ entity mapping is non-trivial | **Recommended** — three DTO redesigns, owned value, and enum conversions make mapping error-prone |
| Soft-deleted parent FK negative test | Create depends on parent FK with soft delete | **Recommended** — POST with a soft-deleted Atendimento should return 404 |
| HTTP integration tests through controller pipeline | Critical status codes beyond unit mocks | **Optional** — Verify HTTP smoke covers routes |

## Fixture Chain

Integration tests require constructing prerequisite entities before Agendamento can be created. The chain below describes structural dependencies, not implementation instructions.

| Step | Entity | Prerequisite | Notes |
|------|--------|-------------|-------|
| 1 | `Paciente` | None | Required for Paciente FK validation |
| 2 | `Atendimento` | `Paciente` | Requires Paciente FK; required for Atendimento FK |
| 3 | `Prontuario` (with `Internacao`) | `Paciente`, `Atendimento` | Internacao is created as a child of Prontuario; required for Agendamento FK |
| 4 | `Agendamento` v1 | `Atendimento`, `Internacao` | Create with required FKs; optional `PacienteId` |
| 5 | Update v1 | `Agendamento` v1 | PUT mutable fields; FK unchanged |
| 6 | Soft delete v1 | Updated `Agendamento` v1 | DELETE; verify exclusion from reads |

**Fixture notes:**
- Internacao is created as part of the Prontuario aggregate — there is no standalone Internacao repository. The integration fixture creates a Prontuario containing an Internacao, then uses the Internacao's identity for Agendamento creation.
- Paciente is optional on Agendamento creation — test both with and without `PacienteId` provided.
- All fixture data is synthetic; no real clinical or patient-identifying data.

## Testing Approach

| Requirement | Test or check approach | Notes |
|-------------|------------------------|-------|
| `REQ-001` | Code review + compilation | Entity has `PacienteId` (Guid?), `Paciente` nav, no `Procedimento` string, enums preserved |
| `REQ-002` | Code review + compilation | Interface methods all use `Guid`; interface compiles |
| `REQ-003` | Unit tests (repository isolation) + SQL integration tests (CRUD round-trip) | Repository behavior verified in isolation; integration tests verify persistence mapping and SQL compatibility |
| `REQ-004` | Code review + schema diff | Paciente relationship is mapped; existing config preserved; zero migration delta |
| `REQ-005` | Code review + security-PHI sensor | Zero diagnostic-logging calls; all routes use Guid constraints |
| `REQ-006` | Unit tests (application-layer FK validation with mocked dependencies) + SQL integration tests (negative FK scenarios) | Verify 404 with correct FK error body for each FK target; integration tests exercise FK constraint behavior |
| `REQ-007` | Unit tests (controller) + Swagger smoke | Mock returns empty/filled data; verify 200/404 status codes |
| `REQ-008` | Unit tests (aggregate + application layer) + SQL integration tests | FK values cannot change through update; integration confirms persistence of mutable fields |
| `REQ-009` | Unit tests (aggregate + repository isolation) + SQL integration tests | Aggregate SoftDelete sets correct state; deleted rows excluded from all query paths |
| `REQ-010` | Code review + compilation | DTO types correct; no `Procedimento`; no `Status` on Create; enums on Read; no FK on Update |
| `REQ-011` | Unit tests (aggregate) | Factory sets `Status = SemSenha` and creation timestamp with non-empty identity; SoftDelete sets deletion markers; Update sets last-modified timestamp; SetSenha mutates owned value |
| `REQ-012` | `dotnet test` run | All Agendamento test suites pass; SQL integration tests pass when Docker is available |
| `REQ-013` | Specify acceptance | Legacy behavior table accepted — Execute follows preserve/adapt/abandon decisions |

## Architectural Capabilities And Expected Test Suites

| Architectural Capability | Expected test suite | Rationale |
|--------------------------|--------------------|-----------|
| Persistence Intent | Repository Tests | New SQL persistence replacing stub — must verify CRUD, query behavior, soft-delete exclusion |
| Physical Persistence | SQL Integration Tests | Schema alignment — entity matches `InitialCreate` without delta; FK constraints exercised |
| API Contract | Controller Tests | Seven endpoints with Guid routing, status codes, FK error format, DTO shapes |
| Business Invariants | Entity Tests | Factory defaults, SoftDelete state, FK immutability, mutation timestamps |

No architectural capabilities are excluded for this feature — a new SQL vertical exercises all four.

### Architectural Decision To Test Scenario Mapping

| Architectural decision | Expected repository test scenario(s) |
|------------------------|--------------------------------------|
| FK targets immutable after create | Update operation cannot alter `InternacaoId`, `AtendimentoId`, or `PacienteId` — persisted row retains original FK values |
| Soft delete excludes from all standard queries | `GetByIdAsync`, `GetAllAsync`, `GetByNameAsync`, `GetByPacienteIdAsync` all exclude rows in the deleted state |
| Factory initializes `Status = SemSenha` with creation timestamp | Created aggregate always has `StatusAgendamento.SemSenha` and a non-default creation timestamp |
| Name search uses substring matching | Query for a partial name returns aggregates whose `Nome` contains that substring |
| Pagination caps at 100 results per page | A page-size request exceeding 100 returns at most 100 results; a page-size request of 0 returns an empty result set |

## Verification Handoff Notes

List expected verifier inputs. The Verifier will select final gates.

- **Expected gates:** `dotnet build`, `dotnet test` (unit + SQL integration), code review (aggregate, interface, entity-to-schema mapping, DTOs, controller), security-PHI sensor (diagnostic-logging removal), Swagger smoke checklist (seven endpoints).
- **Expected review sensors:** `security-phi-review.md` (PHI-bearing logging removal), API contract review (Guid contract, status codes, FK error format), EF migration review (zero delta; entity-schema alignment).
- **Known skipped or manual checks:** HTTP smoke deferred to Verify. Controller HTTP integration tests are optional per Optional Test Debt table. `AtualizadoPor` remains unset — accepted residual risk from absent authentication.
- **Evidence the implementation must provide:** Passing `dotnet test` output; zero diagnostic-logging calls in controller confirmed by review; entity-to-schema mapping includes Paciente relationship; DTOs compile with correct types; repository implementation uses the persistence context.

## Risks

| Risk | Impact | Mitigation |
|------|--------|------------|
| Entity-schema misalignment produces unexpected migration delta | Blocks Execute — migration delta violates "no-new-migrations" constraint | Verify schema diff after entity changes; any delta indicates a column-name or type mismatch requiring mapping adjustment, not a new migration |
| `InternacaoId` NOT NULL blocks outpatient scheduling | Cannot create Agendamento before Internacao exists | Residual risk per Research DC-3 (Medium confidence). If the business requirement materializes, evaluate ADR before changing the constraint |
| Multiple FK dependencies on create increase coupling | POST depends on three upstream aggregates plus the Internacao table | Coupling is acceptable for current architecture — all upstream dependencies are verified and stable. FK validation is a lightweight existence check |
| Owned-value mutation bypasses domain operation | Object mapper may directly set `SenhaAgendamento` properties, circumventing `SetSenha` behavior | Write-direction mappings must use manual construction through aggregate operations — no automated mapping from DTO to aggregate |
| Frontend breaks on Guid identity | WS07 is deferred; frontend continues to use string IDs for Agendamento endpoints | Documented as known drift. WS07 must execute before production use. Backend API is tested independently |
| Name search scans full table on large datasets | Unindexed substring search on `Nome` | Acceptable for MVP — `Nome` is short. Future SDD may add an index if query patterns justify it |

## ADR Evaluation

| Question | Answer |
|----------|--------|
| Does this change affect durable architecture, persistence, schema lifecycle, security/auth, API contracts, ownership, runtime, or irreversible migration decisions? | No |
| Existing ADRs referenced | ADR-001 (soft delete — honored; global query filter preserved) |
| New ADR candidate | None |
| Decision needed before tasks or execution? | No |

**ADR candidate evaluation notes:**
- `InternacaoId` NOT NULL constraint: Research DC-3 identified this as a potential ADR candidate if outpatient scheduling without Internacao is required. The constraint is not being changed — schema is authoritative. If the requirement emerges, evaluate ADR before relaxing.
- No migration delta — no schema lifecycle decision.
- `Procedimento` field abandonment is a data representation choice, not an architectural decision.
- Guid identity follows the established pattern from three verified verticals — no ADR needed.
- FK error format is a contract convention, not an architectural decision.

## Documentation Follow-up Candidates

List possible follow-up. Documentation Update will route final ownership.

- **State:** Update after Design completion; update branch/status when Execute begins; update test counts after Execute.
- **ADR:** None.
- **Architecture docs:** No changes — aggregate boundaries, domain rules, and migration sequencing are unchanged.
- **Technical docs:** `Documentation/Technical/migration-sql.md` — note that Agendamento is the fourth verified SQL vertical (after Execute + Verify).
- **Rules:** No changes identified.
- **Skills:** No changes identified.
- **Review prompts:** No new prompts needed — existing security-PHI, API contract, and EF migration review prompts cover this feature.
- **Templates:** No changes identified.
- **PM:** `Documentation/Product/PM_DocOrgano.md` — update WS01 status after Verify.