# PRD — Doc Organo

> Stable product definition and source of truth for the product "why".  
> Operational planning: [PM_DocOrgano.md](PM_DocOrgano.md). Future horizons: [RoadMap.md](RoadMap.md). Current implementation state: [../State.md](../State.md).

## 1. Purpose

This PRD defines the purpose, target users, product goals, assumptions, and boundaries of Doc Organo. It should stay stable enough to align planning, documentation, and implementation decisions.

This document is not the operational backlog. Use:

- [PM_DocOrgano.md](PM_DocOrgano.md) for epics, tasks, risks, and MVP2 sequencing.
- [RoadMap.md](RoadMap.md) for future product horizons.
- [State.md](../State.md) for what is true right now.
- `Documentation/AI-Harness/sdd/<feature>/` for future feature-level specify/design/tasks artifacts.

## 2. Problem

Medical clinics rely on scattered spreadsheets, PDFs, HTML files, documents, and manual notes for patients, medical records, scheduling, authorizations, care coordination, and post-procedure follow-up.

This creates:

- Manual work and duplicated data entry.
- Lost context across the patient journey.
- Poor visibility into clinical and administrative pending items.
- Fragile workflows tied to spreadsheet structure and external files.
- Higher risk when sensitive clinical information is handled without clear system boundaries.

## 3. Vision

Doc Organo centralizes clinical and administrative data, automates repetitive extraction tasks, and gives clinicians a clear view of each patient's journey from first contact through post-procedure follow-up.

The product should help a small clinic move from spreadsheet-driven operations to a reliable clinical workflow system, while preserving the behavior already validated in the MVP1 Google Sheets version.

## 4. Product Context

Doc Organo is a real-world clinic management platform used in a controlled environment with real clinical workflows.

- **MVP1:** validated the first client-facing version using Google Sheets persistence and Blazor Server UI.
- **MVP2:** focuses on SQL migration, reliability, security, tests, service organization, and documentation/harness maturity.
- **MVP3/Future:** may include mobile, advanced financial workflows, advanced audit, full RBAC and production hardening.

## 5. Users and Personas

| Persona | Needs | Current scope |
|---------|-------|---------------|
| Médico | Patient overview, clinical records, care stage visibility, safe clinical history | Core MVP |
| Secretária | Scheduling, authorizations, administrative data, patient search | Core MVP |
| Enfermeira | Checklist support, care pending items, clinical follow-up | Future per RoadMap |
| Administrador | User management, configuration, auditing, access control | Future per RoadMap |
| Paciente | Visualize and update selected information | Future per RoadMap |

## 6. Goals

1. Provide reliable CRUD and search flows for `Paciente`, `Prontuario`, `Agendamento`, and `Atendimento`.
2. Support the care journey stages: Consulta → Pré-Procedimento → Procedimento → Pós-Procedimento.
3. Preserve clinical history through versioned medical records and soft-delete behavior where appropriate.
4. Generate PDFs, reports, and required clinical documents from existing data.
5. Automate extraction of useful external data, including PDFs, HTML demonstrativos, and authorized-password files.
6. Migrate persistence from Google Sheets to SQL Server for reliability, consistency, and scale.
7. Introduce authentication and authorization in a way that protects clinical data without blocking the current controlled MVP workflow.
8. Improve verification through automated tests, integration tests, and eventually E2E coverage for core flows.

## 7. Non-Goals and Out of Scope

These items are intentionally outside the MVP2 product scope until the clinical SQL migration is stable:

- Public multi-tenant SaaS.
- Full financial reconciliation.
- Mobile MAUI app / V2 mobile experience.
- Production-grade auth/RBAC beyond the initial controlled environment needs.
- Full audit logging with before/after values for every record.
- Financial domain expansion beyond schema preparation and future planning.
- Microservices, CQRS, Mediator, or advanced architecture patterns before the MVP clinical workflow is stable.

## 8. Assumptions

- The MVP runs in a controlled real clinic environment with a small number of users.
- The repository is private, but all patient and clinical information must still be treated as sensitive.
- The `main` branch remains the behavioral reference for the Google Sheets implementation.
- The active SQL migration is incremental: Paciente first, then Prontuario, Agendamento, and Atendimento.
- The product is developed by a solo/small team, so documentation must be concise and useful for AI-assisted development.
- Legacy Sheets code is a behavioral reference until SQL replacements and characterization tests cover equivalent behavior.

## 9. User Stories

### Patient Management

- As a secretary, I want to create and update patient registration data so the clinic has a reliable source for identification, contact, address, and insurance information.
- As a clinician, I want to search patients by relevant criteria such as name or CPF so I can quickly find the correct patient record.
- As the clinic, we want patient data preserved safely so clinical history is not accidentally lost.

### Medical Records

- As a physician, I want to create a new medical record snapshot for a clinical event so the patient's history remains traceable over time.
- As a physician, I want records to preserve clinical details, exams, hospital context, and post-operative data so decisions can be based on complete information.
- As the clinic, we want medical records versioned so later updates do not erase prior clinical context.

### Scheduling

- As a secretary, I want to manage appointments and procedure logistics so the clinic can coordinate dates, locations, rooms, and authorization information.
- As a secretary, I want authorization/password data linked to the appointment flow so administrative pending items are visible before the procedure.

### Care Journey

- As a clinician, I want to track the patient's care journey from consultation to post-procedure so I can understand the current stage and next actions.
- As the clinic, we want pending items and validations before stage progression so important clinical or administrative steps are not missed.
- As a future improvement, we want the Atendimento flow to evolve toward a more formal state machine if it reduces validation complexity.

### Documents and Extraction

- As the clinic, we want to generate documents and reports from structured data so manual document preparation is reduced.
- As the clinic, we want to extract useful data from PDFs and HTML files so external information can feed the workflow without repeated manual entry.

## 10. Success Metrics

- Daily use in a controlled real clinic environment.
- Reduced time spent on manual spreadsheet tasks.
- Stable API and UI for core flows without data loss.
- SQL-backed Paciente flow validated before expanding the migration.
- Prontuario, Agendamento, and Atendimento migrated without losing legacy behavior.
- E2E tests or equivalent smoke tests cover the core clinical flows.
- No patient-identifiable or clinical data exposed in commits, logs, prompts, or documentation examples.

## 11. Constraints

- Sensitive clinical data: no real patient names, CPF, clinical data, credentials, or `.env` contents in commits or documentation examples.
- Privacy and security must be considered even though the repository is private.
- Solo/small team: favor incremental delivery over big-bang rewrites.
- Current frontend is Blazor Server, not Blazor WebAssembly.
- Current backend stack is ASP.NET Core, .NET 7, EF Core 7, and SQL Server.
- SQL Server runs locally through Docker during development.
- Financial tables may exist in the schema, but the Financial domain should not be expanded until the clinical migration is stable.

## 12. Open Questions

- What is the minimum authentication and authorization scope required before broader real-user usage?
- Which core workflows must have automated integration or E2E coverage before `feature/base_DB` can be considered stable?
- Which legacy Atendimento validations should become domain invariants, and which should become application use cases?
- When should the product introduce configurable clinical checklists instead of hard-coded validations?

## 13. Reference Links

- Product backlog and sequencing: [PM_DocOrgano.md](PM_DocOrgano.md)
- Future roadmap: [RoadMap.md](RoadMap.md)
- Current state: [State.md](../State.md)
- Domain language and rules: [Domain_Overview_Business_Rules.md](../Architecture/Domain_Overview_Business_Rules.md)
- SQL migration plan: [migration-sql.md](../Technical/migration-sql.md)
- AI research and harness planning: [AI-Research.md](../AI-Harness/research/AI-Research.md)
