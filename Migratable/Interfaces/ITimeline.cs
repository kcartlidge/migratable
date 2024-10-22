using System.Collections.Generic;

namespace Migratable.Interfaces
{
    /// <summary>Handles the timeline of migrations and database.</summary>
    public interface ITimeline
    {
        /// <summary>
        /// Shows the migrations and the current database position.
        /// </summary>
        void Show();

        /// <summary>
        /// Returns the migrations and the current database position.
        /// </summary>
        List<string> Get();
    }
}