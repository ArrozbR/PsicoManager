using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PsicoManager.Domain.Entities;

namespace PsicoManager.Infrastructure.Persistence.Configurations;

public class SessaoConfiguration : IEntityTypeConfiguration<Sessao>
{
    public void Configure(EntityTypeBuilder<Sessao> b)
    {
        b.ToTable("sessao");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("id");
        b.Property(x => x.PsicologoId).HasColumnName("psicologo_id").IsRequired();
        b.Property(x => x.PacienteId).HasColumnName("paciente_id").IsRequired();
        b.Property(x => x.DataHora).HasColumnName("data_hora").IsRequired();
        b.Property(x => x.DuracaoMin).HasColumnName("duracao_min").HasDefaultValue((short)50);
        b.Property(x => x.Tipo).HasColumnName("tipo").HasConversion<string>().HasMaxLength(15);
        b.Property(x => x.Status).HasColumnName("status").HasConversion<string>().HasMaxLength(15);
        b.Property(x => x.MotivoCancelamento).HasColumnName("motivo_cancelamento");
        b.Property(x => x.CriadoEm).HasColumnName("criado_em");
        b.Property(x => x.AtualizadoEm).HasColumnName("atualizado_em");
        b.HasIndex(x => new { x.PsicologoId, x.DataHora });
    }
}
