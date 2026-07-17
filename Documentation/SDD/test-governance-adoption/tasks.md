# Tasks — Test Governance Adoption

> Feature SDD: `Documentation/SDD/test-governance-adoption/`
> Generated SDD artifacts are written in English.
> **Phase:** Tasks (complete)
> **Lifecycle position:** Research → Specify → Design → **Tasks** → SDD Pre-Execution Review → Execute → Verify → Documentation Follow-Up → Reporting → Teacher Guide
> **Input from:** [Research](research.md), [Specify](specify.md), [Design](design.md)
> **Sizing:** Medium — governance-only Harness feature. No product code, database, API, or test code changes.

---

## Execution Boundary

- **Included:** Creation of `test-governance.md` (new Harness Design document), refinement of 3 governance documents (`sdd-operational.md`, `verification-governance.md`, `harness-architecture.md`), refinement of 1 review prompt (`test-strategy.md`), refinement of 2 skills (`verifier`, `sql-migration-workflow`) across both Cursor and Cline projections, refinement of 3 SDD templates (`design.md`, `tasks.md`, `verification.md`), update of 3 reference documents (`documentation-index.md`, `AGENTS.md`, `Documentation/State.md`).
- **Excluded:** Product code, database schema, API contracts, test code, CI/CD configuration, reporting artifacts, Teacher Guide generation. No new Rules or Skills are created.
- **Explicitly excluded from Execute:** verification artifacts (`verification.md`), reporting (`feature-report.md`, `session-handoff.md`), Teacher Guide generation.
- **Current execution does not begin until SDD Pre-Execution Review exit criteria are satisfied.**

---

## Implementation Tasks (TASK)

### Workstream 1 — Introduce Authoritative Governance

- [ ] `TASK-001` — Create `test-governance.md` as the authoritative Harness Design document for architectural testing
  - **Requirements:** `REQ-001`, `REQ-002`, `REQ-003`, `REQ-004`, `REQ-005`, `REQ-006`, `REQ-007`, `REQ-008`, `REQ-009`, `REQ-010`, `REQ-011`, `REQ-012`, `REQ-013`, `REQ-014`, `NFR-001`, `NFR-003`, `NFR-004`, `NFR-005`, `NFR-006`, `NFR-007`, `NFR-008`
  - **Architectural Decisions:** AD-TG-001 through AD-TG-014, AP-001 through AP-006
  - **Design reference:** §1 (Governance Architecture), §3 (Document Responsibilities), §8 (Future Evolution)
  - **Files:**
    - `Documentation/AI-Harness/Harness-Design/test-governance.md` (create)
  - **Depends on:** None
  - **Expected test suite(s):** None (governance-only — no Architectural Capability affected)
  - **Tests or checks:** Content completeness review against Research §4, §5.2, §12; boundary consistency review against Design §1.3 (what Test Governance owns vs does not own); structure review against Design §1.2 recommended structure
  - **Done when:**
    - Document exists at `Documentation/AI-Harness/Harness-Design/test-governance.md`
    - Codifies all 14 Architectural Decisions (AD-TG-001 through AD-TG-014)
    - Defines the Architecture-Oriented Testing Model (Layer × Capability), Ownership Model, six Architectural Principles, scenario vocabulary, test design guidelines, progressive evolution triggers, verification lenses, architectural review questions, anti-redundancy principle, and capability responsibilities
    - Clearly distinguishes its responsibilities from SDD Operational, Verification Governance, and feature SDD artifacts (Scope Boundaries table per Research §5.2)
    - Documents Future Evolution Capabilities (Business Invariants, Use Case Coordination, Verification Scope) with explicit validation triggers and statement that they are not mandated for current implementation
    - Follows the recommended structure in Design §1.2
    - Satisfies NFR-008 (concise enough for context loading, complete enough as single authoritative reference)

  **Internal implementation checklist** — execution guidance only. Does not create additional tasks, dependencies, IDs, or requirement mappings.

  - [ ] Create the document skeleton (`Documentation/AI-Harness/Harness-Design/test-governance.md`) following the recommended structure in Design §1.2.
  - [ ] Write **Purpose and Scope** — what Test Governance owns, what it does not own, relationship with SDD Operational, Verification Governance, and feature SDD (Scope Boundaries table per Research §5.2).
  - [ ] Write **Architecture-Oriented Testing Model** — two-dimensional classification (Layer × Capability), validated Architectural Capabilities (API Contract, Persistence Intent, Physical Persistence), Future Evolution Capabilities (Business Invariants, Use Case Coordination, Verification Scope) with explicit validation triggers.
  - [ ] Codify **six Architectural Principles** (AP-001 through AP-006) — Architecture before Coverage, Explicit Ownership, Boundary Protection, Behavior Before Implementation, Progressive Governance, Executable Documentation.
  - [ ] Define **Test Suite Ownership Model** — Purpose, Protected Capability, Observable Behavior, Protected Boundary, Out of Scope. Ambiguous ownership as architectural smell.
  - [ ] Document **Test Categories and Architectural Ownership** — Repository Tests → Persistence Intent, SQL Integration Tests → Physical Persistence (dual responsibilities per AD-TG-013), Controller Tests → API Contract, Domain Tests → Business Invariants (Future Evolution), Application Tests → Use Case Coordination (Future Evolution).
  - [ ] Define **Scenario Classification** vocabulary — Happy Path, Boundary Cases, Failure Cases, Business Rule Cases, Planned Behavior.
  - [ ] Document **Test Design Guidelines** — AAA structure, behavior-oriented naming (AD-TG-012), collaborator interaction verification, mapping strategy (real AutoMapper Profiles), Theory vs Fact preference.
  - [ ] Define **Progressive Evolution** — evolution triggers, Builders and Seeds distinction, anti-anticipation principle (AD-TG-007).
  - [ ] Document **Verification Lenses** — State Verification and Behavior Verification as review heuristics, not additional Architectural Capabilities (AD-TG-009).
  - [ ] Document **Anti-Redundancy Principle** — same property may appear across multiple suites when each protects a different Architectural Capability (AD-TG-014).
  - [ ] Document **Capability Responsibilities** — Command Semantics and Query Semantics as refinements within Persistence Intent.
  - [ ] Include standard **Architectural Review Questions** checklist (from Research §2.11).
  - [ ] Write **Future Evolution** section — Business Invariants, Use Case Coordination, Verification Scope with evidence status, validation triggers, and explicit statement that they are not mandated for current implementation (Design §8, Research §12.2).
  - [ ] Write **Authority and Boundaries** — document positioning relative to SDD Operational and Verification Governance (Design §1.1), responsibility boundaries (Design §1.3).
  - [ ] Review for **NFR-008** — document is concise enough for context loading while complete enough as single authoritative reference.
  - [ ] Review for **NFR-007** — every governance statement traceable to AD-TG decisions and Engineering Review Reports (Research §16 Evidence Traceability Matrix).
  - [ ] Validate **boundary consistency** — confirm Test Governance does not duplicate responsibilities owned by SDD Operational or Verification Governance (Design §3 Document Responsibilities table).
  - [ ] Validate **structure** — confirm document follows the recommended structure in Design §1.2.
  - [ ] Confirm **all 14 Architectural Decisions** (AD-TG-001 through AD-TG-014) are codified in the document.

### Workstream 2 — Integrate Governance into SDD Lifecycle

- [ ] `TASK-002` — Refine `sdd-operational.md` to reference Test Governance and add architectural completeness checkpoints
  - **Requirements:** `REQ-005`, `REQ-006`, `REQ-007`, `REQ-015`, `REQ-016`, `REQ-017`, `REQ-018`, `REQ-019`, `NFR-001`, `NFR-006`
  - **Architectural Decisions:** AD-TG-010, AD-TG-011
  - **Design reference:** §2.1 (sdd-operational.md), §5.1 (Design Phase Integration), §5.2 (Execute Phase Integration)
  - **Files:**
    - `Documentation/AI-Harness/Harness-Design/sdd-operational.md` (refine)
  - **Depends on:** `TASK-001` (references `test-governance.md`)
  - **Expected test suite(s):** None (governance-only)
  - **Tests or checks:** Diff review confirming 3 targeted changes per Design §2.1; cross-document reference check confirming `test-governance.md` is referenced; content review confirming lifecycle integration content is retained
  - **Done when:**
    - **Change 1:** Testing Governance Model section (§ Testing Governance Model) replaced with reference to `test-governance.md` + condensed 2–3 sentence summary + retained lifecycle integration content (governance principles lines 758–770 preserved). Loading `test-governance.md` documented as a recommendation when the current task involves test creation or modification (per Design §2.1 Change 1 and DDA-008 resolution).
    - **Change 2:** Design exit criteria include identification of affected Architectural Capabilities and expected test suites, plus mapping of architectural decisions to expected repository test scenarios (per Design §2.1 Change 2).
    - **Change 3:** Execute exit criteria include architectural completeness validation: expected Architectural Capabilities have test suites or justified absence (per Design §2.1 Change 3).
    - No other sections of `sdd-operational.md` are modified.

### Workstream 3 — Integrate Governance into Verification

- [ ] `TASK-003` — Refine `verification-governance.md` test strategy review gate description
  - **Requirements:** `REQ-008`, `REQ-020`, `NFR-001`, `NFR-006`
  - **Architectural Decisions:** AD-TG-010
  - **Design reference:** §2.2 (verification-governance.md), §5.3 (Verify Phase Integration)
  - **Files:**
    - `Documentation/AI-Harness/Harness-Design/verification-governance.md` (refine)
  - **Depends on:** `TASK-001` (references `test-governance.md`)
  - **Expected test suite(s):** None (governance-only)
  - **Tests or checks:** Diff review confirming test strategy review gate description refined per Design §2.2; content review confirming verifier authority section unchanged
  - **Done when:**
    - Test strategy review gate description (line 71) expanded to include parenthetical: "architectural ownership, boundary consistency, scenario coverage, architectural completeness — criteria defined in `test-governance.md`" (per Design §2.2).
    - Verifier authority section (lines 38–43) unchanged.

- [ ] `TASK-004` — Refine `verifier` skill to evaluate test suites against architectural criteria from `test-governance.md`
  - **Requirements:** `REQ-008`, `REQ-021`, `NFR-001`
  - **Architectural Decisions:** AD-TG-010
  - **Design reference:** §6.1 (Verifier Skill), §2.2 (verification-governance.md — related), §5.3 (Verify Phase Integration)
  - **Files:**
    - `.cursor/skills/verifier/SKILL.md` (refine)
    - `.cline/skills/verifier/SKILL.md` (refine — identical content)
  - **Depends on:** `TASK-001` (references `test-governance.md`)
  - **Expected test suite(s):** None (governance-only)
  - **Tests or checks:** Diff review of both projections confirming identical architectural criteria content; content review confirming skill structure, gate selection workflow, and completion policy unchanged
  - **Done when:**
    - **Both** Cursor and Cline projections are refined with identical content.
    - § Review Sensor Selection: under `test-strategy.md`, architectural criteria added per Design §6.1 (architectural ownership, boundary consistency, progressive evolution, architectural completeness, behavior-oriented naming, scenario coverage).
    - § Gate Selection: under "Test strategy review" gate category, note added that this gate evaluates architectural criteria from `test-governance.md`.
    - Verifier skill overall structure, gate selection workflow, review sensor selection mechanism, residual risk classification, completion policy, output format, and anti-patterns remain unchanged.

### Workstream 4 — Refine Supporting Skills and Review Prompts

- [ ] `TASK-005` — Refine `sql-migration-workflow` skill to derive repository test scenarios from SDD architectural decisions
  - **Requirements:** `REQ-011`, `REQ-023`
  - **Architectural Decisions:** AD-TG-011
  - **Design reference:** §6.2 (sql-migration-workflow Skill)
  - **Files:**
    - `.cursor/skills/sql-migration-workflow/SKILL.md` (refine)
    - `.cline/skills/sql-migration-workflow/SKILL.md` (refine — identical content)
  - **Depends on:** `TASK-001` (references `test-governance.md`)
  - **Expected test suite(s):** None (governance-only)
  - **Tests or checks:** Diff review of both projections confirming identical content; content review confirming skill structure, migration guidance, credential management, documentation routing unchanged
  - **Done when:**
    - **Both** Cursor and Cline projections are refined with identical content.
    - § Repeatable Backend Stabilization Pattern, step 5 (Tests): added reference to `test-governance.md` for Repository Test ownership and scenario derivation per Design §6.2.
    - § Preparation: added instruction to load `test-governance.md` when task involves test creation, and to derive repository test scenarios from SDD `design.md` architectural decisions per Design §6.2.
    - Skill overall structure, migration execution guidance, credential management, documentation routing, and output format remain unchanged.

- [ ] `TASK-006` — Refine `review-prompts/test-strategy.md` to add architectural evaluation criteria
  - **Requirements:** `REQ-009`, `REQ-022`
  - **Architectural Decisions:** AD-TG-010, AD-TG-012, AD-TG-014
  - **Design reference:** §2.4 (review-prompts/test-strategy.md)
  - **Files:**
    - `Documentation/AI-Harness/review-prompts/test-strategy.md` (refine)
  - **Depends on:** `TASK-001` (references `test-governance.md`)
  - **Expected test suite(s):** None (governance-only)
  - **Tests or checks:** Diff review confirming 5 architectural items added per Design §2.4; content review confirming existing items (coverage, naming, synthetic data, integration tests, SQL migration-specific debt) preserved; vocabulary alignment check against `test-governance.md` scenario vocabulary
  - **Done when:**
    - Five architectural evaluation items added: architectural ownership, boundary consistency, progressive evolution, architectural completeness, scenario vocabulary alignment (per Design §2.4 items 1–5).
    - Review prompt references `test-governance.md` as the evaluation criteria source (does not embed criteria).
    - Existing items (coverage, naming, synthetic data, integration tests, SQL migration-specific debt) are preserved.
    - Vocabulary aligned with `test-governance.md` scenario classification.

### Workstream 5 — Refine SDD Templates

- [ ] `TASK-007` — Refine `design.md` template to add Architectural Capabilities identification and decision-to-scenario mapping
  - **Requirements:** `REQ-010`, `REQ-025`, `NFR-006`
  - **Architectural Decisions:** AD-TG-011
  - **Design reference:** §7.1 (design.md Template)
  - **Files:**
    - `Documentation/AI-Harness/template/sdd/design.md` (refine)
  - **Depends on:** `TASK-001` (references `test-governance.md`)
  - **Expected test suite(s):** None (governance-only)
  - **Tests or checks:** Diff review confirming new subsections added per Design §7.1; content review confirming existing Testing Approach table, Optional Test Debt section, Fixture Chain section unchanged
  - **Done when:**
    - New subsection "Architectural Capabilities And Expected Test Suites" added after the Testing Approach table, following the pattern of the existing Fixture Chain subsection (optional, used when relevant). Includes: table mapping Architectural Capability → expected test suite → rationale, and table for capabilities NOT expected with justification (per Design §7.1).
    - New subsection "Architectural Decision To Test Scenario Mapping" added, mapping architectural decisions to expected repository test scenarios (satisfies AD-TG-011).
    - Existing Testing Approach table, Optional Test Debt section, Fixture Chain section, and all other template sections remain unchanged.

- [ ] `TASK-008` — Refine `tasks.md` template to add architectural completeness expectations
  - **Requirements:** `REQ-010`, `REQ-024`, `NFR-006`
  - **Architectural Decisions:** AD-TG-010
  - **Design reference:** §7.2 (tasks.md Template)
  - **Files:**
    - `Documentation/AI-Harness/template/sdd/tasks.md` (refine)
  - **Depends on:** `TASK-001` (references `test-governance.md`)
  - **Expected test suite(s):** None (governance-only)
  - **Tests or checks:** Diff review confirming 3 refinements per Design §7.2; content review confirming Execution Boundary, TASK/VP/DF categorization, Dependency Map, Requirement Traceability, Review Sensors, Commit Guidance, Execution Batching, and Completion Handoff unchanged
  - **Done when:**
    - **Refinement 1:** Verification Expectations table gains a new row "Architectural completeness" after "Test strategy review" (per Design §7.2 Refinement 1).
    - **Refinement 2:** Implementation Tasks section template updated: each TASK that introduces an architectural change includes "Expected test suite(s): Repository Tests | SQL Integration Tests | Controller Tests | None (no Architectural Capability affected)" as a standard field (per Design §7.2 Refinement 2).
    - **Refinement 3:** Known Risks And Skipped Checks table includes guidance that missing test suites must be listed with Architectural Capability and justification (per Design §7.2 Refinement 3).
    - Existing template structure (Execution Boundary, TASK/VP/DF categorization, Dependency Map, Requirement Traceability, Review Sensors, Commit Guidance, Execution Batching, Completion Handoff) remains unchanged.

- [ ] `TASK-009` — Refine `verification.md` template to add Architectural Completeness evaluation
  - **Requirements:** `REQ-010`, `REQ-026`, `NFR-006`
  - **Architectural Decisions:** AD-TG-010
  - **Design reference:** §7.3 (verification.md Template)
  - **Files:**
    - `Documentation/AI-Harness/template/verification/verification.md` (refine)
  - **Depends on:** `TASK-001` (references `test-governance.md`)
  - **Expected test suite(s):** None (governance-only)
  - **Tests or checks:** Diff review confirming new subsection added per Design §7.3; content review confirming Change Classification, Execute Handoff Notes, Gate Results, Runtime Validation, Review Sensors, Requirement Evidence, Residual Risk, Follow-up Needed, and Completion Decision unchanged
  - **Done when:**
    - New subsection "Architectural Completeness" added within the existing Test Coverage section (folded in, not a dedicated top-level section, per DDA-002 resolution and OQ-004).
    - Includes: table mapping Architectural Capability → expected test suite → present? → evidence or justification; missing suites justified with residual risk; architectural ownership validation statements (per Design §7.3).
    - All other template sections remain unchanged.

### Workstream 6 — Update Reference Documentation

- [ ] `TASK-010` — Update `documentation-index.md` to add `test-governance.md` reference
  - **Requirements:** `REQ-012`, `REQ-027`
  - **Architectural Decisions:** None (reference update)
  - **Design reference:** §2.5 (documentation-index.md)
  - **Files:**
    - `Documentation/AI-Harness/documentation-index.md` (update)
  - **Depends on:** `TASK-001` (references `test-governance.md`)
  - **Expected test suite(s):** None (governance-only)
  - **Tests or checks:** Cross-document reference check confirming `test-governance.md` appears in the Harness Design section
  - **Done when:**
    - `test-governance.md` added to the Harness Design section of the documentation index with path `Documentation/AI-Harness/Harness-Design/test-governance.md` and description per Design §2.5.

- [ ] `TASK-011` — Update `harness-architecture.md` to register `test-governance.md` as a Harness Design document
  - **Requirements:** `REQ-012`, `REQ-029`
  - **Architectural Decisions:** None (reference update)
  - **Design reference:** §2.3 (harness-architecture.md)
  - **Files:**
    - `Documentation/AI-Harness/Harness-Design/harness-architecture.md` (update)
  - **Depends on:** `TASK-001` (references `test-governance.md`)
  - **Expected test suite(s):** None (governance-only)
  - **Tests or checks:** Cross-document reference check confirming `test-governance.md` appears in Component Responsibilities table and Harness Assets table
  - **Done when:**
    - `test-governance.md` added to the Harness Assets table as a governance document per Design §2.3.
    - `test-governance.md` added to the Component Responsibilities table as a new row per Design §2.3 (with Responsibility, Authority Basis, Consumed By, Trigger for Update, and Notes columns matching the exact content from Design §2.3).

- [ ] `TASK-012` — Update `AGENTS.md` to add `test-governance.md` to harness references
  - **Requirements:** `REQ-012`, `REQ-028`
  - **Architectural Decisions:** None (reference update)
  - **Design reference:** §2.6 (AGENTS.md)
  - **Files:**
    - `AGENTS.md` (update)
  - **Depends on:** `TASK-001` (references `test-governance.md`)
  - **Expected test suite(s):** None (governance-only)
  - **Tests or checks:** Cross-document reference check confirming `test-governance.md` appears in the "Read First" table
  - **Done when:**
    - `test-governance.md` added to the "Read First" table as a harness navigation reference per Design §2.6.

- [ ] `TASK-013` — Update `Documentation/State.md` to reflect Test Governance Adoption lifecycle progression
  - **Requirements:** `REQ-013`, `REQ-030`
  - **Architectural Decisions:** None (operational update)
  - **Design reference:** §2.7 (Documentation/State.md)
  - **Files:**
    - `Documentation/State.md` (update)
  - **Depends on:** `TASK-001`, `TASK-002`, `TASK-003`, `TASK-004`, `TASK-005`, `TASK-006`, `TASK-007`, `TASK-008`, `TASK-009`, `TASK-010`, `TASK-011`, `TASK-012` (all other tasks — State reflects the complete result)
  - **Expected test suite(s):** None (governance-only)
  - **Tests or checks:** Content review confirming lifecycle progression updates
  - **Done when:**
    - SDD research status table row for `test-governance-adoption` updated to reflect Tasks phase completion.
    - Active Epic section updated to reflect Tasks phase complete; next phase is SDD Pre-Execution Review.
    - "Last updated" timestamp refreshed.
    - Recent decisions table updated to record Test Governance Adoption Tasks phase completion.

---

## Verify Preparation (VP)

No runtime validation is required — this is a governance-only feature with no product code, database, API, or test code changes. No VP tasks.

---

## Documentation Follow-Up Preparation (DF)

- [ ] `DF-001` — Identify Documentation Follow-Up targets for post-Verify routing
  - **Purpose:** Prepare the list of documents that Documentation Follow-Up must synchronize after Verify confirms acceptance. This is a governance feature — the created and refined documents themselves become follow-up targets.
  - **Depends on:** All TASK items
  - **Done when:**
    - Documentation Follow-up Candidates section in this tasks.md is populated with all affected documents.
    - `test-governance.md` identified as requiring routing in `documentation-update` skill awareness.
    - State.md update path confirmed (TASK-013).
    - No ADR creation required (confirmed by Design §16).

---

## Dependency Map

| Task | Depends on | Can run in parallel with | Notes |
|------|------------|--------------------------|-------|
| `TASK-001` | None | None (blocking prerequisite) | Creates the authoritative source all other tasks reference |
| `TASK-002` | `TASK-001` | `TASK-003`, `TASK-004`, `TASK-005`, `TASK-006`, `TASK-007`, `TASK-008`, `TASK-009`, `TASK-010`, `TASK-011`, `TASK-012` | References `test-governance.md`; Design §2.1 provides exact content |
| `TASK-003` | `TASK-001` | `TASK-002`, `TASK-004`, `TASK-005`, `TASK-006`, `TASK-007`, `TASK-008`, `TASK-009`, `TASK-010`, `TASK-011`, `TASK-012` | References `test-governance.md`; Design §2.2 provides exact content |
| `TASK-004` | `TASK-001` | `TASK-002`, `TASK-003`, `TASK-005`, `TASK-006`, `TASK-007`, `TASK-008`, `TASK-009`, `TASK-010`, `TASK-011`, `TASK-012` | References `test-governance.md`; both Cursor and Cline projections must be refined identically |
| `TASK-005` | `TASK-001` | `TASK-002`, `TASK-003`, `TASK-004`, `TASK-006`, `TASK-007`, `TASK-008`, `TASK-009`, `TASK-010`, `TASK-011`, `TASK-012` | References `test-governance.md`; both Cursor and Cline projections must be refined identically |
| `TASK-006` | `TASK-001` | `TASK-002`, `TASK-003`, `TASK-004`, `TASK-005`, `TASK-007`, `TASK-008`, `TASK-009`, `TASK-010`, `TASK-011`, `TASK-012` | References `test-governance.md`; Design §2.4 provides exact content |
| `TASK-007` | `TASK-001` | `TASK-002`, `TASK-003`, `TASK-004`, `TASK-005`, `TASK-006`, `TASK-008`, `TASK-009`, `TASK-010`, `TASK-011`, `TASK-012` | References `test-governance.md`; Design §7.1 provides exact content |
| `TASK-008` | `TASK-001` | `TASK-002`, `TASK-003`, `TASK-004`, `TASK-005`, `TASK-006`, `TASK-007`, `TASK-009`, `TASK-010`, `TASK-011`, `TASK-012` | References `test-governance.md`; Design §7.2 provides exact content |
| `TASK-009` | `TASK-001` | `TASK-002`, `TASK-003`, `TASK-004`, `TASK-005`, `TASK-006`, `TASK-007`, `TASK-008`, `TASK-010`, `TASK-011`, `TASK-012` | References `test-governance.md`; Design §7.3 provides exact content |
| `TASK-010` | `TASK-001` | `TASK-002`, `TASK-003`, `TASK-004`, `TASK-005`, `TASK-006`, `TASK-007`, `TASK-008`, `TASK-009`, `TASK-011`, `TASK-012` | References `test-governance.md`; Design §2.5 provides exact content |
| `TASK-011` | `TASK-001` | `TASK-002`, `TASK-003`, `TASK-004`, `TASK-005`, `TASK-006`, `TASK-007`, `TASK-008`, `TASK-009`, `TASK-010`, `TASK-012` | References `test-governance.md`; Design §2.3 provides exact content |
| `TASK-012` | `TASK-001` | `TASK-002`, `TASK-003`, `TASK-004`, `TASK-005`, `TASK-006`, `TASK-007`, `TASK-008`, `TASK-009`, `TASK-010`, `TASK-011` | References `test-governance.md`; Design §2.6 provides exact content |
| `TASK-013` | All TASK items | None (must be last) | State reflects the complete result of all prior tasks |
| `DF-001` | All TASK items | — | Preparation for Documentation Follow-Up; populated as tasks complete |

### Critical Path

```
TASK-001 → TASK-013 (State update)
```

All other tasks (TASK-002 through TASK-012) are parallel after TASK-001 and must complete before TASK-013.

### Parallel Execution Strategy

After TASK-001 creates `test-governance.md`, all remaining TASK items (TASK-002 through TASK-012) can execute in parallel. The Design provides exact content specifications for each refinement, eliminating the need for sequential cross-referencing. In practice, grouping tasks by workstream may reduce context switching:

1. **Batch 1:** `TASK-001` (blocking prerequisite — creates authoritative source)
2. **Batch 2 (parallel):** `TASK-002` + `TASK-003` + `TASK-004` + `TASK-005` + `TASK-006` (governance + skills + review prompt)
3. **Batch 3 (parallel, can overlap with Batch 2):** `TASK-007` + `TASK-008` + `TASK-009` (templates)
4. **Batch 4 (parallel, can overlap with Batch 2-3):** `TASK-010` + `TASK-011` + `TASK-012` (reference docs)
5. **Batch 5:** `TASK-013` (State update — must be last)
6. **Post-execution:** `DF-001` (populated from completed tasks)

---

## Requirement Traceability

| Requirement | Tasks | Tests or checks | Verification evidence |
|-------------|-------|-----------------|-----------------------|
| `REQ-001` | `TASK-001` | Content completeness review against Research §4, §5.2, §12 | `test-governance.md` content |
| `REQ-002` | `TASK-001` | AD-TG-001 through AD-TG-014 codified in document sections | `test-governance.md` content |
| `REQ-003` | `TASK-001` | Architecture-Oriented Testing Model, ownership model, scenario vocabulary, test design guidelines, progressive evolution triggers, verification lenses, architectural review questions documented | `test-governance.md` content |
| `REQ-004` | `TASK-001` | Boundary consistency review against Design §1.3 | `test-governance.md` scope boundaries table |
| `REQ-005` | `TASK-002` | Diff review confirming `sdd-operational.md` references `test-governance.md` | `sdd-operational.md` Testing Governance Model section |
| `REQ-006` | `TASK-002` | Diff review confirming Execute exit criteria includes architectural completeness | `sdd-operational.md` Execute exit criteria |
| `REQ-007` | `TASK-002` | Diff review confirming Design exit criteria includes capability identification | `sdd-operational.md` Design exit criteria |
| `REQ-008` | `TASK-003`, `TASK-004` | Diff review of `verification-governance.md` and verifier skill | Refined gate description + skill content |
| `REQ-009` | `TASK-006` | Diff review of `review-prompts/test-strategy.md` confirming architectural criteria additions | Refined review prompt |
| `REQ-010` | `TASK-007`, `TASK-008`, `TASK-009` | Diff review of 3 templates confirming architectural completeness additions | Refined templates |
| `REQ-011` | `TASK-005` | Diff review of `sql-migration-workflow` skill confirming test derivation reference | Refined skill (both projections) |
| `REQ-012` | `TASK-010`, `TASK-011`, `TASK-012` | Cross-document reference check confirming `test-governance.md` in all 3 documents | `documentation-index.md`, `harness-architecture.md`, `AGENTS.md` |
| `REQ-013` | `TASK-013` | Content review confirming lifecycle progression | `Documentation/State.md` |
| `REQ-014` | `TASK-001` | Content review confirming Future Evolution Capabilities with validation triggers | `test-governance.md` Future Evolution section |
| `REQ-015` | `TASK-002` | Diff review confirming reference to `test-governance.md` | `sdd-operational.md` Testing Governance Model |
| `REQ-016` | `TASK-002` | Diff review confirming Execute exit criteria | `sdd-operational.md` Execute exit criteria |
| `REQ-017` | `TASK-002` | Diff review confirming Design exit criteria | `sdd-operational.md` Design exit criteria |
| `REQ-018` | `TASK-002` | Content review confirming context acquisition recommendation | `sdd-operational.md` Testing Governance Model section |
| `REQ-019` | `TASK-002` | Content review confirming Design phase test scenario derivation requirement | `sdd-operational.md` Design exit criteria |
| `REQ-020` | `TASK-003` | Diff review confirming test strategy review gate scope expansion | `verification-governance.md` gate description |
| `REQ-021` | `TASK-004` | Diff review of verifier skill confirming architectural evaluation criteria (both projections) | Refined verifier skill (Cursor + Cline) |
| `REQ-022` | `TASK-006` | Diff review confirming 5 architectural items added | Refined `test-strategy.md` review prompt |
| `REQ-023` | `TASK-005` | Diff review confirming Repository Test ownership reference (both projections) | Refined `sql-migration-workflow` skill (Cursor + Cline) |
| `REQ-024` | `TASK-008` | Diff review confirming 3 refinements | Refined `tasks.md` template |
| `REQ-025` | `TASK-007` | Diff review confirming Architectural Capabilities section and decision-to-scenario mapping | Refined `design.md` template |
| `REQ-026` | `TASK-009` | Diff review confirming Architectural Completeness subsection | Refined `verification.md` template |
| `REQ-027` | `TASK-010` | Cross-document reference check | `documentation-index.md` |
| `REQ-028` | `TASK-012` | Cross-document reference check | `AGENTS.md` |
| `REQ-029` | `TASK-011` | Cross-document reference check | `harness-architecture.md` Component Responsibilities table |
| `REQ-030` | `TASK-013` | Content review confirming lifecycle progression | `Documentation/State.md` |
| `REQ-031` | All TASK items | Governance consistency review — no contradictory guidance across all refined documents | All refined documents |
| `NFR-001` | `TASK-001`, `TASK-002`, `TASK-003` | Authority hierarchy review | `test-governance.md`, `sdd-operational.md`, `verification-governance.md` |
| `NFR-002` | All TASK items | Diff review confirming only identified documents modified | All refined files vs Design §9 Affected Components |
| `NFR-003` | `TASK-001` | Content review confirming evolution triggers, not prescribed structures | `test-governance.md` Progressive Evolution section |
| `NFR-004` | None (no test code changes) | `dotnet build && dotnet test` — existing 79 tests pass | CLI output |
| `NFR-005` | `TASK-001` | Content review confirming capability model extensibility | `test-governance.md` Architecture-Oriented Testing Model |
| `NFR-006` | `TASK-001`, `TASK-002`, `TASK-003` | Document responsibilities table review | Design §3 — no overlap after refinements |
| `NFR-007` | `TASK-001` | Traceability review — every governance statement in `test-governance.md` traceable to AD-TG decisions and Engineering Review Reports | `test-governance.md` content vs Research §16 Evidence Traceability Matrix |
| `NFR-008` | `TASK-001` | Token-efficiency review — document concise enough for context loading | `test-governance.md` size and structure |

---

## Verification Expectations

The Verifier will select final gates. This section lists expected evidence from the implementation plan.

| Gate category | Expected / Not expected | Evidence or rationale |
|---------------|-------------------------|-----------------------|
| Build | Expected | Regression validation (`dotnet build`). This feature introduces no code changes. The build gate validates that documentation-only changes did not accidentally introduce repository regressions. It is not a functional acceptance criterion of the Test Governance feature itself. |
| Automated tests | Expected | Regression validation (`dotnet test`). Existing 79 tests must pass (AC-020, AC-021, NFR-004). This gate validates backward compatibility — documentation changes did not break existing test suites. It is not a functional acceptance criterion of the Test Governance feature itself. |
| SQL / Persistence | Not expected | No database schema, migration, or persistence behavior changes. |
| API | Not expected | No controller, DTO, route, or API contract changes. |
| UI | Not expected | No Blazor or frontend changes. |
| Security / PHI | Not expected | Governance-only — no patient data, clinical data, credentials, or PHI exposure. `security-phi` rule remains applicable as invariant. |
| Domain review | Not expected | No domain entities, aggregates, invariants, or business rules changed. |
| Documentation review | Expected | Content completeness review for `test-governance.md` against Research §4, §5.2, §12. Cross-document reference check for all refined documents. Governance consistency review — no contradictory guidance across Test Governance, SDD Operational, and Verification Governance. Confirmation that documents not identified for refinement remain unmodified (AC-018). |
| Test strategy review | Expected | Governance consistency review (not test suite evaluation — no test suites were created). Confirm that refined `test-strategy.md` review prompt, verifier skill, and `verification-governance.md` are aligned with `test-governance.md` architectural criteria. |
| Architectural completeness | Expected | All governance artifacts (documents, templates, skills, review prompts, reference documents) refined as specified in Design §2 and §6–§7. No product Architectural Capabilities affected — architectural completeness satisfied by design. |
| ADR evaluation | Not expected | Design §16 confirms no durable architecture decisions requiring ADR. Test Governance Adoption codifies architectural decisions (AD-TG-001 through AD-TG-014) already validated by implementation evidence. |
| Legacy characterization | Not expected | No Legacy behavior involved. |

---

## Review Sensors

- [ ] `Documentation/AI-Harness/review-prompts/check-docs.md` — governance consistency, path drift, cross-document references
- [ ] `Documentation/AI-Harness/review-prompts/test-strategy.md` — governance alignment with architectural criteria (refined in `TASK-006`)
- [ ] `Documentation/AI-Harness/review-prompts/security-phi-review.md` — skipped (no PHI/security impact per Design §14)
- [ ] `Documentation/AI-Harness/review-prompts/domain-review.md` — skipped (no domain changes)

---

## Known Risks And Skipped Checks

| Risk or skipped check | Reason | Owner / follow-up |
|-----------------------|--------|-------------------|
| Governance inconsistency after multiple refinements | 13 tasks modify 15+ files across governance documents, templates, skills, and review prompts. Risk of subtle contradictions despite Design §3 responsibility definitions. | Mitigation: governance consistency review as verification gate (Documentation review sensor). Authority hierarchy (`harness-architecture.md`) resolves any conflicts. |
| `test-governance.md` token consumption | Agents may avoid loading the document if it exceeds practical context budget. | Mitigation: Design §1.2 structure prioritized for conciseness (NFR-008). `sdd-operational.md` Testing Governance Model provides condensed summary as fallback. |
| Skill refinements diverge between Cursor and Cline projections | `TASK-004` and `TASK-005` each modify two files. If refined separately, projections may diverge. | Mitigation: refine both projections in the same Execute session with identical content per Design §15 risk table. Cross-projection diff check. |
| Template refinements create friction for existing SDD workflows | Agents using old mental model until new templates are internalized. | Mitigation: refinements are additive (new subsections, new rows) — existing template sections unchanged per Design §7. |
| `documentation-update` skill unaware of new document | `test-governance.md` becomes a routing target for Documentation Follow-Up. The `documentation-update` skill may not automatically recognize it. | Mitigation: `DF-001` identifies `test-governance.md` as a routing candidate. The skill's existing routing logic handles new Harness Design documents without modification per Design §6.3. If the skill's index needs explicit updating, that becomes Documentation Follow-Up scope. |
| Documents not identified for refinement inadvertently modified | Risk of accidental edits to files outside Design §9 Affected Components. | Mitigation: diff review during Verify confirms only identified files were modified (AC-018, NFR-002). |

---

## Documentation Follow-up Candidates

Documentation Update will decide final routing.

- **State:** `TASK-013` updates `Documentation/State.md` during Execute. Documentation Follow-Up confirms lifecycle progression is accurate and SDD research status table is synchronized.
- **ADR:** None required (Design §16). No ADR creation or revision.
- **Architecture docs:** `harness-architecture.md` updated in `TASK-011`. No further architecture doc changes expected.
- **Technical docs:** None affected.
- **Rules:** No Rules created or modified (Research §9.2).
- **Skills:** `verifier` skill (Cursor + Cline) refined in `TASK-004`. `sql-migration-workflow` skill (Cursor + Cline) refined in `TASK-005`. `documentation-update` skill awareness of `test-governance.md` as routing target confirmed or updated during Documentation Follow-Up.
- **Review prompts:** `test-strategy.md` refined in `TASK-006`.
- **Templates:** `design.md` refined in `TASK-007`. `tasks.md` refined in `TASK-008`. `verification.md` refined in `TASK-009`.
- **Active SDD:** `Documentation/SDD/test-governance-adoption/` — `research.md`, `specify.md`, `design.md`, and `tasks.md` are complete. `verification.md` produced during Verify phase.
- **New governance:** `test-governance.md` created in `TASK-001`. Documentation Follow-Up confirms it is registered in `documentation-index.md`, `AGENTS.md`, `harness-architecture.md`, and recognized by `documentation-update` skill.

---

## Commit Guidance

- Keep commits atomic and focused — each task or logical group of parallel tasks should be its own commit.
- Do not commit secrets, `.env` files, credentials, real patient data, or clinical data.
- Commit message format: `test-governance-adoption: <task-id> <description>` (e.g., `test-governance-adoption: TASK-001 create test-governance.md`).
- For skill refinements affecting both Cursor and Cline projections, commit both files together in one commit per skill per task.
- TASK-013 (State.md) should be the final commit of the Execute phase.

---

## Execution Batching

This is a Medium governance feature. Execute in small batches — implement a coherent subset of tasks, validate (build, test, diff review), commit, then continue.

Suggested batching aligned with workstreams and critical path:

1. **Batch 1 — Blocking prerequisite:** `TASK-001` (create `test-governance.md`). Validate: content completeness review, structure review. Commit.
2. **Batch 2 — Governance documents + review prompt:** `TASK-002` + `TASK-003` + `TASK-006` (can run in parallel or sequentially within batch). Validate: diff review of each file, cross-document references. Commit.
3. **Batch 3 — Skills (both projections):** `TASK-004` + `TASK-005` (4 files total — Cursor + Cline for each skill). Validate: diff review of all 4 files, cross-projection consistency check. Commit.
4. **Batch 4 — Templates:** `TASK-007` + `TASK-008` + `TASK-009`. Validate: diff review of each template, additive-refinement confirmation (existing sections unchanged). Commit.
5. **Batch 5 — Reference documents:** `TASK-010` + `TASK-011` + `TASK-012`. Validate: cross-document reference check. Commit.
6. **Batch 6 — State finalization:** `TASK-013`. Validate: content review. Commit.
7. **Batch 7 — Regression validation:** `dotnet build && dotnet test`. Confirm 79 tests pass. This is a verification step, not a TASK — it validates backward compatibility (AC-020, AC-021, NFR-004).
8. **Post-execution:** `DF-001` populated; Verify handoff prepared.

See `sdd-operational.md` § Implementation Batching Guidance and § Context Acquisition Governance for Execute entry context requirements.

---

## Completion Handoff

Before marking tasks complete, provide the Verifier with:

- Implemented task list (TASK-001 through TASK-013).
- Requirement traceability status (all 31 REQs mapped to completed tasks).
- Tests/checks run: `dotnet build` (regression validation — no repository regressions from documentation changes), `dotnet test` (regression validation — 79 tests passing, backward compatibility confirmed), content completeness review of `test-governance.md`, diff review of all refined documents, cross-document reference verification.
- Tests/checks skipped with reasons: SQL/Persistence (no database changes), API (no API changes), UI (no UI changes), Security/PHI (governance-only), Domain review (no domain changes), Legacy characterization (no Legacy behavior involved).
- Review sensors applied: `check-docs.md`, `test-strategy.md` (governance alignment). Skipped: `security-phi-review.md`, `domain-review.md`.
- Residual risks: governance inconsistency risk (mitigated by consistency review gate), `test-governance.md` token consumption risk (mitigated by condensed summary in `sdd-operational.md`), skill projection divergence risk (mitigated by identical-content refinement in same session), template friction risk (mitigated by additive-only refinements), `documentation-update` skill awareness risk (mitigated by DF-001 candidate identification).
- Documentation follow-up candidates: per DF-001 populated list.

### Task Checkbox Ownership

| Artifact | Execute | Verify | Documentation Follow-Up |
|----------|---------|--------|-------------------------|
| `TASK-*` implementation | Marks work done in handoff; may check boxes when implementation complete | Validates acceptance against requirements | Does **not** own implementation status |
| `VP-*` | None for this feature | — | — |
| `DF-001` | Identifies candidates only | Identifies mandatory targets | Executes doc sync; may mark `DF-001` complete |
| `tasks.md` checkboxes | Execute may mark `TASK-*` complete when done | Verify confirms acceptance | May mark `DF-*` when workflow phases complete |

Do not treat Documentation Follow-Up as the default owner for marking `TASK-*` complete unless Execute never updated the file and Verify confirms acceptance.

---

## Architectural Completeness (Governance Feature Self-Assessment)

This section satisfies the architectural completeness checkpoint that this feature itself introduces (AD-TG-010). It demonstrates the pattern that future product features will follow.

| Architectural Capability | Expected for this feature? | Evidence or justification |
|--------------------------|---------------------------|---------------------------|
| Persistence Intent | Not expected | No persistence behavior changes — governance-only |
| Physical Persistence | Not expected | No schema, migration, or SQL behavior changes |
| API Contract | Not expected | No controller, DTO, or route changes |
| Business Invariants | Not expected (Future Evolution) | No domain behavior changes |
| Use Case Coordination | Not expected (Future Evolution) | No application services created |

**Architectural completeness satisfied by design:** All governance artifacts (documents, templates, skills, review prompts, reference documents) are refined as specified in Design §2, §6, and §7. The Verifier confirms completeness through cross-document reference checks and governance consistency review, not through test suite execution.

**Missing suites justified with residual risk:** Not applicable — no product Architectural Capabilities are affected. No test suites were expected.