using Microsoft.AspNetCore.Mvc;
using PsicoManager.Data;
using PsicoManager.Models;

namespace PsicoManager.Controllers;

public class AgendaController : Controller
{
    private readonly MockDataStore _db;
    public AgendaController(MockDataStore db) => _db = db;

    public IActionResult Index()
    {
        var linhas = _db.Sessoes
            .OrderByDescending(s => s.DataHora)
            .Select(s => new SessaoLinhaVm(
                s.Id, _db.NomePaciente(s.PacienteId), s.DataHora,
                s.DuracaoMin, s.Tipo.ToString(), s.Status.ToString()))
            .ToList();

        return View(new AgendaVm(linhas));
    }
}
