# Teacher Guide — Mandatory Sections

Governance source: `Documentation/AI-Harness/Harness-Design/knowledge-strategy.md`

Section titles MAY use minor wording variation. Every informational obligation MUST be satisfied.

## Document Header

Before section 1, include:

- Title: `# Teacher Guide — <Feature Name>`
- Audience block (junior developers, new contributors, maintainers, project owner)
- Prerequisite note: guide teaches how/why; scope/verification/status live in linked artifacts

---

## Section Obligations

| # | Section | MUST contain |
|---|---------|--------------|
| 1 | **How This Guide Differs From Other Artifacts** | Boundary table: what this guide owns vs SDD, verification, reporting, State, ADRs, technical docs |
| 2 | **Feature Overview** | Business purpose, technical objective, stabilization or delivery goals framed for learning |
| 3 | **What Changed** | Major implementation changes by layer; understanding-focused, not a commit log |
| 4 | **Architecture Walkthrough** | End-to-end flow through actual layers; ASCII flows encouraged |
| 5 | **Concept Inventory** | Evidence-backed concept cards (see fields below) |
| 6 | **Deep Dive By Component** | Responsibilities and interactions for major components touched |
| 7 | **Testing Walkthrough** | Test types used, why each exists, representative tests, known gaps |
| 8 | **Security Concepts** | Security-sensitive patterns applied (e.g. PHI); principles, not compliance certification |
| 9 | **Knowledge Map** | At least two dependency chains; textual trees required; Mermaid MAY be added |
| 10 | **Study Guide** | Beginner / Intermediate / Advanced paths traceable to this feature |
| 11 | **Common Pitfalls** | ≥3 entries with what breaks / why / how to avoid |
| 12 | **Feature Evolution History** | Contract changes, refinements, scope decisions, tradeoffs, security remediation |
| 13 | **Suggested Next Feature** | Logical continuation; concepts reused vs expanded |
| 14 | **Quick Reference — Code Reading Order** | Sequence, rationale, dependency progression |
| 15 | **Related Artifacts** | Links to SDD, verification, reports, ADRs, technical docs |

Sections MUST NOT duplicate full content from linked artifacts. They MUST explain and connect.

---

## Concept Card Fields

Each Concept Inventory entry MUST include:

| Field | Requirement |
|-------|-------------|
| Concept name | Clear, standard terminology |
| Appears in | Primary file(s) or type(s) — method names preferred over line numbers |
| Why it exists | Business or design purpose |
| How it works | Brief mechanism in feature context |
| Related concepts | Cross-links within inventory |
| Study priority | High / Medium / Low |

Group concepts by layer or concern (Domain, Application, Infrastructure, API, Testing, Security) when helpful.

Include a **Concepts not used** subsection when SDD or readers might expect patterns that this feature deliberately omitted.

---

## Common Pitfalls Entry Template

```markdown
### Pitfall N — <short title>

**What commonly breaks:** <observable symptom>

**Why it breaks:** <mechanism rooted in this feature>

**How to avoid it:** <concrete prevention — files, tests, patterns>
```

---

## Feature Evolution History Entry Template

```markdown
### <topic>

- **Before:** <prior behavior, draft, or assumption>
- **After:** <implemented behavior>
- **Rationale:** <why the change or tradeoff was accepted>
```

Or decision/rationale form when before/after does not apply.

---

## Study Guide Entry Template

Per tier, use a table or numbered list:

| Topic | Why it matters | Where in this feature | Suggested order |
|-------|----------------|----------------------|-----------------|

Every row MUST point to a real file, type, or test in this feature.

---

## Code Reading Order Template

```markdown
## Quick Reference — Code Reading Order

Read in this order:

1. `<path>` — <what to learn>
2. ...

### Rationale

<why this order builds the mental model>

### Dependency progression

<how step N enables step N+1>
```

---

## Knowledge Map Template

Minimum: two ASCII trees showing concept dependencies (not file graphs).

```text
<root concept>
    └── <child>
            └── <child>
```

Note when diagram-based maps MAY be added in future revisions.
