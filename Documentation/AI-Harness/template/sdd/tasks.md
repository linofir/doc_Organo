# Tasks - [Feature Name]

> Copy this template to `Documentation/SDD/<feature-slug>/tasks.md`.
> Inputs: `specify.md` and `design.md` in the same feature folder.
> Generated SDD artifacts should be written in English.

## Execution Boundary

State what this task plan will execute and what it will not execute.

- Included:
- Excluded:
- Current execution does not begin until this task plan is accepted.

## Task List

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
