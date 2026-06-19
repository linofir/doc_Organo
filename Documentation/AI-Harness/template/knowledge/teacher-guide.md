# Teacher Guide — [Feature Name]

> **Audience:** Junior developers, developers new to Doc Organo, future maintainers, and the project owner learning the architecture.
>
> **Prerequisite artifacts:** This guide teaches *how and why* the feature works. For scope, verification, and operational status, see the linked artifacts at the end — do not expect this document to repeat them.
>
> Copy to `Documentation/SDD/<feature-slug>/teacher-guide.md` after Reporting when knowledge strategy criteria apply. Use `.cursor/skills/not-a-teacher/SKILL.md`.

---

## How This Guide Differs From Other Artifacts

| Artifact | What it answers | This guide does not repeat it |
|----------|-----------------|-------------------------------|
| `specify.md` / `design.md` | What to build and how to design it | Requirement tables, full API contract tables |
| `verification.md` | Whether it passed and what evidence exists | Gate results, completion decisions |
| `feature-report.md` | What shipped and workflow lessons | Delivery summary, PM references |
| `session-handoff.md` | What to do in the next session | Next steps, branch status |
| `State.md` / PM | Current operational truth | Runtime or backlog status |
| ADRs | Durable architecture decisions | Full ADR text |
| `migration-sql.md` | Migration checklist and endpoint reference | Full route/status tables |

**This guide owns:** concepts, reasoning, architecture flow, code connections, pitfalls, and a study path so you can understand and safely extend the feature.

---

## 1. Feature Overview

### Business purpose

...

### Technical objective

...

### Stabilization goals (what “done” meant for learning purposes)

1. ...
2. ...

---

## 2. What Changed

Summarize by layer (Domain, Application/mapping, Infrastructure, API, Tests). Prefer reasoning over file lists.

---

## 3. Concept Inventory

| Concept | Plain-language definition | Where it lives in code |
|---------|---------------------------|------------------------|
| ... | ... | `path/to/file.cs` |

---

## 4. Architecture Walkthrough

Describe request/data flow for the primary use cases (create, read, update, delete, list, FK validation, etc.).

---

## 5. Code Reading Order

Recommended bottom-up path (typically Domain → mapping → repository → controller → tests).

1. ...
2. ...

---

## 6. Common Pitfalls

At least three feature-specific pitfalls that caused friction during implementation or verification.

1. **...** — ...
2. **...** — ...
3. **...** — ...

---

## 7. Evolution And Deferred Scope

What was intentionally **not** implemented in this slice and which future SDD owns it.

---

## 8. Study Roadmap

| Step | Topic | Read first | Then try |
|------|-------|------------|------------|
| 1 | ... | ... | ... |

---

## 9. Comparative Notes (when applicable)

How this feature relates to prior migration verticals (e.g. Paciente baseline, FK prerequisites).

---

## 10. References

- Active SDD: `Documentation/SDD/<feature-slug>/`
- Verification: `verification.md`
- Feature report: `reports/feature-report.md`
- Related teacher guide(s): ...
- Migration plan: `Documentation/Technical/migration-sql.md`

---

*End of Teacher Guide*
