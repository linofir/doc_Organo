# Documentation Review Report — AI Tool Agnostic Harness Feature

> **Review type:** Documentation analysis only (no implementation, no migration, no file changes).  
> **Purpose:** Prepare planning documentation for the upcoming Research phase of the AI Tool Agnostic Harness feature.  
> **Input for:** Cline Capability Research and subsequent Harness evolution evaluation.

---

## Executive Summary

The Harness is a well-documented, internally coherent system implemented around Cursor as its current reference implementation. Its governance methodology — authority hierarchy, SDD lifecycle, verification ownership, documentation routing, calibration workflow — is defined in tool-agnostic terms in principle, while its machine-facing artifacts (rules and skills) are implemented using Cursor-specific conventions, formats, and paths.

This review evaluates whether the planning and governance documentation is prepared to support the evaluation of evolving the Harness to support multiple AI tools (Cursor, Cline, and potentially others).

The documentation does not yet define this feature in any planning artifact. The Harness architecture, governance strategies, and operational workflows reference Cursor-specific paths and conventions throughout (43+ explicit `.cursor/` path references across 18+ files, and an accepted ADR that establishes `.cursor/` as the home for machine-facing artifacts). These are not defects — they are the current reference implementation choices that the Research phase must evaluate.

The existing governance philosophy is documented with sufficient clarity to serve as a baseline for Research. However, before Research can begin, the planning documentation (PRD, PM, State) must establish the problem definition, scope, stakeholders, success criteria, and open questions for this feature. This review identifies all gaps, assumptions, and recommended planning updates without making architectural decisions.

---

## Findings

### Current State of the Documentation

The Doc Organo AI Harness is a mature, multi-layered system with clear governance. The following table describes the current implementation state of each layer:

| Layer | Artifacts | Current implementation |
|-------|-----------|----------------------|
| Machine-facing rules | `.cursor/rules/*.mdc` (6 rules) | Cursor-specific: `.mdc` format, `alwaysApply`/`glob` frontmatter, `.cursor/` path |
| Machine-facing skills | `.cursor/skills/*/SKILL.md` (7 skills) | Cursor-specific: `name`/`description` frontmatter, `.cursor/` path |
| Agent bootstrap | `AGENTS.md` | References `.cursor/rules/` and `.cursor/skills/` as machine-facing asset locations |
| Operational truth | `Documentation/State.md` | Harness files section links to `.cursor/rules/` and `.cursor/skills/` |
| Harness architecture | `harness-architecture.md` | Defines `.cursor/rules/` and `.cursor/skills/` as core harness components |
| Harness strategies | `agents-strategy.md`, `rules-strategy.md`, `skills-strategy.md` | Titled and scoped to "Cursor rules" and "Cursor skills" |
| Contribution workflow | `CONTRIBUTING-AI.md` | 6 skill path references to `.cursor/skills/` |
| Documentation index | `documentation-index.md` | "Cursor Artifacts" section; `rules.md` described as "Cursor rule inventory" |
| Rules inventory | `rules.md` | Titled "Cursor Rules Inventory" |
| ADRs | `ADR-003` | Accepted decision establishing `.cursor/` for machine-facing Cursor artifacts |
| SDD templates | `verification.md`, `teacher-guide.md`, `governance-improvement-plan.md` | Skill path references to `.cursor/skills/` |
| Feature SDD instances | `verification.md` files in Paciente, Atendimento SDDs | Verifier skill path references to `.cursor/skills/` |
| Security docs | `plano-seguranca-informacao.md` | References `security-phi.mdc` path |
| Runbook | `runbook.md` | Lists "Cursor / VS Code" as development tooling |
| Pilot reports | `sdd-pilot-report-v1.0.md` | References `.cursor/skills/not-a-teacher/` creation |

### Architectural Ownership Classification

The following classification distinguishes what the Harness methodology owns from what is implemented through Cursor-specific conventions. The "Undetermined Ownership" category identifies capabilities whose ownership can only be resolved during Research.

#### Harness Assets

These capabilities belong to the Harness methodology and are defined in tool-agnostic terms:

- Authority hierarchy (ADRs > Architecture docs > State > SDD > Rules > Skills)
- SDD lifecycle (Research → Plan → SDD → Execute → Verify → Documentation Follow-Up → Reporting → Teacher Guide)
- Verification governance (gate selection, review sensors, residual risk, skipped-gate reasoning)
- Documentation routing and path-drift checks
- Harness calibration workflow (pilot reports, governance improvement plans, finding maturity)
- Knowledge transfer strategy (Teacher Guides, concept classification, study paths)
- Artifact ownership model (who owns what truth)
- Adaptive sizing model (Small/Medium/Large/Complex)
- Requirement traceability model
- Brownfield and Legacy behavior migration expectations
- ADR governance policy (creation criteria, conflict resolution, escalation)
- Definition of Done for SDD-backed work
- Reporting strategy (session handoff, feature reports, lessons learned)

#### Tool Assets

These capabilities are currently implemented through Cursor-specific conventions:

| Capability | Current Cursor implementation |
|------------|------------------------------|
| Rule storage | `.cursor/rules/` directory |
| Rule format | `.mdc` files with YAML frontmatter (`alwaysApply`, `description`, `glob`) |
| Rule loading | Cursor auto-injects `alwaysApply: true` rules; `glob` rules apply by file scope |
| Skill storage | `.cursor/skills/` directory with `SKILL.md` per skill |
| Skill format | `SKILL.md` with `name`/`description` frontmatter |
| Skill discovery | Cursor discovers skills by `name`/`description` frontmatter |
| Skill invocation | User or agent triggers a skill by name in the Cursor interface |
| Bootstrap loading | `AGENTS.md` auto-loaded by Cursor as project instructions |

#### Undetermined Ownership

These capabilities contain both Harness methodology and tool-specific implementation. Research must determine which aspects are Harness-owned and which are tool-owned:

| Capability | Why ownership is undetermined |
|------------|------------------------------|
| Rule content (the guardrail text) | The text expresses Harness guardrails, but the format (`.mdc`, frontmatter) is Cursor-specific |
| Skill content (the workflow procedures) | The procedures are Harness workflows, but the `SKILL.md` format and discovery mechanism are Cursor-specific |
| Path references in governance docs | Documentation points to `.cursor/` paths; whether the canonical path should be tool-agnostic or tool-specific is unresolved |
| Context loading strategy | The Harness defines what context to load, but the loading mechanism (auto-injection, on-demand) may differ per tool |
| Rule scoping (always-on vs. file-scoped) | The Harness defines which rules should always apply and which should be scoped, but the scoping mechanism (`alwaysApply`/`glob`) is Cursor-specific |

---

## Documentation Gaps

Grouped by artifact. These describe what is missing from each document relative to this feature.

### PRD (`Documentation/Product/PRD.md`)

| Gap ID | Description |
|--------|-------------|
| PRD-GAP-1 | No problem definition for AI tool coupling or the need to evaluate multi-tool support |
| PRD-GAP-2 | No architectural motivation for evaluating whether the Harness can evolve beyond a single tool |
| PRD-GAP-3 | No mention of Cline or multi-tool support anywhere in the PRD |
| PRD-GAP-4 | No user stories or personas for developers using different AI tools |
| PRD-GAP-5 | No success metrics for multi-tool Harness support |
| PRD-GAP-6 | No constraints related to preserving the current Cursor implementation while evaluating Cline |
| PRD-GAP-7 | No open questions about Cline capabilities, rule/skill format compatibility, or evaluation strategy |
| PRD-GAP-8 | No reference links to this feature's research or SDD artifacts |

### PM (`Documentation/Product/PM_DocOrgano.md`)

| Gap ID | Description |
|--------|-------------|
| PM-GAP-1 | No workstream, epic, or feature item for AI Tool Agnostic Harness evaluation |
| PM-GAP-2 | No priority assignment (P0/P1/P2) for this initiative |
| PM-GAP-3 | No risk entry for tool-specific implementation creating evaluation or evolution debt |
| PM-GAP-4 | No sequencing or dependency mapping relative to clinical SQL migration work |
| PM-GAP-5 | WS08 ("Implementação, Demonstração e Evolução Assistida por IA") does not mention tool-agnostic evaluation |
| PM-GAP-6 | "Iniciativa de Governança — Adoção do AI Harness no MVP2" does not mention multi-tool support |
| PM-GAP-7 | No definition of what "done" means for this feature in the PM context |

### State (`Documentation/State.md`)

| Gap ID | Description |
|--------|-------------|
| STATE-GAP-1 | Current branch is `feature/harness` but no explanation of what this branch targets |
| STATE-GAP-2 | No mention of AI Tool Agnostic Harness initiative in active epic, next steps, or recent decisions |
| STATE-GAP-3 | No blocker or prerequisite related to Cline research or multi-tool evaluation |
| STATE-GAP-4 | Harness files section lists `.cursor/rules/` and `.cursor/skills/` as the machine-facing assets without noting they are Cursor-specific |

### Roadmap (`Documentation/Product/RoadMap.md`)

| Gap ID | Description |
|--------|-------------|
| ROAD-GAP-1 | §3 "Horizonte AI Harness Evolution" does not mention multi-tool support or tool-agnostic architecture evaluation |
| ROAD-GAP-2 | §3.2 "Future Evolution" lists "Integrações com ferramentas externas" but this refers to management/review tools, not AI tool coexistence |
| ROAD-GAP-3 | No horizon for Harness portability or multi-tool methodology |

### ADR Index (`Documentation/Architecture/ADR/`)

| Gap ID | Description |
|--------|-------------|
| ADR-GAP-1 | ADR-003 establishes `.cursor/` for machine-facing Cursor artifacts — this durable decision is relevant context for evaluating multi-tool support |
| ADR-GAP-2 | No ADR proposing or evaluating multi-tool Harness architecture |
| ADR-GAP-3 | No ADR evaluating whether ADR-003 requires revision, supersession, or accommodation |
| ADR-GAP-4 | No ADR index or README summarizing all ADRs for easy review of tool-related decisions |

### Documentation Index (`Documentation/AI-Harness/documentation-index.md`)

| Gap ID | Description |
|--------|-------------|
| INDEX-GAP-1 | "Cursor Artifacts" section names the section and its contents as Cursor-specific |
| INDEX-GAP-2 | `rules.md` described as "Cursor rule inventory and governance metadata" |
| INDEX-GAP-3 | No section or placeholder for multi-tool or Cline-specific artifacts |
| INDEX-GAP-4 | No navigation for a multi-tool Harness architecture document if one is created |

### Harness Architecture (`harness-architecture.md`)

| Gap ID | Description |
|--------|-------------|
| ARCH-GAP-1 | Component Responsibilities table defines `.cursor/rules/` and `.cursor/skills/` as core harness components without distinguishing Harness methodology from tool implementation |
| ARCH-GAP-2 | Context Loading Policy references `.cursor/rules/*` and `.cursor/skills/*` directly |
| ARCH-GAP-3 | No discussion of how the Harness methodology relates to the tool-specific implementation of its components |
| ARCH-GAP-4 | No discussion of how the Harness would evaluate supporting multiple AI tools with different rule/skill loading mechanisms |

### Rules Strategy (`rules-strategy.md`)

| Gap ID | Description |
|--------|-------------|
| RULES-STRAT-GAP-1 | Title and content are scoped to "Cursor rules" without distinguishing rule ownership from rule format |
| RULES-STRAT-GAP-2 | Rule inventory table lists `.mdc` files with `alwaysApply` and `glob` columns — Cursor-specific metadata |
| RULES-STRAT-GAP-3 | No discussion of rule format portability or tool-agnostic rule definition |

### Skills Strategy (`skills-strategy.md`)

| Gap ID | Description |
|--------|-------------|
| SKILLS-STRAT-GAP-1 | Title and content are scoped to "Cursor skills" without distinguishing skill ownership from skill format |
| SKILLS-STRAT-GAP-2 | No discussion of skill format portability or tool-agnostic skill definition |
| SKILLS-STRAT-GAP-3 | Update triggers reference `.cursor/rules/` and `.cursor/skills/` paths directly |

### AGENTS.md

| Gap ID | Description |
|--------|-------------|
| AGENTS-GAP-1 | Read First table and Repository Map reference `.cursor/` as the machine-facing asset location |
| AGENTS-GAP-2 | Harness Workflow section references `.cursor/rules/` and `.cursor/skills/` |
| AGENTS-GAP-3 | No mention of Cline or multi-tool context loading |

### CONTRIBUTING-AI.md

| Gap ID | Description |
|--------|-------------|
| CONTRIB-GAP-1 | Phase Ownership table references `.cursor/skills/verifier/SKILL.md` and `.cursor/skills/documentation-update/SKILL.md` directly |
| CONTRIB-GAP-2 | Before Coding section references `.cursor/skills/doc-organo-context/SKILL.md` |
| CONTRIB-GAP-3 | Documentation Follow-Up section references `.cursor/skills/documentation-update/SKILL.md` |
| CONTRIB-GAP-4 | Teacher Guide section references `.cursor/skills/not-a-teacher/SKILL.md` |
| CONTRIB-GAP-5 | Research section references `.cursor/skills/codebase-decomposition/SKILL.md` |

### SDD Templates

| Gap ID | Description |
|--------|-------------|
| TEMPLATE-GAP-1 | `verification.md` template references `.cursor/skills/verifier/SKILL.md` |
| TEMPLATE-GAP-2 | `teacher-guide.md` template references `.cursor/skills/not-a-teacher/SKILL.md` |
| TEMPLATE-GAP-3 | `governance-improvement-plan.md` template references `.cursor/rules/` and `.cursor/skills/` |

### Feature SDD Instances

| Gap ID | Description |
|--------|-------------|
| SDD-INST-GAP-1 | `paciente-sql-stabilization/verification.md` references `.cursor/skills/verifier/SKILL.md` |
| SDD-INST-GAP-2 | `atendimento-minimal-sql-stabilization/verification.md` references `.cursor/skills/verifier/SKILL.md` |
| SDD-INST-GAP-3 | `atendimento-minimal-sql-stabilization/reports/session-handoff.md` references `.cursor/skills/not-a-teacher/SKILL.md` |

### Security Documentation

| Gap ID | Description |
|--------|-------------|
| SEC-GAP-1 | `plano-seguranca-informacao.md` references `.cursor/rules/security-phi.mdc` as the operational PHI rule |
| SEC-GAP-2 | "IA/Cursor" column header in security documentation assumes Cursor as the only AI tool |

### Runbook

| Gap ID | Description |
|--------|-------------|
| RUNBOOK-GAP-1 | `runbook.md` lists "Cursor / VS Code" as the development tooling, implying Cursor is the primary AI interface |

---

## Recommended PM Updates

These are recommended updates to `Documentation/Product/PM_DocOrgano.md`. **Do not apply them.** These recommendations identify missing planning work only; they do not prescribe architectural solutions.

1. **Add a feature item** for "AI Tool Agnostic Harness" with:
   - Problem statement: the Harness is implemented using Cursor-specific conventions; the project needs to evaluate whether the architecture can support multiple AI tools.
   - Objective: evaluate the feasibility of evolving the Harness to support multiple AI tools while preserving existing workflows, governance, and documentation.
   - Priority: define whether this is P1 or P2 relative to clinical SQL migration.

2. **Add risk entries** to §7 Gestão de Riscos for:
   - Tool-specific implementation creating evaluation or evolution debt.
   - Potential for duplicated documentation and workflows if tool-specific copies are created instead of a shared approach.
   - Potential governance inconsistency if rules/skills diverge across tools.
   - ADR-003 relevance: the accepted ADR establishes `.cursor/` for Cursor artifacts; multi-tool evaluation may require ADR review.

3. **Add sequencing context** clarifying whether this initiative:
   - Blocks clinical SQL migration.
   - Can run in parallel with Prontuario/Agendamento work.
   - Requires its own SDD given cross-layer governance impact.

4. **Update WS08 or the "Iniciativa de Governança"** to mention multi-tool evaluation as a future harness capability, even if deferred.

5. **Add "AI Harness Evolution — Multi-Tool Evaluation" as a PM-tracked item** so it has an owner, status, and expected result.

---

## Recommended PRD Updates

These are recommended updates to `Documentation/Product/PRD.md`. **Do not apply them.** These recommendations identify missing planning work only; they do not prescribe architectural solutions.

1. **Add a problem statement** (in §2 Problem or a new subsection) describing:
   - The Harness is currently implemented using Cursor-specific conventions.
   - The project needs to evaluate whether the Harness can support additional AI tools (e.g., Cline).

2. **Add architectural motivation** explaining why this evaluation is necessary:
   - The Harness methodology is defined in tool-agnostic terms in principle.
   - The project should evaluate whether the implementation can evolve to support multiple tools.

3. **Add assumptions** (in §8 Assumptions) for:
   - Cline (and future tools) may have different rule/skill loading mechanisms than Cursor.
   - The current Cursor implementation must continue to function during and after any evolution.
   - The evaluation should preserve the authority hierarchy, SDD lifecycle, and verification governance.

4. **Add constraints** (in §11 Constraints) for:
   - The evaluation must not break existing Cursor-based workflows.
   - The evaluation should not duplicate rules, skills, or governance documentation per tool.
   - The evaluation must preserve the authority hierarchy, SDD lifecycle, and verification governance.

5. **Add success metrics** (in §10 Success Metrics) for:
   - Evidence that multiple AI tools can apply the same Harness governance.
   - No duplicated rule or skill content per tool.
   - Governance documentation references are consistent with the evaluated architecture.

6. **Add open questions** (in §12 Open Questions) for:
   - What rule/skill formats can multiple AI tools consume?
   - Does Cline support concepts equivalent to Cursor's `alwaysApply` rules and `glob` scoping?
   - Does ADR-003 require revision, supersession, or accommodation?

7. **Add reference links** to the research and SDD artifacts for this feature once created.

---

## Recommended State Updates

These are recommended updates to `Documentation/State.md`. **Do not apply them.** These recommendations identify missing planning work only; they do not prescribe architectural solutions.

1. **Add the AI Tool Agnostic Harness initiative** to the active epic or next steps section, explaining that the current `feature/harness` branch targets this evaluation.

2. **Add a recent decision entry** noting that a documentation review for multi-tool Harness has been completed and Research is the next phase.

3. **Update the "Harness files" section** to note that `.cursor/rules/` and `.cursor/skills/` are Cursor-specific implementations of the Harness methodology.

4. **Add a blocker or prerequisite** if one exists (e.g., "Cline capability research required before architecture evaluation can proceed").

5. **Add next steps** pointing to the research review report and the upcoming Cline Capability Research.

---

## Missing Requirements

These requirements describe what the feature must address. They are framed as evaluation objectives, not prescribed solutions.

### Functional Requirements

| ID | Requirement | Rationale |
|----|-------------|-----------|
| FR-1 | The feature must evaluate whether the Harness can define rule and skill artifacts that are not bound to a single AI tool's directory structure or file format. | Currently, `.cursor/rules/` and `.cursor/skills/` are the only storage and loading mechanism. |
| FR-2 | The feature must evaluate whether at least two AI tools (Cursor and Cline) can apply the same governance rules. | The feature objective is multi-tool coexistence. |
| FR-3 | The feature must evaluate whether at least two AI tools can invoke the same workflow skills. | Skills encode repeatable procedures that may need to work across tools. |
| FR-4 | The feature must preserve the existing SDD lifecycle, verification governance, documentation routing, and authority hierarchy across all supported tools. | The governance philosophy is the core value of the Harness. |
| FR-5 | The feature must evaluate how documentation consistency can be maintained when multiple tools are supported. | 43+ `.cursor/` references currently exist in governance docs. |
| FR-6 | The feature must evaluate how tool-specific loading mechanisms can be reconciled with shared Harness artifacts. | Cursor uses `.mdc` with `alwaysApply`/`glob`; Cline may use a different mechanism. |
| FR-7 | The feature must ensure that adding a new AI tool does not require duplicating rule or skill content. | Duplicated content creates drift risk and governance inconsistency. |
| FR-8 | The feature must evaluate a transition path from the current Cursor-specific implementation to any proposed architecture without breaking existing workflows. | The evolution is incremental, not a rewrite. |

### Non-Functional Requirements

| ID | Requirement | Rationale |
|----|-------------|-----------|
| NFR-1 | **Compatibility with Cursor:** Existing Cursor-based workflows (rule loading, skill invocation, SDD lifecycle) must continue to function without regression. | Cursor is the current reference implementation; breaking it blocks all current work. |
| NFR-2 | **Compatibility with Cline:** The evaluation must determine whether Cline can consume Harness rules and skills in its native context-loading mechanism. | Cline is the immediate target for multi-tool evaluation. |
| NFR-3 | **Future extensibility:** The evaluation must consider whether the architecture could allow adding a third AI tool without redesigning the artifact layer. | The objective is long-term multi-tool support, not just Cursor+Cline. |
| NFR-4 | **Maintainability:** The evaluation must consider how governance documentation can reference tool-agnostic concepts while preserving tool-specific implementation details where needed. | Mixed tool-specific and tool-agnostic content in governance docs creates drift risk. |
| NFR-5 | **Documentation consistency:** The evaluation must consider whether governance documents should reference tool-agnostic or tool-specific paths. | Currently, `harness-architecture.md` defines `.cursor/rules/` as a core component. |
| NFR-6 | **Minimal transition effort:** The evaluation must consider the scope of changes needed and whether an incremental approach is feasible. | 43+ references and 18+ files are affected. |
| NFR-7 | **Backward compatibility:** Existing feature SDD instances, verification artifacts, and pilot reports that reference `.cursor/` paths must remain readable and interpretable. | Historical artifacts should not be invalidated. |
| NFR-8 | **Governance preservation:** The authority hierarchy, ADR policy, calibration workflow, and knowledge strategy must remain unchanged in principle. | These are tool-agnostic by design and should not be re-litigated. |

---

## Architectural Assumptions

Every Cursor-specific assumption currently embedded in the Harness documentation. These are documented as observations of the current implementation, not as defects.

### Path Assumptions

| ID | Assumption | Location |
|----|------------|----------|
| A-1 | `.cursor/rules/` is the canonical directory for machine-facing rules. | ADR-003, AGENTS.md, State.md, harness-architecture.md, documentation-index.md, rules.md, rules-strategy.md, skills-strategy.md, agents-strategy.md, CONTRIBUTING-AI.md, multiple SDD templates and instances |
| A-2 | `.cursor/skills/` is the canonical directory for machine-facing skills. | Same as A-1 |
| A-3 | `.cursor/` is the root for all Cursor-specific artifacts. | ADR-003, documentation-index.md |
| A-4 | Skill paths are referenced as `.cursor/skills/<skill-name>/SKILL.md` in governance docs, templates, and contribution guides. | CONTRIBUTING-AI.md (6 references), verification.md template, teacher-guide.md template, governance-improvement-plan.md template, SDD verification instances, session-handoff.md |
| A-5 | Rule paths are referenced as `.cursor/rules/<rule-name>.mdc` in security documentation. | plano-seguranca-informacao.md |

### Rule Loading Assumptions

| ID | Assumption | Location |
|----|------------|----------|
| A-6 | Rules use `.mdc` file format with YAML frontmatter. | `.cursor/rules/*.mdc` files |
| A-7 | `alwaysApply: true` frontmatter field controls whether a rule is always loaded into agent context. | `security-phi.mdc`, `token-economy.mdc`, `update-doc.mdc` |
| A-8 | `glob` frontmatter field (or description) controls file-scoped rule application (e.g., `DocAPI/**/*.cs`). | `backend-architecture.mdc`, `ef-migrations.mdc`, `blazor-front.mdc` |
| A-9 | The AI tool auto-discovers and auto-loads rules from `.cursor/rules/` without explicit user invocation. | Implied by "Always or glob-scoped" in effective-harness-planning SKILL.md, "always-on" language in rules-strategy.md |
| A-10 | Rules are "injected into agent context" by the tool. | harness-architecture.md Component Responsibilities table |

### Skill Invocation Assumptions

| ID | Assumption | Location |
|----|------------|----------|
| A-11 | Skills use `SKILL.md` format with `name` and `description` frontmatter fields. | All `.cursor/skills/*/SKILL.md` files |
| A-12 | The AI tool discovers skills by `name`/`description` frontmatter and presents them to the user or agent. | Implied by skills-strategy.md "Load by task" and effective-harness-planning "Load policy" |
| A-13 | Skills are loaded on-demand by the user or agent triggering them, not always-on. | effective-harness-planning SKILL.md Harness Map table, skills-strategy.md |
| A-14 | The AI tool's skill system handles the loading and execution of skill instructions. | Implied by all skill references in governance docs |

### Documentation Reference Assumptions

| ID | Assumption | Location |
|----|------------|----------|
| A-15 | `rules.md` is the "Cursor Rules Inventory" — its title and content assume Cursor as the sole rule system. | rules.md |
| A-16 | `rules-strategy.md` defines how to use "Cursor rules" — its title and content assume Cursor. | rules-strategy.md |
| A-17 | `skills-strategy.md` defines how to use "Cursor skills" — its title and content assume Cursor. | skills-strategy.md |
| A-18 | `documentation-index.md` has a "Cursor Artifacts" section — it assumes Cursor is the only AI tool with machine-facing artifacts. | documentation-index.md |
| A-19 | `harness-architecture.md` defines `.cursor/rules/` and `.cursor/skills/` as core Harness components in the Component Responsibilities table. | harness-architecture.md |
| A-20 | ADR-003 states "Keep `.cursor/` for machine-facing Cursor artifacts only" — a durable decision establishing the current reference implementation. | ADR-003 |

### Workflow Assumptions

| ID | Assumption | Location |
|----|------------|----------|
| A-21 | The Harness lifecycle (Research → Plan → SDD → Execute → Verify → ...) is executed within Cursor sessions. | CONTRIBUTING-AI.md, sdd-operational.md (implicit) |
| A-22 | The verifier skill is invoked via `.cursor/skills/verifier/SKILL.md` — no alternative invocation path is documented. | verification.md template, SDD verification instances, verification-governance.md |
| A-23 | The documentation-update skill is invoked via `.cursor/skills/documentation-update/SKILL.md`. | CONTRIBUTING-AI.md, documentation-update SKILL.md self-references |
| A-24 | The not-a-teacher skill is invoked via `.cursor/skills/not-a-teacher/SKILL.md`. | CONTRIBUTING-AI.md, knowledge-strategy.md, teacher-guide.md template |
| A-25 | The doc-organo-context skill is the guided orientation entry point via `.cursor/skills/doc-organo-context/SKILL.md`. | CONTRIBUTING-AI.md |
| A-26 | The codebase-decomposition skill is invoked via `.cursor/skills/codebase-decomposition/SKILL.md`. | CONTRIBUTING-AI.md |

### Context Assumptions

| ID | Assumption | Location |
|----|------------|----------|
| A-27 | `AGENTS.md` is auto-loaded by Cursor as project instructions. | Implied by AGENTS.md being the bootstrap file, Cursor's standard behavior |
| A-28 | Rules with `alwaysApply: true` are auto-injected by Cursor — the Harness assumes certain rules are always present in agent context. | security-phi.mdc, token-economy.mdc, update-doc.mdc, rules-strategy.md |
| A-29 | The Harness assumes a single AI tool context window — no concept of tool-specific context loading differences. | harness-architecture.md Context Loading Policy, sdd-operational.md Context Loading Strategy |
| A-30 | The runbook lists "Cursor / VS Code" as the development tooling — implying Cursor is the expected AI interface. | runbook.md |

---

## Research Hypotheses

The following hypotheses represent architectural questions that should be validated during the Research phase. They are derived from observations in this review but are **not validated** here. Each hypothesis is pending Research.

| ID | Description | Current Status |
|----|-------------|----------------|
| RH-1 | A shared artifact layer for rules and skills can be defined that is not bound to a single AI tool's directory structure or file format. | Pending Research |
| RH-2 | Tool-specific loading mechanisms can be reconciled with shared Harness artifacts without duplicating content. | Pending Research |
| RH-3 | The Harness can support at least two AI tools (Cursor and Cline) applying the same governance rules. | Pending Research |
| RH-4 | The Harness can support at least two AI tools invoking the same workflow skills. | Pending Research |
| RH-5 | The current Cursor implementation can be preserved without regression while additional tool support is evaluated. | Pending Research |
| RH-6 | Governance documentation can be made consistent with a multi-tool architecture without mass path rewriting. | Pending Research |
| RH-7 | ADR-003 can be accommodated, revised, or superseded to support multi-tool architecture. | Pending Research |
| RH-8 | An incremental transition from the current Cursor-specific implementation to a multi-tool architecture is feasible. | Pending Research |
| RH-9 | The rule content (guardrail text) can be separated from the rule format (`.mdc`, frontmatter) to enable format portability. | Pending Research |
| RH-10 | The skill content (workflow procedures) can be separated from the skill format (`SKILL.md`, frontmatter) to enable format portability. | Pending Research |
| RH-11 | The `alwaysApply` and `glob` scoping concepts have equivalents or alternatives in Cline. | Pending Research |
| RH-12 | The `AGENTS.md` bootstrap loading mechanism has an equivalent or alternative in Cline. | Pending Research |
| RH-13 | Existing feature SDD instances and verification artifacts with `.cursor/` references can remain interpretable in a multi-tool architecture. | Pending Research |
| RH-14 | The Harness calibration workflow can accommodate multi-tool pilot reports. | Pending Research |

---

## Research Inputs

The following sources should guide the upcoming Research phase.

### Primary Sources

| Source | Purpose |
|--------|---------|
| Official Cline documentation | Understand Cline's capabilities for rule loading, skill/workflow invocation, context management, and project configuration |

### Secondary Sources

| Source | Purpose |
|--------|---------|
| Current Cursor implementation (`.cursor/rules/`, `.cursor/skills/`) | Reference implementation of the Harness machine-facing layer |
| Existing Rules (6 `.mdc` files) | Understand what guardrails the Harness enforces and how they are formatted |
| Existing Skills (7 `SKILL.md` files) | Understand what workflows the Harness encodes and how they are structured |
| ADRs (ADR-001 through ADR-006) | Understand durable decisions, especially ADR-003 (documentation taxonomy) and ADR-005 (Legacy Sheets behavior reference) |
| Harness Architecture (`harness-architecture.md`) | Understand the target architecture and component responsibilities |
| Harness Strategies (`agents-strategy.md`, `rules-strategy.md`, `skills-strategy.md`) | Understand ownership models for rules, skills, and agent bootstrap |
| SDD Operational Governance (`sdd-operational.md`) | Understand the lifecycle, sizing, and ownership model that must be preserved |
| This review report | Catalogue of gaps, assumptions, hypotheses, and open questions |

---

## Open Questions

Unresolved questions that should be answered during the Research phase.

### Cline Capability Questions

| ID | Question |
|----|----------|
| Q-1 | Does Cline support a concept equivalent to Cursor's `.cursor/rules/` directory for persistent rule loading? |
| Q-2 | Does Cline support `alwaysApply`-style always-on rules, or does it require explicit invocation for all guidance? |
| Q-3 | Does Cline support `glob`-based file scoping for rules (e.g., rules that apply only to `DocAPI/**/*.cs`)? |
| Q-4 | Does Cline have a skill/workflow system equivalent to Cursor's `.cursor/skills/` with `SKILL.md` discovery? |
| Q-5 | What file format does Cline expect for rules and skills? Can it consume `.mdc` files or `SKILL.md` files? |
| Q-6 | Does Cline auto-load `AGENTS.md` or an equivalent bootstrap file? |
| Q-7 | How does Cline handle context loading — does it have a token budget or context window management mechanism? |
| Q-8 | Can Cline and Cursor coexist in the same repository without conflicting machine-facing artifacts? |

### Architecture Questions

| ID | Question |
|----|----------|
| Q-9 | Should the Harness distinguish between tool-agnostic artifacts and tool-specific implementations? |
| Q-10 | Should ADR-003 be revised, superseded, or left in place? |
| Q-11 | Should the `rules.md` inventory be renamed from "Cursor Rules Inventory" to a tool-agnostic name? |
| Q-12 | Should `rules-strategy.md` and `skills-strategy.md` be rewritten to be tool-agnostic, or should tool-specific strategy docs coexist? |
| Q-13 | How should tool-specific loading mechanisms be reconciled with shared Harness content? |
| Q-14 | Should the Harness define a canonical rule/skill format that tools consume, or should each tool have its own format with shared content? |

### Governance Questions

| ID | Question |
|----|----------|
| Q-15 | Does multi-tool evaluation require an ADR, or is it an operational update within the scope of ADR-003? |
| Q-16 | Should the Harness Calibration Workflow be updated to account for multi-tool pilot reports? |
| Q-17 | How should the authority hierarchy reflect the relationship between Harness assets and tool assets? |
| Q-18 | Should the `effective-harness-planning` skill be updated to evaluate tool-specific assumptions during harness reviews? |
| Q-19 | Should the `documentation-update` skill's path drift check be updated to detect tool-specific path references? |

### Transition Questions

| ID | Question |
|----|----------|
| Q-20 | What is the minimal set of changes needed to evaluate multi-tool support without breaking Cursor workflows? |
| Q-21 | Should existing SDD instances (Paciente, Atendimento) have their `.cursor/` references updated, or should they remain as historical artifacts? |
| Q-22 | Should the 43+ `.cursor/` references be updated, or should the architecture evolve to make them consistent by design? |
| Q-23 | What is the rollback strategy if multi-tool evaluation reveals incompatibility? |
| Q-24 | Can the evaluation be done incrementally, or must it be done atomically? |

---

## Research Success Criteria

The upcoming Research phase can be considered complete when the following criteria are met:

| ID | Criterion |
|----|-----------|
| RSC-1 | Cline's rule loading, skill/workflow invocation, and context management capabilities are documented with evidence from official Cline documentation. |
| RSC-2 | The compatibility between Cline's capabilities and the current Harness rule/skill artifacts is evaluated and documented. |
| RSC-3 | The set of capabilities classified as Harness Assets, Tool Assets, and Undetermined Ownership (see Architectural Ownership Classification) is validated or revised based on Cline capabilities. |
| RSC-4 | Each Research Hypothesis (RH-1 through RH-14) is evaluated and marked as validated, invalidated, or requires further investigation. |
| RSC-5 | The implications of ADR-003 for multi-tool architecture are evaluated and documented, including whether revision, supersession, or accommodation is recommended. |
| RSC-6 | The Research artifact identifies which Open Questions (Q-1 through Q-24) are answered, partially answered, or remain open. |
| RSC-7 | The Research artifact recommends whether the feature should proceed to SDD phase and, if so, what sizing is appropriate. |

---

## Research Readiness Assessment

### Current Maturity Level

The Harness documentation demonstrates a **mature, well-structured governance system** implemented around Cursor as its reference implementation. The governance methodology is documented with sufficient clarity to serve as a baseline for evaluating multi-tool evolution. The Cursor-specific implementation choices are identifiable and catalogued in this review.

However, the planning documentation (PRD, PM, State, Roadmap) does not yet define this feature. This is the expected state for a new initiative that has not yet entered the planning phase.

### Prerequisites Before Research Begins

The following planning updates are the expected prerequisite before Research begins. They do not require architectural decisions — they establish the scope and context for Research:

1. **PRD must define the problem.** The PRD currently has no mention of tool coupling, Cline, or multi-tool evaluation. Without a problem definition, Research has no scope boundary.

2. **PM must add a feature item with priority and sequencing.** The PM has no workstream, epic, or feature entry for this initiative. Without a PM item, Research has no operational context or priority.

3. **State.md must record the initiative.** The current `feature/harness` branch is unexplained in State.md. Without State context, Research cannot determine current operational truth.

4. **ADR-003 relevance must be acknowledged.** ADR-003 establishes `.cursor/` for Cursor artifacts. Research must know whether this ADR is in scope for evaluation. At minimum, the planning docs must flag this as an open question.

5. **Success criteria must be defined.** Without success criteria, Research cannot evaluate whether its findings are sufficient.

6. **Stakeholders must be identified.** Research needs to know who the current Cursor users are, who the prospective Cline users are, and who owns the Harness governance decisions.

### What Is Already Sufficient

- The Harness governance philosophy (authority hierarchy, SDD lifecycle, verification, documentation routing) is well-documented and defined in tool-agnostic terms.
- The Cursor-specific implementation choices are identifiable and catalogued in this review.
- The Harness Design docs provide a clear architecture baseline.
- The calibration workflow (pilot reports, governance improvement plans) provides a mechanism for evolving the Harness safely.
- The Architectural Ownership Classification in this report distinguishes Harness Assets from Tool Assets, providing a starting point for Research.

### Recommended Research Scope

Once the PM, PRD, and State updates are made (as recommended above), the Research phase should:

1. Investigate Cline's capabilities for rule loading, skill invocation, and context management (Q-1 through Q-8).
2. Evaluate ADR-003 implications (Q-10, Q-15).
3. Evaluate each Research Hypothesis (RH-1 through RH-14).
4. Validate or revise the Architectural Ownership Classification.
5. Produce a research artifact under `Documentation/AI-Harness/research/` with findings and recommendations for the SDD phase.

---

## Summary

The Doc Organo AI Harness is a well-engineered system that has matured through multiple calibration waves. Its governance methodology is documented in tool-agnostic terms and provides a strong baseline for evaluating multi-tool evolution. The current implementation uses Cursor-specific conventions for its machine-facing artifacts — this is the reference implementation, not a defect.

The planning documentation does not yet define this feature. Before Research can begin, the PRD, PM, and State must be updated to establish the problem, scope, stakeholders, success criteria, and open questions. This review provides the complete catalogue of gaps, assumptions, hypotheses, ownership classification, and recommended planning updates needed to make those updates.

The output of this review should serve as the input for the upcoming **Cline Capability Research**, which will evaluate the feasibility of evolving the Harness to support multiple AI tools.