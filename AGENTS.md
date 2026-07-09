# AGENTS.md — Doc Organo

Stable bootstrap for agents working in this repository. Read `Documentation/State.md` first; it owns the current branch, runtime status, blockers, and next steps.

## Product And Stack

Doc Organo is a real-world clinic management platform for patients, medical records, scheduling, and care journey workflows. The MVP runs in a controlled clinical environment with real users.

| Layer | Technology |
|-------|------------|
| Backend | ASP.NET Core, .NET 7, EF Core 7, SQL Server |
| Frontend | Blazor Server in `DocFront.Web` |
| Local infra | Docker Compose SQL Server, `docorgano-sql` on port 1433 |
| Legacy reference | Google Sheets behavior on `main` only |

## Repository Map

```text
doc_Organo/
├── DocAPI/             # REST API, application, domain, EF repositories, SQL DbContext
├── DocFront.Web/       # Blazor Server UI, services, state, mappers
├── Documentation/      # Product, architecture, technical, AI harness, State
├── .cursor/            # Cursor machine-facing rules and skills
├── .clinerules/        # Cline machine-facing rules
└── .cline/             # Cline machine-facing skills
```

## Read First

| Need | Source |
|------|--------|
| Current status | `Documentation/State.md` |
| Product intent | `Documentation/Product/PRD.md` |
| Backlog and sequencing | `Documentation/Product/PM_DocOrgano.md` |
| Domain rules | `Documentation/Architecture/Domain_Overview_Business_Rules.md` |
| Architecture overview | `Documentation/Architecture/Architecture_Overview.md` |
| Accepted decisions | `Documentation/Architecture/ADR/` |
| SQL migration | `Documentation/Technical/migration-sql.md` |
| Front architecture | `Documentation/Technical/front-architecture.md` |
| Local runbook | `Documentation/Technical/runbook.md` |
| Harness navigation | `Documentation/AI-Harness/documentation-index.md` |

## Authority Hierarchy

- Architecture truth: accepted ADRs, then Architecture docs, then Technical docs.
- Harness governance: Harness Design docs, then Shared Assets (`AGENTS.md`), Harness rules (Cursor: `.cursor/rules/`; Cline: `.clinerules/`), Harness skills (Cursor: `.cursor/skills/`; Cline: `.cline/skills/`), and contribution guides.
- Operational truth: `Documentation/State.md`.
- Feature truth: the active SDD for that feature, when one exists.

Conflict rules:

- ADRs override `AGENTS.md`, rules, skills, and SDD.
- `Documentation/State.md` overrides stale bootstrap information.
- An active SDD owns feature scope and verification expectations, but cannot contradict an accepted ADR without proposing a new ADR.
- Rules are guardrails; skills are workflows. Rules and skills are Harness Assets — tool-agnostic governance content projected into each tool's native format.

## Operational Constraints

- Do not re-enable Google Sheets on the SQL branch unless explicitly requested.
- Treat Legacy Sheets code under `DocAPI/Legacy/_LegacySheetsDb/` as behavioral reference, not implementation to revive.
- Do not expand Financial features until the clinical SQL migration is stable.
- Treat all patient-identifiable and clinical data as sensitive in code, docs, prompts, logs, tests, commits, and PRs.
- Prefer incremental changes and avoid new abstractions unless the local architecture requires them.

## Commands

```bash
docker compose up -d
dotnet run --project DocAPI/DocAPI.csproj
dotnet run --project DocFront.Web/DocFront.Web.csproj
dotnet ef database update --project DocAPI/DocAPI.csproj
dotnet build
dotnet test
```

Local SQL credentials: copy `.env.example` to `.env` and set `SA_PASSWORD` (same value Docker uses). SQL integration workflow: `.\scripts\sql-integration-test.ps1` (see `Documentation/Technical/runbook.md`).

API default: `https://localhost:7004` from `DocFront.Web/appsettings.json`.

## Harness Workflow

Use the lifecycle defined by `Documentation/AI-Harness/Harness-Design/sdd-operational.md`:

```text
Research -> Plan -> SDD when required
  -> SDD Pre-Execution Review (Large/Complex)
  -> Execute -> Verify -> Documentation Follow-Up -> Reporting -> Teacher Guide
  -> State / ADR / Documentation Updates
```

Use SDD for work that is large, cross-layer, clinical/security-sensitive, persistence-related, API-contract-changing, Legacy-behavior-porting, or likely to need an ADR. Templates live under `Documentation/AI-Harness/template/`; feature SDD folders live under `Documentation/SDD/<feature-slug>/`.

Generated SDD artifacts, verification handoffs, reports, and lessons learned should be written in English.

Durable decisions require ADR evaluation. ADRs are for decisions future developers or agents must know to avoid re-litigating or accidentally reversing architecture, persistence, schema, security/auth, public API, cross-context ownership, major runtime, or irreversible migration choices.

## Verification

Work that changes behavior, architecture, persistence, contracts, security, documentation, or governance should pass through verifier responsibility before being considered complete. The verifier selects applicable gates, runs or requests checks, applies review prompts as sensors, records skipped gates with reasons, summarizes residual risk, and recommends State, ADR, documentation, rule, or skill follow-up.

Baseline gates are `dotnet build`, `dotnet test`, targeted SQL/API/UI smoke checks when relevant, review prompts under `Documentation/AI-Harness/review-prompts/`, and the PR checklist.
