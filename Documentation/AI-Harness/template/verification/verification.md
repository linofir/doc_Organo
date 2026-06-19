# Verification - [Feature Name]

> Copy this template to `Documentation/SDD/<feature-slug>/verification.md`.
> Generated verification artifacts should be written in English.
> Use `.cursor/skills/verifier/SKILL.md` as the operational workflow.

## Change Classification

- Feature or scope:
- Active SDD:
- Labels:
- Evidence reviewed:

## Execute Handoff Notes

| Input | Status | Notes |
|-------|--------|-------|
| Implementation diff | Received / Missing | ... |
| Build / test output | Received / Missing | ... |
| SQL integration | Received / Missing / Re-confirmed in Verify | ... |
| Runtime validation (TASK-008) | Received / **Verify will record** | Execute records intent; Verify owns durable table below |
| Execute deviations | None / Documented | ... |

## Gate Results

| Gate | Status | Evidence / Reason |
|------|--------|-------------------|
| Build | Passed / Failed / Skipped / Blocked | ... |
| Automated tests | Passed / Failed / Skipped / Blocked | ... |
| SQL / Persistence | Passed / Failed / Skipped / Blocked | ... |

For environment-dependent gates (Docker, credentials), use **Skipped (environment unavailable)** when Verify cannot reproduce Execute evidence. See `Documentation/AI-Harness/Harness-Design/verification-governance.md` Environment-Dependent Evidence policy.

| API | Passed / Failed / Skipped / Blocked | ... |
| UI | Passed / Failed / Skipped / Blocked | ... |
| Security / PHI | Passed / Failed / Skipped / Blocked | ... |
| Domain review | Passed / Failed / Skipped / Blocked | ... |
| Documentation review | Passed / Failed / Skipped / Blocked | ... |
| Test strategy review | Passed / Failed / Skipped / Blocked | ... |
| ADR evaluation | Needed / Not needed / Blocked | ... |
| Legacy characterization | Passed / Failed / Skipped / Blocked | ... |

## Runtime Validation (Verify-owned)

Populate when API smoke, Swagger checklist, or manual runtime gates apply. This section is the **durable** TASK-008 evidence location.

**Environment:** ...

| Scenario | Endpoint | Expected | Actual | Pass |
|----------|----------|----------|--------|------|
| ... | ... | ... | ... | Yes / No |

## Review Sensors

| Sensor | Applied / Skipped | Findings |
|--------|-------------------|----------|
| `domain-review.md` | ... | Critical / Suggestion / OK |
| `security-phi-review.md` | ... | Critical / Suggestion / OK |
| `check-docs.md` | ... | Critical / Suggestion / OK |
| `test-strategy.md` | ... | Critical / Suggestion / OK |

## Requirement Evidence

| Requirement | Implementation evidence | Test or check evidence | Status |
|-------------|-------------------------|------------------------|--------|
| `REQ-001` | ... | ... | Verified / Partial / Missing |

## Test Coverage

- Existing tests:
- New tests:
- Missing or deferred tests:
- Synthetic data confirmed: Yes / No / Not applicable

## Residual Risk

- Rating: Low / Medium / High
- Justification:
- Accepted by:

## Follow-up Needed

Documentation Update owns final routing.

- State:
- ADR:
- Architecture docs:
- Technical docs:
- Rules:
- Skills:
- SDD:
- Review prompts:
- Templates:
- Reporting:

## Completion Decision

- Decision: Complete / Not complete / Complete with accepted residual risk
- Reason:
- Required next action:
