using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Options;
using DocFront.Config;
using DocFront.Services;
using Microsoft.AspNetCore.Http.Json;
using System.Text.Json.Serialization;
using DocFront.Utils.Serialization;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Carrega a URL base da API do appsettings.json
builder.Services.Configure<ApiSettings>(
    builder.Configuration.GetSection("ApiSettings"));

//Registra HttpClient com base address
builder.Services.AddHttpClient("DocApi", (sp, client) =>
{
    var settings = sp.GetRequiredService<IOptions<ApiSettings>>().Value;
    client.BaseAddress = new Uri(settings.BaseUrl);
});

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.SerializerOptions.Converters.Add(new DateOnlyJsonConverter());
});

// Registra seu ApiService
builder.Services.AddScoped<ApiService>();
builder.Services.AddScoped<PacienteService>();
builder.Services.AddScoped<ProntuarioService>();
builder.Services.AddScoped<AgendamentoService>();

// Registra seu State
builder.Services.AddScoped<PacienteState>();
builder.Services.AddScoped<ProntuarioState>();
builder.Services.AddScoped<AgendamentoState>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
