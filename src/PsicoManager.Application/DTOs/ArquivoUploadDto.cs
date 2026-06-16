namespace PsicoManager.Application.DTOs;

/// <summary>Arquivo recebido para anexar à evolução (CO-01).</summary>
public record ArquivoUploadDto(string Nome, string Tipo, long Tamanho, byte[] Conteudo);
