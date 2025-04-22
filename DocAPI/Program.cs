using DocAPI.Data;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using DocAPI.Profiles;
using DocAPI.Services;
using DocAPI.CLI;

var builder = WebApplication.CreateBuilder(args);

//CLI
if (args.Contains("--extract"))
{
    ExtractExamesCli.Run(args);
    return;
}

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("PacienteConnection");

builder.Services.AddDbContext<PacienteContext>(opts => opts.UseLazyLoadingProxies().UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

var config = new MapperConfiguration(cfg =>
{
    cfg.AddProfile<PacienteProfile>();
    cfg.AddMaps(typeof(Program).Assembly);
});

IMapper mapper = config.CreateMapper();

builder.Services.AddSingleton<IMapper>(mapper);


builder.Services.AddControllers().AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
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

// var googleSheetsService = new GoogleSheetsDB();
// googleSheetsService.LerPlanilha();

app.Run();
