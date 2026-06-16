using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PsicoManager.Domain.Entities;

namespace PsicoManager.Infrastructure.Persistence.Configurations;

public class NotificacaoConfiguration : IEntityTypeConfiguration<Notificacao>
{
    public void Configure(EntityTypeBuilder<Notificacao> b)
    {
        b.ToTable("notificacao");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("id");
        b.Property(x => x.SessaoId).HasColumnName("sessao_id").IsRequired();
        b.Property(x => x.DestinatarioId).HasColumnName("destinatario_id").IsRequired();
        b.Property(x => x.TipoDest).HasColumnName("tipo_dest").HasConversion<string>().HasMaxLength(15);
        b.Property(x => x.Tipo).HasColumnName("tipo").HasMaxLength(40).IsRequired();
        b.Property(x => x.Canal).HasColumnName("canal").HasConversion<string>().HasMaxLength(15);
        b.Property(x => x.Status).HasColumnName("status").HasConversion<string>().HasMaxLength(10);
        b.Property(x => x.Tentativas).HasColumnName("tentativas").HasDefaultValue((short)0);
        b.Property(x => x.AgendadoPara).HasColumnName("agendado_para").IsRequired();
        b.Property(x => x.EnviadoEm).HasColumnName("enviado_em");
        b.Property(x => x.CriadoEm).HasColumnName("criado_em");
        b.ToTable(t => t.HasCheckConstraint("ck_notificacao_tentativas", "tentativas <= 3"));
    }
}
