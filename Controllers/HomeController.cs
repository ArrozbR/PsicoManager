using Microsoft.AspNetCore.Mvc;
using PsicoManager.Domain.Enums;
using PsicoManager.Data;
using PsicoManager.Models;

namespace PsicoManager.Controllers;

public class HomeController : Controller
{
    private readonly MockDataStore _db;
    public HomeController(MockDataStore db) => _db = db;

    public IActionResult Index()
    {
        var hoje = DateTime.UtcNow.Date;
        var inicioMes = new DateOnly(hoje.Year, hoje.Month, 1);

        var sessoesHoje = _db.Sessoes.Count(s => s.DataHora.Date == hoje);
        var agendadas = _db.Sessoes.Count(s => s.Status == StatusSessao.Agendada);

        var cobrancasMes = _db.Cobrancas
            .Where(c => c.DataLancamento >= inicioMes).ToList();
        var faturado = cobrancasMes.Sum(c => c.Valor);
        var recebido = cobrancasMes.Where(c => c.Status == StatusCobranca.Pago).Sum(c => c.Valor);
        var emAberto = cobrancasMes
            .Where(c => c.Status is StatusCobranca.Pendente or StatusCobranca.Inadimplente)
            .Sum(c => c.Valor);

        var totalSess = _db.Sessoes.Count(s => s.Status != StatusSessao.Agendada);
        var realizadas = _db.Sessoes.Count(s => s.Status == StatusSessao.Realizada);
        var taxaPresenca = totalSess == 0 ? 0 : Math.Round((double)realizadas / totalSess * 100, 1);

        var proximas = _db.Sessoes
            .Where(s => s.Status == StatusSessao.Agendada && s.DataHora >= DateTime.UtcNow)
            .OrderBy(s => s.DataHora)
            .Take(6)
            .Select(s => new ProximaSessaoVm(
                _db.NomePaciente(s.PacienteId), s.DataHora,
                s.Tipo.ToString(), s.Status.ToString()))
            .ToList();

        var atividades = _db.Evolucoes
            .OrderByDescending(e => e.DataHora)
            .Take(5)
            .Select(e => new AtividadeVm(e.DataHora, "Evolução clínica registrada", "evolucao"))
            .ToList();

        var vm = new DashboardVm(
            _db.Psicologo.Nome,
            _db.Pacientes.Count(p => p.Ativo),
            sessoesHoje,
            agendadas,
            faturado, recebido, emAberto,
            taxaPresenca,
            proximas,
            atividades);

        return View(vm);
    }
}
