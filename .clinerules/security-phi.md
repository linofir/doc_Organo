# Security — Doc Organo

- Never commit real patient names, CPF, clinical data, credentials, or `.env` files.
- Do not log PHI in `Console.WriteLine` — prefer structured logging without sensitive fields.
- Private repository — still assume all clinical data is sensitive in prompts and outputs.
- Do not expose connection strings or Google service account JSON in code or docs.
- Treat auth, authorization, patient-data exposure and auditability changes as security-sensitive; route deeper checks to the verifier and security/PHI review prompt.

## Additional Context

- `Documentation/AI-Harness/review-prompts/security-phi-review.md`