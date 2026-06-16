using Microsoft.AspNetCore.Mvc;
using PsicoManager.Data;
using PsicoManager.Models;

namespace PsicoManager.Controllers;

public class ProntuarioController : Controller
{
    private readonly MockDataStore _db;
    public ProntuarioController(MockDataStore db) => _db = db;

    public IActionResult Index()
    {
        var pacientes = _db.Pacientes.Select(p =>
        {
            var pront = _db.Prontuarios.First(pr => pr.PacienteId == p.Id);
            var qtd = _db.Evolucoes.Count(e => e.ProntuarioId == pront.Id);
            return new PacienteProntuarioVm(p.Id, p.Nome, qtd, pront.UltimaEvolucaoEm);
        }).ToList();

        return View(new ProntuarioListaVm(pacientes));
    }

    public IActionResult Detalhe(Guid id)
    {
        var paciente = _db.Pacientes.FirstOrDefault(p => p.Id == id);
        if (paciente is null) return NotFound();

        var pront = _db.Prontuarios.First(pr => pr.PacienteId == id);
        var evolucoes = _db.Evolucoes
            .Where(e => e.ProntuarioId == pront.Id)
            .OrderByDescending(e => e.DataHora)
            .Select(e => new EvolucaoLinhaVm(
                e.DataHora, e.TextoEnc, e.Status.ToString(), e.TravarEm))
            .ToList();

        return View(new ProntuarioDetalheVm(paciente.Nome, id, evolucoes));
    }
}
