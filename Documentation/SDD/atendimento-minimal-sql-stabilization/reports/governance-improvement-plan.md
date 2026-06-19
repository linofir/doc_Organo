# Governance Improvement Plan — Atendimento Minimal SQL Stabilization

> **Status:** Complete — Waves 1–3 implemented (2026-06-18)  
> **Template:** [Documentation/AI-Harness/template/governance/governance-improvement-plan.md](../../../AI-Harness/template/governance/governance-improvement-plan.md)  
> **Source evidence:** feature-report, sdd-pilot-report-v0.6, verification.md, Paciente pilot report v1.0

## Executive Summary

Harness calibration from the second forward SDD pilot is **implemented**. Wave 1 addressed credential probe, TASK-008 ownership, integration skip messages, sql-migration-workflow skill, DTO namespace checklist, and runbook UTF-8 no-BOM guidance. Wave 2 added pilot report template, session-handoff phase matrix, migration vertical patterns, verifier/sdd-operational task ownership, test-strategy optional debt, api-smoke script, and CONTRIBUTING-AI supersession rules. Wave 3 added teacher-guide template and Decision Log structure in specify template. Standalone `api-contract.md` and metrics dashboards remain **deferred** by policy.

## Implemented Backlog (GIB)

| GIB | Wave | Target | Status |
|-----|------|--------|--------|
| GIB-001 | 1 | Credential Probe — `template/sdd/specify.md`, runbook | Done |
| GIB-002 | 1 | TASK-008 ownership — tasks, verification templates, verifier skill | Done |
| GIB-003 | 1 | `SqlIntegrationTestGate` + integration tests | Done |
| GIB-004 | 1 | `sql-migration-workflow` skill | Done |
| GIB-005 | 1 | DTO namespace checklist — design template | Done |
| GIB-006 | 1 | UTF-8 no-BOM — runbook | Done |
| GIB-007 | 2 | Pilot supersession — CONTRIBUTING-AI | Done |
| GIB-008 | 2 | Task ownership — sdd-operational, verifier skill | Done |
| GIB-009 | 2 | Optional test debt — design template, test-strategy | Done |
| GIB-010 | 2 | `scripts/api-smoke-atendimento.ps1` | Done |
| GIB-011 | 2 | Phase matrix — session-handoff template | Done |
| GIB-012 | 2 | `template/reporting/sdd-pilot-report.md` | Done |
| GIB-013 | 2 | Migration vertical patterns — migration-sql.md | Done |
| GIB-014 | 2 | State drift blocking — verifier skill | Done |
| GIB-015 | 3 | `template/knowledge/teacher-guide.md` | Done |
| GIB-016 | 3 | Decision Log — specify template | Done |
| GIB-017 | 3 | Standalone api-contract.md | Deferred |
| GIB-018 | 3 | Metrics / observability | Deferred |

## Next Forward SDD

Prontuario SQL Stabilization may proceed using updated templates, skills, and runbook. Run Credential Probe before Execute.
