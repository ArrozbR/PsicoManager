namespace PsicoManager.Application.DTOs;

public record RegistrarEvolucaoRequest(
    Guid PacienteId,
    string Texto,
    Guid? SessaoId = null,
    IReadOnlyList<ArquivoUploadDto>? Arquivos = null);
