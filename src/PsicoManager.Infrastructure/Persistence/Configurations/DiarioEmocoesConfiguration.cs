using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PsicoManager.Domain.Entities;

namespace PsicoManager.Infrastructure.Persistence.Configurations;

public class DiarioEmocoesConfiguration : IEntityTypeConfiguration<DiarioEmocoes>
{
    public void Configure(EntityTypeBuilder<DiarioEmocoes> b)
    {
        b.ToTable("diario_emocoes");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("id");
        b.Property(x => x.PacienteId).HasColumnName("paciente_id").IsRequired();
        b.Property(x => x.Compartilhado).HasColumnName("compartilhado").HasDefaultValue(false);
        b.Property(x => x.CriadoEm).HasColumnName("criado_em");
        b.HasIndex(x => x.PacienteId).IsUnique();
        b.HasMany(x => x.Registros).WithOne().HasForeignKey(r => r.DiarioId);
    }
}
