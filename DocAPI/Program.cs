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

var builder = WebApplication.CreateBuilder(args);

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

// var paciente = new Paciente
// {
//     CPF = "98765432101",
//     Nome = "Maria Teste",
//     Nascimento = new DateTime(1991, 6, 11),
//     Plano = "Plano prata",
//     Carteira = "CBA123456",
//     Email = "maria.teste@example.com",
//     Telefone = "(12) 98765-4321",
//     Endereco = new Endereco
//     {
//         Logradouro = "Rua das Árvores",
//         Numero = "321",
//             Bairro = "Jardim das Palmeiras",
//             Cidade = "São Paulo",
//         UF = "SP",
//         CEP = "93210-567"
//     },
//     RG = "135798462"
// };
// var pathPDF = @"C:\Users\lino\Dropbox\io\Doc_Organo\prontuariov4.pdf";
// var serviceExtractor = new ProntuarioPdfExtractorService(paciente);
// var prontuario = await serviceExtractor.ExtrairProntuarioDePdfAsync(pathPDF);

// var json = JsonSerializer.Serialize(
// prontuario,
// new JsonSerializerOptions
// {
//     WriteIndented = true,
//     Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
// }
// );  

// Console.WriteLine("🔎 Prontuário extraído:");
// Console.WriteLine(json);

// var paciente = new Paciente
// {
//     CPF = "98765432101",
//     Nome = "Maria Teste",
//     Nascimento = new DateTime(1991, 6, 11),
//     Plano = "Plano prata",
//     Carteira = "CBA123456",
//     Email = "maria.teste@example.com",
//     Telefone = "(12) 98765-4321",
//     Endereco = new Endereco
//     {
//         Logradouro = "Rua das Árvores",
//         Numero = "321",
//             Bairro = "Jardim das Palmeiras",
//             Cidade = "São Paulo",
//         UF = "SP",
//         CEP = "93210-567"
//     },
//     RG = "135798462"
// };
// var pathPDF = @"C:\Users\lino\Dropbox\io\Doc_Organo\teste_prontuario.pdf";
// var serviceExtractor = new ProntuarioPdfExtractorService2(paciente);
// var prontuario = await serviceExtractor.ExtrairProntuarioDePdfAsync(pathPDF);

app.Run();

