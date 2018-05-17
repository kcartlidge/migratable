namespace Migratable.Interfaces
{
    public interface IProvider
    {
        /// <summary>Get the current version.</summary>
        long GetVersion();
        /// <summary>Roll to the specified version.</summary>
        void SetVersion(long versionNumber);

        /// <summary>Execute the given instructions.</summary>
        void Execute(string instructions);
    }
}
