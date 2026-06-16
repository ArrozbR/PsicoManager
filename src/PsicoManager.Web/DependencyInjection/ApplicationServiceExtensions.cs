using Microsoft.Extensions.DependencyInjection;
using PsicoManager.Application.UseCases.Agenda;
using PsicoManager.Application.UseCases.Financeiro;
using PsicoManager.Application.UseCases.Notificacoes;
using PsicoManager.Application.UseCases.Paciente;
using PsicoManager.Application.UseCases.Prontuario;
using PsicoManager.Application.UseCases.Relatorios;
using PsicoManager.Application.UseCases.Teleconsulta;

namespace PsicoManager.Web.DependencyInjection;

public static class ApplicationServiceExtensions
{
    /// <summary>Registra os casos de uso da camada de aplicação.</summary>
    public static IServiceCollection AddApplicationUseCases(this IServiceCollection services)
    {
        services.AddScoped<IRegistrarEvolucaoUseCase, RegistrarEvolucaoUseCase>();
        services.AddScoped<IAgendarSessaoUseCase, AgendarSessaoUseCase>();
        services.AddScoped<ILancarCobrancaUseCase, LancarCobrancaUseCase>();
        services.AddScoped<IConfirmarRecebimentoUseCase, ConfirmarRecebimentoUseCase>();
        services.AddScoped<IGerarRelatorioFinanceiroUseCase, GerarRelatorioFinanceiroUseCase>();
        services.AddScoped<IGerarRelatorioPresencaUseCase, GerarRelatorioPresencaUseCase>();
        services.AddScoped<IIniciarTeleconsultaUseCase, IniciarTeleconsultaUseCase>();
        services.AddScoped<IEnviarLembretesUseCase, EnviarLembretesUseCase>();
        services.AddScoped<ICriarPacienteUseCase, CriarPacienteUseCase>();
        services.AddScoped<IRegistrarEmocaoUseCase, RegistrarEmocaoUseCase>();
        return services;
    }
}
