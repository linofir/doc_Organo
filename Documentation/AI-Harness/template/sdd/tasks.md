# Tasks - [Feature Name]

> Copy this template to `Documentation/SDD/<feature-slug>/tasks.md`.
> Inputs: `specify.md` and `design.md` in the same feature folder.
> Generated SDD artifacts should be written in English.

## Execution Boundary

State what this task plan will execute and what it will not execute.

- Included:
- Excluded:
- **Explicitly excluded from Execute:** verification artifacts (`verification.md`), reporting (`feature-report.md`, `session-handoff.md`), project documentation updates (State, PM, migration-sql, runbook), Teacher Guide generation.
- Current execution does not begin until SDD Pre-Execution Review exit criteria are satisfied.

## Implementation Tasks (TASK)

- [ ] `TASK-001` - ...
  - **Requirements:** `REQ-001`
  - **Files:** `...`
  - **Depends on:** None / `TASK-...`
  - **Tests or checks:** ...
  - **Done when:** ...
- [ ] `TASK-002` - ...
  - **Requirements:** `REQ-...`
  - **Files:** `...`
  - **Depends on:** `TASK-001`
  - **Tests or checks:** ...
  - **Done when:** ...

## Verify Preparation (VP)

Workflow preparation for Verify — not Execute implementation.

- [ ] `VP-001` - ...
  - **Purpose:** ...
  - **Depends on:** `TASK-...`
  - **Done when:** ...

## Documentation Follow-Up Preparation (DF)

Workflow preparation for Documentation Follow-Up — not Execute implementation.

- [ ] `DF-001` - ...
  - **Purpose:** ...
  - **Depends on:** `TASK-...` / `VP-...`
  - **Done when:** ...

## Dependency Map

| Task | Depends on | Can run in parallel with | Notes |
|------|------------|--------------------------|-------|
| `TASK-001` | None | ... | ... |

## Requirement Traceability

| Requirement | Tasks | Tests or checks | Verification evidence |
|-------------|-------|-----------------|-----------------------|
| `REQ-001` | `TASK-001` | ... | ... |

## Verification Expectations

The Verifier will select final gates. This section lists expected evidence from the implementation plan.

| Gate category | Expected / Not expected | Evidence or rationale |
|---------------|-------------------------|-----------------------|
| Build | Expected / Not expected | ... |
| Automated tests | Expected / Not expected | ... |
| SQL / Persistence | Expected / Not expected | ... |
| API | Expected / Not expected | ... |
| UI | Expected / Not expected | ... |
| Security / PHI | Expected / Not expected | ... |
| Domain review | Expected / Not expected | ... |
| Documentation review | Expected / Not expected | ... |
| Test strategy review | Expected / Not expected | ... |
| ADR evaluation | Expected / Not expected | ... |
| Legacy characterization | Expected / Not expected | ... |

## Review Sensors

List review prompts expected to apply.

- [ ] `Documentation/AI-Harness/review-prompts/domain-review.md`
- [ ] `Documentation/AI-Harness/review-prompts/security-phi-review.md`
- [ ] `Documentation/AI-Harness/review-prompts/check-docs.md`
- [ ] `Documentation/AI-Harness/review-prompts/test-strategy.md`

## Known Risks And Skipped Checks

| Risk or skipped check | Reason | Owner / follow-up |
|-----------------------|--------|-------------------|
| ... | ... | ... |

## Documentation Follow-up Candidates

Documentation Update will decide final routing.

- State:
- ADR:
- Architecture docs:
- Technical docs:
- Rules:
- Skills:
- Review prompts:
- Templates:
- Active SDD:

## Commit Guidance

- Keep commits atomic and focused.
- Do not commit secrets, `.env` files, credentials, real patient data, or clinical data.
- Commit message should reflect the change type and scope.

## Completion Handoff

Before marking tasks complete, provide the Verifier with:

- Implemented task list.
- Requirement traceability status.
- Tests/checks run.
- Tests/checks skipped with reasons.
- Review sensors applied or skipped.
- Residual risks.
- Documentation follow-up candidates.
