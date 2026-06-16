using PsicoManager.Domain.Entities;

namespace PsicoManager.Domain.Repositories;

public interface ISessaoRepository : IRepositorioBase<Sessao>
{
    /// <summary>RN2 — busca sessões que conflitam com o intervalo informado.</summary>
    Task<IReadOnlyList<Sessao>> BuscarConflitosAsync(
        Guid psicologoId, DateTime inicioUtc, int duracaoMin, CancellationToken ct = default);

    Task<IReadOnlyList<Sessao>> ListarPorPeriodoAsync(
        Guid psicologoId, DateTime inicioUtc, DateTime fimUtc, CancellationToken ct = default);

    Task<bool> ExisteSessaoRealizadaAsync(Guid sessaoId, CancellationToken ct = default);
}
