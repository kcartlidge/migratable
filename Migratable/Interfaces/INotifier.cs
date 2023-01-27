using Migratable.Models;

namespace Migratable.Interfaces
{
    /// <summary>The direction a migration step is going.</summary>
    public enum Direction
    {
        Up, Down
    }

    /// <summary>
    /// Defines what a specific notifier needs to implement.
    /// Notifiers are optional 'hooks' to allow your application
    /// to show messages from the migration process.
    /// </summary>
    public interface INotifier
    {
        void Notify(Migration migration, Direction direction);
    }
}
