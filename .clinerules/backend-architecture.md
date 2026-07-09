---
paths:
  - "DocAPI/**/*.cs"
---

# Doc Organo — Backend Architecture

## Domain language

Use Doc Organo terms: Paciente, Prontuario, Agendamento, Atendimento, EtapaAtendimento, Pendencia, Internacao. Do not replace them with generic "record" or "item" names.

## Layer boundaries

- Controllers handle HTTP concerns only.
- Repositories persist and load data only; business rules do not belong there.
- Entities and thin application use cases own invariants and clinical behavior according to the current architecture.
- `DocAPI/Legacy/_LegacySheetsDb/` is behavioral reference only; do not uncomment or revive Sheets persistence without an explicit request.

## Architecture guardrails

- SQL branch entity IDs use `Guid`; align controllers, DTOs, repositories, and frontend contracts when touching endpoints.
- Do not expand Financial features until clinical SQL migration is stable.
- Add an interface only when implementation change, reuse, or boundary protection justifies it.
- Durable changes to boundaries, persistence, schema lifecycle, security/auth, public API contracts, or cross-context ownership require ADR evaluation.

## Additional Context

See:
- `Documentation/Architecture/**`