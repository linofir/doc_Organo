## Documentation Follow-Up Prompt v4
	### Workflow Phase
	Phase: Documentation Follow-Up
	Occurs after:
	- Research
	- Specify
	- Design
	- Tasks
	- Pre-Execution Review
	- Execute
	- Verify
	Occurs before:
	- Reporting
	- Teacher Guide
	Purpose:
	Synchronize permanent project documentation with the verified implementation.
	Verification is the source of truth.
	
	## Phase Boundaries
	### In Scope
	- Execute documentation updates already identified during SDD and Verify
	- Synchronize project documentation from verified evidence
	- Synchronize active SDD artifacts
	- Validate documentation integrity
	- Prepare Reporting handoff
	### Out of Scope
	- Production code
	- New implementation
	- New requirements
	- Re-running Verify
	- Reclassifying residual risk
	- Harness governance evolution
	- Reporting generation
	- Teacher Guide generation
	## Entry Criteria
	Proceed only if:
	- Verify completed successfully
	- verification.md exists
	- Verification decision is recorded
	- Verify Follow-up section reviewed
	- DF-* items loaded
	- Documentation Candidates from Design + Tasks + Verify loaded
	If these inputs are missing, stop and report missing prerequisites.
	### Feature Context
	| Field | Value |
	|---|---|
	| Feature | \<FEATURE_NAME\> |
	| SDD folder | Documentation/SDD/\<feature-slug\>/ |
	| PM reference | \<PM_ITEM\> |
	| SDD sizing | Small / Medium / Large / Complex |
	| Verification artifact | Documentation/SDD/\<feature-slug\>/verification.md |
	| Verify completion decision | Complete / Not complete / Complete with accepted residual risk |
	| Residual risk (reference only) | Low / Medium / High — do not re-classify |
	| DF items | \<DF-001\>, … |
	## Authority Order
	1. Accepted ADRs
	2. State.md
	3. Verification.md
	4. Product Documentation
	5. Architecture Documentation
	6. Technical Documentation
	7. Active SDD
	8. Rules / Skills / Templates
	Planning artifacts never override verified implementation.
	## Required Reads
	- Documentation/State.md
	- verification.md
	- tasks.md
	- design.md
	- AGENTS.md
	- documentation-index.md
	Only load additional documents that are actual update candidates.
	## Documentation Principles
	### Evidence First
	Every documentation update must trace to:
	- verified implementation
	- verified behavior
	- verified architecture
	- verified operational state
	Never document assumptions.
	### Documentation Minimalism
	Do not edit documents because they exist.
	Edit only when verified evidence requires synchronization.
	Otherwise record:
	> No Update Needed
	with justification.
	### Durable Knowledge
	Permanent documentation stores durable knowledge.
	Feature-specific implementation remains inside the SDD.
	## Documentation Candidates
	This phase consumes—not discovers—documentation candidates.
	Use the candidates identified in:
	- Design
	- Tasks (DF section)
	- Verify
	For each candidate:
				Candidate
				Evidence
				Update Required?
	Then decide:
	- Update
	- No Update Needed
	- Deferred
	Only if verification reveals an unexpected documentation impact may a new candidate be introduced.
	## Mandatory Synchronization
	Execute every mandatory candidate.
	### Operational Truth
	Evaluate:
	State.md
	Update only if:
	- branch changed
	- active work changed
	- blockers changed
	- verification status changed
	- next feature changed
	Otherwise:
	No Update Needed.
	### Product Planning
	Evaluate:
	PM_DocOrgano.md
	Update only if verified delivery changed roadmap status.
	### Technical Documentation
	Evaluate:
	migration-sql.md
	runbook.md
	Update only if verified implementation changed durable technical knowledge.
	## Candidate Synchronization
	For every candidate from Verify:
	1. Locate evidence
	2. Evaluate
	3. Decide
	4. Synchronize
	Record:
	| Candidate | Decision | Reason |
	## Active SDD Synchronization
	Evaluate:
	- specify.md
	- design.md
	- tasks.md
	- verification.md
	Synchronize only if implementation or verification changed SDD truth.
	Never rewrite history.
	## Documentation Integrity Validation
	Run only on modified documents.
	### Path Drift
	Detect obsolete references.
	### Authority Validation
	Ensure:
	- ADRs not contradicted
	- State reflects operational truth
	- SDD reflects feature truth
	### Ownership Validation
	Ensure information belongs in the correct artifact.
	Reject:
	- operational state inside ADR
	- architecture inside State
	- feature knowledge outside SDD
	## Governance Findings
	Record only.
	Do not modify Harness governance.
	Example:
	| Finding | Candidate Owner |
	## Reporting Handoff
	Prepare:
	- synchronized documentation status
	- delivered requirements
	- deferred documentation
	- residual risks (reference only)
	- lessons learned candidates
	Do not generate reports.
	## Deliverables
	### Documentation Summary
	Reviewed:
	Updated:
	No Update Needed:
	Deferred:
	### Evidence Traceability
	| Document | Evidence | Verification Reference | Reason |
	### Mandatory Synchronization Result
	State.md
	PM_DocOrgano.md
	migration-sql.md
	runbook.md
	Updated / No Update Needed / N/A
	### Candidate Synchronization
	| Candidate | Decision | Reason |
	### Integrity Validation
	Path Drift
	Authority
	Ownership
	Results.
	### Deferred Documentation
	| Document | Reason | Owner |
	### Completion Decision
	- Complete
	- Complete with Deferred Items
	- Blocked
	## Exit Criteria
	Documentation Follow-Up is complete when:
	- All mandatory candidates processed.
	- Every candidate has an explicit decision.
	- Permanent documentation reflects verified implementation.
	- Active SDD synchronized where necessary.
	- Integrity validation completed.
	- Reporting handoff prepared.
	