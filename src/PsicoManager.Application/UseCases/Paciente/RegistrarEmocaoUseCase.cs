using PsicoManager.Application.Abstractions;
using PsicoManager.Application.DTOs;
using PsicoManager.Domain.Entities;
using PsicoManager.Domain.Exceptions;
using PsicoManager.Domain.Repositories;
using PsicoManager.Domain.Services;

namespace PsicoManager.Application.UseCases.Paciente;

/// <summary>
/// UC09 — Registrar Diário de Emoções. Humor 1–10; texto opcional
/// criptografado AES-256 em repouso (RNF02).
/// </summary>
public sealed class RegistrarEmocaoUseCase : IRegistrarEmocaoUseCase
{
    private readonly IUsuarioCorrente _usuario;
    private readonly IDiarioEmocoesRepository _diarios;
    private readonly ICriptografiaService _cripto;
    private readonly IUnitOfWork _uow;

    public RegistrarEmocaoUseCase(
        IUsuarioCorrente usuario, IDiarioEmocoesRepository diarios,
        ICriptografiaService cripto, IUnitOfWork uow)
    {
        _usuario = usuario;
        _diarios = diarios;
        _cripto = cripto;
        _uow = uow;
    }

    public async Task<Guid> ExecutarAsync(RegistrarEmocaoRequest request, CancellationToken ct = default)
    {
        var diario = await _diarios.ObterPorPacienteAsync(_usuario.UsuarioId, ct)
            ?? throw new RegraDeNegocioException("Diário do paciente não encontrado.");

        var textoEnc = string.IsNullOrWhiteSpace(request.Texto)
            ? null : _cripto.Criptografar(request.Texto);

        var registro = new RegistroEmocao(diario.Id, request.Humor, textoEnc);
        diario.AdicionarRegistro(registro);
        _diarios.Atualizar(diario);

        await _uow.CommitAsync(ct);
        return registro.Id;
    }
}
