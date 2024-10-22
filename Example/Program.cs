using Migratable;
using System;

namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine();
            Console.WriteLine("MIGRATABLE EXAMPLE");
            Console.WriteLine();
            try
            {
                // Configure.
                var provider = new SampleProvider();
                var notifier = new SampleNotifier();
                var migrator = new Migrator(provider);
                migrator.SetNotifier(notifier);

                // Introduction.
                Console.WriteLine(migrator.Describe());
                Console.WriteLine();

                // Load from the 'migrations' folder.
                Console.WriteLine();
                Console.WriteLine("LOADING MIGRATIONS");
                var migrations = migrator.LoadMigrations("./migrations");

                // Load from the 'migrations' folder.
                Console.WriteLine("CURRENT TIMELINE");
                var timeline = new Timeline(provider, migrations, 3);
                timeline.Show();

                // There are only 3 so this will go forward to there.
                Console.WriteLine("FORWARD TO 5");
                migrator.RollForward(5);
                timeline.Show();

                Console.WriteLine("BACKWARD TO 2");
                migrator.RollBackward(2);
                timeline.Show();

                Console.WriteLine("BACKWARD TO 0");
                migrator.RollBackward(0);
                timeline.Show();

                Console.WriteLine("FORWARD TO 2");
                migrator.RollForward(2);
                timeline.Show();
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("ERROR " + ex.Message);
            }

            Console.WriteLine();
            Console.Write("Press enter/return ... ");
            Console.ReadLine();
        }
    }
}
