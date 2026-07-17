# Review Prompt — Test Strategy

Review this diff for automated test coverage expectations. Evaluation criteria are defined in `Documentation/AI-Harness/Harness-Design/test-governance.md`.

Check:

- [ ] New business behavior has automated test coverage.
- [ ] Existing tests still validate the changed area.
- [ ] Critical business rules are not validated only through manual testing.
- [ ] Test names describe business behavior (behavior-oriented naming per `test-governance.md`).
- [ ] Tests use synthetic data only.
- [ ] New code did not reduce existing coverage without justification.
- [ ] Missing tests are explicitly documented as technical debt.
- [ ] Integration tests exist when behavior spans multiple layers.
- [ ] **Architectural ownership:** Does each test suite explicitly protect an Architectural Capability defined in `test-governance.md`? Is ownership unambiguous?
- [ ] **Boundary consistency:** Are Repository Test and SQL Integration Test responsibilities distinct? Is any suite protecting a capability that belongs to another suite?
- [ ] **Progressive evolution:** Is test infrastructure complexity (split files, Builders, Seeds) proportional to feature maturity? Is there anticipated complexity?
- [ ] **Architectural completeness:** Are all expected Architectural Capabilities from design.md covered by test suites? Are missing suites explicitly justified with residual risk?
- [ ] **Scenario vocabulary:** Do test scenarios use the standard vocabulary (Happy Path, Boundary Cases, Failure Cases, Business Rule Cases, Planned Behavior)?
- [ ] For SQL migration verticals, optional debt is explicit: mapping tests (`*MappingTests`), soft-deleted parent FK negative tests, controller HTTP coverage beyond Verify smoke.
- [ ] Residual optional gaps are listed in SDD design **Optional Test Debt** when accepted.

Report: Critical / Suggestion / OK per finding.
