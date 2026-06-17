# Teacher Guide — Quality Gate

Run before declaring `teacher-guide.md` complete. Governance source: `Documentation/AI-Harness/Harness-Design/knowledge-strategy.md`

## Prerequisites

- [ ] Execute complete — implementation merged or ready for merge
- [ ] `verification.md` exists with completion decision
- [ ] Documentation Follow-Up complete for the feature
- [ ] Reporting artifacts exist when reporting policy applies

## Mandatory Sections

- [ ] How This Guide Differs From Other Artifacts (boundary table)
- [ ] Feature Overview
- [ ] What Changed
- [ ] Architecture Walkthrough
- [ ] Concept Inventory (all concept card fields)
- [ ] Deep Dive By Component
- [ ] Testing Walkthrough
- [ ] Security Concepts
- [ ] Knowledge Map (≥2 dependency chains)
- [ ] Study Guide (Beginner / Intermediate / Advanced)
- [ ] Common Pitfalls (≥3 feature-specific entries)
- [ ] Feature Evolution History (material changes documented)
- [ ] Suggested Next Feature
- [ ] Quick Reference — Code Reading Order (sequence + rationale + dependency progression)
- [ ] Related Artifacts (canonical links)

## Concept Quality

- [ ] Every concept has production code or accepted ADR evidence
- [ ] No planned-but-unimplemented concepts presented as taught
- [ ] No generic language-syntax concepts without design significance
- [ ] ADR-owned decisions linked, not fully re-taught
- [ ] Inventory volume ~8–15 major concepts (flag if >20)

## Pitfalls Quality

- [ ] At least 3 pitfalls
- [ ] Each has what breaks / why / how to avoid
- [ ] Grounded in this feature's implementation, tests, or verification lessons
- [ ] Not verification gate tables or generic best-practice lists

## Study And Reading Paths

- [ ] Every Study Guide topic traceable to implementation location
- [ ] No generic curricula without feature ties
- [ ] Code reading order includes rationale and dependency progression

## Authority Boundaries

- [ ] Does not duplicate verification gate results or completion decisions
- [ ] Does not duplicate feature-report delivery summaries
- [ ] Does not duplicate session-handoff next steps
- [ ] Does not state State.md or PM runtime/backlog status
- [ ] Does not replace ADR text or full API specification tables
- [ ] Does not paste large code blocks
- [ ] Related Artifacts section links canonical sources

## Implementation Fidelity

- [ ] Guide reflects merged production code, not abandoned design
- [ ] Feature Evolution History sourced from SDD/design, diff, verification — not chat alone
- [ ] Conflicts with code would be stale — reviewer should confirm code wins

## Delivery

- [ ] Written in English
- [ ] Path: `Documentation/SDD/<feature-slug>/teacher-guide.md`
- [ ] Human or senior agent review recommended for concept accuracy

## Reject If

Any mandatory section missing, concepts lack evidence, pitfalls < 3 or generic, evolution history absent for material changes, study topics untraceable, or governance boundaries violated.

Fix and re-run gate before delivery.
