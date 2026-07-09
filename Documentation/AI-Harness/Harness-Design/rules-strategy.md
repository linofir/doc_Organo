# Harness Rules

## Purpose

This document defines how Doc Organo should use Harness rules (Cursor: `.cursor/rules/*.mdc`; Cline: `.clinerules/*.md`) as concise, persistent AI guardrails. Rules should prevent recurring mistakes, protect sensitive clinical data, and keep agents aligned with stable project conventions without overloading every session with long documentation.

Rules are not the place for full workflows, feature plans, research notes, or detailed explanations. When guidance becomes procedural, it should move to a skill or durable documentation.

## Responsibilities

Rules should own:

- Always-on safety constraints, especially PHI, credentials, logs, prompts, commits, and test data.
- Short project invariants that agents must follow repeatedly.
- File-scoped implementation conventions for backend, EF migrations, and Blazor Server.
- Context loading guardrails such as reading `Documentation/State.md` and `AGENTS.md` for large tasks.
- Documentation update reminders when architecture or current state changes.
- Short implications of accepted ADRs when they must shape repeated agent behavior.
- Clear links to canonical docs for deeper explanation.

Rules should not own:

- Full architecture narrative.
- Long checklists.
- SDD templates.
- Feature plans or task breakdowns.
- Complete migration workflows.
- Review prompts that require diff analysis.
- Methodology research.

## Current Rule Inventory

| Rule | Load policy | Current responsibility | Assessment |
|------|-------------|------------------------|------------|
| `security-phi.mdc` | Always-on | PHI, secrets, credentials, logging restrictions | Essential and correctly short. Should remain always-on. |
| `token-economy.mdc` | Always-on | State/AGENTS loading, targeted Legacy reads, markdown handoff | Useful. Keep aligned with `Documentation/SDD/` and current harness paths. |
| `update-doc.mdc` | Always-on | Update State for architecture changes; remind agents to apply ADR governance for durable decisions | Useful but not listed in `Documentation/AI-Harness/rules.md`; "significant decision" should be replaced later with the formal ADR policy. |
| `dotnet-ddd.mdc` | `DocAPI/**/*.cs` | Backend CA light, ubiquitous language, repository boundaries, abstraction gate, Financial deferral | Well scoped. Should remain concise and point to Domain Overview for detail. |
| `ef-migrations.mdc` | SQL DB and migrations | EF migration rules, soft delete, owned types, connection string safety | Useful but has a stale ADR path. Could later include migration verification expectations. |
| `blazor-front.mdc` | `DocFront.Web/**/*` | Blazor Server conventions, data flow, API URL, Atendimento UI deferral | Well scoped. Should continue to protect against WASM assumptions and direct `HttpClient` usage in UI. |

## Missing Or Candidate Rules

Candidate rules should only be added if the guidance is short, stable, and frequently needed.

| Candidate | Rationale | Recommended form |
|-----------|-----------|------------------|
| Verification gate rule | Build/test/smoke/review expectations are repeated but not consistently enforced | Add only after deciding exact gate policy; keep short and link to contributing/review prompts |
| Legacy behavior rule | Legacy is behavior reference and should be read by targeted method | May already be covered by `token-economy.mdc` and `dotnet-ddd.mdc`; avoid duplication unless failures recur |
| API contract rule | Controllers, DTOs, `Guid` IDs, and response consistency are recurring risks | Prefer a future API contract doc and review prompt first; add rule only when conventions stabilize |
| SQL migration slice rule | Active migration has repeated per-entity checklist | Prefer a skill because this is procedural |
| Security auth assumption rule | Clinical API work must make auth assumptions explicit | Could be added to `security-phi.mdc` if recurring and concise |

Rules not recommended:

- A large Clean Architecture rule that duplicates Architecture Overview.
- A full SDD rule that duplicates templates.
- A Financial rule beyond the existing deferral guardrail.
- A research methodology rule beyond token economy/context loading.

## Inputs

Rules should be derived from:

- Repeated defects or review findings.
- Accepted ADRs.
- The ADR Governance Policy in `harness-architecture.md`.
- Verifier findings that reveal recurring preventable mistakes.
- Security/PHI policy.
- Stable project conventions in `AGENTS.md`.
- Canonical architecture and technical docs.
- Lessons from SDD and PR reviews.

Rules should not be derived directly from speculative research until that research becomes stable project policy.

## Outputs

Expected outputs:

- Agents avoid sensitive data exposure.
- Agents load the right context without bloating every session.
- Agents preserve backend and frontend architectural boundaries.
- Agents avoid premature Financial work and unnecessary abstractions.
- Agents handle EF migrations without breaking soft delete, owned types, or secret handling.
- Agents know when to update State or create ADRs.
- Agents know when to route verification concerns to a verifier workflow instead of treating a rule as a full checklist.

## Update Triggers

Create or update a rule when:

- The same mistake appears in more than one review or session.
- A new security invariant must always apply.
- An ADR creates a stable coding constraint.
- The ADR Governance Policy changes what counts as a durable decision.
- Verifier findings repeatedly show the same preventable risk.
- A new file area needs scoped guidance.
- A rule points to stale paths or stale policy.
- A short guardrail can prevent high-risk behavior.

Do not create or update a rule when:

- The guidance is task-specific.
- The workflow needs multiple steps or examples.
- The guidance is still experimental.
- The content would duplicate a durable doc.
- A review prompt is the better sensor.

## Relationship With Other Harness Components

| Component | Relationship |
|-----------|--------------|
| `AGENTS.md` | `AGENTS.md` summarizes project context; rules enforce specific recurring guardrails. |
| `Documentation/State.md` | Rules can require State reads/updates, but State owns current status. |
| Harness skills (Cursor: `.cursor/skills/`; Cline: `.cline/skills/`) | Skills should absorb procedural guidance that is too long for rules. |
| Review prompts | Review prompts provide deeper diff-based checks; rules provide concise preventive guidance. |
| SDD | Rules may trigger SDD for larger/riskier work, but SDD owns feature planning. |
| ADRs | ADRs explain decisions; rules encode short implications of accepted decisions. |
| Verifier role | The verifier decides which gates apply; rules may provide short reminders but should not become verification workflows. |
| `Documentation/AI-Harness/rules.md` | Rules index should list all rules, scopes, load policy, purpose, and update trigger. |

## ADR And Verification Boundaries

Rules should not decide ADRs or certify verification by themselves.

ADR boundary:

- Rules may remind agents to apply the ADR Governance Policy.
- Rules may encode concise implications of accepted ADRs.
- Rules should not contain full ADR creation criteria, historical rationale, or decision records.
- If a rule would need examples and exceptions to explain an architectural decision, the durable explanation belongs in an ADR or architecture doc.

Verification boundary:

- Rules may require that verification not be skipped silently.
- Rules may point to the verifier workflow or review prompts.
- Rules should not contain multi-step gate selection logic.
- If verification guidance depends on diff scope, SDD tasks, review prompts, and residual risk, it belongs to the verifier role or a verifier skill.

## Recommendations For Future `rules.md` Updates

These recommendations are for a later operational update pass:

- Add `update-doc.mdc` to the rules index.
- Add a column for purpose, not only scope and `alwaysApply`.
- Add a column for update trigger.
- Record whether a rule is preventive, architectural, security, or workflow-routing.
- Note that workflows longer than a small rule should become skills.
- Replace vague ADR language with a concise pointer to the formal ADR Governance Policy.
- Add verifier-related wording only after the verifier workflow is operationally defined.
- Normalize stale paths, especially old `docs/` compatibility paths and ADR links.

## Rule Design Standard

A Doc Organo rule should meet these conditions:

1. It is short enough to load frequently.
2. It states behavior, not background.
3. It points to a canonical doc for deeper context.
4. It has a clear file scope or is genuinely always-on.
5. It protects a real project risk.
6. It avoids duplicating a skill, review prompt, or SDD template.
