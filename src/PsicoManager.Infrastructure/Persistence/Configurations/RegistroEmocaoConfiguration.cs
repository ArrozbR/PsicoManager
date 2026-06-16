using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PsicoManager.Domain.Entities;

namespace PsicoManager.Infrastructure.Persistence.Configurations;

public class RegistroEmocaoConfiguration : IEntityTypeConfiguration<RegistroEmocao>
{
    public void Configure(EntityTypeBuilder<RegistroEmocao> b)
    {
        b.ToTable("registro_emocao");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("id");
        b.Property(x => x.DiarioId).HasColumnName("diario_id").IsRequired();
        b.Property(x => x.DataHora).HasColumnName("data_hora");
        b.Property(x => x.Humor).HasColumnName("humor").IsRequired();
        b.Property(x => x.TextoEnc).HasColumnName("texto_enc");
        b.Property(x => x.EditavelAte).HasColumnName("editavel_ate").IsRequired();
        b.Property(x => x.CriadoEm).HasColumnName("criado_em");
        b.ToTable(t => t.HasCheckConstraint("ck_registro_humor", "humor >= 1 AND humor <= 10"));
    }
}
