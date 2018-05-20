# Migratable

Simple, efficient, and tested .Net Core database migrations supporting multiple database technologies.

## Status

Fully working. ```IProvider``` implementations are easy to create
(there is an example in the ```Example``` project).

Available on *github*:

* [Migratable](https://github.com/kcartlidge/migratable)
* [Migratable.MySqlProvider](https://github.com/kcartlidge/migratable.mysqlprovider)

Available on *nuget*:

* [Migratable](https://www.nuget.org/packages/Migratable)
* [Migratable.MySqlProvider](https://www.nuget.org/packages/Migratable.MySqlProvider)

## Using Migratable

*Migratable* itself is a versioned database migration manager.
In order to do anything, it requires either a custom or a pre-written provider.

There is an ```Example``` project which is simple and self-contained.
Methods available are discoverable and obvious.

``` cs
// Configure.
var provider = new SampleProvider();
var notifier = new SampleNotifier();
var migrator = new Migratable.Migrator(provider);
migrator.SetNotifier(notifier);

// Load from the 'migrations' folder.
migrator.LoadMigrations("./migrations");

// Roll forward from the current version to version 5.
migrator.RollForward(5);
var newCurrentVersion = migrator.GetVersion();
```

The code above passes in the folder ```./migrations```.
That folder should contain something like:

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
The ```down.sql``` file would contain the SQL needed to reverse that action.

You must start at version one and you cannot omit a version in the sequence.
You may also not have duplicate version numbers.

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
