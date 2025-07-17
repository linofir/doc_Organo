# DocAPI

* Está aplicação é um desenvolvimento que busca estruturar a gestão de dados de um consultório médico

# Extrernal Docs
* [PDFPig](https://github.com/UglyToad/PdfPig/wiki)
* [TabulaSharp](https://github.com/BobLd/tabula-sharp?tab=readme-ov-file)
* [Closedxml](https://www.nuget.org/packages/closedxml/) Provavelmente não será usada
* [NPOI](https://www.nuget.org/packages/npoi/)
* [htmlagilitypack](https://www.nuget.org/packages/htmlagilitypack/)

# Comands 
* dotnet run --project DocAPI/DocAPI.csproj 
* Run script that extract data from PDF.
    dotnet run --extract

# Diretrizes 
    * Etapas para alterar modelos (ex: Paciente.cs):
        - Alterar ou criar o modelo (Models/Paciente.cs)

        - Ajustar os DTOs e Profiles se necessário

        - Atualizar o DbContext se novas tabelas ou relações forem criadas

        - Criar nova migration:
        ```bash
            dotnet ef migrations add AlteracoesPaciente
        ``` 
        - db context Aplicar ao banco, Aplicar no sheetsDB(etapaDEV)
        ```bash
            dotnet ef database update
        ```
        -Testar rotas API, postman

    * Etapas para Criar modelos
        ✔ Criar classe no Models/
        ✔ Criar os DTOs em Data/Dtos/
        ✔ Criar Profile do AutoMapper
        ✔ Criar interfaces
        ✔ Incluir no DbContext, ou adaptações google sheets DB( etapa dev)
        ✔ Criar migration e aplicar
        ✔ Criar controller REST básico

    * Criar Controllers
        ✔ Criar diretriz    
   
    
    * GitHub, Doc_Organo
        ✔ Atualize sua main com o remoto:git checkout main ; git pull origin main
        ✔ Crie e mude para uma nova branch: git checkout -b nome-da-sua-branch
        ✔ Faça suas alterações e commits normalmente: git add . ; git commit -m "nome-feature" ; git push origin nome-da-sua-branch
        ✔ (Opcional) Crie um Pull Request no GitHub: Se estiver colaborando com outras pessoas ou quiser deixar documentado, abra um Pull Request no site do GitHub. Isso te dá chance de revisar e aprovar antes de mesclar.
        ✔ Mescle para main: Quando tiver certeza que está tudo funcionando e testado, você pode fazer o merge: git checkout main ; git pull origin main; git merge nome-da-sua-branch; git push origin main; 
        ✔ Deletar branch antiga: git branch -d nome-da-sua-branch ; git push origin --delete nome-da-sua-branch
 feature-createRelatorioPdf
## Services
* DB no google sheets(Etapa de desenvolvimento)
    ✔ Criar Task necessária na Interface 
    ✔ Criar a comunicação com a interface e a função no PacienteSheetsRepository
    ✔ Criar funções necessárias na GoogleSheetsDB, repositorio
    ✔ Criar Controller

* PdfExtract ok

# ToDo  Product Log
* Atividades auxiliares
    * Update de estruturas
        - Criação de Core OK
        - Criação de infrastructure ok

* Adaptar e criar novo DB
    * Novos modelos.
        - Paciente OK
        - Endereco Ok
        - Prontuário
            - Aç~es Cd Pedidos Cirurgicos, Termo cirurgico, Explicação sobre o procedimento. Paciente acompanhada,
        - Agendamento Cirurgico OK
        - FollowUP
    * Dtos
        - Paciente Ok
        - Endereco ok
        - Prontuario ok
        - Agendamento OK
        - FollowUp

    * Profile
        - Paciente Ok
        - Prontuario Ok
        - Agendamento Ok
        - FollowUp 
    * Definir interfaces e Repositórios
        - Paciente ok
        - Prontuario ok
        - Agendamento
            - Método unificado de GET que lide com tipos diferentes. Incompleto.
            - POST, Adicionar na ultima linha da planilha,append Incompleto
            - POST, Criar o ID Problema da geração do ID fora do contrutor. Incompleto
            - PUT, Os parametros podem ser associados. Incompleto
            - Criar método para chechagem de Senha e Status. pelo email ou arquivo
        - FollowUp
    * DB google sheets
        - Configurar as APis na cloud OK
        - Criar novo serviço, criar teste de primeiro acesso ok
        - Criar Requisições Paciente ok
        - Refatorar com métodos auxiliares, contrução do body das planilhas
    * Criar Controllers
        - Paciente, definir endpoints básicos ok
        - Prontuario, definir endpoints básicos  ok
            - Requisições from form.
        - Agendamentos OK
            - Criar controller de filter  geral. Incompleto
        - FollowUp
    * DBContext
        - Criar do zero 
* Services
    * ProntuarioPdfExtractosService
        - identificar páginas automaticamente.
        - adaptar método para lidar com filesteam, ao invés de path.
        - Testar com diferentes prontuários e adaptar padrões.
        - Adicionar logs e tratamento de exceções.
        - Implementar extração de data
    * PdfGeneratorService
        - Identificar os Logs atuantes. 
        - Criar cenários, logs de erros
        - Desenvolver validações.
    * feature-CollectDemonstrativoDataService 
        - Criar serviço que extrai os dados
            - Classe de extração usando scrapping Incompleto
                - Método de extração da url. Incompleto
                - Fazer uma requisição GET inicial para a página de login:
                - Fazer uma requisição POST para a URL de processamento de login
                - Gerenciar Cookies:
                - Automatizar, periodicidade
        - Logica para validação financeira
    * Serviço para coleta de Senha, prop de agenda
        
* Refatorando Geral.
    - Debug de null reference
    - Refatorar método de Get específicos, iguais ao agendamento.
    - Alinhar os Enuns aos valores das tabelas do Sheets.
    - Regras dos sheets defasadas, validaçoes, funções etc.
    - Definir melhor os padrões de tipos para CPF, Mudar formato da planilha para não haver conflito
    - Documentaçao Geral.
    * endpoints e serviços ja identificados
            - Criar testes especificos para cada endpoint
            - Corrigir erros no POST, não aceita null em solicitacao de internacao, testar exames.
            - Caminhos possíveis de PUT
            - Testes
                - Construir uma camada de logger. 
                - Criar cenários de testes, logs de erros, menssagens de excessão 
                - Revisar validaçoes .

* create-Prontuario-endpoints
    - Adaptar GoogleSheetsDB pra todas entidades, ou criar novos métodos se possível.
    - Criar todos métodos do repositório.     
    - Alterações no model, 
        - novas porpriedades em internacao, descricao basica
    - Alterações nos Dtos de Prontuario.
    - Alterações no Profile do mapper, prontuario
    - Criar endpoints 
        - GET all e ID ok, POST ok, PUT ok, DELETE  ok
* Testes 
    * Organização
        - Estrutura,  Pastas (Testes unitários (Unit), Testes de integração(integration), Testes end-to-end (E2E))
        -Fluxo 
        ```mermaid
            graph TD
            A[Escreve Endpoint / Serviço] --> B[Testes Automatizados]
            B --> C[Logs Detalhados]
            C --> D[Monitoramento de Uso]
            D --> E[Feedback do Usuário]
            E --> F[Priorização de Melhorias]
            F --> A
        ```
    *  Implementar ferramentas  
        * Testes Automatizados	
            -xUnit + mocks, Validar serviços locais e integração com o Sheets
        * log
            - ILogger, Console.WriteLine, arquivos .log, Logue tempo de execução, falhas e IDs manipulados
        * Monitoramento
            - Middleware personalizado ou AppInsights, Log de requisições (tempo, erros, etc.)
        * Feedback 
            - Notas internas, formulário no frontend, Receba ideias de usuários sobre uso real
        * Analise de erros
            -Logs e relatórios, Reproduza o erro com dados de logs e crie testes para isso
        * Adicionar sistema de métricas (Prometheus + Grafana ou AppInsights)
        * Criar testes de carga com k6 ou JMeter
        * Testes E2E com Blazor usando Playwright

    


* Próximo  feature-followUp atual
    - Criar model Atendimento que irá se relacionar com paciente 
        - Adaptar Paciente model ok
        - model Atendimento k
    - Criar Dtos Atendimento 
        - Read ok
        * Criar Create, Update.
    - Criar Atendimento e adaptar métodos repositório e interfaces
        - Refatorar Paciente.
            - Método para busca GET de forma direta. Busca por row, Analisar se é mais eficiente mesmo.
        - Refatorar Agendamento.
            - campos novos para gestão do atendimento
        - Refatorar Prontuarios.
            - Possibilitar outros formatos para prontuarios.
            - Refatorar metodos do repositórios levando em consideração as buscas específicas
        - Criar repo de atendimento
            - Transpor funcionalidade de Criar um relatorio geral para o novo repositório Atendimento ao invés de Prontuario. OK
            - Atendimento primeira etapa.
                - métodos para validação de primeira etapa OK
                - refatorar pendsando em prontuarios de outros tipo
            - Atendimento segunda etapa. Criar teste
            - Atendimento terceira etapa
            - Aprimorar validação de cada etapa, forma independente?
    - Adaptar/Criar serviço 
        - de geração de pdf, acrescendtar Agendamento. OK
        - Coleta de dados das tabelas de senhas
            - Save, load. Testar
    - Adaptar program.cs OK
    - Criar endpoint 
        - Criar controller Atendimento 
            - GET report by paciente id OK
    - Testes
        - Identificar os Logs atuantes. 
        - Criar cenários, logs de erros
        - Desenvolver validações.
    


///////
* Próximo RefatorandoGeral. prox dev
    - Criar model
    - Criar Dtos
    - Criar método repositório e interfaces
    - Adaptar program.cs 
    - Criar endpoint 
        - Criar controller OK
        - Adaptar program.cs
    - Testes
        - Identificar os Logs atuantes. 
        - Criar cenários, logs de erros
        - Desenvolver validações.
   
    

* Criar projeto do frontll
    * Básico
        ```bash
            dotnet new blazorserver -n DocFront
        ```
    * Acrescentar no snl
        ```bash
            dotnet sln doc_Organo.sln add DocFront/DocFront.csproj
        ```
* Autenticação
    * Identity


### Hint 

