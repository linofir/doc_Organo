---
name: sql-migration-workflow
description: EF and SQL migration workflow for Doc Organo. Use when adding or changing migrations, porting an aggregate from Legacy/stub persistence to SQL, or changing EF mappings.
---

# SQL Migration Workflow -- Doc Organo

## Purpose

Use this skill for the operational workflow around EF migrations and SQL persistence changes. EF invariants stay in `.cursor/rules/ef-migrations.mdc`; this skill owns the procedure.

## Start Here

1. Read `Documentation/State.md` for current migration status.
2. Read `Documentation/Technical/migration-sql.md` for migration sequencing and vertical patterns.
3. Read `Documentation/Architecture/ADR/ADR-001-soft-delete.md`.
4. Read `Documentation/Architecture/ADR/ADR-002-sql-server-ef-migration.md`.
5. Read the relevant entity, repository, Fluent config, controller/API contract surface, and tests.

## Preparation

- Identify the aggregate or schema area being changed.
- Confirm whether the work requires SDD because it is cross-layer, clinical, persistence-changing, or likely to need an ADR.
- Compare current SQL behavior with Legacy behavior only by targeted reads.
- Define characterization tests before porting critical Legacy behavior.
- Check whether the change affects public API contracts or frontend expectations.
- Load `Documentation/AI-Harness/Harness-Design/test-governance.md` when the current task involves test creation. Derive repository test scenarios from architectural decisions documented in the active SDD `design.md`.
- Run the **Credential Probe** from SDD Execution Prerequisites before Execute when SQL integration tests apply (see `Documentation/AI-Harness/template/sdd/specify.md`).

## Local Credentials And Scripts

Single canonical credential source: repo-root `.env` (`SA_PASSWORD`), loaded via `scripts/load-env.ps1`.

| Component | Role |
|-----------|------|
| `DocAPI/Infrastructure/SqlDb/SqlConnectionResolver.cs` | Resolves connection for tests, EF, and local runs |
| `scripts/load-env.ps1` | Loads `.env` into the current PowerShell session |
| `scripts/sql-integration-test.ps1` | Docker up -> migrate -> `dotnet test` filter `Sql` |
| `scripts/api-smoke-atendimento.ps1` | HTTP smoke for Atendimento Minimal (TASK-008 helper; DocAPI must be running) |

Connection resolution order: `DOCORGANO_TEST_CONNECTION` -> `SA_PASSWORD` (env or `.env`) -> user-secrets fallback.

If integration tests skip with a **login** message, fix password alignment -- do not assume Docker is down. See `Documentation/Technical/runbook.md`.

## Repeatable Backend Stabilization Pattern (Aggregates #1-#2)

Proven on Paciente and Atendimento Minimal:

1. **Domain** -- factory/create methods; soft delete via `MarcarComoExcluido()` (ADR-001); narrow update methods (no workflow in minimal slice).
2. **Repository** -- persistence-only CRUD; global query filter for soft delete; no FK validation in repository when parent FK is validated upstream.
3. **Controller** -- Guid routes; validate parent FK via repository lookup before create when required; PHI-safe errors (no `Console.WriteLine`).
4. **DTOs / mapping** -- slim create/read/update; watch DTO namespace vs entity name collisions.
5. **Tests** -- repository unit tests derived from architectural decisions in `design.md` (see `test-governance.md` for Repository Test ownership: protect Persistence Intent, validate persistence contracts, update semantics, query semantics). Optional controller unit tests with mocks for FK paths; one `*SqlIntegrationTests` class using `SqlIntegrationTestGate`.
6. **Deferred routes** -- preserve report/workflow routes as 501 when out of scope rather than deleting them.

**Atendimento-specific additions:** `GetByPacienteIdAsync`; multiple concurrent active journeys per patient; Paciente FK on POST.

**FK-prerequisite aggregates (Prontuario, Agendamento):** require valid `AtendimentoId` from Atendimento Minimal before Execute.

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
- Swagger/API smoke when API behavior changes -- **Verify records durable evidence** in `verification.md`
- Blazor smoke when frontend behavior changes
- Domain or security/PHI review prompts when clinical data or auth assumptions are touched

Skipped gates require concrete justification in the verification summary.

Optional test debt to document when accepted: mapping tests, soft-deleted parent FK negative tests, full controller HTTP coverage.

## Documentation Routing

- Update `Documentation/State.md` when migration status, blockers, runtime state, or next steps change.
- Evaluate ADR need for durable schema lifecycle, persistence strategy, API contract, or irreversible migration decisions.
- Update Architecture or Technical docs when the durable model changes.
- Use the `documentation-update` skill when routing is unclear or path drift is suspected.

## Output Format

```markdown
# SQL Migration Handoff: [aggregate/schema area]

## Scope
- ...

## Preparation Findings
- Legacy behavior:
- API/front impact:
- ADR candidates:
- Credential probe result:

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