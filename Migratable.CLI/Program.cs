using System.Reflection;
using Migratable.Interfaces;
using Migratable.Providers;

namespace Migratable.CLI;

class Program
{
    static void Main(string[] args)
    {
        // Version from the assembly definition.
        // Remember to update it on new releases!
        var version = Assembly.GetEntryAssembly()?.GetName().Version;

        // Instructions.
        Console.WriteLine();
        Console.WriteLine($"MIGRATABLE CLI v{version}");
        Console.WriteLine($"Built with Net {Environment.Version}");
        Console.WriteLine();
        Console.WriteLine("Usage:");
        Console.WriteLine("  migratable <db> <env_name> <migrations> <command>");
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine("  db          database; either POSTGRES or MYSQL");
        Console.WriteLine("              (always converted to uppercase)");
        Console.WriteLine("  env_name    the name of an environment variable");
        Console.WriteLine("              with a database connection string");
        Console.WriteLine("              (always converted to uppercase)");
        Console.WriteLine("  migrations  folder containing migration scripts");
        Console.WriteLine("              (https://github.com/kcartlidge/migratable)");
        Console.WriteLine("  command     migration action to perform");
        Console.WriteLine("              (always converted to lowercase)");
        Console.WriteLine();
        Console.WriteLine("Commands:");
        Console.WriteLine();
        Console.WriteLine("  -info        Show migration status");
        Console.WriteLine("  -list        List known migrations");
        Console.WriteLine();
        Console.WriteLine("  -reset       Remove all migrations");
        Console.WriteLine("  -latest      Apply new migrations");
        Console.WriteLine("  -next        Roll forward one migration");
        Console.WriteLine("  -back        Roll backward one migration");
        Console.WriteLine("  -target <0>  Target specific migration");
        Console.WriteLine();

        try
        {
            // Sanity check.
            if (args.Length != 4)
                throw new Exception($"Expected 4 command arguments, got {args.Length}.");

            // Extract command arguments.
            // Fixed and ordered expectations so no fancy parser needed.
            var (dbType, envVar, folder, cmd) = (args[0], args[1], args[2], args[3]);
            dbType = dbType.ToUpperInvariant();
            envVar = envVar.ToUpperInvariant();
            cmd = cmd.ToLowerInvariant();
            Console.WriteLine("Requested:");
            Console.WriteLine();
            Console.WriteLine($"  Database     {dbType}");
            Console.WriteLine($"  Environment  {envVar}");
            Console.WriteLine($"  Migrations   {folder}");
            Console.WriteLine($"  Command      {cmd}");

            // Get the database connection string from the environment.
            var connString = Environment.GetEnvironmentVariable(envVar) ?? "";
            if (string.IsNullOrWhiteSpace(connString)) throw new Exception("No connection string!");

            // Get the correct Migratable instances.
            IProvider p;
            switch (dbType)
            {
                case "POSTGRES":
                    p = new PostgresProvider(connString);
                    break;
                case "MYSQL":
                    p = new MySqlProvider(connString);
                    break;
                default:
                    throw new Exception("Unsupported database type!");
            }
            IMigrator m = new Migrator(p);

            // Describe the connection and current migration status.
            Console.WriteLine();
            Console.WriteLine("Using " + m.Describe());
            var v = m.GetVersion();
            var sql = m.LoadMigrations(folder);
            var max = sql.Any()
                ? sql.Max(x => x.Value.Version)
                : 0;
            Console.WriteLine($"Currently at version {v} (of {max})");
            Console.WriteLine();

            if (max == 0)
            {
                Console.WriteLine($"No migrations found in the given folder.");
            }
            else
            {
                // If we have migrations, consider the commands.
                var t = -1;
                var doWork = true;
                Console.Write("Instruction: ");
                switch (cmd)
                {
                    case "-reset":
                    case "--reset":
                        Console.WriteLine("Remove all migrations");
                        t = 0;
                        break;
                    case "-latest":
                    case "--latest":
                        Console.WriteLine("Apply new migrations");
                        t = max;
                        break;
                    case "-next":
                    case "--next":
                        Console.WriteLine("Roll forward one migration");
                        if (v < max) t = v + 1;
                        break;
                    case "-back":
                    case "--back":
                        Console.WriteLine("Roll backward one migration");
                        if (v > 1) t = v - 1;
                        break;
                    case "-target":
                    case "--target":
                        if (args.Length < 2)
                            throw new Exception("Missing migration number");
                        if (int.TryParse(args[1], out t) == false)
                            throw new Exception("Expected a valid migration number");
                        Console.WriteLine($"Target specific migration ({t})");
                        break;
                    case "-info":
                    case "--info":
                        Console.WriteLine("Show migration status");
                        doWork = false;
                        break;
                    case "-list":
                    case "--list":
                        Console.WriteLine("List known migrations");
                        Console.WriteLine();
                        foreach (var item in sql)
                        {
                            Console.WriteLine($"{item.Value.Version}:{item.Value.Name}");
                        }
                        doWork = false;
                        break;
                    default:
                        Console.WriteLine("N/A");
                        throw new Exception("Unknown command requested!");
                }

                // Do the commands require actions to occur?
                if (doWork)
                {
                    if (t < 0 || t > max)
                        throw new Exception($"Migration target should be 0 to {max}!");

                    if (v == t)
                    {
                        // Already at required version.
                        Console.WriteLine("No actions needed.");
                    }
                    else
                    {
                        // Required version differs from database version.
                        var tn = "-";
                        if (t > 0)
                            tn = sql
                                .FirstOrDefault(x => x.Value.Version == t)
                                .Value.Name;
                        Console.WriteLine($"Targeting version {t}:{tn}");
                        Console.Write("Press enter/return to continue (or Ctrl+C) ... ");
                        Console.ReadLine();
                        Console.WriteLine();
                        Console.WriteLine("Migrating ...");
                        Console.WriteLine();
                        m.SetNotifier(new ConsoleNotifier());
                        m.SetVersion(t);
                        v = m.GetVersion();
                        Console.WriteLine($"Currently at version {v} of {max}");

                        // Fully rolled back? Remove redundant migrations table.
                        if (v == 0)
                        {
                            Console.WriteLine("Removing `migratable_version table`");
                            p.Execute("DROP TABLE migratable_version");
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine();
            Console.WriteLine("ERROR");
            Console.WriteLine(e.Message);
        }
        finally
        {
            Console.WriteLine();
        }
    }
}
