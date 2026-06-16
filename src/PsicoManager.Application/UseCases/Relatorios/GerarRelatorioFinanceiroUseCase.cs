using PsicoManager.Application.Abstractions;
using PsicoManager.Application.DTOs;
using PsicoManager.Domain.Enums;
using PsicoManager.Domain.Repositories;

namespace PsicoManager.Application.UseCases.Relatorios;

/// <summary>
/// CSU08 — Gera relatório financeiro mensal: consolida faturado, recebido,
/// em aberto, inadimplência e projeção de receita do mês seguinte.
/// </summary>
public sealed class GerarRelatorioFinanceiroUseCase : IGerarRelatorioFinanceiroUseCase
{
    private readonly IUsuarioCorrente _usuario;
    private readonly ICobrancaRepository _cobrancas;
    private readonly IPacienteRepository _pacientes;

    public GerarRelatorioFinanceiroUseCase(
        IUsuarioCorrente usuario, ICobrancaRepository cobrancas, IPacienteRepository pacientes)
    {
        _usuario = usuario;
        _cobrancas = cobrancas;
        _pacientes = pacientes;
    }

    public async Task<RelatorioFinanceiroDto> ExecutarAsync(int mes, int ano, CancellationToken ct = default)
    {
        var inicio = new DateOnly(ano, mes, 1);
        var fim = inicio.AddMonths(1).AddDays(-1);

        var cobrancas = await _cobrancas.ListarPorPeriodoAsync(_usuario.UsuarioId, inicio, fim, ct);
        var pacientes = (await _pacientes.ListarPorPsicologoAsync(_usuario.UsuarioId, ct))
            .ToDictionary(p => p.Id, p => p.Nome);

        var totalFaturado = cobrancas.Sum(c => c.Valor);
        var totalRecebido = cobrancas.Where(c => c.Status == StatusCobranca.Pago).Sum(c => c.Valor);
        var emAberto = cobrancas
            .Where(c => c.Status is StatusCobranca.Pendente or StatusCobranca.Inadimplente)
            .Sum(c => c.Valor);
        var inadimplente = cobrancas.Where(c => c.Status == StatusCobranca.Inadimplente).Sum(c => c.Valor);
        var percInadimplencia = totalFaturado == 0 ? 0 : Math.Round(inadimplente / totalFaturado * 100, 2);

        // Projeção simples: média dos recebimentos do mês como base do próximo.
        var projecao = totalRecebido;

        var itens = cobrancas
            .Select(c => new ItemCobrancaDto(
                pacientes.GetValueOrDefault(c.PacienteId, "—"),
                c.Valor, c.Status.ToString().ToLowerInvariant()))
            .ToList();

        return new RelatorioFinanceiroDto(
            mes, ano, totalFaturado, totalRecebido, emAberto,
            percInadimplencia, projecao, itens);
    }
}
