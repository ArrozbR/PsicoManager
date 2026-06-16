using PsicoManager.Domain.Enums;

namespace PsicoManager.Models;

// ---------- Dashboard ----------
public record DashboardVm(
    string NomePsicologo,
    int TotalPacientes,
    int SessoesHoje,
    int SessoesAgendadas,
    decimal FaturamentoMes,
    decimal RecebidoMes,
    decimal EmAberto,
    double TaxaPresenca,
    IReadOnlyList<ProximaSessaoVm> ProximasSessoes,
    IReadOnlyList<AtividadeVm> AtividadesRecentes);

public record ProximaSessaoVm(string Paciente, DateTime DataHora, string Tipo, string Status);
public record AtividadeVm(DateTime Quando, string Descricao, string Tipo);

// ---------- Agenda ----------
public record AgendaVm(IReadOnlyList<SessaoLinhaVm> Sessoes);
public record SessaoLinhaVm(
    Guid Id, string Paciente, DateTime DataHora, short DuracaoMin,
    string Tipo, string Status);

// ---------- Prontuário ----------
public record ProntuarioListaVm(IReadOnlyList<PacienteProntuarioVm> Pacientes);
public record PacienteProntuarioVm(Guid PacienteId, string Nome, int QtdEvolucoes, DateTime? UltimaEvolucao);

public record ProntuarioDetalheVm(
    string Paciente,
    Guid PacienteId,
    IReadOnlyList<EvolucaoLinhaVm> Evolucoes);
public record EvolucaoLinhaVm(DateTime DataHora, string Texto, string Status, DateTime TravarEm);

// ---------- Financeiro ----------
public record FinanceiroVm(
    decimal TotalFaturado,
    decimal TotalRecebido,
    decimal EmAberto,
    decimal Inadimplencia,
    double PercInadimplencia,
    IReadOnlyList<CobrancaLinhaVm> Cobrancas);
public record CobrancaLinhaVm(
    Guid Id, string Paciente, decimal Valor, string Status, DateOnly DataLancamento);

// ---------- Pacientes ----------
public record PacienteLinhaVm(Guid Id, string Nome, string? Telefone, string? Whatsapp, bool Ativo);
