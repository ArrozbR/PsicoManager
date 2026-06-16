using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PsicoManager.Domain.Entities;

namespace PsicoManager.Infrastructure.Persistence.Configurations;

public class ProntuarioConfiguration : IEntityTypeConfiguration<ProntuarioClinico>
{
    public void Configure(EntityTypeBuilder<ProntuarioClinico> b)
    {
        b.ToTable("prontuario_clinico");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("id");
        b.Property(x => x.PacienteId).HasColumnName("paciente_id").IsRequired();
        b.Property(x => x.PsicologoId).HasColumnName("psicologo_id").IsRequired();
        b.Property(x => x.Status).HasColumnName("status").HasConversion<string>().HasMaxLength(15);
        b.Property(x => x.CriadoEm).HasColumnName("criado_em");
        b.Property(x => x.UltimaEvolucaoEm).HasColumnName("ultima_evolucao_em");
        b.HasIndex(x => x.PacienteId).IsUnique(); // 1 prontuário/paciente
        b.HasMany(x => x.Evolucoes).WithOne().HasForeignKey(e => e.ProntuarioId);
    }
}
