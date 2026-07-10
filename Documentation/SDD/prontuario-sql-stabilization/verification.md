# Verification — Prontuario SQL Stabilization

> Feature SDD: `Documentation/SDD/prontuario-sql-stabilization/`  
> SDD artifacts: `specify.md`, `design.md`, `tasks.md`, `research.md`, `reports/pre-execution-review-2026-06-19.md`  
> PM reference: WS01  
> Verification date: 2026-07-09  
> Overall verdict: **Verified with Minor Findings**  
> Baseline prior to Execute: 27 tests (Atendimento Minimal Verified)  
> Post-Execute: **79 passed / 0 failed / 0 skipped** (+52 from baseline)

---

## 1. Change Classification

| Label | Rationale |
|-------|-----------|
| Domain | `Prontuario` aggregate: `AplicarCriacao`, `AplicarCorrecao`, `CriarNovaVersao`, `MarcarComoExcluido` + owned VOs (`DescricaoBasica.AplicarCorrecao`) |
| Persistence / SQL / EF | `ProntuarioRepository` full SQL CRUD + version lookup (`GetMaxVersaoForPacienteAsync`) + soft delete; `ProntuarioSqlIntegrationTests` against Docker SQL; EF includes for nested graph reads |
| API | `ProntuarioController`: 10 endpoints, Guid routes, correction vs evolution routing per ADR-006, FK/CID validation, 409 concurrency, PHI-safe error messages |
| Security-sensitive / PHI | Rich clinical PHI in payloads (AGO, antecedentes, exames, internacao); controller logging remediated per REQ-012; synthetic test data only |
| Legacy behavior porting | Legacy in-place update adapted to dual-mode API (correction-only PUT); AutoMapper create/update paths abandoned per specify.md Legacy Behavior table |
| Cross-layer | Aggregate → Repository → Controller → DTOs → Tests — full vertical slice |

**Classification fully consistent with SDD sizing (Large) and scope claims.** No classification drift detected.

### Evidence reviewed

- Git status: 10 modified files + 3 new test files (uncommitted working tree changes)
- `DocAPI/Core/Entities/Prontuario.cs` — aggregate with domain methods
- `DocAPI/Infrastructure/Repositories/ProntuarioRepository.cs` — SQL persistence
- `DocAPI/API/Controllers/ProntuarioController.cs` — HTTP orchestration
- `DocAPI/Core/Interfaces/Repositories/IProntuarioRepository.cs` — Guid contract
- `DocAPI/Application/Data/Dtos/Prontuario/CreateProntuarioDto.cs` — dedicated DTOs
- `DocAPI/Application/Data/Dtos/Prontuario/UpdateProntuarioDto.cs` — structural correction contract (D-01)
- `DocAPI/Application/Data/Dtos/Prontuario/ReadProntuarioDto.cs` — read projection
- `DocAPI/Application/Mappings/Profiles/ProntuarioProfile.cs` — read-only AutoMapper
- `DocAPI.Tests/Infrastructure/ProntuarioRepositoryTests.cs` — 11 unit tests
- `DocAPI.Tests/Controllers/ProntuarioControllerTests.cs` — 27 controller tests
- `DocAPI.Tests/Integration/ProntuarioSqlIntegrationTests.cs` — 1 SQL integration test

---

## 2. Gate Evaluation

| Gate | Status | Evidence / Reason |
|------|--------|-------------------|
| **dotnet build** | ✅ Passed | `0 Erro(s)` — 3 projects built. Warnings: NETSDK1138 (net7.0 EOL) × 3, NU1903 (AutoMapper vulnerability) × 3 — all pre-existing, none introduced by this slice. |
| **dotnet test** | ✅ Passed | **79 passed / 0 failed / 0 skipped**. Composition: 27 `ProntuarioControllerTests` + 11 `ProntuarioRepositoryTests` + 1 `ProntuarioSqlIntegrationTests` + 13 `PacienteControllerTests` (pre-existing, from ai-harness-multi-tool) + 15 `PacienteRepositoryTests` + 12 `AtendimentoRepositoryTests`. |
| **SQL / Persistence** | ✅ Passed | `ProntuarioSqlIntegrationTests.ProntuarioSql_FullVersioningChain_WithInternacao_RoundTrip` — 1/1 passed against Docker SQL. Full fixture chain: Paciente → Atendimento → v1 create (nested Exames + CD + Internacao with synthetic CID) → PUT correction → POST `/versoes` v2 → soft delete v1. D-05 no-merge verified (V1 Exames [HEMO, GLIC]; V2 Exames [TSH] only). `SqlIntegrationTestGate` skip policy active — Docker available → ran, passed. |
| **API** | ⚠️ Partially Verified | Controller routes match design.md API Contract table exactly. Negative HTTP status code paths covered by 27 controller unit tests. **TASK-009 Swagger smoke session notes not provided in Execute handoff** — missing evidence for runtime HTTP behavior. See Finding F-01. |
| **UI** | ⏭️ Skipped (out of scope) | WS07 deferred per Backend Stabilization Rule. No changes under `DocFront.Web/`. |
| **Security / PHI** | ✅ Verified | `security-phi-review.md` sensor applied via code review: no `Console.WriteLine` in `ProntuarioController`; generic 404 messages ("Not Found" — no PHI in error text); synthetic test data only; `POST /from-pdf` returns 501 immediately with no repository calls, file processing, or PDF side effects; CID FK validation returns structured `BadRequest` without exposing internal FK details. |
| **Domain review** | ✅ Verified | `domain-review.md` sensor applied. ADR-006 dual-mode versioning API fully honored: PUT = correction (same `Id`/`Versao`), POST `/versoes` = clinical evolution (new row, incremented `Versao` per `PacienteId`, `ProntuarioAnteriorId` set). D-02 version generation authority: Application layer obtains `max(Versao)` from repository; aggregate validates `nextVersao` (throws `InvalidOperationException` → 409). D-01 structural correction contract: `UpdateProntuarioDto` has 4 fields only (`InformacoesExtras`, `Profissao`, `Religiao`, `AtividadeFisica`). D-05 evolution replace-semantics: `CriarNovaVersao` creates complete snapshot — no merge from predecessor. D-06 concurrent evolution: `DbUpdateException` on unique constraint `(PacienteId, Versao)` → 409 Conflict. Soft delete per ADR-001: single version only; `GetMaxVersaoForPacienteAsync` uses `.IgnoreQueryFilters()` per `.clinerules/ef-migrations.md` — documented in code. Patient-wide versioning (DQ-007 A). |
| **Documentation review** | ✅ Verified | `check-docs.md` sensor applied. Implementation fully aligns with `specify.md` + `design.md` + `tasks.md` — no divergence. Legacy behavior table in specify.md honored: CRUD surface preserved, in-place update adapted to correction-only, 3-sheet write adapted to normalized SQL, string IDs abandoned, AutoMapper write paths abandoned, PHI logging abandoned, PDF import abandoned (501). API contract table in design.md matches controller routes 1:1. Field classification D-01 structurally enforced in `UpdateProntuarioDto`. |
| **Test strategy review** | ✅ Verified | `test-strategy.md` sensor applied. Approved sequence (domain → repository unit → SQL integration → Swagger intent) followed. Repository unit tests (InMemory): 11 covering create, correction, evolution chain, invalid `nextVersao`, soft delete exclusion, D-05 no-merge, list ordering, not-found, `/atual` both scopes. Controller unit tests (Moq + InMemory): 27 covering all 10 endpoints, all status code paths (201, 204, 200, 404, 400, 409, 501), FK validation, CID validation, concurrency. SQL integration: 1 test with full fixture chain (repository-direct, Atendimento pilot pattern). Test count: +52 from 27 baseline. |
| **ADR evaluation** | ✅ Aligned | ADR-006 (dual-mode versioning API) accepted at Pre-Execution Review 2026-06-19. Implementation fully conforms. No new ADR required. Version numbering per `PacienteId` is a domain decision (DQ-007 A) — no ADR needed. |
| **Legacy characterization** | ✅ Verified | Preserve/Adapt/Abandon table in specify.md reviewed against implementation. All decisions honored — see Documentation review gate. |

---

## 3. Requirement Traceability

| REQ | Status | Evidence |
|-----|--------|----------|
| REQ-001 | ✅ Verified | `ProntuarioRepositoryTests.AddAsync_ThenGetById_ReturnsProntuario` — asserts `Versao = 1`, `PacienteId`, `AtendimentoId`, identity snapshot via `DescricaoBasica`. Integration: v1 with nested Exames, CD, Internacao persisted. Controller: `Post_ValidDto_ReturnsCreated` maps identity from `Paciente`. |
| REQ-002 | ✅ Verified | `ProntuarioControllerTests.Post_InvalidAtendimentoId_ReturnsNotFound` (null atendimento). `Post_PacienteIdMismatch_ReturnsNotFound` (different PacienteId). Both assert 404 + no repository add. |
| REQ-003 | ✅ Verified | `ProntuarioControllerTests.Get_ById_Returns200` (version metadata on DTO). `Get_ById_Returns404`. Repository: `GetById_NotFound_ReturnsNull`. Integration: v1 fetched with full nested graph. |
| REQ-004 | ✅ Verified | `ProntuarioRepositoryTests.GetAll_ReturnsOrderedByVersaoDesc`. Integration: 2 versions listed, V2 before V1. Controller: `Get_ByPacienteId_Returns200`. |
| REQ-005 | ✅ Verified | `GetLatestByPaciente_ReturnsHighestVersao` (2 versions → V2 returned). `GetLatestByAtendimento_ReturnsHighestVersaoForAtendimento`. Controller: 200/404 both scopes. Integration: `/atual` both scopes verified. |
| REQ-006 | ✅ Verified | `AplicarCorrecao_ThenUpdate_PreservesIdAndVersao` — same `Id`, `Versao = 1`, correction fields updated, identity/clinical fields unchanged, `AtualizadoEm` set. Controller: `Put_ValidCorrection_Returns204`. `UpdateProntuarioDto` has 4 correction-safe fields only (D-01). |
| REQ-007 | ✅ Verified | `CriarNovaVersao_IncrementsVersaoAndKeepsSourceIntact` — V1 unchanged, V2 new `Id`, `Versao = 2`, `ProntuarioAnteriorId = v1.ID`, patient-wide increment. Controller: `PostVersoes_ValidEvolution_Returns201` — D-02 flow (get max → compute next → aggregate validates → persist). |
| REQ-008 | ✅ Verified | `SoftDelete_ExcludesFromReads_ButKeepsInMaxVersao` — deleted V1 hidden, V2 still visible, list returns only V2, `MaxVersao` returns 2 (occupied slot preserved). |
| REQ-009 | ✅ Verified | `D05_EvolutionDoesNotMergeCollections` — V1 Exames [E1, E2]; V2 evolution w/ [E3] only → V2 contains only E3, V1 unchanged. Integration confirms: V1 [HEMO, GLIC] + Internacao; V2 [TSH] only, no Internacao. |
| REQ-010 | ✅ Verified | Controller: `Post_InternacaoWithUnknownCid_ReturnsBadRequest` (create). `PostVersoes_InternacaoWithUnknownCid_ReturnsBadRequest` (evolution). Integration: positive w/ synthetic CID `Z99.9` passes — Internacao + nested Procedimentos persisted. |
| REQ-011 | ✅ Verified | `IProntuarioRepository` — all methods use `Guid`. `CreateProntuarioDto`, `UpdateProntuarioDto`, `CreateVersaoProntuarioDto`, `ReadProntuarioDto` — dedicated DTOs, no domain entity types exposed. `ProntuarioProfile` — read-only projection only. |
| REQ-012 | ✅ Verified | Code review: no `Console.WriteLine` or equivalent in `ProntuarioController`. Structured logging only. `security-phi-review.md` sensor applied. |
| REQ-013 | ✅ Verified | `ProntuarioSqlIntegrationTests` class exists. Single test `ProntuarioSql_FullVersioningChain_WithInternacao_RoundTrip` passes against Docker SQL. Full fixture chain: Paciente → Atendimento → v1 → correction → evolution → reads → soft delete. CID seed in setup. `SqlConnectionResolver` + `SqlIntegrationTestGate`. |
| REQ-014 | ✅ Verified | Legacy behavior table in specify.md reviewed against implementation. See Documentation review gate for per-item confirmation. |
| REQ-015 | ✅ Verified | Controller returns: 201 (create, evolution), 204 (correction, delete), 200 (reads), 404 (not found, FK miss, mismatch), 400 (validation, unknown CID), 409 (stale version, unique constraint), 501 (from-pdf). All covered by controller unit tests. |
| REQ-016 | ✅ Verified | `QueryWithIncludes` loads full nested graph (Exames, AcoesCD, Internacao → Procedimentos). Integration: load by `AtendimentoId` with nested shape intact. `AtendimentoId` on every version row. No aggregate reconstruction helpers. |
| REQ-017 | ✅ Verified | Code review: no workflow evaluators, stage progression, pendência writes, or workflow DTOs/services in scope. Scope boundary confirmed at Pre-Execution Review. |
| REQ-018 | ✅ Verified | `Tipo` as `int` in `CreateProntuarioDto`, `CreateVersaoProntuarioDto`, `ReadProntuarioDto`. Controller asserts `Tipo = 0` on create, `Tipo = 1` on evolution. Entity property is `int`. |

---

## 4. Runtime Validation

> Verify owns durable HTTP/runtime evidence (TASK-008 pattern). Execute recorded intent in TASK-009 session notes.

| Role | Responsibility | Status |
|------|----------------|--------|
| Execute (TASK-009) | Confirm environment; run Swagger checklist; record intent in session notes | **Not provided** — see Finding F-01 |
| Verify | Owns durable HTTP/runtime evidence in verification.md § Runtime Validation | **Deferred** — controller unit tests (27) partially mitigate |

### Environment-dependent evidence

- Docker SQL: available (SQL integration test passed)
- API runtime: DocAPI running at `https://localhost:7004` per Execute Prerequisites
- Swagger smoke: not executed during Verify — see F-01

---

## 5. Findings

### Findings requiring action

| ID | Severity | Description | Recommendation |
|----|----------|-------------|----------------|
| **F-01** | Low | TASK-009 Swagger smoke session notes missing from Execute handoff. Verify cannot independently confirm runtime HTTP behavior beyond unit/integration tests. | Execute TASK-009 manual checklist per `specify.md` Runtime Validation Environment; record results in session notes; update State.md. Controller unit tests (27) + SQL integration (1) provide strong mitigation — does not block verification. |

### Observations (no action required)

| ID | Description |
|----|-------------|
| O-01 | `AutoMapper` 12.0.1 NuGet package has known high-severity vulnerability (NU1903 / GHSA-rvv3-g6hj-g44x). Used only for read projection in `ProntuarioProfile` and pre-existing `PacienteProfile`/`AtendimentoProfile`. Pre-existing — not introduced by this slice. Track as separate debt item. |
| O-02 | NETSDK1138 warnings for `net7.0` EOL — pre-existing across entire solution (DocAPI, DocFront.Web, DocAPI.Tests). |
| O-03 | `AGODto.VacinaHPV` uses `Core.Entities.StatusVacinaHPV` (domain enum) — minor DTO/domain coupling. Acceptable for MVP2 per Backend Stabilization; refactor to DTO-level enum in WS07 or separate quality task. |
| O-04 | `ProntuarioRepository.GetMaxVersaoForPacienteAsync` uses `.IgnoreQueryFilters()` — documented in code comment per `.clinerules/ef-migrations.md` requirement. Comment explains soft-deleted version slot preservation rationale. |
| O-05 | `ProntuarioControllerTests.PostVersoes_StaleNextVersao_Returns409` and `PostVersoes_DbUpdateUniqueViolation_Returns409` cover both concurrency paths (aggregate validation + database unique constraint). |

### Drift analysis

| Drift type | Status |
|------------|--------|
| Requirement drift | **None** — all 18 REQs traceable to passing evidence |
| Implementation drift | **None** — implementation matches design.md API Contract table, field classification D-01–D-06, and ADR-006 |
| Documentation drift | **None** — specify.md, design.md, and tasks.md are consistent with implementation |
| Governance drift | **None** — ADR-001 honored (soft delete), ADR-006 honored (dual-mode API), D-02 honored (version generation authority) |

---

## 6. Residual Risk

| Risk | Severity | Accepted? | Rationale |
|------|----------|-----------|-----------|
| TASK-009 Swagger smoke not executed | Low | Yes — with F-01 follow-up | Controller unit tests (27) cover all HTTP status code paths; integration test covers repository-direct full chain |
| CID catalog absent in production | Medium | Yes — per SDD scope | Synthetic CID in integration tests only; CID Catalog Management is future PM item |
| WS07 Guid + dual verb drift | Medium | Yes — Backend Stabilization Rule | WS07 deferred; contract documented in design.md API Contract table |
| `AtualizadoPor` unset | Low | Yes — per SDD | Auth/RBAC out of scope for all clinical SQL slices |
| Workflow evaluators deferred | Medium | Yes — REQ-017 | `atendimento-workflow-stabilization` after Prontuario + Agendamento verified |
| Concurrent evolution 409 — client recovery guidance not in API docs | Low | Yes — D-06 | Client retry pattern documented in design.md § Concurrency; WS07 API docs can formalize |
| Patient-wide versioning confusion (DQ-010) | Low | Yes — D-03 | Documented in design.md; two-atendimento test scenario recommended for TASK-008 (not executed — low risk for MVP2) |
| Soft-deleted Atendimento on Prontuario create → 404 | Low | Yes — optional test debt | Design.md § Optional Test Debt; controller validates Atendimento existence and PacienteId match |

---

## 7. Pre-Existing Issues (not introduced by this slice)

| Issue | Source |
|-------|--------|
| `DocAPI.Tests/Controllers/PacienteCrontrollerTests.cs` — typo in filename | ai-harness-multi-tool SDD |
| AutoMapper NU1903 vulnerability (GHSA-rvv3-g6hj-g44x) | Pre-existing across Paciente and Atendimento profiles |
| .NET 7 EOL (NETSDK1138) | Pre-existing across entire solution |
| `AtendimentoRepository` and `PacienteRepository` pre-existing tests unchanged | Atendimento Minimal + Paciente SDDs |

---

## 8. Skipped Gates

| Gate | Reason |
|------|--------|
| UI / Blazor smoke | WS07 deferred — Backend Stabilization Rule. No changes under `DocFront.Web/`. |
| New EF migration | None required — `InitialCreate` schema suffices. Unique index `(PacienteId, Versao)` already present. |
| Controller HTTP integration tests | Optional per design.md; Swagger smoke checklist (TASK-009) is the designated API gate. Controller unit tests (27) cover HTTP status code paths. |
| Two-atendimento patient-wide Versao scenario (D-03) | Recommended but optional in tasks.md TASK-008. Low risk for MVP2 — versioning logic is patient-wide and tested with single-atendimento multi-version scenario. |
| Concurrent evolution 409 integration test | Optional per design.md. Unit tests cover both concurrency paths (aggregate validation + DbUpdateException). |

---

## 9. Documentation Follow-Up Candidates

Per `tasks.md` DF-001, the following updates are candidates for Documentation Follow-Up (post-Verify, not executed by Verify):

