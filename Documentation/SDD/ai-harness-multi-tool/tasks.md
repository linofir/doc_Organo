# Tasks — AI Harness Multi-Tool Evaluation

> Feature SDD: `Documentation/SDD/ai-harness-multi-tool/`
> Generated SDD artifacts are written in English.
> **Phase:** Tasks
> **Lifecycle position:** Research → Specify → Design → **Tasks** → SDD Pre-Execution Review → Execute → Verify → Documentation Follow-Up → Reporting → Teacher Guide
> **Input from:** [Design](design.md), [Specify](specify.md)

---

## 1. Overview

### Purpose of the Implementation

Evolve the AI Harness from a single-tool implementation (Cursor) into a multi-tool platform whose governance is independent of the AI tool used to execute it. This is achieved by formalizing the three-layer ownership taxonomy — **Harness Assets**, **Tool Assets**, and **Shared Assets** — as the fundamental architectural boundary, preserving full Cursor backward compatibility while enabling first-class Cline support.

### Execution Strategy

The implementation follows **five phases**, each independently verifiable:

1. **Phase 1 — Architectural Foundation:** Establish governance structure and taxonomy through documentation generalization.
2. **Phase 2 — Multi-Tool Enablement:** Implement Cursor classification and Cline artifact creation.
3. **Phase 3 — Governance Consolidation:** Align documentation, skills, and governance workflows for multi-tool operation.
4. **Phase 4 — Verification Preparation:** Prepare validation procedures for the Verify phase.
5. **Phase 5 — Documentation Follow-Up Preparation:** Prepare ADR, State, and template update candidates.

### Implementation Philosophy

- **Additive, not transformative.** Cursor artifacts are not modified, moved, or removed. Cline artifacts are added alongside existing Cursor artifacts.
- **Content is canonical; format is tool-specific.** Rule content and skill content are authored once; each tool consumes them through its native format.
- **Governance docs reference tool-agnostic concepts by default.** Tool-specific paths appear in parenthetical disambiguation when implementation detail is needed.
- **Each phase is independently verifiable and reversible.** Rollback for Phase 2 is removal of `.clinerules/` and `.cline/` directories. Documentation changes are revertible via version control. ADR changes are reversible through ADR governance.

### Migration Approach

The migration proceeds through the **Transition Architecture** (Cursor artifacts as de facto canonical source) toward the **Target Architecture** (Harness Assets defined independently of tool implementations). The transition is additive — no existing artifact is removed or transformed.

---

## 2. Execution Boundary

**Included:**

- Governance documentation generalization (naming, path references, section titles)
- AGENTS.md updates for multi-tool clarity
- ADR-003 revision preparation and new ADR preparation (but not ADR drafting)
- Cline artifact creation: `.clinerules/*.md`, `.cline/skills/*/SKILL.md`, `.clineignore`
- Cursor artifact baseline capture and integrity verification
- Skill content updates: `documentation-update` path drift extension, `effective-harness-planning` multi-tool awareness
- Pilot report template updates for multi-tool fields
- Documentation update candidates identification (State, runbook, SDD templates)
- Verification preparation artifacts (VP items)
- Documentation follow-up preparation (DF items)

**Excluded:**

- Verification artifacts (`verification.md`) — produced in Verify phase
- Reporting artifacts (`feature-report.md`, `session-handoff.md`) — produced in Reporting phase
- Teacher Guide generation — produced after Reporting when warranted
- Product code changes (`DocAPI/`, `DocFront.Web/`) — governance-only feature
- ADR drafting — tasks prepare ADR candidates; ADR creation belongs to ADR governance
- Canonical format standardization — explicitly rejected by Design (DDA-003)
- Any clinical, database, or API changes

**Execution does not begin until SDD Pre-Execution Review exit criteria are satisfied.**

---

## 3. Workstreams and Tasks

### Phase 1 — Architectural Foundation

#### Workstream A — Governance Architecture

---

##### TASK-A01 — Formalize Harness Asset / Tool Asset / Shared Asset Taxonomy

- **Purpose:** Establish the three-layer ownership taxonomy as the fundamental architectural boundary. All subsequent tasks depend on this taxonomy being defined and documented.
- **Design Traceability:** DDA-001; AP-001; AP-002; REQ-003; REQ-005; Design §Architectural Layers, §Layer Definitions, §Ownership Model
- **Dependencies:** None (foundational task)
- **Deliverables:**
  - Harness Architecture document (`Documentation/AI-Harness/Harness-Design/harness-architecture.md`) updated with the three-layer taxonomy as a documented architectural concept
  - Taxonomy definitions (Harness Assets, Tool Assets, Shared Assets) with layer responsibilities
  - Classification table mapping each existing artifact (6 rules, 7 skills, AGENTS.md, governance docs) to its layer
- **Completion Criteria:**
  - Taxonomy is defined with clear layer boundaries and ownership responsibilities
  - Every existing artifact is classified
  - Cursor artifacts are classified as Cursor Tool Assets (implementation) containing Harness Asset content (governance)
  - No file is moved or modified — classification is conceptual
- **Verification Expectations:** Architecture doc review confirms taxonomy is complete, consistent, and does not contradict the Design.

---

##### TASK-A02 — Define AI Harness Implementation Model

- **Purpose:** Document what constitutes a compliant Harness implementation — the relationship between canonical governance content and tool-specific projections.
- **Design Traceability:** DDA-001; AP-002; AP-004; REQ-002; REQ-006; Design §AI Harness Implementation Model, §Implementation Relationship Model, §Canonical Content Principle, §Future Extensibility
- **Dependencies:** TASK-A01
- **Deliverables:**
  - Implementation Model section in `harness-architecture.md` defining: rule projections, skill projections, bootstrap consumption, context management, and tool-specific optimizations
  - Future tool compliance requirements (what a third tool must provide, may provide, and what the architecture does not require)
  - Distinction between Transition Architecture and Target Architecture documented
- **Completion Criteria:**
  - Implementation Model is documented with clear definitions
  - The "content is canonical; format is tool-specific" principle is stated
  - Future extensibility requirements are listed
  - Transition vs Target Architecture distinction is clear
- **Verification Expectations:** Architecture doc review confirms consistency with DDA-001 and DDA-003.

---

##### TASK-A03 — Establish Governance Ownership Responsibilities

- **Purpose:** Clarify what Harness governance owns vs what each tool implementation owns, defining the asymmetric relationship.
- **Design Traceability:** DDA-001; DDA-009; AP-001; AP-002; REQ-003; Design §Ownership Model, §Ownership Responsibilities
- **Dependencies:** TASK-A01
- **Deliverables:**
  - Ownership responsibilities section in `harness-architecture.md`
  - Clear statement: Tool Assets are implementation details, not governance authorities
  - Clarification: the authority hierarchy remains tool-agnostic — Tool Assets do not compete with ADRs, Architecture docs, or Rules
- **Completion Criteria:**
  - Harness governance ownership documented: rule/skill content, authority hierarchy, SDD lifecycle, verification governance, documentation routing, calibration workflow
  - Tool implementation ownership documented: directory structure, file format, frontmatter schema, tool-specific optimizations
  - The asymmetric relationship (Harness defines what; tools provide how) is explicit
- **Verification Expectations:** Documentation review confirms ownership boundaries do not conflict with ADR-003 or the authority hierarchy.

---

#### Workstream B — Documentation Architecture

---

##### TASK-B01 — Generalize Governance Documentation Naming

- **Purpose:** Rename tool-specific titles in governance documentation to tool-agnostic names.
- **Design Traceability:** DDA-006; DDA-007; REQ-005; NFR-004; Design §Documentation Naming Strategy, §Documentation Taxonomy
- **Dependencies:** TASK-A01
- **Deliverables:**
  - `Documentation/AI-Harness/rules.md`: "Cursor Rules Inventory" → "Rules Inventory"
  - `Documentation/AI-Harness/documentation-index.md`: "Cursor Artifacts" → "Machine-Facing Artifacts"; Cline directories added to index
  - `Documentation/AI-Harness/Harness-Design/rules-strategy.md`: "Cursor Rules" → "Harness Rules"
  - `Documentation/AI-Harness/Harness-Design/skills-strategy.md`: "Cursor Skills" → "Harness Skills"
  - `Documentation/AI-Harness/Harness-Design/harness-architecture.md`: "Cursor rules/skills" → "Harness rules/skills" where describing governance concepts
  - Internal links and cross-references updated
- **Completion Criteria:**
  - All governance docs with tool-specific titles describing Harness concepts are renamed
  - Tool-specific implementation details (e.g., "Cursor uses `alwaysApply: true`") retain Cursor-specific language
  - Historical artifacts are NOT renamed
- **Verification Expectations:** Documentation review confirms consistent naming. No broken internal links.

---

##### TASK-B02 — Define and Apply Tool-Agnostic Path Reference Strategy

- **Purpose:** Update governance documentation to reference tool-agnostic concepts by default, with parenthetical disambiguation for tool-specific paths.
- **Design Traceability:** DDA-005; REQ-005; REQ-011; NFR-004; Design §Path Reference Strategy, §Reference Patterns, §The 43+ `.cursor/` References
- **Dependencies:** TASK-A01, TASK-B01
- **Deliverables:**
  - Path reference strategy documented in governance docs
  - High-impact governance docs updated with tool-agnostic references + parenthetical disambiguation:
    - `harness-architecture.md`, `rules-strategy.md`, `skills-strategy.md`, `sdd-operational.md`, `documentation-index.md`, `rules.md`, `CONTRIBUTING-AI.md`
    - SDD templates updated
  - Example pattern: "Harness rules (Cursor: `.cursor/rules/*.mdc`; Cline: `.clinerules/*.md`)"
- **Completion Criteria:**
  - Governance docs reference "Harness rules" and "Harness skills" as default terms
  - Parenthetical disambiguation used when implementation paths are needed
  - `.cursor/` references describing Cursor-specific implementation details are kept as-is
  - Historical SDD instances, pilot reports, and verification artifacts are NOT modified
- **Verification Expectations:** Documentation review confirms consistent reference pattern. Spot-check 3-5 governance docs.

---

##### TASK-B03 — Define Cross-Skill Reference Strategy

- **Purpose:** Ensure all cross-skill references use skill names rather than tool-specific paths, keeping skill content as pure Harness Assets.
- **Design Traceability:** DDA-008; REQ-008; Design §Cross-Skill Reference Strategy
- **Dependencies:** TASK-A01
- **Deliverables:**
  - Cross-skill reference strategy documented in `skills-strategy.md`
  - All 7 existing Cursor skills reviewed and updated to use name-based references (e.g., "See the `documentation-update` skill") where they currently use path-based references
  - Contribution guide updated with the name-based reference convention
- **Completion Criteria:**
  - No skill contains a path-based reference to another skill that implies a single-tool directory
  - When a path is essential (e.g., contribution guide), both tool paths are provided
  - All 7 skills are reviewed
- **Verification Expectations:** Content review confirms no tool-specific skill paths in skill body content.

---

### Phase 2 — Multi-Tool Enablement

#### Workstream C — Cursor Implementation

---

##### TASK-C01 — Preserve Cursor Rule and Skill Integrity

- **Purpose:** Confirm no Cursor artifact is modified, moved, or removed. Validation task, not a modification task.
- **Design Traceability:** REQ-001; REQ-010; AP-006; Design §Compatibility Guarantees, §What Does NOT Change
- **Dependencies:** None (can run in parallel with Phase 1)
- **Deliverables:**
  - Baseline inventory of all `.cursor/` artifacts captured before any changes begin (file list with checksums or last-modified timestamps)
  - Verification after all other tasks complete that baseline matches — no Cursor file was changed
- **Completion Criteria:**
  - All 6 `.cursor/rules/*.mdc` files unchanged
  - All 7 `.cursor/skills/*/SKILL.md` files unchanged
  - `.cursor/` directory structure unchanged
  - Git diff confirms zero changes to `.cursor/`
- **Verification Expectations:** Git diff confirms zero changes. Critical evidence for REQ-001 acceptance.

---

##### TASK-C02 — Document Cursor as Reference Implementation

- **Purpose:** Formally classify Cursor artifacts as the Initial Reference Implementation within the new taxonomy.
- **Design Traceability:** DDA-001; AP-002; AP-006; Design §Transition Architecture vs Target Architecture
- **Dependencies:** TASK-A01
- **Deliverables:**
  - Cursor classification documented in `harness-architecture.md`
  - Transition Architecture state: Cursor artifacts serve as de facto canonical source during transition
  - Target Architecture vision: no single tool's projection is the permanent canonical source
- **Completion Criteria:**
  - Cursor classified as Initial Reference Implementation
  - Dual role (Tool Assets by format; Harness Asset content source during transition) documented
  - Path from Transition to Target Architecture is clear
- **Verification Expectations:** Architecture doc review confirms classification is consistent with taxonomy.

---

#### Workstream D — Cline Implementation

---

##### TASK-D01 — Create Cline Rule Projections

- **Purpose:** Create `.clinerules/` directory with rule files in Cline's native format (`.md` with `paths` frontmatter or no frontmatter), faithfully reproducing canonical rule content.
- **Design Traceability:** REQ-002; DDA-003; DDA-004; Design §Rule Architecture, §Frontmatter Strategy, §Scoping Intent Mapping
- **Dependencies:** TASK-A01, TASK-C01 (Cursor baseline captured so content source is known)
- **Deliverables:**
  - `.clinerules/` directory created at repository root
  - Six rule files with Cline format:
    | Rule | Scoping intent | Cline format |
    |------|---------------|--------------|
    | `security-phi.md` | Always-on | No frontmatter |
    | `token-economy.md` | Always-on | No frontmatter |
    | `update-doc.md` | Always-on | No frontmatter |
    | `backend-architecture.md` | File-scoped | `paths: ["DocAPI/**/*.cs"]` |
    | `ef-migrations.md` | File-scoped | `paths: ["DocAPI/**/*.cs"]` |
    | `blazor-front.md` | File-scoped | `paths: ["DocFront.Web/**/*"]` |
  - Guardrail text identical to Cursor `.mdc` files
  - `description` field treatment: governance-significant descriptions preserved as markdown headings or comments; Cursor-metadata-only descriptions omitted
- **Completion Criteria:**
  - All 6 rules exist in `.clinerules/` with `.md` extension
  - Always-on rules have no frontmatter; file-scoped rules have correct `paths`
  - Guardrail text matches Cursor source (content comparison)
  - `alwaysApply: true` + `globs` combined case handled per Design: Cline projection is always-on (no frontmatter)
  - `.cursor/rules/` is not modified
- **Verification Expectations:** Content comparison: each `.clinerules/*.md` guardrail text matches corresponding `.cursor/rules/*.mdc` guardrail text. Frontmatter mapping verified.

---

##### TASK-D02 — Create Cline Skill Projections

- **Purpose:** Create `.cline/skills/` directory with `SKILL.md` files identical in content to Cursor skills.
- **Design Traceability:** REQ-002; DDA-003; REQ-008; Design §Skill Architecture, §Skill Format
- **Dependencies:** TASK-B03, TASK-D01
- **Deliverables:**
  - `.cline/skills/` directory created at repository root
  - Seven skill directories, each containing `SKILL.md`:
    - `doc-organo-context`, `codebase-decomposition`, `effective-harness-planning`, `verifier`, `documentation-update`, `sql-migration-workflow`, `not-a-teacher`
  - Content (name, description, workflow procedures) identical to Cursor skills
  - Cross-skill references use skill names (per TASK-B03)
- **Completion Criteria:**
  - All 7 skills exist in `.cline/skills/` with identical content to `.cursor/skills/`
  - `.cursor/skills/` is not modified
- **Verification Expectations:** Content comparison: each `.cline/skills/*/SKILL.md` matches corresponding `.cursor/skills/*/SKILL.md`.

---

##### TASK-D03 — Create .clineignore for Cline Context Optimization

- **Purpose:** Create `.clineignore` as a recommended Cline-specific context optimization.
- **Design Traceability:** DDA-012; AP-005; REQ-006; Design §Context Management Strategy, §Adoption Principle
- **Dependencies:** None (independent)
- **Deliverables:**
  - `.clineignore` file at repository root with patterns reflecting Harness "avoid loading by default" strategy
  - Comment noting this is a Cline-specific optimization, not a Harness governance requirement
- **Completion Criteria:**
  - `.clineignore` exists with reasonable exclusion patterns
  - Does not interfere with Cursor (Cursor does not read `.clineignore`)
- **Verification Expectations:** File review confirms patterns are reasonable and documented as optional optimization.

---

### Phase 3 — Governance Consolidation

#### Workstream E — Multi-Tool Governance

---

##### TASK-E01 — Update AGENTS.md for Multi-Tool Clarity

- **Purpose:** Update `AGENTS.md` references from Cursor-specific paths to tool-agnostic concepts with parenthetical disambiguation.
- **Design Traceability:** REQ-009; DDA-001; Design §AGENTS.md Strategy, §Target State
- **Dependencies:** TASK-A01, TASK-B02
- **Deliverables:**
  - Updated `AGENTS.md`:
    - "Cursor rules" → "Harness rules"; "Cursor skills" → "Harness skills"
    - `.cursor/rules/` → "Harness rules (Cursor: `.cursor/rules/`; Cline: `.clinerules/`)"
    - `.cursor/skills/` → "Harness skills (Cursor: `.cursor/skills/`; Cline: `.cline/skills/`)"
    - Repository map updated to include `.clinerules/` and `.cline/`
  - Authority hierarchy, commands, operational constraints, verification sections: **unchanged**
- **Completion Criteria:**
  - All Cursor-specific references describing Harness concepts are generalized
  - `AGENTS.md` remains auto-loadable by both Cursor and Cline
  - No governance authority is changed
- **Verification Expectations:** Documentation review confirms AGENTS.md references are tool-agnostic where concepts are Harness-owned.

---

##### TASK-E02 — Document Tool-Specific Context Management Strategy

- **Purpose:** Document Harness context loading strategy as Harness Asset and each tool's mechanisms as Tool Assets.
- **Design Traceability:** DDA-012; AP-005; REQ-006; Design §Context Management Strategy, §Adoption Principle
- **Dependencies:** TASK-A01, TASK-D03
- **Deliverables:**
  - Context management section in `harness-architecture.md`:
    - Harness context loading strategy (unchanged)
    - Cursor mechanism: `token-economy` rule
    - Cline mechanisms: `.clineignore`, `/smol`, `/newtask`, Plan/Act mode — documented as tool-specific optimizations
    - Memory Bank: documented as alternative methodology, not adopted
- **Completion Criteria:**
  - Strategy vs mechanism distinction is clear
  - No Cline feature becomes a Harness governance requirement
- **Verification Expectations:** Documentation review confirms Harness/Tool boundary is maintained.

---

##### TASK-E03 — Document Frontmatter Mapping Governance

- **Purpose:** Document scoping intent → frontmatter mapping for each tool.
- **Design Traceability:** DDA-004; REQ-006; REQ-007; Design §Frontmatter Strategy, §Scoping Intent Mapping, §Tool Metadata Ownership
- **Dependencies:** TASK-A01, TASK-D01
- **Deliverables:**
  - Frontmatter mapping table documented in `rules-strategy.md`:
    | Harness scoping intent | Cursor | Cline |
    |---|---|--|
    | Always-on | `alwaysApply: true` | No frontmatter |
    | File-scoped | `globs: pattern` | `paths: [pattern]` |
    | Always-on + file-scoped | `alwaysApply: true` + `globs` | No frontmatter (always-on; glob redundant) |
  - `description` field treatment documented
  - Tool metadata ownership documented: frontmatter schemas are Tool Assets
- **Completion Criteria:**
  - Mapping table documented and matches created Cline rules
  - Combined case treatment explained
- **Verification Expectations:** Content review confirms mapping is accurate and consistent with TASK-D01 deliverables.

---

##### TASK-E04 — Extend documentation-update Skill for Multi-Tool Path Drift

- **Purpose:** Extend path drift detection to flag tool-specific path references in governance docs that describe Harness concepts.
- **Design Traceability:** DDA-011; REQ-005; NFR-001; NFR-004; Design §Documentation Update Evolution, §Path Drift Detection, §Governance Policy
- **Dependencies:** TASK-B02
- **Deliverables:**
  - Updated `documentation-update` skill (both `.cursor/skills/` and `.cline/skills/` copies):
    - Extended path drift check detecting `.cursor/` references in governance docs that describe Harness concepts
    - Guidance on distinguishing appropriate tool-specific references (implementation) from inappropriate ones (Harness concepts with tool-specific paths)
    - Statement that the check is advisory (flags for review), not enforcement
- **Completion Criteria:**
  - Path drift check extended with multi-tool awareness
  - Content is identical in both Cursor and Cline copies
- **Verification Expectations:** Skill content review confirms path drift extension is consistent with DDA-011.

---

##### TASK-E05 — Update effective-harness-planning Skill for Multi-Tool Awareness

- **Purpose:** Add multi-tool awareness check to evaluate tool-specific assumptions during harness reviews.
- **Design Traceability:** DDA-009; AP-003; Design §Governance Evolution
- **Dependencies:** TASK-A01
- **Deliverables:**
  - Updated `effective-harness-planning` skill (both copies):
    - Multi-tool awareness check: evaluates whether governance docs assume Cursor as the only tool
    - Flag: titles referencing "Cursor" where Harness concept is intended; path references using `.cursor/` as if canonical; single-tool assumptions in strategies
- **Completion Criteria:**
  - Multi-tool awareness check is part of standard harness review workflow
  - Content is identical in both Cursor and Cline copies
- **Verification Expectations:** Skill content review confirms multi-tool awareness check is present and consistent with taxonomy.

---

##### TASK-E06 — Update Calibration Workflow Templates for Multi-Tool

- **Purpose:** Update pilot report templates to include multi-tool fields.
- **Design Traceability:** DDA-010; REQ-003; Design §Calibration Workflow Evolution
- **Dependencies:** TASK-A01
- **Deliverables:**
  - Pilot report template updated with "Tool Used" field and "Tool-Specific Observations" field
  - Existing pilot reports NOT modified
- **Completion Criteria:**
  - Pilot report template includes multi-tool fields
  - Historical pilot reports remain as-is
- **Verification Expectations:** Template review confirms multi-tool fields present. Historical artifact review confirms no existing pilot report modified.

---

##### TASK-E07 — Document Tool-Specific Optimization Governance

- **Purpose:** Define how tool-specific capabilities relate to Harness governance without becoming governance requirements.
- **Design Traceability:** DDA-012; DDA-013; DDA-014; AP-005; REQ-006; Design §Global vs Project Scope, §Toggle Governance
- **Dependencies:** TASK-A01
- **Deliverables:**
  - Tool-specific optimizations section in `harness-architecture.md`:
    - Cline features classified as Tool Assets
    - Global scope: not a Harness concern; all Harness artifacts are project-scoped
    - Toggle: Harness does not define toggle policy; toggling is tool-specific runtime behavior
- **Completion Criteria:**
  - All Cline-specific features documented as Tool Assets
  - No Cline feature elevated to Harness governance requirement
- **Verification Expectations:** Documentation review confirms Harness/Tool boundary maintained for tool-specific optimizations.

---

### Phase 4 — Verification Preparation

#### Workstream F — Verification Preparation (VP)

---

##### VP-F01 — Prepare Cursor Compatibility Validation

- **Purpose:** Define verification procedure for Cursor backward compatibility.
- **Design Traceability:** REQ-001; REQ-010; AP-006; Design §Compatibility Guarantees
- **Dependencies:** TASK-C01
- **Deliverables:**
  - Verification checklist: git diff confirms zero `.cursor/` changes; rule loading, skill invocation, AGENTS.md bootstrap confirmed
- **Completion Criteria:** Verification procedure documented and traceable to REQ-001, REQ-010.
- **Verification Expectations:** Verifier uses this during Verify phase.

---

##### VP-F02 — Prepare Cline Compatibility Validation

- **Purpose:** Define verification procedure for Cline consuming Harness governance.
- **Design Traceability:** REQ-002; REQ-009; Design §Runtime Behavior
- **Dependencies:** TASK-D01, TASK-D02, TASK-D03, TASK-E01
- **Deliverables:**
  - Verification checklist: rule loading, skill invocation, AGENTS.md bootstrap, content fidelity, frontmatter correctness, `.clineignore` respected
- **Completion Criteria:** Verification procedure documented and traceable to REQ-002, REQ-009.
- **Verification Expectations:** Verifier uses this during Verify phase.

---

##### VP-F03 — Prepare Governance Consistency Validation

- **Purpose:** Define verification procedure for governance methodology unchanged.
- **Design Traceability:** REQ-003; REQ-004; NFR-005
- **Dependencies:** TASK-A03, TASK-E05, TASK-E06
- **Deliverables:**
  - Verification checklist: authority hierarchy, SDD lifecycle, verification governance, documentation routing, calibration workflow unchanged; Tool Assets are not governance authorities
- **Completion Criteria:** Verification procedure documented and traceable to REQ-003, REQ-004.
- **Verification Expectations:** Documentation review sensor recommended.

---

##### VP-F04 — Prepare Documentation Consistency Validation

- **Purpose:** Define verification procedure for no duplicate governance models.
- **Design Traceability:** REQ-005; NFR-004; NFR-006
- **Dependencies:** TASK-B01, TASK-B02, TASK-E01
- **Deliverables:**
  - Verification checklist: no duplicate inventories; single authoritative model; tool-agnostic concepts by default; parenthetical disambiguation consistency
- **Completion Criteria:** Verification procedure documented and traceable to REQ-005.
- **Verification Expectations:** Documentation review sensor recommended.

---

##### VP-F05 — Prepare Rule/Skill Content Fidelity Validation

- **Purpose:** Define verification procedure for identical content across tool projections.
- **Design Traceability:** REQ-007; REQ-008; NFR-001; Design §Architectural Requirement — Semantic Equivalence
- **Dependencies:** TASK-D01, TASK-D02, TASK-B03
- **Deliverables:**
  - Verification checklist: guardrail text and workflow procedures identical; frontmatter tool-specific (not compared); cross-skill references use skill names
- **Completion Criteria:** Verification procedure documented and traceable to REQ-007, REQ-008.
- **Verification Expectations:** Content review sensor recommended.

---

##### VP-F06 — Prepare Non-Conflicting Coexistence Validation

- **Purpose:** Define verification procedure for directories not conflicting.
- **Design Traceability:** REQ-010
- **Dependencies:** TASK-D01, TASK-D02, TASK-D03, TASK-C01
- **Deliverables:**
  - Verification checklist: no directory overlap; Cursor unaffected by Cline directories; Cline unaffected by Cursor directories
- **Completion Criteria:** Verification procedure documented and traceable to REQ-010.
- **Verification Expectations:** Repository structure review sensor recommended.

---

##### VP-F07 — Prepare Historical Artifact Backward Compatibility Validation

- **Purpose:** Define verification procedure for historical artifacts unmodified.
- **Design Traceability:** REQ-011; AP-006
- **Dependencies:** TASK-C01, TASK-B02
- **Deliverables:**
  - Verification checklist: git diff confirms zero changes to historical SDD instances, pilot reports, verification artifacts
- **Completion Criteria:** Verification procedure documented and traceable to REQ-011.
- **Verification Expectations:** Historical artifact review sensor recommended.

---

##### VP-F08 — Prepare Architecture Conformance Validation

- **Purpose:** Define verification procedure for implementation conforms to approved Design.
- **Design Traceability:** All DDA resolutions; All APs; All NFRs; Design §Design Validation, §Architectural Risks
- **Dependencies:** All preceding tasks
- **Deliverables:**
  - Architecture conformance checklist: taxonomy consistently applied; no shared directory/canonical format introduced (AP-007); Cursor compatibility preserved (AP-006); tool-specific optimizations do not alter methodology (AP-005); architecture supports third tool (NFR-002); migration is additive and reversible (NFR-007)
- **Completion Criteria:** Verification procedure documented and traceable to all architectural principles.
- **Verification Expectations:** Architecture conformance review sensor recommended.

---

### Phase 5 — Documentation Follow-Up Preparation

#### Workstream G — Documentation Follow-Up (DF)

---

##### DF-G01 — Prepare ADR-003 Revision Evaluation

- **Purpose:** Prepare evaluation package for ADR-003 revision.
- **Design Traceability:** DDA-002; Design §ADR-003 Disposition, §ADR Strategy Summary
- **Dependencies:** TASK-A01, TASK-B01, TASK-B02
- **Deliverables:**
  - ADR-003 revision preparation: current text, proposed generalization scope, what changes vs what stays, rationale for revision over accommodation/supersession
  - Note: does NOT draft the revised ADR (ADR governance owns drafting)
- **Completion Criteria:** ADR-003 revision scope is clearly defined; rationale documented; package ready for ADR governance.
- **Verification Expectations:** Verifier confirms preparation is consistent with DDA-002.

---

##### DF-G02 — Prepare New ADR for Harness/Tool/Shared Taxonomy

- **Purpose:** Prepare evaluation package for new ADR establishing the ownership taxonomy.
- **Design Traceability:** DDA-002; DDA-001; Design §New ADR Candidate
- **Dependencies:** TASK-A01, TASK-A02, TASK-A03
- **Deliverables:**
  - New ADR preparation: scope (three-layer taxonomy as permanent boundary), elements (definitions, responsibilities, canonical content principle), justification (ADR creation criteria met), recommended ID (ADR-007 or next)
  - Note: does NOT draft the ADR
- **Completion Criteria:** New ADR scope defined; justification documented; package ready for ADR governance.
- **Verification Expectations:** Verifier confirms preparation is consistent with DDA-002.

---

##### DF-G03 — Prepare Documentation/State.md Update

- **Purpose:** Identify State.md updates needed.
- **Design Traceability:** Design §Documentation Follow-up Candidates; SDD Operational §Documentation Integration Model
- **Dependencies:** All preceding tasks
- **Deliverables:**
  - State.md update candidates: harness files section noting tool-specific directories; current branch and next steps; active epic status
- **Completion Criteria:** Update candidates identified.
- **Verification Expectations:** Documentation Follow-Up executes update after Verify.

---

##### DF-G04 — Prepare Technical Documentation Updates

- **Purpose:** Identify Technical doc updates for runbook.
- **Design Traceability:** Design §Affected Components, §Documentation Follow-up Candidates
- **Dependencies:** TASK-D01, TASK-D02, TASK-E01
- **Deliverables:**
  - Runbook update candidates: mention Cline alongside Cursor; note `.clinerules/` and `.cline/` in repository structure; note `.clineignore`
- **Completion Criteria:** Update candidates identified with specific sections and proposed changes.
- **Verification Expectations:** Documentation Follow-Up executes update after Verify.

---

##### DF-G05 — Prepare SDD Template Updates

- **Purpose:** Identify SDD template updates for tool-agnostic concepts.
- **Design Traceability:** Design §Cross-Skill Reference Strategy, §Path Reference Strategy
- **Dependencies:** TASK-B02, TASK-B03
- **Deliverables:**
  - Template update candidates for `specify.md`, `design.md`, `tasks.md` templates: tool-agnostic concepts; name-based skill references; parenthetical disambiguation
- **Completion Criteria:** Update candidates identified.
- **Verification Expectations:** Documentation Follow-Up executes updates after Verify.

---

##### DF-G06 — Prepare Governance Consolidation Artifacts

- **Purpose:** Final consolidation review and drift check.
- **Design Traceability:** Design §Evolution Strategy; all DDA resolutions
- **Dependencies:** All preceding tasks (runs last)
- **Deliverables:**
  - Consolidation review: all governance docs use tool-agnostic concepts; all links updated; no governance doc claims Cursor as sole tool; new Cline directories in documentation index; cross-skill references use names
  - Drift check report: remaining `.cursor/` references in governance docs that should be tool-agnostic
- **Completion Criteria:** Consolidation review complete; remaining inconsistencies documented as known drift.
- **Verification Expectations:** Documentation Follow-Up routes findings to Documentation Update.

---

## 4. Dependency Map

| Task | Depends on | Can run in parallel with | Phase |
|------|------------|--------------------------|-------|
| TASK-A01 | None | — | 1 |
| TASK-A02 | TASK-A01 | TASK-A03, TASK-C01 | 1 |
| TASK-A03 | TASK-A01 | TASK-A02, TASK-C01 | 1 |
| TASK-B01 | TASK-A01 | TASK-B03, TASK-C01 | 1 |
| TASK-B02 | TASK-A01, TASK-B01 | — | 1 |
| TASK-B03 | TASK-A01 | TASK-B01, TASK-C01 | 1 |
| TASK-C01 | None | Phase 1 tasks | 2 |
| TASK-C02 | TASK-A01 | TASK-D01, TASK-D02 | 2 |
| TASK-D01 | TASK-A01, TASK-C01 | TASK-D02, TASK-D03 | 2 |
| TASK-D02 | TASK-B03, TASK-D01 | TASK-D03 | 2 |
| TASK-D03 | None | TASK-D01, TASK-D02 | 2 |
| TASK-E01 | TASK-A01, TASK-B02 | TASK-E02, TASK-E03, TASK-E07 | 3 |
| TASK-E02 | TASK-A01, TASK-D03 | TASK-E01, TASK-E03, TASK-E07 | 3 |
| TASK-E03 | TASK-A01, TASK-D01 | TASK-E01, TASK-E02, TASK-E07 | 3 |
| TASK-E04 | TASK-B02 | TASK-E05, TASK-E06 | 3 |
| TASK-E05 | TASK-A01 | TASK-E04, TASK-E06 | 3 |
| TASK-E06 | TASK-A01 | TASK-E04, TASK-E05 | 3 |
| TASK-E07 | TASK-A01 | TASK-E01, TASK-E02, TASK-E03 | 3 |
| VP-F01 | TASK-C01 | VP-F03, VP-F06, VP-F07 | 4 |
| VP-F02 | TASK-D01, TASK-D02, TASK-D03, TASK-E01 | VP-F05, VP-F06 | 4 |
| VP-F03 | TASK-A03, TASK-E05, TASK-E06 | VP-F01, VP-F04, VP-F08 | 4 |
| VP-F04 | TASK-B01, TASK-B02, TASK-E01 | VP-F03, VP-F08 | 4 |
| VP-F05 | TASK-D01, TASK-D02, TASK-B03 | VP-F02 | 4 |
| VP-F06 | TASK-D01, TASK-D02, TASK-D03, TASK-C01 | VP-F01, VP-F02 | 4 |
| VP-F07 | TASK-C01, TASK-B02 | VP-F01 | 4 |
| VP-F08 | All preceding tasks | — | 4 |
| DF-G01 | TASK-A01, TASK-B01, TASK-B02 | DF-G02, DF-G03 | 5 |
| DF-G02 | TASK-A01, TASK-A02, TASK-A03 | DF-G01, DF-G03 | 5 |
| DF-G03 | All preceding tasks | DF-G04, DF-G05 | 5 |
| DF-G04 | TASK-D01, TASK-D02, TASK-E01 | DF-G03, DF-G05 | 5 |
| DF-G05 | TASK-B02, TASK-B03 | DF-G03, DF-G04 | 5 |
| DF-G06 | All preceding tasks | — | 5 |

---

## 5. Requirement Traceability

| Requirement | Tasks | VP | Verification Evidence |
|-------------|-------|-----|-----------------------|
| REQ-001 | TASK-C01, TASK-C02 | VP-F01 | Cursor artifacts unchanged (git diff); rule/skill/AGENTS.md loading confirmed |
| REQ-002 | TASK-D01, TASK-D02, TASK-D03 | VP-F02 | Cline loads rules, skills, AGENTS.md; content fidelity confirmed |
| REQ-003 | TASK-A01, TASK-A03, TASK-E05, TASK-E06 | VP-F03 | Authority hierarchy, SDD lifecycle, verification governance, docs routing, calibration unchanged |
| REQ-004 | TASK-A01, TASK-A03 | VP-F03 | SDD methodology remains tool-agnostic |
| REQ-005 | TASK-B01, TASK-B02, TASK-E04, TASK-A01 | VP-F04 | No duplicate inventories; single authoritative documentation model |
| REQ-006 | TASK-E02, TASK-E03, TASK-E07, TASK-D03 | VP-F08 | Tool-specific optimizations do not alter Harness methodology |
| REQ-007 | TASK-D01, TASK-E03 | VP-F05 | Rule guardrail text identical across tool projections |
| REQ-008 | TASK-D02, TASK-B03 | VP-F05 | Skill content identical; cross-skill refs use names |
| REQ-009 | TASK-E01 | VP-F02 | AGENTS.md shared bootstrap; both tools auto-load |
| REQ-010 | TASK-C01, TASK-D01, TASK-D02, TASK-D03 | VP-F06 | Non-conflicting directories; Cursor workflows unaffected |
| REQ-011 | TASK-C01, TASK-B02 | VP-F07 | Historical artifacts unmodified and interpretable |

---

## 6. Verification Expectations

| Gate category | Expected / Not expected | Evidence or rationale |
|---------------|-------------------------|-----------------------|
| Build | Not expected | Governance-only; no code changes |
| Automated tests | Not expected | No code changes; content comparison may be manual |
| SQL / Persistence | Not expected | No persistence changes |
| API | Not expected | No API changes |
| UI | Not expected | No UI changes |
| Security / PHI | Not expected | Governance-only; `security-phi` rule content unchanged across tools |
| Domain review | Not expected | No domain model changes |
| Documentation review | **Expected** | Primary gate — governance consistency, naming, path references, content fidelity |
| Test strategy review | Not expected | No test code changes |
| ADR evaluation | **Expected** | ADR-003 revision and new ADR preparation |
| Legacy characterization | Not expected | No Legacy Codebase behavior change |

**Recommended review sensors:**

- `Documentation/AI-Harness/review-prompts/check-docs.md` — documentation consistency, naming, path references

---

## 7. Migration Checkpoints

### Checkpoint 1 — Governance Architecture Aligned
- **Phase:** End of Phase 1
- **Tasks:** TASK-A01, A02, A03 complete
- **Expected state:** Taxonomy defined; Implementation Model documented; ownership responsibilities clear
- **Validation:** Architecture doc review; taxonomy classification complete
- **Rollback:** Revert documentation changes via version control

### Checkpoint 2 — Cursor Compatibility Preserved
- **Phase:** End of Phase 2
- **Tasks:** TASK-C01, C02, D01, D02, D03 complete
- **Expected state:** Cursor baseline captured; Cline rules, skills, `.clineignore` created; Cursor artifacts unchanged
- **Validation:** Git diff confirms zero `.cursor/` changes; Cline directories populated
- **Rollback:** Remove `.clinerules/`, `.cline/`, `.clineignore` directories

### Checkpoint 3 — Cline Support Operational
- **Phase:** Mid Phase 3
- **Tasks:** TASK-D01, D02, D03, E01 complete
- **Expected state:** Cline can load rules from `.clinerules/`, invoke skills from `.cline/skills/`, bootstrap from AGENTS.md
- **Validation:** Content comparison confirms rule/skill content identical to Cursor source
- **Rollback:** Remove Cline directories

### Checkpoint 4 — Cross-Tool Governance Validated
- **Phase:** End of Phase 3
- **Tasks:** TASK-E01 through E07 complete
- **Expected state:** AGENTS.md updated; context management documented; frontmatter mapping documented; skills updated; calibration template updated
- **Validation:** Documentation review confirms tool-agnostic concepts and parenthetical disambiguation
- **Rollback:** Revert documentation changes via version control

### Checkpoint 5 — Documentation Consistency Achieved
- **Phase:** End of Phase 3 + Phase 4 VP items
- **Tasks:** All TASK items complete; all VP items prepared
- **Expected state:** Governance docs reference tool-agnostic concepts; no duplicate models; verification procedures documented
- **Validation:** VP-F01 through VP-F08 procedures ready; consolidation review complete
- **Rollback:** Revert documentation changes

### Checkpoint 6 — Feature Ready for SDD Pre-Execution Review
- **Phase:** End of Phase 5
- **Tasks:** All TASK, VP, DF items complete
- **Expected state:** All deliverables produced; verification procedures defined; documentation follow-up candidates identified; ADR packages prepared
- **Validation:** All completion criteria satisfied across all tasks
- **Rollback:** Phased rollback per checkpoint

---

## 8. ADR Awareness

The Design identified ADR-related decision areas. Tasks address them as follows:

| ADR | Disposition | Tasks | Owner |
|-----|-------------|-------|-------|
| ADR-003 | **Revise** | DF-G01 prepares revision scope | ADR governance creates revised ADR |
| New ADR (taxonomy) | **Create** | DF-G02 prepares new ADR justification | ADR governance creates ADR |
| Canonical format ADR | **Not needed** | No task created | — |

Tasks do **not** draft ADR content. They prepare evaluation packages that ADR governance will use to create or revise ADRs. This boundary is explicit: TASK items produce implementation deliverables; DF items prepare governance inputs.

---

## 9. Known Risks and Skipped Checks

| Risk or skipped check | Reason | Mitigation / follow-up |
|-----------------------|--------|------------------------|
| Content drift between tool projections | Manual synchronization during transition | TASK-E04 path drift check; governance review during calibration |
| Frontmatter mapping errors | Cline frontmatter created manually | TASK-E03 documents mapping; VP-F05 validates content fidelity |
| Path reference inconsistency during transition | Incremental update approach | Phased update with prioritization; path drift detection catches regressions |
| ADR-003 revision rejected by ADR governance | Governance process outcome | DF-G01 prepares strong rationale; revision preserves core decision |
| Cline environment not available for smoke check | Developer tooling constraints | VP-F02 defines procedure; Verifier may defer runtime check with documented residual risk |
| Cursor environment not available for smoke check | Developer tooling constraints | VP-F01 defines procedure; Verifier may rely on git diff evidence |
| Overengineering: adapter/generation mechanism introduced | Design explicitly rejects this | VP-F08 conformance check verifies no shared directory or canonical format was created |
| Skill content updates cause Cursor regression | Skill content changes may affect Cursor invocation | TASK-E04 and E05 update both copies identically |

---

## 10. Documentation Follow-Up Candidates

Documentation Follow-Up will decide final routing. Candidates identified by this Tasks plan:

- **State:** Update harness files section; current branch; active epic (DF-G03)
- **ADR:** Revise ADR-003; create new ADR for taxonomy (DF-G01, DF-G02)
- **Architecture docs:** `harness-architecture.md` updated with taxonomy, implementation model, ownership (TASK-A01-A03)
- **Technical docs:** Runbook updated to mention Cline (DF-G04)
- **Rules:** Cline rule projections created (TASK-D01); rule content unchanged
- **Skills:** `documentation-update` extended (TASK-E04); `effective-harness-planning` updated (TASK-E05)
- **Review prompts:** No changes identified
- **Templates:** SDD templates updated (DF-G05); pilot report template updated (TASK-E06)
- **Active SDD:** This `tasks.md` artifact

---

## 11. Commit Guidance

- Keep commits atomic and focused per task or small task group
- Do not commit secrets, `.env` files, or credentials
- Commit messages should reference the task ID (e.g., "TASK-A01: Formalize Harness/Tool/Shared taxonomy")
- Phase 2 commits should clearly separate Cline artifact creation from any documentation changes
- Verify `git diff` before committing to ensure no accidental `.cursor/` modifications

---

## 12. Completion Handoff

Before marking tasks complete, provide the Verifier with:

- Implemented task list with status
- Requirement traceability status
- Content comparison reports (rule and skill fidelity)
- Git diff evidence (Cursor artifacts unchanged; historical artifacts unchanged)
- Review sensors applied or skipped
- Residual risks and known drift items
- Documentation follow-up candidates
- ADR preparation packages

### Task Checkbox Ownership

| Artifact | Execute | Verify | Documentation Follow-Up |
|----------|---------|--------|-------------------------|
| `TASK-*` implementation | Marks work done; may check boxes when complete | Validates acceptance against requirements | Does **not** own implementation status |
| `VP-*` | Prepares evidence inputs | Uses for gate selection | — |
| `DF-*` | Identifies candidates only | Identifies mandatory targets | Executes doc sync; may mark DF-* complete |
| `tasks.md` checkboxes | Execute may mark TASK-* complete | Verify confirms acceptance | May mark VP-* / DF-* when workflow phases complete |