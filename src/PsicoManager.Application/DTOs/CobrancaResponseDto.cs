namespace PsicoManager.Application.DTOs;

public record CobrancaResponseDto(
    Guid CobrancaId,
    string? PixCopiaCola,
    string Status,
    string? Aviso = null);
