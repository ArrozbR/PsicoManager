using PsicoManager.Application.Abstractions;
using PsicoManager.Domain.Enums;
using PsicoManager.Domain.Exceptions;
using PsicoManager.Domain.Repositories;
using PsicoManager.Domain.Services;

namespace PsicoManager.Application.UseCases.Financeiro;

/// <summary>UC04b — baixa de pagamento (manual ou via webhook do gateway).</summary>
public sealed class ConfirmarRecebimentoUseCase : IConfirmarRecebimentoUseCase
{
    private readonly IUsuarioCorrente _usuario;
    private readonly ICobrancaRepository _cobrancas;
    private readonly IAuditLogger _audit;
    private readonly IUnitOfWork _uow;

    public ConfirmarRecebimentoUseCase(
        IUsuarioCorrente usuario, ICobrancaRepository cobrancas,
        IAuditLogger audit, IUnitOfWork uow)
    {
        _usuario = usuario;
        _cobrancas = cobrancas;
        _audit = audit;
        _uow = uow;
    }

    public async Task ExecutarAsync(Guid cobrancaId, CancellationToken ct = default)
    {
        var cobranca = await _cobrancas.ObterPorIdAsync(cobrancaId, ct)
            ?? throw new RegraDeNegocioException("Cobrança não encontrada.");

        cobranca.ConfirmarPagamento();
        _cobrancas.Atualizar(cobranca);

        await _audit.RegistrarAsync(_usuario.UsuarioId, PerfilUsuario.Psicologo,
            AcaoAuditoria.Edicao, EntidadeAuditada.Cobranca, cobranca.Id,
            _usuario.IpOrigem, ResultadoAuditoria.Sucesso, ct);

        await _uow.CommitAsync(ct);
    }
}
