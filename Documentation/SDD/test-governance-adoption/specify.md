# Specify — Test Governance Adoption

> Feature SDD: `Documentation/SDD/test-governance-adoption/`
> Generated SDD artifacts are written in English.
> **Phase:** Specify (complete)
> **Lifecycle position:** Research → **Specify** → Design → Tasks → SDD Pre-Execution Review → Execute → Verify → Documentation Follow-Up → Reporting → Teacher Guide
> **Input from:** [Research](research.md)

---

## Context

The Doc Organo AI Harness currently embeds a Testing Governance Model within `sdd-operational.md` (§ Testing Governance Model), but lacks a standalone, authoritative Test Governance document. A test-strategy review prompt (`review-prompts/test-strategy.md`) serves as a review sensor, but no dedicated test governance rule or standalone governance document exists to define the architectural testing framework.

Phase 1 — Evidence Collection produced Engineering Review Reports (v0.1 through v0.8.3) across three completed features (Paciente, Atendimento Minimal, Prontuário). These reports validated architectural testing patterns, identified recurring workflow gaps, and matured an Architecture-Oriented Testing Model based on two dimensions: Architectural Layer and Architectural Capability.

The Research phase consolidated all validated evidence into 14 Architectural Decisions (AD-TG-001 through AD-TG-014) and an Architectural Stability Assessment classifying every major architectural concept. The Research confirmed that the testing architecture is mature enough for formal governance codification.

This feature evolves the Harness itself — creating a new governance document, refining existing governance documents, updating review prompts, refining skills, updating templates, and integrating architectural completeness validation into the SDD lifecycle. It does not introduce new business functionality, API endpoints, entities, or database changes.

- PM item: Iniciativa de Governança — Test Governance Adoption (WS03/WS09)
- Current operational source: `Documentation/State.md`
- Related Harness documents: `sdd-operational.md`, `verification-governance.md`, `harness-architecture.md`, `review-prompts/test-strategy.md`, `documentation-index.md`, `AGENTS.md`
- Related templates: `tasks.md`, `design.md`, `verification.md`

---

## Problem Statement

Three architectural problems result from the absence of a standalone Test Governance document:

1. **Fragmented authority.** Testing principles, ownership model, classification taxonomy, scenario vocabulary, and design guidelines are scattered across Engineering Review Reports, the embedded Testing Governance Model in `sdd-operational.md`, and a review prompt. No single document owns the architectural definition of testing for this project. Agents must synthesize testing expectations from multiple sources rather than consulting one authoritative reference.

2. **Recurring workflow gap.** Controller Tests were omitted during initial Execute in two independent features (Atendimento and Prontuário). Each required explicit follow-up execution. The root cause is a workflow calibration issue: the SDD lifecycle does not require architectural completeness validation before Execute declares completion. This gap has been confirmed as a recurring pattern (Finding 20), not an isolated implementation failure.

3. **Governance gaps in test derivation and capability responsibilities.** Repository tests in Prontuário exercise explicit architectural decisions (version immutability, replace-vs-merge semantics) but were created organically rather than being explicitly derived from SDD architectural decisions during Execute (Finding 24). SQL Integration Tests blend two distinct responsibilities — business workflow validation and SQL Server-specific behavior validation — without clear governance distinction (Finding 26).

The Research confirmed that architectural testing patterns are validated and stable. The problems are governance fragmentation and workflow calibration, not architectural immaturity.

---

## Goals

- [ ] `REQ-001` — A standalone Test Governance document (`test-governance.md`) shall exist as the authoritative reference for architectural testing in the Doc Organo project
- [ ] `REQ-002` — All 14 Architectural Decisions (AD-TG-001 through AD-TG-014) shall be codified into formal Harness governance
- [ ] `REQ-003` — Test Governance shall define the Architecture-Oriented Testing Model (Layer × Capability), ownership model, scenario vocabulary, test design guidelines, progressive evolution triggers, verification lenses, and architectural review questions
- [ ] `REQ-004` — Test Governance shall establish clear boundaries distinguishing its responsibilities from SDD Operational (lifecycle integration), Verification Governance (gate selection and evaluation), and feature-specific SDD artifacts
- [ ] `REQ-005` — `sdd-operational.md` shall reference Test Governance as the authoritative source for architectural testing principles, retaining only lifecycle integration content (when tests are expected, how Execute produces them, how Verify evaluates them)
- [ ] `REQ-006` — The SDD Execute phase shall include an architectural completeness checkpoint: all expected Architectural Capabilities identified during Design must have corresponding test suites generated and passing, or their absence justified with residual risk
- [ ] `REQ-007` — The SDD Design phase shall require explicit identification of affected Architectural Capabilities and mapping of architectural decisions to expected test scenarios
- [ ] `REQ-008` — The Verifier skill and Verification Governance shall evaluate test suites against architectural ownership, boundary consistency, scenario coverage, and architectural completeness — not merely test presence and naming
- [ ] `REQ-009` — Existing review prompts (`test-strategy.md`) shall be refined to align with the Architectural Capability model and reference Test Governance as the evaluation criteria source
- [ ] `REQ-010` — SDD templates (`tasks.md`, `design.md`, `verification.md`) shall be refined to include architectural completeness expectations, expected test suite types, and capability-to-scenario traceability
- [ ] `REQ-011` — The `sql-migration-workflow` skill shall be refined to derive repository test scenarios from SDD architectural decisions during Execute
- [ ] `REQ-012` — `documentation-index.md`, `AGENTS.md`, and `harness-architecture.md` shall be updated to reference the new Test Governance document
- [ ] `REQ-013` — `Documentation/State.md` shall reflect the Test Governance Adoption lifecycle progression
- [ ] `REQ-014` — Future and partially validated Architectural Capabilities (Business Invariants, Use Case Coordination, Verification Scope) shall be documented in Test Governance as planned evolution with explicit validation triggers, not mandated for current implementation
- [ ] `REQ-015` — All Harness governance documents affected by Test Governance Adoption shall remain internally consistent after changes, with no contradictory guidance between Test Governance, SDD Operational, and Verification Governance

---

## Non Goals

This feature does **not** attempt to:

- **Create new Rules.** All candidate rules (test strategy enforcement, repository test coverage, naming convention) were evaluated and rejected. Architectural test governance is better expressed through governance documents, templates, and the verifier skill than through concise persistent guardrails (Research §9.2).
- **Create new Skills.** All candidate skills (architectural test review, test builder/scenario builder, test suite scaffolding) were evaluated and rejected or postponed. Refinement of existing skills (verifier, sql-migration-workflow) is sufficient. Insufficient recurring evidence exists for dedicated new skills (Research §10.2).
- **Mandate Domain Tests or Application Tests.** Business Invariants (Domain Tests) and Use Case Coordination (Application Tests) are Architected as Future Evolution capabilities. They shall be documented for awareness with explicit validation triggers but not mandated for current implementation (Research §12.2, §17.2).
- **Introduce Builders, Seeds, or Scenario Builders as mandatory infrastructure.** These are Progressive Evolution concepts that shall remain governed by complexity triggers, not prescribed structures (AD-TG-007).
- **Change Reporting or Teacher Guide workflows.** Existing Feature Reports already capture test coverage. The Testing Walkthrough section in Teacher Guides already aligns with the Architectural Capability model. No workflow changes are needed (Research §8.4, §8.5).
- **Modify security, PHI, backend architecture, Blazor frontend, or EF migration rules.** Test Governance is a governance evolution that does not affect product guardrails.
- **Change the SDD methodology itself.** The SDD lifecycle, adaptive sizing model, phase ownership, and Definition of Done remain tool-agnostic and unchanged in principle.
- **Implement changes to product code, database schema, API contracts, or test code.** This feature is governance-only.
- **Resolve open Design Questions (DQ-006, DQ-007, DQ-008).** These remain open for future evaluation when new evidence emerges.
- **Change CI/CD configuration or introduce test automation infrastructure.**

---

## Functional Requirements

| ID | Requirement | Traceability |
|----|-------------|--------------|
| `REQ-001` | A standalone `test-governance.md` document shall be created at `Documentation/AI-Harness/Harness-Design/test-governance.md` as the authoritative reference for architectural testing in the Doc Organo project. It shall codify all 14 Architectural Decisions (AD-TG-001 through AD-TG-014). | Research §5.1, §14.1 #1 |
| `REQ-002` | Test Governance shall define the Architecture-Oriented Testing Model using two dimensions: Architectural Layer (Presentation, Application, Domain, Infrastructure) and Architectural Capability (API Contract, Use Case Coordination, Business Invariants, Persistence Intent, Physical Persistence). | AD-TG-001, AD-TG-003 |
| `REQ-003` | Test Governance shall define the Test Suite Ownership Model requiring every test suite to explicitly declare its Purpose, Protected Capability, Observable Behavior, Protected Boundary, and Out of Scope. Ambiguous ownership shall be treated as an architectural smell. | AD-TG-002 |
| `REQ-004` | Test Governance shall codify the six Architectural Principles (AP-001 Architecture before Coverage, AP-002 Explicit Ownership, AP-003 Boundary Protection, AP-004 Behavior Before Implementation, AP-005 Progressive Governance, AP-006 Executable Documentation). | AD-TG-006 |
| `REQ-005` | Test Governance shall document the distinct architectural ownership of Repository Tests (protect Persistence Intent) and SQL Integration Tests (protect Physical Persistence), including the two complementary responsibilities of SQL Integration Tests. | AD-TG-004, AD-TG-013 |
| `REQ-006` | Test Governance shall define the standard Scenario Classification vocabulary: Happy Path, Boundary Cases, Failure Cases, Business Rule Cases, and Planned Behavior. | AD-TG-008 |
| `REQ-007` | Test Governance shall document Test Design Guidelines including AAA structure, behavior-oriented naming convention, collaborator interaction verification, real AutoMapper Profile usage in Controller Tests, and Theory vs Fact preference. | AD-TG-012 |
| `REQ-008` | Test Governance shall define Progressive Evolution triggers — the conditions under which test infrastructure (split files, Builders, Seeds, Scenario Builders, shared fixtures) should evolve. Anticipated complexity shall be avoided. | AD-TG-007 |
| `REQ-009` | Test Governance shall document Verification Lenses (State Verification, Behavior Verification) as review heuristics that complement Architectural Capabilities, not as additional test categories. | AD-TG-009 |
| `REQ-010` | Test Governance shall include the standard Architectural Review Questions checklist for evaluating new test suites. | Research §2.11 |
| `REQ-011` | Test Governance shall document the Anti-Redundancy Principle: the same property may appear across multiple test suites when each suite protects a different Architectural Capability. The governing question is "Which architectural capability is being protected?" — not "Which property is being asserted?" | AD-TG-014 |
| `REQ-012` | Test Governance shall document Future Evolution Capabilities (Business Invariants, Use Case Coordination, Verification Scope) with their current evidence status, validation triggers, and explicit statement that they are not mandated for current implementation. | Research §12.2, §17.2 |
| `REQ-013` | Test Governance shall explicitly define its scope boundaries — what it owns vs what belongs to SDD Operational, Verification Governance, feature SDD artifacts, and CI/CD configuration. | Research §5.2, §5.3 |
| `REQ-014` | Test Governance shall document Capability Responsibilities (Command Semantics, Query Semantics) as refinements within the Persistence Intent Architectural Capability. | Research §12.4 |
| `REQ-015` | `sdd-operational.md` § Testing Governance Model shall be refined to reference `test-governance.md` as the authoritative source for architectural testing principles, retaining only lifecycle integration content. | Research §6.2 #1 |
| `REQ-016` | `sdd-operational.md` Execute exit criteria shall require architectural completeness validation: expected Architectural Capabilities identified during Design have corresponding test suites generated and passing, or their absence justified with residual risk. | AD-TG-010; Research §6.2 #2 |
| `REQ-017` | `sdd-operational.md` Design exit criteria shall require identification of affected Architectural Capabilities and which test suites are expected during Execute. | AD-TG-010; Research §6.2 #4 |
| `REQ-018` | `sdd-operational.md` Execute phase Context Acquisition Governance shall include loading `test-governance.md` when the current task involves test creation or modification. | Research §6.2 #3 |
| `REQ-019` | `sdd-operational.md` Design phase shall require mapping architectural decisions to expected test scenarios for repository tests. | AD-TG-011; Research §6.2 #4 |
| `REQ-020` | `verification-governance.md` test strategy review gate description shall be refined to include architectural ownership validation, boundary consistency checks, scenario coverage evaluation, and architectural completeness assessment. | Research §6.1 |
| `REQ-021` | The Verifier skill shall evaluate test suites against architectural criteria from `test-governance.md`: architectural ownership of each suite, boundary consistency (no overlapping responsibilities), progressive evolution (complexity proportional to maturity), architectural completeness (expected suites present or justified absent), behavior-oriented naming, and scenario coverage. | AD-TG-010; Research §8.2, §10.1 |
| `REQ-022` | `review-prompts/test-strategy.md` shall be refined to add architectural ownership validation, boundary consistency check, progressive evolution check, architectural completeness check, and alignment with the standard scenario vocabulary. | Research §7.1 |
| `REQ-023` | The `sql-migration-workflow` skill shall be refined to derive repository test scenarios from SDD architectural decisions during Execute, referencing `test-governance.md` for Repository Test ownership and responsibilities. | AD-TG-011; Research §10.1 |
| `REQ-024` | The `tasks.md` template shall be refined to include architectural completeness expectations: list expected test suites with their Architectural Capability, each TASK introducing an architectural change shall include expected test suite type, and Verification Expectations shall include expected Architectural Capabilities and their test evidence. | Research §11.3 |
| `REQ-025` | The `design.md` template Testing Approach section shall be refined to add explicit mapping from architectural decisions to expected test scenarios and expected Architectural Capabilities with their corresponding test suites. | AD-TG-011; Research §11.2 |
| `REQ-026` | The `verification.md` template Test Coverage section shall be refined to add architectural completeness evaluation: expected suites per Architectural Capability, suites present/missing/justified absent, and architectural ownership validation per suite. | Research §11.1 |
| `REQ-027` | `documentation-index.md` shall be updated to add `test-governance.md` to the Harness Design index. | Research §7.2 |
| `REQ-028` | `AGENTS.md` shall be updated to add `test-governance.md` to harness references. | Research §7.4 |
| `REQ-029` | `harness-architecture.md` shall be updated to add `test-governance.md` to the component responsibilities table. | Research §6.3 |
| `REQ-030` | `Documentation/State.md` shall reflect the Test Governance Adoption lifecycle progression. | Research §14.1 #15 |
| `REQ-031` | All refined documents shall remain internally consistent — no contradictory guidance between Test Governance, SDD Operational, Verification Governance, templates, review prompts, and skills. | Research §6, §7, §10, §11 |

---

## Non-Functional Requirements

| ID | Requirement | Rationale |
|----|-------------|-----------|
| `NFR-001` | **Governance consistency.** After Test Governance Adoption, no two Harness governance documents shall contain contradictory testing guidance. The authority hierarchy shall clearly establish that Test Governance is authoritative for architectural testing, while SDD Operational governs lifecycle integration and Verification Governance governs gate evaluation. | Research §5.3; AD-TG-001 through AD-TG-014 |
| `NFR-002` | **Minimum necessary change.** Changes shall be limited to documents, templates, skills, and review prompts identified as requiring refinement by the Research. Documents not identified for refinement (reporting-strategy.md, knowledge-strategy.md, sdd-pilot-report-governance.md, teacher-guide template, security-phi rule, token-economy rule, update-doc rule, backend-architecture rule, ef-migrations rule, blazor-front rule) shall not be modified. | Research §6.4, §6.5, §6.6, §8.4, §8.5, §9.1, §11.4 |
| `NFR-003` | **Progressive Evolution preservation.** The governance codification shall not mandate test infrastructure complexity (Builders, Seeds, Scenario Builders) that has not yet been justified by project maturity. Evolution triggers shall govern when complexity increases, not prescribed structures. | AD-TG-007 |
| `NFR-004` | **Backward compatibility.** Existing test suites (Paciente, Atendimento Minimal, Prontuário) shall remain valid and interpretable after governance adoption. No existing test code shall require modification to comply with the new governance. The governance codifies patterns already validated by these suites. | Research §3 Evidence Maturity Assessment |
| `NFR-005` | **Extensibility.** The Architectural Capability model shall accommodate new capabilities (Component Tests, E2E Tests, API Integration Tests) when validated by implementation evidence, without requiring governance restructuring. Future capabilities inherit the architectural completeness obligation automatically. | AD-TG-005; Research §12.2 |
| `NFR-006` | **Separation of concerns.** Test Governance shall own architectural testing definition. SDD Operational shall own lifecycle integration. Verification Governance shall own gate selection and evaluation. No document shall duplicate responsibilities owned by another. | Research §5.3 |
| `NFR-007` | **Evidence traceability.** Every governance statement in `test-governance.md` that codifies an architectural decision shall be traceable to its source Engineering Review Reports and Architectural Decision. | Research §16 Evidence Traceability Matrix |
| `NFR-008` | **Readability and actionability.** Test Governance shall be written as a governance reference, not as another research report. It shall be concise enough for agents to load as context during test-related tasks without excessive token consumption, while complete enough to serve as the single authoritative reference. | Token economy rule; Research §5.2 |

---

## Architectural Principles

These principles are inherited from the Research and govern all design decisions for this feature. They are codified from the six architectural principles validated across three features.

| ID | Principle | Evidence |
|----|-----------|----------|
| `AP-001` | **Architecture before Coverage.** Coverage is a consequence; architecture is the objective. Test suites exist to protect Architectural Capabilities, not to achieve coverage metrics. | Validated across 3 features (v0.1 → v0.7) |
| `AP-002` | **Explicit Ownership.** Every test suite must own one Architectural Capability. Ambiguous ownership is an architectural smell. | Validated across 3 features (v0.4 → v0.7) |
| `AP-003` | **Boundary Protection.** Tests protect architectural boundaries. Repository Tests protect the boundary between application and persistence intent. SQL Integration Tests protect the boundary between persistence intent and physical infrastructure. Controller Tests protect the boundary between external consumers and the API. | Validated across 3 features (v0.1 → v0.7) |
| `AP-004` | **Behavior Before Implementation.** Tests should validate externally observable behavior whenever possible. Test names should describe protected behaviors, not implementation methods. | Validated in Prontuario (strong evidence); Paciente, Atendimento (early evidence) |
| `AP-005` | **Progressive Governance.** Governance evolves with project complexity. Test infrastructure complexity shall be triggered by maintainability needs, never by anticipation. | Validated across 3 features (v0.2 → v0.7) |
| `AP-006` | **Executable Documentation.** Tests communicate expected system behavior. A well-named, well-organized test suite serves as living documentation of architectural contracts. | Validated in Prontuario (strong evidence — behavior-oriented naming) |

These principles shall be codified in `test-governance.md` and shall govern all automated test design in the project (AD-TG-006).

---

## Architectural Vision

The Doc Organo project shall have a single, authoritative Test Governance document that defines the architectural framework for automated testing.

SDD Operational shall define when and how testing integrates into the SDD lifecycle.

Verification Governance shall define how testing is evaluated during verification.

Templates shall consistently reflect architectural testing expectations.

Review prompts and skills shall reference Test Governance as their evaluation criteria source.

No agent shall need to synthesize testing expectations from Engineering Review Reports or scattered governance fragments. Test Governance shall be the single source of truth for architectural testing.

---

## Assumptions

These assumptions remain valid after the Research and underpin the specification:

1. The Architecture-Oriented Testing Model (Layer × Capability) is stable and validated across three features. It shall become the foundation of Test Governance without structural modification. *(Research §3 — Validated; AD-TG-001, AD-TG-003)*

2. The Ownership Model (Purpose, Protected Capability, Observable Behavior, Protected Boundary, Out of Scope) is sufficient for current project complexity and shall not be expanded unless new evidence justifies it. *(AD-TG-002)*

3. Repository Tests and SQL Integration Tests protect distinct Architectural Capabilities and shall not share responsibilities. The SQL Integration dual-responsibility distinction (business workflow vs SQL Server-specific behavior) is valid and shall be documented, but does not require splitting SQL Integration Tests into separate suites. *(AD-TG-004, AD-TG-013)*

4. Controller Tests using the hybrid pattern (mocked repositories + real AutoMapper Profiles) are the validated standard for protecting the API Contract Architectural Capability. *(Research §2.4 — Validated)*

5. The architectural completeness gap (Finding 20) is a workflow calibration issue, not an architectural immaturity issue. Adding an explicit checkpoint to the SDD Execute phase is sufficient to resolve it. *(AD-TG-010)*

6. Existing test suites across Paciente, Atendimento Minimal, and Prontuário are compliant with the architectural patterns being codified. The governance adoption formalizes what already exists, rather than imposing new requirements on existing code. *(Research §3 Evidence Maturity Assessment)*

7. The documentation-update skill, effective-harness-planning skill, and other skills not explicitly identified for refinement require no changes for Test Governance Adoption. *(Research §10.1)*

8. The project's existing rules (security-phi, token-economy, update-doc, backend-architecture, ef-migrations, blazor-front) are sufficient and require no modification for Test Governance Adoption. *(Research §9.1)*

---

## Constraints

1. **Preserve existing governance philosophy.** Evidence over opinion. Minimum necessary change. Progressive Evolution. Refinement over replacement. Preserve existing governance whenever possible. Avoid overengineering. Keep responsibilities clearly separated. *(Research §14; Harness philosophy)*

2. **No new Rules.** Architectural test governance shall be expressed through governance documents, templates, and the verifier skill — not through concise persistent guardrails. All candidate rules were evaluated and rejected. *(Research §9.2)*

3. **No new Skills.** Refinement of existing skills (verifier, sql-migration-workflow) is sufficient. Insufficient recurring evidence exists for dedicated new skills. *(Research §10.2)*

4. **No Domain Tests or Application Tests mandated.** These are Future Evolution capabilities. They shall be documented with explicit validation triggers but shall not be required for feature completion. *(Research §12.2, §17.2)*

5. **No Builders, Seeds, or Scenario Builders mandated.** Progressive Evolution triggers shall govern when these are introduced. Anticipated complexity shall be avoided. *(AD-TG-007)*

6. **No product code, database schema, API contract, or test code changes.** This feature is governance-only. *(Research §14.2)*

7. **No modification of documents not identified for refinement.** The Research explicitly assessed reporting-strategy.md, knowledge-strategy.md, sdd-pilot-report-governance.md, teacher-guide template, and all existing rules as requiring no changes. *(Research §6.4, §6.5, §6.6, §8.4, §8.5, §9.1, §11.4)*

8. **No CI/CD or test automation infrastructure changes.** Testing infrastructure governance belongs to the project's operational configuration, not to Test Governance. *(Research §5.2)*

9. **No implementation in this phase.** The Specify phase defines what the feature must accomplish. Document structure, precise wording, template refinement details, and skill refinement mechanics belong to the Design and Execute phases. *(SDD phase boundaries)*

10. **No ADR creation in this phase.** Test Governance Adoption does not require ADR evaluation because it codifies architectural decisions already validated by implementation evidence. If the Design phase identifies durable architecture decisions requiring formal ADR governance, it shall initiate ADR evaluation at that point. *(Research §4 — ADs are architectural decisions validated by evidence, not ADR candidates)*

---

## Design Decision Areas

The following questions remain intentionally unresolved. They are inputs to the Design phase, not answered here.

| ID | Decision area | Context | Research reference |
|----|---------------|---------|-------------------|
| `DDA-001` | **Document structure and depth.** What is the optimal structure for `test-governance.md`? Should it follow the pattern of existing Harness Design documents, or should it be organized around Architectural Capabilities? What level of detail balances authority with token-efficiency? | Research §5.2 |
| `DDA-002` | **Template refinement granularity.** How extensive should template refinements be? Should `tasks.md` add a dedicated "Architectural Completeness" section or fold expectations into the existing Verification Expectations table? Should `design.md` add a new "Architectural Capabilities" section or extend the existing Testing Approach section? | Research §11; OQ-004 |
| `DDA-003` | **Verifier skill refinement approach.** Should the verifier skill's test strategy review gate reference `test-governance.md` evaluation criteria inline, defer to the refined review prompt, or both? How much architectural evaluation should be embedded in the skill vs delegated to the review prompt? | Research §10.1 |
| `DDA-004` | **sdd-operational.md restructuring.** Should the existing Testing Governance Model section be completely replaced with a reference to `test-governance.md`, or should a condensed summary remain for agents who do not load the full Test Governance document? | Research §6.2 #1 |
| `DDA-005` | **Future Capability documentation format.** How should Future Evolution capabilities (Business Invariants, Use Case Coordination, Verification Scope) be presented in `test-governance.md`? As aspirational targets? As formal capabilities awaiting validation? With explicit acceptance criteria for promotion? | Research §12.2; OQ-003 |
| `DDA-006` | **Architectural completeness checkpoint mechanism.** AD-TG-010 requires an architectural completeness checkpoint. Should this be an Execute exit criterion, a verifier gate, or both? The Research left this as OQ-001. The Design phase shall determine the mechanism. | AD-TG-010; OQ-001 |
| `DDA-007` | **Test governance document positioning.** Where in the Harness Design document hierarchy should `test-governance.md` be positioned relative to `verification-governance.md` and `sdd-operational.md`? The Research identified this relationship conceptually but did not prescribe the document hierarchy. | Research §5.3; OQ-002 |
| `DDA-008` | **Context acquisition governance.** The Research recommends loading `test-governance.md` when the current task involves test creation or modification. Should this be a recommendation or a requirement? Where should this be documented — in `sdd-operational.md` Context Acquisition Governance or in `test-governance.md` itself? | Research §6.2 #3 |
| `DDA-009` | **review-prompts/test-strategy.md restructuring.** Should the refined review prompt be reorganized around Architectural Capabilities, or should it retain its current structure with added architectural evaluation items? Should it reference `test-governance.md` as the evaluation criteria source or embed the criteria? | Research §7.1 |

---

## Accepted Research Conclusions

The following conclusions have been validated by the Research with documented evidence. The Design phase shall treat them as established facts.

### Architectural Decisions (Stable — Codified into Governance)

Every decision below is classified as Stable by the Architectural Stability Assessment (§17.1) and shall be codified into `test-governance.md`.

| Decision | Summary | Validated In |
|----------|---------|--------------|
| AD-TG-001 | Architectural Capability is the primary test classification — not technology, framework, or implementation | 3 features |
| AD-TG-002 | Every test suite must have explicit architectural ownership (Purpose, Protected Capability, Protected Boundary, Out of Scope) | 3 features |
| AD-TG-003 | Test suites use two-dimensional classification: Layer × Capability | 3 features |
| AD-TG-004 | Repository Tests protect Persistence Intent; SQL Integration Tests protect Physical Persistence — distinct capabilities, distinct suites | 3 features |
| AD-TG-005 | Every Architectural Capability introduced or modified requires a corresponding test suite, or explicit justification recorded as residual risk | 3 features (generalized from Finding 20) |
| AD-TG-006 | Six Architectural Principles (AP-001 through AP-006) govern all automated test design | 3 features |
| AD-TG-007 | Progressive Evolution governs test infrastructure complexity — triggers, not prescribed structures | 3 features |
| AD-TG-008 | Standard scenario vocabulary: Happy Path, Boundary Cases, Failure Cases, Business Rule Cases, Planned Behavior | 3 features |
| AD-TG-009 | Verification Lenses (State, Behavior) are review heuristics, not additional Architectural Capabilities | 3 features |
| AD-TG-010 | Architectural completeness must be validated before Execute completion — a three-phase separation: Design identifies, Execute implements, Verifier validates | 2 features (Finding 20) |
| AD-TG-011 | Execute shall derive repository test scenarios from SDD architectural decisions, not leave them to developer initiative | 1 feature (Finding 24) |
| AD-TG-012 | Behavior-oriented naming is the standard — test names describe protected system behaviors, not implementation methods | 1 feature (strong evidence) |
| AD-TG-013 | SQL Integration Tests protect two complementary concerns: business workflow validation and SQL Server-specific behavior validation | 1 feature (Finding 26) |
| AD-TG-014 | Anti-redundancy is governed by Architectural Capability, not property — same property may appear in multiple suites when each protects a different capability | 3 features |

### Key Findings (Validated — Workflow and Governance Implications)

| Finding | Summary | Implication |
|---------|---------|-------------|
| Finding 20 | Controller Tests omitted during initial Execute in 2 independent features — workflow calibration issue, not isolated failure | AD-TG-010 — architectural completeness checkpoint required |
| Finding 23 | Behavior-oriented naming in Prontuario repository tests (e.g., `CriarNovaVersao_IncrementsVersaoAndKeepsSourceIntact`) improves maintainability and transforms suites into executable documentation | AD-TG-012 — codify behavior-oriented naming |
| Finding 24 | Repository tests in Prontuario exercise architectural decisions but were created organically, not derived from SDD decisions — governance gap | AD-TG-011 — require derivation from SDD architectural decisions |
| Finding 25 | Scenario-Based Workflow validation emerged as a strong pattern in Prontuario SQL Integration Tests | Partial evidence — documented as promising pattern, not mandated |
| Finding 26 | SQL Integration Tests have two complementary responsibilities currently blended in governance | AD-TG-013 — document both responsibilities |
| Finding 27 | Concurrency scenarios not validated in SQL Integration suite when workflow correctness depends on sequential resource allocation | Documented as future guideline — insufficient evidence to mandate |
| Finding 28 | Scenario Builders identified as future candidate for repeated SQL integration setup patterns | Deferred — single-feature observation |

### Future Evolution Capabilities (Documented — Not Mandated)

These capabilities are expected to become part of the testing architecture as the domain model matures. They shall be documented in `test-governance.md` as planned evolution with explicit validation triggers.

| Capability | Current Evidence | Validation Trigger |
|------------|------------------|--------------------|
| Business Invariants (Domain Tests) | Prontuario repository tests exercise invariant-adjacent behaviors; no dedicated Domain Test suite exists | First feature with explicit Domain Tests separated from Repository Tests |
| Use Case Coordination (Application Tests) | Working hypothesis only; no Application Services exist in current architecture | First feature with Application Services orchestrating multiple repositories |
| Verification Scope (Component vs Architecture vs System) | Promising concept from Engineering Reviews; no implementation validation | At least 2 features with distinct test scope patterns |

### Deferred And Rejected Concepts

| Concept | Status | Reason |
|---------|--------|--------|
| Mapping Tests as first-class Architectural Capability | Deferred (DQ-006) | No feature has dedicated mapping tests |
| AutoMapper AssertConfigurationIsValid as mandatory | Deferred (DQ-007) | No evidence of recurring mapping failures |
| Scenario Builders for SQL integration | Deferred | Single-feature observation (Prontuário) |
| Concurrency scenarios as mandatory guideline | Deferred | Single-feature observation (Prontuário versioning) |
| Builders and Seeds as standard infrastructure | Deferred | No feature has yet introduced Builders |
| New Rules (test enforcement, coverage, naming) | Rejected | Workflow concerns, not persistent guardrails |
| New Skills (architectural review, builder, scaffolding) | Rejected/Postponed | Insufficient recurring evidence |

---

## Acceptance Criteria

Acceptance criteria validate governance outcomes, not implementation mechanics.

- [ ] `AC-001` — `test-governance.md` exists at `Documentation/AI-Harness/Harness-Design/test-governance.md` and codifies all 14 Architectural Decisions (AD-TG-001 through AD-TG-014); verified by content review against Research §4
- [ ] `AC-002` — `test-governance.md` defines the Architecture-Oriented Testing Model, Ownership Model, six Architectural Principles, scenario vocabulary, test design guidelines, progressive evolution triggers, verification lenses, architectural review questions, anti-redundancy principle, and capability responsibilities; verified by content completeness review
- [ ] `AC-003` — `test-governance.md` clearly distinguishes its responsibilities from SDD Operational (lifecycle integration), Verification Governance (gate evaluation), and feature SDD artifacts; verified by boundary consistency review
- [ ] `AC-004` — `test-governance.md` documents Future Evolution Capabilities with explicit validation triggers and clear statement that they are not mandated for current implementation; verified by content review
- [ ] `AC-005` — `sdd-operational.md` references `test-governance.md` as the authoritative source for architectural testing principles; verified by cross-document reference check
- [ ] `AC-006` — `sdd-operational.md` Execute exit criteria include architectural completeness validation; verified by content review
- [ ] `AC-007` — `sdd-operational.md` Design exit criteria include identification of affected Architectural Capabilities and expected test suites; verified by content review
- [ ] `AC-008` — `verification-governance.md` test strategy review gate description references architectural ownership, boundary consistency, scenario coverage, and architectural completeness; verified by content review
- [ ] `AC-009` — Verifier skill evaluates test suites against architectural criteria from `test-governance.md` (ownership, boundary consistency, progressive evolution, completeness, naming, scenario coverage); verified by skill content review
- [ ] `AC-010` — `review-prompts/test-strategy.md` includes architectural ownership validation, boundary consistency check, progressive evolution check, architectural completeness check, and standard scenario vocabulary; verified by content review
- [ ] `AC-011` — `sql-migration-workflow` skill references `test-governance.md` for Repository Test ownership and derives test scenarios from SDD architectural decisions; verified by skill content review
- [ ] `AC-012` — `tasks.md` template includes architectural completeness expectations (expected test suites, capability mapping, verification expectations); verified by template review
- [ ] `AC-013` — `design.md` template includes mapping from architectural decisions to expected test scenarios and expected Architectural Capabilities with corresponding test suites; verified by template review
- [ ] `AC-014` — `verification.md` template includes architectural completeness evaluation (expected suites per capability, present/missing/justified absent, ownership validation); verified by template review
- [ ] `AC-015` — `documentation-index.md`, `AGENTS.md`, and `harness-architecture.md` reference `test-governance.md`; verified by cross-document reference check
- [ ] `AC-016` — `Documentation/State.md` reflects Test Governance Adoption lifecycle progression; verified by State.md content review
- [ ] `AC-017` — No contradictory testing guidance exists between Test Governance, SDD Operational, and Verification Governance after all refinements; verified by governance consistency review
- [ ] `AC-018` — Documents not identified for refinement (reporting-strategy.md, knowledge-strategy.md, sdd-pilot-report-governance.md, teacher-guide template, all existing rules) remain unmodified; verified by diff review
- [ ] `AC-019` — No new Rules or Skills are created; verified by repository structure review
- [ ] `AC-020` — Existing test suites (Paciente, Atendimento Minimal, Prontuário) remain valid and interpretable; verified by building and running the existing test suite without modifications
- [ ] `AC-021` — `dotnet build` and `dotnet test` pass without regression; verified by executing the project's full test suite

---

## Harness Consumers

Test Governance Adoption affects these Harness consumers. Unlike business features, this feature does not have end-users or user journeys. It has governance consumers — agents, skills, and workflows that interact with the testing governance framework.

| Consumer | Interaction with Test Governance |
|----------|----------------------------------|
| Agent executing SDD Design phase | Consults `test-governance.md` to identify expected Architectural Capabilities and map architectural decisions to expected test scenarios |
| Agent executing SDD Execute phase | Loads `test-governance.md` when creating or modifying tests; derives repository test scenarios from SDD architectural decisions; validates architectural completeness before declaring Execute complete |
| Agent executing SDD Verify phase (Verifier) | Evaluates test suites against architectural criteria from `test-governance.md` — ownership, boundary consistency, progressive evolution, completeness, naming, scenario coverage |
| `sql-migration-workflow` skill | References `test-governance.md` for Repository Test ownership and scenario derivation during Execute |
| `verifier` skill | Applies architectural evaluation criteria from `test-governance.md` during test strategy review gate |
| `documentation-update` skill | Routes test governance changes to `test-governance.md` when testing governance evolves |
| `effective-harness-planning` skill | Reviews `test-governance.md` as part of Harness Design document evaluation |
| Developer reviewing test suites | Consults `test-governance.md` as the single authoritative reference for architectural testing expectations |
| Agent onboarding to the codebase | Loads `test-governance.md` via `AGENTS.md` harness references to understand the project's testing architecture |

---

## Sizing

| Field | Decision |
|-------|----------|
| Size | **Medium** |
| Rationale | The feature creates one new governance document and refines 3 governance documents, 1 review prompt, 2 skills, 3 templates, and 3 reference documents. No product code, database, API, or test code changes. The Architectural Stability Assessment confirms that architectural concepts are stable — no discovery or validation work remains. The Research provides complete content for all governance codification. |
| Migration vertical note | Governance feature — no SQL migration vertical |
| Required phases | Specify / Design / Tasks / SDD Pre-Execution Review / Execute / Verify / Documentation Follow-Up / Reporting / Teacher Guide when warranted |
| Escalation triggers | Design reveals that `test-governance.md` structure is more complex than anticipated; Design reveals that template refinements require Large sizing due to extensive template restructuring; Design identifies a durable architecture decision requiring ADR evaluation; Design reveals conflicting guidance between existing governance documents that requires resolution beyond refinement |

---

## Open Questions

These questions are unresolved and belong to the Design phase or to future evaluation.

### Design Phase Questions

| ID | Question | Context | Research Reference |
|----|----------|---------|-------------------|
| `OQ-001` | Should the architectural completeness checkpoint be an Execute exit criterion, a verifier gate, or both? | AD-TG-010 requires the checkpoint. The mechanism is a Design decision. | Research §8.1 |
| `OQ-002` | Where in the Harness Design document hierarchy should `test-governance.md` be positioned relative to `verification-governance.md` and `sdd-operational.md`? | Test Governance sits between SDD Operational (when/how) and Verification Governance (verification of). The precise hierarchy is a Design decision. | Research §5.3 |
| `OQ-003` | Should Future Capabilities (Business Invariants, Use Case Coordination, Verification Scope) be documented as aspirational targets or as formal Architectural Capabilities awaiting validation? | Partially validated capabilities need clear status without over-committing. | Research §12.2; §17.2 |
| `OQ-004` | Should `verification.md` template refinement include a dedicated "Architectural Completeness" section or fold it into the existing Test Coverage section? | Template refinement decision for Design phase. | Research §11.1 |

### Future Evaluation Questions (from Research — not resolved by this feature)

| ID | Question | Context | Research Reference |
|----|----------|---------|-------------------|
| `DQ-006` | Should Mapping Tests become a first-class Architectural Capability? | No feature has dedicated mapping tests. AutoMapper validation through AssertConfigurationIsValid may be sufficient. | Research §15 |
| `DQ-007` | Should AutoMapper Profile validation (AssertConfigurationIsValid) become a mandatory project convention? | Related to DQ-006. No recurring mapping failures observed. | Research §15 |
| `DQ-008` | Should Verification Scope become an explicit architectural concept? | Component Scope vs Architecture Scope vs System Scope. Promising but unvalidated. | Research §15 |

---

## Domain Language

This feature uses Harness governance terminology. Terms align with `Documentation/AI-Harness/Harness-Design/harness-architecture.md` and `sdd-operational.md`.

- **Test Governance** — The architectural framework for automated testing: principles, ownership model, classification taxonomy, scenario vocabulary, design guidelines, progressive evolution, and review criteria. Owned by `test-governance.md`.
- **Architectural Capability** — A distinct architectural concern that tests protect. Current validated capabilities: API Contract, Persistence Intent, Physical Persistence. Future capabilities: Business Invariants, Use Case Coordination.
- **Architectural Layer** — The system layer where a test suite operates: Presentation, Application, Domain, Infrastructure.
- **Architecture-Oriented Testing Model** — The two-dimensional classification model (Layer × Capability) that organizes all test suites.
- **Test Suite Ownership** — The explicit declaration of Purpose, Protected Capability, Observable Behavior, Protected Boundary, and Out of Scope for every test suite. Ambiguous ownership is an architectural smell.
- **Architectural Completeness** — The condition where every Architectural Capability introduced or modified by a feature has a corresponding test suite, or its absence is explicitly justified with residual risk.
- **Progressive Evolution** — The principle that test infrastructure complexity (split files, Builders, Seeds, Scenario Builders) evolves only when justified by maintainability needs, never by anticipation.
- **Verification Lenses** — Review heuristics (State Verification, Behavior Verification) that help evaluate how completely an Architectural Capability is protected. They are not additional Architectural Capabilities.
- **Scenario Classification** — Standard vocabulary: Happy Path, Boundary Cases, Failure Cases, Business Rule Cases, Planned Behavior.
- **Persistence Intent** — The Architectural Capability protected by Repository Tests. Answers: "How should persistence behave?"
- **Physical Persistence** — The Architectural Capability protected by SQL Integration Tests. Answers: "How must the physical infrastructure behave?"
- **API Contract** — The Architectural Capability protected by Controller Tests. Answers: "How does the system communicate with external consumers?"
- **Future Evolution Capability** — An Architectural Capability with promising but incomplete implementation evidence, documented for awareness with explicit validation triggers, not mandated for current implementation.
- **Architectural Review Questions** — Standard checklist for evaluating new test suites against architectural criteria.
- **Anti-Redundancy Principle** — The same property may appear across multiple test suites when each suite protects a different Architectural Capability. The governing question is "Which architectural capability is being protected?" — not "Which property is being asserted?"

---

## Security And PHI

- Security/PHI review needed: **No** (for this Specify phase)
- Rationale: This feature is governance-only. It does not touch patient identity, clinical data, files, logs, API exposure, auth, or secrets. No PHI is present in governance documents, templates, review prompts, or skills. The `security-phi` rule remains applicable to all Harness artifacts regardless of governance changes — this is a governance invariant, not a new security concern.

---

## Initial Verification Expectations

| Requirement | Expected evidence |
|-------------|-------------------|
| `REQ-001` through `REQ-014` | `test-governance.md` content completeness review against Research §4, §5.2, §12 |
| `REQ-015` through `REQ-019` | `sdd-operational.md` diff review confirming reference to `test-governance.md`, architectural completeness in exit criteria, and Design exit criteria updates |
| `REQ-020` through `REQ-021` | `verification-governance.md` and verifier skill diff review confirming architectural evaluation criteria |
| `REQ-022` | `review-prompts/test-strategy.md` diff review confirming architectural ownership, boundary, evolution, completeness, and vocabulary alignment |
| `REQ-023` | `sql-migration-workflow` skill diff review confirming test derivation from SDD architectural decisions |
| `REQ-024` through `REQ-026` | Template diff reviews confirming architectural completeness expectations, capability-to-scenario mapping, and verification expectations |
| `REQ-027` through `REQ-029` | Cross-document reference check confirming `documentation-index.md`, `AGENTS.md`, and `harness-architecture.md` reference `test-governance.md` |
| `REQ-030` | `Documentation/State.md` content review confirming lifecycle progression |
| `REQ-031` | Governance consistency review — no contradictory guidance across all refined documents |
| `AC-020` | `dotnet build` and `dotnet test` passing without regression |
| `AC-018` | Diff review confirming documents not identified for refinement remain unmodified |
| `AC-019` | Repository structure review confirming no new Rules or Skills created |

Verifier selects final gates. Documentation review sensor recommended for governance consistency. Test execution gate required for regression validation (`dotnet build && dotnet test`). No SQL, API, or clinical gates apply.

---

## ADR Evaluation

| Candidate | Trigger | Status |
|-----------|---------|--------|
| None identified for this feature | Test Governance Adoption codifies architectural decisions already validated by implementation evidence (AD-TG-001 through AD-TG-014). These are Architectural Decisions within the SDD, not Architecture Decision Records requiring formal ADR governance. The feature does not change boundaries, persistence, schema lifecycle, security/auth, public API contracts, or cross-context ownership. | **No ADR required.** If the Design phase identifies a durable architecture decision meeting ADR criteria, it shall initiate ADR evaluation at that point. |

---

## Out of Scope

The following items are explicitly excluded from this feature. They belong to future phases or separate features.

### Implementation details (Design phase)
- Document structure, section organization, and precise wording of `test-governance.md`
- Template refinement granularity and section organization
- Skill refinement mechanics and precise content changes
- Review prompt reorganization approach
- `sdd-operational.md` restructuring approach (complete replacement vs condensed summary)

### Future evolution (separate features or evidence accumulation)
- Domain Tests (Business Invariants) adoption as active, mandated capability
- Application Tests (Use Case Coordination) adoption as active, mandated capability
- Mapping Tests as first-class Architectural Capability (DQ-006 resolution)
- AutoMapper AssertConfigurationIsValid as mandatory convention (DQ-007 resolution)
- Verification Scope as explicit architectural concept (DQ-008 resolution)
- Builders and Seeds as standard test infrastructure
- Scenario Builders for SQL integration
- Concurrency scenarios as mandatory integration test guideline

### Rejected (insufficient evidence or wrong mechanism)
- New Rules for test strategy enforcement, repository coverage, or naming conventions
- New Skills for architectural test review, builder/scenario construction, or test scaffolding
- Reporting workflow changes
- Teacher Guide template changes

### Unaffected (explicitly excluded from modification)
- Product code (`DocAPI/`, `DocFront.Web/`)
- Database schema and migrations
- API contracts
- Test code (existing test suites)
- CI/CD configuration
- `reporting-strategy.md`
- `knowledge-strategy.md`
- `sdd-pilot-report-governance.md`
- Teacher Guide template
- All existing Rules (security-phi, token-economy, update-doc, backend-architecture, ef-migrations, blazor-front)
- All existing Skills except verifier and sql-migration-workflow
- `rules-strategy.md` and `skills-strategy.md` (not identified for refinement)

---

## References

- `Documentation/State.md`
- `AGENTS.md`
- `Documentation/SDD/test-governance-adoption/research.md`
- `Documentation/AI-Harness/Harness-Design/sdd-operational.md`
- `Documentation/AI-Harness/Harness-Design/verification-governance.md`
- `Documentation/AI-Harness/Harness-Design/harness-architecture.md`
- `Documentation/AI-Harness/review-prompts/test-strategy.md`
- `Documentation/AI-Harness/documentation-index.md`
- `Documentation/AI-Harness/template/sdd/tasks.md`
- `Documentation/AI-Harness/template/sdd/design.md`
- `Documentation/AI-Harness/template/sdd/verification.md`