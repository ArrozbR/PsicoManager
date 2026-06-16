using PsicoManager.Domain.Entities;

namespace PsicoManager.Domain.Repositories;

public interface IEvolucaoRepository : IRepositorioBase<EvolucaoClinica>
{
    Task<IReadOnlyList<EvolucaoClinica>> ListarPorProntuarioAsync(Guid prontuarioId, CancellationToken ct = default);

    /// <summary>RNF03 — evoluções cuja janela de trava já venceu mas ainda estão editáveis.</summary>
    Task<IReadOnlyList<EvolucaoClinica>> ListarVencidasNaoTravadasAsync(DateTime agoraUtc, CancellationToken ct = default);
}
