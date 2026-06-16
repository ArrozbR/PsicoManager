using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PsicoManager.Domain.Repositories;
using PsicoManager.Domain.Services;
using PsicoManager.Infrastructure.Integrations;
using PsicoManager.Infrastructure.Persistence;
using PsicoManager.Infrastructure.Persistence.Repositories;
using PsicoManager.Infrastructure.Security;
using PsicoManager.Infrastructure.Services;

namespace PsicoManager.Infrastructure.DependencyInjection;

public static class DependencyInjectionExtensions
{
    /// <summary>Registra DbContext (PostgreSQL), repositórios e serviços de infraestrutura.</summary>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration config)
    {
        var conn = config.GetConnectionString("ClinicoDb")
            ?? throw new InvalidOperationException("ConnectionStrings:ClinicoDb não configurada.");

        services.AddDbContext<PsicoManagerDbContext>(opt => opt.UseNpgsql(conn));

        // Repositórios
        services.AddScoped<IPsicologoRepository, PsicologoRepository>();
        services.AddScoped<IPacienteRepository, PacienteRepository>();
        services.AddScoped<ISessaoRepository, SessaoRepository>();
        services.AddScoped<IProntuarioRepository, ProntuarioRepository>();
        services.AddScoped<IEvolucaoRepository, EvolucaoRepository>();
        services.AddScoped<ICobrancaRepository, CobrancaRepository>();
        services.AddScoped<INotificacaoRepository, NotificacaoRepository>();
        services.AddScoped<ITeleconsultaRepository, TeleconsultaRepository>();
        services.AddScoped<IDiarioEmocoesRepository, DiarioEmocoesRepository>();
        services.AddScoped<IVinculoRepository, VinculoRepository>();
        services.AddScoped<ILogAuditoriaRepository, LogAuditoriaRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Serviços de domínio (implementações)
        services.AddSingleton<ICriptografiaService, CriptografiaService>();
        services.AddScoped<IAuditLogger, AuditLogger>();
        services.AddScoped<IAcessoSigiloService, AcessoSigiloService>();
        services.AddSingleton<IStorageService, StorageService>();
        services.AddSingleton<ISalaVirtualManager, SalaVirtualManager>();
        services.AddSingleton<IGatewayPagamentoService, GatewayPagamentoService>();
        services.AddSingleton<INotificacaoSender, NotificacaoSender>();

        return services;
    }
}
