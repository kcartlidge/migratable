# Migratable

Simple, efficient, and tested .Net Core database migrations supporting multiple database technologies.

## Status

Tested and working, but you would need to create your own ```IProvider``` implementation.
There is an example one in the ```Example``` project, and the methods are simple.
The MySQL/MariaDB provider is in progress.

## Using Migratable

*Migratable* itself is a versioned database migration manager.
In order to do anything, it requires either a custom or a pre-written provider.

There is an ```Example``` project which is simple and self-contained.
Full documentation will be available when the first provider is complete.

``` cs
using System;
using Migratable.Interfaces;
using Migratable.Models;

namespace test_migratable
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var provider = new DummyProvider();
                var notifier = new Notifier();
                var migrator = new Migratable.Migrator(provider);
                migrator.SetNotifier(notifier);
                migrator.LoadMigrations("./migrations");

                Console.WriteLine("Rolling forward to 5");
                migrator.RollForward(5);
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
        }
    }

    public class Notifier : INotifier
    {
        public void Notify(Migration migration, Direction direction)
        {
            Console.WriteLine("   {0}.{1}  {2}", migration.Version, direction, migration.Name);
        }
    }

    public class DummyProvider : IProvider
    {
        private long version = 0;

        public void Execute(string instructions)
        {
            // Real providers would execute the (SQL) instructions.
        }

        public long GetVersion()
        {
            return this.version;
        }

        public void SetVersion(long versionNumber)
        {
            this.version = versionNumber;
        }
    }
}
```

The code above passes in the folder ```./migrations```.
Given that, the runtime folder should contain something like:

```
\migrations
    \001 Create accounts
        up.sql
        down.sql
    \002 Populate accounts
        up.sql
        down.sql
```

In each case above, the subfolder name is preceeded by the version sequence.
The ```up.sql``` file would contain the SQL needed to "Create accounts".

---

## For developers working on Migratable itself

If you only intend making use of *Migratable* in your own projects read no further.

### Generating a build and running the tests

There is no need to generate a build as *Migratable* is a class library not an application.
If you run the tests, a build is generated anyway automatically:

``` sh
cd Migratable.Tests
dotnet test
```

### Creating a new version for Nuget

The ```Migratable/Migratable.csproj``` file contains Nuget settings.
Within that file, update the version number then create the Nuget package:

``` sh
cd Migratable
dotnet build
dotnet pack
```

### Forcing another project to get the latest from Nuget

It sometimes takes a while for a new version to be available after pushing.
You may be able to speed up the process:

``` sh
cd <other-project>
dotnet restore --no-cache
```

---

## MIT Licence

Copyright (c) 2018 K Cartlidge

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
