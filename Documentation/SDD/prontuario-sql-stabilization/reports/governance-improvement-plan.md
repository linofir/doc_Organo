# Governance Improvement Plan — prontuario-sql-stabilization

> **Template:** `Documentation/AI-Harness/template/governance/governance-improvement-plan.md`  
> **Feature slug:** `prontuario-sql-stabilization`  
> **Active SDD:** `Documentation/SDD/prontuario-sql-stabilization/`  
> **Pilot type:** Third forward SDD calibration (aggregate #3)  
> **SDD sizing:** Large  
> **Plan version:** v0.1  
> **Status:** Ready for review  
> **Author / session:** Cline (AI agent)  
> **Date:** 2026-07-10  
> **Supersedes:** N/A (first governance improvement plan for this feature)  
> **Evidence bundle:** [verification.md](../verification.md), [feature-report.md](feature-report.md), [sdd-pilot-report-v0.1.md](sdd-pilot-report-v0.1.md), [sdd-pilot-report-v0.2.md](sdd-pilot-report-v0.2.md), [session-handoff.md](session-handoff.md)

---

## Executive Summary

The Prontuario SQL Stabilization feature confirmed that the SDD harness workflow scales cleanly to a third clinical SQL vertical with composite aggregates, dual-mode versioning (ADR-006), and a 19-day gap between Pre-Execution Review and Execute. The harness performed **strongly**: no implementation drift across the gap, no scope creep, all 18 requirements verified, and 79 tests passing. The primary new friction was the TASK-009 Execute→Verify handoff for runtime validation evidence — Swagger smoke session notes were not recorded, leaving Verification without durable HTTP evidence (mitigated by 27 controller unit tests).

Prior deferred findings from Paciente (GOV-10) and Atendimento (GOV-21, SK-11, GOV-13–15) are **resolved** — their GIB items were implemented before this feature and all held. The Prontuario v0.1 pilot report opened 4 GOV findings, 4 WF findings, and 3 SK findings at Research phase. Execute/Verify evidence **promotes** 7 of these to Proven and **resolves** GOV-27 (ADR-006 accepted). Three new findings emerge from feature-report Lessons Learned.

**Total findings:** 10 Proven, 1 Resolved, 2 Deferred (SK-13, SK-14).  
**Recommended rollout:** **implement before next SDD** (Agendamento) — Wave 1 items are low-effort template/documentation updates with no breaking changes.

---

## Feature Context (Minimal)

| Item | Value |
|------|-------|
| PM item | WS01 — Prontuario SQL Migration & Stabilization |
| Requirements delivered | REQ-001–REQ-018 — all verified (see [verification.md](../verification.md) §3) |
| Completion decision | Verified with Minor Findings — 79 tests, F-01 (low severity) |
| Residual risk | Low — F-01 Swagger smoke pending; CID catalog absent; WS07 deferred; `AtualizadoPor` unset |
| Key friction themes | TASK-009 Execute→Verify handoff gap; D-02 specify-vs-design authority reconciliation; Research Delta formalization pending; third-vertical pattern proved stable |

---

## Calibration Scope

Every finding in this plan targets **governance**, **workflow**, **templates**, **skills**, or **documentation**. No findings target verification gates, engineering practices, or tooling — those performed adequately.

---

## Workflow Performance Evaluation

| Phase | Rating | Evidence (link or artifact) | Primary friction | Improvement candidate |
|-------|--------|----------------------------|------------------|----------------------|
| Research | **Strong** | [research.md](../research.md) Part 1–3; Pilot Report v0.1 | Part 2 stale after prerequisite SDD verified — Part 3 Delta reconciled | Formalize Research Delta in `sdd-operational.md` (GOV-24) |
| Specify | **Strong** | [specify.md](../specify.md) — 18 REQs, Legacy Behavior table, Runtime Validation Environment | D-02 wording conflict with design.md | Flag authority conflicts earlier (DOC-01) |
| Design | **Strong** | [design.md](../design.md) — D-01–D-06, API Contract table, ADR-006 | Version-generation wording mismatch with Specify | Template flag for cross-artifact conflict detection |
| Tasks | **Strong** | [tasks.md](../tasks.md) — TASK-001–010, VP-001, DF-001, Execution Prerequisites, Dependency Map | TASK-008/TASK-009 split clarified; still gap at handoff (WF-16) | Document Execute→Verify handoff expectations for runtime validation tasks |
| SDD Pre-Execution Review | **Strong** | [pre-execution-review-2026-06-19.md](pre-execution-review-2026-06-19.md); 19-day gap → no drift | None — validated as durable phase boundary | Document durability as proven pattern in `sdd-operational.md` (GOV-28) |
| Execute | **Adequate** | All 10 TASKs complete; 79 tests; [tasks.md](../tasks.md) checkboxes | TASK-009 session notes not recorded — handoff gap (WF-16) | Clearer TASK-009 ownership expectations in tasks template |
| Verify | **Strong** | [verification.md](../verification.md) — all 18 REQs traceable; 10 gates evaluated; F-01 documented | TASK-009 evidence missing — compensated by 27 controller tests | No governance change needed; F-01 is feature follow-up |
| Documentation Follow-Up | **Strong** | State.md, migration-sql.md updated; no SDD drift | None — routing correct | Keep pattern |
| Reporting | **Adequate** | feature-report.md, session-handoff.md complete | None | Keep pattern |
| Teacher Guide | **N/A** | Decision: Generate (session-handoff.md); not yet executed | N/A — post-Reporting phase | Keep existing `not-a-teacher` skill workflow |

### Phase Ownership Assessment

| Question | Answer | Finding ID |
|----------|--------|------------|
| Did any phase do another phase's job? | **No** — Execute implemented; Verify verified; Documentation Follow-Up updated docs; Reporting reported | — |
| Did any phase defer work that blocked the next phase? | **Yes** — TASK-009 Swagger smoke was not recorded during Execute, leaving Verify with incomplete runtime evidence | WF-16 |
| Were Execute boundaries respected? | **Yes** — no verification artifacts, reporting, documentation updates, or Teacher Guide generation during Execute | — |
| Was operational truth synchronized before Reporting? | **Yes** — Documentation Follow-Up completed before feature-report.md was written | — |

---

## Governance Performance Evaluation

| Dimension | Rating | Evidence (link or artifact) | Improvement opportunity |
|-----------|--------|----------------------------|-------------------------|
| Authority hierarchy | **Strong** | ADR-006 accepted at Pre-Execution Review; ADR-001 honored; D-02 resolved in favor of design.md + ADR-006 | Document authority resolution precedence when artifacts conflict (DOC-01) |
| Ownership model | **Adequate** | TASK/VP/DF separation held; TASK-009 handoff gap exposed unclear runtime evidence ownership | Clarify Execute vs Verify ownership of runtime validation evidence (WF-16) |
| Phase boundaries | **Strong** | 19-day Pre-Execution Review gap → no drift; phases respected | Document Pre-Execution Review durability (GOV-28) |
| Evidence strategy | **Adequate** | 79 tests + 27 controller tests + 1 SQL integration; but TASK-009 HTTP evidence gap | Formalize runtime evidence handoff for API validation tasks |
| Documentation routing | **Strong** | State.md, migration-sql.md correctly updated; no unnecessary routing | Keep |
| Review process | **Strong** | 10 review sensors applied; all gates evaluated; skipped gates documented | Keep |
| Operational governance | **Strong** | Credential Probe reused; SQL integration skip policy applied; prior pilot patterns held | Keep |
| Governance consistency | **Strong** | Third aggregate confirmed prior calibration; no regression | Keep |

---

## Harness Validation

### Assumptions Validated

| Assumption | Evidence | Validation strength |
|------------|----------|---------------------|
| SDD Pre-Execution Review is a durable phase boundary for multi-session Large features | 19-day gap between Pre-Exec Review (2026-06-19) and Execute (2026-07-08) — no implementation drift, all ADR decisions honored | **Strong** |
| TASK/VP/DF classification prevents Execute scope creep | Execute produced no verification, reporting, or documentation artifacts; all post-Execute phases operated on clean inputs | **Strong** |
| Repository-direct SQL integration pattern scales to composite aggregates with versioning | Integration test with Paciente → Atendimento → Prontuario v1 → correction → evolution → v2 → soft delete passed cleanly; no new abstractions needed | **Strong** |
| Credential Probe + prior pilot harness is reusable without rediscovery | Credential Probe passed on first attempt; `SqlIntegrationTestGate` skip policy applied correctly; no credential friction | **Strong** |
| ADR evaluation at Pre-Execution Review prevents API design scope creep | ADR-006's correction vs evolution split was clear enough that no implementation divergence occurred | **Strong** |
| Legacy characterization table in specify.md provides sufficient behavioral reference | All preserve/adapt/abandon decisions honored; documentation review gate confirmed alignment | **Strong** |

### Assumptions Invalidated

| Assumption | What broke | Finding ID |
|------------|------------|------------|
| TASK-009 Swagger smoke session notes would be reliably recorded during Execute | Execute completed TASK-009 checkbox but did not record session notes; Verify could not independently confirm runtime HTTP behavior | WF-16 |

### Assumptions Requiring Future Validation

| Assumption | Why not validated | Recommended trigger |
|------------|-------------------|---------------------|
| Two-atendimento patient-wide Versao scenario (D-03) is low-risk | Scenario recommended but not executed in TASK-008; single-atendimento multi-version scenario tested | Agendamento or Workflow SDD when multiple attendimentos per patient becomes common |
| Concurrent evolution 409 client recovery guidance in API docs is sufficient | D-06 documented in design.md; WS07 API docs not yet built | WS07 frontend alignment |
| Teacher Guide eligibility criteria produce appropriate generate/skip decisions | Prontuario decision: Generate (3+ layers, first dual-mode versioning, novel patterns) — Teacher Guide not yet written | After Teacher Guide generation and Agendamento's decision |

### Decisions Stable Enough to Adopt

| Decision | Prior confidence | New confidence | Adoption artifact |
|----------|-----------------|----------------|-------------------|
| Repository-direct SQL integration test as the standard persistence gate | Proven (Paciente, Atendimento) | **Confirmed** (third aggregate, composite graph) | `tasks.md` template § TASK-008 pattern; `sql-migration-workflow` skill |
| TASK/VP/DF classification in tasks template | Proven (Atendimento Wave 1 GIB-002) | **Confirmed** (no Execute boundary violations) | `tasks.md` template — no change needed |
| Credential Probe as Execution Prerequisite | Proven (Atendimento Wave 1 GIB-001) | **Confirmed** (reused without friction) | `specify.md` template § Execution Prerequisites — no change needed |
| SDD Pre-Execution Review as mandatory gate for Large features | Proven (Paciente, Atendimento) | **Confirmed** (19-day durability validated) | `sdd-operational.md` — already documented; proven pattern documented in GOV-28 |
| ADR evaluation before non-trivial API design decisions | Preliminary (Paciente — no ADR needed) | **Confirmed** (ADR-006 prevented scope creep) | `sdd-operational.md` — ADR trigger guidance can cite this as exemplar |

---

## Findings Registry

### Governance Findings (GOV-*)

| ID | Description | Impact | Evidence source | Validation source | Recommendation | Confidence |
|----|-------------|--------|-----------------|-------------------|----------------|------------|
| **GOV-24** | Research Delta is an effective reconciliation phase when prerequisites complete mid-discovery, but is not formally named in `sdd-operational.md` | Medium — agents may repeat full Research or act on stale Part 1/2 blockers | Pilot Report v0.1 § Research; feature-report Lesson #1 (19-day gap) | Feature Evidence — Research Part 3 was essential for clean Specify; Pre-Execution Review durability confirmed the value of the phase boundary that Part 3 enabled | Add *Research Delta* subsection to `sdd-operational.md` § Research: trigger, artifact structure (`Part N` in `research.md`), exit criteria (**READY FOR SPECIFY**) | **Proven** |
| **GOV-25** | Multi-part `research.md` needs explicit supersession rule (Part 3 governs operational status; Part 2 governs domain unless superseded) | Medium — planning errors if stale claims read without latest part | Pilot Report v0.1 § Research supersession; feature-report Lesson #1 | Feature Evidence — no agent acted on stale Part 2 Atendimento stub claims after Part 3 was published; supersession was implicit but effective | Add supersession note to `research.md` template header: "Read Part N first — it governs operational status. Earlier parts govern domain decisions unless explicitly superseded." | **Proven** |
| **GOV-26** | Prerequisite SDD should block Specify on downstream feature until Verify completes | Medium — v0.1 flagged risk of embedding upstream REQs incorrectly | Pilot Report v0.1 § GOV-26; Part 2 vs Part 3 | Feature Evidence — Atendimento Minimal was verified before Prontuario Execute; the sequencing held; Part 3 Specified correctly consumed verified contracts | Add prerequisite checklist item to `specify.md` template: distinguish **prerequisite verified** (consume only) vs **in-scope upstream REQ** (implement). Already partially addressed by Atendimento GIB-002; this confirms the pattern. | **Proven** |
| **GOV-27** | Versioning PUT semantic change is ADR-candidate | Medium — resolved by ADR-006 | Pilot Report v0.1 § GOV-27 | Verification — ADR-006 accepted at Pre-Execution Review 2026-06-19; implementation fully conformant; domain review gate confirmed | **Resolved.** No governance action needed. ADR-006 governs. | **Resolved** |
| **GOV-28** | SDD Pre-Execution Review proved durable across a 19-day gap and an intervening SDD (AI Harness Multi-Tool) — this is a proven pattern worth documenting as a governance strength | Low — already working; documentation value only | feature-report Lesson #1; verification.md § Drift Analysis (no governance drift) | Feature Evidence — 19-day gap between Pre-Exec Review and Execute; no implementation drift; all ADR decisions honored; Credential Probe passed on first re-run | Add note to `sdd-operational.md` § SDD Pre-Execution Review: "Pre-Execution Review is a durable phase boundary. Multi-session gaps do not invalidate it — re-run Credential Probe and baseline tests on resume." | **Proven** |

### Workflow Findings (WF-*)

| ID | Description | Impact | Evidence source | Validation source | Recommendation | Confidence |
|----|-------------|--------|-----------------|-------------------|----------------|------------|
| **WF-12** | Research Delta closes the loop when PM sequencing or prerequisite SDDs change after initial Research | Medium — v0.1 identified this; feature execution validated it | Pilot Report v0.1 § WF-12; feature-report Lesson #1 | Feature Evidence — Research Part 3 reconciled Part 1/2 against verified Atendimento Minimal; no second full discovery pass needed | Standardize: trigger Delta when prerequisite Verify completes or `State.md` sequencing changes. Align with GOV-24 formalization in `sdd-operational.md`. | **Proven** |
| **WF-13** | Forward SDD should inherit Execution Prerequisites from prior pilots without rewriting | Low — already working; documentation economy | Pilot Report v0.1 § WF-13 | Feature Evidence — Prontuario Prerequisites section referenced Atendimento runbook + scripts by link; Credential Probe reused without friction | Add "Inherit from prior verified SDD" instruction to `specify.md` template § Execution Prerequisites — link, don't rewrite. | **Proven** |
| **WF-14** | Composite aggregate SDDs need fixture chain documented early (Paciente → Atendimento → Prontuario v1 → correction → evolution → v2) | Medium — integration test design risk if fixture chain not explicit | Pilot Report v0.1 § WF-14; feature-report Lesson #4 | Feature Evidence — integration test with full fixture chain passed; TASK-008 done-when explicitly listed the chain; no integration test design friction | Add "Fixture Chain" subsection to `design.md` template for composite aggregates: list prerequisite entities and ordered creation steps for integration tests. | **Proven** |
| **WF-16** | TASK-009 Execute→Verify handoff for runtime validation evidence is unreliable — session notes not recorded | Medium — Verify lacked HTTP evidence; mitigated by 27 controller tests but would be higher-impact for features with fewer controller tests | feature-report Lesson #3; verification.md F-01 | Feature Evidence — TASK-009 checkbox marked complete but session notes absent; Verify could not independently confirm runtime HTTP behavior | Add explicit "Record session notes" step to TASK-009 in `tasks.md` template; add "TASK-009 session notes provided" to Verify § Runtime Validation entry criteria | **Proven** |

### Skill Findings (SK-*)

| ID | Skill | Description | Evidence source | Validation source | Recommendation | Confidence |
|----|-------|-------------|-----------------|-------------------|----------------|------------|
| **SK-14** | `sql-migration-workflow` | Third aggregate confirms repository-direct integration pattern is stable — skill update from Atendimento GIB-004 is sufficient | Pilot Report v0.1 § SK-14; feature-report Lesson #4 | Feature Evidence — `SqlIntegrationTestGate` + `SqlConnectionResolver` scaled cleanly to composite aggregate with versioning; no new abstractions needed | **Resolved.** Atendimento GIB-004 already updated skill. Third aggregate confirms no further update needed. | **Proven** |
| **SK-13** | `verifier` | Prontuario Verify was heavier than Atendimento (dual-mode API, nested graph, versioning chain evidence) but verifier skill handled it without modification | Pilot Report v0.1 § SK-13 | Feature Evidence — verifier applied all gates, selected appropriate review sensors, documented F-01 with severity and mitigation | **Deferred.** No skill change needed now. Re-evaluate after Agendamento Verify (comparable complexity to Atendimento, not Prontuario). | **Deferred** |

### Template & Artifact Findings (TPL-*)

| ID | Target artifact | Description | Evidence source | Validation source | Recommendation | Confidence |
|----|-----------------|-------------|-----------------|-------------------|----------------|------------|
| **DOC-01** | `specify.md` + `design.md` templates | D-02 version-generation authority conflict between specify.md ("aggregate owns version generation") and design.md ("Application layer obtains max, aggregate validates") required explicit reconciliation at Pre-Execution Review | feature-report Lesson #2; tasks.md § Version generation authority; Pilot Report v0.2 § Pre-Execution Review friction | Feature Evidence — reconciled at Pre-Execution Review 2026-06-19; ADR-006 + design.md declared authoritative; specify.md updated | Add "Cross-Artifact Consistency" checklist to SDD Pre-Execution Review: flag when specify.md and design.md make conflicting authority claims about the same domain decision; resolve before Execute authorization | **Proven** |

### Documentation & Routing Findings (DOC-*)

*(No documentation routing findings identified — all documentation follow-up was correct and complete.)*

### Experimental Findings (EXP-*)

*(No experimental findings identified — all observations have sufficient evidence for Preliminary or Proven confidence.)*

---

## Prior Pilot Deferred Items — Resolution

| Prior ID | Original recommendation | This pilot outcome | New confidence | Action |
|----------|-------------------------|-------------------|----------------|--------|
| **GOV-10** | Governance improvement feedback loop | Addressed at template level — `governance-improvement-plan.md` exists and this instance proves the feedback loop | **Proven** | **Adopt** — keep as operational workflow |
| **GOV-13–15** | Credential single source / skip messages / Docker password | Prontuario reused `.env` + scripts without rediscovery; Credential Probe passed on first attempt | **Proven** | **Adopt** — resolved by Atendimento Wave 1 GIB-001, GIB-003 |
| **GOV-21** | TASK-008 Execute vs Verify evidence split | TASK-008/TASK-009 split worked for repository-direct integration; TASK-009 handoff gap (WF-16) is the remaining friction | **Proven** (pattern) + new gap (WF-16) | **Adopt** — TASK/VP/DF classification confirmed; WF-16 addresses residual gap |
| **SK-11** | `sql-migration-workflow` skill update | Third aggregate confirmed pattern stability; Atendimento GIB-004 update sufficient | **Proven** | **Adopt** — no further skill update needed |
| **GOV-22** | PowerShell JSON BOM for API smoke | Not triggered — Prontuario Verify used controller unit tests, not PowerShell smoke scripts | **Preliminary** | **Keep deferred** — re-evaluate if Agendamento smoke uses PowerShell |

---

## Harness Maturity Evolution

| Evolution dimension | Prior calibration trend | This feature | Delta | Evidence |
|---------------------|------------------------|--------------|-------|----------|
| Workflow predictability | Paciente (emergent) → Atendimento (calibrating) | Prontuario: Pre-Execution Review durable; phase boundaries held; TASK-009 gap is only new friction | **Better** | 19-day gap → no drift; TASK/VP/DF respected; all 18 REQs traceable |
| Governance repeatability | Paciente (first lifecycle) → Atendimento (Wave 1–3 implemented) | Prontuario: prior calibration held; no regression; ADR-006 governed new API design | **Better** | Credential Probe reused; ADR-006 prevented scope creep; soft delete ADR-001 honored |
| Template stability | Paciente (new) → Atendimento (updated) | Prontuario: templates used as-is from Atendimento calibration; D-02 conflict is the only template-level gap discovered | **Stable** | No new template sections needed beyond D-02/authority conflict awareness |
| Documentation correction frequency | Paciente (high — State, PM, migration-sql, runbook all drifted) → Atendimento (lower) | Prontuario: no SDD drift; State.md + migration-sql.md updated correctly in Documentation Follow-Up | **Better** | Documentation review gate confirmed zero drift between specify/design/tasks and implementation |
| Verification consistency | Paciente (first pass) → Atendimiento (calibrated) | Prontuario: all 10 gates evaluated consistently; F-01 documented with severity, mitigation, and follow-up | **Stable** | Same gate set applied; new ADR-006 domain review gate added cleanly |
| New governance gap discovery rate | Paciente (12 GOV, 11 WF) → Atendimento (11 GOV, 8 WF) | Prontuario: 4 GOV (3 proven + 1 resolved), 3 WF (all proven), 1 DOC (proven) at Research; Execute/Verify promoted findings, added 2 more (GOV-28, WF-16) | **Better** — declining new-gap rate per feature | Net-new Proven: GOV-24, GOV-25, GOV-26, GOV-28, WF-16, DOC-01 (6 total). GOV-27 resolved. Prior deferred items resolved. |

**Summary:** The harness is trending toward predictable, repeatable governance. The declining rate of new governance gaps per feature (12 → 11 → 6 net-new Proven) confirms that prior calibration is accumulating effectively. The TASK-009 handoff gap (WF-16) is a narrow, fixable friction — not a structural failure.

---

## Maturity Assessment

| Dimension | Score | Justification |
|-----------|-------|---------------|
| SDD workflow | **4/5** | Pre-Execution Review durable; phase boundaries held; TASK-009 gap is the only friction (-1). Research Delta needs formalization but pattern is proven. |
| Verification | **4/5** | All gates evaluated; F-01 documented with severity and mitigation; runtime evidence gap (-1) was compensated but pattern needs hardening per WF-16. |
| Documentation follow-up & routing | **5/5** | Zero implementation drift across all SDD artifacts; State and migration-sql updated correctly; no unnecessary routing. |
| Reporting & handoff | **4/5** | Feature report and session handoff complete and actionable; TASK-009 session notes gap (-1) is a workflow issue, not a reporting template issue. |
| Knowledge transfer | **N/A** | Teacher Guide decision recorded (Generate) but not yet executed. |
| **Overall harness maturity (this feature)** | **4/5** | Third aggregate confirmed harness stability. Prior calibration held without regression. One point deducted for TASK-009 handoff gap and Research Delta remaining informal. |

### What Failed — Must Change

- **TASK-009 Execute→Verify handoff:** Session notes for Swagger smoke were not recorded. This is a **recurring pattern** — Atendimento also had TASK-008 handoff friction (GOV-21). The mitigation (controller unit tests) worked this time but would not suffice for features with fewer tests. **Must implement WF-16 before Agendamento.**

---

## Preserve Without Changes

| Practice | Justification | Evidence source |
|----------|---------------|-----------------|
| TASK/VP/DF classification in tasks template | Execute produced zero scope creep into Verify, Reporting, or Documentation Follow-Up | tasks.md § Execution Boundary; all 10 TASKs completed within Execute |
| SDD Pre-Execution Review as mandatory gate for Large features | 19-day gap + intervening SDD; no drift; all ADR decisions honored | feature-report Lesson #1; verification.md § Drift Analysis |
| Credential Probe in Execution Prerequisites | Reused without friction; Docker + .env pattern held across third aggregate | Pre-Execution Review report; TASK-004+ prerequisites satisfied |
| Repository-direct SQL integration test pattern | Scaled to composite aggregate with versioning and nested children without new abstractions | feature-report Lesson #4; `ProntuarioSqlIntegrationTests` single test covering full chain |
| ADR evaluation at Pre-Execution Review for non-trivial API design | ADR-006 prevented scope creep; correction vs evolution split was unambiguous enough to produce zero implementation divergence | feature-report Lesson #5; verification.md § ADR evaluation gate |
| Dual-mode versioning API (ADR-006) | Clinical correction and evolution are distinct operations with clear API semantics | All 18 REQs satisfied; 27 controller tests cover both paths; no client ambiguity |
| PHI-safe controller pattern (Paciente → Atendimento → Prontuario) | ProntuarioController: no Console.WriteLine, generic 404 messages, synthetic test data only | verification.md § Security/PHI gate; security-phi-review sensor passed |
| Soft delete per ADR-001 with `.IgnoreQueryFilters()` for version lookup | Version slot preservation correctly implemented with documented rationale | verification.md § Domain review gate; O-04 confirms code comment per `.clinerules/ef-migrations.md` |

---

## Workflow Evolution Metrics

| Metric | This feature | Prior calibration (Atendimento) | Trend |
|--------|-------------|----------------------------------|-------|
| Governance improvements identified (GOV-*) | 5 total (4 Proven + 1 Resolved) | 11 (GOV-13–23) | Declining → maturing |
| Workflow improvements identified (WF-*) | 4 total (3 Proven from v0.1 + 1 new WF-16) | 8 (WF-05–WF-11 + inherited) | Declining → maturing |
| Skill improvements identified (SK-*) | 2 total (1 Proven/Resolved + 1 Deferred) | 3 (SK-09–SK-11) | Stable |
| Template improvements identified (TPL-*/DOC-*) | 1 (DOC-01) | 3 (TPL-01–TPL-03) | Declining |
| Documentation improvements identified (DOC-*) | 0 | 0 | Stable |
| Experimental findings identified (EXP-*) | 0 | 0 | Stable |
| Verification gates skipped with rationale | 5 (UI, EF migration, HTTP integration, D-03 scenario, 409 integration) — all documented | 4 | Slight increase (composite aggregate has more optional gates) |
| Manual interventions during Execute | 0 | 0 | Stable |
| Reports generated | 3 (feature-report, session-handoff, this plan) | 3 (feature-report, session-handoff, governance-improvement-plan) | Stable |
| Calibration findings carried forward from prior plan | 5 resolved (GOV-10, GOV-13–15, GOV-21, SK-11) + 1 kept deferred (GOV-22) | 9 (from Paciente) | Declining carry-forward → prior calibration effective |

---

## Governance Impact Analysis

| Finding ID | Expected benefit | Potential regression | Affected governance artifacts | Estimated effort | Migration complexity | Rollback difficulty |
|------------|-----------------|---------------------|------------------------------|-----------------|---------------------|---------------------|
| GOV-24 + WF-12 | Formalizes Research Delta; prevents agents from repeating full Research or acting on stale blockers | Low — additive section; does not change existing phase boundaries | `sdd-operational.md` § Research | S | None | None |
| GOV-25 | Prevents stale-part reads in multi-part research documents | Low — template header note only | `research.md` template (or SDD README) | S | None | None |
| GOV-26 | Prevents downstream SDDs from incorrectly embedding prerequisite REQs | Low — checklist item addition; pattern already proven | `specify.md` template § Prerequisites | S | None | None |
| GOV-28 | Documents proven Pre-Execution Review durability pattern | Low — note addition only | `sdd-operational.md` § SDD Pre-Execution Review | S | None | None |
| WF-16 | Closes TASK-009 Execute→Verify handoff gap that recurred from Atendimento GOV-21 | Low — template update; no structural change | `tasks.md` template § TASK-009; `verification.md` template § Runtime Validation | S | None | None |
| WF-13 | Reduces template bloat; avoids redundant prerequisite documentation | Low — instruction addition | `specify.md` template § Execution Prerequisites | S | None | None |
| WF-14 | Improves integration test design clarity for composite aggregates | Low — optional subsection; additive | `design.md` template | S | None | None |
| DOC-01 | Catches specify/design authority conflicts before Execute | Low — checklist item in existing Pre-Execution Review | Pre-Execution Review checklist (in `sdd-operational.md` or review template) | S | None | None |

All findings have **Low** migration complexity and **None** rollback difficulty — they are additive template/documentation changes. No structural governance changes, no skill rewrites, no rule changes.

---

## Governance Improvement Backlog

### Wave 1 — Must Implement (before next forward SDD: Agendamento)

| Backlog ID | Source finding ID(s) | Change summary | Target artifact(s) | Wave | Owner | Effort | Acceptance criteria | Status |
|------------|------------------------|----------------|--------------------|------|-------|--------|---------------------|--------|
| **GIB-019** | GOV-24, WF-12 | Add *Research Delta* subsection to `sdd-operational.md` § Research: trigger (prerequisite Verify completes or `State.md` sequencing changes), artifact structure (`Part N` in `research.md`), exit criteria (**READY FOR SPECIFY**) | `Documentation/AI-Harness/Harness-Design/sdd-operational.md` | 1 | AI agent / human reviewer | S | Research Delta section exists in sdd-operational.md; trigger, structure, and exit criteria documented; Agendamento SDD can use it | Not started |
| **GIB-020** | WF-16 | Add explicit "Record session notes" requirement to TASK-009 in `tasks.md` template; add "TASK-009 session notes provided" to Verify § Runtime Validation entry criteria in `verification.md` template | `Documentation/AI-Harness/template/sdd/tasks.md`; `Documentation/AI-Harness/template/verification/verification.md` | 1 | AI agent / human reviewer | S | TASK-009 description includes "Record session notes with Swagger results for Verify handoff"; verification template § Runtime Validation includes prerequisite check for session notes | Not started |
| **GIB-021** | DOC-01 | Add "Cross-Artifact Consistency" checklist item to SDD Pre-Execution Review: flag when specify.md and design.md make conflicting authority claims about the same domain decision; resolve before Execute authorization | `Documentation/AI-Harness/Harness-Design/sdd-operational.md` § SDD Pre-Execution Review; or Pre-Execution Review checklist template | 1 | AI agent / human reviewer | S | Pre-Execution Review checklist includes cross-artifact consistency check for specify vs design authority conflicts; D-02 is cited as exemplar | Not started |

### Wave 2 — Should Implement (next 1–2 features)

| Backlog ID | Source finding ID(s) | Change summary | Target artifact(s) | Wave | Owner | Effort | Acceptance criteria | Status |
|------------|------------------------|----------------|--------------------|------|-------|--------|---------------------|--------|
| **GIB-022** | GOV-25 | Add supersession note to `research.md` template header: "Read Part N first — it governs operational status. Earlier parts govern domain decisions unless explicitly superseded." | `Documentation/AI-Harness/template/sdd/research.md` (or SDD README) | 2 | AI agent / human reviewer | S | Research template header includes supersession rule; Prontuario research.md cited as first exemplar | Not started |
| **GIB-023** | GOV-26 | Add prerequisite verification checklist item to `specify.md` template § Prerequisites: distinguish **prerequisite verified** (consume only) from **in-scope upstream REQ** (implement) | `Documentation/AI-Harness/template/sdd/specify.md` | 2 | AI agent / human reviewer | S | Specify template Prerequisites section includes verified-prerequisite vs in-scope-REQ distinction; Prontuario Part 3 checklist cited as exemplar | Not started |
| **GIB-024** | GOV-28 | Add durability note to `sdd-operational.md` § SDD Pre-Execution Review: "Pre-Execution Review is a durable phase boundary. Multi-session gaps do not invalidate it — re-run Credential Probe and baseline tests on resume." | `Documentation/AI-Harness/Harness-Design/sdd-operational.md` | 2 | AI agent / human reviewer | S | Durability note present in Pre-Execution Review section; Prontuario 19-day gap cited as evidence | Not started |
| **GIB-025** | WF-13 | Add "Inherit from prior verified SDD" instruction to `specify.md` template § Execution Prerequisites: link to prior runbook/scripts instead of rewriting | `Documentation/AI-Harness/template/sdd/specify.md` | 2 | AI agent / human reviewer | S | Execution Prerequisites section includes inheritance instruction with link pattern | Not started |
| **GIB-026** | WF-14 | Add optional "Fixture Chain" subsection to `design.md` template for composite aggregates: list prerequisite entities and ordered creation steps for integration tests | `Documentation/AI-Harness/template/sdd/design.md` | 2 | AI agent / human reviewer | S | Design template includes Fixture Chain subsection (marked optional for non-composite aggregates); Prontuario design.md § Fixture Chain cited as exemplar | Not started |

### Wave 3 — Deferred

| Backlog ID | Source finding ID(s) | Reason deferred | Validation trigger |
|------------|------------------------|-----------------|-------------------|
| **GIB-027** | SK-13 | Verifier skill handled Prontuario (heavier than Atendimento) without modification. No evidence that skill change is needed. | Agendamento Verify (comparable to Atendimento complexity); or next Large/Complex feature with novel verification patterns |
| — | GOV-22 (from Atendimento) | PowerShell JSON BOM issue not triggered by Prontuario Verify. | Agendamento smoke if PowerShell scripts are used for API validation |

---

## Target Artifact Map

| Destination | GIB IDs | Risk if skipped |
|-------------|---------|-----------------|
| `sdd-operational.md` | GIB-019 (Research Delta), GIB-021 (Cross-Artifact Consistency), GIB-024 (Pre-Exec Review Durability) | Agents repeat full Research on prerequisite changes; specify/design authority conflicts go undetected until Execute; multi-session gap handling remains undocumented |
| `template/sdd/tasks.md` | GIB-020 (TASK-009 session notes) | Runtime evidence gaps recur in Agendamento Verify — controller test mitigation may not suffice for simpler features |
| `template/verification/verification.md` | GIB-020 (TASK-009 session notes entry criteria) | Verify cannot independently confirm runtime HTTP behavior — reliance on unit test mitigation increases residual risk |
| `template/sdd/specify.md` | GIB-023 (prerequisite checklist), GIB-025 (inherit prerequisites) | Downstream SDDs may incorrectly embed upstream REQs; redundant prerequisite documentation bloat |
| `template/sdd/research.md` | GIB-022 (supersession rule) | Agents may read stale Part 2 claims without Part 3 context |
| `template/sdd/design.md` | GIB-026 (Fixture Chain subsection) | Composite aggregate integration test design remains ad hoc |

---

## Phased Execution Plan

### Wave 1 — Critical Path (before Agendamento SDD)

| Step | GIB ID | Task | Depends on | Verification of change |
|------|--------|------|------------|-------------------------|
| 1 | GIB-019 | Add Research Delta subsection to `sdd-operational.md` | None | Dry-run: Agendamento research can reference Research Delta trigger and exit criteria |
| 2 | GIB-020 | Update `tasks.md` and `verification.md` templates for TASK-009 session notes requirement | None | Dry-run: Agendamento tasks template includes "Record session notes" in TASK-009; verification template checks for notes |
| 3 | GIB-021 | Add Cross-Artifact Consistency checklist to Pre-Execution Review (in `sdd-operational.md`) | None | Dry-run: Agendamento Pre-Execution Review checklist includes specify-vs-design authority conflict check |

**Wave 1 Definition of Done:**
- [ ] All Wave 1 GIB items marked complete
- [ ] No conflict with accepted ADRs or authority hierarchy
- [ ] `Documentation/State.md` notes Wave 1 calibration completed for Prontuario
- [ ] Agendamento SDD can cite updated `sdd-operational.md` and templates as active guidance

### Wave 2 — Secondary (Agendamento or next feature)

| Step | GIB ID | Task | Depends on | Verification of change |
|------|--------|------|------------|-------------------------|
| 1 | GIB-022 | Add supersession rule to `research.md` template header | None (independent of Wave 1) | Agendamento research.md header includes supersession note |
| 2 | GIB-023 | Add prerequisite distinction to `specify.md` template | None | Agendamento specify.md Prerequisites distinguishes verified-prerequisite from in-scope REQ |
| 3 | GIB-024 | Add durability note to `sdd-operational.md` § Pre-Execution Review | None | Note present; Prontuario 19-day gap cited |
| 4 | GIB-025 | Add inheritance instruction to `specify.md` template § Execution Prerequisites | None | Agendamento Prerequisites links to prior SDDs instead of rewriting |
| 5 | GIB-026 | Add Fixture Chain subsection to `design.md` template | None | Agendamento design.md includes Fixture Chain if composite aggregate |

### Wave 3 — Deferred (explicit non-goals for now)

| GIB ID | Reason deferred | Validation trigger |
|--------|-----------------|-------------------|
| GIB-027 (SK-13) | Verifier skill handled Prontuario without modification; no evidence of need | Next Large/Complex feature with novel verification patterns |
| GOV-22 (from Atendimento) | Not triggered by Prontuario Verify | Agendamento smoke if PowerShell scripts used for API validation |

---

## Consistency Audit Checklist

Run after each wave before marking the plan complete.

- [ ] Updated artifacts agree with `sdd-operational.md` authority hierarchy
- [ ] Templates reference governance sections that now exist (no dangling section names)
- [ ] Verifier skill and verification template aligned on new TASK-009 session notes policy
- [ ] Reporting strategy boundaries preserved (no authority duplication between feature-report, session-handoff, and governance-improvement-plan)
- [ ] `AGENTS.md` unchanged unless bootstrap commands or map materially changed
- [ ] `documentation-index.md` updated if new templates or harness docs added
- [ ] No secrets, credentials, or PHI introduced in docs or examples
- [ ] Prior pilot **Proven** findings (GOV-10, GOV-13–15, GOV-21, SK-11, GOV-22) marked **Adopted** or **Deferred** in this plan's Prior Pilot Deferred Items table

---

## Open Questions

| Question | Why it matters | Owner | Validation trigger |
|----------|----------------|-------|-------------------|
| Should TASK-009 Swagger smoke become an automated HTTP integration test instead of a manual checklist? | WF-16 handoff gap could be eliminated entirely by making runtime validation durable and automated | AI agent / human reviewer | Agendamento — if manual Swagger smoke also has handoff friction, promote to automated |
| Should the Pre-Execution Review consolidation template (from v0.2 friction) be formalized into the template library? | Only one instance exists; too early to generalize | AI agent / human reviewer | Second use in Agendamento Pre-Execution Review |
| Should `sdd-pilot-report` version increments at Execute/Verify be mandatory or optional? | Prontuario only produced v0.1 (Research) and v0.2 (Pre-Exec Review) — no Execute or Verify increment | AI agent / human reviewer | Agendamento — if Execute/Verify produces significant new findings, mandatory increment may be warranted |

---

## Recommended Posture For Next Forward SDD (Agendamento)

### Apply before starting

1. **GIB-019:** Research Delta is formalized in `sdd-operational.md` — Agendamento research can use it if prerequisites complete mid-discovery.
2. **GIB-020:** TASK-009 in Agendamento tasks.md must include explicit "Record session notes" requirement.
3. **GIB-021:** Agendamento Pre-Execution Review must include Cross-Artifact Consistency check (specify vs design authority conflicts).

### Keep unchanged

- TASK/VP/DF classification — proven across three features
- Credential Probe in Execution Prerequisites — reused without friction
- Repository-direct SQL integration test pattern — scaled to composite aggregates
- SDD Pre-Execution Review as mandatory gate for Large features
- Dual-mode versioning API pattern (ADR-006) — if Agendamento needs versioning, ADR-006 governs
- PHI-safe controller pattern
- Soft delete per ADR-001

### Explicitly out of scope for this calibration

- `sql-migration-workflow` skill update — Atendimento GIB-004 sufficient; SK-14 confirmed
- Verifier skill update — SK-13 deferred; no evidence of need
- New ADRs — no architectural decisions emerged from Prontuario that require ADR evaluation
- Rule changes — no `.cursor/rules/` or `.clinerules/` changes needed
- `AGENTS.md` changes — bootstrap unchanged
- Metrics / observability dashboard — deferred by policy (Atendimento GIB-018)

---

## Appendix A — Evidence Index

| Artifact | Path | Role in this plan |
|----------|------|-------------------|
| Verification | [verification.md](../verification.md) | Primary evidence source: F-01, all gates, requirement traceability, drift analysis, residual risk |
| Feature report | [feature-report.md](feature-report.md) | Lessons Learned; scope; requirements delivered; Teacher Guide decision |
| Pilot report v0.1 | [sdd-pilot-report-v0.1.md](sdd-pilot-report-v0.1.md) | Research-phase GOV-24–27, WF-12–15, SK-12–14; attention register A1–A10; prior pilot deferred items |
| Pilot report v0.2 | [sdd-pilot-report-v0.2.md](sdd-pilot-report-v0.2.md) | Pre-Execution Review friction; phase status |
| Session handoff | [session-handoff.md](session-handoff.md) | Phase completion status; next steps; open questions |
| SDD specify | [specify.md](../specify.md) | Requirements REQ-001–018; Scope boundaries; Legacy Behavior table |
| SDD design | [design.md](../design.md) | D-01–D-06; API Contract table; Requirement Mapping; Fixture Chain reference |
| SDD tasks | [tasks.md](../tasks.md) | TASK-001–010, VP-001, DF-001; Execution Prerequisites; TASK-008/TASK-009 split |
| Atendimento governance plan | [../../atendimento-minimal-sql-stabilization/reports/governance-improvement-plan.md](../../atendimento-minimal-sql-stabilization/reports/governance-improvement-plan.md) | Prior calibration GIB-001–018; Wave 1–3 completion status |
| Paciente pilot report | [../../paciente-sql-stabilization/reports/sdd-pilot-report-v1.0.md](../../paciente-sql-stabilization/reports/sdd-pilot-report-v1.0.md) | Original GOV/WF baseline; deferred items |

---

## Appendix B — Finding Confidence Definitions

| Level | Meaning | Eligible for Wave 1? |
|-------|---------|----------------------|
| **Proven** | Observed in this feature with clear evidence; repeated or high-impact | Yes |
| **Preliminary** | Likely valid; needs second pilot or small experiment | Wave 2 only unless review override |
| **Deferred** | Hypothesis or low priority; insufficient evidence | No — Wave 3 |

---

## Appendix C — Validation Source Definitions

| Source | Meaning |
|--------|---------|
| **Feature Evidence** | Direct observation during this feature's Execute or Verify phases |
| **Verification** | Identified through a verification gate, review prompt, or residual risk assessment |
| **Feature Report** | Documented in feature-report.md Lessons Learned |
| **Pilot Report** | Documented in sdd-pilot-report GOV/WF sections |
| **Repeated Observation** | Same finding observed across two or more features or pilots |
| **Prior Improvement Plan** | Inherited from a prior governance improvement plan or deferred item |

---

## Appendix D — Plan Lineage

| Version | Date | Scope | Status |
|---------|------|-------|--------|
| v0.1 | 2026-07-10 | Initial draft from prontuario-sql-stabilization reports | Ready for review |

**Next version trigger:** After Wave 1 implementation, bump to v0.2 with implemented GIB statuses and residual open items.

---

*End of Governance Improvement Plan*