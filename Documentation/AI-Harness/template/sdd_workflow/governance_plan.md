## Governance Improvement Plan v2
	### Workflow Phase
	Governance Improvement Plan
	This phase occurs after the feature lifecycle has completed.
	Typical order:
	```
Research
↓
Specify
↓
Design
↓
Tasks
↓
Pre-Execution Review
↓
Execute
↓
Verify
↓
Documentation Follow-Up
↓
Reporting
↓
Teacher Guide (optional)
↓
Governance Improvement Plan

```
	This phase improves the development process, not the implemented feature.
	
	## Objective
	Consolidate all workflow findings collected throughout the completed feature and transform them into actionable improvements for the AI Harness.
	The implemented feature is already considered complete.
	The purpose of this phase is to improve:
	- Governance
	- Workflow
	- Templates
	- Skills
	- Documentation
	- Review process
	- Operational practices
	for future SDD executions.
	## Phase Boundaries
	### In Scope
	- Consolidate workflow findings
	- Consolidate governance findings
	- Consolidate template improvements
	- Consolidate documentation improvements
	- Identify recurring workflow friction
	- Build Governance Improvement Backlog
	- Prioritize implementation waves
	- Produce implementation roadmap
	### Out of Scope
	- Modify production code
	- Re-open completed feature
	- Re-run verification
	- Rewrite feature reports
	- Rewrite documentation
	- Implement governance changes
	This phase plans governance improvements only.
	## Inputs
	Load every available artifact.
	Not every project contains every artifact.
	Only consume what exists.
	Possible inputs include:
	#### Core
	- Documentation/State.md
	- verification.md
	- feature-report.md
	#### Optional
	- teacher-guide.md
	- session-handoff.md
	- pilot reports
	- previous Governance Improvement Plans
	- SDD artifacts
	- execution reports
	- documentation updates
	Missing artifacts must simply be recorded.
	Do not infer information.
	## Feature Context
				Field
				Value
				Feature
				<FEATURE_NAME>
				Feature Slug
				<FEATURE_SLUG>
				PM Item
				SDD Size
				Small / Medium / Large / Complex
				Lifecycle Status
				Completed
				Previous Governance Plan
				Optional
				Current Plan Version
				v0.X
	## Evidence Inventory
	Before identifying improvements, build an inventory of available evidence.
				Artifact
				Exists
				Used
				Verification
				Yes / No
				Documentation Follow-Up
				Yes / No
				Feature Report
				Yes / No
				Teacher Guide
				Yes / No
				Session Handoff
				Yes / No
				Pilot Reports
				Yes / No
				Previous Governance Plan
				Yes / No
	Only findings backed by evidence should enter the backlog.
	## Workflow Evaluation
	Evaluate every workflow phase.
	Use
	- Strong
	- Adequate
	- Weak
	- Not Executed
	For every phase identify
	- what worked
	- what caused friction
	- candidate improvements
				Phase
				Rating
				Evidence
				Improvement Candidate
				Research
				Specify
				Design
				Tasks
				Pre-Execution Review
				Execute
				Verify
				Documentation Follow-Up
				Reporting
				Teacher Guide
	## Cross-Phase Findings
	Unlike the previous section, capture issues that span multiple phases.
	Examples
	- Evidence was produced but later ignored.
	- Context was recreated repeatedly.
	- Same information appeared in multiple artifacts.
	- Responsibilities were duplicated.
	- Phase boundaries became unclear.
	- Missing handoff information caused downstream failures.
	## Governance Evaluation
	Evaluate governance dimensions.
				Dimension
				Rating
				Improvement
				Authority Hierarchy
				Ownership
				Phase Boundaries
				Documentation Routing
				Evidence Strategy
				Review Process
				Operational Governance
				Consistency
	## Findings Registry
	Organize findings into categories.
	### Governance
	GOV-*
	### Workflow
	WF-*
	### Templates
	TPL-*
	### Skills
	SK-*
	### Documentation
	DOC-*
	### Reporting
	RP-*
	### Verification
	VER-*
	### Operational Process
	OP-*
	### Tooling
	TOOL-*
	### Experimental
	EXP-*
	Each finding should include
	- description
	- impact
	- evidence
	- recommendation
	- confidence
	## Pattern Analysis
	Look for recurring patterns rather than isolated problems.
	Examples
	- repeated missing evidence
	- repeated template ambiguity
	- recurring documentation drift
	- repeated boundary violations
	- recurring AI mistakes
	- recurring manual corrections
	Patterns should drive prioritization.
	## What Worked Well
	Capture practices that consistently produced good outcomes.
	These should be preserved.
	Avoid changing stable processes unnecessarily.
	## Maturity Assessment
	Score major workflow areas.
				Area
				Score
				Workflow
				/5
				Governance
				/5
				Verification
				/5
				Documentation
				/5
				Reporting
				/5
				Knowledge Transfer
				/5
				Overall Harness
				/5
	## Governance Backlog
	Every improvement becomes an actionable backlog item.
				ID
				Source Findings
				Improvement
				Target Artifact
				Priority
				Effort
				Status
	## Target Artifact Map
	Instead of organizing by finding type, organize by destination.
	Examples
	Harness Design
	Templates
	Skills
	Rules
	Documentation
	Review Prompts
	Scripts
	Runbooks
	Operational Docs
	This becomes the implementation checklist.
	## Implementation Waves
	### Wave 1
	Critical improvements
	Must exist before next feature.
	### Wave 2
	High value
	Low risk
	### Wave 3
	Experimental
	Needs additional validation.
	## Consistency Audit
	Before implementing any governance change verify
	- authority hierarchy preserved
	- ownership preserved
	- no duplicated responsibility
	- no conflicting templates
	- documentation indexes updated
	- cross references valid
	## Open Questions
	Capture unresolved ideas separately.
	Avoid promoting hypotheses into backlog items.
	## Recommended Starting Position
	Summarize what should change before beginning the next feature.
	Separate into
	#### Apply immediately
	...
	#### Keep unchanged
	...
	#### Continue observing
	...
	## Deliverables
	Produce
	- Governance Improvement Plan
	- Governance Improvement Backlog
	- Implementation Waves
	- Target Artifact Map
	- Maturity Assessment
	- Cross-Phase Findings
	- Pattern Analysis
	## Exit Criteria
	The phase is complete when
	- workflow evaluated
	- governance evaluated
	- evidence consolidated
	- findings classified
	- backlog prioritized
	- implementation waves defined
	- maturity assessed
	- next-feature recommendations documented