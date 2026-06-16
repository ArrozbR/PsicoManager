namespace PsicoManager.Application.DTOs;

/// <summary>Saída do CO-01: registrarEvolucao().</summary>
public record EvolucaoResponseDto(
    Guid EvolucaoId,
    DateTime DataHora,
    string Status,
    DateTime PrazoTravaEm,
    IReadOnlyList<AnexoDto> Anexos);
