## Governance Improvement Plan — [Feature Name]

### How To Use This Template
#### Prerequisites
Do not start until all are true:
- [ ] Verify is complete with a documented completion decision in `verification.md`
- [ ] Documentation Follow-Up has synchronized operational truth (`State.md`, PM, migration-sql, runbook when applicable)
- [ ] Reporting artifacts exist (`feature-report.md`, `session-handoff.md`)
- [ ] Pilot or consolidated retrospective exists when the feature was a calibration pilot (`sdd-pilot-report-v*.md`)
#### Primary Inputs (read in this order)
| Priority | Source | Extract |
|----------|--------|---------|
| 1 | `Documentation/State.md` | Current operational truth; blockers; calibration status |
| 2 | `Documentation/SDD/<feature-slug>/verification.md` | Gates, skipped gates, residual risk, requirement evidence |
| 3 | `Documentation/SDD/<feature-slug>/reports/feature-report.md` | Lessons learned; harness calibration candidates |
| 4 | `Documentation/SDD/<feature-slug>/reports/sdd-pilot-report-v*.md` | GOV/WF findings; friction; template candidates; maturity notes |
| 5 | `Documentation/SDD/<feature-slug>/reports/session-handoff.md` | Phase completion; open questions; handoff gaps |
| 6 | `Documentation/SDD/<feature-slug>/specify.md`, `design.md`, `tasks.md` | Scope boundaries; prerequisites; task/VP/DF structure; deviations |
| 7 | Prior pilot improvement plans or pilot reports | Deferred items to resolve; ID continuity (GOV-*, WF-*, SK-*, TPL-*) |
**Do not** re-derive feature requirements or re-run verification in this plan. Link to authoritative artifacts instead.
#### Output Location
Choose one destination and record it in the header:
| Context | Recommended path |
|---------|------------------|
| Feature-scoped calibration (default) | `Documentation/SDD/<feature-slug>/reports/governance-improvement-plan.md` |
| Cross-pilot consolidation | `Documentation/AI-Harness/research/governance-improvement-plan-<consolidation-slug>.md` |
After approval and phased rollout, record calibration completion in `Documentation/State.md`.
#### Relationship To Other Artifacts
```text
Feature SDD lifecycle completes
-> Pilot Report / Feature Report (evidence)
-> Governance Improvement Plan (this template)
-> Governance Review / Approval
-> Phased Harness Update (Wave 1 → Wave 2 → Wave 3)
-> Consistency Audit
-> Next forward SDD
```
Pilot reports capture **what happened**. This plan captures **what to change in governance and operational artifacts**, with owners, waves, and acceptance criteria.

### Objective

This phase does not improve the implemented feature.

Its purpose is to improve the AI Harness governance, workflow, templates, operational documentation, engineering process, and supporting artifacts based on evidence collected during the completed feature.

This is a process-improvement phase.

Feature implementation is already considered complete.

This phase identifies improvements for future SDD executions.

### Plan Metadata
| Field | Value |
|-------|-------|
| **Feature** | [Feature Name] |
| **Feature slug** | `<feature-slug>` |
| **Active SDD** | `Documentation/SDD/<feature-slug>/` |
| **Pilot type** | First pilot / Forward SDD calibration / Maintenance SDD / Other |
| **SDD sizing** | Small / Medium / Large / Complex |
| **Plan version** | v0.1 |
| **Status** | Draft / Ready for review / Approved / Partially implemented / Complete |
| **Author / session** | [Agent or human] |
| **Date** | YYYY-MM-DD |
| **Supersedes** | [Prior plan version or N/A] |
| **Evidence bundle** | Links to verification, feature-report, pilot-report, session-handoff |

### Executive Summary
Write 3–6 sentences covering:
1. What the feature validated or broke in the harness workflow
2. Whether prior deferred findings (from earlier pilots) were confirmed, resolved, or remain open
3. How many findings are **Proven** vs **Preliminary** vs **Deferred**
4. Recommended rollout posture: **implement before next SDD** / **implement in parallel** / **defer to third pilot**

### Feature Context (Minimal)
Link; do not duplicate feature-report scope tables.
| Item | Value |
|------|-------|
| PM item | ... |
| Requirements delivered | REQ-xxx summary or link |
| Completion decision | From verification.md |
| Residual risk | Low / Medium / High — link to verification.md |
| Key friction themes | 2–4 bullets (e.g., credential drift, TASK-008 handoff, doc drift) |

### Calibration Scope

Every finding must belong to a single primary improvement scope. Assign each finding to one of the following before populating the Findings Registry.

| Scope | Definition | Example finding |
|-------|------------|-----------------|
| **Governance** | Authority hierarchy, ownership model, decision rights, ADR process, constraint enforcement | GOV finding: ADR trigger was unclear for schema changes |
| **Workflow** | SDD phase sequencing, phase boundaries, handoffs, lifecycle ordering | WF finding: Research phase overflowed into Specify |
| **Templates** | SDD and harness template structure, section completeness, placeholder quality, reuse | TPL finding: tasks.md lacked rollback steps |
| **Skills** | Skill instructions, skill boundaries, skill activation criteria, skill-to-skill handoff | SK finding: verifier skill did not route documentation follow-up |
| **Documentation** | Documentation routing, index accuracy, artifact freshness, operational truth synchronization | DOC finding: runbook.md missed new connection string format |
| **Reporting** | Feature reports, pilot reports, session handoffs, reporting strategy boundaries | DOC finding: feature-report.md omitted verification residual risk |
| **Verification** | Gate selection, review prompts, evidence standards, skipped gate rationale | GOV finding: verification gate skipped without documented reason |
| **Operational Process** | Day-to-day execution patterns, session management, context carryover, tool usage conventions | WF finding: context was lost between Execute sessions |
| **Engineering Practices** | Code patterns, testing strategy, migration hygiene, build/toolchain conventions | WF finding: integration test setup was inconsistent across sessions |
| **Tooling** | Generic engineering tooling (build scripts, test runners, Docker, dotnet CLI, git hooks) — not AI-tool-specific | DOC finding: sql-integration-test.ps1 required manual .env sourcing |

### Workflow Performance Evaluation
Rate each lifecycle phase for **this feature**. Use: **Strong** / **Adequate** / **Weak** / **Not executed** / **N/A**.
Add one sentence of evidence and one improvement candidate per phase.
| Phase | Rating | Evidence (link or artifact) | Primary friction | Improvement candidate |
|-------|--------|----------------------------|------------------|----------------------|
| Research | | | | |
| Specify | | | | |
| Design | | | | |
| Tasks | | | | |
| SDD Pre-Execution Review | | | | |
| Execute | | | | |
| Verify | | | | |
| Documentation Follow-Up | | | | |
| Reporting | | | | |
| Teacher Guide | | | | |
#### Phase Ownership Assessment
| Question | Answer | Finding ID (if any) |
|----------|--------|---------------------|
| Did any phase do another phase's job? | Yes / No — describe | |
| Did any phase defer work that blocked the next phase? | Yes / No — describe | |
| Were Execute boundaries respected? | Yes / No — describe | |
| Was operational truth synchronized before Reporting? | Yes / No — describe | |

### Governance Performance Evaluation

Rate each governance dimension for **this feature**. Use: **Strong** / **Adequate** / **Weak** / **Not exercised**.
| Dimension | Rating | Evidence (link or artifact) | Improvement opportunity |
|-----------|--------|----------------------------|-------------------------|
| Authority hierarchy | | | |
| Ownership model | | | |
| Phase boundaries | | | |
| Evidence strategy | | | |
| Documentation routing | | | |
| Review process | | | |
| Operational governance | | | |
| Governance consistency | | | |

### Harness Validation

Document which governance assumptions were tested by this feature's execution.

#### Assumptions Validated
List governance assumptions that held true during this feature, with supporting evidence.
| Assumption | Evidence | Validation strength (Strong / Moderate / Tentative) |
|------------|----------|-----------------------------------------------------|
| | | |

#### Assumptions Invalidated
List governance assumptions that proved incorrect. Each must have a corresponding finding in the Findings Registry.
| Assumption | What broke | Finding ID |
|------------|------------|------------|
| | | |

#### Assumptions Requiring Future Validation
List governance assumptions that could not be tested by this feature and remain open.
| Assumption | Why not validated | Recommended trigger |
|------------|-------------------|---------------------|
| | | |

#### Decisions Stable Enough to Adopt
List governance decisions that graduated from experimental to adopted during or because of this feature.
| Decision | Prior confidence | New confidence | Adoption artifact (rule, skill, template, ADR) |
|----------|-----------------|----------------|------------------------------------------------|
| | | | |

### Findings Registry
Consolidate findings from feature-report Lessons Learned, pilot-report GOV/WF sections, verification Findings, and session-handoff notes.

**ID convention:** continue prior pilot numbering when possible (`GOV-24`, `WF-12`, `SK-12`, `TPL-07`). New domains may use `DOC-*`, `RULE-*`, `RP-*` when the change target is not workflow/governance/skill/template.
#### Governance Findings (GOV-*)
| ID | Description | Impact | Evidence source | Validation source | Recommendation | Confidence |
|----|-------------|--------|-----------------|-------------------|----------------|------------|
| GOV-XX | | High / Medium / Low | feature-report § / pilot-report § / verification § | Feature Evidence / Verification / Feature Report / Pilot Report / Repeated Observation / Prior Improvement Plan | | Proven / Preliminary / Deferred |
#### Workflow Findings (WF-*)
| ID | Description | Impact | Evidence source | Validation source | Recommendation | Confidence |
|----|-------------|--------|-----------------|-------------------|----------------|------------|
| WF-XX | | | | | | |
#### Skill Findings (SK-*)
| ID | Skill | Description | Evidence source | Validation source | Recommendation | Confidence |
|----|-------|-------------|-----------------|-------------------|----------------|------------|
| SK-XX | verifier / documentation-update / sql-migration-workflow / not-a-teacher / other | | | | | |
#### Template & Artifact Findings (TPL-*)
| ID | Target artifact | Description | Evidence source | Validation source | Recommendation | Confidence |
|----|-----------------|-------------|-----------------|-------------------|----------------|------------|
| TPL-XX | specify / design / tasks / verification / feature-report / session-handoff / pilot-report / other | | | | | |
#### Documentation & Routing Findings (DOC-*)
| ID | Target doc | Description | Evidence source | Validation source | Recommendation | Confidence |
|----|------------|-------------|-----------------|-------------------|----------------|------------|
| DOC-XX | State.md / PM / migration-sql / runbook / sdd-operational / verification-governance / other | | | | | |
#### Experimental Findings (EXP-*)

Not every observation should immediately become governance. This section captures exploratory ideas that lack sufficient evidence for **Proven** or **Preliminary** confidence.

These findings normally feed Wave 3 unless future features validate them to **Preliminary** or **Proven**.

| ID | Description | Hypothesis | Evidence source | Validation source | What would prove it | Confidence |
|----|-------------|------------|-----------------|-------------------|---------------------|------------|
| EXP-XX | | | | | | Deferred |

### Prior Pilot Deferred Items — Resolution
List items marked **Deferred** in prior pilot reports or improvement plans. State whether this feature confirms, resolves, or keeps them deferred.
| Prior ID | Original recommendation | This pilot outcome | New confidence | Action |
|----------|-------------------------|-------------------|----------------|--------|
| GOV-10 | Governance improvement feedback loop | | Proven / Preliminary / Deferred | Adopt / Keep deferred / Superseded |
| ... | | | | |

### Harness Maturity Evolution

Compare this feature against prior feature calibrations to evaluate process evolution, not feature evolution.

| Evolution dimension | Prior calibration trend | This feature | Delta (Better / Stable / Regressed) | Evidence |
|---------------------|------------------------|--------------|-------------------------------------|----------|
| Workflow predictability | | | | |
| Governance repeatability | | | | |
| Template stability | | | | |
| Documentation correction frequency | | | | |
| Verification consistency | | | | |
| New governance gap discovery rate | | | | |

**Summary:** 1–2 sentences on whether the harness is trending toward more predictable, repeatable governance.

### Maturity Assessment
Score **1–5** with one-sentence justification each. Compare to prior pilot when available.
| Dimension | Score | Justification |
|-----------|-------|---------------|
| SDD workflow | /5 | |
| Verification | /5 | |
| Documentation follow-up & routing | /5 | |
| Reporting & handoff | /5 | |
| Knowledge transfer (if applicable) | /5 or N/A | |
| **Overall harness maturity (this feature)** | /5 | |
#### What Failed — Must Change
List recurring failure modes with evidence:
- ...

### Preserve Without Changes

Identify governance practices that consistently worked well and should remain unchanged. Each recommendation must include a short justification based on evidence from this feature.

| Practice | Justification | Evidence source |
|----------|---------------|-----------------|
| | | |

### Workflow Evolution Metrics

Record lightweight quantitative signals to enable maturity trend comparison across future features. Precision is not required; these metrics exist to observe long-term trends.

| Metric | This feature | Prior calibration (if available) | Trend |
|--------|-------------|----------------------------------|-------|
| Governance improvements identified (GOV-*) | | | |
| Workflow improvements identified (WF-*) | | | |
| Skill improvements identified (SK-*) | | | |
| Template improvements identified (TPL-*) | | | |
| Documentation improvements identified (DOC-*) | | | |
| Experimental findings identified (EXP-*) | | | |
| Verification gates skipped with rationale | | | |
| Manual interventions during Execute | | | |
| Reports generated (feature-report, pilot-report, session-handoff) | | | |
| Calibration findings carried forward from prior plan | | | |

### Governance Impact Analysis

Before adding items into the Governance Improvement Backlog, assess the impact of each significant governance change. Complete this for every **Proven** or high-impact **Preliminary** finding that targets a governance, workflow, or template artifact.

| Finding ID | Expected benefit | Potential regression | Affected governance artifacts | Estimated implementation effort (S/M/L) | Migration complexity (None / Low / Medium / High) | Rollback difficulty (None / Low / Medium / High) |
|------------|-----------------|---------------------|------------------------------|------------------------------------------|---------------------------------------------------|---------------------------------------------------|
| | | | | | | |

Findings with **High** migration complexity or **High** rollback difficulty should be explicitly discussed in the Governance Improvement Backlog wave assignment rationale.

### Governance Improvement Backlog
This section is the **execution core**. Every row must be actionable.

**Confidence gate (from CONTRIBUTING-AI):**
- **Wave 1 (Must Implement):** **Proven** findings only — implement before the next comparable SDD unless explicitly overridden in review
- **Wave 2 (Should Implement):** **Proven** or high-value **Preliminary** with low blast radius
- **Wave 3 (Defer):** **Preliminary** or **Deferred** until another pilot or explicit approval

| Backlog ID | Source finding ID(s) | Change summary | Target artifact(s) | Wave | Owner | Effort S/M/L | Acceptance criteria | Status |
|------------|------------------------|----------------|--------------------|------|-------|----------------|---------------------|--------|
| GIB-001 | GOV-XX | | `Documentation/AI-Harness/Harness-Design/sdd-operational.md` | 1 | | S | | Not started |
| GIB-002 | TPL-XX | | `Documentation/AI-Harness/template/sdd/tasks.md` | 1 | | M | | Not started |
| ... | | | | | | | | |
#### Target Artifact Map
Group backlog items by destination for review:
| Destination | GIB IDs | Risk if skipped |
|-------------|---------|-----------------|
| Harness Design docs | | |
| Templates | | |
| Rules (`.cursor/rules/`) | | |
| Skills (`.cursor/skills/`) | | |
| Review prompts | | |
| Technical / runbook docs | | |
| Scripts / tooling | | |
| ADR evaluation (new ADR candidate) | | |

### Phased Execution Plan
Turn Wave 1–3 backlog items into ordered work packages. Each package should be completable in one focused session or PR.
#### Wave 1 — Critical Path (before next forward SDD)
| Step | GIB ID | Task | Depends on | Verification of change |
|------|--------|------|------------|-------------------------|
| 1 | | | | Consistency audit: affected template used in dry-run outline |
| 2 | | | | |
**Wave 1 Definition of Done:**
- [ ] All Wave 1 GIB items marked complete
- [ ] No conflict with accepted ADRs or authority hierarchy
- [ ] `Documentation/State.md` notes calibration wave completed
- [ ] Next forward SDD can cite updated artifacts as active guidance
#### Wave 2 — Secondary (next 1–2 features)
| Step | GIB ID | Task | Depends on | Verification of change |
|------|--------|------|------------|-------------------------|
| 1 | | | | |
#### Wave 3 — Deferred (explicit non-goals for now)
| GIB ID | Reason deferred | Validation trigger |
|--------|-----------------|-------------------|
| | | Third pilot / recurring friction / explicit approval |

### Consistency Audit Checklist
Run after each wave before marking the plan complete.
- [ ] Updated artifacts agree with `sdd-operational.md` authority hierarchy
- [ ] Templates reference governance sections that now exist (no dangling section names)
- [ ] Verifier skill and verification template aligned on new policies
- [ ] Reporting strategy boundaries preserved (no authority duplication)
- [ ] `AGENTS.md` unchanged unless bootstrap commands or map materially changed
- [ ] `documentation-index.md` updated if new templates or harness docs added
- [ ] No secrets, credentials, or PHI introduced in docs or examples
- [ ] Prior pilot **Proven** findings marked **Adopted** in plan metadata or State when implemented

### Open Questions
| Question | Why it matters | Owner | Validation trigger |
|----------|----------------|-------|-------------------|
| | | | |

### Recommended Posture For Next Forward SDD
#### Apply before starting
1. ...
2. ...
#### Keep unchanged
- ...
#### Explicitly out of scope for this calibration
- ...

### Appendix A — Evidence Index
| Artifact | Path | Role in this plan |
|----------|------|-------------------|
| Verification | | |
| Feature report | | |
| Pilot report | | |
| Session handoff | | |
| SDD specify | | |
| SDD design | | |
| SDD tasks | | |
| Teacher guide | | |
| Prior improvement plan | | |

### Appendix B — Finding Confidence Definitions
| Level | Meaning | Eligible for Wave 1? |
|-------|---------|----------------------|
| **Proven** | Observed in this feature with clear evidence; repeated or high-impact | Yes |
| **Preliminary** | Likely valid; needs second pilot or small experiment | Wave 2 only unless review override |
| **Deferred** | Hypothesis or low priority; insufficient evidence | No — Wave 3 |

### Appendix C — Validation Source Definitions
| Source | Meaning |
|--------|---------|
| **Feature Evidence** | Direct observation during this feature's Execute or Verify phases |
| **Verification** | Identified through a verification gate, review prompt, or residual risk assessment |
| **Feature Report** | Documented in feature-report.md Lessons Learned |
| **Pilot Report** | Documented in sdd-pilot-report GOV/WF sections |
| **Repeated Observation** | Same finding observed across two or more features or pilots |
| **Prior Improvement Plan** | Inherited from a prior governance improvement plan or deferred item |

### Appendix D — Plan Lineage
| Version | Date | Scope | Status |
|---------|------|-------|--------|
| v0.1 | | Initial draft from [feature-slug] reports | Draft |
**Next version trigger:** After Wave 1 implementation, bump to v0.2 with implemented GIB statuses and residual open items.

*End of Governance Improvement Plan template instance*