using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PsicoManager.Domain.Entities;

namespace PsicoManager.Infrastructure.Persistence.Configurations;

/// <summary>
/// RNF04 — Log de auditoria imutável. A coluna é configurada como append-only;
/// a proteção contra UPDATE/DELETE é garantida em nível de banco por trigger
/// criada na migration (ver MigrationSql.LogAuditoriaTrigger).
/// </summary>
public class LogAuditoriaConfiguration : IEntityTypeConfiguration<LogAuditoria>
{
    public void Configure(EntityTypeBuilder<LogAuditoria> b)
    {
        b.ToTable("log_auditoria");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("id");
        b.Property(x => x.UsuarioId).HasColumnName("usuario_id").IsRequired();
        b.Property(x => x.Perfil).HasColumnName("perfil").HasConversion<string>().HasMaxLength(15);
        b.Property(x => x.Acao).HasColumnName("acao").HasConversion<string>().HasMaxLength(20);
        b.Property(x => x.Entidade).HasColumnName("entidade").HasConversion<string>().HasMaxLength(30);
        b.Property(x => x.EntidadeId).HasColumnName("entidade_id").IsRequired();
        b.Property(x => x.DataHora).HasColumnName("data_hora");
        b.Property(x => x.IpOrigem).HasColumnName("ip_origem").HasMaxLength(45).IsRequired();
        b.Property(x => x.Resultado).HasColumnName("resultado").HasConversion<string>().HasMaxLength(10);
        b.Property(x => x.CriadoEm).HasColumnName("criado_em");
        b.HasIndex(x => x.EntidadeId);
    }
}
