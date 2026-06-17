# Skills Strategy

## Purpose

This document defines how Doc Organo should use Cursor skills as reusable task workflows. Skills should encode repeatable procedures that are too long for rules but too project-specific to leave as ad hoc prompts.

Skills are the harness layer for "how to do this kind of work here." They should reduce context drift, preserve domain-specific judgment, and turn repeated planning or implementation patterns into reliable agent behavior.

## Responsibilities

Skills should own:

- Multi-step workflows with a clear trigger.
- Task-specific context loading sequences.
- Repeatable analysis formats.
- Risk-aware procedures for high-impact areas such as SQL migration, Legacy rule porting, harness planning, and decomposition.
- Verifier workflows when gate selection, review prompts, skipped-gate reasoning, and residual risk summaries need repeatable structure.
- Progressive disclosure: start with a short checklist, then point to deeper docs or files only when needed.
- Output formats that create useful markdown handoffs.

Skills should not own:

- Always-on constraints. Those belong in rules.
- Stable project facts. Those belong in `AGENTS.md`.
- Current branch truth. That belongs in `Documentation/State.md`.
- Accepted architecture decisions. Those belong in ADRs.
- ADR creation policy. That belongs in `harness-architecture.md`, though skills may ask whether the policy is triggered.
- Feature-specific implementation plans. Those belong in SDD feature folders.
- Broad methodology research. That belongs in `Documentation/AI-Harness/research/playbook.md` or `Documentation/AI-Harness/research/AI-Research.md`.

## Current Skill Inventory

| Skill | Purpose | Current responsibility | Assessment |
|-------|---------|------------------------|------------|
| `doc-organo-context` | Session orientation | Read State, AGENTS, Domain Overview for domain work; summarize stack and do-not rules | Good bootstrap skill. Should stay short and point outward. |
| `codebase-decomposition` | DDD and technical debt analysis | Map bounded context, aggregates, entry points, Legacy references, suggested next slice | Strong fit for refactors, migration planning, and SDD design inputs. |
| `effective-harness-planning` | Harness review and planning | Evaluate AGENTS, rules, skills, PRD, ADRs, SDD templates, State, MCPs, verification | Strong fit for this architecture work. Current duplicate locations should be resolved later. |
| `verifier` | Verification workflow | Select gates, apply review sensors, record skipped gates, summarize residual risk, identify follow-up needs; Verify is not terminal for SDD-backed work | Operational; aligned with Verification Governance |
| `documentation-update` | Documentation routing and mandatory Follow-Up execution | Execute post-Verify checklist (State.md priority, PM, migration-sql, runbook); route additional follow-up; path drift and authority validation | Operational; aligned with sdd-operational Documentation Follow-Up |
| `sql-migration-workflow` | SQL migration workflow | Guide EF and SQL migration changes | Existing workflow for persistence-focused slices. |
| `not-a-teacher` | Teacher Guide generation | Transform verified implementation into pedagogical artifacts per Knowledge Strategy | Operational workflow for post-Reporting learning artifacts; subordinate to code and governance. |

## Missing Or Candidate Skills

Candidate skills should be created when a workflow repeats or carries enough risk to deserve a standard path.

| Candidate skill | Purpose | Inputs | Outputs | Priority rationale |
|-----------------|---------|--------|---------|-------------------|
| `sql-migration-slice` refinement | Refine the existing SQL migration workflow after real pilot usage | `State.md`, `migration-sql.md`, relevant repository/controller/entity, EF configs, tests | Per-aggregate migration plan, verification checklist, State update notes | Active epic; reduces big-bang migration risk |
| `port-legacy-atendimento` | Port `ValidacaoEtapa*` behavior from Legacy Sheets to domain/use cases | Targeted Legacy methods, Domain Overview, Atendimento entity/use cases, characterization tests | Rule map, test cases, destination design | High clinical risk; Legacy is behavior spec |
| `verifier` refinement | Improve the existing verifier skill after real pilot usage | Diff, SDD tasks, PR template, review prompts, commands, `State.md`, ADRs | Gate result, skipped-gate reasons, residual risks, State/ADR/doc follow-up signal | Verification workflow exists; pilot evidence should drive refinements |
| `security-phi-review` | Apply security review workflow before PR or after sensitive changes | Diff, `security-phi-review.md`, PHI rule, PRD constraints | Findings by severity and remediation guidance | Security risks are high and clinical data is sensitive |
| `api-contract-review` | Review endpoint/DTO/ID/response changes | Controllers, DTOs, front services, future API contract doc | Contract drift findings | Useful after API contract doc exists |
| `docs-taxonomy-review` | Check path consistency and documentation placement | Documentation index, ADR-003, touched docs | Documentation consistency findings | Useful while path drift remains |

Skills to defer:

- Custom MCP orchestration skills until GitHub/browser usage and local verification are stable.
- Financial-domain skills until clinical SQL migration is stable.
- Advanced E2E/browser test skills until Blazor/API flows stabilize.
- External SDK automation skills until the local harness has proven patterns.

## Inputs

Skills should consume:

- `Documentation/State.md` for current status.
- `AGENTS.md` for static context.
- Relevant rules for non-negotiable guardrails.
- Domain, architecture, technical, product, and ADR docs based on task type.
- Targeted code files and Legacy methods.
- SDD artifacts when the work is feature-sized.
- Review prompts when the skill includes verification or review.
- The ADR Governance Policy when a workflow may expose durable decisions.
- The Artifact Authority Hierarchy when a workflow needs to resolve conflicting docs.

## Outputs

Skills should produce:

- Structured analysis.
- Focused implementation plans.
- SDD-ready research summaries.
- Review findings.
- Verification checklists.
- Verification summaries with skipped-gate reasons and residual risks when acting as verifier.
- State update recommendations.
- ADR/doc/rule/skill follow-up recommendations when a workflow discovers durable or recurring governance gaps.

They should avoid producing:

- Full code rewrites by default.
- Unbounded backlog lists.
- Duplicated research history.
- Operational doc rewrites unless explicitly requested.

## Update Triggers

Create a skill when:

- A workflow has been repeated enough to standardize.
- A workflow is risky and benefits from a checklist.
- A rule is becoming too long or procedural.
- A recurring task requires multiple docs, code searches, and output structure.
- A new harness phase needs consistent agent behavior.

Update a skill when:

- Canonical paths change.
- The workflow changes after a successful implementation.
- A review finding shows the skill missed a risk.
- A new verification gate becomes mandatory.
- A related rule or ADR changes.
- The verifier role changes from conceptual design to operational checklist, skill, or CI-backed workflow.
- The artifact authority hierarchy changes.

Retire or merge a skill when:

- It duplicates another skill.
- Its workflow is obsolete.
- Its instructions are better represented as a short rule.
- Its output is no longer used in planning or implementation.

## Relationship With Other Harness Components

| Component | Relationship |
|-----------|--------------|
| `AGENTS.md` | `AGENTS.md` routes agents to relevant skills; skills execute the workflow. |
| `.cursor/rules/` | Rules define guardrails skills must obey; skills should not weaken always-on constraints. |
| `Documentation/State.md` | Skills start from State and may recommend State updates after meaningful work. |
| Review prompts | Skills can orchestrate review prompts and convert them into structured findings. |
| SDD | Skills can generate research/design inputs for SDD, but SDD owns feature-specific scope and tasks. |
| ADRs | Skills may identify that ADR criteria are met, but ADRs own accepted durable decisions. |
| Verifier role | A verifier skill can operationalize the role by selecting gates, applying review prompts, recording skipped gates, and summarizing risk. |
| MCPs | Skills can explain when to use MCPs after MCP usage matures. |
| Auxiliary docs | Skills load only the docs needed for the current workflow. |

## Verifier Skill Design Target

The verifier skill now exists and should be refined only after real reviews reveal workflow gaps. It should not replace CI or human review; it should make verification decisions explicit and repeatable.

The skill should:

- Start from the diff or planned change scope.
- Read active SDD tasks when present.
- Map touched areas to applicable gates: build, tests, SQL integration, Swagger/API smoke, Blazor smoke, review prompts, and PR checklist.
- Require security/PHI review for clinical data, identity fields, files, logs, prompts, generated examples, auth assumptions, secrets, or API exposure.
- Record skipped gates with reasons.
- Summarize residual risk and unresolved blockers.
- Recommend State, ADR, documentation, rule, or skill follow-up when evidence changes project truth.

The skill should not:

- Claim success only because automated tests pass.
- Create ADRs automatically without applying ADR governance.
- Rewrite operational artifacts during verification.
- Expand into a full release process before CI and PR routines are stable.

## Skill Design Standard

A Doc Organo skill should include:

- Clear trigger: when to use it.
- Minimal start-here checklist.
- Required context files.
- Targeted search/read guidance.
- Domain-specific constraints.
- Output format.
- Anti-patterns.
- Verification or handoff expectations.

Recommended size:

- Short skill: under 150 lines for simple workflows.
- Standard skill: 300-500 lines for high-value procedures.
- If a skill exceeds this range, split it or move background material into durable docs.

## Recommendations For Future Skill Governance

These recommendations are for a later operational update pass:

- Resolve the duplicate `effective-harness-planning` skill location.
- Add a skill inventory to `Documentation/AI-Harness/documentation-index.md` or keep `rules.md` focused only on rules and create a separate skills index.
- Create new skills only after one real usage path is clear.
- Prefer `sql-migration-slice` and `port-legacy-atendimento` before broader automation.
- Create or pilot a verifier skill before adding more review prompts, so prompts are actually used.
