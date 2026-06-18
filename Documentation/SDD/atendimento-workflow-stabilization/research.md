# Research — Atendimento Workflow Stabilization

> Feature SDD: `Documentation/SDD/atendimento-workflow-stabilization/`  
> Generated SDD artifacts are written in English.  
> **Phase:** Research (complete)  
> **Lifecycle position:** Research → Specify → Design → Tasks → SDD Pre-Execution Review → Execute → Verify → Documentation Follow-Up → Reporting → Teacher Guide

This document consolidates:

1. **Feature Discovery & Scope Decomposition** (prior session) — full Atendimento vertical analysis.
2. **Research Consolidation** — validation of nine approved architectural decisions; workflow architecture recommendation; authoritative input for Specify.

When Part 1 and Part 2 conflict, **Part 2 governs**.

**Prerequisite SDD:** `atendimento-minimal-sql-stabilization` (verified) plus `prontuario-sql-stabilization` and `agendamento-sql-stabilization` (verified).

---

# Part 1 — Feature Discovery Summary (Workflow Slice)

## Executive Summary

Atendimento is the **care journey orchestrator** for the clinic workflow: Consulta → Pré-Procedimento → Procedimento → Pós-Procedimento. Legacy encodes ~300+ lines of cross-aggregate validation in `AtendimentoSheetsRepository` (`ValidacaoEtapaConsulta`, `ValidacaoPreProcedimento`, `ValidacaoEtapaProcedimento`, `ValidacaoEtapaPosProcedimento`). The refactored entity model uses `EtapaAtual`, `AtendimentoPendencia`, and `ClinicalEvent` instead of nested stage objects.

**Current state:** Workflow logic exists only in commented Legacy. SQL `AtendimentoRepository` is stubbed. Minimal SQL slice (separate SDD) will provide FK anchor only. Validations require **live** Prontuario and Agendamento SQL repositories.

**Recommended boundary:** Application-layer workflow orchestration (not repository, not aggregate methods) that reads cross-aggregate data, computes stage status, persists `Pendencias` and `ClinicalEvents`, updates `EtapaAtual` and `MensagemParaMedico`, and exposes journey projections via API — with characterization tests against Legacy behavior where meaningful.

**Sizing:** Large (cross-aggregate orchestration, behavior port, characterization burden; comparable to or greater than Paciente due to rule complexity).

---

## Feature Boundary (Workflow)

### In boundary

| Item |
|------|
| Port legacy validation **behavior** into application workflow layer |
| `ValidacaoEtapaConsulta` behavior (Paciente + Prontuario Tipo Consulta + CD pendencies) |
| `ValidacaoPreProcedimento` behavior (Agendamento list + pre-op status flags) |
| `ValidacaoEtapaProcedimento` behavior (procedure completion, instruções, atestado, pós-op date) |
| `ValidacaoEtapaPosProcedimento` — **net-new domain design** (Decision 07) |
| Pendência persistence (`AtendimentoPendencia`) |
| Clinical event recording (`ClinicalEvent`) on meaningful transitions |
| `MensagemParaMedico` updates |
| `EtapaAtual` progression (domain-approved stage transitions) |
| Journey projection DTO (computed read model — replaces legacy nested stage DTOs) |
| Explicit workflow invocation API endpoint(s) |
| Characterization tests documenting legacy outcomes |
| Unit + integration tests with seeded Prontuario/Agendamento fixtures |
| PHI-safe logging in workflow path |
| Legacy behavior table (preserve/adapt/abandon per rule) |

### Out of boundary

| Item |
|------|
| Minimal SQL CRUD (→ `atendimento-minimal-sql-stabilization`) |
| Prontuario / Agendamento repository implementation |
| PDF report endpoints |
| `CreateReportFollwUpByIdAsync` |
| Senhas file scraping (`FileDataOfSenhaExtractorService` live integration) — **adapt** to Agendamento `SenhaAgendamento` fields |
| `ChecklistExecution` / configurable checklists (MVP3) |
| Formal state machine framework |
| WS07 frontend |
| Google Sheets import |
| Auth/RBAC |
| New EF migrations (default: none) |

---

## Domain Discovery (Workflow)

### Business capability

Monitor patient care journey progress, identify pendências, consolidate physician messaging, and gate stage progression based on clinical and administrative prerequisites.

### Aggregate and ownership evaluation (Decision 04)

| Concept | Owner | Rationale |
|---------|-------|-----------|
| `Atendimento` identity, `PacienteId`, soft delete | Aggregate root | Unchanged from minimal slice |
| `EtapaAtual` value | Aggregate root | Single source of truth for current stage; mutated only through approved transition rules |
| `MensagemParaMedico` | Aggregate root | Operational alert string; set by workflow |
| `AtendimentoPendencia` collection | Aggregate root (child entities) | Materialized blockers; add/resolve via entity methods |
| `ClinicalEvent` collection | Aggregate root (child entities) | Audit trail of journey transitions |
| Cross-aggregate reads (Prontuario, Agendamento) | **Application workflow** | External aggregates; read-only in this feature |
| Validation orchestration | **Application workflow** | Decision 05 — not in repository, not `atendimento.ValidacaoEtapa*()` on entity |
| Journey projection (read DTO) | **Application mapper/projection** | Computed; not persisted denormalized shape |
| Stage object graphs (`EtapaConsulta`, etc.) | **Abandoned** | Decision 06 |

### Invariants (workflow SDD must enforce)

| Invariant | Notes |
|-----------|-------|
| Workflow operates on explicit `AtendimentoId` | Decision 08 — not inferred from `PacienteId` alone |
| Stage transitions follow enum order | `Consulta → PreProcedimento → Procedimento → PosProcedimento → Finalizado` |
| Cannot advance past `Finalizado` | Entity already throws |
| Pendências may block stage advancement | Domain rule |
| Multiple concurrent journeys per patient | Each workflow run scoped to one `AtendimentoId` |
| Historical journeys remain queryable | Soft-deleted or `Finalizado` atendimentos excluded from active workflow default |

### Legacy behavior mapping (Decision 03)

| Legacy behavior | Treatment |
|-----------------|-----------|
| Find latest Consulta prontuario for patient | **Adapt** — scope to atendimento's linked prontuarios (by `AtendimentoId` FK), not all patient prontuarios |
| CD pendency list from prontuario `AcoesCD` | **Preserve** intent → materialize as `AtendimentoPendencia` or projection |
| Pre-op status flags (encaminhamento, exames, termo, etc.) | **Preserve** intent → pendências + projection |
| Senhas from JSON file | **Adapt** → read `Agendamento.SenhaAgendamento` after Agendamento SQL |
| `Local.Equals("0")` placeholder for instruções/atestado | **Abandon** as implementation — use `Agendamento.InstrucaoStatus`, `AtestadoStatus` |
| `ValidacaoEtapaPosProcedimento` empty stub | **New design** — not migration (Decision 07) |
| Patient name in `MensagemParaMedico` | **Preserve** clinical messaging (field content, not logs) |
| Implicit refresh on read | **Adapt** — explicit workflow invocation endpoint |

### Unknown behavior requiring Specify decisions

| Item | Research default |
|------|------------------|
| Workflow trigger | Dedicated `POST /Atendimento/{id}/atualizar-jornada` (or equivalent) |
| Auto-refresh on GET | **No** — explicit invocation only (predictable, testable) |
| Pendência deduplication | Replace pendências on each refresh vs merge — default: replace by `Tipo` |
| Which transitions auto-advance `EtapaAtual` | Specify must define per-stage completion criteria |
| Pós-procedimento completion criteria | Net-new — derive from domain doc + clinical input in Specify |

---

## Legacy Characterization (Workflow)

### Validation methods

| Method | LOC (approx) | Legacy deps | Port class |
|--------|--------------|-------------|------------|
| `ValidacaoEtapaConsulta` | ~100 | Paciente, Prontuario (Tipo Consulta, CD) | Preserve behavior, adapt data sources |
| `ValidacaoPreProcedimento` | ~100 | Agendamento list, Senhas file | Preserve behavior, adapt senhas → Agendamento |
| `ValidacaoEtapaProcedimento` | ~95 | Agendamento status | Preserve intent, abandon placeholder logic |
| `ValidacaoEtapaPosProcedimento` | ~5 (empty) | Agendamento, Prontuario | **New design** |

### Classification table

| Behavior | Preserve | Adapt | Abandon | New design |
|----------|----------|-------|---------|------------|
| CD pendency detection | ✓ | → Pendencias | | |
| Consulta completion by date | ✓ | Atendimento-scoped prontuario | | |
| Pre-op checklist flags | ✓ | → Pendencias/projection | | |
| Senhas file matching | | ✓ → Agendamento senha | ✓ file path | |
| Procedure status from Agendamento | ✓ | Real enum fields | | |
| Instruções/atestado via `Local=="0"` | | | ✓ | |
| Nested stage DTOs on read | | | ✓ | |
| Pos-procedimento follow-up | | | | ✓ |
| Validation in repository | | | ✓ | |
| `atendimento.ValidacaoEtapa*()` on entity | | | ✓ | |

---

## Architecture Discovery

### Workflow placement recommendation (Decision 05)

**Evaluated patterns:**

| Pattern | Fit | Assessment |
|---------|-----|------------|
| Logic inside `Atendimento` aggregate | Poor | Violates Decision 05; couples aggregate to Prontuario/Agendamento |
| Logic inside `AtendimentoRepository` | Poor | Legacy anti-pattern; untestable orchestration |
| Domain Service (pure rules) | Partial | Good for stage transition rules without I/O; insufficient alone for cross-aggregate reads |
| Workflow Service (single god class) | Moderate | Works but risks MVP2 over-abstraction |
| **Application Use Cases (recommended)** | **Best** | Matches Paciente philosophy and harness guidance; explicit, testable, no Mediator |

**Research recommendation:**

```text
Application/UseCases/Atendimento/
├── AtualizarJornadaAtendimentoUseCase.cs    # orchestrator entry point
├── EtapaConsultaWorkflow.cs               # stage evaluator (or private methods)
├── EtapaPreProcedimentoWorkflow.cs
├── EtapaProcedimentoWorkflow.cs
├── EtapaPosProcedimentoWorkflow.cs          # new design
└── AtendimentoJourneyProjection.cs          # read model builder
```

- **Use case** loads `Atendimento` by id, reads Prontuario/Agendamento via repository interfaces, runs stage evaluators, mutates aggregate (pendencias, events, etapa, message), persists via `IAtendimentoRepository`.
- **Domain service** (optional, small): `StageTransitionPolicy` — pure rules for whether `EtapaAtual` may advance (no repository calls).
- **Not recommended:** reviving `atendimento.ValidacaoEtapaConsulta(paciente, …)` as entity methods.

### Stage representation (Decision 06)

| Legacy construct | Target representation |
|------------------|----------------------|
| `EtapaConsulta.StatusGeral`, `CdPendente`, etc. | `EtapaAtual` + `Pendencias` + `JourneyProjectionDto.Consulta` (computed section) |
| `EtapaPreProcedimento.StatusSenha`, etc. | `Pendencias` + projection section |
| `EtapaProcedimento` status object | `Pendencias` + projection + `ClinicalEvent` |
| `EtapaPosProcedimento` | New design → pendências + projection |
| `ProntuariosId` / `AgendamentosId` lists | Query-time: prontuarios/agendamentos where `AtendimentoId` matches |

### Layers affected

| Layer | Impact |
|-------|--------|
| `Application/UseCases` | **New** — primary workflow implementation |
| `Core/Entities/Atendimento` | Medium — pendência/event methods; controlled `AvancarEtapa` or explicit `DefinirEtapa` |
| `Infrastructure/Repositories/AtendimentoRepository` | Medium — load/save includes for Pendencias, Eventos |
| `Application/DTOs` | High — `JourneyProjectionDto` replaces legacy nested stage DTOs on read |
| `API/Controllers` | Medium — workflow endpoint; extend read DTO |
| `DocAPI.Tests` | High — characterization + integration |

### Not involved

- `ChecklistDefinition` seed / execution engine
- `PdfGeneratorService`
- `FileDataOfSenhaExtractorService` (live path)
- WS07

---

## Data Discovery

### Tables written in workflow slice

| Table | Usage |
|-------|-------|
| `Atendimento` | Update `EtapaAtual`, `MensagemParaMedico`, audit fields |
| `AtendimentoPendencia` | Insert/replace on refresh |
| `ClinicalEvent` | Append on transitions |

### Tables read (cross-aggregate)

| Table | Usage |
|-------|-------|
| `Paciente` | Cadastro confirmation |
| `Prontuario` | Consulta tipo, CD actions, pós-op prontuario (new design) |
| `Agendamento` | Dates, status, senha, instruções, atestado |

### Decision 08 — multiple journeys data impact

| Concern | Implication |
|---------|-------------|
| Prontuario linkage | Filter `Prontuario WHERE AtendimentoId = @id` — not all patient prontuarios |
| Agendamento linkage | Filter `Agendamento WHERE AtendimentoId = @id` |
| Legacy assumed patient-level scan | **Adapt** — workflow is per-atendimento |
| Reporting | Reports must include `AtendimentoId` context |
| UI (WS07) | Patient tab must list journeys; user picks atendimento before refresh |
| Future validations | All rules keyed by `AtendimentoId` |

---

## API Discovery

### New / extended endpoints (workflow)

| Method | Route | Purpose |
|--------|-------|---------|
| POST | `/Atendimento/{id}/atualizar-jornada` | Run workflow orchestration; persist pendencias/events/stage/message |
| GET | `/Atendimento/{id}` | Return `ReadAtendimentoJourneyDto` with computed projection (minimal CRUD read may be extended) |

### Read model shape (abandon legacy DTO)

**Abandon** on API surface:

- `EtapaConsulta`, `EtapaPreProcedimento`, `EtapaProcedimento`, `EtapaPosProcedimento` nested objects
- Denormalized `ProntuariosId`, `AgendamentosId` on write DTOs

**Introduce:**

```text
ReadAtendimentoJourneyDto
├── id, pacienteId, etapaAtual, mensagemParaMedico
├── pendencias[]          # persisted
├── eventos[]             # persisted (or summary)
└── projection            # computed stage sections for UI (optional nested object)
```

### Contract ambiguities for Specify

| Item | Research default |
|------|------------------|
| Workflow HTTP status on pendências | 200 with journey DTO; pendências indicate blockers (not 409 unless Specify chooses) |
| Idempotent refresh | Yes — repeated POST recalculates state |
| Partial stage skip | No — must complete criteria or remain with pendências |

---

## Security & PHI Discovery

| Risk | Mitigation |
|------|------------|
| `MensagemParaMedico` contains patient names | Allowed in persisted clinical field; not in logs |
| Legacy `Console.WriteLine` in senha matching | Abandon; no file-based senha in workflow |
| Workflow logs | Structured logging without PHI |
| Cross-aggregate reads | No patient names in debug output |

**Security/PHI review required:** Yes.

**Characterization tests:** Synthetic names only.

---

## Verification Discovery

| Gate | Evidence |
|------|----------|
| Characterization tests | Legacy rule outcomes documented per stage (WS03 P0) |
| Unit tests | Stage evaluators with mocked repositories |
| Integration tests | Seeded Paciente → Atendimento → Prontuario → Agendamento → workflow POST |
| SQL integration | Full journey refresh against Docker SQL |
| Swagger smoke | Workflow endpoint |
| security-phi-review | Workflow + controller |
| domain-review | Stage ownership, pendência model, pos-procedimento new design |

### Upstream fixture requirements

Integration tests require verified minimal Atendimento + Prontuario + Agendamento SQL paths.

---

# Part 2 — Research Consolidation

## Approved Decisions Validation

### Decision 01 — Two independent SDDs

**Validated.** Workflow SDD assumes minimal SQL SDD is complete. No CRUD re-implementation unless gaps found in Verify.

### Decision 02 — Sequencing

**Validated.** Workflow Execute is **blocked** until:

1. `atendimento-minimal-sql-stabilization` — verified
2. `prontuario-sql-stabilization` — verified (Consulta validation reads prontuarios)
3. `agendamento-sql-stabilization` — verified (Pré-OP/Procedimento validations read agendamentos; senha fields replace file integration)

### Decision 03 — Domain authoritative

**Validated.**

| Dimension | Workflow treatment |
|-----------|-------------------|
| Legacy structure | **Abandoned** |
| Legacy behavior | **Selectively preserved** via characterization table |
| Legacy DTO shape | **Abandoned** → journey projection |
| Legacy repository pattern | **Abandoned** → use cases |

### Decision 04 — Rich Domain Model

**Validated.** Aggregate owns pendências and events; workflow orchestrates cross-aggregate reads and invokes entity methods:

| Entity method (indicative) | Purpose |
|----------------------------|---------|
| `RegistrarPendencia(tipo, descricao)` | Materialize blocker |
| `LimparPendencias()` / `SubstituirPendencias(...)` | Refresh semantics |
| `RegistrarEvento(tipo, descricao, usuario)` | Clinical audit |
| `DefinirMensagemParaMedico(msg)` | Physician alert |
| `DefinirEtapa(etapa)` / controlled `AvancarEtapa()` | Stage update when criteria met |

### Decision 05 — Workflow not in aggregate

**Validated.** Use case orchestration is the target. Legacy `atendimento.ValidacaoEtapaConsulta(...)` pattern is **rejected**.

### Decision 06 — No legacy stage objects

**Validated.** Representation via `EtapaAtual` + `Pendencias` + `ClinicalEvents` + computed projection.

### Decision 07 — Pos-procedimento is new design

**Validated.** Legacy `ValidacaoEtapaPosProcedimento` is an empty stub (~5 lines). Specify must define:

- Inputs: atendimento-scoped Agendamento + pós-op Prontuario (if any)
- Outputs: pendências, message, optional stage advance to `Finalizado`
- **Not** classified as legacy migration in verification evidence

### Decision 08 — Multiple active Atendimentos

**Validated.** Critical workflow impacts:

| Area | Requirement |
|------|-------------|
| API | Workflow POST uses `{id}` — atendimento-scoped |
| Queries | Prontuario/Agendamento filtered by `AtendimentoId` |
| Legacy port | Rewrite patient-level scans to atendimento-scoped queries |
| Tests | Two parallel journeys same patient, independent state |
| WS07 (deferred) | Journey picker UI |
| Reporting | `AtendimentoId` required in exports |

**Reject:** "only one active atendimento per patient" — no stronger business rule found in domain docs or schema.

### Decision 09 — Care journey 1:N

**Validated.** Workflow maintains per-journey state. Historical `Finalizado` or soft-deleted journeys are read-only.

---

## Architecture Recommendation (Workflow)

| Layer | Responsibility |
|-------|----------------|
| **Use case** | Orchestration entry; transaction boundary; calls repositories |
| **Stage evaluators** | Per-stage rule modules (private classes or files) |
| **Domain service (optional)** | Pure transition policy |
| **Aggregate** | Pendências, events, etapa, message mutations |
| **Repository** | Persistence only; includes children on save |
| **Projection builder** | Maps aggregate + queried data → `ReadAtendimentoJourneyDto` |
| **Controller** | Thin; delegates to use case |

**Senhas integration (adapt):** After Agendamento SQL, `ValidacaoPreProcedimento` reads `Agendamento.SenhaAgendamento` and `Status` — not `FileDataOfSenhaExtractorService`. External file ingestion remains WS06 future work.

---

## Risk Assessment (Research)

| Risk | Class | Prob. | Impact | Mitigation |
|------|-------|-------|--------|------------|
| Execute before Prontuario/Agendamento verified | Dependency | High | Critical | Gate workflow SDD on upstream Verify |
| Legacy patient-scoped queries port wrong | Domain | High | High | AtendimentoId filter in all cross-reads; tests |
| Placeholder `Local=="0"` logic ported literally | Domain | High | Medium | Abandon; use Agendamento status fields |
| Pos-procedimento undefined blocks Verify | Domain | Medium | Medium | Specify net-new criteria before Execute |
| Scope creep: checklists, state machine, PDF | Governance | Medium | High | Explicit Out of Scope |
| God-class workflow service | Technical | Medium | Medium | Stage evaluator modules |
| Characterization tests absent | Verification | High | High | WS03 VP before Execute |
| PHI in workflow logs | Security | Medium | High | security-phi-review |

---

## Sizing

| Field | Decision |
|-------|----------|
| Size | **Large** |
| Rationale | Cross-aggregate orchestration, four stage evaluators (one net-new), pendência/event persistence, legacy characterization, projection DTO, integration fixtures across four aggregates. Rule complexity exceeds Paciente; comparable to Prontuario composite plus domain port. |
| Required phases | Specify / Design / Tasks / Pre-Execution Review / Execute / Verify |
| Escalation triggers | Checklist engine pulled in; live senhas file integration; formal state machine; schema migration |

---

## Research Conclusions (Workflow)

1. Workflow SDD is **independent** but **downstream** of minimal SQL + Prontuario + Agendamento verification.
2. **Use case + stage evaluator** pattern is the recommended architecture (Decision 05).
3. Legacy structure is **abandoned**; behavior is **selectively preserved** with explicit adapt at atendimento scope (Decision 08).
4. `ValidacaoEtapaPosProcedimento` is **new domain design**, not migration (Decision 07).
5. Senhas file integration is **abandoned** in workflow; Agendamento entity fields are the MVP2 source of truth.
6. Explicit workflow invocation endpoint is recommended over implicit GET refresh.

---

## Readiness Decision

### **NOT READY FOR SPECIFY** (until upstream verified)

Specify may begin when all upstream SDDs are **Verify-complete**:

| Prerequisite SDD | Status (2026-06-17) |
|------------------|---------------------|
| `atendimento-minimal-sql-stabilization` | Not started |
| `prontuario-sql-stabilization` | Research complete; not verified |
| `agendamento-sql-stabilization` | Not started |

### **READY FOR SPECIFY WITH CONDITIONS** (after upstream Verify)

Conditions:

1. Pos-procedimento completion criteria drafted in Specify (net-new design).
2. Pendência taxonomy and refresh semantics documented.
3. Characterization test plan accepted (WS03).
4. Journey projection DTO shape agreed.

### Blocked

| Blocker | Unblocks when |
|---------|---------------|
| Stub Prontuario/Agendamento repos | Prontuario + Agendamento SQL verified |
| Minimal Atendimento not in SQL | Minimal SDD verified |
| Pos-procedimento criteria undefined | Specify clinical design section complete |

### Deferred

| Item | Owner |
|------|-------|
| WS07 journey UI | WS07 |
| PDF reports | Future feature |
| Checklist engine | MVP3 |
| Senhas file/portal ingestion | WS06 |
| Formal state machine | Domain evolution |

---

## Specify Entry Checklist

Before writing `specify.md` (after upstream Verify):

### Scope ownership

- [ ] Workflow use cases — **not** repository, **not** aggregate validation methods.
- [ ] Pendencias + ClinicalEvents persistence in scope.
- [ ] Journey projection DTO — legacy nested stages **abandoned**.
- [ ] Pos-procedimento = **new design** section in Specify.
- [ ] Senhas file integration **out of scope**; Agendamento fields **in scope**.
- [ ] Checklists, PDF, WS07, state machine **out of scope**.

### Decisions recorded in Specify

- [ ] Workflow scoped by `AtendimentoId` (Decision 08).
- [ ] Cross-reads filter by `AtendimentoId`, not patient alone.
- [ ] Explicit `POST .../atualizar-jornada` (or named equivalent).
- [ ] Stage representation: `EtapaAtual` + Pendencias + Events + projection.
- [ ] Legacy characterization table per stage evaluator.
- [ ] Refresh replaces pendências by tipo (or documented alternative).

### Verification expectations

- [ ] Characterization tests for Consulta, Pré-OP, Procedimento stages.
- [ ] New design tests for Pos-procedimento (no legacy baseline).
- [ ] Integration: full fixture chain across four aggregates.
- [ ] security-phi-review + domain-review.

### Upstream gates

- [ ] Minimal Atendimento SDD verification.md accepted.
- [ ] Prontuario SDD verification.md accepted.
- [ ] Agendamento SDD verification.md accepted.

---

## References

- `Documentation/State.md`
- `AGENTS.md`
- `Documentation/Product/PM_DocOrgano.md`
- `Documentation/Product/PRD.md`
- `Documentation/Technical/migration-sql.md`
- `Documentation/Architecture/Domain_Overview_Business_Rules.md`
- `Documentation/Architecture/ADR/ADR-001-soft-delete.md`
- `Documentation/SDD/atendimento-minimal-sql-stabilization/research.md`
- `Documentation/SDD/prontuario-sql-stabilization/research.md`
- `Documentation/SDD/paciente-sql-stabilization/` (pilot reference)
- `Documentation/AI-Harness/Harness-Design/sdd-operational.md`
- `DocAPI/Legacy/_LegacySheetsDb/AtendimentoSheetsRepository.cs` (behavioral reference)
- `DocAPI/Core/Entities/Atendimento.cs`
- `DocAPI/Core/Entities/AtendimentoPendencia.cs`
- `DocAPI/Core/Entities/ClinicalEvent.cs`
