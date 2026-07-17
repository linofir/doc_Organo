# State — Doc Organo

Operational snapshot for agents and developers. Update at the end of each work session or merged PR.

**Last updated:** 2026-07-16 (Test Governance Adoption — Verified PASS WITH OBSERVATIONS; Documentation Follow-Up complete; ready for Reporting)

## Current branch

`feature/harness` — AI Harness Multi-Tool Evaluation implementation complete and verified (Approved with Minor Findings). Three-layer Harness Asset / Tool Asset / Shared Asset taxonomy established as architectural boundary. Cline first-class support implemented (`.clinerules/`, `.cline/skills/`, `.clineignore`). Clinical SQL migration work **paused** — Prontuario SQL Stabilization **verified** (Verified with Minor Findings, 2026-07-09); Agendamento deferred. **Active feature: Test Governance Adoption (WS03/WS09)** — **Verified (PASS WITH OBSERVATIONS, 2026-07-16)**. All 13 TASKs implemented, all 31 REQs satisfied, all 14 AD-TG decisions codified. One non-blocking observation (O-001: PM_DocOrgano.md modified outside planned Affected Components). Documentation Follow-Up complete — State.md and PM_DocOrgano.md synchronized. Ready for Reporting.

## Runtime status

| Component | Status |
|-----------|--------|
| DocAPI (SQL) | **Paciente** backend stabilized — full-field CRUD, collection search, soft delete, duplicate CPF 409, PHI-safe controller (14 unit + 1 SQL integration). **Atendimento Minimal** backend stabilized — CRUD, Paciente FK validation, Guid contracts, soft delete, PHI-safe controller, 501 report routes preserved (11 unit + 1 SQL integration). **Prontuario** backend stabilized — dual-mode versioning API (PUT correction + POST `/versoes` evolution), full nested graph CRUD, Atendimento FK validation, soft delete, PHI-safe controller, 10 endpoints, 11 unit + 27 controller + 1 SQL integration tests. Agendamento stubbed (DI resolves, throws until migrated). **79 tests** total (Verify 2026-07-09) |
| DocAPI (`main`) | Functional with Google Sheets (reference only) |
| DocFront.Web | Blazor Server; expects API at `https://localhost:7004`. Still calls retired `paciente/nome/{nome}` — WS07 debt |
| SQL Server | Docker `docorgano-sql`; requires `SA_PASSWORD` env var |
| AI harness | **Multi-Tool Evaluation complete** — three-layer taxonomy established; Cline first-class support (6 rules, 7 skills, `.clineignore`); governance docs generalized. Verified Approved with Minor Findings. Paciente and Atendimento Minimal pilots also complete. **HGEP v2.0 Phases A-D implemented (2026-07-10).** **Test Governance Adoption (WS03/WS09)** — **Verified (PASS WITH OBSERVATIONS, 2026-07-16)**. All 31 REQs satisfied. One non-blocking observation (O-001). Documentation Follow-Up complete. |

## Active epic

**Test Governance Adoption (WS03/WS09)** — **current active feature**. **Verified (PASS WITH OBSERVATIONS, 2026-07-16)**. All 13 TASKs implemented, all 31 REQs satisfied, all 14 AD-TG decisions codified. Governance consistency confirmed across 16 documents. One non-blocking observation (O-001: PM_DocOrgano.md modified outside planned Affected Components — minor update, does not introduce governance inconsistency). Documentation Follow-Up complete. Next lifecycle phase: Reporting.

**Infraestrutura SQL** — vertical slices **Paciente**, **Atendimento Minimal**, and **Prontuario** backend verified. **Agendamento SQL Migration & Stabilization** deferred while Test Governance Adoption is active. ADR-006 (dual-mode versioning API) accepted and honored. Prontuario verified (2026-07-09) — 79 tests, REQ-001–REQ-018 satisfied.

**AI Harness Multi-Tool Evaluation** — SDD phases complete through Verify. Status: **Approved with Minor Findings**. Documentation Follow-Up, Reporting, and Teacher Guide evaluation pending (background priority — does not block clinical development).

**Approved migration sequencing:**

```text
Paciente → Atendimento Minimal → Prontuario → Agendamento → Atendimento Workflow → WS07 Frontend
```

**Current implementation:** Test Governance Adoption (WS03/WS09) — **Verified (PASS WITH OBSERVATIONS, 2026-07-16)**. Previous: Prontuario SQL Stabilization — **verified** (2026-07-09). Next after Test Governance: **Agendamento SQL Migration & Stabilization**.

## SDD research status

| Feature SDD | Research | Specify | Design | Tasks | Pre-Exec Review |
|-------------|----------|---------|--------|-------|-----------------|
| [paciente-sql-stabilization](SDD/paciente-sql-stabilization/) | Complete (pre-calibration) | Complete — verified | — | — | — |
| [atendimento-minimal-sql-stabilization](SDD/atendimento-minimal-sql-stabilization/) | **Complete** | Complete — verified | — | — | — |
| [atendimento-workflow-stabilization](SDD/atendimento-workflow-stabilization/research.md) | **Complete** | Not ready — blocked on Prontuario + Agendamento Verify | — | — | — |
| [prontuario-sql-stabilization](SDD/prontuario-sql-stabilization/) | **Complete** (Part 3 delta) | **Complete** | **Complete** | **Complete** | **Complete (2026-06-19)** — Execute authorized; **Verified with Minor Findings (2026-07-09)** |
| [ai-harness-multi-tool](SDD/ai-harness-multi-tool/) | **Complete** | **Complete** | **Complete** | **Complete** | **Complete (2026-07-08)** — Execute complete; Verify Approved with Minor Findings |
| [test-governance-adoption](SDD/test-governance-adoption/) | **Complete** — research.md produced | **Complete** | **Complete** | **Complete** — all 13 TASKs implemented; Execute phase complete | Not started (Execute was governance-only — no Pre-Exec Review required) — **Verified (PASS WITH OBSERVATIONS, 2026-07-16)** |

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
| 2026-07-09 | **Prontuario SQL Stabilization:** Execute phase resumed; Execution Readiness Check passed. Pre-Execution Review (2026-06-19) confirmed valid after AI Harness Multi-Tool Evaluation. |
| 2026-07-09 | **Prontuario SQL Stabilization:** Verified with Minor Findings — 79 tests (+52 from 27 baseline); all 18 REQs satisfied; ADR-006 dual-mode versioning API honored; third SQL vertical confirmed. F-01: TASK-009 Swagger smoke pending (low severity; 27 controller unit tests provide strong mitigation). Next: Agendamento SQL Migration & Stabilization. |
| 2026-07-10 | **Harness Governance Execution Plan (HGEP v2.0):** Phases A-D implemented — Context Acquisition Governance propagated to CONTRIBUTING-AI Execute, Token Economy rule evolved (3 new bullets, both Cursor and Cline projections), templates updated (Context Budget notes in specify.md and design.md, Execution Batching in tasks.md), verifier skill updated with CAG observation section (both projections). No new ADRs, rules, skills, or templates created — consolidation only. Phase E runtime validation partially observed during implementation session. |
| 2026-07-14 | **Test Governance Adoption (WS03/WS09):** Feature lifecycle initialized — registered as current active feature in State.md. Prontuario SQL Stabilization confirmed completed (Verified with Minor Findings, 2026-07-09). Agendamento SQL Migration deferred. AI Harness workflow status set to awaiting Research. No SDD artifacts, governance documents, ADRs, or Test Governance content modified. |
| 2026-07-14 | **Test Governance Adoption (WS03/WS09):** Research phase **complete**. `research.md` consolidates validated evidence from 10 Engineering Review Reports (v0.1–v0.8.3) into 14 architectural decisions (AD-TG-001 through AD-TG-014), evidence maturity assessment classifying 27 findings, full impact analysis across all AI Harness components, and recommended implementation scope (15 items in-scope, 11 items out-of-scope). **READY FOR SPECIFY.** |
| 2026-07-15 | **Test Governance Adoption (WS03/WS09):** Execute phase **complete**. All 13 TASKs implemented — new `test-governance.md` created; `sdd-operational.md`, `verification-governance.md`, `test-strategy.md` review prompt, verifier skill (Cursor + Cline), sql-migration-workflow skill (Cursor + Cline), design.md/tasks.md/verification.md templates, documentation-index.md, AGENTS.md, harness-architecture.md, and State.md all updated. Ready for Verify. |
| 2026-07-16 | **Test Governance Adoption (WS03/WS09):** **Verified (PASS WITH OBSERVATIONS)**. All 31 REQs satisfied, all 14 AD-TG decisions codified. Governance consistency confirmed across 16 documents. Skill projections identical (Cursor/Cline). One non-blocking observation (O-001: PM_DocOrgano.md modified outside planned Affected Components). Documentation Follow-Up executed — State.md and PM_DocOrgano.md synchronized. Ready for Reporting. |

- **Atendimento Minimal** prerequisite satisfied for Prontuario and Agendamento; forward SDDs can proceed.
- Atendimento workflow rules (~700 LOC Legacy) port deferred to **atendimento-workflow-stabilization** — requires Prontuario + Agendamento SQL verified first.
- Financial tables in schema — out of scope until clinical migration completes.
- Frontend/API contract drift on Paciente nome search until WS07 aligns with collection search route.

## Verification status

| Feature | Decision | Reference |
|---------|----------|-----------|
| Paciente SQL Stabilization | Complete with accepted residual risk | [verification.md](SDD/paciente-sql-stabilization/verification.md) |
| Atendimento Minimal SQL Stabilization | Complete with accepted residual risk | [verification.md](SDD/atendimento-minimal-sql-stabilization/verification.md) |
| Prontuario SQL Stabilization | Verified with Minor Findings | [verification.md](SDD/prontuario-sql-stabilization/verification.md) |
| AI Harness Multi-Tool Evaluation | Approved with Minor Findings | [verification.md](SDD/ai-harness-multi-tool/verification.md) |
| Harness Governance Execution Plan (HGEP) | Phases A-D implemented (2026-07-10) | [harness-governance-execution-plan.md](SDD/ai-harness-multi-tool/reports/harness-governance-execution-plan.md) |
| Test Governance Adoption | Verified (PASS WITH OBSERVATIONS, 2026-07-16) | [verification.md](SDD/test-governance-adoption/verification.md) |

Residual risk (accepted): SQL integration environment-dependent; optional mapping/negative tests deferred; WS07 frontend alignment pending; `AtualizadoPor` unset; report routes 501-only; workflow port deferred to separate SDD; Prontuario TASK-009 Swagger smoke pending (F-01, low severity); CID catalog absent in production.

## Next steps

1. **Test Governance Adoption (WS03/WS09)** — **Verified (PASS WITH OBSERVATIONS, 2026-07-16).** Next action: **Reporting** phase. SDD folder: `Documentation/SDD/test-governance-adoption/`. Observation O-001 (PM_DocOrgano.md scope) resolved during Documentation Follow-Up — PM_DocOrgano.md status entries synchronized.
2. **Agendamento SQL Migration & Stabilization** — deferred while Test Governance Adoption is active. Atendimento Minimal prerequisite satisfied.
3. **Harness Governance Execution Plan (HGEP)** — Phases A-D complete (2026-07-10). Phase E (HGEP-016 Cline runtime, HGEP-017 Cursor skill invocation, HGEP-018 .clineignore review) partially validated during implementation session. Remaining Phase E items documented below.
4. **TASK-009 Swagger smoke** for Prontuario per F-01 — execute manual checklist from [specify.md](SDD/prontuario-sql-stabilization/specify.md) Runtime Validation Environment; record results.
5. **Complete Documentation Follow-Up** for ai-harness-multi-tool — ADR-003 revision and new taxonomy ADR preparation; SDD template updates (background — does not block clinical development).
6. **Reporting** and **Teacher Guide evaluation** for ai-harness-multi-tool (background).
7. **Reporting** and **Teacher Guide evaluation** for Prontuario SQL Stabilization.
8. Complete **Teacher Guide** for Atendimento Minimal if not already generated (`teacher-guide.md`).
9. Optional test debt: soft-deleted PacienteId → 404 on POST; `AtendimentoMappingTests`; Prontuario two-atendimento patient-wide Versao scenario (D-03).
10. Plan WS07 frontend alignment for retired `GET /Paciente/nome/{nome}` → collection search and Atendimento UI (after Workflow SDD).
11. **HGEP completion:** Run HGEP-017 (Cursor verifier skill invocation) and finalize HGEP-016/018 observations in Agendamento session notes. HGEP Phase E completion does not block Agendamento as per §6 Harness Readiness for Next Feature.

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