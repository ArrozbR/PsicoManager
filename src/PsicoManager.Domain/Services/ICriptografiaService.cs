namespace PsicoManager.Domain.Services;

/// <summary>RNF02 — AES-256 em repouso para dados clínicos.</summary>
public interface ICriptografiaService
{
    string Criptografar(string textoClaro);
    string Descriptografar(string textoCifrado);
    byte[] CriptografarBytes(byte[] dados);
}
