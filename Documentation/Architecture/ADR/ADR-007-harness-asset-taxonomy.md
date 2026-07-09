# ADR-007: Harness Asset / Tool Asset / Shared Asset Taxonomy

## Status

Accepted (2026-07-08)

## Context

The Doc Organo AI Harness was originally implemented around a single AI tool (Cursor). As part of expanding the Harness to support multiple AI tools, a fundamental architectural boundary emerged between governance content (what rules say, what skills do) and implementation format (how each tool loads and applies them).

The AI Harness Multi-Tool Evaluation (SDD `ai-harness-multi-tool`) researched, specified, designed, implemented, and verified that:

- Rule guardrail text and skill workflow procedures are pure markdown — portable between tools with no content change.
- Each AI tool uses different directories, file formats, and frontmatter schemas to load the same governance content.
- `AGENTS.md` is auto-loaded by both Cursor and Cline as a cross-tool bootstrap.
- Cursor and Cline directories (`.cursor/`, `.clinerules/`, `.cline/`) do not conflict and can coexist in the same repository.
- The Harness governance methodology (authority hierarchy, SDD lifecycle, verification governance) is tool-agnostic by design.

Without a formalized boundary, there is a risk that future changes conflate governance decisions with tool-specific implementation details, or that documentation references treat a single tool's file paths as canonical Harness paths.

## Decision

Establish a **three-layer ownership taxonomy** as the fundamental architectural boundary for the AI Harness:

### Harness Assets

Capabilities and artifacts owned by the Harness methodology, defined in tool-agnostic terms. Authored once and consumed by all supported tools through their respective format adaptations.

| Harness Asset | Description |
|---------------|-------------|
| Rule content | The guardrail text of each rule — pure markdown expressing governance intent |
| Skill content | The workflow procedures in each skill body — pure markdown encoding Harness workflows |
| Authority hierarchy | ADRs > Architecture docs > State > SDD > Rules > Skills |
| SDD lifecycle | Research → Specify → Design → Tasks → Pre-Execution Review → Execute → Verify → Documentation Follow-Up → Reporting → Teacher Guide |
| Verification governance | Gate selection, review sensors, residual risk, skipped-gate reasoning |
| Documentation routing | Path-drift checks, follow-up routing |
| Calibration workflow | Pilot Execution → Pilot Report → Implementation Plan → Review → Phased Update → Consistency Audit → Next Pilot |
| Rule scoping intent | Whether a rule is always-on or file-scoped (the *intent*; the *expression* is tool-specific) |
| Context loading strategy | What context to load for each task type (the *strategy*; the *mechanism* is tool-specific) |
| Knowledge transfer strategy | Teacher Guides, concept classification, study paths |
| Artifact ownership model | Who owns what truth |
| Adaptive sizing model | Small/Medium/Large/Complex |
| ADR governance policy | Creation criteria, conflict resolution, escalation |
| Definition of Done | Completion criteria for SDD-backed work |

**Ownership:** Harness governance maintains Harness Assets in `Documentation/AI-Harness/Harness-Design/` and in the content of rules and skills. Changes to Harness Assets require governance review (calibration workflow or ADR evaluation depending on durability). Harness Assets must reference tool-agnostic concepts by default.

### Tool Assets

Capabilities and artifacts owned by a specific AI tool's implementation. Each tool has its own set of Tool Assets. Tool Assets serve the Harness methodology, not the reverse.

| Tool Asset | Cursor | Cline |
|------------|--------|-------|
| Rule storage location | `.cursor/rules/` | `.clinerules/` |
| Rule file extension | `.mdc` | `.md` |
| Rule frontmatter schema | `alwaysApply: true/false`, `globs: pattern`, `description` | `paths: [patterns]` (conditional) or no frontmatter (always-on) |
| Skill storage location | `.cursor/skills/` | `.cline/skills/` |
| Skill format | `SKILL.md` with `name`/`description` | `SKILL.md` with `name`/`description` (identical) |
| Rule loading mechanism | Auto-injection of `alwaysApply: true` rules; `globs` for file-scoped | Context-aware dynamic evaluation of `paths`; always-on for no-frontmatter |
| Tool-specific optimizations | — | `.clineignore`, slash commands, Plan/Act mode, skill/rule toggles, global scope |

**Ownership:** Each tool implementation owns its directory structure, file format, frontmatter schema, and tool-specific optimizations. Tool Assets must faithfully reproduce the canonical governance content — semantic equivalence is required. Tool Assets are implementation details, not governance authorities — they do not participate in governance conflict resolution.

### Shared Assets

Artifacts that are both Harness-owned in content and consumed identically by all tools, sitting at the intersection of Harness and Tool layers.

| Shared Asset | Description |
|--------------|-------------|
| `AGENTS.md` | Cross-tool bootstrap file. Both Cursor and Cline auto-load it identically. Content is Harness-authored; loading is tool-native. |

### Canonical Content Principle

**Content is canonical; format is tool-specific.** There is one authoritative version of each rule's guardrail text and each skill's workflow procedures. Each tool consumes that content through its own format, frontmatter, and directory. "Canonical" refers to the governance-level truth that all Tool Assets must faithfully reproduce.

### Transition Period

During the transition from single-tool to multi-tool, the Cursor projection serves as the de facto canonical source. In the Target Architecture, governance content is independently definable through Harness documentation artifacts without reference to any single tool's format. This transition is additive — Cursor remains fully operational throughout, and Cline support is added alongside it.

### Asymmetric Relationship

Harness Assets define **what** governance should be. Tool Assets provide **how** governance is loaded and applied. Tool Assets serve the methodology, not the reverse. Tool Assets do not compete with ADRs, Architecture docs, or Rules in the authority hierarchy.

### Future Tool Integration

A future AI tool becomes a compliant Harness implementation by providing:

1. **Rule loading** — Load persistent rules from markdown files in a tool-specific directory.
2. **Skill loading** — Load skills on-demand from `SKILL.md` files with `name`/`description` frontmatter.
3. **Bootstrap loading** — Auto-load `AGENTS.md` as the shared cross-tool entry point (or equivalent).
4. **Non-conflicting coexistence** — Directory structure must not conflict with existing tool directories.
5. **Content fidelity** — Rule and skill projections must faithfully reproduce canonical governance content.
6. **Context loading** — Implement the Harness's context loading strategy through native mechanisms.

What the architecture does not require: a specific file format, a specific frontmatter schema, a specific directory name, a specific loading mechanism, or a specific invocation method.

## Consequences

**Positive**

- Clear ownership boundary between governance decisions and tool-specific implementation choices.
- Governance methodology is defined once in tool-agnostic terms and consumed by all tools.
- Each tool uses its native format and conventions — no lowest-common-denominator compromise.
- Adding a third AI tool requires no changes to Harness Assets or existing Tool Assets.
- Documentation references use tool-agnostic concepts by default, with parenthetical disambiguation when implementation paths are needed.

**Negative**

- Rule content and skill content exist in multiple tool-specific files — synchronization must be maintained.
- During the transition, Cursor artifacts serve as the de facto canonical source; the Target Architecture goal of independent governance content is not yet achieved.
- Manual synchronization creates a risk of content drift between tool projections.
- The ownership taxonomy adds a conceptual layer that contributors must understand.

**Mitigations**

- The `documentation-update` skill includes a multi-tool path drift check that flags content inconsistencies during Documentation Follow-Up.
- The `effective-harness-planning` skill includes a multi-tool awareness check during harness reviews.
- The calibration workflow provides a second line of governance review.
- Frontmatter mapping is documented in `Documentation/AI-Harness/Harness-Design/harness-architecture.md`.

## Actions

| Horizon | Action |
|---------|--------|
| Short | Maintain content fidelity between Cursor and Cline projections through manual synchronization |
| Short | Use the `documentation-update` skill's multi-tool path drift check after changes |
| Medium | Evaluate whether a physical canonical representation is warranted if the number of supported tools exceeds two |
| Long | Consider automated synchronization or generation when maintenance burden justifies it |

## References

- `Documentation/AI-Harness/Harness-Design/harness-architecture.md`
- `Documentation/Architecture/ADR/ADR-003-documentation-taxonomy.md`
- `Documentation/SDD/ai-harness-multi-tool/design.md`
- `Documentation/SDD/ai-harness-multi-tool/verification.md`