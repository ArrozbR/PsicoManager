using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PsicoManager.Domain.Entities;

namespace PsicoManager.Infrastructure.Persistence.Configurations;

public class TeleconsultaConfiguration : IEntityTypeConfiguration<Teleconsulta>
{
    public void Configure(EntityTypeBuilder<Teleconsulta> b)
    {
        b.ToTable("teleconsulta");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("id");
        b.Property(x => x.SessaoId).HasColumnName("sessao_id").IsRequired();
        b.Property(x => x.LinkAcesso).HasColumnName("link_acesso").HasMaxLength(255).IsRequired();
        b.Property(x => x.Status).HasColumnName("status").HasConversion<string>().HasMaxLength(20);
        b.Property(x => x.DuracaoRealMin).HasColumnName("duracao_real_min");
        b.Property(x => x.LinkExpiraEm).HasColumnName("link_expira_em").IsRequired();
        b.Property(x => x.CriadoEm).HasColumnName("criado_em");
        b.HasIndex(x => x.SessaoId).IsUnique();
        b.HasIndex(x => x.LinkAcesso).IsUnique();
    }
}
