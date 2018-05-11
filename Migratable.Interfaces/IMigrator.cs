using Migratable.Models;
using System.Collections.Generic;

namespace Migratable.Interfaces
{
    public interface IMigrator
    {
        List<Migration> LoadMigrations(string folderPath);
    }
}
