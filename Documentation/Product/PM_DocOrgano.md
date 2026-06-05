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

Estado atual resumido:

- Branch ativa: `feature/base_DB`.
- Persistência alvo: SQL Server + EF Core.
- Referência de comportamento legado: `main` com Google Sheets.
- Frontend: Blazor Server (`DocFront.Web`).
- Prioridade técnica: Paciente SQL primeiro, depois Prontuario, Agendamento e Atendimento.

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

## 5. MVP2 — Backlog por Workstream

### WS01 — Persistência SQL e Infraestrutura

| Prioridade | Feature | Status | Resultado esperado |
|------------|---------|--------|--------------------|
| P0 | Validar vertical Paciente em SQL | Em andamento | `PacienteRepository` validado contra Docker SQL, Swagger e testes |
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
| P0 | Testes SQL de integração para Paciente | Planejado | Cobertura além de EF InMemory |
| P0 | Testes de caracterização para Atendimento | Planejado | Regras legadas documentadas por testes |
| P1 | CI mínimo com `dotnet test` | Planejado | Feedback automático antes de merge |
| P1 | Smoke tests Swagger/Blazor | Planejado | Validação manual orientada por checklist |
| P2 | E2E tests dos fluxos clínicos | MVP3 candidato | Cobertura ponta a ponta quando UI/API estabilizarem |

### WS04 — Segurança, PHI e Identity/Auth

| Prioridade | Feature | Status | Resultado esperado |
|------------|---------|--------|--------------------|
| P0 | Remover logs com dados sensíveis | Planejado | Nenhum PHI em `Console.WriteLine`, logs, commits ou exemplos |
| P0 | Criar review prompt de segurança/PHI | Planejado | Checklist reutilizável em PRs |
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

Serviços/documentos a planejar:

- `Documentation/Technical/api-contract.md`.
- Review prompt de segurança/PHI.
- Skill `sql-migration-slice`.
- Skill `port-legacy-atendimento`.
- Verifier checklist para conclusão de PR.

### WS07 — Frontend Web

| Prioridade | Feature | Status | Resultado esperado |
|------------|---------|--------|--------------------|
| P0 | Alinhar Paciente com SQL/API atual | Planejado | UI validada contra backend SQL |
| P0 | Integrar Prontuario e Agendamento com novos contratos | Planejado | Tabs e services coerentes com API SQL |
| P0 | Planejar Atendimento no front | Planejado | Service/State/UI após backend estabilizar regras |
| P1 | Padronizar `ApiResponse<T>` e tratamento de erro | Planejado | Feedback claro para usuário e menos duplicação |
| P1 | Criar ErrorBoundary/toasts/loading states | Planejado | Melhor experiência sem grande redesign |
| P1 | Cache, refresh e invalidação simples | Planejado | Evitar chamadas desnecessárias sem arquitetura complexa |
| P2 | Debounce em buscas e autocomplete | Planejado | Buscar paciente por nome/CPF com UX melhor |
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

## 8. Informações Preservadas

A versão anterior do PM continha backlog bruto, sprints antigas, itens de frontend V2, sugestões de UX/produto, arquitetura avançada e riscos iniciais. Essas informações foram consolidadas neste documento em MVP1, MVP2, MVP3/Futuro e Gestão de Riscos.

## 9. Referências

- PRD: [PRD.md](PRD.md)
- RoadMap: [RoadMap.md](RoadMap.md)
- Estado atual: [../State.md](../State.md)
- Domínio: [../Architecture/Domain_Overview_Business_Rules.md](../Architecture/Domain_Overview_Business_Rules.md)
- Migração SQL: [../Technical/migration-sql.md](../Technical/migration-sql.md)
