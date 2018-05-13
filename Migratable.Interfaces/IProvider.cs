namespace Migratable.Interfaces
{
    public interface IProvider
    {
        long GetVersion();

        void Execute(long version, string instructions);
    }
}
