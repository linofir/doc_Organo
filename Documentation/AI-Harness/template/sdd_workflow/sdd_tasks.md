## SDD Tasks Prompt v2
	### Session Type
	Tasks — execution strategy and implementation planning.
	This session transforms the approved Specification and Design into an optimized execution strategy.
	The output is a complete tasks.md.
	This session does not perform implementation, verification, documentation updates, or execution readiness approval.
	
	## Lifecycle Context
	```
Research
→ Plan (optional)
→ Specify
→ Design
→ Tasks                ← THIS SESSION
→ SDD Pre-Execution Review
→ Execute
→ Verify
→ Documentation Follow-Up
→ Reporting
→ Teacher Guide

```
	Tasks is the final planning artifact before implementation.
	Its objective is to minimize execution uncertainty.
	## Tasks Responsibility
	Tasks owns execution planning.
	It does not own:
	- requirements
	- architecture
	- implementation
	- verification
	- documentation updates
	Its responsibility is to organize implementation into an efficient execution strategy.
	#### Feature Context
				Field
				Value
				Feature
				<FEATURE_NAME>
				Feature slug
				<FEATURE_SLUG>
				PM reference
				<PM_ITEM>
				Target folder
				Documentation/SDD/<FEATURE_SLUG>/
				Input artifacts
				specify.md, design.md
				Output file
				Documentation/SDD/<FEATURE_SLUG>/tasks.md
				Template source
				Documentation/AI-Harness/template/sdd/tasks.md
	## Entry Criteria
	Before starting:
	- specify.md approved
	- design.md approved (or explicitly skipped)
	- all Design decisions resolved
	- no unresolved architectural ambiguity
	- implementation scope frozen
	- sizing confirms Tasks is required
	If any prerequisite is missing, stop.
	## Mandatory Context
	Always load:
	- specify.md
	- design.md
	- tasks.md template
	- State.md
	Load when required:
	- prerequisite SDDs
	- referenced ADRs
	- SDD Operational Governance
	- relevant Harness Skills
	Research is context only.
	## Authority Hierarchy
	1. Accepted ADRs
	2. specify.md
	3. design.md
	4. State.md
	5. research.md
	Never introduce new scope.
	## Execution Strategy
	Before creating tasks, define:
	- implementation approach
	- execution sequence
	- architectural dependency flow
	- critical path
	- parallel opportunities
	Tasks exist to optimize execution.
	## Execution Waves
	Group implementation into logical waves.
	Example:
	Wave 1
	Domain
	Wave 2
	Persistence
	Wave 3
	Application/API
	Wave 4
	Validation
	Wave 5
	Documentation Follow-Up Preparation
	Waves organize execution.
	They are not tasks.
	## Execution Batches
	Within each Wave, organize work into Execution Batches.
	Each Batch should:
	- produce one coherent increment
	- minimize dependencies
	- minimize context switching
	- remain independently executable
	- finish in a compilable state whenever practical
	- expose natural verification checkpoints
	Tasks belong to a Batch.
	## Task Design Principles
	Every TASK should have:
	- one architectural responsibility
	- observable completion
	- explicit dependencies
	- embedded verification
	- low coupling
	- high cohesion
	- independent execution
	- minimal execution risk
	Avoid oversized tasks.
	## Task Decomposition Rules
	Prefer decomposition by capability.
	Avoid automatic decomposition by technical layer.
	Layer-first decomposition is acceptable only when it clearly reduces implementation risk.
	Every task should represent one meaningful implementation outcome.
	## TASK / VP / DF Governance
	TASK
	Execute ownership
	VP
	Verify preparation only
	DF
	Documentation Follow-Up preparation only
	Never mix responsibilities.
	## Execution Boundary
	Explicitly define:
	Included
	Excluded
	Deferred
	Future work
	Execute must never silently expand scope.
	## Requirement Traceability
	Every requirement must map to:
	```
Requirement

↓

Design Decision

↓

Execution Batch

↓

TASK

↓

Verification

↓

Expected Evidence

```
	No orphan requirements.
	No orphan tasks.
	## Dependency Analysis
	Document:
	- execution order
	- critical path
	- blocking tasks
	- parallel tasks
	- integration points
	Dependency graph must remain acyclic.
	## Runtime Validation
	When runtime validation is required by Design,
	create a dedicated Runtime Validation task.
	Its position is determined by the execution flow.
	Do not assume fixed numbering.
	## Verification Preparation
	Prepare Verify.
	Do not perform Verify.
	Document:
	- expected gates
	- review sensors
	- expected evidence
	- skipped checks
	- environment assumptions
	## Documentation Follow-Up Preparation
	Identify documentation candidates only.
	Never modify documentation during Tasks.
	## Risk Assessment
	For every Execution Batch identify:
	- implementation risk
	- architectural risk
	- integration risk
	For every TASK indicate:
	- Risk
	(Low / Medium / High)
	- Estimated Context
	(Small / Medium / Large)
	## Architectural Consistency Check
	Before finishing:
	confirm:
	- no new requirements
	- no architectural changes
	- no contract modifications
	- no hidden Design work
	- no Verify work
	- no Documentation Follow-Up work
	Tasks organizes execution.
	It never redesigns the solution.
	## Task Quality Review
	Before completing tasks.md evaluate:
	- decomposition quality
	- task granularity
	- dependency quality
	- execution flow
	- execution batches
	- requirement coverage
	- implementation independence
	- verification readiness
	- execution risk
	Revise decomposition until these criteria are satisfied.
	## Multi-Tool Governance
	The Harness is tool-independent.
	Never reference:
	- Cursor
	- Cline
	- editor-specific folders
	Reference only Harness capabilities.
	Examples:
	- SQL Migration Skill
	- Verification Skill
	- Runtime Validation Skill
	- Documentation Follow-Up Skill
	- AI Harness Governance
	Concrete implementations are resolved by the active tool.
	## Safety Valve
	If task decomposition reveals:
	- excessive task count
	- hidden architectural decisions
	- unresolved contracts
	- unexpected cross-context dependencies
	Stop.
	Escalate to Design or SDD Pre-Execution Review.
	Do not compensate by creating vague tasks.
	## Exit Criteria
	Tasks.md is complete only when:
	- Execution Strategy defined
	- Execution Waves defined
	- Execution Batches defined
	- Execution Boundary explicit
	- TASK / VP / DF separated
	- Critical Path documented
	- Dependencies acyclic
	- Every REQ mapped
	- Every TASK independently executable
	- Verification preparation complete
	- Documentation Follow-Up candidates identified
	- Task Quality Review passed
	- No new scope introduced
	- Written entirely in English
	## Deliverables
	#### Primary
	Documentation/SDD/<FEATURE_SLUG>/tasks.md
	#### Secondary (chat summary)
	- Execution Strategy summary
	- Wave count
	- Batch count
	- TASK / VP / DF counts
	- Critical Path
	- Highest-risk Batch
	- Pre-Execution Review handoff
	- Recommended next action