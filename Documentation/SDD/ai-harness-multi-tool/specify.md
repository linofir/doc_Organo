# Specify — AI Harness Multi-Tool Evaluation

> Feature SDD: `Documentation/SDD/ai-harness-multi-tool/`
> Generated SDD artifacts are written in English.
> **Phase:** Specify (complete)
> **Lifecycle position:** Research → **Specify** → Design → Tasks → SDD Pre-Execution Review → Execute → Verify → Documentation Follow-Up → Reporting → Teacher Guide
> **Input from:** [Research](research.md), [Documentation Review Report](../../AI-Harness/research/review-tool-agnostic-harness.md), [PRD](../../Product/PRD.md), [PM](../../Product/PM_DocOrgano.md), [State](../../State.md)

---

## Context

The Doc Organo AI Harness is a mature governance system whose methodology — authority hierarchy, SDD lifecycle, verification governance, documentation routing, calibration workflow — is defined in tool-agnostic terms in principle. Its current machine-facing implementation, however, uses Cursor-specific conventions: `.cursor/rules/*.mdc` with `alwaysApply`/`globs` frontmatter and `.cursor/skills/*/SKILL.md`.

A documentation review and a Cline capability research have been completed. Both confirm that the Harness methodology is fundamentally tool-agnostic and that Cline can coexist with Cursor in the same repository without conflict. The Research identified real compatibility gaps in directory layout, file format, and frontmatter schema, all of which are adaptable without compromising governance.

This feature evaluates and establishes the architectural direction for evolving the Harness so that multiple AI tools (Cursor and Cline, with future extensibility) can coexist while preserving the current Cursor implementation as the reference architecture.

- PM item: Iniciativa de Governança — Adoção do AI Harness no MVP2 → AI Harness Multi-Tool Evaluation (P2, Planejado)
- Product or domain source: `Documentation/Product/PRD.md` (§8 Assumptions, §10 Success Metrics, §11 Constraints, §12 Open Questions)
- Current operational source: `Documentation/State.md` (branch `feature/harness`, clinical SQL migration intentionally paused)
- Related SDD, ADR, technical, or architecture docs: `Documentation/AI-Harness/research/review-tool-agnostic-harness.md`, `Documentation/SDD/ai-harness-multi-tool/research.md`, `Documentation/Architecture/ADR/ADR-003-documentation-taxonomy.md`, `Documentation/AI-Harness/Harness-Design/harness-architecture.md`, `Documentation/AI-Harness/Harness-Design/sdd-operational.md`, `Documentation/AI-Harness/Harness-Design/rules-strategy.md`, `Documentation/AI-Harness/Harness-Design/skills-strategy.md`

---

## Problem Statement

The AI Harness methodology is defined in tool-agnostic terms, but its machine-facing artifacts (rules, skills, path references) are coupled to Cursor-specific conventions. This coupling creates three architectural problems:

1. **Single-tool dependency.** The Harness can currently only be consumed by Cursor. Developers or agents using a different AI tool (e.g., Cline) cannot apply the same governance rules, invoke the same workflow skills, or bootstrap from the same `AGENTS.md` without manual adaptation.

2. **Documentation ambiguity.** 43+ explicit `.cursor/` path references across 18+ governance files conflate Harness methodology with Cursor implementation. In a multi-tool context, these references become ambiguous — they describe both "where Cursor artifacts live" and "where Harness artifacts live" as if they were the same thing.

3. **ADR-003 tension.** ADR-003 establishes `.cursor/` as the home for machine-facing Cursor artifacts. This accepted decision does not prohibit multi-tool support, but it does not address it. Whether ADR-003 should be accommodated, revised, or superseded is an unresolved architectural question.

The Research confirmed that these problems are solvable without redesigning the Harness. The methodology, rule content, skill content, and `AGENTS.md` bootstrap are all portable. The gaps are in the implementation layer: directory paths, file extensions, and frontmatter schemas.

---

## Goals

- [ ] `REQ-001` — Preserve full Cursor compatibility: existing Cursor-based workflows (rule loading, skill invocation, `AGENTS.md` bootstrap, SDD lifecycle) must continue to function without behavioral regression
- [ ] `REQ-002` — Enable first-class Cline support: Cline must be able to consume Harness governance rules, invoke Harness workflow skills, and bootstrap from the same `AGENTS.md` as Cursor
- [ ] `REQ-003` — Preserve existing governance workflows: the authority hierarchy, SDD lifecycle, verification governance, documentation routing, and calibration workflow must remain unchanged in principle across all supported tools
- [ ] `REQ-004` — Preserve the existing SDD methodology: the SDD lifecycle, adaptive sizing model, phase ownership, and Definition of Done must remain tool-agnostic
- [ ] `REQ-005` — Maintain a single authoritative documentation model: no duplicate governance models, rule inventories, or skill inventories per tool
- [ ] `REQ-006` — Support tool-specific optimizations without changing Harness behavior: each tool may have tool-specific loading mechanisms, frontmatter schemas, or context management features, but these must not alter the Harness methodology
- [ ] `REQ-007` — Preserve rule content portability: the guardrail text of each rule must remain separable from the tool-specific format (file extension, frontmatter schema) so that it can be consumed by multiple AI tools
- [ ] `REQ-008` — Preserve skill content portability: the workflow procedures in each skill must remain separable from tool-specific path references so that they can be consumed by multiple AI tools
- [ ] `REQ-009` — Maintain `AGENTS.md` as a shared bootstrap artifact: both Cursor and Cline must auto-load `AGENTS.md` as the cross-tool entry point
- [ ] `REQ-010` — Ensure non-conflicting tool coexistence: adding support for a second AI tool must not modify, remove, or interfere with existing Cursor workflows, artifacts, or behavioral expectations
- [ ] `REQ-011` — Preserve backward compatibility of historical artifacts: existing feature SDD instances, verification artifacts, and pilot reports that reference `.cursor/` paths must remain readable and interpretable

---

## Non Goals

This feature does **not** attempt to:

- **Replace Cursor.** The current Cursor implementation is the initial reference implementation of the Harness.
- **Redesign the SDD methodology.** The SDD lifecycle, sizing model, phase ownership, and Definition of Done are tool-agnostic by design and remain unchanged.
- **Change project governance.** The authority hierarchy, ADR policy, verification governance, and documentation routing remain as defined in `sdd-operational.md` and related governance docs.
- **Introduce unnecessary abstractions.** The feature should not create generic adapter layers, plugin systems, or framework-level abstractions unless the Design phase proves they reduce real complexity.
- **Evaluate tools other than Cursor and Cline.** Future extensibility is a principle, not a research target. No third tool is in scope.
- **Implement changes to rules, skills, or documentation.** This Specify phase defines what the feature must accomplish, not how it will be implemented.
- **Modify ADR-003.** ADR disposition is a Design phase decision area, not a Specify outcome.
- **Define migration steps or folder structure.** Implementation strategy belongs to the Design and Tasks phases.
- **Expand Financial features or clinical SQL migration.** This feature is governance-only and does not touch product code.

---

## Functional Requirements

| ID | Requirement | Traceability |
|----|-------------|--------------|
| `REQ-001` | Existing Cursor-based workflows (rule loading via `.cursor/rules/*.mdc`, skill invocation via `.cursor/skills/*/SKILL.md`, `AGENTS.md` bootstrap, SDD lifecycle execution) must continue to function without behavioral regression. | PRD §11; Research RH-5 (Validated); Research F-15 |
| `REQ-002` | Cline must be able to consume Harness governance rules (via `.clinerules/*.md` or equivalent), invoke Harness workflow skills (via `.cline/skills/*/SKILL.md` or equivalent), and bootstrap from the same `AGENTS.md` as Cursor. | PRD §8, §12; Research RH-3 (Validated), RH-4 (Validated); Research F-1, F-3, F-4 |
| `REQ-003` | The authority hierarchy (ADRs > Architecture docs > State > SDD > Rules > Skills), SDD lifecycle, verification governance, documentation routing, and calibration workflow must remain unchanged in principle across all supported tools. | PRD §11; Research F-18; Review §Architectural Ownership Classification |
| `REQ-004` | The SDD methodology (lifecycle phases, adaptive sizing model, phase entry/exit criteria, Definition of Done) must remain tool-agnostic and applicable regardless of which AI tool executes the workflow. | PRD §8; `sdd-operational.md` |
| `REQ-005` | The Harness must maintain a single authoritative documentation model. No duplicate rule inventories, skill inventories, governance strategies, or authority hierarchies per tool. | PRD §11; PM §7 (Risk: Duplicação); Research R4 |
| `REQ-006` | Each supported tool may have tool-specific loading mechanisms, frontmatter schemas, file extensions, or context management features. These tool-specific optimizations must not alter the Harness methodology, governance content, or authority hierarchy. | Research F-6, F-8, F-12; Research §13.2 |
| `REQ-007` | The guardrail text of each rule must remain separable from the tool-specific format (file extension, frontmatter schema) so that it can be consumed by multiple AI tools. Rule content is a Harness asset; rule format is a tool asset. | Research RH-9 (Validated); Research F-16; Research §13.2 |
| `REQ-008` | The workflow procedures in each skill must remain separable from tool-specific path references so that they can be consumed by multiple AI tools. Skill content is a Harness asset; skill format and storage location are tool assets. | Research RH-10 (Validated); Research F-17; Research §13.2 |
| `REQ-009` | `AGENTS.md` must remain the shared cross-tool bootstrap artifact. Both Cursor and Cline must auto-load it without conflict. | Research RH-12 (Validated); Research F-3; Research §13.2 (Reclassified: Shared asset) |
| `REQ-010` | Adding support for a second AI tool must not modify, remove, or interfere with existing Cursor workflows, artifacts, or behavioral expectations. Both tools must coexist in the same repository. | PRD §11; Research RH-5 (Validated); Research F-15 |
| `REQ-011` | Existing feature SDD instances, verification artifacts, and pilot reports that reference `.cursor/` paths must remain readable and interpretable. Historical artifacts should not be rewritten to reflect a multi-tool architecture they predate. | Research RH-13 (Validated); Research §12.2 (Q-21 answered: No) |

---

## Non-Functional Requirements

| ID | Requirement | Rationale |
|----|-------------|-----------|
| `NFR-001` | **Maintainability:** The multi-tool architecture must not create content duplication that leads to drift. If rules or skills are adapted per tool, the adaptation mechanism must preserve a single source of truth for content. | Research R4 (Content duplication drift risk) |
| `NFR-002` | **Extensibility:** The architecture should not preclude adding a third AI tool without redesigning the artifact layer. The Design phase should evaluate whether the approach generalizes beyond Cursor and Cline. | PRD §8; Review NFR-3 |
| `NFR-003` | **Backward compatibility:** Existing Cursor workflows, historical SDD artifacts, and governance documentation references must remain functional and interpretable after any multi-tool evolution. | PRD §11; Research RH-5, RH-13 (Validated) |
| `NFR-004` | **Documentation consistency:** Governance documents should reference tool-agnostic concepts where the concept is Harness-owned and tool-specific details where the implementation is tool-owned. The 43+ `.cursor/` references must be addressed by the Design phase's path reference strategy. | Research F-19; Research DA-4 |
| `NFR-005` | **Governance consistency:** The authority hierarchy, ADR policy, calibration workflow, and knowledge strategy must remain unchanged in principle. No tool-specific governance models. | PRD §11; Research F-18 |
| `NFR-006` | **Minimal duplication:** The architecture must avoid duplicating rule content, skill content, or governance documentation per tool. Content is shared; format and storage may be tool-specific. | PRD §11; PM §7 (Risk); Research R4 |
| `NFR-007` | **Incremental adoption:** The transition from the current Cursor-specific implementation to multi-tool support must be feasible in phases, with each phase preserving Cursor workflows without regression. | PRD §8; Research RH-8 (Validated) |

---

## Architectural Principles

These principles emerged from the Research and guide every future design decision. They are architectural principles, not implementation decisions.

| ID | Principle | Evidence |
|----|-----------|----------|
| `AP-001` | **The Harness is the primary product.** The Harness methodology — authority hierarchy, SDD lifecycle, verification governance, documentation routing, calibration workflow — is the core value. AI tools are consumers of this methodology, not the other way around. | Research F-18; Review §Harness Assets |
| `AP-002` | **AI tools are implementations of the Harness.** Each AI tool (Cursor, Cline, future tools) provides a specific implementation of the Harness's rule loading, skill invocation, and context management. The implementation serves the methodology, not the reverse. | Research §13.2; Review §Tool Assets |
| `AP-003` | **Methodology is shared.** The governance methodology is defined once and applied by all supported tools. There is no per-tool SDD lifecycle, per-tool authority hierarchy, or per-tool verification model. | Research F-18; `sdd-operational.md` |
| `AP-004` | **Tool integrations may be tool-specific.** Each tool may have different directory structures, file formats, frontmatter schemas, and loading mechanisms. Tool-specific integration is expected and acceptable as long as it does not alter the methodology. | Research F-4, F-5, F-6; Research §13.2 |
| `AP-005` | **Optimize implementations, not methodology.** When a tool offers capabilities not present in the current Harness (e.g., Cline's `.clineignore`, Memory Bank, slash commands), the Harness may adopt them as implementation optimizations without changing the methodology. | Research F-8, F-14; Research NQ-1 through NQ-3 |
| `AP-006` | **Preserve backward compatibility.** The current Cursor implementation is the reference architecture. Any evolution must preserve it without regression. Historical artifacts must remain interpretable. | PRD §8, §11; Research RH-5, RH-13 (Validated) |
| `AP-007` | **Avoid unnecessary abstraction.** The feature should not introduce generic adapter layers, plugin systems, or framework-level abstractions unless they reduce real complexity. The Design phase must justify any abstraction against the existing implementation. | PM §7 (Risk: Overengineering); `sdd-operational.md` §Brownfield |

---

## Architectural Vision

The Harness shall evolve into a platform whose governance is independent of the AI tool used to execute it.

The methodology, documentation, and governance remain authoritative.

Each AI tool provides its own optimized implementation while preserving identical behavioral expectations.

This separation between Harness Assets and Tool Assets becomes the fundamental architectural boundary for future evolution.

---

## Assumptions

These assumptions remain valid after the Research:

1. The Harness methodology (authority hierarchy, SDD lifecycle, verification governance, documentation routing, calibration workflow) is tool-agnostic by design and requires no changes in principle. *(Research F-18)*

2. The current Cursor implementation is the reference implementation of the Harness and must be preserved without regression. *(PRD §8; Research RH-5)*

3. Cline and Cursor use non-conflicting directory structures (`.clinerules/` + `.cline/` vs `.cursor/rules/` + `.cursor/skills/`) and can coexist in the same repository. *(Research F-15)*

4. `AGENTS.md` is a shared bootstrap artifact — both tools auto-load it. *(Research F-3, RH-12)*

5. Skill format (`SKILL.md` with `name`/`description` frontmatter) is identical between Cursor and Cline. *(Research F-1, F-2)*

6. Rule content (guardrail text) and skill content (workflow procedures) are pure markdown and portable between tools. *(Research F-16, F-17; RH-9, RH-10)*

7. The AI Harness methodology is defined in tool-agnostic terms in principle, while its current machine-facing implementation uses Cursor-specific conventions. *(PRD §8)*

8. The repository is private, but all patient and clinical information must still be treated as sensitive in all artifacts. *(PRD §8; `security-phi.mdc`)*

9. Memory Bank is a tool-agnostic documentation methodology that could complement the Harness's existing `State.md` + durable documentation pattern, but is not required for multi-tool support. *(Research F-14, §5.5)*

---

## Constraints

1. **No Cursor regression.** Multi-tool evaluation must not break existing Cursor-based workflows. *(PRD §11)*

2. **No duplicated governance.** Multi-tool evaluation must not duplicate rules, skills, or governance documentation per tool. *(PRD §11)*

3. **Preserve authority hierarchy, SDD lifecycle, and verification governance.** These are tool-agnostic by design and must remain unchanged in principle. *(PRD §11)*

4. **ADR-003 is an accepted decision.** ADR-003 establishes `.cursor/` for machine-facing Cursor artifacts. Any disposition (accommodate, revise, or supersede) requires formal ADR governance. This Specify phase does not modify ADR-003. *(Research F-20, RH-7; ADR governance policy)*

5. **No clinical, security, or persistence impact.** This feature is governance-only. It does not touch product code, database schema, API contracts, or patient data. *(Research §14.2)*

6. **No implementation in this phase.** The Specify phase defines what the feature must accomplish. Implementation strategy, migration steps, folder structure, and frontmatter design belong to the Design and Tasks phases. *(Task constraints; `sdd-operational.md` §Specify exit criteria)*

7. **No ADR creation in this phase.** ADR evaluation and creation belong to the Design phase and ADR governance. This specification identifies ADR candidates but does not create or modify ADRs. *(Task constraints; `sdd-operational.md` §ADR Integration Model)*

8. **Treat all patient-identifiable and clinical data as sensitive** in all artifacts, prompts, and outputs, even though this feature does not directly touch clinical data. *(PRD §8; `security-phi.mdc`)*

---

## Design Decision Areas

The following architectural questions remain intentionally unresolved. They are inputs to the Design phase, not answered here. Each decision area is supported by Research evidence but requires architectural judgment that exceeds the Specify phase's scope.

| ID | Decision area | Context | Research evidence |
|----|---------------|---------|-------------------|
| `DDA-001` | **Harness Assets vs Tool Assets** | Should the Harness define a clear boundary between tool-agnostic artifacts (rule content, skill content, `AGENTS.md`) and tool-specific artifacts (file format, frontmatter, storage location)? How should this boundary be expressed in the repository? | Research §13.2; Review §Architectural Ownership Classification; DA-1 |
| `DDA-002` | **ADR strategy** | Should ADR-003 be accommodated (add Cline directories alongside `.cursor/` without modifying the ADR), revised (generalize the taxonomy for multi-tool artifacts), or superseded (new ADR establishing a tool-agnostic artifact layer)? Does multi-tool evaluation require a new ADR? | Research F-20, RH-7 (Partially Supported); Research Q-10, Q-15; Review ADR-GAP-2, ADR-GAP-3 |
| `DDA-003` | **Tool-specific loading mechanisms** | How should tool-specific loading mechanisms (Cursor's `alwaysApply`/`globs` vs Cline's `paths` frontmatter) be reconciled with shared Harness content? Should the Harness define a canonical format, or maintain tool-specific formats with shared content? | Research F-6, DA-2, DA-3; Research RH-2 (Partially Supported), RH-11 (Partially Supported) |
| `DDA-004` | **Frontmatter strategy** | Should the Harness define a frontmatter schema that works across tools, or maintain tool-specific frontmatter? How should `alwaysApply`/`globs` (Cursor) map to `paths`/no-frontmatter (Cline)? | Research F-6, DA-3; Research RH-11; Research NQ-5 |
| `DDA-005` | **Path reference strategy** | Should governance docs reference tool-specific paths (`.cursor/rules/`, `.clinerules/`), tool-agnostic abstractions, or both? How should the 43+ existing `.cursor/` references be addressed? | Research F-19, DA-4; Research RH-6 (Partially Supported); Research Q-22 |
| `DDA-006` | **Documentation taxonomy** | Should the documentation taxonomy (established by ADR-003) be generalized for multi-tool artifacts, or should tool-specific sections coexist? How should the documentation index, rules inventory, and skills strategy be named and organized? | Research F-19, DA-7; Review INDEX-GAP-1, RULES-STRAT-GAP-1 |
| `DDA-007` | **Documentation naming** | Should "Cursor Rules Inventory" (`rules.md`), "Cursor Artifacts" section (`documentation-index.md`), and "Cursor rules/skills" titles (`rules-strategy.md`, `skills-strategy.md`) be renamed to tool-agnostic names? | Research F-19, DA-7; Review INDEX-GAP-1, RULES-STRAT-GAP-1, SKILLS-STRAT-GAP-1 |
| `DDA-008` | **Cross-skill reference strategy** | How should skills reference each other across tool boundaries? Should cross-skill path references (e.g., verifier referencing documentation-update) use relative paths, tool-agnostic abstractions, or tool-specific paths? | Research R9, DA-6; Research NQ-6 |
| `DDA-009` | **Governance evolution** | Should the authority hierarchy explicitly reflect the relationship between Harness assets and tool assets? Should `effective-harness-planning` skill evaluate tool-specific assumptions during harness reviews? | Research Q-17, Q-18; Review §Undetermined Ownership |
| `DDA-010` | **Calibration workflow evolution** | Should the Harness Calibration Workflow be updated to account for multi-tool pilot reports? Should pilot report templates include multi-tool fields (which tool was used, what was observed)? | Research RH-14 (Validated); Research Q-16 |
| `DDA-011` | **Documentation Update evolution** | Should the `documentation-update` skill's path drift check detect tool-specific path references? Should it flag `.cursor/` references that should be tool-agnostic? | Research Q-19; Research DA-4 |
| `DDA-012` | **Context management adoption** | Should the Harness adopt Cline-specific context management features (`.clineignore`, Memory Bank, `/smol`, `/newtask`, `/deep-planning`) as best practices, even when using Cursor? Or should these remain Cline-specific optimizations? | Research F-8, F-14; Research NQ-1 through NQ-3, DA-8 |
| `DDA-013` | **Global vs project scope** | Should the Harness define a global scope for rules and skills (Cline supports `~/.cline/`), or should all Harness artifacts remain project-scoped? | Research F-10; Research NQ-4, DA-9 |
| `DDA-014` | **Toggle governance** | Should the Harness define policy around rule/skill toggling? Cline allows toggling rules and skills individually; Cursor's toggle behavior is not documented. | Research F-11; Research NQ-7, DA-10 |

The Harness Asset / Tool Asset / Shared Asset taxonomy must be formally evaluated during the Design phase and, if confirmed, consolidated through ADR governance as part of the Harness architecture.

---

## Accepted Research Conclusions

These conclusions have been validated by the Research with documented evidence. The Design phase must treat them as established facts, not re-litigate them.

### Validated Hypotheses

| Hypothesis | Status | Key evidence |
|------------|--------|--------------|
| RH-3: The Harness can support at least two AI tools applying the same governance rules | **Validated** | Both tools load rules as persistent context from markdown; both auto-load `AGENTS.md`; rule content is portable markdown; format adaptation needed but governance content is shareable |
| RH-4: The Harness can support at least two AI tools invoking the same workflow skills | **Validated** | Both tools use `SKILL.md` with `name`/`description` frontmatter; both load skills on-demand; only parent directory path differs |
| RH-5: The current Cursor implementation can be preserved without regression | **Validated** | Non-conflicting directories; Cline detects `.cursorrules` and `AGENTS.md` without interfering with `.cursor/`; adding Cline directories does not modify Cursor directories |
| RH-8: An incremental transition is feasible | **Validated** | Directories do not conflict; `AGENTS.md` is shared; skill format is identical; rule content is portable; transition can proceed in phases |
| RH-9: Rule content can be separated from rule format | **Validated** | Rule content is pure markdown after frontmatter; only file extension and frontmatter schema are tool-specific |
| RH-10: Skill content can be separated from skill format | **Validated** | Skill format is identical between tools; content is pure markdown; only cross-skill path references need accommodation |
| RH-12: `AGENTS.md` bootstrap has an equivalent in Cline | **Validated** | Cline explicitly detects `AGENTS.md` as a rule type for cross-tool compatibility |
| RH-13: Existing SDD instances with `.cursor/` references remain interpretable | **Validated** | Historical artifacts document what was done at the time; Cline can read any file via `@` mentions; artifacts should not be rewritten |
| RH-14: The calibration workflow can accommodate multi-tool pilot reports | **Validated** | The calibration workflow is a process, not a tool-specific mechanism; pilot reports are markdown artifacts |

### Partially Supported Hypotheses (Require Design Resolution)

| Hypothesis | Status | Unresolved aspect |
|------------|--------|-------------------|
| RH-1: A shared artifact layer can be defined | **Partially Supported** | Skill content is already shared; rule content is portable; but frontmatter schemas are not interchangeable — Design must resolve format reconciliation |
| RH-2: Tool-specific loading mechanisms can be reconciled without duplication | **Partially Supported** | Loading mechanisms can coexist; content duplication is avoidable in principle; but a single file cannot serve both tools' loading mechanisms without adaptation |
| RH-6: Governance docs can be made consistent without mass rewriting | **Partially Supported** | 43+ references exist in 18+ files; incremental approach is feasible but risks inconsistency during transition; Design must choose path reference strategy |
| RH-7: ADR-003 can be accommodated, revised, or superseded | **Partially Supported** | All three options are feasible; Design must decide which |
| RH-11: `alwaysApply` and `globs` have equivalents in Cline | **Partially Supported** | `alwaysApply: true` → no frontmatter (equivalent); `globs` → `paths` (equivalent); `alwaysApply: true` + `globs` combined → no direct equivalent |

### Key Findings (Summary)

| # | Finding | Type |
|---|---------|------|
| F-1 | Cline and Cursor share identical skill format (`SKILL.md` with `name`/`description` frontmatter) | Documented Fact |
| F-3 | Cline and Cursor both auto-load `AGENTS.md` | Documented Fact |
| F-4 | Cline and Cursor use different rule directories (`.clinerules/` vs `.cursor/rules/`) | Documented Fact |
| F-5 | Cline and Cursor use different rule file extensions (`.md`/`.txt` vs `.mdc`) | Documented Fact |
| F-6 | Cline and Cursor use different frontmatter schemas (`paths` vs `alwaysApply`/`globs`) | Documented Fact |
| F-15 | Cline and Cursor directories do not conflict — both can coexist in the same repository | Inference |
| F-16 | Rule content (guardrail text) is pure markdown — portable between tools | Architectural Assessment |
| F-17 | Skill content (workflow procedures) is pure markdown — portable between tools | Architectural Assessment |
| F-18 | The Harness governance methodology is tool-agnostic by design and requires no changes in principle | Architectural Assessment |
| F-19 | 43+ `.cursor/` references in 18+ governance files need accommodation for multi-tool clarity | Architectural Assessment |
| F-20 | ADR-003 does not prohibit adding Cline directories — it only establishes `.cursor/` for Cursor artifacts | Architectural Assessment |

### Architectural Ownership Classification (Research-Validated)

| Capability | Classification |
|------------|----------------|
| Rule content (guardrail text) | **Harness asset** — tool-agnostic markdown |
| Skill content (workflow procedures) | **Harness asset** — tool-agnostic markdown |
| `AGENTS.md` bootstrap | **Shared asset** — both tools auto-load it |
| Context loading strategy | **Harness asset** (strategy) + **Tool asset** (mechanism) |
| Rule scoping intent (always-on vs file-scoped) | **Harness asset** (intent) + **Tool asset** (frontmatter schema) |
| Rule format (`.mdc`, frontmatter schema) | **Tool asset** |
| Skill format (`SKILL.md` structure) | **Tool asset** — but format is shared between Cursor and Cline |
| Rule storage location | **Tool asset** |
| Skill storage location | **Tool asset** |
| `.clineignore` | **Tool asset** (Cline-specific) |
| Memory Bank | **Tool-agnostic methodology** — could be Harness asset if adopted |

---

## Acceptance Criteria

- [ ] `REQ-001` — Cursor continues to load rules from `.cursor/rules/*.mdc`, invoke skills from `.cursor/skills/*/SKILL.md`, and bootstrap from `AGENTS.md` without behavioral regression; verified by Design-phase compatibility assessment and Execute-phase smoke check
- [ ] `REQ-002` — Cline can load Harness governance rules (adapted to `.clinerules/*.md` or equivalent), invoke Harness workflow skills (from `.cline/skills/*/SKILL.md` or equivalent), and bootstrap from the same `AGENTS.md`; verified by Design-phase compatibility assessment
- [ ] `REQ-003` — Authority hierarchy, SDD lifecycle, verification governance, documentation routing, and calibration workflow remain unchanged in principle; verified by documentation review at Verify
- [ ] `REQ-004` — SDD methodology (lifecycle, sizing, phase ownership, Definition of Done) remains tool-agnostic; verified by documentation review at Verify
- [ ] `REQ-005` — No duplicate rule inventories, skill inventories, or governance models per tool; single authoritative documentation model preserved; verified by documentation review at Verify
- [ ] `REQ-006` — Tool-specific optimizations (frontmatter, loading mechanisms, context features) do not alter Harness methodology or governance content; verified by Design-phase boundary assessment
- [ ] `REQ-007` — Rule guardrail text remains separable from tool-specific format and consumable by multiple AI tools; verified by content review at Verify
- [ ] `REQ-008` — Skill workflow procedures remain separable from tool-specific path references and consumable by multiple AI tools; verified by content review at Verify
- [ ] `REQ-009` — AGENTS.md remains the shared bootstrap artifact for both Cursor and Cline; verified by compliance with each tool's documented auto-loading behavior
- [ ] `REQ-010` — Adding support for a second AI tool does not modify, remove, or interfere with existing Cursor workflows, artifacts, or behavioral expectations; verified by repository structure review at Verify
- [ ] `REQ-011` — Existing feature SDD instances and verification artifacts with `.cursor/` references remain readable and interpretable; not rewritten; verified by historical artifact review at Verify

---

## Users And Scenarios

| Actor | Scenario | Outcome |
|-------|----------|---------|
| Developer using Cursor | Opens project in Cursor, writes code, invokes skills | Cursor loads rules from `.cursor/rules/`, skills from `.cursor/skills/`, `AGENTS.md` as bootstrap — no behavioral change from current workflow |
| Developer using Cline | Opens project in Cline, writes code, invokes skills | Cline loads rules from `.clinerules/` (or equivalent), skills from `.cline/skills/` (or equivalent), `AGENTS.md` as bootstrap — same governance applies |
| Developer switching tools | Uses Cursor for one session, Cline for another | Both tools apply the same governance rules, invoke the same workflow skills, and bootstrap from the same `AGENTS.md`; no governance gap between sessions |
| Agent (Cursor) | Follows SDD lifecycle, invokes verifier skill, applies rules | Cursor-specific implementation of Harness works without regression |
| Agent (Cline) | Follows SDD lifecycle, invokes verifier skill, applies rules | Cline-specific implementation of Harness applies the same governance content; workflow procedures are identical |
| Governance maintainer | Updates a rule or skill | Updates content once; both tools consume the updated content through their respective format/directory mechanisms; no content duplication |
| Verifier | Reviews multi-tool feature | Confirms Cursor compatibility, Cline compatibility, governance consistency, no duplication, backward compatibility of historical artifacts |
| Future tool evaluator | Evaluates a third AI tool | Architecture is general enough to evaluate a third tool without redesigning the artifact layer (NFR-002) |

---

## Sizing

| Field | Decision |
|-------|----------|
| Size | **Medium** |
| Rationale | The feature does not involve code changes, database changes, or API changes. It involves rule/skill format adaptation (documentation/markdown), path reference updates in governance docs, possible ADR revision or new ADR, and documentation strategy decisions. No clinical, security, or persistence impact. The Research recommended Medium sizing. |
| Migration vertical note | Not a SQL migration vertical — governance/documentation feature |
| Required phases | Specify / Design / Tasks / SDD Pre-Execution Review / Execute / Verify / Documentation Follow-Up / Reporting / Teacher Guide when warranted |
| Escalation triggers | Design reveals that ADR-003 supersession is required; Design reveals that the feature requires Large sizing due to documentation volume; Design reveals that tool-specific format reconciliation is more complex than Research indicated |

---

## Open Questions

These questions are unresolved and belong to the Design phase. They correspond to the Design Decision Areas above.

| ID | Question | Options | Decision | Date | Owner |
|----|----------|---------|----------|------|-------|
| DQ-001 | Should the Harness use tool-specific directories, a shared directory, or a canonical directory with tool-specific adapters? | A) Tool-specific (`/.cursor/` + `/.clinerules/`) B) Shared canonical C) Canonical + adapters | Open | | Design |
| DQ-002 | Should the Harness maintain `.mdc` for Cursor and `.md` for Cline, or define a canonical format? | A) Tool-specific formats B) Canonical `.md` format C) Canonical with generation | Open | | Design |
| DQ-003 | Should the Harness define a frontmatter schema that works across tools, or maintain tool-specific frontmatter? | A) Cross-tool schema B) Tool-specific frontmatter C) Canonical with mapping | Open | | Design |
| DQ-004 | Should governance docs reference tool-specific paths, tool-agnostic abstractions, or both? | A) Tool-specific B) Tool-agnostic C) Both with disambiguation | Open | | Design |
| DQ-005 | Should ADR-003 be accommodated, revised, or superseded? | A) Accommodate B) Revise C) Supersede with new ADR | Open | | Design + ADR governance |
| DQ-006 | Should `rules.md`, `rules-strategy.md`, `skills-strategy.md`, and `documentation-index.md` be renamed to tool-agnostic names? | A) Rename all B) Rename some C) Keep with disambiguation notes | Open | | Design |
| DQ-007 | How should cross-skill path references work across tool boundaries? | A) Relative paths B) Tool-agnostic abstractions C) Tool-specific paths with documentation | Open | | Design |
| DQ-008 | Should the Harness adopt Cline's `.clineignore`, Memory Bank, or slash commands as best practices? | A) Adopt all B) Adopt selectively C) Keep Cline-specific | Open | | Design |
| DQ-009 | Should the Harness define a global scope for rules and skills? | A) Project-only B) Global + project C) Defer | Open | | Design |
| DQ-010 | Should the Harness define policy around rule/skill toggling? | A) Define toggle policy B) Leave tool-specific C) Defer | Open | | Design |
| DQ-011 | Does multi-tool evaluation require a new ADR? | A) New ADR B) ADR-003 revision C) No ADR (operational update) | Open | | Design + ADR governance |

---

## Domain Language

This feature uses Harness governance terminology, not clinical domain terminology. Terms align with `Documentation/AI-Harness/Harness-Design/harness-architecture.md` and `sdd-operational.md`.

- **Harness** — The AI-assisted development governance system: authority hierarchy, SDD lifecycle, verification governance, documentation routing, calibration workflow, rules, skills, and review prompts. The Harness is the primary product.
- **AI tool** — A specific AI assistant implementation that consumes the Harness (e.g., Cursor, Cline). AI tools are implementations of the Harness.
- **Reference implementation** — The current Cursor-based implementation of the Harness. It is the baseline that must be preserved without regression.
- **Harness asset** — A capability or artifact owned by the Harness methodology, defined in tool-agnostic terms (e.g., rule content, skill content, authority hierarchy, SDD lifecycle).
- **Tool asset** — A capability or artifact owned by a specific AI tool's implementation (e.g., rule format `.mdc`, frontmatter schema, storage location `.cursor/rules/`).
- **Shared asset** — A capability or artifact that is both Harness-owned and consumed identically by all tools (e.g., `AGENTS.md`).
- **Rule** — A concise persistent guardrail loaded as context by an AI tool. Rule content is a Harness asset; rule format and storage are tool assets.
- **Skill** — A repeatable workflow loaded on-demand by an AI tool. Skill content is a Harness asset; skill storage location is a tool asset. Skill format (`SKILL.md`) is shared between Cursor and Cline.
- **Frontmatter** — YAML metadata at the top of a rule or skill file. Frontmatter schema differs between tools (`alwaysApply`/`globs` in Cursor; `paths` in Cline).
- **Bootstrap** — The `AGENTS.md` file, auto-loaded by both Cursor and Cline as the cross-tool entry point.
- **SDD lifecycle** — Research → Specify → Design → Tasks → SDD Pre-Execution Review → Execute → Verify → Documentation Follow-Up → Reporting → Teacher Guide. Tool-agnostic by design.
- **Authority hierarchy** — ADRs > Architecture docs > State > SDD > Rules > Skills. Tool-agnostic by design.

---

## Security And PHI

- Security/PHI review needed: **No** (for this Specify phase)
- Rationale: This feature is governance-only. It does not touch patient identity, clinical data, files, logs, API exposure, auth, or secrets. The feature evaluates AI tool compatibility and documentation structure. No PHI is present in rules, skills, or governance documentation. The `security-phi.mdc` rule remains applicable to all Harness artifacts regardless of which AI tool consumes them — this is a governance invariant, not a new security concern.

---

## Initial Verification Expectations

| Requirement | Expected evidence |
|-------------|-------------------|
| `REQ-001` | Design-phase Cursor compatibility assessment; Execute-phase confirmation that `.cursor/` artifacts are unchanged |
| `REQ-002` | Design-phase Cline compatibility assessment; Execute-phase confirmation that Cline can consume adapted rules and skills |
| `REQ-003` | Documentation review confirming authority hierarchy, SDD lifecycle, verification governance, documentation routing, and calibration workflow are unchanged in principle |
| `REQ-004` | Documentation review confirming SDD methodology is tool-agnostic |
| `REQ-005` | Documentation review confirming no duplicate inventories or governance models |
| `REQ-006` | Design-phase boundary assessment confirming tool-specific optimizations do not alter methodology |
| `REQ-007` | Content review confirming rule guardrail text is separable from tool-specific format and consumable by multiple AI tools |
| `REQ-008` | Content review confirming skill workflow procedures are separable from tool-specific path references and consumable by multiple AI tools |
| `REQ-009` | Both tools' documented auto-loading behavior for `AGENTS.md` |
| `REQ-010` | Repository structure review confirming non-conflicting coexistence |
| `REQ-011` | Historical artifact review confirming existing SDD instances remain interpretable and unmodified |

Verifier selects final gates. Documentation review sensor recommended for governance consistency. No SQL, API, build, or clinical gates apply.

---

## ADR Evaluation

| Candidate | Trigger | Status |
|-----------|---------|--------|
| ADR-003 disposition (accommodate, revise, or supersede) | Durable decision establishing `.cursor/` for machine-facing artifacts; multi-tool architecture may require taxonomy generalization | **Open — Design phase + ADR governance** |
| New ADR for multi-tool Harness architecture | Documentation taxonomy change, cross-context ownership of tool-specific vs Harness assets | **Open — Design phase evaluation** |
| ADR for canonical rule/skill format | Durable decision on whether the Harness defines a canonical format or maintains tool-specific formats | **Open — Design phase evaluation** |

This Specify phase does not create, modify, or supersede any ADR. ADR candidates are identified for the Design phase to evaluate through formal ADR governance.

---

## Out of Scope

The following items belong to future phases and are explicitly excluded from this specification:

### Implementation details
- Folder structure decisions (shared vs tool-specific directories)
- Frontmatter schema design (canonical vs tool-specific)
- Adapter or generation mechanism design
- File naming conventions for multi-tool artifacts

### Migration strategy
- Steps for transitioning from Cursor-only to multi-tool
- Sequencing of documentation updates
- Rollback strategy (Research confirms rollback = remove `.clinerules/` and `.cline/` directories, but formalizing this is a Design/Tasks responsibility)

### Adapter implementation
- Whether adapters, symlinks, generation scripts, or manual copies are used
- How content synchronization works between tool-specific formats

### Frontmatter implementation
- Specific YAML fields for cross-tool compatibility
- Mapping logic between `alwaysApply`/`globs` and `paths`

### Tooling automation
- Scripts or tools for generating tool-specific files from canonical sources
- CI checks for path drift or format consistency
- Automated synchronization between tool-specific directories

### ADR creation
- Drafting ADR text for ADR-003 disposition
- Drafting ADR text for multi-tool architecture decisions

### Product code changes
- Any changes to `DocAPI/`, `DocFront.Web/`, database schema, API contracts, or test code

### Clinical SQL migration
- Resuming Prontuario, Agendamento, or Atendimento Workflow work (paused separately; resumes after Harness evaluation reaches a decision point)

---

## References

- `Documentation/State.md`
- `AGENTS.md`
- `Documentation/Product/PM_DocOrgano.md`
- `Documentation/Product/PRD.md`
- `Documentation/SDD/ai-harness-multi-tool/research.md`
- `Documentation/AI-Harness/research/review-tool-agnostic-harness.md`
- `Documentation/AI-Harness/Harness-Design/sdd-operational.md`
- `Documentation/AI-Harness/Harness-Design/harness-architecture.md`
- `Documentation/AI-Harness/Harness-Design/rules-strategy.md`
- `Documentation/AI-Harness/Harness-Design/skills-strategy.md`
- `Documentation/Architecture/ADR/ADR-003-documentation-taxonomy.md`
- `Documentation/AI-Harness/template/sdd/specify.md`