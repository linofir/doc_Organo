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
- [ ] For SQL migration verticals, optional debt is explicit: mapping tests (`*MappingTests`), soft-deleted parent FK negative tests, controller HTTP coverage beyond Verify smoke.
- [ ] Residual optional gaps are listed in SDD design **Optional Test Debt** when accepted.

Report: Critical / Suggestion / OK per finding.