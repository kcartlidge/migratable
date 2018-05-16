using Migratable.Models;
using System.Collections.Generic;

namespace Migratable.Interfaces
{
    public interface IMigrator
    {
        SortedList<long, Migration> LoadMigrations(string folderPath);

        long GetVersion();
        void SetVersion(long targetVersion);

        void RollForward(long targetVersion);
        void RollBackward(long targetVersion);
    }
}
