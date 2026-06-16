using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PsicoManager.Domain.Entities;

namespace PsicoManager.Infrastructure.Persistence.Configurations;

public class CobrancaConfiguration : IEntityTypeConfiguration<Cobranca>
{
    public void Configure(EntityTypeBuilder<Cobranca> b)
    {
        b.ToTable("cobranca");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("id");
        b.Property(x => x.SessaoId).HasColumnName("sessao_id").IsRequired();
        b.Property(x => x.PsicologoId).HasColumnName("psicologo_id").IsRequired();
        b.Property(x => x.PacienteId).HasColumnName("paciente_id").IsRequired();
        b.Property(x => x.Valor).HasColumnName("valor").HasColumnType("numeric(10,2)");
        b.Property(x => x.Status).HasColumnName("status").HasConversion<string>().HasMaxLength(15);
        b.Property(x => x.FormaPagamento).HasColumnName("forma_pagamento").HasConversion<string>().HasMaxLength(10);
        b.Property(x => x.PixCodigo).HasColumnName("pix_codigo").HasMaxLength(512);
        b.Property(x => x.PixValidade).HasColumnName("pix_validade");
        b.Property(x => x.DataLancamento).HasColumnName("data_lancamento");
        b.Property(x => x.DataPagamento).HasColumnName("data_pagamento");
        b.Property(x => x.CriadoEm).HasColumnName("criado_em");
        b.HasIndex(x => x.SessaoId).IsUnique(); // 1 cobrança/sessão
        b.ToTable(t => t.HasCheckConstraint("ck_cobranca_valor_positivo", "valor > 0"));
    }
}
