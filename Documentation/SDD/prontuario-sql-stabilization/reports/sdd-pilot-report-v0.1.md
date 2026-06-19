# SDD Pilot Report v0.1

**Feature:** Prontuario SQL Stabilization  
**Pilot type:** Third forward SDD harness lifecycle (aggregate #3 — composite clinical graph)  
**Phase covered:** Research + Research Delta (Part 3 reconciliation)  
**Status:** Research complete — **Specify not started**  
**Supersedes:** N/A (initial report for this feature)  
**Sources:** [research.md](../research.md) (Part 1–3); [atendimento-minimal-sql-stabilization/verification.md](../../atendimento-minimal-sql-stabilization/verification.md); [paciente-sql-stabilization/reports/sdd-pilot-report-v1.0.md](../../paciente-sql-stabilization/reports/sdd-pilot-report-v1.0.md); [atendimento-minimal-sql-stabilization/reports/sdd-pilot-report-v0.6.md](../../atendimento-minimal-sql-stabilization/reports/sdd-pilot-report-v0.6.md); `Documentation/AI-Harness/template/governance/governance-improvement-plan.md`

---

## Executive Summary

This report opens the **Prontuario SQL Stabilization** harness pilot at the end of **Research**, including the **Research Delta (Part 3)** that reconciled Part 1/2 findings against the verified **Atendimento Minimal** prerequisite SDD and updated PM/migration sequencing.

Research confirms Prontuario is **Large** sizing: composite aggregate (owned value objects + Exames + Internacao + AcoesCD), **versioning in scope** (immutable snapshots — successor row on PUT, not in-place mutation), mandatory **AtendimentoId** FK validation, unsafe legacy AutoMapper to replace, and PHI-sensitive controller remediation. The upstream Atendimento blocker identified in Part 2 is **resolved** — Prontuario consumes verified API/repository contracts; it does not re-implement Atendimento.

**Decision:** **READY FOR SPECIFY** — no second discovery pass required. Open product/Specify items (PUT HTTP semantics, optional UX error codes, physician “new version vs same version” choice) are documented in Part 3 and must be resolved in `specify.md` / Design, not Research.

**Harness note:** This session validated the **Research Delta** pattern (reconcile stale research after prerequisite SDD completes). Recommend tracking as a governance calibration candidate (GOV-24) after this feature completes Verify.

**Next phase:** Write `specify.md` using Part 2 domain decisions + Part 3 operational reconciliation.

---

## Report Metadata

| Field | Value |
|-------|-------|
| **Feature slug** | `prontuario-sql-stabilization` |
| **SDD sizing (expected)** | **Large** |
| **PM item** | WS01 — Prontuario SQL Migration & Stabilization |
| **Pilot sequence** | Aggregate #3 after Paciente (#1) and Atendimento Minimal (#2) |
| **Completion decision** | N/A — implementation not started |
| **Evidence bundle (current)** | [research.md](../research.md) only |
| **Governance improvement plan** | Not started — input after post-Verify Reporting chain |

### Supersession Rule

Increment this report at each major phase advance. Planned triggers:

| Next version | Trigger |
|--------------|---------|
| **v0.2** | Specify + Design + Tasks complete; SDD Pre-Execution Review outcome recorded |
| **v0.3+** | Execute milestones or mid-Execute friction |
| **v0.x (Verify)** | Verify complete — must supersede Execute-only snapshots |
| **Final** | Documentation Follow-Up + Reporting — consolidate for governance-improvement-plan |

---

## Pilot Timeline

| Phase | What happened | Outcome |
|-------|---------------|---------|
| **Research — Part 1** | Feature discovery; boundary decomposition; blocker identification (Atendimento FK, versioning, CID, mapping) | Scope mapped; **not ready for Specify** at discovery exit |
| **Research — Part 2** | Six approved decisions validated against codebase and governance | Domain direction locked; **critical Atendimento Execute blocker** documented |
| **PM / sequencing correction** | Atendimento split into Minimal vs Workflow; order updated in PM and `migration-sql.md` | Prerequisite path defined outside Prontuario SDD |
| **Prerequisite SDD** | `atendimento-minimal-sql-stabilization` Execute + Verify (2026-06-18) | **27 tests**; Atendimento API/repository verified |
| **Research — Part 3 (Delta)** | Reconciled Part 1/2; code audit; session refinements (UX guidance, workflow data contract, versioning semantics, CID debt, PDF future work) | **READY FOR SPECIFY** |
| **Specify / Design / Tasks** | Not started | — |
| **SDD Pre-Execution Review** | Not started | — |
| **Execute** | Not started | `ProntuarioRepository` still stub |
| **Verify** | Not started | — |
| **Documentation Follow-Up** | Not started | — |
| **Reporting** | Not started | — |
| **Teacher Guide** | Not started | — |

---

## Workflow Performance Evaluation (Research Phase)

| Phase | Rating | Evidence | Primary friction | Improvement candidate |
|-------|--------|----------|------------------|----------------------|
| **Research** | **Strong** (after Part 3) | Part 1–3 in [research.md](../research.md) | Part 2 left stale operational claims until Part 3; initial PM order contradicted Decision 1 | Formalize **Research Delta** trigger in governance (GOV-24) |
| Specify | Not executed | — | — | Apply Part 3 checklist as Specify entry gate |
| Design | Not executed | — | — | Versioning + nested graph design risk |
| Tasks | Not executed | — | — | TASK-008 / SQL fixture complexity |
| SDD Pre-Execution Review | Not executed | — | — | Required for Large sizing |
| Execute | Not executed | — | — | — |
| Verify | Not executed | — | — | — |
| Documentation Follow-Up | Not executed | — | — | — |
| Reporting | Not executed | — | — | — |
| Teacher Guide | Not executed | — | — | — |

### Phase Ownership Assessment (Research)

| Question | Answer | Finding ID |
|----------|--------|------------|
| Did Research do Specify's job? | **No** — REQ themes listed as anticipated, not final acceptance criteria | — |
| Did Research defer work that blocked Specify? | **Yes, then resolved** — Atendimento blocker deferred to separate SDD; Part 3 closed the loop | WF-12 |
| Were prior pilot patterns reused? | **Yes** — Paciente/Atendimento patterns referenced for Guid, soft delete, SQL integration, PHI | — |
| Is operational truth synchronized? | **Yes** — PM/migration-sql/State reflect Atendimento Minimal verified; research Part 3 aligned | — |

---

## Research Outcomes — Locked For Specify

### Decisions confirmed (Part 2 + Part 3)

| # | Decision | Specify must carry forward |
|---|----------|----------------------------|
| 1 | Create Atendimento → Create Prontuario | Validate `AtendimentoId`; no auto-create inside Prontuario |
| 2 | Versioning **in scope** | v1 on POST; PUT creates **successor DB row** (new `Id`); prior row immutable |
| 3 | Internacao optional; CID required when Internacao present | Synthetic CID in test fixtures only |
| 4 | Backend source of truth | Guid + root `PacienteId` / `AtendimentoId` on API |
| 5 | Replace `ProntuarioProfile` AutoMapper | Domain factories + explicit read projection |
| 6 | Rich domain model | Paciente-style factories; `CriarNovaVersao`; repository persistence-only |

### Superseded by Part 3 (do not re-introduce in Specify)

| Stale claim | Superseding truth |
|-------------|-------------------|
| Minimal Atendimento implementation inside Prontuario SDD | Prerequisite **verified** — consume only |
| Atendimento repository stub blocks Execute | **Mitigated** |
| PM order Prontuario before Atendimento | **Corrected** in PM / migration-sql |

### Specify decisions still open (from Part 3)

| Topic | Default recommendation | Owner phase |
|-------|------------------------|-------------|
| PUT returns **201** + new DTO vs alternate route | **201** + new `Id` | Specify / Design |
| Error code when `AtendimentoId` missing (UX messaging) | Optional stable code; default **404** | Specify |
| Physician choice: new version vs edit same version | **Defer** — PUT always creates successor in MVP2 backend | WS07 / product |
| `GET /Prontuario/paciente/{id}` scope | All non-deleted versions | Specify |
| `Tipo` canonical type | **int** (match DB) | Specify |
| ADR for versioning API contract | Evaluate in Specify/Design | Architecture |

### Downstream contract for workflow SDD (document only — not in Prontuario Execute)

Persist queryable fields for future `atendimento-workflow-stabilization` evaluators: `AtendimentoId`, `Tipo`, `DataConsulta`, `AcoesCD`, Internacao graph. No stage evaluators or pendência writes in this SDD.

### Explicit future work registry (Documentation Follow-Up / PM)

| Item | Owner |
|------|-------|
| CID Catalog Management | Future WS01 feature |
| Prontuario PDF ingestion (`POST /from-pdf`) | Future WS06 — extraction model not ready |
| UX: guide create Atendimento after Paciente | WS07 / product |
| Atendimento workflow / `ValidacaoEtapa*` | `atendimento-workflow-stabilization` after Prontuario + Agendamento verified |

---

## What Worked — Keep For This Pilot

- **Splitting Atendimento Minimal into its own SDD** before Prontuario — validated sequencing pattern from Atendimento pilot (GOV-01 / Foundation Before Orchestration).
- **Research Part 2 six-decision framework** — gave Specify stable domain inputs despite operational churn.
- **Research Delta (Part 3)** — efficient reconciliation without repeating Part 1 discovery; supersession table prevents agents from acting on stale blockers.
- **Prior pilot artifacts as templates** — Paciente REQ traceability, Atendimento Minimal design/verification, SQL integration skip policy, credential harness (`SqlConnectionResolver`, scripts).
- **Backend Stabilization Rule** — WS07 deferred; contract drift documented for later frontend alignment.
- **Specify Entry Checklist in research** — Part 3 re-validated checklist with prerequisite items marked satisfied.
- **Pilot report versioning intent** — user workflow (update per phase) aligns with CONTRIBUTING-AI supersession rule (Atendimento v0.6 precedent).

---

## Points Of Attention — Before And During Implementation

| # | Area | Risk | Mitigation for upcoming phases |
|---|------|------|--------------------------------|
| A1 | **Versioning semantics** | PUT as successor row is breaking vs legacy and current API | Resolve in Specify; ADR candidate; domain-review in Verify |
| A2 | **Composite persistence** | Single write touches up to 5 tables | Design explicit transaction/replace semantics for child collections |
| A3 | **AutoMapper** | Known critical bugs (`InformacoesExtras` ← `SolicitacaoInternacao`) | Design must plan removal from create/update path — not patch |
| A4 | **Unique index `(PacienteId, Versao)`** | Version counter is **patient-wide**, not per Atendimento | Centralize `NextVersao` in domain/repository; test second prontuario |
| A5 | **PHI in controller** | `Console.WriteLine` on create today | REQ-style remediation (Paciente/Atendimento precedent) |
| A6 | **CID in production** | No catalog; tests need synthetic rows | Accept debt; route to CID Catalog in Documentation Follow-Up |
| A7 | **Credential / SQL env** | Prior pilot friction (GOV-13–15) | Reuse `.env` + `sql-integration-test.ps1`; probe in Execution Prerequisites |
| A8 | **TASK-008 handoff** | Atendimento Execute omitted structured smoke notes (GOV-21) | Plan runtime evidence owner in tasks.md (Execute intent vs Verify durable record) |
| A9 | **Scope creep** | PDF import, full workflow, frontend | Explicit Out of Scope in Specify — Part 3 registry |
| A10 | **Complexity vs Paciente** | First composite + versioning forward SDD | Keep **Large** sizing; do not compress Pre-Execution Review |

---

## Prior Pilot Deferred Items — Applicability To Prontuario

| Prior ID | Original recommendation | Status for Prontuario pilot | Action in this feature |
|----------|-------------------------|----------------------------|------------------------|
| GOV-10 | Governance improvement feedback loop | **Addressed at template level** — `governance-improvement-plan.md` exists | This pilot report feeds that plan **after Verify** |
| GOV-13–15 | Credential single source / skip messages / Docker password | **Proven** in Atendimento — harness improved | **Apply** — do not rediscover; cite runbook + scripts in Prerequisites |
| GOV-21 | TASK-008 Execute vs Verify evidence split | **Proven** in Atendimento Verify | **Apply** in Prontuario tasks/design |
| GOV-22 | PowerShell JSON BOM for API smoke | **Preliminary** | Watch during Prontuario Verify |
| SK-11 | `sql-migration-workflow` skill update | **Deferred** from Paciente | **Confirm or resolve** if Prontuario repeats same integration pattern |
| TPL — Pilot report template | Formalize pilot report category | **In use** — this v0.1 instance | Continue phase increments |

---

## Governance Findings (GOV-*)

*Research-phase findings only. Confidence: **Preliminary** until confirmed by Execute/Verify evidence. ID sequence continues from Atendimento pilot (GOV-23).*

| ID | Description | Impact | Evidence source | Recommendation | Confidence |
|----|-------------|--------|-----------------|----------------|------------|
| **GOV-24** | **Research Delta** is an effective reconciliation phase when prerequisites complete mid-discovery, but is **not named** in `sdd-operational.md` | Medium — agents may repeat full Research or act on stale Part 1/2 blockers | Part 3 session; user workflow intent | Add *Research Delta* subsection to `sdd-operational.md`: trigger, artifact (`Part N` in `research.md`), exit criteria (**READY FOR SPECIFY**) | **Preliminary** |
| **GOV-25** | Multi-part `research.md` needs explicit **supersession rule** (Part 3 governs operational status; Part 2 governs domain unless superseded) | Medium — planning errors if Part 2 Atendimento stub claims read without Part 3 | [research.md](../research.md) header + Part 3 | Add supersession note to research template or SDD README; Prontuario instance is first exemplar | **Preliminary** |
| **GOV-26** | Prerequisite SDD should **block Specify** on downstream feature until Verify completes — not only Research | High — Prontuario Part 2 recommended embedding Atendimento REQ incorrectly | Part 2 vs Part 3; Atendimento Minimal Verify | Specify Entry Checklist template: distinguish **prerequisite verified** vs **in-scope upstream REQ** | **Preliminary** |
| **GOV-27** | Versioning PUT semantic change is ADR-candidate; research alone insufficient for API authority | Medium — WS07 and API clients affected | Part 2 Decision 2; domain docs | Schedule ADR evaluation in Specify/Design; document in verification domain-review | **Preliminary** |

---

## Workflow Findings (WF-*)

| ID | Description | Impact | Recommendation | Confidence |
|----|-------------|--------|----------------|------------|
| **WF-12** | **Research Delta** closes the loop when PM sequencing or prerequisite SDDs change after initial Research | High — avoided second full discovery pass for Prontuario | Standardize: trigger Delta when prerequisite Verify completes or `State.md` sequencing changes | **Preliminary** |
| **WF-13** | Forward SDD #3 should **inherit** Execution Prerequisites from Atendimento (credentials, Docker, baseline tests) without rewriting | Medium — repeat friction | Reference Atendimento `specify.md` Prerequisites + runbook in Prontuario Specify by link | **Preliminary** |
| **WF-14** | Composite aggregate SDDs need **fixture chain** documented early: Paciente → Atendimento → Prontuario v1 → PUT → v2 (+ optional Internacao/CID) | High — integration test design risk | Add fixture chain to Specify acceptance criteria and Design (from Part 3) | **Preliminary** |
| **WF-15** | Future-work registry (PDF, CID catalog, UX onboarding) should live in **pilot report + Specify Out of Scope** to prevent scope creep | Medium | Part 3 registry copied into Specify; revisit in pilot v0.2 | **Preliminary** |

---

## Skill Findings (SK-*)

| ID | Skill | Description | Recommendation | Confidence |
|----|-------|-------------|----------------|------------|
| **SK-12** | `doc-organo-context` / Research | Loading State + PM + prior SDD verification before Research Delta prevented duplicate Atendimento scoping | Keep "read State first" + link verified prerequisite SDD in Research exit | **Preliminary** |
| **SK-13** | `verifier` (forward-looking) | Prontuario Verify will need versioning chain evidence + nested graph SQL test — heavier than Atendimento | Plan gate expectations in Specify; no skill change until Verify | **Deferred** |
| **SK-14** | `sql-migration-workflow` | Third aggregate should confirm whether skill update (resolver, scripts, fixture pattern) is ready | Resolve after Execute if same pattern holds without friction | **Deferred** |

---

## Template And Harness Improvement Candidates

| Target | Proposed change | Priority | Track in |
|--------|-----------------|----------|----------|
| `sdd-operational.md` | Formal **Research Delta** phase/trigger | High | GOV-24 |
| Research / SDD README | Multi-part research supersession rule | Medium | GOV-25 |
| `specify.md` template | Prerequisite verified vs in-scope upstream REQ checklist items | Medium | GOV-26 |
| `specify.md` / `design.md` | Versioning API + fixture chain sections for composite aggregates | High | WF-14, GOV-27 |
| `tasks.md` template | TASK-008 handoff vs Verify evidence (inherit GOV-21) | Medium | Atendimento Wave 3 |
| `governance-improvement-plan.md` | Input bundle includes phase-increment pilot reports (v0.1 → Verify) | Low | After this feature Reporting |
| Pilot report cadence | Phase-covered field + supersession table (this report) | Low | **Validated by user workflow** |

---

## Residual Risks (Pre-Implementation)

| Risk | Severity | Status | Notes |
|------|----------|--------|-------|
| Acting on Part 2 without Part 3 | Medium | **Mitigated** — Part 3 governs | Agents must read full research header |
| Versioning PUT breaks implicit API clients | High | **Open** — Specify/ADR | WS07 tracks frontend |
| AutoMapper silent data loss if not removed | Critical | **Open** — Design/Execute | Known bugs documented |
| `(PacienteId, Versao)` collision | High | **Open** — Design | Test in integration |
| CID catalog absent in production | Medium | **Accepted debt** | Documentation Follow-Up |
| PDF `/from-pdf` scope creep | Medium | **Mitigated** — Out of Scope registry | Future feature |
| Workflow logic pulled into Prontuario SDD | Medium | **Mitigated** — boundary in Part 3 | Workflow SDD downstream |
| SQL credential drift | Medium | **Mitigated** — prior pilot harness | Operator discipline still required |
| Under-tested nested graph | High | **Open** — Verify planning | WF-14 |

---

## Maturity Assessment (Research Phase Only)

| Dimension | Score | Justification |
|-----------|-------|---------------|
| SDD workflow (Research) | **4/5** | Strong discovery; needed Part 3 delta for operational reconciliation |
| Verification | **N/A** | Not started |
| Documentation follow-up | **N/A** | Not started |
| Reporting & handoff | **2/5** | This v0.1 opens pilot reporting; feature-report not yet |
| Knowledge transfer | **N/A** | Post-Reporting |
| **Overall harness maturity (this feature, to date)** | **3/5** | Research exit strong; implementation evidence pending |

---

## Current Recommendation

| Dimension | Status |
|-----------|--------|
| Research + Research Delta | **Complete** — [research.md](../research.md) Part 3 |
| Prerequisite Atendimento Minimal | **Verified** — do not re-implement |
| Ready for Specify | **Yes** |
| Ready for Execute | **No** — Specify → Design → Tasks → Pre-Execution Review required |
| Pilot report | **v0.1 — Research phase** — update at Specify/Design/Tasks completion (**v0.2**) |
| Governance improvement plan | **Defer** until post-Verify Reporting chain |

### Apply before Specify

1. Use Part 3 **Updated Specify Entry Checklist** as non-negotiable scope input.
2. Carry forward **future-work registry** into Specify Out of Scope.
3. Inherit **Execution Prerequisites** from verified pilots (Docker, `.env`, baseline 27 tests).
4. Plan **fixture chain** and **versioning** acceptance criteria explicitly.

### Keep unchanged

- Large sizing and full lifecycle (including Pre-Execution Review).
- Backend Stabilization Rule (WS07 out of scope).
- Atendimento Minimal as consumed prerequisite — not Prontuario Execute scope.

---

## Open Questions (Pilot Report Tracker)

| Question | Why it matters | Suggested resolution phase | Status |
|----------|----------------|----------------------------|--------|
| PUT → 201 + new Id vs dedicated version route | API contract + WS07 | Specify / Design | Open |
| Stable error code for missing `AtendimentoId` | UX onboarding | Specify | Open |
| Physician chooses new version vs same edit | Product/clinical policy | WS07 — default reject dual mode in MVP2 backend | Open |
| ADR for versioning API | Architecture authority | Specify / Design | Open |
| Properties mutable without new version | Domain immutability | Specify — default **no** for clinical sections | Open |

*Resolve or defer entries during Specify/Design; update this table in pilot report v0.2.*

---

## Appendix A — Evidence Index

| Artifact | Path | Role in v0.1 |
|----------|------|--------------|
| Research (Part 1–3) | [research.md](../research.md) | Primary evidence |
| Atendimento Verify | [atendimento-minimal-sql-stabilization/verification.md](../../atendimento-minimal-sql-stabilization/verification.md) | Prerequisite contracts |
| Paciente pilot report | [sdd-pilot-report-v1.0.md](../../paciente-sql-stabilization/reports/sdd-pilot-report-v1.0.md) | GOV/WF baseline |
| Atendimento pilot report | [sdd-pilot-report-v0.6.md](../../atendimento-minimal-sql-stabilization/reports/sdd-pilot-report-v0.6.md) | Forward SDD #2 lessons |
| Governance improvement plan template | `Documentation/AI-Harness/template/governance/governance-improvement-plan.md` | Post-Verify calibration target |
| specify.md | Not created | v0.2 evidence |
| design.md | Not created | v0.2 evidence |
| tasks.md | Not created | v0.2 evidence |
| verification.md | Not created | Verify-phase report increment |

---

## Appendix B — Finding Confidence (For Future Governance Plan)

| Level | Meaning | v0.1 usage |
|-------|---------|------------|
| **Proven** | Observed with implementation/Verify evidence | None yet in this report |
| **Preliminary** | Likely valid; needs this pilot's Execute/Verify | All GOV/WF/SK findings above |
| **Deferred** | Hypothesis or awaiting third pilot | SK-13, SK-14 |

---

## Report Lineage

| Version | Phase | Key contribution |
|---------|-------|------------------|
| **v0.1** | **Research + Research Delta** | Initial pilot report; Part 3 reconciliation; READY FOR SPECIFY; GOV-24–27; WF-12–15; attention register A1–A10 |

**Next report trigger:** **v0.2** after **Specify + Design + Tasks** and SDD Pre-Execution Review — record contract resolutions, open question closures, and Pre-Execution Review approval status.

---

*End of SDD Pilot Report v0.1*
