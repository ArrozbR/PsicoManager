using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PsicoManager.Domain.Entities;

namespace PsicoManager.Infrastructure.Persistence.Configurations;

public class VinculoConfiguration : IEntityTypeConfiguration<VinculoTerapeutico>
{
    public void Configure(EntityTypeBuilder<VinculoTerapeutico> b)
    {
        b.ToTable("vinculo_terapeutico");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("id");
        b.Property(x => x.PsicologoId).HasColumnName("psicologo_id").IsRequired();
        b.Property(x => x.PacienteId).HasColumnName("paciente_id").IsRequired();
        b.Property(x => x.Ativo).HasColumnName("ativo").HasDefaultValue(true);
        b.Property(x => x.EncerradoEm).HasColumnName("encerrado_em");
        b.Property(x => x.CriadoEm).HasColumnName("criado_em");
        b.HasIndex(x => new { x.PsicologoId, x.PacienteId, x.Ativo });
    }
}
