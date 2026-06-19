namespace DocAPI.Infrastructure.SqlDb;

/// <summary>
/// Resolves the local SQL Server connection string from environment variables or the repo-root .env file.
/// Priority: DOCORGANO_TEST_CONNECTION → SA_PASSWORD (env or .env) → incomplete config templates are ignored.
/// </summary>
public static class SqlConnectionResolver
{
    public const string DefaultServer = "127.0.0.1,1433";
    public const string DefaultDatabase = "DocDb";

    public static string? ResolveConnectionString()
    {
        var configured = Environment.GetEnvironmentVariable("DOCORGANO_TEST_CONNECTION");
        if (!string.IsNullOrWhiteSpace(configured))
            return configured.Trim();

        var envValues = LoadRepoEnvFile();
        var password = FirstNonEmpty(
            Environment.GetEnvironmentVariable("SA_PASSWORD"),
            envValues.GetValueOrDefault("SA_PASSWORD"));

        if (string.IsNullOrWhiteSpace(password))
            return null;

        var database = FirstNonEmpty(
            Environment.GetEnvironmentVariable("DB_NAME"),
            envValues.GetValueOrDefault("DB_NAME"),
            DefaultDatabase)!;

        return Build(password, database);
    }

    public static string Build(string password, string database = DefaultDatabase) =>
        $"Server={DefaultServer};Database={database};User Id=sa;Password={password};Encrypt=False;TrustServerCertificate=True;";

    public static string GetSetupHint() =>
        "Set SA_PASSWORD in .env (copy from .env.example), export it in the shell, or run scripts/load-env.ps1 before dotnet test/ef/run.";

    private static Dictionary<string, string> LoadRepoEnvFile()
    {
        var root = FindRepoRoot();
        if (root is null)
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        var envPath = Path.Combine(root, ".env");
        if (!File.Exists(envPath))
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var line in File.ReadAllLines(envPath))
        {
            if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith('#'))
                continue;

            var separatorIndex = line.IndexOf('=');
            if (separatorIndex <= 0)
                continue;

            var key = line[..separatorIndex].Trim();
            var value = line[(separatorIndex + 1)..].Trim().Trim('"');
            if (!string.IsNullOrEmpty(key))
                values[key] = value;
        }

        return values;
    }

    private static string? FindRepoRoot()
    {
        var current = new DirectoryInfo(Directory.GetCurrentDirectory());
        while (current is not null)
        {
            if (File.Exists(Path.Combine(current.FullName, "docker-compose.yml")))
                return current.FullName;

            current = current.Parent;
        }

        return null;
    }

    private static string? FirstNonEmpty(params string?[] values)
    {
        foreach (var value in values)
        {
            if (!string.IsNullOrWhiteSpace(value))
                return value;
        }

        return null;
    }
}
