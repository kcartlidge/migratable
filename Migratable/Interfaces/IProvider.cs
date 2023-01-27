namespace Migratable.Interfaces
{
    /// <summary>
    /// Defines what a specific database implementation needs to implement.
    /// </summary>
    public interface IProvider
    {
        /// <summary>Human description of the target.</summary>
        string Describe();

        /// <summary>Get the current version.</summary>
        int GetVersion();
        /// <summary>Roll to the specified version.</summary>
        void SetVersion(int versionNumber);

        /// <summary>Execute the given instructions.</summary>
        void Execute(string instructions);
    }
}
