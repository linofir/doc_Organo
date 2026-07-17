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
  - **Expected test suite(s):** Repository Tests | SQL Integration Tests | Controller Tests | None (no Architectural Capability affected)
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

## Runtime Validation (TASK-008 pattern)

When `design.md` Runtime Validation Environment applies:

- [ ] `TASK-008` - Runtime validation intent (Execute)
  - **Requirements:** ...
  - **Execute owns:** Environment confirmed; smoke checklist attempted or explicitly deferred with reason in handoff notes. **Record session notes with Swagger/smoke results for Verify handoff — do not rely on chat history.**
  - **Verify owns:** Durable evidence table in `verification.md` § Runtime Validation (HTTP/Swagger scenarios, pass/fail). **Verify checks that TASK-008 session notes were provided before populating the Runtime Validation table.**
  - **Done when (Execute):** Implementation complete, runtime intent recorded, and session notes captured.
  - **Done when (Verify):** Runtime Validation section populated or gate explicitly skipped with reason.

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
| Architectural completeness | Expected / Not expected | All Architectural Capabilities from design.md have test suites or justified absence |
| ADR evaluation | Expected / Not expected | ... |
| Legacy characterization | Expected / Not expected | ... |

## Review Sensors

List review prompts expected to apply.

- [ ] `Documentation/AI-Harness/review-prompts/domain-review.md`
- [ ] `Documentation/AI-Harness/review-prompts/security-phi-review.md`
- [ ] `Documentation/AI-Harness/review-prompts/check-docs.md`
- [ ] `Documentation/AI-Harness/review-prompts/test-strategy.md`

## Known Risks And Skipped Checks

Missing test suites must be listed here with Architectural Capability and justification.

| Risk or skipped check | Reason | Owner / follow-up |
|-----------------------|--------|-------------------|
| [e.g., Controller Tests for `REQ-005`] | [e.g., Endpoint matches existing pattern; covered by SQL integration workflow validation] | [e.g., Accepted residual risk — re-evaluate when API contract diverges] |

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

## Execution Batching

For Large features, execute in small implementation batches — implement a coherent subset of tasks, validate (build, test, smoke), commit, then continue. See `sdd-operational.md` § Implementation Batching Guidance and § Context Acquisition Governance for Execute entry context requirements.

## Completion Handoff

Before marking tasks complete, provide the Verifier with:

- Implemented task list.
- Requirement traceability status.
- Tests/checks run.
- Tests/checks skipped with reasons.
- Review sensors applied or skipped.
- Residual risks.
- Documentation follow-up candidates.

### Task Checkbox Ownership

| Artifact | Execute | Verify | Documentation Follow-Up |
|----------|---------|--------|-------------------------|
| `TASK-*` implementation | Marks work done in handoff; may check boxes when implementation complete | Validates acceptance against requirements | Does **not** own implementation status |
| `VP-*` | Prepares evidence inputs | Uses for gate selection | — |
| `DF-*` | Identifies candidates only | Identifies mandatory targets | Executes doc sync; may mark `DF-*` complete |
| `tasks.md` checkboxes | Execute may mark `TASK-*` complete when done | Verify confirms acceptance | May mark `VP-*` / `DF-*` when workflow phases complete |

Do not treat Documentation Follow-Up as the default owner for marking `TASK-*` complete unless Execute never updated the file and Verify confirms acceptance.
