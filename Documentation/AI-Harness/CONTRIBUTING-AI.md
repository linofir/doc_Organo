# Contributing With AI — Doc Organo

This guide describes the contribution workflow. It does not replace `AGENTS.md`, rules, skills, SDD artifacts, ADRs, or `Documentation/State.md`.

## Authority

- ADRs own durable architecture decisions.
- Architecture and Technical docs explain the durable model.
- `Documentation/State.md` owns current operational truth.
- Active SDD owns feature scope, design, tasks, and verification expectations.
- Rules are concise guardrails; skills are repeatable workflows.
- Review prompts are sensors; the verifier orchestrates which sensors and gates apply.

## Lifecycle

Use the approved harness lifecycle:

```text
Research -> Plan -> SDD when required -> Implement -> Verification -> State / ADR / Documentation Updates
```

## Before Coding

1. Read `Documentation/State.md` for current branch truth.
2. Read `AGENTS.md` for stable bootstrap and routing.
3. Use `.cursor/skills/doc-organo-context/SKILL.md` when a session needs guided orientation.
4. Load task-specific docs from `Documentation/AI-Harness/documentation-index.md`.

## Research

Research should produce durable findings or a short handoff, not depend on long chat history. Use targeted code reads and relevant skills such as `.cursor/skills/codebase-decomposition/SKILL.md`.

Canonical research context lives in `Documentation/AI-Harness/research/AI-Research.md` and `Documentation/AI-Harness/research/playbook.md`.

## Plan And SDD

Use SDD when the change is large, cross-layer, clinical/security-sensitive, persistence-related, API-contract-changing, Legacy-behavior-porting, or likely to require an ADR.

SDD templates live in `Documentation/AI-Harness/template/sdd/`. Feature SDDs should keep:

- `specify.md` for feature scope and acceptance criteria.
- `design.md` for technical approach, risks, and ADR trigger analysis.
- `tasks.md` for implementation steps and verification expectations.

## Implement

Keep implementation scoped to the accepted plan or active SDD. Use rules for guardrails and skills for workflows. Do not move workflow procedures into rules or stable project facts into skills.

## Verification

Before work is considered complete, apply verifier responsibility:

- Select applicable gates from the diff, active SDD, rules, ADRs, and review prompts.
- Run or request build, tests, SQL/API/UI smoke checks, and review prompts when relevant.
- Record skipped gates with concrete justification.
- Summarize residual risk.
- Recommend follow-up updates to `Documentation/State.md`, ADRs, docs, rules, or skills.

Baseline gates are:

1. `dotnet build`
2. `dotnet test`
3. SQL integration or characterization checks when persistence or Legacy behavior is touched
4. Swagger/API or Blazor smoke checks when behavior changes
5. Review prompts under `Documentation/AI-Harness/review-prompts/`
6. PR checklist in `.github/pull_request_template.md`

## Updates

Update `Documentation/State.md` when current branch/runtime/blocker/next-step truth changes. Evaluate ADR need when a durable decision affects architecture boundaries, persistence, schema lifecycle, security/auth, public API contracts, cross-context ownership, major technology/runtime choices, or irreversible migration decisions.

Use `.cursor/skills/documentation-update/SKILL.md` for documentation routing, path drift checks, and authority validation.

## Commits

Keep commits atomic and focused. Do not commit secrets, `.env` files, credentials, real patient data, or clinical data.
