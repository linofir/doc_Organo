# SDD Pilot Report Governance

## Purpose

This document governs **SDD Pilot Reports** — phased, versioned retrospectives that evaluate how the **harness workflow** performed during a feature SDD, not whether the feature itself is correct.

Pilot reports exist so that, over multiple features and pilots, the project can:

- Compare lifecycle phases (Research → Specify → … → Teacher Guide) with consistent vocabulary.
- Accumulate **calibration findings** (GOV-*, WF-*, SK-*, TPL-*, DOC-*) with explicit confidence.
- Separate **what worked** from **what must change** before the next forward SDD.
- Feed the **Governance Improvement Plan** with evidence — without treating chat history or session notes as the system of record.

Pilot reports are **workflow evaluation artifacts**. They do not replace feature requirements, verification decisions, or operational status.

**Relationship to sibling docs:** Feature continuity and completion summaries belong to [reporting-strategy.md](reporting-strategy.md). Harness changes adopted from pilot evidence belong to [CONTRIBUTING-AI.md](../CONTRIBUTING-AI.md) § Harness Calibration Workflow and [governance-improvement-plan.md](../template/governance/governance-improvement-plan.md) instances. Feature lifecycle rules belong to [sdd-operational.md](sdd-operational.md).

---

## Authority And Boundaries

### What SDD Pilot Reports own

| Responsibility | Owner |
|----------------|-------|
| Per-phase workflow performance rating (Strong / Adequate / Weak / Not executed) | Pilot report |
| Harness friction themes across the SDD lifecycle | Pilot report |
| Calibration findings with IDs and confidence (GOV-*, WF-*, etc.) | Pilot report |
| Cross-pilot comparison inputs (prior deferred items, maturity scores) | Pilot report |
| Template / skill / governance **candidates** (not adopted changes) | Pilot report |
| Report version lineage and supersession | Pilot report |
| Pre-implementation attention register (before Execute) | Pilot report (early versions) |

### What SDD Pilot Reports do not own

| Topic | Authoritative owner |
|-------|---------------------|
| Current branch, runtime status, blockers, next steps | `Documentation/State.md` |
| Feature scope, acceptance criteria, design, tasks | Active SDD (`specify.md`, `design.md`, `tasks.md`) |
| Gate results, skipped gates, completion decision, residual risk acceptance | `verification.md` + Verifier responsibility |
| Session continuity for the next chat | `session-handoff.md` ([reporting-strategy.md](reporting-strategy.md)) |
| Feature completion summary after verified work | `feature-report.md` ([reporting-strategy.md](reporting-strategy.md)) |
| Adopted harness policy | Harness Design docs after governance review |
| Durable architecture decisions | ADRs |
| Backlog sequencing | PM |

### Boundary vs Reporting Strategy

[reporting-strategy.md](reporting-strategy.md) defines **communication reporting** for session and feature continuity. SDD Pilot Report Governance defines **calibration reporting** for workflow evaluation.

| Dimension | Reporting Strategy | SDD Pilot Report Governance (this doc) |
|-----------|-------------------|----------------------------------------|
| **Primary question** | “What happened on this feature, and what should the next session know?” | “How did the harness workflow perform, and what should we change for the next SDD?” |
| **Typical start** | After Verify (+ Documentation Follow-Up for feature report) | From **Research exit** (or first material SDD phase) |
| **Typical end** | Feature report + optional session handoff | Consolidated report after Verify or full lifecycle; feeds improvement plan |
| **Audience** | Next implementer / verifier session | Harness calibrator / governance reviewer |
| **Findings** | Lessons learned embedded in feature report | Structured GOV/WF/SK/TPL/DOC registry with confidence |
| **Authority** | Communication only | Calibration input only — not harness truth until promoted |
| **Versioning** | Usually one feature report per completed slice | **Multiple versions per feature** (`v0.1`, `v0.2`, …) |

No conflict: a single feature may produce **both** a pilot report series (workflow) and a feature report (completion). They answer different questions.

```text
Feature SDD lifecycle
  │
  ├─ Pilot Report v0.x (parallel, from Research) ──► workflow evaluation
  │
  └─ Verify → Documentation Follow-Up
         └─ feature-report.md + session-handoff.md ──► continuity (Reporting Strategy)
                └─ governance-improvement-plan.md ──► harness changes (uses both inputs)
```

---

## When To Create A Pilot Report

### Required

Create and maintain a pilot report series when **all** apply:

1. Work is backed by a feature SDD under `Documentation/SDD/<feature-slug>/`.
2. Sizing is **Large** or **Complex** per [sdd-operational.md](sdd-operational.md), **or** the work is an explicit **harness calibration pilot** (first migration vertical, forward aggregate SDD, or Wave validation target).
3. The intent includes learning that should influence **future SDDs or harness artifacts**.

Current calibration sequence examples: Paciente (#1), Atendimento Minimal (#2), Prontuario (#3).

### Optional

Skip pilot reports for Small/Medium work unless the session explicitly calibrates governance.

### Not a substitute for

- Skipping Verify, Documentation Follow-Up, or feature report when Large SDD work completes.
- Updating `State.md` or SDD artifacts from pilot report prose.

---

## Location, Naming, And Language

| Rule | Value |
|------|-------|
| **Path** | `Documentation/SDD/<feature-slug>/reports/sdd-pilot-report-v<major>.<minor>.md` |
| **Initial version** | `v0.1` at first published report for the feature |
| **Language** | English (same as other generated SDD artifacts) |
| **Template** | [template/reporting/sdd-pilot-report.md](../template/reporting/sdd-pilot-report.md) — extend per sections below when the feature is Large/Complex |

Historical exception: Paciente consolidated report may also appear under `Documentation/AI-Harness/research/`; new pilots should prefer the feature SDD `reports/` folder.

---

## Versioning Model

Pilot reports use **semantic phase versions**, not git tags.

### Recommended version triggers

| Version | Typical phase covered | Minimum new content |
|---------|----------------------|---------------------|
| **v0.1** | Research (+ Research Delta if applicable) | Readiness decision; open questions; pre-implementation risks; preliminary GOV/WF |
| **v0.2** | Specify + Design + Tasks + SDD Pre-Execution Review | Contract resolutions; checklist closure; Execute readiness |
| **v0.3–v0.5** | Execute milestones or SDD Review snapshots | Implementation friction; boundary assessment; interim findings |
| **v0.6+ (example)** | Execute + Verify | Gate results; REQ traceability; **Proven** findings; completion decision reference |
| **v1.0 (optional)** | Full lifecycle consolidated | Final GOV/WF registry; maturity scores; feeds governance improvement plan |

Exact minor numbers are flexible; **phase coverage and supersession matter more than the number**.

### Supersession rule (mandatory)

When a major phase completes — especially **Verify** or **Documentation Follow-Up + Reporting** — publish a new version that:

1. States **`Supersedes:`** the prior version explicitly.
2. Updates **Report Lineage** with what the new version adds.
3. Marks prior versions as historical in lineage (do not delete them).

Do not leave two reports claiming current pilot status for the same feature.

**Example (Atendimento Minimal):** v0.5 Execute-only → superseded by v0.6 Execute + Verify.

### Research Delta

When prerequisite SDDs or PM sequencing change after initial Research, document reconciliation as:

- **Part N — Research Delta** in `research.md` (feature truth), **and**
- A pilot report increment (typically v0.1 or v0.2) capturing workflow finding **WF-*** / **GOV-*** about the delta process itself.

See [Prontuario research Part 3](../../SDD/prontuario-sql-stabilization/research.md) and [sdd-pilot-report-v0.1.md](../../SDD/prontuario-sql-stabilization/reports/sdd-pilot-report-v0.1.md).

---

## Lifecycle Timing

Unlike feature report (Reporting Strategy: after Documentation Follow-Up), pilot reports **start early** and **accumulate**.

| SDD phase | Pilot report activity |
|-----------|------------------------|
| Research / Research Delta | **v0.1** — readiness, attention register, open questions, preliminary findings |
| Specify / Design / Tasks | Update or **v0.2** — scope stability, prerequisite checklist, design risks |
| SDD Pre-Execution Review | Record approval / follow-ups; friction on ambiguities |
| Execute | Increment — boundary violations, harness fixes mid-session, TASK handoff gaps |
| Verify | Increment — gate summary **by reference** to `verification.md`; promote findings to **Proven** when evidenced |
| Documentation Follow-Up | Note doc drift themes; do not duplicate Follow-Up checklist |
| Reporting | Cross-link `feature-report.md` lessons; defer duplicate prose |
| Teacher Guide | Optional maturity note on knowledge transfer |
| Post-Reporting | Final consolidation optional; hand off to governance improvement plan |

Pilot reports may reference `verification.md` but must **not** re-decide completion or residual risk acceptance.

---

## Required Structure

Use the template as a floor. For Large/Complex calibration pilots, include these sections (add empty placeholders in v0.1 if not yet applicable).

### Core (every version)

1. **Report metadata** — feature, pilot type, phase covered, status, supersedes, sources.
2. **Executive summary** — workflow outcome, friction themes, readiness / completion pointer, next phase.
3. **Pilot timeline** — one row per lifecycle phase; honest **Not started** rows are required in early versions.
4. **Report lineage** — version table + **next report trigger**.

### Workflow evaluation (from v0.1)

5. **Workflow performance evaluation** — phase rating table (Strong / Adequate / Weak / Not executed / N/A) with evidence link and one improvement candidate per executed phase.
6. **Phase ownership assessment** — did any phase do another phase’s job? defer blockers?

### Findings registry (grow over versions)

7. **Governance findings (GOV-*)** — impact, recommendation, **confidence**.
8. **Workflow findings (WF-*)** — same.
9. **Skill findings (SK-*)** — when skills were used or should have been.
10. **Template findings (TPL-*)** — specify, design, tasks, verification, pilot-report itself.
11. **Documentation findings (DOC-*)** — State, PM, migration-sql, runbook routing gaps.

### Calibration synthesis

12. **What worked — keep unchanged** — practices to preserve for the next SDD.
13. **Points of attention / what failed** — risks before Execute or recurring friction after.
14. **Prior pilot deferred items** — table mapping prior GOV/WF IDs to this pilot (confirmed / resolved / still deferred).
15. **Template and harness improvement candidates** — backlog hints (not adopted policy).
16. **Residual risks** — feature or workflow; Open / Accepted / Mitigated.
17. **Open questions tracker** — resolve in Specify/Design/Verify; close in later report versions.

### Late-phase additions (Verify+)

18. **Requirement / gate summary** — **link** to `verification.md`; short table only.
19. **Maturity assessment** — 1–5 scores per dimension; compare to prior pilot when useful.

Optional: **Execute boundary assessment**, **Review sensor summary**, **Appendix evidence index**.

---

## Finding ID Conventions

Continue numbering across pilots when the finding is the **same theme**. New themes get new IDs.

| Prefix | Domain | Example |
|--------|--------|---------|
| **GOV-** | Harness governance, authority, prerequisites, phase rules | GOV-24 Research Delta formalization |
| **WF-** | SDD lifecycle sequencing, handoffs, phase ownership | WF-12 Research Delta trigger |
| **SK-** | Skill behavior or gap | SK-12 read State before Research Delta |
| **TPL-** | Template section missing or confusing | TPL-07 tasks.md TASK-008 handoff |
| **DOC-** | Documentation routing or drift | DOC-03 State.md stale after Verify |

Reference prior pilot reports for the next ID block (e.g., after Atendimento GOV-23, Prontuario starts GOV-24).

---

## Confidence Levels

Align pilot report confidence with [CONTRIBUTING-AI.md](../CONTRIBUTING-AI.md) § Finding Maturity And Promotion (META-03).

| Level in pilot report | Meaning | Use in report |
|----------------------|---------|---------------|
| **Proven** | Observed in this feature with clear evidence (often post-Verify) | Eligible for Wave 1 improvement plan after review |
| **Preliminary** | Likely valid; needs Execute/Verify or second pilot | v0.1 Research findings default here |
| **Deferred** | Hypothesis; insufficient evidence | Wave 3; revisit on explicit trigger |

**Rule:** Do not mark findings **Proven** in Research-only reports without implementation evidence.

Promotion **Pilot-Proven → Adopted** happens only through governance improvement plan review — not by editing the pilot report alone.

---

## Cross-Pilot Workflow Evaluation

Over time, pilot reports enable **horizontal** harness assessment. When writing or reviewing a report:

### Compare phases across pilots

Use the **Workflow performance evaluation** table consistently. Reviewers can ask:

- Is Research consistently Strong but Verify Weak? → verification or prerequisite policy gap.
- Is Pre-Execution Review skipped on forward SDDs? → sizing or governance breach.
- Does Execute repeatedly close TASK-008 in Verify? → tasks template or ownership gap (GOV-21 pattern).

### Track deferred items

Each report should include **Prior pilot deferred items — applicability** (see Prontuario v0.1). Governance improvement plan consolidates resolution across features.

### Maturity dimensions (recommended scores)

| Dimension | Question |
|-----------|----------|
| SDD workflow | Did phases run in order with clear exits? |
| Verification | Were gates evidenced and handoffs sufficient? |
| Documentation follow-up | Was operational truth synchronized? |
| Reporting & handoff | Was continuity preserved without authority duplication? |
| Knowledge transfer | Was Teacher Guide warranted and timed correctly? |

Score **1–5** with one-sentence justification. Early versions may score Research only (others N/A).

### Consolidation for governance

After Verify and Reporting, extract **Proven** findings into:

`Documentation/SDD/<feature-slug>/reports/governance-improvement-plan.md`

using [governance-improvement-plan.md](../template/governance/governance-improvement-plan.md).

Pilot report = **evidence corpus**; improvement plan = **execution backlog**; Harness Design updates = **adopted policy**.

---

## Anti-Patterns

| Anti-pattern | Why it hurts | Correct behavior |
|--------------|--------------|------------------|
| Pilot report replaces `verification.md` | Duplicates authority; wrong completion decision | Link to verification; summarize gates briefly |
| Pilot report updates `State.md` | Operational truth must go through Documentation Follow-Up | Route doc updates via Documentation Update / Follow-Up |
| Single report at end of feature only | Loses phase friction; weak calibration signal | Increment versions from Research or first SDD phase |
| Two current versions without supersession | Agents read stale Execute snapshot | Explicit **Supersedes** + lineage |
| **Proven** findings in v0.1 Research | Over-promotes hypotheses | Use **Preliminary** until evidenced |
| Copy feature-report scope tables | Redundant with reporting-strategy | Link to SDD / feature report |
| Pilot report changes SDD scope | Feature truth belongs in SDD | Record scope creep as WF finding |
| Chat-only calibration lessons | Not durable for third pilot | Write finding ID in pilot report |

---

## Reference Instances

Use these as calibration references when authoring new reports — patterns, not mandatory length.

| Feature | Report | Contribution |
|---------|--------|--------------|
| Paciente SQL Stabilization | [sdd-pilot-report-v1.0.md](../../SDD/paciente-sql-stabilization/reports/sdd-pilot-report-v1.0.md) | Full lifecycle consolidated; GOV-01–12; WF-01–11; first Pre-Execution Review emergence |
| Atendimento Minimal | [sdd-pilot-report-v0.6.md](../../SDD/atendimento-minimal-sql-stabilization/reports/sdd-pilot-report-v0.6.md) | Forward SDD #2; phased v0.3→v0.6; supersession; GOV-13–23; TASK-008 / credential friction |
| Prontuario | [sdd-pilot-report-v0.1.md](../../SDD/prontuario-sql-stabilization/reports/sdd-pilot-report-v0.1.md) | Research + Research Delta entry; attention register; open questions tracker; GOV-24+ |

---

## Agent And Reviewer Checklist

Before publishing a new pilot report version:

- [ ] **Phase covered** field matches actual content (not aspirational).
- [ ] **Supersedes** set when replacing prior current report.
- [ ] Timeline rows marked **Not started** where applicable.
- [ ] Findings have IDs, confidence, and evidence source (artifact path or section).
- [ ] No conflict with `verification.md` completion decision.
- [ ] Boundaries vs Reporting Strategy respected (no feature-report duplication).
- [ ] **Next report trigger** stated in lineage.
- [ ] English prose; no PHI, credentials, or secrets in examples.

Before extracting governance changes:

- [ ] Only **Proven** findings enter Wave 1 improvement plan (unless review override documented).
- [ ] Improvement plan links to pilot report sections, not chat history.

---

## Related Artifacts

| Artifact | Role |
|----------|------|
| [template/reporting/sdd-pilot-report.md](../template/reporting/sdd-pilot-report.md) | Minimum section scaffold |
| [reporting-strategy.md](reporting-strategy.md) | Session handoff + feature report boundaries |
| [sdd-operational.md](sdd-operational.md) | SDD lifecycle phases and sizing |
| [verification-governance.md](verification-governance.md) | Verifier authority |
| [template/governance/governance-improvement-plan.md](../template/governance/governance-improvement-plan.md) | Post-pilot harness execution plan |
| [CONTRIBUTING-AI.md](../CONTRIBUTING-AI.md) | Harness Calibration Workflow |
| [harness-architecture.md](harness-architecture.md) | META-02 Pilot Report Role |

---

*SDD Pilot Report Governance — workflow calibration through phased retrospectives. Feature continuity remains under Reporting Strategy.*
