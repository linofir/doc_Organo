## 1. Visão do Projeto (Project Vision)

**Doc Organo é uma plataforma para gestão inteligente de consultórios médicos**, focada em centralizar dados clínicos e administrativos, automatizar processos manuais e oferecer visibilidade clara do fluxo de atendimento do paciente, do primeiro contato ao pós-procedimento.

A plataforma foi desenhada para refletir o fluxo real dos processos médicos, não apenas um projeto somente de portfólio.

 ---
## 1.1 TL;DR Técnico

- Backend: ASP.NET Core (.NET 7), DDD, Repository Pattern, REST API
- Frontend: Blazor WebAssembly, State Management centralizado, fluxo unidirecional
- Persistência atual: Google Sheets (validação rápida)
- Status: MVP em uso real

---
## 2. Objetivos Estratégicos (Strategic Goals)

### Negócios:

- **Automação de Processos:** Automatizar tarefas repetitivas como a extração de dados de demonstrativos financeiros (HTML/XLS) e a verificação de senhas de convênio.

- **Otimização do Fluxo de Atendimento:** Mapear e gerenciar ativamente as etapas do atendimento ao paciente (Consulta, Pré-Procedimento, Procedimento, Pós-Procedimento), fornecendo ao médico uma visão clara do status de cada paciente, além de facilitar a comunicação com a paciente.

- **Geração de Relatórios:** Simplificar a criação de documentos essenciais, como relatórios completos de pacientes e termos cirúrgicos, a partir dos dados já cadastrados.

### Técnico:

- **Centralização de Dados:** Unificar e persistir informações de pacientes, prontuários, agendamentos e atendimentos em um único local, eliminando a dependência de múltiplas planilhas e documentos dispersos.

- **Robustez e Escalabilidade:** Evoluir a arquitetura do sistema, migrando de uma base de dados em Google Sheets para um banco de dados relacional (via Entity Framework) e implementando uma suíte de testes automatizados.

  
---

## 3. Metodologia de Desenvolvimento e conceitos (Development Methodology)

O projeto é desenvolvido como um produto real, com ciclos curtos de entrega, validação contínua com usuários finais e evolução arquitetural progressiva.

### Visão geral do projeto:

* O desenvolvimento é guiado por **feedback contínuo de médicos**, permitindo refinar processos internos e validar regras de negócio diretamente com usuários reais. Essa abordagem garante que o sistema reflita o fluxo real de um consultório médico, e não apenas uma modelagem teórica.

- **Scrum:** O trabalho é organizado em **Sprints**, ciclos de desenvolvimento com duração fixa. Cada Sprint tem uma meta clara e um conjunto de tarefas selecionadas do *Product Backlog*. para ritmo e previsibilidade no desenvolvimento.

- **Kanban:** É utilizado quadro GitHub Projects para visualizar o fluxo de trabalho. Isso ajuda a gerenciar o trabalho em progresso (WIP), identificar gargalos e promover a melhoria contínua. O modelo utilizado: `Backlog` -> `A fazer (Sprint)` -> `Em Desenvolvimento` -> `Em Revisão/Teste` -> `Concluído`.

- A arquitetura segue um padrão semelhante ao **Model-View-Controller (MVC)**, onde:

    - **Model:** As entidades de negócio em `DocAPI/Core/Models`.
    - **Controller:** Os endpoints da API em `DocAPI/Controllers`, que orquestram as requisições.
    - **View:** O frontend (separado), `DocFront`, que consumirá esta API.
   
### Na DocAPI(Backend):

* **Arquitetura em Camadas (Layers)**:

| Camada         | Função                                   | Pasta do projeto |
| -------------- | ---------------------------------------- | ---------------- |
| Apresentação   | Controllers e endpoints da API           | Controllers/     |
| Aplicação      | Orquestra regras e casos de uso          | Services/        |
| Domínio        | Entidades e interfaces de repositórios   | Core/            |
| Infraestrutura | Implementações concretas de persistência | Infrastructure/  |
| Dados          | DTOs, mapeamentos, configurações EF Core | Data/            |  

*  **Domain-Driven Design (DDD)**: O projeto adota princípios de **Domain-Driven Design (DDD)**, organizando o código em torno do domínio do problema (Pacientes, Agendamentos, Prontuários e Atendimentos). As **regras de negócio** são tratadas nos serviços e modelos de domínio.

* **Repository Pattern**: os repositórios são responsáveis exclusivamente pela persistência e recuperação de dados, abstraindo a origem (Google Sheets, banco relacional, etc.).

* **DTOs (Data Transfer Objects)**: São classes usadas para transportar dados entre camadas, sem expor as entidades completas do domínio.

* **API REST**: A API segue os princípios de **REST**, expondo recursos bem definidos por meio de endpoints HTTP(CRUD), retornando códigos de status adequados e separando claramente responsabilidades entre Controller, Service e Repositório.

  

### No DocFrontWeb(Frontend)


* **Separation of Concerns (SoC)**: Arquitetura desacoplada onde a lógica de visualização, regras de interface e chamadas de API residem em camadas distintas.

* **Componentização real**: UI construída com componentes independentes, facilitando a manutenção e a consistência visual.

* **State Management**: O estado da aplicação é centralizado em classes de State, responsáveis por manter dados compartilhados entre páginas, cache básico e controle de erros. Isso evita chamadas duplicadas à API, reduz acoplamento entre componentes e garante fluxo de dados previsível, facilitando manutenção e futura migração para Blazor MAUI.

* **Orquestração centralizada**: As regras de fluxo da aplicação (carregamento de dados, atualização de estado, tratamento de erros e controle de cache) são concentradas nas camadas de Services e States. Garantindo o *fluxo de dados unidirecional*: API → Services → States → UI. Determinando assim que a UI apenas reaga às mudanças de estado , reduzindo o acoplamento e evitando chamadas redundantes à API (cache em memória). Essa abordagem reduz duplicação de lógica, melhora a previsibilidade do comportamento da aplicação e facilita manutenção e testes.

* **Mapeamento de Dados (Mapper):** Camada dedicada a transformar DTOs da API em ViewModels específicos para a interface, garantindo que a UI receba apenas o necessário para exibição.

* MockUp: Criei páginas conceitos para ajudar no desenvolvimento.



---
## 4. Funcionalidades desenvolvidas:

### Na DocAPI(Backend):

1.  **Model:** A classe das entidades para a estrutura de dados da aplicação, em `DocAPI/Core/Models`.
2.  **DTOs:** Data Transfer Objects (DTOs) em `DocAPI/Data/Dtos/` para expor os dados de forma segura.
3.  **AutoMapper:** Um `Profile` para mapear a entidade para seus DTOs. `DocAPI/Profiles/`
4.  **Interface do Repositório:** Defina os contratos necessários na interface correspondente em `DocAPI/Core/Repositories`.
5.  **Repositório (Sheets):** Lógica de acesso aos dados na classe de repositório em `DocAPI/Infrastructure/SheetsDb/`. Isso envolve a comunicação com o serviço `GoogleSheetsDB` responsável pela persistências dos dados(Banco de dados provisório).
6.  **Controller:** Comunicação da API com o front pelo Controller em `DocAPI/Controllers` para expor os novos endpoints.
7.  **Testes:** Validação das novas rotas e a lógica utilizando o Postman.

### No DocFrontWeb(Frontend):

1. ViewModels: Estrutura de dados preparada para ser consumida pela UI.
2. DTOS: Estrutura de dados utilizadas na comunicação com o backend(API) e definir estruturas específicas utilizadas pela UI.
3. Services: abstraem a comunicação HTTP com a API..
4. Mapper: camada consumida pela States responsável pela conversão de dados entre a comunicação entre API e UI.
5. States: centralizam o gerenciamento de estado da aplicação, controlando fluxo de dados com cache básico, regras de exibição e erros.
6. UI: **UI (Pages & Components):** responsáveis apenas pela experiência do usuário, mantendo componentes reutilizáveis e desacoplados.


---

## 5. Tecnologias e Bibliotecas

### Gestão:
- **Quadro Kanban:** GitHub Projects.
- **Controle de Versão:** Git/GitHub com o fluxo de *feature-branch* de acordo com o `Readme.md`.

### Na DocAPI(Backend):
*   **.NET 7:** Framework principal da aplicação.
*   **ASP.NET Core:** Para a construção da API REST. @@ -24,12 +31,23 @@.
*   **Entity Framework Core:** ORM para a futura persistência em banco de dados relacional.
*   **AutoMapper:** Mapeamento de objetos (Models para DTOs).
*   **Google Sheets API:** Utilizada para persistência de dados na fase inicial de desenvolvimento.
*   **PdfPig:** Extração de texto e dados de arquivos PDF.
*   **HtmlAgilityPack:** Parsing de documentos HTML para extração de dados (web scraping).
*   **NPOI:** Manipulação de arquivos Excel (XLS).

#### Documentação Externa
* [PDFPig](https://github.com/UglyToad/PdfPig/wiki)




* [TabulaSharp](https://github.com/BobLd/tabula-sharp?tab=readme-ov-file)
* [Closedxml](https://www.nuget.org/packages/closedxml/) 
* [NPOI](https://www.nuget.org/packages/npoi/)
* [htmlagilitypack](https://www.nuget.org/packages/htmlagilitypack/)
* [CSharp](https://learn.microsoft.com/en-us/aspnet/core/blazor/tutorials/movie-database-app/part-1?view=aspnetcore-10.0&pivots=vsc)
  
### No DocFrontWeb(Frontend):
* **Blazor WebAssembly** é utilizado no frontend web.  O projeto já está estruturado visando futura reutilização de componentes e lógica de estado em uma aplicação **Blazor MAUI**, planejada para a versão 2.0 (mobile).

---
## 6. **Como executar**

### Na DocAPI(Backend):

* Para iniciar a API, execute o seguinte comando na raiz do projeto:
  
```bash

dotnet run --project DocAPI/DocAPI.csproj

```

* Executando Scripts Específicos:
- Extração de dados de um PDF

```bash

dotnet run --project DocAPI/DocAPI.csproj --extract

```

  
### No DocFrontWeb(Frontend):

* Como Executar:

```bash

dotnet run --project DocFront.Web/DocFront.Web.csproj

```

  

---
## 7. Workflow para controle de versionamento do projeto

Este projeto utiliza um fluxo de trabalho baseado em *feature branches*, similar ao GitHub Flow.

1.  **Sincronizar sua branch `main`:**

    ```bash

    git checkout main

    git pull origin main

    ```

2.  **Criar uma nova branch para sua tarefa:**

    Use um nome descritivo, como `feature/nome-da-funcionalidade` ou `fix/descricao-do-bug`.

    ```bash

    git checkout -b feature/nome-da-sua-branch

    ```

3.  **Fazer seus commits:**

    Realize commits pequenos e atômicos com mensagens claras.

    ```bash

    git add .

    git commit -m "feat: Adiciona endpoint para criar paciente"

    git push origin feature/nome-da-sua-branch

    ```

4.  **Abrir um Pull Request (PR):**

    No GitHub, crie um Pull Request da sua branch para a `main`. Descreva o que foi feito. Isso serve como um registro da mudança e um ponto de revisão.

5.  **Mescle e limpe:**

    Após aprovar e mesclar o PR, delete a branch para manter o repositório limpo.

    ```bash

    git push origin --delete feature/nome-da-sua-branch

    git branch -d feature/nome-da-sua-branch

    git branch -D feature/nome-da-sua-branch

    ```

  

---
##  8. Status do projeto

### MVP: O projeto encontra-se em uso real em ambiente controlado, validando fluxos, regras de negócio e pontos de fricção.

* A API oferece um conjunto de operações CRUD (Create, Read, Update, Delete) para as principais entidades do sistema:
+ /paciente: Gestão completa dos dados cadastrais dos pacientes.
+  /prontuario: Gerenciamento de prontuários, incluindo a criação a partir de arquivos PDF.
+  /agendamento: Controle e gestão de agendamentos de procedimentos.
+  /atendimento: Orquestração do fluxo de atendimento do paciente, desde a consulta até o pós-operatório.
+ /atendimento/paciente/{id}/report: Geração de um relatório consolidado em PDF para um paciente específico.
  

### Próximos passos
* A gestão do projeto é feita pelo documento [PM](https://github.com/linofir/doc_Organo/blob/main/PM_DocOrgano.md) onde estão os épicos criados para melhor planejamento do desenvolvimento. abaixo está um resumo das features já previstas no backlog da próxima versão:

+ Coleta automática de dados( ex. prontuarios e senhas de agendamento).
+ Gerar relatórios automatizados.
+ Persistência definitiva (SQL + EF)
+ Observabilidade e logs
+ Autenticação e permissões(Identity)
+ Testes automatizados
+ Mobile (Blazor MAUI)
  
### Justificativas de decisões/trade-offs:

- Uso inicial de Google Sheets como banco provisório para acelerar validação com usuários reais.
- Separação rigorosa entre domínio e infraestrutura para permitir migração futura sem impacto nas regras de negócio.
- Centralização de estado no frontend para reduzir acoplamento e facilitar expansão para Blazor MAUI.
  
### Limitações e pontos de atenção:

- Persistência atual não é adequada para alto volume ou concorrência.
- Testes automatizados ainda estão em evolução.
- Autenticação e autorização ainda não implementadas nesta versão.
- Dados utilizados apenas em ambiente controlado. Nenhuma informação sensível pode ser exposta publicamente.


---
## 9. Demonstração
  

* Link para vídeo: [link-ainda em produção](https://drive.google.com/file/d/12hG9Gp4xclllttx4IYVqKuT9FyWr0qDK/view?usp=sharing) 
* Screenshots: Em produção 

---
### 10. Acesso
Este repositório é privado por se tratar de uma solução real em uso.

* Acesso técnico pode ser concedido para recrutadores e avaliadores mediante solicitação.
* Contato para avaliação: É possível acessa-lo como Collaborator ou link privado enviado após contato.
