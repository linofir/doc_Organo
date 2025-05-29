using DocAPI.Data;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using DocAPI.Profiles;
using DocAPI.Services;
using DocAPI.Infrastructure.Sheets;
using DocAPI.Core.Repositories;
using DocAPI.CLI;
using Newtonsoft.Json.Converters;
using DocAPI.Core.Models;
using UglyToad.PdfPig.Graphics.Colors;
using System.Text.Json;
using QuestPDF.Infrastructure;


var builder = WebApplication.CreateBuilder(args);

QuestPDF.Settings.License = LicenseType.Community;
//CLI
if (args.Contains("--extract"))
{
    ExtractExamesCli.Run(args);
    return;
}

// Add services to the container.
//var connectionString = builder.Configuration.GetConnectionString("PacienteConnection");

//builder.Services.AddDbContext<PacienteContext>(opts => opts.UseLazyLoadingProxies().UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

var config = new MapperConfiguration(cfg =>
{
    cfg.AddProfile<PacienteProfile>();
    cfg.AddProfile<ProntuarioProfile>();
    cfg.AddMaps(typeof(Program).Assembly);
});

IMapper mapper = config.CreateMapper();
//builder.Services.AddAutoMapper(typeof(Program).Assembly); outra opção


builder.Services.AddSingleton<IMapper>(mapper);

builder.Services.AddSingleton<GoogleSheetsDB>();
builder.Services.AddScoped<IPacienteRepository, PacienteSheetsRepository>();
builder.Services.AddScoped<IProntuarioRepository, ProntuarioSheetsRepository>();

builder.Services.AddControllers().AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.Converters.Add(new StringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

var paciente = new Paciente
{
    ID = "ab39fc72-9853-4801-b799-361030d2457a",
    CPF = "98765432101",
    Nome = "Maria Teste",
    Nascimento = new DateTime(1991, 6, 11),
    Plano = "Plano prata",
    Carteira = "CBA123456",
    Email = "maria.teste@example.com",
    Telefone = "(12) 98765-4321",
    Endereco = new Endereco
    {
        Logradouro = "Rua das Árvores",
        Numero = "321",
            Bairro = "Jardim das Palmeiras",
            Cidade = "São Paulo",
        UF = "SP",
        CEP = "93210-567"
    },
    RG = "135798462"
};

var prontuario = new Prontuario
        {
            // O ID do prontuário pode ser gerado aqui ou pelo DB
            ID = Guid.NewGuid().ToString(), // Exemplo de geração de ID

            DescricaoBasica = new DescricaoBasica
            {
                NomePaciente = "Maria Teste",
                Cpf = "98765432101",
                Idade = 37,
                Profissao = "Designer",
                Religiao = "Evangélica",
                QD = "Miomas uterinos",
                AtividadeFisica = "Leve",
                PacienteId = "ab39fc72-9853-4801-b799-361030d2457a" // Atribua o mesmo pacienteId aqui
            },
            AGO = new AGO
            {
                Menarca = "11",
                DUM = "Dumteste", // Convertendo string para DateTime
                Paridade = "AA",
                DesejoGestacao = "Não",
                Intercorrencias = "Miomas recorrentes",
                Amamentacao = "Não",
                VidaSexual = "Ativa",
                Relacionamento = "Casada",
                Parceiros = "1",
                Coitarca = "18",
                IST = "Não",
                VacinaHPV = StatusVacinaHPV.SemVacina,
                CCO = "CCOTESTE", // Convertendo string para DateTime
                MAC_TRH = "Nenhum"
            },
            Antecedentes = new Antecedentes
            {
                Comorbidades = "Anemia",
                Medicacao = "Sulfato ferroso",
                Neoplasias = "Não",
                Cirurgias = "Miomectomia",
                Alergias = "Dipirona",
                Vicios = "Não",
                HabitoIntestinal = "Irregular",
                Vacinas = "Atualizadas"
            },
            AntecedentesFamiliares = new AntecedentesFamiliares
            {
                Neoplasias = "Nenhuma",
                Comorbidades = "Mãe hipertensa"
            },
            // Note que "cd" no seu JSON era uma lista de strings, aqui "Cd" (PascalCase)
            CD = new List<AcoesCD> { AcoesCD.PedidoInternacao, AcoesCD.PastaInformativa },
            InformacoesExtras = "Agendar cirurgia de retirada de mioma.",
            Exames = new List<Exame>
            {
                new Exame { Codigo = "EX015", Nome = "Ultrassom abdominal" }
            },
            DataRequisicao = DateTime.Parse("2025-05-10"), // Convertendo string para DateTime
            
            SolicitacaoInternacao = new Internacao
            {
                Procedimentos = new List<string> { "Miomectomia" },
                IndicacaoClinica = "Miomas grandes e sintomáticos.",
                Observacao = "Paciente deseja preservação do útero.",
                CID = "D25",
                TempoDoenca = "1 ano",
                Diarias = "3",
                Tipo = "Cirurgia",
                Regime = "Eletiva",
                Carater = "Definitivo",
                UsaOPME = false,
                Local = "Hospital São Lucas",
                Guia = long.Parse("12345")
            }
        };
var service =  new PdfGeneratorService();
var pdrTest = service.GeneratePatientReportPdf(paciente, new List<Prontuario>{prontuario});

string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
string outputPath = Path.Combine(desktopPath, $"RelatorioPaciente_{paciente.ID}.pdf");

using (var fileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
{
    pdrTest.CopyTo(fileStream); // Copia o conteúdo do MemoryStream para o FileStream
}

Console.WriteLine($"PDF gerado e salvo em: {outputPath}");
app.Run();

