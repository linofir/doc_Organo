---
name: not-a-teacher
description: Transforms verified implementation knowledge into Teacher Guide learning artifacts per Knowledge Strategy governance. Use when generating or updating teacher-guide.md after Verify and Reporting, when deciding if a feature warrants a Teacher Guide, extracting concepts from production code, or producing knowledge-analysis.md for pedagogical handoff.
---

# Not A Teacher — Doc Organo

## Purpose

This skill operationalizes [knowledge-strategy.md](Documentation/AI-Harness/Harness-Design/knowledge-strategy.md). It is an **implementation interpreter**, not a lecturer.

Transform **verified implementation** into maintainable learning artifacts. Teach what was merged — not what was planned, rejected, or speculative.

**Primary output:** `Documentation/SDD/<feature-slug>/teacher-guide.md`

**Optional output:** `Documentation/SDD/<feature-slug>/knowledge-analysis.md`

## What This Skill Is Not

| Anti-role | Use instead |
|-----------|-------------|
| Requirements or design author | SDD `specify.md`, `design.md` |
| Verification | `.cursor/skills/verifier/SKILL.md`, `verification.md` |
| Project status update | `.cursor/skills/documentation-update/SKILL.md`, `State.md` |
| ADR author | `Documentation/Architecture/ADR/` |
| Feature report | `reports/feature-report.md` |
| Session handoff | `reports/session-handoff.md` |
| Markdown template filler | Read code; interpret implementation |

## Prerequisites (Hard Gates)

Do **not** start until all are true:

1. **Execute** — implementation merged or ready for merge.
2. **Verify** — `verification.md` exists with a completion decision.
3. **Documentation Follow-Up** — State, PM, and relevant technical docs updated.
4. **Reporting** — `feature-report.md` (and session handoff when applicable) exists when reporting policy applies.

If prerequisites fail, stop and report which gate is missing. Do not draft a Teacher Guide from unverified code.

## Start Here

1. Read `Documentation/AI-Harness/Harness-Design/knowledge-strategy.md` — governance authority for this skill.
2. Read `Documentation/State.md` — operational context only; do not restate in the guide.
3. Load the active SDD folder: `Documentation/SDD/<feature-slug>/`.
4. Read recommended inputs when present: `specify.md`, `design.md`, `tasks.md`, `verification.md`, `reports/feature-report.md`, `reports/session-handoff.md`, existing `teacher-guide.md` (for updates).
5. Identify and read **production code** touched by the feature (controllers, entities, repositories, mappings, EF config, DbContext, tests).
6. Use SDD and reports for **intent and evolution context** — implementation code wins on conflict.

## Workflow Overview

```text
Stage 1  Eligibility
Stage 2  Knowledge Extraction
Stage 3  Concept Classification
Stage 4  Architecture Interpretation
Stage 5  Common Pitfalls Discovery
Stage 6  Feature Evolution History
Stage 7  Study Path Generation
Stage 8  Reading Path
Stage 9  Teacher Guide Generation
         → Quality Gate
```

Track progress with the checklist in [quality-gate.md](quality-gate.md).

---

## Stage 1 — Determine Eligibility

Decide whether the feature warrants a Teacher Guide. Apply generation rules from knowledge-strategy.md.

**Generate when any apply:**

- First project instance of a pattern (e.g. first SQL integration test class, first aggregate migration vertical).
- SDD-backed feature crossing **three or more** architectural layers.
- SDD pilot or forward SDD for SQL migration verticals.
- Explicit request for onboarding material for a verified feature.

**May skip when:**

- Bug fix with no new patterns or contract changes.
- Documentation-only or governance-only change.
- Repetitive implementation with no new design decisions (a shortened guide MAY still document new aggregate paths).

**Output:** Eligibility decision + rationale. If skipping with an active SDD, note that Documentation Follow-Up or feature report should record the skip reason.

If not eligible, stop. Optionally produce only `knowledge-analysis.md` with eligibility rationale when the user requested analysis anyway.

---

## Stage 2 — Knowledge Extraction

Extract concepts using the evidence pipeline:

```text
SDD requirements and design → candidates
        ↓
Production code → confirmed usage only
        ↓
Tests and verification → validation anchors
        ↓
De-duplicate → concept inventory
```

**Include a concept only if:**

1. It appears in **production code** or an **accepted ADR** referenced by the feature, AND
2. It contributes to understanding **this feature**, AND
3. It has at least one code location (type/method/file) and a stated business or design purpose.

**Reject:**

- Planned in SDD but not implemented.
- Generic language syntax with no design significance.
- Concepts fully owned by an ADR (link ADR instead of re-teaching).

Target **8–15** major concepts. More than ~20 suggests insufficient filtering.

Record raw inventory in working notes or `knowledge-analysis.md`.

---

## Stage 3 — Concept Classification

Classify each inventory entry:

| Class | Definition | Guide emphasis |
|-------|------------|----------------|
| **Primary** | Core concepts required to understand the feature | Full concept cards; drive Study Guide Beginner tier |
| **Supporting** | Helpful but not central | Concept cards; Intermediate tier |
| **Adjacent** | Present but peripheral | Brief mention or Low priority only |

Examples (illustrative, not feature-specific):

- Primary: Repository pattern, AutoMapper profiles, global query filters.
- Supporting: DTO pattern, owned entities.
- Adjacent: Guid serialization, DateOnly conversion.

Emphasize Primary concepts in architecture interpretation and study paths.

---

## Stage 4 — Architecture Interpretation

Explain **why** the implementation looks the way it does. Do not restate source code.

Cover:

- **Request flow** — typical create/read/update/delete or feature-specific flows through actual layers.
- **Layer responsibilities** — what each layer owns for this slice.
- **Dependency flow** — controller → repository interface → implementation → EF → SQL (adapt to feature).

Use ASCII flow diagrams when they clarify sequence. Name real types and files; avoid large code blocks.

Pilot lesson: reading the controller first hides mapping and domain decisions — interpret bottom-up where applicable.

---

## Stage 5 — Common Pitfalls Discovery

Generate **feature-specific** pitfalls from implementation realities, verification findings, and architectural patterns.

**Minimum: 3 pitfalls.** Each MUST state:

1. **What commonly breaks** — observable symptom or failure mode.
2. **Why it breaks** — underlying mechanism.
3. **How to avoid it** — concrete prevention (file to update, test to run, pattern to follow).

**Must not:** list verification gate outcomes, generic advice, or replace test strategy docs.

Draw from areas that commonly fail: mapping gaps, forgotten repository/domain updates, query filter side effects, PHI regression, contract semantics.

---

## Stage 6 — Feature Evolution History

Capture **why** the implementation diverges from intuitive expectations, earlier drafts, or Legacy behavior.

**Include when applicable:**

- Contract changes (routes, semantics, response shape).
- Architecture refinements (layer responsibility clarifications).
- Scope decisions (deferred frontend, auth, migrations).
- Important tradeoffs (accepted weaknesses with rationale).
- Security remediation (PHI or clinical safety behavior changes).

Use concise before/after or decision/rationale patterns. Sources: SDD design, implementation diff, verification findings — **not chat history alone**.

**Must not:** repeat full Legacy tables from `specify.md` or rewrite ADR rationale.

---

## Stage 7 — Study Path Generation

Produce a **Study Guide** with three tiers. Every topic MUST trace to actual implementation in this feature.

| Tier | Definition |
|------|------------|
| **Beginner** | Required before safely modifying the feature |
| **Intermediate** | Explains why the code is structured as it is |
| **Advanced** | Extension, hardening, cross-feature implications |

For each topic: why it matters, where in this feature, suggested order within tier. External links MAY supplement; they MUST NOT replace internal pointers.

Map Study priority (High/Medium/Low) from concept cards to tiers: High → Beginner; Medium → Intermediate; Low → Advanced.

---

## Stage 8 — Reading Path

Generate **Quick Reference — Code Reading Order**.

**Must include:**

1. **Sequence** — ordered files or components.
2. **Rationale** — why this order.
3. **Dependency progression** — how each step enables the next.

**Default progression** (unless feature constraints justify another order):

```text
Domain entity (behavior)
  → Application mapping (DTO ↔ entity)
    → Repository interface + implementation
      → API controller
        → EF configuration (Fluent API)
          → DbContext (cross-cutting concerns)
            → Unit tests (behavior proof)
              → Integration tests (SQL proof)
```

**Must not:** paste large code blocks or replace reading the codebase.

---

## Stage 9 — Teacher Guide Generation

Write `Documentation/SDD/<feature-slug>/teacher-guide.md` in **English**.

Use section obligations from [teacher-guide-sections.md](teacher-guide-sections.md). Section titles MAY vary slightly; informational obligations MUST be satisfied.

**Writing principles:**

- Interpret implementation; code is source of truth.
- Link canonical artifacts; do not duplicate verification gates, feature-report tables, or State status.
- Audience: junior developers, new contributors, maintainers, project owner learning architecture.
- Open with audience and prerequisite-artifact note (learning vs scope/verification/status).

When updating an existing guide: preserve accurate content; refresh concept inventory, pitfalls, and evolution history for contract or concept-map changes.

---

## Optional Output — knowledge-analysis.md

Produce when the user requests analysis, eligibility is uncertain, or a human reviewer wants a pre-draft inventory.

Suggested structure:

```markdown
# Knowledge Analysis — <feature-slug>

## Eligibility
Decision, rationale

## Concept Inventory
Name | Class (Primary/Supporting/Adjacent) | Evidence (file/type) | Purpose

## Classifications Summary
Primary: ...
Supporting: ...
Adjacent: ...

## Teaching Priorities
Ordered list for guide emphasis

## Identified Gaps
Missing evidence, deferred coverage, stale SDD vs code

## Future Improvement Candidates
Post-guide hardening ideas (not backlog commitments)
```

Do not treat `knowledge-analysis.md` as a substitute for `teacher-guide.md`.

---

## Quality Gate

Before declaring complete, run every check in [quality-gate.md](quality-gate.md).

**Reject output** if:

- Any mandatory section is missing.
- Any concept lacks code evidence.
- Pitfalls < 3 or are generic.
- Feature Evolution History omits material contract/behavior changes.
- Study Guide topics are not traceable to implementation.
- Guide duplicates verification, feature-report, or State content.
- Guide teaches unimplemented or speculative design.

Fix violations before delivery. Recommend human review against codebase for concept accuracy.

---

## Handoff

After a passing quality gate:

1. Confirm output path: `Documentation/SDD/<feature-slug>/teacher-guide.md`.
2. Note whether `knowledge-analysis.md` was produced.
3. Recommend Documentation Follow-Up record the guide path when not already noted.
4. Do **not** update `State.md`, PM, or verification artifacts unless the user explicitly requests documentation updates via `documentation-update` skill.

Reporting MAY summarize that a Teacher Guide exists; it MUST NOT replace the guide.

---

## Anti-Patterns

- Teaching from `design.md` without confirming production code.
- Copying Paciente or any prior guide structure without re-extracting concepts from the current feature.
- Inventing pitfalls from general best practices instead of this implementation.
- Pasting Swagger routes or full REQ tables (link SDD instead).
- Generating before Verify completes.
- Becoming a feature-specific skill (keep patterns generic; evidence comes from the target feature each run).

## Additional Resources

- Governance: [knowledge-strategy.md](Documentation/AI-Harness/Harness-Design/knowledge-strategy.md)
- Section obligations: [teacher-guide-sections.md](teacher-guide-sections.md)
- Quality checklist: [quality-gate.md](quality-gate.md)
- Calibration example (structure reference only): `Documentation/SDD/paciente-sql-stabilization/teacher-guide.md`
