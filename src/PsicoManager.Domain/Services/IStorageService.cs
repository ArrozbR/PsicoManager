namespace PsicoManager.Domain.Services;

/// <summary>Storage de anexos criptografados (S3/Blob AES-256).</summary>
public interface IStorageService
{
    /// <summary>Sobe bytes já criptografados e retorna a URL no storage.</summary>
    Task<string> UploadAsync(byte[] conteudoCriptografado, string caminho, CancellationToken ct = default);
}
