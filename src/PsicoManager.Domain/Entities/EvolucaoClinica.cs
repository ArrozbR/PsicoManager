using PsicoManager.Domain.Common;
using PsicoManager.Domain.Enums;
using PsicoManager.Domain.Exceptions;

namespace PsicoManager.Domain.Entities;

/// <summary>
/// Tabela: evolucao_clinica (EOBD). Texto criptografado AES-256 em repouso
/// (texto_enc). RNF03 — trava de edição após prazo (padrão 24h).
/// </summary>
public class EvolucaoClinica : EntidadeBase
{
    public Guid ProntuarioId { get; private set; }
    public Guid? SessaoId { get; private set; }
    public Guid PsicologoId { get; private set; }
    public string TextoEnc { get; private set; } = null!;   // criptografado em repouso
    public DateTime DataHora { get; private set; } = DateTime.UtcNow;
    public StatusEvolucao Status { get; private set; } = StatusEvolucao.Editavel;
    public short PrazoTravaH { get; private set; } = 24;
    public DateTime TravarEm { get; private set; }          // DataHora + PrazoTravaH

    private readonly List<Anexo> _anexos = new();
    public IReadOnlyCollection<Anexo> Anexos => _anexos.AsReadOnly();

    protected EvolucaoClinica() { } // EF

    public EvolucaoClinica(Guid prontuarioId, Guid psicologoId, string textoEnc,
        Guid? sessaoId = null, short prazoTravaH = 24)
    {
        ProntuarioId = prontuarioId;
        PsicologoId = psicologoId;
        TextoEnc = textoEnc;
        SessaoId = sessaoId;
        PrazoTravaH = prazoTravaH;
        DataHora = DateTime.UtcNow;
        TravarEm = DataHora.AddHours(prazoTravaH);
    }

    public void AdicionarAnexo(Anexo anexo) => _anexos.Add(anexo);

    /// <summary>RNF03 — verifica se a janela de edição já expirou (idempotente).</summary>
    public bool DeveEstarTravada(DateTime agoraUtc) => agoraUtc >= TravarEm;

    public void Travar() => Status = StatusEvolucao.Travada;

    /// <summary>RNF03 — edição só é permitida dentro do prazo.</summary>
    public void Editar(string novoTextoEnc, DateTime agoraUtc)
    {
        if (Status == StatusEvolucao.Travada || DeveEstarTravada(agoraUtc))
        {
            Status = StatusEvolucao.Travada;
            throw new ProntuarioTravadoException();
        }
        if (string.IsNullOrWhiteSpace(novoTextoEnc))
            throw new TextoVazioException();
        TextoEnc = novoTextoEnc;
    }
}
