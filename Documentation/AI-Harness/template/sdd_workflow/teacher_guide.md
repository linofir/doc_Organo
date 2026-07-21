## Teacher Guide Prompt v2
	### Workflow Phase
	Teacher Guide
	This session occurs after:
	- Research
	- SDD (Specify → Design → Tasks)
	- Pre-Execution Review
	- Execute
	- Verify
	- Documentation Follow-Up
	- Reporting
	This is typically the final artifact of the feature lifecycle.
	Its purpose is knowledge transfer, not documentation synchronization.
	
	## Phase Boundaries
	### In scope
	- Decide whether a Teacher Guide should be generated
	- Extract reusable knowledge from the verified implementation
	- Produce or update teacher-guide.md
	- Explain architectural concepts, responsibilities and design decisions implemented by the feature
	- Identify reusable patterns and recommended follow-up study topics
	- Perform a final quality review before completion
	### Out of scope
	- Explain production code line-by-line
	- Document implementation chronology
	- Duplicate SDD, Verification or Reporting
	- Modify production code
	- Re-run verification
	- Update project documentation
	- Create ADRs
	- Teach rejected or speculative designs
	The Teacher Guide explains why the implementation exists, how the solution is organized, and what should be learned—not how every method works.
	## Entry Criteria
	Proceed only when:
	- Execute completed successfully
	- Verification completed
	- Documentation Follow-Up completed
	- Reporting completed
	- Reporting recorded Teacher Guide = Generate, or the user explicitly requests generation
	If any prerequisite is missing, stop and report the missing dependency.
	## Feature Context
				Field
				Value
				Feature
				<FEATURE_NAME>
				SDD folder
				Documentation/SDD/<feature-slug>/
				SDD sizing
				Small / Medium / Large / Complex
				Feature Report
				Documentation/SDD/<feature-slug>/reports/feature-report.md
				Verification
				Documentation/SDD/<feature-slug>/verification.md
				Existing Guide
				teacher-guide.md (new/update)
				Reporting Decision
				Generate / Skip
	## Authority Hierarchy
				Topic
				Authority
				Teacher Guide role
				Implementation truth
				Production code
				Interpret
				Feature intent
				SDD
				Provide context
				Correctness
				Verification
				Reference
				Delivery summary
				Feature Report
				Reference only
				Operational truth
				State.md
				Link only
				Architecture decisions
				ADRs
				Explain impacts
				Technical references
				Technical documentation
				Link only
	The Teacher Guide never establishes project truth.
	It explains already verified implementation.
	## Objective
	Transform the verified implementation into a learning artifact that helps future development.
	The guide should explain:
	- which concepts were implemented
	- why those concepts exist
	- how they fit into the project architecture
	- how the feature should evolve
	- what the reader should study next
	It should avoid implementation mechanics unless necessary to explain a concept.
	## Writing Principles
	### 1. Concept Before Implementation
	Always explain the concept first.
	Only then explain how the implementation realizes that concept.
	Never start from classes or methods.
	### 2. Architecture Before Mechanics
	Focus on:
	- responsibilities
	- boundaries
	- collaboration
	- design decisions
	Avoid explaining code structure for its own sake.
	### 3. Teach Knowledge, Not Code
	The guide is not API documentation.
	Do not explain every class, method or property.
	Instead explain:
	- responsibilities
	- interactions
	- patterns
	- trade-offs
	- architectural intent
	### 4. Final-State Perspective
	Describe the final verified implementation.
	Never narrate development history, debugging sessions or implementation order.
	### 5. Learning-Oriented
	Assume the reader is a junior or early-intermediate developer familiar with C#/.NET fundamentals who wants to understand the project architecture through real implementations.
	Each section should naturally answer:
	- What problem does this solve?
	- Why is this solution appropriate?
	- What concepts should I understand first?
	- What should I study next?
	### 6. Reference Canonical Artifacts
	Link authoritative artifacts.
	Do not duplicate:
	- Verification tables
	- Requirement tables
	- SDD content
	- Reporting summaries
	### 7. Code Is the Source of Truth
	When documentation conflicts with production code:
	Production code wins.
	## Stage 1 — Eligibility
	Generate a Teacher Guide when one or more apply:
	- first implementation of a reusable architectural pattern
	- SDD feature crossing multiple architectural layers
	- SQL migration vertical
	- feature introducing significant domain knowledge
	- explicit user request
	Skip when:
	- trivial bug fix
	- documentation-only work
	- repetitive implementation with no new concepts
	Decision:
	- Generate
	- Skip
	Provide rationale.
	## Stage 2 — Knowledge Inventory
	Before writing the guide, identify the knowledge contained in the feature.
	Extract:
	- architectural concepts
	- domain concepts
	- persistence concepts
	- API concepts
	- testing concepts
	- reusable patterns
	- important design decisions
	- common pitfalls
	- recommended follow-up topics
	This inventory becomes the foundation of the guide.
	Do not begin writing before completing the inventory.
	## Stage 3 — Knowledge Sources
	Use the following sources, in order:
	1. Production code (primary)
	2. Verification evidence
	3. Tests
	4. Active SDD
	5. ADRs
	6. Technical documentation
	Treat Reporting only as contextual support.
	## Stage 4 — Reusability Assessment
	For every identified concept determine whether it is:
	- Feature-specific
	- Reusable
	- Architectural
	- Framework-specific
	Provide detailed explanations only for reusable knowledge.
	Feature-specific implementation details should remain concise.
	## Stage 5 — Guide Generation
	Generate the Teacher Guide from the validated knowledge inventory.
	Prioritize:
	- concepts
	- architecture
	- responsibilities
	- design rationale
	- extension points
	- common mistakes
	- study recommendations
	Avoid:
	- exhaustive code walkthroughs
	- implementation chronology
	- verification summaries
	- documentation history
	## Validation Checks
	Before completion verify:
	- No implementation chronology
	- No line-by-line code explanation
	- No duplicated Verification content
	- No duplicated Reporting content
	- Concepts presented before implementation
	- Architectural responsibilities clearly explained
	- Reusable knowledge prioritized
	- Canonical artifacts referenced instead of copied
	## Deliverables
	### Knowledge Inventory
	Summarize the concepts extracted from the feature.
	### Teacher Guide
	Generate or update:
	Documentation/SDD/<feature-slug>/teacher-guide.md
	### Completion Decision
	Teacher Guide:
	- Complete
	- Complete with deferred topics
	- Skipped (with rationale)
	- Blocked
	Record the reason.
	## Exit Criteria
	The Teacher Guide is complete when:
	- Eligibility decision recorded
	- Knowledge inventory completed
	- Reusable concepts extracted
	- Teacher Guide generated or updated
	- Validation checks passed
	- No authority violations
	- No duplicated project documentation