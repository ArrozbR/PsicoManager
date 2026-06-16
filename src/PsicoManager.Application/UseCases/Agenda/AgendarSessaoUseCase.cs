using PsicoManager.Application.Abstractions;
using PsicoManager.Application.DTOs;
using PsicoManager.Domain.Entities;
using PsicoManager.Domain.Enums;
using PsicoManager.Domain.Exceptions;
using PsicoManager.Domain.Repositories;
using PsicoManager.Domain.Services;

namespace PsicoManager.Application.UseCases.Agenda;

/// <summary>
/// UC01a — Agendar Sessão. Fluxo do C3_Sequencia_Componentes:
/// valida conflito (RN2) → persiste sessão → dispara notificação ao paciente.
/// Extensão 4a: conflito de horário → ConflitoHorarioException (HTTP 409).
/// </summary>
public sealed class AgendarSessaoUseCase : IAgendarSessaoUseCase
{
    private readonly IUsuarioCorrente _usuario;
    private readonly ISessaoRepository _sessoes;
    private readonly INotificacaoRepository _notificacoes;
    private readonly IUnitOfWork _uow;

    public AgendarSessaoUseCase(
        IUsuarioCorrente usuario, ISessaoRepository sessoes,
        INotificacaoRepository notificacoes, IUnitOfWork uow)
    {
        _usuario = usuario;
        _sessoes = sessoes;
        _notificacoes = notificacoes;
        _uow = uow;
    }

    public async Task<SessaoResponseDto> ExecutarAsync(
        AgendarSessaoRequest request, CancellationToken ct = default)
    {
        if (_usuario.Perfil != PerfilUsuario.Psicologo)
            throw new ConflitoAcessoException("Apenas psicólogos podem agendar sessões.");

        var tipo = Enum.Parse<TipoSessao>(
            request.Tipo.Equals("teleconsulta", StringComparison.OrdinalIgnoreCase)
                ? nameof(TipoSessao.Teleconsulta) : nameof(TipoSessao.Presencial));

        // RN2 — validação de conflito de horário (ConflictoHorarioValidator).
        var conflitos = await _sessoes.BuscarConflitosAsync(
            _usuario.UsuarioId, request.DataHoraUtc, request.DuracaoMin, ct);
        if (conflitos.Count > 0)
            throw new ConflitoHorarioException();

        // Persiste a sessão (status='agendada').
        var sessao = new Sessao(_usuario.UsuarioId, request.PacienteId,
            request.DataHoraUtc, tipo, request.DuracaoMin);
        await _sessoes.AdicionarAsync(sessao, ct);

        // Aciona módulo de Notificações (UC05) — confirmação ao paciente.
        var notificacao = new Notificacao(
            sessao.Id, request.PacienteId, PerfilUsuario.Paciente,
            tipo: "confirmacao_agendamento", canal: CanalNotificacao.Whatsapp,
            agendadoParaUtc: DateTime.UtcNow);
        await _notificacoes.AdicionarAsync(notificacao, ct);

        await _uow.CommitAsync(ct);

        return new SessaoResponseDto(
            sessao.Id, sessao.Status.ToString().ToLowerInvariant(),
            sessao.DataHora, tipo.ToString().ToLowerInvariant(),
            ConfirmacaoEnviada: true);
    }
}
