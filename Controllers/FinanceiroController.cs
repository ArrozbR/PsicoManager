using Microsoft.AspNetCore.Mvc;
using PsicoManager.Domain.Enums;
using PsicoManager.Data;
using PsicoManager.Models;

namespace PsicoManager.Controllers;

public class FinanceiroController : Controller
{
    private readonly MockDataStore _db;
    public FinanceiroController(MockDataStore db) => _db = db;

    public IActionResult Index()
    {
        var cobs = _db.Cobrancas;
        var faturado = cobs.Sum(c => c.Valor);
        var recebido = cobs.Where(c => c.Status == StatusCobranca.Pago).Sum(c => c.Valor);
        var emAberto = cobs.Where(c => c.Status == StatusCobranca.Pendente).Sum(c => c.Valor);
        var inadimplencia = cobs.Where(c => c.Status == StatusCobranca.Inadimplente).Sum(c => c.Valor);
        var perc = faturado == 0 ? 0 : Math.Round((double)(inadimplencia / faturado) * 100, 1);

        var linhas = cobs
            .OrderByDescending(c => c.DataLancamento)
            .Select(c => new CobrancaLinhaVm(
                c.Id, _db.NomePaciente(c.PacienteId), c.Valor,
                c.Status.ToString(), c.DataLancamento))
            .ToList();

        return View(new FinanceiroVm(faturado, recebido, emAberto, inadimplencia, perc, linhas));
    }
}
