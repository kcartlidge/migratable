# Migratable

Simple, efficient, and tested DotNet Core database migrations supporting multiple database technologies.

* Create a completely new database by running all your migrations in one go
* Easily bring an existing database structure up to date
* Roll your database structure backwards as well as forwards
* Use it to pre-populate, amend, or remove data
* Your database structure is version-controlled
* Migrations are defined as up and down SQL files in a folder

## Status

Available on *nuget*:

* Migratable - https://www.nuget.org/packages/Migratable
* Migratable.MySqlProvider - https://www.nuget.org/packages/Migratable.MySqlProvider

## Using Migratable

*Migratable* is a versioned database migration manager.
In order to do anything, it requires a *provider* for your chosen database system.
**MySQL/MariaDB** is already available.
Implementing your own is straightforward, being just a single interface.

There is also an ```Example``` project which is totally self-contained as it uses an in-memory provider.

### Sample usage

``` cs
// Configure.
var provider = new SampleProvider();
var migrator = new Migratable.Migrator(provider);
migrator.LoadMigrations("./migrations");

// Confirm the connection.
Console.WriteLine(migrator.Describe());
Console.Write("Press enter/return to continue (or Ctrl+C) ... ");
Console.ReadLine();

// Migrate from the current version to version 5.
Console.WriteLine($"Old version: {migrator.GetVersion()}");
migrator.SetVersion(5);
Console.WriteLine($"New version: {migrator.GetVersion()}");
```

The ```Describe()``` method is designed to give confidence in proceeding.
For MySQL/MariaDB, for example, it shows the server and database name.
The code above passes in the folder ```./migrations``` to ```LoadMigrations```.
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

The folder name starts with the version sequence and is followed by the description.
Inside each folder, the ```up.sql``` file would contain the SQL needed to progress to that version.
The ```down.sql``` file would contain the SQL needed to drop down from this version to the one below.

You must start at version one and you cannot omit a version in the sequence.
You may also not have duplicate version numbers.

## How it works

There are either 2 or 3 components:

* Migrator - what your code should interact with to load/perform migrations
* Provider - a utility package to support a particular database technology
* Notifier - an optional class that can be sent progress messages

You follow this process:

* Create a Provider instance for your database
* Create a Migrator and pass in your Provider
* Optionally create a Notifier and pass that to the Migrator
* Ask your Migrator to load your migrations
* Ask your Migrator to manage your current version

That final stage will result in your up/down SQL statements being issued as needed to transition from your current database version to your target one.

By default, this is supported by an automatically created/updated ```MigratableVersion``` table.
It does, however, depend on the particular Provider.

---

## For developers working on Migratable itself

If you only intend making use of *Migratable* in your own projects read no further.

### Running the tests

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

Copyright (c) 2019 K Cartlidge.
See the included ```LICENCE``` file for details.
