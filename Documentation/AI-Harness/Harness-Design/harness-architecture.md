# Doc Organo AI Harness Architecture

## Purpose

This document defines the target architecture and governance model for the Doc Organo AI Harness. It is the design foundation for future changes to `AGENTS.md`, `.cursor/rules/`, `.cursor/skills/`, `Documentation/State.md`, review prompts, SDD artifacts, MCP usage, and auxiliary documentation.

It does not rewrite operational artifacts and does not create feature implementation tasks. Its role is to define what each harness component should own, how context should be loaded, and how verification feedback should return to durable documentation.

## Architecture Principles

1. **Codebase is context.** Agents should learn from canonical project files instead of repeated prompt explanations.
2. **RPI remains the core workflow.** Research creates durable markdown, Plan converts it into decisions and designs, Implement uses focused context and verification gates.
3. **Static and dynamic context must be separate.** Stable project facts belong in `AGENTS.md` and durable docs; current branch truth belongs in `Documentation/State.md`.
4. **Rules are guardrails, skills are workflows.** Rules should stay short and persistent; skills should handle repeatable multi-step work.
5. **Verification is part of the harness, not an afterthought.** Build, test, smoke checks, review prompts, and PR checklists are sensors that feed back into `State.md`, ADRs, or future rules.
6. **Clinical data is sensitive by default.** Security and PHI constraints apply to code, docs, prompts, logs, examples, tests, and commits.
7. **Adoption should stay incremental.** The harness should mature around the active SQL migration and clinical MVP, not introduce broad process overhead before it is useful.
8. **Authority must be explicit.** When harness artifacts conflict, agents should know which source owns architecture truth, operational truth, feature truth, and machine-facing guidance.

## Multi-Tool Architectural Foundation

### Three-Layer Ownership Taxonomy

The Harness architecture is organized into three layers, distinguished by ownership:

```
┌─────────────────────────────────────────────────────────────┐
│                    HARNESS ASSETS                            │
│  (Tool-agnostic governance — defined once, consumed by all)  │
│                                                             │
│  • Authority hierarchy (ADRs > Arch docs > State > SDD >   │
│    Rules > Skills)                                          │
│  • SDD lifecycle (Research → Specify → Design → Tasks →     │
│    Pre-Execution Review → Execute → Verify → Follow-Up →    │
│    Reporting → Teacher Guide)                               │
│  • Verification governance                                  │
│  • Documentation routing                                    │
│  • Calibration workflow                                     │
│  • Rule content (guardrail text)                             │
│  • Skill content (workflow procedures)                       │
│  • Rule scoping intent (always-on vs file-scoped)           │
│  • Context loading strategy (what to load)                  │
│  • Knowledge transfer strategy                              │
│  • Artifact ownership model                                  │
│  • Adaptive sizing model                                    │
│  • ADR governance policy                                    │
│  • Definition of Done                                       │
└─────────────────────────────────────────────────────────────┘
           │                    │
           │ consumed by        │ consumed by
           ▼                    ▼
┌──────────────────────┐  ┌──────────────────────┐
│   SHARED ASSETS      │  │    TOOL ASSETS        │
│  (Both tools load    │  │  (Per-tool format,    │
│   identically)       │  │   path, frontmatter)  │
│                      │  │                      │
│  • AGENTS.md         │  │  Cursor:              │
│    (bootstrap)       │  │  • .cursor/rules/*.mdc│
│                      │  │  • .cursor/skills/    │
│                      │  │  • alwaysApply/globs  │
│                      │  │  • .mdc extension     │
│                      │  │                      │
│                      │  │  Cline:               │
│                      │  │  • .clinerules/*.md   │
│                      │  │  • .cline/skills/     │
│                      │  │  • paths frontmatter  │
│                      │  │  • .md extension     │
│                      │  │  • .clineignore       │
└──────────────────────┘  └──────────────────────┘
```

### Layer Definitions

#### Harness Assets

Harness Assets are capabilities and artifacts owned by the Harness methodology, defined in tool-agnostic terms. They are authored once and consumed by all supported tools through their respective format adaptations.

| Asset | Description | Why it is Harness-owned |
|------|-------------|------------------------|
| Rule content | The guardrail text of each rule (e.g., "Never commit real patient names...") | Content is pure markdown, portable, and expresses governance intent — not tool mechanics |
| Skill content | The workflow procedures in each `SKILL.md` body | Content is pure markdown, portable, and encodes Harness workflows — not tool loading |
| Authority hierarchy | ADRs > Architecture docs > State > SDD > Rules > Skills | Governance structure is tool-agnostic by design |
| SDD lifecycle | Research → Specify → Design → Tasks → … → Teacher Guide | Lifecycle is tool-agnostic by design |
| Verification governance | Gate selection, review sensors, residual risk, skipped-gate reasoning | Verification model is tool-agnostic by design |
| Documentation routing | Path-drift checks, follow-up routing | Routing is a Harness process, not a tool mechanism |
| Calibration workflow | Pilot Execution → Pilot Report → Implementation Plan → Review → Phased Update → Consistency Audit → Next Pilot | Process is tool-agnostic |
| Rule scoping intent | Whether a rule is always-on or file-scoped | The *intent* is governance; the *expression* is tool-specific |
| Context loading strategy | What context to load for each task type | The *strategy* is governance; the *mechanism* is tool-specific |
| Knowledge transfer strategy | Teacher Guides, concept classification, study paths | Tool-agnostic by design |
| Artifact ownership model | Who owns what truth | Tool-agnostic by design |
| Adaptive sizing model | Small/Medium/Large/Complex | Tool-agnostic by design |
| ADR governance policy | Creation criteria, conflict resolution, escalation | Tool-agnostic by design |
| Definition of Done | Completion criteria for SDD-backed work | Tool-agnostic by design |
| Test governance | Architectural testing principles, ownership model, classification taxonomy, scenario vocabulary, design guidelines, progressive evolution | Tool-agnostic by design |

#### Tool Assets

Tool Assets are capabilities and artifacts owned by a specific AI tool's implementation. Each tool has its own set of Tool Assets. Tool Assets serve the Harness methodology, not the reverse.

| Asset | Cursor implementation | Cline implementation |
|------|----------------------|---------------------|
| Rule storage location | `.cursor/rules/` | `.clinerules/` |
| Rule file extension | `.mdc` | `.md` or `.txt` |
| Rule frontmatter schema | `alwaysApply: true/false`, `globs: pattern`, `description` | `paths: [patterns]` (conditional) or no frontmatter (always-on) |
| Skill storage location | `.cursor/skills/` | `.cline/skills/` |
| Skill format | `SKILL.md` with `name`/`description` | `SKILL.md` with `name`/`description` (identical) |
| Context management mechanism | `token-economy.mdc` rule | `.clineignore`, `/smol`, `/newtask`, Memory Bank |
| Rule loading mechanism | Auto-injection of `alwaysApply: true` rules; `globs` for file-scoped | Context-aware dynamic evaluation of `paths`; always-on for no-frontmatter |
| Tool-specific optimizations | — | `.clineignore`, slash commands, Plan/Act mode, skill/rule toggles, global scope |

#### Shared Assets

Shared Assets are artifacts that are both Harness-owned in content and consumed identically by all tools. They sit at the intersection of Harness and Tool layers.

| Asset | Description | Why it is shared |
|------|-------------|------------------|
| `AGENTS.md` | Cross-tool bootstrap file | Both Cursor and Cline auto-load it identically. Content is Harness-authored; loading is tool-native. |

### Ownership Responsibilities

**Harness governance owns:**

- Defining rule content and skill content (the canonical guardrail text and workflow procedures).
- Defining the authority hierarchy, SDD lifecycle, verification governance, documentation routing, and calibration workflow.
- Maintaining `AGENTS.md` as a shared bootstrap.
- Maintaining Harness Design docs (`harness-architecture.md`, `rules-strategy.md`, `skills-strategy.md`, `sdd-operational.md`).
- Ensuring governance documentation references tool-agnostic concepts by default.

**Each tool implementation owns:**

- The tool's directory structure (`.cursor/` for Cursor, `.clinerules/` + `.cline/` for Cline).
- The tool's file format (`.mdc` for Cursor, `.md` for Cline).
- The tool's frontmatter schema (`alwaysApply`/`globs` for Cursor, `paths` for Cline).
- Tool-specific optimizations (`.clineignore`, slash commands, Plan/Act mode for Cline).
- Ensuring that tool-specific projections faithfully reproduce the canonical governance content.

**The relationship is asymmetric:** Harness governance defines what governance should be; tool implementations provide how it is loaded and applied. Tool implementations serve the methodology, not the reverse. Tool Assets are implementation details, not governance authorities. The authority hierarchy remains tool-agnostic — Tool Assets do not compete with ADRs, Architecture docs, or Rules.

### Artifact Classification

Every existing artifact is classified under this taxonomy. Cursor artifacts carry a dual classification during the transition period: they are Tool Assets by format and location, and their *content* (rule guardrail text, skill workflow procedures) is Harness Asset content. No file is moved or modified — classification is conceptual.

| Artifact | Location | Layer | Notes |
|----------|----------|-------|-------|
| Rule guardrail text (6 rules) | `.cursor/rules/*.mdc` (content) | Harness Asset | Content is canonical governance; format is Cursor Tool Asset |
| Rule format/frontmatter | `.cursor/rules/*.mdc` (format, frontmatter) | Tool Asset (Cursor) | `.mdc` extension, `alwaysApply`/`globs` frontmatter |
| Skill workflow procedures (7 skills) | `.cursor/skills/*/SKILL.md` (content) | Harness Asset | Content is canonical governance; location is Cursor Tool Asset |
| Skill storage location | `.cursor/skills/` | Tool Asset (Cursor) | Cursor-specific directory |
| `AGENTS.md` | Repository root | Shared Asset | Harness-owned content; auto-loaded by both tools |
| Authority hierarchy | `harness-architecture.md` §Artifact Authority Hierarchy | Harness Asset | Tool-agnostic by design |
| SDD lifecycle | `sdd-operational.md` | Harness Asset | Tool-agnostic by design |
| Verification governance | `verification-governance.md` | Harness Asset | Tool-agnostic by design |
| Documentation routing | `documentation-update` skill, `CONTRIBUTING-AI.md` | Harness Asset | Routing is a Harness process |
| Calibration workflow | `CONTRIBUTING-AI.md` | Harness Asset | Process is tool-agnostic |
| Governance documentation | `Documentation/AI-Harness/Harness-Design/` | Harness Asset | Defines Harness methodology |
| SDD templates | `Documentation/AI-Harness/template/sdd/` | Harness Asset | Tool-agnostic templates |
| Review prompts | `Documentation/AI-Harness/review-prompts/` | Harness Asset | Tool-agnostic sensors |
| Cline rule projections | `.clinerules/*.md` (new) | Tool Asset (Cline) | Cline-specific format, frontmatter, and directory |
| Cline skill projections | `.cline/skills/*/SKILL.md` (new) | Tool Asset (Cline) | Cline-specific location; content is Harness Asset |
| `.clineignore` | Repository root (new, optional) | Tool Asset (Cline) | Cline-specific context optimization |

### Canonical Content Principle

**Content is canonical; format is tool-specific.** This principle means:

- There is one authoritative version of each rule's guardrail text.
- There is one authoritative version of each skill's workflow procedures.
- Each tool consumes that content through its own format, frontmatter, and directory.
- The adaptation mechanism (how content moves from canonical to tool-specific) is an implementation concern, not a governance concern.

"Canonical" refers to the governance-level truth that all Tool Assets must faithfully reproduce. During the transition, the Cursor projection serves as the de facto canonical source. The Target Architecture aims for governance content to be independently definable, at which point the canonical source is the Harness governance documentation itself.

### Transition Architecture vs Target Architecture

**Transition Architecture (Current):** Cursor is the Initial Reference Implementation. The canonical governance content exists within Cursor's Tool Assets (`.cursor/rules/*.mdc`, `.cursor/skills/*/SKILL.md`). Cursor artifacts serve a dual role: they are both Tool Assets (by format and location) and the de facto source of Harness Asset content.

**Target Architecture:** The Harness governance is independent of any specific AI tool. Harness Asset content (rule guardrail text, skill workflow procedures) is defined in tool-agnostic terms. Each tool — Cursor, Cline, and future tools — provides its own Tool Assets (format, frontmatter, directory) that project the same canonical governance content.

The transition is additive: Cursor remains fully operational throughout, and Cline support is added alongside it.

### Implementation Model

An AI Harness implementation is a **tool-specific projection of the Harness's canonical governance content into a tool's native format, directory, and loading mechanism**. An implementation consists of:

1. **Rule projections** — Each Harness rule content expressed in the tool's file format, frontmatter schema, and storage directory.
2. **Skill projections** — Each Harness skill content placed in the tool's skill directory using the tool's `SKILL.md` format.
3. **Bootstrap consumption** — The tool auto-loads `AGENTS.md` as the shared entry point.
4. **Context management** — The tool applies its native context loading mechanisms to achieve the Harness's context loading strategy.
5. **Tool-specific optimizations** — The tool may offer capabilities not present in other tools (e.g., Cline's `.clineignore`, slash commands). These are Tool Assets that complement but do not alter Harness governance.

### Future Tool Compliance

To become a compliant Harness implementation, a future AI tool must provide:

1. **Rule loading mechanism** — Load persistent rules from markdown files in a tool-specific directory.
2. **Skill loading mechanism** — Load skills on-demand from `SKILL.md` files with `name`/`description` frontmatter.
3. **Bootstrap loading** — Auto-load `AGENTS.md` as the shared cross-tool entry point (or equivalent).
4. **Non-conflicting coexistence** — Directory structure must not conflict with existing tool directories.
5. **Content fidelity** — Rule and skill projections must faithfully reproduce canonical governance content.
6. **Context loading** — Implement the Harness's context loading strategy through native mechanisms.

What a future tool may optionally provide: tool-specific optimizations (context exclusions, slash commands, Plan/Act mode), global scope for rules and skills, toggle capabilities.

What the architecture does not require: a specific file format, a specific frontmatter schema, a specific directory name, a specific loading mechanism, or a specific invocation method.

### Rule and Skill Projection Architecture

Rule content (guardrail text) is a Harness Asset. Rule format (file extension, frontmatter schema) and rule storage location are Tool Assets.

| Rule | Scoping intent | Cursor format | Cline format |
|------|---------------|---------------|--------------|
| `security-phi` | Always-on | `alwaysApply: true` | No frontmatter |
| `token-economy` | Always-on | `alwaysApply: true` | No frontmatter |
| `update-doc` | Always-on | `alwaysApply: true` | No frontmatter |
| `backend-architecture` | File-scoped (`DocAPI/**/*.cs`) | `globs: "DocAPI/**/*.cs"` | `paths: ["DocAPI/**/*.cs"]` |
| `ef-migrations` | File-scoped (`DocAPI/**/*.cs`) | `globs: "DocAPI/**/*.cs"` | `paths: ["DocAPI/**/*.cs"]` |
| `blazor-front` | File-scoped (`DocFront.Web/**/*`) | `globs: "DocFront.Web/**/*"` | `paths: ["DocFront.Web/**/*"]` |

Skill content (workflow procedures) is a Harness Asset. Skill storage location is a Tool Asset. Skill format (`SKILL.md` with `name`/`description`) is shared.

| Skill | Purpose |
|-------|---------|
| `doc-organo-context` | Session orientation |
| `codebase-decomposition` | DDD and technical debt analysis |
| `effective-harness-planning` | Harness review and planning |
| `verifier` | Verification workflow |
| `documentation-update` | Documentation routing and Follow-Up |
| `sql-migration-workflow` | SQL migration workflow |
| `not-a-teacher` | Teacher Guide generation |

Cross-skill references use **skill names**, not tool-specific paths (e.g., "See the `documentation-update` skill"). When a path is essential, both tool paths are provided in parenthetical disambiguation.

### Path Reference Strategy

Governance documentation references **tool-agnostic concepts by default**, with **tool-specific paths in parenthetical disambiguation** when implementation detail is needed:

| Context | Pattern | Example |
|---------|---------|---------|
| Governance concept (default) | Tool-agnostic concept name | "Harness rules" |
| Implementation detail needed | Concept + parenthetical paths | "Harness rules (Cursor: `.cursor/rules/*.mdc`; Cline: `.clinerules/*.md`)" |
| Single-tool context | Tool-specific path is acceptable | "Cursor loads rules from `.cursor/rules/*.mdc`" |
| Historical artifact | No change — preserve original reference | SDD instances with `.cursor/` references remain as-is |

### Tool-Specific Optimizations Governance

Tool-specific optimizations are **encouraged as implementation optimizations** but are **not adopted as Harness governance**. The Harness defines what context should be loaded; each tool provides how.

| Mechanism | Tool | Classification | Treatment |
|-----------|------|----------------|-----------|
| `token-economy` rule | Cursor | Harness Asset (content) + Tool Asset (mechanism) | Existing — unchanged |
| `.clineignore` | Cline | Tool Asset (Cline-specific) | Documented as a recommended Cline optimization; not a Harness governance requirement |
| `/smol`, `/newtask` | Cline | Tool Asset (Cline-specific) | Documented as Cline context management features |
| Memory Bank | Cline (tool-agnostic methodology) | Not adopted | `State.md` + durable documentation achieves the same goal |
| Plan/Act mode | Cline | Tool Asset (Cline-specific) | Complements the SDD lifecycle |
| Global scope | Cline | Tool Asset (Cline-specific) | Not a Harness governance concern; all Harness artifacts are project-scoped |
| Toggle capabilities | Cline | Tool Asset (Cline-specific) | Deferred — Harness does not define toggle policy |

## Current State Assessment

The harness has completed first-pilot calibration (Paciente SQL Stabilization) and Wave 1–2 governance alignment. It is operational for forward SDD work; CI enforcement and some secondary docs remain incremental improvements.

Existing strengths:

- `AGENTS.md` gives a compact project bootstrap: stack, branch split, repository layout, bounded contexts, commands, SDD entry point, and verification entry points.
- `.cursor/rules/` separates always-on security/token guidance from scoped backend, EF, and Blazor conventions.
- `.cursor/skills/` includes operational workflows: verifier, documentation-update, not-a-teacher, sql-migration, codebase decomposition, and harness planning.
- `Documentation/AI-Harness/` contains calibrated SDD operational governance, verification governance, reporting and knowledge strategy, templates, and contribution workflow including Harness Calibration Workflow.
- First feature SDD instance exists: `Documentation/SDD/paciente-sql-stabilization/` (historical pre-calibration reference).
- Product, architecture, technical, and ADR docs provide durable context for AI-assisted development.

Main architecture risks:

- Path drift may remain in older research or auxiliary docs referencing pre-taxonomy paths.
- `Documentation/State.md` can become stale if Documentation Follow-Up is skipped after Verify.
- Harness secondary docs can drift from `sdd-operational.md` if calibration workflow is not followed after future pilots.
- Review prompt coverage for EF migrations and API contracts remains incomplete.
- CI workflow enforcing at least `dotnet build` and `dotnet test` is not yet in place.
- `Documentation/Technical/api-contract.md` does not exist yet.

## Component Responsibilities

| Component | Purpose | Responsibilities | Inputs | Outputs | Update triggers | Relationship with other components |
|-----------|---------|------------------|--------|---------|-----------------|------------------------------------|
| `AGENTS.md` | Static agent bootstrap | Stable project facts, stack, branch model, repo layout, bounded contexts, commands, task-to-doc map, high-level verification and SDD entry points | PRD, architecture docs, technical docs, ADRs, `State.md` pointers | Consistent session orientation | Stack, layout, commands, architecture rules, or canonical doc map changes | Points agents to `State.md`, docs, rules, skills, SDD, MCPs; should not duplicate full methodology |
| `.cursor/rules/` | Persistent behavior guardrails | Always-on or glob-scoped constraints for PHI, token economy, DDD, EF, Blazor, docs updates | Architecture decisions, security policy, repeated review findings | Runtime guidance injected into agent context | New persistent invariant, recurring defect, new file scope | Rules link to durable docs and may recommend skills for deeper workflows |
| `.cursor/skills/` | Repeatable task workflows | Step-by-step procedures for onboarding, decomposition, harness planning, future SQL slices and legacy rule porting | Rules, docs, codebase patterns, known risky workflows | Structured analysis or execution plan | Workflow repeats, requires tool sequence, or exceeds rule size | Skills consume `AGENTS.md`, `State.md`, architecture docs, and produce markdown handoffs |
| `Documentation/State.md` | Current operational truth | Branch, runtime status, active epic, blockers, next steps, recent decisions, harness pointers | Latest merged work, session outcomes, verification results | Bootstrap state for next session | End of relevant session, PR merge, runtime status change, blocker resolution | Read before large tasks; feeds `AGENTS.md` context indirectly without becoming historical narrative |
| Review prompts | Pre-merge review sensors | Domain, security/PHI, documentation, EF/API contract checklists | Diff, domain docs, security rules, ADRs, PR template | Findings by severity, missing checks, gate result | New review class, recurring bug, new compliance concern | Complement rules; deeper than always-on guidance; should feed PR review and future rules |
| Verifier role | Verification ownership model | Decide applicable gates, run or request checks, apply review prompts, record skipped gates, summarize residual risk | Diff, SDD tasks, review prompts, PR template, ADRs, security rules, `State.md` | Verification summary, skipped-gate reasons, residual risks, State/doc/ADR follow-up signal | Change reaches review/PR boundary, risky work completes, SDD tasks claim done | Orchestrates review prompts and test/smoke gates; does not replace human review or CI |
| `test-governance.md` | Architectural testing authority | Testing principles, ownership model, classification taxonomy, scenario vocabulary, design guidelines, progressive evolution, review questions | Engineering Review Reports, implementation evidence, accepted ADRs | Canonical testing governance consumed by SDD Operational, Verification Governance, verifier skill, and templates | New validated testing patterns, new Architectural Capability adoption | Referenced by `sdd-operational.md` Testing Governance Model; evaluated by verifier test strategy review gate |
| SDD | Feature design workflow | Per-feature specify, design, tasks for features larger than a small change | PRD, PM, RoadMap, Domain Overview, architecture docs, State, research | Scoped feature contract, design decisions, executable tasks, verification plan | Feature > half day, cross-module change, risky clinical/security behavior | Bridges Plan to Execute and post-Verify completion chain; tasks should reference review prompts and Follow-Up |
| MCPs | External tool and runtime bridge | GitHub PR/issues, browser smoke tests, future docs/db access | Tool schemas, repo state, browser or GitHub data | Runtime evidence, PR metadata, smoke-test observations | Need external state or browser verification | MCP results should be summarized into docs or PRs, not become hidden chat-only context |
| Auxiliary docs | Durable knowledge base | PRD, PM, RoadMap, Architecture, ADRs, Technical docs, AI research, playbook, documentation index | Product decisions, architecture decisions, implementation evidence | Canonical context for agents and humans | Product, architecture, technical, methodology, or taxonomy change | Referenced by `AGENTS.md`, skills, SDD, and review prompts |

## Artifact Authority Hierarchy

The harness should use this hierarchy to resolve conflicts between artifacts. A conflict should be fixed in the stale artifact during an operational update pass, but agents need a temporary decision rule while working.

| Authority area | Primary source | Secondary source | Conflict rule |
|----------------|----------------|------------------|---------------|
| Architecture truth | Accepted ADRs | Architecture and Technical docs | ADRs win for accepted durable decisions. Architecture docs explain the current model and should be updated when an ADR changes the model. |
| Harness governance | Harness Design docs | `AGENTS.md`, rules, skills, contribution guides | Design docs define the target model. Operational artifacts implement it later and should not invent conflicting policy. |
| Operational truth | `Documentation/State.md` | PM, PRs, recent verification reports | State wins for current branch, runtime status, blockers, and next steps, even if bootstrap docs are stale. |
| Feature truth | Active SDD feature folder | User request, PM item, implementation notes | SDD owns feature scope, acceptance criteria, design, tasks, and verification expectations once created. It cannot override ADRs without proposing a new ADR. |
| Product truth | PRD | PM and RoadMap | PRD owns stable product intent. PM and RoadMap own sequencing and future horizons. |
| Machine-facing guidance | Rules and skills | `AGENTS.md`, review prompts | Rules enforce concise guardrails; skills own repeatable workflows. Review prompts detect issues but do not redefine project policy by themselves. |

If a conflict exposes a new durable decision, create or update an ADR. If it exposes stale current status, update `Documentation/State.md`. If it exposes recurring agent behavior risk, update a rule or skill in a later operational pass.

## Harness Lifecycle Flow

The target harness flow is:

```text
Research
->
Plan
->
SDD when risk, size, or cross-module scope requires it
  (Specify -> Design -> Tasks -> SDD Pre-Execution Review for Large/Complex)
->
Execute
->
Verify
->
Documentation Follow-Up
->
Reporting
->
Teacher Guide when warranted
->
State / ADR / Documentation Updates
```

| Phase | Purpose | Typical inputs | Required output |
|-------|---------|----------------|-----------------|
| Research | Understand codebase, docs, risks, and existing decisions | Code, docs, ADRs, research baseline, targeted searches | Durable findings or a short handoff, not long chat history |
| Plan | Convert research into a proposed path | Research output, PRD/PM, architecture docs, `State.md` | Plan, design note, or decision that can be reviewed |
| SDD | Capture feature-specific scope and design when risk warrants it | Plan, PRD/PM, Domain Overview, ADRs, technical docs | `specify.md`, `design.md`, `tasks.md`; SDD Pre-Execution Review before Execute for Large/Complex work |
| Execute | Make scoped changes using focused context | SDD or plan, relevant files, rules, skills | Code or documentation changes within the approved scope |
| Verify | Produce evidence that the work is correct enough to continue | Diff, SDD tasks, review prompts, test/smoke commands | Verification summary, skipped-gate reasons, residual risks |
| Documentation Follow-Up | Synchronize operational and technical truth after Verify | Verifier follow-up targets, active SDD | Updated State, PM, migration-sql, runbook, and routed docs |
| Reporting | Preserve continuity and feature summary | Verified implementation, synchronized docs | Session handoff, feature report, embedded lessons learned |
| Teacher Guide | Transfer verified implementation knowledge | Finalized code, verification, reporting | `teacher-guide.md` when knowledge strategy criteria apply |
| Updates | Feed durable learning back into the harness | Verification results, merged work, new decisions | State update, ADR, docs update, or future rule/skill recommendation |

Research and Plan may produce only design documentation. Implementation should not begin just because research exists; it begins when scope, risk, and verification expectations are clear enough.

## Harness Calibration Workflow

Feature SDD completion does not automatically update harness governance. When a pilot produces repeatable harness friction, use the calibration workflow defined operationally in `Documentation/AI-Harness/CONTRIBUTING-AI.md`.

```text
Pilot Execution -> Pilot Report -> Implementation Plan (Proven only)
  -> Review / Approval -> Phased Governance Update -> Consistency Audit -> Next Pilot
```

Pilot reports under `Documentation/SDD/<feature-slug>/reports/sdd-pilot-report-v*.md` are calibration inputs (see [sdd-pilot-report-governance.md](sdd-pilot-report-governance.md)), not feature truth, verification output, or operational truth. They do not override `sdd-operational.md`, `verification-governance.md`, or `Documentation/State.md`.

Finding maturity for adoption: **Experimental** → **Pilot-Proven** (Proven in report) → **Adopted** (implemented in governance) → **Canonical** (stable authority). Preliminary and Deferred findings require second-pilot validation or explicit approval before adoption.

First calibration instance: Paciente SQL Stabilization → [Documentation/SDD/paciente-sql-stabilization/reports/sdd-pilot-report-v1.0.md](../../SDD/paciente-sql-stabilization/reports/sdd-pilot-report-v1.0.md) → Wave 1–2 governance updates.

## ADR Governance Policy

ADRs record durable decisions that future agents and developers must understand to avoid re-litigating or accidentally reversing architectural direction.

Create an ADR when a decision:

- Changes bounded context ownership, aggregate boundaries, or where business rules live.
- Changes persistence strategy, schema lifecycle, migration approach, soft-delete behavior, or data history handling.
- Establishes or changes security, PHI, auth, authorization, logging, or secret-handling architecture.
- Changes public API contracts, ID strategy, DTO shape policy, or compatibility expectations across API/front boundaries.
- Chooses or replaces significant infrastructure, runtime, framework, integration, MCP, or deployment strategy.
- Creates a durable exception to an existing architecture rule.
- Resolves a recurring architectural disagreement that would otherwise keep appearing in plans or reviews.
- Is discovered during SDD design and affects more than the local feature implementation.

Do not create an ADR for:

- Local bug fixes or implementation details that do not change future architecture guidance.
- Mechanical refactors with no behavioral or ownership change.
- Temporary branch status, blockers, or next steps. Use `Documentation/State.md`.
- Feature-specific scope, acceptance criteria, or task sequencing. Use SDD.
- Routine documentation cleanup or path correction unless it changes documentation taxonomy.
- Decisions already covered by an accepted ADR, unless the decision revises or supersedes it.

An ADR should state the status, context, decision, consequences, and affected artifacts. Accepted ADRs may require follow-up updates to architecture docs, rules, skills, review prompts, SDD templates, or `AGENTS.md`, but those updates should happen in the appropriate operational pass.

## Verifier Role

The verifier is a harness role, not necessarily a separate person or tool at first. Its purpose is to prevent agents from self-certifying completion without evidence.

Responsibilities:

- Decide which verification gates apply based on risk, touched areas, SDD tasks, and PR expectations.
- Run or request applicable checks: build, tests, SQL integration tests, Swagger/API smoke, Blazor smoke, review prompts, and PR checklist items.
- Apply security/PHI review whenever clinical data, identity fields, logs, prompts, generated examples, auth assumptions, files, or secrets are touched.
- Record skipped gates with concrete reasons.
- Summarize residual risk and unresolved blockers.
- Recommend whether results should update `Documentation/State.md`, create/update an ADR, change durable docs, or inform future rules/skills.

Inputs:

- Diff or planned change scope.
- SDD `tasks.md` verification section when present.
- Relevant review prompts.
- PR template expectations.
- Security/PHI rule and accepted ADRs.
- Current `Documentation/State.md`.

Outputs:

- Verification summary with commands/checks run.
- Skipped-gate list with reasons.
- Findings grouped by severity when review prompts are used.
- Residual risk statement.
- State, ADR, documentation, rule, or skill follow-up recommendation.

Relationship with review prompts:

- Review prompts are sensors.
- The verifier chooses which sensors apply, runs or requests them, and converts results into a concise gate result.
- A review prompt finding may recommend a future rule or skill, but only after the governance owner decides it is recurring or durable.

Relationship with SDD:

- `specify.md` defines acceptance criteria.
- `design.md` identifies testability, ADR, and review risks.
- `tasks.md` names concrete verification gates.
- The verifier checks whether those gates were satisfied or explicitly skipped with reasons.

## Context Loading Policy

Always load or keep available:

- `Documentation/State.md`
- `AGENTS.md`
- always-on security/PHI and token economy rules

Load by task:

- Domain work: `Documentation/Architecture/Domain_Overview_Business_Rules.md`, relevant entity/controller/repository files, `domain-review.md`.
- SQL/EF migration: `Documentation/Technical/migration-sql.md`, ADR-001, ADR-002, EF rule, relevant Fluent configs/migrations.
- Frontend work: `Documentation/Technical/front-architecture.md`, Blazor rule, relevant page/state/service/mapper files.
- Product planning: `Documentation/Product/PRD.md`, `Documentation/Product/PM_DocOrgano.md`, `Documentation/Product/RoadMap.md`.
- Harness planning: `Documentation/AI-Harness/research/AI-Research.md`, `Documentation/AI-Harness/research/playbook.md`, `Documentation/AI-Harness/*`, `.cursor/rules/*`, `.cursor/skills/*`.
- Legacy behavior: targeted methods in `DocAPI/Legacy/_LegacySheetsDb/`, not the whole folder.

Avoid loading by default:

- Full Legacy folders.
- Full research chats.
- Large methodology docs in implementation sessions unless the task is harness architecture or governance.
- Financial domain files unless the task is explicitly scoped to future planning.

## Feedback And Verification Loop

The target loop is:

1. `AGENTS.md`, rules, skills, SDD, and auxiliary docs guide the agent before changes.
2. The agent produces a small plan or SDD-backed implementation path.
3. Verification gates run according to risk: build, tests, SQL integration, Swagger/Blazor smoke, review prompts, PR checklist.
4. Findings feed back into one of:
   - `Documentation/State.md` for current status or blockers.
   - ADRs for durable architecture decisions.
   - Rules for recurring guardrails.
   - Skills for repeatable workflows.
   - SDD templates when feature planning quality improves.

## Missing Or Weak Architecture Pieces

Missing or incomplete:

- Documentation path normalization guidance for stale `docs/` links and absent `Documentation/AI-Harness/Research/*` paths.
- Ongoing documentation path normalization for renamed or moved harness artifacts.
- Review prompt coverage for EF migrations and API contracts.
- Continued alignment between verification governance, the verifier skill, review prompts, and future CI.
- Skills for legacy Atendimento rule porting beyond sql-migration-workflow.
- CI workflow enforcing at least `dotnet build` and `dotnet test`.
- `Documentation/Technical/api-contract.md`.
- Clear MCP usage policy beyond GitHub and browser recommendations.

Overlaps to govern:

- `AGENTS.md`, `CONTRIBUTING-AI.md`, and `doc-organo-context` repeat session bootstrap guidance. This is acceptable if one canonical sequence is maintained.
- `security-phi.mdc`, `security-phi-review.md`, and PR checklist security items overlap intentionally at different depths.
- `dotnet-ddd.mdc`, Domain Overview, and Architecture Overview overlap intentionally between concise enforcement and durable explanation.
- `AI-Research.md` and `playbook.md` should remain research/methodology inputs, not daily operational truth.
- Duplicate or stale skill paths should be resolved by policy before adding more skills.

## Adoption Strategy

Testing:

- Treat tests as verification sensors, not a bounded context.
- Start with gates already documented: `dotnet build`, `dotnet test`, then SQL integration tests for the active migration slice.
- Add characterization tests before porting critical Legacy Atendimento behavior.
- Defer E2E until API and Blazor flows stabilize.

Verification gates:

- Keep a risk-based ladder: build, test, smoke, review prompts, PR checklist.
- Make the verifier role explicit before relying on agents to self-certify completion.
- Use PR template and review prompts as the human-readable gate until CI is available.

Security reviews:

- Keep PHI/security as always-on guidance plus pre-merge review.
- Require security/PHI review whenever clinical data, logs, files, auth assumptions, API fields, or tests are touched.
- Promote recurring security findings into rules only when they are short and persistent.

SDD workflow:

- Use SDD for features larger than a small change, cross-module work, or clinical/security-sensitive behavior.
- Keep `specify.md` focused on problem, goals, out-of-scope, acceptance criteria, and ubiquitous language.
- Keep `design.md` focused on architecture choices, reuse, data/API/front changes, risks, and ADR needs.
- Keep `tasks.md` focused on executable steps, sequencing, verification, and State update.

MCP usage:

- Phase 1: GitHub for issues/PRs and browser for Swagger/Blazor smoke tests.
- Phase 2: consider docs search or DB introspection only after local docs, verification, and SQL migration practices stabilize.
- MCP evidence should be converted into durable documentation or PR notes when it affects decisions.

## Recommendations For Future Operational Updates

These are design recommendations only; they should be applied in a later operational update pass.

For `AGENTS.md`:

- Align all documentation and SDD paths with the canonical `Documentation/` taxonomy.
- Replace references to missing `Documentation/studies.md` with the chosen canonical methodology or research path.
- Keep the file as a bootstrap, not a methodology manual.
- Add only short stable guidance for verifier expectations if the verification model is adopted.

For `Documentation/AI-Harness/CONTRIBUTING-AI.md`:

- Clarify the canonical SDD template path and feature SDD folder path.
- Add a concise verifier step between implementation and commit/PR.
- Link review prompts as expected review sensors, not optional reading.
- Keep commit guidance brief and avoid duplicating PR template details.

For `Documentation/AI-Harness/rules.md`:

- Include every active rule, including `update-doc.mdc`.
- Add rule ownership metadata: purpose, scope, load policy, update trigger.
- State when not to add a rule and when to create a skill instead.
- Keep it as an index, not a copy of rule contents.
