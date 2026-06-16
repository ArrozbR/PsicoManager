namespace PsicoManager.Application.UseCases.Financeiro;

/// <summary>UC04b — Registrar Recebimento de Pagamento (baixa manual).</summary>
public interface IConfirmarRecebimentoUseCase
{
    Task ExecutarAsync(Guid cobrancaId, CancellationToken ct = default);
}
