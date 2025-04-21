# DocAPI

* Está aplicação é um desenvolvimento que busca estruturar a gestão de dados de um consultório médico

# Start 
* dotnet run --project DocAPI/DocAPI.csproj 
* Rotas definidas pelo post

# Diretrizes 
   
    * [ ] Etapas para alterar modelos (ex: Paciente.cs):
        - Alterar ou criar o modelo (Models/Paciente.cs)

        - Ajustar os DTOs e Profiles se necessário

        - Atualizar o DbContext se novas tabelas ou relações forem criadas

        - Criar nova migration:
        ```bash
            dotnet ef migrations add AlteracoesPaciente
        ``` 
        - db context Aplicar ao banco
        ```bash
            dotnet ef database update
        ```
        -Testar rotas API, postman

    * [ ] Etapas para Criar modelos
        ✔ Criar classe no Models/
        ✔ Criar os DTOs em Data/Dtos/
        ✔ Criar Profile do AutoMapper
        ✔ Incluir no DbContext, ou adaptações google sheets DB
        ✔ Criar migration e aplicar
        ✔ Criar controller REST básico
   
    * Criar DB no google sheets(Etapa de desenvolvimento)
        * Criar uma camada de serviço/repositório que lê/escreve no Google Sheets.
        * Usar o mesmo formato dos seus DTOs para montar essas operações.
        * Depois, trocar essa camada por uma baseada no EF Core com mínimo esforço.
        

* Criar projeto do front
    * Básico
        ```bash
            dotnet new blazorserver -n DocFront
        ```
    * Acrescentar no snl
        ```bash
            dotnet sln doc_Organo.sln add DocFront/DocFront.csproj
        ```

# ToDo
* Adaptar e criar novo DB 
    * Novos modelos.
        - Paciente OK
        - Endereco
        - Prontuário
    * Dtos
        - Paciente Ok
        - Endereco
    * Profile
        - Paciente Ok
    * DB google sheets
        - Configurar as APis na cloud OK
        - Criar novo serviço.
        - criar teste de primeiro acesso
    * DBContext
        - Adicionar endereço
* Criar projeto do front
    * Básico
    * Acrescentar no snl
* Autenticação
    * Identity


## ToDO old
* Criar Um CRUD. ok
* Estruturar um DB usando o framework Entity. ok
* Estruturar o Mapper. ok
* Implemantar novas entidades: Consulta, consultório. ok
* Definir suas relações, config do mapper. ok
* configurar laziness. ok
* Implementar relação Paciente com Consulta. ok
* Implementar a exibição das listas de consultas por pacientes e por consultório. pendente/estudar melhor forma
* Implementar relação Paciente - Consultório.
* Aplicar regras de deleção. para todas entidades.
* Consultas Específicas, LINQ.

*