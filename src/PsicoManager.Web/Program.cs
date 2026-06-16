using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PsicoManager.Application.Abstractions;
using PsicoManager.Infrastructure.DependencyInjection;
using PsicoManager.Web.BackgroundJobs;
using PsicoManager.Web.DependencyInjection;
using PsicoManager.Web.Identity;
using PsicoManager.Web.Middleware;

var builder = WebApplication.CreateBuilder(args);

// --- Camadas ---
builder.Services.AddInfrastructure(builder.Configuration); // DbContext + repos + serviços
builder.Services.AddApplicationUseCases();                 // casos de uso

// --- Usuário corrente (lê claims do JWT) ---
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUsuarioCorrente, UsuarioCorrente>();

// --- Autenticação JWT (RNF: identidade/perfis) ---
var jwtKey = builder.Configuration["Jwt:Key"] ?? "chave-de-desenvolvimento-trocar-em-producao-256bits!!";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "PsicoManager";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "PsicoManager";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });
builder.Services.AddAuthorization();

// --- API + Swagger ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "PsicoManager API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Informe o token JWT."
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// --- Background jobs (RNF03 trava de edição; UC05 lembretes) ---
builder.Services.AddHostedService<TravaEdicaoJob>();
builder.Services.AddHostedService<LembretesJob>();

var app = builder.Build();

// --- Pipeline ---
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
