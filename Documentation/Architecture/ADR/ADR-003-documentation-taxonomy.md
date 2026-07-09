# ADR-003: Documentation Taxonomy and AI Harness Separation

## Status

Accepted (Revised 2026-07-08)

## Context

The repository had two broad documentation folders: `Documentation/` and `docs/`. Product, architecture, harness, ADRs and technical docs were split across both, which made it unclear where new documents should live.

The project also uses machine-facing AI tool artifacts for rules and skills. Originally, only Cursor was supported (`.cursor/rules/*.mdc`, `.cursor/skills/*/SKILL.md`). The AI Harness Multi-Tool Evaluation (2026-07-08) validated that the Harness governance methodology is tool-agnostic and that multiple AI tools can coexist in the same repository while consuming the same governance content through tool-specific projections.

## Decision

Use `Documentation/` as the canonical home for durable project documentation:

- `Documentation/Product/` for PRD, PM and product planning.
- `Documentation/Architecture/` for architecture, domain, ERD and ADRs.
- `Documentation/Technical/` for runbooks, migration plans and technical references.
- `Documentation/AI-Harness/` for AI workflow documentation, SDD templates and review prompts.
- `Documentation/State.md` for current operational state.

Keep tool-specific machine-facing artifacts in each tool's native directory:

**Cursor:**
- `.cursor/rules/` — rules in `.mdc` format with `alwaysApply`/`globs` frontmatter
- `.cursor/skills/` — skills in `SKILL.md` format with `name`/`description` frontmatter

**Cline:**
- `.clinerules/` — rules in `.md` format with `paths` frontmatter (or no frontmatter for always-on)
- `.cline/skills/` — skills in `SKILL.md` format with `name`/`description` frontmatter (identical content to Cursor)

The taxonomy generalizes to any future AI tool: each tool may have its own machine-facing directories using its native format conventions, while `Documentation/` remains the tool-agnostic canonical home for durable project documentation.

Keep old `docs/` files temporarily as compatibility redirects during transition.

### Revision Notes (2026-07-08)

The original decision established `.cursor/` as the home for machine-facing Cursor artifacts. This revision generalizes the principle: `.cursor/` remains the Cursor implementation directory, and other AI tools may have their own equivalent directories. The fundamental separation — `Documentation/` for durable documentation, tool-specific directories for machine-facing artifacts — is preserved and extended.

This revision follows the AI Harness Multi-Tool Evaluation, which validated that:
- Rule content (guardrail text) and skill content (workflow procedures) are Harness Assets — tool-agnostic governance content defined once and consumed by all tools.
- File format, frontmatter schema, and storage location are Tool Assets — owned by each tool's implementation.
- `AGENTS.md` is a Shared Asset — Harness-authored content consumed identically by all tools.

## Consequences

**Positive**

- Clearer location rules for new documentation.
- Less confusion between product docs and AI harness docs.
- Easier context loading for agents.
- Multi-tool support without redesigning the documentation taxonomy — each new AI tool adds its own machine-facing directory without affecting `Documentation/` or existing tool directories.

**Negative**

- Existing links need updating.
- During transition, both old and new locations may coexist.
- Rule content and skill content must be kept in sync across tool projections. The `documentation-update` skill's multi-tool path drift check mitigates this risk.

## Actions

| Horizon | Action |
|---------|--------|
| Short | Update documentation index to explain the taxonomy |
| Short | Move canonical docs to `Documentation/` |
| Medium | Replace old `docs/` files with redirects or remove them after links are updated |

## References

- `Documentation/AI-Harness/research/AI-Research.md`
- `Documentation/AI-Harness/documentation-index.md`
- `Documentation/Architecture/ADR/ADR-007-harness-asset-taxonomy.md`
- `Documentation/SDD/ai-harness-multi-tool/design.md`
- `Documentation/SDD/ai-harness-multi-tool/verification.md`
