using PsicoManager.Application.Abstractions;
using PsicoManager.Application.DTOs;
using PsicoManager.Domain.Entities;
using PsicoManager.Domain.Enums;
using PsicoManager.Domain.Exceptions;
using PsicoManager.Domain.Repositories;
using PsicoManager.Domain.Services;

namespace PsicoManager.Application.UseCases.Teleconsulta;

using TeleconsultaEntity = Domain.Entities.Teleconsulta;

/// <summary>
/// UC03 — Realizar Teleconsulta. Gera sala E2EE (RNF02 / Res. CFP 11/2018)
/// e disponibiliza o link de acesso. 1 sala por sessão (UK).
/// </summary>
public sealed class IniciarTeleconsultaUseCase : IIniciarTeleconsultaUseCase
{
    private readonly IUsuarioCorrente _usuario;
    private readonly ISessaoRepository _sessoes;
    private readonly ITeleconsultaRepository _teleconsultas;
    private readonly ISalaVirtualManager _sala;
    private readonly IUnitOfWork _uow;

    public IniciarTeleconsultaUseCase(
        IUsuarioCorrente usuario, ISessaoRepository sessoes,
        ITeleconsultaRepository teleconsultas, ISalaVirtualManager sala, IUnitOfWork uow)
    {
        _usuario = usuario;
        _sessoes = sessoes;
        _teleconsultas = teleconsultas;
        _sala = sala;
        _uow = uow;
    }

    public async Task<IniciarTeleconsultaResponse> ExecutarAsync(Guid sessaoId, CancellationToken ct = default)
    {
        if (_usuario.Perfil != PerfilUsuario.Psicologo)
            throw new ConflitoAcessoException("Apenas psicólogos iniciam teleconsultas.");

        var sessao = await _sessoes.ObterPorIdAsync(sessaoId, ct)
            ?? throw new RegraDeNegocioException("Sessão não encontrada.");
        if (sessao.Tipo != TipoSessao.Teleconsulta)
            throw new RegraDeNegocioException("Sessão não é do tipo teleconsulta.");

        var existente = await _teleconsultas.ObterPorSessaoAsync(sessaoId, ct);
        if (existente is not null)
            return new IniciarTeleconsultaResponse(
                existente.Id, existente.LinkAcesso, existente.Status.ToString().ToLowerInvariant());

        var link = _sala.CriarSala(sessaoId);
        var tele = new TeleconsultaEntity(sessaoId, link, DateTime.UtcNow.AddHours(2));
        tele.Iniciar();

        await _teleconsultas.AdicionarAsync(tele, ct);
        await _uow.CommitAsync(ct);

        return new IniciarTeleconsultaResponse(tele.Id, tele.LinkAcesso,
            tele.Status.ToString().ToLowerInvariant());
    }
}
