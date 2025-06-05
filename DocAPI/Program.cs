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

var pathToXlsFile = @"C:\Users\lino\Downloads\transferir (1).xls";
var service = new CollectDemonstrativoDataService(pathToXlsFile);

app.Run();

