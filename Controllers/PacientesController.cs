using Microsoft.AspNetCore.Mvc;
using PsicoManager.Data;
using PsicoManager.Models;

namespace PsicoManager.Controllers;

public class PacientesController : Controller
{
    private readonly MockDataStore _db;
    public PacientesController(MockDataStore db) => _db = db;

    public IActionResult Index()
    {
        var linhas = _db.Pacientes
            .OrderBy(p => p.Nome)
            .Select(p => new PacienteLinhaVm(p.Id, p.Nome, p.Telefone, p.Whatsapp, p.Ativo))
            .ToList();
        return View(linhas);
    }
}
