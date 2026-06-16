using PsicoManager.Domain.Common;
using PsicoManager.Domain.Exceptions;

namespace PsicoManager.Domain.Entities;

/// <summary>
/// Tabela: registro_emocao (EOBD). Humor 1–10 (CHECK). Texto opcional
/// criptografado (AES-256). Editável por 24h.
/// </summary>
public class RegistroEmocao : EntidadeBase
{
    public Guid DiarioId { get; private set; }
    public DateTime DataHora { get; private set; } = DateTime.UtcNow;
    public short Humor { get; private set; }                // 1–10
    public string? TextoEnc { get; private set; }            // opcional, criptografado
    public DateTime EditavelAte { get; private set; }       // DataHora + 24h

    protected RegistroEmocao() { } // EF

    public RegistroEmocao(Guid diarioId, short humor, string? textoEnc = null)
    {
        if (humor < 1 || humor > 10)
            throw new RegraDeNegocioException("Humor deve estar entre 1 e 10.");
        DiarioId = diarioId;
        Humor = humor;
        TextoEnc = textoEnc;
        DataHora = DateTime.UtcNow;
        EditavelAte = DataHora.AddHours(24);
    }

    public void Editar(short humor, string? textoEnc, DateTime agoraUtc)
    {
        if (agoraUtc > EditavelAte)
            throw new RegraDeNegocioException("Registro não pode mais ser editado (prazo de 24h expirado).");
        if (humor < 1 || humor > 10)
            throw new RegraDeNegocioException("Humor deve estar entre 1 e 10.");
        Humor = humor;
        TextoEnc = textoEnc;
    }
}
