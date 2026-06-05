# ADR-003: Documentation Taxonomy and AI Harness Separation

## Status

Accepted

## Context

The repository had two broad documentation folders: `Documentation/` and `docs/`. Product, architecture, harness, ADRs and technical docs were split across both, which made it unclear where new documents should live.

The project also uses Cursor-specific artifacts in `.cursor/`, including rules and skills.

## Decision

Use `Documentation/` as the canonical home for durable project documentation:

- `Documentation/Product/` for PRD, PM and product planning.
- `Documentation/Architecture/` for architecture, domain, ERD and ADRs.
- `Documentation/Technical/` for runbooks, migration plans and technical references.
- `Documentation/AI-Harness/` for AI workflow documentation, SDD templates and review prompts.
- `Documentation/State.md` for current operational state.

Keep `.cursor/` for machine-facing Cursor artifacts only:

- `.cursor/rules/`
- `.cursor/skills/`

Keep old `docs/` files temporarily as compatibility redirects during transition.

## Consequences

**Positive**

- Clearer location rules for new documentation.
- Less confusion between product docs and AI harness docs.
- Easier context loading for agents.

**Negative**

- Existing links need updating.
- During transition, both old and new locations may coexist.

## Actions

| Horizon | Action |
|---------|--------|
| Short | Update documentation index to explain the taxonomy |
| Short | Move canonical docs to `Documentation/` |
| Medium | Replace old `docs/` files with redirects or remove them after links are updated |

## References

- `Documentation/AI-Harness/research/AI-Research.md`
- `Documentation/AI-Harness/documentation-index.md`
