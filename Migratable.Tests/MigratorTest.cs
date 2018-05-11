using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Migratable.Models;
using System;
using System.IO;
using System.Linq;

namespace Migratable.Tests
{
    [TestClass]
    public class MigratorTest
    {
        Migrator migrator;

        [TestInitialize]
        public void Setup()
        {
            migrator = new Migrator();
        }

        // Loading.

        [TestMethod]
        public void LoadMigrations_FolderNotFound_ThrowsDirectoryNotFoundException()
        {
            Action action = () => migrator.LoadMigrations(".. not found ..");

            action.Should().ThrowExactly<DirectoryNotFoundException>();
        }

        [TestMethod]
        public void LoadMigrations_EmptyFolderFound_HasNoMigrations()
        {
            migrator.LoadMigrations("no fixtures");

            migrator.migrations.Count.Should().Be(0);
        }

        [TestMethod]
        public void LoadMigrations_FolderHasMigrations_LoadsMigrations()
        {
            migrator.LoadMigrations("fixtures");

            migrator.migrations.Count.Should().Be(3);
            migrator.migrations.First().Should().BeEquivalentTo(new Migration
            {
                Version = 1,
                Name = "001 Create account table",
                Up = "-- UP",
                Down = "",
            });
            migrator.migrations.Skip(1).First().Should().BeEquivalentTo(new Migration
            {
                Version = 2,
                Name = "002 Add default users",
                Up = "-- UP",
                Down = "-- DOWN",
            });
            migrator.migrations.Skip(2).First().Should().BeEquivalentTo(new Migration
            {
                Version = 3,
                Name = "003 Remove root user",
                Up = "",
                Down = "-- DOWN",
            });
        }

        [TestMethod]
        public void LoadMigrations_FolderHasMigrations_IgnoresUnnumberedMigrations()
        {
            migrator.LoadMigrations("fixtures");

            migrator.migrations.Where(x => x.Name == "not-a-migration").Count().Should().Be(0);
        }

        [TestMethod]
        public void LoadMigrations_FolderInvalidMigrations_IgnoresInvalidMigrations()
        {
            migrator.LoadMigrations("fixtures");

            var m3 = migrator.migrations.FirstOrDefault(x => x.Name == "003 Remove root user");
            m3.Should().NotBeNull();
            m3.Up.Should().BeEmpty();
        }
    }
}
