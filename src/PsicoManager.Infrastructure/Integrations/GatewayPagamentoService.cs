using Microsoft.Extensions.Configuration;
using PsicoManager.Domain.Services;

namespace PsicoManager.Infrastructure.Integrations;

/// <summary>
/// Adaptador do Gateway de Pagamento (Pix EMV — BACEN). Implementação stub que
/// gera um payload EMV "copia e cola" mínimo. A disponibilidade é controlada por
/// configuração (Gateway:Disponivel) para exercitar o fluxo de fallback do UC04a.
/// </summary>
public sealed class GatewayPagamentoService : IGatewayPagamentoService
{
    public bool Disponivel { get; }

    public GatewayPagamentoService(IConfiguration config)
        => Disponivel = bool.TryParse(config["Gateway:Disponivel"], out var d) ? d : true;

    public Task<PixGerado> GerarPixCopiaColaAsync(decimal valor, string descricao,
        Guid cobrancaId, TimeSpan validade, CancellationToken ct = default)
    {
        // Payload EMV simplificado — em produção, integrar com PSP homologado BACEN.
        var payload = MontarEmv(valor, cobrancaId);
        return Task.FromResult(new PixGerado(payload, DateTime.UtcNow.Add(validade)));
    }

    private static string MontarEmv(decimal valor, Guid cobrancaId)
    {
        var valorStr = valor.ToString("F2", System.Globalization.CultureInfo.InvariantCulture);
        // Formato ilustrativo (não é um BR Code válido para produção).
        return $"00020126BR.GOV.BCB.PIX5204000053039865406{valorStr}5802BR" +
               $"6009PSICOMGR62070503{cobrancaId.ToString("N")[..8]}6304ABCD";
    }
}
