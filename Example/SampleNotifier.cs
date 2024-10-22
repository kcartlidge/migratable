using System;
using Migratable.Interfaces;
using Migratable.Models;

namespace Example
{
    public class SampleNotifier : INotifier
    {
        public void Notify(Migration migration, Direction direction)
        {
            Console.WriteLine("   {0}.{1}  {2}",
                migration.Version,
                direction.ToString().ToUpperInvariant(),
                migration.Name);
        }
    }
}
