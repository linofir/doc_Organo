# DocFront.Web
## 🚀 Como Executar
```bash
dotnet run --project DocFront.Web/DocFront.Web.csproj
```
## 💻 Fluxo de Desenvolvimento (Como Contribuir)
0. **Backend** 
- Controller REST (GET, POST, PUT, DELETE)
- Retorno padronizado: Result<T> { Success, Data, Error }
- Validação centralizada (DataAnnotations ou FluentValidation)
- Regra de negócio não no controller
- 📌 Justificativa: garante consistência e previsibilidade para o frontend.
1.  **Model:** Crie ou altere a classe da entidade em `DocAPI/Core/Models`.
2.  **Services:** Crie ou ajuste os Data Transfer Objects (DTOs) em `DocAPI/Data/Dtos/` para expor os dados de forma segura.
- Responsabilidades:
    - Comunicação HTTP
        - Serialização / desserialização
        - Nenhuma regra de UI
        - Nenhuma dependência de componentes
- 📌 Qualidade: serviços testáveis e reutilizáveis.
3.  **UI:** Páginas
- Responsabilidades:
    - Buscar dados iniciais (OnInitializedAsync)
    - Manter estado da entidade
    - Executar ações críticas: Salvar, Deletar, Recarregar dados, Centralizar mensagens de sucesso/erro
- 📌 Padrão adotado: Componentes nunca chamam serviços diretamente.
4.  **Wrapper:** 

[API Controller]
       ↓
[Service (Backend)]
       ↓
[DTO / Model]
       ↓
──────── HTTP ────────
       ↓
[Service (Frontend)]
       ↓
[Página Pai (Detalhes)]
       ↓
[Componentes Filhos (Abas)]


## 🔐 Conceitos Técnicos Utilizados
### ✔️ Arquitetura
- Separation of Concerns (SoC)
- Componentização real
- Orquestração centralizada
- Fluxo unidirecional de dados

| Camada                  | Responsabilidade                           |
| ----------------------- | ------------------------------------------ |
| **API (Backend)**       | Regra de negócio, validações, persistência |
| **Services (Frontend)** | Comunicação HTTP, abstração da API         |
| **Pages (Frontend)**    | Orquestração de estado e navegação         |
| **Components**          | UI reutilizável e isolada                  |
| **Models / DTOs**       | Contrato explícito entre camadas           |


### ✔️ Blazor
- EditForm com validação
- EventCallback<T>
- OnParametersSet
- Scoped CSS
- Componentes modais reutilizáveis

### ✔️ UX / UI
- Edição controlada (modo edição)
- Confirmação explícita para ações destrutivas
- Feedback visual centralizado
- Estados previsíveis

### 🧠 Qualidades do Projeto (Visão de Portfólio)
Este projeto demonstra:
- ✔ Capacidade de desenhar arquitetura escalável
- ✔ Entendimento real de fluxo de dados em SPAs
- ✔ Separação clara entre UI, estado e regras
- ✔ Código organizado, legível e extensível
- ✔ Pensamento orientado a produto e manutenção
- ✔ O projeto adota separação clara entre Domain Models, DTOs e ViewModels, evitando o acoplamento entre frontend e backend. A integração é feita por serviços wrapper e DTOs orientados a contexto, com enums compartilhados e resolução de display via metadata

-------

# DocAPI
Esta aplicação é uma API de back-end que tem como objetivo centralizar e otimizar a gestão de dados de um atendimento médico, automatizando processos e unificando informações de pacientes, prontuários e agendamentos. 
Poderá ser associada ao DocFront, front-end da API.
## 🚀 Como Executar
### Executando a Aplicação
Para iniciar a API, execute o seguinte comando na raiz do projeto:
```bash
dotnet run --project DocAPI/DocAPI.csproj
```
### Executando Scripts Específicos
Para executar scripts customizados, como o de extração de dados de um PDF:
```bash
dotnet run --project DocAPI/DocAPI.csproj --extract
```
## Pré-requisitos
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) ou superior.
## 🛠️ Tecnologias e Bibliotecas
*   **.NET 8:** Framework principal da aplicação.
*   **ASP.NET Core:** Para a construção da API REST. @@ -24,12 +31,23 @@.
*   **Entity Framework Core:** ORM para a futura persistência em banco de dados relacional.
*   **AutoMapper:** Mapeamento de objetos (Models para DTOs).
*   **Google Sheets API:** Utilizada para persistência de dados na fase inicial de desenvolvimento.
*   **PdfPig:** Extração de texto e dados de arquivos PDF.
*   **HtmlAgilityPack:** Parsing de documentos HTML para extração de dados (web scraping).
*   **NPOI:** Manipulação de arquivos Excel (XLS).

## Documentação Externa
* [PDFPig](https://github.com/UglyToad/PdfPig/wiki)
* [TabulaSharp](https://github.com/BobLd/tabula-sharp?tab=readme-ov-file)
* [Closedxml](https://www.nuget.org/packages/closedxml/) Provavelmente não será usada
* [NPOI](https://www.nuget.org/packages/npoi/)
* [htmlagilitypack](https://www.nuget.org/packages/htmlagilitypack/)
* [CSharp](https://learn.microsoft.com/en-us/aspnet/core/blazor/tutorials/movie-database-app/part-1?view=aspnetcore-10.0&pivots=vsc)
## 💻 Fluxo de Desenvolvimento (Como Contribuir)
Este projeto utiliza um fluxo de trabalho baseado em *feature branches*, similar ao GitHub Flow.
1.  **Sincronize sua branch `main`:**
    ```bash
    git checkout main
    git pull origin main
    ```
2.  **Crie uma nova branch para sua tarefa:**
    Use um nome descritivo, como `feature/nome-da-funcionalidade` ou `fix/descricao-do-bug`.
    ```bash
    git checkout -b feature/nome-da-sua-branch
    ```
3.  **Faça seus commits:**
    Realize commits pequenos e atômicos com mensagens claras.
    ```bash
    git add .
    git commit -m "feat: Adiciona endpoint para criar paciente"
    git push origin feature/nome-da-sua-branch
    ```
4.  **Abra um Pull Request (PR):**
    No GitHub, crie um Pull Request da sua branch para a `main`. Descreva o que foi feito. Isso serve como um registro da mudança e um ponto de revisão.
5.  **Mescle e limpe:**
    Após aprovar e mesclar o PR, delete a branch para manter o repositório limpo.
    ```bash
    git branch -d feature/nome-da-sua-branch
    git push origin --delete feature/nome-da-sua-branch
    ```

## 🏗️ Conceitos, Arquitetura e Diretrizes de Desenvolvimento

Esta seção serve como um guia rápido para o desenvolvedor, descrevendo os fluxos de trabalho comuns para adicionar ou modificar funcionalidades na API.

A arquitetura segue um padrão semelhante ao **Model-View-Controller (MVC)**, onde:
- **Model:** As entidades de negócio em `DocAPI/Core/Models`.
- **Controller:** Os endpoints da API em `DocAPI/Controllers`, que orquestram as requisições.
- **View:** O frontend (separado), `DocFront`, que consumirá esta API.

### Workflow 1: Adicionando/Modificando uma Entidade (com Google Sheets - *Atual*)

Este é o fluxo de trabalho para a fase de desenvolvimento atual, utilizando o Google Sheets como banco de dados.

1.  **Model:** Crie ou altere a classe da entidade em `DocAPI/Core/Models`.
2.  **DTOs:** Crie ou ajuste os Data Transfer Objects (DTOs) em `DocAPI/Data/Dtos/` para expor os dados de forma segura.
3.  **AutoMapper:** Atualize ou crie um `Profile` para mapear a entidade para seus DTOs.
4.  **Interface do Repositório:** Defina os contratos necessários na interface correspondente em `DocAPI/Core/Repositories`.
5.  **Repositório (Sheets):** Implemente a lógica de acesso aos dados na classe de repositório em `DocAPI/Infrastructure/SheetsDb/`. Isso envolve a comunicação com o serviço `GoogleSheetsDB`.
6.  **Controller:** Crie ou atualize o Controller em `DocAPI/Controllers` para expor os novos endpoints.
7.  **Testes:** Valide as novas rotas e a lógica utilizando o Postman.

### Workflow 2: Adicionando/Modificando uma Entidade (com EF Core - *Futuro*)

Este é o fluxo de trabalho alvo, para quando a migração para um banco de dados relacional estiver completa.

1.  **Model:** Crie ou altere a classe da entidade em `DocAPI/Core/Models`.
2.  **DTOs & AutoMapper:** Ajuste os DTOs e os `Profiles` do AutoMapper.
3.  **DbContext:** Atualize o `DbContext` com o novo `DbSet` ou as novas relações.
4.  **Criar Migration:** Gere uma nova migração para refletir as mudanças no banco de dados.
    ```bash
    dotnet ef migrations add NomeDaAlteracaoNaEntidade
    ```
5.  **Aplicar Migration:** Aplique a migração ao banco de dados.
    ```bash
    dotnet ef database update
    ```
6.  **Repositório (EF Core):** Implemente a lógica de acesso aos dados no repositório correspondente que utiliza o `DbContext`.
7.  **Controller:** Crie ou atualize o Controller.
8.  **Testes:** Valide as rotas via Postman.

## Funcionalidades
+## ✨ Funcionalidades e Endpoints Principais + +A API oferece um conjunto de operações CRUD (Create, Read, Update, Delete) para as principais entidades do sistema: + +- /paciente: Gestão completa dos dados cadastrais dos pacientes. +- /prontuario: Gerenciamento de prontuários, incluindo a criação a partir de arquivos PDF. +- /agendamento: Controle de agendamentos de procedimentos. +- /atendimento: Orquestração do fluxo de atendimento do paciente, desde a consulta até o pós-operatório. +- /atendimento/paciente/{id}/report: Geração de um relatório consolidado em PDF para um paciente específico. +