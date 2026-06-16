using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PsicoManager.Infrastructure.Persistence;

/// <summary>
/// Factory para uso em design-time (dotnet ef migrations / database update).
/// Lê a connection string da variável de ambiente PSICOMANAGER_DB ou usa o
/// padrão local. Necessária porque o DbContext fica numa lib sem host próprio.
/// </summary>
public sealed class PsicoManagerDbContextFactory : IDesignTimeDbContextFactory<PsicoManagerDbContext>
{
    public PsicoManagerDbContext CreateDbContext(string[] args)
    {
        var conn = Environment.GetEnvironmentVariable("PSICOMANAGER_DB")
            ?? "Host=localhost;Port=5432;Database=psicomanager;Username=postgres;Password=postgres";

        var options = new DbContextOptionsBuilder<PsicoManagerDbContext>()
            .UseNpgsql(conn)
            .Options;

        return new PsicoManagerDbContext(options);
    }
}
