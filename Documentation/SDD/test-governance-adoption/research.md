# Research — Test Governance Adoption

**Feature:** Test Governance Adoption (WS03/WS09)
**Phase:** Research
**Status:** Complete
**SDD Folder:** `Documentation/SDD/test-governance-adoption/`

**Created:** 2026-07-14
**Sources:** Engineering Review Reports v0.1 → v0.8.3; Verification Reports (Paciente, Atendimento Minimal, Prontuario); AI Harness governance documents; existing templates, rules, skills, and review prompts.

---

## 1. Research Metadata

### 1.1 Purpose

This research consolidates architectural testing evidence collected during Phase 1 — Evidence Collection. It is not exploratory discovery. The Engineering Review Reports, verification outputs, feature reports, and Teacher Guides produced across three completed features (Paciente, Atendimento Minimal, Prontuário) constitute the complete evidence base.

The purpose is to determine which validated findings should be promoted into official AI Harness governance through the Test Governance Adoption feature.

### 1.2 Scope

This research covers:

- Consolidation of validated architectural testing patterns across all completed features.
- Evidence maturity assessment for each significant finding.
- Architectural decisions derived from validated findings.
- Impact assessment on all AI Harness components.
- Recommended implementation scope for the SDD Design phase.

This research does **not** cover:

- Implementation proposals, design decisions, or task planning (belongs to SDD Specify/Design/Tasks).
- New codebase exploration or feature artifact inspection (evidence already collected).
- Speculative improvements unsupported by implementation evidence.

### 1.3 Primary Sources

| Source | Versions | Type |
|--------|----------|------|
| Engineering Review Reports | v0.1, v0.2, v0.3, v0.4, v0.5, v0.6, v0.7, v0.8.1, v0.8.2, v0.8.3 | Primary evidence |
| Verification Reports | Paciente, Atendimento Minimal, Prontuario | Supporting evidence |
| Feature Reports | Paciente, Atendimento Minimal, Prontuario | Supporting evidence |
| Teacher Guides | Paciente, Prontuario | Supporting evidence |
| Documentation Follow-Up artifacts | Paciente, Atendimento Minimal, Prontuario | Supporting evidence |

### 1.4 Current Harness Context

The AI Harness includes a **Testing Governance Model** embedded in `sdd-operational.md` (§ Testing Governance Model) but does **not** have a standalone Test Governance document. The test-strategy review prompt (`review-prompts/test-strategy.md`) serves as a review sensor. No dedicated test governance rule exists. The verifier skill evaluates test strategy as a review gate.

---

## 2. Evidence Consolidation

All findings below are consolidated from the Engineering Review Reports. Each finding cites the specific report version(s) that validate it.

### 2.1 Architectural Principles

| ID | Principle | Source | Validated In |
|----|-----------|--------|--------------|
| AP-001 | Architecture before Coverage — coverage is a consequence, architecture is the objective | v0.1 §4, v0.2 §4, v0.3 §3, v0.4 §3, v0.5 §3 | Paciente, Atendimento, Prontuario |
| AP-002 | Explicit Ownership — every test suite must own one architectural concern | v0.2 §4, v0.3 §3, v0.4 §7, v0.5 §7, v0.7 | Paciente, Atendimento, Prontuario |
| AP-003 | Boundary Protection — tests protect architectural boundaries | v0.1 §3, v0.2 §4, v0.3 §3, v0.4 §3, v0.5 §3, v0.7 | Paciente, Atendimento, Prontuario |
| AP-004 | Behavior Before Implementation — tests should validate behavior whenever possible | v0.2 §6, v0.3 §3, v0.4 §3, v0.5 §3, v0.7 | Prontuario (strong evidence); Paciente, Atendimento (early evidence) |
| AP-005 | Progressive Governance — governance evolves with project complexity | v0.2 §4, v0.3 §3, v0.4 §11, v0.5 §11, v0.6 §4, v0.7 §4 | All 3 features |
| AP-006 | Executable Documentation — tests communicate expected behavior | v0.1 §4, v0.2 §4, v0.3 §3, v0.4 §3, v0.5 §3, v0.7 | Prontuario (strong evidence — behavior-oriented naming) |

### 2.2 Architecture-Oriented Testing Model

The two-dimensional classification model (Layer × Architectural Capability) evolved across reports v0.2 → v0.7 and was validated as the primary classification mechanism.

**Dimension 1 — Architectural Layer:**

| Layer | Defined in | Validated with test suites |
|-------|------------|---------------------------|
| Presentation | v0.3 §4 | Controller Tests (3 features) |
| Application | v0.3 §4 | Not yet implemented (working hypothesis) |
| Domain | v0.3 §4 | Not yet implemented (expected for aggregates with invariants) |
| Infrastructure | v0.3 §4 | Repository Tests, SQL Integration Tests (3 features) |

**Dimension 2 — Architectural Capability:**

| Capability | Defined in | Architectural Question | Validated |
|------------|------------|----------------------|-----------|
| API Contract | v0.4 §6 | How does the system communicate with external consumers? | 3 features |
| Use Case Coordination | v0.4 §6 (working hypothesis) | How are business use cases coordinated? | Not yet implemented |
| Business Invariants | v0.4 §6 | Which business rules must never be violated? | Prontuario (partial evidence via repository tests exercising invariants) |
| Persistence Intent | v0.4 §6 | How should persistence behave? | 3 features |
| Physical Persistence | v0.4 §6 | How must the physical infrastructure behave? | 3 features |

### 2.3 Ownership Model

The ownership model matured from v0.1 (test categories) through v0.7 (explicit ownership). Each test suite should explicitly define:

- **Purpose** — Why does this suite exist?
- **Protected Capability** — Which Architectural Capability is protected?
- **Observable Behavior** — Which externally observable behavior is protected? (added v0.5)
- **Protected Boundary** — Which architectural boundary does the suite validate?
- **Out of Scope** — Which concerns intentionally belong to other test suites?

Ambiguous ownership is considered an architectural smell (v0.5 §7).

### 2.4 Test Taxonomy And Categories

#### Repository Tests

| Attribute | Value | Source |
|-----------|-------|--------|
| Layer | Infrastructure | v0.4 §8 |
| Protected Capability | Persistence Intent | v0.4 §8, v0.7 §5 |
| Architectural Question | How should persistence behave? | v0.4 §8 |
| Responsibilities | Persistence contracts, update semantics, query semantics, persistence behavior | v0.7 §5 |
| Does NOT validate | Business rules, SQL Server behavior, implementation details | v0.7 §5 |

Evolution observed:
- Paciente/Atendimento: primarily CRUD verification
- Prontuário: shifted to business rule/architectural invariant verification (v0.8.2 Finding 21)

#### SQL Integration Tests

| Attribute | Value | Source |
|-----------|-------|--------|
| Layer | Infrastructure | v0.4 §8 |
| Protected Capability | Physical Persistence | v0.4 §8, v0.7 |
| Architectural Question | How must the physical infrastructure behave? | v0.4 §8 |
| Responsibilities | SQL execution, EF Core mappings, migrations, constraints, transactions, query filters, foreign keys, database compatibility | v0.7 |
| Does NOT validate | Repository implementation decisions | v0.4 §8 |

Two complementary responsibilities identified (v0.8.3 Finding 26):
- Validate complete business workflows against the real database
- Validate SQL Server-specific behaviors when architectural decisions depend on database guarantees

Persistence Lifecycle pattern (Create → Read → Update → Read → Delete → Verify) validated as preferred integration test structure (v0.7 §17).

Scenario-Based Workflow validation emerged as a strong pattern in Prontuário (v0.8.3 Finding 25).

#### Controller Tests

| Attribute | Value | Source |
|-----------|-------|--------|
| Layer | Presentation (Application Interface in v0.6+) | v0.5 §8, v0.6 §11 |
| Protected Capability | API Contract | v0.5 §8, v0.7 §11 |
| Architectural Question | How does the system communicate with external consumers? | v0.5 §8 |
| Responsibilities | HTTP semantics, status codes, DTO contracts, routing, collaborator interaction, mapping integration | v0.7 §11 |

Controller Tests SHOULD use real AutoMapper Profiles while mocking only repositories, infrastructure, and external services (v0.5 §10, v0.7 §11).

Hybrid pattern (mocked repositories + EF Core InMemory for infrastructure-dependent validation) approved as recommended standard (v0.8.1 Finding 23).

Collaborator verification complements HTTP assertions (v0.7 §13).

Planned Behavior (501 Not Implemented) should be protected for intentionally exposed endpoints (v0.5 §9, v0.7 §12).

#### Domain Tests (Future)

| Attribute | Value | Source |
|-----------|-------|--------|
| Layer | Domain | v0.5 §8 |
| Protected Capability | Business Invariants | v0.5 §8 |
| Expected examples | Aggregate invariants, domain transitions, versioning rules, business constraints | v0.5 §8 |

Not yet implemented. Prontuário repository tests exercised invariant-adjacent behaviors (version immutability, replace-vs-merge semantics) suggesting value in eventual dedicated Domain Tests.

#### Application Tests (Future)

| Attribute | Value | Source |
|-----------|-------|--------|
| Layer | Application | v0.5 §8 |
| Protected Capability | Use Case Coordination | v0.5 §8 (working hypothesis) |
| Expected examples | Use case orchestration, transaction coordination, repository coordination, domain interaction, application services | v0.5 §8 |

Not yet implemented. Capability remains a working hypothesis.

### 2.5 Scenario Classification

Standard vocabulary for test scenarios (v0.5 §9, v0.7):

| Category | Definition | Examples |
|----------|------------|----------|
| Happy Path | Expected successful behavior | Successful CRUD, valid payloads |
| Boundary Cases | Operational limits | Empty values, maximum lengths, optional fields, pagination boundaries |
| Failure Cases | Unexpected or invalid scenarios | Entity not found, duplicate identifiers, invalid payloads, concurrency failures |
| Business Rule Cases | Business-specific behavior | Aggregate invariants, versioning, workflow rules, soft delete semantics |
| Planned Behavior | Intentionally exposed before full implementation | HTTP 501, planned endpoints, future contracts |

### 2.6 Test Design Guidelines

Consolidated from v0.5 §10, v0.7:

- **Arrange-Act-Assert (AAA):** Preferred structure for readability and reviewability.
- **Naming Convention:** `Action_Scenario_ExpectedResult` — behavior-oriented names preferred over persistence-oriented names (v0.8.2 Finding 23).
- **Collaborator Interaction Verification:** Controller Tests SHOULD verify collaborator interaction when part of observable behavior. Complements HTTP assertions.
- **Mapping Strategy:** Controller Tests SHOULD use real AutoMapper Profiles. Only infrastructure dependencies should be mocked.
- **Theory vs Fact:** Prefer Theory when protected capability and expected behavior are identical with only input variations.

### 2.7 Progressive Evolution

Validated principle: testing architecture evolves together with domain complexity. Complexity must emerge from necessity, never anticipation (v0.7 §4).

Natural evolution path documented:

```
RepositoryTests
    ↓
Repository + Mapping
    ↓
Repository Commands + Queries
    ↓
Builders
    ↓
Shared Test Infrastructure
    ↓
Complex Fixtures
```

Triggers for evolution:
- Suite size justifies logical organization (Commands/Queries, Create/Update/Delete/Query)
- Repeated construction patterns justify Builders
- Repeated scenario setup justifies Seeds or Scenario Builders
- Cross-suite duplication justifies shared infrastructure

### 2.8 Builders And Seeds

Distinction validated (v0.6 §8, v0.7 §8):

- **Builders:** Construct valid domain objects (e.g., `PacienteBuilder.Build()`)
- **Seeds:** Prepare complete test scenarios (e.g., `SeedPacienteAsync()` — creates, persists, prepares environment)

Builders create objects. Seeds create environments. Both should emerge only when justified by project complexity.

Scenario Builders identified as future candidate for repeated SQL integration setup patterns (v0.8.3 Finding 28).

### 2.9 Aggregate Dependencies

As domain complexity grows, tests naturally require dependent aggregates (v0.7 §9). Example chain documented:

```
Paciente → Atendimento → Prontuario → Workflow
```

Test infrastructure should evolve incrementally to support dependency chains. Builders and Seeds become increasingly valuable as chains expand.

Fixture Chain pattern validated in Prontuário (design.md template, v0.8.3).

### 2.10 Verification Lenses

Two complementary review heuristics (v0.6 §10, v0.7 §10):

| Lens | Protects | Examples |
|------|----------|----------|
| State Verification | "The resulting state is correct" | Persisted values, returned DTOs, stored entities |
| Behavior Verification | "The component behaved according to its responsibility" | Preserved fields, collaborator interaction, update semantics, query semantics |

Verification Lenses are **review heuristics**, not additional Architectural Capabilities. They help reviewers identify missing coverage without introducing new test categories.

### 2.11 Architectural Review Questions

Standard checklist for reviewing new test suites (v0.7 §19):

- Which Architectural Capability is being protected?
- Who owns this capability?
- Which explicit contract is being validated?
- Is another suite already protecting this responsibility?
- Is this testing implementation or contract?
- Are Command Semantics adequately covered?
- Are Query Semantics adequately covered?
- Are Verification Lenses balanced?
- Does the current complexity justify Builders or Seeds?
- Does this suite respect Progressive Evolution?

### 2.12 Workflow Completeness Gap (Finding 20)

**Confirmed recurring finding across 2 independent features.**

Controller Tests were omitted during initial Execute in both Atendimento and Prontuário. Each required an explicit follow-up execution. This is a **workflow calibration issue**, not isolated implementation failure.

Evidence: v0.7 §20, v0.8.1 Finding 20.

Recommendation (from reports): Execute workflow should include explicit architectural completeness validation before considering implementation finished.

### 2.13 Repository Tests — Missing Governance Guideline (Finding 24)

Repository tests in Prontuário exercise explicit architectural decisions (version immutability, replace-vs-merge semantics, version ordering, latest-version selection). However, these tests were created organically after implementation rather than being explicitly derived from SDD architectural decisions during Execute.

The current governance does not require Execute to derive repository test scenarios directly from architectural decisions documented in the SDD. This is a governance gap (v0.8.2 Finding 24).

### 2.14 SQL Integration — Two Complementary Responsibilities

SQL Integration Tests have two distinct responsibilities currently blended in governance (v0.8.3 Finding 26):

1. Validate complete business workflows against the real database.
2. Validate SQL Server-specific behaviors when architectural decisions depend on database guarantees.

Current governance does not clearly distinguish these responsibilities.

### 2.15 Concurrency Scenarios

Prontuário introduces sequential version allocation. While controller logic handles version conflicts, SQL Integration suite does not validate concurrent version creation. Recommended as future guideline when workflow correctness depends on sequential resource allocation (v0.8.3 Finding 27).

### 2.16 Behavior-Oriented Naming

Repository test names in Prontuário describe protected system behaviors rather than implementation methods (v0.8.2 Finding 23). Examples:

- `CriarNovaVersao_IncrementsVersaoAndKeepsSourceIntact`
- `SoftDelete_ExcludesFromReads_ButKeepsInMaxVersao`
- `D05_EvolutionDoesNotMergeCollections`

This improves maintainability and transforms suites into executable documentation.

### 2.17 Anti-Redundancy Principle

A single property may legitimately appear across multiple test suites because each suite protects a different Architectural Capability (v0.7 §2). Example: `EtapaAtual` validated by Domain Tests (Business Invariants), Repository Tests (Persistence Intent), SQL Integration Tests (Physical Persistence), and Controller Tests (API Contract).

This is **not redundancy**. The governing question is: "Which architectural capability is being protected?" — not "Which property is being asserted?"

---

## 3. Evidence Maturity Assessment

Each significant finding is classified by evidentiary maturity before any governance decisions are made.

| Finding | Classification | Evidence Basis | Rationale |
|---------|---------------|----------------|-----------|
| Architectural Capability as primary classification | **Validated** | 3 features; v0.1 → v0.7 progressive refinement | Core model survived all 3 implementations without structural change |
| Ownership Model (Purpose, Protected Capability, Protected Boundary, Out of Scope) | **Validated** | 3 features; v0.4 → v0.7 refinement | Applied consistently across Paciente, Atendimento, Prontuario |
| Architectural Principles (AP-001 through AP-006) | **Validated** | 3 features; v0.1 → v0.7 | Principles drove test design across all features |
| Two-dimensional classification (Layer × Capability) | **Validated** | 3 features; v0.2 → v0.7 | Every test suite mapped to this model |
| Repository Tests protect Persistence Intent | **Validated** | 3 features; v0.4 → v0.7 | Consistent ownership across all features |
| SQL Integration Tests protect Physical Persistence | **Validated** | 3 features; v0.4 → v0.8.3 | Ownership confirmed post-Prontuario |
| Controller Tests protect API Contract | **Validated** | 3 features; v0.5 → v0.8.1 | Ownership confirmed; hybrid pattern validated |
| Scenario Classification (Happy Path, Boundary, Failure, Business Rule, Planned Behavior) | **Validated** | 3 features; v0.5 → v0.7 | Applied across all test suites |
| Progressive Evolution | **Validated** | 3 features; v0.2 → v0.7 | Complexity remained proportional to feature maturity |
| Verification Lenses (State, Behavior) | **Validated** | 3 features; v0.6 → v0.7 | Applied as review heuristics |
| Architectural Review Questions | **Validated** | 3 features; v0.5 → v0.7 | Used in all post-implementation reviews |
| Behavior-oriented naming | **Validated** | Prontuario (strong); Paciente, Atendimento (partial) | Clear evidence in Prontuario v0.8.2 |
| Anti-redundancy principle (same property, different capability) | **Validated** | 3 features; v0.6 → v0.7 | Core architectural insight |
| Workflow completeness (Finding 20) | **Validated** | 2 features (Atendimento, Prontuario); v0.7 §20, v0.8.1 Finding 20 | Recurring pattern across independent features |
| Repository tests derivation from SDD decisions (Finding 24) | **Validated** | Prontuario; v0.8.2 Finding 24 | Clear governance gap identified |
| SQL Integration two complementary responsibilities (Finding 26) | **Validated** | Prontuario; v0.8.3 Finding 26 | Clear distinction observed |
| Builders and Seeds distinction | **Partially Validated** | Theoretical model; limited implementation | Builders not yet introduced in any feature; Seeds used informally |
| Capability Responsibilities (Command/Query Semantics) | **Partially Validated** | v0.6 → v0.7 model; Prontuario shows early evidence | Model defined; repository tests show query semantics but not formal Command/Query split |
| Domain Tests (Business Invariants) | **Partially Validated** | No dedicated Domain Test suite exists | Prontuario repository tests exercise invariant-adjacent behaviors; dedicated layer not yet validated |
| Persistence Lifecycle pattern | **Partially Validated** | Prontuario integration test (1 feature) | Strong pattern; needs confirmation in next feature |
| Scenario-Based Workflow Integration Tests | **Partially Validated** | Prontuario (1 feature); v0.8.3 Finding 25 | Promising direction; single-feature evidence |
| Controller hybrid pattern (mocks + InMemory) | **Partially Validated** | Prontuario (1 feature); v0.8.1 Finding 23 | Approved but single-feature evidence |
| Concurrency scenarios | **Experimental** | Identified as future need; v0.8.3 Finding 27 | No implementation evidence |
| Mapping Tests as first-class Architectural Capability (DQ-006) | **Experimental** | v0.7 §15; insufficient evidence | Open question; no feature has dedicated mapping tests |
| Verification Scope as explicit architectural concept (DQ-008) | **Experimental** | v0.7 §21; insufficient evidence | Promising concept; no implementation validation |
| Application Tests (Use Case Coordination) | **Experimental** | Working hypothesis; no Application layer implementation | Can only be validated when Application Services exist |
| AutoMapper AssertConfigurationIsValid as mandatory (DQ-007) | **Experimental** | Design question; no decision | Open; no evidence of recurring mapping failures |
| Scenario Builders for SQL integration | **Experimental** | v0.8.3 Finding 28; single-feature observation | Premature for governance |

---

## 4. Architectural Decisions

This section transitions from evidence to architectural conclusions. Each decision is directly traceable to validated findings in §3.

### AD-TG-001: Architectural Capability is the primary test classification

**Decision:** Automated tests shall be classified by the Architectural Capability they protect, not by technology, framework, or implementation.

**Evidence:** Validated across 3 features (v0.1 → v0.7).

**Implication:** Test governance documents, review prompts, verification gates, and SDD templates shall reference Architectural Capabilities (Persistence Intent, Physical Persistence, API Contract, Business Invariants, Use Case Coordination) as the primary organizing concept.

### AD-TG-002: Every test suite must have explicit architectural ownership

**Decision:** Every test suite shall explicitly define its Purpose, Protected Capability, Observable Behavior, Protected Boundary, and Out of Scope. Ambiguous ownership is an architectural smell.

**Evidence:** Validated across 3 features (v0.4 → v0.7).

**Implication:** SDD templates and verifier skill shall require ownership statements for new test suites. Test strategy review shall flag ambiguous ownership.

### AD-TG-003: Layers and Capabilities form a two-dimensional classification

**Decision:** Test suites shall be classified using two complementary dimensions: Architectural Layer (Presentation, Application, Domain, Infrastructure) and Architectural Capability (API Contract, Use Case Coordination, Business Invariants, Persistence Intent, Physical Persistence).

**Evidence:** Validated across 3 features (v0.2 → v0.7).

**Implication:** Test governance shall document both dimensions. New capabilities may be added when validated by implementation evidence.

### AD-TG-004: Repository Tests protect Persistence Intent; SQL Integration Tests protect Physical Persistence

**Decision:** Repository Tests and SQL Integration Tests protect distinct Architectural Capabilities and shall not share responsibilities.

- Repository Tests protect Persistence Intent (how should persistence behave?).
- SQL Integration Tests protect Physical Persistence (how must the physical infrastructure behave?).

**Evidence:** Validated across 3 features (v0.4 → v0.8.3).

**Implication:** SDD design shall specify which test suites are expected based on which capabilities are affected. Review shall flag overlapping responsibilities.

### AD-TG-005: Every Architectural Capability requires a corresponding test suite

**Decision:** Execute shall ensure that every Architectural Capability introduced or modified by the implementation has at least one corresponding test suite, or an explicit justification for its absence recorded as residual risk.

This is a general architectural obligation, not a test-type-specific rule. It applies to all current and future Architectural Capabilities.

Current validated examples:

- **Persistence Intent** → Repository Tests
- **Physical Persistence** → SQL Integration Tests
- **API Contract** → Controller Tests

Future examples (when validated by implementation evidence):

- **Business Invariants** → Domain Tests
- **Use Case Coordination** → Application Tests
- **User Interaction** → Component Tests / UI Tests

Controller Tests remain the current Presentation-layer exemplar: they protect the API Contract Architectural Capability, use real AutoMapper Profiles, and mock only infrastructure dependencies.

**Evidence:** Validated across 3 features (v0.5 → v0.8.1 for Controller Tests; generalized from Finding 20 observed in 2 features).

**Implication:** SDD Design shall identify expected Architectural Capabilities. Execute shall produce the corresponding test suites. Verifier shall validate architectural completeness. The obligation is capability-driven, not test-type-driven.

### AD-TG-006: Architectural Principles govern test design

**Decision:** The six architectural principles (AP-001 through AP-006) shall govern all automated test design in the project.

**Evidence:** Validated across 3 features.

**Implication:** Test governance shall codify these principles. Review prompts and verifier skill shall reference them.

### AD-TG-007: Progressive Evolution governs test infrastructure complexity

**Decision:** Test infrastructure (split files, Builders, Seeds, Scenario Builders, shared fixtures) shall evolve only when justified by maintainability needs. Anticipated complexity shall be avoided.

**Evidence:** Validated across 3 features (Progressive Governance principle).

**Implication:** Test governance shall define evolution triggers, not prescribed structures. SDD reviews shall question premature sophistication.

### AD-TG-008: Scenario vocabulary is standardized

**Decision:** Test scenarios shall use the standard vocabulary: Happy Path, Boundary Cases, Failure Cases, Business Rule Cases, and Planned Behavior.

**Evidence:** Validated across 3 features (v0.5 → v0.7).

**Implication:** SDD templates, test governance, and review prompts shall use this vocabulary consistently.

### AD-TG-009: Verification Lenses complement Architectural Capabilities

**Decision:** State Verification and Behavior Verification are review heuristics (lenses), not additional Architectural Capabilities. They help reviewers evaluate how completely a capability is protected.

**Evidence:** Validated across 3 features (v0.6 → v0.7).

**Implication:** Test governance and verifier skill shall reference Verification Lenses as review tools, not as test categories.

### AD-TG-010: Architectural completeness must be validated before Execute completion

**Decision:** The Execute phase shall include an explicit architectural completeness checkpoint. Implementation is not complete until all expected Architectural Capabilities identified during Design have corresponding test suites generated and passing, or their absence is justified with residual risk.

This decision establishes a three-phase separation of responsibilities aligned with the existing Harness governance:

- **Design** identifies the Architectural Capabilities affected by the feature and defines which test suites are expected.
- **Execute** implements both the functionality and the corresponding test suites for every expected Architectural Capability.
- **Verifier** validates that every expected Architectural Capability has been implemented with test evidence, or that its absence is explicitly justified with residual risk.

**Evidence:** Finding 20 confirmed in 2 independent features (Atendimento, Prontuario) where Controller Tests were omitted during initial Execute.

**Implication:** sdd-operational.md Design exit criteria shall require identification of expected Architectural Capabilities. Execute exit criteria shall include architectural completeness validation against those expectations. Verifier shall confirm completeness during the test strategy review gate. The obligation is capability-driven, not test-type-driven — future Architectural Capabilities (Component Tests, API Integration Tests, E2E Tests) inherit the same obligation automatically.

### AD-TG-011: Execute shall derive repository test scenarios from SDD architectural decisions

**Decision:** Repository test scenarios shall be explicitly derived from architectural decisions documented in the SDD, not left to developer initiative.

**Evidence:** Finding 24 (Prontuario v0.8.2).

**Implication:** design.md Testing Approach section shall require mapping architectural decisions to expected test scenarios. Execute shall reference that mapping.

### AD-TG-012: Behavior-oriented naming is the standard

**Decision:** Test names shall describe protected system behaviors rather than implementation methods. Repository tests shall prefer behavior-oriented names over persistence-oriented names.

**Evidence:** Validated in Prontuario (v0.8.2 Finding 23).

**Implication:** Test governance and review prompts shall codify this convention.

### AD-TG-013: SQL Integration Tests have two complementary responsibilities

**Decision:** SQL Integration Tests protect two complementary concerns: (1) business workflow validation against the real database, and (2) SQL Server-specific behavior validation when architectural decisions depend on database guarantees. A given SQL Integration Test may protect one or both concerns depending on the architectural decisions being validated.

**Evidence:** Validated in Prontuario (v0.8.3 Finding 26).

**Implication:** Test governance shall document both responsibilities. SDD design shall specify which responsibility or responsibilities apply. Not every SQL Integration Test must validate both concerns.

### AD-TG-014: Anti-redundancy is governed by Architectural Capability, not property

**Decision:** The same property may appear across multiple test suites when each suite protects a different Architectural Capability. This is not redundancy. The governing question is "Which architectural capability is being protected?" — not "Which property is being asserted?"

**Evidence:** Validated across 3 features (v0.6 → v0.7).

**Implication:** Test strategy review shall not flag multi-suite property assertions as redundant when different capabilities are protected.

---

## 5. Impact Assessment: Test Governance Document

### 5.1 Requirement

A new Harness Design document — `test-governance.md` — shall be created at `Documentation/AI-Harness/Harness-Design/test-governance.md`. This document does not currently exist. The current Testing Governance Model embedded in `sdd-operational.md` (§ Testing Governance Model) provides a partial foundation but lacks the architectural depth validated through implementation evidence.

### 5.2 Scope

Test Governance shall own:

- Architectural testing principles (AP-001 through AP-006 — AD-TG-006).
- Architecture-Oriented Testing Model (Layer × Capability — AD-TG-001, AD-TG-003).
- Ownership Model (AD-TG-002).
- Test categories and their architectural ownership (AD-TG-004, AD-TG-005).
- Scenario classification vocabulary (AD-TG-008).
- Test design guidelines (naming, AAA, collaborator verification, mapping strategy, Theory vs Fact — AD-TG-012).
- Progressive Evolution triggers (AD-TG-007).
- Verification Lenses as review heuristics (AD-TG-009).
- Architectural review questions.
- Repository vs SQL Integration boundaries (AD-TG-013).
- Anti-redundancy principle (AD-TG-014).
- Capability Responsibilities (Command/Query Semantics) — partially validated.

Test Governance shall **not** own:

- Verification workflow (belongs to verification-governance.md).
- Gate selection (belongs to Verifier).
- SDD phase structure (belongs to sdd-operational.md).
- Feature-specific testing plans (belongs to SDD artifacts).
- CI/CD configuration.
- Legacy behavior characterization (belongs to sdd-operational.md brownfield expectations).

### 5.3 Relationship With Existing Governance

| Existing document | Relationship with Test Governance |
|---|---|
| `sdd-operational.md` | Test Governance defines **what** testing architecture looks like; sdd-operational defines **when and how** it is applied in the SDD lifecycle. SDD's Testing Governance Model (§ Testing Governance Model) shall reference Test Governance as the authoritative source for architectural testing principles. |
| `verification-governance.md` | Test Governance defines the testing strategy. Verification Governance defines how verification gates (including test strategy review) are selected and evaluated. |
| `harness-architecture.md` | Test Governance becomes a Harness Design document within the AI Harness component responsibilities. |

---

## 6. Impact Assessment: AI Harness Governance Documents

### 6.1 verification-governance.md

**Impact:** Refinement required.

**Validated finding:** Test strategy review gate currently exists as a gate category. The scope of this gate should be refined to explicitly include architectural ownership validation, capability protection assessment, and boundary consistency checks.

**Change:** Gate category description for "Test strategy review" should reference the Architectural Review Questions from test-governance.md. The verifier should be guided to evaluate test suites against architectural ownership rather than counting tests.

### 6.2 sdd-operational.md

**Impact:** Multiple refinements required.

**Validated findings:** AD-TG-010 (architectural completeness checkpoint), AD-TG-011 (derive tests from SDD decisions), Finding 20 (workflow completeness gap).

**Changes:**

1. **§ Testing Governance Model:** Replace embedded testing governance content with a reference to the new `test-governance.md`. The model should retain only lifecycle integration content (when tests are expected, how Execute produces them, how Verify evaluates them). Architectural principles, ownership model, and test categories move to test-governance.md.

2. **Execute exit criteria (§ Phase Entry And Exit Criteria → Execute):** Add architectural completeness validation:
   - Expected architectural test suites (Repository, SQL Integration, Controller) are generated and passing, or their absence is justified with residual risk.
   - Repository test scenarios are traceable to architectural decisions in design.md.

3. **Context Acquisition Governance (Execute phase):** Test-governance.md should be loaded when the current task involves test creation or modification. This aligns with the existing phased context budget.

4. **Design entry/exit criteria:** Design should identify which Architectural Capabilities are affected and therefore which test suites are expected during Execute.

### 6.3 harness-architecture.md

**Impact:** Minor refinement.

**Validated finding:** New Harness Design document (test-governance.md) to be added to the component responsibilities table.

**Change:** Add test-governance.md to the harness design documents. Update the verification gates list to reference test governance as the source for test strategy review criteria. No structural changes required.

### 6.4 reporting-strategy.md

**Impact:** No change required.

**Rationale:** Reporting Strategy governs session handoff and feature report boundaries. Test governance changes affect implementation and verification, not reporting structure. Feature report may reference architectural test coverage but this is already captured in the verification handoff.

### 6.5 knowledge-strategy.md

**Impact:** No change required.

**Rationale:** Knowledge Strategy already defines a mandatory Testing Walkthrough section in Teacher Guides. The section's content requirements (test types used, why each exists, representative tests, known gaps) already align with the Architectural Capability model. No refinement needed.

### 6.6 sdd-pilot-report-governance.md

**Impact:** No change required.

**Rationale:** Pilot reports evaluate workflow performance. Test governance adoption may produce WF or GOV findings in future pilot reports, but the governance structure for pilot reports does not need modification.

---

## 7. Impact Assessment: Existing Artifacts

### 7.1 review-prompts/test-strategy.md

**Impact:** Refinement required.

**Current state:** Checklist-style review sensor with 10 items covering coverage, naming, synthetic data, integration tests, and SQL migration-specific debt.

**Required changes:**
- Add architectural ownership validation (is each suite protecting an explicit Architectural Capability?).
- Add boundary consistency check (are Repository and SQL Integration responsibilities distinct?).
- Add progressive evolution check (is suite complexity proportional to feature maturity?).
- Align vocabulary with test-governance.md (Happy Path, Boundary Cases, Failure Cases, Business Rule Cases, Planned Behavior).
- Add architectural completeness check (are all expected suites present?).

### 7.2 documentation-index.md

**Impact:** Minor update — add test-governance.md to the Harness Design index.

### 7.3 rules.md

**Impact:** Minor update — add test-governance.md reference when test strategy review gate is mentioned.

### 7.4 AGENTS.md

**Impact:** Minor update — add test-governance.md to the "Read First" table or the harness navigation references.

---

## 8. AI Workflow Impact

This section identifies operational workflow impacts independent of documentation changes.

### 8.1 Execute Phase

**Impact:** Architectural completeness checkpoint required.

**Current behavior:** Execute implements tasks; tests are embedded in tasks but architectural completeness is not explicitly validated. Controller Tests were omitted in 2 features.

**Required behavior:** Before Execute declares completion, confirm:
- Repository Tests exist and cover architectural decisions from design.md (AD-TG-011).
- SQL Integration Tests exist when persistence infrastructure is touched (AD-TG-010).
- Controller Tests exist when API contracts are introduced or changed (AD-TG-010, AD-TG-005).
- Any missing suite is justified with residual risk.

**Implementation approach:** This can be a checklist in tasks.md verification expectations, enforced by the verifier, or both. The SDD Design phase should determine the mechanism.

### 8.2 Verification Phase

**Impact:** Test strategy review gate scope refinement.

**Current behavior:** Test strategy review gate exists as a category. Verifier applies test-strategy review prompt.

**Required behavior:** Test strategy review should evaluate:
- Architectural ownership of each test suite.
- Boundary consistency (no overlapping responsibilities).
- Progressive evolution (complexity proportional to maturity).
- Architectural completeness (expected suites present or justified absent).
- Behavior-oriented naming.
- Scenario coverage (Happy Path, Boundary, Failure, Business Rule, Planned Behavior).

The review prompt refinement (§7.1) and verifier skill refinement (§10) operationalize this.

### 8.3 Documentation Follow-Up

**Impact:** Route test-governance.md when created.

**Current behavior:** Documentation Update identifies follow-up targets after Verify.

**Required behavior:** No structural change. test-governance.md becomes a standard candidate for routing when testing governance changes occur. The documentation-update skill should be aware of the new document.

### 8.4 Reporting

**Impact:** No operational change required.

**Rationale:** Feature reports already summarize test coverage. The architectural framing may improve report quality but no workflow change is needed.

### 8.5 Teacher Guide Generation

**Impact:** No operational change required.

**Rationale:** The Testing Walkthrough section already exists in knowledge-strategy.md § Mandatory Sections. The section requirements (test types used, why each exists, representative tests) align with the Architectural Capability model without modification.

---

## 9. Impact Assessment: Rules

### 9.1 Existing Rules — Review

| Rule | Impact | Rationale |
|------|--------|-----------|
| `security-phi` | No change | Test governance does not affect PHI/security guardrails |
| `token-economy` | No change | Context loading strategy already covers test-governance.md as a harness design document |
| `update-doc` | No change | Existing rule already covers documentation updates including new governance documents |
| `backend-architecture` | No change | Domain language and layer boundaries unchanged by test governance |
| `ef-migrations` | No change | Migration conventions unchanged; SQL integration test ownership already implied |
| `blazor-front` | No change | Frontend testing (Component Tests, UI Tests) remains future scope |

### 9.2 Candidate New Rules — Evaluation

| Candidate | Recommendation | Justification |
|-----------|---------------|---------------|
| Test strategy enforcement rule | **Do not create** | This is a workflow concern (architectural completeness validation), not a concise persistent guardrail. The verifier skill and sdd-operational Execute exit criteria are the correct mechanisms. A rule would be too procedural for the Rules layer. |
| Repository test coverage rule | **Do not create** | The requirement to derive repository tests from SDD architectural decisions (AD-TG-011) is a workflow integration concern, not a guardrail. It belongs in sdd-operational.md and the test-governance.md, not a separate rule. |
| Test naming convention rule | **Do not create** | Behavior-oriented naming (AD-TG-012) is a design guideline, not a persistent guardrail. It belongs in test-governance.md and the review prompt. A rule would be too narrow and prescriptive. |

**Conclusion:** No new rules are justified by current evidence. Existing rules remain sufficient. Architectural test governance is better expressed through Harness Design documents, SDD operational governance, templates, and the verifier skill.

---

## 10. Impact Assessment: Skills

### 10.1 Existing Skills — Refinement Candidates

| Skill | Refinement | Evidence |
|-------|-----------|----------|
| `verifier` | **Refine:** Test strategy review gate shall evaluate architectural ownership, boundary consistency, scenario coverage, and architectural completeness — not just test presence and naming. Reference test-governance.md for evaluation criteria. | v0.7 §19, v0.8.1–v0.8.3, AD-TG-010 |
| `sql-migration-workflow` | **Refine:** During Execute, when implementing repository tests, derive scenarios from SDD architectural decisions. Reference test-governance.md for Repository Test ownership and responsibilities. | v0.8.2 Finding 24, AD-TG-011 |
| `documentation-update` | **Minor:** Be aware of test-governance.md as a routing target when test governance changes occur. | Standard follow-up routing |
| `not-a-teacher` | No change | Testing Walkthrough section already defined; content requirements align with Architectural Capability model |
| `doc-organo-context` | No change | Bootstrap skill; test-governance.md will be referenced naturally through documentation-index.md |
| `codebase-decomposition` | No change | DDD analysis already includes test structure evaluation |
| `effective-harness-planning` | No change | Harness review already covers governance documents |

### 10.2 Candidate New Skills — Evaluation

| Candidate | Recommendation | Justification |
|-----------|---------------|---------------|
| Architectural test review skill | **Do not create — refine verifier instead** | The verifier skill already owns test strategy review. Adding architectural evaluation criteria to the existing skill is simpler and avoids splitting a single responsibility across two skills. |
| Test builder / scenario builder skill | **Postpone** | Builders and Seeds remain a progressive evolution concept. No feature has yet introduced Builders. Scenario Builders were observed once (Prontuário v0.8.3 Finding 28). Insufficient recurring evidence to justify a dedicated skill. Revisit after 2+ features adopt Builders or Scenario Builders. |
| Test suite scaffolding skill | **Postpone** | Creating the expected test suite structure could be automated, but current evidence shows this is a workflow gap (Finding 20), not a tooling gap. The fix is architectural completeness validation, not scaffolding automation. Revisit if workflow gap persists after governance update. |

**Conclusion:** Refine verifier and sql-migration-workflow skills. Do not create new skills. The architectural test governance is better integrated into existing workflows than expressed as separate skills.

---

## 11. Impact Assessment: Templates

### 11.1 verification.md Template

**Impact:** Refinement required for Test Coverage section.

**Current state:** Test Coverage section lists existing tests, new tests, missing/deferred tests, synthetic data confirmation.

**Required refinement:** Add architectural completeness evaluation:
- Expected suites per Architectural Capability.
- Suites present / missing / justified absent.
- Architectural ownership validation per suite.
- All expected suites present or absence justified with residual risk.

### 11.2 design.md Template

**Impact:** Refinement required for Testing Approach section.

**Current state:** Testing Approach table maps requirements to test or check approach.

**Required refinement:**
- Add explicit mapping from architectural decisions to expected test scenarios (AD-TG-011).
- Add expected Architectural Capabilities and their corresponding test suites.
- The Optional Test Debt section and Fixture Chain section appear adequate for current maturity.

### 11.3 tasks.md Template

**Impact:** Refinement required.

**Current state:** Verification Expectations table lists gate categories. No explicit architectural completeness expectations.

**Required refinement:**
- Add architectural completeness expectations: list expected test suites with their Architectural Capability.
- Each TASK that introduces an architectural change should include expected test suite type (Repository, SQL Integration, Controller).
- Verification Expectations should include expected Architectural Capabilities and their test evidence.
- Add review question: "Are all expected architectural test suites present or justified absent?"

### 11.4 teacher-guide.md Template (knowledge)

**Impact:** No change required.

**Rationale:** Testing Walkthrough section already defined with appropriate content requirements. The Architectural Capability model enriches future Teacher Guides without requiring template changes.

### 11.5 Research Template (future consideration)

**Impact:** No current template exists for research.md. This artifact may serve as a reference for future research templates. Evaluate after a second research artifact validates the structure.

---

## 12. Impact Assessment: Architectural Capabilities

### 12.1 Validated Capabilities

These capabilities are validated by implementation evidence and shall become official governance:

| Capability | Layer | Status | Evidence |
|------------|-------|--------|----------|
| API Contract | Presentation | **Adopt into governance** | 3 features |
| Persistence Intent | Infrastructure | **Adopt into governance** | 3 features |
| Physical Persistence | Infrastructure | **Adopt into governance** | 3 features |

### 12.2 Future Evolution Capabilities

These capabilities are expected to become part of the project's testing architecture as the domain model matures. They are documented here as future evolution rather than rejected architecture, with clear validation triggers:

| Capability | Layer | Current Evidence | Validation Trigger |
|-------------|-------|------------------|-------------------|
| Business Invariants | Domain | Prontuario repository tests exercise invariant-adjacent behaviors (version immutability, replace-vs-merge semantics). No dedicated Domain Test suite exists. | First feature with explicit Domain Tests separated from Repository Tests |
| Use Case Coordination | Application | Working hypothesis only — no Application layer implementation exists in the current architecture. | First feature with Application Services that orchestrate multiple repositories or domain operations |
| Verification Scope (DQ-008) | Cross-cutting | Promising concept observed in Engineering Reviews (Component Scope vs Architecture Scope vs System Scope). No implementation validation. | At least 2 features with distinct test scope patterns (e.g., Controller vs Integration vs future E2E) |

### 12.3 Candidate Capabilities — Insufficient Evidence

These capabilities were proposed during the Engineering Reviews but lack sufficient evidence for even future evolution classification:

| Candidate | Status | Reason |
|-----------|--------|--------|
| Object Mapping (DQ-006) | **Postpone** | No feature has dedicated mapping tests. AutoMapper validation through AssertConfigurationIsValid may be sufficient. Revisit when DQ-006 is resolved. |

### 12.4 Capability Responsibilities

Command Semantics and Query Semantics (within Persistence Intent) are partially validated. They should be documented as Capability Responsibilities — refinements within an Architectural Capability, not new capabilities themselves.

---

## 13. Planned Evolution Justification

Assessment of each planned Phase 3 — Harness Evolution item.

### 13.1 Core Governance

| Planned Evolution | Decision | Evidence |
|---|---|---|
| Update Test Governance | **Adopt** — create `test-governance.md` | v0.7 candidate baseline; all AD-TG decisions above |
| Improve Verification Governance integration | **Adopt** — refine test strategy review gate scope | v0.7 §19; §6.1 above |
| Improve SDD Operational integration | **Adopt** — architectural completeness checkpoint, test derivation from SDD decisions | v0.7 §20; v0.8.1 Finding 20; v0.8.2 Finding 24; §6.2 above |

### 13.2 Documentation

| Planned Evolution | Decision | Evidence |
|---|---|---|
| Refine workflow documentation | **Refine** — targeted updates to sdd-operational.md only | Workflow impacts identified in §8; other docs need minor or no changes |
| Refine templates | **Refine** — tasks.md, design.md, verification.md targeted updates | §11 above |
| Improve Documentation Follow-Up workflow | **Refine** — route test-governance.md when created | Standard routing; no workflow change |

### 13.3 Rules And Skills

| Planned Evolution | Decision | Evidence |
|---|---|---|
| Refine Rules | **Postpone** — no clear new rule from current evidence | §9.2 — all candidates evaluated; none justify a new rule |
| Refine Skills | **Refine** — verifier and sql-migration-workflow only | §10.1 — verifier and sql-migration-workflow have clear refinement needs |
| Introduce new Skills | **Postpone** — no new skill justified | §10.2 — creation of architectural test review skill, builder/scenario skill, and scaffolding skill all postponed |

### 13.4 Architectural Capabilities

| Planned Evolution | Decision | Evidence |
|---|---|---|
| Refine Architectural Capabilities | **Refine** — adopt validated, document partially validated, postpone candidates | §12 — 3 validated, 2 partially validated, 2 candidates postponed |
| Introduce Persistence Intent model | **Adopt** — fully validated | v0.7 §5; AD-TG-004 |
| Clarify testing responsibilities | **Adopt** — ownership model | v0.7 §3; AD-TG-002 |

### 13.5 Reporting And Knowledge Transfer

| Planned Evolution | Decision | Evidence |
|---|---|---|
| Improve Reporting workflow | **Postpone** — insufficient evidence of workflow gap | Feature reports already capture test coverage; architectural framing may improve quality but doesn't require governance change |
| Improve Teacher Guide workflow | **Postpone** — existing governance is sufficient | Testing Walkthrough section already defined; Architectural Capability model enriches without requiring template changes |

---

## 14. Recommended Implementation Scope

### 14.1 In Scope

Validated improvements recommended for implementation during the SDD Design phase:

| # | Implementation | Type | Traceability |
|---|---------------|------|--------------|
| 1 | Create `test-governance.md` at `Documentation/AI-Harness/Harness-Design/test-governance.md` | New governance document | AD-TG-001 through AD-TG-014 |
| 2 | Refine `sdd-operational.md` § Testing Governance Model — reference test-governance.md as authority; retain only lifecycle integration content | Governance refinement | §6.2 |
| 3 | Add architectural completeness checkpoint to `sdd-operational.md` Execute exit criteria | Governance refinement | AD-TG-010; Finding 20 |
| 4 | Add test scenario derivation requirement to `sdd-operational.md` Design exit criteria | Governance refinement | AD-TG-011; Finding 24 |
| 5 | Refine `verification-governance.md` test strategy review gate description — include architectural ownership, boundary consistency, scenario coverage | Governance refinement | §6.1 |
| 6 | Refine `review-prompts/test-strategy.md` — add architectural ownership validation, boundary consistency, progressive evolution, architectural completeness | Review prompt refinement | §7.1 |
| 7 | Refine `verifier` skill and Verification Governance integration — test strategy review shall evaluate architectural criteria from test-governance.md | Skill refinement + Governance refinement | §10.1, §6.1 |
| 8 | Refine `sql-migration-workflow` skill — derive repository test scenarios from SDD architectural decisions | Skill refinement | §10.1 |
| 9 | Refine `tasks.md` template — add architectural completeness expectations and expected test suite types | Template refinement | §11.3 |
| 10 | Refine `design.md` template — add mapping from architectural decisions to expected test scenarios | Template refinement | §11.2 |
| 11 | Refine `verification.md` template — add architectural completeness evaluation to Test Coverage section | Template refinement | §11.1 |
| 12 | Update `documentation-index.md` — add test-governance.md reference | Documentation update | §7.2 |
| 13 | Update `AGENTS.md` — add test-governance.md to harness references | Documentation update | §7.4 |
| 14 | Update `harness-architecture.md` — add test-governance.md to component responsibilities | Documentation update | §6.3 |
| 15 | Update `Documentation/State.md` — record Test Governance Adoption Research completion; advance lifecycle | Operational update | State.md protocol |

### 14.2 Out of Scope

Intentionally excluded — insufficient evidence or belongs to future iteration:

| Item | Reason |
|------|--------|
| New Rules (test strategy enforcement, repository test coverage, naming convention) | Workflow concerns, not persistent guardrails. Better expressed through governance, skills, and templates (§9.2). |
| New Skills (architectural test review, test builder, scenario builder, scaffolding) | Insufficient recurring evidence. Refinement of existing skills is sufficient (§10.2). |
| Domain Tests (Business Invariants) adoption as active capability | Partially validated — no dedicated Domain Test suite exists. Document as future capability in test-governance.md but do not mandate implementation. |
| Application Tests (Use Case Coordination) adoption as active capability | Experimental — working hypothesis only. Document as future capability in test-governance.md. |
| Mapping Tests as first-class Architectural Capability | DQ-006 remains open. No feature has dedicated mapping tests. |
| Verification Scope as explicit architectural concept | DQ-008 remains open. Promising but unvalidated. |
| AutoMapper AssertConfigurationIsValid as mandatory convention | DQ-007 remains open. No evidence of recurring mapping failures. |
| Scenario Builders for SQL integration | Single-feature observation (Prontuário). Revisit when pattern repeats. |
| Concurrency scenarios as mandatory integration test guideline | Single-feature observation (Prontuário versioning). Premature for governance. |
| Reporting workflow changes | No evidence of workflow gap. |
| Teacher Guide template changes | Existing Testing Walkthrough section already sufficient. |

---

## 15. Open Questions

The following questions remain unresolved and should be addressed during the SDD Specify or Design phase:

| ID | Question | Context | Source |
|----|----------|---------|--------|
| DQ-006 | Should Mapping Tests become a first-class Architectural Capability? | No feature has dedicated mapping tests. AutoMapper validation through AssertConfigurationIsValid may be sufficient. | v0.7 §15 |
| DQ-007 | Should AutoMapper Profile validation (AssertConfigurationIsValid) become a mandatory project convention? | Related to DQ-006. No recurring mapping failures observed. | v0.7 §16 |
| DQ-008 | Should Verification Scope become an explicit architectural concept? | Component Scope (Repository, Controller) vs Architecture Scope (Integration) vs System Scope (E2E). Promising but unvalidated. | v0.7 §21 |
| OQ-001 | Should the architectural completeness checkpoint be an Execute exit criterion, a verifier gate, or both? | AD-TG-010 requires the checkpoint. The SDD Design phase should determine the mechanism. | §8.1 |
| OQ-002 | Where in the Harness Design document hierarchy should test-governance.md be positioned relative to verification-governance.md and sdd-operational.md? | Test Governance sits between SDD Operational (when/how) and Verification Governance (verification of). | §5 |
| OQ-003 | Should Future Capabilities (Business Invariants, Use Case Coordination) be documented as aspirational targets or as formal Architectural Capabilities awaiting validation? | Partially validated capabilities need clear status in governance without over-committing. | §12.2 |
| OQ-004 | Should verification.md template refinement include a dedicated "Architectural Completeness" section or fold it into the existing Test Coverage section? | Template refinement decision for SDD Design phase. | §11.1 |

---

## 16. Evidence Traceability Matrix

Every architectural decision mapped to its evidence source(s) and affected harness components.

| Decision | Report Source(s) | Affected Harness Component(s) |
|----------|-----------------|------------------------------|
| AD-TG-001 (Capability classification) | v0.1 → v0.7 | test-governance.md, sdd-operational.md, templates |
| AD-TG-002 (Explicit ownership) | v0.4 → v0.7 | test-governance.md, review prompts, templates |
| AD-TG-003 (Two-dimensional model) | v0.2 → v0.7 | test-governance.md |
| AD-TG-004 (Repository vs Integration) | v0.4 → v0.8.3 | test-governance.md, sdd-operational.md, sql-migration-workflow skill |
| AD-TG-005 (Controller Tests) | v0.5 → v0.8.1 | test-governance.md, templates |
| AD-TG-006 (Principles) | v0.1 → v0.7 | test-governance.md |
| AD-TG-007 (Progressive Evolution) | v0.2 → v0.7 | test-governance.md, sdd-operational.md |
| AD-TG-008 (Scenario vocabulary) | v0.5 → v0.7 | test-governance.md, review prompts, templates |
| AD-TG-009 (Verification Lenses) | v0.6 → v0.7 | test-governance.md, verifier skill |
| AD-TG-010 (Architectural completeness) | v0.7 §20, v0.8.1 Finding 20 | sdd-operational.md, verification-governance.md, verifier skill, tasks.md template |
| AD-TG-011 (Derive tests from SDD) | v0.8.2 Finding 24 | sdd-operational.md, design.md template, sql-migration-workflow skill |
| AD-TG-012 (Behavior naming) | v0.8.2 Finding 23 | test-governance.md, review prompts |
| AD-TG-013 (SQL Integration responsibilities) | v0.8.3 Finding 26 | test-governance.md |
| AD-TG-014 (Anti-redundancy) | v0.6 → v0.7 | test-governance.md, review prompts |

---

## 17. Architectural Stability Assessment

This section classifies every major architectural concept identified during the research by its maturity level. The classification is derived from the Evidence Maturity Assessment (§3) and Architectural Decisions (§4). It serves as an explicit communication of architectural stability before the Specify phase begins.

### 17.1 Stable

Concepts validated by implementation evidence across multiple features. These shall become official governance and shall be implemented during the SDD.

| Concept | Classification | Features Validated | Governing Decision |
|---------|---------------|-------------------|--------------------|
| Architectural Capability as primary classification | Stable | 3 | AD-TG-001 |
| Two-dimensional classification (Layer × Capability) | Stable | 3 | AD-TG-003 |
| Ownership Model (Purpose, Protected Capability, Protected Boundary, Out of Scope) | Stable | 3 | AD-TG-002 |
| Architectural Principles (AP-001 through AP-006) | Stable | 3 | AD-TG-006 |
| Repository Tests protect Persistence Intent | Stable | 3 | AD-TG-004 |
| SQL Integration Tests protect Physical Persistence | Stable | 3 | AD-TG-004 |
| Controller Tests protect API Contract | Stable | 3 | AD-TG-005 |
| Scenario Classification vocabulary | Stable | 3 | AD-TG-008 |
| Progressive Evolution | Stable | 3 | AD-TG-007 |
| Verification Lenses (State, Behavior) | Stable | 3 | AD-TG-009 |
| Architectural Review Questions checklist | Stable | 3 | (embedded in test-governance.md) |
| Behavior-oriented naming convention | Stable | 1 (strong); 2 (partial) | AD-TG-012 |
| Anti-redundancy principle (governed by capability) | Stable | 3 | AD-TG-014 |
| Architectural completeness checkpoint | Stable | 2 (Finding 20) | AD-TG-010 |
| Repository tests derived from SDD decisions | Stable | 1 (Finding 24) | AD-TG-011 |
| SQL Integration dual responsibilities | Stable | 1 (Finding 26) | AD-TG-013 |

### 17.2 Future Evolution

Concepts with promising but incomplete evidence. They are expected to become part of the testing architecture as the domain model matures. Documented in test-governance.md as future Architectural Capabilities but not mandated for current implementation.

| Concept | Evidence Status | Validation Trigger |
|---------|----------------|--------------------|
| Business Invariants (Domain Tests) | Prontuario repository tests exercise invariant-adjacent behaviors | First feature with explicit Domain Tests separated from Repository Tests |
| Use Case Coordination (Application Tests) | Working hypothesis only; no Application Services exist | First feature with Application Services orchestrating multiple repositories |
| Verification Scope (Component vs Architecture vs System) | Promising concept from Engineering Reviews; no implementation validation | At least 2 features with distinct test scope patterns |
| Capability Responsibilities (Command/Query Semantics) | Model defined; limited implementation evidence | Features with complex query semantics beyond CRUD |

### 17.3 Deferred

Concepts proposed during the Engineering Reviews but lacking sufficient evidence to classify even as Future Evolution. They remain under observation pending explicit validation triggers.

| Concept | Reason for Deferral | Revisit Trigger |
|---------|--------------------|-----------------|
| Mapping Tests as first-class Architectural Capability (DQ-006) | No feature has dedicated mapping tests | DQ-006 resolution |
| AutoMapper AssertConfigurationIsValid as mandatory convention (DQ-007) | No evidence of recurring mapping failures | DQ-007 resolution |
| Scenario Builders for SQL integration | Single-feature observation (Prontuário) | Pattern repeats in ≥1 additional feature |
| Concurrency scenarios as mandatory integration test guideline | Single-feature observation (Prontuário versioning) | Feature with sequential resource allocation dependency |
| Builders and Seeds as standard test infrastructure | No feature has yet introduced Builders; Seeds used informally | 2+ features adopt Builders or Scenario Builders |

### 17.4 Rejected

Concepts that were evaluated and explicitly rejected for current implementation. These are not architectural failures; they are decisions not to adopt based on insufficient evidence or misalignment with the Harness architecture.

| Concept | Reason for Rejection |
|---------|---------------------|
| New Rules for test strategy enforcement, repository coverage, or naming conventions | Workflow concerns, not persistent guardrails. Better expressed through governance documents, templates, and verifier skill. See §9.2. |
| New Skills for architectural test review, builder/scenario construction, or test scaffolding | Insufficient recurring evidence. Refinement of existing skills (verifier, sql-migration-workflow) is sufficient. See §10.2. |
| Reporting workflow changes | No evidence of workflow gap. Existing feature reports already capture test coverage. See §8.4. |
| Teacher Guide template changes | Existing Testing Walkthrough section already sufficient. See §8.5. |

---

## 18. Research Status

**Status:** Complete

This research consolidates Phase 1 evidence into a stable architectural foundation for the Test Governance Adoption SDD. All findings are traceable to Engineering Review Reports v0.1 → v0.8.3. The architectural decisions in §4 shall become the primary input for the SDD Specify and Design phases.

The **Architectural Stability Assessment** (§17) explicitly communicates the maturity level of every major architectural concept before the Specify phase begins. Stable concepts shall be implemented. Future Evolution concepts shall be documented for awareness. Deferred and Rejected concepts shall not be pursued without new evidence.

**Next phase:** Specify (`specify.md`) — define scope, requirements, acceptance criteria, and sizing for the implementation items identified in §14.1 (In Scope).

**READY FOR SPECIFY.**
