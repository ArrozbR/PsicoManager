using PsicoManager.Domain.Common;
using PsicoManager.Domain.Enums;
using PsicoManager.Domain.Exceptions;

namespace PsicoManager.Domain.Entities;

/// <summary>
/// Tabela: cobranca (EOBD). 1 cobrança por sessão (UK). Status só avança.
/// RN6 — registros imutáveis; estorno retroativo exige log.
/// </summary>
public class Cobranca : EntidadeBase
{
    public Guid SessaoId { get; private set; }
    public Guid PsicologoId { get; private set; }
    public Guid PacienteId { get; private set; }
    public decimal Valor { get; private set; }              // BRL, CHECK > 0
    public StatusCobranca Status { get; private set; } = StatusCobranca.Pendente;
    public FormaPagamento FormaPagamento { get; private set; } = FormaPagamento.Pix;
    public string? PixCodigo { get; private set; }          // EMV BACEN
    public DateTime? PixValidade { get; private set; }
    public DateOnly DataLancamento { get; private set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public DateOnly? DataPagamento { get; private set; }

    protected Cobranca() { } // EF

    public Cobranca(Guid sessaoId, Guid psicologoId, Guid pacienteId, decimal valor,
        FormaPagamento forma = FormaPagamento.Pix)
    {
        if (valor <= 0) throw new RegraDeNegocioException("Valor da cobrança deve ser positivo.");
        SessaoId = sessaoId;
        PsicologoId = psicologoId;
        PacienteId = pacienteId;
        Valor = valor;
        FormaPagamento = forma;
    }

    /// <summary>Gateway disponível: anexa o código Pix EMV gerado pelo BACEN.</summary>
    public void AnexarPix(string codigoEmv, DateTime validadeUtc)
    {
        PixCodigo = codigoEmv;
        PixValidade = validadeUtc;
    }

    /// <summary>Baixa de pagamento (confirmação automática via webhook ou manual).</summary>
    public void ConfirmarPagamento()
    {
        if (Status == StatusCobranca.Pago) return; // idempotente
        Status = StatusCobranca.Pago;
        DataPagamento = DateOnly.FromDateTime(DateTime.UtcNow);
    }

    public void MarcarInadimplente()
    {
        if (Status == StatusCobranca.Pago)
            throw new RegraDeNegocioException("Cobrança paga não pode ser marcada como inadimplente.");
        Status = StatusCobranca.Inadimplente;
    }
}
