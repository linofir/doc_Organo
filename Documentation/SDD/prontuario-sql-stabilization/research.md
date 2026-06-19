# Research — Prontuario SQL Stabilization

> Feature SDD: `Documentation/SDD/prontuario-sql-stabilization/`  
> Generated SDD artifacts are written in English.  
> **Phase:** Research (complete — delta reconciled 2026-06-18)  
> **Lifecycle position:** Research → Specify → Design → Tasks → SDD Pre-Execution Review → Execute → Verify → Documentation Follow-Up → Reporting → Teacher Guide

This document consolidates three planning sessions:

1. **Part 1 — Feature Discovery & Scope Decomposition** — initial boundary analysis before Research decisions were approved.
2. **Part 2 — SDD Research Consolidation** — validation of six approved decisions against codebase and governance; authoritative input for Specify.
3. **Part 3 — Research Delta (post Atendimento Minimal Verify)** — reconciles Part 1/2 against verified prerequisite SDD, updated PM/migration sequencing, and stakeholder refinements from the 2026-06-18 session.

When parts conflict, **Part 3 governs** operational status, Atendimento prerequisite ownership, and PM sequencing; **Part 2 governs** Prontuario domain decisions (versioning, CID, mapping, rich domain) unless Part 3 explicitly supersedes them.

---

# Part 1 — Feature Discovery & Scope Decomposition

## Executive Summary

The PM item **WS01 — Prontuario SQL Migration & Stabilization** understates the real boundary. Paciente stabilization verified a single aggregate with flat fields; Prontuario is a **composite clinical snapshot** with owned value objects, three child persistence surfaces (Exames, Internacao + Procedimentos, AcoesCD), a **mandatory Atendimento FK** (Atendimento repository still stubbed), and unresolved tension between **domain versioning** (immutable snapshots) and **legacy/API in-place update** semantics.

**Current state:** Paciente backend verified; Prontuario/Agendamento/Atendimento stubbed; harness calibration complete; Prontuario forward SDD is the intended next slice.

**Recommended boundary:** Backend stabilization SDD covering `ProntuarioRepository` SQL CRUD, nested child persistence, controller/API contract alignment (Guid IDs, PHI-safe logging, status codes), legacy characterization from `ProntuarioSheetsRepository`, and SQL integration tests — following the Paciente pilot pattern and **Backend Stabilization Rule** (WS07 deferred).

**Blocking discovery gaps that Research must resolve before Specify:**

1. **AtendimentoId FK** — schema requires it; DTOs/controllers do not supply it; Atendimento SQL is not implemented.
2. **Versioning vs PUT update** — domain doc says each change creates a new prontuario; legacy and current API use in-place update.
3. **Entity encapsulation** — Prontuario and children use private setters without application methods (unlike stabilized Paciente).
4. **CID reference data** — `Internacao.CIDCodigo` is a required FK; CID table has no seed.
5. **Contract drift** — string IDs in API vs Guid in entity; PacienteId lives in frontend DTO nested under DescricaoBasica, not in backend create DTO.

**Sizing:** Large (equal to or greater than Paciente pilot).

**Discovery readiness (superseded by Part 2):** Ready to enter Research — not ready for Specify until the five blockers above have documented decisions.

---

## Feature Boundary

### In boundary (core SDD)

| Item |
|------|
| `ProntuarioRepository` SQL implementation against existing `InitialCreate` schema |
| EF configs review (`Prontuario`, `Exame`, `Internacao`, `ProntuarioAcaoCD`, `ProcedimentoInternacao`) |
| `ProntuarioController` hardening (PHI, status codes, Guid route params) |
| CRUD + `GET /Prontuario/paciente/{pacienteId}` + paginated list |
| Nested persistence: value objects, Exames, Internacao, AcoesCD |
| Legacy behavior characterization (preserve/adapt/abandon table) |
| Unit + SQL integration tests |
| Swagger smoke checklist |
| Harness forward SDD lifecycle (Specify → Verify → Reporting → Teacher Guide) |

### Out of boundary (follow-up / other workstreams)

| Item |
|------|
| WS07 Blazor integration, UI smoke, frontend contract alignment |
| Full `AtendimentoRepository` migration and `ValidacaoEtapa*` rules |
| PDF report endpoints (commented in controller and legacy) |
| Google Sheets data import / re-enablement |
| Financial aggregates |
| Auth/RBAC (`AtualizadoPor` unset — accepted residual risk pattern) |
| E2E / Blazor tests |
| Feature flag `Persistence:Provider` |
| New EF migrations (unless Research proves schema fix unavoidable) |

### Ambiguous — Research must decide (discovery defaults; see Part 2 for resolutions)

| Item | Discovery default recommendation |
|------|----------------------------------|
| `POST /Prontuario/from-pdf` | Out of scope — `ProntuarioPdfExtractorService` is fully commented; legacy depended on it but service is non-functional on SQL branch |
| Minimal Atendimento provisioning for FK | In scope as supporting requirement — auto-create stub Atendimento on prontuario create, or document ADR to relax FK *(superseded: explicit Create Atendimento → Create Prontuario workflow)* |
| Versioning on update | Adapt for MVP2 — preserve legacy in-place PUT; defer true versioning to WS02 follow-up *(superseded: versioning IN SCOPE per approved Research decisions)* |

---

## Domain Discovery

### Business capability affected

Clinical record management: capturing, retrieving, updating, and soft-deleting **prontuário snapshots** tied to a patient care episode, including gynecologic history (AGO), antecedents, CD actions, exam orders, hospitalization requests, and post-op notes.

### Domain entities and aggregates affected

| Aggregate / entity | Role |
|--------------------|------|
| **Prontuario** | Root — clinical snapshot |
| **Paciente** | Parent FK (verified upstream) |
| **Atendimento** | Required FK — care journey orchestrator (not migrated) |
| **Exame** | Child collection — exam orders |
| **Internacao** | Optional 1:1 child — hospitalization request |
| **ProcedimentoInternacao** | Grandchild — procedures within internacao |
| **ProntuarioAcaoCD** | Child collection — CD action checklist items |
| **CID** | Reference — required when Internacao is persisted |

### Value objects (owned on Prontuario)

`DescricaoBasica`, `AGO`, `Antecedentes`, `AntecedentesFamiliares`, `PosOp`

### Existing business rules (from domain docs)

| Rule | Source |
|------|--------|
| Prontuario is a clinical **snapshot** of a specific moment | `Domain_Overview_Business_Rules.md` |
| **Immutable after creation** (domain intent) | Same |
| **Versioned** — clinical changes should create new prontuario with `Versao`, `ProntuarioAnteriorId` | Same |
| Unique `(PacienteId, Versao)` | EF config + migration |
| Max one Internacao per Prontuario | Domain + EF 1:1 |
| Multiple Exames per Prontuario | Domain (ERD incorrectly shows unique `ProntuarioId` on Exame — migration does not enforce this) |
| Soft delete preserves history | ADR-001 |

### Existing validations

- DTO `[Required]` on several create fields — partially enforced at API layer only.
- Legacy `ParseCd` threw on invalid `AcoesCD` values.
- Legacy skipped malformed sheet rows silently.
- No domain-level validation methods on `Prontuario` entity today.

### Behavior classification

| Behavior | Preserve / Change / Unknown |
|----------|----------------------------|
| CRUD surface (list, by id, by paciente, create, update, delete) | **Preserve** (repository contract) |
| Multi-section clinical payload (AGO, AP, AF, PosOp, CD, Exames, Internacao) | **Preserve** |
| Patient linkage via PacienteId | **Preserve** (adapt location: root FK vs legacy nested in DescricaoBasica) |
| In-place update via PUT | Preserve for MVP2 in discovery *(Research: **abandon** in-place; versioning IN SCOPE)* |
| Physical delete across 3 sheets | **Change** → ADR-001 soft delete |
| PDF import create | **Unknown** — endpoint exists; service commented |
| PDF patient reports | **Remove** from scope (legacy commented) |
| Version increment on each save | **Unknown** — not implemented in legacy |
| Auto-create Atendimento on prontuario create | **Unknown** — required by schema, absent from legacy/API |
| Cascade behavior on soft delete (Exames, Internacao, AcoesCD) | **Unknown** |
| CID validation on Internacao create | **Unknown** — FK exists, no seed data |

---

## Legacy Characterization

### Existing implementation map

| Layer | Artifact | Status |
|-------|----------|--------|
| Legacy repo | `DocAPI/Legacy/_LegacySheetsDb/ProntuarioSheetsRepository.cs` | Commented; 3-sheet model: `Prontuario`, `PedidosExame`, `PedidosCirurgia` |
| SQL repo | `DocAPI/Infrastructure/Repositories/ProntuarioRepository.cs` | Stub — throws `NotImplementedException` |
| Controller | `DocAPI/API/Controllers/ProntuarioController.cs` | Active; PHI in `Console.WriteLine`; string IDs |
| Interface | `IProntuarioRepository` | string IDs throughout |
| DTOs | `Create/Update/ReadProntuarioDto` | Use domain entity types directly (not dedicated DTO VOs) |
| Mapping | `ProntuarioProfile` | Partial; suspected bug mapping `SolicitacaoInternacao` → `InformacoesExtras` on update |
| PDF extractor | `ProntuarioPdfExtractorService` | Fully commented |
| PDF generator | `PdfGeneratorService` | Exists; report paths commented |
| Frontend | `ProntuarioState`, `ProntuarioService`, tab components | Active; expects working API; uses nested `DescricaoBasicaDto.PacienteId` |

### Classification

| Component | Preserve | Adapt | Remove | Unknown |
|-----------|----------|-------|--------|---------|
| `IProntuarioRepository` method surface | ✓ | | | |
| 3-sheet write pattern | | ✓ (SQL normalized) | | |
| String IDs | | ✓ → Guid | | |
| `DescricaoBasica.PacienteId` (legacy) | | ✓ → `Prontuario.PacienteId` | | |
| In-place update | ✓ (MVP2 in discovery) | | | Long-term versioning TBD |
| Hard delete + sheet row removal | | ✓ → soft delete | | |
| `CreateFromPdfAsync` | | | | ✓ |
| PDF reports | | | ✓ | |
| Legacy PHI logging | | | ✓ | |

---

## Architecture Discovery

### Layers affected

| Layer | Involvement |
|-------|-------------|
| **Core/Entities** | Prontuario aggregate + children — likely needs factory/update methods |
| **Infrastructure/Repositories** | Primary implementation target |
| **Infrastructure/SqlDb/Configurations** | Review only (already present) |
| **Application/DTOs + Mappings** | Fix/create mappings, PacienteId routing, Guid alignment |
| **API/Controllers** | PHI remediation, status codes, Guid params |
| **Application/Services** | PDF extractor out of scope unless explicitly added |
| **DocFront.Web** | Not involved (Backend Stabilization Rule) |
| **Legacy** | Read-only characterization reference |

### Not involved

- Financial tables and services
- Agendamento repository (separate vertical slice)
- Full Atendimento workflow and validation porting
- Auth/Identity infrastructure
- Google Sheets runtime

### Infrastructure

- Docker SQL `docorgano-sql` + `InitialCreate` schema (stable assumption, same as Paciente)
- Possible need for CID seed or optional CID policy (Research spike)

### Database impact

- **No new migration expected** — unless Research finds blocking schema issues (Atendimento FK policy, Internacao optional CID, Tipo int vs string mismatch in DTO)
- Unique index `IX_Prontuario_PacienteId_Versao` affects create/version strategy
- Delete behaviors: Exame cascade on Prontuario delete; Internacao 1:1; AcoesCD cascade

---

## Data Discovery

### Tables involved

`Prontuario` (wide owned columns), `ProntuarioAcaoCD`, `Exame`, `Internacao`, `ProcedimentoInternacao`, `CID` (reference), `Atendimento` (FK parent), `Paciente` (FK parent)

### Mappings involved

- `ProntuarioConfiguration` — owned types, relationships, soft-delete filter
- `ExameConfiguration`, `InternacaoConfiguration`, `ProntuarioAcaoCDConfiguration`, `ProcedimentoInternacaoConfiguration`
- `ProntuarioProfile` — incomplete AutoMapper coverage

### Data risks

| Risk | Severity |
|------|----------|
| Create without valid `AtendimentoId` | **High** — FK violation |
| Internacao without existing CID row | **High** — FK violation |
| `(PacienteId, Versao)` unique constraint on second prontuario with default `Versao=1` | **High** — unless version assignment defined |
| Denormalized PHI in `DescricaoBasica_*` columns (Nome, CPF copied from Paciente) | **Medium** — intentional snapshot but duplication risk |
| Update replacing child collections (Exames/AcoesCD) without clear merge semantics | **Medium** |
| Soft-deleted Prontuario with live Atendimento FK | **Low/Medium** — ADR-001 filter interaction |
| `Tipo` stored as `int` in entity/DB; DTO exposes `string?` | **Medium** — mapping ambiguity |

### Compatibility risks

- Frontend sends `PacienteId` inside `DescricaoBasica`; backend entity expects `Prontuario.PacienteId` at root — **contract ambiguity**
- Frontend/API use `string` IDs; stabilized Paciente uses `Guid` — **breaking change** if aligned without WS07 coordination

---

## API Discovery

### Existing endpoints (`ProntuarioController`)

| Method | Route | Consumer |
|--------|-------|----------|
| POST | `/Prontuario` | `ProntuarioService.Create` |
| POST | `/Prontuario/from-pdf` | Unknown / likely unused (service dead) |
| GET | `/Prontuario?skip&take` | `ProntuarioService.GetTotal` |
| GET | `/Prontuario/{id}` | `ProntuarioService.GetById`, `ProntuarioState` |
| GET | `/Prontuario/paciente/{pacienteId}` | `ProntuarioService.GetByPaciente`, list tabs |
| PUT | `/Prontuario/{id}` | `ProntuarioService.Update` |
| DELETE | `/Prontuario/{id}` | `ProntuarioService.Delete` |

Commented: report PDF by id/cpf.

### Contract ambiguities (must document in Specify)

1. **ID type:** `string` in interface/DTOs vs `Guid` in entity — Paciente precedent is Guid end-to-end.
2. **PacienteId source on create:** nested in `DescricaoBasica` (frontend) vs absent in `CreateProntuarioDto` (backend).
3. **AtendimentoId:** required by DB, absent from API contract.
4. **Update semantics:** full replace vs partial patch; child collection replace strategy.
5. **Delete response:** controller maps all exceptions to 404 — Paciente returns proper 404 only for missing entity.
6. **`InformacoesExtras` vs `SolicitacaoInternacao`:** separate fields in DTO; suspicious AutoMapper cross-mapping on update.
7. **`Tipo`:** string in API, int in entity.
8. **404 vs 501:** stub currently throws — should become proper errors once implemented (WS05 debt).

### Contract changes likely required

- Align route parameters and DTO IDs to `Guid` (following Paciente)
- Explicit `PacienteId` on create (or documented extraction from nested DTO)
- Document Atendimento provisioning contract (hidden field vs auto-create)
- Standardize HTTP status codes per Paciente pattern (201/204/404/409/400)
- Document read DTO shape for nested collections

---

## Frontend Discovery

**Backend Stabilization Rule applies:** WS07 is deferred.

| Aspect | Classification |
|--------|----------------|
| Blazor tab/components changes | **Deferred** |
| `ProntuarioService` / `ProntuarioState` alignment to Guid + new contracts | **Deferred** (WS07 P0) |
| Swagger/API-only validation for backend SDD | **Required** (Verify phase) |
| Frontend smoke blocked until backend ready | **Yes** — tabs call stub API today |
| Contract drift (PacienteId nesting, string IDs) | Document as WS07 follow-up, not hidden Execute work |

Frontend validation is **blocked by backend readiness** but **not required** for SDD completion.

---

## Security & PHI Discovery

| Exposure | Location | Risk |
|----------|----------|------|
| Patient name in logs | `ProntuarioController` `Console.WriteLine` on create/PDF | **High** |
| CPF, clinical history in API payloads | All prontuario DTOs/entities | Expected clinical data — protect in logs/tests/docs |
| Error messages | PDF endpoint returns `ex.Message` to client | **Medium** |
| Test fixtures | Must use synthetic data only | Governance requirement |

**Security review required:** **Yes** — same sensor as Paciente (`security-phi-review.md`). Prontuario carries richer clinical PHI than cadastral Paciente fields.

---

## Verification Discovery

### Evidence required (mirror Paciente pilot)

| Gate | Expectation |
|------|-------------|
| `dotnet build` | Pass |
| `dotnet test` | Unit tests for repository mapping, soft delete, child collections, status codes |
| SQL integration | `ProntuarioSqlIntegrationTests` with Docker skip policy |
| Swagger smoke | All active endpoints + documented status codes |
| Legacy table | REQ-style preserve/adapt/abandon in Specify |
| security-phi-review | Controller + repository logging |
| domain-review | Versioning vs update decision documented |

### Manual validation

- Create prontuario with full clinical sections via Swagger
- Read by id and by paciente
- Update with Exames + Internacao + CD changes
- Soft delete and confirm exclusion from default queries
- Attempt create without Paciente / with invalid CID (negative tests)

### Environment dependencies

- Docker SQL + `SA_PASSWORD`
- `dotnet ef database update`
- Migrated `InitialCreate` schema
- Possibly CID seed data for Internacao test scenarios
- Paciente row must exist for FK tests
- Atendimento strategy must be executable in test setup

---

## Documentation Discovery

Candidates for update **after** Verify (not during Research):

| Document | Likely impact |
|----------|---------------|
| `Documentation/State.md` | Runtime status, verification row, next slice |
| `Documentation/Technical/migration-sql.md` | Prontuario checklist + API contract section |
| `Documentation/Product/PM_DocOrgano.md` | WS01 status, WS03 test item, Atendimento prerequisite |
| `Documentation/Architecture/erd.dbml` | Correct stale indexes (PacienteId unique, Exame unique) |
| `Documentation/Architecture/Domain_Overview_Business_Rules.md` | Reconcile versioning vs PUT update |
| ADR evaluation | Atendimento FK provisioning; versioning policy; optional CID |
| `Documentation/Technical/api-contract.md` | Planned in WS06 — Prontuario section |
| Skills | `sql-migration-workflow`, `verifier`, `not-a-teacher` after completion |
| Harness templates | Forward pilot consumer — no template change expected |

---

## Workstream Impact Matrix

| Workstream | Impact | Notes |
|------------|--------|-------|
| **WS01 — SQL & Infrastructure** | **Direct** | Core feature |
| **WS02 — Domain & Architecture** | **Supporting** | Versioning vs update; entity methods; Atendimento FK policy |
| **WS03 — Testing & Verification** | **Direct** | SQL integration tests; characterization optional |
| **WS04 — Security & PHI** | **Direct** | Controller logging; clinical PHI in payloads |
| **WS05 — Code Quality** | **Supporting** | Guid alignment; stub removal; response standardization |
| **WS06 — Legacy & Integrations** | **Supporting** | Legacy characterization; PDF services out of scope |
| **WS07 — Frontend** | **Supporting (deferred)** | Contract alignment after backend verified |
| **WS08 — AI Harness** | **Direct** | First **forward** post-calibration SDD pilot |

---

## Core Feature

**Prontuario SQL Stabilization SDD** (`Documentation/SDD/prontuario-sql-stabilization/`)

Primary deliverable: production-ready **`ProntuarioRepository`** + hardened **`ProntuarioController`**, verified against Docker SQL with documented legacy decisions, following Paciente REQ-style acceptance criteria and harness lifecycle.

---

## Supporting Features

| Supporting item | Disposition | Rationale |
|-----------------|-------------|-----------|
| Minimal Atendimento row provisioning | Requirement in core SDD (or Research spike → ADR) | Blocking FK; full Atendimento migration is separate PM item |
| CID reference seed for tests | **VP** (Validation Preparation) | Internacao FK tests need valid CID unless policy makes CID optional |
| Prontuario SQL integration tests | Requirement in core SDD | WS03 P0 pattern from Paciente |
| PHI controller remediation | Requirement in core SDD | WS04 P0 extension beyond Paciente |
| Legacy characterization table | Requirement in Specify | REQ-008 pattern |
| Guid/API contract alignment | Requirement in core SDD | Follow Paciente precedent; document WS07 drift |
| Versioning implementation (new row per edit) | Separate Feature or WS02 follow-up in discovery | Research: **IN SCOPE** for this SDD |
| `POST /from-pdf` | Separate Feature or explicit Out of Scope | Service commented; high complexity |
| PDF patient reports | Out of Scope | Legacy commented |
| WS07 frontend integration | Separate Feature (WS07 P0) | Backend Stabilization Rule |
| Atendimento rules port (`ValidacaoEtapa*`) | Separate Feature (WS02 P0) | Different aggregate |
| Characterization tests executable against Legacy | VP optional | WS02 P0 planned; not blocking if legacy table suffices |
| ERD correction | DF (Documentation Follow-Up) | Stale vs EF/migration |
| `migration-sql.md` Prontuario contract | DF | After Verify |
| Teacher Guide | DF per knowledge strategy | After Reporting |

---

## Scope Decomposition

```text
Core SDD (prontuario-sql-stabilization)
├── Research
│   ├── Legacy 3-sheet behavior characterization
│   ├── Atendimento FK strategy decision
│   ├── Versioning vs in-place update decision
│   ├── Entity factory/update method design
│   └── CID policy for Internacao
├── Specify / Design / Tasks
├── Execute
│   ├── ProntuarioRepository (CRUD + nested children)
│   ├── Controller hardening + Guid alignment
│   ├── DTO/mapping fixes
│   ├── Minimal Atendimento provisioning (if decided)
│   └── Unit + SQL integration tests
└── Verify → Documentation Follow-Up → Reporting → Teacher Guide

Deferred / separate
├── WS07 frontend contract alignment
├── Full Atendimento SQL + validations
├── PDF import and reports
└── True versioning workflow (discovery default — superseded by Research)
```

---

## Dependency Analysis

### Upstream (must exist first)

| Dependency | Status at discovery |
|------------|---------------------|
| Paciente SQL stabilization | **Complete** (verified) |
| `InitialCreate` EF migration applied | **Available** |
| Harness calibration (Wave 1+2) | **Complete** |
| Docker SQL infrastructure | **Available** (env-dependent) |
| Research decisions on Atendimento FK + versioning | **Not done** — blocks Specify |

### Downstream (enabled by this feature)

| Enabled work | Workstream |
|--------------|------------|
| WS07 Prontuario tab integration | WS07 |
| Agendamento SQL slice (similar pattern) | WS01 |
| Atendimento migration (Prontuario consumes Atendimento FK) | WS01/WS02 |
| Clinical demo flows including prontuario CRUD | WS08 |
| Merge criterion #2 in `migration-sql.md` | WS01 |

### Blocking dependencies

| Blocker | Impact |
|---------|--------|
| AtendimentoId FK with stub Atendimento repo | Cannot persist Prontuario without strategy |
| Unresolved version assignment on create | Unique index `(PacienteId, Versao)` may fail on 2nd prontuario |
| Entity private setters without application API | Implementation may require entity work beyond pure repository |
| CID empty table | Internacao create fails without seed or optional policy |
| PDF extractor commented | `/from-pdf` cannot work without separate effort |

---

## Risk Analysis (Discovery)

| # | Risk | Class | Prob. | Impact | Mitigation |
|---|------|-------|-------|--------|------------|
| R1 | Atendimento FK blocks all creates | Technical / Data | High | High | Research spike: auto-create minimal Atendimento or ADR for deferred FK |
| R2 | Version index collision (`Versao=1` for all) | Data / Domain | High | High | Define version policy in Research; increment per patient or per atendimento |
| R3 | Domain versioning vs PUT update conflict | Domain / Governance | Medium | High | Document MVP2 adapt decision; ADR if immutable snapshots required |
| R4 | Child collection update semantics undefined | Technical | Medium | High | Design explicit replace/merge strategy in Design phase |
| R5 | Internacao fails on missing CID | Data | Medium | Medium | VP: seed test CIDs; document required CID in API contract |
| R6 | PHI in controller logs | Security | High | Medium | REQ-style remediation in Execute |
| R7 | Scope creep via `/from-pdf` or PDF reports | Governance | Medium | Medium | Explicit Out of Scope in Specify |
| R8 | Frontend breaks on Guid migration | Technical | Medium | Medium | Backend Stabilization Rule; document WS07 follow-up |
| R9 | ERD/schema doc drift misleads agents | Governance | Medium | Low | DF update `erd.dbml` |
| R10 | Under-tested nested graphs | Verification | Medium | High | Integration tests covering Exames + Internacao + CD |
| R11 | AutoMapper bug loses Internacao on update | Technical | Medium | High | Catch in Research code review; fix in Execute |
| R12 | SQL integration env unavailable | Verification | Medium | Low | Skip policy (Paciente precedent) |

---

## Breaking Change Assessment

| Change type | Impact | Mitigation |
|-------------|--------|------------|
| **User-visible** | **Medium** — soft delete vs hard delete behavior; possible error message changes | Document in API contract; WS07 later |
| **API** | **Medium** — string→Guid IDs; PacienteId routing; status code alignment | Follow Paciente pattern; WS07 alignment tracked separately |
| **Database** | **Low** — no schema change expected | If migration needed, ADR + separate approval |
| **Operational** | **Low** — requires Docker SQL for full verification | Document prerequisites like Paciente SDD |

---

## Feature Sizing

| Field | Decision |
|-------|----------|
| **Size** | **Large** |
| **Rationale** | Brownfield composite aggregate with 4 related tables, 5+ owned value objects, mandatory cross-aggregate FK (Atendimento), unresolved domain versioning, no tests today, PHI-sensitive clinical payloads, legacy spread across 3 sheets, and first forward harness SDD after calibration. Strictly more complex than Paciente (flat aggregate). |
| **Calibration reference** | Paciente = Large with 8 REQs, 15 tests, full lifecycle |
| **Escalation triggers** | Schema migration required; versioning mandated in MVP2; PDF import in scope; Atendimento full migration pulled into slice |

---

## Recommended PM Updates (Discovery)

1. Expand WS01 Prontuario item to explicitly list: nested child persistence, Atendimento FK strategy, legacy 3-sheet characterization, PHI remediation, Guid contract alignment — not just "vertical validated."
2. Add WS03 P0: "Prontuario SQL integration tests" (mirror Paciente verified item).
3. Add WS02 supporting prerequisite: "Atendimento minimal provisioning for Prontuario FK" — or mark as Research deliverable inside Prontuario SDD.
4. Fix copy-paste errors in PM lines for Agendamento/Atendimento ("Vertical Prontuário" → correct aggregate name).
5. Add explicit Out of Scope note under WS01: PDF import/reports, WS07 frontend, full Atendimento validation port.
6. Update WS08 status from "calibrar antes do piloto Prontuario" → calibration complete; Prontuario is active forward pilot.
7. Track WS07 debt explicitly: Prontuario Guid + PacienteId contract alignment after backend verification.

---

## Recommended SDD Scope (Discovery)

Single SDD folder: **`prontuario-sql-stabilization`**

### In Specify (anticipated REQ themes, not final)

- Full-field create/read/update/delete persist all clinical sections and child collections
- Soft delete per ADR-001 with child behavior documented
- List + by-id + by-paciente retrieval with pagination
- HTTP status code contract (Paciente-aligned)
- PHI-safe controller
- SQL integration tests
- Legacy characterization table
- Atendimento FK provisioning decision implemented
- Swagger smoke checklist

### Explicit Out of Scope (discovery defaults; refined in Part 2)

- DocFront.Web changes
- PDF import (`/from-pdf`) and report endpoints
- Full Atendimento migration and `ValidacaoEtapa*`
- Google Sheets import
- Auth/RBAC
- New migrations (default)
- True versioning workflow (discovery default defer — **superseded: versioning IN SCOPE**)

### Separate follow-up features

- WS07 Prontuario frontend integration
- Atendimento SQL Migration & Stabilization (WS01 next-after-Prontuario in sequence)
- Prontuario PDF ingestion (if product wants it)

---

## Discovery Readiness Decision

| Phase | Ready? | Justification |
|-------|--------|---------------|
| **Additional Discovery** | Partially complete | Boundaries and blockers identified |
| **Research** | **Yes — proceed** | Governance sources read; codebase mapped; Paciente template available |
| **Specify** | **No — not yet** | Five blocking decisions must be resolved in Research |

### Research entry actions (completed in Part 2)

1. Read targeted legacy methods in `ProntuarioSheetsRepository.cs` (Add/Update/Delete/Collect — not full file).
2. Spike: create prontuario row in SQL with current schema — validate Atendimento + version constraints.
3. Decide: in-place PUT vs versioning for MVP2.
4. Decide: Atendimento auto-create vs minimal stub repository vs ADR schema change.
5. Audit `ProntuarioProfile` and DTO↔entity gaps (including Tipo, PacienteId, Internacao mapping).
6. Draft legacy preserve/adapt/abandon table for Specify handoff.

---

# Part 2 — SDD Research Consolidation

**Phase:** Research (complete for this session)  
**Governance:** PM, PRD, `State.md`, Domain docs, `migration-sql.md`, ADR-001, Paciente pilot, SDD Operational Governance  
**Constraints honored:** No Specify/Design/Tasks, no code, no migrations, no production edits

## Approved Research Decisions (Input)

Research validated six decisions approved before this session:

| # | Decision | Approved direction |
|---|----------|-------------------|
| 1 | Atendimento dependency | Create Atendimento → Create Prontuario; no auto-create inside Prontuario; no FK relaxation unless critical blocker |
| 2 | Versioning strategy | Versioning **IN SCOPE** for MVP2 minimum (v1 on create, new row on update, `ProntuarioAnteriorId`) |
| 3 | CID strategy | Internacao optional; CID mandatory when Internacao exists; catalog = separate feature |
| 4 | Backend source of truth | Domain → Entity → Persistence → API → Frontend |
| 5 | AutoMapper refactoring | Current Prontuario mapping is legacy; audit and replace |
| 6 | Rich domain model | Paciente-style factories/update/version methods; repository persistence only |

---

## Executive Summary (Research)

Research validates the approved direction with **one critical sequencing contradiction** and **one legacy-vs-domain override**.

### Validated as feasible in MVP2

- **Versioning in scope** — schema already has `Versao`, `ProntuarioAnteriorId`; no migration required for minimum versioning.
- **CID strategy** — Internacao remains optional; CID required only when Internacao is supplied; catalog management is a separate feature; tests can use synthetic CID rows.
- **Backend as source of truth** — extensive contract drift documented; Guid + root-level FKs are the target shape.
- **AutoMapper** — current `ProntuarioProfile` is unsafe; replace/refactor is required in Execute (not incremental fix).
- **Rich domain model** — required; current `Prontuario` cannot be persisted correctly via AutoMapper + private setters alone.

### Critical contradiction (Decision 1)

Approved workflow is **Create Atendimento → Create Prontuario**, but:

- `AtendimentoRepository` is **stubbed** (throws on all operations).
- `migration-sql.md` and PM order Prontuario **#2** and Atendimento **#4**.

Prontuario **cannot Execute** without a real SQL path to create and validate Atendimento rows. This is **not** auto-create inside Prontuario create — it is an **upstream persistence prerequisite** that must appear explicitly in Specify scope (minimal Atendimento SQL create) or PM/migration order must change.

### Legacy override (Decision 2)

Legacy `ProntuarioSheetsRepository` has **no versioning** (in-place update, same ID). Domain authority + approved direction require **new row per relevant change**. Legacy behavior for update is **abandoned**; clinical payload preservation is **preserved**.

**Research readiness:** **READY FOR SPECIFY WITH CONDITIONS** — Specify may start once the Specify Entry Checklist at the end is accepted as explicit SDD scope inputs.

---

## Discovery Validation

### Decision 1 — Atendimento Dependency

| Aspect | Finding |
|--------|---------|
| **Schema** | `Prontuario.AtendimentoId` is `NOT NULL` with FK to `Atendimento.Id` (`ProntuarioConfig`, `InitialCreate`). |
| **Domain** | Prontuario is associated with an atendimento (care episode). Approved workflow matches schema. |
| **Current code** | `AtendimentoRepository` stub; `POST /Atendimento` exists but throws at repository. |
| **PM / migration-sql** | Order: Paciente → **Prontuario** → Agendamento → **Atendimento** — **contradicts** approved workflow. |

**Implementation impact**

| Component | Impact |
|-----------|--------|
| **Repositories** | `AtendimentoRepository`: minimal SQL `CreateAsync` + `GetByIdAsync` (+ Paciente existence check). `ProntuarioRepository`: must validate `AtendimentoId` exists, belongs to same `PacienteId`, not soft-deleted. |
| **Contracts** | `CreateProntuarioDto` must require `AtendimentoId` (Guid) at root — not nested, not auto-derived. |
| **Domain rules** | Prontuario create rejected if Atendimento missing or Paciente mismatch. |
| **Controllers** | `AtendimentoController`: Guid alignment, PHI cleanup (parallel to Paciente). `ProntuarioController`: accept `AtendimentoId` on create. |

**Critical blocker discovered (within approved direction)**

Without **minimal Atendimento SQL create**, no Prontuario row can persist. This satisfies the "no auto-create / no stub / no FK relaxation" rule: the fix is **implementing step 1 of the approved workflow**, not hiding Atendimento creation inside Prontuario.

**Recommendation for Specify**

- Add explicit **upstream REQ**: *Minimal Atendimento SQL persistence* (create + read-by-id + Paciente FK validation only).
- Update PM / `migration-sql.md` to document **Atendimento minimal create before Prontuario** (exception to numeric order, not full Atendimento stabilization).

**Validation result:** **Approved direction valid** against schema; **current repo state blocks Execute** until minimal Atendimento scope is owned.

---

### Decision 2 — Versioning Strategy

| Dimension | Assessment |
|-----------|------------|
| **Domain authority** | Immutable snapshot; new version on relevant change; `Versao`, `ProntuarioAnteriorId`. |
| **Legacy** | In-place `UpdateProntuarioAsync` — same sheet row, same ID; **no versioning**. |
| **Schema** | Supports versioning today. Unique index: **`(PacienteId, Versao)`** — version sequence is **per patient**, not per Atendimento. |
| **Migration impact** | **None** for minimum versioning. |
| **Repository impact** | **High.** `UpdateAsync` must **not** mutate existing row. New method e.g. `CriarNovaVersaoAsync(Guid idAnterior, …)` or domain-driven equivalent. |
| **API impact** | **High.** Current `PUT /Prontuario/{id}` implies in-place update. Versioning requires: **(A)** PUT creates successor, returns **201** + new `ReadProntuarioDto` with new `Id`; or **(B)** `POST /Prontuario/{id}/versoes`. Recommend **(A)** with documented breaking semantic change vs legacy. |
| **Verification impact** | Tests must prove: old row unchanged; new row has `Versao = max+1`; `ProntuarioAnteriorId` set; unique index satisfied; soft delete affects only targeted version. |

**Minimum acceptable versioning (feasible MVP2)**

| Rule | Implementation note |
|------|---------------------|
| `Versao = 1` on creation | Set in factory; first row for patient |
| New version on update | New `Guid`, increment `Versao` per `PacienteId`, same `AtendimentoId` unless Specify says otherwise |
| `ProntuarioAnteriorId` | Points to immediate predecessor |
| No rollback / diff / audit UI | Out of scope |
| Immutability | Repository never mutates owned types of existing version row |

**Open Specify decisions (not blockers for Research exit)**

- `GET /Prontuario/paciente/{id}`: return **all non-deleted versions** vs **latest per Atendimento** — default recommend **all versions** (matches legacy list semantics).
- `DELETE`: soft-delete **single version** only; predecessors remain visible unless separately deleted.

**Validation result:** **Versioning IN SCOPE — feasible without schema change.** Legacy in-place update is **rejected** per domain authority.

---

### Decision 3 — CID Strategy

| Aspect | Finding |
|--------|---------|
| **Schema** | `Internacao.CIDCodigo` NOT NULL; FK to `CID.Codigo`. `Internacao` only exists when prontuario has internacao payload. |
| **CID table** | Exists; **no seed data** in migrations. |
| **Entity** | `CID` is a simple reference entity (Codigo, Descricao). |

**Approved direction validation**

| Rule | Valid? |
|------|--------|
| Internacao optional on Prontuario | **Yes** — no Internacao row when omitted |
| CID mandatory when Internacao exists | **Yes** — FK enforced |
| CID catalog = separate feature | **Yes** — production catalog not required for Prontuario slice |
| Prontuario not blocked by missing catalog | **Yes** — create/update without Internacao works; with Internacao, API returns **400/404** if CID code unknown |

**Test strategy (VP, not product feature)**

- SQL integration tests insert **synthetic CID** row(s) in fixture setup (e.g. `Z99.9` / `Test CID`).
- No production seed requirement in this SDD.

**Future feature identified:** **CID Catalog Management** (CRUD/import, admin UI, validation service).

**Migration impact:** None for Prontuario-only scope.

**Validation result:** **Approved direction valid.**

---

### Decision 4 — Backend Source of Truth

**Target chain:** Domain → Entity → Persistence → API → Frontend (Frontend adapts later in WS07)

**Required contract changes (backend-owned)**

| Area | Current drift | Required change |
|------|---------------|-----------------|
| **IDs** | `IProntuarioRepository`, DTOs, controller use `string`; entities use `Guid` | **Guid end-to-end** (Paciente precedent) |
| **PacienteId** | Frontend: `DescricaoBasicaDto.PacienteId`; backend create DTO: **absent**; entity: `Prontuario.PacienteId` root | **Root `PacienteId` on create/read**; read-only denormalized copy in `DescricaoBasica` if needed |
| **AtendimentoId** | Required by DB; **absent from all Prontuario DTOs** | **Required on create**; **on read** DTO |
| **Version fields** | Absent from DTOs | **`Versao`, `ProntuarioAnteriorId`** on `ReadProntuarioDto` |
| **Tipo** | DTO `string?`; entity/DB `int` | **int in API** or enum name mapping — single canonical type in Specify |
| **CD actions** | DTO `List<AcoesCD>`; persistence `ProntuarioAcaoCD` entities | Explicit map; replace-on-new-version for collections |
| **Internacao** | DTO `SolicitacaoInternacao`; entity navigation `Internacao` | Map explicitly; include `ProcedimentoInternacao` list (legacy had `List<string>`) |
| **Exames** | DTO embeds `Exame` entities | Dedicated read/write DTOs (not domain entities in API layer) |
| **Update semantics** | PUT = in-place | PUT = **new version** (Decision 2) |
| **HTTP status codes** | Inconsistent (500/404 conflation) | Paciente-aligned: 201/204/404/400 |
| **Atendimento DTOs** | `CreateAtendimentoDto` legacy shape (`ProntuariosId`, string IDs) | Minimal create: **`PacienteId` (Guid)** only + optional `MensagemParaMedico` |

**WS07 deferred:** Frontend continues using old shapes until backend verified; document drift in Specify Out of Scope / WS07 follow-up.

**Validation result:** **Approved direction valid**; drift catalog is complete enough for Specify.

---

### Decision 5 — AutoMapper Refactoring

**Verdict:** Treat `ProntuarioProfile` as **legacy** — high-risk; Specify should plan **replacement**, not patch-in-place.

**Mapping audit**

| Mapping | Status | Risk |
|---------|--------|------|
| `CreateProntuarioDto → Prontuario` | **Broken / dangerous** | Cannot invoke `Prontuario(Guid, Guid)` constructor; private setters; no `PacienteId`/`AtendimentoId` in DTO |
| `UpdateProntuarioDto → Prontuario` | **Broken / dangerous** | **Confirmed bug:** `InformacoesExtras ← SolicitacaoInternacao` (line 26 in `ProntuarioProfile.cs`) |
| Missing on update | PosOperatorio, CD, SolicitacaoInternacao→Internacao, InformacoesExtras | **Data loss** |
| `Prontuario → ReadProntuarioDto` | **Incomplete** | Entity `Internacao` vs DTO `SolicitacaoInternacao`; missing version/FK fields |
| DTO uses domain entities | `CreateProntuarioDto` embeds `DescricaoBasica`, `AGO`, etc. | **Anti-pattern** — blurs API and domain |
| Commented maps | All nested DTO maps commented | Incomplete migration from Sheets era |

**Mapping risk assessment**

| Risk ID | Description | Severity |
|---------|-------------|----------|
| M1 | AutoMapper create bypasses required constructor | **Critical** |
| M2 | Update maps Internacao into InformacoesExtras | **Critical** |
| M3 | Owned types updated in-place violates versioning | **Critical** |
| M4 | Child collections mapped without aggregate ownership | **High** |
| M5 | Read DTO exposes domain entity types | **High** |
| M6 | `AtendimentoProfile` CreateMap without factory | **High** |

**Recommendation:** Execute uses **domain factories** + **explicit read DTO projection**; limit or remove AutoMapper for Prontuario slice.

**Validation result:** **Approved direction valid** — refactor required.

---

### Decision 6 — Rich Domain Model

**Paciente precedent** (`AplicarCriacao`, `AplicarAtualizacao`, `MarcarComoExcluido`): Prontuario needs equivalent.

**Recommended aggregate responsibilities**

| Responsibility | Owner |
|----------------|-------|
| Version assignment (`Versao`, `ProntuarioAnteriorId`) | **Prontuario** factory / `CriarNovaVersao` |
| Paciente + Atendimento FK invariants | **Prontuario** factory |
| Owned VO construction | **Prontuario** factory from command/DTO |
| Exames / Internacao / AcoesCD on **new version** | **Prontuario** |
| Soft delete one version | **`MarcarComoExcluido()`** on entity |
| Persistence, includes, transactions | **Repository only** |
| HTTP / status codes | **Controller** |
| DTO validation | **Controller or thin validator** — not repository |

**Child collection ownership**

- **Exames**, **ProntuarioAcaoCD**: replace entire collection when creating new version (simplest MVP2 semantics).
- **Internacao + ProcedimentoInternacao**: optional 1:1 + 1:N; create with new version only.

**Encapsulation gaps today**

- `Prontuario` has constructor `(Guid pacienteId, Guid atendimentoId)` but no methods to apply clinical payload.
- Child entities require parent IDs — must be created through aggregate, not AutoMapper.

**Validation result:** **Approved direction valid and necessary.**

---

## Technical Findings

1. **Composite persistence graph:** One Prontuario write can touch up to 5 tables (`Prontuario`, `Exame`, `Internacao`, `ProcedimentoInternacao`, `ProntuarioAcaoCD`).
2. **Soft delete:** ADR-001 global filter on `Prontuario`; child tables use cascade delete on **hard** delete — soft delete of parent must be explicit.
3. **Internacao required columns:** Any Internacao payload must populate all non-null columns (`IndicacaoClinica`, `Observacao`, `CIDCodigo`, `TempoDoenca`, `Diarias`, `Tipo`, `Regime`, `Carater`, `UsaOPME`, `Local`, `Data`).
4. **Exame required columns:** `Codigo`, `Nome` non-null in DB; legacy often used empty codigo.
5. **Atendimento entity naming:** `Atendimento.Id` vs `Paciente.ID` / `Prontuario.ID` — inconsistency to resolve in minimal Atendimento slice.
6. **Integration test pattern:** `PacienteSqlIntegrationTests` provides Docker skip policy — reuse for Prontuario + Atendimento fixtures.
7. **No Prontuario tests exist today.**

---

## Domain Findings

1. **Legacy vs domain on update:** Legacy preserves clinical sections via in-place edit; domain preserves via **new snapshot**. MVP2 must accept API behavior change on PUT.
2. **Denormalized DescricaoBasica:** Snapshot copies Nome/CPF/Idade from patient at creation — factory should snapshot from `Paciente` + payload.
3. **Version scope:** Unique `(PacienteId, Versao)` means version counter is **patient-wide** (e.g. second atendimento's first prontuario gets `Versao = 2` if patient already has v1).
4. **Immutability + soft delete:** Deleted version hidden by filter; chain remains for non-deleted predecessors.
5. **Clinical integrity:** Invalid `AcoesCD` values must fail fast (legacy `ParseCd` threw).

---

## Contract Findings

### API surface (post-Specify target)

| Method | Route | Semantics |
|--------|-------|-----------|
| POST | `/Atendimento` | Create atendimento for `PacienteId` → 201 + `ReadAtendimentoDto` |
| POST | `/Prontuario` | Requires `PacienteId`, `AtendimentoId`, clinical payload → 201 v1 |
| PUT | `/Prontuario/{id}` | Creates **new version** → **201** + new DTO (recommended) |
| GET | `/Prontuario/{id}` | Specific version |
| GET | `/Prontuario/paciente/{pacienteId}` | List (all non-deleted versions; default) |
| DELETE | `/Prontuario/{id}` | Soft-delete **that version** → 204 |
| POST | `/Prontuario/from-pdf` | **Out of scope** — service commented |

### Breaking vs legacy/API today

- Guid IDs
- PUT creates new row (new Id)
- `AtendimentoId` required on create
- Soft delete vs hard delete across 3 sheets

---

## Mapping Findings

Execute must **not** rely on current `ProntuarioProfile` for create/update.

**Priority fixes for Specify/Design:**

1. Remove domain entity types from public DTOs.
2. Introduce factory inputs with `PacienteId`, `AtendimentoId`.
3. Replace update map with `CriarNovaVersao(desdeId, payload)`.
4. Fix Internacao / InformacoesExtras / PosOperatorio / CD mappings.
5. Align `ReadProntuarioDto` with persistence graph including version metadata.

---

## Risk Assessment (Research)

| Risk | Severity | Impact | Mitigation |
|------|----------|--------|------------|
| Atendimento repo stub blocks all Prontuario creates | **Critical** | Execute blocked | Minimal Atendimento SQL create in SDD scope; PM order update |
| Versioning changes PUT contract | **High** | WS07 + API clients break | Document in Specify; ADR candidate; WS07 follow-up |
| `(PacienteId, Versao)` collision | **High** | Second create fails | Centralize `NextVersao(pacienteId)` in domain/repository |
| AutoMapper silent data loss | **Critical** | Wrong clinical data persisted | Remove from create/update path |
| CID missing when Internacao sent | **Medium** | FK error → 500 | Validate CID exists → 400/404; test fixtures seed synthetic CID |
| Internacao partial payload | **Medium** | DB constraint failures | DTO validation when section present |
| Soft delete vs child rows | **Medium** | Orphan or hidden data | Specify child behavior on version soft delete |
| PHI in logs | **High** | Compliance | REQ: remove `Console.WriteLine` in Prontuario/Atendimento controllers |
| Under-tested nested graph | **High** | Production regression | Integration test with Exames + Internacao + CD |
| PM/migration-sql order misleading | **Medium** | Wrong agent assumptions | PM + migration-sql update in Documentation Follow-Up |
| ERD stale (unique indexes) | **Low** | Planning errors | DF update `erd.dbml` |
| Scope creep (PDF, full Atendimento) | **Medium** | Schedule slip | Explicit Out of Scope in Specify |

---

## PM Impact (Research)

Required PM changes (for Documentation Follow-Up after Verify; not applied in Research):

1. **WS01 Prontuario item** — add: nested children, versioning, minimal Atendimento prerequisite, Guid contracts, PHI, legacy **adapt** on update.
2. **New WS01 supporting item** — *Minimal Atendimento SQL Create (Prontuario prerequisite)* before full Atendimento stabilization.
3. **WS03 P0** — Prontuario SQL integration tests (+ minimal Atendimento fixture).
4. **Fix PM copy-paste** — Agendamento/Atendimento rows say "Vertical Prontuário".
5. **`migration-sql.md`** — order exception: minimal Atendimento create before Prontuario; full Atendimento still #4.
6. **WS07** — track Guid, `AtendimentoId`, versioning PUT semantics, root `PacienteId`.
7. **WS08** — harness calibration complete; Prontuario = forward pilot.
8. **New backlog item** — CID Catalog Management (P1/P2).

---

## Follow-Up Features (Research)

| Feature | Trigger | Workstream |
|---------|---------|------------|
| **Minimal Atendimento SQL Create** | Prontuario FK prerequisite | WS01 (in or before Prontuario SDD) |
| **Atendimento SQL Migration & Stabilization** (full) | After Prontuario; includes `ValidacaoEtapa*` | WS01 / WS02 |
| **CID Catalog Management** | Internacao in production without manual DB inserts | WS01 / WS06 |
| **WS07 Prontuario frontend integration** | Backend verified | WS07 |
| **Prontuario PDF ingestion** | Product request | WS06 |
| **Agendamento SQL** | migration-sql sequence | WS01 |
| **ADR: Prontuario versioning API** | PUT semantic change | Architecture |
| **ADR: Version numbering scope** | Per-patient vs per-atendimento | Architecture *(research recommends per-patient per current index)* |

---

## Research Conclusions

1. All six approved decisions are **directionally correct** and **implementable** on `InitialCreate` schema without new migrations (default).
2. **Decision 1** exposes a **hard Execute blocker**: minimal **real** Atendimento SQL create must be in scope or ordered before Prontuario — not auto-create inside Prontuario, not FK relaxation.
3. **Decision 2** is **in scope** and **overrides legacy** in-place update; PUT/API semantics must change — document as REQ + possible ADR.
4. **Decision 3** is validated; CID catalog is a **separate feature**; tests use synthetic CIDs.
5. **Decision 4** contract drift is extensive; backend owns the target contract; WS07 follows.
6. **Decision 5** AutoMapper is **not safe** for Execute; plan replacement.
7. **Decision 6** rich domain is **mandatory** for versioning and child ownership.
8. Feature remains **Large**; larger than Paciente due to graph persistence, versioning, and Atendimento prerequisite.

---

## Readiness Decision

### **READY FOR SPECIFY WITH CONDITIONS**

Specify may begin when stakeholders accept the **Specify Entry Checklist** below as non-negotiable scope inputs. Research does **not** require another discovery pass unless Specify rejects the Atendimento prerequisite or versioning scope.

---

## Specify Entry Checklist

Before writing `specify.md`, confirm:

### Scope ownership

- [ ] **Minimal Atendimento SQL create** is included as upstream REQ (create + get-by-id + Paciente FK validation, Guid-aligned) — **not** auto-create inside Prontuario, **not** stub, **not** FK relaxation.
- [ ] **Versioning is IN SCOPE** with minimum rules: v1 on create, new row on update, `ProntuarioAnteriorId`, no diff/rollback/UI.
- [ ] **Legacy in-place update is ABANDONED**; clinical payload sections **PRESERVED**.
- [ ] **`POST /Prontuario/from-pdf` and PDF reports** are Out of Scope.
- [ ] **WS07 / DocFront.Web** are Out of Scope (Backend Stabilization Rule).
- [ ] **Full Atendimento stabilization** (`ValidacaoEtapa*`) remains Out of Scope.
- [ ] **CID catalog management** is Out of Scope; Internacao optional; CID required when Internacao present.
- [ ] **No new EF migration** unless Design proves otherwise (default: no).

### Decisions recorded in Specify

- [ ] **Version numbering:** per `PacienteId` (per unique index `(PacienteId, Versao)`).
- [ ] **PUT semantics:** new version created; HTTP **201** + new `Id` (or explicitly chosen alternative documented).
- [ ] **List by paciente:** default = all non-deleted versions (unless product chooses filter).
- [ ] **DELETE:** soft-delete single version only.
- [ ] **`PacienteId` + `AtendimentoId`** on create at API root (Guid).
- [ ] **`Tipo`** canonical type chosen (int recommended to match DB).
- [ ] **Internacao** full-field validation when section present.
- [ ] **Child collections:** replace-on-new-version semantics documented.
- [ ] **DescricaoBasica snapshot:** server snapshots identity fields from `Paciente` where applicable.

### Contract / mapping

- [ ] **Guid migration** for Prontuario + minimal Atendimento repository interface.
- [ ] **Dedicated API DTOs** — no domain entity types in public DTOs.
- [ ] **AutoMapper replacement plan** referenced in Design (not patch `InformacoesExtras` bug only).
- [ ] **`ReadProntuarioDto`** includes `Versao`, `ProntuarioAnteriorId`, `PacienteId`, `AtendimentoId`.
- [ ] **Legacy behavior table** (preserve/adapt/abandon) drafted for REQ-008 pattern.

### Verification expectations (for Specify acceptance criteria)

- [ ] Unit tests: factory, versioning chain, soft delete, invalid Atendimento/CID.
- [ ] SQL integration: Paciente → Atendimento → Prontuario v1 → update → v2 round-trip with nested children.
- [ ] Swagger smoke checklist for Prontuario + minimal Atendimento endpoints.
- [ ] security-phi-review on touched controllers.
- [ ] domain-review for versioning vs legacy table.

### PM / documentation (acknowledged for post-Verify DF)

- [ ] PM order / migration-sql exception for Atendimento-before-Prontuario accepted.
- [ ] CID Catalog Management logged as future feature.
- [ ] ADR evaluation scheduled for versioning API contract change.

---

---

# Part 3 — Research Delta (post Atendimento Minimal Verify)

**Date:** 2026-06-18  
**Trigger:** Atendimento Minimal SQL Stabilization verified; PM and `migration-sql.md` resequenced; code audit confirms prerequisite satisfied; stakeholder session refined research-sufficient criteria.  
**Governance:** No new discovery pass required unless Specify rejects items marked **Specify decision required** below.

## Context change

| Change | Before (Part 1/2) | After (Part 3) |
|--------|---------------------|----------------|
| Atendimento SQL | Stub — Execute blocker | **Verified** — [atendimento-minimal-sql-stabilization/verification.md](../atendimento-minimal-sql-stabilization/verification.md) |
| Atendimento scope in Prontuario SDD | Upstream REQ inside Prontuario slice | **Out of scope** — owned by separate verified SDD |
| PM / migration order | Contradiction: Prontuario #2, Atendimento #4 | **Aligned:** Paciente → Atendimento Minimal → Prontuario → Agendamento → Atendimento Workflow |
| Test baseline | Paciente-only fixtures | **27 tests**; `AtendimentoSqlIntegrationTests` reusable as fixture pattern |
| Prontuario code | Stub (unchanged) | Still stub — **Execute target** of this SDD |

## Superseded findings (Part 1/2 — do not carry into Specify)

| Superseded claim | Superseding truth |
|------------------|-------------------|
| "Minimal Atendimento SQL create" is a Prontuario SDD REQ or Execute blocker | Prerequisite **satisfied** via verified SDD; Prontuario **consumes** existing API/repository only |
| "Critical contradiction" on PM order unresolved | Resolved in [PM_DocOrgano.md](../../Product/PM_DocOrgano.md) and [migration-sql.md](../../Technical/migration-sql.md) (2026-06) |
| Supporting feature "Minimal Atendimento row provisioning" in core Prontuario SDD | **Removed** from Prontuario scope — reference verified SDD instead |
| Risk R1 "Atendimento repo stub blocks all Prontuario creates" | **Mitigated** — residual risk is contract misuse (wrong PacienteId / missing Atendimento), not stub |
| Specify Entry Checklist: include minimal Atendimento implementation | **Replaced:** prerequisite verified + Prontuario validates FK only |
| Research readiness "WITH CONDITIONS" on Atendimento blocker | **Conditions satisfied** — ready for Specify |

## Confirmed unchanged (Part 2 decisions — still authoritative)

| # | Decision | Part 3 note |
|---|----------|-------------|
| 1 | Create Atendimento → Create Prontuario; no auto-create inside Prontuario | Atendimento path is **production API** from verified SDD |
| 2 | Versioning **IN SCOPE**; legacy in-place update **abandoned** | See **Versioning semantics** below — clarify "new row" |
| 3 | Internacao optional; CID mandatory when Internacao present | CID catalog out of scope; synthetic CID in tests only |
| 4 | Backend source of truth; Guid + root FKs | Prontuario DTOs/controllers still drift — Execute fixes |
| 5 | Replace `ProntuarioProfile` AutoMapper — do not patch | Unchanged |
| 6 | Rich domain model (factories, `CriarNovaVersao`, soft delete) | Unchanged |
| — | Feature sizing **Large** | Unchanged — composite graph + versioning |

## Atendimento contracts consumed by Prontuario (verified — not re-implemented)

Source: [atendimento-minimal-sql-stabilization/design.md](../atendimento-minimal-sql-stabilization/design.md), [verification.md](../atendimento-minimal-sql-stabilization/verification.md).

| Capability | Contract (stable for Prontuario prerequisite) |
|------------|--------------------------------------------------|
| Create | `POST /Atendimento` — `CreateAtendimentoDto`: `PacienteId` (Guid), optional `MensagemParaMedico` → **201** + `ReadAtendimentoDto`; `EtapaAtual = Consulta` |
| Read | `GET /Atendimento/{id}` → **200** / **404** (soft-deleted excluded) |
| List by patient | `GET /Atendimento/paciente/{pacienteId}` → all non-deleted journeys |
| Update / delete | PUT (message only), DELETE (soft) — **not used** by Prontuario create path |
| FK validation | Invalid or soft-deleted `PacienteId` on create → **404** |
| IDs | **Guid** end-to-end on repository interface and API |
| Out of scope for Atendimento slice | Workflow, stage advancement, pendências, report PDF implementation (**501** routes preserved) |

**Prontuario repository/controller must validate on create:**

- `AtendimentoId` exists and is not soft-deleted
- `Atendimento.PacienteId == Prontuario.PacienteId`
- On failure: **404** (missing/deleted/mismatch) — Paciente/Atendimento precedent

## Research-sufficient criteria (session refinements)

### 1 — Who creates Atendimento before Prontuario?

**Backend (this SDD):** Client calls verified `POST /Atendimento`, then `POST /Prontuario` with explicit `AtendimentoId`. No auto-create inside Prontuario.

**Product / UX (out of scope for backend stabilization — document for WS07 and product):**

- After Paciente registration, UI may **guide** the user to open an Atendimento before clinical documentation (onboarding flow).
- On `POST /Prontuario` without a valid `AtendimentoId`, API returns **404** with a **generic** message (PHI-safe); Specify may add a stable error **code** (e.g. `atendimento_required`) for frontend to show "create atendimento to start clinical workflow" — **not** a new backend endpoint in this slice.

**Specify decision required:** Whether `400` + machine-readable code is preferred over `404` when `AtendimentoId` is omitted vs invalid — default **404** for consistency with Paciente FK pattern.

### 2 — What does Prontuario validate on AtendimentoId?

**In scope for Prontuario SQL Stabilization:**

- Existence, same `PacienteId`, not soft-deleted (persistence gate only).

**Out of scope — owned by `atendimento-workflow-stabilization` (after Prontuario verified):**

Stage evaluators read **Prontuario data** to drive journey state. Legacy `ValidacaoEtapaConsulta` (reference only) uses at minimum:

| Prontuario surface | Legacy / workflow use (indicative) |
|--------------------|-------------------------------------|
| `AtendimentoId` FK | Scope prontuarios to journey (adapt from legacy "all patient prontuarios") |
| `Tipo` (e.g. Consulta) | Select consulta prontuario for Consulta stage |
| `DataConsulta` | Consulta completion by date |
| `AcoesCD` / `ProntuarioAcaoCD` | CD pendency materialization |
| `Internacao` / procedures | Pre-procedimento procedure list (legacy denormalized path) |
| Pós-op prontuario | `ValidacaoEtapaPosProcedimento` — **net-new design** in workflow SDD |

**Prontuario SDD deliverable for downstream workflow:** persist queryable fields with stable types (especially `Tipo`, `DataConsulta`, CD collection, Internacao graph) and **`AtendimentoId` on every version row**. Do **not** implement evaluators or pendência writes in Prontuario Execute.

Cross-reference: [atendimento-workflow-stabilization/research.md](../atendimento-workflow-stabilization/research.md) — workflow blocked until Prontuario + Agendamento verified.

### 3 — Versioning: what PUT does ("new row" explained)

**Terminology:** "New row" means a **new database record** — new `Prontuario.Id` (Guid), new `Versao`, `ProntuarioAnteriorId` pointing to the prior version. The **previous row is never mutated** (immutable clinical snapshot per [Domain_Overview_Business_Rules.md](../../Architecture/Domain_Overview_Business_Rules.md)).

**Default for MVP2 backend stabilization (Recommend for Specify):**

| Action | Behavior |
|--------|----------|
| POST | Creates **version 1** for `(PacienteId, Versao)` sequence |
| PUT | Creates **successor version** (new Id, incremented `Versao`, same `AtendimentoId` unless Specify says otherwise); returns **201** + new `ReadProntuarioDto` |
| Prior version row | Unchanged — historical snapshot preserved |

**Open product questions (not blocking Research exit — resolve in Specify or WS07):**

| Question | Research recommendation |
|----------|-------------------------|
| Are there fields mutable **without** a new version? | **Default no** for clinical sections in MVP2 — domain: "Cada alteração clínica gera um novo prontuário." Non-clinical audit fields (`AtualizadoPor`) already accepted residual risk. |
| Should the physician **choose** "edit same version" vs "new version"? | **Defer to WS07/product.** Backend SDD default: PUT always creates successor (no dual mode in Execute). If product requires choice, add explicit API (e.g. `POST /Prontuario/{id}/versoes`) in a follow-up — not silent in-place PUT. |
| Partial PATCH without versioning? | **Out of scope** — no PATCH in this SDD |

**ADR candidate:** PUT semantic change vs legacy (201 + new Id) — evaluate in Specify/Design.

### 4 — Internacao without CID

**Execute / Verify:** When Internacao payload is present, unknown `CIDCodigo` → **400/404**; SQL integration tests insert **synthetic CID** in fixture setup (e.g. `Z99.9`).

**Accepted technical debt (Documentation Follow-Up — not blocking Specify):**

| Debt | Route to |
|------|----------|
| No production CID catalog | **CID Catalog Management** — PM backlog / future WS01 feature |
| Manual synthetic CID in tests only | Document in `migration-sql.md` Prontuario checklist + post-Verify DF |
| ERD / domain catalog gap | `erd.dbml` + PM item already identified in Part 2 |

### 5 — Explicit out of scope and future work registry

| Item | Disposition | Future owner |
|------|-------------|--------------|
| Atendimento repository/controller implementation | **Done** — verified SDD | — |
| Atendimento workflow / `ValidacaoEtapa*` / pendências | **Out of scope** | `atendimento-workflow-stabilization` after Prontuario + Agendamento verified |
| WS07 Blazor / UX onboarding (create Atendimento after Paciente) | **Out of scope** | WS07 |
| `POST /Prontuario/from-pdf` | **Out of scope** — extractor commented | Future feature: **Prontuario PDF ingestion** — requires updated extraction model (WS06); document in PM when model defined |
| PDF patient reports | **Out of scope** | Workflow/report feature |
| CID catalog | **Out of scope** | CID Catalog Management |
| Auth / RBAC | **Out of scope** | Future |
| Google Sheets | **Out of scope** | — |

**Note on PDF:** Research cannot close PDF ingestion until the extraction model is updated. Part 3 registers this as **follow-up feature**, not a gap in Prontuario SQL stabilization scope.

## Code audit summary (Pass 1 — 2026-06-18)

| Area | Finding | Still valid for Specify? |
|------|---------|--------------------------|
| `AtendimentoRepository` | SQL CRUD + soft delete + list by Paciente | ✓ Prerequisite — do not re-implement |
| `ProntuarioRepository` | Stub — `NotImplementedException` | ✓ Execute target |
| `CreateProntuarioDto` | No root `AtendimentoId` / `PacienteId` | ✓ Contract drift — Specify must require |
| `ProntuarioProfile` | Known unsafe mappings (Part 2) | ✓ Replace in Execute |
| Integration tests | No Prontuario tests; Atendimento pattern available | ✓ REQ in Specify |
| Infra | `SqlConnectionResolver`, `scripts/load-env.ps1`, `sql-integration-test.ps1` | ✓ Execution Prerequisites |

## Updated dependency table

| Dependency | Status |
|------------|--------|
| Paciente SQL stabilization | **Verified** |
| Atendimento Minimal SQL stabilization | **Verified** (2026-06-18) |
| `InitialCreate` schema | **Available** |
| Harness Wave 1–3 calibration | **Complete** |
| Docker SQL + `SA_PASSWORD` | Environment-dependent (runbook) |
| Prontuario repository/controller | **Not started** — this SDD |
| Agendamento SQL | Not required for Prontuario Execute |
| Atendimento Workflow | Not required for Prontuario Execute |

## Updated Specify Entry Checklist

Part 3 re-validates the Part 2 checklist after prerequisite reconciliation.

### Scope ownership

- [x] **Minimal Atendimento SQL** — **prerequisite verified**; not in Prontuario Execute scope
- [x] **Versioning IN SCOPE** — v1 on create; PUT creates successor row; `ProntuarioAnteriorId`; no diff/rollback/UI
- [x] **Legacy in-place update ABANDONED**; clinical sections **PRESERVED** in successor versions
- [x] **`POST /Prontuario/from-pdf` and PDF reports** — Out of Scope (future PDF ingestion feature)
- [x] **WS07 / DocFront.Web** — Out of Scope
- [x] **Full Atendimento workflow** — Out of Scope → `atendimento-workflow-stabilization`
- [x] **CID catalog** — Out of Scope; Internacao optional; CID required when Internacao present
- [x] **No new EF migration** — default unchanged

### Decisions for Specify (carry forward)

- [ ] **Version numbering:** per `PacienteId` (`IX_Prontuario_PacienteId_Versao`)
- [ ] **PUT semantics:** successor version; HTTP **201** + new `Id` (default)
- [ ] **Optional:** error code when `AtendimentoId` missing/invalid for UX messaging
- [ ] **No dual mode** "update same version" in MVP2 backend (product choice → WS07/follow-up)
- [ ] **List by paciente:** default all non-deleted versions
- [ ] **DELETE:** soft-delete single version only
- [ ] **`PacienteId` + `AtendimentoId`** on create at API root (Guid)
- [ ] **`Tipo`** canonical type (int recommended)
- [ ] **Internacao** full-field validation when section present
- [ ] **Child collections:** replace-on-new-version
- [ ] **DescricaoBasica snapshot** from `Paciente` where applicable
- [ ] **Workflow-readable fields** persisted for downstream evaluators (Tipo, DataConsulta, CD, Internacao) — no evaluators in this SDD

### Contract / mapping

- [ ] **Guid** for Prontuario repository interface and API (Atendimento already Guid)
- [ ] **Dedicated API DTOs** — no domain entity types in public DTOs
- [ ] **AutoMapper replacement** for Prontuario slice
- [ ] **`ReadProntuarioDto`** includes `Versao`, `ProntuarioAnteriorId`, `PacienteId`, `AtendimentoId`
- [ ] **Legacy behavior table** (REQ-008 pattern)

### Verification expectations

- [ ] Unit: factory, versioning chain, soft delete, invalid Atendimento/CID
- [ ] SQL integration: Paciente → **Atendimento (existing API)** → Prontuario v1 → PUT → v2 with nested children
- [ ] Swagger smoke — Prontuario endpoints (Atendimento smoke already verified)
- [ ] security-phi-review; domain-review for versioning

### PM / documentation

- [x] PM / migration-sql Atendimento-before-Prontuario — **applied**
- [ ] CID Catalog Management — log in Specify / PM follow-up
- [ ] ADR evaluation — versioning API PUT semantics
- [ ] PDF ingestion — log as future feature when model ready
- [ ] UX: guide create Atendimento after Paciente — WS07 / product note

## Readiness decision (Part 3)

### **READY FOR SPECIFY**

- Atendimento Minimal prerequisite **satisfied** and reconciled.
- Part 1/2 stale blocker and scope-ownership items **superseded**.
- Open items above are **Specify/Design/WS07/product** decisions — not a second Research pass.

**Next harness step:** Write `specify.md` using Part 2 domain decisions + Part 3 operational reconciliation + session refinements in **Research-sufficient criteria**.

---

## References

- `Documentation/State.md`
- `AGENTS.md`
- `Documentation/Product/PM_DocOrgano.md`
- `Documentation/Product/PRD.md`
- `Documentation/Technical/migration-sql.md`
- `Documentation/Architecture/Domain_Overview_Business_Rules.md`
- `Documentation/Architecture/ADR/ADR-001-soft-delete.md`
- `Documentation/SDD/paciente-sql-stabilization/` (pilot reference)
- `Documentation/SDD/atendimento-minimal-sql-stabilization/` (verified prerequisite)
- `Documentation/SDD/atendimento-workflow-stabilization/research.md` (downstream consumer)
- `Documentation/AI-Harness/Harness-Design/sdd-operational.md`
- `DocAPI/Legacy/_LegacySheetsDb/ProntuarioSheetsRepository.cs` (behavioral reference only)
- `DocAPI/Legacy/_LegacySheetsDb/AtendimentoSheetsRepository.cs` (`ValidacaoEtapa*` reference only)
- `DocAPI/Infrastructure/Repositories/ProntuarioRepository.cs`
- `DocAPI/Infrastructure/Repositories/AtendimentoRepository.cs`
- `DocAPI/Application/Mappings/Profiles/ProntuarioProfile.cs`
