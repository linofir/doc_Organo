---
name: sql-migration-workflow
description: EF and SQL migration workflow for Doc Organo. Use when adding or changing migrations, porting an aggregate from Legacy/stub persistence to SQL, or changing EF mappings.
---

# SQL Migration Workflow — Doc Organo

## Purpose

Use this skill for the operational workflow around EF migrations and SQL persistence changes. EF invariants stay in `.cursor/rules/ef-migrations.mdc`; this skill owns the procedure.

## Start Here

1. Read `Documentation/State.md` for current migration status.
2. Read `Documentation/Technical/migration-sql.md` for migration sequencing.
3. Read `Documentation/Architecture/ADR/ADR-001-soft-delete.md`.
4. Read `Documentation/Architecture/ADR/ADR-002-sql-server-ef-migration.md`.
5. Read the relevant entity, repository, Fluent config, controller/API contract surface, and tests.

## Preparation

- Identify the aggregate or schema area being changed.
- Confirm whether the work requires SDD because it is cross-layer, clinical, persistence-changing, or likely to need an ADR.
- Compare current SQL behavior with Legacy behavior only by targeted reads.
- Define characterization tests before porting critical Legacy behavior.
- Check whether the change affects public API contracts or frontend expectations.

## Migration Execution Guidance

Use the repository's normal EF command shape:

```bash
dotnet ef migrations add <Name> --project DocAPI/DocAPI.csproj
dotnet ef database update --project DocAPI/DocAPI.csproj
```

Before accepting a migration:

- Review generated operations for unintended table drops, column loss, or relationship changes.
- Confirm soft-delete query filters remain aligned with ADR-001.
- Confirm owned types and Fluent configuration discovery still work.
- Confirm secrets and local connection strings are not committed.

## Verification Expectations

Route final gate selection to verifier responsibility. Typical gates include:

- `dotnet build`
- `dotnet test`
- SQL integration or repository tests when persistence behavior changes
- Swagger/API smoke when API behavior changes
- Blazor smoke when frontend behavior changes
- Domain or security/PHI review prompts when clinical data or auth assumptions are touched

Skipped gates require concrete justification in the verification summary.

## Documentation Routing

- Update `Documentation/State.md` when migration status, blockers, runtime state, or next steps change.
- Evaluate ADR need for durable schema lifecycle, persistence strategy, API contract, or irreversible migration decisions.
- Update Architecture or Technical docs when the durable model changes.
- Use `.cursor/skills/documentation-update/SKILL.md` when routing is unclear or path drift is suspected.

## Output Format

```markdown
# SQL Migration Handoff: [aggregate/schema area]

## Scope
- ...

## Preparation Findings
- Legacy behavior:
- API/front impact:
- ADR candidates:

## Migration Notes
- ...

## Verification Handoff
- Recommended gates:
- Review prompts:
- Skipped/blocked gates:

## Documentation Follow-up
- State:
- ADR:
- Technical/Architecture:
```
