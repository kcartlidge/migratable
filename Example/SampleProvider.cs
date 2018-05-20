using Migratable.Interfaces;

namespace Example
{
    public class SampleProvider : IProvider
    {
        private int version = 0;

        public void Execute(string instructions)
        {
            // A real provider would execute the instructions (e.g. write to the database).
        }

        public int GetVersion()
        {
            return this.version;
        }

        public void SetVersion(int versionNumber)
        {
            this.version = versionNumber;
        }
    }
}
