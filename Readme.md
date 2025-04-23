# DocAPI

* Está aplicação é um desenvolvimento que busca estruturar a gestão de dados de um consultório médico

# Extrernal Docs
* [PDFPig](https://github.com/UglyToad/PdfPig/wiki)
* [TabulaSharp](https://github.com/BobLd/tabula-sharp?tab=readme-ov-file)

# Start 
* dotnet run --project DocAPI/DocAPI.csproj 
* dotnet run --extract
* Rotas definidas pelo post

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
        ✔ Incluir no DbContext, ou adaptações google sheets DB( etapa dev)
        ✔ Criar migration e aplicar
        ✔ Criar controller REST básico
   
    
    * GitHub, Doc_Organo
        ✔ Atualize sua main com o remoto:git checkout main ; git pull origin main
        ✔ Crie e mude para uma nova branch: git checkout -b nome-da-sua-branch
        ✔ Faça suas alterações e commits normalmente: git add . ; git commit -m "nome-feature" ; git push origin nome-da-sua-branch
        ✔ (Opcional) Crie um Pull Request no GitHub: Se estiver colaborando com outras pessoas ou quiser deixar documentado, abra um Pull Request no site do GitHub. Isso te dá chance de revisar e aprovar antes de mesclar.
        ✔ Mescle para main: Quando tiver certeza que está tudo funcionando e testado, você pode fazer o merge: git checkout main ; git pull origin main; git merge nome-da-sua-branch; git push origin main; 
        ✔ Deletar branch antiga: git branch -d nome-da-sua-branch ; git push origin --delete nome-da-sua-branch
 
## Services
* DB no google sheets(Etapa de desenvolvimento)
    * Criar diretriz.
* PdfExtract ok
# ToDo new-model-Agendamento-Dtos-AM
* Adaptar e criar novo DB
    * Novos modelos.
        - Paciente OK
        - Endereco Ok
        - Prontuário
            - criação dos props
            - Lista de Exames, criação do service de extração, crição do cli ok
            - Criar base de dados procedimentos e CID
        - Agendamento Cirurgico
        - FollowUP
    * Dtos
        - Paciente Ok
        - Endereco 
        - Prontuario
        - Agendamento
        - FollowUp

    * Profile
        - Paciente Ok
    * DB google sheets
        - Configurar as APis na cloud OK
        - Criar novo serviço, criar teste de primeiro acesso
        - Usar o mesmo formato dos DTOs para montar essas operações. Depois trocar essa camada por uma baseada no EF Core com mínimo esforço.
        - 
    * DBContext
        - Adicionar endereço
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