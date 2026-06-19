# Feature Report - [Feature Name]

> Copy this template after meaningful SDD-backed feature work completes the post-Verify chain.
> Place at `Documentation/SDD/<feature-slug>/reports/feature-report.md`.
> Generated reporting artifacts should be written in English.

## Reporting Timing

**Reporting occurs after Documentation Follow-Up**, not immediately after Verify.

Before writing this report:

1. Verify is complete with a documented completion decision in `verification.md`.
2. Documentation Follow-Up has executed the mandatory checklist — especially `Documentation/State.md`, PM, migration-sql, and runbook when applicable.
3. `Documentation/State.md` and routed project docs reflect the final verified operational truth.

Feature reports summarize synchronized truth. They must not describe pre-follow-up drift as current status.

## Summary

Briefly describe what was completed and why it matters.

## Scope

- Feature:
- Active SDD:
- PM item:
- Included:
- Excluded:

## Requirements Delivered

| Requirement | Outcome | Evidence |
|-------------|---------|----------|
| `REQ-001` | Delivered / Partial / Deferred | ... |

## Implementation Areas

| Area | Summary |
|------|---------|
| Domain | ... |
| Persistence | ... |
| API | ... |
| Frontend | ... |
| Documentation / Harness | ... |

## Verification Summary

Link to `verification.md`. Do not re-select gates or accept residual risk here.

- Completion decision:
- Gates passed:
- Gates skipped or blocked:
- Review sensors applied:
- Residual risk:

## Documentation Follow-Up Summary

What was updated during mandatory Follow-Up (not merely identified):

- State.md:
- PM:
- migration-sql.md:
- runbook.md:
- ADR:
- Architecture docs:
- Technical docs:
- SDD sync:
- Rules / Skills / Review prompts / Templates:

## Lessons Learned

Capture workflow findings that may improve future SDD, verification, reporting, templates, rules, skills, or harness calibration. Route durable harness changes through Documentation Update / Harness Calibration — not applied directly from Reporting.

When findings are substantial enough to drive harness changes, consolidate them into a [Governance Improvement Plan](../governance/governance-improvement-plan.md) after Reporting (see `Documentation/AI-Harness/CONTRIBUTING-AI.md`).

- ...

## Teacher Guide

- Generated: Yes / No / Skipped with reason
- Path: `teacher-guide.md` when applicable

## Remaining Work

- ...
