using PsicoManager.Domain.Common;
using PsicoManager.Domain.Exceptions;

namespace PsicoManager.Domain.Entities;

/// <summary>
/// Anexo de evolução clínica. Conteúdo criptografado (AES-256) no Storage;
/// apenas a URL e metadados ficam no banco. CO-01: PDF/JPEG/PNG, ≤ 10 MB.
/// </summary>
public class Anexo : EntidadeBase
{
    public const long TamanhoMaximoBytes = 10 * 1024 * 1024; // 10 MB
    private static readonly string[] TiposPermitidos = { "application/pdf", "image/jpeg", "image/png" };

    public Guid EvolucaoId { get; private set; }
    public string Nome { get; private set; } = null!;
    public string Tipo { get; private set; } = null!;       // mime-type
    public string UrlStorage { get; private set; } = null!; // URL no storage criptografado
    public long Tamanho { get; private set; }

    protected Anexo() { } // EF

    public Anexo(Guid evolucaoId, string nome, string tipo, string urlStorage, long tamanho)
    {
        if (!TiposPermitidos.Contains(tipo))
            throw new ArquivoInvalidoException($"Tipo de arquivo não suportado: {tipo}.");
        if (tamanho > TamanhoMaximoBytes)
            throw new ArquivoInvalidoException("Arquivo excede o limite de 10 MB.");

        EvolucaoId = evolucaoId;
        Nome = nome;
        Tipo = tipo;
        UrlStorage = urlStorage;
        Tamanho = tamanho;
    }

    public static void ValidarMetadados(string tipo, long tamanho)
    {
        if (!TiposPermitidos.Contains(tipo))
            throw new ArquivoInvalidoException($"Tipo de arquivo não suportado: {tipo}.");
        if (tamanho > TamanhoMaximoBytes)
            throw new ArquivoInvalidoException("Arquivo excede o limite de 10 MB.");
    }
}
