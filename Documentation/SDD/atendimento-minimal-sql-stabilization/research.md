# Research — Atendimento Minimal SQL Stabilization

> Feature SDD: `Documentation/SDD/atendimento-minimal-sql-stabilization/`  
> Generated SDD artifacts are written in English.  
> **Phase:** Research (complete)  
> **Lifecycle position:** Research → Specify → Design → Tasks → SDD Pre-Execution Review → Execute → Verify → Documentation Follow-Up → Reporting → Teacher Guide

This document consolidates:

1. **Feature Discovery & Scope Decomposition** (prior session) — boundary analysis for the full Atendimento vertical.
2. **Research Consolidation** — validation of nine approved architectural decisions against codebase and governance; authoritative input for Specify.

When Part 1 and Part 2 conflict, **Part 2 governs**.

---

# Part 1 — Feature Discovery Summary (Minimal Slice)

## Executive Summary

The PM item **WS01 — Atendimento SQL Migration & Stabilization** conflates two independent concerns. Prontuario Research (Decision 1) and schema FK constraints require **real Atendimento rows** before Prontuario or Agendamento can persist. The **minimal slice** isolates persistence-only work: SQL CRUD for the `Atendimento` aggregate root so downstream verticals receive a valid `AtendimentoId`.

**Current state:** `AtendimentoRepository` is fully stubbed (all methods throw). `AtendimentoController` is active but non-functional. Paciente SQL is verified. Entity model is refactored (`EtapaAtual` enum, child collections) and is the persistence target.

**Recommended boundary:** Backend stabilization SDD covering `AtendimentoRepository` SQL CRUD, `AtendimentoController` hardening (Guid, PHI-safe, status codes), Paciente FK validation, soft delete (ADR-001), SQL integration tests, and Swagger smoke — following the Paciente pilot pattern. **No workflow, validations, or journey orchestration.**

**Sizing:** Medium (smaller than Paciente; single root table, no nested child persistence in this slice).

---

## Feature Boundary (Minimal)

### In boundary

| Item |
|------|
| `AtendimentoRepository` SQL implementation against `InitialCreate` schema |
| EF config review (`AtendimentoConfiguration`) |
| `AtendimentoController` CRUD hardening (Guid routes, PHI-safe logging, status codes) |
| `IAtendimentoRepository` Guid contract alignment |
| Create with `PacienteId` validation (patient exists, not soft-deleted) |
| Read by id, paginated list, list/filter by `PacienteId` |
| Update limited to safe root fields (`MensagemParaMedico`, audit metadata — not workflow-driven stage changes) |
| Soft delete (ADR-001) |
| Dedicated API DTOs (slim create/read; abandon legacy denormalized fields) |
| Rich domain factory/update methods on `Atendimento` entity (Paciente pattern) |
| Unit + SQL integration tests |
| Swagger smoke checklist |
| Legacy characterization for CRUD-only surface |

### Out of boundary

| Item |
|------|
| `ValidacaoEtapa*` workflow (→ `atendimento-workflow-stabilization`) |
| Pendência engine, `ClinicalEvent` writes, checklist execution |
| Senhas integration, PDF report endpoints (`report-id`, `followUp-id`) |
| Child entity persistence (`AtendimentoPendencia`, `ClinicalEvent`, `ChecklistExecution`) |
| WS07 Blazor integration |
| Prontuario / Agendamento repository implementation |
| Google Sheets import |
| Auth/RBAC (`AtualizadoPor` unset — accepted residual risk) |
| New EF migrations (default: none) |
| Auto-create Atendimento inside Prontuario or Agendamento create |

---

## Domain Discovery (Minimal)

### Business capability

Provision a **care journey container** (`Atendimento`) linked to a `Paciente`, establishing the FK anchor required by Prontuario and Agendamento without implementing journey logic.

### Aggregate responsibilities (Research evaluation — Decision 04)

| Responsibility | Owner in minimal slice |
|----------------|------------------------|
| Identity (`Id`, `PacienteId`) | `Atendimento` aggregate root |
| Initial stage (`EtapaAtual = Consulta` on create) | Entity factory — not workflow |
| Physician message field storage | Entity — persistence only |
| Stage progression rules | **Out of scope** — workflow SDD |
| Pendência identification | **Out of scope** |
| Clinical event recording | **Out of scope** |
| Cross-aggregate validation | **Out of scope** |

### Approved business rules affecting minimal slice

| Rule | Source | Minimal handling |
|------|--------|------------------|
| Paciente 1:N Atendimento | Decision 09 | Schema + API support multiple rows per patient |
| Multiple concurrent active journeys | Decision 08 | No uniqueness constraint on active atendimento per patient |
| Soft delete preserves history | ADR-001 | Global query filter on `Atendimento` |
| Care journey starts at Consulta | Domain Overview | Factory sets `EtapaAtual.Consulta` |

### Legacy structure vs behavior (Decision 03)

| Dimension | Treatment |
|-----------|-----------|
| Legacy nested stage objects (`EtapaConsulta`, etc.) | **Abandoned** — not recreated |
| Legacy denormalized `ProntuariosId` / `AgendamentosId` on DTO | **Abandoned** — not persisted |
| Legacy string IDs | **Abandoned** → Guid |
| Legacy Sheets row shape | **Abandoned** |
| CRUD surface existence | **Preserved** (create, read, list, update, delete) |
| Paciente linkage on create | **Preserved** |
| Soft delete over physical delete | **Preserved** (ADR upgrade) |

---

## Legacy Characterization (CRUD only)

| Behavior | Source | Decision | Notes |
|----------|--------|----------|-------|
| `GetAllAsync(skip, take)` | `AtendimentoSheetsRepository` | Preserve | Paginated list |
| `GetByIdAsync` | Same | Preserve | Guid PK |
| `CreateAsync` | Same | Preserve | Requires valid Paciente |
| `UpdateAsync` | Same | Adapt | Slim field set; no stage orchestration |
| `DeleteAsync` | Same | Adapt | Soft delete per ADR-001 |
| `CreateReportByIdAsync` | Same | Abandon | Out of scope; PDF service commented |
| `CreateReportFollwUpByIdAsync` | Same | Abandon | Never implemented in Legacy |
| `ValidacaoEtapa*` methods | Same | Separate SDD | `atendimento-workflow-stabilization` |
| PHI `Console.WriteLine` in controller | `AtendimentoController` | Abandon | Remediate per Paciente pattern |

---

## Architecture Discovery

### Layers affected

| Layer | Impact |
|-------|--------|
| `Core/Entities/Atendimento` | Medium — factory, update, soft-delete methods |
| `Core/Interfaces/Repositories/IAtendimentoRepository` | High — Guid alignment |
| `Infrastructure/Repositories/AtendimentoRepository` | High — primary target |
| `Infrastructure/SqlDb/Configurations/AtendimentoConfig` | Low — review only |
| `Application/Data/Dtos/Atendimento` | High — slim dedicated DTOs |
| `Application/Mappings/AtendimentoProfile` | High — replace unsafe maps |
| `API/Controllers/AtendimentoController` | High — Guid, PHI, status codes; **disable or 501 report routes** |
| `DocFront.Web` | None (Backend Stabilization Rule) |

### Not involved

- `AtendimentoPendencia`, `ClinicalEvent`, `ChecklistExecution` tables (no writes in this slice)
- `ProntuarioRepository`, `AgendamentoRepository`
- `PdfGeneratorService`, `FileDataOfSenhaExtractorService`
- Legacy Sheets runtime

---

## Data Discovery

### Tables

| Table | Minimal slice usage |
|-------|---------------------|
| `Atendimento` | Full CRUD |
| `Paciente` | FK validation on create |

### Schema notes

- `EtapaAtual` stored as `varchar(50)` via enum string conversion (existing config).
- No unique index preventing multiple active Atendimentos per `PacienteId` — aligns with Decision 08.
- `Prontuario.AtendimentoId` and `Agendamento.AtendimentoId` are NOT NULL FKs — minimal create unblocks both.

### Data risks

| Risk | Severity | Mitigation |
|------|----------|------------|
| Create with invalid `PacienteId` | High | Repository returns domain error → 404 |
| Soft-deleted Atendimento referenced by new Prontuario | Medium | Validate not deleted on downstream create (Prontuario SDD) |
| AutoMapper drops `PacienteId` | High | Factory method; audit mapper |

---

## API Discovery

### Endpoints in scope

| Method | Route | Target behavior |
|--------|-------|-----------------|
| POST | `/Atendimento` | 201 + `ReadAtendimentoDto`; body: `{ pacienteId }` (+ optional `mensagemParaMedico`) |
| GET | `/Atendimento/{id}` | 200 / 404 |
| GET | `/Atendimento` | 200 paginated list |
| GET | `/Atendimento/paciente/{pacienteId}` | 200 — all non-deleted atendimentos for patient (supports multi-journey + Prontuario UX) |
| PUT | `/Atendimento/{id}` | 204 — limited root field update |
| DELETE | `/Atendimento/{id}` | 204 soft delete |

### Endpoints out of scope (defer or 501)

| Method | Route | Action |
|--------|-------|--------|
| GET | `/Atendimento/report-id/{id}` | Out of scope — return 501 or remove from active Swagger group |
| GET | `/Atendimento/followUp-id/{id}` | Out of scope — same |

### Contract ambiguities resolved in Research

| Ambiguity | Resolution |
|-----------|------------|
| String vs Guid IDs | **Guid** — align with Paciente pilot |
| Legacy create DTO fields | **Abandon** `NomePaciente`, `ProntuariosId`, `AgendamentosId`, `EtapaAtualAtendimento` on create |
| 201 response body | Return `ReadAtendimentoDto` (Paciente pattern), not raw entity |
| List by patient | **In scope** — required for multi-journey (Decision 08) and Prontuario `AtendimentoId` selection |

---

## Security & PHI Discovery

| Risk | Location | Treatment |
|------|----------|-----------|
| `Console.WriteLine` with user context | `AtendimentoController` | Remove in Execute |
| Patient name in error paths | Report handlers | Out of scope — disable routes |
| Test fixtures | Tests | Synthetic data only |

**Security/PHI review required:** Yes (controller remediation).

---

## Verification Discovery

| Gate | Evidence |
|------|----------|
| `dotnet build` | Pass |
| `dotnet test` | Unit tests: create, read, update, soft delete, invalid PacienteId |
| SQL integration | `AtendimentoSqlIntegrationTests` — CRUD round-trip against Docker SQL |
| Swagger smoke | CRUD + list-by-paciente + status codes |
| security-phi-review | Touched controller code |
| Legacy table | CRUD preserve/adapt/abandon documented |

### Test fixture pattern

Reuse Paciente SQL integration fixture: create Paciente → create Atendimento → assert FK → soft delete → assert excluded.

---

# Part 2 — Research Consolidation

## Approved Decisions Validation

### Decision 01 — Split into two SDDs

**Validated.** Minimal slice scope matches FK prerequisite for Prontuario (`Prontuario.AtendimentoId NOT NULL`) and Agendamento (`Agendamento.AtendimentoId NOT NULL`). Workflow concerns are cleanly separable.

**No critical contradiction discovered.**

### Decision 02 — Approved sequencing

```
Paciente SQL Stabilization (verified)
  → Atendimento Minimal SQL Stabilization
  → Prontuario SQL Stabilization
  → Agendamento SQL Stabilization
  → Atendimento Workflow Stabilization
  → WS07 Frontend
```

**Why this resolves dependencies:**

| Step | Unblocks |
|------|----------|
| Paciente verified | Valid `PacienteId` FK for Atendimento create |
| Atendimento Minimal | `AtendimentoId` FK for Prontuario and Agendamento persistence |
| Prontuario SQL | `ValidacaoEtapaConsulta` reads prontuarios; Prontuario rows exist for workflow |
| Agendamento SQL | `ValidacaoPreProcedimento` / `ValidacaoEtapaProcedimento` read agendamentos; senha fields on entity replace file hack |
| Atendimento Workflow | Cross-aggregate reads hit real SQL repos, not stubs |
| WS07 | Stable backend contracts for all four aggregates |

Prontuario Research Decision 1 ("Create Atendimento → Create Prontuario") is satisfied by **minimal create**, not full workflow.

### Decision 03 — Domain authoritative; abandon legacy structure

**Validated for minimal slice.**

- **Abandoned:** nested stage objects, denormalized ID lists, string PKs, Sheets serialization.
- **Preserved:** CRUD capability, Paciente linkage, journey container semantics, soft delete intent.

### Decision 04 — Rich Domain Model

**Validated.** Current `Atendimento` entity has partial behavior (`AvancarEtapa`, `AdicionarPendencia`) but lacks Paciente-style factories and update methods.

**Research recommendation for minimal slice:**

| Method | Purpose |
|--------|---------|
| `Atendimento.Criar(Guid pacienteId)` | Factory; sets `EtapaAtual = Consulta`, `CriadoEm` |
| `AtualizarMensagem(string?)` | Limited PUT support |
| `AplicarSoftDelete()` | ADR-001 |
| `AvancarEtapa()` | **Do not expose via API in minimal slice** — workflow owns stage changes |

Repository persists entity state only; no business orchestration in repository.

### Decision 05 — Workflow logic not in aggregate

**Applies to workflow SDD, not minimal slice.** Minimal slice correctly limits entity to factory + field updates. Stage progression via `AvancarEtapa()` must not be invoked from controller in this SDD.

### Decisions 06–07

Not applicable to minimal slice (no stage validation, no `ValidacaoEtapaPosProcedimento`).

### Decision 08 — Multiple active Atendimentos

**Validated against schema.** No unique constraint on `(PacienteId)` where active.

**Minimal slice impacts:**

| Area | Impact |
|------|--------|
| API | `GET /Atendimento/paciente/{pacienteId}` returns collection, not singleton |
| Create | No "close previous journey" logic in minimal slice |
| Prontuario create | Caller must supply explicit `AtendimentoId` — not inferred from patient alone |
| Tests | Cover two active atendimentos for same patient |

### Decision 09 — Atendimento as care journey (1:N)

**Validated.** `Paciente.Atendimentos` navigation exists; schema supports historical and concurrent journeys.

---

## Architecture Recommendation (Minimal)

| Concern | Recommendation |
|---------|----------------|
| Persistence | `AtendimentoRepository` + `DocDbContext`; ADR-001 global filter |
| Domain | Rich entity factories; private setters; no workflow in entity |
| API | Guid routes; dedicated DTOs; Paciente-controller status code parity |
| Mapping | Explicit factory mapping on create; avoid blind AutoMapper for write paths |
| Report routes | Explicitly out of scope; do not implement in Execute |
| Interface | Migrate `IAtendimentoRepository` to `Guid` parameters (breaking at repository layer; no external consumers on SQL branch) |

---

## Risk Assessment (Research)

| Risk | Class | Prob. | Impact | Mitigation |
|------|-------|-------|--------|------------|
| Scope creep into workflow | Governance | Medium | High | Explicit Out of Scope; separate SDD slug |
| Prontuario blocked without this slice | Dependency | High | High | PM sequencing update; execute before Prontuario |
| AutoMapper contract drift | Technical | High | Medium | Factory + audited read mapping |
| PHI in controller | Security | Medium | High | security-phi-review gate |
| Agents assume full Atendimento done after minimal | Governance | Medium | Medium | Distinct SDD folders + PM split |
| `AvancarEtapa()` called from API accidentally | Domain | Low | Medium | Out of scope in Specify; no controller endpoint |

---

## Sizing

| Field | Decision |
|-------|----------|
| Size | **Medium** |
| Rationale | Single-table aggregate CRUD with Guid/PHI/DTO alignment and SQL integration tests. No nested children, no cross-aggregate orchestration. Smaller than Paciente (no search variants, no duplicate-key policy). Larger than trivial stub replacement due to interface migration and multi-journey list endpoint. |
| Required phases | Specify / Design / Tasks / Execute / Verify |
| Escalation triggers | Schema migration required; child entity persistence pulled in; workflow validations pulled in |

---

## Research Conclusions (Minimal)

1. Minimal Atendimento SQL is a **hard prerequisite** for Prontuario and Agendamento Execute — not optional supporting work.
2. All nine approved decisions are **compatible** with minimal slice scope; Decisions 05–07 apply primarily to workflow SDD.
3. Legacy structure is **abandoned**; CRUD behavior is **selectively preserved**.
4. `GET /Atendimento/paciente/{pacienteId}` is **in scope** to support Decision 08 and explicit `AtendimentoId` selection downstream.
5. Report/followUp endpoints remain **deferred** — non-functional and out of scope.

---

## Readiness Decision

### **READY FOR SPECIFY**

Specify may begin immediately. No additional discovery pass required unless stakeholders reject Guid migration or list-by-paciente endpoint.

### Blocked

None for this slice (Paciente upstream is verified).

### Deferred (explicit)

| Item | Owner SDD |
|------|-----------|
| Workflow validations | `atendimento-workflow-stabilization` |
| Pendências / ClinicalEvents persistence | Workflow SDD |
| PDF / followUp endpoints | Future feature |
| WS07 frontend | WS07 |

---

## Specify Entry Checklist

Before writing `specify.md`, confirm:

### Scope ownership

- [ ] CRUD + soft delete + list-by-paciente only — **no** `ValidacaoEtapa*`, **no** child entity writes.
- [ ] Report/followUp routes Out of Scope (501 or undocumented).
- [ ] WS07 Out of Scope (Backend Stabilization Rule).
- [ ] No auto-create inside Prontuario/Agendamento.
- [ ] No new EF migration (default).

### Decisions recorded in Specify

- [ ] Guid IDs on repository interface and controller.
- [ ] Slim `CreateAtendimentoDto`: `PacienteId` required; optional `MensagemParaMedico`.
- [ ] Factory sets `EtapaAtual = Consulta` on create.
- [ ] Multiple active atendimentos per patient supported; no uniqueness constraint.
- [ ] `AvancarEtapa()` not exposed via API.
- [ ] Legacy CRUD characterization table (REQ-008 pattern).

### Verification expectations

- [ ] Unit tests: create, read, update, soft delete, invalid PacienteId, two atendimentos same patient.
- [ ] SQL integration: Paciente → Atendimento round-trip.
- [ ] Swagger smoke for in-scope endpoints.
- [ ] security-phi-review on `AtendimentoController`.

### Downstream consumers (acknowledged)

- [ ] Prontuario SDD depends on this slice for `AtendimentoId` FK.
- [ ] Agendamento SDD depends on this slice for `AtendimentoId` FK.

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
- `Documentation/SDD/prontuario-sql-stabilization/research.md` (Atendimento FK prerequisite)
- `Documentation/AI-Harness/Harness-Design/sdd-operational.md`
- `DocAPI/Infrastructure/Repositories/AtendimentoRepository.cs`
- `DocAPI/Legacy/_LegacySheetsDb/AtendimentoSheetsRepository.cs` (CRUD reference only)
