# Doc Organo AI Harness Research

> Canonical research baseline for the Doc Organo AI Harness.  
> Use this during Research and Plan phases. Do not paste long research chats into implementation sessions.

## Purpose

This document consolidates the research behind the Doc Organo AI Harness and records the current architecture conclusions after the harness design review. It is not an operational checklist, daily status file, or replacement for the design documents under `Documentation/AI-Harness/Harness-Design/`.

Use this file to understand why the harness exists, what problems it is solving, which adoption risks remain, and which artifacts should drive future operational updates.

## Canonical Sources

| Source | Role | Status |
|--------|------|--------|
| `Documentation/AI-Harness/research/playbook.md` | Methodology: RPI, context engineering, SDD, harness concepts | Canonical after this migration |
| `Documentation/AI-Harness/Harness-Design/harness-architecture.md` | Target harness architecture and governance model | Canonical design |
| `Documentation/AI-Harness/Harness-Design/agents-strategy.md` | `AGENTS.md` responsibility model | Canonical design |
| `Documentation/AI-Harness/Harness-Design/rules-strategy.md` | Rules responsibility and growth policy | Canonical design |
| `Documentation/AI-Harness/Harness-Design/skills-strategy.md` | Skills responsibility and candidate workflows | Canonical design |
| `Documentation/AI-Harness/Harness-Design/sdd-adoption-roadmap.md` | SDD adoption, testing, verification, security, MCP roadmap | Canonical design |
| `Documentation/State.md` | Current branch/runtime truth | Read before large tasks |
| `AGENTS.md` | Static project bootstrap | Stable context only |
| `Documentation/Product/PRD.md` | Stable product intent | Canonical WHY |
| `Documentation/Product/PM_DocOrgano.md` | Active backlog and sequencing | Operational planning |
| `Documentation/Architecture/` | Domain, architecture, ERD, ADRs | Durable architecture context |
| `Documentation/Technical/` | SQL migration, runbook, front architecture | Durable technical context |

Historical note: previous references to `Documentation/studies.md` and `Documentation/AI-Harness/Research/*` were stale. The active research location is now `Documentation/AI-Harness/research/`.

## Executive Summary

Doc Organo has moved from prompt engineering toward harness engineering: the goal is no longer to write better one-off prompts, but to create a reliable environment where agents receive the right context, produce explicit plans, execute in small slices, and verify their work.

The current harness is coherent but still maturing:

- `AGENTS.md`, rules, skills, SDD templates, review prompts, PRD, PM, ADRs, and technical docs exist.
- Responsibilities are now defined in dedicated harness design documents.
- The design layer now defines ADR governance, artifact authority hierarchy, harness lifecycle flow, and the verifier role concept.
- Path drift remains a known governance issue and should be handled in a later operational update pass.
- Verification ownership is defined conceptually but not yet operationalized through CI, a verifier skill, or routine review-prompt usage.
- SQL migration and clinical safety remain the strongest drivers for harness maturity.

The next architecture risk is not lack of documentation. It is inconsistent adoption: stale context, skipped verification, missing feature SDDs, and unclear ownership between rules, skills, State, and review prompts.

## Research Conclusions

### 1. Codebase Is Context

The repository itself is the primary context source. Agents should not rely on repeated prompt summaries for stable facts. Instead:

- `AGENTS.md` provides static bootstrap context.
- `Documentation/State.md` provides current branch truth.
- Product, architecture, technical, and ADR docs provide durable context.
- Rules and skills route behavior before code changes.
- SDD artifacts carry feature-specific intent from Plan to Implement.

Implication: documentation should be concise, canonical, and easy to route. Duplicate or stale paths are not cosmetic problems; they degrade agent behavior.

### 2. RPI Is The Core Workflow

The harness follows Research -> Plan -> Implement:

1. **Research** explores code, docs, risks, and available workflows.
2. **Plan** creates durable markdown artifacts such as harness design docs or SDD files.
3. **Implement** uses focused context, small changes, and verification gates.

Research chat history should not be carried into implementation. Durable markdown is the handoff.

### 3. Static And Dynamic Context Must Stay Separate

`AGENTS.md` should hold stable project context. `Documentation/State.md` should hold current runtime truth. Research docs should explain conclusions and rationale. SDD should hold feature-specific plans.

Mixing these concerns creates session amnesia and stale assumptions. The design documents now define this split explicitly.

### 4. Rules And Skills Have Different Jobs

Rules should be short guardrails:

- PHI/security constraints.
- Token/context loading.
- Backend, EF, and Blazor conventions.
- Documentation update reminders.

Skills should be repeatable workflows:

- Session orientation.
- Codebase decomposition.
- Harness planning.
- Future SQL migration slices.
- Future Legacy Atendimento rule porting.
- Future verifier/security review workflow.

When a rule becomes procedural, it should become a skill. When a skill becomes a stable invariant, the concise part may become a rule.

### 5. Verification Is A Harness Component

Tests, smoke checks, review prompts, and PR checklists are sensors. They complete the loop from guidance to evidence.

Current target ladder:

1. `dotnet build`
2. `dotnet test`
3. SQL integration tests when persistence is touched
4. Swagger or Blazor smoke checks when API/UI behavior changes
5. Domain and security/PHI review prompts when relevant
6. PR checklist as final human-readable gate

The design target is a verifier role that decides which gates apply, runs or requests them, applies review prompts, records skipped gates with reasons, and summarizes residual risk. This role may initially remain a checklist or skill-backed workflow before CI enforces the baseline.

### 6. Security And PHI Must Be Always-On

Doc Organo handles real clinical workflows. Even in a private repository and controlled MVP environment, patient-identifiable and clinical data must be treated as sensitive in:

- Code and logs.
- Tests and examples.
- Documentation.
- Prompts and agent outputs.
- Commits and PRs.
- File extraction and generated documents.

Security should exist at multiple depths: always-on rule, review prompt, PR checklist, and future verifier workflow.

### 7. SDD Should Be Risk-Based

Spec-Driven Design is valuable for changes that are large, risky, or cross-layer. It should not become process overhead for every small edit.

Use SDD for:

- SQL migration slices that affect API/domain/EF/front.
- Legacy behavior porting.
- Clinical rules.
- PHI/security/auth-sensitive work.
- API contract or schema changes.
- Work likely to need ADRs.

Do not require full SDD for local bug fixes, typo fixes, or mechanical refactors.

### 8. MCP Adoption Should Stay Incremental

Immediate MCP priorities:

- GitHub for issues, PRs, comments, checks, and project coordination.
- Browser for Swagger and Blazor smoke tests.

Custom MCPs should wait until local documentation, verification, and SQL migration practices are stable. MCP results should be summarized into durable artifacts when they affect project truth.

### 9. Authority Must Be Explicit

The harness needs a conflict-resolution model because different artifacts own different kinds of truth:

- ADRs own accepted durable architecture decisions.
- Architecture and Technical docs explain the current durable model.
- Harness Design docs own target governance for agent artifacts.
- `Documentation/State.md` owns current branch, runtime, blockers, and next steps.
- SDD owns feature-specific scope, design, tasks, and verification expectations.
- Rules and skills own machine-facing guardrails and workflows.

Implication: future operational refactors should implement this hierarchy instead of letting `AGENTS.md`, rules, skills, and State duplicate or contradict each other.

### 10. ADRs Need Objective Creation Criteria

Research confirmed that "significant decision" is too vague as an ADR trigger. The design target is an ADR policy that creates ADRs for durable choices involving architecture boundaries, persistence, schema/data lifecycle, security/auth/PHI, API contracts, major technology choices, durable exceptions, and cross-feature decisions discovered during SDD.

ADR governance should also prevent inflation: local bug fixes, mechanical refactors, temporary State, feature task sequencing, and decisions already covered by accepted ADRs should not become new ADRs.

## Current Harness Architecture Assessment

| Area | Current state | Risk | Direction |
|------|---------------|------|-----------|
| `AGENTS.md` | Useful static bootstrap | Stale paths and missing source references | Keep compact; normalize paths later |
| Rules | Good initial set | `update-doc.mdc` not indexed; stale paths in some rules | Keep rules short; use skills for workflows |
| Skills | Good starter set | Duplicate harness planning skill path; missing migration/verifier skills | Add only after workflow is proven |
| `State.md` | Designed as current operational truth | Can become stale and should not become history | Update after sessions/PRs when current truth changes |
| Verifier role | Defined conceptually in design | Not yet operationalized as skill, checklist, or CI routine | Create operational verifier workflow before broad refactor adoption |
| Review prompts | Domain and security prompts exist | `check-docs.md` empty; prompts not yet routine | Treat prompts as verification sensors orchestrated by verifier |
| SDD | Templates exist | No feature SDD instances; path drift | Use one real SQL slice as reference example |
| MCPs | GitHub/browser identified | Custom MCPs could be premature | Adopt after verification stabilizes |
| Auxiliary docs | Strong product/architecture baseline | Multiple path references remain stale | Use taxonomy from ADR-003 |

## Doc Organo-Specific Architecture Findings

The harness should reinforce these project constraints:

- Active development is centered on `feature/base_DB`.
- `main` remains the Google Sheets behavior reference.
- SQL migration should be incremental: Paciente -> Prontuario -> Agendamento -> Atendimento.
- Financial domain expansion is deferred until clinical SQL migration is stable.
- Legacy Sheets code is behavioral reference, especially for Atendimento validations.
- Business rules should live in domain entities or thin application use cases, not repositories or controllers.
- Identity/Auth is a supporting capability for MVP2, not a bounded context unless it grows into a product concern.
- Tests are verification sensors, not a bounded context.
- .NET 7 EOL should be planned after current SQL/harness stabilization.

## Known Gaps

These are governance gaps, not implementation tasks:

- Stale path references from `docs/` to `Documentation/`.
- Previous references to absent `studies.md`.
- Previous references to absent `Documentation/AI-Harness/Research/*`.
- Empty `Documentation/AI-Harness/review-prompts/check-docs.md`.
- Missing `Documentation/Technical/api-contract.md`.
- Verifier role is defined in design but missing as an operational workflow or skill.
- ADR creation policy must be implemented operationally after the design update.
- Artifact authority hierarchy must be reflected in operational artifacts during the refactor.
- Harness lifecycle flow must be reflected in contribution and SDD guidance during the refactor.
- Missing SQL migration slice skill.
- Missing Legacy Atendimento porting skill.
- No feature SDD example yet.
- CI not yet enforcing build/test.
- Review prompts are not yet routine gates.

## Adoption Strategy

Near-term harness adoption should focus on governance clarity:

1. Treat the Harness-Design documents as the foundation for future operational updates.
2. Normalize paths only in a dedicated operational update pass.
3. Use the next meaningful SQL migration slice as the first real SDD example.
4. Add verification workflow before expanding review prompts too far.
5. Add skills only for workflows that will repeat.
6. Keep custom MCPs deferred until local gates and docs are reliable.

## Relationship To Harness Design Documents

This file explains research conclusions and context. The design documents define the target architecture:

- `harness-architecture.md` is the umbrella architecture.
- `agents-strategy.md` defines `AGENTS.md` ownership.
- `rules-strategy.md` defines rule ownership and growth policy.
- `skills-strategy.md` defines skill ownership and candidate workflows.
- `sdd-adoption-roadmap.md` defines SDD, testing, verification, security, and MCP adoption.

When research conclusions change, update this file. When governance decisions change, update the design documents. When operational instructions change, update the operational artifact in a separate pass. The current design baseline expects the operational refactor to preserve ADR policy, verifier ownership, artifact authority hierarchy, and the harness lifecycle flow.

## References

- `Documentation/AI-Harness/research/playbook.md`
- `Documentation/AI-Harness/Harness-Design/harness-architecture.md`
- `Documentation/AI-Harness/Harness-Design/agents-strategy.md`
- `Documentation/AI-Harness/Harness-Design/rules-strategy.md`
- `Documentation/AI-Harness/Harness-Design/skills-strategy.md`
- `Documentation/AI-Harness/Harness-Design/sdd-adoption-roadmap.md`
- `Documentation/AI-Harness/documentation-index.md`
- `Documentation/Architecture/ADR/ADR-003-documentation-taxonomy.md`
