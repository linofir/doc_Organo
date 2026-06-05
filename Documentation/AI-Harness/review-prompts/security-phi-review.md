# Review Prompt — Security and PHI

Review this diff for sensitive clinical data exposure and security regressions.

Check:

- [ ] No real patient names, CPF, clinical data, credentials or `.env` values are committed.
- [ ] No PHI is written to `Console.WriteLine`, logs, exceptions, prompts or documentation examples.
- [ ] API changes do not expose sensitive fields unnecessarily.
- [ ] Authentication/authorization assumptions are explicit when touching clinical data.
- [ ] File extraction services do not log sensitive file contents or paths unnecessarily.
- [ ] Secrets stay in environment variables, user secrets or secure configuration.
- [ ] Tests use synthetic data only.

Report: Critical / Suggestion / OK per finding.
