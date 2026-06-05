# SDD Adoption Roadmap

## Purpose

This document defines how Spec-Driven Design should be adopted in the Doc Organo AI Harness. SDD is the planning bridge between product/architecture context and implementation. It should be used when a change is large enough, risky enough, or cross-cutting enough that direct coding would increase ambiguity.

This roadmap does not create feature specifications or implementation tasks. It defines when SDD should be used, what each artifact should own, and how SDD should integrate with testing, verification gates, security review, and MCP usage.

## Responsibilities

SDD should own:

- Feature-specific scope and intent.
- Explicit goals and out-of-scope decisions.
- Acceptance criteria.
- Reuse and architecture design for the feature.
- Data model, API, frontend, and risk analysis for the feature.
- Executable task breakdown after design is accepted.
- Verification expectations for the feature.
- ADR trigger analysis for durable decisions discovered during design.
- Post-merge State update requirement.

SDD should not own:

- Permanent project bootstrap context. That belongs in `AGENTS.md`.
- Current branch status outside the feature. That belongs in `Documentation/State.md`.
- Product-wide strategy. That belongs in PRD, PM, and RoadMap.
- Accepted architecture decisions. Those belong in ADRs.
- General coding conventions. Those belong in rules and technical docs.
- Full research history. That belongs in research artifacts.

## SDD Artifact Model

| Artifact | Purpose | Responsibilities | Inputs | Outputs | Update triggers | Relationship with other components |
|----------|---------|------------------|--------|---------|-----------------|------------------------------------|
| `specify.md` | Define the problem and outcome | Context, problem, goals, out-of-scope, user stories, acceptance criteria, ubiquitous language | PRD, PM epic, State, Domain Overview, user request | Agreed feature boundary | New feature, scope change, acceptance criteria change | Feeds `design.md`; should prevent product and domain ambiguity |
| `design.md` | Define the technical approach | Architecture overview, components, reuse analysis, data model, API/front changes, risks, ADR needs | `specify.md`, codebase exploration, architecture docs, technical docs, ADRs, ADR Governance Policy | Technical design ready for task breakdown; ADR recommendation when needed | Architecture change, risk discovery, API/data/front decision | Feeds `tasks.md`; may trigger ADR under formal criteria |
| `tasks.md` | Convert design into executable work | Task checklist, dependencies, parallelization, verification commands, verifier handoff, commit guidance, post-merge update | `specify.md`, `design.md`, verification policy, verifier role | Implementation plan with gate expectations | Design accepted, task split changes, verification criteria change | Guides implementation and PRs; should update State after completion |

## When To Use SDD

Use SDD when:

- Work is expected to take more than about half a day.
- The change touches multiple layers, such as API, domain, EF, and Blazor.
- The change affects clinical rules, PHI/security, auth, persistence, or data history.
- Legacy behavior must be ported.
- A new schema or API contract decision is needed.
- The task may require an ADR.
- The task needs explicit verifier ownership because skipped gates or residual risk would matter.
- The user asks for planning, architecture, or governance.

Do not require full SDD for:

- Small bug fixes with local scope.
- Documentation typo fixes.
- Mechanical refactors with no behavior change.
- Test-only additions that follow an existing pattern.

For small but risky work, use a short plan plus the relevant review prompt instead of full SDD.

## Current Adoption Assessment

Existing foundation:

- Templates exist for specify, design, and tasks.
- `AGENTS.md`, `CONTRIBUTING-AI.md`, PRD, and PR template already reference SDD behavior.
- PM and migration docs provide likely SDD inputs.

Current blockers:

- SDD path references are inconsistent across docs.
- No feature SDD folders exist yet.
- `check-docs.md` is empty, so documentation review is not a working sensor.
- Verification gates are repeated in docs but not automated or consistently assigned to a verifier role.
- Active SQL migration work is still mostly driven by PM/State rather than feature SDD artifacts.

## Adoption Roadmap

### Phase 1: Normalize The Model

Goal: make SDD understandable before using it heavily.

Architecture decisions:

- Canonicalize the distinction between template path and feature instance path.
- Keep templates under the AI Harness area.
- Place per-feature SDD folders under a predictable path.
- Document that SDD is required by risk and size, not by every change.

Governance outputs:

- `AGENTS.md` points to the canonical SDD policy.
- `CONTRIBUTING-AI.md` references the same paths.
- PR template references feature SDDs consistently.
- Documentation index points to the actual template location.

### Phase 2: Use SDD For One Real Slice

Goal: validate the SDD process on a real but bounded feature.

Best candidates:

- Prontuario SQL migration.
- Agendamento SQL migration.
- Atendimento Legacy validation port.
- Security/auth minimum scope planning.

Selection criteria:

- Meaningful enough to need design.
- Small enough to finish.
- Connected to the active MVP2 SQL migration.
- Has clear verification gates.

Expected result:

- One completed feature SDD becomes the reference example for future agents.
- Gaps in templates are fixed after observing real usage.

### Phase 3: Integrate Verification

Goal: make SDD tasks produce verifiable completion.

Verification expectations:

- Every `tasks.md` includes commands and manual checks appropriate to the change.
- Clinical/security changes include review prompts.
- SQL migration tasks include integration or characterization test expectations.
- Front/API changes include Swagger or browser smoke checks when relevant.
- Post-merge includes `Documentation/State.md` update.

### Phase 4: Mature Governance

Goal: connect SDD with durable architecture and operating practices.

Adoption points:

- ADRs are created when SDD design reveals a durable decision.
- Skills consume SDD artifacts when implementing a feature.
- Review prompts become part of the PR routine.
- CI becomes the automated baseline sensor.
- MCP usage is documented where it provides runtime evidence.

## SDD In The Harness Lifecycle

SDD sits between Plan and Implement in the harness lifecycle:

```text
Research
->
Plan
->
SDD when risk, size, or cross-module scope requires it
->
Implement
->
Verification
->
State / ADR / Documentation Updates
```

SDD is required only when it adds clarity. It should not absorb the whole lifecycle:

- Research findings should be summarized into SDD only when they affect the feature.
- Plan decides whether SDD is needed.
- SDD defines feature scope, design, tasks, ADR needs, and verification expectations.
- Implementation follows SDD but may return to design if new risks appear.
- Verification checks SDD acceptance criteria and gate expectations.
- State, ADRs, or durable docs are updated only when completion or decisions change project truth.

## ADR Governance In SDD

SDD should surface durable decisions early. During `design.md`, apply the ADR Governance Policy from `harness-architecture.md`.

Create or update an ADR when SDD introduces or changes a durable decision involving:

- Bounded context ownership, aggregate boundaries, or business-rule location.
- Persistence strategy, schema lifecycle, migration approach, soft delete, or data history.
- Security, PHI, auth, authorization, logging, or secret-handling architecture.
- Public API contract, ID strategy, DTO shape policy, or API/front compatibility expectations.
- Significant infrastructure, runtime, framework, integration, MCP, or deployment strategy.
- Durable exceptions to existing architecture rules.
- Cross-feature decisions that future work must respect.

Do not create an ADR for:

- Local implementation details inside the feature.
- Task sequencing that belongs in `tasks.md`.
- Acceptance criteria that belong in `specify.md`.
- Temporary blockers or runtime status that belong in `Documentation/State.md`.
- Decisions already covered by an accepted ADR.

If ADR criteria are met, `design.md` should identify the needed ADR before `tasks.md` treats the design as ready for implementation.

## Verifier Handoff From SDD

SDD should make verification explicit enough for the verifier role to act.

`specify.md` should provide:

- Acceptance criteria.
- User-visible behavior.
- Out-of-scope decisions.
- Domain language and clinical/security expectations.

`design.md` should provide:

- Testability risks.
- Data/API/front behavior that needs smoke testing.
- Security/PHI review triggers.
- ADR needs.

`tasks.md` should provide:

- Concrete automated commands when known.
- Manual smoke checks when API/UI behavior changes.
- Review prompts that apply.
- SQL integration or characterization test expectations when persistence or Legacy behavior is touched.
- State update expectations.
- Any known gates that may need to be skipped and why.

The verifier checks whether these gates were satisfied, skipped with reasons, or left as residual risk.

## Testing Adoption Strategy

Testing should grow with migration risk:

1. Keep `dotnet build` as the base gate.
2. Keep `dotnet test` as the normal automated test gate.
3. Add SQL integration tests for repository slices, starting with the active clinical migration.
4. Add characterization tests before porting Legacy Atendimento validations.
5. Add front/service/component tests only after API contracts stabilize.
6. Add E2E tests after the core clinical flows are stable enough to avoid brittle automation.

Testing responsibilities:

- SDD `specify.md` defines acceptance criteria.
- SDD `design.md` identifies testability risks.
- SDD `tasks.md` defines the concrete verification commands.
- Review prompts catch domain and security concerns not covered by tests.
- State records what is currently verified and what remains blocked.
- The verifier role decides which gates apply and records skipped gates or residual risk.

## Verification Gates Adoption Strategy

Target ladder:

1. Build: `dotnet build`.
2. Unit tests: `dotnet test`.
3. Integration tests: SQL or external dependency checks when touched.
4. Manual smoke: Swagger/API or Blazor browser path when behavior changes.
5. Review prompts: domain, security/PHI, docs, EF/API as applicable.
6. PR checklist: final human-readable gate.

Governance principle:

- A gate may be skipped only with a reason recorded in the final report, PR, or State if it affects current truth.

Future verifier role:

- A verifier skill or checklist should decide which gates apply, execute or request them, and summarize residual risk.
- The verifier should not rely on "tests pass" as the only completion signal for domain or PHI-sensitive work.
- SDD `tasks.md` should hand the verifier an explicit gate list instead of leaving gate selection implicit.

## Security Review Adoption Strategy

Security review should apply whenever a change touches:

- Patient identity fields, CPF, clinical data, medical records, care journey, files, logs, prompts, commits, API exposure, tests, auth assumptions, or configuration secrets.

Security review inputs:

- `security-phi.mdc`.
- `security-phi-review.md`.
- PRD constraints.
- ADR-004 for auth as a supporting capability.
- The diff and any generated examples or tests.

Security review outputs:

- Critical findings for PHI exposure, credentials, real data, or unsafe logs.
- Suggestions for auth assumptions, API field exposure, and synthetic test data.
- OK result when no security issue is found.

Future adoption:

- Keep `security-phi.mdc` short and always-on.
- Use `security-phi-review.md` for depth.
- Promote recurring findings into concise rules only when they are stable.

## MCP Usage Adoption Strategy

Phase 1 MCP usage:

- GitHub for issues, PRs, comments, checks, and project coordination.
- Browser for Swagger and Blazor smoke tests.

Phase 2 MCP usage:

- Documentation search MCP if local docs continue to grow and path normalization is complete.
- DB introspection MCP only after SQL migration practices and security constraints are clear.

MCP governance:

- Use MCP when live external state or browser evidence matters.
- Do not use MCP results as hidden context only; summarize decisions into SDD, PRs, State, or ADRs when they change project truth.
- Do not build custom MCPs before GitHub/browser workflows and verification gates are reliable.

## Inputs

SDD adoption depends on:

- `Documentation/Product/PRD.md` for product intent.
- `Documentation/Product/PM_DocOrgano.md` for active backlog and sequencing.
- `Documentation/Product/RoadMap.md` for future boundaries.
- `Documentation/State.md` for current branch truth.
- `Documentation/Architecture/Domain_Overview_Business_Rules.md` for ubiquitous language and clinical rules.
- `Documentation/Architecture/ADR/` for durable decisions.
- `Documentation/Technical/migration-sql.md` for SQL migration sequencing.
- Review prompts for pre-merge sensors.
- Rules and skills for agent behavior during implementation.

## Outputs

Successful SDD adoption should produce:

- Clear feature boundaries before code changes.
- Smaller and safer implementation sessions.
- Better task sequencing and parallelization.
- Explicit verification criteria.
- Earlier detection of ADR needs.
- Less reliance on long chat history.
- Better State updates after completed work.

## Update Triggers

Update this roadmap when:

- The canonical SDD path changes.
- A first real feature SDD reveals template gaps.
- Verification gates become automated or stricter.
- New review prompts become standard.
- MCP usage matures beyond GitHub/browser.
- Product or architecture governance changes the threshold for SDD.

Update SDD templates when:

- Real usage shows missing sections.
- Review prompts identify recurring SDD omissions.
- New verification gates become standard.
- ADR creation criteria need to be clearer.

## Relationship With Other Harness Components

| Component | Relationship |
|-----------|--------------|
| `AGENTS.md` | Defines when SDD applies and points to canonical paths. |
| `.cursor/rules/` | Rules can remind agents to use SDD for risky work. |
| `.cursor/skills/` | Skills can produce SDD inputs or consume SDD tasks during implementation. |
| `Documentation/State.md` | SDD starts from current state and updates State after meaningful completion. |
| Review prompts | Review prompts are verification sensors referenced by SDD tasks. |
| Verifier role | The verifier confirms SDD verification expectations, skipped gates, and residual risk. |
| ADRs | SDD design identifies durable decisions that require ADRs; accepted ADRs outrank feature SDD when conflicts appear. |
| MCPs | MCPs provide live evidence for SDD validation, PR checks, and smoke testing. |
| Auxiliary docs | PRD, PM, RoadMap, Architecture, ADRs, and Technical docs provide SDD context. |

## Recommendations For Future Operational Updates

These recommendations are for a later operational update pass:

- Normalize SDD template and feature folder paths across `AGENTS.md`, `CONTRIBUTING-AI.md`, documentation index, PR template, and rules.
- Fill `check-docs.md` so documentation consistency becomes a real review sensor.
- Add EF migration and API contract review prompts when those review categories stabilize.
- Add a verifier workflow or skill before expanding the number of review prompts too far.
- Use one SQL migration slice as the first real SDD example before requiring SDD broadly.
