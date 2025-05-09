# DocAPI

* Está aplicação é um desenvolvimento que busca estruturar a gestão de dados de um consultório médico

# Extrernal Docs
* [PDFPig](https://github.com/UglyToad/PdfPig/wiki)
* [TabulaSharp](https://github.com/BobLd/tabula-sharp?tab=readme-ov-file)

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
 
## Services
* DB no google sheets(Etapa de desenvolvimento)
    ✔ Criar Task necessária na Interface 
    ✔ Criar a comunicação com a interface e a função no PacienteSheetsRepository
    ✔ Criar funções necessárias na GoogleSheetsDB, repositorio
    ✔ Criar Controller

* PdfExtract ok

# ToDo  
* Atividades auxiliares
    * Update de estruturas
        - Criação de Core OK
        - Criação de infrastructure ok

* Adaptar e criar novo DB
    * Novos modelos.
        - Paciente OK
        - Endereco Ok
        - Prontuário
            - criação dos props
            - Lista de Exames, criação do service de extração, crição do cli ok
            - Criar base de dados procedimentos e CID
        - Agendamento Cirurgico ok
        - FollowUP
    * Dtos
        - Paciente Ok
        - Endereco ok
        - Prontuario ok
        - Agendamento ok
        - FollowUp

    * Profile
        - Paciente Ok
        - Prontuario Ok
        - Agendamento Ok
    * Definir interfaces
        - Paciente ok
        - Prontuario ok
        - Agendamento
    * DB google sheets
        - Configurar as APis na cloud OK
        - Criar novo serviço, criar teste de primeiro acesso ok
        - Criar Requisições Paciente ok
    * Criar Controllers
        - Paciente, definir endpoints básicos ok
        - Prontuario, definir endpoints básicos 
    * DBContext
        - Criar do zero
    * Pull atual create-Prontuario-endpoints
        - Adaptar GoogleSheetsDB pra todas entidades, ou criar novos métodos se possível.
        - Criar todos métodos do repositório.
        - Criar endpoints 
* Criar projeto do front
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
* Dica para sereialização para o front usando enum:
```builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
```
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