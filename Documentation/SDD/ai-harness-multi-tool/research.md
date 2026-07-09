# Research — AI Harness Multi-Tool Evaluation

> Feature SDD: `Documentation/SDD/ai-harness-multi-tool/`
> Generated SDD artifacts are written in English.
> **Phase:** Research (complete)
> **Lifecycle position:** Research → Specify → Design → Tasks → SDD Pre-Execution Review → Execute → Verify → Documentation Follow-Up → Reporting → Teacher Guide
> **Input from:** [Documentation Review Report](../../AI-Harness/research/review-tool-agnostic-harness.md), [PRD](../../Product/PRD.md), [PM](../../Product/PM_DocOrgano.md), [State](../../State.md)

---

## 1. Executive Summary

This Research evaluated Cline's capabilities for rule loading, skill invocation, context management, project configuration, and command integration against the existing Doc Organo AI Harness, which is implemented using Cursor-specific conventions.

The central finding is that **Cline and Cursor share a high degree of conceptual compatibility** at the methodology level. Both tools use markdown-based rule files with YAML frontmatter, both use `SKILL.md` files with `name`/`description` frontmatter for skills, both auto-load `AGENTS.md`, and both support on-demand skill loading. Cline additionally supports conditional rules via a `paths` frontmatter field that is functionally analogous to Cursor's `globs` field, and Cline explicitly detects multiple rule formats including `.cursorrules` and `AGENTS.md`.

However, **structural differences in directory layout, file format, and loading mechanisms create real compatibility gaps**:

- Cursor uses `.cursor/rules/*.mdc` with `alwaysApply`/`globs` frontmatter; Cline uses `.clinerules/*.md` with optional `paths` frontmatter (no frontmatter = always-on).
- Cursor uses `.cursor/skills/*/SKILL.md`; Cline uses `.cline/skills/*/SKILL.md` (also supporting `.clinerules/skills/` and `.claude/skills/`).
- Cline has no documented support for `.mdc` file format; it processes `.md` and `.txt` files.
- Cline introduces additional capabilities not present in the current Cursor Harness: `.clineignore`, Memory Bank methodology, slash commands (`/newtask`, `/smol`, `/deep-planning`, `/newrule`), Plan/Act mode, skill/rule toggles, and global vs. project configuration scopes.

The Research confirms that the existing Harness **can preserve Cursor as the reference implementation** and **can support Cline without compromising its current governance**, provided the Design phase resolves the directory, format, and path-reference differences identified in this report. The governance methodology (authority hierarchy, SDD lifecycle, verification governance, documentation routing) is tool-agnostic by design and requires no changes in principle.

---

## 2. Research Scope

### 2.1 Objective

Gather evidence on how the existing AI Harness can evolve to support Cline as an additional AI tool while preserving the current Cursor implementation as the reference architecture.

### 2.2 In Scope

- Cline's rule organization, loading, and formatting mechanisms.
- Cline's skill execution, discovery, and invocation mechanisms.
- Cline's project context management (`.clineignore`, `@` mentions, Memory Bank).
- Cline's command integration (slash commands).
- Cline's configuration mechanisms (global vs. project, environment variables).
- Comparative analysis with the current Cursor implementation (`.cursor/rules/`, `.cursor/skills/`, `AGENTS.md`).
- Evaluation of all 14 Research Hypotheses from the Documentation Review.
- Identification of risks, open questions, and decision areas for the Design phase.

### 2.3 Out of Scope

- Redesigning the Harness architecture.
- Proposing a migration strategy.
- Creating implementation tasks.
- Updating project documentation, ADRs, Rules, or Skills.
- Evaluating tools other than Cursor and Cline (future extensibility is noted but not researched).

---

## 3. Sources Consulted

### 3.1 Primary Sources (Mandatory)

| # | Source | URL | Topics covered |
|---|--------|-----|----------------|
| 1 | Cline Overview | `https://docs.cline.bot/cline-overview` | What Cline is, model access, applications (CLI, Kanban, VS Code, JetBrains), SDK, IDE support, enterprise |
| 2 | Configuration | `https://docs.cline.bot/getting-started/config` | Global (`~/.cline/`) and project (`.cline/`) config layout, environment variables, CLI config, security notes |
| 3 | Rules | `https://docs.cline.bot/customization/cline-rules` | Rule types, storage locations, conditional rules with `paths`, toggling, writing effective rules, AGENTS.md detection |
| 4 | Skills | `https://docs.cline.bot/customization/skills` | Progressive loading, SKILL.md format, skill structure, triggering, storage locations, bundled files, toggling |
| 5 | .clineignore | `https://docs.cline.bot/customization/clineignore` | Pattern syntax, what to exclude, how it works, @ mention exceptions, multi-root support |
| 6 | Working with Files | `https://docs.cline.bot/core-workflows/working-with-files` | @ mentions (file/folder), drag & drop, context menu commands, terminal commands, source control commands |
| 7 | Commands | `https://docs.cline.bot/core-workflows/using-commands` | Slash commands: `/newtask`, `/smol`, `/newrule`, `/deep-planning`, `/reportbug`; skill triggering via slash commands |
| 8 | Tools | `https://docs.cline.bot/tools-reference/all-cline-tools` | Built-in tools (bash, editor, read_files, apply_patch, search, fetch_web, ask_question), MCP tools, custom tools, approval policies |
| 9 | Memory Bank | `https://docs.cline.bot/best-practices/memory-bank` | Memory Bank methodology, file structure, core files, key commands, context window management, custom instructions |

### 3.2 Secondary Sources

| # | Source | URL | Topics consulted |
|---|--------|-----|------------------|
| 1 | Plan & Act Mode | `https://docs.cline.bot/core-workflows/plan-and-act` | Dual-mode system, Plan mode (read-only), Act mode (execute), model selection per mode, task sizing guidance |

### 3.3 Project Artifacts Reviewed

| Artifact | Purpose |
|----------|---------|
| `Documentation/State.md` | Current operational truth, active epic, next steps |
| `Documentation/Product/PRD.md` | Product definition, assumptions, constraints, open questions |
| `Documentation/Product/PM_DocOrgano.md` | Backlog, feature item, risks, sequencing |
| `Documentation/AI-Harness/research/review-tool-agnostic-harness.md` | Documentation review: gaps, assumptions, hypotheses, ownership classification |
| `.cursor/rules/*.mdc` (6 files) | Current Cursor rule format and content |
| `.cursor/skills/*/SKILL.md` (7 skills) | Current Cursor skill format and content |
| `Documentation/AI-Harness/Harness-Design/harness-architecture.md` | Target architecture, component responsibilities, context loading policy |
| `Documentation/AI-Harness/Harness-Design/rules-strategy.md` | Rule ownership model, inventory, design standard |
| `Documentation/AI-Harness/Harness-Design/skills-strategy.md` | Skill ownership model, inventory, design standard |
| `Documentation/Architecture/ADR/ADR-003-documentation-taxonomy.md` | Accepted decision establishing `.cursor/` for machine-facing Cursor artifacts |
| `AGENTS.md` | Agent bootstrap, authority hierarchy, repository map |

---

## 4. Research Methodology

### 4.1 Evidence Types

This Research distinguishes three types of evidence:

1. **Documented Fact** — Information explicitly stated in the official Cline documentation. Referenced by source page and section.
2. **Observed Behavior (Inference)** — Information inferred from official examples, recommended practices, or structural patterns in the documentation. Clearly labeled as inference.
3. **Architectural Assessment** — Evaluation of the impact of each finding on the current AI Harness. Always references the current Cursor implementation. Does not propose solutions.

### 4.2 Method

1. Fetched and analyzed all 8 mandatory Cline documentation pages.
2. Fetched 1 secondary source (Plan & Act Mode) for additional context management evidence.
3. Reviewed all project artifacts listed in §3.3.
4. Extracted documented facts about Cline's rule loading, skill invocation, context management, configuration, and command mechanisms.
5. Mapped each Cline capability to the corresponding Cursor capability.
6. Evaluated each of the 14 Research Hypotheses from the Documentation Review.
7. Identified risks, open questions, and decision areas for the Design phase.

---

## 5. Operational Model

### 5.1 How Cline Organizes Rules

**Documented Fact (Rules page):**

Cline recognizes rules from multiple sources:

| Rule Type | Location | Description |
|-----------|----------|-------------|
| Cline Rules | `.clinerules/` | Primary rule format |
| Cursor Rules | `.cursorrules` | Automatically detected |
| Windsurf Rules | `.windsurfrules` | Automatically detected |
| AGENTS.md | `AGENTS.md`, `~/.agents/AGENTS.md` | Standard format for cross-tool compatibility |

Cline processes all `.md` and `.txt` files inside `.clinerules/`, combining them into a unified set of rules. Numeric prefixes (like `01-coding.md`) help organize files but are optional. All detected rule types appear in the Rules panel, where they can be toggled individually.

**Documented Fact:** Rules without YAML frontmatter are always active. Rules with frontmatter containing a `paths` field are conditional — they activate only when the current context matches the specified glob patterns.

**Documented Fact:** Cline combines workspace and global rules. Workspace rules take precedence when they conflict with global rules.

### 5.2 How Rules Are Loaded

**Documented Fact (Rules page):**

Cline loads rules as persistent context across all conversations. Rules are consumed as context tokens — the documentation warns to keep rules concise and link to external documentation when detailed reference is needed.

For conditional rules, Cline evaluates the following context signals:
1. File paths mentioned in the user's prompt.
2. Files currently open in the editor.
3. Files visible in active editor panes.
4. Files Cline has created, modified, or deleted during the task.
5. Files Cline is about to edit (pending operations).

When a conditional rule activates, a notification appears: "Conditional rules applied: workspace:frontend-rules.md".

**Inference:** Cline's rule loading is context-aware and dynamic — rules activate and deactivate during a task as the set of touched files changes. This is more granular than Cursor's `globs` mechanism, which is file-pattern-based at rule-load time.

### 5.3 How Skills Are Executed

**Documented Fact (Skills page):**

Skills use progressive loading with three levels:

| Level | When Loaded | Token Cost |
|-------|-------------|------------|
| Metadata | Always (at startup) | ~100 tokens per skill (name + description from YAML frontmatter) |
| Instructions | When skill is triggered | Under 5k tokens (SKILL.md body) |
| Resources | As needed | Effectively unlimited (bundled files accessed via `read_file` or executed scripts) |

When a user sends a message, Cline sees a list of available skills with their descriptions. If the request matches a skill's description, Cline activates it using the `use_skill` tool, which loads the full instructions from `SKILL.md`.

Skills can also be invoked explicitly via slash commands (e.g., `/aws-deploy`).

**Documented Fact:** Skills are enabled by default when discovered and can be toggled on or off without deleting the skill directory.

**Documented Fact:** When a global skill and project skill have the same name, the global skill takes precedence.

### 5.4 How Project Context Is Managed

**Documented Fact (.clineignore page):**

The `.clineignore` file tells Cline which files and directories to skip when analyzing the codebase. It works like `.gitignore` — create a file named `.clineignore` in the project root, add patterns for files to exclude. Without a `.clineignore`, Cline may load the entire project into context, including dependencies, build artifacts, and generated files.

**Documented Fact:** `.clineignore` is separate from `.gitignore`. Files tracked by Git but irrelevant to Cline should go in `.clineignore`. Ignored files can still be referenced explicitly via `@` mentions.

**Documented Fact (Working with Files page):**

Cline uses `@` mentions for context:
- `@/path/to/file` — Cline sees the complete file content.
- `@/path/to/folder/` — Cline sees the folder structure and all file contents.
- In multi-root workspaces: `@workspace-name:/path/to/file`.

Drag & drop, context menu commands (Add to Cline, Fix with Cline, Explain with Cline, Improve with Cline), and terminal "Add to Cline" are also supported.

**Documented Fact (Memory Bank page):**

Memory Bank is a documentation methodology using structured markdown files in a `memory-bank/` directory:
- `projectbrief.md` — Foundation document.
- `productContext.md` — Why the project exists.
- `activeContext.md` — Current work focus (updates most frequently).
- `systemPatterns.md` — Architecture & patterns.
- `techContext.md` — Tech stack & setup.
- `progress.md` — Status & milestones.

Memory Bank instructions are stored in a Cline Rules file (e.g., `.clinerules/memory-bank.md`). Commands: "follow your custom instructions", "initialize memory bank", "update memory bank".

**Documented Fact:** Memory Bank is described as tool-agnostic: "Memory Bank is a documentation methodology that works with any AI that can read docs. Commands may differ but the approach works across tools."

### 5.5 How Memory Bank Works

**Documented Fact (Memory Bank page):**

Memory Bank transforms Cline from a stateless assistant into a persistent development partner through structured markdown files. The methodology relies on the AI reading all Memory Bank files at the start of every task.

Memory Bank updates occur when:
1. Discovering new project patterns.
2. After implementing significant changes.
3. When user requests with "update memory bank" (MUST review ALL files).
4. When context needs clarification.

The documentation recommends using Memory Bank alongside `/newtask` and `/smol` slash commands for context window management.

**Inference:** Memory Bank is conceptually similar to the Harness's `Documentation/State.md` + `AGENTS.md` pattern, but more structured and prescriptive. The Harness already achieves cross-session persistence through `State.md` and durable documentation; Memory Bank represents an alternative methodology, not a gap.

### 5.6 How Commands Are Integrated

**Documented Fact (Commands page):**

Cline provides built-in slash commands:

| Command | What It Does |
|---------|-------------|
| `/newtask` | Start fresh task with distilled context from current conversation |
| `/smol` (alias `/compact`) | Compress conversation history while preserving essential context |
| `/newrule` | Create a rule file to teach Cline preferences |
| `/deep-planning` | Investigate codebase, plan thoroughly, then create implementation task |
| `/reportbug` | Report a bug with diagnostic info |

**Documented Fact:** Enabled skills can also be triggered via slash commands (e.g., `/aws-deploy`). This provides a fast path to skill-specific guidance.

**Inference:** The `/deep-planning` command is conceptually related to the Harness's SDD Plan phase. The `/newtask` command is conceptually related to session handoff in the Reporting phase. Neither maps exactly to a Cursor-specific command in the current Harness.

### 5.7 How File Access Works

**Documented Fact (Tools page):**

Cline's built-in tools (ClineCore) include:

| Tool | Description |
|------|-------------|
| `bash` | Execute shell commands |
| `editor` | View and edit files |
| `read_files` | Batch read multiple files |
| `apply_patch` | Apply unified diffs to files |
| `search` | Ripgrep-powered codebase search |
| `fetch_web` | HTTP requests with HTML-to-markdown conversion |
| `ask_question` | Ask the user for input |

**Documented Fact:** MCP tools from `.cline/mcp.json` are loaded alongside built-ins. Custom tools can be added through plugins (SDK/CLI/Kanban only, not VS Code/JetBrains).

**Documented Fact:** The documentation notes that older docs/examples reference XML-style names like `read_file`, `replace_in_file`, or `execute_command`, but the current SDK/ClineCore runtime uses `read_files`, `apply_patch`, `bash`, etc.

**Inference:** The legacy tool names (`read_file`, `replace_in_file`, `execute_command`) correspond to the tool names used in the current Cline system prompt. This suggests the VS Code extension may still use the XML-style tool interface while the SDK has moved to newer names.

### 5.8 What Configuration Mechanisms Exist

**Documented Fact (Config page):**

Cline configuration lives in two scopes:

| Scope | Location | Purpose |
|-------|----------|---------|
| Global | `~/.cline/` | Applies globally across all Cline applications (IDE, CLI, SDK) |
| Project | `.cline/` | Applies only to the current workspace; commit for team sharing |

Global configuration structure:
```
~/.cline/
├── data/
│   └── settings/
│       ├── providers.json
│       ├── global-settings.json
│       └── cline_mcp_settings.json
├── rules/
├── skills/
├── hooks/
├── agents/
├── plugins/
├── cron/
└── workflows/
```

Project configuration structure:
```
.cline/
├── rules/
├── skills/
├── hooks/
├── agents/
├── plugins/
└── cron/
```

**Documented Fact:** Cline also discovers rules, hooks, plugins, and workflows from `~/Documents/Cline/` for compatibility.

**Documented Fact:** Environment variables include `CLINE_DATA_DIR`, `CLINE_COMMAND_PERMISSIONS` (JSON policy restricting shell commands), `CLINE_HOOKS_DIR`, and others.

**Documented Fact:** Security note: "Only use rules, hooks, skills, and plugins from sources you trust. Hooks and plugins can execute code."

---

## 6. Organization Model

### 6.1 Rules

**Documented Fact (Rules page):**

Cline recommends:
- Workspace rules in `.clinerules/` at the project root.
- One concern per file (e.g., `coding.md` for style, `testing.md` for test requirements, `architecture.md` for structural decisions).
- Numeric prefixes for ordering (optional).
- Always-on rules in files without frontmatter.
- Conditional rules in files with `paths` frontmatter.

Recommended directory structure:
```
.clinerules/
├── api-endpoints.md
├── database-models.md
├── react-components.md
└── universal.md          # No frontmatter = always active
```

**Inference:** Cline's recommended organization is flat (all rules in `.clinerules/`) rather than nested. This differs from the current Cursor implementation, which uses `.cursor/rules/` with `.mdc` files. The "one concern per file" guidance aligns with the Harness's existing rule design standard.

### 6.2 Skills

**Documented Fact (Skills page):**

Cline recommends:
- Project skills in `.cline/skills/` (recommended), also supports `.clinerules/skills/` and `.claude/skills/`.
- Global skills in `~/.cline/skills/`.
- Each skill is a directory containing `SKILL.md` with required `name` and `description` frontmatter.
- `name` must exactly match the directory name (kebab-case).
- `description` max 1024 characters.
- Keep `SKILL.md` under 5k tokens; split into `docs/` for additional content.
- Supporting files in `docs/`, `templates/`, `scripts/` subdirectories.

**Inference:** Cline's skill organization is nearly identical to Cursor's `.cursor/skills/*/SKILL.md` pattern. The format, frontmatter fields, and directory-per-skill structure are the same. The only difference is the parent directory path (`.cline/skills/` vs `.cursor/skills/`).

### 6.3 Project Documentation

**Documented Fact:** Cline documentation does not prescribe a specific project documentation structure beyond rules, skills, and `.clineignore`. The Memory Bank methodology suggests a `memory-bank/` directory, but this is a best-practice recommendation, not a configuration requirement.

**Inference:** Cline does not impose a documentation taxonomy. The Harness's `Documentation/` taxonomy (established by ADR-003) would work unchanged — Cline can read any file via `@` mentions or `read_file` tool.

### 6.4 Context

**Documented Fact:** Cline manages context through:
1. `.clineignore` — controls which files are auto-loaded.
2. `@` mentions — explicit file/folder references.
3. Rule auto-loading — always-on and conditional.
4. Skill progressive loading — metadata always, instructions on-demand.
5. `/smol` and `/newtask` — context compression and handoff.
6. Memory Bank — structured cross-session persistence.

**Inference:** Cline has more explicit context management mechanisms than what is documented in the current Cursor Harness. The Harness's `token-economy.mdc` rule and `doc-organo-context` skill serve similar purposes but are less structured than Cline's built-in tooling.

### 6.5 Project Structure

**Documented Fact:** Cline expects:
```
your-project/
├── .clinerules/          # Workspace rules
├── .cline/               # Project configuration (skills, hooks, agents, plugins)
├── .clineignore          # File exclusion patterns
├── AGENTS.md             # Cross-tool bootstrap
└── ...                   # Project source code
```

**Inference:** Cline uses `.clinerules/` for rules and `.cline/` for skills and other configuration. This is a different directory structure from the current Cursor implementation, which uses `.cursor/rules/` and `.cursor/skills/`. Both tools can coexist in the same repository because the directories do not conflict.

### 6.6 Knowledge Organization

**Documented Fact (Memory Bank page):** Cline recommends organizing knowledge in hierarchical markdown files: foundation (`projectbrief.md`) → context (`productContext.md`, `activeContext.md`) → technical (`systemPatterns.md`, `techContext.md`) → progress (`progress.md`).

**Inference:** The Harness's existing knowledge organization (PRD, PM, State, Architecture, Technical, AI-Harness, SDD) is more granular and domain-specific than the Memory Bank pattern. The two are not in conflict — Memory Bank is a generic methodology; the Harness's documentation taxonomy is project-specific. ADR-003's taxonomy would remain authoritative.

---

## 7. Limitations

### 7.1 Rules

**Documented limitations:**

| Limitation | Source | Detail |
|------------|--------|--------|
| Rules consume context tokens | Rules page | "Avoid lengthy explanations or pasting entire style guides. Keep rules concise." |
| `paths` is the only supported conditional | Rules page | "Currently, `paths` is the supported conditional." |
| Invalid YAML fails open | Rules page | "If frontmatter can't be parsed, Cline fails open. The rule activates with raw content visible to help debugging." |
| No `alwaysApply` boolean field | Rules page (inferred from structure) | Always-on behavior is determined by absence of frontmatter, not by an explicit boolean field |
| No `.mdc` file format support documented | Rules page | Cline processes `.md` and `.txt` files; `.mdc` is not mentioned |
| Empty `paths: []` disables rule | Rules page | "means the rule never activates" — this is a workaround, not a first-class disable mechanism |

**Inference:** Cline's conditional rules are less expressive than Cursor's frontmatter, which supports both `alwaysApply: true/false` and `globs` simultaneously. In Cline, a rule is either always-on (no frontmatter) or conditional (has `paths`); there is no explicit `alwaysApply` field.

### 7.2 Skills

**Documented limitations:**

| Limitation | Source | Detail |
|------------|--------|--------|
| SKILL.md body must be under 5k tokens | Skills page | "Keep SKILL.md under 5k tokens. If your skill needs more content, split it into separate files." |
| `description` max 1024 characters | Skills page | Explicit limit |
| `name` must match directory name exactly | Skills page | "must exactly match the directory name" |
| Global skill takes precedence over project skill with same name | Skills page | Opposite of what some might expect; project-level override is not supported |
| No skill dependency mechanism documented | Skills page (inferred) | Skills are independent; no way to declare that one skill requires another |

### 7.3 Context

**Documented limitations:**

| Limitation | Source | Detail |
|------------|--------|--------|
| `.clineignore` does not support explicit `@` mention blocking | .clineignore page | "You can still reference ignored files explicitly using @ mentions." |
| No documented context window size management policy | Memory Bank page (inferred) | Memory Bank is a workaround for context limits, not a built-in context management feature |
| No documented token budget per rule or skill | Rules/Skills pages | Only approximate costs are given (~100 tokens metadata, <5k instructions) |

### 7.4 Memory

**Documented limitations:**

| Limitation | Source | Detail |
|------------|--------|--------|
| Memory Bank is a methodology, not a built-in feature | Memory Bank page | "Memory Bank is a documentation methodology" — requires custom instructions |
| Memory Bank requires manual commands | Memory Bank page | "follow your custom instructions", "initialize memory bank", "update memory bank" are manual triggers |
| No automatic Memory Bank updates | Memory Bank page | Updates occur on user request or significant milestones, not automatically |

### 7.5 Commands

**Documented limitations:**

| Limitation | Source | Detail |
|------------|--------|--------|
| Limited built-in slash commands | Commands page | Only 5 built-in commands documented (`/newtask`, `/smol`, `/newrule`, `/deep-planning`, `/reportbug`) |
| No custom command creation documented | Commands page (inferred) | Skills can be triggered via slash commands, but creating standalone custom commands is not documented |
| `/deep-planning` prompt is model-dependent | Plan & Act page | "The deep planning prompt is optimized for each model family" — behavior may vary |

### 7.6 Configuration

**Documented limitations:**

| Limitation | Source | Detail |
|------------|--------|--------|
| Global rules directory varies by OS | Rules page | Windows: `Documents\Cline\Rules`; macOS/Linux: `~/Documents/Cline/Rules` |
| Linux/WSL global rules may be in alternate location | Rules page | "If you don't find global rules in `~/Documents/Cline/Rules`, check `~/Cline/Rules`" |
| Plugins execute code | Config page | "Hooks and plugins can execute code. Review them like any other executable artifact." |
| Custom tools are SDK/CLI/Kanban only | Tools page | "This feature currently only applies to Cline SDK, CLI, and Kanban. This feature is not applicable on VSCode and JetBrains Extension for now." |

### 7.7 Project Organization

**Documented limitations:**

| Limitation | Source | Detail |
|------------|--------|--------|
| No documented project documentation structure prescription | All pages (inferred) | Cline does not impose a documentation taxonomy beyond rules and skills directories |
| Rule type coexistence may cause duplication | Rules page (inferred) | If both `.clinerules/` and `.cursorrules` exist, both are detected — potential for conflicting or duplicated rules |
| No documented conflict resolution between rule types | Rules page (inferred) | Documentation states all detected types "appear in the Rules panel, where you can toggle them individually" but does not describe precedence between types |

---

## 8. Cursor × Cline Comparative Analysis

### 8.1 Compatible Capabilities

| Capability | Cursor (current) | Cline (documented) | Compatibility |
|------------|------------------|--------------------|---------------|
| Skill file format | `SKILL.md` with `name`/`description` frontmatter | `SKILL.md` with `name`/`description` frontmatter | **Fully compatible** — identical format |
| Skill directory structure | One directory per skill with `SKILL.md` + optional subdirs | One directory per skill with `SKILL.md` + optional `docs/`, `templates/`, `scripts/` | **Fully compatible** — same structure |
| Skill loading model | On-demand, triggered by user/agent | On-demand, triggered by description match or slash command | **Conceptually compatible** — both load on demand |
| AGENTS.md auto-loading | Auto-loaded as project instructions | Auto-detected as rule type (`AGENTS.md`, `~/.agents/AGENTS.md`) | **Fully compatible** — Cline explicitly detects `AGENTS.md` |
| Rule purpose | Persistent guardrails (always-on or file-scoped) | Persistent guardrails (always-on or path-scoped) | **Conceptually compatible** — same purpose |
| Rule content format | Markdown body after frontmatter | Markdown body after frontmatter | **Fully compatible** |
| Skill progressive disclosure | Skills point to deeper docs | Skills support `docs/` subdirectory for additional content | **Compatible** — Cline adds explicit subdirectory convention |
| MCP support | Not documented in current Harness | `.cline/mcp.json` for MCP tools | **Cline additional capability** — no conflict |

### 8.2 Partially Compatible Capabilities

| Capability | Cursor (current) | Cline (documented) | Gap |
|------------|------------------|--------------------|-----|
| Rule storage location | `.cursor/rules/*.mdc` | `.clinerules/*.md` or `.cline/rules/*.md` | Different directory and file extension |
| Rule frontmatter | `alwaysApply: true/false`, `globs: pattern`, `description` | `paths: [patterns]` (conditional) or no frontmatter (always-on) | Different frontmatter schema; `alwaysApply` has no direct equivalent; `globs` maps to `paths` |
| Skill storage location | `.cursor/skills/*/SKILL.md` | `.cline/skills/*/SKILL.md` (also `.clinerules/skills/`, `.claude/skills/`) | Different parent directory |
| Rule file format | `.mdc` extension | `.md` or `.txt` extension | Cline does not document `.mdc` support |
| Rule toggling | Not documented in current Harness | Toggle individual rules in Rules panel | Cline adds toggle capability |
| Skill toggling | Not documented in current Harness | Toggle individual skills | Cline adds toggle capability |
| Global vs. project scope | Not documented in current Harness (all rules in `.cursor/rules/`) | Global (`~/.cline/`) + project (`.cline/`) | Cline adds global scope |

### 8.3 Incompatible Capabilities

| Capability | Cursor (current) | Cline (documented) | Incompatibility |
|------------|------------------|--------------------|-----------------|
| `.mdc` file format | Native format for Cursor rules | Not documented; Cline processes `.md` and `.txt` | `.mdc` files may not be recognized by Cline |
| `alwaysApply: true` frontmatter | Explicit boolean field for always-on rules | No equivalent field; always-on is implicit (no frontmatter) | An `.mdc` file with `alwaysApply: true` would have frontmatter, which Cline would attempt to parse as conditional |
| `globs` frontmatter field | Cursor-specific glob scoping | `paths` field with glob patterns — different field name | Frontmatter field name differs; Cline looks for `paths`, not `globs` |
| Cursor-specific rule auto-injection | Cursor auto-injects `alwaysApply: true` rules into agent context | Cline loads all always-on rules as context tokens | Mechanism differs; Cline does not have "injection" semantics — it loads rules as context |

### 8.4 Missing Equivalents

| Capability in Cursor | Cline equivalent | Status |
|---------------------|------------------|--------|
| `.cursor/rules/` directory | `.clinerules/` directory | Cline has equivalent but different path |
| `.cursor/skills/` directory | `.cline/skills/` directory | Cline has equivalent but different path |
| `.mdc` file format | `.md` file format | No `.mdc` support documented |
| `alwaysApply` boolean | Absence/presence of frontmatter | Different mechanism |
| `globs` frontmatter | `paths` frontmatter | Different field name, similar semantics |

| Capability in Cline | Cursor equivalent | Status |
|---------------------|-------------------|--------|
| `.clineignore` | Not documented in current Harness | **No equivalent** — Cursor may handle this differently or not at all |
| Memory Bank | `State.md` + durable docs (conceptually similar) | **Partial** — Harness achieves cross-session persistence differently |
| `/newtask` command | Not documented | **No equivalent** |
| `/smol` command | Not documented | **No equivalent** |
| `/deep-planning` command | Not documented | **No equivalent** — Harness uses SDD Plan phase |
| `/newrule` command | Not documented | **No equivalent** |
| Plan/Act mode | Not documented in current Harness | **No equivalent** — Harness uses SDD lifecycle phases |
| Rule/skill toggles | Not documented | **No equivalent** |
| Global vs. project scope | Not documented | **No equivalent** in current Harness |
| Conditional rule activation notification | Not documented | **No equivalent** |
| Multi-format rule detection (`.cursorrules`, `.windsurfrules`, `AGENTS.md`) | Cursor uses `.cursor/rules/*.mdc` only | **No equivalent** — Cursor does not detect other formats |

### 8.5 Behavioral Differences

| Behavior | Cursor | Cline | Impact |
|----------|--------|-------|--------|
| Rule loading trigger | Cursor auto-loads based on `alwaysApply` and `globs` at file-access time | Cline evaluates context (open files, edited files, message paths) dynamically | Cline's conditional rules may activate mid-task as files are touched; Cursor's `globs` may be more static |
| Skill trigger | User or agent triggers by name in Cursor interface | Auto-matching by description OR explicit slash command | Cline has two trigger paths; Cursor has one |
| Context window management | Harness relies on `token-economy.mdc` rule | Cline has `/smol`, `/newtask`, and Memory Bank | Different mechanisms for similar problem |
| Plan/Act separation | Harness uses SDD lifecycle (Research → Plan → SDD → Execute → Verify) | Cline has built-in Plan/Act mode separation | Conceptually similar but operationally different |
| `AGENTS.md` loading | Auto-loaded as project instructions by Cursor | Auto-detected as a rule type by Cline | Both load it, but Cline treats it as a rule (toggleable); Cursor treats it as bootstrap |
| Global skill precedence | Not applicable (no global scope in current Harness) | Global skill takes precedence over project skill with same name | Cline's precedence rule is the opposite of common expectations |

---

## 9. Harness Impact Assessment

### 9.1 Cursor-Specific Assumptions

The Documentation Review identified 30 architectural assumptions (A-1 through A-30). This Research validates their status:

| Assumption group | Affected assumptions | Impact assessment |
|------------------|---------------------|-------------------|
| **Path assumptions (A-1 to A-5)** | `.cursor/rules/`, `.cursor/skills/`, `.cursor/` paths referenced in 18+ files | **Directly impacted** — Cline uses different paths (`.clinerules/`, `.cline/skills/`). Path references in governance docs would need accommodation. |
| **Rule loading assumptions (A-6 to A-10)** | `.mdc` format, `alwaysApply`, `globs`, auto-discovery, injection | **Directly impacted** — Cline uses `.md`/`.txt`, no `alwaysApply`, `paths` instead of `globs`. Rule format and loading mechanism differ. |
| **Skill invocation assumptions (A-11 to A-14)** | `SKILL.md` format, `name`/`description` discovery, on-demand loading | **Minimally impacted** — Cline uses the same `SKILL.md` format and discovery mechanism. Only the parent directory differs. |
| **Documentation reference assumptions (A-15 to A-20)** | "Cursor Rules Inventory", "Cursor skills" titles, ADR-003 | **Impacted** — Documentation titles and ADR-003 assume Cursor as the sole tool. Multi-tool support requires documentation accommodation. |
| **Workflow assumptions (A-21 to A-26)** | Harness lifecycle in Cursor sessions, skill invocation paths | **Partially impacted** — Workflow concepts are valid, but skill path references (`.cursor/skills/...`) would differ in Cline. |
| **Context assumptions (A-27 to A-30)** | `AGENTS.md` auto-loading, `alwaysApply` injection, single tool context, runbook | **Partially impacted** — `AGENTS.md` auto-loading is valid in Cline. `alwaysApply` injection differs. Single-tool context assumption is invalid. |

### 9.2 Tool-Specific Behaviors

| Behavior | Cursor | Cline | Harness impact |
|----------|--------|-------|----------------|
| Rule directory | `.cursor/rules/` | `.clinerules/` | Governance docs reference `.cursor/rules/` throughout — would need path accommodation |
| Rule format | `.mdc` with `alwaysApply`/`globs` | `.md` with optional `paths` | Rule content is portable; frontmatter needs adaptation |
| Skill directory | `.cursor/skills/` | `.cline/skills/` | Skill content is portable; path references in docs/templates need accommodation |
| Skill format | `SKILL.md` with `name`/`description` | `SKILL.md` with `name`/`description` | **No impact** — identical format |
| Bootstrap | `AGENTS.md` auto-loaded | `AGENTS.md` auto-detected | **No impact** — both load `AGENTS.md` |
| Context management | `token-economy.mdc` rule | `.clineignore`, `/smol`, `/newtask`, Memory Bank | Cline offers additional mechanisms; Harness can adopt selectively |
| Planning | SDD lifecycle | Plan/Act mode, `/deep-planning` | Conceptually compatible; Cline's built-in tools complement SDD |

### 9.3 Governance Impacts

| Governance area | Impact | Detail |
|-----------------|--------|--------|
| Authority hierarchy | **No change in principle** | ADRs > Architecture docs > State > SDD > Rules > Skills — this hierarchy is tool-agnostic |
| SDD lifecycle | **No change in principle** | Research → Plan → SDD → Execute → Verify → Documentation Follow-Up → Reporting → Teacher Guide — lifecycle is tool-agnostic |
| Verification governance | **No change in principle** | Gate selection, review sensors, residual risk — verification model is tool-agnostic |
| Documentation routing | **Impacted** | Path references to `.cursor/` throughout governance docs (43+ references in 18+ files) need accommodation |
| ADR-003 | **Impacted** | ADR-003 establishes `.cursor/` for machine-facing Cursor artifacts; multi-tool support requires ADR evaluation |
| Calibration workflow | **Impacted** | Pilot reports and governance improvement plans reference `.cursor/` paths; multi-tool pilots would need path accommodation |
| Rules inventory (`rules.md`) | **Impacted** | Titled "Cursor Rules Inventory"; would need renaming or generalization |
| Skills strategy | **Minimally impacted** | Skill format is identical; only path references differ |
| Contribution workflow | **Impacted** | `CONTRIBUTING-AI.md` has 6 skill path references to `.cursor/skills/` |

### 9.4 Documentation Impacts

| Document | Impact | Detail |
|----------|--------|--------|
| `harness-architecture.md` | **High** | Component Responsibilities table defines `.cursor/rules/` and `.cursor/skills/` as core harness components; Context Loading Policy references `.cursor/` paths |
| `rules-strategy.md` | **High** | Title and content scoped to "Cursor rules"; inventory table lists `.mdc` files with `alwaysApply` and `globs` columns |
| `skills-strategy.md` | **Medium** | Title and content scoped to "Cursor skills"; update triggers reference `.cursor/` paths |
| `documentation-index.md` | **Medium** | "Cursor Artifacts" section; `rules.md` described as "Cursor rule inventory" |
| `rules.md` | **Medium** | Titled "Cursor Rules Inventory" |
| `AGENTS.md` | **Medium** | References `.cursor/rules/` and `.cursor/skills/` as machine-facing asset locations |
| `CONTRIBUTING-AI.md` | **Medium** | 6 skill path references to `.cursor/skills/` |
| `State.md` | **Low** | Harness files section links to `.cursor/rules/` and `.cursor/skills/` |
| SDD templates | **Low** | `verification.md`, `teacher-guide.md`, `governance-improvement-plan.md` reference `.cursor/skills/` paths |
| Feature SDD instances | **Low** | `paciente-sql-stabilization/verification.md`, `atendimento-minimal-sql-stabilization/verification.md` reference `.cursor/skills/verifier/SKILL.md` |
| Security docs | **Low** | `plano-seguranca-informacao.md` references `security-phi.mdc` path |
| Runbook | **Low** | Lists "Cursor / VS Code" as development tooling |

### 9.5 Workflow Impacts

| Workflow | Impact | Detail |
|----------|--------|--------|
| Rule application | **Medium** | Cursor auto-injects `alwaysApply` rules; Cline loads always-on rules as context. Behavior is similar but mechanism differs. |
| Skill invocation | **Low** | Both tools load skills on-demand. Cline adds slash command trigger; Cursor uses interface trigger. Skill content is portable. |
| Session bootstrap | **Low** | Both tools auto-load `AGENTS.md`. Bootstrap sequence is portable. |
| Verification | **None** | Verification gates (build, test, SQL, API, UI, security, documentation) are tool-agnostic commands. |
| Documentation follow-up | **Low** | Path references in follow-up routing would need accommodation. |
| Reporting | **None** | Session handoff and feature reports are markdown artifacts — tool-agnostic. |
| Teacher guide | **None** | Knowledge transfer artifacts are markdown — tool-agnostic. |

### 9.6 Operational Risks

| Risk | Detail |
|------|--------|
| Rule format divergence | If rules are maintained in `.mdc` format only, Cline cannot consume them without format adaptation |
| Path reference drift | 43+ `.cursor/` references in governance docs may become ambiguous or misleading in a multi-tool context |
| Dual maintenance | If rules/skills are duplicated per tool, content drift is inevitable |
| ADR-003 conflict | ADR-003 establishes `.cursor/` for machine-facing artifacts; multi-tool support may require ADR revision or supersession |
| Toggle inconsistency | Cline allows toggling rules/skills; if Cursor does not, behavior may differ between tools for the same project |
| Global vs. project ambiguity | Cline's global scope introduces a configuration dimension not present in the current Harness |

---

## 10. Research Hypotheses Evaluation

### RH-1: A shared artifact layer for rules and skills can be defined that is not bound to a single AI tool's directory structure or file format.

**Status: Partially Supported**

**Evidence:**
- Cline and Cursor use identical `SKILL.md` format with `name`/`description` frontmatter — skill content is already shared in principle.
- Rule content (markdown body) is portable; only frontmatter schema differs.
- Cline detects multiple rule formats (`.clinerules/`, `.cursorrules`, `.windsurfrules`, `AGENTS.md`), demonstrating that multi-format coexistence is feasible.
- However, Cline does not document `.mdc` support, and the frontmatter schemas (`alwaysApply`/`globs` vs. `paths`) are not interchangeable.
- A shared artifact layer would require format reconciliation or a canonical format that both tools consume.

---

### RH-2: Tool-specific loading mechanisms can be reconciled with shared Harness artifacts without duplicating content.

**Status: Partially Supported**

**Evidence:**
- Skill loading is nearly identical between tools — both use `name`/`description` discovery and on-demand loading.
- Rule loading differs: Cursor uses `alwaysApply`/`globs`; Cline uses presence/absence of `paths` frontmatter.
- Cline's multi-format detection (including `.cursorrules` and `AGENTS.md`) shows that loading mechanisms can coexist.
- Content duplication is avoidable in principle, but the frontmatter schema difference means a single file cannot serve both tools' loading mechanisms without adaptation.
- The `AGENTS.md` file is already a shared artifact — both tools auto-load it.

---

### RH-3: The Harness can support at least two AI tools (Cursor and Cline) applying the same governance rules.

**Status: Validated**

**Evidence:**
- Both tools load rules as persistent context from markdown files.
- Both tools auto-load `AGENTS.md`.
- Cline explicitly detects `AGENTS.md` as a rule type for cross-tool compatibility.
- Rule content (the guardrail text) is markdown — portable between tools.
- The governance rules (security-phi, token-economy, update-doc, backend-architecture, ef-migrations, blazor-front) contain tool-agnostic guardrails that any AI can follow.
- Format adaptation (frontmatter schema, file extension) is needed, but the governance content itself is shareable.

---

### RH-4: The Harness can support at least two AI tools invoking the same workflow skills.

**Status: Validated**

**Evidence:**
- Both tools use `SKILL.md` with `name`/`description` frontmatter for skill discovery.
- Both tools load skills on-demand.
- Cline adds slash command invocation as an additional trigger, but the core mechanism (description-based discovery) is the same.
- Skill content (workflow procedures) is markdown — portable between tools.
- The only difference is the parent directory path (`.cursor/skills/` vs `.cline/skills/`), which is a storage location difference, not a content or format difference.
- Cline also supports `.clinerules/skills/` and `.claude/skills/` as additional skill locations, showing flexibility.

---

### RH-5: The current Cursor implementation can be preserved without regression while additional tool support is evaluated.

**Status: Validated**

**Evidence:**
- Cline and Cursor use non-conflicting directory structures (`.clinerules/` + `.cline/` vs `.cursor/rules/` + `.cursor/skills/`).
- Cline explicitly detects `.cursorrules` and `AGENTS.md` — it does not interfere with `.cursor/` artifacts.
- Adding `.clinerules/` and `.cline/` directories to the repository does not modify or remove `.cursor/` directories.
- `AGENTS.md` is shared by both tools — no conflict.
- The evaluation can proceed by adding Cline-specific directories alongside existing Cursor directories without any regression to Cursor workflows.
- The only risk is documentation ambiguity (governance docs referencing `.cursor/` paths), which is a documentation issue, not a functional regression.

---

### RH-6: Governance documentation can be made consistent with a multi-tool architecture without mass path rewriting.

**Status: Partially Supported**

**Evidence:**
- 43+ `.cursor/` references exist in 18+ files (Documented in the Review Report).
- The references are in governance docs, strategies, templates, SDD instances, and security docs.
- Cline's multi-format detection means `.cursor/rules/` and `.clinerules/` can coexist — governance docs could reference both or use a tool-agnostic abstraction.
- However, the volume of references means a mass update would be labor-intensive.
- An incremental approach (updating references as docs are touched) is feasible but risks inconsistency during transition.
- The documentation-update skill's path drift check could be extended to detect tool-specific path references.

---

### RH-7: ADR-003 can be accommodated, revised, or superseded to support multi-tool architecture.

**Status: Partially Supported**

**Evidence:**
- ADR-003 states: "Keep `.cursor/` for machine-facing Cursor artifacts only."
- This decision does not prohibit adding `.clinerules/` or `.cline/` directories — it only establishes `.cursor/` as the home for Cursor artifacts.
- ADR-003 could be **accommodated** by adding Cline-specific directories alongside `.cursor/` without modifying the ADR.
- ADR-003 could be **revised** to generalize the taxonomy for multi-tool artifacts.
- ADR-003 could be **superseded** by a new ADR that establishes a tool-agnostic artifact layer.
- The Research cannot determine which option is best — that is a Design phase decision. But all three options are feasible.

---

### RH-8: An incremental transition from the current Cursor-specific implementation to a multi-tool architecture is feasible.

**Status: Validated**

**Evidence:**
- Cline and Cursor directories do not conflict — both can coexist in the same repository.
- `AGENTS.md` is already shared.
- Skill format (`SKILL.md`) is identical — skills can be copied or symlinked.
- Rule content is portable — only frontmatter needs adaptation.
- The transition can proceed in phases:
  1. Add `.clineignore` and `.clinerules/` alongside `.cursor/`.
  2. Create Cline-compatible rule files (`.md` with `paths` frontmatter) from existing `.mdc` content.
  3. Create `.cline/skills/` with `SKILL.md` files (same content, different parent directory).
  4. Update governance documentation references.
- Each phase preserves Cursor workflows without regression.

---

### RH-9: The rule content (guardrail text) can be separated from the rule format (`.mdc`, frontmatter) to enable format portability.

**Status: Validated**

**Evidence:**
- Rule content in the current Harness is markdown text after the YAML frontmatter.
- Cline processes `.md` files with optional YAML frontmatter — the content format is compatible.
- The guardrail text (e.g., "Never commit real patient names, CPF, clinical data...") is pure markdown — no Cursor-specific syntax.
- The only format-specific elements are:
  - File extension (`.mdc` vs `.md`)
  - Frontmatter schema (`alwaysApply`/`globs` vs. `paths`)
- Both can be adapted without modifying the rule content.

---

### RH-10: The skill content (workflow procedures) can be separated from the skill format (`SKILL.md`, frontmatter) to enable format portability.

**Status: Validated**

**Evidence:**
- Skill format is identical between Cursor and Cline: `SKILL.md` with `name`/`description` frontmatter.
- Skill content (workflow procedures, step-by-step instructions, output formats) is pure markdown.
- The only path-specific references inside skill content are `.cursor/skills/` paths in cross-skill references (e.g., verifier skill references `.cursor/skills/documentation-update/SKILL.md`).
- These path references can be made relative or tool-agnostic.
- Skill content is fully portable; only cross-skill path references need accommodation.

---

### RH-11: The `alwaysApply` and `glob` scoping concepts have equivalents or alternatives in Cline.

**Status: Partially Supported**

**Evidence:**
- `alwaysApply: true` → **Equivalent exists**: In Cline, rules without frontmatter are always active. This is functionally equivalent to `alwaysApply: true`.
- `alwaysApply: false` + `globs: pattern` → **Equivalent exists**: In Cline, rules with `paths` frontmatter are conditional. The `paths` field uses glob patterns similar to `globs`.
- `alwaysApply: true` + `globs: pattern` → **No direct equivalent**: In Cline, a rule is either always-on (no frontmatter) or conditional (has `paths`). There is no way to have both always-on and path-scoped behavior in the same file.
- `globs` field name → **Different field name**: Cline uses `paths`, not `globs`. The glob pattern syntax is similar but the field name differs.
- The concepts have equivalents, but the frontmatter schema is not interchangeable — a `.mdc` file with `alwaysApply: true` would be parsed by Cline as having frontmatter (conditional), not as always-on.

---

### RH-12: The `AGENTS.md` bootstrap loading mechanism has an equivalent or alternative in Cline.

**Status: Validated**

**Evidence:**
- Cline explicitly detects `AGENTS.md` as a rule type: "AGENTS.md | `AGENTS.md`, `~/.agents/AGENTS.md` | Standard format for cross-tool compatibility."
- Cline also reads global AGENTS instructions from `~/.agents/AGENTS.md`.
- The `AGENTS.md` file is auto-detected and loaded by Cline — functionally equivalent to Cursor's auto-loading.
- Cline treats `AGENTS.md` as a rule (toggleable in the Rules panel), while Cursor treats it as project instructions. The loading mechanism differs in classification but not in effect.

---

### RH-13: Existing feature SDD instances and verification artifacts with `.cursor/` references can remain interpretable in a multi-tool architecture.

**Status: Validated**

**Evidence:**
- Existing SDD instances (Paciente, Atendimento Minimal) reference `.cursor/skills/verifier/SKILL.md` — these are historical artifacts documenting what was done at the time.
- Cline can read any file via `@` mentions or `read_file` tool — `.cursor/` paths are not inaccessible to Cline.
- The references remain interpretable because they describe the tool that was used during that SDD.
- Historical artifacts should not be rewritten to reflect a multi-tool architecture they predate.
- The Harness's backward compatibility requirement (NFR-7) is satisfiable: existing artifacts remain readable and interpretable.

---

### RH-14: The Harness calibration workflow can accommodate multi-tool pilot reports.

**Status: Validated**

**Evidence:**
- The calibration workflow (Pilot Execution → Pilot Report → Implementation Plan → Review → Phased Governance Update → Consistency Audit → Next Pilot) is a process, not a tool-specific mechanism.
- Pilot reports are markdown artifacts — tool-agnostic.
- A multi-tool pilot report would document which tool was used, what was observed, and what governance improvements are recommended — the same structure as existing pilot reports.
- The calibration workflow does not depend on Cursor-specific mechanisms; it depends on the SDD lifecycle, which is tool-agnostic.

---

## 11. Risks

| # | Risk | Type | Probability | Impact | Evidence |
|---|------|------|-------------|--------|----------|
| R1 | `.mdc` format incompatibility | Technical | Medium | Medium | Cline processes `.md`/`.txt`; `.mdc` not documented |
| R2 | Frontmatter schema divergence | Technical | High | Medium | `alwaysApply`/`globs` vs. `paths` — not interchangeable |
| R3 | Path reference drift in governance docs | Governance | High | Medium | 43+ `.cursor/` references in 18+ files |
| R4 | Content duplication if rules/skills are copied per tool | Governance | Medium | High | Duplicated content creates drift risk |
| R5 | ADR-003 may require revision or supersession | Governance | Medium | Medium | ADR-003 establishes `.cursor/` for Cursor artifacts |
| R6 | Toggle behavior inconsistency | Operational | Low | Low | Cline allows toggling; Cursor behavior not documented |
| R7 | Global vs. project scope confusion | Operational | Low | Low | Cline has global scope; current Harness does not |
| R8 | Conditional rule activation differs | Behavioral | Medium | Low | Cline evaluates context dynamically; Cursor uses static globs |
| R9 | Cross-skill path references break in Cline | Technical | Medium | Medium | Skills reference `.cursor/skills/` paths internally |
| R10 | Documentation index becomes ambiguous | Governance | Medium | Low | "Cursor Artifacts" section name assumes single tool |
| R11 | Skill precedence differs (global > project in Cline) | Behavioral | Low | Low | Cline global skill takes precedence; opposite of common expectation |
| R12 | `.clineignore` absent in Cursor may cause context bloat | Operational | Low | Low | Cursor may load files that Cline would ignore |
| R13 | Multi-format rule coexistence creates confusion | Governance | Low | Medium | Both `.cursorrules` and `.clinerules/` detected by Cline |

---

## 12. Open Questions

### 12.1 Questions Answered by This Research

| Original Question ID | Question | Answer |
|----------------------|----------|--------|
| Q-1 | Does Cline support a concept equivalent to Cursor's `.cursor/rules/` directory for persistent rule loading? | **Yes** — `.clinerules/` directory with `.md`/`.txt` files |
| Q-2 | Does Cline support `alwaysApply`-style always-on rules? | **Yes** — rules without frontmatter are always active; no explicit `alwaysApply` field |
| Q-3 | Does Cline support `glob`-based file scoping for rules? | **Yes** — `paths` frontmatter field with glob patterns |
| Q-4 | Does Cline have a skill/workflow system equivalent to Cursor's `.cursor/skills/`? | **Yes** — `.cline/skills/*/SKILL.md` with identical format |
| Q-5 | What file format does Cline expect for rules and skills? | Rules: `.md`/`.txt`; Skills: `SKILL.md` — `.mdc` not documented |
| Q-6 | Does Cline auto-load `AGENTS.md`? | **Yes** — explicitly detected as a rule type |
| Q-7 | How does Cline handle context loading? | `.clineignore`, `@` mentions, progressive skill loading, `/smol`, `/newtask`, Memory Bank |
| Q-8 | Can Cline and Cursor coexist in the same repository? | **Yes** — non-conflicting directories; Cline detects `.cursorrules` and `AGENTS.md` |
| Q-9 | Should the Harness distinguish between tool-agnostic artifacts and tool-specific implementations? | **Decision area for Design phase** — Research provides evidence but does not prescribe |
| Q-13 | How should tool-specific loading mechanisms be reconciled with shared Harness content? | **Decision area for Design phase** — frontmatter schemas differ but content is portable |
| Q-14 | Should the Harness define a canonical rule/skill format, or should each tool have its own format? | **Decision area for Design phase** |

### 12.2 Questions Remaining Open

| Question ID | Question | Status |
|-------------|----------|--------|
| Q-10 | Should ADR-003 be revised, superseded, or left in place? | **Open** — Research confirms all three options are feasible; Design phase must decide |
| Q-11 | Should `rules.md` be renamed from "Cursor Rules Inventory" to a tool-agnostic name? | **Open** — depends on Design phase decision on tool-agnostic vs. tool-specific documentation |
| Q-12 | Should `rules-strategy.md` and `skills-strategy.md` be rewritten to be tool-agnostic? | **Open** — depends on Design phase decision on documentation strategy |
| Q-15 | Does multi-tool evaluation require an ADR? | **Open** — ADR governance policy suggests yes (changes to documentation taxonomy); Design phase must evaluate |
| Q-16 | Should the Harness Calibration Workflow be updated for multi-tool pilot reports? | **Open** — calibration workflow is tool-agnostic, but pilot report templates may need multi-tool fields |
| Q-17 | How should the authority hierarchy reflect the relationship between Harness assets and tool assets? | **Open** — current hierarchy is tool-agnostic; Design phase must determine if tool assets need explicit hierarchy placement |
| Q-18 | Should `effective-harness-planning` skill evaluate tool-specific assumptions during harness reviews? | **Open** — depends on Design phase decision on tool-specific evaluation |
| Q-19 | Should `documentation-update` skill's path drift check detect tool-specific path references? | **Open** — depends on Design phase decision on path reference strategy |
| Q-20 | What is the minimal set of changes needed to evaluate multi-tool support without breaking Cursor? | **Open** — Research identifies the changes but does not prescribe minimal set |
| Q-21 | Should existing SDD instances have `.cursor/` references updated? | **Answered: No** — historical artifacts should remain as-is (RH-13) |
| Q-22 | Should the 43+ `.cursor/` references be updated, or should the architecture evolve to make them consistent? | **Open** — Design phase decision |
| Q-23 | What is the rollback strategy if multi-tool evaluation reveals incompatibility? | **Open** — Research confirms coexistence is safe; rollback = remove `.clinerules/` and `.cline/` directories |
| Q-24 | Can the evaluation be done incrementally? | **Answered: Yes** — RH-8 validates incremental feasibility |

### 12.3 New Questions Identified During Research

| Question ID | Question |
|-------------|----------|
| NQ-1 | Should the Harness adopt Cline's `.clineignore` as a best practice even if Cursor is the primary tool? |
| NQ-2 | Should the Harness adopt Memory Bank methodology, or is `State.md` + durable documentation sufficient? |
| NQ-3 | Should the Harness leverage Cline's `/deep-planning` command as a complement to the SDD Plan phase? |
| NQ-4 | How should the Harness handle Cline's global vs. project scope distinction for rules and skills? |
| NQ-5 | Should the Harness define a canonical frontmatter schema that works across tools, or maintain tool-specific frontmatter? |
| NQ-6 | How should cross-skill path references (e.g., verifier referencing documentation-update) be written to work in both tools? |
| NQ-7 | Should the Harness adopt Cline's skill toggle capability as a governance practice? |

---

## 13. Findings

### 13.1 Key Findings

| # | Finding | Evidence type |
|---|---------|---------------|
| F-1 | Cline and Cursor share identical skill format (`SKILL.md` with `name`/`description` frontmatter) | Documented Fact |
| F-2 | Cline and Cursor share identical skill directory structure (one directory per skill) | Documented Fact |
| F-3 | Cline and Cursor both auto-load `AGENTS.md` | Documented Fact |
| F-4 | Cline and Cursor use different rule directories (`.clinerules/` vs `.cursor/rules/`) | Documented Fact |
| F-5 | Cline and Cursor use different rule file extensions (`.md`/`.txt` vs `.mdc`) | Documented Fact |
| F-6 | Cline and Cursor use different frontmatter schemas (`paths` vs `alwaysApply`/`globs`) | Documented Fact |
| F-7 | Cline detects multiple rule formats including `.cursorrules`, `.windsurfrules`, and `AGENTS.md` | Documented Fact |
| F-8 | Cline has additional context management mechanisms not present in current Harness (`.clineignore`, Memory Bank, `/smol`, `/newtask`) | Documented Fact |
| F-9 | Cline has Plan/Act mode and `/deep-planning` command — conceptually related to SDD lifecycle | Documented Fact |
| F-10 | Cline supports global and project scopes for rules, skills, hooks, agents, and plugins | Documented Fact |
| F-11 | Cline allows toggling rules and skills individually without deletion | Documented Fact |
| F-12 | Cline's conditional rules evaluate context dynamically (open files, edited files, message paths) | Documented Fact |
| F-13 | Cline's global skill takes precedence over project skill with same name | Documented Fact |
| F-14 | Memory Bank is described as tool-agnostic: "works with any AI that can read docs" | Documented Fact |
| F-15 | Cline and Cursor directories do not conflict — both can coexist in the same repository | Inference from non-overlapping paths |
| F-16 | Rule content (guardrail text) is pure markdown — portable between tools | Architectural Assessment |
| F-17 | Skill content (workflow procedures) is pure markdown — portable between tools | Architectural Assessment |
| F-18 | The Harness governance methodology is tool-agnostic by design and requires no changes in principle | Architectural Assessment |
| F-19 | 43+ `.cursor/` references in 18+ governance files need accommodation for multi-tool clarity | Architectural Assessment |
| F-20 | ADR-003 does not prohibit adding Cline directories — it only establishes `.cursor/` for Cursor artifacts | Architectural Assessment |

### 13.2 Architectural Ownership Classification Update

The Documentation Review proposed an initial classification. This Research validates or revises it:

| Capability | Review classification | Research validation |
|------------|----------------------|---------------------|
| Rule content (guardrail text) | Undetermined | **Harness asset** — content is tool-agnostic markdown |
| Skill content (workflow procedures) | Undetermined | **Harness asset** — content is tool-agnostic markdown |
| Path references in governance docs | Undetermined | **Undetermined** — depends on Design phase decision |
| Context loading strategy | Undetermined | **Harness asset** (strategy) + **Tool asset** (mechanism) — what to load is Harness; how to load is tool |
| Rule scoping (always-on vs. file-scoped) | Undetermined | **Harness asset** (intent) + **Tool asset** (frontmatter schema) — which rules are always-on is Harness; how to express it is tool |
| Rule format (`.mdc`, frontmatter) | Tool asset | **Confirmed Tool asset** |
| Skill format (`SKILL.md`, frontmatter) | Tool asset | **Confirmed Tool asset** — but format is shared between Cursor and Cline |
| Rule storage location | Tool asset | **Confirmed Tool asset** |
| Skill storage location | Tool asset | **Confirmed Tool asset** |
| Bootstrap loading (`AGENTS.md`) | Tool asset | **Reclassified: Shared asset** — both tools auto-load `AGENTS.md` |
| `.clineignore` | Not classified | **Tool asset (Cline-specific)** — no Cursor equivalent documented |
| Memory Bank | Not classified | **Tool-agnostic methodology** (per Cline docs) — could be Harness asset if adopted |

---

## 14. Recommendations for the Design Phase

### 14.1 Decision Areas

The following decision areas should be addressed during the Design phase. These are **areas requiring architectural decisions**, not prescribed solutions.

| # | Decision area | Context | Evidence |
|---|---------------|---------|----------|
| DA-1 | **Artifact location strategy** | Should the Harness use tool-specific directories (`.cursor/rules/` + `.clinerules/`), a shared directory, or a canonical directory with tool-specific adapters? | F-4, F-15, F-20, RH-1, RH-2, RH-8 |
| DA-2 | **Rule format strategy** | Should the Harness maintain `.mdc` files for Cursor and `.md` files for Cline, or define a canonical format? | F-5, F-6, RH-9, RH-11 |
| DA-3 | **Frontmatter schema reconciliation** | Should the Harness define a frontmatter schema that works across tools, or maintain tool-specific frontmatter? | F-6, RH-11, NQ-5 |
| DA-4 | **Path reference strategy** | Should governance docs reference tool-specific paths, tool-agnostic abstractions, or both? | F-19, RH-6, Q-22 |
| DA-5 | **ADR-003 disposition** | Should ADR-003 be accommodated, revised, or superseded? | F-20, RH-7, Q-10 |
| DA-6 | **Cross-skill reference strategy** | How should skills reference each other across tool boundaries? | R9, NQ-6, RH-10 |
| DA-7 | **Documentation title and section naming** | Should "Cursor Rules Inventory" and "Cursor Artifacts" be renamed? | F-19, Q-11, Q-12 |
| DA-8 | **Context management adoption** | Should the Harness adopt `.clineignore`, Memory Bank, or Cline's slash commands as best practices? | F-8, NQ-1, NQ-2, NQ-3 |
| DA-9 | **Global vs. project scope** | Should the Harness define a global scope for rules and skills? | F-10, NQ-4 |
| DA-10 | **Toggle governance** | Should the Harness define policy around rule/skill toggling? | F-11, NQ-7 |
| DA-11 | **Multi-tool SDD sizing** | Should this feature proceed to SDD, and if so, what sizing is appropriate? | F-15, F-18, RH-5, RH-8 |

### 14.2 Recommendations on SDD Phase Entry

Based on the evidence gathered:

1. **The feature should proceed to the Specify/Design phase.** The Research has sufficient evidence to inform architectural decisions.

2. **Sizing recommendation: Medium.** The feature does not involve code changes, database changes, or API changes. It involves:
   - Rule/skill format adaptation (documentation/markdown).
   - Path reference updates in governance docs.
   - Possible ADR revision or new ADR.
   - Documentation strategy decisions.
   - No clinical, security, or persistence impact.

3. **The Design phase should produce:**
   - A design document addressing the 11 decision areas above.
   - An ADR evaluation for ADR-003 disposition.
   - A documentation update plan for the 43+ path references.
   - A rule/skill format adaptation strategy.

4. **The Design phase should NOT:**
   - Implement any changes to rules, skills, or documentation.
   - Modify ADR-003 without formal ADR governance.
   - Create implementation tasks.

### 14.3 Success Criteria Validation

| Success criterion (from task) | Status | Evidence |
|-------------------------------|--------|----------|
| Can the existing Harness preserve Cursor as the reference implementation? | **Yes** | RH-5 validated; directories do not conflict; `AGENTS.md` shared |
| Can the Harness support Cline without compromising its current governance? | **Yes** | RH-3, RH-4 validated; governance methodology is tool-agnostic; format differences are adaptable |
| Which architectural assumptions remain valid? | See §9.1 | Skill format, AGENTS.md loading, governance methodology, verification model |
| Which assumptions are invalid? | See §9.1 | `.mdc` format universality, `alwaysApply`/`globs` universality, single-tool context assumption |
| Which questions remain unanswered? | See §12.2 | ADR-003 disposition, path reference strategy, documentation naming, context management adoption |
| Which topics require architectural decisions during the Design phase? | See §14.1 | 11 decision areas identified |

---

## Summary

The Research confirms that the Doc Organo AI Harness can evolve to support Cline as an additional AI tool while preserving the current Cursor implementation as the reference architecture. The governance methodology is tool-agnostic by design. The skill format is identical between tools. The rule content is portable. The main differences are in directory paths, file extensions, and frontmatter schemas — all of which are adaptable without compromising governance.

The Design phase should focus on resolving the 11 decision areas identified in this report, with particular attention to artifact location strategy, rule format strategy, ADR-003 disposition, and path reference strategy. The Research provides sufficient evidence for these decisions without prescribing solutions.

This Research remains neutral regarding the final architecture. It provides the evidence base for the upcoming Architecture & Design phase.