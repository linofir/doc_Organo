# Research — Agendamento SQL Migration & Stabilization

> Feature SDD: `Documentation/SDD/Agendamento_Stabilization/`
> Generated SDD artifacts are written in English.
> **Phase:** Research Part 1 + Part 2 (complete)
> **Lifecycle position:** Research → Specify → Design → Tasks → SDD Pre-Execution Review → Execute → Verify → Documentation Follow-Up → Reporting → Teacher Guide

This document consolidates:
1. **Part 1 — Feature Discovery & Scope Decomposition** — boundary analysis, legacy characterization, data/API/security/architecture discovery
2. **Part 2 — Evidence Consolidation & Decision Analysis** — resolves 6 architectural unknowns via codebase evidence and governance validation

When Part 1 and Part 2 conflict, **Part 2 governs**.

---

# Part 1 — Feature Discovery & Scope Decomposition

## Executive Summary

### PM item vs real boundary

The PM item **WS01 — Agendamento SQL Migration & Stabilization** (status: Planejado) describes a single feature: migrate the Agendamento aggregate from Google Sheets to SQL Server. The real implementation boundary includes:

- **Entity-schema mismatch resolution** — the entity model (`Agendamento.cs`) lacks `PacienteId` and `Procedimento` properties that exist in the SQL schema, DTOs, and Legacy behavior
- **Interface Guid migration** — `IAgendamentoRepository` uses `string` IDs; must align with the established Guid pattern (Paciente, Atendimento Minimal, Prontuario)
- **Owned value object** — `SenhaAgendamento` is the first SQL vertical using EF owned types (Prontuario used nested entities)
- **Multiple FK targets** — `AtendimentoId` (verified), `InternacaoId` (via Prontuario, verified), and `PacienteID` (migration column exists; entity missing)
- **Controller hardening** — PHI removal, Guid routes, FK validation, soft delete
- **Frontend enum drift** — backend and frontend `StatusAgendamento` enums have diverged
- **Legacy behavior preservation** — CRUD surface, status workflow, name/PacienteId queries

### Core recommendation

Stabilize the Agendamento aggregate as the fourth SQL vertical slice following the established pattern from Atendimento Minimal. The work must resolve entity-schema ambiguity before Execute. The primary question: **does Agendamento have a direct FK to Paciente (as the migration suggests) or only via navigation through Atendimento/Internacao?**

### Preliminary sizing and SDD necessity

| Field | Recommendation |
|-------|---------------|
| Size | **Large** |
| SDD Required | **Yes** — SQL migration vertical, owned value object, multiple FKs, entity-schema mismatch, DTO redesign, enum drift, Legacy reference |
| Rationale | More complex than Atendimento Minimal (Medium): owned type, 3 FK targets, entity remodeling required. First aggregate where entity properties do not match the existing migration schema. |

---

## Feature Boundary

### In boundary

| Item | Classification |
|------|---------------|
| `AgendamentoRepository` SQL implementation against `InitialCreate` schema | Core |
| Entity model alignment with migration schema | Core |
| EF config review — `AgendamentoConfiguration` | Core |
| `IAgendamentoRepository` Guid contract alignment | Core |
| `AgendamentoController` GUID routing, PHI-safe logging, status codes | Core |
| Create with FK validation (AtendimentoId, InternacaoId, PacienteId) | Core |
| Read by id, paginated list, list by PacienteId, list by Nome | Core |
| Update limited to mutable fields | Core |
| Soft delete (ADR-001) | Core |
| Dedicated API DTOs (aligned with entity) | Core |
| Domain methods on `Agendamento` entity (factory, soft-delete) | Core |
| Unit + SQL integration tests | Core |
| Swagger smoke checklist | Core |
| Legacy characterization for CRUD surface | Core |

### Out of boundary

| Item | Classification |
|------|---------------|
| `ValidacaoPreProcedimento`, `ValidacaoEtapaProcedimento` Legacy workflow methods | Owned by `atendimento-workflow-stabilization` |
| Integration with `CollectSenhasAutorizadasDataService` | WS06 integration |
| PDF report generation with Agendamento data | Out of scope |
| WS07 Blazor frontend alignment | Deferred — WS07 |
| New EF migrations | Out of scope (default) |
| Google Sheets import | Out of scope |
| Auth/RBAC | Out of scope |
| Atendimento Workflow stage advancement via Agendamento status | Owned by workflow SDD |
| InternacaoRepository creation | Out of scope — Internacao is accessed via Prontuario aggregate |

---

## Domain Discovery

### Business capability affected

Agendamento controls the **administrative planning of medical procedures**: scheduling, location, authorization codes (senhas), and procedural status tracking. It is a supporting aggregate for the Atendimento journey.

### Domain entities and aggregates affected

| Entity / Aggregate | Role in this feature |
|-------------------|---------------------|
| `Agendamento` | **Primary target** — SQL stabilization |
| `SenhaAgendamento` | **Owned value object** |
| `Atendimento` | **FK target** — verified; FK validation on create |
| `Internacao` | **FK target** — child of Prontuario aggregate; FK validation on create |
| `Paciente` | **FK target** — migration column confirmed |
| `Prontuario` | **Indirect** — Internacao belongs to Prontuario aggregate |
| `ProcedimentoInternacao` | **Indirect** — the domain source for procedure information |

### Existing business rules, validations, and invariants

| Rule | Source | Disposition | Notes |
|------|--------|-------------|-------|
| Agendamento controls dates, horario, local, sala | Domain_Overview_Business_Rules.md | **Preserve** | Core administrative fields |
| Agendamento controls autorizações (senhas) | Domain_Overview_Business_Rules.md | **Preserve** | `SenhaAgendamento` owned type |
| SenhaAgendamento fields | Legacy Sheets row (cols 8-11) | **Preserve** | Codigo, DataPedido, DataLiberacao, Validade |
| Agendamento linked to Paciente | Domain_Overview, ERD, migration | **Preserve** — optional FK | `PacienteID` column is nullable |
| Agendamento linked to Atendimento | ERD, migration | **Preserve** | `AtendimentoId` NOT NULL |
| Agendamento linked to Internacao | ERD, migration | **Preserve** — required FK | `InternacaoId` NOT NULL |
| Status workflow | Legacy enum + `InstantiateAgendamento` | **Preserve** | 7 states (0–6) |
| Nome query support | `IAgendamentoRepository.GetByNameAsync` | **Preserve** | Legacy + frontend usage |
| PacienteId query support | `IAgendamentoRepository.GetByPacienteIdAsync` | **Preserve** | Frontend uses it |
| Soft delete preserves history | ADR-001 | **Preserve** | Entity lacks method; DbContext has filter |
| Agendamento multiple per patient | Domain rule | **Preserve** | No unique constraint needed |
| Procedimento field | Legacy Sheets row[4] | **Abandon** as entity property | Use `Internacao → ProcedimentoInternacao` relationship |

---

## Legacy Characterization Discovery

### Source: `DocAPI/Legacy/_LegacySheetsDb/AgendamentoSheetsRepository.cs` (commented)

| Behavior | Decision | Notes |
|----------|----------|-------|
| `GetAllAsync(skip, take)` | **Preserve** | Paginated list |
| `GetByIdAsync(string id)` | **Adapt** → Guid | String comparison → Guid PK |
| `GetByNameAsync(string name)` | **Preserve** | Filter by Nome (col 0) |
| `GetByPacienteIdAsync(string pacienteId)` | **Preserve** | Filter by PacienteID (col 12) |
| `CreateAsync` | **Preserve** | Auto-generate Guid ID |
| `UpdateAsync` | **Adapt** → Guid | Row identified by Guid |
| `DeleteAsync` | **Adapt** → soft delete | Physical delete → ADR-001 |
| `InstantiateAgendamento(row)` | **Preserve** (mapping) | 17-column Sheets row → entity |
| `CreateAgendamentoToSheets` | **Abandon** | Sheets serialization |
| `ParseStatusAgendamento` | **Preserve** (mapping) | String → enum mapping |
| `CheckSenhaAgendmaneto` | **Abandon** | Stubbed; returns empty Senha |
| `AgendamentosFilter` enum | **Abandon** | Sheets infrastructure |

### Legacy sheet columns (17 cols, A-Q)

| Col | Field | Disposition |
|-----|-------|------------|
| 0 | Nome | Preserve |
| 1 | Aviso | Preserve |
| 2 | Data | Preserve |
| 3 | Horario | Preserve |
| 4 | Procedimento | **Abandon** (use Internacao.Procedimentos) |
| 5 | Local | Preserve |
| 6 | Sala | Preserve |
| 7 | Status | Preserve |
| 8 | Senha.Codigo | Preserve |
| 9 | Senha.DataPedido | Preserve |
| 10 | Senha.DataLiberacao | Preserve |
| 11 | Senha.Validade | Preserve |
| 12 | PacienteID | Preserve (optional) |
| 13 | ID | Preserve |
| 14 | StatusAtestado | Preserve (string) |
| 15 | StatusInstrucoes | Preserve (string) |
| 16 | DataConsulta | Preserve |

---

## Architecture Discovery

### Layers affected

| Layer | Impact | Details |
|-------|--------|---------|
| `Core/Entities/Agendamento` | **High** | Add `PacienteId` (Guid?), resolve enum types |
| `Core/Entities/SenhaAgendamento` | **Low** | Review only |
| `Core/Interfaces/Repositories/IAgendamentoRepository` | **High** | string→Guid migration |
| `Infrastructure/Repositories/AgendamentoRepository` | **High** | Primary implementation target |
| `Infrastructure/SqlDb/Configurations/AgendamentoConfig` | **Medium** | Add PacienteId mapping; review enum config |
| `Application/Data/Dtos/Agendamento/*` | **High** | Redesign DTOs |
| `Application/Mappings/Profiles/AgendamentoProfile` | **High** | Replace AutoMapper write paths |
| `API/Controllers/AgendamentoController` | **High** | Guid routes, PHI removal, FK validation |
| `DocDbContext` | **Low** | Review global query filter |

### Components explicitly not involved

- `PacienteRepository`, `AtendimentoRepository` — queried for FK validation only
- `ProntuarioRepository` — queried for Internacao FK validation
- `InternacaoRepository` — does not exist; Internacao accessed via Prontuario
- `PdfGeneratorService` — commented; out of scope
- `DocFront.Web` — deferred to WS07

---

## Data Discovery

### Tables

| Table | Usage |
|-------|-------|
| `Agendamento` | **Full CRUD** — primary target |
| `Atendimento` | Read-only FK validation |
| `Internacao` | Read-only FK validation (via ProntuarioRepository) |
| `Paciente` | Read-only FK validation |

### Entity (post-Part 2 evidence)

| Property | Schema column | Nullable | Entity status | Part 2 resolution |
|----------|--------------|----------|---------------|-------------------|
| `ID` | `ID` | NOT NULL | Present | Preserve |
| `InternacaoId` | `InternacaoId` | NOT NULL | Present | **Preserve as required** |
| `AtendimentoId` | `AtendimentoId` | NOT NULL | Present | Preserve |
| `PacienteId` | `PacienteID` | **Nullable** | **Missing** | **Add as `Guid?`** |
| `Nome` | `Nome` | NOT NULL | Present | Preserve |
| `Aviso` | `Aviso` | NOT NULL | Present | Preserve |
| `Procedimento` | **Not in schema** | N/A | **Missing** | **Do not add** — use Internacao.Procedimentos |
| `Data` | `Data` | NOT NULL | Present | Preserve |
| `Horario` | `Horario` | NOT NULL | Present | Preserve |
| `Local` | `Local` | NOT NULL | Present | Preserve |
| `Sala` | `Sala` | NOT NULL | Present | Preserve |
| `Status` | `Status` | NOT NULL | Present | Preserve (enum) |
| `InstrucaoStatus` | `InstrucaoStatus` | NOT NULL | Present | **Keep enum** |
| `AtestadoStatus` | `AtestadoStatus` | NOT NULL | Present | **Keep enum** |
| `DataConsulta` | `DataConsulta` | NOT NULL | Present | Preserve |
| `SenhaAgendamento` | Owned columns | Nullable | Present | Preserve |
| Audit fields | — | — | Present | Preserve |

### EF mappings

| Concern | Current | Part 2 resolution |
|---------|---------|-------------------|
| `HasQueryFilter(a => !a.Deletado)` | Present | OK |
| Owned type `SenhaAgendamento` | Correct | OK |
| `AtendimentoId` FK | Configured | Index exists via FK; no additional index needed |
| `InternacaoId` FK | Configured | OK |
| `PacienteID` FK | **Missing from entity** | Add `PacienteId` (nullable) + `Paciente` nav property |
| `Procedimento` column | **Not in schema** | Do not add; use Internacao relationship |
| `InstrucaoStatus` / `AtestadoStatus` | Enum → string conversion | Preserve; string storage is correct |

---

## API Discovery

### Endpoints (target behavior)

| Method | Route | Target behavior |
|--------|-------|-----------------|
| GET | `/Agendamento?skip=&take=` | 200 + paginated `ReadAgendamentoDto[]` |
| GET | `/Agendamento/{id:guid}` | 200 / 404 |
| GET | `/Agendamento/by-name?nome=` | 200 / 404 |
| GET | `/Agendamento/by-pacientId?pacienteId={guid}` | 200 / 404 |
| POST | `/Agendamento` | 201 + `ReadAgendamentoDto`; FK validation (AtendimentoId, InternacaoId, PacienteId) |
| PUT | `/Agendamento/{id:guid}` | 204; limited field update |
| DELETE | `/Agendamento/{id:guid}` | 204; soft delete |

### Contract changes (Part 2 refined)

| Change | Rationale |
|--------|-----------|
| `string id` → `Guid id` | Align with Paciente/Atendimento/Prontuario pattern |
| Add `AtendimentoId` (Guid) to create DTO | FK required |
| Add `InternacaoId` (Guid) to create DTO | FK required |
| `PacienteId` → `Guid?` (nullable) | Schema is `nullable: true` |
| Remove `Procedimento` from DTOs | Use Internacao.Procedimentos relationship |
| `StatusInstrucoes`/`StatusAtestado` in DTOs | Keep as enum-typed (API serializes as string) |
| `Status` field | Keep as enum (backend authoritative values 0–6) |

### API consumers

| Consumer | Status | Impact |
|----------|--------|--------|
| `DocFront.Web` — `AgendamentoService` | Active; expects string IDs | **Breaks** — WS07 fix |
| `DocFront.Web` — `AgendamentoState` | Active; uses string IDs | **Breaks** — WS07 fix |
| Atendimento Workflow (future) | Not implemented | Will consume Guid-contract API |

---

## Frontend Discovery

### Backend Stabilization Rule

Frontend validation, UI fixes, and enum alignment are **out of Execute scope**. Deferred to WS07.

### Frontend enum drift (backend vs frontend)

| Enum value | Backend | Frontend | Drift |
|------------|---------|----------|-------|
| SemSenha | 0 | 0 | No |
| SenhaPendente | 1 | 1 | No |
| SenhaAprovada | 2 | 2 | No |
| AgendamentoEfetuado | 3 | **4** | **Drift** |
| AgendamentoRemarcado | 4 | **5** | **Drift** |
| ProcedimentoConcluido | 5 | **6** | **Drift** |
| Cancelada | 6 | **7** | **Drift** |

Backend values are authoritative. WS07 must align.

---

## Security & PHI Discovery

| Risk | Location | Severity | Treatment |
|------|----------|----------|-----------|
| `Console.WriteLine` with `agendamento.Nome` | `PostAgendamento` (line 70) | **High** | Remove |
| `Console.WriteLine` with `agendamento.ID` | `PostAgendamento` (line 71) | **Low** | Remove |
| Other `Console.WriteLine` calls | Multiple controller methods | **Medium** | Remove |
| Test fixtures | Tests | **Medium** | Synthetic data only |
| `Nome` field | Entity property | **Medium** | Non-PHI if copied from Paciente |


# Part 2 — Evidence Consolidation & Decision Analysis

## Decision Candidate 1: Should Agendamento own a direct PacienteId foreign key?

### Decision

Should the Agendamento entity include a direct FK to Paciente, or should it rely on the Atendimento → Paciente FK chain?

---

### Evidence Collected

| Evidence | Type | Source |
|----------|------|--------|
| Migration column `PacienteID` exists on Agendamento table | **Verified fact** | `InitialCreate.cs` line 529 |
| Column is `nullable: true` | **Verified fact** | `InitialCreate.cs`: `PacienteID = table.Column<Guid>(type: "uniqueidentifier", nullable: true)` |
| FK constraint `FK_Agendamento_Paciente_PacienteID` exists | **Verified fact** | `InitialCreate.cs` FK section |
| Entity lacks `PacienteId` property and `Paciente` navigation | **Verified fact** | `Agendamento.cs` |
| Legacy Sheets had `PacienteID` in row[12] | **Verified fact** | `AgendamentoSheetsRepository.cs` `InstantiateAgendamento` |
| `IAgendamentoRepository` has `GetByPacienteIdAsync(string)` | **Verified fact** | Interface |
| Frontend `AgendamentoState.BuscarTodosPorPacienteAsync` uses this query | **Verified fact** | `AgendamentoState.cs` |
| Atendimento already has `PacienteId` FK | **Verified fact** | `Atendimento.cs`, `AtendimentoConfiguration` |
| Agendamento has `AtendimentoId` FK → chain to Paciente exists via Atendimento | **Reasonable inference** | Schema |
| Prontuario has both `PacienteId` and `AtendimentoId` FKs | **Verified fact** | `Prontuario.cs` — precedent for dual FK |
| Legacy `PacienteID` in Sheets was used for Nome filling and filtering | **Reasonable inference** | Legacy code |

---

### Alternatives

#### Alternative A: Add `PacienteId` as nullable FK (align with schema)

**Benefits:**
- Matches existing migration schema (no migration delta needed)
- Enables direct `GetByPacienteIdAsync` without joining through Atendimento
- Supports use cases where Agendamento is queried by patient without loading Atendimento
- Precedent: Prontuario has both `PacienteId` and `AtendimentoId`
- Nullable allows Agendamentos where only Atendimento is known (Paciente can be inferred)

**Drawbacks:**
- Denormalized data — PacienteId is theoretically redundant with Atendimento → Paciente chain
- Risk of inconsistency if `Agendamento.PacienteId` ≠ `Agendamento.Atendimento.PacienteId`
- Additional FK validation on create

**Architectural impact:** Low. Follows Prontuario dual-FK precedent.
**Migration impact:** None (column already exists).

#### Alternative B: Remove `PacienteID` from schema, rely on Atendimento chain only

**Benefits:**
- No denormalization
- Single source of truth for Paciente linkage

**Drawbacks:**
- Requires **migration delta** to drop column + FK constraint
- `GetByPacienteIdAsync` must join through Atendimento table
- Frontend query pattern breaks
- Confirmed Legacy behavior (PacienteID in Sheets) is abandoned
- Prontuario dual-FK precedent is contradicted

**Architectural impact:** Medium — migration delta required; query pattern changes.
**Migration impact:** Delta required (drop column + FK).

---

### Recommendation

**Alternative A — Add `PacienteId` as nullable Guid FK.**

The migration schema, Legacy behavior, repository interface, and frontend all expect direct Paciente access. Prontuario already established the dual-FK precedent (PacienteId + AtendimentoId). Nullable accommodates Agendamentos where PacienteId is not provided at create time (e.g., inferred from Atendimento or populated later).

---

### Confidence

**High.** All evidence converges on preserving the existing schema column.

---

### Remaining Uncertainty

None. Evidence is conclusive.

---

## Decision Candidate 2: Should the legacy Procedimento field remain?

### Decision

Should Agendamento carry a `Procedimento` string property (as in Legacy and current DTOs), or should procedure information be derived from the Internacao → ProcedimentoInternacao relationship?

---

### Evidence Collected

| Evidence | Type | Source |
|----------|------|--------|
| No `Procedimento` column exists in Agendamento migration table | **Verified fact** | `InitialCreate.cs` lines 507–529 — columns explicitly listed |
| Legacy Sheets had `Procedimento` in row[4] | **Verified fact** | `AgendamentoSheetsRepository.cs` |
| `CreateAgendamentoDto` has `Procedimento` (string) | **Verified fact** | DTO |
| `ReadAgendamentoDto` has `Procedimento` (string) | **Verified fact** | DTO |
| `UpdateAgendamentoDto` has `Procedimento` (string) | **Verified fact** | DTO |
| `Agendamento.cs` entity has **no** `Procedimento` property | **Verified fact** | Entity |
| `ProcedimentoInternacao` entity exists with `CodigoProcedimento` + `Descricao` | **Verified fact** | `ProcedimentoInternacao.cs` |
| Internacao has `ICollection<ProcedimentoInternacao> Procedimentos` | **Verified fact** | `Internacao.cs` line 54–55 |
| Agendamento has `InternacaoId` FK → Internacao → ProcedimentoInternacao chain | **Verified fact** | Schema |
| Frontend `AgendamentoViewModel` has `Procedimento` (string) | **Verified fact** | Frontend ViewModel |
| ERD does not show `Procedimento` column on Agendamento | **Verified fact** | `erd.dbml` lines 428–461 |

---

### Alternatives

#### Alternative A: Abandon `Procedimento` field; derive from Internacao.Procedimentos

**Benefits:**
- No migration delta needed (column doesn't exist)
- Normalized: procedure data is owned by Internacao aggregate
- Eliminates duplication between Agendamento and Internacao.Procedimentos
- Entity model is simpler

**Drawbacks:**
- API consumers must traverse Agendamento → Internacao → Procedimentos for procedure info
- Frontend currently expects a `Procedimento` string in DTO (WS07 must adapt)
- Legacy behavior is abandoned (Procedimento was a simple string copy)

**Architectural impact:** Low. Aligns with normalized domain model.
**Migration impact:** None.

#### Alternative B: Add `Procedimento` column via migration delta

**Benefits:**
- Preserves Legacy behavior exactly
- No frontend DTO change needed for this field

**Drawbacks:**
- **Migration delta required** — new column on Agendamento table
- Denormalized: duplicates data available in Internacao.Procedimentos
- Risk of inconsistency between Agendamento.Procedimento and Internacao.Procedimentos
- Entity gains a field that isn't in the current migration schema

**Architectural impact:** Medium — migration delta; denormalization decision.
**Migration impact:** Delta required (add column).

---

### Recommendation

**Alternative A — Abandon `Procedimento` field.**

The migration schema does not have this column. The domain already models procedure information through `Internacao → ProcedimentoInternacao`. Adding a denormalized string would introduce data duplication and inconsistency risk. The frontend can adapt in WS07 to display Internacao.Procedimentos when needed. If a denormalized copy is needed for query performance, that decision belongs to Design/Tasks, not Research.

---

### Confidence

**High.** The migration schema is authoritative; no column exists.

---

### Remaining Uncertainty

None. The schema is conclusive.

---

## Decision Candidate 3: Should InternacaoId remain required or become nullable?

### Decision

Should `InternacaoId` on Agendamento remain NOT NULL (as in migration), or should it become nullable to accommodate the business lifecycle where Internacao may not exist at Agendamento creation time?

---

### Evidence Collected

| Evidence | Type | Source |
|----------|------|--------|
| Migration: `InternacaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)` | **Verified fact** | `InitialCreate.cs` line 508 |
| Entity constructor requires `Guid internacaoId` | **Verified fact** | `Agendamento.cs` line 9 |
| Entity has `Internacao` navigation property (non-nullable) | **Verified fact** | `Agendamento.cs` line 23 |
| ERD shows `InternacaoId` as NOT NULL | **Verified fact** | `erd.dbml` line 430 |
| Prontuario's `Internacao` is nullable (`Internacao?`) | **Verified fact** | `Prontuario.cs` |
| Legacy `InstantiateAgendamento` does not reference Internacao | **Reasonable inference** | Legacy reads from Sheets, not Internacao table |
| Internacao is created as part of Prontuario graph (not standalone) | **Verified fact** | `ProntuarioRepository` includes Internacao in graph |
| Internacao has `Procedimentos` navigation | **Verified fact** | `Internacao.cs` |
| Business lifecycle: Internacao may not exist when Agendamento is first created | **Assumption** | Requires domain expertise |

---

### Alternatives

#### Alternative A: Preserve NOT NULL (match migration + ERD)

**Benefits:**
- No migration delta
- Guarantees Agendamento always has procedure context
- Entity model and schema are consistent
- Enforces referential integrity

**Drawbacks:**
- Agendamento cannot be created before Internacao exists
- Internacao is nullable on Prontuario — a Prontuario may exist without Internacao
- If business allows Agendamento without Internacao (outpatient scheduling), this constraint forces artificial Internacao creation
- `InternacaoRepository` does not exist — FK validation requires querying through ProntuarioRepository

**Architectural impact:** Low (preserves current schema).
**Migration impact:** None.

#### Alternative B: Make nullable

**Benefits:**
- Agendamento can be created independently of Internacao
- Matches Prontuario's nullable Internacao pattern
- Supports outpatient scheduling scenarios

**Drawbacks:**
- **Migration delta required** — alter column nullability
- FK validation becomes conditional
- ERD must be updated
- Weaker referential integrity

**Architectural impact:** Medium — migration delta; schema change; ADR candidate.
**Migration impact:** Delta required.

---

### Recommendation

**Alternative A — Preserve NOT NULL.**

The migration schema, ERD, and entity model are consistent in requiring InternacaoId. Prontuario's nullable Internacao is a different concern (Prontuario lifecycle vs Agendamento lifecycle). If outpatient scheduling without Internacao is a real business need, that decision should come through an ADR, not this Research. For now, the schema is authoritative.

Agendamento FK validation for InternacaoId can be done through `ProntuarioRepository` (which includes Internacao in its query graph) or by directly querying `_context.Set<Internacao>()`.

---

### Confidence

**Medium.** The evidence from schema/ERD is conclusive, but domain expertise on the business lifecycle could override this. If stakeholders confirm outpatient scheduling without Internacao is needed, this becomes an ADR candidate.

---

### Remaining Uncertainty

**Requires stakeholder/domain decision** — if the business lifecycle allows Agendamento without Internacao. For now, accept migration schema as authoritative.

---

## Decision Candidate 4: Should InstrucaoStatus and AtestadoStatus remain enums or become strings?

### Decision

The entity model defines `StatusInstrucoes` and `StatusAtestado` as enums. DTOs and Legacy use strings. The migration stores varchar(30). Should the domain model use enums or strings?

---

### Evidence Collected

| Evidence | Type | Source |
|----------|------|--------|
| Entity: `StatusInstrucoes` is `enum StatusInstrucoes` | **Verified fact** | `Agendamento.cs` line 42 |
| Entity: `AtestadoStatus` is `enum StatusAtestado` | **Verified fact** | `Agendamento.cs` line 44 |
| Migration: `InstrucaoStatus = table.Column<string>(type: "varchar(30)", nullable: false)` | **Verified fact** | `InitialCreate.cs` line 517 |
| Migration: `AtestadoStatus = table.Column<string>(type: "varchar(30)", nullable: false)` | **Verified fact** | `InitialCreate.cs` line 518 |
| EF config: `HasConversion<string>().HasColumnType("varchar(30)")` | **Verified fact** | `AgendamentoConfig.cs` lines 39–45 |
| DTOs: `StatusInstrucoes` and `StatusAtestado` are `string` | **Verified fact** | `CreateAgendamentoDto.cs`, `ReadAgendamentoDtos.cs`, `UpdateAgendamento.cs` |
| Legacy: `StatusAtestado` and `StatusInstrucoes` stored as strings | **Verified fact** | `InstantiateAgendamento` row[14], row[15] |
| Enums have defined values: `StatusInstrucoes` = { SemSolicitacao=0, EmAnalise=1, Negado=2, Concluido=3 } | **Verified fact** | `Agendamento.cs` lines 101–113 |
| Enums: `StatusAtestado` = { NaoRealizado=0, Realizado=1, Pendente=2 } | **Verified fact** | `Agendamento.cs` lines 89–99 |
| `Status` enum on same entity uses identical pattern (HasConversion<string>) | **Verified fact** | `AgendamentoConfig.cs` line 35–37 |
| Paciente has no similar enum/string mixing pattern | **Reasonable inference** | Paciente vertical |
| Atendimento's `EtapaAtual` is enum with string conversion | **Verified fact** | Precedent for enum→string persistence |

---

### Alternatives

#### Alternative A: Keep enums in entity (current state)

**Benefits:**
- Type safety in domain layer
- Compile-time validation of valid states
- Consistent with `Status` enum pattern on same entity
- Consistent with `EtapaAtual` on Atendimento entity
- EF `HasConversion<string>()` handles persistence transparently

**Drawbacks:**
- DTOs currently use strings — mismatch must be resolved
- Adding new status values requires code change (rebuild, redeploy)

**Architectural impact:** None (preserves current entity model).
**Migration impact:** None.

#### Alternative B: Change entity to strings

**Benefits:**
- Matches DTOs and Legacy exactly
- Dynamic status values possible without code changes
- Simpler mapping (no conversion needed)

**Drawbacks:**
- Loses compile-time safety
- Inconsistent with `Status` enum on same entity
- Inconsistent with Atendimento `EtapaAtual` enum pattern
- No validation of valid status values at domain level

**Architectural impact:** Medium — entity model change; pattern inconsistency.
**Migration impact:** None (storage is already varchar(30)).

---

### Recommendation

**Alternative A — Keep enums in entity.**

The entity model is consistent: `Status`, `InstrucaoStatus`, and `AtestadoStatus` all use enums with string conversion. Atendimento's `EtapaAtual` uses the same pattern. DTOs should be aligned to use the enum types (not strings). The EF conversion layer handles string storage transparently. This maintains domain type safety without compromising persistence.

---

### Confidence

**High.** Consistent with existing entity patterns and Atendimento precedent.

---

### Remaining Uncertainty

None. Pattern is well-established across the codebase.

---

## Decision Candidate 5: Does the Internacao SQL repository exist?

### Decision

Can Agendamento FK validation for InternacaoId rely on an existing Internacao repository, or must Agendamento query Internacao through ProntuarioRepository or `DbContext.Set<Internacao>()`?

---

### Evidence Collected

| Evidence | Type | Source |
|----------|------|--------|
| No `InternacaoRepository` class exists in codebase | **Verified fact** | `search_files` returned 0 results |
| No `IInternacaoRepository` interface exists | **Verified fact** | `search_files` confirmed |
| No `DbSet<Internacao>` in `DocDbContext` | **Verified fact** | `DbContext.cs` — DbSet scan confirms only: Paciente, Atendimento, Prontuario, Agendamento, ClinicalEvent, AtendimentoPendencia, ChecklistDefinition, ChecklistExecution, ProntuarioAcaoCD, CID, DemonstrativoFinanceiro, GuiaFinanceira, ItemFinanceiro, ConciliacaoFinanceira, ItemConciliacao, AuditLog |
| Internacao is accessible via `Prontuario.Include(p => p.Internacao)` | **Verified fact** | `ProntuarioRepository.QueryWithIncludes()` |
| Prontuario's `Internacao` navigation is nullable (`Internacao?`) | **Verified fact** | `Prontuario.cs` |
| ProntuarioRepository is SQL-implemented (not stubbed) | **Verified fact** | `ProntuarioRepository.cs` |
| Prontuario verification confirms Internacao round-trip tested | **Verified fact** | `verification.md` — `ProntuarioSql_FullVersioningChain_WithInternacao_RoundTrip` |
| `Internacao` table exists in migration schema | **Verified fact** | `InitialCreate.cs` — `CreateTable("Internacao")` |
| EF Core can query `_context.Set<Internacao>()` even without explicit DbSet | **Reasonable inference** | EF Core convention |

---

### Alternatives

#### Alternative A: Query Internacao through ProntuarioRepository

**Benefits:**
- Uses existing, verified infrastructure
- Internacao is loaded with Prontuario graph already
- No new repository or interface needed

**Drawbacks:**
- FK validation requires loading full Prontuario graph to check Internacao existence
- Heavier query than direct Internacao lookup
- Prontuario may not have Internacao (nullable) — need to handle null case

#### Alternative B: Create minimal InternacaoRepository

**Benefits:**
- Clean FK validation via dedicated repository
- Explicit architectural boundary

**Drawbacks:**
- **New interface + repository** — violates "add interface only when justified"
- Internacao is not an aggregate root; creating a repository for it blurs boundaries
- Migration-sql.md does not list Internacao as a separate vertical
- Increased scope (not part of Agendamento SDD)

#### Alternative C: Query `_context.Set<Internacao>()` directly in AgendamentoRepository

**Benefits:**
- Simplest implementation
- No new abstractions
- Direct FK existence check
- Internacao table exists in schema

**Drawbacks:**
- AgendamentoRepository depends on Internacao table directly
- No explicit contract for Internacao queries

---

### Recommendation

**Alternative C — Query `_context.Set<Internacao>()` directly in AgendamentoRepository for FK validation.**

Internacao is not an aggregate root; it's a child of Prontuario. Creating a repository for it would violate aggregate boundaries. Querying through ProntuarioRepository is unnecessarily heavy (loading full Prontuario graph just to check InternacaoId existence). Using `_context.Set<Internacao>().AnyAsync(i => i.ID == internacaoId)` is the pragmatic pattern established by AtendimentoRepository for Paciente FK validation.

---

### Confidence

**High.** No InternacaoRepository exists; creating one is architecturally inappropriate.

---

### Remaining Uncertainty

None.

---

## Decision Candidate 6: Should AtendimentoId have a dedicated database index?

### Decision

The EF configuration (`AgendamentoConfig.cs`) does not define an explicit index on `AtendimentoId`. Should one be introduced?

---

### Evidence Collected

| Evidence | Type | Source |
|----------|------|--------|
| No explicit `HasIndex(x => x.AtendimentoId)` in EF config | **Verified fact** | `AgendamentoConfig.cs` |
| FK `FK_Agendamento_Atendimento_AtendimentoId` creates index `IX_Agendamento_AtendimentoId` | **Verified fact** | `InitialCreate.cs` — `CreateIndex("IX_Agendamento_AtendimentoId", "Agendamento", "AtendimentoId")` |
| SQL Server automatically indexes FK columns | **Verified fact** | SQL Server convention |
| AtendimentoRepository does not query Agendamentos | **Verified fact** | No reverse navigation |
| `GetByPacienteIdAsync` is the primary query pattern | **Verified fact** | `IAgendamentoRepository` |
| No query pattern exists that filters Agendamento by AtendimentoId | **Reasonable inference** | Current codebase |
| Atendimento Workflow (future) may query Agendamentos by AtendimentoId | **Reasonable inference** | `ValidacaoPreProcedimento` receives `atendimento` |

---

### Alternatives

#### Alternative A: No explicit index (current state)

**Benefits:**
- FK constraint already provides an implicit index
- No additional EF configuration needed
- No migration delta

**Drawbacks:**
- If Atendimento Workflow queries Agendamento by AtendimentoId heavily, the implicit FK index suffices

#### Alternative B: Add explicit `HasIndex(x => x.AtendimentoId)` in EF config

**Benefits:**
- Explicitly documents the access pattern
- No functional difference from implicit FK index

**Drawbacks:**
- May cause EF to attempt creating a duplicate index (FK already provides one)
- Redundant configuration

---

### Recommendation

**Alternative A — No explicit index.**

SQL Server FK constraints automatically create indexes on FK columns. The migration already confirms `IX_Agendamento_AtendimentoId` exists. Adding an explicit `HasIndex` in EF config would be redundant and could cause migration issues (duplicate index attempt). When Atendimento Workflow introduces query patterns that need specific indexes, that SDD should evaluate index design.

---

### Confidence

**High.** The FK constraint provides the needed index.

---

### Remaining Uncertainty

None.

---

## Historical Evidence Review

| SDD | Why Loaded | Uncertainty Reduced | Evidence Status |
|-----|-----------|---------------------|-----------------|
| Prontuario SQL Stabilization | Internacao persistence pattern, InternacaoRepository existence | Confirmed no InternacaoRepository; Internacao is child of Prontuario; FK validation pattern | **Sufficient** |
| Atendimento Minimal SQL Stabilization | Pattern reference for stubbed repo → SQL; Guid migration; FK validation in controller | Confirmed FK validation pattern, single-table CRUD approach | **Sufficient** (already loaded in Part 1) |
| Paciente SQL Stabilization | Owned type pattern (Endereco) | Owned type pattern is well-established; Agendamento's SenhaAgendamento follows same approach | **Sufficient** (optional reference) |

No additional SDDs need to be loaded. Historical evidence is sufficient.

---

## Dependency Review

| Dependency | Status | Architectural Impact | Sequencing | Blocker? |
|-----------|--------|---------------------|------------|----------|
| Paciente SQL Stabilization | **Verified** | PacienteId FK validation (nullable FK) | Must be verified before Agendamento Execute | No |
| Atendimento Minimal SQL Stabilization | **Verified** | AtendimentoId FK validation | Must be verified before Agendamento Execute | No |
| Prontuario SQL Stabilization | **Verified** | Internacao FK validation via ProntuarioRepository | Must be verified before Agendamento Execute | No |
| InternacaoRepository | **Does not exist** | FK validation via `_context.Set<Internacao>()` | No additional dependency | No |
| Test Governance Adoption | **Verified** | Test governance standards apply | No sequencing constraint | No |

### Updated Blockers

| Blocker | Part 1 Status | Part 2 Resolution |
|---------|--------------|-------------------|
| Entity-schema mismatch | **Blocking** | **Resolved** — 6 decision candidates evaluated with recommendations |
| Internacao SQL repository unknown | **Blocking** | **Resolved** — confirmed not needed; use `_context.Set<Internacao>()` |
| Frontend contract drift | Deferred (WS07) | Accepted risk |

---

## Readiness Decision

### Ready for Specify

All six decision candidates have been resolved with evidence-based recommendations:
1. **PacienteId**: Add as nullable Guid FK (matches schema)
2. **Procedimento**: Abandon; use Internacao.Procedimentos
3. **InternacaoId**: Preserve NOT NULL (matches schema/ERD)
4. **InstrucaoStatus/AtestadoStatus**: Keep enums (consistent with Status and Atendimento.EtapaAtual)
5. **Internacao repository**: Use `_context.Set<Internacao>()` directly
6. **AtendimentoId index**: Not needed (FK provides index)

### Open questions for Specify (not blockers)

The following are specification concerns, not architectural unknowns:
- Exact shape of `CreateAgendamentoDto` (field list)
- `Nome` field: copied from Paciente or user-entered?
- Controller FK validation error messages
- Whether `GetByNomeAsync` should use partial match or exact match
- Pagination strategy for `GetAllAsync`

### Specify Entry Checklist

- [x] No unresolved architectural ambiguity
- [x] Prerequisite dependencies understood and verified
- [x] API/integration contracts sufficiently defined
- [x] Migration strategy understood (no migration delta needed)
- [x] Feature boundaries stable
- [x] Required historical evidence reviewed

---

## References

- `Documentation/State.md`
- `AGENTS.md`
- `Documentation/Product/PM_DocOrgano.md`
- `Documentation/Technical/migration-sql.md`
- `Documentation/Architecture/Domain_Overview_Business_Rules.md`
- `Documentation/Architecture/erd.dbml`
- `Documentation/Architecture/ADR/ADR-001-soft-delete.md`
- `Documentation/SDD/atendimento-minimal-sql-stabilization/research.md`
- `Documentation/SDD/prontuario-sql-stabilization/verification.md`
- `DocAPI/Core/Entities/Agendamento.cs`
- `DocAPI/Core/Entities/Internacao.cs`
- `DocAPI/Core/Entities/ProcedimentoInternacao.cs`
- `DocAPI/Core/Entities/Prontuario.cs`
- `DocAPI/Core/Interfaces/Repositories/IAgendamentoRepository.cs`
- `DocAPI/Infrastructure/Repositories/AgendamentoRepository.cs`
- `DocAPI/Infrastructure/Repositories/ProntuarioRepository.cs`
- `DocAPI/API/Controllers/AgendamentoController.cs`
- `DocAPI/Infrastructure/SqlDb/Configurations/AgendamentoConfig.cs`
- `DocAPI/Infrastructure/SqlDb/DbContext/DbContext.cs`
- `DocAPI/Migrations/20260416212120_InitialCreate.cs`
- `DocAPI/Legacy/_LegacySheetsDb/AgendamentoSheetsRepository.cs`
- `DocFront.Web/Services/AgendamentoService.cs`
- `DocFront.Web/State/AgendamentoState.cs`
- `DocFront.Web/Mappers/AgendamentoMapper.cs`
- `DocFront.Web/Models/Enums/StatusAgendamento.cs`