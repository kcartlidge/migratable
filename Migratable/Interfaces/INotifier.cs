using Migratable.Models;

namespace Migratable.Interfaces
{
    public enum Direction
    {
        Up, Down
    }

    public interface INotifier
    {
        void Notify(Migration migration, Direction direction);
    }
}
