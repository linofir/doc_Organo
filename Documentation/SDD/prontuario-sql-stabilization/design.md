# Design — Prontuario SQL Stabilization

> Input: `specify.md` in `Documentation/SDD/prontuario-sql-stabilization/`  
> Generated SDD artifacts are written in English.

## Design Summary

This feature replaces the stubbed `ProntuarioRepository` with SQL persistence for a **composite clinical snapshot** aggregate: owned value objects plus nested Exames, ProntuarioAcaoCD, and optional Internacao with Procedimentos. The API exposes **explicit versioning operations** — correction via `PUT` (in-place, same `Id` and `Versao`) and clinical evolution via `POST /Prontuario/{id}/versoes` (new row, patient-wide incremented `Versao`). The backend **never infers** evolution from payload diffs; the client chooses the operation.

After implementation, callers can create v1 prontuarios against verified Atendimento FKs, list full patient version history, read latest by patient or atendimento, correct eligible fields in place, evolve clinical state through explicit successor creation, and soft-delete individual versions. The **aggregate owns the versioning rule** and validates assigned version numbers; the **application layer obtains** the next version from persistence; the **repository persists** assigned values and performs lookup queries only — it does not decide business version policy. AutoMapper is removed from create/update/evolution paths; dedicated DTOs and explicit read projection replace unsafe legacy maps.

**Workflow logic, evaluators, and pendências are out of scope.** Persistence must remain queryable by `AtendimentoId` with nested children loaded through repository/API read paths so `atendimento-workflow-stabilization` can consume data later without aggregate reconstruction helpers.

Execute chooses local implementation details (method names, exception types, projection helpers) as long as outcomes satisfy `specify.md` and this design.

## Requirement Mapping

| Requirement | Design response |
|-------------|-----------------|
| `REQ-001` | Domain factory for v1 create: accepts `PacienteId`, `AtendimentoId`, clinical payload; sets `Versao = 1`, `ProntuarioAnteriorId = null`; snapshots identity fields into `DescricaoBasica` from `Paciente` where applicable; repository persists in single transaction with children |
| `REQ-002` | Controller/Application validates Atendimento via `IAtendimentoRepository` (or equivalent lookup) before create: exists, not soft-deleted, `PacienteId` match → **404** on failure (**DQ-008**) |
| `REQ-003` | `GetByIdAsync(Guid)` with includes for nested graph; global soft-delete filter; project to `ReadProntuarioDto` |
| `REQ-004` | `GetByPacienteIdAsync(Guid)` ordered `Versao DESC`, then `CriadoEm DESC`; all non-deleted versions |
| `REQ-005` | `GetLatestByPacienteIdAsync` / `GetLatestByAtendimentoIdAsync` — max `Versao` among non-deleted (tie-breaker `CriadoEm DESC`) |
| `REQ-006` | `UpdateProntuarioDto` exposes **correction-safe fields only** (structural contract — **D-01**); `AplicarCorrecao` on aggregate; no runtime field-diff against persisted state; **204** / **404** |
| `REQ-007` | Application obtains next `Versao` from repository persistence lookup (`GetMaxVersaoForPacienteAsync` or equivalent); aggregate `CriarNovaVersao(source, nextVersao, payload)` validates rule and assigns identity; repository inserts new graph; prior row unchanged; **201** + DTO; concurrent create → **409** (**D-06**) |
| `REQ-008` | Entity soft-delete method per version; ADR-001 filter; other versions unaffected |
| `REQ-009` | Create/evolution persist full nested graph; evolution uses **replace** for Exames, AcoesCD, Internacao on new version row; optional Internacao omitted when not in payload |
| `REQ-010` | When Internacao section present: validate required columns; lookup CID exists → **400/404** before save |
| `REQ-011` | `IProntuarioRepository` Guid contract; slim dedicated DTOs; remove domain types from public API surface |
| `REQ-012` | Remove PHI logging from `ProntuarioController`; structured safe logging only |
| `REQ-013` | `ProntuarioSqlIntegrationTests` — full fixture chain with synthetic CID seed in setup |
| `REQ-014` | Legacy table in `specify.md` — design adds field classification and API semantics |
| `REQ-015` | Controller maps domain/repository outcomes to Paciente-aligned status codes |
| `REQ-016` | Repository read methods expose flat queryable shape: `AtendimentoId`, `Tipo`, `DataConsulta`, CD collection, Internacao navigation via EF includes — no workflow services |
| `REQ-017` | No Application workflow services, evaluators, or pendência writes in this slice — scope boundary for Pre-Execution Review |
| `REQ-018` | DTOs and API expose `Tipo` as **int**; no string/int ambiguity |

## Field Classification (Specify Design Follow-Up)

Classification governs correction vs evolution. **`UpdateProntuarioDto` contains only correction-safe fields** — evolution-only fields are **absent from the contract** (**D-01**). Validation is **structural** (DTO shape / model binding), not value-comparison against persisted state. **POST /versoes** accepts full clinical payload for the successor version (complete snapshot — **D-05**).

### Correction DTO contract (D-01)

- `UpdateProntuarioDto` **must not** declare evolution-only properties (`Tipo`, `DataConsulta`, `AGO`, collections, etc.).
- Clients cannot send evolution-only fields on PUT — they are not part of the API contract.
- **No runtime field-diff analysis** is required on correction.
- If a client bypasses the contract (e.g. raw JSON with extra properties), API model binding or explicit contract validation may reject unknown members — **400** — but the primary guard is **structural**, not comparing old vs new values.

### Immutable (never change after row is persisted)

| Field / group | Rationale |
|---------------|-----------|
| `Id` | Row identity |
| `PacienteId` | Aggregate ownership |
| `Versao` | Version metadata — correction does not increment |
| `ProntuarioAnteriorId` | Chain pointer — set only at evolution create |
| `CriadoEm` | Audit origin |
| `AtendimentoId` | Journey link for this version row — copied from source on evolution; not reassignable in MVP2 |

### Correction-safe (mutable via `PUT /Prontuario/{id}` only)

| Field / group | Rationale |
|---------------|-----------|
| `InformacoesExtras` | Administrative / non-clinical notes; typo fixes |
| `DescricaoBasica` — identity snapshot fields (`Profissao`, `Religiao`, `AtividadeFisica`) | Demographic snapshot corrections (not clinical progression) |
| `AtualizadoEm` | Set on correction ( `AtualizadoPor` residual risk if unset ) |

### Evolution-only (changes require `POST /Prontuario/{id}/versoes`)

| Field / group | Rationale |
|---------------|-----------|
| `DescricaoBasica.QD` and other clinical narrative in snapshot | Clinical content progression |
| `AGO`, `Antecedentes`, `AntecedentesFamiliares`, `PosOperatorio` | Owned clinical sections |
| `DataConsulta`, `Tipo` | Workflow-readable clinical classification |
| `Exames` collection | Clinical orders evolution |
| `ProntuarioAcaoCD` collection | CD checklist evolution |
| `Internacao` + `ProcedimentoInternacao` | Hospitalization request evolution |

Evolution-only fields appear **only** on `CreateProntuarioDto` and `CreateVersaoProntuarioDto`, never on `UpdateProntuarioDto`.

## Workflow Read Model Readiness (D-04)

Workflow logic remains out of scope (**REQ-017**). Persistence and read projection must still allow future workflow consumers to determine clinical **presence** from stored data and efficient reads — **without** aggregate reconstruction logic or workflow services.

| Presence signal | How persisted / read exposes it (outcomes) |
|-----------------|------------------------------------------|
| Consultation presence | `Tipo`, `DataConsulta`, and/or root row existence for `AtendimentoId` readable via GET by id, list, or `/atual` |
| Hospitalization presence | Optional `Internacao` navigation populated on read when row exists; absence = no internacao for that version |
| Procedure presence | `Internacao.Procedimentos` (or equivalent collection) populated on read when present |
| CD actions presence | `ProntuarioAcaoCD` collection count / items on read |

**Guideline:** Do not hide these signals behind computed domain behavior in this slice. Repository/API reads use EF includes (or equivalent) so workflow SDDs can query by `AtendimentoId` and inspect nested shape directly (**REQ-016**).

## Child Collection Snapshot Semantics (D-05)

Each evolved version is a **complete clinical snapshot**, not a delta.

**Example:**

- **V1** has `Exames = [E1, E2]` and `AcoesCD = [CD1]`.
- Client calls `POST /Prontuario/{v1Id}/versoes` with payload `Exames = [E3]` only (full snapshot semantics for supplied sections).
- **V2** persists **exactly** what the evolution payload supplies for collections — **replace**, not merge.
- **V2 does not inherit** `E1`, `E2` from V1 unless the client includes them in the evolution payload.
- **V1** row remains unchanged with `[E1, E2]`.

Collections are **not delta-based**. Execute must not implement merge-from-predecessor behavior unless Specify scope changes.

## API Contract

### Contract resolutions

| Ambiguity | Resolution |
|-----------|------------|
| Correction vs evolution | Client selects HTTP verb/route; backend never infers from payload |
| Correction DTO shape | **`UpdateProntuarioDto` — correction-safe fields only**; evolution-only properties absent (**D-01**) |
| POST create response | **201 + `ReadProntuarioDto`** (Paciente pattern) |
| POST `/versoes` response | **201 + `ReadProntuarioDto`** with new `Id` |
| PUT correction response | **204** — no body |
| Invalid / missing `AtendimentoId` on create | **404** (**DQ-008**) |
| Unknown CID when Internacao present | **400** or **404** — not **500** |
| `AtendimentoId` on evolution | Copied from source row; **not** client-reassignable in MVP2 |
| Paginated list | `GET /Prontuario?skip&take` — all non-deleted versions (not patient-scoped); existing contract preserved |
| `/from-pdf` | Route may remain; returns **501** or **404** — no implementation (Out of Scope) |

### Endpoints

| Method | Route | Request | Response | Notes |
|--------|-------|---------|----------|-------|
| POST | `/Prontuario` | `CreateProntuarioDto` | 201 + `ReadProntuarioDto` | Requires root `PacienteId`, `AtendimentoId` (Guid); full initial payload |
| GET | `/Prontuario/{id}` | — | 200 + DTO / 404 | Specific version |
| GET | `/Prontuario/paciente/{pacienteId}` | — | 200 + array | All versions; **Versao DESC** |
| GET | `/Prontuario/paciente/{pacienteId}/atual` | — | 200 + DTO / 404 | Max `Versao` for patient |
| GET | `/Prontuario/atendimento/{atendimentoId}/atual` | — | 200 + DTO / 404 | Max `Versao` for atendimento |
| GET | `/Prontuario` | `skip`, `take` | 200 + array | Paginated global list |
| PUT | `/Prontuario/{id}` | `UpdateProntuarioDto` (correction-safe only) | 204 / 404 / **400** | Structural contract — no evolution-only properties |
| POST | `/Prontuario/{id}/versoes` | `CreateVersaoProntuarioDto` (same shape as create minus FKs) | 201 + DTO / 404 / 400 | Clinical evolution |
| DELETE | `/Prontuario/{id}` | — | 204 / 404 | Soft delete single version |
| POST | `/Prontuario/from-pdf` | — | 501 | Out of Scope — not implemented |

### DTO policy (outcomes)

| DTO | Must include | Must not include |
|-----|--------------|------------------|
| `CreateProntuarioDto` | `PacienteId`, `AtendimentoId`, clinical sections, `Tipo` (int), `DataConsulta` | Domain entity types; nested `PacienteId` only in deprecated nested DTO paths |
| `UpdateProntuarioDto` | Correction-safe fields **only** (`InformacoesExtras`; allowed `DescricaoBasica` snapshot fields per classification) | **Any evolution-only property** — absent from type definition (**D-01**) |
| `CreateVersaoProntuarioDto` | Full clinical payload for successor (complete snapshot) | `PacienteId` / `AtendimentoId` (from source row) |
| `ReadProntuarioDto` | `Id`, `PacienteId`, `AtendimentoId`, `Versao`, `ProntuarioAnteriorId`, nested read models, `Tipo` int | Domain entity types |

### Status code parity

| Condition | Code |
|-----------|------|
| Success create / evolution | 201 |
| Success read / list | 200 |
| Success correction / delete | 204 |
| Missing resource / FK miss | 404 |
| Validation / bad CID / unknown JSON members on PUT | 400 |
| Concurrent evolution — `(PacienteId, Versao)` unique violation | **409 Conflict** (**D-06**) |
| Unhandled persistence conflict (other) | 409 (if applicable) |

## Affected Components

| Component | Path | Responsibility | Expected change |
|-----------|------|----------------|-----------------|
| Prontuario aggregate | `DocAPI/Core/Entities/Prontuario.cs` (+ owned types, children) | Version rules, factories, correction, evolution, soft delete | Add domain methods; child ownership on new version |
| Child entities | `Exame`, `Internacao`, `ProntuarioAcaoCD`, `ProcedimentoInternacao` | Nested persistence | Created through aggregate; not AutoMapper-mapped on write |
| Repository interface | `DocAPI/Core/Interfaces/Repositories/IProntuarioRepository.cs` | Persistence contract | **Guid** params; latest-query methods; **`GetMaxVersaoForPacienteAsync`** (persistence lookup only); remove string IDs |
| Prontuario repository | `DocAPI/Infrastructure/Repositories/ProntuarioRepository.cs` | SQL graph persistence | Replace stub; transactions; includes; **lookup only for max Versao — no business policy** |
| Controller / Application | `ProntuarioController` + application orchestration | HTTP, FK validation, version lookup orchestration, PHI | Guid routes; obtain `nextVersao` from repository then invoke aggregate; map **409** on unique conflict |
| DTOs | `DocAPI/Application/Data/Dtos/Prontuario/` (or equivalent) | API contracts | Dedicated read/write DTOs; int `Tipo` |
| AutoMapper profile | `DocAPI/Application/Mappings/Profiles/ProntuarioProfile.cs` | Legacy maps | **Remove** create/update/evolution maps; optional read-only helpers deprecated in favor of explicit projection |
| EF configurations | `ProntuarioConfig`, child configs | Mapping | Review only — `InitialCreate` stable |
| DbContext | `DocDbContext` | Filters, sets | Verify Prontuario soft-delete filter |
| Unit tests | `DocAPI.Tests/` | Domain + repository | Factory, versioning, correction guard, soft delete |
| SQL integration | `DocAPI.Tests/Integration/` | Docker chain | `ProntuarioSqlIntegrationTests` + CID seed helper |
| Legacy reference | `ProntuarioSheetsRepository.cs` | Behavior evidence | Read-only targeted methods |

**Explicitly not changed:** `AtendimentoRepository`, workflow Application services, `DocFront.Web/*`.

## Reuse Analysis

| Existing code or pattern | Reuse decision | Notes |
|--------------------------|----------------|-------|
| `PacienteRepository` / `AtendimentoRepository` SQL patterns | Reuse pattern | DocDbContext, soft delete, Guid, transactions |
| `Paciente` factory / soft delete | Reuse pattern | `MarcarComoExcluido`, apply update methods |
| `AtendimentoMinimal` FK validation in Controller | Reuse pattern | Lookup before create → 404 |
| `PacienteSqlIntegrationTests` / `AtendimentoSqlIntegrationTests` | Reuse pattern | Skip policy, `SqlConnectionResolver`, synthetic data |
| Credential Probe / `sql-integration-test.ps1` | Reuse | From Atendimento specify/design |
| Verified `POST /Atendimento` in fixtures | Reuse | Prerequisite — do not re-implement |
| `ProntuarioProfile` create/update maps | **Do not reuse** | Known critical bugs |
| Domain types in DTOs | **Do not reuse** | Abandon |
| String IDs on `IProntuarioRepository` | **Do not reuse** | Guid migration |
| In-place PUT as only update (legacy) | **Do not reuse** as sole path | Split correction vs evolution |

## Architecture And Ownership

- **Architecture impact:** Implements third SQL vertical within existing repository + aggregate pattern; dual-mode versioning API governed by **[ADR-006](../../Architecture/ADR/ADR-006-prontuario-dual-mode-versioning-api.md)**.
- **Bounded context:** Clinical — Prontuario aggregate root owns snapshot versioning; Atendimento is upstream FK only.
- **Ownership boundaries:**

| Responsibility | Owner |
|----------------|-------|
| Versioning **rule** (validates assigned number, chain integrity) | **Prontuario aggregate** |
| Next version **persistence lookup** (`max(Versao)` for `PacienteId`) | **Application layer** via repository query (**D-02**) |
| Correction vs evolution routing | **Controller/Application** |
| Graph persistence, transactions | **Repository** |
| Atendimento FK validation on create | **Controller/Application** |
| HTTP semantics, PHI, **409** on version conflict | **Controller** |
| Workflow evaluators / pendências | **atendimento-workflow-stabilization** (out of scope) |

- **Conflicts:** ADR-001 overrides hard delete; Specify versioning rules override legacy in-place-only updates for clinical evolution.

### Version generation (D-02)

Separation of **rule ownership** vs **persistence lookup**:

```text
Application (Controller / use case)
  → Repository.GetMaxVersaoForPacienteAsync(pacienteId)   // persistence lookup only
  → nextVersao = max + 1 (or 1 if none)
  → Aggregate.CriarNovaVersao(source, nextVersao, payload) // validates rule + assigns identity
  → Repository.AddAsync(newVersionGraph)                   // persists assigned Versao
```

| Step | Owner | Outcome |
|------|-------|---------|
| First prontuario for patient | Aggregate factory | `Versao = 1` (no lookup required) |
| Evolution — lookup | **Application** + repository query | Obtains current max from persistence |
| Evolution — rule | **Aggregate** `CriarNovaVersao` | Validates `nextVersao`; assigns new `Id`, `ProntuarioAnteriorId`; copies `AtendimentoId` from source |
| Evolution — persist | Repository | Inserts graph with aggregate-assigned `Versao` |
| Correction | Aggregate `AplicarCorrecao` | `Versao` unchanged |
| Concurrent evolution | Repository + DB unique index | Second writer fails → **409 Conflict** (**D-06**) |

The aggregate **must not** query the database. The repository **must not** decide business version policy — it only returns persistence facts (e.g. max existing `Versao`).

### Patient-wide lineage — future consideration (DQ-010, D-03)

| Field | Decision |
|-------|----------|
| **DQ-010** | Should version lineage remain patient-wide or become atendimento-scoped? |
| **Current decision** | **Patient-wide** versioning (`Versao` scoped to `PacienteId`) |
| **Reason now** | Domain decision in Specify; matches existing `(PacienteId, Versao)` unique index; avoids migration |
| **Future consideration** | Clinical lineage visualization may benefit from **atendimento-scoped** version chains (separate lines for Atendimento A vs B under the same patient). Implementation unchanged in this SDD — document only. |

**Example (patient-wide numbering today):**

```text
Paciente P
  Atendimento A → Prontuario V1 (Versao=1)
                 → Prontuario V2 (Versao=2)   // evolution from V1
  Atendimento B → Prontuario V3 (Versao=3)   // new journey; Versao continues patient-wide
```

Atendimento-scoped **latest** reads (`GET .../atendimento/{id}/atual`) still work; **version numbers** do not restart per atendimento in MVP2.

### Repository responsibility (persistence-only)

| Responsibility | Repository | Not repository |
|----------------|------------|----------------|
| Insert/update/delete SQL graph | Yes | |
| EF includes for reads | Yes | |
| Transaction boundaries | Yes | |
| `GetMaxVersaoForPacienteAsync` (persistence fact) | Yes | Returns max stored `Versao` — **not** a business policy decision |
| Business version policy / validation | **No** | Aggregate |
| Atendimento FK validation | **No** | Controller/Application |
| Structural correction DTO enforcement | **No** | DTO type + model binding |
| Workflow reads across aggregates | **No** | Future workflow SDD |

## Data And Persistence

- **Entities:** `Prontuario` root; owned `DescricaoBasica`, `AGO`, `Antecedentes`, `AntecedentesFamiliares`, `PosOp`; children `Exame`, `ProntuarioAcaoCD`, `Internacao`, `ProcedimentoInternacao`
- **Migration impact:** **None expected** — patient-wide `(PacienteId, Versao)` unique index per Specify **DQ-007 A**
- **DTO / entity namespace check:** DTOs must not shadow entity names without explicit namespaces in any remaining maps
- **Write semantics:**
  - **Create v1:** Single transaction — insert parent + children
  - **Correction:** Update owned columns in place; update correction-safe scalars; do not touch evolution-only columns if unchanged; child evolution-only tables unchanged unless explicitly in correction scope (default: no child mutation on correction except none — corrections are scalar/admin only per classification)
  - **Evolution:** Insert new parent row + **complete snapshot** child graph from payload (**replace**, not merge from predecessor — **D-05**); prior row untouched
- **Read semantics:** `Include` / `ThenInclude` for nested graph; list queries use `OrderByDescending Versao`; latest queries use filtered max version subquery or ordered first
- **Soft delete:** Parent `Deletado` flag per ADR-001; child rows on soft-deleted version remain in DB but parent hidden from default queries — document child visibility (predecessor versions' children remain with their version row)
- **CID:** Test fixtures insert synthetic `CID` row; production catalog out of scope
- **Synthetic test data only**

### Concurrency (D-06)

Unique index `(PacienteId, Versao)` prevents duplicate version numbers.

| Scenario | Expected behavior |
|----------|-------------------|
| Two clients evolve concurrently from the same source version | Both may read the same max `Versao`; first insert succeeds; second hits unique constraint |
| Second writer | Surface as **409 Conflict** (or equivalent domain/application error mapped to 409) — not **500** |
| Client recovery | Client refreshes list/`/atual` and retries evolution if clinically appropriate |

Version creation **must tolerate** unique constraint violations as an expected concurrency outcome.

### Correction child mutation policy

Default MVP2: **PUT correction does not mutate child collections** (Exames, AcoesCD, Internacao). Only correction-safe scalar/VO fields listed above. Clinical or child changes require **POST /versoes**. This keeps correction narrowly scoped and reduces accidental in-place clinical edits.

## Frontend Impact

Backend Stabilization — **no frontend work**.

| Area | Impact |
|------|--------|
| Pages/components | None |
| Services/state | None |
| UI smoke | None |
| WS07 follow-up | Guid IDs; root FKs; PUT vs POST `/versoes`; `/atual` routes; `Tipo` int |

## Security And PHI Design

- **Security-sensitive:** Yes — rich clinical PHI in payloads and owned columns
- **Review prompt expected:** Yes — `security-phi-review.md`
- **Mitigations:**
  - Remove all `Console.WriteLine` from `ProntuarioController`
  - Do not log payload bodies or patient names/CPF
  - Generic **404** messages for FK failures — no PHI in error text
  - Synthetic fixtures only in tests
  - `/from-pdf` not invoked (501 / out of scope)

## Legacy Behavior Decision

| Behavior | Source | Preserve / Adapt / Abandon | Design rationale |
|----------|--------|----------------------------|------------------|
| CRUD surface | `ProntuarioSheetsRepository` | Preserve | Guid + dual update paths |
| Multi-section payload | Same | Preserve | Factory + evolution replace |
| In-place sheet update | Same | **Adapt** | **Correction-only** PUT |
| Version on save | Legacy absent | **Adapt** | POST `/versoes` only |
| 3-sheet writes | Same | Adapt | Normalized SQL transaction |
| String IDs | Legacy/API | Abandon | Guid |
| AutoMapper writes | `ProntuarioProfile` | Abandon | Explicit domain + projection |
| PDF import | Controller | Abandon | 501 / out of scope |
| PHI logging | Controller | Abandon | REQ-012 |

## Runtime Validation Environment

| Check | Environment | Expected result | Evidence location |
|-------|-------------|-----------------|-------------------|
| Swagger smoke | DocAPI + Docker SQL | Create, read, list, `/atual`, PUT correction, POST versoes, delete | Session notes; **Verify** owns durable table in `verification.md` |
| SQL integration | Docker + credentials | Full fixture chain **passes** | `dotnet test` output |
| PUT with extra JSON properties (non-contract) | API test | **400** if binding rejects unknown members |
| Concurrent POST `/versoes` | Parallel integration (optional) | One **201**, one **409** | Unit + integration |
| Invalid AtendimentoId | POST create | **404** | Negative test |
| CID negative | POST with Internacao | **400/404** | Integration |

**Runtime validation ownership:** Execute records intent; Verify owns durable HTTP evidence (TASK-008 pattern from Atendimento pilot).

## Optional Test Debt (Forward Migration Verticals)

| Optional test | When to include | Default for this slice |
|---------------|-----------------|------------------------|
| `ProntuarioMappingTests` | Non-trivial read projection | **Recommended** — many DTO sections |
| Soft-deleted AtendimentoId on create → 404 | FK negative | **Recommended** |
| Controller HTTP integration tests | If Verify smoke insufficient | Optional when Swagger checklist covers routes |
| Version chain edge case (two atendimentos, one patient) | Patient-wide versioning | **Recommended** in SQL integration |

## Testing Approach

Approved sequence: domain methods → repository unit tests (InMemory) → SQL integration (full chain) → Swagger smoke intent.

| Requirement | Test or check approach | Notes |
|-------------|------------------------|-------|
| `REQ-001` | Unit factory; integration create v1 | Assert `Versao == 1` |
| `REQ-002` | Negative AtendimentoId → 404 | |
| `REQ-003` | GetById with includes | Version metadata on DTO |
| `REQ-004` | List order Versao DESC | Multi-version fixture |
| `REQ-005` | `/atual` patient + atendimento scopes | |
| `REQ-006` | PUT correction same Id/Versao; `UpdateProntuarioDto` has no evolution-only properties (**D-01**) | |
| `REQ-007` | POST versoes: prior row unchanged; new Versao; concurrent **409** test optional (**D-06**) | Application lookup + aggregate rule (**D-02**) |
| `REQ-008` | Delete one version; others visible | |
| `REQ-009` | Integration nested Exames + Internacao + CD; **evolution omits V1 exames → V2 must not contain them** (**D-05**) | Synthetic CID in setup |
| `REQ-010` | CID negative + positive | |
| `REQ-011` | Compile Guid; no entity in DTO public API | |
| `REQ-012` | security-phi-review | |
| `REQ-013` | **Required** `ProntuarioSqlIntegrationTests` passing | |
| `REQ-014` | Legacy + field classification review | |
| `REQ-015` | Swagger status codes | |
| `REQ-016` | Integration read by `AtendimentoId` without reconstruction helper | |
| `REQ-017` | Scope review at Pre-Execution Review | |
| `REQ-018` | Contract assert `Tipo` int | |

### Integration fixture chain (REQ-013 / REQ-016)

```text
Paciente (test helper)
  → POST /Atendimento (verified contract)
  → POST /Prontuario (v1, nested payload optional)
  → PUT /Prontuario/{id} (correction-safe field)
  → POST /Prontuario/{id}/versoes (v2, nested + optional Internacao + synthetic CID)
  → GET /Prontuario/atendimento/{atendimentoId}/atual
  → assert workflow-readable fields present on DTO/entity read
```

## Verification Handoff Notes

- **Expected gates:** Build, automated tests, SQL/persistence, API Swagger smoke, security-phi-review, domain-review (versioning + legacy table), documentation review
- **Expected review sensors:** `security-phi-review.md`, `domain-review.md`, test-strategy, check-docs
- **Known manual / deferred:** SQL skip = residual risk only; WS07 UI not gated; CID catalog production debt accepted
- **Evidence Execute must provide:** `dotnet build` / `dotnet test`; passing SQL integration preferred; Swagger checklist; field classification honored in code review

## Risks

| Risk | Impact | Mitigation |
|------|--------|------------|
| AutoMapper left on write path | Critical data loss | Remove create/update maps; code review gate |
| Aggregate queries database for Versao | High — layering violation | Application + repository lookup only (**D-02**) |
| Repository decides version policy | High — domain violation | Aggregate validates rule; repository returns facts |
| PUT DTO includes evolution fields | Medium — correction/evolution blur | Structural `UpdateProntuarioDto` (**D-01**) |
| Evolution merge-from-predecessor | High — wrong clinical snapshot | Complete snapshot replace (**D-05**) |
| Composite graph partial write | High | Single transaction per operation |
| Patient-wide versioning confusion | Medium | DQ-010 documented; test two-atendimento scenario (**D-03**) |
| Concurrent `(PacienteId, Versao)` collision | Medium — expected | **409 Conflict** (**D-06**) |
| Scope creep — workflow / PDF | Medium | REQ-017; Out of Scope |
| WS07 breaks on Guid / dual verbs | Medium | Document drift; Backend Stabilization |
| CID missing in prod | Medium | Accepted debt; synthetic tests only |

## ADR Evaluation

| Question | Answer |
|----------|--------|
| Does this change affect durable architecture, persistence, schema lifecycle, security/auth, API contracts, ownership, runtime, or irreversible migration decisions? | **Yes — API contract** (dual-mode versioning: PUT correction vs POST evolution) |
| Existing ADRs referenced | ADR-001 (soft delete), ADR-006 (dual-mode versioning API) |
| New ADR | **[ADR-006](../../Architecture/ADR/ADR-006-prontuario-dual-mode-versioning-api.md)** — accepted 2026-06-19 at SDD Pre-Execution Review (**DQ-009** closed) |
| Version numbering per PacienteId | **No ADR** — domain decision in Specify; **DQ-010** documents future atendimento-scoped consideration (**D-03**) |

## Design Follow-Ups Applied

| ID | Summary | Status |
|----|---------|--------|
| **D-01** | `UpdateProntuarioDto` structural correction contract — no evolution-only fields; no runtime field-diff | Applied |
| **D-02** | Application obtains next `Versao` from persistence; aggregate validates rule; aggregate does not query DB | Applied |
| **D-03** | **DQ-010** patient-wide lineage documented with future atendimento-scoped consideration | Applied |
| **D-04** | Workflow Read Model Readiness — presence signals without reconstruction | Applied |
| **D-05** | Complete snapshot / non-delta collection semantics with example | Applied |
| **D-06** | Concurrent version creation → **409 Conflict** | Applied |

## Documentation Follow-up Candidates

- **State:** Prontuario backend status; test count; next slice Agendamento
- **ADR:** Dual-mode versioning API — **ADR-006 accepted** (Documentation Follow-Up: optional Domain Overview note)
- **Architecture:** Optional Domain Overview note on correction vs evolution qualification
- **Technical:** `migration-sql.md` Prontuario checklist; synthetic CID test note; API contract section when WS06 doc exists
- **Skills:** `sql-migration-workflow` — third vertical confirmation
- **Review prompts:** None unless Verify finds gaps
- **Templates:** Pilot report v0.2 after Tasks + Pre-Execution Review

## References

- `Documentation/SDD/prontuario-sql-stabilization/specify.md`
- `Documentation/SDD/prontuario-sql-stabilization/research.md`
- `Documentation/SDD/atendimento-minimal-sql-stabilization/design.md`
- `Documentation/SDD/paciente-sql-stabilization/design.md`
- `Documentation/Architecture/ADR/ADR-001-soft-delete.md`
- `Documentation/Architecture/ADR/ADR-006-prontuario-dual-mode-versioning-api.md`
- `Documentation/AI-Harness/Harness-Design/sdd-operational.md`
- `DocAPI/Legacy/_LegacySheetsDb/ProntuarioSheetsRepository.cs`
