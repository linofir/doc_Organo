# Plano de Gestão de Projeto — Doc Organo

> Documento operacional de planejamento.  
> PRD: [PRD.md](PRD.md). Roadmap: [RoadMap.md](RoadMap.md). Estado atual: [../State.md](../State.md).

## 1. Propósito

Este documento organiza o planejamento do Doc Organo em épicos, features e riscos. Ele responde **o que será feito, em que ordem e por quê agora**.

Responsabilidades:

- **PRD** define o porquê do produto.
- **PM** organiza MVPs, épicos, features, débitos e prioridades.
- **RoadMap** registra horizontes futuros, sem virar backlog de sprint.
- **State** registra o que é verdade agora na branch ativa.

## 2. Contexto Atual

O MVP1 validou a primeira versão do produto com foco em fluxo clínico, Google Sheets como persistência e interface web inicial. A fase atual é o **MVP2**, com foco em profissionalizar a base técnica: SQL Server, EF Core, testes, segurança, padronização e continuidade do fluxo clínico.

Contexto de planejamento:

- Persistência alvo: SQL Server + EF Core.
- Referência de comportamento legado: Google Sheets como evidência para migração e testes de caracterização.
- Frontend: Blazor Server (`DocFront.Web`).
- Prioridade técnica: Paciente SQL primeiro, depois Prontuario, Agendamento e Atendimento.
- Capacidade de suporte: AI Harness usado para aumentar rastreabilidade, continuidade documental, verificação e segurança da migração clínica.

Status operacional de branch, runtime, blockers e próximos passos deve ser consultado em [../State.md](../State.md).

## 3. MVP1 — Épicos Executados

Os épicos abaixo representam a primeira validação funcional apresentada ao cliente, com foco em entregar valor rapidamente.

| Código | Épico | Resultado | Observações |
|--------|-------|-----------|-------------|
| E01 | Gestão e estruturação do desenvolvimento | Executado | Base inicial de organização, documentação e gestão do projeto |
| E02 | Estruturação do Atendimento | Executado parcialmente | CRUD/modelagem inicial feitos; regras de validação ainda precisam ser portadas para MVP2 |
| E03 | Gestão de Prontuários | Executado parcialmente | CRUD e extração/geração iniciados; precisa migração SQL e revisão de serviços |
| E04 | Extração e análise de dados externos | Executado parcialmente | Serviços de demonstrativo/senhas criados; integração e testes ficam para MVP2 |
| E05 | Gestão de Agendamentos | Executado | Fluxos principais entregues; precisa migração SQL e integração com autorizações |
| E06 | Gestão de Pacientes | Executado | Base de cadastro entregue; agora é a primeira vertical SQL |
| E07 | Frontend Web — DocFront.Web | Executado parcialmente | Base Blazor Server criada; faltam Atendimento, erros globais, testes e refinamento UX |

## 4. MVP2 — Objetivo

Transformar o MVP validado em uma base técnica mais confiável para evolução contínua.

Objetivos do MVP2:

1. Migrar os fluxos clínicos principais de Google Sheets para SQL Server.
2. Preservar o comportamento validado no MVP1.
3. Reduzir riscos de dados sensíveis, logs inseguros e ausência de auth.
4. Criar uma base mínima de testes e revisão.
5. Organizar serviços legados e integrações para uso real.
6. Preparar o frontend para os novos contratos do backend.
7. Manter Clean Architecture leve, sem overengineering.
8. Adotar governança de AI Harness suficiente para rastrear requisito, design, execução, teste, verificação e follow-up documental.
9. Validar o fluxo SDD/verificação em fatias reais da migração SQL antes de ampliar automações ou integrações avançadas.

## 5. MVP2 — Backlog por Workstream

### WS01 — Persistência SQL e Infraestrutura

| Prioridade | Feature | Status | Resultado esperado |
|------------|---------|--------|--------------------|
| P0 | Paciente SQL Stabilization Pilot | Verificado | `PacienteRepository` validado contra Docker SQL, Swagger e testes; REQ-001–REQ-008 satisfeitos — ver [verification.md](../SDD/paciente-sql-stabilization/verification.md) |
| P0 | Implementar `ProntuarioRepository` EF | Próximo | CRUD SQL funcional e alinhado ao domínio |
| P0 | Implementar `AgendamentoRepository` EF | Próximo | CRUD SQL funcional e preparado para autorizações |
| P0 | Implementar `AtendimentoRepository` EF | Próximo | CRUD SQL com regras portadas do legado |
| P1 | Atualizar plano de migração SQL | Planejado | Checklist por entidade e critérios de merge claros |
| P1 | Avaliar feature flag de persistência | Planejado | Decidir se ainda faz sentido manter fallback/híbrido |
| P2 | Planejar upgrade para .NET 8 LTS | Futuro próximo | ADR e execução após estabilização da migração clínica |

### WS02 — Domínio Clínico e Clean Architecture

| Prioridade | Feature | Status | Resultado esperado |
|------------|---------|--------|--------------------|
| P0 | Portar validações de Atendimento do Legacy | Planejado | `ValidacaoEtapa*` fora de repositórios e dentro de domínio/use cases |
| P0 | Criar testes de caracterização do Legacy | Planejado | Comportamento validado antes de remover código legado |
| P1 | Introduzir use cases finos quando necessário | Planejado | Orquestração multi-agregado sem Mediator/CQRS prematuro |
| P1 | Padronizar IDs e contratos | Planejado | `Guid` consistente em entidades, DTOs e controllers |
| P1 | Revisar exclusão e histórico clínico | Planejado | Soft delete e versionamento preservando histórico |
| P2 | Avaliar checklists clínicos configuráveis | MVP3 candidato | Só implementar se reduzir complexidade das validações |

### WS03 — Testes e Verificação

| Prioridade | Feature | Status | Resultado esperado |
|------------|---------|--------|--------------------|
| P0 | Testes SQL de integração para Paciente | Verificado | `PacienteSqlIntegrationTests` com skip policy; 15 testes no total (14 unit + 1 SQL) |
| P0 | Testes de caracterização para Atendimento | Planejado | Regras legadas documentadas por testes |
| P1 | CI mínimo com `dotnet test` | Planejado | Feedback automático antes de merge |
| P1 | Smoke tests Swagger/Blazor | Planejado | Validação manual orientada por checklist |
| P2 | E2E tests dos fluxos clínicos | MVP3 candidato | Cobertura ponta a ponta quando UI/API estabilizarem |

### WS04 — Segurança, PHI e Identity/Auth

| Prioridade | Feature | Status | Resultado esperado |
|------------|---------|--------|--------------------|
| P0 | Remover logs com dados sensíveis | Verificado (Paciente) | PHI removido de `PacienteController`; demais controllers ainda pendentes |
| P0 | Criar review prompt de segurança/PHI | Executado | `Documentation/AI-Harness/review-prompts/security-phi-review.md` — aplicado no piloto Paciente |
| P1 | Definir escopo mínimo de auth para MVP2 | Planejado | ADR com decisão: ambiente controlado vs auth mínima |
| P1 | Planejar Identity/Auth como capacidade de suporte | Planejado | Login e autorização sem criar bounded context prematuro |
| P2 | RBAC avançado por perfil | MVP3 | Médico, secretária, admin, enfermeira e paciente |

### WS05 — Qualidade de Código e Padrões

| Prioridade | Feature | Status | Resultado esperado |
|------------|---------|--------|--------------------|
| P0 | Corrigir repositórios stub ou explicitar indisponibilidade | Planejado | Endpoints não parecem prontos quando ainda não foram migrados |
| P1 | Padronizar namespaces e DTOs | Planejado | `DocAPI.Core.Interfaces.Repositories` e `Application/Data` coerentes |
| P1 | Padronizar respostas de endpoints | Planejado | Contratos previsíveis para o front |
| P1 | Padronizar controllers | Planejado | Rotas, IDs, status codes e validação consistentes |
| P2 | Cache e performance | Futuro | Só após os fluxos principais estabilizarem |
| P3 | Unit of Work, CQRS, Mediator, Domain Events | MVP3/futuro | Só se houver necessidade comprovada |

### WS06 — Serviços Legados e Integrações

| Serviço/área | Situação | Planejamento MVP2 |
|--------------|----------|-------------------|
| `GoogleSheetsDB` | MVP1 only | Manter como referência em `main`/Legacy; não reativar em `feature/base_DB` sem decisão explícita |
| `PacienteSheetsRepository` | Legado | Usar como referência de comportamento para migração |
| `ProntuarioSheetsRepository` | Legado | Usar para testes de caracterização e migração |
| `AtendimentoSheetsRepository` | Legado crítico | Portar regras de validação para domínio/use cases |
| PDF extractor/generator | Criado, precisa revisão | Planejar integração, testes e tratamento seguro de arquivos |
| `CollectDemonstrativoDataService` | Criado, futuro financeiro | Não expandir financeiro antes da migração clínica |
| `CollectSenhasAutorizadasDataService` | Criado, precisa integração | Avaliar integração com Agendamento/Atendimento no MVP2 |
| Intake de informações externas | Capacidade a organizar | Planejar entrada de dados externos sem definir implementação antes da estabilização das fatias clínicas |
| Document ingestion | Planejado | Revisar como documentos enviados ou coletados entram no fluxo clínico/administrativo |
| Normalização de extrações | Planejado | Definir critérios de planejamento para transformar dados extraídos em informação utilizável e rastreável |
| Rastreamento de origem | Planejado | Garantir que informação importada mantenha vínculo com fonte, paciente, atendimento ou contexto administrativo |
| Fronteiras de integração externa | Planejado | Separar intake de PDFs/portais/sistemas externos sem acoplar o domínio clínico a detalhes de fornecedores |
| Tratamento PHI-safe | Planejado | Planejar extração e ingestão sem expor dados sensíveis em logs, exemplos, prompts ou commits |

Documentos e capacidades a planejar:

- `Documentation/Technical/api-contract.md`.
- Review prompt de segurança/PHI.
- Verifier checklist para conclusão de PR.

### WS07 — Frontend Web

| Prioridade | Feature | Status | Resultado esperado |
|------------|---------|--------|--------------------|
| P0 | Alinhar Paciente com SQL/API atual | Planejado | UI validada contra backend SQL |
| P0 | Integrar Prontuario e Agendamento com novos contratos | Planejado | Tabs e services coerentes com API SQL |
| P0 | Planejar e integrar Atendimento no front | Planejado | Service/State/UI após backend estabilizar regras |
| P1 | Padronizar `ApiResponse<T>` e tratamento de erro | Planejado | Feedback claro para usuário e menos duplicação |
| P1 | Criar ErrorBoundary/toasts/loading states | Planejado | Melhor experiência sem grande redesign |
| P1 | Revisar navegação e fluxo das páginas clínicas | Planejado | Caminhos principais mais claros após estabilização dos contratos SQL/API |
| P1 | Melhorar consistência visual e layout básico | Planejado | Telas principais com estrutura mais previsível sem iniciar redesign amplo |
| P1 | Preparar reutilização de componentes | Planejado | Componentes comuns identificados para reduzir duplicação gradual |
| P1 | Cache, refresh e invalidação simples | Planejado | Evitar chamadas desnecessárias sem arquitetura complexa |
| P2 | Debounce em buscas e autocomplete | Planejado | Buscar paciente por nome/CPF com UX melhor |
| P2 | Avaliar prontidão para design system futuro | Planejado | Diretrizes iniciais para evolução visual sem transformar MVP2 em redesign |
| P2 | Testes de services/componentes | Planejado | Cobertura pragmática para fluxos críticos |
| P3 | Mobile/MAUI | MVP3 | Deferir até validação do MVP2 |

### WS08 — Implementação, Demonstração e Evolução Assistida por IA

| Prioridade | Feature | Status | Resultado esperado |
|------------|---------|--------|--------------------|
| P1 | Criar roteiro de demonstração MVP2 | Planejado | Script para apresentar evolução ao cliente |
| P1 | Gerar documentação de release | Planejado | Notas claras por evolução entregue |
| P2 | Criar material visual | Planejado | Screenshots, vídeo curto e resumo técnico |
| P2 | Skill de acompanhamento de evolução | Planejado | Skill futura para sugerir posts, demos e documentação a partir do progresso |
| P3 | Posts LinkedIn/currículo | Futuro | Separar de backlog técnico principal |

### Iniciativa de Governança — Adoção do AI Harness no MVP2

Esta iniciativa é uma capacidade de suporte para o MVP2, não um novo domínio do produto. Ela organiza o trabalho necessário para que a migração SQL clínica use planejamento, verificação, rastreabilidade e documentação de forma consistente. Os detalhes operacionais permanecem nos documentos de governança do AI Harness.

| Grupo | Prioridade | Status | Resultado esperado |
|-------|------------|--------|--------------------|
| AI Harness Foundation | P1 | Planejado | Governança, documentação, regras, skills, índices e caminhos alinhados aos artefatos canônicos |
| AI Harness Adoption | P1 | Em calibração | Piloto SDD Paciente SQL Stabilization executado e verificado; calibrar templates antes do piloto Prontuario forward |
| AI Harness Evolution | P2 | Planejado | Estratégia leve para reporting futuro, session handoff, feature reports, lessons learned e preparação para observabilidade de workflow |
| Verification Adoption | P1 | Verificado (piloto) | Verifier aplicado no piloto Paciente SQL Stabilization; `verification.md` gerado |

Itens planejados para MVP2:

- Calibrar retrospectivamente `Validar vertical Paciente em SQL` como reconstrução de rastreabilidade, validação de reporting e ajuste de governança.
- Usar `ProntuarioRepository` EF como primeiro candidato preferencial para validação SDD forward.
- Aplicar o Verifier como responsabilidade explícita antes de considerar trabalho relevante concluído.
- Usar Documentation Update para avaliar follow-up de State, ADRs, documentação técnica/arquitetural, regras, skills, prompts e SDD quando a verdade do projeto mudar.
- Ajustar templates SDD apenas depois da calibração e do primeiro piloto revelarem necessidades reais.
- Avaliar necessidade futura de skill SDD somente depois da validação prática do workflow.
- Manter reporting em nível leve durante MVP2: session handoff, feature reports e lessons learned, sem criar formatos pesados antes da prática estabilizar.
- Adotar a governança aprovada do Verifier em execuções reais, sem recriar o modelo no PM.

Fora do MVP2, salvo decisão explícita:

- Orquestração avançada de agentes.
- Workflows autônomos.
- Self-healing governance.
- Ecossistemas MCP customizados.
- Métricas e observabilidade completas do harness.

## 6. MVP3 / Futuro

Itens que devem permanecer fora do MVP2, salvo decisão explícita:

- Mobile/MAUI e experiência mobile completa.
- Conciliação financeira completa.
- Persistência e análise avançada do demonstrativo financeiro.
- Auditoria avançada com valores anteriores/novos.
- RBAC completo e administração avançada de usuários.
- Cloud production hardening, HomeLab e Azure.
- CQRS, Mediator, Domain Events ou arquitetura avançada.
- Design system completo e redesign amplo.
- E2E completo de todos os fluxos.
- Orquestração avançada de agentes, workflows autônomos, self-healing governance e MCPs customizados.
- Estratégia MCP detalhada, critérios de adoção e rollout de integrações externas avançadas.
- Métricas completas e observabilidade avançada do AI Harness.

## 7. Gestão de Riscos e Desafios

| Risco/Desafio | Probabilidade | Impacto | Mitigação |
|----------------|---------------|---------|-----------|
| Dependência do comportamento legado em Sheets | Alta | Alto | Usar Legacy como referência e criar testes de caracterização antes de remover código |
| Migração SQL incompleta gerar endpoints quebrados | Alta | Alto | Expor status dos repositórios, priorizar Paciente → Prontuario → Agendamento → Atendimento |
| PHI em logs, prompts ou commits | Média | Alto | Aplicar regra `security-phi`, criar review prompt e remover `Console.WriteLine` sensível |
| Ausência de auth em ambiente com dados reais | Média | Alto | Definir ADR de auth mínima ou restrição explícita de ambiente controlado |
| Regras de Atendimento ficarem em repositório | Alta | Alto | Portar para domínio/use cases e revisar com checklist DDD |
| Serviços de extração frágeis por formatos externos | Média | Médio/Alto | Criar testes com arquivos de exemplo e logging seguro |
| Acúmulo de débitos técnicos durante MVP2 | Média | Médio | Tratar débitos como features planejadas por workstream, não como lista solta |
| Overengineering arquitetural | Média | Médio | Manter CA leve; adiar CQRS/Mediator/Domain Events |
| Ponto único de falha no desenvolvimento | Alta | Médio | Manter PRD, PM, RoadMap, State e ADRs atualizados |
| Front avançar antes dos contratos backend estabilizarem | Média | Médio | Planejar UI por vertical slice e validar contra API real |
| Governança SDD/verificação existir nos documentos mas não ser adotada nas fatias reais | Média | Alto | Validar primeiro com Paciente retrospectivo e Prontuario forward; usar Verifier e Documentation Update como gates de conclusão |
| Reporting virar histórico solto ou duplicar State/Verifier | Média | Médio | Manter reporting leve no MVP2 e limitar seu papel a handoff, feature reports e lessons learned |

## 8. Informações Preservadas

A versão anterior do PM continha backlog bruto, sprints antigas, itens de frontend V2, sugestões de UX/produto, arquitetura avançada e riscos iniciais. Essas informações foram consolidadas neste documento em MVP1, MVP2, MVP3/Futuro e Gestão de Riscos.

## 9. Referências

- PRD: [PRD.md](PRD.md)
- RoadMap: [RoadMap.md](RoadMap.md)
- Estado atual: [../State.md](../State.md)
- Domínio: [../Architecture/Domain_Overview_Business_Rules.md](../Architecture/Domain_Overview_Business_Rules.md)
- Migração SQL: [../Technical/migration-sql.md](../Technical/migration-sql.md)
- Governança AI Harness: [../AI-Harness/Harness-Design/harness-architecture.md](../AI-Harness/Harness-Design/harness-architecture.md)
- Governança SDD: [../AI-Harness/Harness-Design/sdd-operational.md](../AI-Harness/Harness-Design/sdd-operational.md)
