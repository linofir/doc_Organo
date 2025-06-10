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
using System.Net.Http;


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
builder.Services.AddSingleton<PdfGeneratorService>();
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

var pathToXlsFile = @"C:\Users\lino\Downloads\transferir (3).xls";
var service = new FileDataExtractorService(pathToXlsFile);
// string novoCaminho = @"C:\Users\lino\Dropbox\io\Doc_Organo\Descricao.xlsx";
var descritivo = await service.ExtractDataFromFileAsync(pathToXlsFile);
Console.WriteLine($"período do descritivo: {descritivo}");
var descritivoJson = await service.PrintDadosExtraidosComoJson(new List<DescritivoFinanceiro>(){descritivo});
//Console.WriteLine(result);
if (descritivo != null)
{
    service.SaveDescritivo(descritivo);
}

string jsonOutputPath = @"C:\Users\lino\Projetos_Programação\doc_Organo\DocAPI\Secrets\DescricaoFinanceiraTeste.json";
await File.WriteAllTextAsync(jsonOutputPath, descritivoJson);
Console.WriteLine($"\nDados extraídos com sucesso e salvos em: {jsonOutputPath}");


// Dictionary<>
            

// var httpClientTest = new HttpClient();
// // Opcional: configurar um timeout para o HttpClient
// httpClientTest.Timeout = TimeSpan.FromSeconds(30); // 30 segundos de timeout

// var service = new HtmlDataExtractorService(httpClientTest); // Use o nome da sua classe de serviço

// var urlTest = "https://cooperado.unimedsorocaba.coop.br/UnimedMVC/Demonstrativo/AnaliticoVisualiza?crm=151200&sequencia=508487&periodo=202505&nrPagamento=508487&imprimir=True";

// try
// {
//     Console.WriteLine($"Tentando extrair dados da URL: {urlTest}");
//     List<ListaDados> dadosExtraidos = await service.ExtractDataFromUrlAsync(urlTest);

//     Console.WriteLine("Dados extraídos com sucesso. Imprimindo como JSON...");
//     // Crie uma instância da sua classe que contém o método PrintDadosExtraidosComoJson
//     // Você pode colocar esse método dentro do próprio ThirdPartyDataExtractorService ou em uma classe utilitária.
//     // Assumindo que você o colocou em uma classe utilitária por enquanto, ou no próprio serviço:
//     var jsonOutput = service.PrintDadosExtraidosComoJson(dadosExtraidos); // Assumindo que está no serviço
//     Console.WriteLine(jsonOutput);

//     Console.WriteLine($"Total de Tipos de Guia encontrados: {dadosExtraidos.Count}");
//     foreach (var lista in dadosExtraidos)
//     {
//         Console.WriteLine($"  Tipo de Guia: {lista.TipoGuia ?? "N/A"}, Total de Dados Financeiros: {lista.Dados?.Count ?? 0}");
//     }
// }
// catch (HttpRequestException ex)
// {
//     Console.Error.WriteLine($"Erro HTTP ao acessar a URL: {ex.Message}");
//     Console.Error.WriteLine($"Status Code: {ex.StatusCode}");
// }
// catch (Exception ex)
// {
//     Console.Error.WriteLine($"Erro inesperado durante a extração: {ex.Message}");
//     Console.Error.WriteLine($"StackTrace: {ex.StackTrace}");
// }
// finally
// {
//     // É uma boa prática descartar HttpClient quando criado manualmente e não gerenciado por AddHttpClient()
//     httpClientTest.Dispose();
// }

// var service = new HtmlDataExtractorService( httpClientTest);
// var urlTest = "https://cooperado.unimedsorocaba.coop.br/UnimedMVC/Demonstrativo/AnaliticoVisualiza?crm=151200&sequencia=508487&periodo=202505&nrPagamento=508487&imprimir=True";
// ListaDados listaDados = new ListaDados();
// List<ListaDados> lista = new List<ListaDados>();
// lista = await service.ExtractDataFromUrlAsync(urlTest);
// service.PrintDadosExtraidosComoJson(lista);

app.Run();

