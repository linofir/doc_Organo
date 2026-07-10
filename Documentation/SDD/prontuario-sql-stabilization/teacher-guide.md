# Teacher Guide — Prontuario SQL Stabilization

> **Audience:** Junior developers, developers new to Doc Organo, future maintainers, and the project owner learning the architecture.
>
> **Prerequisite artifacts:** This guide teaches *how and why* the feature works. For scope, verification, and operational status, see the linked artifacts at the end — do not expect this document to repeat them.

---

## How This Guide Differs From Other Artifacts

| Artifact | What it answers | This guide does not repeat it |
|----------|-----------------|-------------------------------|
| `specify.md` / `design.md` | What to build and how to design it | Requirement tables, full API contract tables, field classification D-01–D-06 |
| `verification.md` | Whether it passed and what evidence exists | Gate results, completion decisions, residual risk ratings |
| `feature-report.md` | What shipped and workflow lessons | Delivery summary, PM references, lessons learned |
| `session-handoff.md` | What to do in the next session | Next steps, branch status |
| `State.md` / PM | Current operational truth | Runtime or backlog status |
| ADR-001 / ADR-006 | Durable soft-delete and dual-mode API decisions | Full ADR text, consequence analysis |
| `migration-sql.md` | Migration checklist and endpoint reference | Full route/status tables |

**This guide owns:** concepts, reasoning, architecture flow, code connections, and a study path so you can understand and safely extend the Prontuario backend — the first Doc Organo aggregate with dual-mode versioning (correction vs clinical evolution).

---

## 1. Feature Overview

### Business purpose

**Prontuário** is the clinical snapshot aggregate — the central clinical documentation unit in Doc Organo. Each prontuário captures a patient's clinical state at a point in time: demographic identity snapshot, gynecologic history (AGO), personal and family antecedents, exam orders, CD (Conducta) action items, hospitalization requests, and post-operative notes. Multiple prontuários form a patient's versioned medical history.

### Technical objective

Prontuario was the **third aggregate migrated from Google Sheets to SQL Server**, and the first requiring **explicit versioning semantics**. Unlike Paciente (flat cadastral fields) and Atendimento Minimal (single-table journey container), Prontuario is a **composite clinical snapshot** — owned value objects, three child collection tables (Exames, AcoesCD, Internacao + Procedimentos), a mandatory Atendimento FK, and a patient-wide `Versao` counter.

The core technical challenge was resolving the tension between **domain versioning** (clinical changes create new immutable rows) and **operational correction** (typo fixes on the active record). The solution — ADR-006 dual-mode API — splits these into distinct HTTP operations.

### Stabilization goals (what "done" meant for learning purposes)

1. **Full nested-graph persistence** — Create, correction, and evolution persist Prontuario root + owned VOs + child collections (Exames, AcoesCD, Internacao with Procedimentos) in single transactions.
2. **Explicit versioning API** — The client chooses correction (`PUT`, same `Id`/`Versao`) or clinical evolution (`POST /versoes`, new row, incremented `Versao`). The backend never infers intent from payload diffs.
3. **Structural correction contract** — `UpdateProntuarioDto` exposes only 4 correction-safe fields. Evolution-only properties (Tipo, DataConsulta, AGO, collections) are absent from the type definition — they cannot be sent on PUT.
4. **Complete snapshot evolution** — Each evolved version is a full clinical picture for the supplied sections. Collections use **replace** semantics, never merge from the predecessor.
5. **Version generation authority split** — The application layer queries persistence for `max(Versao)`; the aggregate validates the assigned number and owns the versioning rule; the aggregate never queries the database.
6. **Soft delete per version** — Deleting one version hides that row; other versions in the patient's history remain visible.
7. **PHI-safe handling** — Rich clinical payloads (AGO, antecedentes, exames, internacao) never appear in logs or error messages.
8. **Test confidence** — 11 repository unit tests, 27 controller unit tests, 1 full-chain SQL integration test (79 total, +52 from baseline).

Frontend alignment was intentionally deferred (WS07). The backend is the learning baseline for Agendamento and future versioned aggregates.

---

## 2. What Changed

Think of the stabilization as four layers of replacement, not an incremental fix.

### Domain layer

- **`AplicarCriacao(dataConsulta, tipo, ..., exames, internacao)`** — Populates all clinical payload after v1 construction. Accepts full owned VOs, child collections, and optional Internacao. Sets `DataConsulta`, `Tipo` (int), `InformacoesExtras`, and all clinical sections in one call.
- **`AplicarCorrecao(informacoesExtras, profissao, religiao, atividadeFisica)`** — Updates only correction-safe scalar and VO fields in place. Does **not** touch child collections per D-01. Sets `AtualizadoEm`. Identity snapshot fields (NomePaciente, Cpf, Idade) and clinical narrative (QD) are immutable via correction.
- **`CriarNovaVersao(nextVersao, ...)`** — Creates a clinical-evolution successor. Validates `nextVersao > Versao` (throws `InvalidOperationException` if violated). Produces a new `Prontuario` instance with new `Guid`, assigned `Versao`, `ProntuarioAnteriorId` pointing to source, copied `PacienteId`/`AtendimentoId`, and all clinical payload from the evolution request. Returns the successor — does **not** mutate the source row.
- **`MarcarComoExcluido()`** — Sets `Deletado = true` and `DeletadoEm`. Single-version soft delete per ADR-001.
- **`DescricaoBasica.AplicarCorrecao(profissao, religiao, atividadeFisica)`** — Returns a **new** `DescricaoBasica` instance with correction-safe demographic fields updated; identity and clinical fields preserved from the original. This is an owned VO — EF persists it as columns on the Prontuario table.
- **`Prontuario(PacienteId, AtendimentoId)` constructor** — Sets `Versao = 1`, `CriadoEm = UtcNow`, new `Guid`. Does **not** accept clinical payload — that comes via `AplicarCriacao`.

### Application layer (DTOs + mapping)

- **`CreateProntuarioDto`** — Root `PacienteId` (Guid) + `AtendimentoId` (Guid) at top level. `Tipo` as **int**. Nested dedicated DTOs for all clinical sections — no domain entity types exposed.
- **`CreateVersaoProntuarioDto`** — Same clinical payload shape as create, but **without** `PacienteId`/`AtendimentoId` (inherited from source row). Used only by `POST /versoes`.
- **`UpdateProntuarioDto`** — **4 fields only**: `InformacoesExtras`, `Profissao`, `Religiao`, `AtividadeFisica`. This is the structural D-01 contract — evolution-only fields are absent from the type. No runtime field-diff is performed.
- **`ReadProntuarioDto`** — Includes `Id`, `PacienteId`, `AtendimentoId`, `Versao`, `ProntuarioAnteriorId`, `CriadoEm`, `AtualizadoEm`, `Deletado`, `DataConsulta`, `Tipo` (int), `InformacoesExtras`, and nested DTOs for all clinical sections.
- **`ProntuarioProfile` — read-only AutoMapper.** Create, update, and evolution write paths use explicit static helper methods in the controller (`MapDtoToDescricaoBasica`, `MapDtoToAGO`, etc.). AutoMapper is restricted to entity → `ReadProntuarioDto` projection only.
- **Controller mapping helpers** — `MapDtoToDescricaoBasica` snapshots `Nome`/`CPF`/`Idade` from the `Paciente` entity when available (REQ-001 identity snapshot), falling back to DTO values. `MapDtoToInternacao` constructs the optional 1:1 Internacao with nested Procedimentos. All helpers are `private static` — no AutoMapper on write paths.

### Infrastructure layer (repository + EF)

- **`ProntuarioRepository`** — Full SQL persistence replacing the stub. Reads use `QueryWithIncludes()` that loads the full nested graph (`Include` AcoesCD, Exames, Internacao → `ThenInclude` Procedimentos). All read methods apply the global soft-delete filter (ADR-001).
  - **`GetMaxVersaoForPacienteAsync`** — Uses `.IgnoreQueryFilters()` so soft-deleted versions still occupy their `Versao` slot. Returns `max(Versao)` or 0 when none. This is a **persistence fact lookup** — it does not compute next version or apply business policy.
  - **`UpdateAsync`** — Loads the tracked entity, calls `AplicarCorrecao` with correction-safe fields from the passed-in aggregate, saves. Does **not** mutate child collections (D-01).
  - **`DeleteAsync`** — Loads entity, calls `MarcarComoExcluido`, saves. Caller is responsible for invoking `MarcarComoExcluido` first.
  - **`AddAsync`** — Inserts a complete Prontuario graph in a single `SaveChangesAsync` call. Used for both v1 create and evolution successor insertion.
- **EF configuration** — No changes from `InitialCreate`. The unique index `(PacienteId, Versao)` already existed and is the persistence enforcement of D-06 concurrency. Global query filter on `Prontuario.Deletado` per ADR-001.

### API layer (controller)

- **10 endpoints** — Matches `design.md` API Contract table exactly. Guid route parameters throughout.
- **POST `/Prontuario`** — Validates Atendimento FK (exists, not soft-deleted, `PacienteId` match → 404). Loads Paciente for identity snapshot into `DescricaoBasica`. Validates CID when Internacao present (→ 400 if unknown). Builds clinical sections via static helpers, constructs aggregate, persists. Returns **201** + `ReadProntuarioDto`.
- **PUT `/Prontuario/{id}`** — Loads existing, calls `AplicarCorrecao` with correction-safe fields from DTO, persists via `UpdateAsync`. Returns **204** or **404**.
- **POST `/Prontuario/{id}/versoes`** — Loads source, validates CID if Internacao present, obtains `max(Versao) + 1` from repository, builds clinical sections, calls `CriarNovaVersao`. Two concurrency guards: aggregate `InvalidOperationException` (stale `nextVersao`) → **409**; `DbUpdateException` on unique index collision → **409**. Returns **201** + new `ReadProntuarioDto`.
- **DELETE `/Prontuario/{id}`** — Loads, marks excluded, persists. Returns **204** or **404**.
- **`GET /Prontuario/{id}`**, **`GET /Prontuario`** (paginated), **`GET /Prontuario/paciente/{id}`**, **`GET .../paciente/{id}/atual`**, **`GET .../atendimento/{id}/atual`** — All project to `ReadProntuarioDto` via AutoMapper.
- **`POST /Prontuario/from-pdf`** — Returns **501** immediately with no processing. The PDF extractor service is commented and out of scope.
- **PHI remediation** — No `Console.WriteLine` or equivalent. Error messages are generic ("Not Found", "CID não encontrado para a internação solicitada").

### Tests

- **11 repository unit tests** (InMemory) — Cover create v1, correction (same Id/Versao), evolution (new Id, incremented Versao, source unchanged), D-05 no-merge collection semantics, soft delete exclusion + `MaxVersao` slot preservation, list ordering (Versao DESC), latest-by-paciente and latest-by-atendimento, `GetMaxVersao` returns 0 when none, not-found returns null.
- **27 controller unit tests** (Moq) — Cover all 10 endpoints, all status code paths (201, 204, 200, 404, 400, 409, 501), FK validation (missing Atendimento, PacienteId mismatch), CID validation (unknown CID on create and evolution), concurrency (stale `nextVersao` → 409, `DbUpdateException` → 409), and structural DTO constraints.
- **1 SQL integration test** — Full fixture chain against Docker SQL: Paciente → Atendimento → Prontuario v1 (nested Exames + CD + Internacao with synthetic CID) → PUT correction → POST `/versoes` v2 (D-05 no-merge verified: V1 [HEMO, GLIC]; V2 [TSH] only) → soft delete v1 → V2 still visible. Uses `SqlIntegrationTestGate` with Docker skip policy.

---

## 3. Architecture Walkthrough

Doc Organo uses a **light Clean Architecture** split. For Prontuario, each operation flows through distinct paths due to the dual-mode versioning API.

### Create v1 flow

```text
HTTP POST /Prontuario (JSON: CreateProntuarioDto)
        │
        ▼
ProntuarioController.PostProntuario
        │  Validates Atendimento FK (exists, not soft-deleted, PacienteId match)
        │  Loads Paciente for identity snapshot
        │  Validates CID if Internacao present
        │  Maps DTO sections → domain VOs/entities via static helpers
        │    (MapDtoToDescricaoBasica, MapDtoToAGO, …, MapDtoToInternacao)
        ▼
new Prontuario(pacienteId, atendimentoId)   → Versao = 1, Id = new Guid
        │  .AplicarCriacao(dataConsulta, tipo, …, exames, internacao)
        ▼
ProntuarioRepository.AddAsync(prontuario)
        │  context.Prontuarios.Add(prontuario)
        │  SaveChangesAsync — single transaction for root + children
        ▼
ReadProntuarioDto via IMapper (ProntuarioProfile)
        │  Back to caller: 201 + DTO with Versao=1, full nested graph
```

### Correction flow (PUT)

```text
HTTP PUT /Prontuario/{id} (JSON: UpdateProntuarioDto — 4 fields only)
        │
        ▼
ProntuarioController.UpdateProntuario
        │  Loads existing by id → 404 if missing
        │  Calls existing.AplicarCorrecao(infosExtras, profissao, religiao, ativFisica)
        │    → Sets InformacoesExtras, updates DescricaoBasica (new instance)
        │    → Sets AtualizadoEm; Id, Versao, child collections UNCHANGED
        ▼
ProntuarioRepository.UpdateAsync(prontuario)
        │  Loads tracked entity by id
        │  Calls AplicarCorrecao again on the tracked instance
        │  SaveChangesAsync — UPDATE single row only
        ▼
204 No Content — same Id, same Versao
```

### Clinical evolution flow (POST /versoes)

```text
HTTP POST /Prontuario/{id}/versoes (JSON: CreateVersaoProntuarioDto)
        │
        ▼
ProntuarioController.CriarNovaVersao
        │  Loads source by id → 404 if missing
        │  Validates CID if Internacao present → 400 if unknown
        │  D-02: Repository.GetMaxVersaoForPacienteAsync(pacienteId)  → persistence fact
        │  Computes nextVersao = max + 1
        │  Maps clinical sections from DTO via static helpers
        │
        ▼
source.CriarNovaVersao(nextVersao, dataConsulta, tipo, …, exames, internacao)
        │  Validates: nextVersao > source.Versao (InvalidOperationException if violated)
        │  Constructs new Prontuario(PacienteId, AtendimentoId) — new Guid
        │    Sets Versao = nextVersao, ProntuarioAnteriorId = source.Id
        │    Calls AplicarCriacao for clinical payload
        │  Returns successor — source row BYTE-STABLE
        │
        ▼
ProntuarioRepository.AddAsync(novaVersao)
        │  context.Prontuarios.Add(novaVersao) — INSERT new row + children
        │  SaveChangesAsync
        │  DbUpdateException on (PacienteId, Versao) unique violation → 409 Conflict
        ▼
201 Created + ReadProntuarioDto — new Id, incremented Versao, ProntuarioAnteriorId set
```

### Read flow (with nested graph)

```text
HTTP GET /Prontuario/{id}
        │
        ▼
ProntuarioRepository.GetByIdAsync(id)
        │  QueryWithIncludes()
        │    → Include(AcoesCD)
        │    → Include(Exames)
        │    → Include(Internacao).ThenInclude(Procedimentos)
        │    → Global soft-delete filter hides Deletado=true rows
        ▼
ReadProntuarioDto via IMapper (ProntuarioProfile)
        │  Entity → DTO projection with all nested sections
        ▼
200 + DTO or 404
```

### Layer responsibilities

| Layer | Owns | Does NOT own |
|-------|------|--------------|
| **Controller** | HTTP semantics, FK/CID validation, identity snapshot from Paciente, version orchestration (obtain max → compute next → invoke aggregate), D-06 409 mapping | Domain rules, persistence, DTO→entity mapping (uses static helpers) |
| **Repository** | SQL CRUD, EF includes, transactions, `GetMaxVersaoForPacienteAsync` lookup, soft-delete filter | Business version policy, FK validation, CID validation, DTO mapping |
| **Aggregate** | Versioning rule validation (D-02), factory for v1, `AplicarCorrecao`, `CriarNovaVersao`, `MarcarComoExcluido`, child ownership on new version | Database queries, HTTP status codes, DTO validation |
| **DTOs** | API contract shape, structural correction-only properties (D-01), `Tipo` as int | Domain behavior, persistence |

---

## 4. Concept Inventory

### 1. Dual-Mode Versioning API (Primary — High Priority)

- **Appears in:** `ProntuarioController.UpdateProntuario` (PUT), `ProntuarioController.CriarNovaVersao` (POST /versoes), ADR-006
- **Why it exists:** Clinical documentation requires both operational correction (typo fixes, admin notes) and immutable clinical evolution (new snapshot on relevant change). The client explicitly chooses the operation via HTTP verb and route — the backend never infers intent from payload content.
- **How it works:** PUT `/Prontuario/{id}` accepts only `UpdateProntuarioDto` (4 correction-safe fields). POST `/Prontuario/{id}/versoes` accepts full clinical payload as `CreateVersaoProntuarioDto`. PUT updates in place (same `Id`/`Versao`); POST creates a new row (new `Id`, incremented `Versao`).
- **Related concepts:** Structural Correction Contract (D-01), Version Generation Split (D-02), Complete Snapshot Evolution (D-05)
- **Study priority:** High

### 2. Prontuario Aggregate (Rich Domain) (Primary — High Priority)

- **Appears in:** `DocAPI/Core/Entities/Prontuario.cs`
- **Why it exists:** The Prontuario is a composite clinical snapshot aggregate root. It owns identity (PacienteId, AtendimentoId), versioning metadata (Versao, ProntuarioAnteriorId), clinical scalars (DataConsulta, Tipo as int, InformacoesExtras), five owned value objects (DescricaoBasica, AGO, Antecedentes, AntecedentesFamiliares, PosOp), and three child collections (Exames, AcoesCD, Internacao). Domain methods enforce invariants — the constructor sets `Versao = 1`, `CriarNovaVersao` validates the assigned version number, `AplicarCorrecao` never touches child collections.
- **How it works:** `AplicarCriacao` is the unified clinical payload setter used by both v1 create and evolution. `CriarNovaVersao` produces a new `Prontuario` instance — it never mutates the source. `AplicarCorrecao` delegates to `DescricaoBasica.AplicarCorrecao` for VO-level correction. All property setters are `private` — mutation is only through named domain methods.
- **Related concepts:** Version Generation Split (D-02), Owned Value Objects, Child Collections
- **Study priority:** High

### 3. Version Generation Authority Split (D-02) (Primary — High Priority)

- **Appears in:** `ProntuarioController.CriarNovaVersao` (lines 177–178), `ProntuarioRepository.GetMaxVersaoForPacienteAsync`, `Prontuario.CriarNovaVersao` (line 124)
- **Why it exists:** Separates the **persistence fact** (current max `Versao` for a patient) from the **business rule** (whether an assigned version number is valid). The aggregate must not query the database — that would couple domain logic to infrastructure. The repository must not decide business policy — that would leak domain rules into persistence.
- **How it works:** 1) Controller calls `repository.GetMaxVersaoForPacienteAsync(pacienteId)` — returns 0 or an integer. 2) Controller computes `nextVersao = max + 1`. 3) Controller passes `nextVersao` to `source.CriarNovaVersao(nextVersao, ...)`. 4) Aggregate validates `nextVersao > Versao` — throws `InvalidOperationException` if violated. 5) Repository persists the aggregate-assigned `Versao` with `AddAsync`.
- **Related concepts:** Dual-Mode Versioning API, Concurrent Evolution 409 (D-06)
- **Study priority:** High

### 4. Structural Correction Contract (D-01) (Primary — High Priority)

- **Appears in:** `DocAPI/Application/Data/Dtos/Prontuario/UpdateProntuarioDto.cs`
- **Why it exists:** Prevents correction-vs-evolution ambiguity at the API contract level. If `UpdateProntuarioDto` contained `Tipo`, `DataConsulta`, or collection properties, clients could accidentally send clinical changes via PUT and expect evolution behavior. By structurally excluding evolution-only fields from the DTO type, the contract itself enforces the boundary — no runtime field-diff or value comparison is needed.
- **How it works:** `UpdateProntuarioDto` declares exactly 4 properties: `InformacoesExtras`, `Profissao`, `Religiao`, `AtividadeFisica`. The controller calls `AplicarCorrecao` with these 4 values. If a client sends extra JSON properties, model binding may reject them (400). The primary guard is the type definition — evolution-only fields simply do not exist on the DTO.
- **Related concepts:** Dual-Mode Versioning API, Field Classification (immutable / correction-safe / evolution-only)
- **Study priority:** High

### 5. Complete Snapshot Evolution (D-05) (Primary — High Priority)

- **Appears in:** `Prontuario.CriarNovaVersao` (line 134 calls `AplicarCriacao`), `ProntuarioRepositoryTests.D05_EvolutionDoesNotMergeCollections`, `ProntuarioSqlIntegrationTests` (lines 214–218)
- **Why it exists:** Each version is a complete clinical picture at a point in time. If evolution merged collections from the predecessor, a clinician removing an exam from the current plan would still see it inherited from an earlier version — clinically dangerous. Replace semantics ensure the evolution payload is the authoritative state for that version.
- **How it works:** `CriarNovaVersao` calls `AplicarCriacao` on the new instance with the evolution payload's collections. If the client supplies `Exames = [E3]` only, the new version has exactly `[E3]`. The predecessor's `[E1, E2]` remain on the predecessor row only. There is no merge step — EF inserts the new child rows and the predecessor's children stay with the predecessor's `ProntuarioId` FK.
- **Related concepts:** Prontuario Aggregate, Child Collections
- **Study priority:** High

### 6. Patient-Wide Version Numbering (Supporting — High Priority)

- **Appears in:** `Prontuario.Versao`, EF unique index `(PacienteId, Versao)`, `ProntuarioRepository.GetMaxVersaoForPacienteAsync`
- **Why it exists:** Medical history belongs to the patient, not to a single Atendimento. A patient may have multiple care journeys, each contributing to the clinical timeline. Version numbering across all atendimentos gives a single chronological sequence for the patient's entire prontuario history.
- **How it works:** `Versao` is scoped to `PacienteId`. Vacant atendimento B creates the patient's third prontuario, it gets `Versao = 3` (not 1). The unique index `(PacienteId, Versao)` enforces this at the database level. `GetMaxVersaoForPacienteAsync` queries `MAX(Versao)` across all atendimentos for the patient. `/atual` endpoints scope to patient or atendimento but return the highest `Versao` within that scope.
- **Related concepts:** Version Generation Split (D-02), Concurrent Evolution 409 (D-06)
- **Study priority:** Medium

### 7. Soft Delete Per Version (ADR-001) (Supporting — Medium Priority)

- **Appears in:** `Prontuario.MarcarComoExcluido`, `ProntuarioRepository.DeleteAsync`, `ProntuarioRepository.GetMaxVersaoForPacienteAsync` (`.IgnoreQueryFilters()`)
- **Why it exists:** Deleting a single version should not erase the patient's entire clinical history. ADR-001 requires logical deletion. The twist for versioned aggregates: a soft-deleted version still occupies its `Versao` slot — otherwise the next evolution would reuse the deleted version number, creating ambiguity.
- **How it works:** `MarcarComoExcluido` sets `Deletado = true` and `DeletadoEm`. Normal queries (via global filter) hide the row. `GetMaxVersaoForPacienteAsync` uses `.IgnoreQueryFilters()` so the max calculation includes soft-deleted rows — preserving their occupied slots. Other versions are unaffected.
- **Related concepts:** Prontuario Aggregate, Version Generation Split (D-02)
- **Study priority:** Medium

### 8. FK Validation on Create — Atendimento and Paciente (Supporting — Medium Priority)

- **Appears in:** `ProntuarioController.PostProntuario` (lines 46–56)
- **Why it exists:** A prontuario must belong to a valid, active Atendimento that belongs to the same patient. Without this validation, the database FK constraint would reject the insert with a cryptic SQL error. The controller translates this into structured HTTP responses.
- **How it works:** Before creating v1, the controller: 1) Loads Atendimento by `dto.AtendimentoId` — returns **404** if null or soft-deleted. 2) Checks `atendimento.PacienteId == dto.PacienteId` — returns **404** on mismatch. 3) Loads Paciente by `dto.PacienteId` — returns **404** if null (edge case; upstream Paciente should exist). All errors are generic (no PHI).
- **Related concepts:** CID Validation
- **Study priority:** Medium

### 9. CID Validation for Internacao (Supporting — Medium Priority)

- **Appears in:** `ProntuarioController.PostProntuario` (lines 59–65), `ProntuarioController.CriarNovaVersao` (lines 168–174)
- **Why it exists:** When a prontuario includes an Internacao section, the `CIDCodigo` FK must reference an existing CID row. Without validation, the database would return a FK violation as an unhandled 500. The controller catches this early and returns a structured 400.
- **How it works:** Before `AplicarCriacao`, if `dto.SolicitacaoInternacao != null`, the controller queries `context.CIDs.AnyAsync(c => c.Codigo == dto.SolicitacaoInternacao.CIDCodigo)`. If false → **400** "CID não encontrado para a internação solicitada." In tests, a synthetic CID row (`Z99.9`) is seeded in the fixture setup. Production CID catalog management is a separate feature.
- **Related concepts:** FK Validation on Create
- **Study priority:** Medium

### 10. Concurrent Evolution 409 (D-06) (Supporting — High Priority)

- **Appears in:** `ProntuarioController.CriarNovaVersao` (lines 192–214), `Prontuario.CriarNovaVersao` (line 124), EF unique index `(PacienteId, Versao)`
- **Why it exists:** Two clinicians evolving the same patient's prontuario simultaneously could both read `max(Versao) = 2`, both compute `nextVersao = 3`, and both attempt to insert. One must fail gracefully — not with a 500.
- **How it works:** Two layers of defense: 1) **Aggregate validation** — `CriarNovaVersao` throws `InvalidOperationException` if `nextVersao <= Versao`. This catches stale-version cases where the computed `nextVersao` is no longer ahead of the source version. 2) **Database unique constraint** — `(PacienteId, Versao)` unique index rejects the second insert with `DbUpdateException`. The controller catches this (matching on "UNIQUE" or "IX_Prontuario" in the inner exception message) and returns **409 Conflict**. The client should refresh and retry.
- **Related concepts:** Version Generation Split (D-02), Patient-Wide Version Numbering
- **Study priority:** Medium

### 11. AutoMapper Read-Only + Explicit Write Mapping (Supporting — Medium Priority)

- **Appears in:** `DocAPI/Application/Mappings/Profiles/ProntuarioProfile.cs`, `ProntuarioController` static helpers (lines 236–348)
- **Why it exists:** The legacy `ProntuarioProfile` used AutoMapper for create and update, causing known critical bugs (Internacao mapped into `InformacoesExtras`, missing fields, private setters bypassed). The fix was to **remove AutoMapper from all write paths** and use explicit, auditable static helpers in the controller. AutoMapper remains for the read projection only (`Prontuario → ReadProntuarioDto`), which maps the full nested graph cleanly.
- **How it works:** The controller has private static methods (`MapDtoToDescricaoBasica`, `MapDtoToAGO`, etc.) that construct domain objects from DTOs. These are transparent, testable, and have no hidden side effects. `ProntuarioProfile` is annotated as read-only and only contains `CreateMap` for entity → DTO direction.
- **Related concepts:** Prontuario Aggregate, Structural Correction Contract (D-01)
- **Study priority:** Medium

### 12. EF Includes for Nested Graph Reads (Supporting — Medium Priority)

- **Appears in:** `ProntuarioRepository.QueryWithIncludes` (lines 120–127)
- **Why it exists:** Prontuario reads must return the full clinical picture — Exames, AcoesCD, and Internacao with nested Procedimentos. Without eager loading, accessing navigation properties would either throw (detached context) or trigger N+1 queries.
- **How it works:** `QueryWithIncludes` returns `IQueryable<Prontuario>` with `Include` for AcoesCD, Exames, and `Include + ThenInclude` for Internacao → Procedimentos. All read methods (`GetByIdAsync`, `GetByPacienteIdAsync`, etc.) build on this base query. The global soft-delete filter is applied automatically by EF.
- **Related concepts:** Soft Delete Per Version (ADR-001), Prontuario Aggregate
- **Study priority:** Medium

### 13. Repository-Direct Integration Tests (Supporting — Medium Priority)

- **Appears in:** `DocAPI.Tests/Integration/ProntuarioSqlIntegrationTests.cs`
- **Why it exists:** Following the Atendimento pilot pattern, SQL integration tests exercise the repository directly (not through HTTP). This validates the persistence layer — entity mapping, FK constraints, unique indexes, transaction boundaries, soft-delete filter — without the overhead of HTTP client setup. HTTP smoke is covered separately (TASK-009 Swagger).
- **How it works:** The test creates a `DocDbContext` with real SQL Server connection (via `SqlConnectionResolver`), uses verified `PacienteRepository` and `AtendimentoRepository` for upstream fixtures, then exercises `ProntuarioRepository` directly. `SqlIntegrationTestGate` provides the skip policy — test is skipped when Docker is unavailable. A synthetic CID row is seeded in the test setup for Internacao scenarios.
- **Related concepts:** All repository and aggregate concepts
- **Study priority:** Medium

### 14. Identity Snapshot in DescricaoBasica (Adjacent — Low Priority)

- **Appears in:** `ProntuarioController.MapDtoToDescricaoBasica` (lines 236–253)
- **Why it exists:** The prontuario captures the patient's demographic identity at the time of documentation. If the patient's cadastral data changes later (e.g., name correction), the clinical snapshot should reflect what was true at documentation time. The controller snapshots `Nome`, `CPF`, and computed `Idade` from the `Paciente` entity at creation time.
- **How it works:** `MapDtoToDescricaoBasica` prefers `paciente.Nome`, `paciente.CPF`, and computed age over DTO values. If the Paciente is null (shouldn't happen due to FK validation), it falls back to DTO values. Identity fields are immutable via correction — only `Profissao`, `Religiao`, and `AtividadeFisica` can be corrected.
- **Related concepts:** Structural Correction Contract (D-01), Prontuario Aggregate
- **Study priority:** Low

### 15. Owned Value Objects and Child Collections (Adjacent — Low Priority)

- **Appears in:** `Prontuario.cs` (owned VOs: DescricaoBasica, AGO, Antecedentes, AntecedentesFamiliares, PosOp), `Exame.cs`, `Internacao.cs`, `ProntuarioAcaoCD`
- **Why it exists:** The Prontuario is a wide clinical document. Owned value objects (mapped as columns on the Prontuario table via EF) group related clinical fields. Child entities (separate tables with FK to Prontuario) represent collections that vary independently — exam orders, CD actions, and optional hospitalization.
- **How it works:** Owned VOs are constructed with all fields in their constructor and use private setters. They are persisted as columns on the Prontuario table (e.g., `DescricaoBasica_NomePaciente`). Child entities have their own `Id` (Guid) and `ProntuarioId` FK. The aggregate root creates and owns them — repository persistence cascades via EF change tracking.
- **Related concepts:** Prontuario Aggregate, Complete Snapshot Evolution (D-05)
- **Study priority:** Low

---

## 5. Deep Dive By Component

### Prontuario Aggregate (`DocAPI/Core/Entities/Prontuario.cs`)

The aggregate is 148 lines of domain behavior plus ~200 lines of owned value objects in the same file. Key design decisions:

- **Constructor sets identity, not clinical state.** `Prontuario(Guid pacienteId, Guid atendimentoId)` sets `Versao = 1`, `CriadoEm`, and navigation FKs. Clinical payload comes separately via `AplicarCriacao`.
- **`AplicarCriacao` is the single clinical setter** used by both v1 create and `CriarNovaVersao`. It accepts all sections including nullable child collections (null = no collection supplied).
- **`CriarNovaVersao` returns a new instance** — it never mutates `this`. The source row remains byte-stable. This is the immutability pattern: clinical evolution creates, never overwrites.
- **`AplicarCorrecao` is narrow** — 4 parameters, updates `InformacoesExtras` and `DescricaoBasica` only. Sets `AtualizadoEm`. Does not touch `DataConsulta`, `Tipo`, `AGO`, or any child collection.
- **`DescricaoBasica.AplicarCorrecao` returns a new VO instance** — it preserves `NomePaciente`, `Cpf`, `Idade` (identity) and `QD` (clinical narrative) from the original, and applies correction-safe demographic fields. This follows the same immutability pattern at the VO level.
- **`MarcarComoExcluido` is single-version** — sets `Deletado = true`, `DeletadoEm = UtcNow`. Other versions unaffected.

### ProntuarioRepository (`DocAPI/Infrastructure/Repositories/ProntuarioRepository.cs`)

128 lines. Key design decisions:

- **`QueryWithIncludes` is the read backbone** — all 6 read methods delegate to it. Centralized eager loading strategy. Adding a new navigation? Add the `Include` here once.
- **`GetMaxVersaoForPacienteAsync` ignores query filters** — this is intentional and documented. Without `.IgnoreQueryFilters()`, a soft-deleted V1 would make `MAX(Versao)` return 0 for a patient with only a V2 remaining, causing duplicate version assignment on the next evolution.
- **`UpdateAsync` reloads the entity** — it loads the tracked entity by ID, then calls `AplicarCorrecao` on the tracked instance. This avoids attaching a potentially disconnected entity and ensures only correction-safe fields change.
- **`AddAsync` is used for both v1 and evolution** — there is no separate "insert successor" method. The aggregate already produces a complete graph; the repository just adds it.
- **`DeleteAsync` expects pre-marked entity** — the aggregate's `MarcarComoExcluido` must be called before `DeleteAsync`. The repository loads the entity again and re-applies `MarcarComoExcluido` on the tracked instance.

### ProntuarioController (`DocAPI/API/Controllers/ProntuarioController.cs`)

349 lines. Key design decisions:

- **Static mapping helpers, not AutoMapper on write** — 8 private static methods convert DTOs to domain objects. Each is transparent: you can trace every field assignment.
- **`PostProntuario` orchestrates 4 validation steps** — Atendimento FK existence, PacienteId match, Paciente existence (for snapshot), CID existence (when Internacao present). All before touching the aggregate.
- **`CriarNovaVersao` has two concurrency catch blocks** — `InvalidOperationException` from aggregate validation (stale `nextVersao`) and `DbUpdateException` matching on "UNIQUE" or "IX_Prontuario" (database unique constraint). Both return **409**.
- **`UpdateProntuario` calls `AplicarCorrecao` twice** — once on the loaded entity (for the response path), once inside `UpdateAsync` (on the tracked instance). This is safe but redundant; future refactoring could streamline to one call.
- **`PostFromPDF` returns 501 immediately** — no file processing, no repository call, no side effects. The entire PDF pipeline is out of scope.

### DTOs (`DocAPI/Application/Data/Dtos/Prontuario/`)

- **`CreateProntuarioDto` and `CreateVersaoProntuarioDto`** share the same nested DTO types (DescricaoBasicaDto, AGODto, etc.) but differ at the root — `CreateProntuarioDto` has `PacienteId` + `AtendimentoId`; `CreateVersaoProntuarioDto` does not.
- **`UpdateProntuarioDto` is the D-01 enforcement point** — 4 fields, no collections, no clinical scalars. Adding a field to this DTO is an API contract change.
- **`ReadProntuarioDto`** mirrors the entity shape with version metadata surfaced (`Versao`, `ProntuarioAnteriorId`). `Tipo` is `int`.
- **Nested DTOs** share names with domain entities (e.g., `AGODto` vs `AGO`) but are in the `DocAPI.Data.Dtos.ProntuarioDtos` namespace — no domain types leak into public contracts.

### ProntuarioProfile (`DocAPI/Application/Mappings/Profiles/ProntuarioProfile.cs`)

48 lines. Read-only projection. Each nested entity (`DescricaoBasica`, `AGO`, `Exame`, `Internacao`, etc.) has an explicit `CreateMap` to its corresponding DTO. The main `Prontuario → ReadProntuarioDto` map uses `.ForMember` for every property (no auto-convention) — this is intentional to prevent silent mapping failures when entity and DTO property names diverge.

### Tests (DocAPI.Tests)

- **Repository tests (InMemory)** — 11 tests covering the full lifecycle. Uses `UseInMemoryDatabase` with unique database names per test. Each test sets up Paciente + Atendimento fixtures, exercises the repository, and asserts domain behavior through repository reads.
- **Controller tests (Moq)** — 27 tests covering all endpoints and status codes. Mocks `IProntuarioRepository`, `IPacienteRepository`, `IAtendimentoRepository`, and `DocDbContext` (for CID validation). Tests FK validation, CID validation, concurrency paths, and edge cases.
- **SQL integration** — Single test with full fixture chain. Uses `SqlConnectionResolver` for connection string, `SqlIntegrationTestGate` for skip policy, and verified upstream repositories (`PacienteRepository`, `AtendimentoRepository`). Seeds synthetic CID. Validates D-05 no-merge at SQL level.

---

## 6. Testing Walkthrough

### Test types and their roles

| Test type | Count | What it validates | Why it exists |
|-----------|-------|-------------------|---------------|
| **Repository unit** (InMemory) | 11 | Domain behavior through persistence: create, correction, evolution, D-05, soft delete, list ordering, latest reads | Fast feedback on aggregate + repository contract; no Docker dependency |
| **Controller unit** (Moq) | 27 | HTTP semantics: status codes, FK/CID validation, concurrency 409, route wiring, DTO shape enforcement | Covers all API contract paths without SQL; fast enough to run on every save |
| **SQL integration** | 1 | Full persistence chain against real SQL Server: FK constraints, unique index, EF mapping, soft-delete filter, nested graph round-trip | Proves the ORM mapping works end-to-end; catches EF/config mismatches that InMemory misses |

### Representative tests to read

1. **`AddAsync_ThenGetById_ReturnsProntuario`** — The v1 create test. Asserts `Versao = 1`, `PacienteId`, `AtendimentoId`, `ProntuarioAnteriorId = null`, `Deletado = false`.
2. **`AplicarCorrecao_ThenUpdate_PreservesIdAndVersao`** — The correction test. Asserts same `Id`/`Versao` after update, correction fields changed, identity fields unchanged, `AtualizadoEm` set.
3. **`CriarNovaVersao_IncrementsVersaoAndKeepsSourceIntact`** — The evolution test. Asserts V1 unchanged (same QD), V2 has new Id, `Versao = 2`, `ProntuarioAnteriorId = V1.Id`, both versions listable.
4. **`D05_EvolutionDoesNotMergeCollections`** — The no-merge test. V1 gets Exames [E1, E2]; V2 evolution supplies [E3] only. Asserts V1 still has 2 exames, V2 has exactly 1 (E3).
5. **`SoftDelete_ExcludesFromReads_ButKeepsInMaxVersao`** — The soft-delete test. Asserts deleted V1 hidden from `GetByIdAsync` and list, V2 visible, `MaxVersao` still returns 2.
6. **`ProntuarioSql_FullVersioningChain_WithInternacao_RoundTrip`** — The full integration test. Exercises Paciente → Atendimento → V1 (nested with Internacao) → PUT correction → POST /versoes V2 (no Internacao, [TSH] only) → D-05 assertion → soft delete V1 → V2 visible → MaxVersao preserved.

### Known test gaps

- **Two-atendimento patient-wide Versao scenario (D-03)** — Recommended but optional in design.md. A single patient with two atendimentos and prontuarios on each would validate that patient-wide `Versao` numbering works across atendimento boundaries.
- **Concurrent evolution 409 integration test** — Optional. The unit test `PostVersoes_StaleNextVersao_Returns409` and `PostVersoes_DbUpdateUniqueViolation_Returns409` cover both concurrency paths without requiring parallel test infrastructure.
- **Soft-deleted AtendimentoId → 404 on POST** — Optional test debt. Controller validates Atendimento existence and PacienteId match, but does not explicitly test the soft-deleted Atendimento path at the unit level.

---

## 7. Security Concepts

### PHI containment

Prontuario carries the richest clinical PHI in Doc Organo: gynecologic history (AGO), personal and family antecedents, exam orders, hospitalization details with procedures, and post-operative notes. The following patterns protect this data:

- **No logging of payload bodies or patient identifiers.** The controller has zero `Console.WriteLine` calls. Error responses use generic messages — "Not Found" for missing resources, "CID não encontrado para a internação solicitada" for CID validation. They never include patient names, CPF values, or clinical payload content.
- **Synthetic test data only.** All test fixtures use generated names ("Joana Teste", "Paciente Pront Teste"), synthetic CPFs (generated at runtime), and clinical descriptions that are obviously test data. No real patient data appears in any test file.
- **`POST /from-pdf` returns 501 immediately.** The endpoint performs no file processing, no repository calls, and no data extraction — eliminating a potential PHI ingestion vector from unverified PDF content.
- **CID FK validation returns structured errors, not raw SQL.** Unknown CID returns `BadRequest("CID não encontrado...")` — not a `DbUpdateException` with the full FK constraint name exposed.

### Soft delete vs hard delete

ADR-001 soft delete means deleted prontuarios remain in the database but are hidden from normal queries. This preserves clinical audit history. `GetMaxVersaoForPacienteAsync` uses `.IgnoreQueryFilters()` to ensure deleted version slots are not reused — preventing version number collision on re-evolution after deletion.

### Scope boundaries

- **Auth/RBAC is out of scope.** `AtualizadoPor` remains null on corrections — accepted residual risk.
- **Production CID catalog is out of scope.** CID validation uses the existing CID table; synthetic rows in tests only. Unknown CIDs return 400.
- **WS07 frontend alignment is deferred.** The backend does not serve UI-specific error codes or onboarding flows.

---

## 8. Knowledge Map

### Dependency chain 1: Versioning

```text
ADR-006 (dual-mode API policy)
  → UpdateProntuarioDto (D-01 structural correction contract)
    → AplicarCorrecao (narrow: 4 fields, no children)
  → CreateVersaoProntuarioDto (full clinical payload, no FKs)
    → GetMaxVersaoForPacienteAsync (persistence lookup, .IgnoreQueryFilters)
      → CriarNovaVersao (validates nextVersao > Versao, assigns identity)
        → AplicarCriacao (complete snapshot — D-05 replace, not merge)
          → AddAsync (single transaction insert)
        → DbUpdateException on (PacienteId, Versao) unique → 409 (D-06)
```

### Dependency chain 2: Create v1

```text
CreateProntuarioDto (root PacienteId + AtendimentoId, Tipo as int)
  → FK validation: Atendimento exists, PacienteId match → 404
  → Paciente loaded for identity snapshot
  → CID validation when Internacao present → 400
  → new Prontuario(pacienteId, atendimentoId) → Versao = 1
    → AplicarCriacao (all clinical sections via static helpers)
      → AddAsync (single transaction)
        → ReadProntuarioDto via AutoMapper (read-only projection)
```

### Dependency chain 3: Read with nested graph

```text
QueryWithIncludes (AcoesCD, Exames, Internacao → Procedimentos)
  → Global soft-delete filter (ADR-001)
    → GetByIdAsync / GetByPacienteIdAsync / GetLatestBy*
      → ReadProntuarioDto via ProntuarioProfile
```

### Concept dependency graph (textual)

```text
ADR-001 (Soft Delete)
  ├── Soft Delete Per Version → GetMaxVersaoForPacienteAsync (.IgnoreQueryFilters)
  └── Global Query Filter → All read methods

ADR-006 (Dual-Mode Versioning API)
  ├── Structural Correction Contract (D-01) → UpdateProntuarioDto (4 fields)
  ├── Version Generation Split (D-02) → GetMaxVersaoForPacienteAsync + CriarNovaVersao
  ├── Complete Snapshot Evolution (D-05) → CriarNovaVersao → AplicarCriacao
  └── Concurrent Evolution 409 (D-06) → CriarNovaVersao validation + DbUpdateException catch

Prontuario Aggregate
  ├── AplicarCriacao → v1 create + evolution payload
  ├── AplicarCorrecao → correction-safe only + DescricaoBasica.AplicarCorrecao
  ├── CriarNovaVersao → version validation + successor construction
  └── MarcarComoExcluido → soft delete single version

EF Infrastructure
  ├── QueryWithIncludes → eager loading nested graph
  ├── Unique index (PacienteId, Versao) → D-06 enforcement
  └── Global soft-delete filter → ADR-001 enforcement
```

---

## 9. Study Guide

### Beginner — Required before safely modifying this feature

| # | Topic | Where | Why it matters |
|---|-------|-------|----------------|
| 1 | **Dual-Mode Versioning API concept** | ADR-006, Controller PUT + POST /versoes | Every change to Prontuario endpoints must respect correction vs evolution. Wrong verb = wrong clinical semantics. |
| 2 | **Prontuario aggregate methods** | `Prontuario.cs`: `AplicarCriacao`, `AplicarCorrecao`, `CriarNovaVersao`, `MarcarComoExcluido` | These are the only ways to mutate a Prontuario. All property setters are private. |
| 3 | **DTO shapes and D-01 enforcement** | `CreateProntuarioDto.cs`, `UpdateProntuarioDto.cs`, `CreateVersaoProntuarioDto.cs`, `ReadProntuarioDto.cs` | Adding a field to the wrong DTO breaks the correction/evolution contract. |
| 4 | **Repository read pattern** | `ProntuarioRepository.QueryWithIncludes` | All reads go through this method. New navigations need an `Include` here. |
| 5 | **FK and CID validation in controller** | `ProntuarioController.PostProntuario` (lines 46–65) | Missing FK validation = 500 errors from SQL. Must validate before touching the aggregate. |

### Intermediate — Explains why the code is structured as it is

| # | Topic | Where | Why it matters |
|---|-------|-------|----------------|
| 1 | **Version Generation Split (D-02)** | Controller `CriarNovaVersao` (lines 177–178), `GetMaxVersaoForPacienteAsync` | Understanding who queries persistence and who validates the rule is critical before changing version logic. |
| 2 | **Complete Snapshot Evolution (D-05)** | `CriarNovaVersao` calls `AplicarCriacao`; repository test `D05_EvolutionDoesNotMergeCollections` | If you add merge-from-predecessor logic, you break clinical integrity. The pattern is replace, not merge. |
| 3 | **Concurrent Evolution 409 (D-06)** | Controller `CriarNovaVersao` catch blocks (lines 199–214) | Two-layer defense: aggregate validation + database unique constraint. Must maintain both. |
| 4 | **Soft delete and `.IgnoreQueryFilters()`** | `GetMaxVersaoForPacienteAsync` | Removing `.IgnoreQueryFilters()` causes duplicate `Versao` assignment after soft delete. The comment in code explains why. |
| 5 | **AutoMapper read-only strategy** | `ProntuarioProfile.cs`, Controller static helpers | Do not reintroduce AutoMapper on write paths. The legacy bugs (Internacao → InformacoesExtras cross-mapping) came from blind mapping. |

### Advanced — Extension, hardening, cross-feature implications

| # | Topic | Where | Why it matters |
|---|-------|-------|----------------|
| 1 | **Workflow SDD consumption of Prontuario** | `REQ-016` — fields queryable by `AtendimentoId` with includes | When `atendimento-workflow-stabilization` reads prontuario data, it must use existing read paths — not aggregate reconstruction. |
| 2 | **Patient-wide vs atendimento-scoped versioning (D-03)** | `design.md` D-03, DQ-010 | If product wants atendimento-scoped version chains in the future, this SDD documents the migration path and current tradeoffs. |
| 3 | **CID Catalog Management integration** | Controller CID validation, synthetic CID in tests | When production CID catalog is implemented, replace synthetic CID seed patterns with catalog lookups. Controller validation paths are ready. |
| 4 | **WS07 frontend alignment** | Guid IDs, dual verbs (PUT/POST /versoes), `/atual` routes, `Tipo` as int | All documented in `design.md` API Contract table. Frontend must send `AtendimentoId` and `PacienteId` at root on create. |
| 5 | **Correction child mutation policy (future)** | `design.md` Correction child mutation policy | Currently PUT correction never mutates child collections. If product requires CD/Exame corrections without evolution, this policy must be re-evaluated with ADR consideration. |

---

## 10. Common Pitfalls

### Pitfall 1: Adding an evolution-only field to `UpdateProntuarioDto`

**What commonly breaks:** A developer adds `Tipo`, `DataConsulta`, or a clinical section to `UpdateProntuarioDto` for a "quick fix." The field is now accepted on PUT, blurring the correction vs evolution boundary. Clinical changes arrive via PUT and the version history is lost.

**Why it breaks:** `UpdateProntuarioDto` is the structural enforcement of D-01. The DTO type definition — not runtime validation — is the primary guard. Adding a property to the class immediately opens the API contract to accept it on PUT.

**How to avoid it:** Before adding any property to any Prontuario DTO, consult `design.md` Field Classification. Immutable → never on any write DTO. Correction-safe → `UpdateProntuarioDto` only. Evolution-only → `CreateProntuarioDto` / `CreateVersaoProntuarioDto` only. When in doubt, prefer evolution — it preserves clinical history.

---

### Pitfall 2: Removing `.IgnoreQueryFilters()` from `GetMaxVersaoForPacienteAsync`

**What commonly breaks:** A developer sees `.IgnoreQueryFilters()` and removes it as "cleanup" because it looks like a debugging leftover. After soft-deleting V1, the next evolution attempt creates a duplicate `Versao = 2` because `MAX(Versao)` returned 0 (V1 hidden by filter) instead of 1 (occupied slot).

**Why it breaks:** Soft-deleted versions still occupy their `Versao` slot in the patient-wide sequence. If `GetMaxVersaoForPacienteAsync` ignores deleted rows, the computed `nextVersao` collides with an existing (deleted) version's slot, or worse, reuses a version number still visible in historical queries.

**How to avoid it:** The code comment on `GetMaxVersaoForPacienteAsync` explains the rationale. When touching soft-delete or version lookup logic, run `SoftDelete_ExcludesFromReads_ButKeepsInMaxVersao` — it asserts `MaxVersao` returns 2 after V1 is soft-deleted with V2 still active. The `.clinerules/ef-migrations.md` rule requires documenting `.IgnoreQueryFilters()` usage.

---

### Pitfall 3: Implementing merge-from-predecessor in `CriarNovaVersao`

**What commonly breaks:** A developer thinks "evolution should carry forward the predecessor's exames unless explicitly changed" and adds merge logic to `CriarNovaVersao`. V2 now contains V1's old exames plus the new ones — the clinician who removed an exam from the evolution payload still sees it in V2.

**Why it breaks:** D-05 specifies **complete snapshot replace** semantics. Each version is a clinical picture at a point in time. Merging collections from the predecessor silently reintroduces data the clinician intended to remove. This is a clinical safety issue, not just a data issue.

**How to avoid it:** `CriarNovaVersao` calls `AplicarCriacao` with the evolution payload as-is — no merge step. The test `D05_EvolutionDoesNotMergeCollections` asserts V2 has exactly what the evolution payload supplied. If product requirements later demand delta semantics, that's a new ADR and a separate API operation — never silent merge on the existing `CriarNovaVersao`.

---

### Pitfall 4: Adding AutoMapper back to write paths

**What commonly breaks:** A developer sees the 8 static mapping helpers in the controller and thinks "this is boilerplate, AutoMapper can do this in 2 lines." They add `CreateMap<CreateProntuarioDto, Prontuario>` to `ProntuarioProfile`. Fields silently fail to map, child collections are constructed wrong, and Internacao cross-maps into `InformacoesExtras` again.

**Why it breaks:** The legacy `ProntuarioProfile` had known critical bugs because `Prontuario` uses private setters and AutoMapper cannot invoke `AplicarCriacao` or `AplicarCorrecao`. The controller's static helpers explicitly construct domain objects and call domain methods — they are the auditable write mapping. AutoMapper is restricted to read projection only.

**How to avoid it:** `ProntuarioProfile.cs` is annotated as read-only. The controller helpers are `private static` and named `MapDtoTo*`. Before adding a mapping to the profile, check: is this entity → DTO (read) or DTO → entity (write)? Write mappings belong in the controller helpers, not AutoMapper.

---

### Pitfall 5: Forgetting to add `Include` for a new navigation in `QueryWithIncludes`

**What commonly breaks:** A developer adds a new child collection or navigation property to `Prontuario` and corresponding fields to `ReadProntuarioDto`. The AutoMapper profile maps the new navigation. But when reading via `GetByIdAsync`, the new collection is always null or throws a lazy-loading exception.

**Why it breaks:** EF Core does not lazy-load by default with the project's configuration. All navigations needed for read projection must be eagerly loaded via `Include` / `ThenInclude` in `QueryWithIncludes`. The repository method is the single point of eager loading — new navigations need an `Include` added there.

**How to avoid it:** When adding a navigation to `Prontuario`, update `QueryWithIncludes` in `ProntuarioRepository.cs`. Run `ProntuarioSqlIntegrationTests` — the full fixture chain will fail if a navigation needed for `ReadProntuarioDto` projection is missing an Include.

---

## 11. Feature Evolution History

### Contract changes

| Before (Legacy / Stub) | After (Stabilized) | Rationale |
|------------------------|---------------------|-----------|
| `PUT /Prontuario/{id}` always in-place, blind full replace | PUT = correction-only (4 fields, same Id/Versao) | ADR-006: correction and evolution are distinct clinical operations |
| No versioning on save | `POST /Prontuario/{id}/versoes` creates successor | Domain requirement: clinical evolution produces immutable snapshots |
| No `/atual` endpoints | `GET .../paciente/{id}/atual`, `GET .../atendimento/{id}/atual` | Common clinical query: "show me the latest" |
| String IDs throughout API | Guid on all routes and DTOs | Paciente/Atendimento precedent; type safety |
| `Tipo` as `string?` in DTOs | `Tipo` as `int` in all DTOs | Canonical type matching entity and database |
| `PacienteId` nested in `DescricaoBasica` (frontend) | `PacienteId` at root on `CreateProntuarioDto` | FK at aggregate root level |
| `AtendimentoId` absent from API contract | `AtendimentoId` at root on `CreateProntuarioDto` (required) | Schema required it; controller validates before create |
| `POST /Prontuario/from-pdf` with dead service | Returns **501** immediately | PDF extractor commented; out of scope |
| `Console.WriteLine` with patient data | Generic error messages only | PHI compliance (REQ-012) |

### Architecture refinements

| Before | After | Rationale |
|--------|-------|-----------|
| AutoMapper on all create/update paths | AutoMapper read-only; explicit static helpers on write | Known critical bugs in legacy `ProntuarioProfile` (Internacao → InformacoesExtras cross-mapping, private setters) |
| Repository stub (all `NotImplementedException`) | Full SQL CRUD + version lookup + includes | Third SQL vertical |
| No domain methods on Prontuario | `AplicarCriacao`, `AplicarCorrecao`, `CriarNovaVersao`, `MarcarComoExcluido` | Rich domain model (Research Decision 6) |
| No version generation policy | D-02: Application obtains max(Versao); aggregate validates rule | Separation of persistence fact from business rule |
| No concurrency handling | Two-layer D-06: aggregate validation + unique index → **409** | Concurrent evolution is expected; must not 500 |

### Scope decisions

| Deferred | Why | Future owner |
|----------|-----|--------------|
| WS07 Blazor frontend | Backend Stabilization Rule — verify backend first | WS07 |
| Atendimento Workflow (`ValidacaoEtapa*`) | Requires Prontuario + Agendamento verified first | `atendimento-workflow-stabilization` |
| CID catalog in production | CID table exists but no production seed data | CID Catalog Management (future PM item) |
| Auth/RBAC (`AtualizadoPor`) | Out of scope for all clinical SQL slices | Future auth feature |
| New EF migrations | `InitialCreate` schema suffices; `(PacienteId, Versao)` index already present | Not required for this slice |
| PDF import/reports | Extractor commented; model undefined | Future PDF ingestion feature |

### Important tradeoffs

| Tradeoff | Accepted weakness | Rationale |
|----------|-------------------|-----------|
| Patient-wide `Versao` numbering | Version counter spans all atendimentos for a patient; not per-atendimento restart | Matches `(PacienteId, Versao)` unique index; avoids migration; documented as DQ-010 future consideration |
| Correction never mutates child collections | Cannot fix an Exame typo or remove an AcoesCD item without evolution | Keeps correction narrowly scoped to admin/demographic fields; clinical changes require explicit evolution |
| `UpdateProntuarioDto` structural exclusion | Cannot add a "quick correction" field without DTO change | Compile-time enforcement of D-01; prevents accidental clinical edits via PUT |
| `GetMaxVersaoForPacienteAsync` ignores query filters | Soft-deleted slots permanently occupy version numbers | Prevents version number reuse; documented in code per `.clinerules/ef-migrations.md` |
| `DbUpdateException` message matching for 409 | Fragile — depends on SQL Server error message text | No EF Core 7 built-in unique constraint violation type; acceptable for MVP2 with two-layer defense (aggregate validation is the primary guard) |

### Security remediation

| Before | After |
|--------|-------|
| `Console.WriteLine($"CPF: {cpf}")` and similar in controller | All `Console.WriteLine` removed; generic error messages ("Not Found", "CID não encontrado") |
| `ex.Message` returned to client on errors | Structured status codes; no exception details in responses |
| PDF endpoint processed files through commented service | `POST /from-pdf` returns 501 with no processing |

---

## 12. Suggested Next Feature

**Agendamento SQL Migration & Stabilization** is the logical next vertical slice per `migration-sql.md` sequence: Paciente → Atendimento Minimal → Prontuario → **Agendamento** → Atendimento Workflow.

Concepts from this feature that Agendamento will reuse:

- **Repository-direct SQL integration pattern** — `SqlConnectionResolver`, `SqlIntegrationTestGate`, upstream fixture reuse (PacienteRepository, AtendimentoRepository, now also ProntuarioRepository).
- **Guid end-to-end on repository interface and API** — now the standard across all three verified aggregates.
- **PHI-safe controller pattern** — no `Console.WriteLine`, generic error messages.
- **AutoMapper read-only strategy** — explicit DTO mapping on write paths.

Concepts Agendamento will expand:

- **Workflow integration** — Agendamento is the first aggregate that `atendimento-workflow-stabilization` will consume for scheduling-driven stage evaluation.
- **Date/time logic** — Scheduling constraints not present in Prontuario's versioning-focused design.

---

## 13. Quick Reference — Code Reading Order

Follow this sequence to build understanding from domain rules outward:

| # | File | Rationale | Enables understanding of |
|---|------|-----------|--------------------------|
| 1 | `DocAPI/Core/Entities/Prontuario.cs` | Domain behavior first: understand what the aggregate owns and enforces before seeing how it's persisted or served | `AplicarCriacao`, `AplicarCorrecao`, `CriarNovaVersao`, `MarcarComoExcluido` — the four domain methods |
| 2 | `DocAPI/Core/Entities/Exame.cs` + `Internacao.cs` | Child entities: understand the shape of collections the aggregate owns | How child collections are constructed and linked to parent |
| 3 | `DocAPI/Application/Data/Dtos/Prontuario/CreateProntuarioDto.cs` | DTO shapes: understand what the API accepts and the D-01 structural correction contract | Why `UpdateProntuarioDto` has only 4 fields |
| 4 | `DocAPI/Application/Data/Dtos/Prontuario/ReadProntuarioDto.cs` | Read projection shape: understand what the API returns | Version metadata surfaced to clients |
| 5 | `DocAPI/Application/Mappings/Profiles/ProntuarioProfile.cs` | Read-only AutoMapper: understand entity → DTO projection | Why AutoMapper is restricted to reads |
| 6 | `DocAPI/Core/Interfaces/Repositories/IProntuarioRepository.cs` | Repository contract: understand the persistence surface | Method signatures with XML doc comments explaining D-02 and D-01 roles |
| 7 | `DocAPI/Infrastructure/Repositories/ProntuarioRepository.cs` | Repository implementation: understand SQL persistence, includes, soft delete, and `.IgnoreQueryFilters()` | How `QueryWithIncludes` works, why `GetMaxVersaoForPacienteAsync` ignores filters |
| 8 | `DocAPI/API/Controllers/ProntuarioController.cs` | Controller orchestration: understand the full HTTP flow — FK/CID validation, D-02 version lookup + aggregate invocation, D-06 concurrency mapping | How all previous layers connect at the API boundary |
| 9 | `DocAPI.Tests/Infrastructure/ProntuarioRepositoryTests.cs` | Unit tests: understand expected domain behavior through tests | D-05 no-merge proof, soft-delete slot preservation, version chain integrity |
| 10 | `DocAPI.Tests/Controllers/ProntuarioControllerTests.cs` | Controller tests: understand HTTP contract — all status codes, FK/CID validation, concurrency | How the API behaves for every documented scenario |
| 11 | `DocAPI.Tests/Integration/ProntuarioSqlIntegrationTests.cs` | SQL integration: understand the full fixture chain against real SQL Server | Proof that EF mapping, FK constraints, unique index, and soft-delete filter work end-to-end |
| 12 | `Documentation/Architecture/ADR/ADR-006-prontuario-dual-mode-versioning-api.md` | ADR: understand the durable decision behind the dual-mode API | Why PUT = correction and POST /versoes = evolution |

**Pilot lesson:** Reading the controller first is tempting but hides the domain decisions that explain most stabilization bugs (D-02 version split, D-05 replace semantics, D-01 structural contract). Start with the aggregate — the controller is orchestration of domain concepts, not the source of truth.

---

## 14. Related Artifacts

| Artifact | Path | Role |
|----------|------|------|
| **Specify** | `Documentation/SDD/prontuario-sql-stabilization/specify.md` | Feature requirements and acceptance criteria |
| **Design** | `Documentation/SDD/prontuario-sql-stabilization/design.md` | Architecture design, field classification D-01–D-06, API contract |
| **Tasks** | `Documentation/SDD/prontuario-sql-stabilization/tasks.md` | Implementation task list with requirement traceability |
| **Research** | `Documentation/SDD/prontuario-sql-stabilization/research.md` | Feature discovery, legacy characterization, research decisions |
| **Verification** | `Documentation/SDD/prontuario-sql-stabilization/verification.md` | Verification gates, findings (F-01), requirement traceability evidence |
| **Feature Report** | `Documentation/SDD/prontuario-sql-stabilization/reports/feature-report.md` | Delivery summary, lessons learned, Teacher Guide decision |
| **Pre-Execution Review** | `Documentation/SDD/prontuario-sql-stabilization/reports/pre-execution-review-2026-06-19.md` | ADR-006 acceptance, Execute authorization |
| **ADR-001** | `Documentation/Architecture/ADR/ADR-001-soft-delete.md` | Soft delete policy |
| **ADR-006** | `Documentation/Architecture/ADR/ADR-006-prontuario-dual-mode-versioning-api.md` | Dual-mode versioning API policy |
| **Domain Overview** | `Documentation/Architecture/Domain_Overview_Business_Rules.md` | Domain rules for Prontuario versioning |
| **Migration SQL** | `Documentation/Technical/migration-sql.md` | Migration checklist and vertical sequencing |
| **State** | `Documentation/State.md` | Current operational truth |
| **Paciente Teacher Guide** | `Documentation/SDD/paciente-sql-stabilization/teacher-guide.md` | Calibration reference for the Teacher Guide format |
| **Atendimento Minimal SDD** | `Documentation/SDD/atendimento-minimal-sql-stabilization/` | Verified prerequisite — Atendimento FK consumed by Prontuario |