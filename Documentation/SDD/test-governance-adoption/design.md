# Design — Test Governance Adoption

> Feature SDD: `Documentation/SDD/test-governance-adoption/`
> Generated SDD artifacts are written in English.
> **Phase:** Design (complete)
> **Lifecycle position:** Research → Specify → **Design** → Tasks → SDD Pre-Execution Review → Execute → Verify → Documentation Follow-Up → Reporting → Teacher Guide
> **Input from:** [Research](research.md), [Specify](specify.md)
> **Context budget:** Medium. Harness Design documents, templates, and skills loaded only for integration point analysis.

---

## Design Summary

Test Governance Adoption introduces a standalone `test-governance.md` as the authoritative Harness Design document for architectural testing, and refines the existing Harness to consume it consistently. The design follows three architectural moves:

1. **Establish a single source of truth.** `test-governance.md` owns architectural testing definition — principles, ownership model, two-dimensional classification, scenario vocabulary, design guidelines, progressive evolution triggers, verification lenses, and review questions. No other document duplicates these responsibilities.

2. **Propagate authority through the SDD lifecycle.** Design identifies Architectural Capabilities affected by the feature. Execute produces implementation and corresponding test suites. Verifier validates architectural completeness against expectations. Documentation Follow-Up captures governance evolution. This creates a clean authority chain without new workflow phases or ceremony.

3. **Refine consumers, do not replace them.** `sdd-operational.md` replaces its embedded Testing Governance Model with a reference to `test-governance.md`, retaining lifecycle integration content. The verifier skill and `sql-migration-workflow` skill gain architectural evaluation criteria. Templates gain architectural completeness expectations. No new rules, skills, or workflow phases are created.

The design resolves all 9 Design Decision Areas and 4 Open Questions from Specify. Every decision is traceable to an Architectural Decision (AD-TG-*) validated in Research.

---

## Requirement Mapping

| Requirement | Design response |
|-------------|-----------------|
| `REQ-001` | `test-governance.md` created as new Harness Design document; structure defined in §1 |
| `REQ-002` | AD-TG-001 through AD-TG-014 codified as sections within `test-governance.md` (§1) |
| `REQ-003` | Architecture-Oriented Testing Model, ownership model, scenario vocabulary, design guidelines, progressive evolution triggers, verification lenses, and review questions documented (§1) |
| `REQ-004` | Scope boundaries table in `test-governance.md` distinguishes Test Governance from SDD Operational, Verification Governance, and feature SDD (§1, §3) |
| `REQ-005` | `sdd-operational.md` Testing Governance Model refined to reference `test-governance.md`; lifecycle integration content retained (§2.1) |
| `REQ-006` | Architectural completeness checkpoint added to Execute exit criteria in `sdd-operational.md` (§2.1, §4) |
| `REQ-007` | Design exit criteria refined in `sdd-operational.md` to require identification of affected Architectural Capabilities and expected test suites (§2.1) |
| `REQ-008` | Verifier skill test strategy review gate refined to evaluate architectural ownership, boundary consistency, scenario coverage, and architectural completeness (§6.1) |
| `REQ-009` | `review-prompts/test-strategy.md` refined to align with Architectural Capability model (§2.4) |
| `REQ-010` | Templates refined: `design.md` (§7.1), `tasks.md` (§7.2), `verification.md` (§7.3) |
| `REQ-011` | `sql-migration-workflow` skill refined to derive repository test scenarios from SDD architectural decisions (§6.2) |
| `REQ-012` | `documentation-index.md`, `AGENTS.md`, `harness-architecture.md` updated with `test-governance.md` reference (§2.3, §2.5, §2.6) |
| `REQ-013` | `Documentation/State.md` updated — lifecycle progression recorded (§2.7) |
| `REQ-014` | Future Evolution Capabilities documented in `test-governance.md` with validation triggers (§8) |
| `REQ-015` | Governance consistency preserved through authority hierarchy and document responsibility definitions (§3) |
| `REQ-016` through `REQ-019` | Covered by `sdd-operational.md` refinements (§2.1) |
| `REQ-020` through `REQ-021` | Covered by `verification-governance.md` and verifier skill refinements (§2.2, §6.1) |
| `REQ-022` | Covered by `review-prompts/test-strategy.md` refinement (§2.4) |
| `REQ-023` | Covered by `sql-migration-workflow` skill refinement (§6.2) |
| `REQ-024` through `REQ-026` | Covered by template evolution (§7) |
| `REQ-027` through `REQ-029` | Covered by reference document updates (§2.3, §2.5, §2.6) |
| `REQ-030` | Covered by State.md update (§2.7) |
| `REQ-031` | Covered by authority hierarchy and document responsibility design (§3) |

---

## 1. Governance Architecture

### 1.1 Document Positioning

`test-governance.md` is created at `Documentation/AI-Harness/Harness-Design/test-governance.md`. It is a Harness Design document — the same class as `harness-architecture.md`, `sdd-operational.md`, `verification-governance.md`, `rules-strategy.md`, and `skills-strategy.md`.

**Resolution of DDA-007 (Document positioning):** Test Governance sits between SDD Operational and Verification Governance in the governance stack:

```
sdd-operational.md          ← Defines WHEN and HOW testing integrates into SDD lifecycle
       ↓ references
test-governance.md          ← Defines WHAT architectural testing looks like
       ↓ evaluated by
verification-governance.md  ← Defines HOW testing is evaluated during verification
```

This is not a formal hierarchy — it is a separation of concerns. Test Governance does not own lifecycle phases; SDD Operational does. SDD Operational does not own architectural testing definition; Test Governance does. Verification Governance owns gate evaluation, not testing architecture.

### 1.2 Document Structure

**Resolution of DDA-001 (Document structure and depth):** `test-governance.md` is organized around Architectural Capabilities, not around report findings or engineering review chronology. This makes it a governance reference that agents can consult during test-related tasks without needing to understand the evidence collection history.

Recommended structure:

```
# Test Governance — Doc Organo

## Purpose and Scope
  - What Test Governance owns
  - What Test Governance does not own
  - Relationship with SDD Operational, Verification Governance, feature SDD

## Architecture-Oriented Testing Model
  - Two-dimensional classification (Layer × Capability)
  - Validated Architectural Capabilities
  - Future Evolution Capabilities

## Architectural Principles (AP-001 through AP-006)

## Test Suite Ownership Model
  - Purpose, Protected Capability, Observable Behavior, Protected Boundary, Out of Scope
  - Ambiguous ownership as architectural smell

## Test Categories and Architectural Ownership
  - Repository Tests → Persistence Intent
  - SQL Integration Tests → Physical Persistence (dual responsibilities)
  - Controller Tests → API Contract
  - Domain Tests → Business Invariants (Future Evolution)
  - Application Tests → Use Case Coordination (Future Evolution)

## Scenario Classification
  - Happy Path, Boundary Cases, Failure Cases, Business Rule Cases, Planned Behavior

## Test Design Guidelines
  - AAA structure
  - Behavior-oriented naming
  - Collaborator interaction verification
  - Mapping strategy (real AutoMapper Profiles)
  - Theory vs Fact preference

## Progressive Evolution
  - Evolution triggers
  - Builders and Seeds distinction
  - Anti-anticipation principle

## Verification Lenses
  - State Verification
  - Behavior Verification
  - Review heuristics, not additional capabilities

## Anti-Redundancy Principle

## Capability Responsibilities
  - Command Semantics
  - Query Semantics

## Architectural Review Questions

## Future Evolution
  - Business Invariants (Domain Tests)
  - Use Case Coordination (Application Tests)
  - Verification Scope

## Authority and Boundaries
```

The document should be concise enough for agents to load as context during test-related tasks (aligned with NFR-008), while complete enough to serve as the single authoritative reference.

### 1.3 Responsibilities

Test Governance owns:

- Architectural testing principles (AP-001 through AP-006).
- Architecture-Oriented Testing Model (Layer × Capability).
- Test Suite Ownership Model.
- Test categories and their architectural ownership.
- Scenario classification vocabulary.
- Test design guidelines (naming, AAA, collaborator verification, mapping strategy, Theory vs Fact).
- Progressive Evolution triggers.
- Verification Lenses as review heuristics.
- Architectural review questions.
- Anti-redundancy principle.
- Capability Responsibilities (Command/Query Semantics).
- Repository vs SQL Integration boundaries.
- SQL Integration dual responsibilities.
- Future Evolution capabilities with validation triggers.

Test Governance does **not** own:

- SDD phase structure, entry/exit criteria, or lifecycle sequencing (belongs to `sdd-operational.md`).
- Gate selection, review sensor application, residual risk classification, or completion decisions (belongs to `verification-governance.md` and verifier skill).
- Feature-specific testing plans or expected test suites for a specific feature (belongs to SDD feature artifacts).
- CI/CD configuration or test automation infrastructure.
- Legacy behavior characterization workflow (belongs to `sdd-operational.md` brownfield expectations).

---

## 2. Consumer Document Refinements

### 2.1 sdd-operational.md

**Resolution of DDA-004 (sdd-operational.md restructuring):** The Testing Governance Model section (§ Testing Governance Model, currently lines 754–770) is refined to reference `test-governance.md` as the authoritative source, retaining only lifecycle integration content. A condensed 2–3 sentence summary of the architectural model remains — just enough for an agent who does not load the full `test-governance.md` to understand that a testing architecture exists and where to find it.

Three targeted changes:

**Change 1 — § Testing Governance Model (lines 754–770).** Replace the embedded governance content with:

```markdown
## Testing Governance Model

The authoritative reference for architectural testing — principles, ownership model, classification taxonomy, scenario vocabulary, design guidelines, and progressive evolution — is `Documentation/AI-Harness/Harness-Design/test-governance.md`. Load it when the current task involves test creation or modification.

Testing is part of implementation, not a separate phase after implementation. The SDD lifecycle integrates testing as follows:

- **Design** identifies affected Architectural Capabilities and which test suites are expected during Execute.
- **Execute** implements both functionality and test suites. Every expected Architectural Capability is satisfied through automated test suites (Repository Tests for Persistence Intent, SQL Integration Tests for Physical Persistence, Controller Tests for API Contract, or future suite types). A capability whose test suite is absent is explicitly justified with residual risk.
- **Verify** evaluates test evidence against architectural criteria — ownership, boundary consistency, scenario coverage, and architectural completeness.

[Retain existing governance principles lines 758–770 — they are lifecycle integration, not architectural definition]
```

**Change 2 — Design exit criteria (line 207).** Add after "ADR candidates are identified or explicitly ruled out":

```markdown
- Affected Architectural Capabilities are identified, and expected test suites (Repository, SQL Integration, Controller, or future suite types) are listed. For repository tests, architectural decisions from the SDD are mapped to expected test scenarios.
```

**Change 3 — Execute exit criteria (lines 260–267).** Add as a new bullet:

```markdown
- **Architectural completeness confirmed:** All expected Architectural Capabilities identified during Design are satisfied through corresponding test suites (generated and passing), or their absence is explicitly justified with residual risk. Repository test scenarios are traceable to architectural decisions in design.md.
```

**Resolution of DDA-008 (Context acquisition governance):** Loading `test-governance.md` when the current task involves test creation or modification is documented as a recommendation in the Testing Governance Model section (see Change 1 above: "Load it when the current task involves test creation or modification"). It is a recommendation, not a hard requirement — agents may already have the architectural model in working memory from prior context. The recommendation appears in `sdd-operational.md` because that is where Context Acquisition Governance lives.

**Resolution of DDA-006 (Architectural completeness checkpoint mechanism):** The architectural completeness checkpoint operates at two levels, resolving OQ-001:

1. **Execute exit criterion (primary mechanism):** Execute confirms that all expected test suites are present and passing before declaring implementation complete. This is a confirmation (a self-check against the list of expected Architectural Capabilities from Design), not a substitute for verification authority.
2. **Verifier gate (secondary mechanism):** The verifier's test strategy review gate evaluates architectural completeness independently. If Execute claimed completeness but a suite is missing without justification, the verifier flags it.

This dual mechanism avoids the single-point-of-failure risk observed in Finding 20. Execute self-checks prevent omission; verifier independent evaluation confirms.

### 2.2 verification-governance.md

The test strategy review gate description is refined to explicitly include architectural evaluation criteria. The current gate category (line 71: "Test strategy review") remains; its scope is expanded to reference `test-governance.md`:

**Change:** After line 71 ("Test strategy review."), add parenthetical:

```markdown
Test strategy review (architectural ownership, boundary consistency, scenario coverage, architectural completeness — criteria defined in `test-governance.md`)
```

The Verifier authority section (lines 38–43) is unchanged — the verifier already selects gates. The refinement only clarifies what the test strategy review gate evaluates.

### 2.3 harness-architecture.md

**Change:** Add `test-governance.md` to the Component Responsibilities table (lines 281–291) as a new row between existing Harness Design documents. Also add it to the Harness Assets table (lines 78–93) as a governance document.

Component Responsibilities addition:

```markdown
| `test-governance.md` | Architectural testing authority | Testing principles, ownership model, classification taxonomy, scenario vocabulary, design guidelines, progressive evolution, review questions | Engineering Review Reports, implementation evidence, accepted ADRs | Canonical testing governance consumed by SDD Operational, Verification Governance, verifier skill, and templates | New validated testing patterns, new Architectural Capability adoption | Referenced by `sdd-operational.md` Testing Governance Model; evaluated by verifier test strategy review gate |
```

### 2.4 review-prompts/test-strategy.md

**Resolution of DDA-009 (review prompt restructuring):** The review prompt retains its current checklist-style structure with added architectural evaluation items. It references `test-governance.md` as the evaluation criteria source rather than embedding criteria. This keeps the review prompt as a sensor, not a governance authority.

Five items are added:

1. **Architectural ownership:** Does each test suite explicitly protect an Architectural Capability defined in `test-governance.md`? Is ownership unambiguous?
2. **Boundary consistency:** Are Repository Test and SQL Integration Test responsibilities distinct? Is any suite protecting a capability that belongs to another suite?
3. **Progressive evolution:** Is test infrastructure complexity (split files, Builders, Seeds) proportional to feature maturity? Is there anticipated complexity?
4. **Architectural completeness:** Are all expected Architectural Capabilities from design.md covered by test suites? Are missing suites explicitly justified with residual risk?
5. **Scenario vocabulary:** Do test scenarios use the standard vocabulary (Happy Path, Boundary Cases, Failure Cases, Business Rule Cases, Planned Behavior)?

Existing items (coverage, naming, synthetic data, integration tests, SQL migration-specific debt) are preserved. The vocabulary is aligned with `test-governance.md`.

### 2.5 documentation-index.md

**Change:** Add `test-governance.md` to the Harness Design section of the documentation index. Path: `Documentation/AI-Harness/Harness-Design/test-governance.md`. Description: "Architectural testing authority — principles, ownership model, classification, design guidelines."

### 2.6 AGENTS.md

**Change:** Add `test-governance.md` to the "Read First" table as a harness navigation reference:

```markdown
| Test governance | `Documentation/AI-Harness/Harness-Design/test-governance.md` |
```

### 2.7 Documentation/State.md

**Change:** Update the Test Governance Adoption lifecycle status. The SDD research status table row for `test-governance-adoption` is updated to reflect Design phase completion. The Active Epic section is updated to reflect that Design is complete and Tasks is the next phase.

---

## 3. Document Responsibilities

This section defines the responsibility boundary for every document affected by Test Governance Adoption. The objective is eliminating overlap — no two documents should claim ownership of the same responsibility.

| Document | Owns | Does NOT own | Consumes from Test Governance |
|----------|------|--------------|------------------------------|
| `test-governance.md` | Architectural testing definition: principles, ownership model, classification, scenario vocabulary, design guidelines, progressive evolution, verification lenses, review questions, anti-redundancy, capability responsibilities | SDD lifecycle integration, gate selection, feature-specific test plans, CI/CD configuration | N/A (authoritative source) |
| `sdd-operational.md` | SDD lifecycle phases, entry/exit criteria, when tests are expected, how Execute produces tests, how Verify evaluates tests, context acquisition governance | Architectural testing principles, test design guidelines, scenario vocabulary (references test-governance.md for these) | Architectural completeness expectations, capability-driven test derivation, context loading recommendation |
| `verification-governance.md` | Gate categories, verifier authority, completion policy, environment-dependent evidence policy | Architectural testing definition, specific gate evaluation criteria | Test strategy review gate now references architectural criteria from test-governance.md |
| `harness-architecture.md` | Harness component responsibilities, authority hierarchy, three-layer taxonomy, context loading policy | Per-document content details | Registers test-governance.md as a Harness Design document |
| `review-prompts/test-strategy.md` | Review sensor checklist for test strategy evaluation | Testing architecture definition (sensor, not authority) | Architectural ownership, boundary consistency, progressive evolution, architectural completeness, scenario vocabulary as evaluation criteria |
| `design.md` template | Feature-level design structure, testing approach section | Architectural testing definition | Expected Architectural Capabilities identification, architectural decision to test scenario mapping |
| `tasks.md` template | Task decomposition, verification expectations, execution boundary | Architectural testing definition | Expected test suite types per task, architectural completeness expectations |
| `verification.md` template | Gate results, requirement evidence, test coverage summary | Architectural testing definition | Architectural completeness evaluation, capability-to-suite mapping |
| `verifier` skill | Gate selection workflow, review sensor application, residual risk classification | Testing architecture definition (references test-governance.md for evaluation criteria) | Architectural criteria for test strategy review gate |
| `sql-migration-workflow` skill | SQL migration procedure, repository stabilization pattern | Testing architecture definition (references test-governance.md for Repository Test ownership) | Repository Test ownership and scenario derivation from SDD decisions |

No responsibility overlap exists after these refinements. Each document has one clear owner for each architectural concern.

**Resolution of REQ-015 and NFR-006:** The authority hierarchy from `harness-architecture.md` remains the conflict resolution mechanism. Test Governance is a Harness Design document — it sits at the same authority level as `sdd-operational.md` and `verification-governance.md`. In case of conflict between these documents, the authority hierarchy resolves: ADRs override all three. Among the three, each owns a distinct concern with no overlap, so conflicts should not arise by design.

---

## 4. Authority Propagation

This section defines how architectural testing knowledge propagates through the SDD lifecycle. It is the architectural backbone of Test Governance Adoption.

### 4.1 Propagation Flow

```
Research
  ↓ Consumes: Engineering Review evidence
  ↓ Produces: Validated architectural patterns
  ↓
Specify
  ↓ Consumes: Research findings
  ↓ Produces: Requirements for governance codification
  ↓
Design (this phase)
  ↓ Consumes: Research, Specify
  ↓ Produces: Architectural capability expectations for the feature
  ↓ Identifies: Which existing governance documents need refinement
  ↓ Identifies: Expected Architectural Capabilities the feature itself introduces
  ↓   (For this Harness feature: no new product capabilities — governance only)
  ↓
Execute
  ↓ Consumes: Design expectations, test-governance.md
  ↓ Produces: Implementation + corresponding test suites for every expected
  ↓   Architectural Capability, or explicit justification with residual risk
  ↓ Validates: Architectural completeness before declaring completion
  ↓
Verify
  ↓ Consumes: Implementation diff, design.md capability expectations, test-governance.md
  ↓ Evaluates: Architectural ownership, boundary consistency, scenario coverage,
  ↓   architectural completeness, behavior-oriented naming
  ↓ Produces: Verification summary, residual risk, follow-up targets
  ↓
Documentation Follow-Up
  ↓ Consumes: Verification results
  ↓ Produces: Synchronized operational truth
  ↓ Routes: test-governance.md updates when new patterns are validated
  ↓
Reporting
  ↓ Consumes: Synchronized operational truth
  ↓ Produces: Feature summary, lessons learned
  ↓ Observes: Whether architectural completeness was achieved
  ↓
Teacher Guide
  ↓ Consumes: Verified implementation knowledge
  ↓ Produces: Testing Walkthrough section enriched by Architectural Capability model
```

### 4.2 Authority Origination Points

| Authority | Originates from | Consumed by |
|-----------|----------------|-------------|
| Architectural testing principles (AP-001–AP-006) | `test-governance.md` | All SDD phases, verifier, review prompts, skills |
| Expected Architectural Capabilities for a feature | `design.md` (identified during Design) | Execute (implementation target), Verify (completeness validation) |
| Test design guidelines | `test-governance.md` | Execute (test creation) |
| Architectural review questions | `test-governance.md` | Verify (test strategy review gate) |
| Architectural completeness expectations | `sdd-operational.md` Execute exit criteria + `test-governance.md` | Execute (self-check), Verify (independent evaluation) |

### 4.3 Operationalization Points

| Activity | Phase | Who operationalizes | Reference |
|----------|-------|---------------------|-----------|
| Identify expected Architectural Capabilities | Design | Agent executing Design | `sdd-operational.md` Design exit criteria |
| Derive repository test scenarios from SDD decisions | Execute | Agent executing Execute | AD-TG-011; `sql-migration-workflow` skill |
| Implement test suites for every expected capability | Execute | Agent executing Execute | AD-TG-005; `sdd-operational.md` Execute exit criteria |
| Validate architectural completeness | Execute (self-check) → Verify (evaluation) | Executing agent + Verifier | AD-TG-010; Execute exit criteria + test strategy review gate |
| Evaluate architectural ownership | Verify | Verifier | Test strategy review gate; `test-governance.md` |
| Evaluate boundary consistency | Verify | Verifier | Test strategy review gate; `test-governance.md` |

### 4.4 Verification Points

| What is verified | When | By whom | Against what |
|------------------|------|---------|--------------|
| Architectural completeness | Execute exit + Verify | Agent + Verifier | `design.md` expected capabilities |
| Architectural ownership | Verify | Verifier | `test-governance.md` ownership model |
| Boundary consistency | Verify | Verifier | `test-governance.md` test categories |
| Scenario coverage | Verify | Verifier | `test-governance.md` scenario vocabulary |
| Progressive evolution | Verify | Verifier | `test-governance.md` evolution triggers |
| Behavior-oriented naming | Verify | Verifier | `test-governance.md` naming convention |

No responsibility is duplicated. Design identifies expectations. Execute produces evidence. Verify evaluates evidence. Documentation Follow-Up captures evolution. Each phase owns one distinct role in the authority chain.

---

## 5. Workflow Integration

### 5.1 Design Phase Integration

The Design phase for product features gains one new responsibility — identifying expected Architectural Capabilities. This is a refinement of the existing design responsibility, not a new phase or checkpoint.

**What changes:** When a feature design identifies architecture, persistence, API, or domain changes, it also identifies which Architectural Capabilities are affected and therefore which test suites are expected during Execute.

**What does not change:** The Design phase structure, entry/exit criteria governance, and the existing testing approach section. The refinement adds a field to an existing section, not a new section.

**Integration mechanism:** The `design.md` template Testing Approach section gains a mapping from architectural decisions to expected test scenarios (see §7.1). `sdd-operational.md` Design exit criteria gains the capability identification requirement (see §2.1).

### 5.2 Execute Phase Integration

The Execute phase gains the architectural completeness checkpoint. This is a refinement of the existing exit criteria, not a new phase.

**What changes:** Before declaring Execute complete, the agent validates that every Architectural Capability identified during Design has a corresponding test suite or an explicit justification for its absence.

**What does not change:** Execute structure, task execution order, implementation batching, context acquisition governance. The checkpoint is an exit criterion, not a separate activity.

**Integration mechanism:** `sdd-operational.md` Execute exit criteria gains the completeness validation bullet (see §2.1). The `tasks.md` template Verification Expectations section lists expected test suites with their Architectural Capability (see §7.2).

**For product feature SDDs:** The agent loads `test-governance.md` when creating or modifying tests, derives repository test scenarios from architectural decisions in `design.md`, implements all expected test suites, and validates completeness before handoff.

**For this Harness feature (governance-only):** No test suites are produced. The architectural completeness checkpoint is satisfied by design — this feature introduces no new Architectural Capabilities that require test evidence. The verifier confirms that all governance documents, templates, skills, and review prompts were refined as expected.

### 5.3 Verify Phase Integration

The Verify phase test strategy review gate gains architectural evaluation criteria.

**What changes:** The verifier evaluates test suites against architectural ownership, boundary consistency, scenario coverage, progressive evolution, architectural completeness, and behavior-oriented naming — not just test presence and naming.

**What does not change:** Gate selection authority, review sensor application, residual risk classification, completion decisions. The verifier still owns these responsibilities.

**Integration mechanism:** The verifier skill test strategy review gate is refined (see §6.1). `review-prompts/test-strategy.md` is refined (see §2.4). `verification-governance.md` test strategy review gate description is refined (see §2.2).

### 5.4 Documentation Follow-Up Integration

No structural change. `test-governance.md` becomes a standard routing candidate when testing governance evolves. The `documentation-update` skill is aware of the new document. No workflow change is needed — Documentation Follow-Up already routes new governance documents through the mandatory checklist and Documentation Update workflow.

### 5.5 Reporting Integration

No structural change. Feature reports already summarize test coverage. The architectural framing may improve report quality (linking coverage to capabilities rather than counting tests) but no workflow change is needed.

### 5.6 Teacher Guide Integration

No structural change. The Testing Walkthrough section in Teacher Guides already aligns with the Architectural Capability model. The section's content requirements (test types used, why each exists, representative tests, known gaps) are enriched by the capability-based classification without requiring template changes.

---

## 6. Skill Integration

### 6.1 Verifier Skill

**Resolution of DDA-003 (Verifier skill refinement approach):** The verifier skill's test strategy review gate references `test-governance.md` as the evaluation criteria source. The skill does not embed architectural criteria — it delegates to `test-governance.md` for the definition and to the refined `review-prompts/test-strategy.md` for the sensor checklist. This preserves separation of concerns: the skill owns the workflow (when to apply the gate), `test-governance.md` owns the criteria (what to evaluate), and the review prompt owns the checklist (how to detect issues).

**Refinement:** In the verifier skill § Review Sensor Selection, under `test-strategy.md`, add:

```markdown
- `test-strategy.md`: automated coverage for new behavior, meaningful test names, synthetic data, explicit missing-test debt. **Architectural criteria from `test-governance.md`:** architectural ownership of each suite, boundary consistency (no overlapping responsibilities), progressive evolution (complexity proportional to maturity), architectural completeness (expected suites present or justified absent), behavior-oriented naming, scenario coverage (Happy Path, Boundary, Failure, Business Rule, Planned Behavior).
```

In § Gate Selection, under "Test strategy review" (the gate category), add a note that this gate evaluates architectural criteria from `test-governance.md`.

**What does not change:** The verifier skill's overall structure, gate selection workflow, review sensor selection mechanism, residual risk classification, completion policy, output format, and anti-patterns.

### 6.2 sql-migration-workflow Skill

**Refinement:** During Execute, when implementing repository tests, the skill references `test-governance.md` for Repository Test ownership and responsibilities. Repository test scenarios are derived from SDD architectural decisions, not left to developer initiative.

**Change:** In the skill § Repeatable Backend Stabilization Pattern, under step 5 (Tests), add:

```markdown
5. **Tests** — repository unit tests derived from architectural decisions in `design.md` (see `test-governance.md` for Repository Test ownership: protect Persistence Intent, validate persistence contracts, update semantics, query semantics). Optional controller unit tests with mocks for FK paths; one `*SqlIntegrationTests` class using `SqlIntegrationTestGate`.
```

In § Preparation, add:

```markdown
- Load `Documentation/AI-Harness/Harness-Design/test-governance.md` when the current task involves test creation. Derive repository test scenarios from architectural decisions documented in the active SDD `design.md`.
```

**What does not change:** The skill's overall structure, migration execution guidance, credential management, documentation routing, output format.

### 6.3 Skills NOT Refined

| Skill | Refinement needed? | Rationale |
|-------|-------------------|-----------|
| `documentation-update` | **Minor awareness only** — no structural change | `test-governance.md` becomes a routing target. The skill's existing routing logic handles new Harness Design documents without modification. |
| `not-a-teacher` | No | Testing Walkthrough section already defined; content requirements align with Architectural Capability model without modification. |
| `doc-organo-context` | No | Bootstrap skill; `test-governance.md` referenced naturally through `documentation-index.md` and `AGENTS.md`. |
| `codebase-decomposition` | No | DDD analysis already includes test structure evaluation. |
| `effective-harness-planning` | No | Harness review already covers governance documents. |

**No new skills are created.** Research §10.2 evaluated and rejected or postponed all candidate new skills. The architectural test governance is better integrated into existing skills than expressed as separate skills.

---

## 7. Template Evolution

### 7.1 design.md Template

**Resolution of DDA-002 (Template refinement granularity — design.md):** The existing Testing Approach section is extended, not replaced. A new "Architectural Capabilities" subsection is added within Testing Approach, following the pattern of the existing Fixture Chain subsection (optional, used when relevant).

**Refinement:** After the Testing Approach table (line 135), add:

```markdown
## Architectural Capabilities And Expected Test Suites

Identify which Architectural Capabilities are affected by this feature and which test suites are expected during Execute. See `test-governance.md` for capability definitions and test suite ownership.

| Architectural Capability | Expected test suite | Rationale |
|--------------------------|--------------------|-----------|
| Persistence Intent | Repository Tests | [e.g., New aggregate with versioning semantics] |
| Physical Persistence | SQL Integration Tests | [e.g., Schema changes, migration] |
| API Contract | Controller Tests | [e.g., New endpoints, DTO changes] |

For each Architectural Capability that is NOT expected, state why:

| Architectural Capability | Not expected because |
|--------------------------|---------------------|
| [e.g., Business Invariants] | [e.g., No new domain invariants — existing aggregate invariants already covered] |

### Architectural Decision To Test Scenario Mapping

Map architectural decisions from this design to expected repository test scenarios. This satisfies AD-TG-011: repository test scenarios shall be derived from SDD architectural decisions.

| Architectural decision | Expected repository test scenario(s) |
|------------------------|--------------------------------------|
| [e.g., Version immutability — created versions never mutate] | [e.g., Update after version creation preserves all fields] |
| [e.g., Soft delete excludes from reads but preserves for version queries] | [e.g., SoftDelete excludes from GetById; SoftDelete preserves version for MaxVersao] |
```

**What does not change:** The existing Testing Approach table, Optional Test Debt section, Fixture Chain section, and all other template sections.

**For this Harness feature (governance-only):** The Architectural Capabilities section states "No product Architectural Capabilities are affected. This feature is governance-only. Architectural completeness is satisfied by design — all governance artifacts refined as expected."

### 7.2 tasks.md Template

**Resolution of DDA-002 (Template refinement granularity — tasks.md):** The existing Verification Expectations table is extended with a new row for architectural completeness. Implementation tasks that introduce architectural changes include expected test suite types. No new dedicated section is created — architectural completeness is folded into Verification Expectations.

**Refinement 1:** In the Verification Expectations table (lines 76–89), add a row after "Test strategy review":

```markdown
| Architectural completeness | Expected / Not expected | All Architectural Capabilities from design.md have test suites or justified absence |
```

**Refinement 2:** In the Implementation Tasks section, each TASK that introduces an architectural change includes the expected test suite type:

```markdown
- [ ] `TASK-001` - ...
  - **Requirements:** `REQ-001`
  - **Files:** `...`
  - **Depends on:** None
- **Expected test suite(s):** Repository Tests | SQL Integration Tests | Controller Tests | None (no Architectural Capability affected)
  - **Tests or checks:** ...
  - **Done when:** ...
```

**Refinement 3:** In the Known Risks And Skipped Checks table, add guidance that missing test suites must be listed here with Architectural Capability and justification:

```markdown
| Risk or skipped check | Reason | Owner / follow-up |
|-----------------------|--------|-------------------|
| [e.g., Controller Tests for `REQ-005`] | [e.g., Endpoint matches existing pattern; covered by SQL integration workflow validation] | [e.g., Accepted residual risk — re-evaluate when API contract diverges] |
```

**What does not change:** Execution Boundary, TASK/VP/DF categorization, Dependency Map, Requirement Traceability, Review Sensors, Commit Guidance, Execution Batching, Completion Handoff.

**For this Harness feature:** Tasks are governance refinement tasks. No test suite types are expected. The Architectural completeness row states "Expected — all governance artifacts refined as specified."

### 7.3 verification.md Template

**Resolution of DDA-002 (verification.md) and OQ-004:** The existing Test Coverage section is extended with architectural completeness evaluation, not replaced with a dedicated section. The refinement adds architectural framing to the existing coverage summary.

**Refinement:** In the Test Coverage section (lines 69–73), add after the existing subsections:

```markdown
### Architectural Completeness

| Architectural Capability (from design.md) | Expected test suite | Present? | Evidence or justification |
|--------------------------------------------|--------------------|----------|---------------------------|
| Persistence Intent | Repository Tests | Yes / No / Not expected | [e.g., ProntuarioRepositoryTests.cs — 27 tests] |
| Physical Persistence | SQL Integration Tests | Yes / No / Not expected | [e.g., ProntuarioSqlIntegrationTests.cs — 1 test] |
| API Contract | Controller Tests | Yes / No / Not expected | [e.g., ProntuarioControllerTests.cs — 27 tests] |

Missing suites justified with residual risk:
- [e.g., No Controller Tests for `REQ-005` — endpoint matches existing pattern; accepted residual risk]

Architectural ownership validation:
- [e.g., Repository Tests own Persistence Intent — no overlap with SQL Integration Tests]
- [e.g., All suites have explicit Protected Capability per test-governance.md ownership model]
```

**What does not change:** Change Classification, Execute Handoff Notes, Gate Results, Runtime Validation, Review Sensors, Requirement Evidence, Residual Risk, Follow-up Needed, Completion Decision.

**For this Harness feature:** The Architectural Completeness table documents that this is a governance-only feature with no product Architectural Capabilities. The table states "Not applicable — governance feature; no product code, database, API, or test code changes." The architectural completeness of the governance itself is validated by cross-document reference checks and governance consistency review.

---

## 8. Future Evolution

**Resolution of DDA-005 (Future Capability documentation format) and OQ-003:** Future Evolution capabilities are documented as formal Architectural Capabilities awaiting validation — not as aspirational targets. Each has a clear definition, current evidence status, and explicit validation trigger. This approach communicates that these capabilities are expected parts of the architecture (not speculative wishes) while being honest that they are not yet validated for mandate.

### 8.1 Business Invariants (Domain Tests)

| Attribute | Value |
|-----------|-------|
| Architectural Capability | Business Invariants |
| Layer | Domain |
| Current status | **Future Evolution** — not mandated for current implementation |
| Evidence | Prontuario repository tests exercise invariant-adjacent behaviors (version immutability, replace-vs-merge semantics). No dedicated Domain Test suite exists. |
| Validation trigger | First feature with explicit Domain Tests separated from Repository Tests |
| Expected test suite | Domain Tests — pure unit tests without infrastructure dependencies |
| Architectural question | "Which business rules must never be violated?" |

### 8.2 Use Case Coordination (Application Tests)

| Attribute | Value |
|-----------|-------|
| Architectural Capability | Use Case Coordination |
| Layer | Application |
| Current status | **Future Evolution** — not mandated for current implementation |
| Evidence | Working hypothesis only. No Application Services exist in the current architecture. |
| Validation trigger | First feature with Application Services orchestrating multiple repositories or domain operations |
| Expected test suite | Application Tests — coordinate repositories, validate transaction boundaries |
| Architectural question | "How are business use cases coordinated?" |

### 8.3 Verification Scope

| Attribute | Value |
|-----------|-------|
| Concept | Verification Scope (Component Scope vs Architecture Scope vs System Scope) |
| Current status | **Future Evolution** — not mandated for current implementation |
| Evidence | Promising concept observed in Engineering Reviews. No implementation validation. |
| Validation trigger | At least 2 features with distinct test scope patterns (e.g., Controller vs Integration vs future E2E) |
| Purpose | Classify tests by verification scope in addition to Architectural Capability |

### 8.4 Other Deferred Concepts

The following concepts from Research §17.3 remain deferred. They are not documented as Future Evolution capabilities because they lack even partial validation:

- Mapping Tests as first-class Architectural Capability (DQ-006).
- AutoMapper AssertConfigurationIsValid as mandatory convention (DQ-007).
- Scenario Builders for SQL integration.
- Concurrency scenarios as mandatory integration test guideline.
- Builders and Seeds as standard test infrastructure.

These remain under observation. Revisit when new evidence emerges.

### 8.5 Future Capability Integration

When a Future Evolution capability is promoted to validated status, the integration path is:

1. New implementation evidence meets the validation trigger.
2. An Engineering Review or feature SDD documents the evidence.
3. `test-governance.md` is updated: the capability moves from Future Evolution to Validated Capabilities, and the corresponding test suite type becomes expected during Execute.
4. `sdd-operational.md` Design exit criteria (capability identification) automatically cover the new capability — no structural change needed.
5. The verifier skill test strategy review gate automatically evaluates the new capability — it evaluates all Architectural Capabilities from `test-governance.md`.

This path is designed for minimal friction. The architectural completeness obligation (AD-TG-005) applies to all Architectural Capabilities — current and future — without modification.

---

## 9. Affected Components

| Component | Path | Responsibility | Change |
|-----------|------|----------------|--------|
| `test-governance.md` (new) | `Documentation/AI-Harness/Harness-Design/test-governance.md` | Architectural testing authority | Create |
| `sdd-operational.md` | `Documentation/AI-Harness/Harness-Design/sdd-operational.md` | SDD lifecycle governance | Refine — Testing Governance Model, Design exit criteria, Execute exit criteria |
| `verification-governance.md` | `Documentation/AI-Harness/Harness-Design/verification-governance.md` | Verification governance | Refine — test strategy review gate description |
| `harness-architecture.md` | `Documentation/AI-Harness/Harness-Design/harness-architecture.md` | Harness component responsibilities | Update — add test-governance.md |
| `review-prompts/test-strategy.md` | `Documentation/AI-Harness/review-prompts/test-strategy.md` | Test strategy review sensor | Refine — architectural criteria |
| `design.md` template | `Documentation/AI-Harness/template/sdd/design.md` | Feature design template | Refine — Architectural Capabilities section, decision-to-scenario mapping |
| `tasks.md` template | `Documentation/AI-Harness/template/sdd/tasks.md` | Task planning template | Refine — expected test suite types, architectural completeness row |
| `verification.md` template | `Documentation/AI-Harness/template/verification/verification.md` | Verification template | Refine — Architectural Completeness subsection |
| `verifier` skill | `.cursor/skills/verifier/SKILL.md`, `.cline/skills/verifier/SKILL.md` | Verification workflow | Refine — test strategy review gate architectural criteria |
| `sql-migration-workflow` skill | `.cursor/skills/sql-migration-workflow/SKILL.md`, `.cline/skills/sql-migration-workflow/SKILL.md` | SQL migration workflow | Refine — test derivation from SDD decisions |
| `documentation-index.md` | `Documentation/AI-Harness/documentation-index.md` | Documentation index | Update — add test-governance.md |
| `AGENTS.md` | Repository root | Agent bootstrap | Update — add test-governance.md reference |
| `Documentation/State.md` | `Documentation/State.md` | Operational truth | Update — lifecycle progression |

---

## 10. Reuse Analysis

| Existing pattern | Reuse decision | Notes |
|------------------|----------------|-------|
| Harness Design document structure | Reuse | `test-governance.md` follows the same pattern as `verification-governance.md` and `sdd-operational.md` — Purpose, Scope, Responsibilities, Authority Boundaries |
| Test strategy review gate | Adapt | Existing gate category refined to include architectural criteria; gate remains in verifier skill and verification governance |
| SDD template section patterns | Adapt | Existing Testing Approach, Verification Expectations, and Test Coverage sections extended — not replaced |
| Skill refinement approach from HGEP v2.0 | Reuse | Same pattern: targeted section additions without structural changes |
| Architectural Capability from Engineering Reviews | Reuse — codify | Model validated across 3 features; codified as-is without structural modification |
| Authority hierarchy from harness-architecture.md | Reuse | Existing conflict resolution rules apply to test-governance.md without modification |

---

## 11. Architecture And Ownership

- **Architecture impact:** A new Harness Design document is introduced. Three existing governance documents are refined. Two skills are refined. Three templates are refined. One review prompt is refined. Three reference documents are updated. No new rules, skills, or workflow phases are introduced.
- **Ownership boundaries:** Test Governance owns architectural testing definition. SDD Operational owns lifecycle integration. Verification Governance owns gate evaluation. No ownership boundaries change — responsibilities are clarified, not moved.
- **Conflicts or constraints:** None. The Research confirmed that all 14 Architectural Decisions are validated and stable. The Specify phase confirmed that no existing governance contradicts the proposed refinements. The Design resolves all 9 DDAs and 4 OQs from Specify.

---

## 12. Testing Approach

This is a governance feature. No product code, database schema, API contracts, or test code changes.

| Requirement | Test or check approach | Notes |
|-------------|------------------------|-------|
| `REQ-001` through `REQ-014` | Content review of `test-governance.md` against Research §4, §5.2, §12 | Verifier content completeness gate |
| `REQ-015` through `REQ-019` | Diff review of `sdd-operational.md` | Confirm reference to test-governance.md, architectural completeness in exit criteria |
| `REQ-020` through `REQ-021` | Diff review of `verification-governance.md` and verifier skill | Confirm architectural evaluation criteria |
| `REQ-022` | Diff review of `review-prompts/test-strategy.md` | Confirm architectural criteria additions |
| `REQ-023` | Diff review of `sql-migration-workflow` skill | Confirm test derivation from SDD decisions |
| `REQ-024` through `REQ-026` | Diff review of templates | Confirm architectural completeness additions |
| `REQ-027` through `REQ-029` | Cross-document reference check | Confirm test-governance.md referenced in all three documents |
| `REQ-030` | Content review of `Documentation/State.md` | Confirm lifecycle progression |
| `REQ-031` | Governance consistency review | No contradictory guidance across all refined documents |
| `AC-020` | `dotnet build && dotnet test` | Regression validation — existing 79 tests must pass |
| `AC-021` | `dotnet build && dotnet test` | Same as AC-020 |

---

## 13. Architectural Capabilities And Expected Test Suites

This feature is governance-only. No product Architectural Capabilities are affected. No test suites are expected.

| Architectural Capability | Expected for this feature? | Rationale |
|--------------------------|---------------------------|-----------|
| Persistence Intent | No | No persistence behavior changes |
| Physical Persistence | No | No schema, migration, or SQL behavior changes |
| API Contract | No | No controller, DTO, or route changes |
| Business Invariants | No (Future Evolution) | No domain behavior changes |
| Use Case Coordination | No (Future Evolution) | No application services created |

Architectural completeness for this feature is satisfied by design: all governance artifacts (documents, templates, skills, review prompts, reference documents) are refined as specified in §2 and §6–§7. The verifier confirms completeness through cross-document reference checks and governance consistency review, not through test suite execution.

---

## 14. Verification Handoff Notes

- **Expected gates:** Build (`dotnet build`), Automated tests (`dotnet test` — regression validation), Documentation review (content completeness + cross-document consistency), Test strategy review (governance consistency, not test suite evaluation), ADR evaluation (not needed — no durable architecture decisions).
- **Expected review sensors:** `check-docs.md` (governance consistency, path drift), `test-strategy.md` (governance alignment with architectural criteria), `security-phi-review.md` (skipped — no PHI/security impact).
- **Known skipped or manual checks:** SQL/Persistence (skipped — no database changes), API (skipped — no API changes), UI (skipped — no UI changes), Security/PHI (skipped — governance-only), Domain review (skipped — no domain changes), Legacy characterization (skipped — no Legacy behavior involved).
- **Evidence the implementation must provide:** Diffs for all refined documents, content of new `test-governance.md`, cross-document reference verification, `dotnet build && dotnet test` passing.

---

## 15. Risks

| Risk | Impact | Mitigation |
|------|--------|------------|
| Governance inconsistency after multiple refinements | Conflicting guidance between Test Governance, SDD Operational, and Verification Governance | Governance consistency review as verification gate; cross-document reference check; authority hierarchy resolves any conflicts |
| test-governance.md token consumption | Agents may avoid loading due to size | Document structure prioritized for conciseness (NFR-008); condensed summary in sdd-operational.md provides fallback |
| Template refinements create friction for existing SDD workflows | Agents using old mental model until new templates are internalized | Refinements are additive (new subsections, new rows) — existing template sections unchanged |
| Skill refinements not synchronized between Cursor and Cline projections | Verifier and sql-migration-workflow skills diverge between tool projections | Both projections refined in the same Execute session; content is identical, only storage location differs |

---

## 16. ADR Evaluation

| Question | Answer |
|----------|--------|
| Does this change affect durable architecture, persistence, schema lifecycle, security/auth, API contracts, ownership, runtime, or irreversible migration decisions? | No |
| Existing ADRs referenced | ADR-001 (soft delete — test governance references it for Repository Test query filter expectations but does not change it), ADR-006 (dual-mode versioning API — test governance references it as an example of architectural decision to test scenario derivation but does not change it) |
| New ADR candidate | None |
| Decision needed before tasks or execution? | No |

Test Governance Adoption codifies architectural decisions (AD-TG-001 through AD-TG-014) that are already validated by implementation evidence. These are Architectural Decisions within the SDD, not Architecture Decision Records requiring formal ADR governance. The Design does not identify any durable architecture decisions meeting ADR criteria.

---

## 17. Design Decision Areas — Resolutions

All 9 Design Decision Areas from Specify are resolved by this Design.

| DDA | Resolution | Design section |
|-----|-----------|----------------|
| DDA-001 (Document structure and depth) | Organized around Architectural Capabilities, not report chronology. Concise governance reference structure defined. | §1.2 |
| DDA-002 (Template refinement granularity) | design.md: new subsection within Testing Approach. tasks.md: new row in Verification Expectations + expected test suite per TASK. verification.md: new subsection within Test Coverage. All additive — existing sections unchanged. | §7 |
| DDA-003 (Verifier skill refinement approach) | Skill references test-governance.md for criteria; review prompt referenced for sensor checklist. Skill owns workflow, not criteria. | §6.1 |
| DDA-004 (sdd-operational.md restructuring) | Replace embedded governance with reference + condensed 2–3 sentence summary. Retain lifecycle integration content. | §2.1 |
| DDA-005 (Future Capability documentation format) | Formal Architectural Capabilities awaiting validation — not aspirational targets. Each has definition, evidence status, and validation trigger. | §8 |
| DDA-006 (Architectural completeness checkpoint) | Dual mechanism: Execute exit criterion (primary) + verifier test strategy review gate (secondary). Resolves OQ-001. | §2.1 |
| DDA-007 (Test governance document positioning) | Sits between SDD Operational (when/how) and Verification Governance (verification of). Same authority level as both — concern separation, not hierarchy. Resolves OQ-002. | §1.1 |
| DDA-008 (Context acquisition governance) | Recommendation in sdd-operational.md Testing Governance Model section. Not a hard requirement — agents may already have model in working memory. | §2.1 |
| DDA-009 (review-prompts/test-strategy.md restructuring) | Retains current checklist structure with added architectural items. References test-governance.md as criteria source. | §2.4 |

---

## 18. Open Questions — Resolutions

All 4 Open Questions from Specify are resolved by this Design.

| OQ | Resolution | Design section |
|----|------------|----------------|
| OQ-001 (Checkpoint mechanism) | Dual mechanism: Execute exit criterion (primary) + verifier test strategy review gate (secondary) | §2.1, §4 |
| OQ-002 (Document positioning) | Test Governance sits between SDD Operational and Verification Governance — same authority level, distinct concerns | §1.1 |
| OQ-003 (Future Capability format) | Formal Architectural Capabilities awaiting validation with evidence status and explicit validation triggers | §8 |
| OQ-004 (verification.md refinement) | Folded into existing Test Coverage section as a new "Architectural Completeness" subsection — not a dedicated top-level section | §7.3 |

---

## 19. Design Completeness Checklist

Before this Design is considered complete:

- [x] All 14 AD-TG decisions addressed in governance architecture
- [x] All 31 REQs mapped to design responses
- [x] All 9 DDAs resolved
- [x] All 4 OQs resolved
- [x] Document responsibilities defined for all affected artifacts
- [x] Authority propagation defined through full SDD lifecycle
- [x] Workflow integration described for Design, Execute, Verify, Documentation Follow-Up, Reporting, Teacher Guide
- [x] Architectural Completeness concept designed with dual mechanism
- [x] Skill refinements designed for verifier and sql-migration-workflow
- [x] Template evolution designed for design.md, tasks.md, verification.md
- [x] Future Evolution capabilities documented with validation triggers
- [x] No new rules, skills, or workflow phases introduced
- [x] Backward compatibility preserved — existing test suites unaffected
- [x] Authority hierarchy respected — no ownership conflicts
- [x] ADR evaluation completed — no ADR required
- [x] Risks identified and mitigated

---

## 20. References

- `Documentation/SDD/test-governance-adoption/research.md`
- `Documentation/SDD/test-governance-adoption/specify.md`
- `Documentation/AI-Harness/Harness-Design/harness-architecture.md`
- `Documentation/AI-Harness/Harness-Design/sdd-operational.md`
- `Documentation/AI-Harness/Harness-Design/verification-governance.md`
- `Documentation/AI-Harness/template/sdd/design.md`
- `Documentation/AI-Harness/template/sdd/tasks.md`
- `Documentation/AI-Harness/template/verification/verification.md`
- `Documentation/AI-Harness/review-prompts/test-strategy.md`
- `.cursor/skills/verifier/SKILL.md`
- `.cline/skills/verifier/SKILL.md`
- `.cursor/skills/sql-migration-workflow/SKILL.md`
- `.cline/skills/sql-migration-workflow/SKILL.md`
- `Documentation/State.md`
- `AGENTS.md`