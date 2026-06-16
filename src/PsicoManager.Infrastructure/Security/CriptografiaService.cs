using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using PsicoManager.Domain.Services;

namespace PsicoManager.Infrastructure.Security;

/// <summary>
/// RNF02 — AES-256 em modo CBC com IV aleatório por operação. A chave de 32 bytes
/// vem de configuração (Crypto:Key, Base64). O IV é prefixado ao texto cifrado.
/// Em produção a chave deve residir em KMS/Key Vault, nunca em appsettings.
/// </summary>
public sealed class CriptografiaService : ICriptografiaService
{
    private readonly byte[] _key;

    public CriptografiaService(IConfiguration config)
    {
        var b64 = config["Crypto:Key"]
            ?? throw new InvalidOperationException("Crypto:Key não configurada.");
        _key = Convert.FromBase64String(b64);
        if (_key.Length != 32)
            throw new InvalidOperationException("Crypto:Key deve ter 32 bytes (AES-256).");
    }

    public string Criptografar(string textoClaro)
    {
        var dados = Encoding.UTF8.GetBytes(textoClaro);
        return Convert.ToBase64String(CriptografarBytes(dados));
    }

    public byte[] CriptografarBytes(byte[] dados)
    {
        using var aes = Aes.Create();
        aes.Key = _key;
        aes.GenerateIV();
        using var enc = aes.CreateEncryptor();
        var cifrado = enc.TransformFinalBlock(dados, 0, dados.Length);
        // prefixa o IV (16 bytes) ao payload.
        var saida = new byte[aes.IV.Length + cifrado.Length];
        Buffer.BlockCopy(aes.IV, 0, saida, 0, aes.IV.Length);
        Buffer.BlockCopy(cifrado, 0, saida, aes.IV.Length, cifrado.Length);
        return saida;
    }

    public string Descriptografar(string textoCifrado)
    {
        var bytes = Convert.FromBase64String(textoCifrado);
        using var aes = Aes.Create();
        aes.Key = _key;
        var iv = new byte[16];
        Buffer.BlockCopy(bytes, 0, iv, 0, 16);
        aes.IV = iv;
        using var dec = aes.CreateDecryptor();
        var claro = dec.TransformFinalBlock(bytes, 16, bytes.Length - 16);
        return Encoding.UTF8.GetString(claro);
    }
}
