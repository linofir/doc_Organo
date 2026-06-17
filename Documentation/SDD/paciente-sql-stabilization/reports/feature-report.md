# Feature Report - Paciente SQL Stabilization

> Active SDD: `Documentation/SDD/paciente-sql-stabilization/`
> Verification: [verification.md](../verification.md)

## Summary

The Paciente SQL backend vertical was stabilized through the first SDD pilot: full-field CRUD persistence, soft delete (ADR-001), collection search contract, PHI-safe controller, expanded unit tests, SQL integration tests, and Swagger runtime validation. All REQ-001 through REQ-008 were verified with accepted residual risk.

## Scope

- Feature: Paciente SQL Stabilization
- Active SDD: `Documentation/SDD/paciente-sql-stabilization/`
- PM items: WS01 P0, WS03 P0, WS04 P0 (Paciente scope)
- Included: Backend domain, repository, API, mapping, unit tests, SQL integration tests, legacy characterization
- Excluded: Frontend (WS07), migrations, auth, PDF, other aggregates, State/PM edits during Execute

## Requirements Delivered

| Requirement | Outcome | Evidence |
|-------------|---------|----------|
| `REQ-001` | Delivered | Full field CRUD; mapping + repository + SQL integration tests |
| `REQ-002` | Delivered | Soft delete via ADR-001; query filter exclusion tests |
| `REQ-003` | Delivered | CPF exact; nome collection search; retired single-result nome route |
| `REQ-004` | Delivered | 201/200/204/404/409 status codes; runtime smoke |
| `REQ-005` | Delivered | PHI logging removed from PacienteController |
| `REQ-006` | Delivered | `PacienteSqlIntegrationTests` with skip policy |
| `REQ-007` | Delivered | Swagger runtime smoke all scenarios |
| `REQ-008` | Delivered | Legacy characterization table in specify.md |

## Implementation Areas

| Area | Summary |
|------|---------|
| Domain | `ComplementarCadastro()`, `MarcarComoExcluido()` |
| Persistence | `PacienteRepository` — full fields, CPF exact, nome partial, pagination |
| API | Collection search routes; duplicate CPF 409; PHI-safe errors |
| Frontend | None — WS07 alignment deferred |
| Documentation / Harness | SDD pilot; verification.md; documentation follow-up complete |

## Verification Summary

- Completion decision: Complete with accepted residual risk
- Gates passed: Build, automated tests (14 pass), API smoke, security/PHI, domain, legacy characterization
- Gates skipped: UI (expected); SQL integration skipped on Verify re-run (Docker unavailable; Execute evidence accepted)
- Review sensors applied: domain, security-phi, check-docs, test-strategy
- Residual risk: Low–Medium — frontend contract drift until WS07; environment-dependent SQL tests; no controller API tests

## Documentation And ADR Follow-up

- State: Updated — Paciente backend verified
- ADR: None — ADR-001 referenced only
- Architecture docs: No changes required
- Technical docs: `migration-sql.md` Paciente checklist + API contract; `runbook.md` SQL integration prerequisites
- SDD: tasks.md marked complete
- Rules: None
- Skills: Consider `sql-migration-workflow` update if integration pattern becomes standard
- Review prompts: None
- Templates: Pilot calibration notes captured in Lessons Learned below

## Lessons Learned

- **State drift is predictable after Execute** — check-docs sensor correctly flagged stale State.md; Documentation Follow-Up should run promptly after Verify.
- **SQL integration skip policy works** — `SkippableFact` + `DOCORGANO_TEST_CONNECTION` allows CI/local flexibility; runbook should document prerequisites (now added).
- **API contract belongs in migration-sql for now** — standalone `api-contract.md` deferred; Paciente contract section added to migration plan until multi-aggregate contract doc is justified.
- **Runtime smoke fills controller test gap** — acceptable for pilot; Prontuario forward SDD should evaluate lightweight API status-code tests.
- **tasks.md checkboxes lag Execute** — mark complete during Verify or Documentation Follow-Up, not left for manual discovery.
- **Reporting stays lightweight** — feature report + session handoff sufficient; no separate lessons-learned file needed for v1.

## Remaining Work

- Commit implementation changes (if not yet committed)
- Re-run full 15-test suite with Docker SQL before merge
- WS07: align frontend with collection search; retire `paciente/nome/{nome}` consumption
- ProntuarioRepository forward SDD pilot
- Optional: controller-level API tests; empty-search 200 `[]` automation
- Project-wide: ADR-001 relationship-level soft-delete filter behavior (open item)
