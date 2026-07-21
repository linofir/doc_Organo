## SDD Specify Prompt v2
	### Session Type
	Specify — define feature scope, business outcomes, requirements, contracts, assumptions, and acceptance criteria.
	Produces
	```
Documentation/SDD/<FEATURE_SLUG>/specify.md

```
	This session does not produce Design, Tasks, Review, Execute, or Verify artifacts.
	
	## Lifecycle
	```
Research
→ Plan (optional)
→ Specify                 ← THIS SESSION
→ Design
→ Tasks
→ SDD Pre-Execution Review
→ Execute
→ Verify
→ Documentation Follow-Up
→ Reporting
→ Teacher Guide

```
	Specify converts validated Research into an implementation-independent specification.
	## Session Objective
	Produce a specification that:
	- completely defines feature scope
	- eliminates business ambiguity
	- closes contractual decisions
	- enables Design without discovering new requirements
	## Session Guardrails
	### Never
	- implement code
	- modify production files
	- generate EF migrations
	- write design.md
	- write tasks.md
	- write verification.md
	- perform reviews
	- update State.md
	- redefine architecture
	- introduce implementation details
	### Always
	- write specify.md from template
	- consolidate Research conclusions
	- define REQs
	- define Acceptance Criteria
	- define assumptions
	- define constraints
	- define scope
	- define Out Of Scope
	- resolve business decisions
	- identify execution prerequisites
	- identify verification expectations
	## Entry Criteria
	Continue only if
	- Research is complete
	- Research contradictions resolved
	- feature sequencing approved
	- prerequisite SDDs identified
	- blockers documented
	Otherwise stop.
	## Mandatory Context
	Always
	- research.md
	- State.md
	- specify template
	- sdd-operational.md
	Load when required
	- Product docs
	- Architecture docs
	- ADRs
	- prerequisite SDDs
	- migration docs
	- domain documentation
	## Authority
	1. Accepted ADRs
	2. Approved Product Decisions
	3. State.md
	4. Research
	5. Legacy
	Research becomes authoritative after contradictions are resolved.
	## Responsibilities
	Specify owns
	- problem
	- scope
	- goals
	- REQs
	- acceptance criteria
	- assumptions
	- constraints
	- execution prerequisites
	- backend stabilization declaration
	- domain language
	- business rules
	- security expectations
	- PHI expectations
	- legacy decisions
	- sizing
	Specify does not own
	- architecture
	- implementation
	- task decomposition
	- technical solution
	- execution sequence
	## Research Consolidation Rule
	Research must already contain
	- evaluated alternatives
	- discarded options
	- decision rationale
	Specify must consume only the final conclusions.
	Do not reopen Research discussions.
	## Requirement Rule
	Every REQ must be
	- atomic
	- measurable
	- testable
	- implementation-independent
	Avoid
	- vague wording
	- hidden requirements
	- architectural prescriptions
	## Acceptance Criteria Rule
	Every REQ must have observable success criteria.
	Acceptance Criteria should describe
	observable system behavior
	—not implementation.
	Whenever possible include
	- expected outputs
	- expected persistence
	- expected API behavior
	- expected business state
	## Decision Closure Rule
	Every DQ must become exactly one of:
	Resolved
	Defaulted
	Escalated
	No silent ambiguity may remain.
	Design must never guess business intent.
	## Scope Rule
	Out Of Scope should be explicit.
	Avoid hidden exclusions.
	Whenever another SDD owns work,
	reference it explicitly.
	## Execution Readiness Rule
	Specify should prepare execution without defining implementation.
	Document
	- prerequisites
	- dependencies
	- external constraints
	- runtime assumptions
	Do not describe Tasks.
	## Batch Awareness
	When useful,
	identify natural implementation domains
	such as
	- Domain
	- Repository
	- Persistence
	- API
	- DTO
	- Tests
	Only as implementation boundaries.
	Never decompose work.
	## Backend Stabilization Rule
	When stabilizing an existing backend
	explicitly declare
	- frontend excluded
	- UI excluded
	- API compatibility expectations
	- SQL expectations
	## Legacy Rule
	When Legacy exists,
	record
	Preserve
	Adapt
	Abandon
	with rationale.
	Do not reproduce Legacy implementation.
	## Security Rule
	When PHI exists
	document
	- security expectations
	- logging restrictions
	- privacy expectations
	- verification expectations
	without prescribing implementation.
	## Reviewability Rule
	The resulting specify.md should be objectively reviewable.
	A reviewer should be able to answer:
	- Is scope complete?
	- Is every REQ measurable?
	- Are acceptance criteria objective?
	- Are business decisions closed?
	- Is Design able to start without discovering requirements?
	If any answer is "No",
	Specify is incomplete.
	## Multi-Tool Harness Rule
	Never reference Cursor or Cline.
	Reference Harness Skills, Governance, or Operational Phases only.
	## Traceability Rule
	Every REQ should naturally map later to
	Design
	Tasks
	Verification
	without introducing new requirements.
	## Exit Criteria
	Before finishing verify
	- Research fully consolidated
	- scope complete
	- Out Of Scope complete
	- assumptions documented
	- constraints documented
	- REQs complete
	- Acceptance Criteria complete
	- DQs closed
	- sizing assigned
	- execution prerequisites documented
	- verification expectations documented
	- Design handoff complete
	- implementation-independent language
	- artifact reviewable
	## Chat Summary
	Return
	### Specify Readiness
	Research consumed
	Blockers
	Sizing
	REQ count
	### Decision Summary
	Resolved DQs
	Defaulted DQs
	Escalated DQs
	### Design Handoff
	Main technical themes
	Potential ADR evaluations
	Natural implementation domains
	### Next Step
	Design
	## Deliverable
	Primary
	```
Documentation/SDD/<FEATURE_SLUG>/specify.md

```
	Secondary
	Short session summary.
	Do not produce Design.
	Do not produce Tasks.
	Do not implement cod-e-.