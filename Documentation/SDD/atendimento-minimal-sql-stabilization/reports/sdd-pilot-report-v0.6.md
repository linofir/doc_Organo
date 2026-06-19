# SDD Pilot Report v0.6

**Feature:** Atendimento Minimal SQL Stabilization  
**Pilot type:** Second forward SDD harness lifecycle (aggregate #2)  
**Phase covered:** Execute + Verify (with credential-alignment follow-up)  
**Status:** Verify complete — Documentation Follow-Up pending  
**Supersedes:** SDD Pilot Report v0.5 (Execute-only snapshot; archived by this report)  
**Sources:** Pilot reports v0.3–v0.4; `specify.md`, `design.md`, `tasks.md`; Execute session evidence; [verification.md](verification.md); Paciente pilot report v1.0 (GOV-02 precedent)

---

## Executive Summary

The second SDD pilot completed **Execute** and **Verify** for Atendimento Minimal SQL Stabilization — the prerequisite persistence slice that unblocks Prontuario and Agendamento FK work. Implementation delivered SQL CRUD, Guid contracts, slim DTOs, controller-level Paciente FK validation, PHI remediation, repository unit tests, controller tests, and SQL integration tests. Ownership boundaries approved in v0.3–v0.4 were preserved in code and confirmed in Verify.

Execute exposed a **credential-alignment failure** (GOV-13–15): Docker, EF tools, API, and integration tests resolved SQL credentials from different sources. A mid-Execute harness fix introduced `SqlConnectionResolver`, `.env.example`, and `scripts/sql-integration-test.ps1`.

**Verify (2026-06-18)** re-ran `dotnet build` and `dotnet test` (27 passed, 0 skipped), confirmed SQL integration against Docker SQL, applied review sensors, and closed the **TASK-008** gap with live HTTP smoke (17/17 scenarios against `https://localhost:7004`). Artifact: [verification.md](verification.md).

**Decision:** Complete with accepted residual risk — pending Documentation Follow-Up (`State.md`, `migration-sql.md`, `tasks.md`, PM).

---

## Pilot Timeline (Consolidated v0.3 → v0.6)

| Phase | What happened | Outcome |
|-------|---------------|---------|
| **Research** | Atendimento split (Minimal vs Workflow); sequencing validated | Research complete; Minimal scoped as FK prerequisite |
| **SDD — Specify / Design / Tasks** | REQ-001–REQ-010; Large sizing; ownership model documented | SDD package produced |
| **SDD Review (v0.3)** | Boundary, sequencing, ownership, PHI, 501 report strategy reviewed | **APPROVED WITH MINOR FOLLOW-UP ITEMS** |
| **SDD Consolidation Review (v0.4)** | Workflow health; anti-pattern watchlist; sequencing pattern validated | **Healthy — no Workflow redesign** |
| **SDD Pre-Execution Review** | Size → Large; API contract resolutions; prerequisite table | Execute authorized |
| **Execute** | Stub → SQL repository; controller hardening; tests; credential friction | **Code complete**; credential harness improved mid-session |
| **Verify** | Gates, review sensors, HTTP smoke, `verification.md` | **Complete with accepted residual risk** |
| **Documentation Follow-Up** | State, PM, migration-sql, tasks sync | **Pending** |
| **Reporting** | feature-report, session-handoff | Pending after Documentation Follow-Up |

---

## Verify Phase Outcomes

### Gate Results

| Gate | Status | Evidence |
|------|--------|----------|
| Build | Passed | `dotnet build DocAPI/DocAPI.csproj` — 0 errors (2026-06-18) |
| Automated tests | Passed | 27 passed, 0 skipped, 0 failed |
| SQL / Persistence | Passed | `AtendimentoSqlIntegrationTests` passed on Verify re-run (Docker available) |
| API | Passed | HTTP smoke 17/17; controller unit tests (POST 201/404, 501 routes) |
| UI | Skipped (expected) | WS07 deferred |
| Security / PHI | Passed | No `Console.WriteLine` in controller; synthetic data |
| Domain review | Passed | Factory + persistence-only repository; no workflow API |
| Documentation review | Partial | `State.md`, `migration-sql.md`, `tasks.md` still stale |
| Test strategy | Passed with suggestions | No mapping tests; soft-deleted PacienteId not explicitly tested |
| ADR evaluation | Not needed | ADR-001 only |
| Legacy characterization | Passed | Table in `specify.md` complete |

### Requirement Verification (post-Verify)

| Requirement | Status | Notes |
|-------------|--------|-------|
| REQ-001–005 | Verified | Unit + SQL + HTTP smoke |
| REQ-006 | Partially verified | Invalid PacienteId → 404 tested; soft-deleted Paciente not explicit |
| REQ-007 | Verified | Guid throughout |
| REQ-008 | Verified | SQL integration passed on Verify re-run |
| REQ-009 | Verified | PHI-safe controller |
| REQ-010 | Verified | Legacy table accepted |

### TASK-008 — Runtime Validation (closed in Verify)

Execute did not record structured Swagger session notes. Verify ran equivalent HTTP smoke against live DocAPI. All scenarios passed — see [verification.md](verification.md) Runtime Validation section.

| Scenario group | Result |
|----------------|--------|
| Create / Read / Update / Delete | Pass |
| List-by-paciente + two active journeys | Pass |
| Invalid PacienteId → 404 | Pass |
| Soft delete exclusion | Pass |
| Report / followUp routes → 501 | Pass |

### Review Sensors

| Sensor | Finding |
|--------|---------|
| `domain-review.md` | OK |
| `security-phi-review.md` | OK |
| `check-docs.md` | Suggestion — State / migration-sql drift |
| `test-strategy.md` | Suggestion — mapping tests; soft-deleted Paciente negative test |

### Completion Decision

- **Complete with accepted residual risk**
- **Residual risk rating:** Low
- **Reference:** [verification.md](verification.md)

---

## Consolidated Findings From v0.3 (SDD Review)

### Approved and Held Through Execute + Verify

| Area | v0.3 Status | Validation |
|------|-------------|------------|
| Feature boundary (no workflow/frontend) | Approved | **Held** — no scope creep |
| Sequencing (Paciente → Minimal → Prontuario…) | Approved | **Held** |
| Aggregate ownership (factory on create) | Approved | **Held** |
| Stage ownership (`EtapaAtual` workflow-owned) | Approved | **Held** — HTTP POST returns `Consulta`; update does not change stage |
| Multiple active atendimentos per patient | Approved | **Held** — unit + HTTP smoke (count=2) |
| PHI controls | Approved | **Held** |
| Report routes → 501 | Approved | **Held** — HTTP smoke confirmed |

### v0.3 Follow-Up Resolution Status

| Follow-Up | Status |
|-----------|--------|
| FU-01 — Size → Large | Resolved |
| FU-02 — Repository persistence-only | Resolved |
| FU-03 — EtapaAtual protection test | Resolved |
| FU-04 — SQL integration passing run | Resolved — Execute + Verify re-run |
| FU-05 — `AtualizadoPor` debt | Accepted residual risk |

---

## Consolidated Findings From v0.4 (Consolidation Review)

### Governance Finding 01 — Foundation Before Orchestration

**Status: Validated in Execute and Verify.**

Real `AtendimentoId` rows available for downstream FK. Prontuario and Agendamento Execute may proceed after Documentation Follow-Up syncs operational truth.

### Workflow Health (unchanged from v0.4)

Boundary, sequencing, aggregate ownership, stage ownership — **Excellent**. Rework risk — **Low**.

---

## Execute Phase Outcomes (retained from v0.5)

### Implementation Delivered

| Component | Change |
|-----------|--------|
| `Atendimento` entity | Factory; `AtualizarMensagemParaMedico`; `MarcarComoExcluido` |
| `IAtendimentoRepository` | Guid contract; `GetByPacienteIdAsync` |
| DTOs | Slim create/read/update |
| `AtendimentoRepository` | Full SQL CRUD |
| `AtendimentoController` | Paciente FK validation; 501 report routes; list-by-paciente |
| Tests | 8 repository + 3 controller + 1 SQL integration (new) |
| Harness (calibration) | `SqlConnectionResolver`, `.env.example`, scripts |

### Execute Validation (historical)

| Gate | Result |
|------|--------|
| Build | Pass |
| Post-alignment full suite | 27 passed, 0 skipped |
| Swagger smoke (TASK-008) at Execute end | **Not recorded** — closed in Verify |

---

## Major Friction Point — SQL Credential Misalignment (GOV-13–15)

Retained from v0.5 — see v0.5 archive content in git history if needed.

**Summary:** Integration tests skipped with misleading “Docker unreachable” when root cause was **password mismatch** across user-secrets, `.env`, and runbook placeholder. Fixed via `SqlConnectionResolver` and canonical `.env` workflow.

**Verify note:** Docker and credentials were available during Verify re-run; SQL integration passed without skip.

---

## Governance Findings (v0.5 → v0.6 update)

| ID | Description | v0.6 status |
|----|-------------|-------------|
| **GOV-13** | Single local credential source | **Proven** — harness fix applied; Verify passed with `.env` |
| **GOV-14** | Skip reason masked auth failure | **Proven** — still open for skip-message improvement |
| **GOV-15** | Docker volume password immutability | **Proven** — documented in runbook |
| **GOV-16** | DTO namespace vs entity name | **Proven** — `global::` workaround in `ReadAtendimentoDto` |
| **GOV-17** | Credential probe in prerequisites | Preliminary |
| **GOV-18** | Standard sql-integration script | **Partially addressed** — script exists |
| **GOV-19** | Moq for controller FK tests | Preliminary — implemented |
| **GOV-20** | Durable runtime validation evidence | **Partially addressed** — evidence now in `verification.md`; still no dedicated template/script |

### New Verify Observations (v0.6)

| ID | Description | Recommendation |
|----|-------------|----------------|
| **GOV-21** | TASK-008 expected in Execute session notes but often completed in Verify | Split TASK-008: Execute records intent; Verify owns durable HTTP evidence in `verification.md` |
| **GOV-22** | PowerShell JSON BOM breaks API POST when using `Set-Content -Encoding UTF8` | Document UTF-8 no-BOM for curl smoke; or add `scripts/api-smoke-atendimento.ps1` |
| **GOV-23** | Pilot report v0.x cadence works but needs explicit supersession on phase advance | Supersede Execute-only report when Verify completes (this v0.6) |

---

## Execute Boundary Assessment

| Boundary | Violation? | Notes |
|----------|------------|-------|
| Repository persistence-only | No | Verified |
| Controller owns Paciente FK | No | Verified |
| Workflow deferred | No | Verified |
| Frontend out of scope | No | Verified |
| State/PM updates in Execute | No (intentional deferral) | **Documentation Follow-Up still pending** |
| Harness infra in Execute | Minor expansion | Justified — pilot calibration |

---

## SDD Process Observations

### What worked

- v0.3–v0.4 reviews prevented scope creep through Verify.
- Large sizing matched verification burden despite smaller aggregate surface.
- Paciente patterns reused with low friction.
- Verify prompt v2 + verifier skill produced traceable `verification.md`.
- Closing TASK-008 in Verify with HTTP smoke was effective when Execute notes were missing.

### What did not work

- Execute did not produce TASK-008 session notes despite SDD requirement.
- Credential drift not caught by prerequisites (GOV-13).
- Documentation operational truth (`State.md`, `migration-sql.md`) lagged behind verified implementation.
- Manual curl/JSON tooling friction (BOM, PowerShell encoding) — not a product defect.

---

## Template And Harness Improvement Candidates

| Target | Proposed change | Priority | v0.6 note |
|--------|-----------------|----------|-----------|
| Execution Prerequisites | Credential probe + `.env` check | High | Open |
| `tasks.md` template | Clarify TASK-008 handoff vs Verify evidence | High | GOV-21 |
| `runtime-validation.md` or smoke script | Durable API smoke | Medium | GOV-20 partial |
| Integration test skip messages | Auth vs network | Medium | GOV-14 |
| SDD design checklist | DTO namespace collision | Medium | GOV-16 |
| `sql-migration-workflow` skill | Document resolver + scripts | Medium | Open |
| Pilot report supersession | Auto-note when Verify completes | Low | Done in v0.6 |

---

## Residual Risks (post-Verify)

| Risk | Severity | Status |
|------|----------|--------|
| Documentation drift (State, migration-sql, tasks) | Medium | **Open** — Documentation Follow-Up |
| Soft-deleted PacienteId → 404 not explicitly tested | Low | Accepted |
| No `AtendimentoMappingTests` | Low | Accepted |
| `AtualizadoPor` unset | Low | Accepted — Auth ADR deferred |
| Report endpoints 501-only | Low | Accepted |
| User-secrets override on developer machine | Medium | Operator action — prefer `.env` |
| WS07 frontend/API drift | Medium | Deferred |

---

## Current Recommendation

| Dimension | Status |
|-----------|--------|
| Execute implementation | **Complete** |
| Verify | **Complete with accepted residual risk** |
| REQ-001–REQ-010 | **Met** (REQ-006 partially — accepted) |
| TASK-008 | **Met** — evidence in `verification.md` |
| Credential harness | **Improved** |
| Downstream unblock | **Prontuario / Agendamento** may proceed after doc sync |
| **Next phase** | **Documentation Follow-Up** → Reporting → optional Teacher Guide |

---

## Appendix — Evidence Summary

| Evidence | Value |
|----------|-------|
| Baseline tests (pre-change) | 14 passed, 1 skipped |
| Post-alignment Execute | 27 passed, 0 skipped |
| Verify re-run (2026-06-18) | **27 passed, 0 skipped** |
| Atendimento tests added | 12 (8 repo + 3 controller + 1 SQL) |
| HTTP smoke (Verify) | **17/17 passed** |
| Verification artifact | [verification.md](verification.md) |

---

## Report Lineage

| Version | Phase | Key contribution |
|---------|-------|----------------|
| v0.3 | SDD Review | Boundary approval; FU-01–FU-05 |
| v0.4 | Consolidation Review | Workflow health; anti-pattern watchlist |
| v0.5 | Execute | Implementation; GOV-13–15; credential harness — **superseded by v0.6** |
| **v0.6** | **Execute + Verify** | Verification gates; TASK-008 closed; completion decision; GOV-21–23; Documentation Follow-Up handoff |

**Next report:** v0.7 optional after **Documentation Follow-Up + Reporting** — consolidate operational doc sync and lessons for third aggregate pilot (Prontuario).
