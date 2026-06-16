using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PsicoManager.Domain.Entities;

namespace PsicoManager.Infrastructure.Persistence.Configurations;

public class PacienteConfiguration : IEntityTypeConfiguration<Paciente>
{
    public void Configure(EntityTypeBuilder<Paciente> b)
    {
        b.ToTable("paciente");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("id");
        b.Property(x => x.PsicologoId).HasColumnName("psicologo_id").IsRequired();
        b.Property(x => x.Nome).HasColumnName("nome").HasMaxLength(150).IsRequired();
        b.Property(x => x.Cpf).HasColumnName("cpf").HasMaxLength(11);
        b.Property(x => x.DataNascimento).HasColumnName("data_nascimento");
        b.Property(x => x.Email).HasColumnName("email").HasMaxLength(255);
        b.Property(x => x.Telefone).HasColumnName("telefone").HasMaxLength(20);
        b.Property(x => x.Whatsapp).HasColumnName("whatsapp").HasMaxLength(20);
        b.Property(x => x.CriadoEm).HasColumnName("criado_em");
        b.Property(x => x.Ativo).HasColumnName("ativo").HasDefaultValue(true);
        b.HasIndex(x => new { x.PsicologoId, x.Email }).IsUnique();
    }
}
