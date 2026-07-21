## SDD Design Prompt v2
	### Session Type
	Design — define the technical solution, architecture boundaries, contracts, ownership, persistence model, risks, and execution strategy.
	Produces:
	```
Documentation/SDD/<FEATURE_SLUG>/design.md

```
	This session does not implement code, generate Tasks, perform reviews, or produce verification evidence.
	
	## Lifecycle
	```
Research
→ Plan (optional)
→ Specify
→ Design                  ← THIS SESSION
→ Tasks
→ SDD Pre-Execution Review
→ Execute (Batch Execution)
→ Verify
→ Documentation Follow-Up
→ Reporting
→ Teacher Guide

```
	Design converts approved requirements into an executable architecture.
	It is the last phase allowed to make technical decisions.
	## Session Objective
	Produce a Design document that allows Tasks and Execute to proceed without introducing new architectural decisions.
	Every significant implementation choice should already be justified here.
	## Session Guardrails
	### Never
	- implement production code
	- modify project files
	- generate EF migrations
	- write tasks.md
	- produce verification.md
	- perform Execute
	- perform SDD Review
	- update State.md
	- update PM
	- redefine requirements
	- introduce new product scope
	### Always
	- write design.md from template
	- map every REQ
	- resolve technical ambiguities
	- define contracts
	- define ownership
	- document persistence impacts
	- define testing strategy
	- evaluate ADR necessity
	- identify execution batches
	- prepare Tasks handoff
	## Entry Criteria
	Continue only if:
	- Specify is complete
	- all REQs are accepted
	- Acceptance Criteria exist
	- Out Of Scope exists
	- assumptions documented
	- sizing determined
	- blockers resolved or explicitly deferred
	Otherwise stop.
	## Mandatory Context
	Always load
	- specify.md
	- design template
	- State.md
	- sdd-operational.md
	Load when required
	- research.md
	- prerequisite SDDs
	- ADRs
	- architecture docs
	- migration docs
	- domain docs
	- harness governance
	## Authority Order
	1. Accepted ADRs
	2. specify.md
	3. State.md
	4. Architecture
	5. research.md
	6. Legacy
	Research never overrides Specify.
	## Design Responsibilities
	Design owns:
	- architecture
	- ownership
	- contracts
	- persistence
	- API
	- security
	- PHI decisions
	- testing architecture
	- runtime validation strategy
	- execution architecture
	- risks
	- residual risks
	Design does not own:
	- implementation order
	- implementation details
	- Tasks
	- verification
	- reports
	## Outcome-Oriented Rule
	Describe:
	- behaviors
	- contracts
	- constraints
	- persistence outcomes
	- ownership
	Avoid prescribing:
	- concrete methods
	- factories
	- mapping implementation
	- repository internals
	unless required by an ADR.
	## Reuse First Rule
	Before introducing anything new:
	1. inspect previous SDDs
	2. inspect existing patterns
	3. inspect existing repositories
	4. inspect existing DTOs
	5. inspect existing tests
	Document why reuse is or is not possible.
	## Batch Awareness
	Design should anticipate Execute.
	Whenever possible identify natural execution batches.
	Typical batches:
	- Domain
	- Persistence
	- Repository
	- API
	- DTO
	- Mapping
	- Tests
	Do not create Tasks.
	Only expose natural implementation boundaries.
	## Implementation Independence
	Design should avoid prescribing:
	- exact methods
	- exact files
	- exact code
	Instead describe what Execute must accomplish.
	Tasks and Execute retain implementation freedom.
	## API Contract Rule
	Every endpoint must define:
	- route
	- request
	- response
	- status codes
	- compatibility notes
	- validation behavior
	No API ambiguity should remain after Design.
	Anything unresolved becomes a Pre-Execution Review item.
	## Ownership Rule
	Every responsibility must belong to one owner.
	Example:
	Aggregate
	Repository
	Application
	Controller
	Infrastructure
	Never duplicate ownership.
	## Persistence Rule
	Whenever persistence changes:
	document
	- entity impact
	- DTO impact
	- EF impact
	- migration impact
	- SQL implications
	- query filters
	- indexes
	- constraints
	If no migration is expected, explicitly state why.
	## Testing Architecture
	Describe
	- unit testing
	- integration testing
	- runtime validation
	- fixture chain
	- prerequisites
	Do not enumerate Tasks.
	## Runtime Validation
	If runtime validation exists:
	Describe
	- environment
	- objective
	- expected behavior
	Verification owns evidence.
	## ADR Evaluation
	For every architectural decision:
	State
	- existing ADR reused
	- ADR candidate
	- rationale
	- whether Tasks are blocked
	Never approve ADRs.
	## Multi-Tool Harness Rule
	Design must remain tool-independent.
	Reference Harness Skills or Governance capabilities instead of Cursor/Cline implementations.
	Example
	Good
	> SQL Migration Workflow Skill
	Bad
	> Cursor SQL Skill
	## Traceability Rule
	Every REQ must have
	- design response
	- ownership
	- persistence impact (if any)
	- testing impact
	No orphan requirements.
	## Design Exit Criteria
	Before finishing verify:
	- all REQs mapped
	- ownership complete
	- contracts complete
	- persistence documented
	- testing strategy defined
	- runtime validation described
	- risks documented
	- ADR evaluation complete
	- execution batches identified
	- reusable patterns evaluated
	- no implementation script written
	- sufficient for Tasks without architectural guessing
	## Chat Summary
	Return only
	### Design Readiness
	- Specify version
	- REQs mapped
	- ADR summary
	### Batch Preview
	Natural execution batches identified.
	### Review Focus
	Items requiring Design Review.
	### Next Step
	Tasks.
	## Deliverable
	Primary
	```
Documentation/SDD/<FEATURE_SLUG>/design.md

```
	Secondary
	Brief Design summary.
	Do not implement code.
	Do not generate Tasks.
	Do not execute implementation.