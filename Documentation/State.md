# State — Doc Organo

Operational snapshot for agents and developers. Update at the end of each work session or merged PR.

**Last updated:** 2026-07-08 (AI Harness Multi-Tool Evaluation — Verified Approved with Minor Findings; Documentation Follow-Up in progress)

## Current branch

`feature/harness` — AI Harness Multi-Tool Evaluation implementation complete and verified (Approved with Minor Findings). Three-layer Harness Asset / Tool Asset / Shared Asset taxonomy established as architectural boundary. Cline first-class support implemented (`.clinerules/`, `.cline/skills/`, `.clineignore`). Clinical SQL migration work intentionally paused.

## Runtime status

| Component | Status |
|-----------|--------|
| DocAPI (SQL) | **Paciente** backend stabilized — full-field CRUD, collection search, soft delete, duplicate CPF 409, PHI-safe controller (14 unit + 1 SQL integration). **Atendimento Minimal** backend stabilized — CRUD, Paciente FK validation, Guid contracts, soft delete, PHI-safe controller, 501 report routes preserved (11 unit + 1 SQL integration). Prontuario/Agendamento stubbed (DI resolves, throws until migrated). **27 tests** total (Verify 2026-06-18) |
| DocAPI (`main`) | Functional with Google Sheets (reference only) |
| DocFront.Web | Blazor Server; expects API at `https://localhost:7004`. Still calls retired `paciente/nome/{nome}` — WS07 debt |
| SQL Server | Docker `docorgano-sql`; requires `SA_PASSWORD` env var |
| AI harness | **Multi-Tool Evaluation complete** — three-layer taxonomy established; Cline first-class support (6 rules, 7 skills, `.clineignore`); governance docs generalized. Verified Approved with Minor Findings. Paciente and Atendimento Minimal pilots also complete. |

## Active epic

**AI Harness Multi-Tool Evaluation** — SDD phases complete through Verify. Status: **Approved with Minor Findings**. Documentation Follow-Up in progress; Reporting and Teacher Guide evaluation pending.

**Infraestrutura SQL** — vertical slices **Paciente** and **Atendimento Minimal** backend verified. **Ready to resume** Prontuario SQL Stabilization Execute — Harness evaluation complete.

**Approved migration sequencing (resumes after Harness evaluation):**

```text
Paciente → Atendimento Minimal → Prontuario → Agendamento → Atendimento Workflow → WS07 Frontend
```

**Next planned implementation:** Prontuario SQL Stabilization — **Execute** (Pre-Execution Review complete 2026-06-19; Harness evaluation complete — unblocked). ADR-006 (dual-mode versioning API) accepted.

## SDD research status

| Feature SDD | Research | Specify | Design | Tasks | Pre-Exec Review |
|-------------|----------|---------|--------|-------|-----------------|
| [paciente-sql-stabilization](SDD/paciente-sql-stabilization/) | Complete (pre-calibration) | Complete — verified | — | — | — |
| [atendimento-minimal-sql-stabilization](SDD/atendimento-minimal-sql-stabilization/) | **Complete** | Complete — verified | — | — | — |
| [atendimento-workflow-stabilization](SDD/atendimento-workflow-stabilization/research.md) | **Complete** | Not ready — blocked on Prontuario + Agendamento Verify | — | — | — |
| [prontuario-sql-stabilization](SDD/prontuario-sql-stabilization/) | **Complete** (Part 3 delta) | **Complete** | **Complete** | **Complete** | **Complete (2026-06-19)** — Execute authorized |
| [ai-harness-multi-tool](SDD/ai-harness-multi-tool/) | **Complete** | **Complete** | **Complete** | **Complete** | **Complete (2026-07-08)** — Execute complete; Verify Approved with Minor Findings |

## Recent decisions

| Date | Decision |
|------|----------|
| 2026-05 | Entities refactor + `InitialCreate` EF migration (`b550477`) |
| 2026-05 | ERD versioned as `Documentation/Architecture/erd.dbml` |
| 2026-05 | AI harness foundation: AGENTS.md, rules, skills, SDD templates |
| 2026-05 | Legacy Sheets repos moved to `Legacy/_LegacySheetsDb/` (commented) |
| 2026-06 | SDD Operational Governance established as the canonical governance document |
| 2026-06 | Feature SDD location set to `Documentation/SDD/<feature-slug>/`; generated SDD, verification, reporting, and lessons-learned artifacts should be written in English |
| 2026-06 | Verification Governance v1, Reporting Strategy v1, Template Architecture, and v1 verification/reporting templates prepared for first SDD pilot |
| 2026-06 | Paciente SQL Stabilization verified (REQ-001–REQ-008); `GET /Paciente/nome/{nome}` retired; collection search adopted; ADR-001 referenced, no new ADR |
| 2026-06 | Wave 1 harness calibration: SDD Pre-Execution Review, post-Verify lifecycle, Execution Prerequisites, Environment-Dependent Evidence, mandatory Documentation Follow-Up |
| 2026-06 | Wave 2 harness calibration: Definition of Done alignment, CONTRIBUTING-AI post-Verify chain, reporting templates, SDD README, Harness Calibration Workflow |
| 2026-06 | Atendimento split into two SDDs: **Minimal SQL** (FK prerequisite for Prontuario/Agendamento) and **Workflow** (journey orchestration after Prontuario + Agendamento). Approved order documented in [migration-sql.md](Technical/migration-sql.md) and [PM_DocOrgano.md](Product/PM_DocOrgano.md) |
| 2026-06 | Atendimento Minimal SQL Stabilization verified (REQ-001–REQ-010); 27 tests; SQL integration + HTTP smoke; ADR-001 referenced, no new ADR; workflow and WS07 frontend deferred |
| 2026-06 | **Wave 3 harness calibration (Atendimento retrospective):** Credential Probe in SDD templates; TASK-008 Verify ownership; `SqlIntegrationTestGate`; `scripts/api-smoke-atendimento.ps1`; sql-migration-workflow skill update; pilot report + teacher guide + **governance-improvement-plan** templates; migration vertical patterns in migration-sql.md |
| 2026-06-19 | **ADR-006 accepted:** Prontuario dual-mode versioning API (PUT correction vs POST `/versoes` evolution); Prontuario SDD Pre-Execution Review complete; Credential Probe pass (27/27 baseline, 2/2 SQL integration); Execute authorized |
| 2026-07-07 | **AI Harness Multi-Tool:** Documentation review completed ([review-tool-agnostic-harness.md](AI-Harness/research/review-tool-agnostic-harness.md)); PM, PRD, and State updated to formally incorporate the feature |
| 2026-07-08 | **AI Harness Multi-Tool:** SDD Execute and Verify complete — Approved with Minor Findings. Three-layer Harness Asset / Tool Asset / Shared Asset taxonomy established as architectural boundary. Cline first-class support implemented (`.clinerules/` with 6 rules, `.cline/skills/` with 7 skills, `.clineignore`). Cursor compatibility preserved. Governance documentation generalized for multi-tool. |

- Prontuario and Agendamento repositories still stubbed — **Atendimento Minimal** prerequisite satisfied; forward SDDs can proceed.
- Atendimento workflow rules (~700 LOC Legacy) port deferred to **atendimento-workflow-stabilization** — requires Prontuario + Agendamento SQL verified first.
- Financial tables in schema — out of scope until clinical migration completes.
- Frontend/API contract drift on Paciente nome search until WS07 aligns with collection search route.

## Verification status

| Feature | Decision | Reference |
|---------|----------|-----------|
| Paciente SQL Stabilization | Complete with accepted residual risk | [verification.md](SDD/paciente-sql-stabilization/verification.md) |
| Atendimento Minimal SQL Stabilization | Complete with accepted residual risk | [verification.md](SDD/atendimento-minimal-sql-stabilization/verification.md) |
| AI Harness Multi-Tool Evaluation | Approved with Minor Findings | [verification.md](SDD/ai-harness-multi-tool/verification.md) |

Residual risk (accepted): SQL integration environment-dependent; optional mapping/negative tests deferred; WS07 frontend alignment pending; `AtualizadoPor` unset; report routes 501-only; workflow port deferred to separate SDD.

## Next steps

1. **Complete Documentation Follow-Up** for ai-harness-multi-tool — ADR-003 revision and new taxonomy ADR preparation; SDD template updates.
2. **Reporting** and **Teacher Guide evaluation** for ai-harness-multi-tool.
3. **Resume Prontuario SQL Stabilization** — Execute [tasks.md](SDD/prontuario-sql-stabilization/tasks.md) starting **TASK-001** (legacy sign-off). Harness evaluation complete — unblocked.
4. Complete **Teacher Guide** for Atendimento Minimal if not already generated (`teacher-guide.md`).
5. Optional test debt: soft-deleted PacienteId → 404 on POST; `AtendimentoMappingTests`.
6. Plan WS07 frontend alignment for retired `GET /Paciente/nome/{nome}` → collection search and Atendimento UI (after Workflow SDD).

## Task management

- **Now:** Git branches + [PM_DocOrgano.md](Product/PM_DocOrgano.md)
- **Future:** GitHub Projects or Linear (note here when switched)

## Harness files

- [AGENTS.md](../AGENTS.md)
- Harness rules (Cursor: [.cursor/rules/](../.cursor/rules/) — `.mdc` format; Cline: [.clinerules/](../.clinerules/) — `.md` format)
- Harness skills (Cursor: [.cursor/skills/](../.cursor/skills/); Cline: [.cline/skills/](../.cline/skills/))
- [.clineignore](../.clineignore) — Cline-specific context optimization
- [Documentation/SDD/](SDD/)
- [Documentation/Technical/migration-sql.md](Technical/migration-sql.md)
- [Harness Architecture](AI-Harness/Harness-Design/harness-architecture.md)
- [Rules Inventory](AI-Harness/rules.md)
- [Documentation Index](AI-Harness/documentation-index.md)
