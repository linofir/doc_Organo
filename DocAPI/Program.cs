using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Newtonsoft.Json.Converters;
using QuestPDF.Infrastructure;

using DocAPI.CLI;
using DocAPI.Profiles;
using DocAPI.Core.Interfaces.Repositories;
using DocAPI.Interfaces.Repositories;
using DocAPI.Infrastructure.Repositories;
using DocAPI.Infrastructure.SqlDb;
using DocAPI.Infrastructure.SqlDb.Context;



var builder = WebApplication.CreateBuilder(args);

QuestPDF.Settings.License = LicenseType.Community;
//CLI
if (args.Contains("--extract"))
{
    ExtractExamesCli.Run(args);
    return;
}

var connectionString = SqlConnectionResolver.ResolveConnectionString()
    ?? builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString) ||
    connectionString.Contains("${SA_PASSWORD}", StringComparison.Ordinal))
{
    throw new InvalidOperationException(
        $"SQL connection is not configured. {SqlConnectionResolver.GetSetupHint()}");
}

builder.Services.AddDbContext<DocDbContext>(options =>
    options.UseSqlServer(connectionString));

// var conn = builder.Configuration.GetConnectionString("DefaultConnection");
// Console.WriteLine("CONN STRING USADA:");
// Console.WriteLine(conn);
// var name = Environment.GetEnvironmentVariable("DB_NAME");

// builder.Services.AddDbContext<DocDbContext>(options =>
//     options.UseSqlServer(
//         $"Server=localhost,1433;Database=DocDb;User Id=sa;Password={password};TrustServerCertificate=True;"
//     ));
// Console.WriteLine($"DB Name:{name}");

var config = new MapperConfiguration(cfg =>
{
    cfg.AddProfile<PacienteProfile>();
    cfg.AddProfile<ProntuarioProfile>();
    cfg.AddProfile<AgendamentoProfile>();
    cfg.AddProfile<AtendimentoProfile>();
    cfg.AddMaps(typeof(Program).Assembly);
});

IMapper mapper = config.CreateMapper();
//builder.Services.AddAutoMapper(typeof(Program).Assembly); outra opção. talvez na prox refatoração


builder.Services.AddSingleton<IMapper>(mapper);

// builder.Services.AddAutoMapper(typeof(Program)); //Preparar para próxima refatoração

// builder.Services.AddSingleton<GoogleSheetsDB>();
// builder.Services.AddSingleton<PdfGeneratorService>(); refatorar para novo repo
// builder.Services.AddSingleton<FileDataOfSenhaExtractorService>(); refatorar para novo repo

builder.Services.AddScoped<IPacienteRepository, PacienteRepository>();
builder.Services.AddScoped<IProntuarioRepository, ProntuarioRepository>();
builder.Services.AddScoped<IAgendamentoRepository, AgendamentoRepository>();
builder.Services.AddScoped<IAtendimentoRepository, AtendimentoRepository>();

builder.Services.AddControllers().AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.Converters.Add(new StringEnumConverter());
    });
    // .AddJsonOptions(options =>
    // {
    //     options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    // });

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

