# Architecture Overview

## Visão Geral

O Doc Organo é uma aplicação backend orientada a domínio desenvolvida com foco em organização de fluxo clínico e preservação de histórico médico.

A arquitetura segue princípios de separação de responsabilidades e organização em camadas.

O objetivo é manter o domínio do sistema desacoplado da infraestrutura, permitindo evolução gradual da aplicação.

---

# Camadas da aplicação

A aplicação é organizada nas seguintes camadas:

### Domain

Contém as entidades principais do sistema e regras de negócio.

Inclui:

- Entidades
- Value Objects
- Regras de domínio
- Interfaces de repositório

Essa camada não possui dependência de infraestrutura.

---

### Application

Responsável por coordenar casos de uso da aplicação.

Inclui:

- Services
- DTOs
- Validações
- Orquestração de operações

Essa camada conecta o domínio com a infraestrutura.

---

### Infrastructure

Responsável por persistência e integração com recursos externos.

Inclui:

- Entity Framework
- Configurações de banco
- Repositórios
- Migrations

---

### API

Camada responsável pela exposição de endpoints HTTP.

Inclui:

- Controllers
- Configuração de rotas
- Serialização de respostas

---

# Persistência de dados

O sistema utiliza:

- **SQL Server**
- **Entity Framework Core**

O modelo relacional foi projetado para:

- preservar histórico clínico
- permitir consultas eficientes
- garantir integridade relacional

---

# Estratégia de versionamento clínico

Prontuários são tratados como registros imutáveis.

Alterações clínicas geram um novo prontuário com referência ao anterior.

Isso permite reconstruir toda evolução clínica da paciente.

---

# Infraestrutura de desenvolvimento

Atualmente o sistema roda **localmente** para desenvolvimento e testes.

O ambiente inclui:

- Backend em execução local
- Banco SQL Server local
- Ambiente de desenvolvimento isolado

---

# Estratégia futura de infraestrutura

O projeto foi planejado para evoluir para um ambiente em nuvem.

A infraestrutura poderá incluir:

- hospedagem de API em cloud
- banco de dados gerenciado
- controle de acesso seguro
- escalabilidade conforme demanda

A escolha de serviços em nuvem será feita de acordo com necessidades de:

- desempenho
- segurança
- custo operacional

---

# Ambiente de desenvolvimento

Para desenvolvimento local, está sendo utilizado um servidor local para testes.

Esse ambiente permite:

- validar arquitetura
- testar integrações
- executar migrations
- validar fluxo clínico completo

---

# Evolução da infraestrutura

No futuro, a aplicação poderá ser distribuída em camadas separadas:

API Server  
Database Server  
Serviços auxiliares (autenticação, monitoramento)

Essa evolução permitirá maior escalabilidade e segurança da aplicação.



////////// Posicionar melhor essas Requisições

----
# Planned Improvements

O Doc Organo foi projetado com uma arquitetura que permite evolução gradual do sistema.  
Algumas funcionalidades avançadas estão planejadas para versões futuras, conforme o sistema amadurece.

Essas melhorias visam aumentar segurança, rastreabilidade, controle clínico e escalabilidade do sistema.

---

# Clinical Checklist System

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

# Role-Based Access Control (RBAC)

O sistema poderá evoluir para um modelo de **controle de acesso baseado em papéis**.

Isso permitirá restringir funcionalidades de acordo com o perfil do usuário.

### Perfis previstos

Médico  
Secretária  
Enfermeira  
Administrador

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