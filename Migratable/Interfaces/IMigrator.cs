using Migratable.Models;
using System.Collections.Generic;

namespace Migratable.Interfaces
{
    public interface IMigrator
    {
        /// <summary>Loads all the migrations in the given folder path.</summary>
        SortedList<long, Migration> LoadMigrations(string folderPath);

        /// <summary>Register for version movement notifications.</summary>
        void SetNotifier(INotifier notifier);

        /// <summary>Get the current version.</summary>
        long GetVersion();
        /// <summary>Roll to the specified version.</summary>
        void SetVersion(long targetVersion);

        /// <summary>Specifically roll forward to the given version.</summary>
        void RollForward(long targetVersion);
        /// <summary>Specifically roll backward to the given version.</summary>
        void RollBackward(long targetVersion);
    }
}
