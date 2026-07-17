# Test Governance — Doc Organo

The authoritative reference for architectural testing in the Doc Organo project. This document defines principles, models, vocabulary, and guidelines that govern all automated test design. It does not define lifecycle phases (see `sdd-operational.md`) or gate evaluation procedures (see `verification-governance.md`).

---

## Purpose and Scope

### What Test Governance Owns

- Architectural testing principles (AP-001 through AP-006)
- Architecture-Oriented Testing Model (Layer × Capability)
- Test Suite Ownership Model
- Test categories and their architectural ownership
- Scenario classification vocabulary
- Test design guidelines (naming, AAA, collaborator verification, mapping strategy, Theory vs Fact)
- Progressive Evolution triggers
- Verification Lenses as review heuristics
- Architectural review questions
- Anti-redundancy principle
- Capability Responsibilities (Command/Query Semantics)
- Repository vs SQL Integration boundaries
- SQL Integration dual responsibilities
- Future Evolution capabilities with validation triggers

### What Test Governance Does Not Own

- SDD phase structure, entry/exit criteria, or lifecycle sequencing (belongs to `sdd-operational.md`)
- Gate selection, review sensor application, residual risk classification, or completion decisions (belongs to `verification-governance.md` and verifier skill)
- Feature-specific testing plans or expected test suites for a specific feature (belongs to SDD feature artifacts)
- CI/CD configuration or test automation infrastructure
- Legacy behavior characterization workflow (belongs to `sdd-operational.md` brownfield expectations)

### Scope Boundaries

| Concern | Owned by | Notes |
|---------|----------|-------|
| Architectural testing principles | test-governance.md | — |
| When tests are expected in the SDD lifecycle | sdd-operational.md | Consumes test-governance.md for what those tests protect |
| How Execute produces test suites | sdd-operational.md | Consumes test-governance.md for ownership model and design guidelines |
| How Verify evaluates test suites | verification-governance.md + verifier skill | Consumes test-governance.md for architectural criteria |
| Which test suites a specific feature needs | Feature SDD — design.md | Consumes test-governance.md for capability definitions |
| CI/CD pipeline or automation | CI/CD config (out of scope) | — |

---

## Architecture-Oriented Testing Model

### Two-Dimensional Classification

Test suites are classified using two complementary dimensions. **Layer** describes where in the system architecture the suite operates. **Architectural Capability** describes which architectural concern the suite protects.

#### Dimension 1 — Architectural Layer

| Layer | Scope | Validated test suites |
|-------|-------|-----------------------|
| Presentation | API surface, external consumer contracts, HTTP interaction | Controller Tests |
| Application | Use case coordination across repositories and domain operations | Not yet implemented — Future Evolution |
| Domain | Aggregate invariants, business rules, domain transitions | Not yet implemented — Future Evolution |
| Infrastructure | Persistence contracts, physical database behavior, EF Core mappings | Repository Tests, SQL Integration Tests |

#### Dimension 2 — Architectural Capability

This is the primary classification dimension. Every test suite protects exactly one Architectural Capability.

### Validated Architectural Capabilities

Capabilities validated by implementation evidence across three features (Paciente, Atendimento Minimal, Prontuário).

| Architectural Capability | Architectural Question | Protected by | Validated |
|--------------------------|----------------------|--------------|-----------|
| API Contract | How does the system communicate with external consumers? | Controller Tests | 3 features |
| Persistence Intent | How should persistence behave? | Repository Tests | 3 features |
| Physical Persistence | How must the physical infrastructure behave? | SQL Integration Tests | 3 features |

### Future Evolution Capabilities

Capabilities with promising but incomplete evidence. They are expected to become part of the architecture as the domain model matures. See "Future Evolution" below for validation triggers. These are not mandated for current implementation. The architectural completeness obligation (AD-TG-005) automatically applies when they are promoted.

| Architectural Capability | Architectural Question | Expected test suite | Current evidence |
|--------------------------|----------------------|---------------------|------------------|
| Business Invariants | Which business rules must never be violated? | Domain Tests | Prontuario repository tests exercise invariant-adjacent behaviors |
| Use Case Coordination | How are business use cases coordinated? | Application Tests | Working hypothesis only — no Application Services exist yet |

---

## Architectural Principles

These six principles govern all automated test design in the project. They are validated across three features.

### AP-001 — Architecture before Coverage

Coverage is a consequence; architecture is the objective. Test suites exist to protect Architectural Capabilities, not to achieve coverage metrics.

**Test names that capture behavior, not implementation methods, produce coverage as a natural by-product.** Well-organized suites that protect one Architectural Capability each give **meaningful** coverage — coverage that tells you what behavior is protected, not just which lines were executed.

### AP-002 — Explicit Ownership

Every test suite must explicitly protect one Architectural Capability. Ambiguous ownership is an architectural smell.

A suite protects exactly one capability and is transparent about what it does **not** own.

### AP-003 — Boundary Protection

Tests protect architectural boundaries.

- Repository Tests protect the boundary between application and persistence intent.
- SQL Integration Tests protect the boundary between persistence intent and physical infrastructure.
- Controller Tests protect the boundary between external consumers and the API.

### AP-004 — Behavior Before Implementation

Tests should validate externally observable behavior whenever possible. Test names should describe protected behaviors, not implementation methods.

When a test name describes a method call, it implies an implementation detail is the contract. When a test name describes a behavior, it tells the reader what the system promises to do, regardless of how it does it.

### AP-005 — Progressive Governance

Governance evolves with project complexity. Test infrastructure complexity (split files, Builders, Seeds, Scenario Builders, shared fixtures) shall be triggered by maintainability needs, never by anticipation.

### AP-006 — Executable Documentation

Tests communicate expected system behavior. A well-named, well-organized test suite serves as living documentation of architectural contracts. When test names describe protected behaviors and suites are organized by Architectural Capability, the test suite becomes the most reliable form of documentation — always current, always executable.

---

## Test Suite Ownership Model

Every test suite shall explicitly declare:

| Attribute | Purpose |
|-----------|---------|
| **Purpose** | Why does this suite exist? |
| **Protected Capability** | Which Architectural Capability is protected? |
| **Observable Behavior** | Which externally observable behavior is protected? |
| **Protected Boundary** | Which architectural boundary does the suite validate? |
| **Out of Scope** | Which concerns intentionally belong to other test suites? |

Ambiguous ownership — where a suite does not clearly protect one Architectural Capability — is treated as an architectural smell. It signals that the suite may have overlapping responsibilities with another suite, or that the protected capability is not well understood.

---

## Test Categories and Architectural Ownership

### Repository Tests

| Attribute | Value |
|-----------|-------|
| **Layer** | Infrastructure |
| **Protected Capability** | Persistence Intent |
| **Architectural Question** | How should persistence behave? |
| **Purpose** | Validate that persistence contracts are honored — repository methods behave according to their declared intent when the database is abstracted away |
| **Observable Behavior** | Persistence contracts: create semantics (what is returned after creation?), update semantics (which fields are preserved vs replaced?), query semantics (what is returned for each query type?), delete semantics (soft delete vs hard delete) |
| **Protected Boundary** | Between application and persistence intent |
| **Out of Scope** | Business rules (future Domain Tests), SQL Server behavior (SQL Integration Tests), implementation details of the underlying provider |

Repository Tests exercise explicit architectural decisions about persistence behavior. They are the authoritative specification for how persistence should behave, independent of any specific database provider.

### SQL Integration Tests

| Attribute | Value |
|-----------|-------|
| **Layer** | Infrastructure |
| **Protected Capability** | Physical Persistence |
| **Architectural Question** | How must the physical infrastructure behave? |
| **Purpose** | Validate that the physical database correctly implements the persistence contracts defined by the application |
| **Observable Behavior** | SQL execution, EF Core mappings, migrations, constraints, transactions, query filters, foreign keys, database-specific compatibility |
| **Protected Boundary** | Between persistence intent and physical infrastructure |
| **Out of Scope** | Repository implementation decisions (Repository Tests) |

SQL Integration Tests protect two complementary concerns:

1. **Business workflow validation** — complete business workflows exercised against the real database (create, read, update, read, delete, verify lifecycle).
2. **SQL Server-specific behavior validation** — database guarantees that architectural decisions depend on (constraints, transactions, query filters, foreign keys, migration compatibility).

A given SQL Integration Test may protect one or both concerns depending on the architectural decisions being validated.

The Persistence Lifecycle pattern (Create → Read → Update → Read → Delete → Verify) is the preferred integration test structure. Scenario-Based Workflow validation is a promising direction for features with complex business workflows.

### Controller Tests

| Attribute | Value |
|-----------|-------|
| **Layer** | Presentation |
| **Protected Capability** | API Contract |
| **Architectural Question** | How does the system communicate with external consumers? |
| **Purpose** | Validate that HTTP endpoints honor their declared contracts — status codes, DTO shapes, routing, and collaborator interaction |
| **Observable Behavior** | HTTP semantics (status codes, response bodies), DTO contracts (field presence, types), routing (endpoint paths), collaborator interaction (which dependencies are called under which conditions) |
| **Protected Boundary** | Between external consumers and the API |
| **Out of Scope** | Repository implementation (Repository Tests), database behavior (SQL Integration Tests), business rules (future Domain Tests) |

Controller Tests shall use real AutoMapper Profiles. Only infrastructure dependencies (repositories, external services) should be mocked. The hybrid pattern (mocked repositories + real AutoMapper Profiles) is the validated standard.

Collaborator verification complements HTTP assertions — it validates that the controller invokes the correct collaborators with the correct parameters when that interaction is part of the observable behavior.

Planned Behavior (HTTP 501 Not Implemented for intentionally exposed but not-yet-implemented endpoints) should be tested to protect the API surface against accidental exposure or regression.

### Domain Tests — Future Evolution

Business Invariants. Layer: Domain. Not yet implemented. Prontuario repository tests exercise invariant-adjacent behaviors (version immutability, replace-vs-merge semantics) suggesting value in eventual dedicated Domain Tests. Validation trigger: first feature with explicit Domain Tests separated from Repository Tests.

### Application Tests — Future Evolution

Use Case Coordination. Layer: Application. Not yet implemented. Working hypothesis only. Validation trigger: first feature with Application Services orchestrating multiple repositories or domain operations.

---

## Scenario Classification

Standard vocabulary for test scenarios. Use this vocabulary consistently in test names, SDD artifacts, review prompts, and verification discussions.

| Category | Definition | Examples |
|----------|------------|----------|
| **Happy Path** | Expected successful behavior — the primary flow | Successful CRUD operations, valid payloads producing expected results |
| **Boundary Cases** | Operational limits and edge conditions | Empty values, maximum lengths, optional fields, pagination boundaries, null vs empty |
| **Failure Cases** | Unexpected or invalid scenarios — the system rejects or recovers gracefully | Entity not found, duplicate identifiers, invalid payloads, concurrency conflicts |
| **Business Rule Cases** | Business-specific behavior — invariants and rules | Aggregate invariants, versioning rules, workflow rules, soft delete semantics |
| **Planned Behavior** | Intentionally exposed before full implementation — future contracts protected now | HTTP 501 Not Implemented, planned endpoints, future contract shapes |

---

## Test Design Guidelines

### AAA Structure

Arrange-Act-Assert is the preferred structure for readability and reviewability. Every test should have a clear arrangement (setup), action (the behavior being validated), and assertion (the expected outcome).

### Behavior-Oriented Naming

Test names shall describe protected system behaviors, not implementation methods. Prefer behavior-oriented names over persistence-oriented names.

Convention: `Action_Scenario_ExpectedResult`.

Examples of behavior-oriented naming:
- `CriarNovaVersao_IncrementsVersaoAndKeepsSourceIntact`
- `SoftDelete_ExcludesFromReads_ButKeepsInMaxVersao`
- `Update_WithValidDto_ReturnsUpdatedEntity`

A test name that describes a behavior tells the reader what the system promises to do regardless of how it does it.

### Collaborator Interaction Verification

Controller Tests should verify collaborator interaction when that interaction is part of the observable behavior. This complements HTTP assertions — the controller's API contract includes both what it returns and which collaborators it invokes under which conditions.

### Mapping Strategy

Controller Tests shall use real AutoMapper Profiles. Only infrastructure dependencies (repositories, external services) should be mocked. This validates that DTO mapping contracts are correct — a mapping misconfiguration is an API contract defect.

### Theory vs Fact

Prefer `[Theory]` when the protected capability and expected behavior are identical across inputs with only parameter variation. Use `[Fact]` for behaviors that are distinct and warrant individual documentation.

---

## Progressive Evolution

Test infrastructure complexity shall evolve only when justified by maintainability needs. Anticipated complexity shall be avoided (anti-anticipation principle).

### Evolution Triggers

Introduce additional complexity only when:

- **Split files:** Suite size justifies logical organization (Commands/Queries, Create/Update/Delete/Query).
- **Builders:** Repeated construction patterns with meaningful defaults justify Builders.
- **Seeds:** Repeated scenario setup across multiple tests justifies Seeds (create, persist, prepare environment).
- **Scenario Builders:** Repeated SQL integration setup patterns across features justify Scenario Builders.
- **Shared infrastructure:** Cross-suite duplication justifies shared test infrastructure.

### Builders and Seeds — Distinction

- **Builders:** Construct valid domain objects in memory (e.g., `PacienteBuilder.Build()`).
- **Seeds:** Prepare complete test scenarios (e.g., `SeedPacienteAsync()` — creates, persists, prepares environment).

Builders create objects. Seeds create environments. Both should emerge only from necessity.

---

## Verification Lenses

Two complementary review heuristics for evaluating test suite quality. They are not additional Architectural Capabilities or test categories — they help reviewers identify missing coverage within existing suites.

| Lens | Protects | Examples |
|------|----------|----------|
| **State Verification** | "The resulting state is correct" | Persisted values in database, returned DTO fields, stored entity properties after an operation |
| **Behavior Verification** | "The component behaved according to its responsibility" | Preserved fields during updates, collaborator interaction (which methods were called), update semantics (replace vs merge), query semantics (which records are returned) |

When reviewing a test suite, ask: are both lenses adequately applied? A suite that only asserts state may miss behavior contracts (e.g., which collaborator methods were called). A suite that only asserts behavior may miss state correctness (e.g., what was persisted).

---

## Anti-Redundancy Principle

The same property may appear across multiple test suites when each suite protects a different Architectural Capability. This is not redundancy.

**The governing question is "Which Architectural Capability is being protected?" — not "Which property is being asserted?"**

Example: `EtapaAtual` (current stage). It could be validated by:
- **Domain Tests** (Business Invariants) — "Can this stage transition to that stage?"
- **Repository Tests** (Persistence Intent) — "When the stage is updated, is it correctly persisted?"
- **SQL Integration Tests** (Physical Persistence) — "When the stage column is updated through EF Core, does SQL Server enforce the constraint?"
- **Controller Tests** (API Contract) — "When the client updates the stage, is the correct HTTP status returned?"

Each suite protects a different architectural concern. The property is the same; the capability being protected is different. The test strategy review gate shall not flag multi-suite property assertions as redundant when different Architectural Capabilities are protected.

---

## Capability Responsibilities

Refinements within the Persistence Intent Architectural Capability. They are not separate capabilities — they describe complementary aspects of how persistence should behave.

### Command Semantics

What happens when data is created, updated, or deleted?

- Return semantics: what does the repository return after an operation (the created entity? a DTO? the ID only?).
- Update semantics: which fields are replaced vs preserved? Immutable fields?
- Delete semantics: soft delete vs hard delete? What happens to related entities?

### Query Semantics

What happens when data is read?

- Return semantics: what shape is returned for each query type?
- Filter semantics: which records are included or excluded (e.g., soft-deleted records)?
- Ordering: which ordering guarantees exist for returned collections?
- Null handling: what is returned when no matching record exists?

---

## Architectural Review Questions

Standard checklist for evaluating new test suites or reviewing existing suites. Apply these questions during Design and Verify.

1. Which Architectural Capability is being protected?
2. Who owns this capability?
3. Which explicit contract is being validated?
4. Is another suite already protecting this responsibility?
5. Is this testing implementation or contract?
6. Are Command Semantics adequately covered?
7. Are Query Semantics adequately covered?
8. Are Verification Lenses balanced (State and Behavior)?
9. Does the current complexity justify Builders or Seeds?
10. Does this suite respect Progressive Evolution?
11. Is the suite name behavior-oriented?
12. Is the suite's ownership declaration (Purpose, Protected Capability, Observable Behavior, Protected Boundary, Out of Scope) explicit and unambiguous?

---

## Future Evolution

The following capabilities are expected to become part of the Doc Organo testing architecture as the domain model matures. Each has a clear definition, current evidence status, and explicit validation trigger. They are not mandated for current implementation. The architectural completeness obligation (AD-TG-005) applies automatically when they are promoted.

### Business Invariants → Domain Tests

| Attribute | Value |
|-----------|-------|
| **Architectural Capability** | Business Invariants |
| **Layer** | Domain |
| **Architectural Question** | Which business rules must never be violated? |
| **Expected test suite** | Domain Tests — pure unit tests without infrastructure dependencies |
| **Current evidence** | Prontuario repository tests exercise invariant-adjacent behaviors (version immutability, replace-vs-merge semantics). No dedicated Domain Test suite exists. |
| **Validation trigger** | First feature with explicit Domain Tests separated from Repository Tests. |

### Use Case Coordination → Application Tests

| Attribute | Value |
|-----------|-------|
| **Architectural Capability** | Use Case Coordination |
| **Layer** | Application |
| **Architectural Question** | How are business use cases coordinated? |
| **Expected test suite** | Application Tests — coordinate repositories, validate transaction boundaries, orchestrate domain operations |
| **Current evidence** | Working hypothesis only. No Application Services exist in the current architecture. |
| **Validation trigger** | First feature with Application Services orchestrating multiple repositories or domain operations. |

### Verification Scope (Cross-Cutting)

| Attribute | Value |
|-----------|-------|
| **Concept** | Verification Scope: Component Scope (Repository, Controller) vs Architecture Scope (Integration) vs System Scope (E2E) |
| **Current evidence** | Promising concept observed in Engineering Reviews. No implementation validation. |
| **Validation trigger** | At least 2 features with distinct test scope patterns (e.g., Controller vs Integration vs future E2E). |

### Deferred Concepts — Under Observation

The following concepts were proposed but lack sufficient evidence for even Future Evolution classification. They remain under observation and are not promoted without new implementation evidence:

- Mapping Tests as first-class Architectural Capability
- AutoMapper `AssertConfigurationIsValid` as mandatory project convention
- Scenario Builders for SQL integration
- Concurrency scenarios as mandatory integration test guideline
- Builders and Seeds as standard test infrastructure (governed by Progressive Evolution triggers instead)

### Promotion Path

When a Future Evolution capability is promoted to validated status:

1. Implementation evidence meets the documented validation trigger.
2. `test-governance.md` is updated: the capability moves to Validated Capabilities.
3. `sdd-operational.md` Design exit criteria automatically cover the new capability.
4. The verifier skill test strategy review gate automatically evaluates it.
5. No structural Harness changes are required.

---

## Authority and Boundaries

Test Governance is a Harness Design document — the same class as `sdd-operational.md`, `verification-governance.md`, `harness-architecture.md`, `rules-strategy.md`, and `skills-strategy.md`. It sits at the same authority level as `sdd-operational.md` and `verification-governance.md`. Each owns a distinct concern:

```
sdd-operational.md          ← WHEN and HOW testing integrates into SDD lifecycle
       ↓ references
test-governance.md          ← WHAT architectural testing looks like
       ↓ evaluated by
verification-governance.md  ← HOW testing is evaluated during verification
```

This is a **separation of concerns**, not a formal hierarchy. Test Governance does not own lifecycle phases; SDD Operational does. SDD Operational does not own architectural testing definition; Test Governance does. Verification Governance owns gate evaluation, not testing architecture.

In case of conflict, the authority hierarchy from `harness-architecture.md` resolves: ADRs override all Harness Design documents. Among Harness Design documents, each owns a distinct concern with no overlap — conflicts should not arise by design.

Test Governance is the single source of truth for architectural testing in the Doc Organo project. No agent should need to synthesize testing expectations from Engineering Review Reports or scattered governance fragments.