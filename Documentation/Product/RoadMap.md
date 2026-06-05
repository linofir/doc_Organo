# RoadMap — Doc Organo

> Futuro do produto e horizontes de evolução.  
> Backlog operacional e prioridades do MVP2 ficam em [PM_DocOrgano.md](PM_DocOrgano.md).

## 1. Papel deste documento

Este RoadMap registra direções futuras do Doc Organo. Ele não deve substituir o PM nem virar lista de tasks. Quando uma capacidade entra em execução, ela deve ser detalhada no PM ou em documentos SDD específicos.

Responsabilidades:

- **RoadMap:** para onde o produto pode evoluir.
- **PM:** o que será priorizado no MVP atual.
- **PRD:** por que o produto existe e quais problemas resolve.
- **ADR:** decisões técnicas duráveis.

## 2. Horizonte Atual — MVP2

O MVP2 está em execução/planejamento e deve ficar detalhado no PM. O foco estratégico é:

- Migração SQL dos fluxos clínicos principais.
- Confiabilidade de dados e testes.
- Segurança básica e tratamento de PHI.
- Organização dos serviços criados no MVP1.
- Frontend alinhado aos novos contratos do backend.
- Documentação e harness de IA mais confiáveis.

Não repetir aqui a lista de features do MVP2; ver [PM_DocOrgano.md](PM_DocOrgano.md).

## 3. Horizonte MVP3 — Produto Clínico Avançado

### 3.1 Checklist clínico configurável

Sistema configurável de checklists clínicos para definir requisitos obrigatórios antes da progressão de etapas do atendimento.

Objetivos:

- Garantir que etapas clínicas importantes não sejam esquecidas.
- Padronizar protocolos médicos.
- Melhorar segurança do atendimento.
- Reduzir regras hard-coded quando o domínio estiver mais estável.

### 3.2 Atendimento State Machine

Evolução do fluxo de Atendimento para uma máquina de estados formal, caso isso simplifique validações e reduza risco.

Etapas previstas:

- Consulta.
- Pré-Procedimento.
- Procedimento.
- Pós-Procedimento.
- Finalizado.

### 3.3 Clinical Event Timeline

Linha do tempo clínica para registrar e visualizar eventos relevantes cronologicamente.

Exemplos:

- Consulta realizada.
- Exames solicitados.
- Resultado de exame recebido.
- Autorização hospitalar liberada.
- Procedimento realizado.
- Consulta pós-operatória.

## 4. Horizonte Financeiro

### 4.1 Persistência do demonstrativo financeiro

O sistema poderá persistir dados do demonstrativo financeiro como fonte externa importada e auditável.

Objetivos:

- Estruturar dados por hospital, lote, guias e itens financeiros.
- Persistir dados para consulta e verificação.
- Evitar dependência do arquivo externo após importação.
- Criar base para auditoria financeira.

### 4.2 Conciliação financeira

Após a persistência financeira estar estável, o sistema poderá conciliar dados internos com demonstrativos externos.

Possíveis conciliações:

- Procedimentos realizados em internação.
- Consultas associadas a atendimento/prontuário.
- Exames, quando aplicável.
- Valores pagos vs. valores esperados.

## 5. Segurança, Identidade e Auditoria

### 5.1 Identity and Authentication

O sistema deverá evoluir para autenticação completa quando sair do ambiente controlado ou quando a operação exigir maior segregação de acesso.

Possíveis tecnologias:

- ASP.NET Identity.
- JWT Authentication.
- OAuth providers, se houver necessidade real.

### 5.2 RBAC

Controle de acesso por perfil.

Perfis previstos:

- Médico.
- Secretária.
- Enfermeira.
- Administrador.
- Paciente.

### 5.3 Advanced Audit Logging

Auditoria detalhada de alterações no banco de dados.

Informações possíveis:

- Usuário responsável pela alteração.
- Data da alteração.
- Valores anteriores.
- Valores novos.

## 6. Infraestrutura e Operação

Evolução futura de ambiente local/controlado para infraestrutura mais robusta.

Possíveis caminhos:

- Cloud hosting para API e frontend.
- Banco de dados gerenciado.
- Monitoramento e logging centralizado.
- Deploy automatizado.
- Hardening de servidor local/HomeLab, se continuar relevante.

## 7. Segurança de Dados e Compliance

Como o sistema lida com dados clínicos, melhorias futuras podem incluir:

- Criptografia de dados sensíveis.
- Controle de acesso avançado.
- Auditoria completa.
- Políticas de retenção e exclusão lógica.
- Conformidade com regulamentações aplicáveis de proteção de dados.

## 8. Testes e Qualidade

Evolução futura da estratégia de testes:

- Testes unitários por domínio.
- Testes de integração SQL.
- Testes de serviços de extração com arquivos exemplo.
- Testes de componentes no frontend quando houver estabilidade.
- E2E tests para os fluxos clínicos centrais.

## 9. Mobile e UX Avançado

A experiência mobile/MAUI deve permanecer como horizonte futuro, não como prioridade do MVP2.

Possibilidades:

- Blazor Hybrid + MAUI.
- Responsividade avançada.
- Design system mais completo.
- AppShell e fluxos mobile específicos.

## 10. Critério para mover item do RoadMap para PM

Um item sai deste RoadMap e entra no PM quando:

1. Existe problema claro e atual.
2. Há benefício para o MVP em andamento.
3. O escopo pode ser fatiado em features pequenas.
4. Existem critérios de aceite ou validação.
5. A implementação não conflita com prioridades mais básicas.

## 11. Informações Preservadas

A versão anterior continha persistência do demonstrativo, conciliação financeira, checklist clínico, state machine, timeline clínica, auditoria avançada, RBAC, identity, infraestrutura, segurança/compliance e E2E. Esses temas foram preservados e reorganizados como horizontes.
