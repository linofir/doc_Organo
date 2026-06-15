# Reporting Strategy

## Purpose

This document defines the lightweight reporting strategy for the Doc Organo AI Harness. Reporting exists to preserve continuity across sessions, summarize completed feature work, and capture lessons learned that may improve future governance.

Reporting is communication, not authority. It must not replace `Documentation/State.md`, active SDD artifacts, Verifier outputs, ADRs, PM, RoadMap, or Documentation Update.

## Scope

Reporting v1 supports the first SDD pilot preparation and follow-up. It covers:

- Session handoff.
- Feature report.
- Lessons learned.
- Future workflow observability boundaries.

This v1 does not define full metrics, dashboards, release reporting, external-tool ingestion, MCP reporting, or autonomous workflow observability.

## Responsibilities

Reporting owns:

- Short, reviewable summaries that help the next session continue from the right place.
- Feature-level completion summaries after verification.
- Lessons learned that may inform later template, skill, rule, review prompt, or governance updates.
- A lightweight vocabulary for future reporting and observability.

Reporting does not own:

- Current branch, runtime status, blockers, or next steps. Those belong in `Documentation/State.md`.
- Feature scope, requirements, design, tasks, or expected verification. Those belong in active SDD artifacts.
- Gate selection, skipped gates, residual risk, or completion decisions. Those belong to Verifier responsibility.
- Durable architecture decisions. Those belong in ADRs.
- Backlog sequencing. That belongs in PM.
- Future product horizons. Those belong in RoadMap.

## Reporting Outputs

### Session Handoff

Use when a session pauses, changes phase, or leaves work for a later session.

It should answer:

- What phase was active?
- What was completed?
- What remains next?
- What files or artifacts matter for continuation?
- What blockers or open questions remain?
- What verification or documentation follow-up is pending?

Session handoff should be concise. It is not a replacement for State, SDD, or Verifier outputs.

### Feature Report

Use after a feature or pilot slice has gone through verification.

It should answer:

- What feature or slice was completed?
- Which requirements or acceptance criteria were addressed?
- Which implementation areas changed?
- Which verification gates and review sensors were evaluated?
- What residual risk remains?
- What follow-up was routed or deferred?
- What lessons should influence future templates or workflows?

Feature reports should link to the active SDD and verification output when they exist.

### Lessons Learned

Lessons learned capture workflow findings, not historical narrative.

They should be considered when:

- A template section was missing or confusing.
- A verifier handoff lacked enough evidence.
- Reporting duplicated State or verification output.
- A recurring agent failure suggests a future rule, skill, review prompt, or template update.
- A pilot reveals that governance is too heavy or too thin.

Lessons learned should become durable changes only after Documentation Update routes them to the correct owner.

## Relationship With State

`Documentation/State.md` owns current operational truth. Reporting may summarize State-relevant findings, but it must not become the canonical place for branch status, runtime status, blockers, or next steps.

If reporting reveals current truth changed, route the update through Documentation Update and then update State.

## Relationship With Feature SDDs

Feature SDDs live under `Documentation/SDD/`.

Reporting may link to a feature SDD, summarize progress, and capture lessons from using it. Reporting must not change feature scope, acceptance criteria, design, tasks, or expected verification.

## Relationship With Verifier

Verifier outputs are the source for gate status, skipped gates, residual risk, and completion decisions.

Reporting may summarize verification results after they exist. It must not select gates or accept residual risk.

## Relationship With Future Metrics

Future workflow observability may track:

- SDD phases completed.
- Verification gates selected and skipped.
- Review sensors used.
- Template gaps discovered.
- Documentation follow-up routed.
- Repeated residual risks.

These are future capabilities. They should not be implemented until the SDD pilot has produced enough real evidence to justify metrics.

## v1 Policy

For MVP2, keep reporting lightweight:

- Use session handoff when continuity would otherwise depend on chat history.
- Use feature report after meaningful SDD-backed work completes verification.
- Capture lessons learned only when they can improve future governance.
- Avoid broad reporting formats until Paciente retrospective calibration and the ProntuarioRepository forward SDD pilot have been reviewed.
