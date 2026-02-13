# Domain Overview
## Business Rules
O Doc Organo é uma plataforma de gestão de atendimentos médicos orientada a fluxo de atendimento e histórico clínico. O sistema foi modelado a partir de regras de negócio reais, observadas na rotina de consultórios médicos, cobrindo desde o cadastro da paciente até o acompanhamento pós-operatório.

* O domínio foi desenhado para:

* Preservar histórico clínico (imutabilidade de prontuários)

* Permitir evolução do atendimento ao longo do tempo

* Separar claramente planejamento (Agendamentos), registro clínico (Prontuários) e gestão do fluxo (Atendimento)

### Paceinte
Explicação conceitual
Parece alinhado com o domínio, não tem porblema conceitual e Vo embutido faz sentido no DB. Parece que fa sentido criar navegação para Prontuario já que uma paciente pode ter diversos prontuarios de diversos tipos e em momentos diferentes, assim como Agendamento. Para Atendmineto é preciso refletir mais sobre a regra de nogócio que farei abaixo.

### Em Prontuario 
Explicação conceitual: Uma paciente pode ter varios tipos de prontuariosao longo do atendimento, cada prontuario pode ter alterações, ou seja versões. Além disso, como existirão tipos diferentes de prontuario, alguns terão dados específicos, por exemplo o PosOP será somente utilizado em um prontuario de pos operação. Parece que faz sentido os VOs separados no DB. Seria melhor separar nos models tb?hoje são classes dentro do arquivo que contem a classe Prontuario. A lista de Exames como propriedade em prontuario é definida apos a geração de um prontuario, então, mesmo que seja relacionada a uma paciente é diretamente agregada a um prontuario. Pode acontecer inclusive um primeira consulta gera um prontuario que gera uma lista de exames, em um retorno pode ser gerado um novo prontuario e novos exames.

### Em Agendamento
Explicação conceitual:	Aqui é um controle dos agendamentos de procedimentos que acontecem ao longo do tratamento, por exemplo depois de uma consulta a médica define uma cirurgia(requisição do prontuário) A entidade cuida da gestão do procedimento desde a autorização do hospital até dados do procedimento agendado. Ainda tenho dúvida se é necessário atrelar á um prontuario

### Em Atendimento
Explicação conceitual:
Não será somente um agregador, é onde monitora o acompanhamento da paciente como um todo. desde o cadastro da paciente até o pós operatório da mesma.
Sim ele irá referenciar dados das outras entidades. posso aprofundar mais de como já modelei o model e o repositório(ainda não funcional)
Irá definir que etapa do atendimento a paciente se encontra. Por exemplo fez a consulta, fez os exames e está aguardando a cirurgia.
Irá conter dados únicos e gerados para a gestão do atendimento. por exemplo alertas de um acompanhamento pós operatório depois de um ano para que se faça uma verificação do estado da paciente.


## Ubiquitous Language
Os termos abaixo são utilizados de forma consistente no código, documentação e comunicação com stakeholders:

* Paciente: Pessoa atendida pelo consultório
* Prontuário: Registro clínico de um evento médico específico
* Agendamento: Planejamento de procedimentos 
* Atendimento: Jornada completa da paciente em um ciclo de cuidado
* Versionamento de Prontuário: Evolução histórica de um prontuário
* Value Object (VO): Estrutura de dados sem identidade própria
* Aggregate Root: Entidade principal que controla regras e consistência

## Aggregate Roots
### Paciente 

### Responsável por:
* Identidade da paciente
* Dados cadastrais
* Endereço (VO embutido)

### Relacionamentos:
* 1:N com Prontuários
* 1:N com Agendamentos
* 1:N com Atendimentos

## Prontuário 

### Características:
* Representar um registro clínico específico.
* Imutável após criação
* Versionado (histórico preservado)
* Pode assumir diferentes tipos (consulta, retorno, pós-op, followUp)

### Responsável por:
* Registros dos dados clínicos
* Exames e procedimentos associados
* Controle de versão e hitórico clínico

## Agendamento (Aggregate Root)

### Responsável por
* Planejamento/gerenciamento de procedimentos.

### Características:
* Independente de prontuário
* Pode ou não gerar registros clínicos
* Controla status, datas e autorizações do procedimento

## Atendimento (Aggregate Root worflow)


### Responsável por:
* Representa a jornada completa da paciente.
* Orquestrar e comunicar etapas do atendimento
* Consolidar informações
* Gerar alertas e pendências
* Permitir múltiplos atendimentos ao longo da vida da paciente

## Entity vs Value Object
### Paciente
* Endereço

### Prontuario
* Descrição Básica
* AGO
* Antecedentes
* Antecedentes Familiares
* Pós-Operatório
* Exames, internaçao

### Critérios adotados:

* VO não possui identidade própria
* VO depende totalmente do Aggregate Root
* VO pode ser persistido como:
- Colunas embutidas (ex: Endereço)
- Tabela própria (Owned Entity)

## Versioning Strategy
### Prontuário

#### Novas porps/campos principais:
* Versão
* Data de Criação
* ProntuarioAnteriorId

#### Benefícios e objetivos:
* Cada alteração gera um novo registro
* O histórico é preservado
* Auditoria completa
* Segurança clínica
* Simplicidade no MVP

## ERD
O ERD foi desenhado para:

* Coerência com o domínio
* Migração segura para SQL
* Compatibilidade com Entity Framework
* Relacionamentos explícitos
* Separação clara entre agregados
* Adequação de tipagem para persistência 

## Decisões arquiteturais

## Evolução planejada