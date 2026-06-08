# Review Prompt — Test Strategy

Review this diff for automated test coverage expectations.

Check:

- [ ] New business behavior has automated test coverage.
- [ ] Existing tests still validate the changed area.
- [ ] Critical business rules are not validated only through manual testing.
- [ ] Test names describe business behavior.
- [ ] Tests use synthetic data only.
- [ ] New code did not reduce existing coverage without justification.
- [ ] Missing tests are explicitly documented as technical debt.
- [ ] Integration tests exist when behavior spans multiple layers.

Report: Critical / Suggestion / OK per finding.