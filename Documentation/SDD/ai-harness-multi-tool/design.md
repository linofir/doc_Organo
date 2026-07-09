# Design — AI Harness Multi-Tool Evaluation

> Feature SDD: `Documentation/SDD/ai-harness-multi-tool/`
> Generated SDD artifacts are written in English.
> **Phase:** Design
> **Lifecycle position:** Research → Specify → **Design** → Tasks → SDD Pre-Execution Review → Execute → Verify → Documentation Follow-Up → Reporting → Teacher Guide
> **Input from:** [Specify](specify.md), [Research](research.md), [Documentation Review Report](../../AI-Harness/research/review-tool-agnostic-harness.md), [Harness Architecture](../../AI-Harness/Harness-Design/harness-architecture.md), [ADR-003](../../Architecture/ADR/ADR-003-documentation-taxonomy.md)

---

## Design Summary

The AI Harness evolves from a single-tool implementation (Cursor) into a multi-tool platform whose governance is independent of the AI tool used to execute it. This evolution is achieved by formalizing a three-layer ownership taxonomy — **Harness Assets**, **Tool Assets**, and **Shared Assets** — as the fundamental architectural boundary. Harness Assets (rule content, skill content, authority hierarchy, SDD lifecycle, verification governance) are defined once in tool-agnostic terms. Tool Assets (file format, frontmatter schema, storage location) are owned by each tool's implementation. Shared Assets (`AGENTS.md`) are consumed identically by all tools.

The architecture does **not** introduce a shared directory, a canonical file format, a cross-tool frontmatter schema, or an adapter/generation framework. Instead, it establishes that **content is canonical and format is tool-specific**: each tool consumes the same governance content through its own native format and directory. This preserves full backward compatibility with the existing Cursor implementation (the reference implementation) while enabling first-class Cline support through tool-specific format adaptation.

Governance documentation references tool-agnostic concepts by default, with tool-specific paths in parenthetical disambiguation when implementation detail is needed. Historical artifacts that reference `.cursor/` paths are not rewritten. ADR-003 is revised to generalize the documentation taxonomy for multi-tool artifacts, and a new ADR is recommended to establish the Harness Asset / Tool Asset / Shared Asset taxonomy as a permanent architectural boundary.

**Outcome-oriented design:** After implementation, the following must be true:

- Cursor loads rules from `.cursor/rules/*.mdc`, skills from `.cursor/skills/*/SKILL.md`, and `AGENTS.md` as bootstrap — with zero behavioral regression.
- Cline loads rules from `.clinerules/*.md`, skills from `.cline/skills/*/SKILL.md`, and `AGENTS.md` as bootstrap — with identical governance content.
- Governance documentation references tool-agnostic concepts by default, with tool-specific paths disambiguated parenthetically.
- No rule content or skill content is duplicated per tool.
- Historical SDD artifacts with `.cursor/` references remain readable and unmodified.
- The architecture supports adding a third AI tool without redesigning the artifact layer.

---

## Requirement Mapping

| Requirement | Design response |
|-------------|-----------------|
| `REQ-001` | Cursor artifacts (`.cursor/rules/*.mdc`, `.cursor/skills/*/SKILL.md`) remain unchanged. No Cursor file is modified, moved, or removed. The Design explicitly preserves `.cursor/` as the Cursor implementation directory. |
| `REQ-002` | Cline artifacts (`.clinerules/*.md`, `.cline/skills/*/SKILL.md`) are created from the same governance content as Cursor artifacts, adapted to Cline's format and directory. `AGENTS.md` is shared. |
| `REQ-003` | The authority hierarchy, SDD lifecycle, verification governance, documentation routing, and calibration workflow are Harness Assets — defined once, tool-agnostic, unchanged in principle. |
| `REQ-004` | The SDD methodology is a Harness Asset. The lifecycle, sizing model, phase ownership, and Definition of Done remain tool-agnostic. |
| `REQ-005` | A single authoritative documentation model is maintained. Rule content and skill content are authored once. No duplicate inventories, strategies, or governance models per tool. |
| `REQ-006` | Tool-specific optimizations (Cline's `.clineignore`, slash commands, Plan/Act mode) remain Tool Assets. They do not alter Harness methodology or governance content. |
| `REQ-007` | Rule guardrail text is a Harness Asset. File extension (`.mdc` vs `.md`) and frontmatter schema are Tool Assets. Content is separable from format. |
| `REQ-008` | Skill workflow procedures are a Harness Asset. Storage location (`.cursor/skills/` vs `.cline/skills/`) is a Tool Asset. Cross-skill references use skill names, not tool-specific paths. |
| `REQ-009` | `AGENTS.md` is a Shared Asset. Both Cursor and Cline auto-load it without conflict. Its content references tool-agnostic concepts with parenthetical tool-specific disambiguation. |
| `REQ-010` | Cline directories (`.clinerules/`, `.cline/`) do not conflict with Cursor directories (`.cursor/`). Adding Cline support does not modify, remove, or interfere with Cursor artifacts. |
| `REQ-011` | Historical SDD instances, verification artifacts, and pilot reports with `.cursor/` references are not rewritten. They remain interpretable as historical artifacts documenting what was done at the time. |

---

## Architectural Vision (Validated)

The Harness shall evolve into a platform whose governance is independent of the AI tool used to execute it.

The methodology, documentation, and governance remain authoritative. Each AI tool provides its own optimized implementation while preserving identical behavioral expectations. The separation between Harness Assets and Tool Assets becomes the fundamental architectural boundary for future evolution.

This vision is validated by the Research:

- **RH-3 (Validated):** Two tools can apply the same governance rules.
- **RH-4 (Validated):** Two tools can invoke the same workflow skills.
- **RH-5 (Validated):** The current Cursor implementation can be preserved without regression.
- **RH-8 (Validated):** An incremental transition is feasible.
- **RH-9 (Validated):** Rule content can be separated from rule format.
- **RH-10 (Validated):** Skill content can be separated from skill format.
- **RH-12 (Validated):** `AGENTS.md` bootstrap has an equivalent in Cline.
- **F-18:** The Harness governance methodology is tool-agnostic by design.

The vision does not require redesigning the Harness. It requires formalizing the boundary that already exists implicitly between content (Harness) and format (Tool).

---

## Current Architecture

The current Harness is a mature governance system implemented around Cursor as its sole AI tool.

### Current Layers

| Layer | Artifacts | Ownership (current) |
|-------|-----------|---------------------|
| Machine-facing rules | `.cursor/rules/*.mdc` (6 rules) | Cursor-specific format, path, and frontmatter |
| Machine-facing skills | `.cursor/skills/*/SKILL.md` (7 skills) | Cursor-specific path; format shared with Cline |
| Agent bootstrap | `AGENTS.md` | Shared (both tools auto-load), but content references `.cursor/` paths |
| Governance methodology | Authority hierarchy, SDD lifecycle, verification governance, documentation routing, calibration workflow | Tool-agnostic in principle, but documented with Cursor-specific path references |
| Documentation taxonomy | `Documentation/` structure established by ADR-003 | Tool-agnostic, but ADR-003 scopes `.cursor/` as the machine-facing artifact home |
| Harness strategies | `harness-architecture.md`, `rules-strategy.md`, `skills-strategy.md` | Titled and scoped to "Cursor rules" and "Cursor skills" |
| Rules inventory | `rules.md` | Titled "Cursor Rules Inventory" |
| Documentation index | `documentation-index.md` | "Cursor Artifacts" section |

### Current Assumptions That Are Impacted

The Research and Review identified 30 architectural assumptions (A-1 through A-30). The key impacted assumptions:

- **Path assumptions (A-1 to A-5):** `.cursor/` paths referenced in 18+ files as if they were the canonical Harness paths, not Cursor-specific implementation paths.
- **Rule loading assumptions (A-6 to A-10):** `.mdc` format, `alwaysApply`/`globs` frontmatter, and auto-injection are Cursor-specific.
- **Documentation reference assumptions (A-15 to A-20):** Titles, section names, and ADR-003 assume Cursor as the sole tool.
- **Context assumptions (A-27 to A-30):** `AGENTS.md` auto-loading is valid in both tools; `alwaysApply` injection and single-tool context are Cursor-specific.

### What Works Today

- The governance methodology is defined in tool-agnostic terms in principle.
- The skill format (`SKILL.md` with `name`/`description`) is identical between Cursor and Cline.
- `AGENTS.md` is auto-loaded by both tools.
- Rule and skill content is pure markdown — portable between tools.
- Cursor and Cline directories do not conflict.

---

## Target Architecture

### Transition Architecture vs Target Architecture

The Harness is currently at a transition point between two architectural states:

**Transition Architecture (Current):** Cursor is the Initial Reference Implementation. The canonical governance content exists within Cursor's Tool Assets (`.cursor/rules/*.mdc`, `.cursor/skills/*/SKILL.md`). Cursor artifacts serve a dual role: they are both Tool Assets (by format and location) and the de facto source of Harness Asset content. This reflects the current maturity of the Harness, not its permanent design.

**Target Architecture:** The Harness governance is independent of any specific AI tool. Harness Asset content (rule guardrail text, skill workflow procedures) is defined in tool-agnostic terms. Each tool — Cursor, Cline, and future tools — provides its own Tool Assets (format, frontmatter, directory) that project the same canonical governance content. No tool's artifacts serve as the permanent governance authority.

The Design proceeds through the Transition Architecture and explicitly aims for the Target Architecture. The transition is additive: Cursor remains fully operational throughout, and Cline support is added alongside it. The Target Architecture is an architectural objective; its full realization — where no single tool's projection is the de facto canonical source — is achieved when the governance content is independently definable without reference to any specific tool's format.

This distinction does not change the implementation plan, migration strategy, or tool-specific format decisions. It clarifies what is a transition property (Cursor artifacts as de facto canonical source) versus a target property (Harness Assets defined independently of tool implementations).

### Architectural Layers

The target architecture formalizes three layers:

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
| Authority hierarchy | ADRs > Architecture docs > State > SDD > Rules > Skills | Governance structure is tool-agnostic by design (F-18) |
| SDD lifecycle | Research → Specify → Design → Tasks → ... → Teacher Guide | Lifecycle is tool-agnostic by design (F-18) |
| Verification governance | Gate selection, review sensors, residual risk, skipped-gate reasoning | Verification model is tool-agnostic by design |
| Documentation routing | Path-drift checks, follow-up routing | Routing is a Harness process, not a tool mechanism |
| Calibration workflow | Pilot Execution → Pilot Report → Implementation Plan → Review → Phased Update → Consistency Audit → Next Pilot | Process is tool-agnostic (RH-14 Validated) |
| Rule scoping intent | Whether a rule is always-on or file-scoped | The *intent* is governance; the *expression* is tool-specific |
| Context loading strategy | What context to load for each task type | The *strategy* is governance; the *mechanism* is tool-specific |
| Knowledge transfer strategy | Teacher Guides, concept classification, study paths | Tool-agnostic by design |
| Artifact ownership model | Who owns what truth | Tool-agnostic by design |
| Adaptive sizing model | Small/Medium/Large/Complex | Tool-agnostic by design |
| ADR governance policy | Creation criteria, conflict resolution, escalation | Tool-agnostic by design |
| Definition of Done | Completion criteria for SDD-backed work | Tool-agnostic by design |

**Ownership responsibilities:** Harness Assets are maintained in `Documentation/AI-Harness/Harness-Design/` and in the content of rules and skills. Changes to Harness Assets require governance review (calibration workflow or ADR evaluation depending on durability). Harness Assets must not reference tool-specific paths as if they were canonical — they reference tool-agnostic concepts.

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

**Ownership responsibilities:** Tool Assets are maintained in each tool's respective directory (`.cursor/` for Cursor, `.clinerules/` + `.cline/` for Cline). Changes to Tool Assets must not alter Harness Asset content. Tool Assets are implementation details, not governance authorities.

#### Shared Assets

Shared Assets are artifacts that are both Harness-owned in content and consumed identically by all tools. They sit at the intersection of Harness and Tool layers.

| Asset | Description | Why it is shared |
|------|-------------|------------------|
| `AGENTS.md` | Cross-tool bootstrap file | Both Cursor and Cline auto-load it identically (F-3, RH-12). Content is Harness-authored; loading is tool-native. |

**Ownership responsibilities:** `AGENTS.md` content is a Harness Asset (authored by governance, referencing tool-agnostic concepts). Its loading mechanism is native to each tool. Changes to `AGENTS.md` must preserve its role as a shared bootstrap — it must not become tool-specific in content.

---

## AI Harness Implementation Model

### What Constitutes an AI Harness Implementation?

An AI Harness implementation is a **tool-specific projection of the Harness's canonical governance content into a tool's native format, directory, and loading mechanism**.

Formally, an implementation consists of:

1. **Rule projections** — Each Harness rule content is expressed in the tool's file format, frontmatter schema, and storage directory. The guardrail text is identical; the format is tool-native.

2. **Skill projections** — Each Harness skill content is placed in the tool's skill directory using the tool's `SKILL.md` format. The workflow procedures are identical; the storage location is tool-native.

3. **Bootstrap consumption** — The tool auto-loads `AGENTS.md` as the shared entry point.

4. **Context management** — The tool applies its native context loading mechanisms to achieve the Harness's context loading strategy (what to load, when to load).

5. **Tool-specific optimizations** — The tool may offer capabilities not present in other tools (e.g., Cline's `.clineignore`, slash commands). These are Tool Assets that complement but do not alter Harness governance.

### Implementation Relationship Model

```
Canonical Governance Content (Harness Assets)
         │
         │ projected into
         │
    ┌────┴────┐
    │         │
  Cursor    Cline
  Impl      Impl
    │         │
    │         ├── .clinerules/*.md (rule projections)
    │         ├── .cline/skills/*/SKILL.md (skill projections)
    │         ├── AGENTS.md (shared bootstrap)
    │         └── .clineignore (tool-specific optimization)
    │
    ├── .cursor/rules/*.mdc (rule projections)
    ├── .cursor/skills/*/SKILL.md (skill projections)
    └── AGENTS.md (shared bootstrap)
```

### Canonical Content Principle

**Content is canonical; format is tool-specific.** This principle means:

- There is one authoritative version of each rule's guardrail text.
- There is one authoritative version of each skill's workflow procedures.
- Each tool consumes that content through its own format, frontmatter, and directory.
- The adaptation mechanism (how content moves from canonical to tool-specific) is an implementation concern, not a governance concern.

**Canonical Governance is an architectural concept, not a physical layer.** The Design intentionally avoids introducing a separate physical canonical repository or directory. Instead, "canonical" refers to the governance-level truth that all Tool Assets must faithfully reproduce. During the transition, the Cursor projection serves as the de facto canonical source. The Target Architecture aims for governance content to be independently definable, at which point the canonical source is the Harness governance documentation itself (e.g., the Rules Inventory in `Documentation/AI-Harness/rules.md`). This preserves simplicity while enabling future evolution — a physical canonical representation may be introduced later if maintenance complexity justifies it.

**Transition-period conflict resolution:** During the transition, the Cursor implementation remains the implementation baseline. Harness governance documentation progressively becomes the canonical governance source. If a discrepancy is discovered between Cursor artifacts and Harness governance documentation, it shall be explicitly resolved rather than silently propagated into new tool projections. Discrepancies shall be resolved during Cline artifact creation (Phase 3) and documented in implementation reports.

This principle is superior to alternatives because:

- It preserves backward compatibility (Cursor's existing `.mdc` files are not touched).
- It enables Cline support without requiring a format that both tools must parse.
- It avoids the complexity of a canonical directory with adapters or generation scripts.
- It keeps each tool's implementation native to that tool's ecosystem.

### Runtime Behavior

Runtime behavior is tool-specific. The Harness defines **behavioral expectations** (what governance should be applied), not **runtime mechanics** (how the tool loads and applies them). As long as the behavioral expectations are met, the runtime mechanism is a Tool Asset.

| Behavioral expectation | Cursor runtime | Cline runtime |
|------------------------|---------------|---------------|
| Always-on rules are present in agent context | `alwaysApply: true` rules auto-injected | No-frontmatter rules loaded as context |
| File-scoped rules apply when relevant files are touched | `globs` pattern matching at load time | `paths` pattern matching dynamically during task |
| Skills are available on-demand | User/agent triggers by name in interface | Description-based auto-matching or slash command |
| `AGENTS.md` is loaded at session start | Auto-loaded as project instructions | Auto-detected as rule type |

The behavioral expectations are identical; the runtime mechanisms differ. This is acceptable and expected (AP-004).

---

## Repository Architecture

### Ownership Model

The repository contains three categories of artifacts, distinguished by ownership:

| Category | Location | Owner | Examples |
|----------|----------|-------|----------|
| Harness Assets (governance docs) | `Documentation/AI-Harness/Harness-Design/` | Harness governance | `harness-architecture.md`, `rules-strategy.md`, `skills-strategy.md`, `sdd-operational.md` |
| Harness Assets (rule/skill content) | Canonical content is expressed in each tool's projections | Harness governance | Guardrail text, workflow procedures |
| Shared Assets | Repository root | Harness governance (content), all tools (loading) | `AGENTS.md` |
| Tool Assets (Cursor) | `.cursor/` | Cursor implementation | `.cursor/rules/*.mdc`, `.cursor/skills/*/SKILL.md` |
| Tool Assets (Cline) | `.clinerules/`, `.cline/` | Cline implementation | `.clinerules/*.md`, `.cline/skills/*/SKILL.md`, `.clineignore` |
| Product documentation | `Documentation/` (per ADR-003 taxonomy) | Product/architecture governance | PRD, PM, Architecture, Technical, ADRs, State |

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

**The relationship is asymmetric:** Harness governance defines what governance should be; tool implementations provide how it is loaded and applied. Tool implementations serve the methodology, not the reverse (AP-002).

### What Does NOT Change

- `Documentation/` taxonomy remains as established by ADR-003 (generalized for multi-tool).
- `.cursor/` remains the Cursor implementation directory.
- `AGENTS.md` remains at the repository root.
- Product code (`DocAPI/`, `DocFront.Web/`) is untouched.

---

## Evolution Strategy

The evolution from single-tool to multi-tool is **additive, not transformative**. The Harness does not move to a new directory, adopt a new format, or introduce a new abstraction layer. Instead, it:

1. **Formalizes the existing boundary** between content (Harness) and format (Tool) that already exists implicitly.
2. **Adds Cline-specific directories** alongside existing Cursor directories.
3. **Updates governance documentation** to reference tool-agnostic concepts by default.
4. **Renames tool-specific titles** to tool-agnostic names.
5. **Revises ADR-003** to generalize the taxonomy for multi-tool artifacts.
6. **Recommends a new ADR** to establish the Harness Asset / Tool Asset / Shared Asset taxonomy.

Each step preserves Cursor workflows without regression. The evolution can proceed in phases, with each phase independently verifiable.

---

## Documentation Taxonomy

### Decision

The documentation taxonomy established by ADR-003 is **generalized** for multi-tool artifacts. The `Documentation/` structure remains tool-agnostic and authoritative. Tool-specific directories (`.cursor/`, `.clinerules/`, `.cline/`) are referenced as implementation details, not as governance locations.

### Taxonomy Structure

| Location | Purpose | Ownership |
|----------|---------|-----------|
| `Documentation/Product/` | PRD, PM, product planning | Product governance |
| `Documentation/Architecture/` | Architecture, domain, ERD, ADRs | Architecture governance |
| `Documentation/Technical/` | Runbooks, migration plans, technical references | Technical governance |
| `Documentation/AI-Harness/` | Harness design, SDD templates, review prompts, research | Harness governance |
| `Documentation/State.md` | Current operational state | Operational governance |
| `Documentation/SDD/` | Feature SDD instances | Feature governance |
| `AGENTS.md` | Shared cross-tool bootstrap | Harness governance (content), all tools (loading) |
| `.cursor/` | Cursor-specific machine-facing artifacts | Cursor implementation |
| `.clinerules/` | Cline-specific rules | Cline implementation |
| `.cline/` | Cline-specific skills and configuration | Cline implementation |

### Generalization Principle

Governance documentation under `Documentation/AI-Harness/` references **tool-agnostic concepts** by default. When a tool-specific implementation detail is needed, it is provided in parenthetical disambiguation:

> "Harness rules (Cursor: `.cursor/rules/*.mdc`; Cline: `.clinerules/*.md`) are loaded as persistent context."

This pattern is used consistently across all governance docs, strategies, templates, and contribution guides.

---

## Rule Architecture

### Decision

Rule content is a Harness Asset. Rule format (file extension, frontmatter schema) and rule storage location are Tool Assets. Each tool consumes the same rule content through its own format.

### Rule Content (Harness Asset)

The guardrail text of each rule is authored once and is identical across all tools. The six current rules and their content:

| Rule | Scoping intent | Guardrail text |
|------|---------------|----------------|
| `security-phi` | Always-on | PHI, secrets, credentials, logging restrictions |
| `token-economy` | Always-on | State/AGENTS loading, targeted reads, markdown handoff |
| `update-doc` | Always-on | Update State for changes; ADR governance for durable decisions |
| `backend-architecture` | File-scoped (`DocAPI/**/*.cs`) | Backend CA, ubiquitous language, repository boundaries |
| `ef-migrations` | File-scoped (SQL DB and migrations) | EF migration rules, soft delete, owned types, secret safety |
| `blazor-front` | File-scoped (`DocFront.Web/**/*`) | Blazor Server conventions, data flow, API URL |

### Rule Format (Tool Asset)

| Property | Cursor | Cline |
|----------|--------|-------|
| File extension | `.mdc` | `.md` |
| Directory | `.cursor/rules/` | `.clinerules/` |
| Always-on frontmatter | `alwaysApply: true` | No frontmatter (absence = always-on) |
| File-scoped frontmatter | `globs: pattern` | `paths: [pattern]` |
| Description field | `description: text` | Not used (Cline has no `description` field for rules) |

### Scoping Intent Mapping

The Harness defines scoping **intent** (always-on vs file-scoped). Each tool expresses that intent through its own frontmatter:

| Harness scoping intent | Cursor expression | Cline expression |
|------------------------|-------------------|------------------|
| Always-on | `alwaysApply: true` (no `globs`) | No frontmatter |
| File-scoped | `alwaysApply: false` + `globs: pattern` (or `globs` alone) | `paths: [pattern]` |
| Always-on + file-scoped (combined) | `alwaysApply: true` + `globs: pattern` | No direct equivalent — see below |

**Combined always-on + file-scoped:** The Research (RH-11) identified that `alwaysApply: true` + `globs` has no direct Cline equivalent. In Cline, a rule is either always-on (no frontmatter) or conditional (`paths`). The Design decision is: **if a rule is always-on, the `globs` scoping is redundant in Cline** — the rule applies regardless of file context. Therefore, the Cline projection of an always-on + file-scoped rule is simply an always-on rule (no frontmatter). The file-scoping in Cursor is an optimization that narrows when the rule is injected; in Cline, the rule is always present, which is a superset of the intended behavior. This does not alter governance — the rule's guardrail text is identical; only the activation timing differs.

### Rule Design Standard (Unchanged)

The rule design standard from `rules-strategy.md` remains unchanged:

1. Short enough to load frequently.
2. States behavior, not background.
3. Points to a canonical doc for deeper context.
4. Has a clear file scope or is genuinely always-on.
5. Protects a real project risk.
6. Avoids duplicating a skill, review prompt, or SDD template.

---

## Skill Architecture

### Decision

Skill content is a Harness Asset. Skill storage location is a Tool Asset. Skill format (`SKILL.md` with `name`/`description`) is shared between Cursor and Cline — it is a Tool Asset that happens to be identical.

### Skill Content (Harness Asset)

The workflow procedures in each `SKILL.md` body are authored once and are identical across all tools. The seven current skills:

| Skill | Purpose |
|-------|---------|
| `doc-organo-context` | Session orientation |
| `codebase-decomposition` | DDD and technical debt analysis |
| `effective-harness-planning` | Harness review and planning |
| `verifier` | Verification workflow |
| `documentation-update` | Documentation routing and Follow-Up |
| `sql-migration-workflow` | SQL migration workflow |
| `not-a-teacher` | Teacher Guide generation |

### Skill Format (Shared Tool Asset)

| Property | Cursor | Cline |
|----------|--------|-------|
| File name | `SKILL.md` | `SKILL.md` (identical) |
| Frontmatter | `name` + `description` | `name` + `description` (identical) |
| Directory structure | One directory per skill | One directory per skill (identical) |
| Storage location | `.cursor/skills/` | `.cline/skills/` |
| Discovery | By `name`/`description` | By `name`/`description` (identical) |
| Invocation | User/agent triggers by name | Description-based auto-matching or slash command |

### Cross-Skill Reference Strategy

**Decision:** Cross-skill references use **skill names**, not tool-specific paths.

| Pattern | Example |
|---------|---------|
| Name-based reference (preferred) | "See the `documentation-update` skill" |
| Name + parenthetical disambiguation (when path is needed) | "The `verifier` skill (`.cursor/skills/verifier/SKILL.md` in Cursor, `.cline/skills/verifier/SKILL.md` in Cline)" |
| Avoided: tool-specific path only | "See `.cursor/skills/documentation-update/SKILL.md`" |

**Rationale:** Skill names are tool-agnostic and stable. Paths differ per tool. Name-based references work across all tools and do not break when a tool changes its directory structure. This resolves DDA-008 and Research R9.

---

## Path Reference Strategy

### Decision

Governance documentation references **tool-agnostic concepts by default**, with **tool-specific paths in parenthetical disambiguation** when implementation detail is needed.

### Reference Patterns

| Context | Pattern | Example |
|---------|---------|---------|
| Governance concept (default) | Tool-agnostic concept name | "Harness rules" |
| Implementation detail needed | Concept + parenthetical paths | "Harness rules (Cursor: `.cursor/rules/*.mdc`; Cline: `.clinerules/*.md`)" |
| Single-tool context | Tool-specific path is acceptable | "Cursor loads rules from `.cursor/rules/*.mdc`" |
| Historical artifact | No change — preserve original reference | SDD instances with `.cursor/` references remain as-is |

### The 43+ `.cursor/` References

The 43+ `.cursor/` references in 18+ governance files are addressed as follows:

| Reference type | Treatment |
|---------------|-----------|
| References that describe Harness concepts (e.g., "rules are loaded from `.cursor/rules/`") | Updated to tool-agnostic concept + parenthetical disambiguation |
| References that describe Cursor-specific implementation (e.g., "Cursor uses `alwaysApply: true`") | Kept as-is — they are accurately Cursor-specific |
| References in historical SDD instances, pilot reports, verification artifacts | Not changed — historical artifacts remain as-is (REQ-011, RH-13) |
| References in `AGENTS.md` | Updated to tool-agnostic concept + parenthetical disambiguation |
| References in templates | Updated to tool-agnostic concept + parenthetical disambiguation |

### Path Drift Detection

The `documentation-update` skill's path drift check is extended to detect **tool-specific path references that should be tool-agnostic**. Specifically, it flags `.cursor/` references in governance docs that describe Harness concepts (not Cursor-specific implementation details). This is a skill content update, not a governance change.

---

## Frontmatter Strategy

### Decision

The Harness maintains **tool-specific frontmatter**. No cross-tool frontmatter schema is defined.

### Rationale

A cross-tool schema would require both tools to support fields they do not natively use:

- Cursor uses `alwaysApply` (boolean) and `globs` (glob pattern). Cline has no `alwaysApply` field and uses `paths` instead of `globs`.
- Cline uses `paths` (array of glob patterns). Cursor has no `paths` field.
- A canonical schema with both fields would be ignored by each tool for the fields it does not recognize, creating silent failures.

Tool-specific frontmatter is the natural expression of each tool's loading mechanism. The Harness defines **scoping intent** (always-on vs file-scoped); each tool expresses that intent through its own frontmatter.

### Frontmatter Mapping

| Harness scoping intent | Cursor frontmatter | Cline frontmatter |
|------------------------|--------------------|--------------------|
| Always-on | `---\nalwaysApply: true\ndescription: ...\n---` | (no frontmatter — file body starts immediately) |
| File-scoped | `---\nalwaysApply: false\nglobs: "DocAPI/**/*.cs"\ndescription: ...\n---` | `---\npaths:\n  - "DocAPI/**/*.cs"\n---` |
| Always-on + file-scoped | `---\nalwaysApply: true\nglobs: "DocAPI/**/*.cs"\ndescription: ...\n---` | (no frontmatter — always-on; glob scoping is redundant) |

### Tool Metadata Ownership

Tool metadata — frontmatter schemas, `description` fields, file extensions, and storage locations — are **Tool Assets** owned by each tool's implementation. The Harness defines behavioral expectations (what a rule governs, when it should be active), not metadata format. Each tool follows its native conventions for how metadata is expressed.

#### `description` Field

Cursor supports a `description` field in rule frontmatter. Cline does not use a `description` field for rules (it uses `name`/`description` only for skills).

The `description` field is a Tool Asset — it is metadata whose format and usage follow each tool's native conventions. If the `description` content carries governance-significant information (e.g., the rule's purpose statement), that information should be preserved in the Cline projection — for example, as a comment or markdown heading at the top of the rule body. If the `description` is purely Cursor metadata with no governance significance, it may be freely omitted from Cline projections. The distinction between governance content and tool metadata is made per-rule during the Tasks phase.

---

## AGENTS.md Strategy

### Decision

`AGENTS.md` remains the **shared cross-tool bootstrap artifact**. Its content is updated to reference tool-agnostic concepts by default, with parenthetical disambiguation for tool-specific paths.

### Current State

`AGENTS.md` currently references `.cursor/rules/` and `.cursor/skills/` as the machine-facing asset locations. In a multi-tool context, these references are ambiguous — they describe both "where Cursor artifacts live" and "where Harness artifacts live" as if they were the same thing.

### Target State

`AGENTS.md` references are updated to:

| Current reference | Target reference |
|-------------------|------------------|
| `.cursor/rules/` | "Harness rules (Cursor: `.cursor/rules/`; Cline: `.clinerules/`)" |
| `.cursor/skills/` | "Harness skills (Cursor: `.cursor/skills/`; Cline: `.cline/skills/`)" |
| "Cursor rules" | "Harness rules" |
| "Cursor skills" | "Harness skills" |

### What Does NOT Change in AGENTS.md

- The authority hierarchy section remains tool-agnostic.
- The repository map remains structurally accurate.
- The commands section remains unchanged.
- The harness workflow section remains tool-agnostic.
- The operational constraints section remains unchanged.
- The verification section remains unchanged.

### Loading Behavior

| Tool | Loading behavior | Classification |
|------|-----------------|----------------|
| Cursor | Auto-loads `AGENTS.md` as project instructions | Shared Asset |
| Cline | Auto-detects `AGENTS.md` as a rule type for cross-tool compatibility | Shared Asset |

Both tools load `AGENTS.md` without conflict. Cline treats it as a toggleable rule; Cursor treats it as project instructions. The loading mechanism differs, but the effect (bootstrap context is available) is identical.

---

## Context Management Strategy

### Decision

The Harness's context loading **strategy** (what to load, when to load) is a Harness Asset. The **mechanism** (how to load, what tool features to use) is a Tool Asset. Tool-specific context management features remain Tool Assets — they complement but do not alter the Harness strategy.

### Harness Context Loading Strategy (Unchanged)

The context loading policy from `harness-architecture.md` remains:

- **Always load:** `Documentation/State.md`, `AGENTS.md`, always-on security/PHI and token economy rules.
- **Load by task:** Domain work, SQL/EF migration, frontend work, product planning, harness planning, legacy behavior.
- **Avoid loading by default:** Full Legacy folders, full research chats, large methodology docs in implementation sessions, financial domain files.

### Tool-Specific Context Mechanisms

| Mechanism | Tool | Classification | Treatment |
|-----------|------|----------------|-----------|
| `token-economy` rule | Cursor | Harness Asset (content) + Tool Asset (mechanism) | Existing — unchanged |
| `.clineignore` | Cline | Tool Asset (Cline-specific) | Documented as a recommended Cline optimization; not a Harness governance requirement. `.clineignore` is an optional optimization artifact whose contents should be determined during implementation based on observed context-loading behavior. It is not part of canonical Harness governance. Different projects may legitimately require different `.clineignore` contents. |
| `/smol`, `/newtask` | Cline | Tool Asset (Cline-specific) | Documented as Cline context management commands; not adopted as Harness methodology |
| Memory Bank | Cline (tool-agnostic methodology) | Not adopted | The Harness's `State.md` + durable documentation pattern achieves the same goal; Memory Bank is not required (Assumption 9) |
| Plan/Act mode | Cline | Tool Asset (Cline-specific) | Complements the SDD lifecycle; not adopted as a Harness phase |
| `/deep-planning` | Cline | Tool Asset (Cline-specific) | Conceptually related to SDD Plan phase; not adopted as a Harness command |

### Adoption Principle

Tool-specific context management features are **encouraged as implementation optimizations** (AP-005) but are **not adopted as Harness governance**. The Harness defines what context should be loaded; each tool provides how. A developer using Cline may use `.clineignore` and `/smol` for context management; a developer using Cursor relies on the `token-economy` rule and manual context selection. Both are valid implementations of the same context loading strategy.

---

## Documentation Naming Strategy

### Decision

Tool-specific titles in governance documentation are **renamed to tool-agnostic names**.

| Current name | Target name | Rationale |
|--------------|-------------|-----------|
| "Cursor Rules Inventory" (`rules.md`) | "Rules Inventory" | The inventory covers all tool projections, not just Cursor |
| "Cursor Artifacts" section (`documentation-index.md`) | "Machine-Facing Artifacts" | The section covers all tool implementations |
| "Cursor rules" (`rules-strategy.md` title) | "Harness Rules" | The strategy covers rule governance, not Cursor mechanics |
| "Cursor skills" (`skills-strategy.md` title) | "Harness Skills" | The strategy covers skill governance, not Cursor mechanics |
| "Cursor rules/skills" (in `harness-architecture.md`) | "Harness rules/skills" | The architecture covers Harness governance |

### What Is NOT Renamed

- Tool-specific implementation details within docs (e.g., "Cursor uses `alwaysApply: true`") — these are accurately tool-specific.
- Historical artifacts (SDD instances, pilot reports) — these are not changed (REQ-011).
- ADR-003 — this is revised, not renamed.

---

## Migration Strategy

### Architectural Migration Phases

The migration from Cursor-only to multi-tool support proceeds in four architectural phases. Each phase preserves Cursor workflows without regression and is independently verifiable.

#### Phase 1: Governance Documentation Generalization

**Scope:** Update governance documentation to reference tool-agnostic concepts by default. Rename tool-specific titles. Update `AGENTS.md` references.

**Architectural checkpoint:** All governance docs under `Documentation/AI-Harness/` reference tool-agnostic concepts. `.cursor/` references that describe Harness concepts are updated to parenthetical disambiguation. Historical artifacts are not touched.

**Compatibility guarantee:** Cursor workflows are unaffected — no `.cursor/` files are modified. Documentation changes do not alter runtime behavior.

**Rollback principle:** Revert documentation changes. No structural or runtime impact.

#### Phase 2: ADR Governance

**Scope:** Revise ADR-003 to generalize the documentation taxonomy for multi-tool artifacts. Create a new ADR establishing the Harness Asset / Tool Asset / Shared Asset taxonomy.

**Architectural checkpoint:** ADR-003 revision is accepted. New ADR is accepted. ADR governance policy is followed.

**Compatibility guarantee:** ADR changes are governance decisions. They do not modify runtime artifacts.

**Rollback principle:** ADRs can be superseded by future ADRs. ADR governance is reversible by design.

#### Phase 3: Cline Artifact Creation

**Scope:** Create `.clinerules/` directory with Cline-formatted rule files (`.md` with `paths` frontmatter or no frontmatter). Create `.cline/skills/` directory with `SKILL.md` files (identical content to Cursor skills). Create `.clineignore` if desired.

**Architectural checkpoint:** Cline can load rules from `.clinerules/`, invoke skills from `.cline/skills/`, and bootstrap from `AGENTS.md`. Rule content matches Cursor rule content. Skill content matches Cursor skill content.

**Compatibility guarantee:** Cursor artifacts are not modified, moved, or removed. `.clinerules/` and `.cline/` do not conflict with `.cursor/`.

**Rollback principle:** Remove `.clinerules/` and `.cline/` directories. Cursor workflows are unaffected. (Research Q-23 confirmed this rollback.)

#### Phase 4: Skill and Rule Refinement

**Scope:** Update the `documentation-update` skill's path drift check to detect tool-specific path references that should be tool-agnostic. Update the `effective-harness-planning` skill to evaluate tool-specific assumptions during harness reviews. Update pilot report templates to include multi-tool fields.

**Architectural checkpoint:** Path drift check flags `.cursor/` references in governance docs that describe Harness concepts. Harness planning skill evaluates tool-specific assumptions. Pilot report template includes "tool used" and "observations" fields.

**Compatibility guarantee:** Skill updates are content changes within existing skill files. They do not alter the skill format or loading mechanism.

**Rollback principle:** Revert skill content changes. No structural impact.

### Compatibility Guarantees

| Guarantee | How it is preserved |
|-----------|---------------------|
| Cursor rule loading | `.cursor/rules/*.mdc` files are not modified, moved, or removed |
| Cursor skill invocation | `.cursor/skills/*/SKILL.md` files are not modified, moved, or removed |
| `AGENTS.md` bootstrap | `AGENTS.md` remains at repository root; both tools auto-load it |
| Historical artifact readability | SDD instances, pilot reports, and verification artifacts with `.cursor/` references are not rewritten |
| Governance methodology | Authority hierarchy, SDD lifecycle, verification governance, documentation routing, and calibration workflow are unchanged in principle |

### Rollback Principles

1. **Documentation changes** (Phase 1, 4) are revertible via version control. No runtime impact.
2. **ADR changes** (Phase 2) are reversible through ADR governance (supersession by a new ADR).
3. **Cline artifact creation** (Phase 3) is reversible by removing `.clinerules/` and `.cline/` directories. Cursor is unaffected.
4. **No phase creates irreversible state.** The architecture is additive — it adds Cline support without removing or transforming Cursor support.

---

## Design Decision Area Resolution

### DDA-001: Harness Assets vs Tool Assets

**Decision:** Formalize the three-layer taxonomy (Harness Assets, Tool Assets, Shared Assets) as the fundamental architectural boundary. The boundary is expressed through ownership classification and documentation conventions, not through a shared directory or adapter layer.

**Alternatives considered:**

| Alternative | Description | Why rejected |
|-------------|-------------|--------------|
| Shared canonical directory | A single directory (e.g., `.harness/rules/`) with tool-specific adapters | Adds a new directory and adapter mechanism that neither tool natively loads. Requires generation or symlink logic. Violates AP-007 (avoid unnecessary abstraction). Both tools would need to be reconfigured to read from the new location. |
| Single file serving both tools | One file with both `alwaysApply`/`globs` and `paths` frontmatter | Each tool would ignore fields it does not recognize, creating silent failures. Cline would parse `alwaysApply` as unknown frontmatter and treat the rule as conditional (not always-on). |
| No formal boundary | Leave the boundary implicit as it is today | The 43+ `.cursor/` references demonstrate that an implicit boundary leads to documentation ambiguity. Formalizing the boundary makes future evolution easier, not harder. |

**Rationale:** The Research validated that content is portable (RH-9, RH-10) and that directories do not conflict (F-15). The ownership classification is a conceptual boundary, not a physical one. It does not require new directories, adapters, or generation scripts. It requires only that governance documentation distinguish between Harness-owned concepts and tool-owned implementations.

**Consequences:**

- Governance documentation must distinguish Harness concepts from Tool implementations.
- Each tool's projections must faithfully reproduce canonical content.
- The boundary is enforceable through documentation review and path drift detection, not through technical mechanisms.

**Risks:**

- Content drift between tool projections if the adaptation mechanism is manual and infrequent.
- Mitigation: The `documentation-update` skill's path drift check is extended to detect content inconsistencies.

**Impact on existing Harness:** The existing `.cursor/` artifacts are reclassified as Tool Assets (Cursor implementation). Their content is reclassified as Harness Assets. No files are moved or modified — only the conceptual ownership is formalized.

---

### DDA-002: ADR Strategy

**Decision:** Revise ADR-003 to generalize the documentation taxonomy for multi-tool artifacts. Recommend a new ADR to establish the Harness Asset / Tool Asset / Shared Asset taxonomy as a permanent architectural boundary.

**Alternatives considered:**

| Alternative | Description | Why rejected |
|-------------|-------------|--------------|
| Accommodate ADR-003 (no modification) | Add Cline directories alongside `.cursor/` without modifying ADR-003 | Leaves the 43+ `.cursor/` references ambiguous. ADR-003 establishes `.cursor/` as the home for "machine-facing Cursor artifacts" but does not address multi-tool artifacts. Accommodation preserves the ambiguity that this feature is designed to resolve. |
| Supersede ADR-003 with a new ADR | Replace ADR-003 entirely with a new ADR establishing a tool-agnostic artifact layer | Loses the historical context of ADR-003's decision. ADR-003's core decision (`.cursor/` for Cursor artifacts) remains valid and should be preserved. Supersession is appropriate only when the original decision is wrong, not when it is incomplete. |
| No ADR (operational update) | Treat multi-tool support as an operational update within ADR-003's scope | The Harness Asset / Tool Asset / Shared Asset taxonomy is a durable architectural boundary that future developers and agents must understand. It meets ADR creation criteria: it changes documentation taxonomy, establishes cross-context ownership, and affects future architecture guidance. |

**Rationale:** Revision preserves ADR-003's core decision (`.cursor/` for Cursor artifacts) while generalizing the principle: `Documentation/` is the tool-agnostic home for durable documentation, and each tool has its own machine-facing directory. A new ADR establishes the ownership taxonomy as a permanent boundary, which is a durable decision that meets ADR governance criteria.

**Consequences:**

- ADR-003 is revised (status changes from "Accepted" to "Accepted (Revised)" or a new revision is created per ADR governance).
- A new ADR is created for the Harness Asset / Tool Asset / Shared Asset taxonomy.
- Future ADRs may reference the taxonomy as established architecture.

**Risks:**

- ADR revision process may surface disagreements about the taxonomy boundaries.
- Mitigation: The taxonomy is validated by Research (§13.2) and Review (§Architectural Ownership Classification). The evidence is strong.

**Impact on existing Harness:** ADR-003's decision is generalized, not reversed. `.cursor/` remains the Cursor implementation directory. `Documentation/` remains the canonical documentation home. The new ADR formalizes what the Research and Review already validated.

---

### DDA-003: Tool-Specific Loading Mechanisms

**Decision:** Maintain tool-specific loading mechanisms with shared content. No canonical format. Each tool consumes the same governance content through its own native loading mechanism.

**Alternatives considered:**

| Alternative | Description | Why rejected |
|-------------|-------------|--------------|
| Canonical format with generation | Define a canonical format and generate tool-specific files | Requires a generation script or build step. Adds complexity (AP-007). The canonical format would need to be a new format that neither tool natively loads, requiring generation for every tool. |
| Canonical format = Cursor format | Use `.mdc` with `alwaysApply`/`globs` as canonical; Cline adapts | Cline does not document `.mdc` support (F-5). Making Cursor's format canonical would privilege one tool's format as the standard, violating the principle that AI tools are implementations of the Harness (AP-002). |
| Canonical format = Cline format | Use `.md` with `paths` as canonical; Cursor adapts | Cursor does not support `paths` frontmatter. Would break Cursor backward compatibility (REQ-001). |

**Rationale:** Tool-specific formats with shared content is the simplest approach that preserves backward compatibility and enables Cline support. The Research (RH-2) confirmed that content duplication is avoidable in principle. The loading mechanisms can coexist (F-15). The adaptation mechanism (how content moves from canonical to tool-specific) is an implementation concern for the Tasks phase.

**Consequences:**

- Each tool has its own set of rule files and skill files.
- Content must be kept in sync across tool projections.
- The adaptation mechanism is a Tasks-phase decision (manual copy, generation script, or other).

**Architectural requirement — Semantic Equivalence:** Tool-specific implementations shall remain semantically equivalent. Any synchronization mechanism adopted during implementation shall preserve semantic equivalence between Tool Assets. This property is defined by the Design; how it is achieved belongs to Tasks.

Semantic equivalence means preserving the following across all tool projections, independently of tool-specific implementation details such as directories, frontmatter, file extensions, and loading mechanisms:

- **Governance intent:** The purpose and authority of each rule and skill — what it governs, why it exists, and where it fits in the authority hierarchy.
- **Guardrail behavior:** Each rule's guardrail text produces the same behavioral constraint regardless of which tool applies it. The constraint on developer or agent behavior is identical.
- **Workflow behavior:** Each skill's workflow procedures produce the same process outcome regardless of which tool invokes it. The sequence, decision points, and expected outputs are identical.
- **Behavioral expectations:** The runtime behavior expectations (which rules are always active, which are file-scoped, which skills are available) are preserved according to the Harness's scoping intent and context loading strategy, even though the tool-specific mechanisms that fulfill those expectations differ.

**Risks:**

- Content drift between tool projections if synchronization is infrequent.
- Mitigation: Path drift detection in `documentation-update` skill; governance review during calibration workflow.

**Impact on existing Harness:** Cursor's `.mdc` files remain unchanged. Cline's `.md` files are new additions. No existing loading mechanism is altered.

---

### DDA-004: Frontmatter Strategy

**Decision:** Maintain tool-specific frontmatter. No cross-tool frontmatter schema.

**Alternatives considered:**

| Alternative | Description | Why rejected |
|-------------|-------------|--------------|
| Cross-tool schema with both fields | Define a schema with `alwaysApply`, `globs`, and `paths` | Each tool would ignore unknown fields. Cline would parse `alwaysApply` as unknown frontmatter, making the rule conditional instead of always-on. Silent failures. |
| Canonical frontmatter with mapping | Define a canonical schema and map to tool-specific frontmatter | Requires a mapping layer. Adds complexity (AP-007). The mapping would need to handle the `alwaysApply: true` + `globs` combined case, which has no Cline equivalent. |
| No frontmatter (all always-on) | Make all rules always-on in all tools | Loses file-scoping optimization. Backend, EF, and Blazor rules would be loaded in all contexts, increasing token cost. Violates the rule design standard (clear file scope). |

**Rationale:** Tool-specific frontmatter is the natural expression of each tool's loading mechanism. The Harness defines scoping intent (always-on vs file-scoped); each tool expresses that intent through its own frontmatter. The mapping is straightforward for all cases except `alwaysApply: true` + `globs`, which is resolved by treating the Cline projection as always-on (the glob scoping is redundant when the rule is always present).

**Consequences:**

- Rule files in each tool have different frontmatter.
- The scoping intent mapping must be documented and followed during content adaptation.
- The `description` field is Cursor-specific; Cline projections omit it or include it as a comment.

**Risks:**

- Incorrect frontmatter mapping could change rule activation behavior.
- Mitigation: The mapping table is documented in this Design. The Tasks phase should include a verification step for frontmatter correctness.

**Impact on existing Harness:** Cursor frontmatter is unchanged. Cline frontmatter is new. No existing frontmatter is altered.

---

### DDA-005: Path Reference Strategy

**Decision:** Governance docs reference tool-agnostic concepts by default, with tool-specific paths in parenthetical disambiguation when implementation detail is needed. Historical artifacts are not rewritten.

**Alternatives considered:**

| Alternative | Description | Why rejected |
|-------------|-------------|--------------|
| Tool-specific paths only | Keep `.cursor/` references as-is; add `.clinerules/` references where needed | Creates ambiguity in a multi-tool context. A reference to `.cursor/rules/` in a governance doc could mean "Cursor's rules" or "the Harness's rules (implemented in Cursor)." This is the ambiguity this feature is designed to resolve. |
| Tool-agnostic abstractions only | Replace all `.cursor/` references with abstract concepts ("Harness rules") with no path detail | Loses implementation specificity. A developer or agent who needs to find the actual rule files would not know where to look. |
| Mass rewrite of all references | Update all 43+ references including historical SDD instances | Violates REQ-011 and RH-13. Historical artifacts should not be rewritten to reflect a multi-tool architecture they predate. |

**Rationale:** The "both with disambiguation" approach provides tool-agnostic clarity for governance concepts while preserving tool-specific detail for implementation guidance. It is incremental — references are updated as governance docs are touched, with a focused pass for the most impactful docs (harness-architecture, rules-strategy, skills-strategy, AGENTS.md, documentation-index).

**Consequences:**

- Governance docs use a consistent reference pattern.
- Historical artifacts remain as-is.
- The `documentation-update` skill detects tool-specific references that should be tool-agnostic.

**Risks:**

- Inconsistency during transition (some docs updated, others not).
- Mitigation: Phased update with path drift detection. The transition period is acceptable because the architecture is additive — ambiguity is reduced incrementally, not atomically.

**Impact on existing Harness:** Governance docs are updated to use tool-agnostic references. `.cursor/` references that describe Cursor-specific implementation are kept. Historical artifacts are not touched.

---

### DDA-006: Documentation Taxonomy

**Decision:** Generalize the existing taxonomy (established by ADR-003) for multi-tool artifacts. The `Documentation/` structure remains tool-agnostic. Tool-specific directories are referenced as implementation details.

**Alternatives considered:**

| Alternative | Description | Why rejected |
|-------------|-------------|--------------|
| Tool-specific sections coexist | Add separate "Cursor Artifacts" and "Cline Artifacts" sections in the documentation index | Creates the impression of separate governance models per tool. Violates REQ-005 (single authoritative documentation model). |
| New taxonomy for multi-tool | Create a new documentation taxonomy that explicitly separates Harness and Tool artifacts | The existing taxonomy is fundamentally sound. It needs generalization, not replacement. A new taxonomy would require moving all governance docs and updating all references — high effort, low value. |

**Rationale:** The existing taxonomy (ADR-003) correctly places durable documentation in `Documentation/` and machine-facing artifacts in tool-specific directories. The generalization is minimal: rename tool-specific section names to tool-agnostic names, and reference tool-specific directories as implementation details within the existing structure.

**Consequences:**

- The documentation index uses "Machine-Facing Artifacts" instead of "Cursor Artifacts."
- The rules inventory is titled "Rules Inventory" instead of "Cursor Rules Inventory."
- Strategy docs are titled "Harness Rules" and "Harness Skills" instead of "Cursor Rules" and "Cursor Skills."

**Risks:**

- Renaming may temporarily break internal links.
- Mitigation: Links are updated during the documentation generalization phase.

**Impact on existing Harness:** Documentation titles and section names are updated. Content structure is unchanged. No documentation is moved to a new location.

---

### DDA-007: Documentation Naming

**Decision:** Rename tool-specific titles to tool-agnostic names.

**Alternatives considered:**

| Alternative | Description | Why rejected |
|-------------|-------------|--------------|
| Keep with disambiguation notes | Add a note at the top of each doc saying "This document applies to all tools, not just Cursor" | Does not resolve the ambiguity. The title "Cursor Rules Inventory" still implies Cursor-only scope. Notes are easily missed. |
| Rename some, keep others | Rename `rules.md` but keep `rules-strategy.md` as "Cursor Rules Strategy" | Inconsistent. If the strategy covers all tools, the title should reflect that. Partial renaming creates more confusion than full renaming. |

**Rationale:** Tool-agnostic names are accurate for all tools. They do not lose information — tool-specific implementation details are preserved within the document content. Renaming is a low-risk, high-clarity change.

**Consequences:**

- All governance docs with "Cursor" in the title or section names are renamed.
- Internal links and references are updated.
- Historical artifacts are not renamed.

**Risks:**

- Temporary link breakage during renaming.
- Mitigation: Links are updated in the same phase as renaming.

**Impact on existing Harness:** Document titles and section names change. Content is reorganized for clarity but not moved to new files.

---

### DDA-008: Cross-Skill Reference Strategy

**Decision:** Cross-skill references use **skill names**, not tool-specific paths. When a path is needed for clarity, both tool paths are provided in parenthetical disambiguation.

**Alternatives considered:**

| Alternative | Description | Why rejected |
|-------------|-------------|--------------|
| Relative paths | Use relative paths from one skill to another (e.g., `../documentation-update/SKILL.md`) | Relative paths are still tool-specific — they assume a specific directory structure. They break if the skill directory structure changes. |
| Tool-specific paths with documentation | Use `.cursor/skills/` paths and document that Cline uses `.cline/skills/` | Creates tool-specific coupling within skill content. Skill content is a Harness Asset and should not embed tool-specific paths. |

**Rationale:** Skill names are tool-agnostic, stable, and meaningful. They do not break when directory structures change. They work across all tools. This approach keeps skill content as a pure Harness Asset.

**Consequences:**

- Skill content references other skills by name.
- When a path is needed (e.g., in a contribution guide), both tool paths are provided.
- The `verifier` skill references the `documentation-update` skill by name, not by path.

**Risks:**

- A developer who needs the actual file path must know which tool they are using.
- Mitigation: Contribution guides and documentation index provide both paths.

**Impact on existing Harness:** Skill content is updated to use name-based references. This is a content change within existing skill files. No skill is moved or renamed.

---

### DDA-009: Governance Evolution

**Decision:** The authority hierarchy does **not** need explicit tool-asset placement. Tool Assets are implementation details, not governance authorities. The `effective-harness-planning` skill is updated to evaluate tool-specific assumptions during harness reviews.

**Alternatives considered:**

| Alternative | Description | Why rejected |
|-------------|-------------|--------------|
| Add tool assets to the authority hierarchy | Place Tool Assets below Rules and Skills in the hierarchy | Tool Assets are not governance authorities. They do not compete with ADRs, Architecture docs, or Rules for authority. Adding them to the hierarchy would blur the distinction between governance and implementation. |
| Create a separate tool-asset hierarchy | Define a parallel hierarchy for tool-specific decisions | Adds complexity without value. Tool-specific decisions (e.g., which frontmatter schema to use) are implementation choices, not governance decisions. They do not need a hierarchy. |

**Rationale:** The authority hierarchy resolves conflicts between governance truth sources (ADRs, Architecture docs, State, SDD, Rules, Skills). Tool Assets do not participate in governance conflict resolution. A frontmatter schema does not override an ADR. A storage location does not compete with a Rule. The hierarchy remains tool-agnostic.

The `effective-harness-planning` skill should evaluate tool-specific assumptions (e.g., "Does this governance doc assume Cursor as the only tool?") during harness reviews. This is a skill content update, not a governance change.

**Consequences:**

- The authority hierarchy remains unchanged.
- The `effective-harness-planning` skill includes a multi-tool awareness check.
- Tool-specific assumptions are flagged during harness reviews.

**Risks:**

- Harness reviews may miss tool-specific assumptions if the skill check is not followed.
- Mitigation: The check is part of the skill's standard workflow.

**Impact on existing Harness:** The authority hierarchy is unchanged. One skill is updated to include multi-tool awareness.

---

### DDA-010: Calibration Workflow Evolution

**Decision:** Update pilot report templates to include multi-tool fields (which tool was used, what was observed). The calibration workflow itself remains unchanged in principle.

**Alternatives considered:**

| Alternative | Description | Why rejected |
|-------------|-------------|--------------|
| Create separate calibration workflows per tool | One workflow for Cursor pilots, one for Cline pilots | Violates REQ-005 (single authoritative model). The calibration workflow is tool-agnostic by design (RH-14). |
| No changes to calibration workflow | Leave pilot report templates as-is | Pilot reports would not capture which tool was used, making multi-tool calibration less effective. The template update is low-risk and high-value. |

**Rationale:** The calibration workflow is a process, not a tool-specific mechanism (RH-14 Validated). Pilot reports are markdown artifacts. Adding "tool used" and "observations" fields to the pilot report template is a natural evolution that does not change the workflow itself.

**Consequences:**

- Pilot report templates include multi-tool fields.
- Future pilot reports document which tool was used.
- The calibration workflow process is unchanged.

**Risks:**

- Existing pilot reports do not have these fields (they are historical artifacts and are not updated).
- Mitigation: The template update applies to future pilot reports only.

**Impact on existing Harness:** Pilot report template is updated. Calibration workflow process is unchanged. Historical pilot reports are not modified.

---

### DDA-011: Documentation Update Evolution

**Decision:** Extend path drift detection to flag `.cursor/` references in governance docs that describe Harness concepts (not Cursor-specific implementation details).

**Governance Policy (Harness Asset):** Tool-specific path references in governance documentation that describe Harness concepts shall be updated to tool-agnostic concepts with parenthetical disambiguation. This policy is owned by the Harness governance model — it applies regardless of which tool or mechanism enforces it.

**Mechanism (Tool Asset):** The `documentation-update` skill implements this policy through its path drift check. The skill (executed by an agent or human) can perform the semantic analysis that distinguishes between appropriate tool-specific references (implementation details) and inappropriate ones (Harness concepts described with tool-specific paths). If a tool does not have a `documentation-update` skill, the governance policy still applies and must be satisfied through other means (e.g., manual review during calibration).

**Alternatives considered:**

| Alternative | Description | Why rejected |
|-------------|-------------|--------------|
| No path drift check update | Leave the path drift check as-is | The 43+ `.cursor/` references would continue to accumulate. New governance docs might use tool-specific references where tool-agnostic references are appropriate. |
| Automated CI check for tool-specific references | Add a CI check that flags all `.cursor/` references in governance docs | Too broad — many `.cursor/` references are legitimately Cursor-specific (e.g., "Cursor uses `alwaysApply: true`"). A CI check cannot distinguish between appropriate and inappropriate references without semantic analysis. |

**Rationale:** The governance policy is separate from its tool-specific implementation. This separation preserves the Harness Asset / Tool Asset boundary: the Harness defines what must be detected; the skill defines how detection is performed. The skill serves the policy; the policy does not depend on any single tool's mechanism.

**Consequences:**

- The path drift detection policy is a Harness Asset (governance policy).
- The `documentation-update` skill implements the policy as a Tool Asset (mechanism).
- The check distinguishes between appropriate tool-specific references (implementation details) and inappropriate ones (Harness concepts described with tool-specific paths).
- The check is advisory — it flags potential issues for review, not enforcement.

**Risks:**

- The check may produce false positives (flagging appropriate tool-specific references).
- Mitigation: The check is advisory and reviewed by the documentation-update workflow.
- A tool without a `documentation-update` skill may not implement the check automatically.
- Mitigation: The governance policy still applies; manual review during calibration covers this gap.

**Impact on existing Harness:** The path drift detection governance policy is formalized. The `documentation-update` skill content is updated to implement it. No new skill is created.

---

### DDA-012: Context Management Adoption

**Decision:** Adopt selectively. `.clineignore` is documented as a recommended Cline optimization. Memory Bank is not adopted (State.md + durable docs is sufficient). Slash commands and Plan/Act mode remain Cline-specific.

**Alternatives considered:**

| Alternative | Description | Why rejected |
|-------------|-------------|--------------|
| Adopt all Cline features as Harness best practices | Make `.clineignore`, Memory Bank, slash commands, and Plan/Act mode part of the Harness methodology | Blurs the Harness/Tool boundary (AP-002, AP-005). These are tool-specific optimizations, not governance methodology. Adopting them as Harness requirements would make the Harness dependent on Cline features. |
| Keep all Cline features Cline-specific with no documentation | Do not document Cline features in Harness docs | Developers using Cline would not know which Cline features complement the Harness. Lack of guidance leads to inconsistent adoption. |
| Adopt Memory Bank as Harness methodology | Replace State.md + durable docs with Memory Bank pattern | The Harness already achieves cross-session persistence through State.md and durable documentation (Assumption 9). Memory Bank is an alternative methodology, not a gap. Adopting it would require restructuring existing documentation and would not improve governance. |

**Rationale:** The Harness defines context loading **strategy** (what to load). Each tool provides **mechanisms** (how to load). `.clineignore` is a useful Cline mechanism for implementing the "avoid loading by default" strategy. Documenting it as a recommended Cline optimization helps Cline users without making it a Harness requirement. Memory Bank and slash commands are Cline-specific methodologies that complement but do not replace Harness governance.

**Consequences:**

- `.clineignore` is documented as a recommended Cline optimization in the Harness docs.
- Memory Bank is documented as an alternative methodology that is not adopted.
- Slash commands and Plan/Act mode are documented as Cline-specific features that complement the SDD lifecycle.
- None of these become Harness governance requirements.

**Risks:**

- Cline users may adopt Memory Bank independently, creating a parallel documentation structure.
- Mitigation: The Harness docs explain why State.md + durable docs is the canonical pattern and how Memory Bank relates to it.

**Impact on existing Harness:** Harness documentation is updated to mention Cline-specific optimizations as implementation options. No governance change. No new Harness requirements.

---

### DDA-013: Global vs Project Scope

**Decision:** All Harness artifacts remain **project-scoped**. Global scope is a tool-specific capability, not a Harness governance concern.

**Alternatives considered:**

| Alternative | Description | Why rejected |
|-------------|-------------|--------------|
| Define global scope for Harness rules and skills | Maintain a global set of rules/skills in `~/.cline/` or equivalent | The Harness is project-specific. Global scope introduces a configuration dimension not present in the current architecture. It would create ambiguity about which rules apply (global vs project) and complicate governance. |
| Document global scope as an option | Mention that Cline supports global scope but recommend project-only | Adds documentation complexity for a feature that is not needed for governance. Developers who want global rules can use Cline's native capability without Harness endorsement. |

**Rationale:** The Harness is defined per-project. All governance rules, skills, and documentation are project-scoped. Cline's global scope is a tool capability that developers may use independently, but the Harness does not define, maintain, or govern global artifacts. This keeps the governance model simple and project-contained.

**Consequences:**

- All Harness rules and skills are project-scoped.
- Cline's global scope is a tool capability, not a Harness concern.
- No Harness artifacts are maintained in global directories.

**Risks:**

- Developers who use Cline's global scope may create rules that conflict with project rules.
- Mitigation: Cline's documented behavior is that workspace rules take precedence over global rules when they conflict. The Harness relies on this precedence.

**Impact on existing Harness:** No change. All existing artifacts are already project-scoped.

---

### DDA-014: Toggle Governance

**Decision:** **Defer.** The Harness does not define toggle policy. Toggling is a tool-specific runtime behavior.

**Alternatives considered:**

| Alternative | Description | Why rejected |
|-------------|-------------|--------------|
| Define toggle policy | Specify which rules/skills may be toggled and when | Toggling is a runtime behavior, not a governance concern. The Harness defines what rules should exist and what they should say, not whether a user can temporarily disable them. A toggle policy would add governance overhead for a low-risk, low-frequency behavior. |
| Prohibit toggling | Require that all rules and skills are always enabled | Not enforceable — the Harness cannot control tool runtime behavior. Cline allows toggling; Cursor's toggle behavior is not documented. Prohibiting it would be unenforceable and would not improve governance. |

**Rationale:** Toggling rules or skills is a developer's runtime choice. The Harness's authority comes from the content of its rules and skills, not from whether they can be disabled. A developer who toggles off `security-phi` is responsible for any PHI exposure that results — this is already covered by the rule's content and the security review process. Deferring toggle governance avoids overengineering (AP-007) and keeps the Harness focused on content, not runtime control.

**Consequences:**

- No toggle policy is defined.
- Toggling remains a tool-specific runtime behavior.
- The Harness does not mention toggling in governance docs (except to note that Cline supports it as a tool capability).

**Risks:**

- A developer may toggle off a critical rule (e.g., `security-phi`) and introduce a governance gap.
- Mitigation: This risk exists today (Cursor may have equivalent behavior). The security review process and verifier role catch PHI exposure regardless of rule toggle state. The risk is low and does not justify governance overhead.

**Impact on existing Harness:** No change. The Harness does not currently define toggle policy.

---

## ADR Evaluation

### ADR-003 Disposition

| Question | Answer |
|----------|--------|
| Should ADR-003 be accommodated, revised, or superseded? | **Revised** — ADR-003's core decision (`.cursor/` for Cursor artifacts, `Documentation/` for durable docs) remains valid. The revision generalizes the taxonomy to acknowledge that multiple tools may have their own machine-facing directories. |
| Rationale | Accommodation preserves ambiguity. Supersession loses historical context. Revision preserves the valid decision while generalizing the principle. |
| What changes in ADR-003? | The decision section is generalized: `Documentation/` remains the canonical home for durable project documentation (unchanged). `.cursor/` remains for Cursor-specific machine-facing artifacts (unchanged). The revision adds that other AI tools may have their own machine-facing directories (e.g., `.clinerules/` for Cline rules, `.cline/` for Cline skills). The taxonomy is generalized from "Cursor artifacts" to "tool-specific machine-facing artifacts." |
| What does NOT change in ADR-003? | The `Documentation/` taxonomy is unchanged. `.cursor/` remains the Cursor directory. The core decision is preserved. |

### New ADR Candidate

| Question | Answer |
|----------|--------|
| Does multi-tool evaluation require a new ADR? | **Yes** — The Harness Asset / Tool Asset / Shared Asset taxonomy is a durable architectural boundary that affects documentation taxonomy, cross-context ownership, and future architecture guidance. It meets ADR creation criteria. |
| What should the new ADR establish? | The three-layer ownership taxonomy (Harness Assets, Tool Assets, Shared Assets) as the fundamental architectural boundary for the AI Harness. The canonical content principle (content is canonical, format is tool-specific). The requirement that tool-specific projections faithfully reproduce canonical governance content. |
| ADR ID | To be assigned during ADR governance (recommended: ADR-007 or next available) |

### ADR for Canonical Rule/Skill Format

| Question | Answer |
|----------|--------|
| Does the canonical format decision require an ADR? | **No** — The decision to maintain tool-specific formats (no canonical format) is an implementation strategy, not a durable architectural decision. It does not change bounded context ownership, persistence, security, API contracts, or cross-context ownership. It is a format choice within the Harness Asset / Tool Asset taxonomy. If the taxonomy ADR is accepted, the format decision is an operational consequence, not a separate ADR. |

### ADR Strategy Summary

| ADR | Disposition | Trigger | Status |
|-----|-------------|---------|--------|
| ADR-003 | **Revise** | Generalize documentation taxonomy for multi-tool artifacts | Recommended for ADR governance |
| New ADR (taxonomy) | **Create** | Establish Harness Asset / Tool Asset / Shared Asset taxonomy as permanent architecture | Recommended for ADR governance |
| Canonical format ADR | **Not needed** | Tool-specific formats decision is operational, not durable | Not recommended |

**This Design does not draft ADRs.** It provides architectural recommendations for ADR governance. ADR creation and revision belong to the ADR governance process, not the Design phase.

---

## Future Extensibility

### What a Future AI Tool Must Provide

To become a compliant Harness implementation, a future AI tool must provide:

1. **Rule loading mechanism** — The tool must be able to load persistent rules from markdown files in a tool-specific directory. The rules must be loaded as context tokens (always-on or conditional based on file scope).

2. **Skill loading mechanism** — The tool must be able to load skills on-demand from `SKILL.md` files with `name`/`description` frontmatter in a tool-specific directory. (If the tool uses a different skill format, the Harness's skill content must be adapted to that format.)

3. **Bootstrap loading** — The tool must auto-load `AGENTS.md` as the shared cross-tool entry point. (If the tool does not auto-load `AGENTS.md`, it must provide an equivalent bootstrap mechanism that loads the same content.)

4. **Non-conflicting coexistence** — The tool's directory structure must not conflict with existing tool directories (`.cursor/`, `.clinerules/`, `.cline/`).

5. **Content fidelity** — The tool's rule and skill projections must faithfully reproduce the canonical governance content. The guardrail text and workflow procedures must be identical across all tools.

6. **Context loading** — The tool must be able to implement the Harness's context loading strategy (what to load, when to load) through its native mechanisms.

### What a Future AI Tool May Optionally Provide

- Tool-specific optimizations (e.g., `.clineignore`, slash commands, Plan/Act mode) that complement but do not alter Harness governance.
- Global scope for rules and skills (tool-specific capability, not a Harness concern).
- Toggle capabilities for rules and skills (tool-specific runtime behavior).
- Additional context management mechanisms (e.g., context compression, memory bank).

### What the Architecture Does Not Require

- A specific file format — each tool may use its own format (`.mdc`, `.md`, `.txt`, or future formats).
- A specific frontmatter schema — each tool may use its own frontmatter (or no frontmatter).
- A specific directory name — each tool may use its own directory (`.cursor/`, `.clinerules/`, `.cline/`, or future directories).
- A specific loading mechanism — each tool may load rules and skills through its own mechanism.
- A specific invocation method — each tool may invoke skills through its own method (interface trigger, slash command, description matching).

### Extensibility Validation

The architecture remains valid for future tools because:

- The ownership taxonomy (Harness / Tool / Shared) is not specific to Cursor or Cline.
- The canonical content principle (content is canonical, format is tool-specific) applies to any tool.
- The path reference strategy (tool-agnostic concepts + parenthetical disambiguation) accommodates any number of tools.
- The documentation taxonomy (`Documentation/` for governance, tool-specific directories for implementations) generalizes to any tool.
- The migration strategy (additive, phased, reversible) applies to adding any new tool.

A third tool would require: a new tool-specific directory, rule projections in the tool's format, skill projections in the tool's format, and `AGENTS.md` auto-loading (or equivalent). No Harness Asset would need to change. No existing tool implementation would need to change.

### Future Canonical Representation

The current Design intentionally avoids introducing a separate physical canonical repository or directory. This keeps the architecture simple: during the transition, the Cursor projection serves as the de facto canonical source; in the Target Architecture, governance content is independently definable through Harness documentation artifacts.

Future evolution may introduce a physical canonical representation if maintenance complexity justifies it — for example, if the number of supported tools exceeds three, or if synchronization overhead becomes a measurable maintenance burden. Such an evolution would:

- Introduce a canonical location (e.g., a rule content section in `Documentation/AI-Harness/rules.md`) without requiring a new file format or directory.
- Preserve tool-specific formats as projections of the canonical content.
- Maintain backward compatibility with all existing tool implementations.
- Require an ADR evaluation, as it would establish a new architectural layer.

This possibility is a design property of the architecture, not a planned feature. The Design does not require it; it enables it.

---

## Architectural Risks

| Risk | Impact | Probability | Mitigation |
|------|--------|-------------|------------|
| Content drift between tool projections | Governance inconsistency — different tools apply different rules | Medium | Path drift detection in `documentation-update` skill; governance review during calibration workflow; single-authoring principle |
| Frontmatter mapping errors | Rule activation behavior differs between tools | Low | The mapping table is documented in this Design; Tasks phase includes frontmatter verification |
| Path reference inconsistency during transition | Some docs use tool-agnostic references, others use tool-specific | Medium | Phased update with prioritization of high-impact docs; path drift detection catches regressions |
| ADR-003 revision rejected | Taxonomy generalization is not formally accepted | Low | The revision preserves ADR-003's core decision; the evidence is strong; rejection would leave ambiguity unresolved |
| Overengineering through adapter/generation mechanisms | Unnecessary complexity added during Tasks phase | Medium | This Design explicitly rejects canonical format with generation; the Tasks phase must respect this decision |
| Tool capability divergence | Future tools may not support `SKILL.md` format or `AGENTS.md` auto-loading | Low | The architecture defines what a compliant implementation must provide; tools that cannot meet these requirements are not compliant |
| Memory Bank parallel adoption | Cline users adopt Memory Bank, creating a parallel documentation structure | Low | Harness docs explain why State.md + durable docs is canonical; Memory Bank is documented as an alternative, not a replacement |
| Toggle-induced governance gaps | A developer toggles off a critical rule | Low | Security review process and verifier role catch issues regardless of toggle state; the risk is not new (it exists today) |

---

## Trade-offs

| Trade-off | Decision | What we gain | What we accept |
|-----------|----------|-------------|----------------|
| Tool-specific formats vs. canonical format | Tool-specific | Backward compatibility; native tool behavior; no adapter complexity | Content must be projected to each tool's format; synchronization effort |
| Tool-specific frontmatter vs. cross-tool schema | Tool-specific | No silent failures; each tool uses its native frontmatter | Frontmatter mapping must be documented and followed |
| Tool-specific directories vs. shared directory | Tool-specific | No conflicts; each tool loads from its native location; no reconfiguration | Multiple directories for the same governance content |
| Name-based cross-skill references vs. path-based | Name-based | Tool-agnostic; stable; works across all tools | Developers must know their tool's directory to find the actual file |
| Parenthetical disambiguation vs. pure tool-agnostic | Parenthetical | Both governance clarity and implementation specificity | Slightly more verbose documentation |
| ADR-003 revision vs. supersession | Revision | Preserves historical context; preserves core decision | ADR-003 becomes longer; revision must be carefully scoped |
| Defer toggle governance vs. define policy | Defer | Avoids overengineering; keeps Harness focused on content | Toggle behavior is ungoverned; relies on developer responsibility |
| Project-scoped only vs. global scope | Project-only | Simple governance model; no global/project ambiguity | Cline's global scope is not utilized by the Harness |

---

## Design Validation

### Against Architectural Principles

| Principle | How the Design satisfies it |
|-----------|-----------------------------|
| `AP-001`: The Harness is the primary product | The Design formalizes Harness Assets as the core value; Tool Assets serve the methodology |
| `AP-002`: AI tools are implementations of the Harness | The Implementation Model defines tools as projections of canonical content |
| `AP-003`: Methodology is shared | The authority hierarchy, SDD lifecycle, and verification governance are defined once, tool-agnostic |
| `AP-004`: Tool integrations may be tool-specific | Each tool has its own format, frontmatter, and directory; tool-specific optimizations are encouraged |
| `AP-005`: Optimize implementations, not methodology | Cline features (`.clineignore`, slash commands) are documented as tool-specific optimizations, not governance |
| `AP-006`: Preserve backward compatibility | Cursor artifacts are not modified; historical artifacts are not rewritten; rollback is additive-removal |
| `AP-007`: Avoid unnecessary abstraction | No shared directory, no canonical format, no adapter layer, no cross-tool frontmatter schema |

### Against Requirements

| Requirement | Validation |
|-------------|------------|
| `REQ-001` | Cursor artifacts unchanged; no `.cursor/` file is modified |
| `REQ-002` | Cline artifacts created from same content; `AGENTS.md` shared |
| `REQ-003` | Governance methodology is Harness Asset — unchanged in principle |
| `REQ-004` | SDD methodology is Harness Asset — tool-agnostic |
| `REQ-005` | Single authoritative model; content authored once; no duplicate inventories |
| `REQ-006` | Tool-specific optimizations are Tool Assets; do not alter methodology |
| `REQ-007` | Rule content is Harness Asset; format is Tool Asset; separable |
| `REQ-008` | Skill content is Harness Asset; storage is Tool Asset; cross-skill refs use names |
| `REQ-009` | `AGENTS.md` is Shared Asset; both tools auto-load it |
| `REQ-010` | Cline directories do not conflict with Cursor directories |
| `REQ-011` | Historical artifacts are not rewritten; remain interpretable |

### Against NFRs

| NFR | Validation |
|-----|------------|
| `NFR-001` | Single source of truth for content; path drift detection prevents drift |
| `NFR-002` | Architecture generalizes beyond Cursor and Cline; future tool requirements defined |
| `NFR-003` | Cursor workflows preserved; historical artifacts preserved; rollback is additive-removal |
| `NFR-004` | Governance docs reference tool-agnostic concepts by default; parenthetical disambiguation for detail |
| `NFR-005` | Authority hierarchy, ADR policy, calibration workflow unchanged in principle |
| `NFR-006` | No duplicated rule content, skill content, or governance documentation per tool |
| `NFR-007` | Migration is phased; each phase preserves Cursor workflows; rollback is reversible |

### Against Constraints

| Constraint | Validation |
|------------|------------|
| No Cursor regression | Cursor artifacts are not modified, moved, or removed |
| No duplicated governance | Content is authored once; format is tool-specific |
| Preserve authority hierarchy, SDD lifecycle, verification governance | These are Harness Assets — unchanged in principle |
| ADR-003 is an accepted decision | ADR-003 is revised, not superseded; revision follows ADR governance |
| No clinical, security, or persistence impact | Feature is governance-only; no product code, database, API, or patient data touched |
| No implementation in this phase | Design defines architecture; no files are created or modified |
| No ADR creation in this phase | Design recommends ADR disposition; does not draft ADRs |
| Treat all patient data as sensitive | No patient data is present in governance artifacts; `security-phi` rule applies to all tools |

---

## Affected Components

| Component | Path | Responsibility | Change |
|-----------|------|----------------|--------|
| `AGENTS.md` | `AGENTS.md` | Shared bootstrap | Update references to tool-agnostic concepts with parenthetical disambiguation |
| Harness architecture | `Documentation/AI-Harness/Harness-Design/harness-architecture.md` | Target architecture | Update component references to tool-agnostic concepts; add multi-tool architecture section |
| Rules strategy | `Documentation/AI-Harness/Harness-Design/rules-strategy.md` | Rule governance | Rename to "Harness Rules Strategy"; update path references; add multi-tool format section |
| Skills strategy | `Documentation/AI-Harness/Harness-Design/skills-strategy.md` | Skill governance | Rename to "Harness Skills Strategy"; update path references; add cross-skill reference strategy |
| Rules inventory | `Documentation/AI-Harness/rules.md` | Rule inventory | Rename to "Rules Inventory"; update to include tool-specific format columns |
| Documentation index | `Documentation/AI-Harness/documentation-index.md` | Documentation navigation | Rename "Cursor Artifacts" to "Machine-Facing Artifacts"; add Cline directories |
| Contribution guide | `Documentation/AI-Harness/CONTRIBUTING-AI.md` | Contribution workflow | Update skill path references to name-based or parenthetical disambiguation |
| SDD templates | `Documentation/AI-Harness/template/sdd/` | SDD templates | Update skill path references to name-based or parenthetical disambiguation |
| `documentation-update` skill | `.cursor/skills/documentation-update/SKILL.md` | Documentation routing | Extend path drift check to detect tool-specific references in tool-agnostic contexts |
| `effective-harness-planning` skill | `.cursor/skills/effective-harness-planning/SKILL.md` | Harness review | Add multi-tool awareness check for tool-specific assumptions |
| Pilot report template | `Documentation/AI-Harness/` | Calibration input | Add multi-tool fields (tool used, observations) |
| ADR-003 | `Documentation/Architecture/ADR/ADR-003-documentation-taxonomy.md` | Documentation taxonomy | Revise to generalize for multi-tool artifacts |
| New ADR | `Documentation/Architecture/ADR/` | Harness/Tool/Shared taxonomy | Create new ADR for ownership taxonomy (recommended) |
| Cline rules | `.clinerules/*.md` (new) | Cline rule projections | Create from canonical rule content with Cline format |
| Cline skills | `.cline/skills/*/SKILL.md` (new) | Cline skill projections | Create from canonical skill content with Cline format |
| `.clineignore` | `.clineignore` (new, optional) | Cline context exclusion | Create if desired for Cline context optimization |
| State.md | `Documentation/State.md` | Operational truth | Update harness files section to note tool-specific directories |
| Runbook | `Documentation/Technical/runbook.md` | Local runbook | Update tooling section to mention Cline alongside Cursor |

---

## Reuse Analysis

| Existing pattern | Reuse decision | Notes |
|------------------|----------------|-------|
| `Documentation/` taxonomy (ADR-003) | Reuse (generalized) | The taxonomy is fundamentally sound; generalization is minimal |
| `SKILL.md` format | Reuse (shared) | Format is identical between Cursor and Cline; no change needed |
| `AGENTS.md` bootstrap | Reuse (shared) | Both tools auto-load it; content is updated for multi-tool clarity |
| Rule design standard | Reuse (unchanged) | The standard from `rules-strategy.md` remains valid for all tools |
| Skill design standard | Reuse (unchanged) | The standard from `skills-strategy.md` remains valid for all tools |
| Authority hierarchy | Reuse (unchanged) | Tool-agnostic by design |
| SDD lifecycle | Reuse (unchanged) | Tool-agnostic by design |
| Calibration workflow | Reuse (unchanged) | Tool-agnostic by design; pilot report template updated |
| Path drift detection | Adapt | Extended to detect tool-specific references in tool-agnostic contexts |

---

## Security And PHI Design

- Security-sensitive: **No** (for this Design phase)
- Review prompt expected: **No**
- Rationale: This feature is governance-only. It does not touch patient identity, clinical data, files, logs, API exposure, auth, or secrets. The `security-phi` rule applies to all tools equally — it is a Harness Asset whose content is identical across all tool projections. Multi-tool support does not introduce new PHI exposure vectors. The security invariant ("treat all patient data as sensitive") is tool-agnostic and applies regardless of which AI tool consumes the rules.

---

## Verification Handoff Notes

- **Expected gates:** Documentation review (governance consistency); repository structure review (non-conflicting coexistence); content review (rule/skill content fidelity across tools); historical artifact review (backward compatibility)
- **Expected review sensors:** Documentation review sensor for governance consistency
- **Known skipped or manual checks:** No SQL, API, build, or clinical gates apply
- **Evidence the implementation must provide:**
  - Cursor compatibility: `.cursor/` artifacts are unchanged
  - Cline compatibility: `.clinerules/` and `.cline/` artifacts are loadable
  - Governance consistency: authority hierarchy, SDD lifecycle, verification governance unchanged
  - No duplication: rule/skill content is single-authored
  - Backward compatibility: historical artifacts with `.cursor/` references are unmodified

---

## Documentation Follow-up Candidates

- **State:** Update harness files section to note tool-specific directories; update active epic and next steps
- **ADR:** Revise ADR-003 for multi-tool taxonomy generalization; create new ADR for Harness/Tool/Shared taxonomy
- **Architecture docs:** No architecture doc changes (governance-only feature)
- **Technical docs:** Update runbook to mention Cline alongside Cursor
- **Rules:** No rule content changes; Cline rule projections are new Tool Assets
- **Skills:** Update `documentation-update` skill (path drift check); update `effective-harness-planning` skill (multi-tool awareness); update pilot report template
- **Review prompts:** No review prompt changes
- **Templates:** Update SDD templates to use name-based skill references