using System.Globalization;
using Microsoft.AspNetCore.Localization;
using PsicoManager.Data;

var builder = WebApplication.CreateBuilder(args);

// Cultura pt-BR: moeda em R$ e datas dd/MM/yyyy.
var culturaBr = new CultureInfo("pt-BR");
CultureInfo.DefaultThreadCurrentCulture = culturaBr;
CultureInfo.DefaultThreadCurrentUICulture = culturaBr;

builder.Services.AddControllersWithViews();

// Dados mockados em memória (singleton — sem banco real).
builder.Services.AddSingleton<MockDataStore>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();

var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(culturaBr),
    SupportedCultures = new List<CultureInfo> { culturaBr },
    SupportedUICultures = new List<CultureInfo> { culturaBr }
};
app.UseRequestLocalization(localizationOptions);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
