using System.Reflection;
using PsicoManager.Domain.Entities;
using PsicoManager.Domain.Enums;

namespace PsicoManager.Data;

/// <summary>
/// Repositório em memória com dados mockados. Substitui o banco real
/// (PostgreSQL) para permitir execução imediata em localhost. Singleton:
/// os dados persistem enquanto a aplicação está no ar.
/// </summary>
public sealed class MockDataStore
{
    public Psicologo Psicologo { get; }
    public List<Paciente> Pacientes { get; } = new();
    public List<Sessao> Sessoes { get; } = new();
    public List<Cobranca> Cobrancas { get; } = new();
    public List<EvolucaoClinica> Evolucoes { get; } = new();
    public List<ProntuarioClinico> Prontuarios { get; } = new();

    public MockDataStore()
    {
        // ---- Psicólogo logado (mock) ----
        Psicologo = new Psicologo(
            nome: "Dra. Ana Beatriz Costa",
            crp: "GO/012345",
            email: "ana.costa@psicomanager.app",
            senhaHash: "$2a$11$mockhashmockhashmockhashmock",
            telefone: "(62) 99999-0001");

        // ---- Pacientes ----
        var pacientesSeed = new[]
        {
            ("Mariana Oliveira",  "(62) 98888-1010", new DateOnly(1992, 3, 14)),
            ("Lucas Pereira",     "(62) 98888-2020", new DateOnly(1988, 7, 2)),
            ("Carla Mendes",      "(62) 98888-3030", new DateOnly(1995, 11, 28)),
            ("Rafael Souza",      "(62) 98888-4040", new DateOnly(1990, 1, 9)),
            ("Beatriz Lima",      "(62) 98888-5050", new DateOnly(2000, 5, 21)),
        };

        foreach (var (nome, tel, nasc) in pacientesSeed)
        {
            var p = new Paciente(Psicologo.Id, nome,
                dataNascimento: nasc, telefone: tel, whatsapp: tel);
            Pacientes.Add(p);
            var pront = new ProntuarioClinico(p.Id, Psicologo.Id);
            Prontuarios.Add(pront);
        }

        SeedSessoesECobrancas();
        SeedEvolucoes();
    }

    private void SeedSessoesECobrancas()
    {
        var rnd = new Random(42);
        var agora = DateTime.UtcNow;

        // Sessões passadas (realizadas/faltas) — montadas via reflection p/ driblar
        // a validação de "data no passado" do construtor de domínio.
        for (int dia = 30; dia >= 1; dia -= 2)
        {
            var paciente = Pacientes[rnd.Next(Pacientes.Count)];
            var data = agora.AddDays(-dia).Date.AddHours(9 + rnd.Next(8));
            var tipo = rnd.Next(4) == 0 ? TipoSessao.Teleconsulta : TipoSessao.Presencial;

            var sessao = CriarSessaoComData(paciente.Id, data, tipo);
            var sorteio = rnd.Next(10);
            if (sorteio < 7) SetStatus(sessao, StatusSessao.Realizada);
            else if (sorteio < 9) SetStatus(sessao, StatusSessao.Falta);
            else SetStatus(sessao, StatusSessao.Cancelada);
            Sessoes.Add(sessao);

            // Cobrança para sessões realizadas
            if (sessao.Status == StatusSessao.Realizada)
            {
                var valor = 150m + rnd.Next(0, 4) * 50;
                var cobranca = new Cobranca(sessao.Id, Psicologo.Id, paciente.Id, valor);
                var st = rnd.Next(10);
                if (st < 6) cobranca.ConfirmarPagamento();
                else if (st >= 8) cobranca.MarcarInadimplente();
                Cobrancas.Add(cobranca);
            }
        }

        // Sessões futuras (agendadas) — construtor normal.
        for (int dia = 1; dia <= 14; dia += 1)
        {
            if (rnd.Next(10) < 4) continue;
            var paciente = Pacientes[rnd.Next(Pacientes.Count)];
            var data = agora.AddDays(dia).Date.AddHours(9 + rnd.Next(8));
            var tipo = rnd.Next(4) == 0 ? TipoSessao.Teleconsulta : TipoSessao.Presencial;
            var sessao = new Sessao(Psicologo.Id, paciente.Id, data, tipo);
            Sessoes.Add(sessao);
        }
    }

    private void SeedEvolucoes()
    {
        var textos = new[]
        {
            "Paciente relatou melhora no quadro de ansiedade ao longo da semana. Aplicadas técnicas de respiração diafragmática.",
            "Sessão focada em reestruturação cognitiva. Identificados pensamentos automáticos relacionados ao ambiente de trabalho.",
            "Trabalho com exposição gradual a situações sociais. Paciente demonstrou maior autoconfiança.",
            "Acompanhamento de processo de luto. Paciente em fase de aceitação, com avanços significativos.",
        };
        var rnd = new Random(7);
        foreach (var pront in Prontuarios.Take(3))
        {
            var qtd = 1 + rnd.Next(3);
            for (int i = 0; i < qtd; i++)
            {
                var ev = new EvolucaoClinica(pront.Id, Psicologo.Id,
                    textoEnc: textos[rnd.Next(textos.Length)]);
                Evolucoes.Add(ev);
                pront.RegistrarMovimentoEvolucao(ev.DataHora);
            }
        }
    }

    // ---- Helpers de reflection p/ dados históricos ----
    // As propriedades têm setter privado; acessamos via o setter não-público.
    private Sessao CriarSessaoComData(Guid pacienteId, DateTime data, TipoSessao tipo)
    {
        var sessao = new Sessao(Psicologo.Id, pacienteId, DateTime.UtcNow.AddDays(1), tipo);
        SetPropriedadePrivada(sessao, nameof(Sessao.DataHora), data);
        return sessao;
    }

    private static void SetStatus(Sessao sessao, StatusSessao status)
        => SetPropriedadePrivada(sessao, nameof(Sessao.Status), status);

    private static void SetPropriedadePrivada(object alvo, string propriedade, object valor)
    {
        var tipo = alvo.GetType();
        // 1) tenta o setter privado da propriedade
        var setter = tipo.GetProperty(propriedade)?.GetSetMethod(nonPublic: true);
        if (setter is not null)
        {
            setter.Invoke(alvo, new[] { valor });
            return;
        }
        // 2) fallback: escreve direto no backing field gerado pelo compilador
        var field = tipo.GetField($"<{propriedade}>k__BackingField",
            BindingFlags.Instance | BindingFlags.NonPublic);
        field?.SetValue(alvo, valor);
    }

    public string NomePaciente(Guid pacienteId)
        => Pacientes.FirstOrDefault(p => p.Id == pacienteId)?.Nome ?? "—";
}
