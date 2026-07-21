## SDD Verification Prompt v3
	### Session Type
	Verify — independent post-Execute validation.
	This session determines whether the implemented feature satisfies the approved SDD using implementation evidence.
	This session does not:
	- implement code
	- redesign architecture
	- redefine requirements
	- execute Documentation Follow-Up
	- generate reports unless explicitly requested
	Verify is an independent technical audit.
	## Lifecycle Context
	```
Research
→ Plan (optional)
→ Specify
→ Design
→ Tasks
→ SDD Pre-Execution Review
→ Execute
→ Verify ← THIS SESSION
→ Documentation Follow-Up
→ Reporting
→ Teacher Guide

```
	Verify validates the implementation against the approved SDD.
	Documentation Follow-Up updates documentation afterwards.
	Reporting summarizes the completed work afterwards.
	## Session Responsibilities
	Verify owns:
	- independent validation
	- requirement verification
	- implementation audit
	- evidence collection
	- gate evaluation
	- drift detection
	- residual risk assessment
	- documentation follow-up identification
	Verify does not own:
	- implementation
	- refactoring
	- documentation updates
	- report generation
	- architectural redesign
	## Mandatory Context Load
	Always load:
	- Documentation/State.md
	- Active SDD folder
		- research.md
		- specify.md
		- design.md
		- tasks.md
	- verification-governance.md
	- AGENTS.md
	Load when applicable:
	- accepted ADRs
	- migration-sql.md
	- prerequisite verified SDDs
	- PM item
	- review prompts selected by tasks.md
	#### 4. Feature Context
	- Feature: <FEATURE_NAME>
	- PM Reference: <PM_REFERENCE>
	- Feature SDD Folder: <SDD_FOLDER>
	- Approved Artifacts: specify.md, design.md, tasks.md
	- Approved Reviews: SDD Review, Pre-Execution Review
	- Feature Classification: Small / Medium / Large / Complex
	- Scope Variant (if applicable): Backend Stabilization / SQL Migration Vertical / Standard
	## Implementation Evidence Collection (Mandatory)
	Before evaluating any requirement or gate, Verify MUST inspect the implementation.
	Implementation evidence takes precedence over assumptions.
	Mandatory evidence collection:
	#### Production Code
	Inspect every production file referenced by tasks.md.
	Confirm implementation directly from source.
	Never assume implementation status.
	#### Test Code
	Inspect every new or modified test project.
	Identify:
	- implemented suites
	- missing suites
	- skipped suites
	- environment-dependent tests
	#### Runtime Evidence
	Collect available evidence:
	- dotnet build
	- dotnet test
	- SQL Integration
	- Swagger
	- Runtime validation notes
	If unavailable:
	Record Missing Evidence.
	Do not infer failure.
	#### Source of Truth
	Priority:
	1. Source Code
	2. Build/Test Outputs
	3. Runtime Evidence
	4. Execute Session Notes
	5. Git Diff (if available)
	If Execute notes disagree with source code:
	Source code wins.
	Record the inconsistency.
	#### Forbidden Conclusions
	Never write:
	- "Not Implemented"
	- "Repository not verified"
	- "Controller not implemented"
	unless the corresponding implementation files were inspected.
	Instead record:
	- inspected
	- partially implemented
	- implemented with deviations
	- unable to inspect (state why)
	## Evidence Inventory
	Before Gate Evaluation produce an inventory.
	Example:
				Evidence
				Status
				Source inspected
				Yes
				Build evidence
				Yes
				Tests inspected
				Yes
				Runtime evidence
				Partial
				SQL evidence
				Missing
				Execute notes
				Present
	## Change Classification
	Classify implementation:
	- Documentation
	- Domain
	- Persistence
	- API
	- UI
	- Security
	- Legacy
	- Cross-layer
	Multiple selections allowed.
	## Gate Selection
	Select gates according to:
	- SDD
	- classification
	- ADRs
	- implementation
	Do not execute irrelevant gates.
	## Gate Evaluation
	Typical gates:
	- Build
	- Automated Tests
	- SQL
	- API
	- Runtime
	- Security
	- Domain
	- Documentation
	- Test Strategy
	- ADR
	- Legacy
	Possible status:
	- Passed
	- Failed
	- Partially Passed
	- Skipped (environment)
	- Blocked
	Every gate requires evidence.
	## Requirement Verification
	Verify every REQ.
	Required traceability:
	```
REQ

↓

TASK

↓

Implementation

↓

Evidence

↓

Verification Result

```
	Possible result:
	- Verified
	- Partially Verified
	- Not Verified
	- Cannot Verify
	## Review Sensors
	Execute only the sensors required by:
	- change classification
	- tasks.md
	- governance
	Never execute every sensor by default.
	## Drift Analysis
	Evaluate independently:
	- Requirement Drift
	- Design Drift
	- Task Drift
	- Implementation Drift
	- Documentation Drift
	- Governance Drift
	Record evidence for every finding.
	## Residual Risks
	Classify:
	- Accepted
	- Requires User Decision
	- Blocks Completion
	Every residual risk must reference evidence.
	## Documentation Follow-Up Candidates
	Identify only.
	Do not update documentation.
	Possible targets:
	- State
	- ADR
	- Technical Docs
	- PM
	- Templates
	- Rules
	- Skills
	- Architecture
	- Review Prompts
	## Executive Summary
	Produce a concise audit summary.
	Example:
				Item
				Result
				REQs Verified
				13 / 13
				Gates Passed
				9 / 10
				Critical Findings
				0
				Review Sensors
				3
				Residual Risks
				2
				Documentation Candidates
				4
	## Completion Decision
	Possible decisions:
	- Approved
	- Approved with Residual Risk
	- Partially Verified
	- Blocked
	- Cannot Verify
	Every decision must include:
	- supporting evidence
	- blocking findings
	- residual risks
	- recommended next action
	
	### Artifact Production Rules
	#### Primary Deliverable (Mandatory)
	The primary output of this session is the file:
	```
Documentation/SDD/<FEATURE_SLUG>/verification.md

```
	This artifact is mandatory.
	Do not stop after producing an analysis in chat.
	All verification findings, evidence tables, gate results, requirement traceability, residual risks, review sensor results, and final verification status must be written into verification.md.
	The chat response is only a concise summary of the generated artifact.
	#### Chat Output Policy
	After writing verification.md, return only:
	- verification outcome
	- overall status
	- failed gates (if any)
	- major risks
	- next workflow phase
	Do not duplicate the complete report in chat.
	The authoritative record is always verification.md.
	## Verify Exit Criteria
	Before finishing the session verify that verification.md contains at least:
	- Production implementation inspected
	- Test implementation inspected
	- Runtime evidence evaluated
	- Every REQ verified
	- Applicable gates evaluated
	- Review sensors executed
	- Drift analysis completed
	- Residual risks documented
	- Documentation Follow-Up candidates identified
	- Independent completion decision produced
	Do not implement code.
	Do not update documentation.
	Do not generate reports.