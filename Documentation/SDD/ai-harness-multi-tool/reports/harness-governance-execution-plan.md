# Harness Governance Execution Plan (HGEP)

> **Feature SDD:** `Documentation/SDD/ai-harness-multi-tool/`
> **Evidence base:** [reporting.md](../reporting.md), [Pilot_Review_Report_v3.md](../Pilot_Review_Report_v3.md), [Pilot_Review_Report_v4 .md](../Pilot_Review_Report_v4%20.md), [verification.md](../verification.md)
> **Related calibration:** [Prontuario governance improvement plan](../../prontuario-sql-stabilization/reports/governance-improvement-plan.md)
> **Plan version:** v2.0
> **Date:** 2026-07-10
> **Status:** Ready for execution

---

## 1. Executive Summary

This document is the authoritative execution plan for governance changes validated by the AI Harness Multi-Tool Evaluation and the first real multi-tool Execute session (Prontuario SQL Stabilization).

### What This Document Does

It executes proven governance improvements through deterministic, evidence-backed changes to existing Harness artifacts.

### What This Document Does Not Do

This plan introduces **no new ADRs**, **no new Rules**, **no new Skills**, and **no new Templates**. Every change is an update to an existing asset supported by evidence. The purpose is consolidation, not expansion.

### Current State

Significant governance work is already complete:

- **Context Acquisition Governance** is implemented in `sdd-operational.md` § Context Acquisition Governance (lines 365–520).
- **ADR-003** is revised (2026-07-08) for multi-tool taxonomy.
- **ADR-007** is accepted (2026-07-08), establishing the three-layer ownership taxonomy.
- **Cline projections** (6 rules, 7 skills) are created and verified.
- **Governance documentation** is generalized for multi-tool.

### Remaining Work

The remaining work is **propagation** (Context Acquisition Governance → rules, templates, CONTRIBUTING-AI), **evolution** (Token Economy rule), **completion** (PHI typo fix, runtime smoke checks), and **integration** (Prontuario governance improvement items GIB-019 through GIB-026).

---

## 2. Governance Admission Criteria

### Eligibility

A governance improvement is eligible for execution in this HGEP only when at least one of the following is true:

- Supported by repeated evidence across multiple sessions or features
- Resolves a high-impact execution problem documented in a pilot review
- Resolves an architectural inconsistency identified during verification
- Resolves an objective documentation defect (e.g., broken path, typo)
- Validated through multiple implementations

### Ineligibility

The following are **not eligible** for execution:

- Hypotheses
- Personal preferences
- Speculative optimizations
- Single-session observations without corroboration
- Convenience improvements
- Framework expansion

### Role Separation

Pilot Review Reports accumulate observations. Only the HGEP executes proven findings. Observations that lack sufficient evidence are recorded in § Future Validation Opportunities but receive no backlog items, no IDs, and no execution status.

---

## 3. Architecture Preservation

### Preserved Without Changes

| Artifact | Authority |
|----------|-----------|
| ADR-007 (Harness Asset / Tool Asset / Shared Asset taxonomy) | Permanent architectural boundary |
| ADR-003 (Documentation taxonomy, revised) | Canonical documentation home |
| Authority hierarchy (ADRs > Architecture docs > State > SDD > Rules > Skills) | Tool-agnostic, unchanged |
| SDD lifecycle (Research → Specify → Design → Tasks → Pre-Execution Review → Execute → Verify → Documentation Follow-Up → Reporting → Teacher Guide) | Tool-agnostic, unchanged |
| Three-layer ownership model | Harness defines what; tools provide how |
| Context Acquisition Governance in `sdd-operational.md` | Already comprehensive — no new governance concepts needed |
| Calibration workflow | Documented in `CONTRIBUTING-AI.md`, unchanged |

### No New ADRs Required

The evidence does not justify new ADRs. Context Acquisition Governance is operational, not architectural. Token Economy evolution is a rule update. Implementation batching is recommended practice.

### No New Rules Required

No new rules are created. The Token Economy rule is evolved (additive update to existing content). The `security-phi` rule receives a typo fix. No `context-acquisition` or `execution-batching` rule is created — those concepts are governed in `sdd-operational.md` and referenced by the evolved Token Economy rule.

### No New Skills Required

No new skills are created. The `verifier` skill receives an additive update. All other skills remain unchanged.

### No New Templates Required

No new templates are created. Four existing templates (`specify.md`, `design.md`, `tasks.md`, `verification.md`) receive minor additive updates.

---

## 4. Execution Backlog

### Backlog Structure

Items are ordered by architectural dependency: Governance → Rules → Templates → Skills → Runtime Validation. Each item specifies exactly what changes, where, and what the deterministic completion condition is. Verification is separated from implementation.

---

### Phase A — Governance Document Updates

#### HGEP-001

| Field | Value |
|-------|-------|
| **Evidence** | Prontuario GIB-019, GOV-24, WF-12; sdd-operational.md § Research (already partially present) |
| **Target** | `Documentation/AI-Harness/Harness-Design/sdd-operational.md` § Research |
| **Action** | Verify that § Research contains a **Research Delta** subsection with: (1) trigger condition (prerequisite SDD reaches Verify after initial Research, or `State.md` sequencing changes), (2) artifact structure (`Part N` in `research.md`), (3) exit criteria (`READY FOR SPECIFY` declared with reconciled scope). If any element is missing, insert it. If all elements are present, mark complete. |
| **Dependencies** | None |
| **Effort** | S |
| **Completion condition** | `sdd-operational.md` § Research contains Research Delta trigger, structure, and exit criteria — verifiable by direct inspection. |
| **Verification** | Documentation review: section exists and contains all three elements. |

#### HGEP-002

| Field | Value |
|-------|-------|
| **Evidence** | Prontuario GIB-021, DOC-01; sdd-operational.md § SDD Pre-Execution Review (already partially present) |
| **Target** | `Documentation/AI-Harness/Harness-Design/sdd-operational.md` § SDD Pre-Execution Review exit criteria |
| **Action** | Verify that exit criteria include a **Cross-Artifact Consistency** check: "specify.md and design.md do not make conflicting authority claims about the same domain decision. If conflicts exist, they are resolved before Execute authorization." If missing, insert. If present, mark complete. |
| **Dependencies** | None |
| **Effort** | S |
| **Completion condition** | Pre-Execution Review exit criteria contain Cross-Artifact Consistency check — verifiable by direct inspection. |
| **Verification** | Documentation review: checklist item exists with Prontuario D-02 cited as exemplar. |

#### HGEP-003

| Field | Value |
|-------|-------|
| **Evidence** | Prontuario GIB-024, GOV-28; sdd-operational.md § SDD Pre-Execution Review (already partially present) |
| **Target** | `Documentation/AI-Harness/Harness-Design/sdd-operational.md` § SDD Pre-Execution Review |
| **Action** | Verify that a **durability note** is present: "Pre-Execution Review is a durable phase boundary. Multi-session gaps and intervening SDDs do not invalidate it — re-run Credential Probe and baseline tests on resume." If missing, insert. If present, mark complete. |
| **Dependencies** | None |
| **Effort** | S |
| **Completion condition** | Durability note present in Pre-Execution Review section — verifiable by direct inspection. |
| **Verification** | Documentation review: note exists with Prontuario 19-day gap cited as evidence. |

#### HGEP-004

| Field | Value |
|-------|-------|
| **Evidence** | PRR-v3 § Immediate; PRR-v4 §PRR-001 |
| **Target** | `Documentation/AI-Harness/CONTRIBUTING-AI.md` § Execute |
| **Action** | Append the following text to the end of the § Execute section, after the existing content: "During Execute, follow Context Acquisition Governance: acquire context only for the current task, expand incrementally when blocked, stop when the required artifact is identified. For Large features, execute in small implementation batches. See `sdd-operational.md` § Context Acquisition Governance and § Implementation Batching Guidance." Do not modify existing Execute section content. |
| **Dependencies** | None |
| **Effort** | S |
| **Completion condition** | `CONTRIBUTING-AI.md` § Execute contains a reference to Context Acquisition Governance and implementation batching — verifiable by direct inspection. |
| **Verification** | Documentation review: reference exists and points to `sdd-operational.md` sections. |

---

### Phase B — Rule Updates

#### HGEP-005

| Field | Value |
|-------|-------|
| **Evidence** | verification.md § Typo Finding; reporting.md §5.2 (PHI-TYPO-01) |
| **Target** | `.cursor/rules/security-phi.mdc` |
| **Action** | Replace `Documantation` with `Documentation` in the path reference within the "Additional Context" section (line 16). No other content changes. |
| **Dependencies** | None |
| **Effort** | S |
| **Completion condition** | `security-phi.mdc` contains `Documentation` (not `Documantation`) in all path references — verifiable by string search. |
| **Verification** | Content comparison: no occurrence of `Documantation` in `.cursor/rules/security-phi.mdc`. |

#### HGEP-006

| Field | Value |
|-------|-------|
| **Evidence** | HGEP-005 (re-sync after Cursor source fix) |
| **Target** | `.clinerules/security-phi.md` |
| **Action** | Update Cline projection to match Cursor source after typo fix. The Cline projection already uses correct spelling `Documentation` — verify it matches the corrected Cursor source exactly in all other content. |
| **Dependencies** | HGEP-005 |
| **Effort** | S |
| **Completion condition** | Cline `security-phi.md` content matches Cursor `security-phi.mdc` content (excluding frontmatter format differences) — verifiable by content comparison. |
| **Verification** | Content comparison between Cursor and Cline projections. |

#### HGEP-007

| Field | Value |
|-------|-------|
| **Evidence** | PRR-v4 §PRR-001; PRR-v3 § Proposed Evolution |
| **Target** | `.cursor/rules/token-economy.mdc` and `.clinerules/token-economy.md` |
| **Action** | Append 3 new bullets after the existing 6 bullets in both projections. The exact content to insert after the last existing bullet (`Prefer focused skills...`):<br><br>`- Acquire context progressively — start with minimum required context, expand only when blocked, stop when the required artifact is identified. See \`sdd-operational.md\` § Context Acquisition Governance.`<br><br>`- During Execute, acquire context per-task, not per-feature. Do not preload context for future tasks.`<br><br>`- For large features, execute in small implementation batches — implement, validate, commit, continue. See \`sdd-operational.md\` § Implementation Batching Guidance.`<br><br>Do not modify or remove any existing bullet. Both projections must contain identical content (excluding frontmatter). |
| **Dependencies** | None |
| **Effort** | S |
| **Completion condition** | Both Cursor and Cline Token Economy projections contain exactly 9 bullets (6 existing + 3 new) with identical text — verifiable by content comparison. |
| **Verification** | Content comparison: both projections contain the 3 new bullets; no existing bullet modified or removed. |

---

### Phase C — Template Updates

#### HGEP-008

| Field | Value |
|-------|-------|
| **Evidence** | PRR-v3 § Execution Batches; sdd-operational.md § Implementation Batching Guidance |
| **Target** | `Documentation/AI-Harness/template/sdd/tasks.md` |
| **Action** | Insert an **Execution Batching** note in the template body, after the task list section and before verification expectations. The note should state: "For Large features, execute in small implementation batches — implement a coherent subset of tasks, validate (build, test, smoke), commit, then continue. See `sdd-operational.md` § Implementation Batching Guidance and § Context Acquisition Governance for Execute entry context requirements." |
| **Dependencies** | None |
| **Effort** | S |
| **Completion condition** | `tasks.md` template contains Execution Batching note referencing `sdd-operational.md` — verifiable by direct inspection. |
| **Verification** | Documentation review: note exists and references sdd-operational.md sections. |

#### HGEP-009

| Field | Value |
|-------|-------|
| **Evidence** | Prontuario GIB-020, WF-16 |
| **Target** | `Documentation/AI-Harness/template/sdd/tasks.md` and `Documentation/AI-Harness/template/verification/verification.md` |
| **Action** | In `tasks.md` template: add to the TASK-009 pattern description (or equivalent runtime validation task pattern) the requirement: "Record session notes with runtime validation results for Verify handoff." In `verification.md` template: add to Runtime Validation entry criteria: "TASK-009 (or equivalent) session notes provided by Execute." |
| **Dependencies** | None |
| **Effort** | S |
| **Completion condition** | `tasks.md` template includes session notes requirement in runtime validation task pattern; `verification.md` template includes session notes in Runtime Validation entry criteria — verifiable by direct inspection. |
| **Verification** | Documentation review: both templates contain the session notes requirement. |

#### HGEP-010

| Field | Value |
|-------|-------|
| **Evidence** | PRR-v4 §PRR-001 (transversal concern) |
| **Target** | `Documentation/AI-Harness/template/sdd/specify.md` |
| **Action** | Insert a **Context Budget** note in the template body: "Context budget for Specify is Medium. Load product intent, domain rules, and scope boundaries — not implementation details. See `sdd-operational.md` § Context Acquisition Governance." |
| **Dependencies** | None |
| **Effort** | S |
| **Completion condition** | `specify.md` template contains Context Budget note referencing `sdd-operational.md` — verifiable by direct inspection. |
| **Verification** | Documentation review: note exists and references sdd-operational.md. |

#### HGEP-011

| Field | Value |
|-------|-------|
| **Evidence** | PRR-v4 §PRR-001 (transversal concern); Prontuario GIB-026, WF-14 |
| **Target** | `Documentation/AI-Harness/template/sdd/design.md` |
| **Action** | Insert a **Context Budget** note: "Context budget for Design is Medium. Load architecture, ADRs, and integration points — not full codebase. See `sdd-operational.md` § Context Acquisition Governance." Additionally, insert an optional **Fixture Chain** subsection placeholder: "### Fixture Chain (optional — for composite aggregates)\n\nList prerequisite entities and ordered creation steps for integration tests." |
| **Dependencies** | None |
| **Effort** | S |
| **Completion condition** | `design.md` template contains Context Budget note and optional Fixture Chain subsection — verifiable by direct inspection. |
| **Verification** | Documentation review: both elements exist and reference sdd-operational.md where applicable. |

#### HGEP-012

| Field | Value |
|-------|-------|
| **Evidence** | Prontuario GIB-022, GOV-25 |
| **Target** | `Documentation/AI-Harness/template/sdd/research.md` (or SDD README if no research template exists) |
| **Action** | Insert a **supersession rule** in the template header: "Multi-part research: Read Part N first — it governs operational status. Earlier parts govern domain decisions unless explicitly superseded." |
| **Dependencies** | None |
| **Effort** | S |
| **Completion condition** | Research template (or SDD README) header contains supersession rule — verifiable by direct inspection. |
| **Verification** | Documentation review: supersession note exists. |

#### HGEP-013

| Field | Value |
|-------|-------|
| **Evidence** | Prontuario GIB-023, GOV-26 |
| **Target** | `Documentation/AI-Harness/template/sdd/specify.md` |
| **Action** | Insert a **prerequisite verification checklist item** in the Prerequisites section: "Distinguish **prerequisite verified** (consume contracts only — do not re-implement upstream REQs) from **in-scope upstream REQ** (implement within this SDD)." |
| **Dependencies** | HGEP-010 (same target file — apply together) |
| **Effort** | S |
| **Completion condition** | `specify.md` template Prerequisites section includes verified-prerequisite vs in-scope-REQ distinction — verifiable by direct inspection. |
| **Verification** | Documentation review: checklist item exists. |

#### HGEP-014

| Field | Value |
|-------|-------|
| **Evidence** | Prontuario GIB-025, WF-13 |
| **Target** | `Documentation/AI-Harness/template/sdd/specify.md` |
| **Action** | Insert an **inheritance instruction** in the Execution Prerequisites section: "Inherit Execution Prerequisites from prior verified SDDs by link. Do not rewrite runbook steps or credential instructions already documented in prior SDDs or `Documentation/Technical/runbook.md`." |
| **Dependencies** | HGEP-010, HGEP-013 (same target file — apply together) |
| **Effort** | S |
| **Completion condition** | `specify.md` template Execution Prerequisites section includes inheritance instruction — verifiable by direct inspection. |
| **Verification** | Documentation review: instruction exists. |

---

### Phase D — Skill Updates

#### HGEP-015

| Field | Value |
|-------|-------|
| **Evidence** | PRR-v3 § Expected Benefits; PRR-v4 §PRR-001 |
| **Target** | `verifier` skill — Cursor (`.cursor/skills/verifier/SKILL.md`) and Cline (`.cline/skills/verifier/SKILL.md`) |
| **Action** | Insert a **Context Acquisition Governance Observation** section in the verifier skill body. The section should contain three observation prompts (not gates): (1) "Did Execute follow progressive context expansion (incremental, not preemptive)?" (2) "Were stop conditions satisfied before implementation began?" (3) "Was context acquisition task-oriented rather than feature-oriented?" State explicitly: "These are observations for the feature report and governance improvement plan, not verification gates. They do not invalidate implementation." Both Cursor and Cline copies must receive identical content. |
| **Dependencies** | None |
| **Effort** | M |
| **Completion condition** | Both Cursor and Cline `verifier` SKILL.md files contain the Context Acquisition Governance Observation section with 3 observation prompts — verifiable by content comparison. |
| **Verification** | Skill review: section exists in both projections with identical content. |

---

### Phase E — Runtime Validation

#### HGEP-016

| Field | Value |
|-------|-------|
| **Evidence** | reporting.md §5.4; verification.md § Runtime Verification |
| **Target** | Runtime environment — Cline |
| **Action** | Execute a manual Cline session that: (1) loads rules from `.clinerules/`, (2) invokes a skill from `.cline/skills/`, (3) auto-loads `AGENTS.md`, (4) respects `.clineignore` patterns. Record observations. |
| **Dependencies** | All Phase A–D items complete (rules and skills must be in final state before validation) |
| **Effort** | M |
| **Completion condition** | Cline session confirms: rules load, skills invoke, AGENTS.md bootloads, .clineignore respected — verifiable by manual session observation recorded in session notes. |
| **Verification** | Manual Cline session: each of the 4 checks passes or documented issue is raised. |

#### HGEP-017

| Field | Value |
|-------|-------|
| **Evidence** | reporting.md §5.4; verification.md § Runtime Verification |
| **Target** | Runtime environment — Cursor |
| **Action** | Execute a manual Cursor session that invokes a skill with updated cross-skill name references (e.g., `verifier` or `documentation-update`). Confirm no invocation errors. |
| **Dependencies** | All Phase A–D items complete |
| **Effort** | S |
| **Completion condition** | Cursor session confirms skills with updated name references invoke without error — verifiable by manual session observation. |
| **Verification** | Manual Cursor session: skill invocation succeeds. |

#### HGEP-018

| Field | Value |
|-------|-------|
| **Evidence** | reporting.md §8.1 |
| **Target** | `.clineignore` |
| **Action** | After HGEP-016 Cline session, evaluate whether `.clineignore` patterns effectively implement the "avoid loading by default" strategy. If patterns are too aggressive (blocking needed files) or too permissive (allowing unnecessary loading), adjust patterns. If patterns are effective, mark complete. |
| **Dependencies** | HGEP-016 |
| **Effort** | S |
| **Completion condition** | `.clineignore` patterns reviewed and confirmed effective or adjusted — verifiable by Cline session observation. |
| **Verification** | Cline session observation: excluded files are not loaded; included files are accessible. |

---

## 5. Implementation Order

Execute items in this order. Items within the same phase may be executed in parallel unless a dependency is declared.

```
Phase A — Governance Document Updates (HGEP-001, HGEP-002, HGEP-003, HGEP-004)
    ↓
Phase B — Rule Updates (HGEP-005, HGEP-006, HGEP-007)
    ↓
Phase C — Template Updates (HGEP-008, HGEP-009, HGEP-010, HGEP-011, HGEP-012, HGEP-013, HGEP-014)
    ↓
Phase D — Skill Updates (HGEP-015)
    ↓
Phase E — Runtime Validation (HGEP-016, HGEP-017, HGEP-018)
```

### Ordering Rationale

1. **Governance documents first** — `sdd-operational.md` and `CONTRIBUTING-AI.md` are authorities referenced by all downstream artifacts. Any missing governance content must be verified before rules and templates reference it.
2. **Rules second** — Rules are persistent guardrails loaded before templates. The Token Economy evolution references `sdd-operational.md`; the PHI typo fix is independent.
3. **Templates third** — Templates reference governance sections and rules. They must be updated after governance and rules are in final state.
4. **Skills fourth** — The verifier skill update references Context Acquisition Governance. It must be updated after governance is verified.
5. **Runtime validation last** — Runtime smoke checks validate the final state of all rules and skills. They must execute after all content changes are complete.

---

## 6. Definition of Done

### Per-Item Completion

Each HGEP item is complete when its **Completion condition** is satisfied. Completion conditions are objectively verifiable — they require no subjective judgment.

### Overall Completion

The governance update is complete when:

1. All 18 HGEP items are marked complete.
2. No item is deferred or skipped without documented rationale.
3. Consistency audit (§7) passes.
4. `Documentation/State.md` notes governance update completion.

### Harness Readiness for Next Feature (Agendamento)

The Harness is ready for Agendamento when **Phase A through Phase C** are complete. Phase D (verifier skill) and Phase E (runtime validation) may run in parallel with Agendamento research/specify phases, but must complete before Agendamento Execute.

### Feature Validation (After Agendamento Execution)

Feature validation is **not part of this HGEP**. It is the responsibility of the Agendamento Pilot Report, which will determine whether the governance changes produced the expected execution behavior. The flow is:

```
HGEP Implementation → Consistency Audit → Next Feature (Agendamento) → Pilot Report → Future HGEP (if warranted)
```

---

## 7. Consistency Audit Checklist

Run after all items are complete.

- [ ] `sdd-operational.md` authority hierarchy unchanged
- [ ] All template references to `sdd-operational.md` sections resolve (no dangling section names)
- [ ] Token Economy rule content is identical between Cursor (`.cursor/rules/token-economy.mdc`) and Cline (`.clinerules/token-economy.md`) projections
- [ ] Security-PHI rule content is identical between Cursor and Cline projections (after typo fix)
- [ ] Verifier skill content is identical between Cursor and Cline projections
- [ ] Verifier skill and verification template aligned on TASK-009 session notes policy
- [ ] CONTRIBUTING-AI Execute section references Context Acquisition Governance
- [ ] No new ADRs, Rules, Skills, or Templates were created
- [ ] `AGENTS.md` unchanged
- [ ] `documentation-index.md` unchanged (no new templates or harness docs added)
- [ ] No secrets, credentials, or PHI introduced
- [ ] `Documentation/State.md` notes governance update completion

---

## 8. Risk Assessment

### Migration Risks

| Risk | Probability | Impact | Mitigation |
|------|-------------|--------|------------|
| Token Economy rule evolution breaks existing agent behavior | Low | Low | Additive only — 6 existing bullets preserved, none removed |
| Template updates confuse agents familiar with prior format | Low | Low | Additive only — new notes/subsections, no restructuring |
| CONTRIBUTING-AI update creates conflicting guidance | Low | Medium | References `sdd-operational.md` as authority; no conflicting content introduced |

### Compatibility Risks

| Risk | Probability | Impact | Mitigation |
|------|-------------|--------|------------|
| Cline projection content drift after rule updates | Medium | Low | Update both projections simultaneously; content comparison verification |
| Template updates affect historical SDD instances | Low | None | Historical SDD instances are not rewritten; templates are forward-looking |
| Verifier skill update changes verification behavior | Low | Low | Context Acquisition Governance check is an observation, not a gate |

### Workflow Risks

| Risk | Probability | Impact | Mitigation |
|------|-------------|--------|------------|
| Agents ignore Context Acquisition Governance during Execute | Medium | High | Token Economy rule provides concise guardrail; CONTRIBUTING-AI provides workflow guidance; verifier observation provides feedback loop |
| Execution batching guidance adds overhead for Small features | Low | Low | Batching is recommended for Large features only; Small features exempt per adaptive sizing |

### Multi-Tool Risks

| Risk | Probability | Impact | Mitigation |
|------|-------------|--------|------------|
| Cline runtime behavior differs from documented conventions | Medium | Medium | HGEP-016 validates behavior in Cline session |
| Cursor skill content changes affect invocation | Low | Low | HGEP-017 validates behavior in Cursor session |
| `.clineignore` patterns ineffective | Low | Low | HGEP-018 reviews effectiveness after Cline session |

### Token Consumption Risks

| Risk | Probability | Impact | Mitigation |
|------|-------------|--------|------------|
| Context Acquisition Governance insufficient to prevent context exhaustion | Medium | High | Phase B–C propagation is the first defense; verifier observation (Phase D) provides feedback; future HGEP may add escalation if evidence warrants |
| Token Economy rule exceeds effective guardrail length | Low | Low | 9 bullets total — within concise guardrail scope |

---

## 9. Recommended Posture for Next Forward SDD (Agendamento)

### Apply Before Starting

1. **HGEP-004:** CONTRIBUTING-AI Execute section references Context Acquisition Governance.
2. **HGEP-007:** Token Economy rule evolved — Agendamento Execute will have Context Acquisition Governance guardrail.
3. **HGEP-008:** Tasks template includes execution batching guidance.
4. **HGEP-009:** TASK-009 in Agendamento tasks.md must include "Record session notes" requirement.
5. **HGEP-001:** Research Delta is formalized — Agendamento research can use it if prerequisites complete mid-discovery.
6. **HGEP-002:** Agendamento Pre-Execution Review must include Cross-Artifact Consistency check.

### Keep Unchanged

- Three-layer ownership taxonomy (ADR-007)
- Authority hierarchy
- SDD lifecycle phases
- Context Acquisition Governance in `sdd-operational.md`
- TASK/VP/DF classification
- Credential Probe in Execution Prerequisites
- Repository-direct SQL integration test pattern
- SDD Pre-Execution Review as mandatory gate for Large features
- PHI-safe controller pattern
- Soft delete per ADR-001

---

## 10. Future Validation Opportunities

This section records observations from Pilot Review Reports that lack sufficient evidence for execution. These items are **explicitly out of scope**. They contain no backlog items, no IDs, no priorities, and no execution status. They are recorded solely to preserve observations for future evidence accumulation.

- **Context Escalation models** — PRR-v4 recommends evaluating "repository exploration escalation models." Current Progressive Context Expansion and Stop Conditions are sufficient. Future evidence: a Large/Complex feature where context exhaustion recurs despite Phase B–C propagation.
- **Runtime Context Expansion protocols** — PRR-v4 recommends evaluating "progressive context expansion strategies" at the runtime level. Runtime protocols are Tool Assets, not Harness governance. Future evidence: third AI tool evaluation or Cline capability research update.
- **Physical canonical representation** — reporting.md §8.2 notes this may be warranted if the number of supported tools exceeds three or synchronization overhead becomes measurable. Future evidence: third AI tool adoption or measurable maintenance burden.

These observations will be evaluated through the standard SDD process (Research → Specify → Design) if future evidence accumulates. They are not pre-approved for adoption.

---

## Appendix A — Evidence Index

| Artifact | Path | Role |
|----------|------|------|
| Implementation Report | [reporting.md](../reporting.md) | Feature completion; residual risks; deferred items |
| Pilot Review v3 | [Pilot_Review_Report_v3.md](../Pilot_Review_Report_v3.md) | Context Acquisition Governance findings; root cause; proposed evolution |
| Pilot Review v4 | [Pilot_Review_Report_v4 .md](../Pilot_Review_Report_v4%20.md) | Findings PRR-001–004; future research recommendations |
| Verification Report | [verification.md](../verification.md) | Requirements matrix; PHI-TYPO-01; runtime verification gaps |
| Prontuario Governance Plan | [../../prontuario-sql-stabilization/reports/governance-improvement-plan.md](../../prontuario-sql-stabilization/reports/governance-improvement-plan.md) | GIB-019–026; proven findings from third aggregate |
| SDD Operational Governance | `Documentation/AI-Harness/Harness-Design/sdd-operational.md` | Context Acquisition Governance (already implemented) |
| Harness Architecture | `Documentation/AI-Harness/Harness-Design/harness-architecture.md` | Three-layer taxonomy; context loading policy |
| ADR-003 | `Documentation/Architecture/ADR/ADR-003-documentation-taxonomy.md` | Already revised |
| ADR-007 | `Documentation/Architecture/ADR/ADR-007-harness-asset-taxonomy.md` | Already accepted |

---

## Appendix B — Evidence-to-Change Traceability

| Evidence | Finding | HGEP Item |
|----------|---------|-----------|
| PRR-v3 § Root Cause | Execute did not govern context acquisition | HGEP-004, HGEP-007, HGEP-008, HGEP-010, HGEP-011 |
| PRR-v3 § Execution Batches | Large implementations need batching | HGEP-008 |
| PRR-v3 § Legacy Context Policy | Legacy loaded too early | HGEP-007 (rule reinforcement) |
| PRR-v3 § Immediate | Introduce explicit execution guidance | HGEP-004, HGEP-007, HGEP-008 |
| PRR-v3 § Expected Benefits | Verifier should observe CAG compliance | HGEP-015 |
| PRR-v4 § PRR-001 | Missing Context Acquisition Governance | Already in sdd-operational.md; propagated by HGEP-004, 007, 008, 010, 011, 015 |
| PRR-v4 § PRR-002 | Repository-wide exploration before implementation | Addressed by existing CAG; propagated by HGEP-007, 008 |
| PRR-v4 § PRR-003 | Progressive Discovery not governed | Already in sdd-operational.md; propagated by HGEP-007 |
| PRR-v4 § PRR-004 | Legacy access needs validation | Already strengthened; monitor during next feature |
| reporting.md §5.2 | PHI-TYPO-01 typo | HGEP-005, HGEP-006 |
| reporting.md §5.4 | Runtime verification pending | HGEP-016, HGEP-017 |
| reporting.md §8.1 | .clineignore effectiveness | HGEP-018 |
| verification.md § Typo Finding | PHI-TYPO-01 | HGEP-005, HGEP-006 |
| verification.md § Runtime Verification | Runtime gaps | HGEP-016, HGEP-017 |
| Prontuario GIB-019 | Research Delta | HGEP-001 |
| Prontuario GIB-020 | TASK-009 session notes | HGEP-009 |
| Prontuario GIB-021 | Cross-Artifact Consistency | HGEP-002 |
| Prontuario GIB-022 | Research supersession | HGEP-012 |
| Prontuario GIB-023 | Prerequisite checklist | HGEP-013 |
| Prontuario GIB-024 | Pre-Exec Review durability | HGEP-003 |
| Prontuario GIB-025 | Inherit prerequisites | HGEP-014 |
| Prontuario GIB-026 | Fixture Chain | HGEP-011 |

---

## Appendix C — Plan Lineage

| Version | Date | Scope | Status |
|---------|------|-------|--------|
| v1.0 | 2026-07-10 | Initial HGEP | Superseded |
| v2.0 | 2026-07-10 | Consolidation pass: admission criteria, deterministic items, removed experimental scope, separated execution from validation, reordered by dependency | Ready for execution |

**Next version trigger:** After implementation, bump to v2.1 with completed HGEP statuses.

---

*End of Harness Governance Execution Plan*