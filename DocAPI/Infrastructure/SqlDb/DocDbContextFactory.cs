using DocAPI.Infrastructure.SqlDb.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DocAPI.Infrastructure.SqlDb;

public class DocDbContextFactory : IDesignTimeDbContextFactory<DocDbContext>
{
    public DocDbContext CreateDbContext(string[] args)
    {
        var connectionString = SqlConnectionResolver.ResolveConnectionString();
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                $"EF tools require a SQL connection. {SqlConnectionResolver.GetSetupHint()}");
        }

        var options = new DbContextOptionsBuilder<DocDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        return new DocDbContext(options);
    }
}
