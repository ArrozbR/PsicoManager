using Microsoft.Extensions.DependencyInjection;
using PsicoManager.Domain.Repositories;

namespace PsicoManager.Web.BackgroundJobs;

/// <summary>
/// RNF03 — varre periodicamente as evoluções cujo prazo de trava venceu e as
/// sela (status 'travada'), impedindo edições posteriores. Roda a cada 30 min.
/// </summary>
public sealed class TravaEdicaoJob : BackgroundService
{
    private readonly IServiceProvider _provider;
    private readonly ILogger<TravaEdicaoJob> _logger;
    private static readonly TimeSpan Intervalo = TimeSpan.FromMinutes(30);

    public TravaEdicaoJob(IServiceProvider provider, ILogger<TravaEdicaoJob> logger)
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
                var repo = scope.ServiceProvider.GetRequiredService<IEvolucaoRepository>();
                var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                var vencidas = await repo.ListarVencidasNaoTravadasAsync(DateTime.UtcNow, stoppingToken);
                foreach (var ev in vencidas)
                {
                    ev.Travar();
                    repo.Atualizar(ev);
                }
                if (vencidas.Count > 0)
                {
                    await uow.CommitAsync(stoppingToken);
                    _logger.LogInformation("RNF03: {Qtd} evoluções travadas.", vencidas.Count);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha no TravaEdicaoJob");
            }

            await Task.Delay(Intervalo, stoppingToken);
        }
    }
}
