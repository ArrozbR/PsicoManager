using Microsoft.Extensions.Configuration;
using PsicoManager.Domain.Services;

namespace PsicoManager.Infrastructure.Services;

/// <summary>
/// Implementação de Storage para anexos criptografados. Aqui usamos disco local
/// (raiz configurável em Storage:Root) como stand-in do S3/Blob. O conteúdo já
/// chega criptografado (AES-256) da camada de aplicação.
/// </summary>
public sealed class StorageService : IStorageService
{
    private readonly string _root;

    public StorageService(IConfiguration config)
        => _root = config["Storage:Root"] ?? Path.Combine(AppContext.BaseDirectory, "storage");

    public async Task<string> UploadAsync(byte[] conteudoCriptografado, string caminho, CancellationToken ct = default)
    {
        var destino = Path.Combine(_root, caminho);
        Directory.CreateDirectory(Path.GetDirectoryName(destino)!);
        await File.WriteAllBytesAsync(destino, conteudoCriptografado, ct);
        return $"storage://{caminho}";
    }
}
