using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PsicoManager.Domain.Entities;

namespace PsicoManager.Infrastructure.Persistence.Configurations;

public class PsicologoConfiguration : IEntityTypeConfiguration<Psicologo>
{
    public void Configure(EntityTypeBuilder<Psicologo> b)
    {
        b.ToTable("psicologo");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("id");
        b.Property(x => x.Nome).HasColumnName("nome").HasMaxLength(150).IsRequired();
        b.Property(x => x.Crp).HasColumnName("crp").HasMaxLength(10).IsRequired();
        b.Property(x => x.Email).HasColumnName("email").HasMaxLength(255).IsRequired();
        b.Property(x => x.Telefone).HasColumnName("telefone").HasMaxLength(20);
        b.Property(x => x.SenhaHash).HasColumnName("senha_hash").HasMaxLength(255).IsRequired();
        b.Property(x => x.MfaAtivo).HasColumnName("mfa_ativo").HasDefaultValue(false);
        b.Property(x => x.CriadoEm).HasColumnName("criado_em");
        b.Property(x => x.Ativo).HasColumnName("ativo").HasDefaultValue(true);
        b.HasIndex(x => x.Crp).IsUnique();
        b.HasIndex(x => x.Email).IsUnique();
        b.HasMany(x => x.Pacientes).WithOne().HasForeignKey(p => p.PsicologoId);
    }
}
