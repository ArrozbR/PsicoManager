using PsicoManager.Application.Abstractions;
using PsicoManager.Application.DTOs;
using PsicoManager.Domain.Entities;
using PsicoManager.Domain.Enums;
using PsicoManager.Domain.Exceptions;
using PsicoManager.Domain.Repositories;
using PsicoManager.Domain.Services;

namespace PsicoManager.Application.UseCases.Financeiro;

/// <summary>
/// UC04a — lancarCobranca(). Fluxo do C4_Sequencia_Op2_LancarCobranca:
/// verifica sessão realizada → impede cobrança duplicada (UK sessao_id) →
/// cria cobrança 'pendente' → tenta gerar Pix EMV.
///   • Gateway disponível: anexa Pix e disponibiliza no painel do paciente (UC08).
///   • Gateway indisponível: registra local + baixa manual posterior (aviso).
/// Registra no log de auditoria (RNF04).
/// </summary>
public sealed class LancarCobrancaUseCase : ILancarCobrancaUseCase
{
    private readonly IUsuarioCorrente _usuario;
    private readonly ISessaoRepository _sessoes;
    private readonly ICobrancaRepository _cobrancas;
    private readonly IGatewayPagamentoService _gateway;
    private readonly IAuditLogger _audit;
    private readonly IUnitOfWork _uow;

    public LancarCobrancaUseCase(
        IUsuarioCorrente usuario, ISessaoRepository sessoes, ICobrancaRepository cobrancas,
        IGatewayPagamentoService gateway, IAuditLogger audit, IUnitOfWork uow)
    {
        _usuario = usuario;
        _sessoes = sessoes;
        _cobrancas = cobrancas;
        _gateway = gateway;
        _audit = audit;
        _uow = uow;
    }

    public async Task<CobrancaResponseDto> ExecutarAsync(
        LancarCobrancaRequest request, CancellationToken ct = default)
    {
        if (_usuario.Perfil != PerfilUsuario.Psicologo)
            throw new ConflitoAcessoException("Apenas psicólogos podem lançar cobranças.");

        var sessao = await _sessoes.ObterPorIdAsync(request.SessaoId, ct)
            ?? throw new RegraDeNegocioException("Sessão não encontrada.");

        // verificarSessaoRealizada()
        if (sessao.Status != StatusSessao.Realizada)
            throw new RegraDeNegocioException("Só é possível cobrar sessões realizadas.");

        // verificarCobrancaExistente() — UK sessao_id (1 cobrança/sessão).
        if (await _cobrancas.ObterPorSessaoAsync(request.SessaoId, ct) is not null)
            throw new RegraDeNegocioException("Já existe cobrança para esta sessão.");

        // criar() cobrança status=PENDENTE, formaPagamento=PIX.
        var cobranca = new Cobranca(sessao.Id, sessao.PsicologoId, sessao.PacienteId,
            request.Valor, FormaPagamento.Pix);

        string? pixCopiaCola = null;
        string? aviso = null;

        if (_gateway.Disponivel)
        {
            // Caminho [Gateway disponível]: gera Pix automaticamente.
            var pix = await _gateway.GerarPixCopiaColaAsync(
                request.Valor, $"Sessão {sessao.Id}", cobranca.Id, TimeSpan.FromHours(24), ct);
            cobranca.AnexarPix(pix.CodigoEmv, pix.Validade);
            pixCopiaCola = pix.CodigoEmv;
        }
        else
        {
            // Caminho [Gateway indisponível]: registro local + baixa manual.
            aviso = "Pix indisponível — lance manualmente.";
        }

        await _cobrancas.AdicionarAsync(cobranca, ct);

        await _audit.RegistrarAsync(_usuario.UsuarioId, PerfilUsuario.Psicologo,
            AcaoAuditoria.Criacao, EntidadeAuditada.Cobranca, cobranca.Id,
            _usuario.IpOrigem, ResultadoAuditoria.Sucesso, ct);

        await _uow.CommitAsync(ct);

        return new CobrancaResponseDto(
            cobranca.Id, pixCopiaCola, cobranca.Status.ToString().ToLowerInvariant(), aviso);
    }
}
