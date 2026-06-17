# Knowledge Strategy

## Purpose

This document defines the governance boundaries for **knowledge-transfer artifacts** in the Doc Organo AI Harness. Knowledge artifacts exist to help contributors understand verified implementations — not to track delivery, prove correctness, or preserve session continuity.

Knowledge strategy governs:

- The **Teacher Guide** artifact category.
- Ownership boundaries between Teacher Guides and existing harness artifacts.
- Mandatory structure, generation timing, and quality expectations.
- Concept extraction, study roadmap, and code-reading guidance policies.

Knowledge strategy does **not** govern:

- Feature scope, requirements, design, or tasks (active SDD).
- Verification evidence or completion decisions (Verifier).
- Operational truth ( `Documentation/State.md` ).
- Backlog sequencing (PM).
- Durable architecture decisions (ADRs).
- Session continuity or workflow lessons (reporting artifacts).

This document is governance, not a Teacher Guide template. The operational workflow for generating Teacher Guides is implemented in `.cursor/skills/not-a-teacher/SKILL.md`.

## Scope

Knowledge strategy v1 applies after the first SDD pilot (**Paciente SQL Stabilization**), which produced the first Teacher Guide and validated the pedagogical gap in the harness lifecycle.

v1 covers:

- Teacher Guide purpose and boundaries.
- Mandatory and recommended sections.
- Concept inventory and study roadmap rules.
- Generation timing within the harness lifecycle.
- Relationship to SDD, verification, reporting, and documentation follow-up.

v1 does **not** define:

- Automated Teacher Guide generation.
- Knowledge graphs as mandatory diagrams.
- Teacher Guides for non-SDD or trivial changes by default.
- Metrics or dashboards for knowledge artifact usage.

## Knowledge Artifact Definition

### Teacher Guide

A **Teacher Guide** is a post-implementation pedagogical artifact that explains **what was implemented**, **why it was implemented**, **how it works**, and **what to study** to understand and safely evolve the feature.

A Teacher Guide MUST enable someone unfamiliar with the implementation to:

- Navigate the codebase with intent.
- Understand architectural flow across layers.
- Recognize concepts actually exercised in production code.
- Avoid common extension mistakes.
- Know where to read next without duplicating SDD, verification, or reporting artifacts.

A Teacher Guide MUST be grounded in **finalized implementation**. It MUST NOT teach abandoned ideas, rejected approaches, or speculative designs that were not merged.

**Canonical location:** `Documentation/SDD/<feature-slug>/teacher-guide.md`

**Language:** Generated Teacher Guide artifacts SHOULD be written in English, consistent with SDD, verification, and reporting artifacts.

## Authority Boundaries

Knowledge artifacts are **communication for learning**, not authority. They MUST NOT replace canonical sources.

| Authority area | Primary source | Teacher Guide role |
|----------------|----------------|-------------------|
| Feature scope and acceptance | Active SDD | Link; summarize intent only for learning context |
| Design decisions (durable) | ADRs + SDD design | Link ADRs; apply decisions to feature; do not restate full ADR |
| Correctness and evidence | `verification.md` | Link; do not repeat gate tables or completion decisions |
| Delivery summary | `feature-report.md` | Link; do not repeat shipped-scope tables |
| Session continuity | `session-handoff.md` | Link; do not repeat next steps or branch status |
| Operational truth | `Documentation/State.md` | Link; do not state runtime or backlog status |
| API contracts | Technical docs / SDD design | Link; do not duplicate full endpoint reference tables |
| Implementation truth | Codebase | Teacher Guide interprets code; code wins on conflict |

If a Teacher Guide conflicts with code, the guide is stale and MUST be updated or marked superseded.

## Responsibilities

### Teacher Guide owns

- Architecture walkthroughs for the verified feature slice.
- Concept explanations tied to actual implementation.
- Code navigation guidance (reading order, entry points, what to skip).
- Dependency and knowledge-map explanations.
- Study roadmaps traceable to the feature.
- Onboarding-oriented reasoning (why the code looks the way it does).
- Common pitfalls when extending the feature.
- Feature evolution history (material decisions and contract changes).
- Learning references (internal docs, ADRs, external docs) scoped to the feature.

### Teacher Guide must not become

| Anti-role | Owner instead |
|-----------|---------------|
| Verification report | `verification.md` |
| Feature report | `reports/feature-report.md` |
| Session handoff | `reports/session-handoff.md` |
| Project status tracker | `Documentation/State.md` |
| Backlog document | `Documentation/Product/PM_DocOrgano.md` |
| ADR replacement | `Documentation/Architecture/ADR/` |
| API specification | `Documentation/Technical/` (e.g. `migration-sql.md`, future `api-contract.md`) |
| Design document | SDD `design.md` |
| Requirements document | SDD `specify.md` |
| Task checklist | SDD `tasks.md` |
| Code dump or inline API reference | Code + Swagger |

## Relationship To Existing Artifacts

```text
Research → Plan → SDD → SDD Pre-Execution Review → Execute → Verify → Documentation Follow-Up → Reporting → Teacher Guide
                                                                                      ↑
                                        pedagogical layer; depends on finalized implementation
```

| Phase artifact | Question it answers | Teacher Guide MUST NOT duplicate |
|----------------|---------------------|----------------------------------|
| SDD | What should we build and how should we design it? | REQ tables, full design specs, task lists |
| Verification | Did we meet acceptance with evidence? | Gate results, residual risk ratings |
| Documentation Follow-Up | What project docs need updating? | State/PM/migration routing decisions |
| Feature Report | What shipped and what did we learn about the workflow? | Delivery summaries, PM references |
| Session Handoff | What happens next in the harness workflow? | Next steps, files to load for continuation |
| Teacher Guide | How do I understand and safely work on this feature? | — |

Reporting MAY summarize that a Teacher Guide exists. Reporting MUST NOT replace the Teacher Guide.

Documentation Update SHOULD route whether a Teacher Guide is warranted and record its path in follow-up notes when generated.

## Harness Lifecycle — Generation Timing

### When Teacher Guides are created

Teacher Guide generation MUST occur **after** all of the following are complete for the feature:

1. **Execute** — implementation merged or ready for merge.
2. **Verify** — `verification.md` exists with a completion decision.
3. **Documentation Follow-Up** — State, PM, and relevant technical docs updated.
4. **Reporting** — `feature-report.md` (and session handoff when applicable) generated when reporting policy applies.

Teacher Guides depend on **finalized implementation knowledge**. Generating a Teacher Guide before Verify or before documentation follow-up risks teaching unverified or stale behavior.

### Recommended workflow position

```text
Research
  → SDD (Specify / Design / Tasks / SDD Pre-Execution Review)
  → Execute
  → Verify
  → Documentation Follow-Up
  → Reporting
  → Teacher Guide
```

Teacher Guide is the **final learning artifact** of the feature lifecycle. It MAY be authored in the same session as Reporting or Documentation Follow-Up, but MUST NOT precede Verify.

### Generation triggers

A Teacher Guide SHOULD be generated when **any** of the following apply:

- First project instance of a pattern (e.g. first SQL integration test class, first aggregate migration vertical).
- SDD-backed feature crossing **three or more** architectural layers (e.g. API, application, domain, infrastructure, tests).
- SDD pilot or forward SDD for SQL migration verticals (WS01).
- Explicit request for onboarding material for a verified feature.

A Teacher Guide MAY be skipped when:

- Change is a bug fix with no new patterns or contract changes.
- Documentation-only or governance-only change with no implementation learning value.
- Feature repeats an identical pattern with no new design decisions (a shortened guide MAY still be produced for the new aggregate name and file paths).

Whether to skip MUST be recorded in Documentation Follow-Up or feature report when an SDD existed but no Teacher Guide was produced.

## Mandatory Teacher Guide Sections

Every Teacher Guide MUST include the sections below. Section titles MAY include minor wording variation, but the informational obligation MUST be satisfied.

| # | Section | MUST contain |
|---|---------|--------------|
| 1 | **How This Guide Differs From Other Artifacts** | Boundary table: what this guide owns vs SDD, verification, reporting, State, ADRs, technical docs |
| 2 | **Feature Overview** | Business purpose, technical objective, stabilization or delivery goals framed for learning |
| 3 | **What Changed** | Major implementation changes by layer; understanding-focused, not a commit log |
| 4 | **Architecture Walkthrough** | End-to-end flow through actual layers (e.g. Controller → mapping → domain → repository → EF → SQL) |
| 5 | **Concept Inventory** | Evidence-backed concepts with name, location, purpose, how it works, related concepts, study priority |
| 6 | **Deep Dive By Component** | Responsibilities and interactions for major components touched by the feature |
| 7 | **Testing Walkthrough** | Test types used, why each exists, representative tests to read, known gaps |
| 8 | **Security Concepts** | Security-sensitive patterns applied (e.g. PHI); principles, not compliance certification |
| 9 | **Knowledge Map** | Concept dependency chains; textual REQUIRED; diagram MAY be added |
| 10 | **Study Guide** | Beginner / Intermediate / Advanced paths traceable to this feature |
| 11 | **Common Pitfalls** | See dedicated governance below |
| 12 | **Feature Evolution History** | See dedicated governance below |
| 13 | **Suggested Next Feature** | Logical continuation; concepts reused vs expanded |
| 14 | **Quick Reference — Code Reading Order** | See Code Reading Guidance below |
| 15 | **Related Artifacts** | Links to SDD, verification, reports, ADRs, technical docs |

Sections MUST NOT duplicate full content from linked artifacts. They MUST explain and connect.

## Mandatory Section — Common Pitfalls

Every Teacher Guide MUST include a **Common Pitfalls** section.

### Purpose

Capture mistakes likely when **extending or modifying** the verified feature — not mistakes made during original implementation unless they inform future readers.

### Required structure

For each pitfall entry, the guide MUST state:

1. **What commonly breaks** — observable symptom or failure mode.
2. **Why it breaks** — underlying mechanism (e.g. mapping gap, filter semantics, missing config).
3. **How to avoid it** — concrete prevention (code location to update, test to run, pattern to follow).

### Minimum content rules

- MUST include at least **three** pitfalls derived from the actual feature.
- Pitfalls MUST be grounded in implementation or pilot lessons — not generic advice.
- SHOULD cover, when applicable to the feature:
  - forgotten mapping (DTO, AutoMapper, EF Fluent API);
  - forgotten repository or domain method updates;
  - test type that would catch the mistake;
  - query filter or persistence side effects;
  - security/PHI regression patterns;
  - contract assumptions (single-resource vs collection semantics).

### MUST NOT

- List verification failures or gate outcomes.
- Replace test strategy documentation or review prompts.

## Mandatory Section — Feature Evolution History

Every Teacher Guide MUST include a **Feature Evolution History** section.

### Purpose

Help maintainers understand **why the implementation looks the way it does** — especially when behavior diverges from Legacy, earlier drafts, or intuitive CRUD expectations.

### Required content categories

The section MUST capture, when applicable:

- **Contract changes** — routes, semantics, or response shape before vs after stabilization.
- **Architecture refinements** — layer responsibilities clarified during implementation.
- **Scope decisions** — what was deferred and why (e.g. frontend, auth, migrations).
- **Important tradeoffs** — accepted weaknesses with rationale (e.g. exception-message-based duplicate detection).
- **Security remediation** — material behavior changes for PHI or clinical safety.

### Format

Each entry SHOULD use a concise before/after or decision/rationale pattern. Example patterns (illustrative only — do not treat as required wording):

- *Before:* ambiguous single-result search route → *After:* collection search with 200 + empty array.
- *Removed:* PHI logging in controller → *Replaced by:* generic error messages.

### Sources

Evolution history MUST be derived from SDD design decisions, implementation diff, and verification findings — not from chat history alone.

### MUST NOT

- Repeat full Legacy behavior tables from `specify.md`.
- Rewrite ADR rationale in full.

## Code Reading Guidance

Every Teacher Guide MUST include **Quick Reference — Code Reading Order** (section 14 above).

### Purpose

Provide a recommended path through the codebase so readers build mental models in dependency order.

### Requirements

The reading order MUST include:

1. **Sequence** — ordered list of files or components to open.
2. **Rationale** — why this order (dependencies, bottom-up understanding).
3. **Dependency progression** — how each step enables the next.

### Strongly recommended default progression

For backend vertical slices, the reading order SHOULD follow this progression unless the feature justifies a different order:

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

Pilot lesson: reading controller first is tempting but hides mapping and domain decisions that explain most stabilization bugs.

### MUST NOT

- Paste large code blocks.
- Replace reading the codebase.

## Knowledge Map Guidance

Every Teacher Guide MUST include a **Knowledge Map** section.

### Purpose

Visualize **concept dependencies** so readers understand prerequisites and layering.

### Requirements

- MUST express at least **two** dependency chains relevant to the feature.
- MAY use ASCII/text trees (required minimum for v1).
- SHOULD note where diagram-based representations (Mermaid or similar) MAY be added in future revisions.
- Each node MUST correspond to a concept exercised in the feature — not a generic technology stack.

Example pattern (illustrative):

```text
DTO → AutoMapper → Domain entity → Repository → EF Core → SQL Server
```

### Future encouragement

Future Teacher Guides MAY include diagram-based knowledge maps. Diagrams SHOULD remain concept-level, not file-level graphs.

Open governance: whether diagrams become mandatory is deferred (see Open Governance Questions).

## Concept Inventory Governance

### Extraction process

Concepts MUST be extracted using this evidence pipeline:

```text
SDD requirements and design → candidates
        ↓
Implementation (production code) → confirmed usage only
        ↓
Tests and verification → confirmed validation or proof anchor
        ↓
De-duplicate and classify → Concept Inventory
```

A concept MUST be included only if:

1. It appears in **production code** or an **accepted ADR** referenced by the feature, AND
2. It contributes to understanding **this feature**, AND
3. It has at least one code location (type/method/file) and a stated business or design purpose.

A concept MUST be excluded if:

- Planned in SDD but not implemented.
- Generic language syntax with no design significance.
- Already fully owned by an ADR (link ADR instead of re-teaching).

### Concept card fields

Each inventory entry MUST include:

| Field | Requirement |
|-------|-------------|
| Concept name | Clear, standard terminology |
| Appears in | Primary file(s) or type(s) — method names preferred over line numbers |
| Why it exists | Business or design purpose |
| How it works | Brief mechanism in feature context |
| Related concepts | Cross-links within inventory |
| Study priority | High / Medium / Low mapped to Beginner / Intermediate / Advanced |

### Volume guidance

Teacher Guides SHOULD target **8–15** major concepts per feature. More than ~20 suggests duplication or insufficient filtering.

## Study Roadmap Governance

Every Teacher Guide MUST include a **Study Guide** with three tiers:

| Tier | Definition | MUST |
|------|------------|------|
| **Beginner** | Required before safely modifying the feature | Every topic linked to where it appears in this feature |
| **Intermediate** | Explains why the code is structured as it is | Topics derived from design decisions and patterns used |
| **Advanced** | Extension, hardening, cross-feature implications | Tradeoffs, caveats, and pilot-identified gaps |

### Rules

- Every listed topic MUST be **traceable to actual implementation** in the feature.
- MUST NOT publish generic curricula (e.g. “learn C#”) without tying to feature locations.
- External links MAY supplement but MUST NOT replace internal pointers.
- Study order within each tier SHOULD be numbered.

Pilot lesson: study roadmaps copied from textbooks fail onboarding; roadmaps tied to `PacienteProfile`, query filters, and integration skip policy succeed.

## Ownership And Maintenance

| Role | Responsibility |
|------|----------------|
| **Author** | Agent or developer with implementation context; drafts Teacher Guide after Reporting |
| **Reviewer** | Human or senior agent validates concept accuracy against code |
| **Router** | Documentation Update decides if guide is warranted and records path |
| **Consumer** | New contributors, maintainers, project owner learning architecture |

### Update policy

| Event | Action |
|-------|--------|
| Feature refactor changes concept map or contracts | Teacher Guide MUST be updated |
| ADR supersedes a decision explained in guide | Update evolution history; link new ADR |
| Feature superseded | Archive or mark guide superseded with pointer |
| No code change for extended period | Update not required |
| Pitfall discovered post-merge | SHOULD add to Common Pitfalls on next touch |

Teacher Guides are **durable learning docs** but **subordinate to code and ADRs**.

## Quality Gate

A Teacher Guide MUST NOT be considered complete until:

- [ ] All mandatory sections present.
- [ ] Every concept in inventory has code evidence.
- [ ] Common Pitfalls has ≥3 feature-specific entries.
- [ ] Feature Evolution History documents material contract or behavior changes.
- [ ] Code reading order includes rationale and dependency progression.
- [ ] Study Guide topics are traceable to implementation.
- [ ] No duplication of verification gates, feature-report summary, or State/PM status.
- [ ] Related Artifacts section links to canonical sources.
- [ ] Reviewer confirms implementation is source of truth.

## Pilot Lessons Applied

The first Teacher Guide (Paciente SQL Stabilization) and its harness workflow produced these governance inputs:

| Lesson | Governance response |
|--------|---------------------|
| Existing artifacts do not teach concepts or onboarding paths | Teacher Guide category defined with strict boundaries |
| Implementation differs from design drafts | MUST teach merged code only; Feature Evolution History captures drift |
| Mapping and config are common failure points | Common Pitfalls mandatory; concept extraction includes mapping and EF config |
| Verification and reporting duplicate easily | Boundary table mandatory in every guide |
| Study plans must be feature-grounded | Study roadmap rules require implementation traceability |
| Code reading order accelerates onboarding | Quick Reference mandatory with dependency progression |
| Teacher Guide fits after Reporting | Lifecycle position fixed after Verify, Documentation Follow-Up, and Reporting |
| First guide sufficient for governance; second guide needed for template | v1 defines structure; template creation deferred |

Workflow lessons from `feature-report.md` (State drift, tasks.md lag, runtime smoke vs API tests) inform **pitfall and study content**, not verification or reporting policy.

## Relationship To Future Template And Skill

Knowledge strategy defines **what** Teacher Guides must contain. The `not-a-teacher` skill implements this governance operationally. A future `template/knowledge/teacher-guide.md` MAY add a static section scaffold after a second calibration instance validates structure.

Template creation MUST wait until at least one manual Teacher Guide (Paciente) and one forward-SDD guide (e.g. Prontuario) validate the section structure.

## Open Governance Questions

The following decisions are **intentionally deferred** pending more pilot evidence:

| Question | v1 stance | Future decision trigger |
|----------|-----------|-------------------------|
| Should Teacher Guides be generated for **every** SDD feature? | SHOULD for significant / cross-layer / migration verticals; MAY skip trivial repeats | After Prontuario forward SDD |
| Should Teacher Guides be **mandatory** in verifier or documentation follow-up gates? | MAY be routed in Documentation Follow-Up; not a verification gate | After second guide + template |
| Should guides be **updated** after every refactor? | MUST when contracts or concept map break | Refactor policy per aggregate |
| Should **knowledge diagrams** become mandatory? | Textual Knowledge Map MUST; diagrams MAY | If maintainability cost is acceptable |
| Should **Common Pitfalls** feed back into rules or review prompts? | MAY inform; does not auto-update prompts | Recurring pitfall across ≥2 features |
| Should Teacher Guides exist **outside** SDD folders? | MUST live under active feature SDD path for v1 | If non-SDD features need guides |
| Should **automated** concept extraction from diff be used? | Not in v1 | Tooling maturity |
| Should WS07 frontend features share one guide with backend or split? | Open | First frontend alignment SDD |

## References

- [harness-architecture.md](harness-architecture.md) — lifecycle and authority hierarchy
- [sdd-operational.md](sdd-operational.md) — SDD phases and ownership
- [verification-governance.md](verification-governance.md) — verification boundaries
- [reporting-strategy.md](reporting-strategy.md) — reporting boundaries
- [template-architecture.md](template-architecture.md) — template ecosystem (future Teacher Guide template)
- Operational workflow: `.cursor/skills/not-a-teacher/SKILL.md`
- First calibration instance: `Documentation/SDD/paciente-sql-stabilization/teacher-guide.md`
