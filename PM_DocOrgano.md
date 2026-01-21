# Plano de Gestão de Projeto - DocAPI (Doc_Organo)

Este documento serve como um guia central para o desenvolvimento, planejamento e execução do projeto DocAPI. Ele será mantido e atualizado conforme o projeto evolui.

## 1. Visão do Projeto (Project Vision)

**Tornar-se a plataforma central e inteligente para a gestão de dados de um consultório médico, automatizando processos, centralizando informações de pacientes e otimizando o fluxo de atendimento, desde a consulta inicial até o acompanhamento pós-procedimento.**

## 2. Objetivos Estratégicos (Strategic Goals)

- **Centralização de Dados:** Unificar informações de pacientes, prontuários, agendamentos e atendimentos em um único local, eliminando a dependência de múltiplas planilhas e documentos dispersos.
- **Automação de Processos:** Automatizar tarefas repetitivas como a extração de dados de demonstrativos financeiros (HTML/XLS) e a verificação de senhas de convênio.
- **Otimização do Fluxo de Atendimento:** Mapear e gerenciar ativamente as etapas do atendimento ao paciente (Consulta, Pré-Procedimento, Procedimento, Pós-Procedimento), fornecendo ao médico uma visão clara do status de cada paciente.
- **Geração de Relatórios:** Simplificar a criação de documentos essenciais, como relatórios completos de pacientes e termos cirúrgicos, a partir dos dados já cadastrados.
- **Robustez e Escalabilidade:** Evoluir a arquitetura do sistema, migrando de uma base de dados em Google Sheets para um banco de dados relacional (via Entity Framework) e implementando uma suíte de testes automatizados.

## 3. Metodologia de Desenvolvimento (Development Methodology)

Adotaremos uma abordagem híbrida, combinando os pontos fortes do **Scrum** e do **Kanban**.

- **Scrum:** O trabalho será organizado em **Sprints**, ciclos de desenvolvimento com duração fixa (sugestão: 2 semanas). Cada Sprint terá uma meta clara e um conjunto de tarefas selecionadas do *Product Backlog*. Isso nos dará ritmo e previsibilidade.
- **Kanban:** Utilizaremos um quadro Kanban para visualizar o fluxo de trabalho. Isso nos ajudará a gerenciar o trabalho em progresso (WIP), identificar gargalos e promover a melhoria contínua. As colunas do quadro podem ser: `Backlog` -> `A Fazer (Sprint)` -> `Em Desenvolvimento` -> `Em Revisão/Teste` -> `Concluído`.

**Ferramentas Sugeridas:**
- **Quadro Kanban:** GitHub Projects. 
- **Controle de Versão:** Git com o fluxo de *feature-branch* de acordo com o `Readme.md`.

## 4. Product Backlog (Épicos e Features)

Este é o backlog inicial, derivado do seu `ToDo Product Log`. Ele está organizado em Épicos (grandes blocos de funcionalidade) e Features (entregas de valor).

---
### **Épico: 🚀 Gestão (Core do Negócio)**
- **Feature:** Estruturar desenvolvimento. OK
- **Feature:** Definir Documentação.        
- **Feature:** Criar Springs recorrentes de gestão.
- **Feature:** Criar Épico Infra.

### **Épico: 🚀 Estruturar Atendimento (Core do Negócio)**
- **Feature:** Modelagem e CRUD do fluxo de atendimento (`Atendimento`). OK
- **Feature:** Implementar a lógica de validação das etapas do atendimento (Consulta, Pré-Op, Procedimento, Pós-Op).
- **Feature:** Automação dos atendimentos.

### **Épico: 📄 Gestão de Prontuários**
- **Feature:** Extração de dados de prontuário a partir de um arquivo PDF.
- **Feature:** CRUD completo para `Prontuario`.
- **Feature:** Geração de documentos específicos (Termo Cirúrgico, Pedidos).

### **Épico: 📊 Extração e Análise de Dados Externos**
- **Feature:** Serviço de extração de dados financeiros de arquivos HTML (`CollectDemonstrativoDataService`).
- **Feature:** Serviço de extração de senhas autorizadas de arquivos HTML (`CollectSenhasAutorizadasDataService`).
- **Feature:** Armazenamento e versionamento dos dados financeiros extraídos.

### **Épico: 📅 Gestão de Agendamentos**
- **Feature:** CRUD completo para `Agendamento`.
- **Feature:** Integração com o serviço de senhas para validação automática de status.

### **Épico: 🧑‍⚕️ Gestão de Pacientes**
- **Feature:** CRUD completo para `Paciente` e `Endereco`.
- **Feature:** Busca de pacientes por múltiplos critérios (CPF, Nome).

### **Épico: 🏗️ Infraestrutura
-  **Feature:** modelo ERD completo.    
-  **Feature:** Migrar persistência de dados do Google Sheets para um banco de dados relacional com Entity Framework.
-  **Feature:** Gerar o DbContext (DocDbContext).
-  **Feature:** Criar migrations para subir no Azure SQL.
-  **Feature:** Substituir gradualmente seus repositórios Sheets → SQL.

### **Épico: 🏗️ Débitos Técnicos**
- **Feature:** Implementar um sistema de Logging robusto em toda a aplicação.
- **Feature:** Criar suíte de testes unitários e de integração (xUnit).
- **Feature:** Configurar autenticação e autorização com .NET Identity.
- **Feature:** Criar UnitOfWork que agrupa repositórios e DbContext.
- **Feature:** implementar cache
- **Feature:** Padronizar respostas de endpoints e dtos
- **Feature:** Padronizar controllers

----------- 
### **Épico: 🖥️ Frontend (DocFront WEB)** 
- **Feature:** Criar a base técnica web (Blazor + MAUI). (feature/Integracao_API)
    - **Itens:** 
        - `[Task Done]`  **Projeto** DocFront.Web (Blazor Server — para web) 
        - `[Task Done]`  **Pastas** /Pages, /Components, /Models, /Services, /Utils, /Styles.
        - `[Task Done]`  **Integração** Associação com inicial API Injetar `HttpClient`, configurar `appsettings.json`.
        - `[Task Done]`  **Integração** Configurar program.cs.
        - `[Task Done]`  **Teste:** Teste Iniciais.
- **Feature:** Integração com API (backend pronto)
    - **Itens:** 
        - `[Task Done]`  **Serviços** Criar ApiService base com HttpClient injetado (DI).
        - `[Task Done]`  **Settings** Configurar appsettings.json com URL do backend.
        - `[Task Done]`  **Wrappers** Criar wrappers para endpoints. Mapear principais endpoints.
        - `[Task Done]`  **Erros** Validar tratamento de erro local paciente (BadRequest, 500 etc). Criar tratamento de erros friendly
        - `[Task Done]`  **Loading** Implementar loading states e retry. Loading global
        - `[Task Done]`  **Models** Criar modelos C# idênticos aos DTOs.
        - `[Task Done]`  **viweModels** Criar modelos para exibição (ViewModels quando necessário).
        - `[Task Done]`  **Components** Componentes básicos para pacientes, tabs, cards, aba, button. 
        - `[Task Done]`  **Páginas** páginas base para Pacientes.
- **Feature:** Integração com API (prontuarios, Agendamento)(feature/integracao_models)
    - **Itens:** 
        - `[Task]`  **Models** Criar view modelos e Dtos necessários.
        - `[Task]`  **Serviços** Criar ApiService base com HttpClient injetado (DI), implementar wrappers endpoints.
        - `[Task]`  **Components** Componentes básicos para novas entidades, tabs, cards, aba, button. 
        - `[Task]`  **Páginas** Ajustar páginas base para entidades, Criar, Detalhes.
        - `[Task ]`  **Erros** Validar tratamento de erro local básico
- **Feature:** Integração com API (Agendamento)(feature/integracao_models_Agendamento) Ativa
    - **Itens:** 
        - `[Task]`  **Models** Criar view models e Dtos necessários.
            - Models para distinguindo as classes de Agendamento e Enums necessários
            - Dtos do paraa Agendamento
            - Ajustes para a implementação
        - `[Task]`  **Serviços** Criar ApiService base com HttpClient injetado (DI), implementar wrappers endpoints.
            - Adequação de ApiService
            - Criação de AgendamentoService
            - Criação de AgendamentoMapper, PacienteMapper
            - Adequação da Api, novo endpoint de Agendamento
        - `[Task]`  **Páginas** Ajustar páginas base para entidades, Criar, Detalhes.
            - Adequção da página Detalhes
        - `[Task]`  **Components** Componentes básicos para novas entidades, tabs, cards, aba, button. 
            - cards Agendamento
            - AbaAgendamento
            - Componente AgendamentoList 
            - Componentes de section da AbaAgendamento
        - `[Task ]`  **Erros** Validar tratamento de erro local básico
        

        
- **Feature:** Tools / Observabilidade
    - **Itens:** 
        - `[Task]`  **debugger** Configurar logging local (ILogger).
        - `[Task]`  **Settings** Criar ErrorBoundary global. (Interceptor HTTP)
        - `[Task]`  **Wrappers** Adicionar toast / popup para erros e sucesso.
        - `[Task]`  **Wrappers** Loading states (Skeleton / Spinner)
- **Feature:** Store de Dados (State Management)
    - **Itens:** 
        - `[Task]`  **Ferramente** Scoped Services ou fluxor.
        - `[Task]`  **Cache** Criar cache para os dados necessários.
        - `[Task]`  **Funcionalidades** Implementar refresh e invalidação de cache.(ex: IMemoryCache)
        - `[Task]`  **Funcionalidades** Implementar debounce para chamadas (evitar spam).
- **Feature:** Validação
    - **Itens:** 
        - `[Task]`  **Funcionalidades** validar funcionalidades básicas.
        - `[Task]`  **Erros** Tratar erros da API com feedback ao usuário, global.
        - `[Task]`  **Funcionalidades** Implementar refresh e invalidação de cache.
        - `[Task]`  **Funcionalidades** Implementar debounce para chamadas (evitar spam).
### **Épico: 🖥️ Frontend (DocFront) V2**
- **Feature:** Criar a base técnica do aplicativo (Blazor hybrid + MAUI). 
    - **Itens:** 
        - `[Task]`  **Projeto** Criação/Estruturação do projeto Blazor Server. Blazor Hybrid + .NET MAUI.(2)
        - `[Task]`  **Pastas** /Pages, /Components, /Models, /Services, /Utils, /Styles.
        - `[Task]`  **Ferramentas** .NET SDK, workload MAUI, Android/iOS emuladores.(2)
        - `[Task]`  **Ferramentas** emuladores para testes, Android/IOS(2)
        - `[Task]`  **Interface** Criar AppShell com rotas definidas.(2)
        - `[Task]`  **Integração** Associação com inicial API Injetar `HttpClient`, configurar `appsettings.json`.
        - `[Task]`  **Teste:** Teste desde cedo em emuladores Android/iOS.
### **Épico: 🖥️ Pendencias (DocFront) V2**
- **Feature:** novas implementações. 
    - **Itens:** 
        - `[Task]`  **Testes** Testes unitários de Services
        - `[Task]`  **Testes** Testes de componentes (bUnit)
        - `[Task]`  **Nav** Ajustes de navegação caso atualize a página.
        - Limpar messagens de erro e sucesso
        - depois da execusao de enpoints destinar para páginas corretas. 
        - Carregar as opções/enums para edicao e criacao  
        - busca por nome e cpf para pacientes, caminho para criar prontuario, epecificar o nome da paciente depois d um novo paciente: não precisa procurar os prontuarios(criar essa excessão). Fazer mapper de pacientes
        - padronizar nomenclaturas dtos, models, components
        - padronizar endpoints, associando com api
        - ajustes da de UI para Prontuarios, uso de enuns( adequar api para fornecer lista de exames, alterar section exames)


       
- **Feature:** UI/UX. 
    - **Itens:** 
        - `[Task]`  **Componentes** CAjustar componentes para mobile.
        - `[Task]`  **Validação** Responsividade e UX para mobile .

### **Épico: 🖥️ UX / UI & Layout**
- **Feature:** Layout/ UI
    - **Itens:** 
        - `[Task]`  **Mock-Up** Criar mockups no Figma.
        - `[Task]`  **Biblioteca de Componentes:** Criar Design System Base (cores, espaçamento, fontes).
        - `[Task]`  **Biblioteca de Componentes:** Desenvolva os componentes de UI com design responsivo.(Sidebar/navbar, cards, forms padrões, botões, loaders, dialog modal)
        - `[Task]`  **Pagina** Desenvolvimento de telas para visualização e cadastro de todos os dados. UI Base + Navegação
        - `[Task]`  **Responsividade** Implementar responsividade (CSS + Flex + Grid).
- **Feature:** Branding.
    - **Itens:** 
        - `[Task]`  **Moodboard** Criar moodboard visual.
        - `[Task]`  **Referências** Definir paleta de cores.
        - `[Task]`  **Referências** Definir tipografia.
        - `[Task]`  **Identidade** Criar identidade mínima (ícone do app).
        
### **Épico: 🖥️ MVP**
- **Feature:** Build, Deploy e Testes
    - **Itens:** 
        - `[Task]`  **Build** Criar pipeline para gerar Build Windows (.msix ou .exe) e Build Android (.apk)
        - `[Task]`  **mobile** Deploy local no celular (USB).
        - `[Task]`  **Testes** Testar performance e responsividade Window/Android.
        - `[Task]`  **Documentação** Documentar instruções de instalação do app.
- **Feature:** MVP Final / Demonstração.
    - **Itens:** 
        - `[Task]`  **Apresentação** Criar script da apresentação.
        - `[Task]`  **Screens** Criar vídeo curto (screen capture).
        - `[Task]`  **Documentação** Criar PDF com arquitetura geral.
        - `[Task]`  **App2.0** Criar backlog para versão 2.0.
    
-Sugestões de [epicos]
🔜 UX / Produto
 Toasts globais de feedback
 Confirmações padronizadas
 Undo lógico para exclusão
 Bloqueio de navegação com alterações não salvas
 Auditoria (created_at, updated_at)

🔜 Arquitetura Avançada
 CQRS para entidades críticas
 Mediator no backend
 Autorização por perfil
 Feature flags
 Domain Events
---

## 5. Roteiro de Produto (Product Roadmap - 3 Sprints Iniciais)

### **Sprint 0: Estruturar desenvolvimento**
- **Meta:** **Estruturar e criar ferramentas para a gestão do projeto**
- **Itens:**
    - `[Feature Done]` Ferramentas de Gestão.
    - `[Task Done]` ~~Configurar o quadro Kanban (ex: GitHub Projects) com as colunas do fluxo de trabalho~~.
    - `[Task Done]` ~~Validar e formalizar as diretrizes do Git Flow no `Readme.md`~~.
    - `[Refactor Done]` ~~Refatorar Readme~~.
    - `[Task Done]` Revisar e refinar os Épicos e o Roadmap inicial (este documento).
    - `[Task Done]` Criar base da documentação do DOc_Organo.
    - `[Task Done]` Criar Kanban backlog para próxima sprint.
### **Sprint 1: Fundações do Atendimento**
- **Meta:** **Estabilizar o fluxo de atendimento e a geração de relatórios básicos.**
- **Itens:**
    - `[Feature Done]` Versão do modelo `Atendimento`.
    - `[Task Done]` **Model:** Crie ou altere a classe da entidade em `DocAPI/Core/Models`.
    - `[Task Done]` **DTOs:** Crie ou ajuste os Data Transfer Objects (DTOs) em `DocAPI/Data/Dtos/` para expor os dados de forma segura.
    - `[Task Done]` **AutoMapper:** Atualize ou crie um `Profile` para mapear a entidade para seus DTOs.
    - `[Task Done]` **Interface do Repositório:** Defina os contratos necessários na interface correspondente em `DocAPI/Core/Repositories`.
    - `[Task Done]` **Repositório (Sheets):** Implemente a lógica de acesso aos dados na classe de repositório em `DocAPI/Infrastructure/SheetsDb`.Isso envolve a comunicação com o serviço `GoogleSheetsDB`. Implementar o `AtendimentoSheetsRepository` com CRUD básico e a lógica de    `AtualizarAtendimento`, refatorar `ValidacaoEtapaConsulta` e `ValidacaoPreProcedimento` para serem mais robustos.
    - `[Task Done]` **Controller:** Crie ou atualize o Controller em `DocAPI/Controllers` para expor os novos endpoints.
    - `[Test Done]` **Testes:** Valide as novas rotas e a lógica utilizando o Postman. Criar testes unitários para o `AtendimentoService` (se já  existir) ou para a lógica no repositório.
    - `[Test Done]`  Elaborar próximo Épico. Criar próxima spring
### **Sprint : Infraestrutura SQL**
- **Meta:** **Implementar DB**
- **Itens:**
    - `[Task]` **Épico:** Criar épico`.
    - `[Task]` **ERD:** Criar épico`.
### **Sprint : FrontENd**
- **Meta:** **Interface**
- **Itens:**
### **Sprint : Infraestrutura**
- **Meta:** **SQL DB**
- **Itens:**
### **Sprint : Automação da Coleta de Dados**
- **Meta:** **Automatizar a extração de dados de senhas e integrá-la ao fluxo de atendimento.**
- **Itens:**
    - `[Feature]` Finalizar o `FileDataOfSenhaExtractorService` para extrair senhas de arquivos HTML.
    - `[Task]` Implementar a funcionalidade de salvar e carregar o JSON de senhas (`SaveDescritivo`, `LoadDescritivosFromFile`).
    - `[Task]` Integrar a verificação de senhas na etapa `ValidacaoPreProcedimento` do `AtendimentoSheetsRepository`.
    - `[Refactor]` Melhorar o tratamento de erros e logging nos serviços de extração de dados.
    - `[Test]` Criar testes para o `FileDataOfSenhaExtractorService` usando um arquivo HTML de exemplo.

### **Sprint : Pagando Débitos Técnicos e BD Relacional**
- **Meta:** **Iniciar a migração para um banco de dados real e melhorar a qualidade do código.**
- **Itens:**
    - `[Refactor]` Refatorar métodos de busca genéricos (ex: `GetByFilterAsync`) para evitar duplicação de código.
    - `[Refactor]` Alinhar os Enums (ex: `StatusAgendamento`) com os valores exatos usados nas planilhas para evitar erros de parse.
    - `[Feature]` Definir o `DbContext` do Entity Framework com os modelos (`Paciente`, `Atendimento`, `Prontuario`, `Agendamento`).
    - `[Task]` Criar a primeira `migration` para o novo banco de dados.
    - `[Task]` Criar um `PacienteEFRepository` como primeira implementação usando o EF Core, para substituir o `PacienteSheetsRepository`.

## 6. Gestão de Riscos e Desafios

| Risco/Desafio                               | Probabilidade | Impacto | Plano de Mitigação                                                                                                                                         |     |
| :------------------------------------------ | :------------ | :------ | :--------------------------------------------------------------------------------------------------------------------------------------------------------- | --- |
| **Dependência do Google Sheets**            | Alta          | Alto    | Priorizar a migração para um BD relacional (iniciar na Sprint 3). O Sheets não é escalável e pode levar a erros de concorrência.                           |     |
| **Formatos de Arquivos Externos Instáveis** | Média         | Alto    | Criar uma suíte de testes para os serviços de extração com vários arquivos de exemplo. Implementar logging detalhado para diagnosticar falhas rapidamente. |     |
| **Ponto Único de Falha (1 Desenvolvedor)**  | Alta          | Alto    | Manter este documento (`PROJECT_MANAGEMENT.md`) e o `Readme.md` sempre atualizados. Documentar decisões de arquitetura no código ou em um local central.   |     |
| **Acúmulo de Débito Técnico**               | Média         | Média   | Dedicar tempo em cada Sprint para refatoração (como planejado na Sprint 3). Adotar um linter e analisadores de código estático.                            |     |


---

## Perguntas e Respostas

> **Como seria a melhor forma de armazenar esse histórico de desenvolvimento? O Kanban é usado para isso?**

Ótima pergunta! A resposta se divide em duas partes:

1.  **O "Quê" (Decisões e Artefatos):** O histórico de *decisões de arquitetura*, *planejamento de Sprints* e a *visão do produto* deve ser armazenado em arquivos Markdown no próprio repositório Git, assim como este que acabamos de criar. O `Readme.md` e o `PROJECT_MANAGEMENT.md` são exemplos perfeitos. O Git, por natureza, versiona esse histórico para você.

2.  **O "Como" (Fluxo de Trabalho):** O **Kanban** não armazena o histórico, mas sim **visualiza o estado atual e o fluxo do trabalho**. O histórico do *trabalho realizado* é, na verdade, uma combinação de:
    - **Commits do Git:** A fonte da verdade mais granular. Boas mensagens de commit são essenciais (`feat: Adiciona endpoint para criar prontuário via PDF`).
    - **Pull Requests (PRs):** Documentam a discussão, a revisão e a integração de uma feature.
    - **Cartões Movidos para "Concluído":** Ferramentas como Jira ou GitHub Projects mantêm um registro dos cartões que foram concluídos em cada Sprint ou período, o que gera relatórios de *velocidade* e *lead time*.

**Em resumo:** Use o **Git + Markdown** para o histórico de *planejamento e decisões* e o **Quadro Kanban/Scrum** para gerenciar e visualizar o *fluxo de execução*, cujo histórico fica registrado nos commits, PRs e na própria ferramenta do quadro.

---



Ideias de prompt:
<!--
[PROMPT_SUGGESTION]Como posso configurar um quadro Kanban no GitHub Projects para este projeto?[/PROMPT_SUGGESTION]

