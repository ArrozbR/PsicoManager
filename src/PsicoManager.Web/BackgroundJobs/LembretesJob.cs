using Microsoft.Extensions.DependencyInjection;
using PsicoManager.Application.UseCases.Notificacoes;

namespace PsicoManager.Web.BackgroundJobs;

/// <summary>UC05 — processa notificações pendentes (lembretes) a cada 5 min.</summary>
public sealed class LembretesJob : BackgroundService
{
    private readonly IServiceProvider _provider;
    private readonly ILogger<LembretesJob> _logger;
    private static readonly TimeSpan Intervalo = TimeSpan.FromMinutes(5);

    public LembretesJob(IServiceProvider provider, ILogger<LembretesJob> logger)
    {
        _provider = provider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _provider.CreateScope();
                var uc = scope.ServiceProvider.GetRequiredService<IEnviarLembretesUseCase>();
                var enviadas = await uc.ProcessarPendentesAsync(stoppingToken);
                if (enviadas > 0)
                    _logger.LogInformation("UC05: {Qtd} notificações enviadas.", enviadas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha no LembretesJob");
            }

            await Task.Delay(Intervalo, stoppingToken);
        }
    }
}
