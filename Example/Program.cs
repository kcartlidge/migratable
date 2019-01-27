using System;

namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Configure.
                var provider = new SampleProvider();
                var notifier = new SampleNotifier();
                var migrator = new Migratable.Migrator(provider);
                migrator.SetNotifier(notifier);

                // Introduction.
                Console.WriteLine(migrator.Describe());
                Console.WriteLine();

                // Load from the 'migrations' folder.
                Console.WriteLine("Loading migrations");
                migrator.LoadMigrations("./migrations");

                // There are only 3 so this will go forward to there.
                Console.WriteLine("Rolling forward to 5");
                migrator.RollForward(5);
                Console.WriteLine("Now at {0}", migrator.GetVersion());

                Console.WriteLine("Rolling backward to 2");
                migrator.RollBackward(2);
                Console.WriteLine("Now at {0}", migrator.GetVersion());

                Console.WriteLine("Rolling backward to 0");
                migrator.RollBackward(0);
                Console.WriteLine("Now at {0}", migrator.GetVersion());

                Console.WriteLine("Rolling forward to 2");
                migrator.RollForward(2);
                Console.WriteLine("Now at {0}", migrator.GetVersion());
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
