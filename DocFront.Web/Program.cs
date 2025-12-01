using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using DocFront.Web.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Carrega a URL base da API do appsettings.json
var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"];

// Registra HttpClient com base address
// builder.Services.AddHttpClient("DocApi", client =>
// {
//     client.BaseAddress = new Uri(apiBaseUrl!);
// });

// // Registra seu ApiService
// builder.Services.AddScoped<ApiService>();

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
