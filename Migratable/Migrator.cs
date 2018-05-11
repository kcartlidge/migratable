using Migratable.Interfaces;
using Migratable.Models;
using System.Collections.Generic;
using System.IO;

namespace Migratable
{
    public class Migrator : IMigrator
    {
        public List<Migration> migrations = new List<Migration>();

        public List<Migration> LoadMigrations(string folderPath)
        {
            migrations = new List<Migration>();
            var folder = new DirectoryInfo(folderPath);
            var subfolders = folder.GetDirectories();
            foreach (var subfolder in subfolders)
            {
                var bits = subfolder.Name.Split(new char[] { ' ' }, 2);
                if (bits.Length == 2 && long.TryParse(bits[0], out long version))
                {
                    var upFile = new FileInfo(Path.Combine(subfolder.FullName, "up.sql"));
                    var downFile = new FileInfo(Path.Combine(subfolder.FullName, "down.sql"));
                    migrations.Add(new Migration
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
    }
}
