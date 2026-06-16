namespace PsicoManager.Application.DTOs;

public record SessaoResponseDto(
    Guid SessaoId,
    string Status,
    DateTime DataHora,
    string Tipo,
    bool ConfirmacaoEnviada);
