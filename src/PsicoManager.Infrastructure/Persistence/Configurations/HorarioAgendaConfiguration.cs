using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PsicoManager.Domain.Entities;

namespace PsicoManager.Infrastructure.Persistence.Configurations;

public class HorarioAgendaConfiguration : IEntityTypeConfiguration<HorarioAgenda>
{
    public void Configure(EntityTypeBuilder<HorarioAgenda> b)
    {
        b.ToTable("horario_agenda");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("id");
        b.Property(x => x.PsicologoId).HasColumnName("psicologo_id").IsRequired();
        b.Property(x => x.Data).HasColumnName("data").IsRequired();
        b.Property(x => x.HoraInicio).HasColumnName("hora_inicio").IsRequired();
        b.Property(x => x.DuracaoMin).HasColumnName("duracao_min").HasDefaultValue((short)50);
        b.Property(x => x.Tipo).HasColumnName("tipo").HasMaxLength(15);
        b.Property(x => x.Recorrencia).HasColumnName("recorrencia").HasMaxLength(15);
        b.Property(x => x.DataTermino).HasColumnName("data_termino");
        b.Property(x => x.SessaoId).HasColumnName("sessao_id");
        b.Property(x => x.CriadoEm).HasColumnName("criado_em");
    }
}
