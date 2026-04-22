# Domain Overview

## Business Rules

O **Doc Organo** é uma plataforma de gestão de atendimentos médicos orientada ao fluxo clínico e preservação do histórico da paciente.

O sistema foi modelado a partir de regras de negócio observadas na rotina real de consultórios médicos, cobrindo desde o cadastro da paciente até o acompanhamento pós-operatório.

O domínio foi projetado para:

- Preservar histórico clínico através de **versionamento de prontuários**
- Gerenciar a evolução do atendimento ao longo do tempo
- Separar claramente responsabilidades entre entidades do sistema
- Melhorar a eficiência da comunicação com a Paciente

Principais separações conceituais:

- **Paciente** → cadastro da pessoa atendida
- **Prontuário** → registro clínico de eventos médicos
- **Agendamento** → planejamento logístico de procedimentos
- **Atendimento** → gestão da jornada clínica da paciente

Essa separação permite representar corretamente a realidade clínica, onde múltiplos registros podem existir ao longo do tratamento.

---

# Entidades do Domínio

## Paciente

Representa a pessoa atendida pelo consultório.

É responsável pelo armazenamento das informações cadastrais e serve como raiz para diversas informações clínicas associadas.

### Responsabilidades

- Identificação da paciente
- Armazenamento de dados cadastrais
- Controle de informações de contato
- Registro de convênio

### Relacionamentos

Uma paciente pode possuir:

- Vários **Prontuários**
- Vários **Atendimentos**
- Vários **Agendamentos**

### Value Objects

- Endereço

---

## Prontuário

O prontuário representa um **snapshot clínico** de um momento específico do atendimento.

Cada evento clínico relevante pode gerar um novo prontuário.

### Características

- Imutável após criação
- Versionado
- Associado a um atendimento
- Pode possuir exames e internação

### Responsabilidades

- Registrar dados clínicos
- Preservar histórico médico
- Permitir acompanhamento evolutivo da paciente

---

## Internação

Representa o contexto clínico necessário para realização de um procedimento hospitalar.

Cada prontuário pode gerar **no máximo uma internação**.

### Responsabilidades

- Registro da indicação clínica
- Associação com CID
- Informações administrativas hospitalares
- Controle de procedimentos realizados

### Relacionamentos

- 1:1 com Prontuário
- 1:N com Procedimentos de Internação

---

## Procedimentos de Internação

Representa procedimentos realizados dentro de uma internação.

Uma internação pode possuir múltiplos procedimentos.

Esse modelo evita duplicação de dados clínicos e mantém a estrutura normalizada.

---

## Exames

Representa exames solicitados no contexto de um prontuário.

Cada prontuário pode gerar múltiplos exames.

### Responsabilidades

- Registrar exames solicitados
- Controlar status de execução
- Registrar datas de solicitação e resultado

---

## Agendamento

O agendamento representa o planejamento logístico de procedimentos médicos.

Ele controla o processo administrativo necessário para que um procedimento ocorra.

### Responsabilidades

- Controle de datas e horários
- Registro do local do procedimento
- Controle de autorizações hospitalares
- Gestão de senhas de autorização

### Exemplos de dados controlados

- Data e horário
- Local e sala
- Status do agendamento
- Autorização hospitalar
- Senha de liberação de procedimento

---

## Atendimento

Representa a **jornada completa da paciente em um ciclo de cuidado**.

Enquanto o prontuário registra dados clínicos, o atendimento controla o **fluxo do tratamento**.

### Responsabilidades

- Monitorar progresso do atendimento
- Consolidar informações do tratamento
- Identificar pendências
- Controlar etapas clínicas

### Etapas do atendimento

- Consulta
- Pré-procedimento
- Procedimento
- Pós-procedimento

Cada etapa possui verificações específicas antes de permitir avanço no fluxo.

---

# Controle de Pendências e Alertas

O sistema possui um mecanismo de **pendências clínicas e administrativas** associadas ao atendimento.

Esse mecanismo permite identificar bloqueios ou ações necessárias antes da progressão do atendimento.

### Exemplos de pendências

- Exames não realizados
- Autorização hospitalar pendente
- Termo cirúrgico não assinado
- Consulta pós-operatória pendente

Essas informações são utilizadas para:

- Alertar profissionais de saúde
- Impedir progressão prematura de etapas
- Facilitar visualização do estado do atendimento
- MAnter a comunicação com a Paciente otimizada

---

# Ubiquitous Language

Os seguintes termos são utilizados consistentemente no sistema:

Paciente  
Pessoa atendida pelo consultório.

Prontuário  
Registro clínico de um evento médico.

Agendamento  
Planejamento logístico de um procedimento.

Atendimento  
Jornada completa da paciente durante um ciclo de cuidado.

Internação  
Contexto clínico necessário para realização de procedimento hospitalar.

Procedimento  
Intervenção realizada durante uma internação.

Versionamento de Prontuário  
Estratégia para preservar histórico clínico.

Value Object  
Objeto sem identidade própria.

Aggregate Root  
Entidade responsável por garantir consistência dentro de um agregado.

---

# Aggregate Roots

## Paciente

Responsável por:

- Identidade da paciente
- Dados cadastrais
- Relacionamentos clínicos principais

---

## Prontuário

Responsável por:

- Registro clínico
- Versionamento
- Associação com exames e internações

---

## Agendamento

Responsável por:

- Planejamento administrativo de procedimentos
- Controle de autorizações
- Organização logística

---

## Atendimento

Responsável por:

- Orquestrar o fluxo do tratamento
- Controlar progresso clínico
- Identificar pendências
- Consolidar a jornada da paciente

---

# Entity vs Value Object

## Paciente

Value Object:

- Endereço

---

## Prontuário

Value Objects:

- Descrição Básica
- AGO
- Antecedentes
- Antecedentes Familiares
- Pós-operatório

---

# Versioning Strategy

## Prontuário

Cada alteração clínica gera **um novo prontuário**, preservando o histórico completo.

Campos utilizados:

- Versao
- ProntuarioAnteriorId
- CriadoEm
- CriadoPor

Benefícios:

- Histórico completo
- Auditoria clínica
- Segurança médica
- Simplicidade de implementação

---

# Auditoria

O sistema implementa auditoria básica para rastrear alterações em dados sensíveis.

Campos comuns:

- CriadoEm
- AtualizadoEm
- CriadoPor
- AtualizadoPor

Esses dados permitem identificar quando e por quem um registro foi criado ou modificado.

---

# Soft Delete

Para preservar integridade histórica, algumas entidades utilizam **Soft Delete**.

Em vez de remover dados fisicamente do banco, o registro é marcado como removido.

Campos utilizados:

- Deletado
- DeletadoEm

Isso evita perda de histórico clínico e mantém consistência entre registros relacionados.

---

# Evolução planejada

## Checklist clínico configurável

Sistema de checklists clínicos associados ao prontuário.

Exemplo:

- Solicitação de exames
- Assinatura de termo cirúrgico
- Orientações pós-operatórias

---

## State Machine de Atendimento

Implementação futura de máquina de estados para controlar formalmente o fluxo de atendimento.

Benefícios:

- Fluxo previsível
- Redução de erros operacionais
- Controle rigoroso das etapas clínicas

---

## Estrutura de permissões

Sistema de papéis para diferentes profissionais:

- Médico
- Secretária
- Enfermeira
- Administrador



////

## Decisões arquiteturais
### Reflexões para decisão da infraestrutura

* Isolamento de Recursos: "Quanto de RAM vou limitar para o SQL?" No Docker, você pode limitar o container para não 'comer' toda a memória do seu notebook e travar seu VS Code.

* Segurança (Secrets): Nunca coloca a senha do banco direto no código ou no docker-compose. Ele usa arquivos .env.

* Paridade de Ambiente: "O container que roda no meu notebook é exatamente igual ao que rodará na Azure?" Isso elimina o erro "na minha máquina funciona".

* Eficiência de Dev: Em vez de instalar o SQL Server "pesado" no Windows, o container é levantado apenas quando você vai codar (docker-compose up -d) e desligado depois, mantendo o notebook rápido.

### Trade-offs reais
🟢 Vantagens

✔ Ambiente idêntico ao Azure
✔ Isolamento
✔ Banco descartável
✔ Controle de memória
✔ Workflow profissional

🔴 Desvantagens

✔ Complexidade inicial maior
✔ Mais camadas para debug
✔ Docker consome RAM

## Evolução planejada