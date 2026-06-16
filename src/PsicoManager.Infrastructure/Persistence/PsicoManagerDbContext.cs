using Microsoft.EntityFrameworkCore;
using PsicoManager.Domain.Entities;

namespace PsicoManager.Infrastructure.Persistence;

/// <summary>
/// Contexto do banco CLÍNICO (prontuários, sessões, cobranças). Separado do
/// banco de autenticação por restrição de projeto (C1 — Implantação).
/// PostgreSQL em produção; convenções EOBD aplicadas via Configurations.
/// </summary>
public class PsicoManagerDbContext : DbContext
{
    public PsicoManagerDbContext(DbContextOptions<PsicoManagerDbContext> options) : base(options) { }

    public DbSet<Psicologo> Psicologos => Set<Psicologo>();
    public DbSet<Paciente> Pacientes => Set<Paciente>();
    public DbSet<Sessao> Sessoes => Set<Sessao>();
    public DbSet<HorarioAgenda> HorariosAgenda => Set<HorarioAgenda>();
    public DbSet<ProntuarioClinico> Prontuarios => Set<ProntuarioClinico>();
    public DbSet<EvolucaoClinica> Evolucoes => Set<EvolucaoClinica>();
    public DbSet<Anexo> Anexos => Set<Anexo>();
    public DbSet<Cobranca> Cobrancas => Set<Cobranca>();
    public DbSet<Notificacao> Notificacoes => Set<Notificacao>();
    public DbSet<LogAuditoria> LogsAuditoria => Set<LogAuditoria>();
    public DbSet<Teleconsulta> Teleconsultas => Set<Teleconsulta>();
    public DbSet<DiarioEmocoes> Diarios => Set<DiarioEmocoes>();
    public DbSet<RegistroEmocao> RegistrosEmocao => Set<RegistroEmocao>();
    public DbSet<VinculoTerapeutico> Vinculos => Set<VinculoTerapeutico>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PsicoManagerDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
