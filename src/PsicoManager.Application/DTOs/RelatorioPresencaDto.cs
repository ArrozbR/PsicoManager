namespace PsicoManager.Application.DTOs;

/// <summary>CSU09 — Relatório de Presença.</summary>
public record RelatorioPresencaDto(
    DateTime Inicio,
    DateTime Fim,
    double TaxaPresencaGeral,
    IReadOnlyList<ItemPresencaDto> Sessoes,
    IReadOnlyList<TaxaNoShowDto> NoShowPorPaciente);

public record ItemPresencaDto(DateTime Data, Guid PacienteId, string Tipo, string Status);
public record TaxaNoShowDto(Guid PacienteId, double TaxaNoShow);
