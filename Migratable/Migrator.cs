using Migratable.Models;
using Migratable.Interfaces;
using System.Collections.Generic;
using System.IO;

namespace Migratable
{
    public class Migrator : IMigrator
    {
        public Migrator(IProvider provider)
        {
            this.provider = provider;
        }

        private SortedList<long, Migration> migrations = new SortedList<long, Migration>();
        private readonly IProvider provider;

        public SortedList<long, Migration> LoadMigrations(string folderPath)
        {
            migrations = new SortedList<long, Migration>();
            var folder = new DirectoryInfo(folderPath);
            var subfolders = folder.GetDirectories();
            foreach (var subfolder in subfolders)
            {
                var bits = subfolder.Name.Split(new char[] { ' ' }, 2);
                if (bits.Length == 2 && long.TryParse(bits[0], out long version))
                {
                    var upFile = new FileInfo(Path.Combine(subfolder.FullName, "up.sql"));
                    var downFile = new FileInfo(Path.Combine(subfolder.FullName, "down.sql"));
                    migrations.Add(version, new Migration
                    {
                        Version = version,
                        Name = subfolder.Name,
                        Up = upFile.Exists ? File.ReadAllText(upFile.FullName).Trim() : string.Empty,
                        Down = downFile.Exists ? File.ReadAllText(downFile.FullName).Trim() : string.Empty,
                    });
                }
            }
            return migrations;
        }

        public long GetVersion()
        {
            return provider.GetVersion();
        }
        public void SetVersion(long targetVersion)
        {
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

        public void RollForward(long targetVersion)
        {
            var currentVersion = GetVersion();
            while (currentVersion < targetVersion)
            {
                currentVersion += 1;
                if (migrations.ContainsKey(currentVersion))
                {
                    var up = migrations[currentVersion].Up;
                    if (!string.IsNullOrWhiteSpace(up))
                    {
                        provider.Execute(currentVersion, up);
                    }
                }
            }
        }
        public void RollBackward(long targetVersion)
        {
            var currentVersion = GetVersion();
            while (currentVersion > targetVersion)
            {
                if (migrations.ContainsKey(currentVersion))
                {
                    var down = migrations[currentVersion].Down;
                    if (!string.IsNullOrWhiteSpace(down))
                    {
                        provider.Execute(currentVersion, down);
                    }
                }
                currentVersion -= 1;
            }
        }
    }
}
