# Review Prompt — Domain & Business Rules

Review this diff against `Documentation/Architecture/Domain_Overview_Business_Rules.md`.

Check:

- [ ] Ubiquitous language preserved (Paciente, Prontuario, Atendimento, etc.)
- [ ] Business rules not placed in Controllers or Repositories
- [ ] Prontuario versioning respected (immutable snapshots)
- [ ] Atendimento stage transitions validated appropriately
- [ ] No Financial domain changes unless task explicitly scoped

Report: Critical / Suggestion / OK per finding.
