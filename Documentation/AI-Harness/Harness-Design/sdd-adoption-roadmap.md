# SDD Operational Governance

## Purpose

This document defines how Spec-Driven Development operates inside the Doc Organo AI Harness. It is the authoritative governance source for when SDD is required, how deeply it should be applied, what each SDD phase owns, and how SDD integrates with verification, documentation, ADR, architecture, and testing workflows.

SDD is the feature-level planning contract between product or architecture intent and implementation. It does not replace `AGENTS.md`, `Documentation/State.md`, ADRs, Documentation Update, Verifier, rules, skills, review prompts, product docs, architecture docs, or technical docs. It coordinates with them while preserving their ownership boundaries.

This file keeps the existing `sdd-adoption-roadmap.md` path for compatibility, but the document identity is now SDD Operational Governance.

This document focuses on governance expectations, not templates, schemas, tooling, automation, MCP strategy, sub-agent orchestration, CI/CD architecture, or future skill implementation.

## Governance Review

The previous roadmap established useful foundations:

- SDD owns feature-specific scope, design, task planning, and verification expectations.
- SDD should be required by risk and size, not for every change.
- `specify.md`, `design.md`, and `tasks.md` already have clear high-level responsibilities.
- Verification, testing, ADR evaluation, and documentation follow-up are recognized as part of completion.
- Legacy Codebase behavior, SQL migration, clinical/security risk, and cross-layer changes are treated as stronger SDD triggers.

The roadmap needed to evolve from adoption guidance into operational governance because it mixed several concerns:

- Adoption phases and future maturity work.
- SDD artifact responsibilities.
- Verification policy.
- Security review expectations.
- MCP recommendations.
- Future verifier-skill recommendations.

The improved model keeps the useful thresholds and artifact ownership, adds adaptive workflow depth, makes phase entry and exit criteria explicit, and clarifies handoffs to existing governance workflows.

## Gap Analysis

| Gap | Impact | Governance response |
|-----|--------|---------------------|
| Fixed pipeline implied by artifact list | Small work could inherit unnecessary ceremony; risky small work could be under-planned | Adopt adaptive sizing with explicit escalation rules |
| Phase completion conditions were implicit | Agents could claim a phase is complete without evidence | Define entry and exit criteria for every phase |
| Skipped phases lacked handoff rules | Work could skip `tasks.md` without a concrete execution plan | Require inline atomic steps when Tasks is skipped |
| Ownership overlap with Verifier and Documentation Update | SDD could accidentally duplicate gate selection or documentation routing | Define SDD as planning/handoff owner, not verifier or router |
| Traceability was implied but not governed | Requirements, tests, and verification evidence could drift apart | Add lightweight requirement-to-verification traceability |
| Task quality was not fully specified | Task lists could become vague checklists instead of executable slices | Add task independence, dependency, requirement, ADR, and testing rules |
| Brownfield migration needs were scattered | SDD could default to greenfield thinking | Add explicit Legacy, characterization, incremental migration, and bounded-context expectations |

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
5. Execute implements the accepted scope and records deviations.
6. Verify determines whether completion evidence is sufficient.
7. Documentation Update and ADR governance handle durable follow-up after verification identifies the need.

The canonical lifecycle is:

```text
Specify -> Design -> Tasks -> Execute -> Verify
```

Phases may be intentionally simplified or skipped based on sizing, but the lifecycle responsibility does not disappear. The missing responsibility must be carried by a lighter artifact, an inline plan, or verifier evidence.

## New Sections To Add

This governance model adds the following concepts beyond the old roadmap:

- Adaptive sizing by Small, Medium, Large, and Complex work.
- Explicit rules for when Specify, Design, and Tasks are required, simplified, or skipped.
- Entry and exit criteria for every phase.
- Safety valve for skipped Tasks.
- Context loading strategy for SDD sessions.
- Escalation policy for authority, architecture, requirement, technical constraint, Legacy Codebase, and verification conflicts.
- Feature lifecycle ownership and governance-level Definition of Done.
- Ownership model across SDD, Verifier, Documentation Update, ADR governance, architecture, rules, skills, review prompts, and State.
- Lightweight requirement-to-documentation-follow-up traceability.
- Task quality principles.
- Brownfield and Legacy Codebase migration expectations.
- Verification, testing, ADR, and documentation integration models.

## Sections To Remove

The following roadmap-style content should not remain in this document as operational SDD governance:

- Phased adoption roadmap language that describes future maturity milestones as the main structure.
- MCP usage strategy, except where runtime evidence is generally mentioned as verifier input.
- Future verifier-skill design details.
- CI/CD architecture recommendations.
- Template-format instructions beyond artifact purpose and governance expectations.
- Security review adoption strategy as a standalone section; security-sensitive work is handled through sizing, verifier gate selection, review prompts, and existing security/PHI governance.

## Adaptive Sizing Model

Process depth is determined by complexity, risk, uncertainty, and blast radius, not by a fixed pipeline.

| Size | Typical scope | Specify | Design | Tasks | Execute | Verify |
|------|---------------|---------|--------|-------|---------|--------|
| Small | Up to about 3 files; simple bug fix; configuration update; typo; low-risk adjustment | Inline statement of intent is enough | Skip unless risk appears | Skip; list atomic steps inline before editing | Required | Required, often lightweight |
| Medium | Clear feature or behavior change with limited scope and known implementation path | Required, may be brief | May be skipped if architecture is straightforward and existing patterns apply | May be skipped if there are 3 or fewer obvious steps | Required | Required |
| Large | Multi-component feature; cross-layer work; persistence/API/UI/domain interaction; significant implementation effort | Full `specify.md` required | Full `design.md` required | Full `tasks.md` required | Required | Required |
| Complex | Ambiguity, architectural uncertainty, new domain knowledge, security/PHI sensitivity, Legacy Codebase behavior migration, irreversible decisions, or significant risk | Full `specify.md` required, including assumptions and open questions | Full `design.md` required, including risks and ADR candidates | Full `tasks.md` required, including dependencies and verification expectations | Required with scope monitoring | Required with explicit residual risk |

Sizing must be conservative for brownfield clinical work. A change that appears small by file count can be Medium, Large, or Complex if it touches clinical behavior, PHI/security, persistence, Legacy Codebase behavior, schema, API contracts, bounded-context ownership, or ADR criteria.

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
| Specify | Define what is needed, why it matters, boundaries, assumptions, constraints, and acceptance criteria | SDD owns feature scope; Product and Domain docs provide inputs | Inline intent or `specify.md`; requirements; out-of-scope; assumptions; open questions; acceptance criteria | Hand Design enough scope to choose an implementation approach; hand Verify acceptance criteria |
| Design | Define how the solution should work and how it fits Doc Organo architecture | SDD owns feature design; ADRs own durable decisions; Architecture/Technical docs own durable explanations | Inline design note or `design.md`; affected components; integration points; risks; testing approach; ADR candidates | Hand Tasks an implementable approach; hand ADR governance any durable decision candidates |
| Tasks | Convert accepted scope and design into executable slices | SDD owns feature task plan; tasks do not create new scope | Inline atomic steps or `tasks.md`; sequencing; dependencies; requirement mapping; verification expectations | Hand Execute clear implementation slices; hand Verifier expected evidence |
| Execute | Implement the scoped change using focused context | Implementation session owns code/doc edits within accepted scope | Code, docs, tests, migration artifacts, or governance edits; recorded deviations when scope changes | Hand Verify diff, tests, skipped checks, deviations, and any SDD updates needed |
| Verify | Decide whether work is complete enough to continue | Verifier owns gate selection, review sensors, residual risk, and completion decision | Verification summary; gate evidence; skipped-gate reasons; review findings; residual risk; follow-up needs | Hand Documentation Update and ADR governance any durable follow-up signals |

If a phase is skipped, the next phase must explicitly carry the missing responsibility. For example, if Design is skipped, Tasks or Execute must state that the implementation follows an existing pattern and identify that pattern. If Tasks is skipped, Execute must begin with atomic implementation steps.

## Feature Lifecycle Ownership

| Lifecycle area | Primary owner | Decision authority | SDD responsibility |
|----------------|---------------|--------------------|--------------------|
| Specify | Active SDD | Product docs, domain docs, user-approved scope, and accepted ADRs constrain scope | Capture feature requirements, boundaries, assumptions, open questions, and acceptance criteria |
| Design | Active SDD | Accepted ADRs, Architecture docs, Technical docs, and approved domain models constrain design | Propose feature-level design, surface conflicts, identify ADR candidates, and preserve ownership boundaries |
| Tasks | Active SDD | Accepted Specify and Design constrain tasks | Convert scope and design into executable slices with dependencies, traceability, and verification expectations |
| Execute | Implementation session | Active SDD, rules, skills, accepted ADRs, and current code patterns constrain changes | Implement within scope, add required tests, and record deviations or escalation triggers |
| Verify | Verifier workflow | Verifier owns gate selection, review sensor use, residual risk, and completion judgment | Provide acceptance criteria, expected gates, risks, and traceability evidence for verifier evaluation |
| Documentation follow-up | Documentation Update workflow | Owning documentation artifact determines final routing | Identify candidate follow-up needs without duplicating routing decisions |
| ADR decisions | ADR governance | Accepted ADRs are the durable architecture authority | Identify ADR candidates and stop for escalation when unresolved decisions block safe design or execution |
| Operational truth | `Documentation/State.md` | `Documentation/State.md` owns current branch, runtime status, blockers, and next steps | Read current truth before planning and identify State update needs after verification |

## Phase Entry And Exit Criteria

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
- Design is specific enough to create implementation tasks or execute directly.

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

### Execute

Entry criteria:

- Scope is known through inline Specify or `specify.md`.
- Required Design and Tasks phases are complete or intentionally skipped with rationale.
- If Tasks are skipped, atomic implementation steps have been listed before editing.
- Task-specific context and rules/skills are loaded without broad unnecessary context.

Exit criteria:

- Implementation stays within accepted scope or records deviations.
- Required tests are added or updated as part of implementation.
- Known acceptance criteria are addressed.
- SDD artifacts are updated when implementation reveals scope, design, task, or verification changes.
- Evidence and skipped checks are ready for verifier review.

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

## Definition Of Done

A feature is not done solely because implementation finished. The governance-level Definition of Done applies to SDD-backed work and to simplified SDD flows where the same responsibilities are carried inline.

A feature may be considered complete only when:

- Implementation addresses the accepted scope or explicitly records approved deviations.
- Required automated tests were added, updated, or intentionally deferred with reason and residual risk.
- Applicable verification gates were selected and evaluated by verifier responsibility.
- Failed, skipped, blocked, or deferred gates have concrete explanations.
- Residual risk is classified and accepted at the appropriate level.
- ADR evaluation was performed when architecture, persistence, schema, security/auth, API contract, ownership, runtime, or irreversible decisions were involved.
- Documentation follow-up was evaluated for State, ADRs, Architecture docs, Technical docs, rules, skills, review prompts, and active SDD artifacts.
- Requirement traceability is complete enough for the feature size and risk.
- Legacy Codebase behavior decisions are explicit when behavior was preserved, adapted, or abandoned.

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

## Context Loading Strategy

SDD should support large-context AI workflows without encouraging indiscriminate loading.

Always load or consider:

- `Documentation/State.md` for current operational truth.
- `AGENTS.md` for stable bootstrap, authority hierarchy, and routing.

Load on demand:

- Active SDD artifacts for the current feature.
- Accepted ADRs when architecture, persistence, security, API, ownership, runtime, or irreversible decisions are relevant.
- Architecture docs for domain boundaries, business rules, and system structure.
- Technical docs for SQL migration, frontend architecture, runbooks, API guidance, or runtime behavior.
- Product docs for product intent, backlog sequencing, and roadmap boundaries.
- Review prompts when planning verification or reviewing changes.
- Task-specific workflows, such as Documentation Update, Verifier, SQL migration, codebase decomposition, or Legacy Codebase behavior workflows.
- Targeted Legacy methods only when behavior migration or characterization is in scope.

Context priorities:

1. Current truth and authority: `Documentation/State.md`, `AGENTS.md`, accepted ADRs.
2. Feature truth: active SDD artifacts.
3. Domain and architecture truth: Domain Overview, Architecture Overview, relevant Technical docs.
4. Implementation evidence: targeted code, tests, migrations, API/UI files.
5. Review and verification sensors: review prompts and verifier workflow.
6. Research or methodology notes only when creating or revising governance.

Context boundaries:

- Load the minimum set needed to decide or execute the current phase.
- Prefer targeted files and methods over entire folders.
- Do not load multiple unrelated feature SDDs in one implementation session.
- Do not load full Legacy folders; Legacy is behavioral reference, not implementation to revive.
- Do not use long chat history as the durable source of truth; summarize important findings into SDD, ADRs, State, or owning docs.

Context anti-patterns:

- Treating SDD as a place to paste all research.
- Loading every architecture and technical document for a Small change.
- Carrying stale roadmap assumptions instead of checking `Documentation/State.md`.
- Using review prompts as policy sources instead of sensors.
- Reading broad Legacy files when a targeted method is enough.

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
Requirement -> Design -> Task -> Implementation -> Test -> Verification -> Documentation Follow-up
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
- Documentation follow-up records whether requirement, architecture, operational, or governance truth changed and which owner must route it.

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

SDD identifies documentation follow-up needs but does not own documentation routing.

Documentation follow-up may be needed when:

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

The Verifier identifies documentation follow-up needs after gates and review sensors. Documentation Update decides the owning artifact and applies or recommends changes.

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
- Record whether each material behavior is preserved, adapted, or abandoned in Specify, Design, Tasks, verification evidence, or documentation follow-up as appropriate.
- Read targeted Legacy methods, not broad folders.
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
   - Medium: brief Specify; Design and Tasks only when risk or dependency requires them.
   - Large: full Specify, Design, Tasks, Execute, Verify.
   - Complex: full Specify, Design, Tasks, Execute with scope monitoring, Verify with explicit residual risk.
4. Load only task-specific context needed for the current phase.
5. Complete phase entry and exit criteria before moving forward.
6. Pause for escalation when authority, architecture, requirement, technical constraint, Legacy Codebase, or verification conflicts cannot be resolved safely.
7. If Tasks are skipped, list atomic implementation steps before editing.
8. Escalate to formal Tasks when hidden complexity appears.
9. Implement tests as part of executable work.
10. Hand off to Verifier for gate selection, review prompts, evidence, skipped checks, residual risk, and Definition of Done evaluation.
11. Evaluate documentation follow-up candidates before routing them through the owning workflow.

## Governance Consistency Review

Current improvements reduce ownership overlap by assigning:

- Feature scope, design, task planning, and expected verification to SDD.
- Gate selection, review prompt application, residual risk, and completion judgment to Verifier.
- Documentation routing and authority validation to Documentation Update.
- Durable architecture decisions to ADR governance.
- Current branch and runtime truth to `Documentation/State.md`.

Remaining gaps to monitor:

- SDD templates may need light updates later to reflect escalation, Definition of Done, verifier handoff, and traceability expectations.
- Existing docs and research notes may still describe this document as a roadmap rather than operational governance.
- `AGENTS.md` still uses implementation-specific Legacy Sheets wording; this is acceptable current context but may need future generalization.
- Review prompt coverage for EF migrations and API contracts remains a future candidate, not a current SDD responsibility.
- A completed feature SDD example is still needed to validate whether the governance is practical in real migration work.

Duplicated responsibilities to avoid:

- Do not let SDD choose final verification gates after implementation; it defines expectations, while Verifier selects and evaluates gates.
- Do not let SDD route documentation updates; it identifies candidates, while Documentation Update owns routing.
- Do not let SDD override ADRs; it proposes or escalates durable decisions.
- Do not let Legacy Codebase behavior override accepted architecture, documented business rules, security requirements, or approved domain models.

## Future Capability Recommendations

Future work may build on this governance, but should not be implemented by this document:

- Create a future `spec-workflow` skill only after this governance has been used on at least one real feature SDD.
- Normalize references to this governance in `AGENTS.md`, `CONTRIBUTING-AI.md`, documentation index, PR template, rules, and skills during a separate documentation update pass.
- Improve SDD templates after observing real usage against Prontuario, Agendamento, Atendimento, or another SQL migration slice.
- Consider template support for escalation outcomes, Definition of Done checks, verifier handoff, documentation follow-up evaluation, and Legacy Codebase behavior decisions.
- Add or refine review prompts for EF migrations and API contracts if recurring verification gaps appear.
- Use one completed SQL migration or Legacy Codebase behavior port as the reference SDD example.
- Consider lightweight traceability conventions in templates only after requirement IDs prove useful in real work.
- integration with reporting-strategy, a governance that creates session handoff, feature report, harness metrics, workflow observability. 
