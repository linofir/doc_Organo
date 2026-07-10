# SDD Operational Governance

## Purpose

This document defines how Spec-Driven Development operates inside the Doc Organo AI Harness. It is the authoritative governance source for when SDD is required, how deeply it should be applied, what each SDD phase owns, and how SDD integrates with verification, documentation, ADR, architecture, and testing workflows.

SDD is the feature-level planning contract between product or architecture intent and implementation. It does not replace `AGENTS.md`, `Documentation/State.md`, ADRs, Documentation Update, Verifier, rules, skills, review prompts, product docs, architecture docs, or technical docs. It coordinates with them while preserving their ownership boundaries.

This document focuses on governance expectations, not templates, schemas, tooling, automation, MCP strategy, sub-agent orchestration, CI/CD architecture, or future skill implementation.

## Governance Model

The operational model establishes these foundations:

- SDD owns feature-specific scope, design, task planning, and verification expectations.
- SDD is required by risk and size, not for every change.
- `specify.md`, `design.md`, and `tasks.md` have distinct responsibilities.
- Verification, testing, ADR evaluation, and documentation follow-up are part of completion.
- Legacy Codebase behavior, SQL migration, clinical/security risk, and cross-layer changes are strong SDD triggers.
- Adaptive workflow depth prevents low-risk work from inheriting unnecessary ceremony while keeping risky work explicit.
- Phase entry and exit criteria prevent agents from claiming completion without evidence.
- Context Acquisition Governance defines expected context depth per phase and governs how context is expanded incrementally, preventing context exhaustion during execution.
- SDD hands off to existing governance workflows instead of replacing them.

## Conflict Analysis

SDD must respect the existing authority hierarchy:

- Accepted ADRs own durable architecture decisions and override active SDD artifacts.
- `Documentation/State.md` owns current branch, runtime status, blockers, and next steps.
- Product docs own product intent, backlog sequencing, and roadmap horizons.
- Architecture and Technical docs own durable explanation of the current model.
- Verifier owns gate selection, review sensor application, skipped-gate recording, residual risk, and completion judgment.
- Documentation Update owns routing, path drift checks, and documentation authority validation.
- Rules own concise persistent guardrails.
- Skills own repeatable workflows.
- Review prompts are sensors, not governance authorities.

An active SDD owns feature-specific truth after it is created: scope, acceptance criteria, design, tasks, and expected verification. If an active SDD conflicts with an accepted ADR, the ADR wins and the SDD must be revised or an ADR change must be proposed. If an active SDD conflicts with `Documentation/State.md`, decide whether the conflict is stale feature planning or stale operational truth; update the owning artifact rather than carrying both claims forward.

## Escalation Policy

The workflow must pause for human review when a conflict cannot be resolved by the existing authority hierarchy or when resolving it would change durable project direction. SDD should make conflicts visible; it should not silently resolve architectural, operational, or verification uncertainty.

Escalation is required when:

- SDD scope, design, tasks, or verification expectations conflict with an accepted ADR.
- SDD claims conflict with `Documentation/State.md` and it is unclear which artifact is stale.
- Design choices conflict with documented architecture, approved domain models, or bounded-context ownership.
- Requirements conflict with technical constraints, security requirements, data constraints, or migration constraints.
- Legacy Codebase behavior conflicts with ADRs, documented business rules, security requirements, or approved domain models.
- Verification fails and the correct fix, rollback, scope change, or accepted residual risk is unclear.
- A decision would preserve, adapt, or abandon material Legacy Codebase behavior without clear authorization.

Standard escalation outcome:

- **Conflict Detected:** State the conflicting artifacts, requirements, code evidence, or verification results.
- **Options:** List viable paths, including revising SDD, changing implementation, proposing an ADR, changing scope, or accepting residual risk.
- **Trade-offs:** Explain consequences for architecture, domain behavior, security/PHI, migration, testing, and delivery.
- **Recommendation:** Provide the preferred path and why it best preserves project governance.
- **Required Human Decision:** State the exact decision needed before the workflow can proceed.

After escalation is resolved, update the owning artifact. Do not bury the decision only in chat history.

## Recommended Structure

Use this operational structure for SDD governance:

1. Adaptive sizing determines process depth.
2. Specify captures what is needed and why.
3. Design captures how the solution should work when technical judgment is material.
4. Tasks capture executable implementation slices when sequencing, dependencies, or verification expectations are non-trivial.
5. SDD Pre-Execution Review resolves ambiguities, validates prerequisites, and confirms Execute readiness before coding.
6. Execute implements the accepted scope and records deviations.
7. Verify determines whether completion evidence is sufficient.
8. Documentation Follow-Up executes mandatory project documentation updates identified by Verify.
9. Reporting produces session handoff and feature report after operational truth is synchronized.
10. Teacher Guide generation transfers verified implementation knowledge when warranted.
11. Documentation Update and ADR governance handle routing and durable follow-up beyond the mandatory checklist.

The canonical SDD lifecycle is:

```text
Specify -> Design -> Tasks -> SDD Pre-Execution Review -> Execute -> Verify
  -> Documentation Follow-Up -> Reporting -> Teacher Guide
```

For Large and Complex SDD-backed work, SDD Pre-Execution Review and the post-Verify chain are mandatory phases, not optional refinements.

Phases may be intentionally simplified or skipped based on sizing, but the lifecycle responsibility does not disappear. The missing responsibility must be carried by a lighter artifact, an inline plan, or verifier evidence. Small and Medium work may inline SDD Review checks before Execute and may combine Documentation Follow-Up with Reporting when risk is low.

## Adaptive Sizing Model

Process depth is determined by complexity, risk, uncertainty, and blast radius, not by a fixed pipeline.

| Size | Typical scope | Specify | Design | Tasks | Execute | Verify |
|------|---------------|---------|--------|-------|---------|--------|
| Small | Up to about 3 files; simple bug fix; configuration update; typo; low-risk adjustment | Inline statement of intent is enough | Skip unless risk appears | Skip; list atomic steps inline before editing | Required | Required, often lightweight |
| Medium | Clear feature or behavior change with limited scope and known implementation path | Required, may be brief | May be skipped if architecture is straightforward and existing patterns apply | May be skipped if there are 3 or fewer obvious steps | Required | Required |
| Large | Multi-component feature; cross-layer work; persistence/API/UI/domain interaction; significant implementation effort | Full `specify.md` required | Full `design.md` required | Full `tasks.md` required | Required | Required |
| Complex | Ambiguity, architectural uncertainty, new domain knowledge, security/PHI sensitivity, Legacy Codebase behavior migration, irreversible decisions, or significant risk | Full `specify.md` required, including assumptions and open questions | Full `design.md` required, including risks and ADR candidates | Full `tasks.md` required, including dependencies and verification expectations | Required with scope monitoring | Required with explicit residual risk |

For **Large** and **Complex** SDD-backed work, the sizing table above covers Specify through Verify. The same sizes also require, unless explicitly skipped with rationale:

- SDD Pre-Execution Review before Execute.
- Documentation Follow-Up after Verify.
- Reporting after Documentation Follow-Up.
- Teacher Guide after Reporting when knowledge strategy criteria apply.

Sizing must be conservative for brownfield clinical work. A change that appears small by file count can be Medium, Large, or Complex if it touches clinical behavior, PHI/security, persistence, Legacy Codebase behavior, schema, API contracts, bounded-context ownership, or ADR criteria.

For the first SQL migration vertical pilot in a bounded context, prefer **Large** sizing even when implementation complexity appears modest. Large sizing reflects process validation, Legacy behavior characterization, cross-layer verification, and harness calibration — not code volume alone.

### Required Phase Rules

- Specify is always required as a responsibility. For Small work, it can be a short inline intent statement instead of `specify.md`.
- `specify.md` is required for Medium work unless the user request already provides unambiguous scope and acceptance expectations.
- Full `specify.md` is mandatory for Large and Complex work.
- `design.md` may be skipped only when the solution follows an existing local pattern, has no architecture decision, has no new integration point, and has no meaningful risk or uncertainty.
- `design.md` is mandatory for Large and Complex work, persistence changes, API contract changes, cross-context ownership changes, Legacy Codebase behavior migration, and likely ADR candidates.
- `tasks.md` may be skipped only when implementation can be expressed as 3 or fewer atomic steps with no hidden dependencies.
- `tasks.md` is mandatory for Large and Complex work, multi-session work, dependency-heavy work, parallelizable work, or work where verification expectations need to be explicit before implementation.

## Workflow Lifecycle

| Phase | Purpose | Ownership | Expected outputs | Handoff responsibility |
|-------|---------|-----------|------------------|------------------------|
| Specify | Define what is needed, why it matters, boundaries, assumptions, constraints, and acceptance criteria | SDD owns feature scope; Product and Domain docs provide inputs | Inline intent or `specify.md`; requirements; out-of-scope; assumptions; open questions; acceptance criteria; Execution Prerequisites when Large or Complex | Hand Design enough scope to choose an implementation approach; hand Verify acceptance criteria |
| Design | Define how the solution should work and how it fits Doc Organo architecture | SDD owns feature design; ADRs own durable decisions; Architecture/Technical docs own durable explanations | Inline design note or `design.md`; affected components; integration points; risks; testing approach; Runtime Validation Environment when relevant; ADR candidates | Hand Tasks an implementable approach; hand SDD Review outcome-oriented design and unresolved API contract risks |
| Tasks | Convert accepted scope and design into executable slices | SDD owns feature task plan; tasks do not create new scope | Inline atomic steps or `tasks.md`; TASK / VP / DF lists; sequencing; dependencies; requirement mapping; verification expectations; Execution Boundary | Hand SDD Review a reviewable plan; hand Execute clear implementation slices after review exit |
| SDD Pre-Execution Review | Resolve ambiguities, validate prerequisites, confirm scope boundaries, and gate Execute readiness | SDD Review session owns review findings; active SDD owns resulting updates | Review findings; resolved API contracts; confirmed Execution Prerequisites; updated SDD artifacts when needed; explicit Execute readiness decision | Hand Execute only after exit criteria pass; hand Verify updated acceptance criteria and expected evidence |
| Execute | Implement the scoped change using focused context | Implementation session owns code/doc edits within accepted scope | Code, tests, migration artifacts; recorded deviations when scope changes; no verification artifacts, reporting, or project doc updates unless explicitly in scope | Hand Verify diff, tests, skipped checks, deviations, and any SDD updates needed |
| Verify | Decide whether work is complete enough to continue | Verifier owns gate selection, review sensors, residual risk, and completion decision | Verification summary; gate evidence; skipped-gate reasons; review findings; residual risk; follow-up needs | Hand Documentation Follow-Up mandatory update targets; hand ADR governance any durable decision candidates |
| Documentation Follow-Up | Execute mandatory project documentation updates after Verify | Documentation Update workflow owns routing and edits | Updated State, PM, migration-sql, runbook, and other routed docs; SDD artifact sync when needed | Hand Reporting synchronized operational truth and feature summary inputs |
| Reporting | Preserve session continuity and feature summary after operational truth is updated | Reporting owns communication artifacts, not authority | `session-handoff.md`, `feature-report.md`, embedded lessons learned | Hand Teacher Guide generation verified implementation knowledge when warranted |
| Teacher Guide | Transfer verified implementation knowledge for onboarding and safe evolution | Knowledge strategy and `not-a-teacher` skill own generation workflow | `teacher-guide.md` when warranted | End of feature lifecycle for Large SDD-backed work unless explicitly skipped with reason |

If a phase is skipped, the next phase must explicitly carry the missing responsibility. For example, if Design is skipped, Tasks or Execute must state that the implementation follows an existing pattern and identify that pattern. If Tasks is skipped, Execute must begin with atomic implementation steps. If SDD Pre-Execution Review is skipped for Medium work, Execute entry must document inline review checks that substitute for formal review exit criteria.

## Feature Lifecycle Ownership

| Lifecycle area | Primary owner | Decision authority | SDD responsibility |
|----------------|---------------|--------------------|--------------------|
| Specify | Active SDD | Product docs, domain docs, user-approved scope, and accepted ADRs constrain scope | Capture feature requirements, boundaries, assumptions, open questions, and acceptance criteria |
| Design | Active SDD | Accepted ADRs, Architecture docs, Technical docs, and approved domain models constrain design | Propose feature-level design, surface conflicts, identify ADR candidates, and preserve ownership boundaries |
| Tasks | Active SDD | Accepted Specify and Design constrain tasks | Convert scope and design into executable slices with dependencies, traceability, and verification expectations |
| SDD Pre-Execution Review | SDD Review session | Active SDD, `sdd-operational.md`, and accepted ADRs constrain review scope | Resolve ambiguities, validate prerequisites, confirm Execute boundaries, and gate Execute readiness |
| Execute | Implementation session | Active SDD, rules, skills, accepted ADRs, and current code patterns constrain changes | Implement within scope, add required tests, and record deviations or escalation triggers |
| Verify | Verifier workflow | Verifier owns gate selection, review sensor use, residual risk, and completion judgment | Provide acceptance criteria, expected gates, risks, and traceability evidence for verifier evaluation |
| Documentation Follow-Up | Documentation Update workflow | Owning documentation artifact determines final routing beyond mandatory checklist | Execute mandatory updates and route additional follow-up needs |
| Reporting | Reporting workflow | Reporting artifacts are communication, not authority | Provide continuity and feature summary after Documentation Follow-Up |
| Teacher Guide | Knowledge strategy and `not-a-teacher` skill | Code and ADRs win on conflict | Generate pedagogical artifact after Reporting when warranted |
| ADR decisions | ADR governance | Accepted ADRs are the durable architecture authority | Identify ADR candidates and stop for escalation when unresolved decisions block safe design or execution |
| Operational truth | `Documentation/State.md` | `Documentation/State.md` owns current branch, runtime status, blockers, and next steps | Read current truth before planning and identify State update needs after verification |

## Phase Entry And Exit Criteria

### Research

The Research phase is not a mandatory SDD stage with formal entry/exit criteria — it is an exploration phase that may produce a `research.md` artifact. When prerequisites complete or `State.md` sequencing changes after initial Research, a **Research Delta** (new `Part N` in `research.md`) reconciles stale findings against the new operational truth without repeating full discovery.

Research Delta trigger:
- A prerequisite SDD reaches Verify after initial Research completed.
- `Documentation/State.md` sequencing changes after initial Research.
- The latest `Part N` in `research.md` governs operational status and supersedes earlier parts where they conflict. Earlier parts govern domain decisions unless explicitly superseded.

Research Delta exit criteria:
- Stale operational claims identified and reconciled.
- Prerequisite status verified (consumed as **verified prerequisite**, not re-implemented).
- **READY FOR SPECIFY** declared with reconciled scope, or a second discovery pass is warranted.

Prontuario SQL Stabilization Research Part 3 is the first exemplar of a Research Delta triggered by Atendimento Minimal reaching Verify mid-discovery.

### Specify

Entry criteria:

- User request, PM item, bug report, or planning goal exists.
- `Documentation/State.md` and `AGENTS.md` have been considered for current status and authority.
- Relevant product, domain, architecture, technical, ADR, or Legacy Codebase context has been loaded only as needed.

Exit criteria:

- Scope and out-of-scope are explicit.
- Requirements or acceptance criteria are clear enough to verify.
- Assumptions, constraints, and open questions are recorded.
- Sizing has been assigned and justified.
- Required next phase is identified.

### Design

Entry criteria:

- Specify responsibility is complete.
- Sizing or risk requires design, or the team intentionally chooses design for clarity.
- Relevant architecture, technical, ADR, domain, SQL migration, frontend, or Legacy references have been identified.

Exit criteria:

- Affected components and ownership boundaries are identified.
- Existing patterns and reuse points are considered before new abstractions.
- Architecture, persistence, API, frontend, security, and migration impacts are described when relevant.
- Testing approach and testability risks are identified.
- ADR candidates are identified or explicitly ruled out.
- Design states outcomes and constraints, not implementation prescriptions. Prefer describing what must be true over naming specific classes, methods, or mapping strategies unless a durable ADR requires them.
- Design is specific enough to create implementation tasks or execute directly.
- API contract ambiguities are identified; unresolved contract decisions are flagged for SDD Pre-Execution Review.

### Tasks

Entry criteria:

- Specify responsibility is complete.
- Design responsibility is complete or intentionally skipped with rationale.
- Work is too large, dependent, risky, or verification-sensitive to execute from an inline step list.

Exit criteria:

- Tasks are independently executable once declared dependencies are satisfied.
- Dependencies are explicit and acyclic.
- Each task maps to one or more requirements when requirement IDs exist.
- Tasks constrained by ADRs or ADR candidates reference them.
- Testing expectations are embedded in implementation tasks.
- Verification gates, review prompts, smoke checks, and expected evidence are listed at the right level of detail.
- TASK items cover Execute implementation only. VP items cover Verify preparation. DF items cover Documentation Follow-Up preparation. Do not mix these categories in a single list without labels.
- Execution Boundary explicitly excludes verification artifacts, reporting, project documentation updates, and Teacher Guide generation unless explicitly approved as in-scope work.

### SDD Pre-Execution Review

Entry criteria:

- Specify, Design, and Tasks responsibilities are complete for the current sizing.
- Active SDD artifacts exist or inline equivalents are available for Medium work.
- Open questions, API contract ambiguities, and Execution Prerequisites have been identified.

Exit criteria:

- Scope boundaries and Execute Boundaries are confirmed; adjacent work is deferred, not hidden in tasks.
- Execution Prerequisites are documented and validated or explicitly accepted with residual risk.
- Baseline build and test counts are captured when required by sizing.
- API contract ambiguities are resolved or explicitly deferred with human approval and residual risk.
- Design prescriptions that belong in Execute are removed or reframed as outcomes.
- Backend Stabilization scope is confirmed when the feature is a backend aggregate stabilization slice.
- **Cross-Artifact Consistency:** specify.md and design.md do not make conflicting authority claims about the same domain decision (e.g., version generation, FK validation ownership, ID strategy). If conflicts exist, they are resolved before Execute authorization. Prontuario D-02 (version generation authority) is the first exemplar — specify.md and design.md initially diverged; reconciled in favor of design.md + ADR-006 at Pre-Execution Review.
- Execute readiness is explicitly declared or escalation is triggered.

Pre-Execution Review is a durable phase boundary. Multi-session gaps and intervening SDDs do not invalidate it — Prontuario maintained a clean Execute after a 19-day gap and the intervening AI Harness Multi-Tool SDD. On resume, re-run Credential Probe and baseline tests; no other re-validation is required.

### Execute

Entry criteria:

- Scope is known through inline Specify or `specify.md`.
- Required Design and Tasks phases are complete or intentionally skipped with rationale.
- SDD Pre-Execution Review exit criteria are satisfied for Large and Complex work, or inline review checks are documented for Medium work.
- Execution Prerequisites are satisfied or explicitly accepted with documented residual risk, including infrastructure, credentials, and baseline build/test evidence when required.
- If Tasks are skipped, atomic implementation steps have been listed before editing.
- Task-specific context and rules/skills are loaded without broad unnecessary context, following Context Acquisition Governance: acquire context only for the current task, expand incrementally, and stop when a context stop condition is satisfied.

Exit criteria:

- Implementation stays within accepted scope or records deviations.
- Required tests are added or updated as part of implementation.
- Known acceptance criteria are addressed.
- SDD artifacts are updated when implementation reveals scope, design, task, or verification changes.
- Evidence and skipped checks are ready for verifier review.
- **`TASK-*` checkboxes:** Execute may mark implementation tasks complete when code and tests for that task are done. Execute records runtime validation **intent** for TASK-008; Verify owns durable HTTP evidence.

### Verify

Entry criteria:

- Implementation diff or documentation change is available.
- Active SDD artifacts or inline plan are available when they exist.
- Known acceptance criteria, tests, and expected gates are available.

Exit criteria:

- Verifier has classified the change by behavior and risk.
- Applicable gates were selected and evaluated.
- Review prompts were applied or skipped with reasons.
- Testing evidence was evaluated against the feature requirements.
- Missing gates, failed checks, and deferred tests are recorded.
- Residual risk is classified and justified.
- Follow-up needs for State, ADRs, Architecture/Technical docs, SDD, rules, skills, or review prompts are identified.
- Work is declared complete, not complete, or complete only with accepted residual risk.
- **Runtime validation:** When TASK-008 or equivalent applies, Verify populates durable evidence in `verification.md` even if Execute omitted session notes.
- **`TASK-*` acceptance:** Verify confirms requirement-level acceptance; Documentation Follow-Up does not substitute for Verify acceptance.

### Documentation Follow-Up

Entry criteria:

- Verify exit criteria are satisfied or Verify declared completion with accepted residual risk.
- Verifier identified documentation follow-up targets.

Exit criteria:

- Mandatory checklist executed: `Documentation/State.md`, `Documentation/Product/PM_DocOrgano.md`, `Documentation/Technical/migration-sql.md`, and `Documentation/Technical/runbook.md` when the feature touched migration or runtime prerequisites.
- Additional follow-up candidates routed through Documentation Update when warranted.
- Active SDD artifacts synchronized when implementation or verification changed scope, design, tasks, or expected evidence.
- Documentation Follow-Up is execution, not suggestion. Routing evaluation alone is insufficient.

## Backend Stabilization Rule

When a feature is classified as backend aggregate stabilization or SQL migration vertical work:

- Exclude frontend validation, Blazor smoke checks, and UI contract alignment from default Execute scope unless explicitly required by acceptance criteria.
- Document frontend/API contract drift as follow-up or out-of-scope, not as hidden Execute tasks.
- Prefer API and repository verification over UI smoke for the stabilization slice.

This rule prevents scope drift during SDD Pre-Execution Review and Execute for SQL migration verticals.

## Execution Prerequisites

Large and Complex SDD-backed work must document Execution Prerequisites before Execute. Prerequisites include:

- Infrastructure availability, such as Docker SQL Server running locally.
- Credential availability, such as `SA_PASSWORD` and `DOCORGANO_TEST_CONNECTION` when SQL integration tests apply.
- Baseline build and automated test counts captured before implementation begins.
- Runtime validation environment expectations when manual smoke checks are required.

Missing prerequisites must block Execute or be explicitly accepted with documented residual risk during SDD Pre-Execution Review. Do not discover false test failures during Execute when prerequisites were knowable earlier.

## Definition Of Done

A feature is not done solely because implementation finished. The governance-level Definition of Done applies to SDD-backed work and to simplified SDD flows where the same responsibilities are carried inline.

For Large and Complex SDD-backed work, completion requires the full post-Verify chain unless a phase is explicitly skipped with documented rationale and residual risk.

A feature may be considered complete only when:

- Implementation addresses the accepted scope or explicitly records approved deviations.
- Required automated tests were added, updated, or intentionally deferred with reason and residual risk.
- **Verify** is complete: applicable verification gates were selected and evaluated by verifier responsibility; failed, skipped, blocked, or deferred gates have concrete explanations; residual risk is classified and accepted at the appropriate level.
- **Documentation Follow-Up** is executed for SDD-backed work: mandatory checklist in Documentation Integration Model is applied — not merely evaluated. `Documentation/State.md` is the highest-priority mandatory item when operational truth changed.
- **Reporting** is complete when reporting policy applies: `feature-report.md` and session handoff when needed, produced after Documentation Follow-Up so reports reflect synchronized operational truth.
- **Teacher Guide** is complete when knowledge strategy criteria apply: `teacher-guide.md` generated after Reporting, or skip recorded with reason in Documentation Follow-Up or feature report.
- ADR evaluation was performed when architecture, persistence, schema, security/auth, API contract, ownership, runtime, or irreversible decisions were involved.
- Additional documentation follow-up beyond the mandatory checklist was routed when warranted.
- Requirement traceability is complete enough for the feature size and risk.
- Legacy Codebase behavior decisions are explicit when behavior was preserved, adapted, or abandoned.

Verify completion alone does not satisfy Definition of Done for SDD-backed work. Documentation Follow-Up, Reporting, and Teacher Guide (when applicable) remain required completion responsibilities.

If any required evidence is missing, the feature is not complete. It may be reported as blocked, partially complete, or complete only with accepted residual risk.

## Safety Valve For Skipped Tasks

When `tasks.md` is skipped, Execute must begin by listing atomic implementation steps inline.

Escalate to formal `tasks.md` before continuing if the inline step list reveals:

- More than 3 meaningful implementation steps.
- Hidden dependencies or uncertain sequencing.
- Work that would benefit from independent task execution.
- Cross-layer changes that need coordinated verification.
- Architectural uncertainty or ADR candidates.
- Legacy Codebase behavior comparison or characterization testing.
- Scope expansion beyond the original request.
- Significant uncertainty about required tests or verification gates.

This safety valve prevents under-scoped work. Escalation is not process failure; it is the expected correction when actual complexity is higher than initial sizing.

## Context Acquisition Governance

### Purpose

The existing governance specifies WHAT artifacts are authoritative but must also govern HOW context is acquired during execution. During the first real Multi-Tool Execute session, an agent exhausted its context budget before completing a single implementation task by aggressively loading large portions of the repository — approximately 180 files including architectural documents, templates, ADRs, business documentation, legacy repositories, the entire frontend, and the entire backend. No implementation task was completed before the context limit was reached.

Context Acquisition Governance is a transversal operational concept that applies to every SDD phase. It defines expected context depth per phase, governs incremental expansion, and prevents context exhaustion during execution. This governance is tool-independent and improves execution quality for Cursor, Cline, future agents, and future AI tools.

### Context Budget

Each phase has an expected context depth. The budget is guidance, not a hard limit, but agents should stay within the expected range unless implementation genuinely requires expansion.

| Phase | Expected Context | Rationale |
|-------|-----------------|-----------|
| Research | High | Research intentionally explores architecture, domain, Legacy behavior, and technical constraints to inform planning. Broad loading is expected here. |
| Specify | Medium | Specify needs product intent, domain rules, and scope boundaries, but not implementation details. |
| Design | Medium | Design needs architecture, ADRs, and integration points, but not full codebase or unrelated features. |
| Tasks | Low | Tasks work from accepted Specify and Design. Implementation code is not needed at this phase. |
| Execute | Very Low (incremental) | Execute acquires context only for the current task. This is the most constrained phase. |
| Verify | Medium | Verify needs implementation diff, acceptance criteria, test evidence, and review prompts. |
| Documentation Follow-Up | Low | Documentation Follow-Up needs verification results and the mandatory checklist targets. |
| Reporting | Low | Reporting needs synchronized operational truth and feature summary inputs. |
| Knowledge | Medium | Teacher Guide generation needs verified implementation knowledge, code evidence, and domain context. |

### Context Loading Strategy

Every phase must define what should be loaded, what may be loaded, and what should not be loaded.

#### Primary Context — Always Load

- `Documentation/State.md` for current operational truth.
- `AGENTS.md` for stable bootstrap, authority hierarchy, and routing.

#### Secondary Context — Load Only If Required

- Active SDD artifacts for the current feature.
- Accepted ADRs when architecture, persistence, security, API, ownership, runtime, or irreversible decisions are relevant.
- Architecture docs for domain boundaries, business rules, and system structure.
- Technical docs for SQL migration, frontend architecture, runbooks, API guidance, or runtime behavior.
- Product docs for product intent, backlog sequencing, and roadmap boundaries.
- Review prompts when planning verification or reviewing changes.
- Task-specific workflows, such as Documentation Update, Verifier, SQL migration, codebase decomposition, or Legacy Codebase behavior workflows.
- Targeted Legacy methods only when behavior migration or characterization is in scope.

#### Forbidden Context — Do Not Load Unless Explicitly Justified

- Entire Legacy folders (`DocAPI/Legacy/_LegacySheetsDb/`). Load targeted methods only.
- Multiple unrelated feature SDDs in one implementation session.
- The entire frontend (`DocFront.Web/`) when the current task touches backend only.
- The entire backend (`DocAPI/`) when the current task touches frontend only.
- All ADRs when only one or two are relevant to the current decision.
- All architecture and technical documents for a Small change.
- Full chat history as a durable source of truth. Summarize findings into SDD, ADRs, State, or owning docs instead.

#### Context Priorities

1. Current truth and authority: `Documentation/State.md`, `AGENTS.md`, accepted ADRs.
2. Feature truth: active SDD artifacts.
3. Domain and architecture truth: Domain Overview, Architecture Overview, relevant Technical docs.
4. Implementation evidence: targeted code, tests, migrations, API/UI files.
5. Review and verification sensors: review prompts and verifier workflow.
6. Research or methodology notes only when creating or revising governance.

#### Context Boundaries

- Load the minimum set needed to decide or execute the current phase.
- Prefer targeted files and methods over entire folders.
- Do not load multiple unrelated feature SDDs in one implementation session.
- Do not load full Legacy folders; Legacy is behavioral reference, not implementation to revive.
- Do not use long chat history as the durable source of truth; summarize important findings into SDD, ADRs, State, or owning docs.

#### Context Anti-Patterns

- Treating SDD as a place to paste all research.
- Loading every architecture and technical document for a Small change.
- Carrying stale roadmap assumptions instead of checking `Documentation/State.md`.
- Using review prompts as policy sources instead of sensors.
- Reading broad Legacy files when a targeted method is enough.
- Aggressively loading large portions of the repository before beginning execution.
- Preloading implementation context for future tasks during the current task.
- Repository-wide exploration when a targeted search would identify the needed artifact.
- Continuing to search after the required information has been found.

### Progressive Context Expansion

Context acquisition must be incremental, not preemptive.

- Never acquire additional context until the current information becomes insufficient.
- Expand incrementally — one artifact, one method, one file at a time.
- Stop expansion immediately after the missing information is found.
- Avoid repository-wide exploration. Use targeted search before broad listing.
- Each expansion must have a specific question it is trying to answer.

### Task-Oriented Context

During Execute, context acquisition is task-oriented, not feature-oriented.

- The agent shall acquire context only for the current task.
- Future tasks must not preload their implementation context.
- Each task starts with a fresh context acquisition process.
- When a task is complete, commit progress before starting the next task's context acquisition.
- Do not carry context from a completed task into the next task unless it is explicitly shared infrastructure.

### Context Stop Conditions

Context acquisition ends when a specific condition is satisfied. Once satisfied, stop searching and begin implementation.

Stop conditions:

- Required implementation artifact identified — the file, class, or method to modify is known.
- Required dependency identified — the upstream or downstream component that the current task interacts with is known.
- Required interface identified — the API contract, DTO, repository interface, or service interface the current task implements or consumes is known.
- Required behavioral reference identified — the Legacy method, business rule, or domain behavior the current task preserves, adapts, or replaces is known.

Once any stop condition is satisfied for the current task:

- Stop searching.
- Begin implementation.
- Expand further only if implementation reveals a new unknown.

### Legacy Loading

The existing governance states that Legacy implementations are reference sources, not primary context. This is strengthened operationally:

- Legacy shall only be loaded when the current task requires behavioral comparison.
- Never preload legacy repositories.
- Load targeted methods only — the specific method that contains the behavior being preserved, adapted, or replaced.
- Do not load entire Legacy folders or multiple Legacy repositories for a single task.
- Legacy loading must be justified by a specific behavioral question, not general familiarity.

### Implementation Batching Guidance

The Execute phase explicitly recommends batching for large features.

- Large features should be executed in small implementation batches.
- Each batch should:
  - implement a coherent subset of tasks;
  - validate (build, test, or smoke check as appropriate);
  - commit progress;
  - start a new execution session if necessary.
- The workflow discourages attempting to execute an entire feature within one long-running AI session.
- Batching protects against context exhaustion, lost progress, and incomplete implementation.
- Each batch boundary is a natural checkpoint for State.md updates and session handoff.

### Flexibility Preservation

This governance does not forbid reading additional documentation. It governs how context is acquired, not whether it can be acquired.

The agent may expand context whenever implementation genuinely requires additional information. The expansion must be:

- **Purpose-driven:** There is a specific question or unknown that requires the additional context.
- **Minimal:** The smallest set of artifacts that answers the question.
- **Temporary:** The context serves the current task, not future tasks.
- **Proportional:** The context depth matches the phase budget and the task complexity.

If expansion is needed, state what is unknown, load the minimum artifact that resolves it, and continue.

## Ownership Model

| Artifact or workflow | Owner responsibility | SDD relationship |
|----------------------|----------------------|------------------|
| `Documentation/State.md` | Current branch, runtime status, blockers, next steps, verification-relevant operational truth | SDD reads it at start; verifier or documentation follow-up may recommend updates |
| ADRs | Accepted durable architecture decisions | SDD identifies candidates and references accepted ADRs; ADR governance decides and owns records |
| Architecture docs | Durable architecture and domain explanation | SDD consumes them and may identify update needs after verification |
| Technical docs | Runtime, SQL migration, frontend, API, and operational technical guidance | SDD consumes them and may identify follow-up needs |
| SDD artifacts | Feature-specific scope, design, tasks, and expected verification | SDD owns these directly |
| Verification outputs | Gate evidence, skipped checks, review sensor findings, residual risk, completion decision | Verifier owns outputs; SDD provides acceptance criteria and expected gates |
| Documentation updates | Routing, authority validation, path drift checks, and updates to owning docs | Documentation Update owns routing; SDD identifies possible needs |
| Rules | Concise persistent guardrails | SDD may reveal recurring guardrail needs but does not own rules |
| Skills | Repeatable workflows | SDD may become future skill input, but this document does not create a skill |
| Review prompts | Review sensors for domain, security/PHI, documentation, testing, and future categories | SDD references applicable sensors; Verifier selects and applies them |
| Product docs | Product intent, backlog, roadmap horizons | SDD consumes product context and must not redefine product strategy |

## SDD Artifact Definitions

### `specify.md`

Purpose: capture what is needed.

Typical contents:

- Requirements.
- Scope and out-of-scope.
- Assumptions and constraints.
- Open questions.
- Acceptance criteria.
- Relevant domain language.
- Sizing decision and rationale.

Required when:

- Work is Medium, Large, or Complex and scope is not already unambiguous.
- User-visible behavior changes.
- Clinical behavior, persistence, security/PHI, Legacy Codebase behavior, API contracts, or cross-layer behavior is involved.
- Requirement traceability matters for verification.

May be simplified when:

- Work is Small and low-risk.
- The user request already contains clear scope and acceptance criteria.
- The change is documentation-only and does not alter governance, architecture, or operational truth.

### `design.md`

Purpose: capture how the solution should work.

Typical contents:

- Architecture impact.
- Affected components and ownership boundaries.
- Existing patterns to reuse.
- Technical decisions.
- Integration points.
- Data, API, frontend, security, and migration impacts when relevant.
- Risks and mitigations.
- Testing approach and testability concerns.
- ADR candidates.

Required when:

- Work is Large or Complex.
- The change touches architecture boundaries, persistence, schema, API contracts, security/PHI, cross-context ownership, or Legacy Codebase behavior migration.
- The work may require an ADR.
- The implementation path is not obvious from existing local patterns.

May be skipped when:

- The change follows an established pattern.
- No architecture decision or ownership boundary changes.
- No new persistence, API, frontend, security, or cross-layer integration concerns are introduced.
- The design can be safely captured as an inline note before Execute.

### `tasks.md`

Purpose: capture implementation planning.

Typical contents:

- Implementation slices.
- Dependencies and sequencing.
- Requirement mapping.
- ADR mapping when relevant.
- Testing expectations embedded in implementation work.
- Verification gates and review prompts.
- Known skipped-gate risks or expected manual checks.

Required when:

- Work is Large or Complex.
- Work has more than 3 meaningful steps.
- Work spans multiple components or sessions.
- Dependencies or sequencing matter.
- Parallel work is possible.
- Verification expectations are non-trivial.

May be skipped when:

- Work can be executed from 3 or fewer atomic steps.
- Dependencies are obvious and local.
- Verification is simple and can be stated inline.

## Requirement Traceability Model

Traceability should be lightweight, useful, and maintained only at the depth justified by feature size.

Recommended chain:

```text
Requirement -> Design -> Task -> Implementation -> Test -> Verification
  -> Documentation Follow-Up -> Reporting -> Teacher Guide
```

Requirement IDs:

- Mandatory for Large and Complex work.
- Recommended for Medium work when multiple acceptance criteria, tests, or tasks exist.
- Optional for Small work.
- Use simple stable IDs such as `REQ-001`, `REQ-002`, not complex schemas.

Traceability expectations:

- `specify.md` defines requirement IDs when used.
- `design.md` references requirement IDs for design decisions that satisfy or constrain requirements.
- `tasks.md` maps each task to the requirements it implements.
- Implementation evidence maps changed code, docs, migrations, or configuration back to the task or requirement when the connection is not obvious.
- Tests should clearly map to requirements through test names, task notes, or verification summary.
- Verification validates that accepted requirements have implementation and test evidence or documented residual risk.
- Documentation Follow-Up executes mandatory project documentation updates and routes additional follow-up when warranted.
- Reporting summarizes the feature after operational truth is synchronized; it does not replace Verify or Documentation Follow-Up.
- Teacher Guide transfers verified implementation knowledge when warranted; it does not replace Reporting or SDD artifacts.

Traceability review should look for missing links:

- Requirements with no design response.
- Design choices with no task or execution path.
- Tasks that implement no accepted requirement.
- Implementation changes that are not mapped to a task, requirement, bug, or approved deviation.
- Tests that do not cover critical requirements.
- Verification evidence that does not address acceptance criteria or residual risk.
- Documentation follow-up needs that are identified but not routed to an owner.
- Legacy Codebase behavior decisions that do not state whether behavior was preserved, adapted, or abandoned.

Traceability should not become bureaucracy. It exists to prevent lost requirements, untested behavior, unclear verification, and unowned documentation follow-up, especially during SQL migration and Legacy Codebase behavior porting.

## Task Governance Model

Tasks are implementation slices, not independent sources of scope. They must implement the accepted Specify and Design artifacts.

Task ID conventions:

- `TASK-*` — Execute implementation work only.
- `VP-*` — Verify preparation work, such as verification artifact structure or gate planning. Not Execute work.
- `DF-*` — Documentation Follow-Up preparation work, such as identifying update candidates. Not Execute work.

Keep TASK, VP, and DF items in separate lists or clearly labeled sections in `tasks.md`. Do not mix workflow preparation into TASK lists without labels.

Task quality principles:

- Tasks should be independently executable once declared dependencies are satisfied.
- Tasks should not rely on undeclared work.
- Dependencies must be explicit and acyclic.
- Circular dependencies are decomposition failures and require task redesign.
- Tasks should be small enough to verify meaningfully.
- Tasks should identify the requirements they implement when requirement IDs exist.
- Tasks constrained by accepted ADRs or ADR candidates should reference them.
- Tasks should include expected tests or checks as part of implementation, not as detached cleanup.
- Tasks should identify manual smoke checks only where automated coverage is not appropriate or not yet available.
- Tasks should call out known risks, skipped checks, or residual uncertainty early enough for verifier review.

Task anti-patterns:

- Vague tasks such as "update backend" without files, behavior, or done condition.
- Tasks that mix unrelated bounded contexts.
- Tasks that create new requirements not present in Specify.
- Tasks that hide architecture decisions inside implementation.
- Tasks that defer all tests to a final generic verification step.

## Verification Integration Model

Implementation is not completion. Completion requires verification.

SDD participation:

- Specify provides acceptance criteria and user-visible behavior.
- Design identifies risk, testability concerns, integration points, and ADR candidates.
- Tasks provide expected gates, review prompts, tests, smoke checks, and known skipped-gate risks.
- Execute provides implementation evidence and records deviations.

Verifier authority:

- The Verifier selects applicable gates based on the diff, active SDD, rules, ADRs, review prompts, and risk classification.
- The Verifier applies or skips review prompts with concrete reasons.
- The Verifier evaluates testing evidence.
- The Verifier records failed, skipped, blocked, or deferred gates.
- The Verifier classifies residual risk.
- The Verifier decides whether work is complete, not complete, or complete only with accepted residual risk.

Typical gate categories:

- Build.
- Automated tests.
- SQL, EF, migration, or repository checks.
- API smoke checks.
- UI smoke checks.
- Security/PHI review.
- Domain review.
- Documentation review.
- Test strategy review.
- ADR evaluation.
- Legacy Codebase characterization or behavior comparison.

SDD must not self-certify completion. It prepares the verifier handoff.

## SDD Verification Expectations

Every SDD-backed feature must provide enough information for the Verifier to select gates, evaluate evidence, and classify residual risk. The depth should match the feature size and risk, but the responsibility always exists.

An SDD should identify:

- Expected behavior: user-visible behavior, domain behavior, API behavior, persistence behavior, or documentation/governance behavior that must be true after implementation.
- Critical requirements: requirements that block completion if missing, especially clinical, security/PHI, persistence, migration, or cross-layer requirements.
- Verification evidence: expected automated tests, build commands, SQL checks, API or UI smoke checks, review prompts, screenshots, logs, or manual observations when relevant.
- Coverage expectations: where automated coverage is required, where existing coverage is sufficient, and where missing coverage would be residual risk.
- Known risks: testability limits, migration risk, PHI/security risk, Legacy Codebase behavior uncertainty, external dependency risk, or architecture uncertainty.
- Affected integrations: API/front contracts, EF repositories, SQL migrations, Blazor state/services, files, auth assumptions, logging, or other runtime boundaries.
- Traceability expectations: requirement IDs, task mappings, test mappings, and documentation follow-up mappings when required by feature size.

For Small work, this can be an inline verifier handoff. For Medium, Large, and Complex work, these expectations should be captured in `specify.md`, `design.md`, and `tasks.md` according to phase ownership.

## Testing Governance Model

Testing is part of implementation, not a separate phase after implementation.

Governance principles:

- A feature without required tests is not complete.
- Tests should be planned before implementation.
- Executable tasks should include testing expectations.
- Verification validates test evidence against requirements and risk.
- Missing tests must be recorded as technical debt with reason, owner, and residual risk.
- Critical business rules should not rely only on manual testing.
- Tests must use synthetic data only.
- SQL migration work should add repository, integration, or migration checks proportional to persistence risk.
- Legacy Codebase behavior migration should use characterization tests or explicit behavior comparison before replacing behavior.

Testing depth should scale with risk:

- Small low-risk changes may rely on existing tests plus a focused check.
- Medium behavior changes should add or update automated tests for the changed behavior.
- Large cross-layer work should include automated tests for core behavior and smoke checks for integration boundaries.
- Complex clinical, persistence, security, or Legacy Codebase migration work should include characterization, integration, or targeted review evidence as appropriate.

## ADR Integration Model

SDD identifies ADR candidates; ADR governance owns ADR creation, acceptance, supersession, and ownership.

ADR trigger criteria:

- Architecture boundary changes.
- Bounded-context ownership changes.
- Aggregate boundary or business-rule location changes.
- Persistence strategy, schema lifecycle, migration approach, soft delete, or data history changes.
- Security model, PHI handling, auth, authorization, logging, or secret-handling architecture changes.
- API contract ownership, DTO policy, ID strategy, or compatibility decisions.
- Cross-context ownership decisions.
- Major runtime, framework, integration, infrastructure, or deployment decisions.
- Durable exceptions to existing architecture rules.
- Irreversible or hard-to-reverse technical decisions.
- Recurring architectural disagreements that need a stable answer.

Do not create ADRs for:

- Local implementation choices inside an accepted design.
- Task sequencing.
- Temporary branch status or blockers.
- Routine bug fixes.
- Mechanical refactors without ownership or behavior change.
- Decisions already covered by an accepted ADR.

SDD should distinguish:

- Architecture decisions: durable, cross-feature, or future-guiding decisions that may need ADRs.
- Implementation decisions: local choices that satisfy the current design and can remain in SDD or code review notes.

If an ADR candidate is identified during Design or Execute, continue only when the decision is understood well enough not to violate current ADRs. For major uncertainty, pause implementation and route the ADR decision before Tasks or further Execute work.

## Documentation Integration Model

SDD identifies documentation follow-up needs, but Documentation Update owns routing. Documentation Follow-Up is a mandatory post-Verify phase for SDD-backed work, not an optional evaluation.

After Verify, execute the mandatory Documentation Follow-Up checklist before Reporting:

- `Documentation/State.md`
- `Documentation/Product/PM_DocOrgano.md`
- `Documentation/Technical/migration-sql.md` when persistence or migration behavior changed
- `Documentation/Technical/runbook.md` when runtime prerequisites or local setup changed

The Verifier identifies follow-up needs after gates and review sensors. Documentation Follow-Up executes mandatory updates and routes additional candidates through Documentation Update.

Documentation follow-up may also be needed when:

- Operational truth changes, such as current branch status, runtime status, blockers, verification status, or next steps.
- An accepted ADR changes architecture direction.
- Architecture docs need to explain a durable model.
- Technical docs need to explain SQL migration, frontend, API, runbook, or runtime behavior.
- SDD artifacts no longer match implementation.
- Rules, skills, review prompts, or indexes reveal path drift or authority drift.

Before routing, evaluate:

| Candidate follow-up | Evaluation criteria before routing |
|---------------------|------------------------------------|
| `Documentation/State.md` | Did current branch, runtime status, blockers, verification status, active epic, or next steps change? |
| ADR evaluation | Did the work introduce or revise a durable decision affecting architecture boundaries, persistence, schema lifecycle, security/auth, API contracts, cross-context ownership, runtime choices, or irreversible migration decisions? |
| Architecture docs | Did accepted design or ADR outcome change the durable domain model, bounded-context ownership, business-rule placement, or architecture explanation? |
| Technical docs | Did runtime guidance, SQL migration practice, API behavior, frontend architecture, runbook steps, or operational technical guidance change? |
| Rules | Did a recurring concise guardrail emerge that should be always-on or glob-scoped rather than repeated in SDDs? |
| Skills | Did a repeatable multi-step workflow emerge that is too procedural for a rule and likely to recur? |
| Review prompts | Did verification reveal a recurring review sensor gap or obsolete prompt guidance? |
| Active SDD artifacts | Did implementation, verification, or escalation change feature scope, design, tasks, traceability, or expected evidence? |

Documentation Update owns:

- Routing follow-up to the correct artifact.
- Path drift checks.
- Authority validation.
- Ownership validation.

Reporting follows Documentation Follow-Up, not Execute or Verify. Teacher Guide generation follows Reporting when knowledge strategy criteria apply.

## Brownfield And Legacy Considerations

Doc Organo is a brownfield DDD project migrating validated Legacy Codebase behavior into SQL-backed bounded contexts. The current Legacy reference source is the Google Sheets implementation under `DocAPI/Legacy/_LegacySheetsDb/`, but this governance applies to any future legacy reference source. SDD must avoid greenfield assumptions.

Brownfield expectations:

- Preserve existing behavior unless the accepted scope explicitly changes it.
- Prefer existing architecture, domain language, and local patterns.
- Do not introduce abstractions unless they reduce real complexity or match established patterns.
- Keep changes incremental and bounded by aggregate or feature ownership.
- Treat Financial domain expansion as out of scope unless explicitly planned.
- Respect accepted ADRs for SQL migration and Legacy Codebase behavior reference.

Legacy Codebase behavior migration expectations:

- Treat Legacy Codebase code as behavioral reference, not runtime code to revive.
- Treat Legacy code as a source of observed behavior, not as an authority source.
- Accepted ADRs, documented business rules, security requirements, and approved domain models override Legacy Codebase behavior when they conflict.
- Legacy Codebase behavior may be preserved, adapted, or intentionally abandoned when the decision is explicit and traceable.
- For Legacy-behavior migration SDDs, include a Legacy characterization step before implementation — preserve/adapt/abandon table, targeted method reads, and characterization tests when clinical rules are material.
- Record whether each material behavior is preserved, adapted, or abandoned in Specify, Design, Tasks, verification evidence, or documentation follow-up as appropriate.
- Read targeted Legacy methods, not broad folders. See Context Acquisition Governance for Legacy loading rules: Legacy shall only be loaded when the current task requires behavioral comparison, never preloaded.
- Capture the behavior being preserved or intentionally changed.
- Add characterization tests or explicit behavior comparison for critical clinical rules.
- Port business rules into domain methods or application use cases, not controllers or repositories.
- Verify SQL-backed replacements before treating Legacy Codebase behavior as replaced.

SQL-backed bounded-context expectations:

- Migrate by vertical clinical slices.
- Keep bounded-context ownership explicit.
- Include persistence and migration verification when schema or repository behavior changes.
- Do not mark a migrated slice complete without tests or justified residual risk.

## Updated Workflow Proposal

Use this decision flow:

1. Load `Documentation/State.md` and `AGENTS.md`.
2. Classify the change by size, risk, uncertainty, and touched ownership boundaries.
3. Decide SDD depth:
   - Small: inline Specify, inline steps, Execute, Verify.
   - Medium: brief Specify; Design and Tasks only when risk or dependency requires them; inline review before Execute when Large phases are skipped.
   - Large: full Specify, Design, Tasks, SDD Pre-Execution Review, Execute, Verify, Documentation Follow-Up, Reporting, Teacher Guide when warranted.
   - Complex: full lifecycle with scope monitoring during Execute and explicit residual risk during Verify.
4. Load only task-specific context needed for the current phase, following Context Acquisition Governance: start with primary context, expand progressively only when current information is insufficient, stop immediately when a context stop condition is satisfied, and never preload context for future tasks.
5. Complete phase entry and exit criteria before moving forward.
6. Pause for escalation when authority, architecture, requirement, technical constraint, Legacy Codebase, or verification conflicts cannot be resolved safely.
7. If Tasks are skipped, list atomic implementation steps before editing.
8. Escalate to formal Tasks when hidden complexity appears.
9. Complete SDD Pre-Execution Review before Execute for Large and Complex work.
10. Implement tests as part of executable work.
11. Hand off to Verifier for gate selection, review prompts, evidence, skipped checks, residual risk, and Definition of Done evaluation.
12. Execute Documentation Follow-Up before Reporting.
13. Generate Teacher Guide after Reporting when knowledge strategy criteria apply.
14. Evaluate additional documentation follow-up candidates beyond the mandatory checklist before routing them through Documentation Update.

## Governance Consistency Review

Current improvements reduce ownership overlap by assigning:

- Feature scope, design, task planning, and expected verification to SDD.
- Gate selection, review prompt application, residual risk, and completion judgment to Verifier.
- Documentation routing and authority validation to Documentation Update.
- Durable architecture decisions to ADR governance.
- Current branch and runtime truth to `Documentation/State.md`.

Resolved gaps:

- Context Acquisition Governance now defines expected context depth per phase, progressive expansion, task-oriented context, stop conditions, Legacy loading rules, and implementation batching — addressing the context exhaustion observed during the first real Multi-Tool Execute session.

Remaining gaps to monitor:

- Review prompt coverage for EF migrations and API contracts remains a future candidate, not a current SDD responsibility.
- Paciente SQL Stabilization SDD is a pre-calibration reference; Prontuario forward SDD is the first post-calibration consumer of updated templates and governance.
- Harness Calibration Workflow should be documented in `CONTRIBUTING-AI.md` after Wave 1 governance updates complete.

Duplicated responsibilities to avoid:

- Do not let SDD choose final verification gates after implementation; it defines expectations, while Verifier selects and evaluates gates.
- Do not let SDD route documentation updates; it identifies candidates, while Documentation Update owns routing.
- Do not let SDD override ADRs; it proposes or escalates durable decisions.
- Do not let Legacy Codebase behavior override accepted architecture, documented business rules, security requirements, or approved domain models.

## Future Capability Recommendations

Future work may build on this governance, but should not be implemented by this document:

- Create a future `spec-workflow` skill only after this governance has been used on at least one real feature SDD.
- Improve SDD templates after observing real usage against Prontuario, Agendamento, Atendimento, or another SQL migration slice.
- Consider template support for escalation outcomes, Definition of Done checks, verifier handoff, documentation follow-up evaluation, and Legacy Codebase behavior decisions.
- Add or refine review prompts for EF migrations and API contracts if recurring verification gaps appear.
- Use one completed SQL migration or Legacy Codebase behavior port as the reference SDD example.
- Consider lightweight traceability conventions in templates only after requirement IDs prove useful in real work.
- Integrate with `reporting-strategy.md` for lightweight session handoff, feature report, lessons learned, and future workflow observability boundaries.
- Integrate with `sdd-pilot-report-governance.md` for phased SDD pilot reports, workflow calibration findings, and governance improvement plan inputs.
