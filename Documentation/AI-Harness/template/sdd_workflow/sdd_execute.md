# SDD Execute Prompt v4

## 1. Workflow Context

**Lifecycle**

Research
→ Specify
→ Design
→ Tasks
→ SDD Review
→ Pre-Execution Review
→ **Execute**
→ Verify
→ Documentation Follow-Up
→ Reporting
→ Teacher Guide
→ Governance Improvement Plan

Current phase: **Execute**

This is an implementation phase.

Execution starts only after the approved SDD package and Pre-Execution Review.

---

# 2. Objective

Implement the approved feature by reconstructing the implementation context from canonical project artifacts.

Execution produces implementation and objective evidence.

Execution never certifies correctness.

Verification remains an independent phase.

---

# 3. Entry Criteria

Proceed only when:

- Approved SDD package exists.
- Pre-Execution Review completed.
- Feature is ready for implementation.
- No blocking operational issue exists in State.md.

If prerequisites are not satisfied:

STOP.

---

# 4. Load Canonical Artifacts

Always load:

- Documentation/State.md
- specify.md
- design.md
- tasks.md (when applicable)
- Pre-Execution Review
- Relevant ADRs
- AGENTS.md

Load additional documentation only when required by the current task.

Never reconstruct context from previous chat sessions.

---

# 5. Reconstruct Implementation Context

Before implementing:

Reconstruct:

- approved scope
- requirements
- implementation design
- ownership boundaries
- accepted ADRs
- operational truth
- verification expectations
- active architectural capabilities
- batching strategy
- previous governance findings (when applicable)

If reconstruction is impossible from project artifacts:

STOP.

Route the issue as a governance problem.

---

# 6. Execution Responsibilities

Execute is responsible for:

- implementing approved tasks
- respecting architectural ownership
- following batching
- producing implementation evidence
- recording discoveries
- preparing downstream phases

Execute is NOT responsible for:

- redesigning
- approving requirements
- declaring verification complete
- updating governance artifacts
- modifying operational documentation

---

# 7. Discovery Classification

Classify discoveries during implementation.

### Level 1

Local implementation.

Destination:

Implementation only.

### Level 2

Feature refinement.

Destination:

Documentation Follow-Up.

### Level 3

Governance or workflow improvement.

Destination:

Governance Improvement Plan.

---

# 8. Validation During Execute

Execute validates implementation to produce evidence.

Typical evidence:

- Build
- Unit Tests
- Integration Tests
- Runtime observations
- SQL validation
- API smoke
- DI validation
- Domain invariant validation

Execute records evidence.

Verify evaluates evidence.

---

# 9. Phase Boundaries

Execute never:

- expands scope
- silently redesigns
- changes architectural ownership
- bypasses ADRs
- modifies governance
- certifies correctness

When uncertainty affects implementation:

STOP.

Document the issue.

---

# 10. Stop Conditions

Stop execution when:

- governance conflict
- ownership conflict
- missing prerequisite
- design ambiguity
- architectural contradiction
- ADR candidate
- scope expansion
- unrecoverable validation failure

Document:

- issue
- affected artifacts
- evidence
- options
- required decision

---

# 11. Prepare Downstream Phases

Prepare objective inputs for:

## Verify

Implementation evidence only.

No conclusions.

---

## Documentation Follow-Up

Documentation candidates.

No documentation updates.

---

## Reporting

Lessons learned.

Feature summary inputs.

Session continuity inputs.

---

## Teacher Guide

Knowledge extraction candidates.

Learning opportunities.

---

## Governance Improvement Plan

Workflow findings.

Template findings.

Skill findings.

Governance findings.

Operational findings.

Do not propose governance changes during Execute.

Only record findings.

---

# 12. Deliverables

Execution completes with:

## Implementation

- modified files
- completed tasks
- implementation notes

## Evidence

- build
- tests
- runtime observations
- skipped validations

## Discovery Log

- Level 1
- Level 2
- Level 3

## Reconstruction Inputs

Prepared inputs for:

- Verify
- Documentation Follow-Up
- Reporting
- Teacher Guide
- Governance Improvement Plan

---

# 13. References

When deeper guidance is required, follow:

- SDD Operational Governance
- Verification Governance
- Documentation Follow-Up Governance
- Reporting Strategy
- Teacher Guide Strategy
- ADR Governance
- Architecture Documentation
- Domain Documentation

The prompt defines workflow orchestration.

Governance documents define execution methodology.

Canonical governance always takes precedence over prompt wording.

# 14. Execution Principle

Execution is session-independent.

Implementation context must always be reconstructed from canonical project artifacts.

Every downstream phase must be able to independently reconstruct the same implementation state using project artifacts rather than conversational history.

The Harness—not the chat session—is the source of execution continuity.