using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PsicoManager.Domain.Entities;

namespace PsicoManager.Infrastructure.Persistence.Configurations;

public class EvolucaoConfiguration : IEntityTypeConfiguration<EvolucaoClinica>
{
    public void Configure(EntityTypeBuilder<EvolucaoClinica> b)
    {
        b.ToTable("evolucao_clinica");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("id");
        b.Property(x => x.ProntuarioId).HasColumnName("prontuario_id").IsRequired();
        b.Property(x => x.SessaoId).HasColumnName("sessao_id");
        b.Property(x => x.PsicologoId).HasColumnName("psicologo_id").IsRequired();
        b.Property(x => x.TextoEnc).HasColumnName("texto_enc").IsRequired(); // AES-256 em repouso
        b.Property(x => x.DataHora).HasColumnName("data_hora");
        b.Property(x => x.Status).HasColumnName("status").HasConversion<string>().HasMaxLength(15);
        b.Property(x => x.PrazoTravaH).HasColumnName("prazo_trava_h").HasDefaultValue((short)24);
        b.Property(x => x.TravarEm).HasColumnName("travar_em");
        b.Property(x => x.CriadoEm).HasColumnName("criado_em");
        b.HasMany(x => x.Anexos).WithOne().HasForeignKey(a => a.EvolucaoId);
    }
}
