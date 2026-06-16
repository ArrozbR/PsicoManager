namespace PsicoManager.Domain.Services;

/// <summary>Resultado da geração de Pix EMV via gateway (BACEN).</summary>
public record PixGerado(string CodigoEmv, DateTime Validade);

/// <summary>
/// Adaptador para o Gateway de Pagamento (Pix EMV — BACEN). Quando indisponível,
/// o serviço de aplicação faz fallback para registro local + baixa manual.
/// </summary>
public interface IGatewayPagamentoService
{
    bool Disponivel { get; }
    Task<PixGerado> GerarPixCopiaColaAsync(decimal valor, string descricao,
        Guid cobrancaId, TimeSpan validade, CancellationToken ct = default);
}
