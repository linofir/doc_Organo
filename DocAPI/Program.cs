using DocAPI.Data;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using DocAPI.Profiles;
using DocAPI.Services;
// using DocAPI.Services.FileDataOfSenhaExtractorService;
using DocAPI.Infrastructure.SheetsDb;
using DocAPI.Core.Repositories;

using DocAPI.Infrastructure.Sql;
// using DocAPI.Core.Models;
using DocAPI.CLI;
using Newtonsoft.Json.Converters;
// using UglyToad.PdfPig.Graphics.Colors;
// using System.Text.Json;
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

builder.Services.AddDbContext<DocDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));


// Add services to the container.
//var connectionString = builder.Configuration.GetConnectionString("PacienteConnection");

//builder.Services.AddDbContext<PacienteContext>(opts => opts.UseLazyLoadingProxies().UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

var config = new MapperConfiguration(cfg =>
{
    cfg.AddProfile<PacienteProfile>();
    cfg.AddProfile<ProntuarioProfile>();
    cfg.AddProfile<AgendamentoProfile>();
    cfg.AddProfile<AtendimentoProfile>();
    cfg.AddMaps(typeof(Program).Assembly);
});

IMapper mapper = config.CreateMapper();
//builder.Services.AddAutoMapper(typeof(Program).Assembly); outra opção


builder.Services.AddSingleton<IMapper>(mapper);

builder.Services.AddSingleton<GoogleSheetsDB>();
builder.Services.AddSingleton<PdfGeneratorService>();
builder.Services.AddSingleton<FileDataOfSenhaExtractorService>();
builder.Services.AddScoped<IPacienteRepository, PacienteSheetsRepository>();
builder.Services.AddScoped<IProntuarioRepository, ProntuarioSheetsRepository>();
builder.Services.AddScoped<IAgendamentoRepository, AgendamentoSheetsRepository>();
builder.Services.AddScoped<IAtendimentoRepository, AtendimentoSheetsRepository>();

builder.Services.AddControllers().AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.Converters.Add(new StringEnumConverter());
    });
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
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

app.Run();

