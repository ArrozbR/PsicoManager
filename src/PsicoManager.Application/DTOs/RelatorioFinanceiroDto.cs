namespace PsicoManager.Application.DTOs;

/// <summary>CSU08 — Relatório Financeiro Mensal.</summary>
public record RelatorioFinanceiroDto(
    int Mes,
    int Ano,
    decimal TotalFaturado,
    decimal TotalRecebido,
    decimal ValorEmAberto,
    decimal PercentualInadimplencia,
    decimal ProjecaoReceitaProximoMes,
    IReadOnlyList<ItemCobrancaDto> Itens);

public record ItemCobrancaDto(string Paciente, decimal Valor, string Status);
