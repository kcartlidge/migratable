using Migratable.Models;
using Migratable.Interfaces;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System;

namespace Migratable
{
    public class Migrator : IMigrator
    {
        public Migrator(IProvider provider)
        {
            this.provider = provider;
        }

        private SortedList<int, Migration> migrations = new SortedList<int, Migration>();
        private INotifier notifier;
        private readonly IProvider provider;

        public void SetNotifier(INotifier notifier)
        {
            this.notifier = notifier;
        }

        public SortedList<int, Migration> LoadMigrations(string folderPath)
        {
            migrations = new SortedList<int, Migration>();
            var folder = new DirectoryInfo(folderPath);
            var subfolders = folder.GetDirectories();
            int lastVersion = 0;
            var sortedFolders = new SortedList<string, DirectoryInfo>();
            foreach (var subfolder in subfolders)
            {
                sortedFolders.Add(subfolder.Name, subfolder);
            }

            var splitBy = new char[] { ' ' };
            foreach (var subfolder in sortedFolders.Values)
            {
                var bits = subfolder.Name.Split(splitBy, 2, StringSplitOptions.RemoveEmptyEntries);
                if (bits.Length == 2 && int.TryParse(bits[0], out int version))
                {
                    if (lastVersion > 0 && lastVersion != (version - 1))
                    {
                        var err = string.Format("Version goes from {0} to {1}", lastVersion, version);
                        throw new Exception(err);
                    }
                    lastVersion = version;
                    var upFile = new FileInfo(Path.Combine(subfolder.FullName, "up.sql"));
                    var downFile = new FileInfo(Path.Combine(subfolder.FullName, "down.sql"));
                    migrations.Add(version, new Migration
                    {
                        Version = version,
                        Name = bits[1],
                        Up = upFile.Exists ? File.ReadAllText(upFile.FullName).Trim() : string.Empty,
                        Down = downFile.Exists ? File.ReadAllText(downFile.FullName).Trim() : string.Empty,
                    });
                }
            }
            return migrations;
        }

        public int GetVersion()
        {
            return provider.GetVersion();
        }

        public void SetVersion(int targetVersion)
        {
            if (targetVersion > 0 && !migrations.ContainsKey(targetVersion))
            {
                throw new Exception(string.Format("Unknown version {0}", targetVersion));
            }
            var currentVersion = GetVersion();
            if (currentVersion > targetVersion)
            {
                RollBackward(targetVersion);
            }
            else if (currentVersion < targetVersion)
            {
                RollForward(targetVersion);
            }
        }

        public void RollForward(int targetVersion)
        {
            var currentVersion = GetVersion();
            while (currentVersion < targetVersion)
            {
                currentVersion += 1;
                if (migrations.ContainsKey(currentVersion))
                {
                    var migration = migrations[currentVersion];
                    var up = migration.Up;
                    if (!string.IsNullOrWhiteSpace(up))
                    {
                        Notify(migration, Direction.Up);
                        provider.Execute(up);
                        provider.SetVersion(currentVersion);
                    }
                }
            }
        }

        public void RollBackward(int targetVersion)
        {
            var currentVersion = GetVersion();
            while (currentVersion > targetVersion)
            {
                if (migrations.ContainsKey(currentVersion))
                {
                    var migration = migrations[currentVersion];
                    var down = migration.Down;
                    if (!string.IsNullOrWhiteSpace(down))
                    {
                        Notify(migration, Direction.Down);
                        provider.Execute(down);
                        provider.SetVersion(currentVersion - 1);
                    }
                }
                currentVersion -= 1;
            }
        }

        private void Notify(Migration migration, Direction direction)
        {
            if (notifier != null)
            {
                notifier.Notify(migration, direction);
            }
        }
    }
}
