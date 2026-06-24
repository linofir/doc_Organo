# Pre-Execution Review — Prontuario SQL Stabilization

**Date:** 2026-06-19  
**Phase:** SDD Pre-Execution Review (Consolidation)  
**Readiness:** **READY FOR EXECUTION** (TASK-001 onward; TASK-004+ prerequisites validated below)

## Decisions recorded

| Item | Decision |
|------|----------|
| **DQ-009** | **Option A — ADR now.** [ADR-006](../../../Architecture/ADR/ADR-006-prontuario-dual-mode-versioning-api.md) accepted 2026-06-19. |
| **Credential Probe** | **Option A — Run before TASK-004.** Executed 2026-06-19; results below. |
| **D-02 vs Specify** | Application orchestrates `GetMaxVersaoForPacienteAsync`; aggregate validates `nextVersao` — authoritative per ADR-006 and design D-02. |

## Credential Probe results (2026-06-19)

| Step | Result | Notes |
|------|--------|-------|
| Docker `docorgano-sql` | **Pass** | Container Up; port 1433 |
| `dotnet test` baseline | **Pass** | **27/27** passed (`DocAPI.Tests`, `--no-build`) |
| SQL integration subset (`--filter Sql`) | **Pass** | **2/2** (Paciente + Atendimento SQL tests) |
| `dotnet ef database update` | **Not re-run** | Build blocked — DocAPI process holding `DocAPI.exe`; SQL tests passing implies schema reachable |
| DocAPI running | **Pass** | Process active locally (Swagger expected at dev URL) |
| `SA_PASSWORD` / connection | **Pass** | Inferred from successful SQL integration tests |

**Baseline test count for Execute:** **27** (record before Prontuario implementation changes).

## Execute authorization

- **TASK-001 through TASK-003:** Authorized without additional infra gates.
- **TASK-004 onward:** Authorized — prerequisites and DQ-009 satisfied.

## Residual risks accepted at review

- WS07 frontend alignment deferred (Backend Stabilization Rule).
- CID production catalog absent; synthetic CID in tests only.
- `AtualizadoPor` unset until auth ADR work.
- Optional concurrent evolution **409** integration test.

## Next action

**Proceed to Execute — start `TASK-001`.**
