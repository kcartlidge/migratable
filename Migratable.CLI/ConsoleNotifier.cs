using Migratable.Interfaces;
using Migratable.Models;

namespace Migratable.CLI
{
    internal class ConsoleNotifier : INotifier
    {
        public void Notify(Migration migration, Direction direction)
        {
            Console.WriteLine($"{direction.ToString().ToUpper()} {migration.Version}:{migration.Name}");
        }
    }
}

