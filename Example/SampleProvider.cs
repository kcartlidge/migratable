using Migratable.Interfaces;

namespace Example
{
    public class SampleProvider : IProvider
    {
        private int version = 0;

        public string Describe()
        {
            return $"Sample provider that just moves the version (currently {version}).";
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
