# Planned Improvements

O Doc Organo foi projetado com uma arquitetura que permite evolução gradual do sistema.  
Algumas funcionalidades avançadas estão planejadas para versões futuras, conforme o sistema amadurece.

Essas melhorias visam aumentar segurança, rastreabilidade, controle clínico e escalabilidade do sistema.

---
# Etapa 1 — Persistência do Demonstrativo (Base)

O sistema passará a armazenar os dados do demonstrativo financeiro como uma fonte externa de verdade.

Esses dados não devem ser alterados manualmente após importação.

## Objetivos
- Estruturar os dados de acordo com o Hospital :Lote (agrupador), Guias, Itens financeiros (procedimentos, consultas, etc.)
- Persistir os dados a serem consultados e verificados.
- Auditabilidade 

## Benefícios
- Rastreabilidade completa dos pagamentos recebidos
- Base sólida para conciliação automática
- Possibilidade de auditoria financeira
- Independência do sistema externo (evita depender do arquivo externo depois)

--- 

# Etapa 2 — Conciliação Financeira

Após persistência dos dados, será implementado um mecanismo de conciliação entre sistema interno e demonstrativo externo.

## Objetivos
- conciliação dos Procedimentos realizados (Internação)
- Conciliação das Consultas (Atendimento / Prontuário)
- Consiliação dos Exames (se aplicável)
- Consiliação completa de Valores pagos vs valores esperados

## Benefícios
- Identificação de divergências financeiras
- Segurança no faturamento médico
- Redução de perdas financeiras
- Base para relatórios e indicadores

# checklist clinico

Uma melhoria planejada é a implementação de um **sistema configurável de checklists clínicos**.

Esse sistema permitirá definir requisitos obrigatórios antes da progressão de etapas do atendimento.

### Objetivos

- Garantir que etapas clínicas importantes não sejam esquecidas
- Padronizar protocolos médicos
- Melhorar segurança do atendimento

### Exemplos de itens de checklist

- Solicitação de exames obrigatórios
- Assinatura de termo cirúrgico
- Entrega de orientações pré-operatórias
- Confirmação de exames laboratoriais
- Agendamento de retorno

Cada tipo de prontuário poderá possuir checklists específicos.

---

# Atendimento State Machine

O fluxo de atendimento poderá evoluir para uma **máquina de estados formal (State Machine)**.

Atualmente o fluxo é controlado por validações no domínio, porém uma implementação baseada em estados permitirá maior controle e previsibilidade.

### Etapas previstas

Consulta  
Pré-Procedimento  
Procedimento  
Pós-Procedimento  
Finalizado

### Benefícios

- Fluxo clínico mais previsível
- Prevenção de transições inválidas
- Melhor rastreamento da jornada da paciente
- Simplificação da lógica de validação

---

# Clinical Event Timeline

Outra evolução planejada é a implementação de uma **linha do tempo clínica completa**.

Cada evento relevante do atendimento poderá ser registrado e visualizado cronologicamente.

### Exemplos de eventos

- Consulta realizada
- Exames solicitados
- Resultado de exame recebido
- Autorização hospitalar liberada
- Procedimento realizado
- Consulta pós-operatória

### Benefícios

- Visão completa da evolução do atendimento
- Melhor suporte à decisão clínica
- Histórico claro para auditoria médica

---

# Advanced Audit Logging

Uma evolução futura é a implementação de **auditoria detalhada de alterações no banco de dados**.

Além dos campos básicos de auditoria (CriadoEm, AtualizadoEm), o sistema poderá registrar alterações completas nos registros.

### Informações registradas

- Usuário responsável pela alteração
- Data da alteração
- Valores anteriores
- Valores novos

### Benefícios

- Rastreabilidade completa
- Conformidade com requisitos regulatórios
- Maior segurança de dados clínicos

---

#   

O sistema poderá evoluir para um modelo de **controle de acesso baseado em papéis**.

Isso permitirá restringir funcionalidades de acordo com o perfil do usuário.

### Perfis previstos

Médico  
Secretária  
Enfermeira  
Administrador
Paciente

### Exemplos de restrições

- Apenas médicos podem criar ou editar prontuários
- Secretárias podem gerenciar agendamentos
- Administradores podem gerenciar usuários

---

# Identity and Authentication

O sistema deverá integrar um mecanismo completo de autenticação e identidade.

Essa camada permitirá:

- Login seguro
- Controle de sessões
- Gestão de usuários
- Controle de permissões

A implementação poderá utilizar tecnologias como:

- ASP.NET Identity
- JWT Authentication
- OAuth providers

---

# Infrastructure Evolution

Atualmente o sistema está sendo executado em ambiente local para desenvolvimento e testes.

No futuro, a infraestrutura poderá evoluir para um ambiente em nuvem com maior escalabilidade e segurança.

Possíveis evoluções incluem:

- hospedagem da API em cloud
- banco de dados gerenciado
- monitoramento e logging centralizado
- escalabilidade conforme demanda

---

# Data Security and Compliance

Devido à natureza sensível dos dados clínicos, melhorias futuras poderão incluir mecanismos adicionais de segurança.

### Possíveis melhorias

- criptografia de dados sensíveis
- controle de acesso avançado
- logs de auditoria completos
- conformidade com regulamentações de proteção de dados

Essas melhorias garantirão maior segurança e confiabilidade do sistema em ambientes de produção.



# E2E tests

Front, application, infra , domain tests
