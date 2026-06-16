using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PsicoManager.Domain.Entities;

namespace PsicoManager.Infrastructure.Persistence.Configurations;

public class AnexoConfiguration : IEntityTypeConfiguration<Anexo>
{
    public void Configure(EntityTypeBuilder<Anexo> b)
    {
        b.ToTable("anexo");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("id");
        b.Property(x => x.EvolucaoId).HasColumnName("evolucao_id").IsRequired();
        b.Property(x => x.Nome).HasColumnName("nome").HasMaxLength(255).IsRequired();
        b.Property(x => x.Tipo).HasColumnName("tipo").HasMaxLength(50).IsRequired();
        b.Property(x => x.UrlStorage).HasColumnName("url_storage").HasMaxLength(512).IsRequired();
        b.Property(x => x.Tamanho).HasColumnName("tamanho");
        b.Property(x => x.CriadoEm).HasColumnName("criado_em");
    }
}
