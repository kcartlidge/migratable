using Migratable.Interfaces;
using System;

namespace Migratable.MySqlProvider
{
    public class Provider : IProvider
    {
        public long GetVersion()
        {
            throw new NotImplementedException();
        }
        public void Execute(long version, string instructions)
        {
            throw new NotImplementedException();
        }
    }
}
