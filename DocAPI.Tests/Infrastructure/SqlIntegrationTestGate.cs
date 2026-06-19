using DocAPI.Infrastructure.SqlDb;
using Microsoft.Data.SqlClient;

namespace DocAPI.Tests.Infrastructure;

/// <summary>
/// Shared skip gate for SQL integration tests. Distinguishes missing credentials from unreachable SQL.
/// </summary>
public static class SqlIntegrationTestGate
{
    public static async Task<string?> GetSkipReasonAsync()
    {
        var connectionString = SqlConnectionResolver.ResolveConnectionString();
        if (connectionString is null)
            return SqlConnectionResolver.GetSetupHint();

        try
        {
            await using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            return null;
        }
        catch (SqlException ex) when (ex.Number is 18456 or 4060)
        {
            return "SQL login or database open failed — SA_PASSWORD likely does not match the docorgano-sql Docker volume. Align repo-root .env with the volume password or reset with docker compose down -v.";
        }
        catch (SqlException)
        {
            return "SQL Server rejected the connection — verify SA_PASSWORD in .env matches the container volume and run dotnet ef database update.";
        }
        catch (Exception)
        {
            return "Docker SQL is not reachable on 127.0.0.1:1433 — start docorgano-sql with docker compose up -d.";
        }
    }
}
