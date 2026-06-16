using PsicoManager.Application.Abstractions;
using PsicoManager.Application.DTOs;
using PsicoManager.Domain.Entities;
using PsicoManager.Domain.Enums;
using PsicoManager.Domain.Repositories;

namespace PsicoManager.Application.UseCases.Relatorios;

/// <summary>
/// CSU09 — Relatório de presença: classifica sessões por status (realizada,
/// cancelada, no-show, remarcada) e calcula taxa de no-show por paciente.
/// </summary>
public sealed class GerarRelatorioPresencaUseCase : IGerarRelatorioPresencaUseCase
{
    private readonly IUsuarioCorrente _usuario;
    private readonly ISessaoRepository _sessoes;

    public GerarRelatorioPresencaUseCase(IUsuarioCorrente usuario, ISessaoRepository sessoes)
    {
        _usuario = usuario;
        _sessoes = sessoes;
    }

    public async Task<RelatorioPresencaDto> ExecutarAsync(
        DateTime inicioUtc, DateTime fimUtc, Guid? pacienteId = null, CancellationToken ct = default)
    {
        var sessoes = await _sessoes.ListarPorPeriodoAsync(_usuario.UsuarioId, inicioUtc, fimUtc, ct);
        if (pacienteId.HasValue)
            sessoes = sessoes.Where(s => s.PacienteId == pacienteId.Value).ToList();

        var total = sessoes.Count;
        var realizadas = sessoes.Count(s => s.Status == StatusSessao.Realizada);
        var taxaPresenca = total == 0 ? 0d : Math.Round((double)realizadas / total * 100, 2);

        var itens = sessoes
            .Select(s => new ItemPresencaDto(
                s.DataHora, s.PacienteId,
                s.Tipo.ToString().ToLowerInvariant(),
                s.Status.ToString().ToLowerInvariant()))
            .ToList();

        var noShow = sessoes
            .GroupBy(s => s.PacienteId)
            .Select(g =>
            {
                var faltas = g.Count(s => s.Status == StatusSessao.Falta);
                var taxa = g.Count() == 0 ? 0d : Math.Round((double)faltas / g.Count() * 100, 2);
                return new TaxaNoShowDto(g.Key, taxa);
            })
            .ToList();

        return new RelatorioPresencaDto(inicioUtc, fimUtc, taxaPresenca, itens, noShow);
    }
}
