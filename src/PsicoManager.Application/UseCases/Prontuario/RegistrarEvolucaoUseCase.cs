using PsicoManager.Application.Abstractions;
using PsicoManager.Application.DTOs;
using PsicoManager.Domain.Entities;
using PsicoManager.Domain.Enums;
using PsicoManager.Domain.Exceptions;
using PsicoManager.Domain.Repositories;
using PsicoManager.Domain.Services;

namespace PsicoManager.Application.UseCases.Prontuario;

/// <summary>
/// Implementação do Contrato de Operação CO-01: registrarEvolucao().
/// Segue o diagrama de sequência C4_Sequencia_Op1_RegistrarEvolucao.
///
/// Pré-condições (CO-01):
///   1. Psicólogo autenticado (JWT perfil 'psicologo').
///   2. Vínculo terapêutico ativo entre psicologoId e pacienteId.
///   3. Prontuário do paciente existe e está 'ativo'.
///   4. texto não nulo/vazio.
///   5. Cada arquivo é PDF/JPEG/PNG e ≤ 10 MB.
///
/// Pós-condições: nova EvolucaoClinica('editavel'); ultima_evolucao_em atualizado;
/// anexos criptografados no storage; entrada no log_auditoria (CRIACAO/SUCESSO);
/// agendamento de trava (data_hora + prazo_trava_h); texto nunca retornado em claro.
/// </summary>
public sealed class RegistrarEvolucaoUseCase : IRegistrarEvolucaoUseCase
{
    private readonly IUsuarioCorrente _usuario;
    private readonly IAcessoSigiloService _sigilo;
    private readonly IProntuarioRepository _prontuarios;
    private readonly IEvolucaoRepository _evolucoes;
    private readonly ICriptografiaService _cripto;
    private readonly IStorageService _storage;
    private readonly IAuditLogger _audit;
    private readonly IUnitOfWork _uow;

    public RegistrarEvolucaoUseCase(
        IUsuarioCorrente usuario,
        IAcessoSigiloService sigilo,
        IProntuarioRepository prontuarios,
        IEvolucaoRepository evolucoes,
        ICriptografiaService cripto,
        IStorageService storage,
        IAuditLogger audit,
        IUnitOfWork uow)
    {
        _usuario = usuario;
        _sigilo = sigilo;
        _prontuarios = prontuarios;
        _evolucoes = evolucoes;
        _cripto = cripto;
        _storage = storage;
        _audit = audit;
        _uow = uow;
    }

    public async Task<EvolucaoResponseDto> ExecutarAsync(
        RegistrarEvolucaoRequest request, CancellationToken ct = default)
    {
        var psicologoId = _usuario.UsuarioId;

        // Pré-condição 1: perfil precisa ser psicólogo.
        if (_usuario.Perfil != PerfilUsuario.Psicologo)
            throw new ConflitoAcessoException("Apenas psicólogos podem registrar evoluções.");

        // Pré-condição 4: texto não vazio (antes de qualquer acesso a dados).
        if (string.IsNullOrWhiteSpace(request.Texto))
            throw new TextoVazioException();

        // Pré-condição 5: valida metadados dos arquivos ANTES de processar.
        if (request.Arquivos is { Count: > 0 })
            foreach (var arq in request.Arquivos)
                Anexo.ValidarMetadados(arq.Tipo, arq.Tamanho);

        // Passo 1 (C4): AcessoSigiloService ANTES de qualquer acesso a dados do prontuário.
        // Lança ConflitoAcessoException (403) e loga ACESSO_NEGADO se não houver vínculo (RN1/RNF01).
        await _sigilo.ValidarAcessoAsync(psicologoId, request.PacienteId, _usuario.IpOrigem,
            EntidadeAuditada.Prontuario, request.PacienteId, ct);

        // Pré-condição 3: prontuário existe e está ativo.
        var prontuario = await _prontuarios.ObterPorPacienteAsync(request.PacienteId, ct)
            ?? throw new RegraDeNegocioException("Prontuário do paciente não encontrado.");
        if (prontuario.Status != StatusProntuario.Ativo)
            throw new RegraDeNegocioException("Prontuário não está ativo.");

        // Pós-condição 1: cria a evolução com texto criptografado (RNF02 — AES-256 em repouso).
        var textoEnc = _cripto.Criptografar(request.Texto);
        var evolucao = new EvolucaoClinica(prontuario.Id, psicologoId, textoEnc, request.SessaoId);

        // Pós-condição 3: para cada arquivo → cripto + upload + Anexo persistido.
        if (request.Arquivos is { Count: > 0 })
        {
            foreach (var arq in request.Arquivos)
            {
                var bytesCripto = _cripto.CriptografarBytes(arq.Conteudo);
                var caminho = $"prontuarios/{request.PacienteId}/{evolucao.Id}/{arq.Nome}";
                var url = await _storage.UploadAsync(bytesCripto, caminho, ct);
                evolucao.AdicionarAnexo(new Anexo(evolucao.Id, arq.Nome, arq.Tipo, url, arq.Tamanho));
            }
        }

        await _evolucoes.AdicionarAsync(evolucao, ct);

        // Pós-condição 2: atualiza ultima_evolucao_em (base de retenção 5 anos — RN3/RNF05).
        prontuario.RegistrarMovimentoEvolucao(evolucao.DataHora);
        _prontuarios.Atualizar(prontuario);

        // Pós-condição 5: o agendamento da trava está embutido em TravarEm (RNF03);
        // o TravaEdicaoJob (background) varre e sela após o prazo.

        // Pós-condição 4: log de auditoria imutável (RNF04).
        await _audit.RegistrarAsync(psicologoId, PerfilUsuario.Psicologo, AcaoAuditoria.Criacao,
            EntidadeAuditada.Evolucao, evolucao.Id, _usuario.IpOrigem,
            ResultadoAuditoria.Sucesso, ct);

        await _uow.CommitAsync(ct);

        // Saída: texto NUNCA retornado em claro (pós-condição 6).
        var anexosDto = evolucao.Anexos
            .Select(a => new AnexoDto(a.Nome, a.Tipo, a.Tamanho))
            .ToList();

        return new EvolucaoResponseDto(
            evolucao.Id, evolucao.DataHora, evolucao.Status.ToString().ToLowerInvariant(),
            evolucao.TravarEm, anexosDto);
    }
}
