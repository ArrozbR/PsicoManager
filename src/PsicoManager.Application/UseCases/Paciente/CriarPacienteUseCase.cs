using PsicoManager.Application.Abstractions;
using PsicoManager.Application.DTOs;
using PsicoManager.Domain.Entities;
using PsicoManager.Domain.Enums;
using PsicoManager.Domain.Exceptions;
using PsicoManager.Domain.Repositories;

namespace PsicoManager.Application.UseCases.Paciente;

using PacienteEntity = Domain.Entities.Paciente;

/// <summary>
/// Cria paciente, prontuário (1:1) e vínculo terapêutico ativo (RN1) numa
/// única transação. Também cria o diário de emoções do paciente.
/// </summary>
public sealed class CriarPacienteUseCase : ICriarPacienteUseCase
{
    private readonly IUsuarioCorrente _usuario;
    private readonly IPacienteRepository _pacientes;
    private readonly IProntuarioRepository _prontuarios;
    private readonly IVinculoRepository _vinculos;
    private readonly IDiarioEmocoesRepository _diarios;
    private readonly IUnitOfWork _uow;

    public CriarPacienteUseCase(
        IUsuarioCorrente usuario, IPacienteRepository pacientes, IProntuarioRepository prontuarios,
        IVinculoRepository vinculos, IDiarioEmocoesRepository diarios, IUnitOfWork uow)
    {
        _usuario = usuario;
        _pacientes = pacientes;
        _prontuarios = prontuarios;
        _vinculos = vinculos;
        _diarios = diarios;
        _uow = uow;
    }

    public async Task<PacienteDto> ExecutarAsync(CriarPacienteRequest request, CancellationToken ct = default)
    {
        if (_usuario.Perfil != PerfilUsuario.Psicologo)
            throw new ConflitoAcessoException("Apenas psicólogos cadastram pacientes.");

        var paciente = new PacienteEntity(_usuario.UsuarioId, request.Nome, request.Cpf,
            request.DataNascimento, request.Email, request.Telefone, request.Whatsapp);
        await _pacientes.AdicionarAsync(paciente, ct);

        var prontuario = new ProntuarioClinico(paciente.Id, _usuario.UsuarioId);
        await _prontuarios.AdicionarAsync(prontuario, ct);

        var vinculo = new VinculoTerapeutico(_usuario.UsuarioId, paciente.Id);
        await _vinculos.AdicionarAsync(vinculo, ct);

        var diario = new DiarioEmocoes(paciente.Id);
        await _diarios.AdicionarAsync(diario, ct);

        await _uow.CommitAsync(ct);

        return new PacienteDto(paciente.Id, paciente.Nome, paciente.Email, paciente.Whatsapp, paciente.Ativo);
    }
}
