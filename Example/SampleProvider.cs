using Migratable.Interfaces;

namespace Example
{
    public class SampleProvider : IProvider
    {
        private long version = 0;

        public void Execute(string instructions)
        {
            // A real provider would execute the instructions (e.g. write to the database).
        }

        public long GetVersion()
        {
            return this.version;
        }

        public void SetVersion(long versionNumber)
        {
            this.version = versionNumber;
        }
    }
}
