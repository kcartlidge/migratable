using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Migratable.Interfaces;
using Migratable.Models;
using Moq;
using System;
using System.IO;
using System.Linq;

namespace Migratable.Tests
{
    [TestClass]
    public class MigratorTest
    {
        private IMigrator migrator;
        private Mock<IProvider> provider;
        private Mock<INotifier> notifier;

        [TestInitialize]
        public void Setup()
        {
            provider = new Mock<IProvider>();
            notifier = new Mock<INotifier>();
            migrator = new Migrator(provider.Object);
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
            var result = migrator.LoadMigrations("no fixtures");

            result.Count.Should().Be(0);
        }

        [TestMethod]
        public void LoadMigrations_FolderHasMigrations_LoadsMigrations()
        {
            var result = migrator.LoadMigrations("fixtures");

            result.Count.Should().Be(5);

            result.Values.Should().BeEquivalentTo(new Migration[]
            {
                new Migration
                {
                    Version = 1,
                    Name = "Create account table",
                    Up = "-- UP 1",
                    Down = "",
                },
                new Migration
                {
                    Version = 2,
                    Name = "Add default users",
                    Up = "-- UP 2",
                    Down = "-- DOWN 2",
                },
                new Migration
                {
                    Version = 3,
                    Name = "Remove root user",
                    Up = "",
                    Down = "-- DOWN 3",
                },
                new Migration
                {
                    Version = 4,
                    Name = "Do something else",
                    Up = "-- UP 4",
                    Down = "-- DOWN 4",
                },
                new Migration
                {
                    Version = 5,
                    Name = "One final thing",
                    Up = "-- UP 5",
                    Down = "-- DOWN 5",
                },
            });
        }

        [TestMethod]
        public void LoadMigrations_FolderHasMigrations_IgnoresUnnumberedMigrations()
        {
            var result = migrator.LoadMigrations("fixtures");

            result.Where(x => x.Value.Name == "not-a-migration").Count().Should().Be(0);
        }

        [TestMethod]
        public void LoadMigrations_FolderHasInvalidMigrations_IgnoresInvalidMigrations()
        {
            var result = migrator.LoadMigrations("fixtures");

            result[1].Down.Should().BeEmpty();
            result[1].Up.Should().NotBeEmpty();
            result[3].Down.Should().NotBeEmpty();
            result[3].Up.Should().BeEmpty();
        }

        [TestMethod]
        public void LoadMigrations_CalledRepeatedly_DoesNotDuplicate()
        {
            migrator.LoadMigrations("fixtures");
            migrator.LoadMigrations("fixtures");
            var result = migrator.LoadMigrations("fixtures");

            result.Count.Should().Be(5);
        }

        [TestMethod]
        public void LoadMigrations_WithDuplicateVersions_ThrowsException()
        {
            Action action = () => migrator.LoadMigrations("fixtures-with-duplicates");

            action.Should().Throw<Exception>();
        }

        [TestMethod]
        public void LoadMigrations_FolderHasMissingMigrations_ThrowsException()
        {
            Action action = () => migrator.LoadMigrations("fixtures-with-version-gap");

            action.Should().Throw<Exception>();
        }

        // Versioning.

        [TestMethod]
        public void GetVersion_ReturnsVersionFromProvider()
        {
            var version = 1;
            provider.Setup(x => x.GetVersion()).Returns(version);

            int result = migrator.GetVersion();

            result.Should().Be(version);
        }

        [TestMethod]
        public void SetVersion_WithUnknownVersion_ThrowsException()
        {
            var targetVersion = 99;
            var migrations = migrator.LoadMigrations("fixtures");

            Action action = () => migrator.SetVersion(targetVersion);

            action.Should().Throw<Exception>();
        }

        [TestMethod]
        public void SetVersion_WhenAtHigherVersion_RollsBackward()
        {
            var version = 2;
            var targetVersion = 1;
            var migrations = migrator.LoadMigrations("fixtures");
            provider.Setup(x => x.GetVersion()).Returns(version);

            migrator.SetVersion(targetVersion);

            provider.Verify(x => x.Execute(migrations[2].Down), Times.Once);   // has UP
            provider.Verify(x => x.SetVersion(1), Times.Once);
            provider.Verify(x => x.Execute(migrations[1].Down), Times.Never);  // target version
            foreach (var migration in migrations)
            {
                // UP should never be actioned.
                provider.Verify(x => x.Execute(migration.Value.Up), Times.Never);
            }
        }

        [TestMethod]
        public void SetVersion_WhenAtSameVersion_DoesNothing()
        {
            var version = 1;
            migrator.LoadMigrations("fixtures");
            provider.Setup(x => x.GetVersion()).Returns(version);

            migrator.SetVersion(version);

            provider.Verify(x => x.Execute(It.IsAny<string>()), Times.Never);
            provider.Verify(x => x.SetVersion(It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public void SetVersion_WhenAtLesserVersion_RollsForward()
        {
            var version = 1;
            var targetVersion = 2;
            var migrations = migrator.LoadMigrations("fixtures");
            provider.Setup(x => x.GetVersion()).Returns(version);

            migrator.SetVersion(targetVersion);

            provider.Verify(x => x.Execute(migrations[1].Up), Times.Never);  // starting version
            provider.Verify(x => x.SetVersion(1), Times.Never);
            provider.Verify(x => x.Execute(migrations[2].Up), Times.Once);   // target version
            provider.Verify(x => x.SetVersion(2), Times.Once);
            foreach (var migration in migrations)
            {
                // DOWN should never be actioned.
                provider.Verify(x => x.Execute(migration.Value.Down), Times.Never);
            }
        }

        // Rolling forward.

        [TestMethod]
        public void RollForward_WhenAtHigherVersion_DoesNothing()
        {
            var version = 2;
            var targetVersion = 1;
            provider.Setup(x => x.GetVersion()).Returns(version);

            migrator.RollForward(targetVersion);

            provider.Verify(x => x.Execute(It.IsAny<string>()), Times.Never);
            provider.Verify(x => x.SetVersion(It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public void RollForward_WhenAtSameVersion_DoesNothing()
        {
            var version = 1;
            provider.Setup(x => x.GetVersion()).Returns(version);

            migrator.RollForward(version);

            provider.Verify(x => x.Execute(It.IsAny<string>()), Times.Never);
            provider.Verify(x => x.SetVersion(It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public void RollForward_WhenAtLesserVersion_RollsForwardThenStops()
        {
            var version = 1;
            var targetVersion = 4;
            var migrations = migrator.LoadMigrations("fixtures");
            provider.Setup(x => x.GetVersion()).Returns(version);

            migrator.RollForward(targetVersion);

            provider.Verify(x => x.Execute(migrations[1].Up), Times.Never);  // starting version
            provider.Verify(x => x.SetVersion(1), Times.Never);
            provider.Verify(x => x.Execute(migrations[2].Up), Times.Once);   // has UP
            provider.Verify(x => x.SetVersion(2), Times.Once);
            provider.Verify(x => x.Execute(migrations[3].Up), Times.Never);  // has empty UP
            provider.Verify(x => x.SetVersion(3), Times.Never);
            provider.Verify(x => x.Execute(migrations[4].Up), Times.Once);   // has UP
            provider.Verify(x => x.SetVersion(4), Times.Once);
            provider.Verify(x => x.Execute(migrations[5].Up), Times.Never);  // beyond target version
            provider.Verify(x => x.SetVersion(5), Times.Never);
            foreach (var migration in migrations)
            {
                // DOWN should never be actioned.
                provider.Verify(x => x.Execute(migration.Value.Down), Times.Never);
            }
        }

        [TestMethod]
        public void RollForward_WhenOneVersionFails_IgnoresRemainder()
        {
            var version = 2;
            var targetVersion = 5;
            var migrations = migrator.LoadMigrations("fixtures");
            provider.Setup(x => x.GetVersion()).Returns(version);
            provider.Setup(x => x.Execute(migrations[4].Up)).Throws<Exception>();

            Action action = () => migrator.RollForward(targetVersion);

            action.Should().Throw<Exception>();
            provider.Verify(x => x.Execute(migrations[4].Up), Times.Once);
            provider.Verify(x => x.SetVersion(4), Times.Never);
        }

        // Rolling backward.

        [TestMethod]
        public void RollBackward_WhenAtLowerVersion_DoesNothing()
        {
            var version = 1;
            var targetVersion = 2;
            provider.Setup(x => x.GetVersion()).Returns(version);

            migrator.RollBackward(targetVersion);

            provider.Verify(x => x.Execute(It.IsAny<string>()), Times.Never);
            provider.Verify(x => x.SetVersion(It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public void RollBackward_WhenAtSameVersion_DoesNothing()
        {
            var version = 1;
            provider.Setup(x => x.GetVersion()).Returns(version);

            migrator.RollBackward(version);

            provider.Verify(x => x.Execute(It.IsAny<string>()), Times.Never);
            provider.Verify(x => x.SetVersion(It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public void RollBackward_WhenAtHigherVersion_RollsBackwardThenStops()
        {
            var version = 5;
            var targetVersion = 1;
            var migrations = migrator.LoadMigrations("fixtures");
            provider.Setup(x => x.GetVersion()).Returns(version);

            migrator.RollBackward(targetVersion);

            provider.Verify(x => x.Execute(migrations[5].Down), Times.Once);   // starting version
            provider.Verify(x => x.SetVersion(5), Times.Never);
            provider.Verify(x => x.Execute(migrations[4].Down), Times.Once);   // has DOWN
            provider.Verify(x => x.SetVersion(4), Times.Once);
            provider.Verify(x => x.Execute(migrations[3].Down), Times.Once);   // has DOWN
            provider.Verify(x => x.SetVersion(3), Times.Once);
            provider.Verify(x => x.Execute(migrations[2].Down), Times.Once);   // has DOWN
            provider.Verify(x => x.SetVersion(2), Times.Once);
            provider.Verify(x => x.Execute(migrations[1].Down), Times.Never);  // target version
            provider.Verify(x => x.SetVersion(1), Times.Once);
            foreach (var migration in migrations)
            {
                // UP should never be actioned.
                provider.Verify(x => x.Execute(migration.Value.Up), Times.Never);
            }
        }

        [TestMethod]
        public void RollBackward_WhenOneVersionFails_IgnoresRemainder()
        {
            var version = 5;
            var targetVersion = 2;
            var migrations = migrator.LoadMigrations("fixtures");
            provider.Setup(x => x.GetVersion()).Returns(version);
            provider.Setup(x => x.Execute(migrations[4].Down)).Throws<Exception>();

            Action action = () => migrator.RollBackward(targetVersion);

            action.Should().Throw<Exception>();
            provider.Verify(x => x.Execute(migrations[4].Down), Times.Once);
            provider.Verify(x => x.SetVersion(3), Times.Never);
        }

        // Notifier.

        [TestMethod]
        public void RollForward_WhenMigrationOccurs_IssuesNotification()
        {
            var version = 1;
            var targetVersion = 2;
            var migrations = migrator.LoadMigrations("fixtures");
            provider.Setup(x => x.GetVersion()).Returns(version);
            migrator.SetNotifier(notifier.Object);

            migrator.RollForward(targetVersion);

            notifier.Verify(x => x.Notify(migrations[2], Direction.Up), Times.Once);
            notifier.Verify(x => x.Notify(migrations[2], Direction.Down), Times.Never);
        }

        [TestMethod]
        public void RollBackward_WhenMigrationOccurs_IssuesNotification()
        {
            var version = 2;
            var targetVersion = 1;
            var migrations = migrator.LoadMigrations("fixtures");
            provider.Setup(x => x.GetVersion()).Returns(version);
            migrator.SetNotifier(notifier.Object);

            migrator.RollBackward(targetVersion);

            notifier.Verify(x => x.Notify(migrations[2], Direction.Down), Times.Once);
            notifier.Verify(x => x.Notify(migrations[2], Direction.Up), Times.Never);
        }
    }
}
