using PsicoManager.Domain.Enums;
using PsicoManager.Domain.Exceptions;
using PsicoManager.Domain.Repositories;
using PsicoManager.Domain.Services;

namespace PsicoManager.Infrastructure.Services;

/// <summary>
/// RN1 / RNF01 — verifica vínculo terapêutico ativo antes de liberar acesso
/// a dados clínicos. Sem vínculo: registra ACESSO_NEGADO (RNF04) e lança 403.
/// </summary>
public sealed class AcessoSigiloService : IAcessoSigiloService
{
    private readonly IVinculoRepository _vinculos;
    private readonly IAuditLogger _audit;

    public AcessoSigiloService(IVinculoRepository vinculos, IAuditLogger audit)
    {
        _vinculos = vinculos;
        _audit = audit;
    }

    public async Task ValidarAcessoAsync(Guid psicologoId, Guid pacienteId, string ipOrigem,
        EntidadeAuditada entidade, Guid entidadeId, CancellationToken ct = default)
    {
        var temVinculo = await _vinculos.ExisteVinculoAtivoAsync(psicologoId, pacienteId, ct);
        if (!temVinculo)
        {
            await _audit.RegistrarAsync(psicologoId, PerfilUsuario.Psicologo,
                AcaoAuditoria.AcessoNegado, entidade, entidadeId, ipOrigem,
                ResultadoAuditoria.Negado, ct);
            throw new ConflitoAcessoException();
        }
    }
}
