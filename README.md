# Migratable

Simple and efficient database migrations supporting multiple database technologies.

Available as both a **cross-platform CLI tool** suitable for any ecosystem (eg *DotNet, Go, Node etc*) and a set of **nuget packages** for inclusion in your own DotNet apps (eg to automate migrations).

*Runs on Linux, Mac (Intel/ARM), and Windows. Currently supports PostgreSQL and MySQL.*

## Contents

- [About the cross-platform CLI tool](#about-the-cross-platform-cli-tool)
- [Benefits of both the CLI and the Nuget packages](#benefits-of-both-the-cli-and-the-nuget-packages)
- [What's available?](#whats-available)
- [Requirements](#requirements)
  - [A database connection string in an environment variable](#a-database-connection-string-in-an-environment-variable)
  - [A folder of SQL migration scripts](#a-folder-of-sql-migration-scripts)
- [Using Migratable via the CLI tool](#using-migratable-via-the-cli-tool)
- [Using the Migratable Nuget packages](#using-the-migratable-nuget-packages)
  - [How it works](#how-it-works)
  - [Timeline Display](#timeline-display)
- [Note about MySQL](#note-about-mysql)
- [For developers working on Migratable itself](#for-developers-working-on-migratable-itself)
  - [Running the tests](#running-the-tests)
  - [Creating a new version for Nuget](#creating-a-new-version-for-nuget)
  - [Forcing another project to get the latest from Nuget](#forcing-another-project-to-get-the-latest-from-nuget)
- [MIT Licence](#mit-licence)

---

## About the cross-platform CLI tool

- It's *not dependent on the DotNet platform*
- It fits any ecosystem (eg *Go, Python, Node etc*)
- It's easy to configure, needing only:
  - A connection string in an environment variable
  - A folder with named migrations using up/down SQL scripts

## Benefits of both the CLI and the Nuget packages

- Your database structure can be *version-controlled*
- Roll your database backwards as well as forwards
- Seed/pre-populate, update, or remove data
- Run in *transactions* for atomic up/down
- Uses the [MIT licence](./LICENCE)

## What's available?

Cross-platform releases (*not dependent on DotNet*):

- [Command-line tool](./builds)

Packages available on *Nuget* for DotNet:

- [Migratable](https://www.nuget.org/packages/Migratable)
- [Migratable.PostgresProvider](https://www.nuget.org/packages/Migratable.PostgresProvider)
- [Migratable.MySqlProvider](https://www.nuget.org/packages/Migratable.MySqlProvider)

## Requirements

These requirements are the same whether you are using the command-line CLI tool or the Nuget packages.

You need a suitable database server installed (either MySQL or PostgreSQL). You also need the following:

### A database connection string in an environment variable

You can use any environment variable name you like.
Here's an example for Linux/Mac connecting to PostgreSQL:

``` sh
export MY_CONNSTR="Server=127.0.0.1;Port=5432;Database=my_database;Search Path=my_schema;User Id=my_user;Password=my_password"
```

*(This is an example, not a revealed secret.)*

It's similar for Windows, but you'd replace `export` with `set` instead.  However in Windows it's probably simpler to edit the environment variables from your Control Panel / Settings area.

### A folder of SQL migration scripts

A single folder holds all migration scripts.
Inside is a subfolder per migration, where the name begins with the migration sequence number (optional leading zeros) followed by a description.
Each migration in turn consists of an `up.sql` file and a `down.sql` file.

``` text
/migrations
  /001 Create account table
    down.sql
    up.sql
  /002 Create news table
    down.sql
    up.sql
  /003 Insert sample data
    down.sql
    up.sql
```

[There's an example folder here](./Example/migrations).

You must start at version one and you cannot omit a version in the sequence.  You may also not have duplicate version numbers.

## Using Migratable via the CLI tool

- [Download a release](./builds) and place it somewhere convenient
- Add the connection string into your environment
- Create the folder of SQL migration scripts
- Run the CLI

On a Mac for example you could do:

``` sh
# Create a folder and copy the builds into it.
cd <repository_root>
sudo mkdir -p /usr/local/bin/Migratable
sudo cp -r builds/macos-arm64/* /usr/local/bin/Migratable

# Run it.
/usr/local/bin/Migratable/Migratable.CLI postgres my_connstr ~/my-app/migrations --info
```

In the above example the parameters are:

- `postgres` is the database type (`postgres` or `mysql`)
- `my_connstr` is the name of the environment variable containing your database connection string
- `~/my-app/migrations` is the folder containing all your migrations
- `--info` is the Migratable command to run

You can run without any command arguments to get a summary of usage:

``` text
Usage:
  migratable <db> <env_name> <migrations> <command>


  db          database; either POSTGRES or MYSQL
  env_name    the name of an environment variable
              with a database connection string
  migrations  folder containing migration scripts
              (https://github.com/kcartlidge/migratable)
  command     migration action to perform

Commands:

  info        Show migration status
  list        List known migrations

  reset       Remove all migrations
  latest      Apply new migrations
  next        Roll forward one migration
  back        Roll backward one migration
  target=0    Target specific migration
```

---

If it fails to run on Linux and Mac one of the following may help:

- Run `chmod +x Migratable.CLI` to ensure the CLI tool is executable
- Open it in Finder/Nemo/whatever first to deal with any security unlocking needed

## Using the Migratable Nuget packages

There is an [Example project](./Example) in this solution.  It's totally self-contained as it uses an in-memory provider.
It also shows the use of a `Timeline` to show the known migrations and the current status.

Here's some sample code to show the packages in use. `SampleProvider` is defined in the example project just mentioned. You could also look at the (simple and commented) [code for the CLI tool](./Migratable.CLI).

``` csharp
// Configure.
var provider = new SampleProvider();
var migrator = new Migratable.Migrator(provider);
var migrations = migrator.LoadMigrations("./migrations");

// Show the known migrations and current database position.
var timeline = new Timeline(provider, migrations, 3);
timeline.Show();

// Confirm the connection.
Console.WriteLine(migrator.Describe());
Console.Write("Press enter/return to continue (or Ctrl+C) ... ");
Console.ReadLine();

// Migrate from the current version to version 5.
Console.WriteLine($"Old version: {migrator.GetVersion()}");
migrator.SetVersion(5);
Console.WriteLine($"New version: {migrator.GetVersion()}");
```

The `Describe()` method is designed to give confidence in proceeding. For MySQL/MariaDB it shows the server and database name.

### How it works

There are 2 necessary components, with two optional ones:

- *Migrator* - what your code should interact with to load/perform migrations
- *Provider* - a utility package to support a particular database technology
- *Notifier* - an *optional* class that can be sent progress messages for you to output to the console, log to a file, send via email, or whatever you choose
  - The Notifier 'hook' is there but no delivery mechanism is provided, although the example project shows that a [console notifier](./Example/SampleNotifier.cs) is just one line of code
- *Timeline* - an *optional* class that can show/return a list of migrations with the current migration position

In brief, you follow this process:

- Create a Provider instance for your database
- Create a Migrator and pass in your Provider
- Optionally create a Notifier and pass that to the Migrator
- Ask your Migrator to load your migrations
- Ask your Migrator to perform versioned actions
- Optionally use the Timeline before and/or after applying actions

That final Migrator step will result in your up/down SQL statements being issued as needed to transition from your current database version to your target one.
This is supported by an automatically created/updated `migratable_version` table.

### Timeline Display

The `Timeline` class has options to either write a timeline to the `Console` or to return a `List<string>` containing the equivalent text.
If you're writing to the screen a blank separator line is included before and after; this is not present if you request the text.

When creating a new `Timeline` instance, the `2` is an optional indent for the text to ensure it fits with your own output. You can also provide overrides for the title and the position text.

``` csharp
var timeline = new Timeline(provider, migrations, 2);
...
migrator.SetVersion(2);
timeline.Show();
```

Depending upon the migrations (this is based on one of the test fixtures) it would give something like this:

``` text
  STATUS
  001 Create accounts
  002 Populate accounts
  <-- YOU ARE HERE
  003 Create themes
```

## Note about MySQL

MySQL has a habit of silently committing structural changes (add column, create table etc) mid-transaction.
You should therefore avoid using multiple statements in a single migration if any one of them is structural.
If you do, and one of the other statements fails, the transaction rollback may fail to undo a structural change that has already been applied in the current migration script.

---

## For developers working on Migratable itself

*If you only intend making use of Migratable in your own projects read no further.*

### Running the tests

``` sh
cd Migratable.Tests
dotnet test
```

### Creating a new version for Nuget

The `Migratable/Migratable.csproj` file contains Nuget settings.
Within that file, update the version number then create the Nuget package.
The version exists twice, in `VersionPrefix` and `Version`.

The pack command that follows is done at the *solution* level not the project.

``` sh
cd <solution>
dotnet build
dotnet pack -c Release
```

The `pack` command will mention issues with other projects in the solution; we are not packing them so it's fine to ignore the suggestions (eg a README for the example).
When completed the *Successfully created package* line says where the `.nupkg` file is.

If you are changing the CLI that project contains its own version number.
It also has its own script for creating new cross-platform builds:

``` sh
cd <solution>
cd Migratable.CLI
./build.sh
```

On Windows use `build.bat` rather than `build.sh`.

### Forcing another project to get the latest from Nuget

It sometimes takes a while for a new version to be available after pushing.
You may be able to speed up the process:

``` sh
cd <other-project>
dotnet restore --no-cache
```

---

## MIT Licence

Copyright (c) 2019-2024 K Cartlidge.
See the included [LICENCE file](./LICENCE) for details.
