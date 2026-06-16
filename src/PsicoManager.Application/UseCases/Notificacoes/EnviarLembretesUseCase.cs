using PsicoManager.Domain.Repositories;
using PsicoManager.Domain.Services;

namespace PsicoManager.Application.UseCases.Notificacoes;

/// <summary>
/// UC05 — Processa notificações pendentes e dispara via canal (WhatsApp/SMTP).
/// Em caso de falha, registra e reprograma (RetryScheduler, máx. 3 tentativas).
/// </summary>
public sealed class EnviarLembretesUseCase : IEnviarLembretesUseCase
{
    private static readonly TimeSpan IntervaloRetry = TimeSpan.FromMinutes(15);

    private readonly INotificacaoRepository _notificacoes;
    private readonly INotificacaoSender _sender;
    private readonly IUnitOfWork _uow;

    public EnviarLembretesUseCase(
        INotificacaoRepository notificacoes, INotificacaoSender sender, IUnitOfWork uow)
    {
        _notificacoes = notificacoes;
        _sender = sender;
        _uow = uow;
    }

    public async Task<int> ProcessarPendentesAsync(CancellationToken ct = default)
    {
        var pendentes = await _notificacoes.ListarPendentesAsync(DateTime.UtcNow, ct);
        var enviadas = 0;

        foreach (var n in pendentes)
        {
            var ok = await _sender.EnviarAsync(
                n.Canal, n.DestinatarioId.ToString(), n.Tipo,
                new Dictionary<string, string> { ["sessaoId"] = n.SessaoId.ToString() }, ct);

            if (ok) { n.MarcarEnviada(); enviadas++; }
            else { n.RegistrarFalha(IntervaloRetry); }

            _notificacoes.Atualizar(n);
        }

        await _uow.CommitAsync(ct);
        return enviadas;
    }
}
