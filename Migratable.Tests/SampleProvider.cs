using Migratable.Interfaces;

namespace Migratable.Tests
{
    public class SampleProvider : IProvider
    {
        private int version = 0;

        public string Describe()
        {
            return $"Sample in-memory provider that just moves the version (which starts at {version}).";
        }

        public void Execute(string instructions)
        {
            // A real provider would execute the instructions (e.g. write to the database).
        }

        public int GetVersion()
        {
            return version;
        }

        public void SetVersion(int versionNumber)
        {
            version = versionNumber;
        }
    }
}
