using Migratable.Models;
using System.Collections.Generic;

namespace Migratable.Interfaces
{
    /// <summary>Defines the migrator implementation.</summary>
    public interface IMigrator
    {
        /// <summary>Human description of the target.</summary>
        string Describe();

        /// <summary>Loads all the migrations in the given folder path.</summary>
        SortedList<int, Migration> LoadMigrations(string folderPath);

        /// <summary>Register for version movement notifications.</summary>
        void SetNotifier(INotifier notifier);

        /// <summary>Get the current version.</summary>
        int GetVersion();
        /// <summary>Roll to the specified version.</summary>
        void SetVersion(int targetVersion);

        /// <summary>Specifically roll forward to the given version.</summary>
        void RollForward(int targetVersion);
        /// <summary>Specifically roll backward to the given version.</summary>
        void RollBackward(int targetVersion);
    }
}
