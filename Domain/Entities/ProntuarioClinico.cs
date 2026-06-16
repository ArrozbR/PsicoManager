using PsicoManager.Domain.Common;
using PsicoManager.Domain.Enums;
using PsicoManager.Domain.Exceptions;

namespace PsicoManager.Domain.Entities;

/// <summary>
/// Tabela: prontuario_clinico (EOBD). 1 prontuário por paciente (UK).
/// RNF05/RN3 — retenção mínima de 5 anos a contar da última evolução.
/// </summary>
public class ProntuarioClinico : EntidadeBase
{
    public Guid PacienteId { get; private set; }
    public Guid PsicologoId { get; private set; }
    public StatusProntuario Status { get; private set; } = StatusProntuario.Ativo;
    public DateTime? UltimaEvolucaoEm { get; private set; } // base para retenção 5 anos

    private readonly List<EvolucaoClinica> _evolucoes = new();
    public IReadOnlyCollection<EvolucaoClinica> Evolucoes => _evolucoes.AsReadOnly();

    protected ProntuarioClinico() { } // EF

    public ProntuarioClinico(Guid pacienteId, Guid psicologoId)
    {
        PacienteId = pacienteId;
        PsicologoId = psicologoId;
    }

    /// <summary>Pós-condição 2 do CO-01: atualiza ultima_evolucao_em.</summary>
    public void RegistrarMovimentoEvolucao(DateTime quandoUtc)
        => UltimaEvolucaoEm = quandoUtc;

    public void Arquivar() => Status = StatusProntuario.Arquivado;

    /// <summary>RNF05 — bloqueia exclusão antes de 5 anos da última sessão registrada.</summary>
    public void GarantirRetencaoCumprida()
    {
        var referencia = UltimaEvolucaoEm ?? CriadoEm;
        if (referencia.AddYears(5) > DateTime.UtcNow)
            throw new RetencaoObrigatoriaException();
    }
}
